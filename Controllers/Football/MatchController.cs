using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ReqFB = WebApi.Models.Request.Football;
using ResFB = WebApi.Models.Response.Football;



namespace WebApi.Controllers.Football
{
    public class MatchController : ApiController
    {
        private readonly DataBase.DB db = new DataBase.DB();
        /// <summary>
        /// 添加比赛
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SaveMatch(ReqFB.Match match)
        {
            if (ModelState.IsValid)
            {
                DataBase.FT_Match matchDB = new DataBase.FT_Match
                {
                    Id = Guid.NewGuid(),
                    HomeTeam = match.HomeTeam,
                    GuestTeam = match.GuestTeam,
                    Time = match.Time
                };
                db.Entry(matchDB).State = System.Data.Entity.EntityState.Added;
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

        /// <summary>
        /// 修改比赛主客队时间
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateMatch(ReqFB.Match match)
        {
            if (ModelState.IsValid)
            {
                if(match.Id == null)
                {
                    return Json(new { status = "fail", msg = "Id为空" });
                }
               
                var matchDB = await db.FT_Match.FindAsync(match.Id);
                if (matchDB == null)
                {
                    return Json(new { status = "fail", msg = "比赛不存在" });
                }
                matchDB.HomeTeam = match.HomeTeam;
                matchDB.GuestTeam = match.GuestTeam;
                matchDB.Time = match.Time;
                db.Entry(matchDB).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "修改成功" });
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

        /// <summary>
        /// 筛选+分页比赛
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ListMatches(ReqFB.ListMatches param)
        {
            if (ModelState.IsValid)
            {
                var matches = db.FT_Match.AsNoTracking().AsQueryable();
                if (param.HomeTeam != Guid.Empty)
                {
                    matches = matches.Where(m => m.HomeTeam == param.HomeTeam);
                }
                if (param.GuestTeam != Guid.Empty)
                {
                    matches = matches.Where(m => m.GuestTeam == param.GuestTeam);
                }
                if (param.StartTime != DateTime.MinValue)
                {
                    matches = matches.Where(m => m.Time >= param.StartTime);
                }
                if (param.EndTime != DateTime.MinValue)
                {
                    matches = matches.Where(m => m.Time <= param.EndTime);
                }
                if (param.League != Guid.Empty)
                {
                    var belongTeams = Factory.TeamFactory.GetBelongTeams(param.League,Models.Config.Status.normal,db);
                    matches = matches.Where(p => belongTeams.Contains(p.Id));
                }
                
                if (matches.Any())
                {
                    var results = matches.Select(p => new
                    {
                        p.Id,
                        HomeName = p.FT_Team_Home.Name,
                        GuestName = p.FT_Team_Guest.Name,
                        //-1为未开赛
                        p.HomeScore,
                        p.GuestScore,
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

        #region 合并到保存比赛详情

        //[HttpPut]
        //public async Task<IHttpActionResult> UpdateMatchScore(ReqFB.MatchScore param)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        var matchDB = await db.FT_Match.FindAsync(param.Id);
        //        if (matchDB == null)
        //        {
        //            return Json(new { status = "fail", msg = "比赛不存在" });
        //        }
        //        matchDB.HomeScore = param.HomeScore;
        //        matchDB.GuestScore = param.GuestScore;
        //        matchDB.Total = (short)(param.HomeScore + param.GuestScore);
        //        db.Entry(matchDB).State = System.Data.Entity.EntityState.Modified;
        //        try
        //        {
        //            await db.SaveChangesAsync();
        //            return Json(new { status = "success", msg = "修改成功" });
        //        }
        //        catch (Exception ex)
        //        {
        //            Helper.LogHelper.WriteErrorLog(ex);
        //            return Json(new { status = "fail", msg = "服务器错误" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { status = "fail", msg = "请求参数错误", content = ModelState });
        //    }
        //}
        #endregion

        /// <summary>
        /// 获取比赛进球详情
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetMatchDetail(Guid matchId)
        {
            var matchDB = await db.FT_Match.FindAsync(matchId);
            if (matchDB == null)
            {
                return Json(new { status = "fail", msg = "比赛不存在" });
            }
            ResFB.MatchDetail matchDetail = new ResFB.MatchDetail
            {
                Match = new ResFB.Match
                {
                    Id = matchId,
                    HomeTeam = matchDB.FT_Team_Home.Name,
                    GuestTeam = matchDB.FT_Team_Guest.Name,
                    HomeScore = matchDB.HomeScore,
                    GuestScore = matchDB.GuestScore,
                    Total = matchDB.Total,
                    Time = matchDB.Time
                },
                HomeScores = new List<ResFB.MatchScore>(),
                GuestScores = new List<ResFB.MatchScore>()
            };
            var matchScores = db.FT_Score.Where(s => s.Match == matchId).Select(s=>new ResFB.MatchScore
            { 
                Team = s.FT_Player_Scorer.FT_Team.Name,
                Scorer = s.FT_Player_Scorer.Name,
                Assistant = s.Assistant == null ? "" :s.FT_Player_Assistant.Name,
                Keeper = s.FT_Player_Keeper.Name,
                Time = s.Time,
                Flag = s.Flag
            }).ToList();
            foreach(var score in matchScores)
            {
                if (score.Team.Equals(matchDetail.Match.HomeTeam))
                {
                    matchDetail.HomeScores.Add(score);
                }
                else
                {
                    matchDetail.GuestScores.Add(score);
                }
            }
            return Json(new { status = "success", msg = "获取成功",content = matchDetail });

        }

        /// <summary>
        /// 保存比赛进球
        /// </summary>
        /// <param name="matchDetail"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SaveMatchDetail(ReqFB.MatchDetail matchDetail)
        {
            if (ModelState.IsValid)
            {

                var matchDB = await db.FT_Match.FindAsync(matchDetail.Id);
                if (matchDB == null)
                {
                    return Json(new { status = "fail", msg = "比赛不存在" });
                }
                matchDB.HomeScore = matchDB.GuestScore = matchDB.Total = 0;
                foreach(ReqFB.Score score in matchDetail.Scores)
                {
                    DataBase.FT_Score scoreDB = new DataBase.FT_Score
                    {
                        Scorer = score.Scorer,
                        Assistant = score.Assistant,
                        Keeper = score.Keeper,
                        Time = score.Time,
                        Flag = score.Flag
                    };
                    if(db.FT_Player.Find(score.Scorer).FT_Team.Id == matchDB.HomeTeam)
                    {
                        matchDB.HomeScore++;
                    }
                    else
                    {
                        matchDB.GuestScore++;
                    }
                    matchDB.Total++;
                    db.Entry(scoreDB).State = System.Data.Entity.EntityState.Added;
                }
                db.Entry(matchDB).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "添加成功" });
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
    }
}
