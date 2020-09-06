using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class CommonController : ApiController
    {
        public IHttpActionResult GetMenuType()
        {
            return Json(new { code = 200, msg = "获取成功", content = Models.Setting.menuType });
        }
    }
}
