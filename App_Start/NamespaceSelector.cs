﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace WebApi.App_Start
{
    /// <summary>
    /// 重写请求选择器
    /// </summary>
    public class NamespaceSelector:DefaultHttpControllerSelector
    {
        private const string NamespaceRouteVariableName = "namespaces";

        private readonly HttpConfiguration _configuration;

        /// <summary>
        /// 多个线程同时访问的键/值对的线程安全集合
        /// </summary>
        private readonly Lazy<ConcurrentDictionary<string, Type>> _apiControllerCache;

        private ConcurrentDictionary<string, Type> InitializeApiControllerCache()
        {
            IAssembliesResolver assembliesResolver = _configuration.Services.GetAssembliesResolver();
            var types = _configuration.Services.GetHttpControllerTypeResolver()
                .GetControllerTypes(assembliesResolver).ToDictionary(t => t.FullName, t => t);
            return new ConcurrentDictionary<string, Type>(types);
        }

        public NamespaceSelector(HttpConfiguration configuration):base(configuration)
        {
            _configuration = configuration;
            _apiControllerCache = new Lazy<ConcurrentDictionary<string, Type>>(new Func<ConcurrentDictionary<string, Type>>(InitializeApiControllerCache));
        }

        public IEnumerable<string> GetControllerFullName(HttpRequestMessage request,string controllerName)
        {
            object namespaceName;
            var data = request.GetRouteData();
            IEnumerable<string> keys = _apiControllerCache.Value.ToDictionary<KeyValuePair<string, Type>, string, Type>(t => t.Key,
                    t => t.Value, StringComparer.CurrentCultureIgnoreCase).Keys.ToList();

            if(!data.Values.TryGetValue(NamespaceRouteVariableName,out namespaceName))
            {
                return from k in keys
                       where k.EndsWith(string.Format(".{0}{1}", controllerName,
                       DefaultHttpControllerSelector.ControllerSuffix), StringComparison.CurrentCultureIgnoreCase)
                       select k;
            }
            string[] namespaces = (string[])namespaceName;
            return from n in namespaces
                   join k in keys on string.Format("{0}.{1}{2}", n, controllerName,
                   DefaultHttpControllerSelector.ControllerSuffix).ToLower() equals k.ToLower()
                   select k;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            Type type;
            if(request == null)
            {
                throw new ArgumentException("request");
            }
            string controllerName = GetControllerName(request);
            if (string.IsNullOrEmpty(controllerName))
            {
                throw new HttpResponseException(request.CreateErrorResponse(System.Net.HttpStatusCode.NotFound,
                     string.Format("无法通过API路由匹配到您所请求的URI '{0}'",
                    new object[] { request.RequestUri })));
            }

            IEnumerable<string> fullNames = GetControllerFullName(request, controllerName);
            if(fullNames.Count() == 0)
            {
                throw new HttpResponseException(request.CreateErrorResponse(System.Net.HttpStatusCode.NotFound,
                        string.Format("无法通过API路由匹配到您所请求的URI '{0}'",
                        new object[] { request.RequestUri })));
            }

            if(_apiControllerCache.Value.TryGetValue(fullNames.First(),out type))
            {
                return new HttpControllerDescriptor(_configuration, controllerName, type);
            }
            throw new HttpResponseException(request.CreateErrorResponse(System.Net.HttpStatusCode.NotFound,
               string.Format("无法通过API路由匹配到您所请求的URI '{0}'",
               new object[] { request.RequestUri })));
        }
    }
}