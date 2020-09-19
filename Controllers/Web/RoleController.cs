using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using RoleReq = WebApi.Models.Request.Web.Role;
using RoleRes = WebApi.Models.Response.Web.Role;

namespace WebApi.Controllers.Web
{
    public class RoleController : ApiController
    {
        private readonly DataBase.DB db = new DataBase.DB();


        /// <summary>
        /// 获取角色树列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetOrgTree()
        {
            Guid userId = Guid.Parse(HttpContext.Current.Request.Headers["sessionId"]);
            try
            {
                var roleIds = db.MemOrg.Where(mo => mo.Member == userId).Select(mo => mo.Org).ToList();
                var roles = db.Role.Where(r => r.Status == Models.Config.Status.normal && (roleIds.Contains(r.Id) || roleIds.Contains(r.PId))).OrderBy(r=>r.OrderNum).Select(o => new RoleRes.Role
                {
                    Id = o.Id,
                    PId = o.PId,
                    Name = o.Name,
                    OrderNum = o.OrderNum,
                });
                List<RoleRes.RoleTree> orgTrees = new List<RoleRes.RoleTree>();
                foreach (Guid role in roleIds)
                {
                    var rolesTemp = new List<RoleRes.Role>();
                    rolesTemp.AddRange(roles);
                    orgTrees.Add(RoleTreeHelper(role, rolesTemp));
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
        /// <param name="roles"></param>
        /// <returns></returns>
        private RoleRes.RoleTree RoleTreeHelper(Guid pId, List<RoleRes.Role> roles)
        {
            if (roles == null || roles.Count() == 0)
            {
                return null;
            }
            var role = roles.Where(m => m.Id == pId).First();
            var children = roles.Where(o => o.PId == pId).OrderBy(o => o.OrderNum).ToList();
            roles.Remove(role);
            var child = new RoleRes.RoleTree
            {
                Id = role.Id,
                Name = role.Name,

            };
            if (children.Any())
            {
                child.Children = new List<RoleRes.RoleTree>();
                foreach (var item in children)
                {
                    child.Children.Add(RoleTreeHelper(item.Id, roles));
                }
            }
            return child;
        }

        /// <summary>
        /// 懒加载角色树
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetLazyRole(Guid pId)
        {
            Guid userId = Guid.Parse(HttpContext.Current.Request.Headers["sessionId"]);
            try
            {
                if (pId == Guid.Empty)
                {
                    //第一次加载
                    var roles = db.MemRole.Where(mr => mr.Member == userId).Join(db.Role.Where(r => r.Status != Models.Config.Status.deleted).OrderBy(r => r.OrderNum), mr => mr.Role, r => r.Id, (mr, r)=>
                      new
                      {
                          r.Id,
                          r.PId,
                          r.Name,
                          r.Icon,
                          r.Code,
                          r.Org,
                          r.Status,
                          r.OrderNum,
                          MemberCount = db.MemRole.Where(mr2 => mr2.Role == r.Id).Select(mr2 => mr.Member).Intersect(db.Member.Where(mem => mem.Status == Models.Config.Status.normal).Select(mem => mem.Id)).Count(),
                          MenuCount = db.RoleMenu.Where(rm => rm.Role == r.Id).Select(om => om.Menu).Intersect(db.Menu.Where(menu => menu.Status == Models.Config.Status.normal).Select(menu => menu.Id)).Count(),
                          HasChildren = db.Role.Where(r2 => r2.PId == r.Id && r2.Status != Models.Config.Status.deleted).Any()
                      });
                    return Json(new { status = "success", msg = "获取成功", content = roles });
                }
                else
                {
                    var powerRole = db.MemRole.Where(mr => mr.Member == userId).Select(mr => mr.Role);
                    var powerRoles = powerRole.Concat(db.Role.Where(r => powerRole.Contains(r.PId) && r.Status != Models.Config.Status.deleted).Select(r => r.Id));
                    if (!powerRoles.Contains(pId))
                    {
                        return Json(new { status = "fail", msg = "获取失败" });
                    }
                    var roles = db.Role.Where(r => r.Id == pId && r.Status != Models.Config.Status.deleted).Select(r => new
                    {
                        r.Id,
                        r.PId,
                        r.Name,
                        r.Icon,
                        r.Code,
                        r.Org,
                        r.Status,
                        r.OrderNum,
                        MemberCount = db.MemRole.Where(mr2 => mr2.Role == r.Id).Select(mr2 => mr2.Member).Intersect(db.Member.Where(mem => mem.Status == Models.Config.Status.normal).Select(mem => mem.Id)).Count(),
                        MenuCount = db.RoleMenu.Where(rm => rm.Role == r.Id).Select(om => om.Menu).Intersect(db.Menu.Where(menu => menu.Status == Models.Config.Status.normal).Select(menu => menu.Id)).Count(),
                        HasChildren = db.Role.Where(r2 => r2.PId == r.Id && r2.Status != Models.Config.Status.deleted).Any()
                    });
                    return Json(new { status = "success", msg = "获取成功", content = roles });
                }
            }
            catch (Exception ex)
            {
                Helper.LogHelper.WriteErrorLog(ex);
                return Json(new { status = "fail", msg = "获取失败" });
            }
        }

        /// <summary>
        /// 添加角色（包括权限）
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SaveOrg(RoleReq.Role role)
        {
            if (ModelState.IsValid)
            {
                var pRole =  await db.Role.FindAsync(role.PId);
                if(pRole == null)
                {
                    //父节点非法
                    return Json(new { status = "fail", msg = "保存失败" });
                }
                if(db.Menu.Where(m=>m.Status == Models.Config.Status.normal).Select(m => m.Id).Intersect(role.Menus).Count() != role.Menus.Count()){
                    //菜单非法
                    return Json(new { status = "fail", msg = "保存失败" });
                }
                DataBase.Role roleDB = new DataBase.Role
                {
                    Id = Guid.NewGuid(),
                    PId = role.PId,
                    Name = role.Name,
                    Code = role.Code,
                    Icon = role.Icon,
                    Status = role.Status,
                    OrderNum = role.OrderNum,
                    RoleMenu = new List<DataBase.RoleMenu>()
                };
                foreach (Guid menu in role.Menus)
                {
                    roleDB.RoleMenu.Add(new DataBase.RoleMenu { Role = roleDB.Id, Menu = menu });
                }
                db.Entry(roleDB).State = System.Data.Entity.EntityState.Added;
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
        /// 修改角色（包括权限）
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateOrg(RoleReq.Role role)
        {
            if (ModelState.IsValid)
            {
                DataBase.Role roleDB = await db.Role.FindAsync(role.Id);
                if (roleDB == null || roleDB.Id == Guid.Parse("00000000-0000-0000-0001-000000000000"))
                {
                    //节点非法
                    //根部门不允许修改
                    return Json(new { status = "fail", msg = "请求参数错误" });
                }
                var pRole = await db.Role.FindAsync(role.PId);
                if (pRole == null)
                {
                    //父节点非法
                    return Json(new { status = "fail", msg = "保存失败" });
                }
                if (db.Menu.Where(m => m.Status == Models.Config.Status.normal).Select(m => m.Id).Intersect(role.Menus).Count() != role.Menus.Count())
                {
                    //菜单非法
                    return Json(new { status = "fail", msg = "保存失败" });
                }
                roleDB.Id = role.Id;
                roleDB.PId = role.PId;
                roleDB.Name = role.Name;
                roleDB.Org = role.Org;
                roleDB.Code = role.Code;
                roleDB.Icon = role.Icon;
                roleDB.Status = role.Status;
                roleDB.OrderNum = role.OrderNum;
                roleDB.RoleMenu = new List<DataBase.RoleMenu>();
                foreach (Guid menu in role.Menus)
                {
                    roleDB.RoleMenu.Add(new DataBase.RoleMenu { Role = roleDB.Id, Menu = menu });
                }
                db.Entry(roleDB).State = System.Data.Entity.EntityState.Modified;
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
        /// <param name="roleStatus"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateOrgStatus(RoleReq.RoleStatus roleStatus)
        {
            if (ModelState.IsValid)
            {
                DataBase.Role roleDB = await db.Role.FindAsync(roleStatus.Id);
                if (roleDB == null || roleStatus.Id == Guid.Parse("00000000-0000-0000-0001-000000000000"))
                {
                    //根部门不许操作
                    return Json(new { status = "fail", msg = "请求参数错误" });
                }
                roleDB.Status = roleStatus.Status;
                if (roleStatus.Status == -1)
                {
                    //删除
                    roleDB.RoleMenu = new List<DataBase.RoleMenu>();
                    roleDB.MemRole = new List<DataBase.MemRole>();
                }
                db.Entry(roleDB).State = System.Data.Entity.EntityState.Modified;
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
