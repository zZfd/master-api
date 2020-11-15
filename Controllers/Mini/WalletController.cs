using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebApi.Controllers.Mini
{
    public class WalletController : ApiController
    {
        private readonly MiniDB.MiniDB db = new MiniDB.MiniDB();
        private const string TOKEN = "ZFDYES";

        /// <summary>
        /// 查询收入,提现
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public  IHttpActionResult GetIncome(DateTime startTime, DateTime endTime, bool type)
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
            if (startTime == DateTime.MinValue || endTime == DateTime.MinValue || startTime < endTime)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest, msg = "时间错误" });
            }
            var source = db.WalletLog.AsNoTracking().Where(w => w.Member == userId && w.Type == type && w.Time >= startTime && w.Time <= endTime);
            if (source.Any())
            {
                var result = source.Select(w => new
                {
                    w.Id,
                    title = w.Article == null ? "" : w.Article1.Title,
                    w.Time,
                    w.Money,
                    w.Remarks
                });
                return Json(new { statusCode = HttpStatusCode.OK, msg = "查询成功", content = result });
            }
            else
            {
                return Json(new { statusCode = HttpStatusCode.NoContent, msg = "查询为空" });
            }


        }

       
    }
}
