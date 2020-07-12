using System;
using System.Net;
using System.Web;


namespace QREST.App_Logic.BusinessLogicLayer
{
    public static class GetIP
    {
        public static string GetLocalIPAddress(HttpContext _context)
        {
            string visitorIpAddress = "";

            try
            {
                visitorIpAddress = _context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(visitorIpAddress))
                    visitorIpAddress = _context.Request.ServerVariables["REMOTE_ADDR"];

                if (string.IsNullOrEmpty(visitorIpAddress))
                    visitorIpAddress = _context.Request.UserHostAddress;

                if (string.IsNullOrEmpty(visitorIpAddress) || visitorIpAddress.Trim() == "::1")
                {
                    visitorIpAddress = string.Empty;
                }

                if (string.IsNullOrEmpty(visitorIpAddress))
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
                            visitorIpAddress = arrIpAddress[arrIpAddress.Length - 1].ToString();
                        else
                            visitorIpAddress = arrIpAddress[0].ToString();
                    }
                    catch
                    {
                        try
                        {
                            visitorIpAddress = arrIpAddress[0].ToString();
                        }
                        catch
                        {
                            try
                            {
                                arrIpAddress = Dns.GetHostAddresses(stringHostName);
                                visitorIpAddress = arrIpAddress[0].ToString();
                            }
                            catch
                            {
                                visitorIpAddress = "127.0.0.1";
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            return visitorIpAddress;
        }

    }
}