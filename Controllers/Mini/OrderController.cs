using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using EntityReq = WebApi.Models.Request.Mini;
using EntityRes = WebApi.Models.Response.Mini;

namespace WebApi.Controllers.Mini
{
    public class OrderController : ApiController
    {
        private readonly MiniDB.MiniDB db = new MiniDB.MiniDB();
        private const string TOKEN = "ZFDYES";

        /// <summary>
        /// 保存订单
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> Save(EntityReq.Order order)
        {
            if (ModelState.IsValid)
            {
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);


                MiniDB.Order orderDB = new MiniDB.Order
                {
                    Id = Guid.NewGuid(),
                    Member = userId,
                     Article= order.article,
                    TimeStart = DateTime.Now,
                    Money = order.money,
                    Status = Models.Config.Status.forbidden
                };
                var logDb = new MiniDB.Log
                {
                    Member = userId,
                    Time = DateTime.Now,
                    Type = "创建订单",
                    Remarks = "创建订单" + orderDB.Id,
                    IP = HttpContext.Current.Request.UserHostAddress,
                    UserAgent = HttpContext.Current.Request.UserAgent
                };
                db.Log.Add(logDb);
                db.Entry(orderDB).State = System.Data.Entity.EntityState.Added;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { statusCode = HttpStatusCode.Created, msg = "保存成功", content = orderDB.Id });
                }
                catch (Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { statusCode = HttpStatusCode.InternalServerError, msg = "服务器错误" });
                }
            }
            else
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest, msg = "请求参数错误", content = ModelState.Values });
            }
        }

        /// <summary>
        /// 修改订单
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> Update(EntityReq.OrderUpdate order)
        {
            if (ModelState.IsValid)
            {
                if (order.id == Guid.Empty)
                {
                    return Json(new { statusCode = HttpStatusCode.NotFound, msg = "订单不存在" });
                }
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);

                var orderDB = await db.Order.FindAsync(order.id);
                if (orderDB == null || orderDB.Member != userId)
                {
                    return Json(new { statusCode = HttpStatusCode.NotFound, msg = "订单不存在" });
                }

                orderDB.TimeExpire = DateTime.Now;
                orderDB.Status = order.status;
                var logDb = new MiniDB.Log
                {
                    Member = userId,
                    Time = DateTime.Now,
                    Type = "修改订单",
                    Remarks = "修改订单" + orderDB.Id + " status=" + order.status,
                    IP = HttpContext.Current.Request.UserHostAddress,
                    UserAgent = HttpContext.Current.Request.UserAgent
                };
                db.Log.Add(logDb);
                db.Entry(orderDB).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { statusCode = HttpStatusCode.OK, msg = "修改成功" });
                }
                catch (Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { statusCode = HttpStatusCode.InternalServerError, msg = "服务器错误" });
                }
            }
            else
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest, msg = "请求参数错误", content = ModelState.Values });
            }
        }

        /// <summary>
        /// 筛选订单
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult ListOrder(EntityReq.ListOrder param)
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);

            var orders = db.Order.AsNoTracking().Where(b => b.Member == userId && b.Status == param.status);


            if (param.startTime != DateTime.MinValue)
            {
                orders = orders.Where(b => b.TimeStart >= param.startTime);
            }
            if (param.endTime != DateTime.MinValue)
            {
                orders = orders.Where(b => b.TimeStart <= param.endTime);
            }

            if (!string.IsNullOrWhiteSpace(param.author))
            {
                orders = orders.Where(b => b.Article1.Member.NickName.Contains(param.author));
            }


            if (orders.Any())
            {
                var results = orders.Select(o => new EntityRes.Order
                {
                    id = o.Id,
                    match = o.Article1.Match,
                    time = o.TimeExpire,
                    title = o.Article1.Title,
                    author = o.Article1.Member.NickName,
                    isTrue = (bool)o.Article1.IsTrue,
                    article = o.Article
                }).OrderBy(s => s.time);
                return Json(new { statusCode = HttpStatusCode.OK, msg = "查询成功", content = Helper.PaginationHelper<EntityRes.Order>.Paging(results, param.pageIndex, param.pageSize) });
            }
            else
            {
                return Json(new { statusCode = HttpStatusCode.NotFound, msg = "查询为空" });
            }
        }
    }
}
