using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Security
{
    public class LoginModel
    {
        public string loginID { get; set; }
        public string loginPassword { get; set; }

        public string timezone { get; set; }

        public LoginModel()
        {
            loginID = "";
            loginPassword = "";
            timezone = "";
        }

        public static LoginModel CreateLoginModel (LoginModel vm)
        {
            return new LoginModel
            {
                loginID = vm.loginID,
                loginPassword = vm.loginPassword,
                timezone = vm.timezone
            };
        }
    }
}