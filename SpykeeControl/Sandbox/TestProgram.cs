using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SpykeeControl.Command;

namespace SpykeeControl.Sandbox {
    class TestProgram {
        static void Main(string[] args) {
            TestProgram testProgram = new TestProgram();
            testProgram.Start();
        }

        private void Start() {
            Spykee spykee = new Spykee();

            spykee.OnSpykeeAnswer += SpykeeAnswerHandle;

            spykee.IpAddress = "172.17.6.1";
            spykee.Port = 9000;
            spykee.Connect();

            spykee.Execute(new SpykeeCommandLogin("admin", "0bstkuchen"));
            spykee.StartListenToAnswersThread();
            while (true) {
                //spykee.ListenToAnswers();
            }

            /*
            while (true) {
                spykee.Execute(new SpykeeCommandMove(SpykeeCommandMove.Directions.Left, 25));
                Thread.Sleep(1000);
                //spykee.Execute(new SpykeeCommandMove(SpykeeCommandMove.Directions.Backward, 1));
                //Thread.Sleep(1000);
            }
             */
            /*
            int speed = 50;
            spykee.Execute(new SpykeeCommandMove(SpykeeCommandMove.Directions.Left, speed));
            spykee.ListenToAnswers();
            Thread.Sleep(1000);
            spykee.ListenToAnswers();
            spykee.Execute(new SpykeeCommandMove(SpykeeCommandMove.Directions.Left, speed));
            spykee.ListenToAnswers();
            Thread.Sleep(1000);
            spykee.ListenToAnswers();
            spykee.Execute(new SpykeeCommandMove(SpykeeCommandMove.Directions.Left, speed));
            spykee.ListenToAnswers();
            Thread.Sleep(1000);
            spykee.ListenToAnswers();
            spykee.Execute(new SpykeeCommandMove(SpykeeCommandMove.Directions.Left, speed));
            spykee.ListenToAnswers();
            Thread.Sleep(1000);
            spykee.ListenToAnswers();
            /*
            spykee.Execute(new SpykeCommandMove(SpykeCommandMove.Directions.Backwards, 50));
            Thread.Sleep(500);
             */
            //spykee.Execute(new SpykeeCommandMoveStop());
        }

        public void SpykeeAnswerHandle(SpykeeAnswer answer) {
            System.Console.WriteLine(answer.ToString());
        }
    }
}
