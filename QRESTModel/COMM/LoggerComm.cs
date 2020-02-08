using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace QRESTModel.COMM
{
    public class CommMessageLog {
        public string CommMessageType { get; set; }
        public bool CommMessageStatus { get; set; }
        public string CommResponse { get; set; }
    }

    public static class LoggerComm
    {
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


        public static CommMessageLog ConnectTcpClientSailer(string ip, ushort port, string pwd, string message)
        {
            var log = new CommMessageLog();

            // This discards any pending data and Winsock resets the connection.
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
                            //*************** send sailer message *****************************
                            string xxx = SendReceiveMessage(stream, message, 700, 700, true);
                            if (xxx != null && xxx.Length > 10)
                                log = new CommMessageLog { CommMessageStatus = true, CommMessageType = "Data", CommResponse = xxx };
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

                    log = new CommMessageLog { CommMessageStatus = false, CommMessageType = "Ping Socket Exception 3", CommResponse = ex.Message };
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
        public static string SendReceiveMessage(NetworkStream stream, string message, int? msWaitPreSend, int? msWaitPostSend, bool appendEOFInd)
        {
            string resp = "init";
            string respPrev;

            //cancel service call if takes longer than 1 minute
            CancellationTokenSource _cancelToken = new CancellationTokenSource(60000);

            while (!_cancelToken.IsCancellationRequested)
            {
                // ****************Send message to the connected TcpServer ********************************
                Thread.Sleep(msWaitPreSend ?? 700); //wait for socket to be ready write data

                Byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(message);


                if (appendEOFInd)
                {
                    //******** ADD MODULO-100 CHECKSUM **************
                    int byteSum = 0;
                    for (int i = 1; i <= bytesToSend.Length-1; i++)
                        byteSum += bytesToSend[i];
                    int mod = byteSum % 100;

                    //"#00030001DL5,29"
                    var ascii = Encoding.ASCII;
                    bytesToSend = Encoding.ASCII.GetBytes(message +  mod.ToString())
                        .Concat(ascii.GetBytes(new[] { (char)3 }))
                        .ToArray();
                }

                stream.Write(bytesToSend, 0, bytesToSend.Length);

                // ****************Read response after the send command ********************************
                Thread.Sleep(msWaitPostSend ?? 700); //wait for socket to begin writing response
                var ms = new MemoryStream();
                byte[] data = new byte[1024];
                int numBytesRead;
                do
                {
                    respPrev = resp;
                    numBytesRead = stream.Read(data, 0, data.Length);
                    ms.Write(data, 0, numBytesRead);
                    resp = Encoding.ASCII.GetString(ms.ToArray());

                    Thread.Sleep(1500);  //wait before reading again to let logger write out more if still writing
                } while (stream.DataAvailable && respPrev != resp);
                
                return resp;
            }
            return "TIMEOUT:" + resp;
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
    }
}
