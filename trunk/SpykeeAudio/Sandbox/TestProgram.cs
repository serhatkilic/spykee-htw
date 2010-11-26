using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using SpeechLib;

namespace SpykeeAudio.Sandbox {
    class TestProgram {
        static void Main(string[] args) {
            TestProgram p = new TestProgram();
            p.Start();
        }

        public void Start() {
            SpeechCommandRecognition scr = new SpeechCommandRecognition();

            scr.SpeechEnabled = true;
            scr.AddItem("Test");
            scr.AddItem("Tobias");
            scr.AddItem("Los");
            scr.AddItem("Stop");
            scr.AddItem("On");
            scr.AddItem("Off");
            scr.WordRecognized += WordRecognizedHandler;
            scr.SpeechEnabled = true;

            Application.Run();
        }

        public void WordRecognizedHandler(ISpeechPhraseProperty recognizedPhrase) {
            if (recognizedPhrase.EngineConfidence >= 0.1) {
                System.Console.WriteLine("Recognized: " + recognizedPhrase.Name + " (confidence: " + recognizedPhrase.EngineConfidence);
            } else {
                System.Console.WriteLine("     (Recognized: " + recognizedPhrase.Name + " (confidence: " + recognizedPhrase.EngineConfidence + ")");
            }
        }
    }
}
