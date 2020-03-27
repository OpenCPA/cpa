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
                User thisUser = DBMan.Instance.FindWithQuery<User>("SELECT * FROM Users WHERE Username=?", username);
                if (thisUser == null) { return Response.AsRedirect("/manage/users?err=That user does not exist."); }

                //Generate a cryptographically secure random string.
                string newPassword = Hash.GetCryptoSecureString(DBMan.Settings.PasswordResetLength);
                thisUser.HashedPassword = Hash.Create(newPassword, DBMan.Settings.PasswordHashStrength);

                //Update user. Redirect to page displaying new login details.
                DBMan.Instance.Update(thisUser);
                return View["manage_user_reset", new UserResetModel(newPassword)];
            });

            //Create a new user.
            Post("/users/createuser", (req) =>
            {
                //Bind to user create model.
                var model = this.Bind<UserCreateModel>();

                //Check the username isn't a dupe.
                if (DBMan.Instance.FindWithQuery<User>("SELECT * FROM Users WHERE Username=?", model.Username) != null)
                {
                    return Response.AsRedirect("/manage/users?err=A user already exists with username'" + model.Username + "'.");
                }

                //Create the user.
                DBMan.Instance.Insert(new User()
                {
                    GUID = Guid.NewGuid().ToString(),
                    Username = model.Username,
                    HashedPassword = Hash.Create(model.Password, DBMan.Settings.PasswordHashStrength),
                    Permissions = model.Permissions
                });

                return Response.AsRedirect("/manage/users?msg=Successfully created user.");
            });

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

                //If a profile pic is specified, download the resource. This is for administrators only, don't verify file extension.
                string guid = "";
                if (artistCreate.ProfilePictureLink != null && artistCreate.ProfilePictureLink != "")
                {
                    guid = ResourceMan.DownloadResourceFromURL(artistCreate.ProfilePictureLink, ResourceType.IMAGE);
                    if (guid == null)
                    {
                        return Response.AsRedirect("/manage/artists?err=Invalid profile picture link, please try again.");
                    }
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

            //Artist editing (GET).
            Get("/artists/edit", (req) =>
            {
                //Is there an ID?
                if (Request.Query.id == null)
                {
                    return Response.AsRedirect("/manage/artists?err=Invalid artist ID to edit.");
                }
                int id = Request.Query.id;

                //Get the artist with that ID from the DB.
                Artist artist = DBMan.Instance.FindWithQuery<Artist>("SELECT * FROM Artists WHERE ID=?", id);
                if (artist == null)
                {
                    return Response.AsRedirect("/manage/artists?err=No artist exists with ID '" + id + "'.");
                }

                //Open the edit page.
                return View["manage_artists_edit", new EditArtistModel(artist)];
            });
            Post("/artists/edit", (req) =>
            {
                //Bind the model.
                var data = this.Bind<ArtistCreateModel>();

                //Get the ID out.
                if (Request.Query.id == null)
                {
                    return Response.AsRedirect("/manage/artists?err=Invalid artist ID to edit.");
                }
                int id = Request.Query.id;

                //Get the artist with that ID from the DB.
                Artist artist = DBMan.Instance.FindWithQuery<Artist>("SELECT * FROM Artists WHERE ID=?", id);
                if (artist == null)
                {
                    return Response.AsRedirect("/manage/artists?err=No artist exists with ID '" + id + "'.");
                }

                //Edit the fields as necessary.
                artist.EnglishName = data.EnglishName;
                artist.Description = data.Description;
                artist.NativeName = data.NativeName;

                //If the profile picture is different, download the new resource and delete the old one.
                if (data.ProfilePictureLink != null && data.ProfilePictureLink != "")
                {
                    string guid = ResourceMan.DownloadResourceFromURL(data.ProfilePictureLink, ResourceType.IMAGE);
                    if (guid != null)
                    {
                        ResourceMan.DeleteResource(artist.ProfileGUID);
                        artist.ProfileGUID = guid;
                    }
                    else
                    {
                        return Response.AsRedirect("/manage/artists?err=Invalid profile picture given. Please try again.");
                    }
                }

                //Update the artist, redirect.
                DBMan.Instance.Update(artist);
                return Response.AsRedirect("/manage/artists?msg=Successfully edited artist.");
            });

            //Deleting an artist.
            Get("/artists/delete", (req) =>
            {
                //Is an ID provided?
                if (Request.Query.id == null)
                {
                    return Response.AsRedirect("/manage/artists?err=Invalid artist ID provided.");
                }

                //Attempt to get that artist from the database.
                Artist artist = DBMan.Instance.FindWithQuery<Artist>("SELECT * FROM Artists WHERE ID=?", Request.Query.id.ToString());
                if (artist == null)
                {
                    return Response.AsRedirect("/manage/artists?err=Invalid artist ID provided.");
                }

                //Delete all resources associated with the artist.
                ResourceMan.DeleteResource(artist.ProfileGUID);

                //Delete that artist, return success.
                DBMan.Instance.Delete(artist);
                return Response.AsRedirect("/manage/artists?msg=Successfully deleted artist.");
            });

            ////////////////////////
            /// ALBUM MANAGEMENT ///
            ////////////////////////

            Get("/albums", (req) =>
            {
                //Get the page number to be accessed.
                int pageNum = 1;
                int albumsPerPage = DBMan.Settings.AlbumsPerPage;
                if (Request.Query.page != null)
                {
                    try
                    {
                        pageNum = int.Parse(Request.Query.page.ToString());
                    }
                    catch { }
                }
                if (pageNum < 1) { pageNum = 1; }

                //How many records are there? Valid page?
                int records = DBMan.Instance.Query<Album>("SELECT ID FROM Albums").Count;
                int recordsIn = (pageNum - 1) * albumsPerPage;
                if (recordsIn > records)
                {
                    //Not a valid page. Redirect to the very last page.
                    pageNum = records / albumsPerPage + 1;
                    return Response.AsRedirect("/manage/albums?page=" + pageNum);
                }

                //Get the albums for the page.
                var albums = DBMan.Instance.Query<Album>("SELECT * FROM Albums LIMIT ? OFFSET ?", albumsPerPage, recordsIn);

                //Get the albums to be displayed.
                int thisPage = recordsIn / albumsPerPage + 1;
                int lastPage = (thisPage - 1 < 1) ? 1 : thisPage - 1;
                int nextPage = (thisPage + 1 > records / albumsPerPage + 1) ? records / albumsPerPage + 1 : thisPage + 1;
                return View["manage_albums", new ManageAlbumsModel(Request.Query.err, Request.Query.msg, albums, lastPage, nextPage)];
            });
            Post("/albums", (req) =>
            {
                //SEARCH TERM
                //Bind model.
                var model = this.Bind<AlbumSearchModel>();

                //Posting for a search term.
                var albums = DBMan.Instance.Query<Album>("SELECT * FROM Albums WHERE EnglishName LIKE ?", model.SearchTerm);
                return View["manage_albums", new ManageAlbumsModel(Request.Query.err, Request.Query.msg, albums, 1, 1)];
            });

            //Creating an album.
            Post("/albums/create", (req) =>
            {
                //Bind the model data out.
                var model = this.Bind<AlbumCreateModel>();

                //Check the artist ID is valid.
                if (DBMan.Instance.Query<Artist>("SELECT ID FROM Artists WHERE ID=?", model.Artist).Count == 0)
                {
                    return Response.AsRedirect("/manage/albums?err=Invalid artist ID given.");
                }

                //Attempt to download the profile resource (if provided).
                string guid = "";
                if (model.AlbumArtLink != null && model.AlbumArtLink != "")
                {
                    guid = ResourceMan.DownloadResourceFromURL(model.AlbumArtLink, ResourceType.IMAGE);
                    if (guid == null)
                    {
                        return Response.AsRedirect("/manage/albums?err=Invalid profile picture link, please try again.");
                    }
                }

                //Create the album.
                DBMan.Instance.Insert(new Album()
                {
                    AlbumArtGUID = guid,
                    Artist = model.Artist,
                    EnglishName = model.EnglishName,
                    NativeName = model.NativeName,
                    ReleaseYear = model.ReleaseYear,
                    Tracks = ""
                });

                return Response.AsRedirect("/manage/albums?msg=Successfully created album.");
            });

            //Deleting an album.
            //...
        }
    }
}