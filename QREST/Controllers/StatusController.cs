using QREST.Models;
using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QREST.Controllers
{
    public class StatusController : Controller
    {
        // GET: Status
        public ActionResult Index()
        {
            var xxx = db_Air.GetT_QREST_DATA_HOURLY_MostRecentRecord();

            var model = new vmStatusIndex { 
                currTime = xxx.Item1,
                sampTime = xxx.Item2 ?? System.DateTime.UtcNow.AddDays(-1),
                staleness = xxx.Item3
            };
            return View(model);
        }
    }
}