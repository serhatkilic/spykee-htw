using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SpykeeControl {
    public abstract class SpykeeCommand {
        protected byte[] firstLine;
        protected byte[] secondLine;

        protected SpykeeCommand() {
        }

        public virtual void Execute(BinaryWriter clientBinaryWriter) {
            byte[] complete = new byte[firstLine.Length + secondLine.Length];
            for (int i = 0; i < firstLine.Length; i++) {
                complete[i] = firstLine[i];
            }
            for (int i = 0; i < secondLine.Length; i++) {
                complete[firstLine.Length + i] = secondLine[i];
            }
            clientBinaryWriter.Write(complete);
        }

        protected void SetFirstLine(int a, int b, int c) {
            //firstLine = "PK" + (char)a + (char)b + (char)c;
            firstLine = new byte[5];
            firstLine[0] = (byte)'P';
            firstLine[1] = (byte)'K';
            firstLine[2] = (byte)a;
            firstLine[3] = (byte)b;
            firstLine[4] = (byte)c;
        }

        protected void SetSecondLine(int a, int b) {
            //secondLine = "" + (char)a + ((char)b);
            secondLine = new byte[2];
            secondLine[0] = (byte)a;
            secondLine[1] = (byte)b;
        }

        protected void SetSecondLine(string str) {
            secondLine = new byte[str.Length];
            for (int i = 0; i < str.Length; i++) {
                secondLine[i] = (byte)str[i];
            }
        }
    }
}
