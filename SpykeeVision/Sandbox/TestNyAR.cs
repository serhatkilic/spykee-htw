using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.detector;
using System.Drawing;
using Emgu.CV.CvEnum;
using NyARToolkitCSUtils.NyAR;

namespace SpykeeVision.Sandbox {
    public class TestNyAR {
        static void Main(string[] args) {
            TestNyAR test = new TestNyAR();
            test.Start();
        }

        private CameraCapture cameraCapture;

        private NyARParam ap;
        private NyARCode code;
        //private DsBGRX32Raster ra;
        private NyARRgbRaster_BGRA ra;
        private NyARSingleDetectMarker ar;
        private NyARTransMatResult result_mat;
        private Image<Bgr, byte> frame, image, flipped;
        private Image<Bgra, byte> bgra_img;
        private Image<Gray, byte> gray;
        private MCvMat dft;

        private Size frameSize;

        private const string CODE_FILE = "data/patt.hiro";
        //private const string DATA_FILE = "data/320x240ABGR.raw";
        private const string CAMERA_FILE = "data/camera_para.dat";

        private void Start() {
            cameraCapture = new CameraCapture(0);

            Init(cameraCapture);

            CvInvoke.cvNamedWindow("NyAR-Test");

            while (true) {
                cameraCapture.Update();
                Update();

                Image<Bgr, byte> image = cameraCapture.CreateImageBgrCopy();


                float matX = (float)result_mat.m03;
                float matY = (float)result_mat.m13;
                float matZ = (float)result_mat.m23;

                PointF trackedPoint = new PointF();
                trackedPoint.X = matX + frameSize.Width / 2;
                trackedPoint.Y = matY + frameSize.Height / 2;

                trackedPoint.X = (float)refCenter2D.x;
                trackedPoint.Y = (float)refCenter2D.y;
                image.Draw(new CircleF(new Point((int)trackedPoint.X, (int)trackedPoint.Y), 5), new Bgr(0, 0, 255), -1);

                CvInvoke.cvShowImage("NyAR-Test", image);

                int c = CvInvoke.cvWaitKey(10);
                if ((char)c == 27)
                    break;
                switch ((char)c) {
                    case ' ':
                        break;
                }
            }

            CvInvoke.cvDestroyWindow("NyAR-Test");
        }

        private void Init(CameraCapture capture) {
            ap = new NyARParam();
            ap.loadARParamFromFile(CAMERA_FILE);

            code = new NyARCode(16, 16);
            code.loadARPattFromFile(CODE_FILE);

            frame = capture.Frame;
            if (frame != null) {
                frameSize = frame.Size;

                ra = new NyARRgbRaster_BGRA(frameSize.Width, frameSize.Height, false); // UNSURE i_is_alloc
                //ra = new DsBGRX32Raster(frameSize.Width, frameSize.Height, frame.Width * 4);//frame.Width * sizeof(Byte) / 8);
                ap.changeScreenSize(frameSize.Width, frameSize.Height);
                ar = new NyARSingleDetectMarker(ap, code, 80.0, ra.getBufferType());
                code = null;/*code‚ÌŠ—LŒ ‚ÍNyARSingleDetectMarker‚Ö“nˆÚ“®*/ // UNSURE ...what?
                ar.setContinueMode(false);

                //arglCameraFrustumRH(ap, 1.0, 100.0, camera_proj);

                /*
                CvSize s = cvGetSize(frame);
                image = cvCreateImage(s, 8, 3);
                image.Origin = frame.origin;
                bgra_img = cvCreateImage(s, 8, 4);
                bgra_img.origin = frame.origin;
                gray = cvCreateImage(s, 8, 1);
                gray.origin = frame.origin;
                flipped = cvCreateImage(s, 8, 3);
                 */
                image = new Image<Bgr, byte>(frameSize);
                bgra_img = new Image<Bgra, byte>(frameSize);
                gray = new Image<Gray, byte>(frameSize);
                flipped = new Image<Bgr, byte>(frameSize);

                dft = new MCvMat();
                dft.data = CvInvoke.cvCreateMat(frameSize.Width, frameSize.Height, MAT_DEPTH.CV_32F);

                //ra.setBuffer(bgra_img, true);
                ra.wrapBuffer(bgra_img.Bytes);

                result_mat = new NyARTransMatResult();
            }
        }

        private NyARSquare refSquare = new NyARSquare();
        private NyARDoublePoint2d refCenter2D = new NyARDoublePoint2d();

        private void Update() {
            bool found = false;

            frame = cameraCapture.Frame;

            image.Bitmap = frame.Bitmap;
            gray.Bitmap = image.Bitmap;
            bgra_img.Bitmap = gray.Bitmap;
            flipped.Bitmap = frame.Bitmap;
            flipped.Flip(FLIP.VERTICAL);

            ra.wrapBuffer(bgra_img.Bytes);

            if (ar.detectMarkerLite(ra, 50)) {
                refSquare = ar.refSquare();
                refSquare.getCenter2d(refCenter2D);
                ar.getTransmationMatrix(result_mat);
                //	printf("Marker confidence\n cf=%f,direction=%d\n",ar->getConfidence(),ar->getDirection());
                //printf("Transform Matrix\n");
                //printf(
                //	"% 4.8f,% 4.8f,% 4.8f,% 4.8f\n"
                //	"% 4.8f,% 4.8f,% 4.8f,% 4.8f\n"
                //	"% 4.8f,% 4.8f,% 4.8f,% 4.8f\n",
                //	result_mat.m00,result_mat.m01,result_mat.m02,result_mat.m03,
                //	result_mat.m10,result_mat.m11,result_mat.m12,result_mat.m13,
                //	result_mat.m20,result_mat.m21,result_mat.m22,result_mat.m23);
                found = true;
                System.Console.WriteLine("Found @ (" + (int)result_mat.m03 + "|" + (int)result_mat.m13 + "|" + (int)result_mat.m23 + ") w/ confidence: " + ar.getConfidence());
            }
        }
    }
}
