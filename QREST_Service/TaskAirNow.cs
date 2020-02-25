using QREST_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRESTModel.DAL;
using QRESTModel.BLL;
using System.IO;

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

            //create AirNow directory if not there
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\AirNow";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //write data to text file
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\AirNow\\" + System.DateTime.Now.ToString("yyyyMMddHHmm") + "_840.TRX";
            if (!File.Exists(filepath))
            {
                //create file
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    List<AIRNOW_LAST_HOUR> _rows = db_Air.GetAIRNOW_LAST_HOUR();
                    if (_rows != null)
                    {
                        foreach (AIRNOW_LAST_HOUR _row in _rows)
                        {
                            string _line = _row.AIRNOW_SITE + "," + _row.DATA_STATUS + ",2," + _row.DT + "," + _row.PAR_CODE + ",60,," + _row.DATA_VALUE + "," + _row.UNIT_CODE + ",0," + _row.POC + ",,,,,,,,,";
                            if (_line.Length>50)
                                sw.WriteLine(_line);
                        }
                    }
                }


            }

            bExecutingGenLedSvcStatus = false;
        }

    }
}
