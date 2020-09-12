using StackExchange.Profiling;
using StackExchange.Profiling.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            

            //ע��ASP.NET MVCӦ�ó����е���������
            //AreaRegistration.RegisterAllAreas();

            //����WebApi
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //ע�������
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            //ע��·������
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            StackExchange.Profiling.EntityFramework6.MiniProfilerEF6.Initialize();

    //        MiniProfiler.Configure(new MiniProfilerOptions
    //        {
    //            RouteBasePath = "~/profiler",
    //            //PopupRenderPosition = RenderPosition.Right,  // defaults to left
    //            //PopupMaxTracesToShow = 10,                   // defaults to 15
    //            //ColorScheme = ColorScheme.Auto,              // defaults to light
    //            //ResultsAuthorize = request => request.IsLocal,//Ӧ���Ǳ��ص��Ե���˼
    //            //ResultsListAuthorize = request =>
    //            //{
    //            //    // you may implement this if you need to restrict visibility of profiling lists on a per request basis
    //            //    return true; // all requests are legit in this example
    //            //},
    //            StackMaxLength = 256,
    //            // default is 120 characters
    //            // (Optional)You can disable "Connection Open()","Connection Close()"(and async variant) tracking.
    //            // (defaults to true, and connection opening/closing is tracked)
    //            //TrackConnectionOpenClose = true
    //        }.ExcludeType("SessionFactory")  // Ignore any class with the name of SessionFactory)
    //.ExcludeAssembly("NHibernate")  // Ignore any assembly named NHibernate
    //.ExcludeMethod("Flush")         // Ignore any method with the name of Flush
    //);              // Add MVC view profiling (you want this)());// Ȼ����֮ǰ�����ú�߼���AddEntityFramework()��
    //                //  MiniProfiler;
                    //������ݿ�
            App_Start.CreatDB.Creat();

        }

        protected void Application_BeginRequest()
        {
            MiniProfiler.Start();
        }
        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }
        //protected void Application_BeginRequest()
        //{
        //    MiniProfiler.StartNew();
        //}
        //protected void Application_EndRequest()
        //{
        //    MiniProfiler.Current?.Stop();
        //}
    }
}
