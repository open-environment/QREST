using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QREST_TaskConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                WriteToFile("Logger Polling process started.");

                List<SitePollingConfigType> _sites = db_Air.GetT_QREST_SITES_POLLING_CONFIG_ReadyToPoll();
                if (_sites != null && _sites.Count>0)
                {
                    foreach (SitePollingConfigType _site in _sites)
                    {
                        string fileSite = _site.ORG_ID.Substring(0,2) + "_" + _site.SITE_IDX.ToString().Substring(0, 8);
                        
                        List<SitePollingConfigDetailType> _config_dtl = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID_Simple(_site.POLL_CONFIG_IDX);
                        if (_config_dtl != null && _config_dtl.Count > 0)
                        {
                            WriteToFile("Starting poll for org:" + _site.ORG_ID + " site: " + _site.SITE_ID);

                            //****************** ZENO DATA LOGGER *********************************************************
                            //****************** ZENO DATA LOGGER *********************************************************
                            //****************** ZENO DATA LOGGER *********************************************************
                            if (_site.LOGGER_TYPE == "ZENO")
                            {
                                bool SuccInd = ConnectZeno(fileSite, _site.LOGGER_SOURCE, _site.LOGGER_PORT ?? 23, _site.LOGGER_PASSWORD);
                                if (SuccInd)
                                    ParseFile(fileSite, _site, _config_dtl, _site.POLLING_FREQ_TYPE, _site.POLLING_FREQ_NUM, _site.SITE_IDX);
                            }

                            //****************** SUTRON DATA LOGGER *********************************************************
                            //****************** SUTRON DATA LOGGER *********************************************************
                            //****************** SUTRON DATA LOGGER *********************************************************
                            if (_site.LOGGER_TYPE == "SUTRON")
                            {
                                    
                            }

                            WriteToFile("Ending poll for org:" + _site.ORG_ID + " site: " + _site.SITE_ID);
                        }
                        else
                            WriteToFile("No column mappings found for polling configuration");
                    }
                }
                else
                    WriteToFile("No sites ready for polling.");

                WriteToFile("Logger Polling process ended.");

                System.Threading.Thread.Sleep(120000); //2 minutes loop
            }
        }


        static bool ConnectZeno(string siteID, string host, int port, string pwd)
        {
            hawin32.HyperACCESS hawin;
            hawin32.IHAScript hascript;
            hawin = new hawin32.HyperACCESS();
            hascript = (hawin32.IHAScript)hawin.haInitialize("test");

            //restores the Hyperaccess GUI screen so it could be viewable while this script is running
            hascript.haSizeHyperACCESS(3);

            //Tell HyperACCESS which entry file to open
            hascript.haOpenSession(@"C:\qrest_trans\conns\zeno_conn.HAW");

            //Programmatically set destination
            hascript.haSetDestination(host, 0);
            hascript.haSetDestination(port.ToString(), 1);

            //Starts the connection process within the current session.
            int ConnID = hascript.haConnectSession(0);
            if (ConnID == 0)  //if successful
            {
                hascript.haWait(1000);

                int isConnected = hascript.haGetConnectionStatus();

                if (isConnected == 1)
                {
                    //interact with terminal
                    hascript.haWait(3000);
                    hascript.haTypeText(0, "u\r\n");
                    hascript.haWait(3000);
                    hascript.haTypeText(0, pwd + "\r\n");
                    hascript.haWait(3000);
                    hascript.haTypeText(0, "D\r\n");
                    hascript.haWait(3000);
                    hascript.haTypeText(0, "XL20\r\n");
                    hascript.haSetXferProtocol(2, 5);
                    hascript.haWait(7000);
                    hascript.haXferReceive("data_" + siteID + ".txt");
                    hascript.haWait(7000);
                    hascript.haTypeText(0, "Q\r\n");
                    hascript.haWait(3000);
                    hascript.haDisconnectSession();
                    hascript.haMenuString("FX");

                    return true;
                }
                else
                {
                    WriteToFile("Unable to connect to Zeno");
                    hascript.haDisconnectSession();
                    hascript.haMenuString("FX");

                    return false;
                }
            }
            else
            {
                WriteToFile("Unable to connect to Zeno. Err 2");
                hascript.haMenuString("FX");

                return false;
            }

        }


        public static bool ParseFile(string siteID, SitePollingConfigType config, List<SitePollingConfigDetailType> config_dtl, string pollFreqType, int? pollFreqNum, Guid siteIDX)
        {
            try
            {
                string line;
                using (StreamReader sr = new StreamReader(@"C:\qrest_trans\Download\data_" + siteID + ".txt"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length > 0)
                        {
                            //FIVE MINUTE RAW DATA
                            if (config.RAW_DURATION_CODE == "H")
                                db_Air.InsertT_QREST_DATA_FIVE_MIN_fromLine(line, config, config_dtl);
                            //ONE MINUTE RAW DATA
                            //if (config.RAW_DURATION_CODE == "G")
                            //    db_Air.InsertT_QREST_DATA_ONE_MIN_fromLine(line, config, config_dtl);
                        }
                    }
                }

                //update next run for the site
                DateTime nextrun = System.DateTime.Now.AddMinutes(15);  //default to 15 minutes next run
                if (pollFreqType == "M")
                    nextrun = System.DateTime.Now.AddMinutes(pollFreqNum ?? 15);

                db_Air.InsertUpdatetT_QREST_SITES(siteIDX, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 
                    System.DateTime.Now, nextrun, null, null, null, null);

                return true;
            }
            catch (Exception ex)
            {
                db_Ref.CreateT_QREST_SYS_LOG(null, "POLLING", ex.InnerException?.ToString());
                return false;
            }
        }


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

        #region testingOld

        //public static void tryTCP()
        //{
        //// Initial/default values
        //string serverAddress = "1.1.1.1";
        //ushort serverPort = 14109;
        //int bufferSize = 1024;

        //TcpClient simpleTcp = null;
        //NetworkStream tcpStream = null;
        //byte[] sendBuffer = new byte[bufferSize], receiveBuffer = new byte[bufferSize], byteCount;
        //int bytesToRead = 0, nextReadCount, rc;

        //try
        //{
        //    // Create the client and indicate the server to connect to
        //   Console.WriteLine("TCP client: Creating the client and indicate the server to connect to...");
        //    simpleTcp = new TcpClient(serverAddress, (int)serverPort);

        //    // Retrieve the NetworkStream so we can do Read and Write
        //    Console.WriteLine("TCP client: Retrieving the NetworkStream so we can do Read and Write...");
        //    tcpStream = simpleTcp.GetStream();

        //    // First send the number of bytes the client is sending
        //    Console.WriteLine("TCP client: Sending the number of bytes the client is sending...");

        //            //Byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes("u\r\n");
        //            //stream.Write(bytesToSend, 0, bytesToSend.Length);

        //    byteCount = BitConverter.GetBytes(sendBuffer.Length);
        //    tcpStream.Write(byteCount, 0, byteCount.Length);

        //    // Send the actual data
        //    Console.WriteLine("TCP client: Sending the actual data...");
        //    tcpStream.Write(sendBuffer, 0, sendBuffer.Length);
        //    tcpStream.Read(byteCount, 0, byteCount.Length);

        //    // Read how many bytes the server is responding with
        //    Console.WriteLine("TCP client: Reading how many bytes the server is responding with...");
        //    bytesToRead = BitConverter.ToInt32(byteCount, 0);

        //    // Receive the data
        //    Console.WriteLine("TCP client: Receiving, reading & displaying the data...");
        //    while (bytesToRead > 0)
        //    {
        //        // Make sure we don't read beyond what the first message indicates
        //        //    This is important if the client is sending multiple "messages" --
        //        //    but in this sample it sends only one
        //        if (bytesToRead < receiveBuffer.Length)
        //            nextReadCount = bytesToRead;
        //        else
        //            nextReadCount = receiveBuffer.Length;

        //        // Read the data
        //        rc = tcpStream.Read(receiveBuffer, 0, nextReadCount);

        //        // Display what was read
        //        string readText = System.Text.Encoding.ASCII.GetString(receiveBuffer, 0, rc);
        //        Console.WriteLine("TCP client: Received: {0}", readText);
        //        bytesToRead -= rc;
        //    }
        //}
        //catch (SocketException err)
        //{
        //    // Exceptions on the TcpListener are caught here
        //    Console.WriteLine("TCP client: Socket error occurred: {0}", err.Message);
        //}
        //catch (System.IO.IOException err)
        //{
        //    // Exceptions on the NetworkStream are caught here
        //    Console.WriteLine("TCP client: I/O error: {0}", err.Message);
        //}
        //finally
        //{
        //    // Close any remaining open resources
        //    Console.WriteLine("TCP client: Closing all the opening resources...");
        //    if (tcpStream != null)
        //        tcpStream.Close();
        //    if (simpleTcp != null)
        //        simpleTcp.Close();
        //}





        //////********************TEST 3 SOCKET ******************************
        //Create a TCPClient object at the IP and port
        //TcpClient client = new TcpClient("1.1.1.1", 14109);

        //// Get a client stream for reading and writing.
        //NetworkStream stream = client.GetStream();



        //// Send "U" to the connected TcpServer. 
        //Byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes("u\r\n");
        //stream.Write(bytesToSend, 0, bytesToSend.Length);

        ////Read back the text---
        //byte[] bytesToRead = new byte[client.ReceiveBufferSize];
        //int bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);
        //string responseData = System.Text.Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);




        //// Send password to the connected TcpServer. 
        //Byte[] bytesToSend2 = System.Text.Encoding.ASCII.GetBytes("******\r\n");
        //stream.Write(bytesToSend2, 0, bytesToSend2.Length);

        ////Read back the text---
        //byte[] bytesToRead2 = new byte[client.ReceiveBufferSize];
        //int bytesRead2 = stream.Read(bytesToRead2, 0, client.ReceiveBufferSize);
        //string responseData2 = System.Text.Encoding.ASCII.GetString(bytesToRead2, 0, bytesRead2);


        //// Send "D" to the connected TcpServer. 
        //bytesToSend = System.Text.Encoding.ASCII.GetBytes("\r\n");
        //stream.Write(bytesToSend, 0, bytesToSend.Length);

        //bytesToSend = System.Text.Encoding.ASCII.GetBytes("D\r\n");
        //stream.Write(bytesToSend, 0, bytesToSend.Length);

        ////Read back the text---
        //bytesToRead = new byte[client.ReceiveBufferSize];
        //bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);
        //responseData = System.Text.Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);


        //// Send "Q" to the connected TcpServer. 
        //bytesToSend = System.Text.Encoding.ASCII.GetBytes("Q\r\n");
        //stream.Write(bytesToSend, 0, bytesToSend.Length);

        ////Read back the text---
        //bytesToRead = new byte[client.ReceiveBufferSize];
        //bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);
        //responseData = System.Text.Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

        //// Close everything.
        //stream.Close();
        //client.Close();

        //}


        //static void Connect(String server, String message)
        //{
        //    try
        //    {
        //        // Create a TcpClient.
        //        // Note, for this client to work you need to have a TcpServer 
        //        // connected to the same address as specified by the server, port
        //        // combination.
        //        Int32 port = 14109;
        //        TcpClient client = new TcpClient(server, port);

        //        // Translate the passed message into ASCII and store it as a Byte array.
        //        Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

        //        // Get a client stream for reading and writing.
        //        NetworkStream stream = client.GetStream();

        //        // Send the message to the connected TcpServer. 
        //        stream.Write(data, 0, data.Length);

        //        Console.WriteLine("Sent: {0}", message);

        //        // Receive the TcpServer.response.

        //        // Buffer to store the response bytes.
        //        data = new Byte[256];

        //        // String to store the response ASCII representation.
        //        String responseData = String.Empty;

        //        // Read the first batch of the TcpServer response bytes.
        //        Int32 bytes = stream.Read(data, 0, data.Length);
        //        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        //        Console.WriteLine("Received: {0}", responseData);

        //        // Read the second batch of the TcpServer response bytes.
        //        bytes = stream.Read(data, 0, data.Length);
        //        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        //        Console.WriteLine("Received: {0}", responseData);

        //        // Close everything.
        //        stream.Close();
        //        client.Close();
        //    }
        //    catch (ArgumentNullException e)
        //    {
        //        Console.WriteLine("ArgumentNullException: {0}", e);
        //    }
        //    catch (SocketException e)
        //    {
        //        Console.WriteLine("SocketException: {0}", e);
        //    }

        //    Console.WriteLine("\n Press Enter to continue...");
        //    Console.Read();
        //}

        #endregion
    }
}




//IPHostEntry ipHostInfo = Dns.GetHostEntry("1.1.1.1");
//IPAddress ipAddress = ipHostInfo.AddressList[0];
//IPEndPoint remoteEP = new IPEndPoint(ipAddress, 14109);

//// Create a TCP/IP  socket.  
//Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
//// Connect the socket to the remote endpoint. Catch any errors.  
//try
//{
//    sender.Connect(remoteEP);

//    // Encode the data string into a byte array.  
//    byte[] msg = Encoding.ASCII.GetBytes("u\r\n");

//    // Send the data through the socket.  
//    int bytesSent = sender.Send(msg);

//    // Receive the response from the remote device.  
//    int bytesRec = sender.Receive(bytes);
//    Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));

//    // Release the socket.  
//    sender.Shutdown(SocketShutdown.Both);
//    sender.Close();

//}
//catch (ArgumentNullException ane)
//{
//    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
//}
//catch (SocketException se)
//{
//    Console.WriteLine("SocketException : {0}", se.ToString());
//}
//catch (Exception e)
//{
//    Console.WriteLine("Unexpected exception : {0}", e.ToString());
//}
