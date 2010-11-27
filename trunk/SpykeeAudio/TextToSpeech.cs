using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpeechLib;

namespace SpykeeAudio {
    public class TextToSpeech {
        private SpVoice voice;
        public SpeechVoiceSpeakFlags standardFlags { get; set; }

        public TextToSpeech() {
            voice = new SpVoice();
            standardFlags = SpeechVoiceSpeakFlags.SVSFDefault | SpeechVoiceSpeakFlags.SVSFlagsAsync;
        }

        public void Speak(string str) {
            voice.Speak(str, standardFlags);
        }

        public void Speak(string str, SpeechVoiceSpeakFlags flags) {
            voice.Speak(str, flags);
        }
    }
}
