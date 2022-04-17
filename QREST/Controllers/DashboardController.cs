using QREST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QRESTModel.DAL;
using Microsoft.AspNet.Identity;

namespace QREST.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            string UserIDX = User.Identity.GetUserId();

            var model = new vmDashboardIndex
            {
                Announcement = db_Ref.GetT_QREST_APP_SETTING_CUSTOM().ANNOUNCEMENTS,
                MySiteCount = db_Air.GetT_QREST_SITES_ByUser_OrgID_count(null, UserIDX),
                MyMonitorCount = db_Air.GetT_QREST_MONITORS_ByUser_OrgID_Count(null, UserIDX),
                MyAlertCount = db_Account.GetT_QREST_USER_NOTIFICATION_ByUserIDUnreadCount(UserIDX),
                T_QREST_SITES = db_Air.GetT_QREST_SITES_ByUser_OrgID(null, UserIDX),
                ddl_MyMonitors = ddlHelpers.get_monitors_sampled_by_user(UserIDX, 30)
            };

            return View(model);
        }




        [HttpPost]
        public JsonResult RawDataTodayChart(Guid? selMon)
        {
            var data = db_Air.GetT_QREST_DATA_HOURLY_Last24Records(selMon);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}