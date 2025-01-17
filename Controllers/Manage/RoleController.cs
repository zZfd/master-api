﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ReqManage = WebApi.Models.Request.Manage;
using ResManage = WebApi.Models.Response.Manage;

namespace WebApi.Controllers.Manage
{

    public class RoleController : ApiController
    {
        private readonly DataBase.DB db = new DataBase.DB();
        private const string TOKEN = "ZFDYES";
        private readonly Guid SUPERMember = Guid.Parse("00000000-0000-0001-0000-000000000000");

        [HttpGet]
        public IHttpActionResult ListRoles()
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
            var roles = db.MemRole.Where(mr => mr.Member == userId && mr.Roles.Status != Models.Config.Status.deleted).OrderBy(mr => mr.Roles.Org).Select(mr => new
            {
                mr.Roles.Id,
                mr.Roles.PId,
                mr.Roles.Name,
                mr.Roles.Icon,
                mr.Roles.Code,
                mr.Roles.Org,
                OrgName = db.Orgs.FirstOrDefault(o => o.Id == mr.Roles.Org).Name,
                mr.Roles.Status,
                mr.Roles.OrderNum,
                Menus = db.RoleMenu.Where(rm => rm.Role == mr.Roles.Id && rm.Menus.Status == Models.Config.Status.normal).Select(rm => rm.Menu),
                MemberCount = db.MemRole.Where(mr2 => mr2.Role == mr.Roles.Id && mr2.Members.Status == Models.Config.Status.normal).Count(),
            });
            return Json(new { status = "success", msg = "获取成功", content = roles });
        }

        /// <summary>
        /// 获取角色树列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetRoleTree()
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);

            try
            {
                var orgs = Factory.OrgFactory.GetPowerOrgs(userId,Models.Config.Status.normal,db);
                if(orgs.Count() == 0)
                {
                    return Json(new { status ="fail",msg="部门为空"});
                }
                var orgRoles = db.Roles.Where(r => r.Status == Models.Config.Status.normal && orgs.Contains(r.Org)).OrderBy(r=>r.OrderNum).Select(o => new ResManage.Role
                {
                    Id = o.Id,
                    PId = o.PId,
                    Name = o.Name,
                    OrderNum = o.OrderNum,
                }).ToList();
                var memberRoles = db.MemRole.Where(mr => mr.Roles.Status == Models.Config.Status.normal && mr.Member == userId).OrderBy(mr => mr.Roles.OrderNum).Select(mr => new ResManage.Role
                {
                    Id = mr.Roles.Id,
                    PId = mr.Roles.PId,
                    Name = mr.Roles.Name,
                    OrderNum = mr.Roles.OrderNum,
                }).ToList();
                var roleIds = memberRoles.Select(r => r.Id).ToList();
                List<ResManage.RoleTree> roleTrees = new List<ResManage.RoleTree>();
                foreach (Guid role in roleIds)
                {
                    var rolesTemp = new List<ResManage.Role>();
                    rolesTemp.AddRange(orgRoles);
                    roleTrees.Add(RoleTreeHelper(role, rolesTemp));
                }
                return Json(new { status = "success", msg = "获取成功", content = roleTrees });
            }
            catch (Exception ex)
            {
                Helper.LogHelper.WriteErrorLog(ex);
                return Json(new { status = "fail", msg = "获取失败" });
            }
        }


       

       /// <summary>
       /// 懒加载角色树，单次加载一个部门角色树
       /// </summary>
       /// <param name="pId"></param>
       /// <param name="org"></param>
       /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetLazyRole(Guid org, Guid pId)
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);

            List<Guid> powerOrgs = Factory.OrgFactory.GetPowerOrgs(userId, Models.Config.Status.normal,db);
            if (powerOrgs.Count() == 0)
            {
                return Json(new { status = "fail", msg = "暂无部门角色" });
            }
            if (org != Guid.Empty && !powerOrgs.Contains(org) )
            {
                return Json(new { status = "fail", msg = "用户不属于该部门" });
            }
            try
            {
                if (pId == Guid.Empty)
                {
                    Guid firstOrg = org == Guid.Empty ? powerOrgs[0]:org;
                    //第一次加载
                    var roles = db.MemRole.Where(mr => mr.Member == userId && mr.Roles.Org == firstOrg && mr.Roles.Status != Models.Config.Status.deleted ).OrderBy(mr => mr.Roles.OrderNum).Select(mr=>
                      new
                      {
                          mr.Roles.Id,
                          mr.Roles.PId,
                          mr.Roles.Name,
                          mr.Roles.Icon,
                          mr.Roles.Code,
                          mr.Roles.Org,
                          OrgName = db.Orgs.FirstOrDefault(o => o.Id == mr.Roles.Org).Name,
                          mr.Roles.Status,
                          mr.Roles.OrderNum,
                          Menus = db.RoleMenu.Where(rm => rm.Role == mr.Roles.Id && rm.Menus.Status == Models.Config.Status.normal).Select(rm => rm.Menu),
                          MemberCount = db.MemRole.Where(mr2 => mr2.Role == mr.Roles.Id &&mr2.Members.Status == Models.Config.Status.normal).Count(),
                          HasChildren = db.Roles.Where(r2 => r2.PId == mr.Roles.Id && r2.Status != Models.Config.Status.deleted).Any()
                      });
                    return Json(new { status = "success", msg = "获取成功", content = roles });
                }
                else
                {

                    var powerRoles = db.Roles.Where(r => powerOrgs.Contains(r.Org) && r.Status != Models.Config.Status.deleted).Select(r => r.Id);
                    if (!powerRoles.Contains(pId))
                    {
                        //节点非法
                        return Json(new { status = "fail", msg = "节点非法" });
                    }
                    var roles = db.Roles.Where(r => r.PId == pId && r.Status != Models.Config.Status.deleted && r.Org == org).OrderBy(r=>r.OrderNum).Select(r => new
                    {
                        r.Id,
                        r.PId,
                        r.Name,
                        OrgName = db.Orgs.FirstOrDefault(o => o.Id == r.Org).Name,
                        r.Icon,
                        r.Code,
                        r.Org,
                        r.Status,
                        r.OrderNum,
                        Menus = db.RoleMenu.Where(rm=>rm.Role == r.Id && rm.Menus.Status == Models.Config.Status.normal).Select(rm=>rm.Menu),
                        MemberCount = db.MemRole.Where(mr2 => mr2.Role == r.Id && mr2.Members.Status == Models.Config.Status.normal).Count(),
                        HasChildren = db.Roles.Where(r2 => r2.PId == r.Id && r2.Status != Models.Config.Status.deleted).Any()
                    });
                    return Json(new { status = "success", msg = "获取成功", content = roles });
                }
            }
            catch (Exception ex)
            {
                Helper.LogHelper.WriteErrorLog(ex);
                return Json(new { status = "fail", msg = "服务器内部错误" });
            }
        }

        /// <summary>
        /// 添加角色（包括权限）
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SaveRole(ReqManage.Role role)
        {
            if (ModelState.IsValid)
            {
                if (role.Status > Models.Config.Status.forbidden || role.Status < Models.Config.Status.deleted)
                {
                    return Json(new { status = "fail", msg = "状态错误" });
                }
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);

               
                //根据用户角色查找所有的menu
                var menuIds = (from mr in db.MemRole
                               where mr.Member == userId
                               join rm in db.RoleMenu
                               on mr.Role equals rm.Role
                               where rm.Menus.Status == Models.Config.Status.normal
                               select rm.Menu);
                if(menuIds.Intersect(role.Menus).Count() != role.Menus.Count())
                {
                    //菜单非法
                    return Json(new { status = "fail", msg = "菜单非法" });
                }
                List<Guid> powerOrgs = Factory.OrgFactory.GetPowerOrgs(userId, Models.Config.Status.normal, db);
                
                if (powerOrgs.Count() == 0)
                {
                    return Json(new { status = "fail", msg = "暂无部门角色" });
                }
                if (!powerOrgs.Contains(role.Org))
                {
                    //部门非法
                    return Json(new { status = "fail", msg = "部门非法" });
                }
                var roles = db.Roles.Where(r => powerOrgs.Contains(r.Org) && r.Status == Models.Config.Status.normal).Select(r=>r.Id);
                if (!roles.Contains(role.PId))
                {
                    //节点非法
                    return Json(new { status = "fail", msg = "节点非法" });
                }
                DataBase.Roles roleDB = new DataBase.Roles
                {
                    Id = Guid.NewGuid(),
                    PId = role.PId,
                    Name = role.Name,
                    Org = role.Org,
                    Code = role.Code,
                    Icon = role.Icon,
                    Status = role.Status,
                    OrderNum = role.OrderNum,
                    RoleMenu = new List<DataBase.RoleMenu>()
                };
                db.Entry(roleDB).State = System.Data.Entity.EntityState.Added;
                foreach (Guid menu in role.Menus)
                {
                    roleDB.RoleMenu.Add(new DataBase.RoleMenu { Role = roleDB.Id, Menu = menu });
                }

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
        /// 修改角色（包括权限）
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateRole(ReqManage.Role role)
        {
            if (ModelState.IsValid)
            {
               
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);

                
                //根据用户角色查找所有的menu
                var menuIds = (from mr in db.MemRole
                               where mr.Member == userId
                               join rm in db.RoleMenu
                               on mr.Role equals rm.Role
                               where rm.Menus.Status == Models.Config.Status.normal
                               select rm.Menu);
                if (menuIds.Intersect(role.Menus).Count() != role.Menus.Count())
                {
                    //菜单非法
                    return Json(new { status = "fail", msg = "菜单非法" });
                }
                List<Guid> powerOrgs = Factory.OrgFactory.GetPowerOrgs(userId, Models.Config.Status.normal, db);

                if (powerOrgs.Count() == 0)
                {
                    return Json(new { status = "fail", msg = "暂无部门角色" });
                }
              
                if (!powerOrgs.Contains(role.Org))
                {
                    //部门非法
                    return Json(new { status = "fail", msg = "部门非法" });
                }
                var roles = db.Roles.Where(r => powerOrgs.Contains(r.Org) && r.Status == Models.Config.Status.normal).Select(r => r.Id);
                if (!roles.Contains(role.PId) || !roles.Contains((Guid)role.Id) || role.Id == Guid.Parse("00000000-0000-0000-0001-000000000000"))
                {
                    //节点非法
                    return Json(new { status = "fail", msg = "节点非法" });
                }
                DataBase.Roles roleDB = await db.Roles.FindAsync(role.Id);

                roleDB.Id = (Guid)role.Id;
                roleDB.PId = role.PId;
                roleDB.Name = role.Name;
                roleDB.Org = role.Org;
                roleDB.Code = role.Code;
                roleDB.Icon = role.Icon;
                roleDB.Status = role.Status;
                roleDB.OrderNum = role.OrderNum;
                //删除已有的多对多关系
                db.RoleMenu.RemoveRange(db.RoleMenu.Where(rm=>rm.Role == roleDB.Id).Select(rm=>rm));
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
        /// 禁用、启用、删除部门
        /// 删除部门时，情况部门下所有用户，角色，菜单
        /// </summary>
        /// <param name="roleStatus"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateRoleStatus(ReqManage.RoleStatus roleStatus)
        {
            if (ModelState.IsValid)
            {
               
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);

                DataBase.Roles roleDB = await db.Roles.FindAsync(roleStatus.Id);

                List<Guid> powerOrgs = Factory.OrgFactory.GetPowerOrgs(userId, Models.Config.Status.normal, db);

                if (powerOrgs.Count() == 0)
                {
                    return Json(new { status = "fail", msg = "暂无部门角色" });
                }

                var roles = db.Roles.Where(r => powerOrgs.Contains(r.Org) && r.Status != Models.Config.Status.deleted).Select(r => r.Id);
                if (!roles.Contains(roleStatus.Id))
                {
                    //节点非法
                    return Json(new { status = "fail", msg = "节点非法" });
                }

                roleDB.Status = roleStatus.Status;
                //禁用，删除下级角色
                if (roleStatus.Status != Models.Config.Status.normal)
                {
                    foreach (var role in db.Roles.Where(m => m.PId == roleStatus.Id && m.Status != Models.Config.Status.deleted))
                    {
                        role.Status = roleStatus.Status;
                        db.Entry(role).State = System.Data.Entity.EntityState.Modified;
                    }
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
        /// 递归生成组织树结构（辅助方法）
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        private ResManage.RoleTree RoleTreeHelper(Guid pId, List<ResManage.Role> roles)
        {
            if (roles == null || roles.Count() == 0)
            {
                return null;
            }
            var role = roles.Where(m => m.Id == pId).First();
            var children = roles.Where(o => o.PId == pId).OrderBy(o => o.OrderNum).ToList();
            roles.Remove(role);
            var child = new ResManage.RoleTree
            {
                Id = role.Id,
                Name = role.Name,

            };
            if (children.Any())
            {
                child.Children = new List<ResManage.RoleTree>();
                foreach (var item in children)
                {
                    child.Children.Add(RoleTreeHelper(item.Id, roles));
                }
            }
            return child;
        }
       
    }
}
