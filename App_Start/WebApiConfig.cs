using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using WebApi.App_Start;
//using Microsoft.Owin.Security.OAuth;

using WebApi.Filter;

namespace WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            // 将 Web API 配置为仅使用不记名令牌身份验证。
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            //config.Filters.Add(new WebApiProfilingActionFilter());
            // Web API 路由

            //var jsonFormatter = new JsonMediaTypeFormatter();
            //config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(jsonFormatter));
            config.Services.Replace(typeof(IHttpControllerSelector), new NamespaceSelector(config));

            //Web API 路由
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //      name: "Default",
            //    routeTemplate: "{controller}/{action}/{id}",
            //    defaults: new { controller = "swagger", action = "ui", id = "index" }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "MiniApi",
            //    routeTemplate: "api/mini/{controller}/{action}/{id}",
            //    defaults: new
            //    {
            //        id = RouteParameter.Optional,
            //        namespaces = new[] { "WebApi.Controllers.Mini" }
            //    }
            //    );
            config.Routes.MapHttpRoute(
                name: "WebApi",
                routeTemplate: "api/web/{controller}/{action}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional,
                    namespaces = new[] { "WebApi.Controllers.Web" }
                }
                );
        }

        public class JsonContentNegotiator : IContentNegotiator
        {
            private readonly JsonMediaTypeFormatter _jsonFormatter;

            public JsonContentNegotiator(JsonMediaTypeFormatter formatter)
            {
                _jsonFormatter = formatter;
            }


            public ContentNegotiationResult Negotiate(Type type,HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
            {
                var result = new ContentNegotiationResult(_jsonFormatter, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
                return result;
            }
        }
    }
}
