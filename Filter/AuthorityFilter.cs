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
        private Guid menuId = Guid.Empty;
        //private readonly DataBase.DB db = new DataBase.DB();
        public AuthorityFilter()
        {
        }
        public AuthorityFilter(string _menuId)
        {
            menuId = Guid.Parse(_menuId);
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
                {
                    return;
                }
                Guid userId = Guid.Parse(actionContext.Request.Headers.GetValues("sessionId").First());
                string token = actionContext.Request.Headers.GetValues("X-Token").First();
                using (DataBase.DB db = new DataBase.DB())
                {
                    var user = db.Members.Find(userId);
                    //token校验成功
                    if (user.Status == Models.Config.Status.normal && Helper.EncryptionHelper.CheckToken(token, userId, user.PasswordSalt))
                    {
                        return;
                    }
                    //无需校验菜单权限
                    if (menuId == Guid.Empty)
                    {
                        return;
                    }
                    //菜单权限校验成功
                    if (Menu(menuId, userId, db))
                    {
                        return;
                    }
                }
                //权限校验失败
                throw new Exception();
            }
            catch
            {
                HttpRequest request = HttpContext.Current.Request;
                string msg = string.Format("请求主机：{0}\n请求代理：{1}\n请求方法：{2}\n请求IP：{3}\n请求文本类型：{4}",
                   request.UserHostAddress, request.UserAgent, request.HttpMethod, request.RawUrl, request.ContentType);
                Helper.LogHelper.WriteUnAuthorizationLog(msg);
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new { status = "fail", msg = "无权限" });
            }
        }

        /// <summary>
        /// 菜单权限验证
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="memberId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool Menu(Guid menuId, Guid memberId, DataBase.DB db)
        {
            var menu = db.Menus.Find(menuId);
            if (menu == null || menu.Status != Models.Config.Status.normal) return false;

            foreach (var memRole in db.Members.Find(memberId).MemRole)
            {
                foreach (var roleMenu in memRole.Roles.RoleMenu)
                {
                    if (menuId == roleMenu.Menu)
                    {
                        return true;
                    }
                }
            }
            return false;

            //List<Guid> memberMenusIds = (from menus in db.Menus
            //                             orderby menus.OrderNum
            //                             where (
            //                                 from rolemenus in db.RoleMenu
            //                                 where (
            //                                     from role in db.Roles
            //                                     where (
            //                                        from roles in db.MemRole
            //                                        where roles.Member == memberId
            //                                        select roles.Role
            //                                     ).Contains(role.Id) && role.Status == 0
            //                                     select role.Id
            //                                 ).Contains(rolemenus.Role)
            //                                 select rolemenus.Menu
            //                             ).Contains(menus.Id) && menus.Status == 0
            //                             select menus.Id).ToList();
            //if (!memberMenusIds.Contains(menuId)) return false;

            //return true;
        }
    }
}