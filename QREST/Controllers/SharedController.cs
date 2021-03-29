using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using QREST.Models;
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

    }
}