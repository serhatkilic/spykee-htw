using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;

namespace SpykeeControl.Answer {
    public class SpykeeAnswerImage : SpykeeAnswer {
    	//private ImageIcon img;

	    public SpykeeAnswerImage(byte[] imageData) {
            System.Console.WriteLine("Got image! Total array length: " + imageData);
            //Image<Bgr, byte> image = new Image<Bgr, byte>();
		    //img = new ImageIcon(imageData);
		    //System.out.println("Answer Image: " + toString());
	    }
    	
        /*
	    public ImageIcon getImg() {
		    return img;
	    }
         */
    	
	    public override string ToString() {
            String s = "[SpykeeAnswerImage]";
		    return s;
	    }
    }
}
