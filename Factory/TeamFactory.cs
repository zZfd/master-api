using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ResFB = WebApi.Models.Response.Football;

namespace WebApi.Factory
{
    public static class TeamFactory
    {
        /// <summary>
        /// 获取联赛
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static List<Models.Response.Common.IdEName> GetLeagues(DataBase.DB db)
        {
            var leagues = db.FT_Team.Where(t => t.Flag == Models.Config.FTeamFlag.league && t.Status == Models.Config.Status.normal)
                .Select(c => new Models.Response.Common.IdEName
                {
                    Id = c.Id,
                    Name = c.Name,
                    EName = c.EName
                }).ToList();
            return leagues;
        }

        /// <summary>
        /// 获取国家及下属联赛
        /// Name EName
        /// </summary>
        /// <returns></returns>
        public static List<ResFB.CountryLeagues> GetCountryLeagues(DataBase.DB db)
        {
            var countryLeagues = db.FT_Team.Where(t => t.Flag == Models.Config.FTeamFlag.country && t.Status == Models.Config.Status.normal)
                .OrderBy(t=>t.OrderNum)
                .Select(c => new ResFB.CountryLeagues
                {
                    Id = c.Id,
                    Name = c.Name,
                    EName = c.EName,
                    Leagues = db.FT_Team.Where(t => t.Flag == Models.Config.FTeamFlag.league && t.Status == Models.Config.Status.normal && t.PId == c.Id)
                .Select(t => new Models.Response.Common.IdEName
                {
                    Id = t.Id,
                    Name = t.Name,
                    EName = t.EName
                }).ToList()
        }).ToList();
            return countryLeagues;
        }

        /// <summary>
        /// 获取球队
        /// Name EName
        /// </summary>
        /// <returns></returns>
        public static List<Models.Response.Common.IdEName> GetTeams(DataBase.DB db)
        {
            var teams = db.FT_Team.Where(t => t.Flag == Models.Config.FTeamFlag.team && t.Status == Models.Config.Status.normal)
                .Select(c => new Models.Response.Common.IdEName
                {
                    Id = c.Id,
                    Name = c.Name,
                    EName = c.EName
                }).ToList();
            return teams;
        }

        /// <summary>
        /// 获取下层所有球队Id
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="status"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static List<Guid> GetBelongTeams(Guid pId, short status, DataBase.DB db)
        {
            var team = db.FT_Team.Where(t => t.Id == pId && t.Status != Models.Config.Status.deleted && t.Flag != Models.Config.FTeamFlag.team).Select(t => 
            new ResFB.Team { Id = t.Id, PId = t.PId, OrderNum = t.OrderNum,Flag = t.Flag }).ToList();
            if (team == null)
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
        private static void TeamHelper(List<ResFB.Team> source, List<Guid> result, short status, DataBase.DB db)
        {
            result.AddRange(source.Where(s=>s.Flag == Models.Config.FTeamFlag.team).Select(s => s.Id));
            if (status == Models.Config.Status.normal)
            {
                //只包含正常
                foreach (var team in source)
                {
                    var s = db.FT_Team.Where(t => t.PId == team.Id && t.Status == status && t.Flag == Models.Config.FTeamFlag.team).OrderBy(t => t.OrderNum).Select(o => new ResFB.Team
                    {
                        Id = o.Id,
                        PId = o.PId,
                        Flag = o.Flag
                    }).ToList();
                    TeamHelper(s, result, status, db);
                }
            }
            else
            {
                //不包含已删除
                foreach (var team in source)
                {
                    var s = db.FT_Team.Where(t => t.PId == team.Id && t.Status != Models.Config.Status.deleted && t.Flag == Models.Config.FTeamFlag.team).OrderBy(t => t.OrderNum).Select(o => new ResFB.Team
                    {
                        Id = o.Id,
                        PId = o.PId
                    }).ToList();
                    TeamHelper(s, result, status, db);
                }
            }
        }

        /// <summary>
        /// 获取下层所有内容 包括联赛及球队
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="status"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static List<Guid> GetBelongs(Guid pId, short status, DataBase.DB db)
        {
            var team = db.FT_Team.Where(t => t.Id == pId && t.Status != Models.Config.Status.deleted && t.Flag != Models.Config.FTeamFlag.team).Select(t =>
            new ResFB.Team { Id = t.Id, PId = t.PId, OrderNum = t.OrderNum, Flag = t.Flag }).ToList();
            if (team == null)
            {
                return new List<Guid>();
            }
            List<Guid> teamIds = new List<Guid>();
            BelongsHelper(team, teamIds, status, db);
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
        private static void BelongsHelper(List<ResFB.Team> source, List<Guid> result, short status, DataBase.DB db)
        {
            result.AddRange(source.Select(s => s.Id));
            if (status == Models.Config.Status.normal)
            {
                //只包含正常
                foreach (var team in source)
                {
                    var s = db.FT_Team.Where(t => t.PId == team.Id && t.Status == status).OrderBy(t => t.OrderNum).Select(o => new ResFB.Team
                    {
                        Id = o.Id,
                        PId = o.PId,
                        Flag = o.Flag
                    }).ToList();
                    TeamHelper(s, result, status, db);
                }
            }
            else
            {
                //不包含已删除
                foreach (var team in source)
                {
                    var s = db.FT_Team.Where(t => t.PId == team.Id && t.Status != Models.Config.Status.deleted).OrderBy(t => t.OrderNum).Select(o => new ResFB.Team
                    {
                        Id = o.Id,
                        PId = o.PId
                    }).ToList();
                    BelongsHelper(s, result, status, db);
                }
            }
        }
    }
}