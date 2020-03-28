using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using OpenCPA;
using OpenCPA.Data;
using OpenCPA.Database;
using System;
using System.IO;

public class CPABootstrapper : DefaultNancyBootstrapper
{
    /// <summary>
    /// Run immediately upon startup.
    /// </summary>
    protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
    {
        //Initialize the database, resources and settings.
        DBMan.Initialize();
        ResourceMan.Initialize();
        
        //Enable authentication for all modules.
        FormsAuthentication.Enable(pipelines, new FormsAuthenticationConfiguration()
        {
            UserMapper = new UserDB(), //maps GUIDs to users
            RedirectUrl = "/login/noauth" //where to redirect upon failed auth.
        });
    }
}