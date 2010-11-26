using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Drawing;

namespace SpykeeVision.Sandbox {
    public class LkDemo {
        Image<Gray, byte> grey, prev_grey, pyramid, prev_pyramid;
        Image<Bgr, byte> image;

        int win_size = 10;
        const int MAX_COUNT = 500;

        PointF[][] points = new PointF[2][]; // MCvPoint2D64f
        PointF[] swap_points = new PointF[MAX_COUNT];
        byte[] status;
        int count = 0;
        bool need_to_init = false;
        bool night_mode = false;
        LKFLOW_TYPE flags = 0;
        bool add_remove_pt = false;
        Point pt;

        /*
        void on_mouse( int event, int x, int y, int flags, void* param )
        {
            if( !image )
                return;

            if( image->origin )
                y = image->height - y;

            if( event == CV_EVENT_LBUTTONDOWN )
            {
                pt = cvPoint(x,y);
                add_remove_pt = 1;
            }
        }
         * */

        public void Start() {
            Capture capture = new Capture(0);

            if (capture == null) {
                throw new Exception("Could not initialize capturing...\n");
            }

            /* print a welcome message, and the OpenCV version */
            //System.Console.Write ("Welcome to lkdemo, using OpenCV version "+CV_VERSION+" ("+CV_MAJOR_VERSION+"."+CV_MINOR_VERSION+"."++CV_SUBMINOR_VERSION")\n");

            System.Console.Write("Hot keys: \n" +
                    "\tESC - quit the program\n" +
                    "\tr - auto-initialize tracking\n" +
                    "\tc - delete all the points\n" +
                    "\tn - switch the \"night\" mode on/off\n" +
                    "To add/remove a feature point click it\n");

            CvInvoke.cvNamedWindow("LkDemo");
            //CvInvoke.cvSetMouseCallback("LkDemo", on_mouse, 0 );
            //ImageBox imageBox = new ImageBox();

            for (; ; ) {
                Image<Bgr, byte> frame = null;
                int i, k, c;

                frame = capture.QueryFrame();
                if (frame == null)
                    break;

                if (image == null) {
                    /* allocate all the buffers */
                    //image = CvInvoke.cvCreateImage( frame.Size, IPL_DEPTH.IPL_DEPTH_8S, 3 );
                    //image->origin = frame->origin;
                    //grey = CvInvoke.cvCreateImage( frame.Size, IPL_DEPTH.IPL_DEPTH_8S, 1 );
                    //prev_grey = CvInvoke.cvCreateImage( frame.Size, IPL_DEPTH.IPL_DEPTH_8S, 1 );
                    //pyramid = CvInvoke.cvCreateImage( frame.Size, IPL_DEPTH.IPL_DEPTH_8S, 1 );
                    prev_grey = new Image<Gray, byte>(frame.Size);
                    pyramid = new Image<Gray, byte>(frame.Size);
                    prev_pyramid = new Image<Gray, byte>(frame.Size);
                    //prev_pyramid = CvInvoke.cvCreateImage( frame.Size, IPL_DEPTH.IPL_DEPTH_8S, 1 );
                    //points[0] = (CvPoint2D32f*)cvAlloc(MAX_COUNT*sizeof(points[0, 0]));
                    //points[1] = (CvPoint2D32f*)cvAlloc(MAX_COUNT*sizeof(points[0, 0]));
                    //status = (char*)cvAlloc(MAX_COUNT);
                    points[0] = new PointF[MAX_COUNT];
                    points[1] = new PointF[MAX_COUNT];
                    status = new byte[MAX_COUNT];
                    flags = LKFLOW_TYPE.DEFAULT;
                }

                image = new Image<Bgr, byte>(frame.Bitmap);     //CvInvoke.cvCopy( frame.Ptr, image.Ptr, 0 );
                grey = new Image<Gray, byte>(image.Bitmap);
                //CvInvoke.cvCvtColor( image.Ptr, grey.Ptr, COLOR_CONVERSION.CV_BGR2GRAY );

                if (night_mode)
                    CvInvoke.cvZero(image);

                if (need_to_init) {
                    /* automatic initialization */
                    //Image<Gray, byte> eig = CvInvoke.cvCreateImage( grey.Size, IPL_DEPTH.IPL_DEPTH_32F, 1 );
                    //Image<Gray, byte> temp = CvInvoke.cvCreateImage( grey.Size, IPL_DEPTH.IPL_DEPTH_32F, 1 );
                    //Image<Gray, byte> eig = new Image<Gray,byte>(grey.Size);
                    //Image<Gray, byte> temp = new Image<Gray,byte>(grey.Size);
                    double quality = 0.01;
                    double min_distance = 10;

                    count = MAX_COUNT;
                    PointF[][] corners;
                    corners = grey.GoodFeaturesToTrack(MAX_COUNT, quality, min_distance, 3, false, 0.04);
                    /*
                    CvInvoke.cvGoodFeaturesToTrack( grey.Ptr, eig.Ptr, temp.Ptr, points[1], ref count,
                                           quality, min_distance, null, 3, 0, 0.04 );
                     */
                    grey.FindCornerSubPix(corners,
                        new Size(win_size, win_size), new Size(-1, -1),
                        new MCvTermCriteria(20, 0.03));
                    /*
                    CvInvoke.cvFindCornerSubPix( grey, points[1], count,
                        new Size(win_size, win_size), new Size(-1,-1),
                        new MCvTermCriteria(20,0.03));
                    */
                    //CvInvoke.cvReleaseImage( &eig );
                    //CvInvoke.cvReleaseImage( &temp );

                    points[1] = corners[0];
                    count = points[1].Length;

                    add_remove_pt = false;
                } else if (count > 0) {
                    float[] trackError;
                    OpticalFlow.PyrLK(prev_grey, grey, prev_pyramid, pyramid, points[0], new Size(win_size, win_size), 3, new MCvTermCriteria(20, 0.03), flags, out points[1], out status, out trackError);
                    /*
                    CvInvoke.cvCalcOpticalFlowPyrLK( prev_grey.Ptr, grey.Ptr, prev_pyramid.Ptr, pyramid.Ptr,
                        points[0], points[1], count, new Size(win_size, win_size), 3, status, null,
                        new MCvTermCriteria(20, 0.03), flags );
                     */
                    flags |= LKFLOW_TYPE.CV_LKFLOW_PYR_A_READY;
                    for (i = k = 0; i < count; i++) {
                        if (add_remove_pt) {
                            double dx = pt.X - points[1][i].X;
                            double dy = pt.Y - points[1][i].Y;

                            if (dx * dx + dy * dy <= 25) {
                                add_remove_pt = false;
                                continue;
                            }
                        }

                        if (status[i] == 0)
                            continue;

                        points[1][k++] = points[1][i];
                        //CvInvoke.cvCircle( image, CvInvoke.cvPointFrom32f(points[1, i]), 3, CV_RGB(0,255,0), -1, 8,0);
                        image.Draw(new CircleF(new Point((int)points[1][i].X, (int)points[1][i].Y), 3), new Bgr(0, 255, 0), -1);
                    }
                    count = k;
                }

                if (add_remove_pt && count < MAX_COUNT) {
                    points[1][count++] = pt;
                    PointF[] tmpSinglePointArray = new PointF[1];
                    tmpSinglePointArray[0] = pt;
                    CvInvoke.cvFindCornerSubPix(grey, tmpSinglePointArray, 1,
                        new Size(win_size, win_size), new Size(-1, -1),
                        new MCvTermCriteria(20, 0.03));
                    add_remove_pt = false;
                }

                prev_grey.Bitmap = grey.Bitmap;
                prev_pyramid.Bitmap = pyramid.Bitmap;
                points[0] = points[1];
                //CV_SWAP( prev_grey, grey, swap_temp );
                //CV_SWAP( prev_pyramid, pyramid, swap_temp );
                //CV_SWAP( points[0], points[1], swap_points );
                need_to_init = false;
                CvInvoke.cvShowImage("LkDemo", image);

                c = CvInvoke.cvWaitKey(10);
                if ((char)c == 27)
                    break;
                switch ((char)c) {
                    case 'r':
                        need_to_init = true;
                        break;
                    case 'c':
                        count = 0;
                        break;
                    case 'n':
                        night_mode = !night_mode;
                        break;
                    default:
                        break;
                }
            }

            //CvInvoke.cvReleaseCapture( &capture );
            CvInvoke.cvDestroyWindow("LkDemo");
        }
    }
}
