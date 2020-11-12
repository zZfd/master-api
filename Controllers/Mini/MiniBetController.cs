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
    public class MiniBetController : ApiController
    {
        private readonly MiniDB.MiniDB db = new MiniDB.MiniDB();
        private const string TOKEN = "ZFDYES";

        /// <summary>
        /// 筛选
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ListBets(EntityReq.ListBet param)
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);

            var bets = db.Bet.AsNoTracking().Where(b => b.Member == userId && b.IsSuccess == param.isSuccess);
            if (!string.IsNullOrWhiteSpace(param.team))
            {
                bets = bets.Where(b => b.Team.Contains(param.team));
            }
            if (!string.IsNullOrWhiteSpace(param.match))
            {
                bets = bets.Where(b => b.Match.Contains(param.match));
            }

            if (param.minMoney != 0)
            {
                bets = bets.Where(b => b.Money >= param.minMoney);
            }
            if (param.maxMoney != 0)
            {
                bets = bets.Where(b => b.Money <= param.maxMoney);
            }

            if (param.startTime != DateTime.MinValue)
            {
                bets = bets.Where(b => b.Time >= param.startTime);
            }
            if (param.endTime != DateTime.MinValue)
            {
                bets = bets.Where(b => b.Time <= param.endTime);
            }

            if (!string.IsNullOrWhiteSpace(param.platform))
            {
                bets = bets.Where(b => b.Platform.Contains(param.platform));
            }


            if (bets.Any())
            {
                var results = bets.Select(b => new EntityRes.BetDetail
                {
                    id = b.Id,
                    match = b.Match,
                    time = b.Time,
                    platform = b.Platform,
                    isSuccess = (bool)b.IsSuccess
                }).OrderBy(s => s.time);
                return Json(new { statusCode = HttpStatusCode.OK, msg = "查询成功", content = Helper.PaginationHelper<EntityRes.BetDetail>.Paging(results, param.pageIndex, param.pageSize) });
            }
            else
            {
                return Json(new { statusCode = HttpStatusCode.NotFound, msg = "查询为空" });
            }
        }



        /// <summary>
        /// 添加投注
        /// </summary>
        /// <param name="bet"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SaveBet(EntityReq.Bet bet)
        {
            if (ModelState.IsValid)
            {
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);


                MiniDB.Bet betDB = new MiniDB.Bet
                {
                    Id = Guid.NewGuid(),
                    Member = userId,
                    Match = bet.match,
                    Team = bet.team,
                    Remarks = bet.remarks,
                    Time = bet.time,
                    Money = bet.money,
                    Odds = bet.odds,
                    Profit = bet.profit,
                    Platform = bet.platform,
                    IsSuccess = bet.isSuccess
                };
                db.Entry(betDB).State = System.Data.Entity.EntityState.Added;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { statusCode = HttpStatusCode.Created, msg = "保存成功", content = betDB.Id });
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
        /// 更新投注（修改、删除）
        /// </summary>
        /// <param name="bet"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateBet(EntityReq.BetUpdate bet)
        {
            if (ModelState.IsValid)
            {
                if (bet.id == Guid.Empty)
                {
                    return Json(new { statusCode = HttpStatusCode.NotFound, msg = "投注不存在" });
                }
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);

                var betDB = await db.Bet.FindAsync(bet.id);
                if (betDB == null || betDB.Member != userId)
                {
                    return Json(new { statusCode = HttpStatusCode.NotFound, msg = "投注不存在" });
                }

                betDB.Profit = bet.profit;
                betDB.IsSuccess = bet.isSuccess;

                db.Entry(betDB).State = System.Data.Entity.EntityState.Modified;
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
        /// 获取单个投注
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetBetDetail(Guid id)
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);

            var betDB = await db.Bet.FindAsync(id);
            if (betDB == null || betDB.Member != userId)
            {
                return Json(new { statusCode = HttpStatusCode.NotFound, msg = "投注不存在" });
            }
            var attachmentDB = db.Attachment.FirstOrDefault(a => a.Belong == id && a.Status == Models.Config.Status.normal);
            var detail = new EntityRes.BetDetail
            {
                id = betDB.Id,
                match = betDB.Match,
                team = betDB.Team,
                time = betDB.Time,
                platform = betDB.Platform,
                isSuccess = (bool)betDB.IsSuccess,
                money = betDB.Money,
                profit = betDB.Profit,
                odds = betDB.Odds,
                remarks = betDB.Remarks,
                attachment = attachmentDB.Id,
                attachmentName = attachmentDB == null ? "" : attachmentDB.FileName
            };
            return Json(new { statusCode = HttpStatusCode.OK, msg = "查询成功", content = detail });
        }
    }
}
