using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using QREST.Models;
using QRESTModel.DAL;

namespace QREST.Controllers
{
    [Authorize]
    public class SharedController : Controller
    {
        public ActionResult _PartialHeadNotification()
        {
            var model = new vmSharedNotificationHeader();
            string UserIDX = User.Identity.GetUserId();

            model.Notifications = db_Account.GetT_QREST_USER_NOTIFICATION_ByUserIDUnreadTop3(UserIDX);
            model.NotifyCount = db_Account.GetT_QREST_USER_NOTIFICATION_ByUserIDUnreadCount(UserIDX);

            return PartialView(model);
        }

    }
}