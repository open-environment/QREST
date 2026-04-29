using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace QREST.Controllers
{
    public class HealthController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                bool isConnection = db_Ref.AnyT_QREST_APP_SETTING();

                if (isConnection) {
                    return new HttpStatusCodeResult(200);
                }
                else
                {
                    return new HttpStatusCodeResult(503);
                }

            }
            catch
            {
                return new HttpStatusCodeResult(503);
            }

        }
    }
}