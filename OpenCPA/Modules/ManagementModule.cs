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

            //Editing an album.
            Get("/albums/edit", (req) =>
            {
                //Is there an ID attached?
                if (Request.Query.id == null)
                {
                    return Response.AsRedirect("/manage/albums?err=No ID to edit provided.");
                }

                //Try to find the album with that ID.
                Album album = DBMan.Instance.FindWithQuery<Album>("SELECT * FROM Albums WHERE ID=?", Request.Query.id.Value);
                if (album == null)
                {
                    return Response.AsRedirect("/manage/albums?err=Invalid album ID to edit.");
                }

                //Loop through all tracks and add them to a list.
                List<Track> tracks = new List<Track>();
                if (album.Tracks != null && album.Tracks != "")
                {
                    string[] trackIDs = album.Tracks.Split(',');
                    foreach (var track in trackIDs)
                    {
                        //Search for that ID in the DB.
                        int tid;
                        try
                        {
                            tid = int.Parse(track);
                        }
                        catch { continue; }
                        Track trackObj = DBMan.Instance.FindWithQuery<Track>("SELECT * FROM Tracks WHERE ID=?", track);
                        if (trackObj == null) { continue; } //could not find

                        //Add to list.
                        tracks.Add(trackObj);
                    }
                }

                //Load the view.
                return View["edit_album", new AlbumEditModel(tracks, album, this.Request.Query.msg, this.Request.Query.err)];
            });

            //Deleting an album.
            Get("/albums/delete", (req) =>
            {
                //Is an ID supplied?
                if (Request.Query.id == null)
                {
                    return Response.AsRedirect("/manage/albums?err=No ID supplied for album to delete.");
                }

                //Attempt to get album by ID.
                Album album = DBMan.Instance.FindWithQuery<Album>("SELECT * FROM Albums WHERE ID=?", Request.Query.id);
                if (album == null)
                {
                    return Response.AsRedirect("/manage/albums?err=Invalid album ID provided to delete.");
                }

                //Get all the track IDs out from that album, grab each and delete resources then themselves.
                if (album.Tracks != null && album.Tracks != "")
                {
                    string[] trackIDs = album.Tracks.Split(',');
                    foreach (var track in trackIDs)
                    {
                        //Search for that ID in the DB.
                        Track trackObj = DBMan.Instance.FindWithQuery<Track>("SELECT * FROM Tracks WHERE ID=?", track);
                        if (trackObj == null) { continue; } //could not find

                        //Delete the file resource for this track.
                        ResourceMan.DeleteResource(trackObj.FileGUID);

                        //Delete the track.
                        DBMan.Instance.Delete(trackObj);
                    }
                }

                //Delete the album art.
                ResourceMan.DeleteResource(album.AlbumArtGUID);

                //Delete the album.
                DBMan.Instance.Delete(album);

                //Done!
                return Response.AsRedirect("/manage/albums?msg=Successfully deleted album and all album tracks.");
            });

            //Edit an album.
            Post("/albums/edit", (req) =>
            {
                var model = this.Bind<AlbumCreateModel>();

                //Is there an album ID attached?
                if (Request.Query.id == null)
                {
                    return Response.AsRedirect("/manage/albums?err=No album ID provided to edit.");
                }

                //Get the ID out.
                int albumID;
                try
                {
                    albumID = int.Parse(Request.Query.id.ToString());
                }
                catch
                {
                    return Response.AsRedirect("/manage/albums?err=Invalid album ID provided to edit.");
                }

                //Get the album out.
                Album album = DBMan.Instance.FindWithQuery<Album>("SELECT * FROM Albums WHERE ID=?", albumID);
                if (album == null)
                {
                    return Response.AsRedirect("/manage/albums?err=Invalid album ID provided to edit.");
                }

                //Edit the album properties.
                album.EnglishName = model.EnglishName;
                album.NativeName = model.NativeName;
                album.ReleaseYear = model.ReleaseYear;
                
                //Edited album art?
                if (model.AlbumArtLink != null && model.AlbumArtLink != "")
                {
                    //Create the new resource.
                    string guid = "";
                    if (model.AlbumArtLink != null && model.AlbumArtLink != "")
                    {
                        guid = ResourceMan.DownloadResourceFromURL(model.AlbumArtLink, ResourceType.IMAGE);
                        if (guid == null)
                        {
                            return Response.AsRedirect("/manage/albums?err=Invalid profile picture link, please try again.");
                        }
                    }

                    //Delete the old one, set album art URL.
                    ResourceMan.DeleteResource(album.AlbumArtGUID);
                    album.AlbumArtGUID = guid;
                }

                //Update the album. Success!
                DBMan.Instance.Update(album);
                return Response.AsRedirect("/manage/albums/edit?id=" + albumID + "&msg=Successfully updated album.");
            });

            ////////////////////////
            /// TRACK MANAGEMENT ///
            ////////////////////////

            //Adding a track to an album.
            Post("/tracks/add", (req) =>
            {
                //Bind POST data.
                var model = this.Bind<AddTrackModel>();
                if (model.File == null)
                {
                    return Response.AsRedirect("/manage/albums?err=No file provided for this track.");
                }

                //Get the album ID from the query.
                if (Request.Query.album == null)
                {
                    return Response.AsRedirect("/manage/albums?err=No album supplied to add the track to.");
                }

                //Get the album out.
                string albumId = Request.Query.album.Value;
                Album album = DBMan.Instance.FindWithQuery<Album>("SELECT * FROM Albums WHERE ID=?", albumId);
                if (album == null)
                {
                    return Response.AsRedirect("/manage/albums/edit?id=" + albumId + "&err=Invalid album ID given to add track to.");
                }

                //Parse length.
                int trackLength;
                try
                {
                    trackLength = int.Parse(model.Length);
                }
                catch
                {
                    return Response.AsRedirect("/manage/albums/edit?id=" + albumId + "&err=Invalid track length.");
                }

                //Create the track from the uploaded file.
                string guid = ResourceMan.MakeResourceFromPOST(model.File, ResourceType.AUDIO);
                if (guid == null)
                {
                    return Response.AsRedirect("/manage/albums/edit?id=" + albumId + "&err=Invalid track file uploaded. Please try again.");
                }

                //Create the track.
                Track track = new Track()
                {
                    FileGUID = guid,
                    Album = album.ID,
                    EnglishName = model.EnglishName,
                    NativeName = model.NativeName,
                    Length = trackLength
                };
                DBMan.Instance.Insert(track);

                //Add track ID to the list of tracks.
                List<string> tracks = (album.Tracks == null || album.Tracks == "") ? new List<string>() : album.Tracks.Split(',').ToList();
                tracks.RemoveAll(x => x == "");
                tracks.Add(track.ID.ToString());
                
                //Update the album.
                album.Tracks = string.Join(",", tracks).Trim(',');
                DBMan.Instance.Update(album);

                //Success!
                return Response.AsRedirect("/manage/albums/edit?id=" + albumId + "&msg=Successfully added track to album.");
            });

            //Delete a track.
            Get("/tracks/delete", (req) =>
            {
                //Get the ID to delete.
                if (Request.Query.id == null)
                {
                    return Response.AsRedirect("/manage/albums?err=No track ID given to delete.");
                }

                //Get the track to delete.
                Track track = DBMan.Instance.FindWithQuery<Track>("SELECT * FROM Tracks WHERE ID=?", Request.Query.id.ToString());
                if (track == null)
                {
                    return Response.AsRedirect("/manage/albums?err=Invalid track ID supplied.");
                }

                //Get the album, remove the track from the track list.
                Album album = DBMan.Instance.FindWithQuery<Album>("SELECT * FROM Albums WHERE ID=?", track.Album);
                if (album != null)
                {
                    List<string> tracks = (album.Tracks == null || album.Tracks == "") ? new List<string>() : album.Tracks.Split(',').ToList();
                    tracks.RemoveAll(x => x == track.ID.ToString());
                    album.Tracks = string.Join(",", tracks).Trim(',');
                    DBMan.Instance.Update(album);
                }

                //Delete the resource for the track.
                ResourceMan.DeleteResource(track.FileGUID);

                //Delete the track.
                DBMan.Instance.Delete(track);
                return Response.AsRedirect("/manage/albums/edit?id=" + album.ID + "&msg=Successfully deleted track.");
            });
        }
    }
}