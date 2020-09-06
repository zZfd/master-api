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