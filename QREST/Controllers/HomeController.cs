using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using QREST.App_Logic.BusinessLogicLayer;
using QREST.App_Logic.DataAccessLayer;
using QREST.Models;

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
            ViewBag.Message = "Your application description page.";

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




    }
}