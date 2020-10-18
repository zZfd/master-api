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
            //var jsonFormatter = new JsonMediaTypeFormatter();
            //config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(jsonFormatter));
            config.Services.Replace(typeof(IHttpControllerSelector), new NamespaceSelector(config));

            // Web API 路由
            config.MapHttpAttributeRoutes();



            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/manage/{controller}/{action}",
                defaults: new
                {
                    id = RouteParameter.Optional,
                    namespaces = new [] { "WebApi.Controllers.Manage" }
                    //namespaces = new[] { "WebApi.Controllers.Football", "WebApi.Controllers.Manage" }
                }
              
                );

            config.Routes.MapHttpRoute(
                name: "FootballApi",
                routeTemplate: "api/football/{controller}/{action}",
                defaults: new
                {
                    id = RouteParameter.Optional,
                    namespaces = new[] { "WebApi.Controllers.Football" }
                }
                );

            //config.Routes.IgnoreRoute(
            //    routeName: "FootballApi",
            //    routeTemplate: "api/manage/{controller}/{action}/{id}",
            //    constraints: new
            //    {
            //        id = RouteParameter.Optional,
            //        namespaces = new[] { "WebApi.Controllers.Manage" }
            //    }
            //    );
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
