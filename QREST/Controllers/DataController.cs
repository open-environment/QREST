using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QREST.Models;

namespace QREST.Controllers
{
    public class DataController : Controller
    {
        // GET: Data
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QCList()
        {
            var model = new vmDataQCList();
            return View(model);
        }


        public ActionResult QCEntry()
        {
            var model = new vmDataQCEntry();
            return View(model);
        }
    }
}