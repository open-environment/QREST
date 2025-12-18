using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using QRESTModel.DAL;
using QREST.Controllers;
using Microsoft.AspNet.Identity;
using QREST.App_Logic.BusinessLogicLayer;
using System.Linq;


namespace QREST
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            //System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);  //adding web api support to QREST
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        
        /// <summary>
        /// Global error handling routine
        /// </summary>
        protected void Application_Error(object sender, EventArgs e)
        {
            // Get the context
            var httpContext = HttpContext.Current;

            // Grab information about the last error occurred 
            var ex = Server.GetLastError();

            var httpException = ex as HttpException;
            var statusCode = httpException?.GetHttpCode() ?? 500;

            // List of ignorable 404 paths
            var requestPath = httpContext?.Request?.Url?.AbsolutePath.ToLowerInvariant();

            var ignored404s = new[] {
                "/favicon.ico",
                "/robots.txt",
                "/.well-known/traffic-advice"
            };

            if (statusCode == 404 && requestPath != null && ignored404s.Contains(requestPath))
            {
                // Don't log these
                Server.ClearError();
                return;
            }

            //keep going to inner exception
            Exception realerror = ex;
            while (realerror.InnerException != null)
                realerror = realerror.InnerException;

            //try getting current user
            string userIDX = (Request.IsAuthenticated ? User.Identity.GetUserId() : "PublicUser");
            string IP = GetIP.GetLocalIPAddress(System.Web.HttpContext.Current);

            //log error
            db_Ref.CreateT_QREST_SYS_LOG(userIDX, null, realerror.Message + IP );

            //var httpContext = ((MvcApplication)sender).Context;

            //get current MVC route
            var currentController = " ";
            var currentAction = " ";
            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                    currentController = currentRouteData.Values["controller"].ToString();

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                    currentAction = currentRouteData.Values["action"].ToString();
            }


            var controller = new ErrorController();
            var routeData = new RouteData();
            var action = "Index";

            //route to 404 error page if this is that kind of error
            if (ex is HttpException)
            {
                var httpEx = ex as HttpException;
                if (httpEx.GetHttpCode() == 404)
                    action = "NotFound";
            }

            // Clear the response stream 
            httpContext.ClearError();
            httpContext.Response.Clear();

            // Inject error code into the routed page (so it is not sent as an Http OK 200)
            httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;

            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = action;

            controller.ViewData.Model = new HandleErrorInfo(ex, currentController, currentAction);
            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));

        }
        
    }
}
