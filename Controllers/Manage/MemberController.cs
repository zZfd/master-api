using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using ReqManage = WebApi.Models.Request.Manage;
using WebApi.Models.Response;
using System.Threading.Tasks;
using System.Text;
using System.Web;
using ResManage = WebApi.Models.Response.Manage;

namespace WebApi.Controllers.Manage
{

    public class MemberController : ApiController
    {
        private readonly DataBase.DB db = new DataBase.DB();
        private const string TOKEN = "ZFDYES";

        /// <summary>
        /// 获取RSA公钥
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetPublicKey()
        {
            return Json(new {status ="success",msg="获取成功",content = ConfigurationManager.AppSettings["RSAPublic"].ToString() });
            //return Json(new { status = "success",msg = "获取成功",content= ConfigurationManager.AppSettings["RSAPublic"].ToString() });
        }
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Login(ReqManage.Login member)
        {
            if (ModelState.IsValid)
            {
                //Helper.RSACryptoService rsaCryptoService = new Helper.RSACryptoService(ConfigurationManager.AppSettings["RSAPrivate"].ToString(), ConfigurationManager.AppSettings["RSAPublic"].ToString());
                DataBase.Members mem = db.Members.FirstOrDefault(m => m.Name == member.UserName);
                if (mem == null || mem.Status != Models.Config.Status.normal)
                {
                    return Json(new { code = 20000, msg = "用户不存在或已被禁用" });
                }
                //string password = rsaCryptoService.Decrypt(member.Password);
                if(!Helper.EncryptionHelper.SHA1(member.Password + mem.PasswordSalt, Encoding.UTF8, false).Equals(mem.Password))
                {
                    return Json(new { status = "fail", msg = "用户名密码错误" });
                }
                return Json(new { code = 20000, msg = "登录成功", data = new { token = Helper.EncryptionHelper.CreateToken(mem.Id, mem.PasswordSalt) } });
            }
            else
            {
                return Json(new { code = 20000, msg = "请求参数格式错误" });
            }

        }
        /// <summary>
        /// 获取用户的基础信息 + role + org
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetLoginerInfo(string token)
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(token);
            if (userId == Guid.Empty)
            {
                return Json(new { status = "fail", msg = "请求错误" });
            }
            var member = await db.Members.FindAsync(userId);
            if (member == null || member.Status != Models.Config.Status.normal)
            {
                return Json(new { status = "fail", msg = "用户不存在或已被禁用" });
            }
            var loginInfo = db.Members.Where(m => m.Id == userId).Select(m => new { 
                m.Name,
                m.NickName,
                m.Phone,
                m.Avatar,
                Orgs = db.MemOrg.Where(mo=>mo.Member == userId && mo.Orgs.Status == Models.Config.Status.normal).Select(mo => new { 
                    mo.Orgs.Id,
                    mo.Orgs.Name
                }),
                Roles = db.MemRole.Where(mr=>mr.Member == userId && mr.Roles.Status == Models.Config.Status.normal).Select(mr => new { 
                    mr.Roles.Id,
                    mr.Roles.Name
                })
            }).FirstOrDefault();
            return Json(new { code = 20000, msg = "获取成功", data = loginInfo });
        }
        
        /// <summary>
        /// 添加账户
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> SaveMember(ReqManage.Member member)
        {
            if (ModelState.IsValid)
            {
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
                var powerOrgs = Factory.OrgFactory.GetPowerOrgs(userId, Models.Config.Status.normal, db);
                if (powerOrgs.Intersect(member.Orgs).Count() != member.Orgs.Count())
                {
                    return Json(new { status = "fail", msg = "部门非法" });
                }
                var powerRoles = db.Roles.Where(r => r.Status == Models.Config.Status.normal && powerOrgs.Contains(r.Org)).Select(r => r.Id);
                if (powerRoles.Intersect(member.Roles).Count() != member.Roles.Count())
                {
                    return Json(new { status = "fail", msg = "角色非法" });
                }
                var memberDB = new DataBase.Members
                {
                    Id = Guid.NewGuid(),
                    Name = member.Name,
                    NickName = member.NickName,
                    Phone = member.Phone,
                    Status = member.Status,
                    PasswordSalt = Helper.RandomHelper.GetCodeStr(8),
                    MemOrg = new List<DataBase.MemOrg>(),
                    MemRole = new List<DataBase.MemRole>()
                };
                memberDB.Password = Helper.EncryptionHelper.SHA1("123456" + memberDB.PasswordSalt, Encoding.UTF8, false);
                foreach(Guid org in member.Orgs)
                {
                    memberDB.MemOrg.Add(new DataBase.MemOrg { Member = memberDB.Id, Org = org });
                }
                foreach(Guid role in member.Roles)
                {
                    memberDB.MemRole.Add(new DataBase.MemRole { Member = memberDB.Id, Role = role });
                }
                db.Entry(memberDB).State = System.Data.Entity.EntityState.Added;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "保存成功" });
                }catch(Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "服务器内部错误" });
                }
            }
            else
            {
                return Json(new { status = "fail", msg = "请求参数错误" });
            }
        }

        /// <summary>
        /// 更新账户
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateMember(ReqManage.Member member)
        {
            if (ModelState.IsValid)
            {
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
                var powerOrgs = Factory.OrgFactory.GetPowerOrgs(userId, Models.Config.Status.normal, db);
                var filterMemberIds = db.MemOrg.Where(mo => powerOrgs.Contains(mo.Org)).Select(mo => mo.Member).ToList();
                if(member.Id == null || !filterMemberIds.Contains((Guid)member.Id))
                {
                    return Json(new { status = "fail", msg = "账户不存在或非法" });
                }
                if (powerOrgs.Intersect(member.Orgs).Count() != member.Orgs.Count())
                {
                    return Json(new { status = "fail", msg = "部门非法" });
                }
                var powerRoles = db.Roles.Where(r => r.Status == Models.Config.Status.normal && powerOrgs.Contains(r.Org)).Select(r => r.Id);
                if (powerRoles.Intersect(member.Roles).Count() != member.Roles.Count())
                {
                    return Json(new { status = "fail", msg = "角色非法" });
                }

                var memberDB = await db.Members.FindAsync(member.Id);
                memberDB.Name = member.Name;
                memberDB.NickName = member.NickName;
                memberDB.Phone = member.Phone;
                memberDB.Status = member.Status;
                memberDB.MemOrg = new List<DataBase.MemOrg>();
                memberDB.MemRole = new List<DataBase.MemRole>();
                memberDB.Password = Helper.EncryptionHelper.SHA1(member.Password + memberDB.PasswordSalt, Encoding.UTF8, false);
                db.MemOrg.RemoveRange(db.MemOrg.Where(mo=>mo.Member == member.Id));
                db.MemRole.RemoveRange(db.MemRole.Where(mr => mr.Member == member.Id));
                foreach (Guid org in member.Orgs)
                {
                    memberDB.MemOrg.Add(new DataBase.MemOrg { Member = memberDB.Id, Org = org });
                }
                foreach (Guid role in member.Roles)
                {
                    memberDB.MemRole.Add(new DataBase.MemRole { Member = memberDB.Id, Role = role });
                }
                db.Entry(memberDB).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "修改成功" });
                }
                catch (Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "服务器内部错误" });
                }
            }
            else
            {
                return Json(new { status = "fail", msg = "请求参数错误" });
            }
        }

        /// <summary>
        /// 更新账户状态
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateMemberStatus(ReqManage.MemberStatus member)
        {
            if (ModelState.IsValid)
            {
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
                var powerOrgs = Factory.OrgFactory.GetPowerOrgs(userId, Models.Config.Status.normal, db);
                var filterMemberIds = db.MemOrg.Where(mo => powerOrgs.Contains(mo.Org)).Select(mo => mo.Member).ToList();
                if (member.Id == null || !filterMemberIds.Contains(member.Id))
                {
                    return Json(new { status = "fail", msg = "账户不存在或非法" });
                }
                var memberDB = await db.Members.FindAsync(member.Id);
                memberDB.Status = member.Status;
                db.Entry(memberDB).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return Json(new { status = "success", msg = "修改成功" });
                }
                catch (Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "服务器内部错误" });
                }
            }
            else
            {
                return Json(new { status = "fail", msg = "请求参数错误" });
            }
        }
        
        
        /// <summary>
        /// 检索+分页查找用户
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ListMembers(ReqManage.ListMember query)
        {
            if (ModelState.IsValid)
            {
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
                var powerOrgs = Factory.OrgFactory.GetPowerOrgs(userId, Models.Config.Status.normal, db);
                if (query.Org != null && !powerOrgs.Contains((Guid)query.Org))
                {
                    return Json(new { status = "fail", msg = "部门非法" });
                }
                var powerRoles = db.Roles.Where(r => r.Status == Models.Config.Status.normal && powerOrgs.Contains(r.Org)).Select(r => r.Id);
                if (query.Role != null && !powerRoles.Contains((Guid)query.Role))
                {
                    return Json(new { status = "fail", msg = "角色非法" });
                }
                var filterMemberIds = db.MemOrg.Where(mo => powerOrgs.Contains(mo.Org)).Select(mo => mo.Member).ToList();

                if (query.Org != null || query.Role != null)
                {
                    //首先得到权限下的所有用户
                    var allMembers = db.MemOrg.Where(mo => powerOrgs.Contains(mo.Org)).Join(db.MemRole, mo => mo.Member, mr => mr.Member, (mo, mr) => new { mo.Member, mo.Org, mr.Role });
                    if (query.Org != null)
                    {
                        //筛选所选部门
                        allMembers = allMembers.Where(ml => ml.Org == query.Org);
                    }
                    if (query.Role != null)
                    {
                        //筛选所选角色
                        allMembers = allMembers.Where(ml => ml.Role == query.Role);
                    }
                    filterMemberIds = allMembers.Select(ml => ml.Member).ToList();
                }
                var members = db.Members.Where(m => m.Status != Models.Config.Status.deleted && filterMemberIds.Contains(m.Id));
                if (!string.IsNullOrWhiteSpace(query.Name))
                {
                    //筛选用户名
                    members = members.Where(m => m.Name == query.Name);
                }
                if (query.Phone != null)
                {
                    //筛选手机号
                    members = members.Where(m => m.Phone == query.Phone);
                }
                var source = members.Select(m => new ResManage.Member
                {
                    Id = m.Id,
                    Name = m.Name,
                    Phone = m.Phone,
                    NickName = m.NickName,
                    Status = m.Status,
                    Roles = db.MemRole.Where(mr => mr.Member == m.Id && mr.Roles.Status == Models.Config.Status.normal).Select(mr => new ResManage.IdName{ Id = mr.Role, Name = mr.Roles.Name }).ToList(),
                    Orgs = db.MemOrg.Where(mo => mo.Member == m.Id && mo.Orgs.Status == Models.Config.Status.normal).Select(mr => new ResManage.IdName { Id = mr.Org,Name = mr.Orgs.Name }).ToList()

                }).OrderBy(m=>m.Status);
                var pagination = Helper.PaginationHelper<ResManage.Member>.Paging(source, query.PageIndex, query.PageSize);
                if(pagination == null)
                {
                    return Json(new { status = "fail", msg = "查询为空" });
                }
                return Json(new { status = "success", msg = "获取成功", content = new { data = pagination,total = pagination.Total} });
            }
            else
            {
                return Json(new { status = "fail", msg = "请求参数错误" });
            }

        }
    }
}
