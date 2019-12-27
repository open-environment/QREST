using QREST_Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using QRESTModel.DAL;

namespace QRESTServiceCatalog
{
    class LoggerPolling
    {
        private static LoggerPolling objInstance = null;
        private static bool bExecutingGenLedSvcStatus = false;

        public static LoggerPolling Instance
        {
            get
            {
                if (objInstance == null)
                    objInstance = new LoggerPolling();
                return objInstance;
            }
        }

        public void RunService()
        {
            try
            {
                if (!bExecutingGenLedSvcStatus)
                    ExecuteLoggerPolling();
            }
            catch (Exception ex)
            {
                General.WriteToFile("Error in ExecuteLoggerPolling: Error Message : " + ex.ToString());
                bExecutingGenLedSvcStatus = false;
            }
        }

        private void ExecuteLoggerPolling()
        {
            bExecutingGenLedSvcStatus = true;
            General.WriteToFile("ExecuteLoggerPolling process started.");

            ////this is where logic for the task goes
            //List<T_QREST_SITES> _sites = db_Air.GetT_QREST_SITES_ReadyToPoll();
            //if (_sites != null)
            //{
            //    foreach (T_QREST_SITES _site in _sites)
            //    {
            //        General.WriteToFile("Starting poll for org:" + _site.ORG_ID + " site: " + _site.SITE_ID);

            //        General.WriteToFile("Ending poll for org:" + _site.ORG_ID + " site: " + _site.SITE_ID);
            //    }
            //}
            //else
            //    General.WriteToFile("No sites ready for polling.");

            General.WriteToFile("ExecuteLoggerPolling process finished.");
            bExecutingGenLedSvcStatus = false;
        }

    }
}
