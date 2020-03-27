using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using OpenCPA.Data;
using OpenCPA.Database;
using OpenCPA.Models;
using OpenCPA.Security;

namespace OpenCPA.Modules
{
    /// <summary>
    /// Represents the administrator pages on OCPA.
    /// </summary>
    public class ManagementModule : NancyModule
    {
        public ManagementModule() : base("/manage")
        {
            //Require an administrator login.
            this.RequiresClaims((Claim c) => { return c.Type == "Admin"; });

            //The root management route.
            Get("/", (req) =>
            {
                return View["manage", new ManageModel()];
            });

            //User management page.
            Get("/users", (req) =>
            {
                //Get all users from the database.
                var users = DBMan.Instance.Query<User>("SELECT * FROM Users");
                var msg = Request.Query.msg;
                return View["manage_users", new ManageUserModel(this.Request.Query.err, msg, users)];
            });

            //Delete/reset users.
            Get("/users/deleteuser", (req) =>
            {
                //Get the username of the user to delete.
                var user = this.Request.Query.user;
                if (user == null) { return Response.AsRedirect("/manage/users?err=No user specified."); }
                string username = user.Value;

                //Delete the supplied user.
                DBMan.Instance.Query<User>("DELETE FROM Users WHERE Username=?", username);

                //Redirect.
                return Response.AsRedirect("/manage/users?msg=Successfully deleted user.");
            });
            Get("/users/resetuser", (req) =>
            {
                //Get the username of the user to reset.
                var user = this.Request.Query.user;
                if (user == null) { return Response.AsRedirect("/manage/users?err=No user specified."); }
                string username = user.Value;

                //Get the user from database, edit and update.
                User thisUser = DBMan.Instance.FindWithQuery<User>("DELETE FROM Users WHERE Username=?", username);
                if (thisUser == null) { return Response.AsRedirect("/manage/users?err=That user does not exist."); }

                //Generate a cryptographically secure random string.
                string newPassword = Hash.GetCryptoSecureString(DBMan.Settings.PasswordResetLength);
                thisUser.HashedPassword = Hash.Create(newPassword, DBMan.Settings.PasswordHashStrength);

                //Update user. Redirect to page displaying new login details.
                DBMan.Instance.Update(thisUser);
                return View["/manage/user_reset", new UserResetModel(newPassword)];
            });

            //Create a new user.
            //...

            /////////////////////////
            /// ARTIST MANAGEMENT ///
            /////////////////////////

            Get("/artists", (req) =>
            {
                //Get all artists from the database.
                var artists = DBMan.Instance.Query<Artist>("SELECT * FROM Artists");
                var msg = Request.Query.msg;
                return View["manage_artists", new ManageArtistsModel(this.Request.Query.err, msg, artists)];
            });

            //Creating an artist with POST.
            Post("/artists/create", (req) =>
            {
                //Bind to artist creation model.
                var artistCreate = this.Bind<ArtistCreateModel>();

                //Is the profile URL link valid?
                if (!artistCreate.ProfilePictureLink.EndsWith(".png") && !artistCreate.ProfilePictureLink.EndsWith(".jpg"))
                {
                    return Response.AsRedirect("/manage/artists?err=Profile picture must be a PNG or JPG file.");
                }

                //Download the resource.
                string guid = ResourceMan.DownloadResourceFromURL(artistCreate.ProfilePictureLink, ResourceType.IMAGE);
                if (guid == null)
                {
                    return Response.AsRedirect("/manage/artists?err=Invalid profile picture link, please try again.");
                }

                //Create the artist.
                var thisArtist = new Artist()
                {
                    ProfileGUID = guid,
                    EnglishName = artistCreate.EnglishName,
                    NativeName = artistCreate.NativeName,
                    Description = artistCreate.Description
                };
                DBMan.Instance.Insert(thisArtist);

                return Response.AsRedirect("/manage/artists?msg=Successfully created artist '" + thisArtist.EnglishName + "'.");
            });
        }
    }
}