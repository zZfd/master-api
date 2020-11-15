using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApi.Filter
{
    public class AuthorityFilter : AuthorizationFilterAttribute
    {
        //private Guid menuId = Guid.Empty;
        private const string TOKEN = "ZFDYES";
        private readonly string PaswordSalt = "vdsjlfjasldj";

        //private readonly DataBase.DB db = new DataBase.DB();
        public AuthorityFilter()
        {
        }
        //public AuthorityFilter(string _menuId)
        //{
        //    menuId = Guid.Parse(_menuId);
        //}

        /// <summary>
        /// token及菜单权限校验过滤器
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                //[AllowAnonymous]
                //具备该特性的api不需要进行token及菜单权限校验
                if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
                {
                    return;
                }
                string token = actionContext.Request.Headers.GetValues(TOKEN).First();
                Guid userId = Helper.EncryptionHelper.GetUserId(token);
                if (userId == Guid.Empty)
                {
                    throw new Exception("token解析错误");
                }

                using (MiniDB.MiniDB db = new MiniDB.MiniDB())
                {
                    var user = db.Member.Find(userId);
                    if (user == null || user.Status != Models.Config.Status.normal)
                    {
                        throw new Exception("用户不存在或非正常状态");
                    }
                    //token校验成功
                    if (Helper.EncryptionHelper.CheckToken(token, userId, PaswordSalt))
                    {
                        return;
                        #region 菜单校验
                        ////无需校验菜单权限
                        //if (menuId == Guid.Empty)
                        //{
                        //    return;
                        //}
                        ////菜单权限校验成功
                        //if (Menu(menuId, userId, db))
                        //{
                        //    return;
                        //}
                        //else
                        //{
                        //    //菜单权限校验失败
                        //    throw new Exception("token用户不具备该菜单权限" + menuId.ToString("N"));
                        //}
                        #endregion
                    }
                    else
                    {
                        //权限校验失败
                        HttpRequest request = HttpContext.Current.Request;
                        string msg = string.Format("请求IP：{0}\n请求代理：{1}\n用户Id：{2}\n",
                  request.UserHostAddress, request.UserAgent, userId);
                        Helper.LogHelper.WriteUnAuthorizationLog(msg);
                        //var logDb = new MiniDB.Log
                        //{
                        //    Member = userId,
                        //    Time = DateTime.Now,
                        //    Type = "token校验失败",
                        //    Remarks = "token校验失败",
                        //    IP = HttpContext.Current.Request.UserHostAddress,
                        //    UserAgent = HttpContext.Current.Request.UserAgent
                        //};

                        //db.Log.Add(logDb);
                        //db.SaveChanges();
                        throw new Exception("token校验失败");
                    }

                }

            }
            catch
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new { statusCode = HttpStatusCode.Unauthorized, msg = "无权限" });
            }
        }

        #region 菜单校验
        /// <summary>
        /// 菜单权限验证
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="memberId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        //public bool Menu(Guid menuId, Guid memberId, DataBase.DB db)
        //{
        //    var menu = db.Menus.Find(menuId);
        //    if (menu == null || menu.Status != Models.Config.Status.normal) return false;

        //    foreach (var memRole in db.Members.Find(memberId).MemRole)
        //    {
        //        foreach (var roleMenu in memRole.Roles.RoleMenu)
        //        {
        //            if (menuId == roleMenu.Menu)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;

        //    //List<Guid> memberMenusIds = (from menus in db.Menus
        //    //                             orderby menus.OrderNum
        //    //                             where (
        //    //                                 from rolemenus in db.RoleMenu
        //    //                                 where (
        //    //                                     from role in db.Roles
        //    //                                     where (
        //    //                                        from roles in db.MemRole
        //    //                                        where roles.Member == memberId
        //    //                                        select roles.Role
        //    //                                     ).Contains(role.Id) && role.Status == 0
        //    //                                     select role.Id
        //    //                                 ).Contains(rolemenus.Role)
        //    //                                 select rolemenus.Menu
        //    //                             ).Contains(menus.Id) && menus.Status == 0
        //    //                             select menus.Id).ToList();
        //    //if (!memberMenusIds.Contains(menuId)) return false;

        //    //return true;
        //}
        #endregion
    }
}