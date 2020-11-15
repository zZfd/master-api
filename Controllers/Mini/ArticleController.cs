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
    [Filter.AuthorityFilter]
    public class ArticleController : ApiController
    {

        private const string TOKEN = "ZFDYES";
        private readonly MiniDB.MiniDB DB = new MiniDB.MiniDB();

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

                var userDb = await DB.Member.FindAsync(userId);
                if (!userDb.Maker)
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
                    Status = Models.Config.Status.forbidden // 文章发布后需要审核
                };
                var logDb = new MiniDB.Log
                {
                    Member = userId,
                    Time = DateTime.Now,
                    Type = "文章添加",
                    Remarks = "文章添加",
                    IP = HttpContext.Current.Request.UserHostAddress,
                    UserAgent = HttpContext.Current.Request.UserAgent
                };
                try
                {
                    DB.Article.Add(articleDb);
                    DB.Log.Add(logDb);

                    await DB.SaveChangesAsync();
                    return Json(new { statusCode = HttpStatusCode.Created, msg = "添加成功", content = articleDb.Id }); // 回传id，用户附件保存
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

        /// <summary>
        /// 修改（审核，删除）文章
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> Update(EntityReq.Article article)
        {

            if (ModelState.IsValid)
            {
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
                if (userId == Guid.Empty)
                {
                    return Json(new { statusCode = HttpStatusCode.Unauthorized, msg = "请先登录" });
                }
                var userDb = await DB.Member.FindAsync(userId);
                // 权限下一个版本再做全
                if (userDb == null || !userDb.Phone.Equals("13776050390"))
                {
                    return Json(new { statusCode = HttpStatusCode.Forbidden, msg = "用户不具备权限" });
                }

                var articleDb = await DB.Article.FindAsync(article.id);
                if (articleDb == null)
                {
                    return Json(new { statusCode = HttpStatusCode.NotFound, msg = "文章不存在" });
                }
                articleDb.Title = article.title;
                articleDb.Match = article.match;
                articleDb.Recommend = article.recommend;
                articleDb.Money = article.money;
                articleDb.Analysis = article.analysis;
                articleDb.Status = article.status;

                var logDb = new MiniDB.Log
                {
                    Member = userId,
                    Time = DateTime.Now,
                    Type = "文章修改",
                    Remarks = "文章修改" + article.status,
                    IP = HttpContext.Current.Request.UserHostAddress,
                    UserAgent = HttpContext.Current.Request.UserAgent
                };

                try
                {
                    DB.Log.Add(logDb);
                    await DB.SaveChangesAsync();
                    return Json(new { statusCode = HttpStatusCode.OK, msg = "修改成功", content = articleDb.Id }); // 回传id
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

        /// <summary>
        /// 设置结果,点赞
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateOther(EntityReq.Article article)
        {

            if (ModelState.IsValid)
            {
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
                if (userId == Guid.Empty)
                {
                    return Json(new { statusCode = HttpStatusCode.Unauthorized, msg = "请先登录" });
                }
                var userDb = await DB.Member.FindAsync(userId);
                // 权限下一个版本再做全
                if (userDb == null || !userDb.Phone.Equals("13776050390"))
                {
                    return Json(new { statusCode = HttpStatusCode.Forbidden, msg = "用户不具备权限" });
                }

                var articleDb = await DB.Article.FindAsync(article.id);
                if (articleDb == null || articleDb.Status != Models.Config.Status.normal)
                {
                    return Json(new { statusCode = HttpStatusCode.NotFound, msg = "文章不存在" });
                }
                if (article.isTrue != null)
                {
                    articleDb.IsTrue = article.isTrue;
                }
                if (article.preference != null)
                {
                    if (article.preference == true)
                    {
                        articleDb.Preference++;
                    }
                    else
                    {
                        articleDb.Preference--;
                    }
                }

                try
                {
                    await DB.SaveChangesAsync();
                    return Json(new { statusCode = HttpStatusCode.OK, msg = "修改成功", content = articleDb.Id }); // 回传id
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

        /// <summary>
        ///  筛选所有文章（id,key,author,pi,ps)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult List(EntityReq.ArtcileQuery query)
        {
            var source = DB.Article.Where(a => a.Status == Models.Config.Status.normal && a.Money <= query.money);

            if (query.author != Guid.Empty)
            {
                source = source.Where(a => a.Author == query.author);
            }
            if (!string.IsNullOrWhiteSpace(query.key))
            {
                source = source.Where(a => (string.Concat(a.Member.NickName, a.Title, a.Match, a.Recommend, a.Analysis)).Contains(query.key));
            }
            if (source.Any())
            {
                var result = source.Select(a => new EntityReq.Article
                {
                    id = a.Id,
                    title = a.Title,
                    author = a.Member.NickName,
                    time = a.Time,
                    money = a.Money,
                    match = a.Match,
                    isTrue = a.IsTrue,
                    preferenceCount = a.Preference,
                    collectionCount = a.Collection,
                    cover = a.Member.AvatarUrl,
                }).OrderByDescending(a => a.time);
                var pagerResult = Helper.PaginationHelper<EntityReq.Article>.Paging(result, query.pageIndex, query.pageSize);
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

        /// <summary>
        ///  筛选免费文章（id,key,author,pi,ps)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ListFree(EntityReq.ArtcileQuery query)
        {
            var source = DB.Article.Where(a => a.Status == Models.Config.Status.normal && a.Money == 0);

            if (query.author != Guid.Empty)
            {
                source = source.Where(a => a.Author == query.author);
            }
            if (!string.IsNullOrWhiteSpace(query.key))
            {
                source = source.Where(a => (string.Concat(a.Member.NickName, a.Title, a.Match, a.Recommend, a.Analysis)).Contains(query.key));
            }
            if (source.Any())
            {
                var result = source.Select(a => new EntityReq.Article
                {
                    id = a.Id,
                    title = a.Title,
                    author = a.Member.NickName,
                    time = a.Time,
                    match = a.Match,
                    money = a.Money,
                    isTrue = a.IsTrue,
                    preferenceCount = a.Preference,
                    collectionCount = a.Collection,
                    cover = a.Member.AvatarUrl,
                }).OrderByDescending(a => a.time);
                var pagerResult = Helper.PaginationHelper<EntityReq.Article>.Paging(result, query.pageIndex, query.pageSize);
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

        /// <summary>
        ///  筛选收费文章（id,key,author,pi,ps)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ListCharge(EntityReq.ArtcileQuery query)
        {
            var source = DB.Article.Where(a => a.Status == Models.Config.Status.normal && a.Money != 0);

            if (query.author != Guid.Empty)
            {
                source = source.Where(a => a.Author == query.author);
            }
            if (!string.IsNullOrWhiteSpace(query.key))
            {
                source = source.Where(a => (string.Concat(a.Member.NickName, a.Title, a.Match, a.Recommend, a.Analysis)).Contains(query.key));
            }
            if (source.Any())
            {
                var result = source.Select(a => new EntityReq.Article
                {
                    id = a.Id,
                    title = a.Title,
                    author = a.Member.NickName,
                    time = a.Time,
                    match = a.Match,
                    money = a.Money,
                    isTrue = a.IsTrue,
                    preferenceCount = a.Preference,
                    collectionCount = a.Collection,
                    cover = a.Member.AvatarUrl,
                }).OrderByDescending(a => a.time);
                var pagerResult = Helper.PaginationHelper<EntityReq.Article>.Paging(result, query.pageIndex, query.pageSize);
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

        /// <summary>
        ///  查询单个文章
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetArticle(Guid id)
        {
            if (id != Guid.Empty)
            {
                Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);
                var article = await DB.Article.FindAsync(id);
                
                if (article == null) return Json(new { statusCode = HttpStatusCode.NoContent, msg = "查询为空" });
                // 收费文章
                if (article.Money > 0 && DB.Order.FirstOrDefault(o => o.Member == userId && o.Article == id) == null)
                {
                    // 没有缴费
                    return Json(new { statusCode = HttpStatusCode.OK, msg = "查询成功",content = new EntityReq.Article
                    {
                        id = article.Id,
                        title = article.Title,
                        author = article.Member.NickName,
                        time = article.Time,
                        match = article.Match,
                        money = article.Money,
                        preferenceCount = article.Preference,
                        collectionCount = article.Collection,
                        cover = article.Member.AvatarUrl
                    }
                    });
                }
                return Json(new { statusCode = HttpStatusCode.OK, msg = "查询成功", content = new EntityReq.Article
                {
                    id = article.Id,
                    title = article.Title,
                    author = article.Member.NickName,
                    time = article.Time,
                    match = article.Match,
                    money = article.Money,
                    recommend = article.Recommend,
                    analysis = article.Analysis,
                    //attachment = DB.Attachment.FirstOrDefault(a=>a.Belong == article.Id).Id,
                    preferenceCount = article.Preference,
                    collectionCount = article.Collection,
                    cover = article.Member.AvatarUrl
                }
                });

            }
            else
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest, msg = "请求错误" });
            }
        }
    }
}
