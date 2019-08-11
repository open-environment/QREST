using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
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


    }
}