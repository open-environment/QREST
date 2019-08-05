using QREST.App_Logic.DataAccessLayer;
using QREST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QREST.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            var model = new vmDashboardIndex
            {
                Announcement = db_Ref.GetT_QREST_APP_SETTING_CUSTOM().ANNOUNCEMENTS
            };

            return View(model);
        }
    }
}