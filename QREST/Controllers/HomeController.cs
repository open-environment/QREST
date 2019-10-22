using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using QREST.App_Logic.BusinessLogicLayer;
using QREST.Models;
using QRESTModel.DAL;

namespace QREST.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new vmHomeIndex();
            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Map()
        {
            return View();
        }


        public ActionResult Help()
        {
            var model = new vmHomeHelp();
            model.HelpTopics = db_Ref.GetT_QREST_HELP_DOCS();
            return View(model);
        }



        public ActionResult Terms ()
        {
            var model = new vmHomeTerms();
            T_QREST_APP_SETTINGS_CUSTOM cust = db_Ref.GetT_QREST_APP_SETTING_CUSTOM();
            model.TermsAndConditions = cust.TERMS_AND_CONDITIONS;
            return View(model);
        }


        public ActionResult Test()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    Uri myUri = new Uri("https://aqs.epa.gov/data/api/monitors/bySite?email=test@aqs.api&key=test&bdate=20000101&edate=20251231&state=41&county=011&site=1036", UriKind.Absolute);
                    var json = httpClient.GetStringAsync(myUri).Result;
                    dynamic stuff = JsonConvert.DeserializeObject(json);
                    foreach (var item in stuff.Data)
                    {
                        TempData["Success"] = "Read stuff success.";
                    }

                    
                }

            }
            catch (AggregateException err)
            {
                foreach (var errInner in err.InnerExceptions)
                    db_Ref.CreateT_QREST_SYS_LOG(null, "ERROR", "Failed to import monitor - code 5 " + errInner);
            }
            catch (Exception ex)
            {
                db_Ref.CreateT_QREST_SYS_LOG(null, "ERROR", "Failed to import monitor - code 4 " + ex.Message);
            }

            return View();
        }

    }
}