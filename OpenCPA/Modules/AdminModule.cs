using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Nancy;
using Nancy.Security;
using OpenCPA.Models;

namespace OpenCPA.Modules
{
    /// <summary>
    /// Represents the administrator pages on OCPA.
    /// </summary>
    public class AdminModule : NancyModule
    {
        public AdminModule() : base("/manage")
        {
            //Require an administrator login.
            this.RequiresClaims((Claim c) => { return c.Type == "Admin"; });

            //The root management route.
            Get("/", (req) =>
            {
                return View["manage", new ManageModel()];
            });
        }
    }
}