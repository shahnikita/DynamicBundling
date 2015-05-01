using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DynamicBundling
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            ParseDynamicBundle();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
           
        }
         private void ParseDynamicBundle(){
             string[] pathParts = Request.FilePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
             string fileChain = Request.QueryString["files"];

             if (pathParts.Length < 2 || fileChain == "")
                 return;

             string bundleType = pathParts[0];

             if (!Enum.GetNames(typeof(bundleTypes)).Contains(bundleType))
                 return;

             string bundleName = pathParts[1];
            
             Bundle bundle = new Bundle("~/" + bundleType + "/" + bundleName);

             if (bundleType == bundleTypes.Script.ToString())
             {
               
                 bundle = new ScriptBundle("~/" + bundleType + "/" + bundleName);
             }
             else if (bundleType == bundleTypes.Style.ToString())
             {
                 
                 bundle = new StyleBundle("~/" + bundleType + "/" + bundleName);
             }


             foreach (string fileName in fileChain.Split(','))
             {

                 if (fileName.IndexOf("bundle", StringComparison.InvariantCultureIgnoreCase) >-1)
                 {
                 
                     var resolver = new BundleResolver(BundleTable.Bundles);
                     List<string> cont = resolver.GetBundleContents(fileName).ToList();
                     foreach (var bundleFile in cont)
                     {
                         bundle.Include(bundleFile);
                     }
                 }
                 else
                 {
                     bundle.Include(fileName);
                 }
             }

             BundleTable.Bundles.Add(bundle);
        }
         private enum bundleTypes
         {
             
             Script,
             Style
         };
    }
}