using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpykeeControl.Answer {
    public static class SpykeeAnswerFactory {
        public static SpykeeAnswer createSpykeeAnswer(int type, byte[] buf) {
            //if(type == 1 || type == 2 || type == 3 || type == 11 || type == 16) {
            SpykeeAnswer ans = null;
            switch (type) {

                // not sure if case 1 is really audio!!!
                case 1:									// audio
                    ans = new SpykeeAnswerVoIP(buf);
                    break;

                case 2:									// image/video
                    ans = new SpykeeAnswerImage(buf);
                    break;

                case 3:									// battery
                    ans = new SpykeeAnswerBattery(buf);
                    break;

                case 11:								// login
                    ans = new SpykeeAnswerLogin(buf);
                    break;

                case 16:								// docked
                    ans = new SpykeeAnswerDocked(buf);
                    break;

                default:
                    throw new Exception("Unknown answer type");
            }
            return ans;
        }

    }
}
