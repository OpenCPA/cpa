using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Security;
using Nancy.Extensions;
using Nancy.Authentication.Forms;
using OpenCPA.Data;
using Nancy.ModelBinding;
using OpenCPA.Models;
using OpenCPA.Database;

namespace OpenCPA
{
    /// <summary>
    /// The login module, handles secure login/logout.
    /// </summary>
    public class LoginModule : NancyModule
    {
        public LoginModule()
        {
            //The main login page.
            Get("/login", (req) =>
            {
                //Is there an error to display?
                string err = this.Request.Query.err;
                if (err != null && err.Contains("?")) { err = err.Split('?')[0]; }

                //Get a redirect, if there is one.
                string redirect = this.Request.Query.returnUrl;

                //Pass in and render.
                return View["login", new LoginModel(err, redirect)];
            });

            //API for logging in and logging out.
            Get("/logout", (req) =>
            {
                //Log the user out.
                return this.LogoutAndRedirect("/");
            });

            Post("/login", (req) =>
            {
                //Bind to login model.
                LoginDetails login = this.Bind<LoginDetails>();

                //Validate the username/password.
                var userGuid = UserDB.ValidateUser(login.Username, login.Password);
                if (userGuid == null) { return this.Context.GetRedirect("/login?err=Invalid username or password."); }
                Guid user = (Guid)userGuid;

                //Is there a redirect?
                string redirUrl = "/";
                if (Request.Query.redirect != null)
                {
                    redirUrl = Request.Query.redirect.ToString();
                }

                //Configuring cookie expiry.
                return this.LoginAndRedirect(user, DateTime.Now.AddHours(DBMan.Settings.LoginTime), redirUrl);
            });

            //User attempted to access page without authorization.
            Get("/login/noauth", (req) =>
            {
                //Redirect to proper login page, fixing arguments.
                string retUrl = Request.Query.returnUrl.ToString();
                return Response.AsRedirect("/login?err=You were not authorized to view that page.&returnUrl=" + retUrl);
            });
        }
    }
}