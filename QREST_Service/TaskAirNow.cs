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
    class AirNow
    {
        private static AirNow objInstance = null;
        private static bool bExecutingGenLedSvcStatus = false;

        public static AirNow Instance
        {
            get
            {
                if (objInstance == null)
                    objInstance = new AirNow();
                return objInstance;
            }
        }

        public void RunService()
        {
            try
            {
                if (!bExecutingGenLedSvcStatus)
                    ExecuteAirNow();
            }
            catch (Exception ex)
            {
                General.WriteToFile("Error in AirNow: Error Message : " + ex.ToString());
                bExecutingGenLedSvcStatus = false;
            }
        }

        private void ExecuteAirNow()
        {
            bExecutingGenLedSvcStatus = true;

            //this is where logic for the task goes
            List<T_QREST_SITES> _sites = db_Air.GetT_QREST_SITES_ReadyToAirNow();
            if (_sites != null)
            {
                foreach (T_QREST_SITES _site in _sites)
                {
                    General.WriteToFile("Sending AirNow for : " + _site.SITE_ID);


                }
            }



            bExecutingGenLedSvcStatus = false;
        }

    }
}
