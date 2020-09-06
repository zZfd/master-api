using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DataBase;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class MembersController : ApiController
    {
        private DB db = new DB();

        // GET: api/Members
        public IQueryable<MemberModel> GetMember()
        {
            var power = db.Member.Find(Guid.Parse("00000000-0001-0002-0000-000000000000")).MemRole.ToList();
            return db.Member.Select(s=>new MemberModel { Id = s.Id});
        }

        // GET: api/Members/5
        [ResponseType(typeof(Member))]
        public async Task<IHttpActionResult> GetMember(Guid id)
        {
            Member member = await db.Member.FindAsync(id);
            MemberModel model = new MemberModel { Id = member.Id};
            if (member == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        // PUT: api/Members/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMember(Guid id, Member member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != member.Id)
            {
                return BadRequest();
            }

            db.Entry(member).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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

        // POST: api/Members
        [ResponseType(typeof(Member))]
        public async Task<IHttpActionResult> PostMember(Member member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Member.Add(member);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MemberExists(member.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = member.Id }, member);
        }

        // DELETE: api/Members/5
        [ResponseType(typeof(Member))]
        public async Task<IHttpActionResult> DeleteMember(Guid id)
        {
            Member member = await db.Member.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            db.Member.Remove(member);
            await db.SaveChangesAsync();

            return Ok(member);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MemberExists(Guid id)
        {
            return db.Member.Count(e => e.Id == id) > 0;
        }
    }
}