using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpykeeControl.Command {
    public class SpykeeCommandLogin : SpykeeCommand {
        public enum Directions {
            Forward, Backwards, Left, Right
        }

        public String Username { get; private set; }
        public String Password { get; private set; }

        public SpykeeCommandLogin(String username, String password) {
            Username = username;
            Password = password;

            SetFirstLine(10, 0, username.Length + password.Length + 2);
            SetSecondLine((char)username.Length + username + (char)password.Length + password);
        }
    }
}
