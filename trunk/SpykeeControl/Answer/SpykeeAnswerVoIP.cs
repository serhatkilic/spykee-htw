using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpykeeControl.Answer {
    public class SpykeeAnswerVoIP : SpykeeAnswer {
	    byte[] audio;

	    public SpykeeAnswerVoIP(byte[] buf) {
		    this.audio = buf;
	    }

	    public byte[] getAudio() {
		    return audio;
	    }


        public override string ToString() {
		    String s = "Audio: " + audio.Length;
		    return s;
	    }
    }
}
