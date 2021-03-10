using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.ErrorManagement
{
    public class LoginError
    {
        public string errorType { get; set; }
        public bool isError { get; set; }
        public string errorMessage { get; set; }

        public LoginError()
        {

        }

        public static LoginError CreateModel(string errorType, bool isError, string errorMessage)
        {
            return new LoginError
            {
                errorMessage = errorMessage,
                errorType = errorType,
                isError = isError
            };
        }
    }
}