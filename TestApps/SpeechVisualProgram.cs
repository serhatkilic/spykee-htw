using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using SpykeeVision;
using SpykeeAudio;
using SpeechLib;
using NLog;

namespace TestApps {
    class SpeechVisualProgram {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private TrackMe trackMe;
        private SpeechCommandRecognition scr;
        private Boolean exitCommand = false;

        static void Main(string[] args) {
            SpeechVisualProgram p = new SpeechVisualProgram();

            p.Start();
        }

        private void Start() {
            scr = new SpeechCommandRecognition();

            scr.SpeechEnabled = true;
            scr.AddItem("los");
            scr.AddItem("aufhören");
            scr.AddItem("neustart");
            scr.AddItem("beenden");
            scr.WordRecognized += WordRecognizedHandler;
            scr.SpeechEnabled = true;

            trackMe = new TrackMe();

            Boolean goOn = true;
            while (goOn) {
                //Application.DoEvents();
                goOn = trackMe.Process();
                if (exitCommand) {
                    goOn = false;
                }
            }
            //Application.Exit();
        }

        void IdleHandler(object sender, EventArgs e) {
        }

        public void WordRecognizedHandler(ISpeechPhraseProperty recognizedPhrase) {
            if (recognizedPhrase.EngineConfidence >= 0.1) {
                if (recognizedPhrase.Name == "los") {
                    trackMe.startTracking();
                } else if (recognizedPhrase.Name == "aufhören") {
                    trackMe.stopTracking();
                } else if (recognizedPhrase.Name == "neustart") {
                    trackMe.initialize();
                } else if (recognizedPhrase.Name == "beenden") {
                    exitCommand = true;
                }
            }
        }
    }
}
