using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

                List<T_QREST_SITES> _sites = db_Air.GetT_QREST_SITES_ReadyToPoll();
                if (_sites != null)
                {
                    foreach (T_QREST_SITES _site in _sites)
                    {
                        //get the polling config
                        T_QREST_SITE_POLL_CONFIG _config = db_Air.GetT_QREST_SITE_POLL_CONFIG_ActiveByID(_site.SITE_IDX);
                        if (_config != null)
                        {
                            WriteToFile("Starting poll for org:" + _site.ORG_ID + " site: " + _site.SITE_ID);


                            if (_config.LOGGER_TYPE == "ZENO")
                            {
                                bool SuccInd = Zeno(_config.LOGGER_PASSWORD);
                                if (SuccInd)
                                {
                                    //parse file
                                    ParseFile("");

                                    //update next run for the site
                                    DateTime nextrun = System.DateTime.Now.AddMinutes(15);  //default to 15 minutes next run
                                    if (_site.POLLING_FREQ_TYPE == "M")
                                        nextrun = System.DateTime.Now.AddMinutes(_site.POLLING_FREQ_NUM ?? 15);

                                    db_Air.InsertUpdatetT_QREST_SITES(_site.SITE_IDX, null, null, null, null, null, null, null, null, null, null, null, null,
                                        null, null, null, null, null, System.DateTime.Now, nextrun, null, null, null, null);
                                }
                            }

                            WriteToFile("Ending poll for org:" + _site.ORG_ID + " site: " + _site.SITE_ID);
                        }
                        else
                            WriteToFile("No config found for site");

                    }


                }
                else
                    WriteToFile("No sites ready for polling.");




                WriteToFile("Logger Polling process ended.");

                System.Threading.Thread.Sleep(120000); //2 minutes loop

            }
        }


        static bool Zeno(string pwd)
        {
            hawin32.HyperACCESS hawin;
            hawin32.IHAScript hascript;
            hawin = new hawin32.HyperACCESS();
            hascript = (hawin32.IHAScript)hawin.haInitialize("test");

            //restores the Hyperaccess GUI screen so it could be viewable while this script is running
            hascript.haSizeHyperACCESS(3);

            //Tell HyperACCESS which entry file to open
            hascript.haOpenSession(@"C:\temp\COOS_Conn3.HAW");

            //Starts the connection process within the current session.
            int ConnID = hascript.haConnectSession(0);
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
                hascript.haWait(3000);
                hascript.haXferReceive("data1.txt");
                hascript.haWait(3000);
                hascript.haTypeText(0, "Q\r\n");
                hascript.haWait(3000);
                hascript.haDisconnectSession();
                hascript.haMenuString("FX");

                return true;
            }
            else
            {
                WriteToFile("Unable to connect");
                hascript.haDisconnectSession();
                hascript.haMenuString("FX");

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


        public static void ParseFile(string file)
        {
            int DateColumn = 1; //zero-based
            int TimeColumn = 2; //zero-based



            string line;
            StreamReader sr = new StreamReader(@"C:\QREST_trans\Download\data1.txt");

            while ((line = sr.ReadLine()) != null)
            {
                string[] cols = line.Split(',');

                if (cols.Length > 1)  //skip blank rows
                {

                }
            }
        }
    }
}




//////********************TEST 3 SOCKET ******************************
////Create a TCPClient object at the IP and port
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
//stream.E();

//// Send password to the connected TcpServer. 
//bytesToSend = System.Text.Encoding.ASCII.GetBytes("password\r\n");
//stream.Write(bytesToSend, 0, bytesToSend.Length);

////Read back the text---
//bytesToRead = new byte[client.ReceiveBufferSize];
//bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);
//responseData = System.Text.Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);


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







//IPHostEntry ipHostInfo = Dns.GetHostEntry("1.1.1.1");
//IPAddress ipAddress = ipHostInfo.AddressList[0];
//IPEndPoint remoteEP = new IPEndPoint(ipAddress, 14109);


//// Create a TCP/IP  socket.  
//Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
//// Connect the socket to the remote endpoint. Catch any errors.  
//try
//{
//    sender.Connect(remoteEP);

//    Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());

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
