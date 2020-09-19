using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MenuRes = WebApi.Models.Response.Web.Menu;

namespace WebApi.Controllers.Web
{
    public class MenuController : ApiController
    {
        private readonly DataBase.DB db = new DataBase.DB();
        /// <summary>
        /// 懒加载获取用户角色菜单
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult LazyMenu(Guid pId)
        {
            if(pId == Guid.Empty)
            {
                return Json(new { status = "fail", msg = "参数错误" });
            }
            var menu = db.Menu.Find(pId);
            if (menu.Status == Models.Config.Status.deleted)
            {
                return Json(new { status = "fail", msg = "查询为空" });
            }
            Guid userId = Guid.Parse(HttpContext.Current.Request.Headers["sessionId"]);
            //根据用户角色查找所有的menu
            var menuIds = (from mr in db.MemRole
                         where mr.Member == userId
                         join rm in db.RoleMenu
                         on mr.Role equals rm.Role
                         select rm.Menu).ToList();
            if(menuIds.Count() == 0)
            {
                return Json(new { status = "fail", msg = "查询为空" });
            }
            //判断所查menu是否存在
            bool isOwned = menuIds.Contains(pId);
            if (!isOwned)
            {
                return Json(new { status = "fail", msg = "权限不足" });
            }
           
            var content = from m in db.Menu
                          where m.PId == pId && menuIds.Contains(m.Id) && m.Status != Models.Config.Status.deleted
                          orderby m.OrderNum
                          select new MenuRes.Menu
                          {
                              Id = m.Id,
                              PId = m.PId,
                              Name = m.Name,
                              Controller = m.Controller,
                              Action = m.Action,
                              Type = m.Type,
                              Icon = m.Icon,
                              OpenNewPage = m.OpenNewPage,
                              OrderNum = m.OrderNum,
                              Status = m.Status,
                              //判断menu中是否有子节点
                              HasChildren = db.Menu.FirstOrDefault(mc => mc.PId == m.Id && menuIds.Contains(mc.Id) && mc.Status != Models.Config.Status.deleted) != null,
                              //RoleCount = 0,
                              //OrgCount = 0,
                              //MemberCount = 0
                          };
            return Json(new { status = "success", msg = "获取成功", content });
        }

        /// <summary>
        /// 获取用户角色菜单树
        /// 可用于elemet ui el-tree等树形结构
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult MenuTreeMR()
        {
            Guid userId = Guid.Parse(HttpContext.Current.Request.Headers["sessionId"]);
            //根据用户角色查找所有的menu
            var menuIds = (from mr in db.MemRole
                           where mr.Member == userId
                           join rm in db.RoleMenu
                           on mr.Role equals rm.Role
                           select rm.Menu).ToList();
            if (menuIds.Count() == 0)
            {
                return Json(new { status = "fail", msg = "查询为空" });
            }
            var menus = (from m in db.Menu
                          where menuIds.Contains(m.Id) && m.Status == Models.Config.Status.normal
                         select new MenuRes.Menu
                          {
                              Id = m.Id,
                              PId = m.PId,
                              Name = m.Name,
                              Controller = m.Controller,
                              Action = m.Action,
                              Icon = m.Icon,
                              OrderNum = m.OrderNum
                          }).ToList();
            var menuIdArr = menus.Select(m => m.Id).ToList();

            List<MenuRes.MenuTree> menuTrees = new List<MenuRes.MenuTree>();
            foreach(var menu in menus)
            {
                if (!menuIdArr.Contains(menu.PId))
                {
                    //以分支节点，创建菜单树
                    var menusTemp = new List<MenuRes.Menu>();
                    menusTemp.AddRange(menus);
                    menuTrees.Add(MenuTreeHelper(menu.Id, menusTemp));
                }

            }
            return Json(new { status = "success", msg = "获取成功", content = menuTrees });
        }

        /// <summary>
        /// 获取用户部门菜单树
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult MenuTreeMO()
        {
            Guid userId = Guid.Parse(HttpContext.Current.Request.Headers["sessionId"]);
            //根据用户部门查找所有的menu
            var menuIds = (from mo in db.MemOrg
                           where mo.Member == userId
                           join om in db.OrgMenu
                           on mo.Org equals om.Org
                           select om.Menu).ToList();
            if(menuIds.Count() == 0)
            {
                return Json(new { status = "fail", msg = "查询为空" });
            }
            var menus = (from m in db.Menu
                         where  m.Status == Models.Config.Status.normal && (menuIds.Contains(m.Id) || menuIds.Contains(m.PId))
                         select new MenuRes.Menu
                         {
                             Id = m.Id,
                             PId = m.PId,
                             Name = m.Name,
                             Controller = m.Controller,
                             Action = m.Action,
                             Icon = m.Icon,
                             OrderNum = m.OrderNum
                         }).ToList();
            var menuIdArr = menus.Select(m => m.Id).ToList();

            List<MenuRes.MenuTree> menuTrees = new List<MenuRes.MenuTree>();
            foreach (var menu in menus)
            {
                if (!menuIdArr.Contains(menu.PId))
                {
                    //以分支节点，创建菜单树
                    var menusTemp = new List<MenuRes.Menu>();
                    menusTemp.AddRange(menus);
                    menuTrees.Add(MenuTreeHelper(menu.Id, menusTemp));
                }

            }
            return Json(new { status = "success", msg = "获取成功", content = menuTrees });
        }

        /// <summary>
        /// 获取用户角色权限路由树
        /// 可用于elemet ui（el-menu权限路由）等菜单路由
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult RouteTreeMR()
        {
            Guid userId = Guid.Parse(HttpContext.Current.Request.Headers["sessionId"]);
            //根据用户角色查找所有的menu
            var menuIds = (from mr in db.MemRole
                           where mr.Member == userId
                           join rm in db.RoleMenu
                           on mr.Role equals rm.Role
                           select rm.Menu).ToList();
            if (menuIds.Count() == 0)
            {
                return Json(new { status = "fail", msg = "查询为空" });
            }
            var menus = (from m in db.Menu
                         where menuIds.Contains(m.Id) && m.Status == Models.Config.Status.normal && m.Type != "BUTTON"
                         select new MenuRes.Menu
                         {
                             Id = m.Id,
                             PId = m.PId,
                             Name = m.Name,
                             Controller = m.Controller,
                             Action = m.Action,
                             Icon = m.Icon,
                             OrderNum = m.OrderNum
                         }).ToList();
            var menuIdArr = menus.Select(m => m.Id).ToList();

            List<MenuRes.MenuTree> routerTrees = new List<MenuRes.MenuTree>();
            foreach (var menu in menus)
            {
                if (!menuIdArr.Contains(menu.PId))
                {
                    //以分支节点，创建菜单树
                    var menusTemp = new List<MenuRes.Menu>();
                    menusTemp.AddRange(menus);
                    routerTrees.Add(MenuTreeHelper(menu.Id, menusTemp));
                }

            }
            return Json(new { status = "success", msg = "获取成功", content = routerTrees });
        }


        /// <summary>
        /// 获取用户部门权限路由树
        /// 可用于elemet ui（el-menu权限路由）等菜单路由
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult RouteTreeMO()
        {
            Guid userId = Guid.Parse(HttpContext.Current.Request.Headers["sessionId"]);
            //根据用户部门查找所有的menu
            var menuIds = (from mo in db.MemOrg
                           where mo.Member == userId
                           join om in db.OrgMenu
                           on mo.Org equals om.Org
                           select om.Menu).ToList();
            if (menuIds.Count() == 0)
            {
                return Json(new { status = "fail", msg = "查询为空" });
            }
            var menus = (from m in db.Menu
                         where menuIds.Contains(m.Id) && m.Status == Models.Config.Status.normal && m.Type != "BUTTON"
                         select new MenuRes.Menu
                         {
                             Id = m.Id,
                             PId = m.PId,
                             Name = m.Name,
                             Controller = m.Controller,
                             Action = m.Action,
                             Icon = m.Icon,
                             OrderNum = m.OrderNum
                         }).ToList();
            var menuIdArr = menus.Select(m => m.Id).ToList();

            List<MenuRes.MenuTree> routerTrees = new List<MenuRes.MenuTree>();
            foreach (var menu in menus)
            {
                if (!menuIdArr.Contains(menu.PId))
                {
                    //以分支节点，创建菜单树
                    var menusTemp = new List<MenuRes.Menu>();
                    menusTemp.AddRange(menus);
                    routerTrees.Add(MenuTreeHelper(menu.Id, menusTemp));
                }

            }
            return Json(new { status = "success", msg = "获取成功", content = routerTrees });
        }
        /// <summary>
        /// 递归生成菜单树结构（辅助方法）
        /// </summary>
        /// <param name="pId">总根节点</param>
        /// <param name="menus">Id,PId,Name,Controller,Action,Icon,OrderNum</param>
        /// <returns></returns>
        private MenuRes.MenuTree MenuTreeHelper(Guid pId,List<MenuRes.Menu> menus)
        {
            if(menus == null || menus.Count() == 0)
            {
                return null;
            }
            var menu = menus.Where(m => m.Id == pId).First();
            var children = menus.Where(m => m.PId == pId).OrderBy(m => m.OrderNum).ToList();
            menus.Remove(menu);
            var child = new MenuRes.MenuTree
            {
                Id = menu.Id,
                Name = menu.Name,
                Controller = menu.Controller,
                Action = menu.Action,
                Icon = menu.Icon,
            };
            if (children.Any())
            {
                child.Children = new List<MenuRes.MenuTree>();
                foreach(var item in children)
                {
                    child.Children.Add(MenuTreeHelper(item.Id, menus));
                }
            }
            return child;
        }
    }
}
