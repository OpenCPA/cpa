using Nancy;
using OpenCPA.Data;
using System.Collections.Generic;
using System.Reflection;

namespace OpenCPA.Models
{
    /// <summary>
    /// Represents dynamic data related to the management index page.
    /// </summary>
    public class ManageModel : CPAModel
    {
        public string PageName { get; set; } = "Manage";
        public string Version
        {
            get
            {
                return "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }

    /// <summary>
    /// Represents dynamic data related to the manage user page.
    /// </summary>
    public class ManageUserModel : ManageModel
    {
        public new string PageName { get; set; } = "Manage Users";

        /// <summary>
        /// The error that has been sent to this page.
        /// </summary>
        public string Error { get; set; } = null;

        /// <summary>
        /// The status message that has been passed to this page.
        /// </summary>
        public string Message { get; set; } = null;

        /// <summary>
        /// The list of users to manage.
        /// </summary>
        public List<User> Users { get; set; } = new List<User>();

        public ManageUserModel(string err, string msg, List<User> users)
        {
            Error = err;
            Users = users;
            Message = msg;
        }
    }

    /// <summary>
    /// Used to access a password reset page in OCPA.
    /// </summary>
    public class UserResetModel : ManageModel
    {
        public new string PageName { get; set; } = "Password Reset!";
        public string Password { get; set; }
        public UserResetModel(string newPass)
        {
            Password = newPass;
        }
    }

    /// <summary>
    /// Bindings for creating a user.
    /// </summary>
    public class UserCreateModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Permissions { get; set; }
    }

    /// <summary>
    /// Model for viewing the "Manage Artists" page.
    /// </summary>
    public class ManageArtistsModel : ManageModel
    {
        public new string PageName { get; set; } = "Manage Artists";

        /// <summary>
        /// The error that has been sent to this page.
        /// </summary>
        public string Error { get; set; } = null;

        /// <summary>
        /// The status message that has been passed to this page.
        /// </summary>
        public string Message { get; set; } = null;

        /// <summary>
        /// The list of artists to manage.
        /// </summary>
        public List<Artist> Artists { get; set; } = new List<Artist>();

        public ManageArtistsModel(string err, string msg, List<Artist> artists)
        {
            Error = err;
            Artists = artists;
            Message = msg;
        }
    }

    /// <summary>
    /// Model for creating an artist.
    /// </summary>
    public class ArtistCreateModel
    {
        public string EnglishName { get; set; }
        public string NativeName { get; set; }
        public string ProfilePictureLink { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Model for editing a single artist.
    /// </summary>
    public class EditArtistModel : ManageModel
    {
        public new string PageName { get; set; } = "Edit Artist";
        public Artist Artist { get; set; }
        public EditArtistModel(Artist a)
        {
            Artist = a;
        }
    }

    /// <summary>
    /// Model for viewing the "Manage Artists" page.
    /// </summary>
    public class ManageAlbumsModel : ManageModel
    {
        public new string PageName { get; set; } = "Manage Albums";

        /// <summary>
        /// The error that has been sent to this page.
        /// </summary>
        public string Error { get; set; } = null;

        /// <summary>
        /// The status message that has been passed to this page.
        /// </summary>
        public string Message { get; set; } = null;

        public int LastPage { get; set; }
        public int NextPage { get; set; }

        /// <summary>
        /// The list of artists to manage.
        /// </summary>
        public List<Album> Albums = new List<Album>();

        public ManageAlbumsModel(string err, string msg, List<Album> albums, int lastPage, int nextPage)
        {
            Error = err;
            Albums = albums;
            Message = msg;
            LastPage = lastPage;
            NextPage = nextPage;
        }
    }

    /// <summary>
    /// Model for POST data searching the albums page.
    /// </summary>
    public class AlbumSearchModel
    {
        public string SearchTerm { get; set; }
    }

    /// <summary>
    /// Bind model for creating a new album.
    /// </summary>
    public class AlbumCreateModel
    { 
        public string EnglishName { get; set; }
        public string NativeName { get; set; }
        public string AlbumArtLink { get; set; }
        public int ReleaseYear { get; set; }
        public int Artist { get; set; }
    }

    /// <summary>
    /// Data for loading the "Edit Album" page.
    /// </summary>
    public class AlbumEditModel : ManageModel
    {
        public List<Track> Tracks;
        public Album Album;
        /// <summary>
        /// The error that has been sent to this page.
        /// </summary>
        public string Error { get; set; } = null;

        /// <summary>
        /// The status message that has been passed to this page.
        /// </summary>
        public string Message { get; set; } = null;
        public new string PageName { get; set; } = "Edit Album";

        public AlbumEditModel(List<Track> tracks, Album album, string msg, string err)
        {
            Tracks = tracks;
            Album = album;
            Error = err;
            Message = msg;
        }
    }

    /// <summary>
    /// Model for adding tracks to an album.
    /// </summary>
    public class AddTrackModel
    {
        public HttpFile File;
        public string EnglishName;
        public string NativeName;
        public string Length;
    }
}