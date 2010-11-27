using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;

namespace SpykeeVision.Sandbox {
    class TestProgram {
        static void Main(string[] args) {
            CameraCapture cameraCapture = new CameraCapture(0);

            TrackMe trackMe = new TrackMe(cameraCapture);

            System.Console.Write("Hot keys: \n" +
                    "\tESC - quit the program\n" +
                    "\tr - auto-initialize tracking\n" +
                    "\tn - switch the \"night\" mode on/off\n" +
                    "To add/remove a feature point click it\n");

            CvInvoke.cvNamedWindow("LkDemo");

            bool nightMode = false;

            while (true) {
                cameraCapture.Update();
                trackMe.Update();

                Image<Bgr, byte> image = cameraCapture.CreateImageBgrCopy();
                if (nightMode)
                    CvInvoke.cvZero(image);

                for (int i = 0; i < trackMe.currentPoints.Length; i++) {
                    image.Draw(new CircleF(new Point((int)trackMe.currentPoints[i].X, (int)trackMe.currentPoints[i].Y), 3), new Bgr(0, 255, 0), -1);

                    PointF middle = trackMe.GetMiddle();
                    image.Draw(new CircleF(new Point((int)middle.X, (int)middle.Y), 5), new Bgr(0, 0, 255), -1);
                }

                CvInvoke.cvShowImage("LkDemo", image);

                int c = CvInvoke.cvWaitKey(10);
                if ((char)c == 27)
                    break;
                switch ((char)c) {
                    case ' ':
                        trackMe.toggleTracking();
                        break;
                    case 'r':
                        trackMe.initialize();
                        break;
                    case 'n':
                        nightMode = !nightMode;
                        break;
                    default:
                        break;
                }
            }

            /*
            String win1 = "Test Window"; //The name of the window
            CvInvoke.cvNamedWindow(win1); //Create the window using the specific name

            Image<Bgr, Byte> img = new Image<Bgr, byte>(400, 200, new Bgr(255, 0, 0)); //Create an image of 400x200 of Blue color
            MCvFont f = new MCvFont(FONT.CV_FONT_HERSHEY_COMPLEX, 1.0, 1.0); //Create the font

            img.Draw("Hello, world", ref f, new System.Drawing.Point(10, 80), new Bgr(0, 255, 0)); //Draw "Hello, world." on the image using the specific font

            CvInvoke.cvShowImage(win1, img); //Show the image
            CvInvoke.cvWaitKey(0);  //Wait for the key pressing event
            CvInvoke.cvDestroyWindow(win1); //Destory the window
             */
        }
    }
}
