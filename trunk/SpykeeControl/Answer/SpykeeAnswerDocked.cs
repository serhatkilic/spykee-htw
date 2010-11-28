using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpykeeControl.Answer {
    public class SpykeeAnswerDocked : SpykeeAnswer {
	    private bool bDocked = true;

	    public SpykeeAnswerDocked(byte[] buf) {
		    if (buf.Length > 0) {		// Spykee answered correctly
			    if ( buf[0] == 2 ) {
				    bDocked = true;
			    } else if ( buf[0] == 1 ) {
				    bDocked = false;
			    } else {
				    System.Console.WriteLine("SpykeeAnswerDocked: unknown return value (1)");					
			    }
    				
		    } else {										// something went wrong
			    System.Console.WriteLine("SpykeeAnswerDocked: unknown return value (2)");
		    }
		    System.Console.WriteLine("Answer Docked: " + ToString());
	    }

	    public bool isbDocked() {
		    return bDocked;
	    }

	    public override string ToString() {
            String s = "[SpykeeAnswerDocked Docked: " + isbDocked() + "]";
		    return s;
	    }
    }
}
