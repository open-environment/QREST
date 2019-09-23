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

            //this is where logic for the task goes
            List<T_QREST_SITES> _sites = db_Air.GetT_QREST_SITES_ReadyToPoll();
            if (_sites != null)
            {
                foreach (T_QREST_SITES _site in _sites)
                {
                    General.WriteToFile("Starting poll for org:" + _site.ORG_ID + " site: " + _site.SITE_ID);


                    ////************** HANDLE TERMINAL SERVICE POLLING (ZENO ETC) ***********************************
                    //if (_config.LOGGER_TYPE == "ZENO")
                    //{
                    //    hawin32.HyperACCESS hawin;
                    //    hawin32.IHAScript hascript;
                    //    hawin = new hawin32.HyperACCESS();
                    //    hascript = (hawin32.IHAScript)hawin.haInitialize("test");

                    //    //restores the Hyperaccess GUI screen so it could be viewable while this script is running
                    //    hascript.haSizeHyperACCESS(3);

                    //    //Tell HyperACCESS which entry file to open
                    //    hascript.haOpenSession(@"C:\temp\COOS_Conn3.HAW");

                    //    //Starts the connection process within the current session.
                    //    int ConnID = hascript.haConnectSession(0);

                    //    //attempt to manually write stuff
                    //    hascript.haWait(3000);
                    //    hascript.haTypeText(0, "u\r\n");
                    //    hascript.haWait(3000);
                    //    hascript.haTypeText(0, "password\r\n");
                    //    hascript.haWait(3000);
                    //    hascript.haTypeText(0, "D\r\n");
                    //    hascript.haWait(3000);
                    //    hascript.haTypeText(0, "XL20\r\n");
                    //    hascript.haSetXferProtocol(2, 5);
                    //    hascript.haWait(3000);
                    //    hascript.haXferReceive(_site.SITE_IDX.ToString() + ".txt");
                    //    hascript.haWait(3000);
                    //    hascript.haTypeText(0, "Q\r\n");
                    //    hascript.haWait(3000);
                    //    hascript.haDisconnectSession();
                    //    hascript.haMenuString("FX");


                    //}


                    General.WriteToFile("Ending poll for org:" + _site.ORG_ID + " site: " + _site.SITE_ID);
                }
            }
            else
                General.WriteToFile("No sites ready for polling.");

            General.WriteToFile("ExecuteLoggerPolling process finished.");
            bExecutingGenLedSvcStatus = false;
        }

    }
}
