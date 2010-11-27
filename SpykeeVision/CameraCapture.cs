using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace SpykeeVision {
    public class CameraCapture {
        private Capture capture;
        private Image<Bgr, byte> frame;

        public CameraCapture(int cameraIndex) {
            capture = new Capture(cameraIndex);

            if (capture == null) {
                throw new Exception("Could not initialize capturing...\n");
            }

            Update();
        }

        public void Update() {
            frame = capture.QueryFrame();
        }

        public Image<Bgr, byte> Frame { get { return frame; } }

        public Image<Bgr, byte> CreateImageBgrCopy() {
            return new Image<Bgr, byte>(frame.Bitmap);
        }

        public Image<Gray, byte> CreateImageGreyCopy() {
            return new Image<Gray, byte>(frame.Bitmap);
        }

        public Size FrameSize { get { return frame.Size; } }
    }
}
