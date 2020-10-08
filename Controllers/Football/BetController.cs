using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ReqFB = WebApi.Models.Request.Football;
using ResFB = WebApi.Models.Response.Football;

namespace WebApi.Controllers.Football
{
    public class BetController : ApiController
    {
        private readonly DataBase.DB db = new DataBase.DB();

        [HttpPost]
        public IHttpActionResult ListBet(ReqFB.ListBet param)
        {
            if (ModelState.IsValid)
            {
                var bets = db.FT_Bet.AsNoTracking().AsQueryable();
                if (param.Team != Guid.Empty)
                {
                    bets = bets.Where(b => b.Team == param.Team);
                }
                if (param.Match != Guid.Empty)
                {
                    bets = bets.Where(b => b.Match == param.Match);
                }
                if (param.MinMoney != -1)
                {
                    bets = bets.Where(b => b.Money >= param.MinMoney);
                }
                if (param.MaxMoney != -1)
                {
                    bets = bets.Where(b => b.Money <= param.MaxMoney);
                }

                if (param.StartTime != DateTime.MinValue)
                {
                    bets = bets.Where(b => b.Time >= param.StartTime);
                }
                if (param.EndTime != DateTime.MinValue)
                {
                    bets = bets.Where(b => b.Time <= param.EndTime);
                }

                if (!string.IsNullOrWhiteSpace(param.Platform))
                {
                    bets = bets.Where(b => b.Platform.Contains(param.Platform));
                }

                if (param.IsSuccess != null)
                {
                    bets = bets.Where(b => b.IsSuccess == param.IsSuccess);
                }
                if (bets.Any())
                {
                    var results = bets.Select(b => new
                    {
                        b.Id,
                        Match = b.FT_Match.FT_Team_Home.Name + ":" + b.FT_Match.FT_Team_Guest.Name,
                        Team = b.FT_Team.Name,
                        b.Time,
                        b.Platform,
                        Status = Factory.CommonFactory.BetResultFormat(b.IsSuccess)
                    });
                    return Json(new { status = "success", msg = "查询成功", content = results });
                }
                else
                {
                    return Json(new { status = "fail", msg = "查询为空" });
                }
            }
            else
            {
                return Json(new { status = "fail", msg = "请求参数错误", content = ModelState });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> SaveBet(ReqFB.Bet bet)
        {
            if (ModelState.IsValid)
            {
                if (await db.FT_Match.FindAsync(bet.Match) == null)
                {
                    return Json(new { status = "fail", msg = "比赛不存在" });
                }

                if (await db.FT_Team.FindAsync(bet.Team) == null)
                {
                    return Json(new { status = "fail", msg = "球队不存在" });
                }

                var attachmentDB = await db.Attachments.FindAsync(bet.Attachment);
                if (attachmentDB == null)
                {
                    return Json(new { status = "fail", msg = "附件不存在" });
                }
                DataBase.FT_Bet betDB = new DataBase.FT_Bet
                {
                    Match = bet.Match,
                    Team = bet.Team,
                    Remarks = bet.Remarks,
                    Attachment = bet.Attachment,
                    Time = bet.Time,
                    Money = bet.Money,
                    Odds = bet.Odds,
                    Profit = 0,
                    Platform = bet.Platform,
                    IsSuccess = Models.Config.Status.deleted
                };
                //保存按钮 --> 上传附件（状态禁用） --> 保存投注 （修改附件状态为正常）
                attachmentDB.Status = Models.Config.Status.normal;
                db.Entry(betDB).State = System.Data.Entity.EntityState.Added;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "保存成功" });
                }
                catch (Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "服务器错误" });
                }
            }
            else
            {
                return Json(new { status = "fail", msg = "请求参数错误", content = ModelState });
            }
        }

        [HttpPut]
        public async Task<IHttpActionResult> UpdateBet(ReqFB.Bet bet)
        {
            if (ModelState.IsValid)
            {
                if (bet.Id == Guid.Empty)
                {
                    return Json(new { status = "fail", msg = "请选择投注单" });
                }

                var betDB = await db.FT_Bet.FindAsync(bet.Id);
                if (betDB == null)
                {
                    return Json(new { status = "fail", msg = "投注不存在" });
                }


                if (await db.FT_Match.FindAsync(bet.Match) == null)
                {
                    return Json(new { status = "fail", msg = "比赛不存在" });
                }

                if (await db.FT_Team.FindAsync(bet.Team) == null)
                {
                    return Json(new { status = "fail", msg = "球队不存在" });
                }
                var attachmentDB = await db.Attachments.FindAsync(bet.Attachment);
                if (attachmentDB == null)
                {
                    return Json(new { status = "fail", msg = "附件不存在" });
                }
                betDB.Match = bet.Match;
                betDB.Team = bet.Team;
                betDB.Remarks = bet.Remarks;
                betDB.Time = bet.Time;
                betDB.Money = bet.Money;
                betDB.Odds = bet.Odds;
                betDB.Profit = betDB.Profit;
                betDB.Platform = bet.Platform;
                betDB.IsSuccess = bet.IsSuccess;
                if (betDB.Attachment != bet.Attachment)
                {
                    var oldAttachment = await db.Attachments.FindAsync(betDB.Attachment);
                    if (oldAttachment != null)
                    {
                        oldAttachment.Status = Models.Config.Status.deleted;
                    }
                    betDB.Attachment = bet.Attachment;
                    attachmentDB.Status = Models.Config.Status.normal;
                }
                db.Entry(betDB).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "保存成功" });
                }
                catch (Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "服务器错误" });
                }
            }
            else
            {
                return Json(new { status = "fail", msg = "请求参数错误", content = ModelState });
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetBetDetail(Guid id)
        {
            var betDB = await db.FT_Bet.FindAsync(id);
            if (betDB == null)
            {
                return Json(new { status = "fail", msg = "投注不存在" });
            }
            var detail = new ResFB.BetDetail
            {
                Id = betDB.Id,
                Match = betDB.FT_Match.FT_Team_Home.Name + ":" + betDB.FT_Match.FT_Team_Guest.Name,
                Team = betDB.FT_Team.Name,
                Time = betDB.Time,
                Platform = betDB.Platform,
                Status = Factory.CommonFactory.BetResultFormat(betDB.IsSuccess),
                Money = betDB.Money,
                Profit = betDB.Profit,
                Odds = betDB.Odds,
                Remarks = betDB.Remarks,
                Attachment = betDB.Attachment
            };
            return Json(new { status = "success", msg = "查询成功", content = detail });
        }
    }
}
