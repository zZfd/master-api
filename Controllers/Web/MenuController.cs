using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MenuRes = WebApi.Models.Response.Web.Menu;
using MenuReq = WebApi.Models.Request.Web.Menu;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;

namespace WebApi.Controllers.Web
{
    public class MenuController : ApiController
    {
        private readonly DataBase.DB db = new DataBase.DB();

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SaveMenu(MenuReq.Menu menu)
        {
            if (ModelState.IsValid)
            {
                var pMenu = await db.Menu.FindAsync(menu.PId);
                if (pMenu == null)
                {
                    //父节点非法
                    return Json(new { status = "fail", msg = "保存失败" });
                }

                DataBase.Menu menuDB = new DataBase.Menu
                {
                    Id = Guid.NewGuid(),
                    PId = menu.PId,
                    Name = menu.Name,
                    Controller = menu.Controller,
                    Action = menu.Action,
                    Type = menu.Type,
                    Code = menu.Code,
                    Icon = menu.Icon,
                    Status = menu.Status,
                    OrderNum = menu.OrderNum,
                };

                db.Entry(menuDB).State = System.Data.Entity.EntityState.Added;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "保存成功" });
                }
                catch (DbUpdateException ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "保存失败" });
                }
            }
            else
            {
                return Json(new { status = "fail", msg = "请求参数错误" });
            }
        }


        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateOrg(MenuReq.Menu menu)
        {
            if (ModelState.IsValid)
            {
                DataBase.Menu menuDB = await db.Menu.FindAsync(menu.Id);
                if (menuDB == null || menuDB.Id == Guid.Parse("00000000-0000-0000-0001-000000000000"))
                {
                    //节点非法
                    //根部门不允许修改
                    return Json(new { status = "fail", msg = "请求参数错误" });
                }
                var pMenu = await db.Menu.FindAsync(menu.PId);
                if (pMenu == null)
                {
                    //父节点非法
                    return Json(new { status = "fail", msg = "保存失败" });
                }

                menuDB.Id = menu.Id;
                menuDB.PId = menu.PId;
                menuDB.Name = menu.Name;
                menuDB.Controller = menu.Controller;
                menuDB.Action = menu.Action;
                menuDB.Type = menu.Type;
                menuDB.Code = menu.Code;
                menuDB.Icon = menu.Icon;
                menuDB.OrderNum = menu.OrderNum;
                menuDB.Status = menu.Status;
                
                db.Entry(menuDB).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "修改成功" });
                }
                catch (DbUpdateException ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "修改失败" });
                }

            }
            else
            {
                return Json(new { status = "fail", msg = "请求参数错误" });
            }
        }

        /// <summary>
        /// 禁用、启用、删除菜单
        /// </summary>
        /// <param name="menuStatus"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateOrgStatus(MenuReq.MenuStatus menuStatus)
        {
            if (ModelState.IsValid)
            {
                DataBase.Menu menuDB = await db.Menu.FindAsync(menuStatus.Id);
                if (menuDB == null || menuStatus.Id == Guid.Parse("00000000-0000-0000-0001-000000000000"))
                {
                    //根部门不许操作
                    return Json(new { status = "fail", msg = "请求参数错误" });
                }
                menuDB.Status = menuStatus.Status;
                db.Entry(menuDB).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "修改成功" });
                }
                catch (DbUpdateException ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "修改失败" });
                }
            }
            else
            {
                return Json(new { status = "fail", msg = "请求参数错误" });
            }
        }
        /// <summary>
        /// 懒加载获取用户角色菜单
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult LazyMenu(Guid pId)
        {
            Guid userId = Guid.Parse(HttpContext.Current.Request.Headers["sessionId"]);
            try
            {

                var menuIds = (from mr in db.MemRole
                               where mr.Member == userId
                               join rm in db.RoleMenu
                               on mr.Role equals rm.Role
                               select rm.Menu).ToList();
                if (menuIds.Count() == 0)
                {
                    return Json(new { status = "fail", msg = "查询为空" });
                }
                if (pId == Guid.Empty)
                {
                    //第一次加载
                    //根据用户角色查找所有的分支menu
                    var menuPIds = db.Menu.Where(m => menuIds.Contains(m.Id) && !menuIds.Contains(m.PId)).OrderBy(m => m.OrderNum).Select(m => new MenuRes.Menu
                    {
                        Id = m.Id,
                        PId = m.PId,
                        Name = m.Name,
                        Controller = m.Controller,
                        Action = m.Action,
                        Type = m.Type,
                        Icon = m.Icon,
                        OrderNum = m.OrderNum,
                        Status = m.Status,
                        //判断menu中是否有子节点
                        HasChildren = db.Menu.FirstOrDefault(mc => mc.PId == m.Id && menuIds.Contains(mc.Id) && mc.Status != Models.Config.Status.deleted) != null,
                        //RoleCount = 0,
                        //OrgCount = 0,
                        //MemberCount = 0
                    });
                    return Json(new { status = "success", msg = "获取成功", content = menuPIds });
                }
                else
                {
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
            }
            catch (Exception ex)
            {
                Helper.LogHelper.WriteErrorLog(ex);
                return Json(new { status = "fail", msg = "获取失败" });
            }


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
            if (menuIds.Count() == 0)
            {
                return Json(new { status = "fail", msg = "查询为空" });
            }
            var menus = (from m in db.Menu
                         where m.Status == Models.Config.Status.normal && menuIds.Contains(m.Id)
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
                         where menuIds.Contains(m.Id) && m.Status == Models.Config.Status.normal && m.Type != Models.Config.MenuType.button
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
        //[HttpGet]
        //public IHttpActionResult RouteTreeMO()
        //{
        //    Guid userId = Guid.Parse(HttpContext.Current.Request.Headers["sessionId"]);
        //    //根据用户部门查找所有的menu
        //    var menuIds = (from mo in db.MemOrg
        //                   where mo.Member == userId
        //                   join om in db.OrgMenu
        //                   on mo.Org equals om.Org
        //                   select om.Menu).ToList();
        //    if (menuIds.Count() == 0)
        //    {
        //        return Json(new { status = "fail", msg = "查询为空" });
        //    }
        //    var menus = (from m in db.Menu
        //                 where menuIds.Contains(m.Id) && m.Status == Models.Config.Status.normal && m.Type != "BUTTON"
        //                 select new MenuRes.Menu
        //                 {
        //                     Id = m.Id,
        //                     PId = m.PId,
        //                     Name = m.Name,
        //                     Controller = m.Controller,
        //                     Action = m.Action,
        //                     Icon = m.Icon,
        //                     OrderNum = m.OrderNum
        //                 }).ToList();
        //    var menuIdArr = menus.Select(m => m.Id).ToList();

        //    List<MenuRes.MenuTree> routerTrees = new List<MenuRes.MenuTree>();
        //    foreach (var menu in menus)
        //    {
        //        if (!menuIdArr.Contains(menu.PId))
        //        {
        //            //以分支节点，创建菜单树
        //            var menusTemp = new List<MenuRes.Menu>();
        //            menusTemp.AddRange(menus);
        //            routerTrees.Add(MenuTreeHelper(menu.Id, menusTemp));
        //        }

        //    }
        //    return Json(new { status = "success", msg = "获取成功", content = routerTrees });
        //}
        /// <summary>
        /// 递归生成菜单树结构（辅助方法）
        /// </summary>
        /// <param name="pId">总根节点</param>
        /// <param name="menus">Id,PId,Name,Controller,Action,Icon,OrderNum</param>
        /// <returns></returns>
        private MenuRes.MenuTree MenuTreeHelper(Guid pId, List<MenuRes.Menu> menus)
        {
            if (menus == null || menus.Count() == 0)
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
                foreach (var item in children)
                {
                    child.Children.Add(MenuTreeHelper(item.Id, menus));
                }
            }
            return child;
        }
    }
}
