using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using OrgReq = WebApi.Models.Request.Web.Org;
using OrgRes = WebApi.Models.Response.Web.Org;

namespace WebApi.Controllers.Web
{
    public class OrgController : ApiController
    {
        private readonly DataBase.DB db = new DataBase.DB();
        private const string TOKEN = "ZFDYES";
        /// <summary>
        /// 获取部门树列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetOrgTree()
        {
            Guid userId = Guid.Parse(HttpContext.Current.Request.Headers["sessionId"]);
            try
            {
                var orgIds = db.MemOrg.Where(mo => mo.Member == userId).Select(mo => mo.Org).ToList();
                var orgs = db.Orgs.Where(o => o.Status == Models.Config.Status.normal && (orgIds.Contains(o.Id) || orgIds.Contains(o.PId))).OrderBy(o => o.OrderNum).Select(o => new OrgRes.Org
                {
                    Id = o.Id,
                    PId = o.PId,
                    Name = o.Name,
                    OrderNum = o.OrderNum,
                });
                List<OrgRes.OrgTree> orgTrees = new List<OrgRes.OrgTree>();
                foreach (Guid org in orgIds)
                {
                    var orgsTemp = new List<OrgRes.Org>();
                    orgsTemp.AddRange(orgs);
                    orgTrees.Add(OrgTreeHelper(org, orgsTemp));
                }
                return Json(new { status = "success", msg = "获取成功", content = orgTrees });
            }
            catch (Exception ex)
            {
                Helper.LogHelper.WriteErrorLog(ex);
                return Json(new { status = "fail", msg = "获取失败" });
            }
        }


        /// <summary>
        /// 递归生成组织树结构（辅助方法）
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="orgs"></param>
        /// <returns></returns>
        private OrgRes.OrgTree OrgTreeHelper(Guid pId, List<OrgRes.Org> orgs)
        {
            if (orgs == null || orgs.Count() == 0)
            {
                return null;
            }
            var org = orgs.Where(m => m.Id == pId).First();
            var children = orgs.Where(o => o.PId == pId).OrderBy(o => o.OrderNum).ToList();
            orgs.Remove(org);
            var child = new OrgRes.OrgTree
            {
                Id = org.Id,
                Name = org.Name,

            };
            if (children.Any())
            {
                child.Children = new List<OrgRes.OrgTree>();
                foreach (var item in children)
                {
                    child.Children.Add(OrgTreeHelper(item.Id, orgs));
                }
            }
            return child;
        }

        /// <summary>
        /// 懒加载部门树
        /// 当Pid为空时，返回所有根部门
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetLazyOrg(Guid pId)
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
            if (userId == Guid.Empty)
            {
                return Json(new { status = "fail", msg = "请求错误" });
            }
            var member = await db.Members.FindAsync(userId);
            if (member == null || member.Status != Models.Config.Status.normal)
            {
                return Json(new { status = "fail", msg = "用户不存在或已被禁用" });
            }
            try
            {
                if (pId == Guid.Empty)
                {
                    //第一次加载
                    var orgs = db.MemOrg.Where(mo => mo.Member == userId).Join(db.Orgs.Where(o => o.Status != Models.Config.Status.deleted).OrderBy(o => o.OrderNum), mo => mo.Org, o => o.Id, (mo, o) => new
                    {
                        o.Id,
                        o.PId,
                        o.Name,
                        o.Icon,
                        o.Code,
                        o.Status,
                        o.OrderNum,
                        MemberCount = db.MemOrg.Where(mo2 => mo2.Org == o.Id).Select(mo2 => mo2.Member).Intersect(db.Members.Where(mem => mem.Status == Models.Config.Status.normal).Select(mem => mem.Id)).Count(),
                        MenuCount = db.OrgMenu.Where(om => om.Org == o.Id).Select(om => om.Menu).Intersect(db.Menus.Where(menu => menu.Status == Models.Config.Status.normal).Select(menu => menu.Id)).Count(),
                        RoleCount = db.Roles.Where(role => role.Org == o.Id && role.Status == Models.Config.Status.normal).Count(),
                        HasChildren = db.Orgs.Where(org => org.PId == o.Id && org.Status != Models.Config.Status.deleted).Any()
                    });
                    return Json(new { status = "success", msg = "获取成功", content = orgs });
                }
                else
                {
                    var powerOrg = db.MemOrg.Where(mo => mo.Member == userId).Select(mo => mo.Org);
                    var powerOrgs = powerOrg.Concat(db.Orgs.Where(o => powerOrg.Contains(o.PId) && o.Status != Models.Config.Status.deleted).Select(o => o.Id));
                    if (!powerOrgs.Contains(pId))
                    {
                        return Json(new { status = "fail", msg = "所选部门不存在或无权限" });
                    }
                    var orgs = db.Orgs.Where(o => o.Id == pId && o.Status != Models.Config.Status.deleted).Select(o => new
                    {
                        o.Id,
                        o.PId,
                        o.Name,
                        o.Icon,
                        o.Code,
                        o.Status,
                        o.OrderNum,
                        //只统计正常的
                        MemberCount = db.MemOrg.Where(mo2 => mo2.Org == o.Id).Select(mo2 => mo2.Member).Intersect(db.Members.Where(mem => mem.Status == Models.Config.Status.normal).Select(mem => mem.Id)).Count(),
                        MenuCount = db.OrgMenu.Where(om => om.Org == o.Id).Select(om => om.Menu).Intersect(db.Menus.Where(menu => menu.Status == Models.Config.Status.normal).Select(menu => menu.Id)).Count(),
                        RoleCount = db.Roles.Where(role => role.Org == o.Id && role.Status == Models.Config.Status.normal).Count(),
                        HasChildren = db.Orgs.Where(org => org.PId == o.Id && org.Status != Models.Config.Status.deleted).Any()
                    });
                    return Json(new { status = "success", msg = "获取成功", content = orgs });
                }
            }
            catch (Exception ex)
            {
                Helper.LogHelper.WriteErrorLog(ex);
                return Json(new { status = "fail", msg = "服务器内部错误" });
            }
        }

        /// <summary>
        /// 添加部门（包括权限）
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SaveOrg(OrgReq.Org org)
        {
            if (ModelState.IsValid)
            {
                var pOrg = await db.Roles.FindAsync(org.PId);
                if (pOrg == null)
                {
                    //父节点非法
                    return Json(new { status = "fail", msg = "保存失败" });
                }
                if (db.Menus.Where(m => m.Status == Models.Config.Status.normal).Select(m => m.Id).Intersect(org.Menus).Count() != org.Menus.Count())
                {
                    //菜单非法
                    return Json(new { status = "fail", msg = "保存失败" });
                }
                DataBase.Orgs orgDB = new DataBase.Orgs
                {
                    Id = Guid.NewGuid(),
                    PId = org.PId,
                    Name = org.Name,
                    Code = org.Code,
                    Icon = org.Icon,
                    Status = org.Status,
                    OrderNum = org.OrderNum,
                    OrgMenu = new List<DataBase.OrgMenu>()
                };
                foreach (Guid menu in org.Menus)
                {
                    orgDB.OrgMenu.Add(new DataBase.OrgMenu { Org = orgDB.Id, Menu = menu });
                }
                db.Entry(orgDB).State = System.Data.Entity.EntityState.Added;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "保存成功" });
                }
                catch (DbUpdateException ex)
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
        /// 修改部门（包括权限）
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateOrg(OrgReq.Org org)
        {
            if (ModelState.IsValid)
            {
                DataBase.Orgs orgDB = await db.Orgs.FindAsync(org.Id);
                if (orgDB == null || org.Id == Guid.Parse("00000000-0000-0000-0001-000000000000"))
                {
                    //节点非法
                    //根部门不允许修改
                    return Json(new { status = "fail", msg = "请求参数错误" });
                }
                var pOrg = await db.Roles.FindAsync(org.PId);
                if (pOrg == null)
                {
                    //父节点非法
                    return Json(new { status = "fail", msg = "保存失败" });
                }
                if (db.Menus.Where(m => m.Status == Models.Config.Status.normal).Select(m => m.Id).Intersect(org.Menus).Count() != org.Menus.Count())
                {
                    //菜单非法
                    return Json(new { status = "fail", msg = "保存失败" });
                }
                orgDB.Id = org.Id;
                orgDB.PId = org.PId;
                orgDB.Name = org.Name;
                orgDB.Code = org.Code;
                orgDB.Icon = org.Icon;
                orgDB.Status = org.Status;
                orgDB.OrderNum = org.OrderNum;
                orgDB.OrgMenu = new List<DataBase.OrgMenu>();
                foreach (Guid menu in org.Menus)
                {
                    orgDB.OrgMenu.Add(new DataBase.OrgMenu { Org = orgDB.Id, Menu = menu });
                }
                db.Entry(orgDB).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "修改成功" });
                }
                catch (DbUpdateException ex)
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
        /// 禁用、启用、删除部门
        /// 删除部门时，情况部门下所有用户，角色，菜单
        /// </summary>
        /// <param name="orgStatus"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateOrgStatus(OrgReq.OrgStatus orgStatus)
        {
            if (ModelState.IsValid)
            {
                DataBase.Orgs orgDB = await db.Orgs.FindAsync(orgStatus.Id);
                if (orgDB == null || orgStatus.Id == Guid.Parse("00000000-0000-0000-0001-000000000000"))
                {
                    //根部门不许操作
                    return Json(new { status = "fail", msg = "请求参数错误" });
                }
                orgDB.Status = orgStatus.Status;
                if (orgStatus.Status == -1)
                {
                    //删除
                    orgDB.OrgMenu = new List<DataBase.OrgMenu>();
                    orgDB.MemOrg = new List<DataBase.MemOrg>();
                    orgDB.Roles = new List<DataBase.Roles>();
                }
                db.Entry(orgDB).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "修改成功" });
                }
                catch (DbUpdateException ex)
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
    }
}
