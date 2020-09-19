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

namespace WebApi.Controllers.Web
{
    public class AttachmentsController : ApiController
    {
        private DB db = new DB();

        // GET: api/Attachments
        public IQueryable<Attachment> GetAttachment()
        {
            return db.Attachment;
        }

        // GET: api/Attachments/5
        [ResponseType(typeof(Attachment))]
        public async Task<IHttpActionResult> GetAttachment(Guid id)
        {
            Attachment attachment = await db.Attachment.FindAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }

            return Ok(attachment);
        }

        // PUT: api/Attachments/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAttachment(Guid id, Attachment attachment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != attachment.Id)
            {
                return BadRequest();
            }

            db.Entry(attachment).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttachmentExists(id))
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

        // POST: api/Attachments
        [ResponseType(typeof(Attachment))]
        public async Task<IHttpActionResult> PostAttachment(Attachment attachment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Attachment.Add(attachment);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AttachmentExists(attachment.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = attachment.Id }, attachment);
        }

        // DELETE: api/Attachments/5
        [ResponseType(typeof(Attachment))]
        public async Task<IHttpActionResult> DeleteAttachment(Guid id)
        {
            Attachment attachment = await db.Attachment.FindAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }

            db.Attachment.Remove(attachment);
            await db.SaveChangesAsync();

            return Ok(attachment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AttachmentExists(Guid id)
        {
            return db.Attachment.Count(e => e.Id == id) > 0;
        }
    }
}