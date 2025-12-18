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

            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Polls\\Poll_" + SiteID + "_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
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

        public static void ArchiveCampbellPollingFile(string SiteID)
        {
            //create archive directory if needed
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Polls\\Archive";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //see if file exists that needs to be moved
            string sourceFilepath = AppDomain.CurrentDomain.BaseDirectory + "\\Polls\\Poll_" + SiteID + ".dat";
            if (File.Exists(sourceFilepath))
                File.Move(sourceFilepath, AppDomain.CurrentDomain.BaseDirectory + "\\Polls\\Archive\\" + SiteID + DateTime.Now.ToString("yyyy-dd-M--HH-mm") + ".dat");
            else
                WriteToFile("No file found to archive");

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
