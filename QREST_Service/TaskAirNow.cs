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


            //only run this task if the global setting says to
            string activeInd = db_Ref.GetT_QREST_APP_SETTING("AIRNOW_ACTIVE_IND");
            if (activeInd == "1")
            {
                //create AirNow directory if not there
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\AirNow";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //write data to text file
                string file = System.DateTime.Now.ToString("yyyyMMddHHmm") + "_840.TRX";  //840 is USA country code
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\AirNow\\" + file;
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
                                if (_line.Length > 50)
                                    sw.WriteLine(_line);
                            }
                        }
                    }

                    string ftpUser = db_Ref.GetT_QREST_APP_SETTING("AIRNOW_FTP_USER");
                    string ftpPwd = db_Ref.GetT_QREST_APP_SETTING("AIRNOW_FTP_PWD");
                    string ip = "webdmcdata.airnowtech.org";
                    using (var client = new Renci.SshNet.SftpClient(ip, ftpUser, ftpPwd))
                    {
                        try
                        {
                            client.Connect();
                            client.ChangeDirectory("AQCSV");

                            var xxx = client.ListDirectory(".");

                            using (var fileStream = new FileStream(filepath, FileMode.Open))
                            {
                                client.BufferSize = 4 * 1024; // bypass payload error large files
                                client.UploadFile(fileStream, Path.GetFileName(filepath));
                            }

                            client.Disconnect();
                            General.WriteToFile("AirNow: File sent - " + file);
                        }
                        catch (Exception ex)
                        {
                            General.WriteToFile("AirNow: ERROR - " + ex.Message);
                        }
                    }

                    //PREVIOUS AIRNOW CONNECTION APPROACH************************************
                    //using (var client = new System.Net.WebClient())
                    //{
                    //    string ftpUser = db_Ref.GetT_QREST_APP_SETTING("AIRNOW_FTP_USER");
                    //    string ftpPwd = db_Ref.GetT_QREST_APP_SETTING("AIRNOW_FTP_PWD");

                    //    client.Credentials = new System.Net.NetworkCredential(ftpUser, ftpPwd);
                    //    client.UploadFile("ftp://ftp.airnowdata.org/incoming/data/AQCSV/" + file, System.Net.WebRequestMethods.Ftp.UploadFile, filepath);
                    //}
                    //General.WriteToFile("AirNow: File sent - " + file);
                    //END PREVIOUS AIRNOW CONNECTION APPROACH************************************
                }
            }

            bExecutingGenLedSvcStatus = false;
        }

    }
}
