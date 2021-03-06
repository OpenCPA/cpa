﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenCPA.Models
{
    /// <summary>
    /// Model for dynamic data on the login page.
    /// </summary>
    public class LoginModel : CPAModel
    {
        public string PageName { get; set; } = "Login";
        public string Error { get; set; } = null;
        public bool HasError { get; set; } = false;

        public string Redirect { get; set; } = "/";

        public LoginModel(string err, string redirect)
        {
            Error = err;
            if (Error != null)
            {
                HasError = true;
            }

            if (redirect != null && redirect != "")
            {
                Redirect = redirect;
            }
        }
    }
}