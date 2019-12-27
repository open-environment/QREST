using QREST_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRESTModel.DAL;
using QRESTModel.BLL;

namespace QRESTServiceCatalog
{
    class HourlyValidation
    {
        private static HourlyValidation objInstance = null;
        private static bool bExecutingGenLedSvcStatus = false;

        public static HourlyValidation Instance
        {
            get
            {
                if (objInstance == null)
                    objInstance = new HourlyValidation();
                return objInstance;
            }
        }

        public void RunService()
        {
            try
            {
                if (!bExecutingGenLedSvcStatus)
                    ExecuteHourlyValidation();
            }
            catch (Exception ex)
            {
                General.WriteToFile("Error in ExecuteLoggerPolling: Error Message : " + ex.ToString());
                bExecutingGenLedSvcStatus = false;
            }
        }

        private void ExecuteHourlyValidation()
        {
            bExecutingGenLedSvcStatus = true;
            General.WriteToFile("ExecuteHourlyValidation process started.");

            //this is where logic for the task goes
            db_Air.SP_VALIDATE_HOURLY();

            //then send out notifications
            List<string> NotifyUsers = db_Air.GetT_QREST_DATA_HOURLY_NotificationUsers();
            foreach (string u in NotifyUsers)
            {
                string msg = "" + Environment.NewLine;
                List<RawDataDisplay> notifies = db_Air.GetT_QREST_DATA_HOURLY_NotificationsListForUser(u);
                foreach (RawDataDisplay n in notifies)
                {
                    msg += n.SITE_ID + ": " + n.PAR_NAME + ": " + n.VAL_CD + " alert." + Environment.NewLine;
                }

                var emailParams = new Dictionary<string, string> { { "notifyMsg", msg } };
                UtilsNotify.NotifyUser(u, null, null, null, null, "POLLING_ALERT", emailParams, null);
            }

            //then update all records to notified
            List<T_QREST_DATA_HOURLY> xxx = db_Air.GetT_QREST_DATA_HOURLY_NotNotified();
            foreach (T_QREST_DATA_HOURLY xx in xxx)
            {
                db_Air.UpdateT_QREST_DATA_HOURLY_Notified(xx.DATA_HOURLY_IDX);
            }



            General.WriteToFile("ExecuteHourlyValidation process finished.");
            bExecutingGenLedSvcStatus = false;
        }

    }
}
