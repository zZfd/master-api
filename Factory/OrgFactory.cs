using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrgRes = WebApi.Models.Response.Web.Org;


namespace WebApi.Factory
{
    public static class OrgFactory
    {
        /// <summary>
        /// 获取用户的权限部门
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static List<Guid> GetPowerOrgs(Guid userId, short status,DataBase.DB db)
        {
            var powerOrg = db.MemOrg.Where(mo => mo.Member == userId).Select(mo => new OrgRes.Org { Id = mo.Org, PId = mo.Orgs.PId }).ToList();
            List<Guid> powerOrgs = new List<Guid>();
            PowerOrgsHelper(powerOrg, powerOrgs, status, db);
            return powerOrgs;
        }
        /// <summary>
        /// 递归向下查找权限部门
        /// </summary>
        /// <param name="source"></param>
        /// <param name="result"></param>
        /// <param name="status"></param>
        /// <param name="db"></param>
        private static void PowerOrgsHelper(List<OrgRes.Org> source, List<Guid> result, short status,DataBase.DB db)
        {
            result.AddRange(source.Select(s => s.Id));
            if (status == Models.Config.Status.normal)
            {
                //只包含正常
                foreach (var org in source)
                {
                    var s = db.Orgs.Where(o => o.PId == org.Id && o.Status == status).Select(o => new OrgRes.Org
                    {
                        Id = o.Id,
                        PId = o.PId
                    }).ToList();
                    PowerOrgsHelper(s, result, status, db);
                }
            }
            else
            {
                //不包含已删除
                foreach (var org in source)
                {
                    var s = db.Orgs.Where(o => o.PId == org.Id && o.Status != Models.Config.Status.deleted).Select(o => new OrgRes.Org
                    {
                        Id = o.Id,
                        PId = o.PId
                    }).ToList();
                    PowerOrgsHelper(s, result, status, db);
                }
            }
        }
    }
}