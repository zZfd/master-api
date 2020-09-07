using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using DataBase;

namespace WebApi.Controllers
{
    public class MenusController : ApiController
    {
        private DB db = new DB();

        // GET: api/Menus
        //[HttpPost]
        //public IHttpActionResult GetMenuChild(Guid pid)
        //{

        //}

        /// <summary>
        /// 获取菜单树
        /// 可用于elemet ui el-tree等树形结构
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetMenuTree()
        {
            var menus = db.Menu.Where(m =>m.Status == (short)Models.Setting.NormalStauts.正常)
                .Select(m => new Models.Menu.MenuModel
                {
                    Id = m.Id,
                    PId = m.PId,
                    Action = m.Action,
                    Controller = m.Controller,
                    Icon = m.Icon,
                    Name = m.Name,
                }).ToList();
            var menuTree = MenuTree(Guid.Parse("D4406EB9-C2D7-4BFD-8510-DCB417FA7F33"), menus);
            return Json(new { code = 200, msg = "获取成功", content = menuTree });
        }

        /// <summary>
        /// 获取权限路由树
        /// 可用于elemet ui（el-menu权限路由）等菜单路由
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetRouterTree()
        {
            var routers = db.Menu.Where(m => m.Type != "BUTTON" && m.Status == (short)Models.Setting.NormalStauts.正常)
                .Select(m => new Models.Menu.MenuModel
                {
                    Id = m.Id,
                    PId = m.PId,
                    Action = m.Action,
                    Controller = m.Controller,
                    Icon = m.Icon,
                    Name = m.Name,
                }).ToList();
            var routerTree = MenuTree(Guid.Parse("D4406EB9-C2D7-4BFD-8510-DCB417FA7F33"), routers);
            return Json(new { code = 200, msg = "获取成功", content = routerTree });
        }
        public Models.Menu.MenuTree MenuTree(Guid pid,List<Models.Menu.MenuModel> menus)
        {
            if(menus == null || menus.Count() == 0)
            {
                return null;
            }
            var menu = menus.Where(m => m.Id == pid).FirstOrDefault();
            var children = menus.Where(m => m.PId == pid).ToList();
            menus.Remove(menu);
            Models.Menu.MenuTree child = new Models.Menu.MenuTree
            {
                Id = menu.Id,
                Name = menu.Name,
                Controller = menu.Controller,
                Action = menu.Action,
                Icon = menu.Icon,
            };
            if (children.Any())
            {
                child.Children = new List<Models.Menu.MenuTree>();

                foreach (var item in children)
                {
                    child.Children.Add(MenuTree(item.Id, menus));
                }
            }
            return child;
        }
        //public IHttpActionResult Get

        // GET: api/Menus/5
        [ResponseType(typeof(Menu))]
        public IHttpActionResult GetMenu(Guid id)
        {
            #region 菜单权限验证
            if (!Filter.Power.Menu(Guid.Parse("CCB94B3B-0CFD-4168-9621-63BFF56A057F"), Guid.Parse(HttpContext.Current.Request.Headers["sessionId"]), db))
            {
                return Json(new { code = 203, msg = "权限不足" });
            }
            #endregion

            if (id == null)
            {
                id = Guid.Parse("00000000-0000-0000-0000-000000000000");//根节点
            }

            var menu = from a in db.Menu
                       where a.PId == id && a.Status != -1
                       orderby a.OrderNum
                       select new
                       {
                           a.Action,
                           a.Controller,
                           a.Icon,
                           a.Id,
                           a.Code,
                           a.Name,
                           a.Type,
                           a.OpenNewPage,
                           a.OrderNum,
                           a.PId,
                           a.Status,
                           hasChildren = (db.Menu.Where(s => s.PId == a.Id && s.Status != -1).Count() > 0)
                       };

            return Json(new { code = 200, msg = "获取成功", content = menu });
        }

        // PUT: api/Menus/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMenu(Guid id, Menu menu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != menu.Id)
            {
                return BadRequest();
            }

            db.Entry(menu).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Menus
        [ResponseType(typeof(Menu))]
        public async Task<IHttpActionResult> PostMenu(Menu menu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Menu.Add(menu);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MenuExists(menu.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = menu.Id }, menu);
        }

        // DELETE: api/Menus/5
        [ResponseType(typeof(Menu))]
        public async Task<IHttpActionResult> DeleteMenu(Guid id)
        {
            Menu menu = await db.Menu.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            db.Menu.Remove(menu);
            await db.SaveChangesAsync();

            return Ok(menu);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MenuExists(Guid id)
        {
            return db.Menu.Count(e => e.Id == id) > 0;
        }
    }
}