using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenCPA.Models
{
    /// <summary>
    /// Describes a set of login details for the site.
    /// </summary>
    public class LoginDetails
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}