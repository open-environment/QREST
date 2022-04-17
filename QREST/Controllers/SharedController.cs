using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using QREST.Models;
using QRESTModel.BLL;
using QRESTModel.DAL;

namespace QREST.Controllers
{
    public class SharedController : Controller
    {
        [Authorize]
        public ActionResult _PartialHeadNotification()
        {
            var model = new vmSharedNotificationHeader();
            string UserIDX = User.Identity.GetUserId();

            model.Notifications = db_Account.GetT_QREST_USER_NOTIFICATION_ByUserIDUnreadTop3(UserIDX);
            model.NotifyCount = db_Account.GetT_QREST_USER_NOTIFICATION_ByUserIDUnreadCount(UserIDX);

            return PartialView(model);
        }

        public ActionResult _PartialHeadEnvironment()
        {
            var model = new vmSharedEnvironment();
            model.environment = db_Ref.GetT_QREST_APP_SETTING("ENVIRONMENT");
            return PartialView(model);
        }




        /// <summary>
        /// Ajax call from Admin/LogActivity view
        /// and Site/ViewChangeLog view
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LogActivityData()
        {
            var draw = Request.Form.GetValues("draw")?.FirstOrDefault();  //pageNum
            int pageSize = Request.Form.GetValues("length").FirstOrDefault().ConvertOrDefault<int>();  //pageSize
            int? start = Request.Form.GetValues("start")?.FirstOrDefault().ConvertOrDefault<int?>();  //starting record #
            int orderCol = Request.Form.GetValues("order[0][column]").FirstOrDefault().ConvertOrDefault<int>();  //ordering column
            string orderColName = Request.Form.GetValues("columns[" + orderCol + "][name]").FirstOrDefault();
            string orderDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault(); //ordering direction

            //data filters
            DateTime? minDate = Request.Form.GetValues("mini")?.FirstOrDefault().ConvertOrDefault<DateTime?>();
            DateTime? maxDate = Request.Form.GetValues("maxi")?.FirstOrDefault().ConvertOrDefault<DateTime?>();
            string supportid = Request.Form.GetValues("supportid")?.FirstOrDefault().ConvertOrDefault<string>();
            string orgID = Request.Form.GetValues("orgi")?.FirstOrDefault().ConvertOrDefault<string>();

            //validation
            if (!string.IsNullOrEmpty(orgID))
            {
                if (db_Account.CanAccessThisOrg(User.Identity.GetUserId(), orgID, false) == false) {
                    return RedirectToAction("AccessDenied", "Error");
                }
            }

            var data = db_Ref.GetT_QREST_SYS_LOG_ACTIVITY(supportid, orgID, minDate, maxDate, pageSize, start, orderColName, orderDir);
            var recordsTotal = db_Ref.GetT_QREST_SYS_LOG_ACTIVITYcount(supportid, orgID, minDate, maxDate);

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }



    }
}