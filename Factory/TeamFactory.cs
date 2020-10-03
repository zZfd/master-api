using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ResFB = WebApi.Models.Response.Web.Football;

namespace WebApi.Factory
{
    public static class TeamFactory
    {
        /// <summary>
        /// 获取下层所有球队Id
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="status"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static List<Guid> GetTeams(Guid pId,short status, DataBase.DB db)
        {
            var team = db.FT_Team.Where(t=>t.Id == pId && t.Status != Models.Config.Status.deleted && t.Flag != Models.Config.FTeamFlag.team).Select(t => new ResFB.Football { Id = t.Id,PId = t.PId,OrderNum = t.OrderNum}).ToList();
            if(team == null)
            {
                return new List<Guid>();
            }
            List<Guid> teamIds = new List<Guid>();
            TeamHelper(team, teamIds, status, db);
            return teamIds;
        }
        /// <summary>
        /// 递归向下查找下属球队返回Ids
        /// 包含自身
        /// </summary>
        /// <param name="source"></param>
        /// <param name="result"></param>
        /// <param name="status"></param>
        /// <param name="db"></param>
        private static void TeamHelper(List<ResFB.Football> source, List<Guid> result, short status, DataBase.DB db)
        {
            result.AddRange(source.Select(s => s.Id));
            if (status == Models.Config.Status.normal)
            {
                //只包含正常
                foreach (var team in source)
                {
                    var s = db.FT_Team.Where(t => t.PId == team.Id && t.Status == status && t.Flag == Models.Config.FTeamFlag.team).OrderBy(t=>t.OrderNum).Select(o => new ResFB.Football
                    {
                        Id = o.Id,
                        PId = o.PId
                    }).ToList();
                    TeamHelper(s, result, status, db);
                }
            }
            else
            {
                //不包含已删除
                foreach (var team in source)
                {
                    var s = db.FT_Team.Where(t => t.PId == team.Id && t.Status != Models.Config.Status.deleted && t.Flag == Models.Config.FTeamFlag.team).OrderBy(t => t.OrderNum).Select(o => new ResFB.Football
                    {
                        Id = o.Id,
                        PId = o.PId
                    }).ToList();
                    TeamHelper(s, result, status, db);
                }
            }
        }
    }
}