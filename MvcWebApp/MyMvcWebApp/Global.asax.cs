﻿using LoggerPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MvcWebApp
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //启用压缩
            BundleTable.EnableOptimizations = true;
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            //Application["online"] = 0;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //系统级别Exception记录
            Exception objExp = HttpContext.Current.Server.GetLastError();
            LogHelper.ErrorLog("<br/><strong>CusIP</strong>：" + Request.UserHostAddress + "<br /><strong>ErrUrl</strong>：" + Request.Url, objExp);
            var urlArray = Request.Url.ToString().Split('/');
            var errorUrl = urlArray[0] + @"//" + urlArray[2] + @"/Error/SysError";
            Response.Redirect(errorUrl, true);
        }

        protected void Session_Start()
        {
            //Application.Lock();
            //Application["online"] = (int)Application["online"] + 1;
            //Application.UnLock();
        }

        protected void Session_End()
        {
            //Application.Lock();
            //Application["online"] = (int)Application["online"] - 1;
            //Application.UnLock();
        }

    }
}