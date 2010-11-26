using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SpykeeControl.Command {
    public class SpykeeCommandMove : SpykeeCommand {
        public enum Directions {
            Forward, Backward, Left, Right
        }

        public Directions Direction { get; private set; }
        public int MotorSpeed { get; private set; }

        /*
        public override void Execute(StreamWriter normal, BinaryWriter clientStreamWriter) {
            normal.Write(firstLine);
            //clientStreamWriter.Write(secondLine);
            byte[] bytes = new byte[1];
            bytes[0] = 255;
            //bytes[1] = 128;
            
            clientStreamWriter.Write(bytes, 0, bytes.Length);
            clientStreamWriter.Write(bytes, 0, bytes.Length);
            clientStreamWriter.Flush();
        }
        */

        public SpykeeCommandMove(int speedLeft, int speedRight) {
            SetFirstLine(5, 0, 2);
            SetSpeed(speedLeft, speedRight);
        }

        public SpykeeCommandMove(Directions direction, int motorSpeed) {
            if (motorSpeed > 127) motorSpeed = 127;
            if (motorSpeed < 1) motorSpeed = 1;

            Direction = direction;
            MotorSpeed = motorSpeed;

            SetFirstLine(5, 0, 2);

            // 0 -> Stop
            // 1..127 -> Forward (slow..fast)
            // 128..255 -> Backward (fast..slow)
            int a;
            int b;
            switch (direction) {
                case Directions.Forward:
                    a = motorSpeed;
                    b = motorSpeed;
                    break;

                case Directions.Backward:
                    a = 256 - motorSpeed;
                    b = 256 - motorSpeed;
                    break;

                case Directions.Left:
                    a = motorSpeed;
                    b = 256 - motorSpeed;
                    break;

                case Directions.Right:
                    a = 256 - motorSpeed;
                    b = motorSpeed;
                    break;

                default:
                    throw new Exception("No code for direction: " + direction.ToString());
            }

            SetSecondLine(a, b);
        }

        protected void SetSpeed(int left, int right) {
            /*
            int signLeft = Math.Sign(left);
            int speedLeft = Math.Abs(left);
            if (speedLeft < 3) speedLeft = 3;
            if (speedLeft > 122) speedLeft = 122;

            int signRight = Math.Sign(right);
            int speedRight = Math.Abs(right);
            if (speedRight < 3) speedRight = 3;
            if (speedRight > 122) speedRight = 122;

            int a = (signLeft >= 0 ? 0 : 125) + speedLeft;
            int b = (signRight >= 0 ? 0 : 125) + speedRight;
             */

            int a = left;
            int b = right;

            SetSecondLine(a, b);
        }
    }
}
