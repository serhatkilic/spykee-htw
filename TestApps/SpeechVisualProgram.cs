using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using SpykeeVision;
using SpykeeAudio;
using SpeechLib;
using NLog;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace TestApps {
    class SpeechVisualProgram {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private CameraCapture cameraCapture;
        private TrackMe trackMe;
        private SpeechCommandRecognition scr;
        private TextToSpeech textToSpeech;

        private Boolean exitCommand = false;

        static void Main(string[] args) {
            SpeechVisualProgram p = new SpeechVisualProgram();

            p.Start();
        }

        private void Start() {
            textToSpeech = new TextToSpeech();

            textToSpeech.Speak("Welcome! Please wait...");

            scr = new SpeechCommandRecognition("spiki");

            scr.AddItem("los");
            scr.AddItem("aufhören");
            scr.AddItem("neustart");
            scr.AddItem("beenden");
            scr.WordRecognized += WordRecognizedHandler;

            CvInvoke.cvNamedWindow("TestProgram");

            cameraCapture = new CameraCapture(0);
            trackMe = new TrackMe(cameraCapture);

            scr.SpeechEnabled = true;

            while (true) {
                Application.DoEvents();

                cameraCapture.Update();
                trackMe.Update();

                DrawDebugWindow();

                if (exitCommand) {
                    break;
                }
            }
            Application.Exit();
        }

        private void DrawDebugWindow() {
            Image<Bgr, byte> image = cameraCapture.CreateImageBgrCopy();

            for (int i = 0; i < trackMe.currentPoints.Length; i++) {
                image.Draw(new CircleF(new Point((int)trackMe.currentPoints[i].X, (int)trackMe.currentPoints[i].Y), 3), new Bgr(0, 255, 0), -1);

                PointF middle = trackMe.GetMiddle();
                image.Draw(new CircleF(new Point((int)middle.X, (int)middle.Y), 5), new Bgr(0, 0, 255), -1);
            }

            CvInvoke.cvShowImage("TestProgram", image);
        }

        public void WordRecognizedHandler(ISpeechPhraseProperty recognizedPhrase) {
            if (recognizedPhrase.EngineConfidence >= 0.1) {
                if (recognizedPhrase.Name == "los") {
                    trackMe.startTracking();
                    textToSpeech.Speak("Tracking started");
                } else if (recognizedPhrase.Name == "aufhören") {
                    trackMe.stopTracking();
                    textToSpeech.Speak("Okay, I stopped");
                } else if (recognizedPhrase.Name == "neustart") {
                    trackMe.initialize();
                    textToSpeech.Speak("Restarting");
                } else if (recognizedPhrase.Name == "beenden") {
                    textToSpeech.Speak("Goodbye!", SpeechVoiceSpeakFlags.SVSFDefault);
                    exitCommand = true;
                }
            }
        }
    }
}
