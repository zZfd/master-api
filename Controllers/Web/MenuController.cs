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
        /// 懒加载获取用户菜单
        /// </summary>
        /// <param name="PId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult LazyMenu(Guid PId)
        {
            if(PId == Guid.Empty)
            {
                return Json(new { status = "fail", msg = "参数错误" });
            }
            var menu = db.Menu.Find(PId);
            if (menu.Status == (short)Models.Setting.NormalStauts.已删除)
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
            bool isOwned = menuIds.Contains(PId);
            if (!isOwned)
            {
                return Json(new { status = "fail", msg = "权限不足" });
            }
           
            var content = from m in db.Menu
                          where m.PId == PId && menuIds.Contains(m.Id) && m.Status != (short)Models.Setting.NormalStauts.已删除
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
                              HasChildren = db.Menu.FirstOrDefault(mc => mc.PId == m.Id && menuIds.Contains(mc.Id) && mc.Status != (short)Models.Setting.NormalStauts.已删除) != null
                          };
            return Json(new { status = "success", msg = "获取成功", content });
        }

        /// <summary>
        /// 获取用户菜单树
        /// 可用于elemet ui el-tree等树形结构
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult MenuTree()
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
                          where menuIds.Contains(m.Id) && m.Status == (short)Models.Setting.NormalStauts.正常
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
            return Json(new { status = "success", msg = "获取成功", content = MenuTreeHelper(Guid.Parse("00000000-0000-0000-0001-000000000000"), menus).Children });
        }

        /// <summary>
        /// 获取用户权限路由树
        /// 可用于elemet ui（el-menu权限路由）等菜单路由
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult RouteTree()
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
                         where menuIds.Contains(m.Id) && m.Status == (short)Models.Setting.NormalStauts.正常 && m.Type != "BUTTON"
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
            return Json(new { status = "success", msg = "获取成功", content = MenuTreeHelper(Guid.Parse("00000000-0000-0000-0001-000000000000"), menus).Children });
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
