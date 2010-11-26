using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpykeeControl.Command {
    public class SpykeeCommandMoveStop : SpykeeCommand {
        public SpykeeCommandMoveStop() {
            SetFirstLine(5, 0, 2);
            SetSecondLine(0, 0);
        }
    }
}
