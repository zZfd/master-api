using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ReqFB = WebApi.Models.Request.Football;


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
                var matches = db.FT_Match.AsQueryable();
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

        /// <summary>
        /// 更新比赛总比分
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
      
        [HttpPut]
        public async Task<IHttpActionResult> UpdateMatchScore(ReqFB.MatchScore param)
        {
            if (ModelState.IsValid)
            {
              
                var matchDB = await db.FT_Match.FindAsync(param.Id);
                if (matchDB == null)
                {
                    return Json(new { status = "fail", msg = "比赛不存在" });
                }
                matchDB.HomeScore = param.HomeScore;
                matchDB.GuestScore = param.GuestScore;
                matchDB.Total = (short)(param.HomeScore + param.GuestScore);
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
        /// 获取比赛进球详情
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetMatchScore(Guid matchId)
        {

        }
    }
}
