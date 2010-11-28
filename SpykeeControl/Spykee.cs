using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using SpykeeControl.Command;
using SpykeeControl.Answer;

namespace SpykeeControl {
    // initial code taken from http://www.spykee-robot.com/forum/viewtopic.php?p=232#232
    public class Spykee {
        private TcpClient tcpClient;
        private PeekableStream clientBinaryReader;
        private BinaryWriter clientBinaryWriter;

        public bool Connected { get; private set; }

        private int currentAnswerByteCounter = -1;

        public delegate void SpykeeAnswerHandle(SpykeeAnswer answer);
        public event SpykeeAnswerHandle OnSpykeeAnswer;

        public delegate void SpykeeAnswerBatteryHandle(SpykeeAnswerBattery answer);
        public event SpykeeAnswerBatteryHandle OnSpykeeAnswerBattery;

        public delegate void SpykeeAnswerDockedHandle(SpykeeAnswerDocked answer);
        public event SpykeeAnswerDockedHandle OnSpykeeAnswerDocked;

        public delegate void SpykeeAnswerImageHandle(SpykeeAnswerImage answer);
        public event SpykeeAnswerImageHandle OnSpykeeAnswerImage;

        public delegate void SpykeeAnswerLoginHandle(SpykeeAnswerLogin answer);
        public event SpykeeAnswerLoginHandle OnSpykeeAnswerLogin;

        public delegate void SpykeeAnswerVoIPHandle(SpykeeAnswerVoIP answer);
        public event SpykeeAnswerVoIPHandle OnSpykeeAnswerVoIP;
        
        public bool IsConnected { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }

        public SpykeeAnswerBattery lastAnswerBattery { get; private set; }
        public SpykeeAnswerDocked lastAnswerDocked { get; private set; }
        public SpykeeAnswerImage lastAnswerImage { get; private set; }
        public SpykeeAnswerLogin lastAnswerLogin { get; private set; }
        public SpykeeAnswerVoIP lastAnswerVoIP { get; private set; }

        private bool stopListenToAnswersWorker;
        private Thread listenToAnswersThread;

        private bool stopExecuteQueueWorker;
        private Thread executeQueueWorkerThread;

        public Spykee() {
            listenToAnswersThread = new Thread(ListenToAnswersWorker);
            executeQueueWorkerThread = new Thread(ExecuteQueueWorker);
            Connected = false;
        }

        public bool Connect() {
            if (Connected)
                return false;

            //connect to server at given port
            try {
                tcpClient = new TcpClient(IpAddress, Port);
                //get a network stream from spykee
                NetworkStream clientSockStream = tcpClient.GetStream();
                clientBinaryReader = new PeekableStream(clientSockStream);
                clientBinaryWriter = new BinaryWriter(clientSockStream);
                
            } catch (Exception ex) {
                if (clientBinaryReader != null) {
                    clientBinaryReader.Close();
                    clientBinaryReader = null;
                }

                if (clientBinaryWriter != null) {
                    clientBinaryWriter.Close();
                    clientBinaryWriter = null;
                }

                if (tcpClient != null) {
                    tcpClient.Close();
                    tcpClient = null;
                }

                throw ex;
            }

            Connected = true;

            return true;
        }

        public void Disconnect() {
            if (!Connected)
                return;

            Connected = false;

            clientBinaryReader.Close();
            clientBinaryReader = null;

            clientBinaryWriter.Close();
            clientBinaryWriter = null;

            tcpClient.Close();
            tcpClient = null;
        }

        public void Execute(SpykeeCommand command) {
            command.Execute(clientBinaryWriter);
        }




        public void StartExecuteQueueWorkerThread() {
            if (executeQueueWorkerThread.IsAlive)
                return;

            stopExecuteQueueWorker = false;
            executeQueueWorkerThread.Start();

            while (!executeQueueWorkerThread.IsAlive) { }
        }

        public void StopExecuteQueueWorkerThread(bool waitForIt) {
            stopExecuteQueueWorker = true;

            if (waitForIt)
                executeQueueWorkerThread.Join();
        }

        private void ExecuteQueueWorker() {
            while (!stopExecuteQueueWorker) {
            }
        }



        public void StartListenToAnswersThread() {
            if (listenToAnswersThread.IsAlive)
                return;

            stopListenToAnswersWorker = false;
            listenToAnswersThread.Start();

            while (!listenToAnswersThread.IsAlive) { }
        }

        public void StopListenToAnswersThread(bool waitForIt) {
            stopListenToAnswersWorker = true;

            if (waitForIt)
                listenToAnswersThread.Join();
        }

        private void ListenToAnswersWorker() {
            while (!stopListenToAnswersWorker) {
                ListenToAnswers();
            }
        }

        public void ListenToAnswers() {
            while (clientBinaryReader.PeekByte() != -1) {
                int c = clientBinaryReader.ReadByte();

				// Not already reading? Then a new answer just started! Listen to the 'P'
                if (currentAnswerByteCounter == -1) {
					if ( c != 'P' ) {		// every new answer should start with 'PK'
						Log("skip P " + c );
					} else {
						//log("" + c );
						currentAnswerByteCounter = 0;						
					}				
					
				// If we just started reading, listen to the 'K'
				} else if (currentAnswerByteCounter == 0) {
					if ( c != 'K' ) {		// every new answer should start with 'PK'
						Log("skip K " + c );
                        currentAnswerByteCounter = -1;
					} else {
						//log("P" + c );
						currentAnswerByteCounter++;
					}
					
				// now we are ready to receive the rest of the answer:
				} else {
					int type = c;
			        if(type == 1 || type == 2 || type == 3 || type == 11 || type == 16) {
			        	//log("Known Type: "+type);			        	
						int high = clientBinaryReader.ReadByte() & 0xff;
						int low = clientBinaryReader.ReadByte() & 0xff;

                        int answerLengthRemaining = high << 8 | low;
			            byte[] buf = new byte[answerLengthRemaining];

                        int offset = 0;
			            while (answerLengthRemaining > 0)
			            {
			                //byte[] readBytes = clientBinaryReader.ReadBytes(answerLengthRemaining);
                            int readBytesCount = clientBinaryReader.Read(buf, offset, answerLengthRemaining);

                            /*
                            for (int copyIndex = 0; copyIndex < readBytes.Length; copyIndex++) {
                                buf[copyIndex + offset] = readBytes[copyIndex];
                            }
                             */

                            answerLengthRemaining -= readBytesCount;
                            offset += readBytesCount;
			            }
			            
			            // create an answer object and put it on the Queue:
			            SpykeeAnswer ans = SpykeeAnswerFactory.createSpykeeAnswer( type, buf );
                        BroadcastAnswer(ans);

                        currentAnswerByteCounter = -1;
			        	
			        } else {
			        	Log("Unknown answer type coming from Spykee: "+type);
                        currentAnswerByteCounter = -1;		        	
			        }
					
				}
            }
        }

        private void BroadcastAnswer(SpykeeAnswer answer) {
            if (answer is SpykeeAnswerBattery) {
                lastAnswerBattery = answer as SpykeeAnswerBattery;
                if (OnSpykeeAnswerBattery != null)
                    OnSpykeeAnswerBattery(lastAnswerBattery);

            } else if (answer is SpykeeAnswerDocked) {
                lastAnswerDocked = answer as SpykeeAnswerDocked;
                if (OnSpykeeAnswerDocked != null)
                    OnSpykeeAnswerDocked(lastAnswerDocked);

            } else if (answer is SpykeeAnswerImage) {
                lastAnswerImage = answer as SpykeeAnswerImage;
                if (OnSpykeeAnswerImage != null)
                    OnSpykeeAnswerImage(lastAnswerImage);

            } else if (answer is SpykeeAnswerLogin) {
                lastAnswerLogin = answer as SpykeeAnswerLogin;
                if (OnSpykeeAnswerLogin != null)
                    OnSpykeeAnswerLogin(lastAnswerLogin);

            } else if (answer is SpykeeAnswerVoIP) {
                lastAnswerVoIP = answer as SpykeeAnswerVoIP;
                if (OnSpykeeAnswerVoIP != null)
                    OnSpykeeAnswerVoIP(lastAnswerVoIP);

            } else {
                throw new Exception("Answer which won't be broadcast: " + answer);
            }
            if (OnSpykeeAnswer != null)
                OnSpykeeAnswer(answer);
        }

        private void Log(string str) {
            System.Console.WriteLine(str);
        }
    }
}
