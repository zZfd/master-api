using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ReqFB = WebApi.Models.Request.Football;
using ResFB = WebApi.Models.Response.Football;


namespace WebApi.Controllers.Football
{
    public class PlayerController : ApiController
    {
        private readonly DataBase.DB db = new DataBase.DB();

        /// <summary>
        /// 添加球员
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SavePlayer(ReqFB.Player player)
        {
            if (ModelState.IsValid)
            {
                DataBase.FT_Player playerDB = new DataBase.FT_Player
                {
                    Id = Guid.NewGuid(),
                    Name = player.Name,
                    EName = player.EName,
                    Flag = player.Flag,
                    Team = player.Team,
                    Country = player.Country,
                    Status = player.Status,
                    Age = player.Age
                };
                db.Entry(playerDB).State = System.Data.Entity.EntityState.Added;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { code = 20000,status = "success", msg = "保存成功" });
                }
                catch (Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "服务器错误" });
                }
            }
            else
            {
                return Json(new { code = 20000,status = "fail", msg = "请求参数错误", content = ModelState });
            }
        }

        /// <summary>
        /// 修改球员
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdatePlayer(ReqFB.Player player)
        {
            if (ModelState.IsValid)
            {
                if (player.Id == null)
                {
                    return Json(new { code = 20000, status = "fail", msg = "Id为空" });
                }
                var playerDB = await db.FT_Player.FindAsync(player.Id);
                if (playerDB == null)
                {
                    return Json(new { code = 20000, status = "fail", msg = "球员不存在" });
                }
                playerDB.Name = player.Name;
                playerDB.EName = player.EName;
                playerDB.Flag = player.Flag;
                playerDB.Team = player.Team;
                playerDB.Country = player.Country;
                playerDB.Status = player.Status;
                playerDB.Age = player.Age;
                db.Entry(playerDB).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { code = 20000,status = "success", msg = "修改成功" });
                }
                catch (Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "服务器错误" });
                }
            }
            else
            {
                return Json(new { code = 20000, status = "fail", msg = "请求参数错误", content = ModelState });
            }
        }

        /// <summary>
        /// 查询球员
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ListPlayers(ReqFB.ListPlayer query)
        {
            if (ModelState.IsValid)
            {
                var players = db.FT_Player.Where(p => p.Status != Models.Config.Status.deleted);
                if (!string.IsNullOrWhiteSpace(query.Name))
                {
                    players = players.Where(p => p.Name.StartsWith(query.Name));
                }
                if (!string.IsNullOrWhiteSpace(query.EName))
                {
                    players = players.Where(p => p.Name.StartsWith(query.EName));
                }
                if (query.MinAge != -1)
                {
                    players = players.Where(p => p.Age >= query.MinAge);
                }
                if (query.MaxAge != -1)
                {
                    players = players.Where(p => p.Age <= query.MaxAge);
                }
                if (query.Flag != -1)
                {
                    players = players.Where(p => p.Flag == query.Flag);
                }
                if (query.Team != null)
                {
                    players = players.Where(p => p.Team == query.Team);
                }
                if (query.Country != null)
                {
                    players = players.Where(p => p.Country == query.Country);
                }
                if (players.Any())
                {
                    var results = players.Select(p => new ResFB.Player
                    {
                        Id = p.Id,
                        Name = p.Name,
                        EName = p.EName,
                        Age = p.Age,
                        Status = p.Status,
                        Team = p.Team,
                        Country = p.Country,
                        Flag = p.Flag
                    }).OrderBy(s => s.Status);
                    var res = Helper.PaginationHelper<ResFB.Player>.Paging(results, query.PageIndex, query.PageSize);

                    return Json(new { code = 20000, status = "success", msg = "查询成功", content = new { data = res,total = res.Total} });
                }
                else
                {
                    return Json(new { code = 20000, status = "fail", msg = "查询为空" });
                }
            }
            else
            {
                return Json(new { code = 20000, status = "fail", msg = "请求参数错误", content = ModelState });
            }
        }

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetPlayerDetail(Guid id)
        {
            var player = await db.FT_Player.FindAsync(id);
            if (player == null || player.Status == Models.Config.Status.deleted)
            {
                return Json(new { status = "fail", msg = "查询为空" });
            }
            var results = db.FT_Player.Where(p => p.Id == id).Select(p => new
            {
                p.Id,
                p.Name,
                p.EName,
                p.Age,
                p.Status,
                p.Team,
                p.Country,
                p.Flag,
                //球员在当前球队进球情况
                Score = db.FT_Score.Where(s => s.Scorer == p.Id && s.FT_Player_Scorer.Team == p.Team)
                  .Select(s => new { AssistantName = s.FT_Player_Assistant.Name, s.Time, Keeper = s.FT_Player_Keeper.Name, LoseTeam = s.FT_Player_Keeper.FT_Team.Name }),
                //球员在当前球队助攻情况
                Assistant = db.FT_Score.Where(s => s.Assistant == p.Id && s.FT_Player_Assistant.Team == p.Team)
                .Select(s => new { ScorerName = s.FT_Player_Scorer.Name, s.Time, Keeper = s.FT_Player_Keeper.Name, LoseTeam = s.FT_Player_Keeper.FT_Team.Name }),

            });
            return Json(new { status = "success", msg = "查询成功", content = results });
        }
    }
}
