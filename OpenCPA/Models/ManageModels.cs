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
}