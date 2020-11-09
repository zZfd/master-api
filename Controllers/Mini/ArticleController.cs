using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using EntityReq = WebApi.Models.Request.Mini;
using EntityRes = WebApi.Models.Response.Mini;

namespace WebApi.Controllers.Mini
{
    public class ArticleController : ApiController
    {

        private const string TOKEN = "ZFDYES";
        private MiniDB.MiniDB DB = new MiniDB.MiniDB();

        /// <summary>
        /// 添加文章
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Add(EntityReq.Article article)
        {

            if (ModelState.IsValid)
            {
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
                if(userId == Guid.Empty)
                {
                    return Json(new { statusCode = HttpStatusCode.Unauthorized, msg = "请先登录" });
                }
                var userDb = await DB.Member.FindAsync(userId);
                if(userDb == null || !userDb.Maker)
                {
                    return Json(new { statusCode = HttpStatusCode.Forbidden, msg = "用户不具备权限" });
                }

                if(!userDb.Expert && article.money > 0)
                {
                    return Json(new { statusCode = HttpStatusCode.Forbidden, msg = "请先成为创作者" });
                }

                MiniDB.Article articleDb = new MiniDB.Article
                {
                    Id = Guid.NewGuid(),
                    Title = article.title,
                    Author = userId,
                    Match = article.match,
                    Recommend = article.recommend,
                    Money = article.money,
                    Analysis = article.analysis,
                    Time = DateTime.Now,
                    Status = Models.Config.Status.normal
                };
                try
                {
                    DB.Article.Add(articleDb);
                    await DB.SaveChangesAsync();
                    return Json(new { statusCode = HttpStatusCode.Created, msg = "添加成功",content = articleDb.Id }); // 回传id，用户附件保存
                }
                catch
                {
                    return Json(new { statusCode = HttpStatusCode.InternalServerError, msg = "服务器内部错误" });
                }

            }
            else
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest, msg = ModelState.Values }); // 回复错误信息
            }
        }


    }
}
