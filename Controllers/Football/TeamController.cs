using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ReqFB = WebApi.Models.Request.Football;
using ResFB = WebApi.Models.Response.Football;

namespace WebApi.Controllers.Football
{
    public class TeamController : ApiController
    {
        private readonly DataBase.DB db = new DataBase.DB();
        private const string TOKEN = "ZFDYES";
        /// <summary>
        /// 获取球队树列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetTeamTree()
        {
            try
            {
              
                var teams = db.FT_Team.Where(t => t.Status == Models.Config.Status.normal).Select(t => new ResFB.Team
                {
                    Id = t.Id,
                    PId = t.PId,
                    Name = t.Name,
                    OrderNum = t.OrderNum,
                }).ToList();
                
                return Json(new { status = "success", msg = "获取成功", content = TeamTreeHelper(Guid.Empty, teams).Children });
            }
            catch (Exception ex)
            {
                Helper.LogHelper.WriteErrorLog(ex);
                return Json(new { status = "fail", msg = "服务器内部错误" });
            }
        }




        /// <summary>
        /// 懒加载球队树
        /// 当Pid为空时，返回所有球队
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetLazyTeam(Guid pId)
        {
            var team = db.FT_Team.Find(pId);
            if( pId != Guid.Empty && team == null)
            {
                return Json(new { status = "fail", msg = "查询不存在" });
            }
            //获得下属所有球队Id
            var teamIds = Factory.TeamFactory.GetBelongs(pId,Models.Config.Status.deleted,db);
            if(teamIds.Count() == 0)
            {
                //无下属球队时添加自身
                teamIds.Add(pId);
            }

            try
            {
                var teams = db.FT_Team.AsNoTracking().Where(t => t.PId == pId && t.Status != Models.Config.Status.deleted).Select(t=>new { 
                    t.Id,
                    t.PId,
                    t.Name,
                    t.EName,
                    t.Status,
                    Player = db.FT_Player.Where(p=>teamIds.Contains(p.Team) && p.Status != Models.Config.Status.deleted).Count(),
                    Match = db.FT_Match.Where(m=>teamIds.Contains(m.HomeTeam) || teamIds.Contains(m.GuestTeam)).Count(),
                    HasChildren = db.FT_Team.Where(ft=>ft.PId == t.Id && ft.Status != Models.Config.Status.deleted).Any()

                });
                return Json(new { code = 20000, status = "success",msg = "获取成功",data = teams});
            }
            catch (Exception ex)
            {
                Helper.LogHelper.WriteErrorLog(ex);
                return Json(new { code = 20000, status = "fail", msg = "服务器内部错误" });
            }
        }

        /// <summary>
        /// 添加球队
        /// </summary>
        /// <param name="football"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SaveTeam(ReqFB.Team football)
        {
            if (ModelState.IsValid)
            {
              
                var pTeam = await db.FT_Team.FindAsync(football.PId);

                if (pTeam == null || pTeam.Flag == Models.Config.FTeamFlag.team)
                {
                    return Json(new { status = "fail", msg = "父级非法" });
                }
                DataBase.FT_Team teamDB = new DataBase.FT_Team
                {
                    Id = Guid.NewGuid(),
                    PId = football.PId,
                    Name = football.Name,
                    EName = football.EName,
                    Flag = football.Flag,
                    Status = football.Status,
                    OrderNum = football.OrderNum,
                };
                
                db.Entry(teamDB).State = System.Data.Entity.EntityState.Added;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "保存成功" });
                }
                catch (Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "保存失败" });
                }

            }
            else
            {
                return Json(new { status = "fail", msg = "请求参数错误" });
            }
        }


        /// <summary>
        /// 修改球队
        /// </summary>
        /// <param name="football"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateTeam(ReqFB.Team football)
        {
            if (ModelState.IsValid)
            {
                var team = await db.FT_Team.FindAsync(football.Id);
                var pTeam = await db.FT_Team.FindAsync(football.PId);

                if (team == null || pTeam == null)
                {
                    return Json(new { status = "fail", msg = "Id非法" });
                }
                team.Id = (Guid)football.Id;
                team.PId = football.PId;
                team.Name = football.Name;
                team.EName = football.EName;
                team.Flag = football.Flag;
                team.Status = football.Status;
                team.OrderNum = football.OrderNum;
                
                db.Entry(team).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "修改成功" });
                }
                catch (Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "修改失败" });
                }

            }
            else
            {
                return Json(new { status = "fail", msg = "请求参数错误" });
            }
        }

        /// <summary>
        /// 禁用、启用、删除球队
        /// </summary>
        /// <param name="updateStatus"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateTeamStatus(Models.Request.Common.UpdateStatus updateStatus)
        {
            if (ModelState.IsValid)
            {

                var team = await db.FT_Team.FindAsync(updateStatus.Id);

                if (team == null)
                {
                    return Json(new { status = "fail", msg = "Id非法" });
                }

                team.Status = updateStatus.Status;
                //禁用，删除下属所有球队Id
                if (updateStatus.Status != Models.Config.Status.normal)
                {
                    //获得下属所有球队Id,不包含已删除的
                    var teamIds = Factory.TeamFactory.GetBelongTeams(updateStatus.Id, Models.Config.Status.deleted, db);
                    if(teamIds.Any())
                    {
                        foreach (var belong in db.FT_Team.Where(t => teamIds.Contains(t.Id)))
                        {
                            belong.Status = updateStatus.Status;
                            db.Entry(belong).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                   
                }
                db.Entry(team).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "修改成功" });
                }
                catch (Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "修改失败" });
                }
            }
            else
            {
                return Json(new { status = "fail", msg = "请求参数错误" });
            }
        }

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetTeamDetail(Guid id)
        {
            var team = await db.FT_Team.FindAsync(id);
            if (team == null || team.Status == Models.Config.Status.deleted || team.Flag != Models.Config.FTeamFlag.team)
            {
                return Json(new { status = "fail", msg = "查询为空" });
            }
            var results = db.FT_Team.Where(t => t.Id == id).Select(t => new {
                t.Id,
                t.Name,
                t.EName,
                t.Status,
                //当前球队比赛情况
                Match = db.FT_Match.Where(m => m.HomeTeam == id || m.GuestTeam == id).Select(m => new { m.HomeScore, m.GuestScore, HomeTeam = m.FT_Team_Home.Name,GuestTeam = m.FT_Team_Guest.Name })
            });
            return Json(new { status = "success", msg = "查询成功", content = results });
        }

        /// <summary>
        /// 递归生成球队树结构（辅助方法）
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="teams"></param>
        /// <returns></returns>
        private ResFB.FootballTree TeamTreeHelper(Guid pId, List<ResFB.Team> teams)
        {
            if (teams == null || teams.Count() == 0)
            {
                return null;
            }
            var team = teams.Where(m => m.Id == pId).First();
            var children = teams.Where(t => t.PId == pId).OrderBy(t => t.OrderNum).ToList();
            teams.Remove(team);
            var child = new ResFB.FootballTree
            {
                Id = team.Id,
                Name = team.Name,
            };
            if (children.Any())
            {
                child.Children = new List<ResFB.FootballTree>();
                foreach (var item in children)
                {
                    child.Children.Add(TeamTreeHelper(item.Id, teams));
                }
            }
            return child;
        }
    }
}
