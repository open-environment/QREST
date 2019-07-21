using System;
using System.Net;
using System.Web;


namespace QREST.App_Logic.BusinessLogicLayer
{
    public static class GetIP
    {
        public static string GetLocalIPAddress(HttpContext _context)
        {
            string visitorIPAddress = "";

            try
            {
                bool GetLan = true;
                visitorIPAddress = _context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (String.IsNullOrEmpty(visitorIPAddress))
                    visitorIPAddress = _context.Request.ServerVariables["REMOTE_ADDR"];

                if (string.IsNullOrEmpty(visitorIPAddress))
                    visitorIPAddress = _context.Request.UserHostAddress;

                if (string.IsNullOrEmpty(visitorIPAddress) || visitorIPAddress.Trim() == "::1")
                {
                    GetLan = true;
                    visitorIPAddress = string.Empty;
                }

                if (GetLan)
                {
                    if (string.IsNullOrEmpty(visitorIPAddress))
                    {
                        //This is for Local(LAN) Connected ID Address
                        string stringHostName = Dns.GetHostName();
                        //Get Ip Host Entry
                        IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
                        //Get Ip Address From The Ip Host Entry Address List
                        System.Net.IPAddress[] arrIpAddress = ipHostEntries.AddressList;

                        try
                        {
                            if (arrIpAddress[arrIpAddress.Length - 1].AddressFamily.ToString() == "InterNetwork")
                                visitorIPAddress = arrIpAddress[arrIpAddress.Length - 1].ToString();
                            else
                                visitorIPAddress = arrIpAddress[0].ToString();
                        }
                        catch
                        {
                            try
                            {
                                visitorIPAddress = arrIpAddress[0].ToString();
                            }
                            catch
                            {
                                try
                                {
                                    arrIpAddress = Dns.GetHostAddresses(stringHostName);
                                    visitorIPAddress = arrIpAddress[0].ToString();
                                }
                                catch
                                {
                                    visitorIPAddress = "127.0.0.1";
                                }
                            }
                        }
                    }
                }
            }
            catch
            { }

            return visitorIPAddress;
        }

    }
}