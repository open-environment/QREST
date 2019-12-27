using System;
using System.Collections.Generic;
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
        /// Sends a message on NetworkStream and listens for response. Continues listening until response stops writing or cancelation token is reached. 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="message"></param>
        /// <param name="msWaitPreSend"></param>
        /// <param name="msWaitPostSend"></param>
        /// <returns>Network stream response</returns>
        public static string SendReceiveMessage(NetworkStream stream, string message, int? msWaitPreSend, int? msWaitPostSend)
        {
            string resp = "init";
            string respPrev;

            //cancel service call if takes longer than 1 minute
            CancellationTokenSource _cancelToken = new CancellationTokenSource(60000);

            while (!_cancelToken.IsCancellationRequested)
            {
                // ****************Send message to the connected TcpServer. ********************************
                System.Threading.Thread.Sleep(msWaitPreSend ?? 700); //wait for terminal to be ready write data
                Byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(message);
                stream.Write(bytesToSend, 0, bytesToSend.Length);


                // ****************Read response after the send command. ********************************
                System.Threading.Thread.Sleep(msWaitPostSend ?? 700); //wait for terminal to begin writing response
                var ms = new MemoryStream();
                byte[] data = new byte[1024];
                int numBytesRead;
                do
                {
                    respPrev = resp;
                    numBytesRead = stream.Read(data, 0, data.Length);
                    ms.Write(data, 0, numBytesRead);
                    resp = Encoding.ASCII.GetString(ms.ToArray());

                    System.Threading.Thread.Sleep(500);  //wait before reading again to let logger write out more if still writing
                //} while (numBytesRead == data.Length);
                } while (stream.DataAvailable && respPrev != resp);

                return resp;
            }
            return "TIMEOUT:" + resp;
        }


        public static CommMessageLog SendReceiveMessage_CheckSuccessAndLog(string commMessageType, NetworkStream stream, string message, int? msWaitPreSend, int? msWaitPostSend, string checkResponseContains)
        {
            string xxx = SendReceiveMessage(stream, message, msWaitPreSend, msWaitPostSend);
            return new CommMessageLog
            {
                CommMessageType = commMessageType,
                CommMessageStatus = xxx.Contains(checkResponseContains),
                CommResponse = xxx
            };
        }
    }
}
