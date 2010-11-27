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
        private const int POINT_COUNT = 50;
        private int minDistance = 5;
        private int maxDistance = 50;
        private float errorDistanceSqrt = 60 * 60;

        private Image<Gray, byte> grey, prev_grey, pyramid, prev_pyramid;

        private int win_size = 10;

        public PointF[] currentPoints;
        private PointF[] previousPoints;
        private byte[] status;
        private LKFLOW_TYPE flags = 0;

        private bool tracking = false;

        private CameraCapture capture;

        public PointF[] CurrentPoints { get { return currentPoints; } }

        public TrackMe(CameraCapture capture) {
            this.capture = capture;

            grey = new Image<Gray, byte>(capture.FrameSize);
            prev_grey = new Image<Gray, byte>(capture.FrameSize);
            pyramid = new Image<Gray, byte>(capture.FrameSize);
            prev_pyramid = new Image<Gray, byte>(capture.FrameSize);

            flags = LKFLOW_TYPE.DEFAULT;

            currentPoints = new PointF[0];

            initialize();
        }

        public void destroy() {
            CvInvoke.cvDestroyWindow("LkDemo");
        }

        public void Update() {
            Image<Bgr, byte> frame = capture.Frame;
            if (frame == null)
                throw new Exception("No camera frame");

            grey.Bitmap = frame.Bitmap;

            if (currentPoints.Length > 0) {
                if (tracking) {
                    float[] trackError;
                    OpticalFlow.PyrLK(prev_grey, grey, prev_pyramid, pyramid, previousPoints, new Size(win_size, win_size), 3, new MCvTermCriteria(20, 0.03), flags, out currentPoints, out status, out trackError);
                    flags |= LKFLOW_TYPE.CV_LKFLOW_PYR_A_READY;

                    CheckPoints();
                }
            }

            prev_grey.Bitmap = grey.Bitmap;
            prev_pyramid.Bitmap = pyramid.Bitmap;
            previousPoints = currentPoints;
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

        public PointF GetMiddle() {
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

                // If the status for this point is 0, update it in any case
                bool update = (status[i] == 0);

                // ...else check the distance...
                if (!update) {
                    float dx = p.X - middle.X;
                    float dy = p.Y - middle.Y;
                    float distSqrt = dx * dx + dy * dy;

                    update = distSqrt > errorDistanceSqrt;
                }

                // And, should we update now?
                if (update) {
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
