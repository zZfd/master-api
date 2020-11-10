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
    public class CollectionController : ApiController
    {

        private const string TOKEN = "ZFDYES";
        private readonly MiniDB.MiniDB DB = new MiniDB.MiniDB();

        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Add(Guid articleId)
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
            if (userId == Guid.Empty)
            {
                return Json(new { statusCode = HttpStatusCode.Unauthorized, msg = "请先登录" });
            }
            var article = await DB.Article.FindAsync(articleId);
            if (article == null || article.Status != Models.Config.Status.normal)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest, msg = "文章不存在" });
            }

            MiniDB.Collection collectionDb = new MiniDB.Collection
            {
                Member = userId,
                Article = articleId,
                Time = DateTime.Now,
                Status = Models.Config.Status.normal
            };
            try
            {
                DB.Collection.Add(collectionDb);

                await DB.SaveChangesAsync();
                return Json(new { statusCode = HttpStatusCode.Created, msg = "收藏成功", content = collectionDb.Id });
            }
            catch
            {
                return Json(new { statusCode = HttpStatusCode.InternalServerError, msg = "服务器内部错误" });
            }

        }

        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="collectionId"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> Cancel(Guid collectionId)
        {

            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
            if (userId == Guid.Empty)
            {
                return Json(new { statusCode = HttpStatusCode.Unauthorized, msg = "请先登录" });
            }

            var collectionDb = await DB.Collection.FindAsync(collectionId);
            if (collectionDb != null && collectionDb.Status == Models.Config.Status.normal && collectionDb.Member == userId)
            {
                collectionDb.Status = Models.Config.Status.deleted;
            }
            else
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest, msg = "请求错误" });
            }
            try
            {
                await DB.SaveChangesAsync();
                return Json(new { statusCode = HttpStatusCode.Created, msg = "取消收藏", content = collectionDb.Id }); // 回传id
            }
            catch
            {
                return Json(new { statusCode = HttpStatusCode.InternalServerError, msg = "服务器内部错误" });
            }
        }


        /// <summary>
        ///  获取收藏列表（pi,ps)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult List(EntityReq.ArtcileQuery query)
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
            if (userId == Guid.Empty)
            {
                return Json(new { statusCode = HttpStatusCode.Unauthorized, msg = "请先登录" });
            }
            var source = DB.Collection.Where(c => c.Member == userId);
            if (source.Any())
            {
                var result = source.Select(a => new EntityRes.Collection
                {
                    id = a.Id,
                    title = a.Article1.Title,
                    author = a.Member1.NickName,
                    time = a.Time,
                    match = a.Article1.Match,
                    isTrue = (bool)a.Article1.IsTrue,
                    preference = a.Article1.Preference,
                    collection = a.Article1.Collection,
                    cover = a.Member1.AvatarUrl,
                }).OrderByDescending(a => a.time);
                var pagerResult = Helper.PaginationHelper<EntityRes.Collection>.Paging(result, query.pageIndex, query.pageSize);
                return Json(new
                {
                    statusCode = HttpStatusCode.OK,
                    msg = "查询成功",
                    content = new
                    {
                        articles = pagerResult,
                        total = pagerResult.Total
                    }
                });
            }
            else
            {
                return Json(new { statusCode = HttpStatusCode.NoContent, msg = "查询为空" });
            }
        }

       
    }
}
