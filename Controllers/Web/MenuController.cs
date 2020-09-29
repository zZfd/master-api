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
        private const string TOKEN = "ZFDYES";
        private readonly Guid SUPER = Guid.Parse("00000000-0000-0000-0001-000000000000");
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
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
                var menuIds = (from mr in db.MemRole
                               where mr.Member == userId
                               join rm in db.RoleMenu
                               on mr.Role equals rm.Role
                               where rm.Menus.Status == Models.Config.Status.normal
                               select rm.Menu).ToList();
              
                var pMenu = await db.Menus.FindAsync(menu.PId);
                if (!menuIds.Contains(menu.PId))
                {
                    //父节点非法
                    return Json(new { status = "fail", msg = "父节点非法" });
                }
                if(menu.Type == Models.Config.MenuType.root)
                {
                    return Json(new { status = "fail", msg = "不能添加根节点" });
                }
                if (menu.Status > Models.Config.Status.forbidden || menu.Status < Models.Config.Status.deleted)
                {
                    return Json(new { status = "fail", msg = "状态错误" });
                }

                DataBase.Menus menuDB = new DataBase.Menus
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
                //向超级管理员添加该菜单
                db.RoleMenu.Add(new DataBase.RoleMenu { Role = SUPER, Menu = menuDB.Id});
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "保存成功" });
                }
                catch (Exception ex)
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
        public async Task<IHttpActionResult> UpdateMenu(MenuReq.Menu menu)
        {
            if (ModelState.IsValid)
            {
                if (menu.Status > Models.Config.Status.forbidden || menu.Status < Models.Config.Status.deleted)
                {
                    return Json(new { status = "fail", msg = "状态错误" });
                }
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
                var menuIds = (from mr in db.MemRole
                               where mr.Member == userId
                               join rm in db.RoleMenu
                               on mr.Role equals rm.Role
                               where rm.Menus.Status == Models.Config.Status.normal
                               select rm.Menu).ToList();
                if(!menuIds.Contains((Guid)menu.Id) ||!menuIds.Contains(menu.PId) || menu.Id == Guid.Parse("00000000-0000-0000-0001-000000000000")){
                    //节点非法
                    return Json(new { status = "fail", msg = "所选节点非法" });
                }
                DataBase.Menus menuDB = await db.Menus.FindAsync(menu.Id);
                menuDB.Id = (Guid)menu.Id;
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
                catch (Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "服务器内部错误" });
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
        public async Task<IHttpActionResult> UpdateMenuStatus(MenuReq.MenuStatus menuStatus)
        {
            if (ModelState.IsValid)
            {
                if (menuStatus.Status > Models.Config.Status.forbidden || menuStatus.Status < Models.Config.Status.deleted)
                {
                    return Json(new { status = "fail", msg = "状态错误" });
                }
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
                var menuIds = (from mr in db.MemRole
                               where mr.Member == userId
                               join rm in db.RoleMenu
                               on mr.Role equals rm.Role
                               where rm.Menus.Status != Models.Config.Status.deleted
                               select rm.Menu).ToList();
                if (!menuIds.Contains(menuStatus.Id) || menuStatus.Id == Guid.Parse("00000000-0000-0000-0001-000000000000"))
                {
                    //节点非法
                    return Json(new { status = "fail", msg = "所选节点非法" });
                }
                DataBase.Menus menuDB = await db.Menus.FindAsync(menuStatus.Id);

                menuDB.Status = menuStatus.Status;
                //禁用，删除下级菜单
                if(menuStatus.Status != Models.Config.Status.normal)
                {
                    foreach(var menu in db.Menus.Where(m => m.PId == menuStatus.Id && m.Status != Models.Config.Status.deleted))
                    {
                        menu.Status = menuStatus.Status;
                        db.Entry(menu).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                db.Entry(menuDB).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "修改成功" });
                }
                catch (Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "服务器内部错误" });
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
        public IHttpActionResult GetLazyMenu(Guid pId)
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
            try
            {

                var menuIds = (from mr in db.MemRole
                               where mr.Member == userId
                               join rm in db.RoleMenu
                               on mr.Role equals rm.Role
                               where mr.Roles.Status != Models.Config.Status.deleted
                               select rm.Menu).ToList();
                if (menuIds.Count() == 0)
                {
                    return Json(new { status = "fail", msg = "查询为空" });
                }
                if (pId == Guid.Empty)
                {
                    //第一次加载
                    //根据用户角色查找所有的分支menu
                    var menuPIds = db.Menus.Where(m => menuIds.Contains(m.Id) && !menuIds.Contains(m.PId)).OrderBy(m => m.OrderNum).Select(m => new MenuRes.Menu
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
                        HasChildren = db.Menus.FirstOrDefault(mc => mc.PId == m.Id && menuIds.Contains(mc.Id) && mc.Status != Models.Config.Status.deleted) != null,
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

                    var content = from m in db.Menus
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
                                      HasChildren = db.Menus.FirstOrDefault(mc => mc.PId == m.Id && menuIds.Contains(mc.Id) && mc.Status != Models.Config.Status.deleted) != null,
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
                return Json(new { status = "fail", msg = "服务器内部错误" });
            }


        }


        /// <summary>
        /// 获取用户角色菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ListMenuMR()
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
            
            //根据用户角色查找所有的menu
            var menuIds = (from mr in db.MemRole
                           where mr.Member == userId
                           join rm in db.RoleMenu
                           on mr.Role equals rm.Role
                           where rm.Menus.Status == Models.Config.Status.normal
                           select rm.Menu).ToList();
            if (menuIds.Count() == 0)
            {
                return Json(new { status = "fail", msg = "查询为空" });
            }
            var menus = (from m in db.Menus
                         where menuIds.Contains(m.Id) && m.Status == Models.Config.Status.normal
                         orderby m.OrderNum
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
            if (menus.Count() == 0)
            {
                return Json(new { status = "fail", msg = "查询为空" });
            }
            return Json(new { status = "success", msg = "获取成功", content = menus });
        }
        /// <summary>
        /// 获取用户角色菜单树
        /// 可用于elemet ui el-tree等树形结构
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetMenuTreeMR()
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
           
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
            var menus = (from m in db.Menus
                         where menuIds.Contains(m.Id) && m.Status == Models.Config.Status.normal
                         orderby m.OrderNum
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
        public IHttpActionResult GetMenuTreeMO()
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
            
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
            var menus = (from m in db.Menus
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
        public IHttpActionResult GetRouterTreeMR()
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
            
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
            var menus = (from m in db.Menus
                         where menuIds.Contains(m.Id) && m.Status == Models.Config.Status.normal 
                         && m.Type != Models.Config.MenuType.button && m.Type != Models.Config.MenuType.root
                         orderby m.OrderNum
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
        //    var menus = (from m in db.Menus
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
