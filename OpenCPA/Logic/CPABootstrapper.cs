using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using OpenCPA.Data;
using OpenCPA.Database;

public class CPABootstrapper : DefaultNancyBootstrapper
{
    /// <summary>
    /// Run immediately upon startup.
    /// </summary>
    protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
    {
        //Initialize the database and settings.
        DBMan.Initialize();

        //Enable authentication for all modules.
        FormsAuthentication.Enable(pipelines, new FormsAuthenticationConfiguration()
        {
            UserMapper = new UserDB(), //maps GUIDs to users
            RedirectUrl = "/" //where to redirect upon failed auth.
        });
    }


}