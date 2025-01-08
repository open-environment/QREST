using Newtonsoft.Json.Linq;
using QRESTModel.BLL;
using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QRESTModel.COMM
{
    public class CommMessageLog {
        public string CommMessageType { get; set; }
        public bool CommMessageStatus { get; set; }
        public string CommResponse { get; set; }
    }

    public static class LoggerComm
    {
        /// <summary>
        /// Connect to Zeno User Interface (Log in and log out only)
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static List<CommMessageLog> ConnectTcpClientPing(string ip, ushort port, string pwd )
        {
            var log = new List<CommMessageLog>();

            // This discards any pending data and Winsock resets the connection.
            LingerOption lingerOption = new LingerOption(true, 0);

            //Create a TCPClient object at the IP and port
            using (TcpClient client = new TcpClient { SendTimeout = 2000, ReceiveTimeout = 2000, LingerState = lingerOption })
            {
                try
                {
                    if (!client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(2)))
                        log.Add(new CommMessageLog { CommMessageStatus = false, CommMessageType = "Connect", CommResponse= "" });
                    else
                    {
                        log.Add(new CommMessageLog { CommMessageStatus = true, CommMessageType = "Connect", CommResponse = "" });

                        // Get a client stream for reading and writing.
                        using (NetworkStream stream = client.GetStream())
                        {
                            //*************** send u to enter user menu *****************************
                            CommMessageLog _log = LoggerComm.SendReceiveMessage_CheckSuccessAndLog("Access Zeno User Menu", stream, "u\r\n", 700, 700, false, "Level 1");
                            log.Add(_log);

                            if (_log.CommMessageStatus)
                            {
                                //*************** enter password when prompted *****************************
                                CommMessageLog _log2 = LoggerComm.SendReceiveMessage_CheckSuccessAndLog("Enter password", stream, pwd + "\r", 700, 700, false, "ommun");
                                log.Add(_log2);
                            }

                            //*************** quit zeno user menu*****************************
                            CommMessageLog _log5 = LoggerComm.SendReceiveMessage_CheckSuccessAndLog("Exiting Zeno User Menu", stream, "Q\r", 1500, 1500, false, "Exiting");
                            log.Add(_log5);

                            //disconnect
                            client.Client.Close();
                            System.Threading.Thread.Sleep(250);
                            client.Close();
                        }
                    }

                }
                catch (SocketException sex)
                {
                    //disconnect if exception
                    client.Client.Close();
                    System.Threading.Thread.Sleep(250);
                    client.Close();

                    log.Add(new CommMessageLog { CommMessageStatus = false, CommMessageType = "Ping Socket Exception 1", CommResponse = sex.Message });
                }
                catch (Exception ex)
                {
                    //disconnect if exception
                    client.Client.Close();
                    System.Threading.Thread.Sleep(250);
                    client.Close();

                    log.Add(new CommMessageLog { CommMessageStatus = false, CommMessageType = "Ping Socket Exception 3", CommResponse = ex.Message });
                    log.Add(new CommMessageLog { CommMessageStatus = false, CommMessageType = "Ping Socket Exception 3", CommResponse = ex.InnerException.ToString() });
                }
            }

            return log;
        }


        /// <summary>
        /// Creates a TCP Connection to a data logger, and then uses a SAILER command to connect to a ZENO or SUTRON data logger and return the logger response, based on the input message.
        /// </summary>
        /// <param name="ip">Data logger IP address</param>
        /// <param name="port">Data logger port</param>
        /// <param name="message">CCSAILER command to issue to logger</param>
        /// <param name="siteID">Four digit site ID that the logger uses to define the site</param>
        /// <returns></returns>
        public static CommMessageLog ConnectTcpClientSailer(string ip, ushort port, string message, string siteID, int? postSend = null)
        {
            var log = new CommMessageLog();

            // This dictates that the socket will not longer open after the socket is closed
            LingerOption lingerOption = new LingerOption(true, 0);

            //Create a TCPClient object at the IP and port
            using (TcpClient client = new TcpClient { SendTimeout = 5000, ReceiveTimeout = 10000, LingerState = lingerOption })
            {
                try
                {
                    if (!client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(2)))
                        log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Connect", CommResponse = "" };
                    else
                    {
                        // Get a client stream for reading and writing.
                        using (NetworkStream stream = client.GetStream())
                        {
                            //*************** send sailer message *****************************
                            string xxx = SendReceiveMessage(stream, "#" + siteID + "0001" + message, 700, postSend ?? 700, true);
                            //string xxx = Task.Run(() => SendReceiveMessageAsync(stream, "#" + siteID + "0001" + message, 700, postSend ?? 700, true)).Result;

                            if (xxx != null && xxx.Length > 10)
                            {
                                xxx = stripMessage(xxx, "#0001" + siteID); //strip unnecessary stuff from beginning of file (TO DO replace with validity check)
                                log = new CommMessageLog { CommMessageStatus = true, CommMessageType = "Data", CommResponse = xxx };
                            }
                            else
                                log = new CommMessageLog { CommMessageStatus = false, CommMessageType = xxx, CommResponse = "" };

                            //disconnect
                            client.Client.Close();
                            System.Threading.Thread.Sleep(250);
                            client.Close();
                        }
                    }

                }
                catch (SocketException sex)
                {
                    //disconnect if exception
                    client.Client.Close();
                    System.Threading.Thread.Sleep(250);
                    client.Close();
                    log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Ping Socket Exception 1", CommResponse = sex.Message };
                }
                catch (Exception ex)
                {
                    //disconnect if exception
                    client.Client.Close();
                    System.Threading.Thread.Sleep(250);
                    client.Close();

                    log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Ping General Exception 2", CommResponse = ex.Message };
                }
            }

            return log;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="command">DDDHHMMSS|Y|DDDHHMMSS</param>
        /// <param name="siteID"></param>
        /// <returns></returns>
        public static CommMessageLog ConnectTcpESC(string ip, ushort port, string command, string siteID)
        {
            var log = new CommMessageLog();

            //Create a TCPClient object at the IP and port
            using (TcpClient client = new TcpClient { SendTimeout = 2000, ReceiveTimeout = 2000, LingerState = new LingerOption(true, 0) })
            {
                try
                {
                    if (!client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(10)))
                        log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Connect", CommResponse = "" };
                    else
                    {
                        // Get a client stream for reading and writing.
                        using (NetworkStream stream = client.GetStream())
                        {//@TM!5600001H215010000|Y|215230000&$
                            //*************** send message and get response *****************************
                            string xxx = SendReceiveMessage(stream, "@" + siteID + "!5600" + "001H" + command + "&$", 700, 700, false);
                            if (xxx != null && xxx.Length > 10)
                            {
                                xxx = xxx.Replace("&", System.Environment.NewLine);
                                xxx = xxx.Replace("@" + siteID + "a", "");
                                xxx = stripMessage(xxx, "#0001" + siteID); //strip unnecessary stuff from beginning of file (TO DO replace with validity check)
                                log = new CommMessageLog { CommMessageStatus = true, CommMessageType = "Data", CommResponse = xxx };
                            }
                            else
                                log = new CommMessageLog { CommMessageStatus = false, CommMessageType = xxx, CommResponse = "" };


                            //disconnect
                            client.Client.Close();
                            System.Threading.Thread.Sleep(250);
                            client.Close();
                        }
                    }

                }
                catch (SocketException sex)
                {
                    //disconnect if exception
                    client.Client.Close();
                    System.Threading.Thread.Sleep(250);
                    client.Close();
                    log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Ping Socket Exception 1", CommResponse = sex.Message };
                }
                catch (Exception ex)
                {
                    //disconnect if exception
                    client.Client.Close();
                    System.Threading.Thread.Sleep(250);
                    client.Close();
                    log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Ping General Exception 2", CommResponse = ex.Message };
                }
            }

            return log;
        }


        /// <summary>
        /// Creates a TCP Connection to a Sutron data logger with LEADs module enabled, and then uses a command to return the logger response, based on the input message.
        /// </summary>
        /// <param name="ip">Data logger IP address</param>
        /// <param name="port">Data logger port</param>
        /// <param name="message">command to issue to logger</param>
        /// <param name="siteID">Four digit site ID that the logger uses to define the site</param>
        /// <returns></returns>
        public static CommMessageLog ConnectTcpSutron(string ip, ushort port, string username, string password, string message)
        {
            var log = new CommMessageLog();

            // This dictates that the socket will not longer open after the socket is closed
            LingerOption lingerOption = new LingerOption(true, 0);

            //Create a TCPClient object at the IP and port
            using (TcpClient client = new TcpClient { SendTimeout = 2000, ReceiveTimeout = 2000, LingerState = lingerOption })
            {
                try
                {
                    if (!client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(2)))
                        log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Connect", CommResponse = "" };
                    else
                    {
                        // Get a client stream for reading and writing.
                        using (NetworkStream stream = client.GetStream())
                        {
                            //*************** send username *****************************
                            string _usrResp = SendReceiveMessage(stream, username + "\r", 700, 700, false);
                            if (_usrResp != null && _usrResp.Length > 10 && _usrResp.Contains("assword"))
                            {
                                string _pwdResp = SendReceiveMessage(stream, password + "\r", 700, 700, false);
                                if (_pwdResp != null && _pwdResp.Length > 10 && _pwdResp.Contains("Flash"))
                                {
                                    string xxx = SendReceiveMessage(stream, message + "\r", 700, 700, false);
                                    if (xxx != null && xxx.Length > 10)
                                    {
                                        //xxx = stripMessage(xxx, "#0001" + siteID); //strip unnecessary stuff from beginning of file (TO DO replace with validity check)
                                        xxx = xxx.Replace(' ', ','); //change dates from space delimited to comma
                                        string[] lines = xxx.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Skip(2).ToArray();
                                        xxx = string.Join(Environment.NewLine, lines);

                                        log = new CommMessageLog { CommMessageStatus = true, CommMessageType = "Data", CommResponse = xxx };
                                    }
                                    else
                                        log = new CommMessageLog { CommMessageStatus = false, CommMessageType = xxx, CommResponse = "" };
                                }
                                else
                                    log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Username/password failed", CommResponse = "" };
                            }
                            else
                                log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Username/password failed", CommResponse = "" };

                            //disconnect
                            client.Client.Close();
                            System.Threading.Thread.Sleep(250);
                            client.Close();
                        }
                    }

                }
                catch (SocketException sex)
                {
                    //disconnect if exception
                    client.Client.Close();
                    System.Threading.Thread.Sleep(250);
                    client.Close();
                    log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Ping Socket Exception 1", CommResponse = sex.Message };
                }
                catch (Exception ex)
                {
                    //disconnect if exception
                    client.Client.Close();
                    System.Threading.Thread.Sleep(250);
                    client.Close();

                    log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Ping General Exception 2", CommResponse = ex.Message };
                }
            }

            return log;
        }


        /// <summary>
        /// Creates a TCP Connection to a Met One BAM 1020, and then uses a command to return the logger response.
        /// </summary>
        /// <param name="ip">Data logger IP address</param>
        /// <param name="port">Data logger port</param>
        /// <returns></returns>
        public static CommMessageLog ConnectTcpBAM(string ip, ushort port)
        {
            var log = new CommMessageLog();

            // This dictates that the socket will not longer open after the socket is closed
            LingerOption lingerOption = new LingerOption(true, 0);

            //Create a TCPClient object at the IP and port
            using (TcpClient client = new TcpClient { SendTimeout = 2000, ReceiveTimeout = 2000, LingerState = lingerOption })
            {
                try
                {
                    if (!client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(2)))
                        log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Connect", CommResponse = "" };
                    else
                    {
                        // Get a client stream for reading and writing.
                        using (NetworkStream stream = client.GetStream())
                        {
                            //*************** send three enters *****************************
                            string _usrResp = SendReceiveMessage(stream, "\r\n\r\n\r\n", 700, 700, false);
                            if (_usrResp != null && _usrResp.Contains("*"))
                            {
                                //view menu to see what kind of BAM1020 we're dealing with
                                string getMenu = SendReceiveMessage(stream, "h" + "\r", 700, 700, false);  //h means menu
                                if (getMenu != null && getMenu.Length > 10 && getMenu.Contains("Display Current Day"))
                                {
                                    //daily report option
                                    string xxx = SendReceiveMessage(stream, "1" + "\r", 700, 700, false);  //1 means all data for today
                                    if (xxx != null && xxx.Length > 10)
                                    {
                                        string[] lines = xxx.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Skip(2).ToArray();
                                        xxx = string.Join(Environment.NewLine, lines);

                                        log = new CommMessageLog { CommMessageStatus = true, CommMessageType = "Data", CommResponse = xxx };
                                    }
                                    else
                                        log = new CommMessageLog { CommMessageStatus = false, CommMessageType = xxx, CommResponse = "" };
                                }
                                else
                                {
                                    //set pointer (how many hours of data to retrieve
                                    string _goPointer = SendReceiveMessage(stream, "9", 700, 700, false);  //hit 9 for ...
                                    //byte[] bytestosend = { 0x1B };
                                    string _ignore1 = SendReceiveMessage(stream, "3 6\r", 700, 700, false, true);  //hit escape then 3 is the hourly data file, then 6 is number of hours
                                    string _ignore2 = SendReceiveMessage(stream, "\r\n\r\n\r\n", 700, 700, false);  //hit enter 3 times to return to menu
                                    string _csvdata = SendReceiveMessage(stream, "6\r\n", 700, 700, false);  //hit 6 for CSV data then 3 to get data
                                    string _csvdata2 = SendReceiveMessage(stream, "3\r\n ", 700, 700, false);  //3 to get new data
                                    if (_csvdata2 != null && _csvdata2.Length > 10)
                                    {
                                        string[] lines = _csvdata2.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Skip(2).ToArray();
                                        _csvdata2 = string.Join(Environment.NewLine, lines);

                                        log = new CommMessageLog { CommMessageStatus = true, CommMessageType = "Data", CommResponse = _csvdata2 };
                                    }
                                    else
                                        log = new CommMessageLog { CommMessageStatus = false, CommMessageType = _csvdata2, CommResponse = "" };
                                }
                            }
                            else
                                log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Username/password failed", CommResponse = "" };

                            //disconnect
                            client.Client.Close();
                            System.Threading.Thread.Sleep(250);
                            client.Close();
                        }
                    }

                }
                catch (SocketException sex)
                {
                    //disconnect if exception
                    client.Client.Close();
                    System.Threading.Thread.Sleep(250);
                    client.Close();
                    log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Ping Socket Exception 1", CommResponse = sex.Message };
                }
                catch (Exception ex)
                {
                    //disconnect if exception
                    client.Client.Close();
                    System.Threading.Thread.Sleep(250);
                    client.Close();

                    log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Ping General Exception 2", CommResponse = ex.Message };
                }
            }

            return log;
        }


        /// <summary>
        /// Creates a TCP Connection to a Met One BAM 1022, and then uses a command to return the logger response.
        /// </summary>
        /// <param name="ip">Data logger IP address</param>
        /// <param name="port">Data logger port</param>
        /// <returns></returns>
        public static CommMessageLog ConnectTcpBAM1022(string ip, ushort port, int? numRecs)
        {
            var log = new CommMessageLog();

            // This dictates that the socket will not longer open after the socket is closed
            LingerOption lingerOption = new LingerOption(true, 0);

            //Create a TCPClient object at the IP and port
            using (TcpClient client = new TcpClient { SendTimeout = 2000, ReceiveTimeout = 2000, LingerState = lingerOption })
            {
                try
                {
                    if (!client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(2)))
                        log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Connect", CommResponse = "" };
                    else
                    {
                        // Get a client stream for reading and writing.
                        using (NetworkStream stream = client.GetStream())
                        {
                            //*************** send three enters *****************************
                            string _usrResp = SendReceiveMessage(stream, "\r\n\r\n\r\n", 700, 700, false);
                            if (_usrResp != null && _usrResp.Contains("*"))
                            {
                                string numRecsStr = (numRecs ?? 10).ToString();
                                //if numrecs == -1, then query all
                                string recsCommand = numRecs == -1 ? "2\r" : "4 " + numRecsStr + "\r";
                                //set pointer (how many hours of data to retrieve
                                string _csvdata2 = SendReceiveMessage(stream, recsCommand, 700, 700, false);  //hit 4 10 to retrieve last 10 records
                                if (_csvdata2 != null && _csvdata2.Length > 10)
                                {
                                    string[] lines = _csvdata2.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Skip(2).ToArray();
                                    _csvdata2 = string.Join(Environment.NewLine, lines);

                                    log = new CommMessageLog { CommMessageStatus = true, CommMessageType = "Data", CommResponse = _csvdata2 };
                                }
                                else
                                    log = new CommMessageLog { CommMessageStatus = false, CommMessageType = _csvdata2, CommResponse = "" };

                            }
                            else
                                log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Username/password failed", CommResponse = "" };

                            //disconnect
                            client.Client.Close();
                            System.Threading.Thread.Sleep(250);
                            client.Close();
                        }
                    }

                }
                catch (SocketException sex)
                {
                    //disconnect if exception
                    client.Client.Close();
                    System.Threading.Thread.Sleep(250);
                    client.Close();
                    log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Ping Socket Exception 1", CommResponse = sex.Message };
                }
                catch (Exception ex)
                {
                    //disconnect if exception
                    client.Client.Close();
                    System.Threading.Thread.Sleep(250);
                    client.Close();

                    log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Ping General Exception 2", CommResponse = ex.Message };
                }
            }

            return log;
        }


        /// <summary>
        /// Sends a message on NetworkStream and listens for response. Continues listening until response stops writing or cancelation token is reached. 
        /// </summary>
        /// <param name="stream">Established network stream / socket</param>
        /// <param name="message">Message to send</param>
        /// <param name="msWaitPreSend">Wait in milliseconds before sending message</param>
        /// <param name="msWaitPostSend">Wait in milliseconds between sending message and reading response</param>
        /// <param name="appendEOFInd">Indicates whether an End Of File ascii character should be appended to the end of the message</param>
        /// <returns>Network stream response</returns>
        public static string SendReceiveMessage(NetworkStream stream, string message, int? msWaitPreSend, int? msWaitPostSend, bool appendEOFInd, bool prependEscape = false)
        {
            string resp = "init";
            string respPrev;

            //cancel service call if takes longer than 1 minute
            CancellationTokenSource _cancelToken = new CancellationTokenSource(60000);

            while (!_cancelToken.IsCancellationRequested)
            {
                // ****************Send message to the connected TcpServer ********************************
                Thread.Sleep(msWaitPreSend ?? 100); //wait for socket to be ready write data

                byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(message);

                if (appendEOFInd)
                {
                    //******** ADD MODULO-100 CHECKSUM **************
                    int byteSum = 0;
                    for (int i = 1; i <= bytesToSend.Length-1; i++)
                        byteSum += bytesToSend[i];
                    int mod = byteSum % 100;

                    var ascii = Encoding.ASCII;
                    bytesToSend = Encoding.ASCII.GetBytes(message +  mod.ToString())
                        .Concat(ascii.GetBytes(new[] { (char)3 }))
                        .ToArray();
                }

                if (prependEscape)
                {
                    byte[] newValues = new byte[bytesToSend.Length + 1];
                    newValues[0] = 0x1B;                                // set the prepended value
                    Array.Copy(bytesToSend, 0, newValues, 1, bytesToSend.Length); // copy the old values
                    bytesToSend = newValues;
                }
                stream.Write(bytesToSend, 0, bytesToSend.Length);

                // ****************Read response after the send command ********************************
                Thread.Sleep(msWaitPostSend ?? 100); //wait for socket to begin writing response
                var ms = new MemoryStream();
                byte[] data = new byte[8192];
                int numBytesRead;
                do
                {
                    respPrev = resp;

                    stream.ReadTimeout = 5000;   // UPDATE INCREASE TIMEOUT
                    numBytesRead = stream.Read(data, 0, data.Length);
                    ms.Write(data, 0, numBytesRead);
                    resp = Encoding.ASCII.GetString(ms.ToArray());

                    Thread.Sleep(1500);  //wait before reading again to let logger write out more if still writing
                } while (stream.DataAvailable && respPrev != resp);
                
                return resp;
            }
            return "TIMEOUT:" + resp;
        }


        public static async Task<string> SendReceiveMessageAsync(NetworkStream stream, string message, int? msWaitPreSend, int? msWaitPostSend, bool appendEOFInd, bool prependEscape = false)
        {
            string response = "init";
            string responsePrev;

            //cancel service call if takes longer than 1 minute
            using (var cancelTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1)))
            {
                try
                {
                    //wait for socket to be ready write data
                    await Task.Delay(msWaitPreSend ?? 100, cancelTokenSource.Token);

                    byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(message);

                    if (appendEOFInd)
                        bytesToSend = AddChecksumAndEOF(bytesToSend, message);

                    if (prependEscape)
                        bytesToSend = PrependEscapeCharacter(bytesToSend);

                    await stream.WriteAsync(bytesToSend, 0, bytesToSend.Length, cancelTokenSource.Token);

                    //wait for socket to begin writing response
                    await Task.Delay(msWaitPostSend ?? 100, cancelTokenSource.Token);

                    using (var ms = new MemoryStream())
                    {
                        byte[] data = new byte[8192];
                        int numBytesRead;

                        do
                        {
                            responsePrev = response;

                            stream.ReadTimeout = 5000;
                            numBytesRead = await stream.ReadAsync(data, 0, data.Length, cancelTokenSource.Token);
                            ms.Write(data, 0, numBytesRead);
                            response = Encoding.ASCII.GetString(ms.ToArray());

                            await Task.Delay(1500, cancelTokenSource.Token);
                        } while (stream.DataAvailable && responsePrev != response);

                        return response;
                    }
                }
                catch (OperationCanceledException)
                {
                    return "TIMEOUT:" + response;
                }
                catch (Exception ex)
                {
                    return "ERROR:" + ex.Message;
                }
            }
        }


        private static byte[] AddChecksumAndEOF(byte[] bytesToSend, string message)
        {
            int byteSum = bytesToSend.Skip(1).Sum(b => b);
            int mod = byteSum % 100;

            var ascii = Encoding.ASCII;
            return Encoding.ASCII.GetBytes(message + mod.ToString())
                   .Concat(ascii.GetBytes(new[] { (char)3 }))
                   .ToArray();
        }

        private static byte[] PrependEscapeCharacter(byte[] bytesToSend)
        {
            byte[] newValues = new byte[bytesToSend.Length + 1];
            newValues[0] = 0x1B;
            Array.Copy(bytesToSend, 0, newValues, 1, bytesToSend.Length);
            return newValues;
        }


        public static CommMessageLog SendReceiveMessage_CheckSuccessAndLog(string commMessageType, NetworkStream stream, string message, int? msWaitPreSend, int? msWaitPostSend, bool appendEOFInd, string checkResponseContains)
        {
            string xxx = SendReceiveMessage(stream, message, msWaitPreSend, msWaitPostSend, appendEOFInd);
            return new CommMessageLog
            {
                CommMessageType = commMessageType,
                CommMessageStatus = xxx.Contains(checkResponseContains),
                CommResponse = xxx
            };
        }


        public static string stripMessage(string inputMsg, string match)
        {
            int resultIndex = inputMsg.IndexOf(match);
            if (resultIndex != -1)
                inputMsg = inputMsg.Remove(0, resultIndex + 11);

            return inputMsg;
        }


        /// <summary>
        /// Parses an entire polling file (either tab separated or comma separated) into the raw FIVE_MINUTE or HOURLY table, based on a provided polling configuration. 
        /// Can optionally update the next time polling should run.
        /// </summary>
        /// <param name="loggerData"></param>
        /// <param name="config"></param>
        /// <param name="updateNextRunTime"></param>
        /// <returns>True only if ran successfully.</returns>
        public static bool ParseFlatFile(string loggerData, SitePollingConfigType config, List<SitePollingConfigDetailType> _config_dtl, bool updateNextRunTime, bool? overrideConfigDuration = false, bool insertsOnly = false)
        {
            try
            {
                T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(config.SITE_IDX);

                string line;
                bool SuccInd = true;
                using (System.IO.StringReader sr = new System.IO.StringReader(loggerData))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length > 20 && line.Contains("ConcR") == false)
                        {
                            //FIVE MINUTE RAW DATA
                            if (config.RAW_DURATION_CODE == "H" || overrideConfigDuration == true)
                            {
                                SuccInd = db_Air.InsertT_QREST_DATA_FIVE_MIN_fromLine(line, config, _config_dtl, insertsOnly);
                            }
                            //HOURLY RAW DATA
                            else if (config.RAW_DURATION_CODE == "1")
                            {
                                SuccInd = db_Air.InsertT_QREST_DATA_HOURLY_fromLine(line, config, _config_dtl, _site.LOCAL_TIMEZONE.ConvertOrDefault<int>());
                            }
                            //ONE MINUTE RAW DATA
                            //if (config.RAW_DURATION_CODE == "G")
                            //    db_Air.InsertT_QREST_DATA_ONE_MIN_fromLine(line, config, config_dtl);
                        }
                    }
                }
                

                if (updateNextRunTime)
                {
                    //update next run for the site
                    DateTime nextrun = System.DateTime.Now.AddMinutes(15);  //default to 15 minutes next run
                    if (config.POLLING_FREQ_TYPE == "M")
                        nextrun = System.DateTime.Now.AddMinutes(config.POLLING_FREQ_NUM ?? 15);

                    db_Air.InsertUpdatetT_QREST_SITES(config.SITE_IDX, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                        System.DateTime.Now, nextrun, null, null, null, null, null, null, null, null, null, null);
                }

                return SuccInd;
            }
            catch (Exception ex)
            {
                db_Ref.CreateT_QREST_SYS_LOG(null, "POLLING", ex.InnerException?.ToString());
                return false;
            }

        }


        /// <summary>
        /// Parses an entire ESC/AGILAIRE polling file into the HOURLY table, based on a provided polling configuration. 
        /// Can optionally update the next time polling should run.
        /// </summary>
        /// <param name="loggerData"></param>
        /// <param name="config"></param>
        /// <param name="updateNextRunTime"></param>
        /// <returns>True only if ran successfully.</returns>
        public static bool ParseFlatFileESC(string loggerData, SitePollingConfigType config, List<SitePollingConfigDetailType> _config_dtl, bool updateNextRunTime, bool? overrideConfigDuration = false, bool insertsOnly = false)
        {
            try
            {
                T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(config.SITE_IDX);

                string line;
                bool SuccInd = true;
                using (System.IO.StringReader sr = new System.IO.StringReader(loggerData))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        //FIVE MINUTE RAW DATA
                        if (config.RAW_DURATION_CODE == "H" || overrideConfigDuration == true)
                        {
                            //SuccInd = db_Air.InsertT_QREST_DATA_FIVE_MIN_fromLine(line, config, _config_dtl, insertsOnly);
                        }
                        //HOURLY RAW DATA
                        else if (config.RAW_DURATION_CODE == "1")
                        {
                            SuccInd = db_Air.InsertT_QREST_DATA_HOURLY_fromLine_ESC(line, config, _config_dtl, _site.LOCAL_TIMEZONE.ConvertOrDefault<int>());
                        }
                    }
                }


                if (updateNextRunTime)
                {
                    //update next run for the site
                    DateTime nextrun = System.DateTime.Now.AddMinutes(15);  //default to 15 minutes next run
                    if (config.POLLING_FREQ_TYPE == "M")
                        nextrun = System.DateTime.Now.AddMinutes(config.POLLING_FREQ_NUM ?? 15);

                    db_Air.InsertUpdatetT_QREST_SITES(config.SITE_IDX, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                        System.DateTime.Now, nextrun, null, null, null, null, null, null, null, null, null, null);
                }

                return SuccInd;
            }
            catch (Exception ex)
            {
                db_Ref.CreateT_QREST_SYS_LOG(null, "POLLING", ex.InnerException?.ToString());
                return false;
            }

        }

        /// <summary>
        /// Parses aMet One BAM file into the HOURLY table, based on a provided polling configuration. 
        /// Can optionally update the next time polling should run.
        /// </summary>
        /// <param name="loggerData"></param>
        /// <param name="config"></param>
        /// <param name="updateNextRunTime"></param>
        /// <returns>True only if ran successfully.</returns>
        public static bool ParseFlatFileMetOneBAM(string loggerData, SitePollingConfigType config, List<SitePollingConfigDetailType> _config_dtl, bool updateNextRunTime, bool? overrideConfigDuration = false, bool insertsOnly = false)
        {
            try
            {
                T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(config.SITE_IDX);

                string line;
                bool SuccInd = true;
                using (System.IO.StringReader sr = new System.IO.StringReader(loggerData))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        //HOURLY RAW DATA
                        if (config.RAW_DURATION_CODE == "1")
                        {
                            SuccInd = db_Air.InsertT_QREST_DATA_HOURLY_fromLine_BAM(line, config, _config_dtl, _site.LOCAL_TIMEZONE.ConvertOrDefault<int>());
                        }
                    }
                }


                if (updateNextRunTime)
                {
                    //update next run for the site
                    DateTime nextrun = System.DateTime.Now.AddMinutes(15);  //default to 15 minutes next run
                    if (config.POLLING_FREQ_TYPE == "M")
                        nextrun = System.DateTime.Now.AddMinutes(config.POLLING_FREQ_NUM ?? 15);

                    db_Air.InsertUpdatetT_QREST_SITES(config.SITE_IDX, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                        System.DateTime.Now, nextrun, null, null, null, null, null, null, null, null, null, null);
                }

                return SuccInd;
            }
            catch (Exception ex)
            {
                db_Ref.CreateT_QREST_SYS_LOG(null, "POLLING", ex.InnerException?.ToString());
                return false;
            }

        }


        public static async Task<bool> RetrieveWeatherCompanyPWS(T_QREST_SITE_POLL_CONFIG config) {

            using (var client = new System.Net.Http.HttpClient())
            {
                var xxx = await client.GetStringAsync("https://api.weather.com/v2/pws/observations/current?stationId=" + config.LOGGER_SOURCE + "&format=json&units=e&apiKey=" + config.LOGGER_PASSWORD); //uri
                try
                {
                    JObject rss = JObject.Parse(xxx);

                    string site = (string)rss["observations"][0]["stationID"];

                    //take this as a successful reading and continue with parsing
                    if (site == config.LOGGER_SOURCE)
                    {
                        string obsTimeLocal = (string)rss["observations"][0]["obsTimeLocal"];
                        DateTime local = DateTime.Parse(obsTimeLocal, new CultureInfo("en-US"), DateTimeStyles.NoCurrentDateDefault);
                        DateTime localrounded = new DateTime(local.Year, local.Month, local.Day, local.Hour, 0, 0);
                        string obsTimeUtc = (string)rss["observations"][0]["obsTimeUtc"];
                        DateTime utc = DateTime.Parse(obsTimeUtc, new CultureInfo("en-US"), DateTimeStyles.NoCurrentDateDefault);
                        DateTime utcrounded = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, 0, 0);
                        string solarRad = (string)rss["observations"][0]["solarRadiation"];
                        string windDir = (string)rss["observations"][0]["winddir"];
                        string temp1 = (string)rss["observations"][0]["imperial"]["temp"];
                        string windSpeed = (string)rss["observations"][0]["imperial"]["windSpeed"];
                        string pressure = (string)rss["observations"][0]["imperial"]["pressure"];
                        string dewpt = (string)rss["observations"][0]["imperial"]["dewpt"];
                        //string windChill = (string)rss["observations"][0]["imperial"]["windChill"];
                        //string precipRate = (string)rss["observations"][0]["imperial"]["precipRate"];
                        string precipTotal = (string)rss["observations"][0]["imperial"]["precipTotal"];


                        List<PollConfigDtlDisplay> _dtls = db_Air.GetT_QREST_SITE_POLL_CONFIG_DTL_ByID(config.POLL_CONFIG_IDX);
                        foreach (PollConfigDtlDisplay _dtl in _dtls)
                        {
                            if (_dtl.PAR_CODE == "63301" && solarRad.Length>0)
                                db_Air.InsertUpdateT_QREST_DATA_HOURLY(_dtl.MONITOR_IDX.GetValueOrDefault(), localrounded, utcrounded, 0, solarRad, _dtl.COLLECT_UNIT_CODE, false, null, null);
                            if (_dtl.PAR_CODE == "61104" && windDir.Length>0)
                                db_Air.InsertUpdateT_QREST_DATA_HOURLY(_dtl.MONITOR_IDX.GetValueOrDefault(), localrounded, utcrounded, 0, windDir, _dtl.COLLECT_UNIT_CODE, false, null, null);
                            if (_dtl.PAR_CODE == "62101" && temp1.Length>0)
                                db_Air.InsertUpdateT_QREST_DATA_HOURLY(_dtl.MONITOR_IDX.GetValueOrDefault(), localrounded, utcrounded, 0, temp1, _dtl.COLLECT_UNIT_CODE, false, null, null);
                            if (_dtl.PAR_CODE == "61101" && windSpeed.Length>0)
                                db_Air.InsertUpdateT_QREST_DATA_HOURLY(_dtl.MONITOR_IDX.GetValueOrDefault(), localrounded, utcrounded, 0, windSpeed, _dtl.COLLECT_UNIT_CODE, false, null, null);
                            if (_dtl.PAR_CODE == "64101" && pressure.Length>0)
                                db_Air.InsertUpdateT_QREST_DATA_HOURLY(_dtl.MONITOR_IDX.GetValueOrDefault(), localrounded, utcrounded, 0, pressure, _dtl.COLLECT_UNIT_CODE, false, null, null);

                            if (_dtl.PAR_CODE == "62201" && dewpt.Length > 0)
                                db_Air.InsertUpdateT_QREST_DATA_HOURLY(_dtl.MONITOR_IDX.GetValueOrDefault(), localrounded, utcrounded, 0, dewpt, _dtl.COLLECT_UNIT_CODE, false, null, null);
                            if (_dtl.PAR_CODE == "65102" && precipTotal.Length > 0)
                                db_Air.InsertUpdateT_QREST_DATA_HOURLY(_dtl.MONITOR_IDX.GetValueOrDefault(), localrounded, utcrounded, 0, precipTotal, _dtl.COLLECT_UNIT_CODE, false, null, null);
                        }

                    }
                }
                catch {
                    db_Ref.CreateT_QREST_SYS_LOG("SYSTEM", "POLLING ERROR", "FAILURE ON POLLING WEATHER STATION - unreadable format");
                    return false;
                }

            }

            return true;
        }


        public static string RetrieveCampbell(SitePollingConfigType _config)
        {
            //retrieve file from 
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Polls\\Poll_" + _config.SITE_ID + ".dat";
            bool fileExist = File.Exists(filepath);
            if (fileExist)
                return File.ReadAllText(filepath, Encoding.UTF8);
            else
            {
                //db_Ref.CreateT_QREST_SYS_LOG("SYSTEM", "POLLING ERROR2", filepath);
                return null;
            }
        }
    }
}
