using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpykeeControl.Answer {
    public class SpykeeAnswerLogin : SpykeeAnswer {
	    private bool bSuccessFullLogin = false;
	    private String spykeeName = "";
	    private String spykeeName2 = "";
	    private String spykeePass = "";
	    private String firmWareVersion = "";
    	
	    public SpykeeAnswerLogin(byte[] buf) {
		    // 1,10,...,6,49,46,48,46,50,50,1,1,2,Socket closed
		    if ( (buf.Length > 0) && (buf[0] == 1) ) {		// login successful
			    bSuccessFullLogin = true;
			    int pointer = 2;
			    while ((pointer < buf.Length) && (buf[pointer] != 10)) {
				    spykeeName += (char)buf[pointer];
				    pointer++;
			    }
			    pointer++;
			    while ((pointer < buf.Length) && (buf[pointer] != 7)) {
				    spykeeName2 += (char)buf[pointer];
				    pointer++;
			    }
			    pointer++;
			    while ((pointer < buf.Length) && (buf[pointer] != 6)) {
				    spykeePass += (char)buf[pointer];
				    pointer++;
			    }
			    pointer++;
			    while ((pointer < buf.Length) && (buf[pointer] != 1)) {
				    firmWareVersion += (char)buf[pointer];
				    pointer++;
			    }
    			
		    // 0,1,Connection reset
		    } else {									// login NOT successful
			    spykeeName = "unsuccessful login";
			    bSuccessFullLogin = false;
		    }
	    }

	    public bool isbSuccessFullLogin() {
		    return bSuccessFullLogin;
	    }

	    public override string ToString() {
            if (bSuccessFullLogin) {
                return "[SpykeeAnswerLogin spykeeName: '" + spykeeName + "', spykeeName2: '" + spykeeName2 + "', spykeePass: '" + spykeePass + "', firmWareVersion: '" + firmWareVersion + "']";
            } else {
		        return "[SpykeeAnswerLogin: unsuccessful login]";
            }
	    }	
    }
}
