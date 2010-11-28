using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpykeeControl.Answer {
    public class SpykeeAnswerBattery : SpykeeAnswer {
	    private int batteryLevel = 0;

	    public SpykeeAnswerBattery(byte[] b) {
		    batteryLevel = b[0];
	    }

	    public int getBatteryLevel() {
		    return batteryLevel;
	    }

	    public override string ToString() {
            String s = "[SpykeeAnswerBattery BatteryLevel: " + batteryLevel + "]";
		    return s;
	    }
    }
}
