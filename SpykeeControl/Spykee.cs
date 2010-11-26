using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using SpykeeControl.Command;

namespace SpykeeControl {
    // initial code taken from http://www.spykee-robot.com/forum/viewtopic.php?p=232#232
    public class Spykee {
        private BinaryReader clientBinaryReader;
        private BinaryWriter clientBinaryWriter;

        public bool IsConnected { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }

        public bool Connect() {
            //connect to server at given port
            try {
                var tcpClient = new TcpClient(IpAddress, Port);
                //get a network stream from spykee
                var clientSockStream = tcpClient.GetStream();
                clientBinaryReader = new BinaryReader(clientSockStream);
                clientBinaryWriter = new BinaryWriter(clientSockStream);

                /*
                StreamWriter streamWriter = new StreamWriter(clientSockStream);
                streamWriter.Write("C" + (char)128); streamWriter.Flush();
                clientBinaryWriter.Write(new byte[] { (byte)'C', (byte)128 });
                 */


            } catch (Exception ex) {
                throw ex;
            }

            return true;
        }

        public void Execute(SpykeeCommand command) {
            command.Execute(clientBinaryWriter);
        }

        internal void ListenToAnswers() {
            string result = null;

            //  get some answer
            while (clientBinaryReader.PeekChar() >= 0) {
                result += (char)clientBinaryReader.Read();
            }
        }
    }
}
