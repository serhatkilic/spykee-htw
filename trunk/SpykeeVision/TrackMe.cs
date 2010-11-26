using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Drawing;

namespace SpykeeVision {
    public class TrackMe {
        const int POINT_COUNT = 50;
        int minDistance = 5;
        int maxDistance = 50;
        float errorDistanceSqrt = 60*60;

        Image<Bgr, byte> image;
        Image<Gray, byte> grey, prev_grey, pyramid, prev_pyramid;

        int win_size = 10;

        PointF[] currentPoints;
        PointF[] previousPoints;
        byte[] status;
        bool nightMode = false;
        LKFLOW_TYPE flags = 0;

        bool tracking = false;

        Capture capture;
        Image<Bgr, byte> frame;

        public TrackMe() {
            capture = new Capture(0);

            if (capture == null) {
                throw new Exception("Could not initialize capturing...\n");
            }

            System.Console.Write("Hot keys: \n" +
                    "\tESC - quit the program\n" +
                    "\tr - auto-initialize tracking\n" +
                    "\tn - switch the \"night\" mode on/off\n" +
                    "To add/remove a feature point click it\n");

            CvInvoke.cvNamedWindow("LkDemo");
            //CvInvoke.cvSetMouseCallback("LkDemo", on_mouse, 0 );
            //ImageBox imageBox = new ImageBox();

            frame = capture.QueryFrame();

            image = new Image<Bgr, byte>(frame.Size);
            grey = new Image<Gray, byte>(image.Size);
            prev_grey = new Image<Gray, byte>(frame.Size);
            pyramid = new Image<Gray, byte>(frame.Size);
            prev_pyramid = new Image<Gray, byte>(frame.Size);

            flags = LKFLOW_TYPE.DEFAULT;

            currentPoints = new PointF[0];

            initialize();
        }

        public void destroy() {
            CvInvoke.cvDestroyWindow("LkDemo");
        }

        public Boolean Process() {
            int i, k, c;

            frame = capture.QueryFrame();
            if (frame == null)
                return false;

            image.Bitmap = frame.Bitmap;
            grey.Bitmap = image.Bitmap;

            if (nightMode)
                CvInvoke.cvZero(image);

            if (currentPoints.Length > 0) {
                if (tracking) {
                    float[] trackError;
                    OpticalFlow.PyrLK(prev_grey, grey, prev_pyramid, pyramid, previousPoints, new Size(win_size, win_size), 3, new MCvTermCriteria(20, 0.03), flags, out currentPoints, out status, out trackError);
                    flags |= LKFLOW_TYPE.CV_LKFLOW_PYR_A_READY;

                    CheckPoints();
                }

                for (i = k = 0; i < currentPoints.Length; i++) {
                    if (tracking) {
                        if (status[i] == 0)
                            continue;
                    }

                    currentPoints[k++] = currentPoints[i];
                    image.Draw(new CircleF(new Point((int)currentPoints[i].X, (int)currentPoints[i].Y), 3), new Bgr(0, 255, 0), -1);

                    PointF middle = GetMiddle();
                    image.Draw(new CircleF(new Point((int)middle.X, (int)middle.Y), 5), new Bgr(0, 0, 255), -1);
                }
            }

            prev_grey.Bitmap = grey.Bitmap;
            prev_pyramid.Bitmap = pyramid.Bitmap;
            previousPoints = currentPoints;
            CvInvoke.cvShowImage("LkDemo", image);

            c = CvInvoke.cvWaitKey(10);
            if ((char)c == 27)
                return false;
            switch ((char)c) {
                case ' ':
                    toggleTracking();
                    break;
                case 'r':
                    initialize();
                    break;
                case 'n':
                    nightMode = !nightMode;
                    break;
                default:
                    break;
            }

            return true;
        }

        public void startTracking() {
            tracking = true;
        }

        public void stopTracking() {
            tracking = false;
            initialize();
        }

        public void toggleTracking() {
            tracking = !tracking;
            if (!tracking) {
                initialize();
            }
        }

        public void initialize() {
            currentPoints = new PointF[50];

            PointF middle = new PointF(grey.Width / 2, grey.Height / 2);
            for (int i = 0; i < 50-1; i += 2) {
                GetPointsAround(middle, ref currentPoints[i], ref currentPoints[i + 1], minDistance, maxDistance);
            }

            flags = LKFLOW_TYPE.DEFAULT;

            UpdatePoints();
        }

        protected Random random = new Random();

        protected void GetPointAround(PointF middle, ref PointF p1, int minDistance, int maxDistance) {
            if (p1 == null) p1 = new PointF();

            double angle = random.NextDouble() * Math.PI * 2;
            float distance = random.Next(minDistance, maxDistance);
            float diffX = (float)Math.Cos(angle) * distance;
            float diffY = (float)Math.Sin(angle) * distance;
            p1.X = middle.X + diffX;
            p1.Y = middle.Y + diffY;
        }

        protected void GetPointsAround(PointF middle, ref PointF p1, ref PointF p2, int minDistance, int maxDistance) {
            if (p1 == null) p1 = new PointF();
            if (p2 == null) p2 = new PointF();

            double angle = random.NextDouble() * Math.PI * 2;
            float distance = random.Next(minDistance, maxDistance);
            float diffX = (float)Math.Cos(angle) * distance;
            float diffY = (float)Math.Sin(angle) * distance;
            p1.X = middle.X + diffX;
            p1.Y = middle.Y + diffY;
            p2.X = middle.X - diffX;
            p2.Y = middle.Y - diffY;
        }

        protected void SetPointDistance(PointF middle, ref PointF p, int distance) {
            PointF v = new PointF(p.X - middle.X, p.Y - middle.Y);
            float length = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
            v.X /= length;
            v.Y /= length;

            p.X = middle.X + v.X * distance;
            p.Y = middle.Y + v.Y * distance;
        }

        protected PointF GetMiddle() {
            PointF middle = new PointF();

            foreach (PointF p in currentPoints) {
                middle.X += p.X;
                middle.Y += p.Y;
            }
            middle.X /= currentPoints.Length;
            middle.Y /= currentPoints.Length;

            return middle;
        }

        private void CheckPoints() {
            PointF middle = GetMiddle();

            for (int i = 0; i < currentPoints.Length; i++) {
                PointF p = currentPoints[i];

                float dx = p.X - middle.X;
                float dy = p.Y - middle.Y;
                float distSqrt = dx * dx + dy * dy;
                if (distSqrt > errorDistanceSqrt) {
                    SetPointDistance(middle, ref currentPoints[i], maxDistance);
                    UpdatePoint(i);
                    middle = GetMiddle();
                }
            }
        }

        private void UpdatePoint(int i) {
            grey.FindCornerSubPix(new PointF[][] { new PointF[] { currentPoints[i] } }, new Size(win_size, win_size), new Size(-1, -1), new MCvTermCriteria(20, 0.03));
            previousPoints[i] = currentPoints[i];
        }

        private void UpdatePoints() {
            grey.FindCornerSubPix(new PointF[][] { currentPoints }, new Size(win_size, win_size), new Size(-1, -1), new MCvTermCriteria(20, 0.03));
            previousPoints = currentPoints;
        }
    }
}
