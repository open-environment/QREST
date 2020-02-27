using System;
using System.IO;

namespace QREST_Service
{
    class General
    {
        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                    sw.WriteLine(DateTime.Now + ": " + Message);
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                    sw.WriteLine(DateTime.Now + ": " + Message);
            }
        }

        public static void WriteToPollingFile(string Message, string SiteID)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Polls";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Polls\\Poll_" + SiteID + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                    sw.WriteLine(Message);
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                    sw.WriteLine(Message);
            }
        }


        public static void WriteToAirNowFile(string Message)
        {
            //create AirNow directory if not there
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\AirNow";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);


            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\AirNow\\" + System.DateTime.Now.ToString("yyyyMMddHHmm") + "_840.TRX";
            if (!File.Exists(filepath))
            {


                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }

        }

    }
}
