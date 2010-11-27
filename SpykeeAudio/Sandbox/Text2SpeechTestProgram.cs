using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpeechLib;

namespace SpykeeAudio.Sandbox {
    public class Text2SpeechTestProgram {
        static void Main(string[] args) {
            Text2SpeechTestProgram p = new Text2SpeechTestProgram();
            p.Start();
            //while (true) {
            //}
        }

        public void Start() {
            SpVoice voice = new SpVoice();

            //ISpeechObjectTokens speechTokens = voice.GetVoices("Name=LH Stefan", "Language=407");
            //voice.Voice = speechTokens.Item(0);

            voice.Speak("<pitch middle = '+30'/>Hello world! This is a first <pitch middle = '-30'/> test with the speech API.", SpeechVoiceSpeakFlags.SVSFIsXML);
        }
    }
}
