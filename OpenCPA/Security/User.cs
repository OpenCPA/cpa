using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using Nancy;
using Nancy.Authentication.Forms;
using OpenCPA.Database;
using OpenCPA.Security;
using SQLite;

namespace OpenCPA.Data
{
    /// <summary>
    /// Represents a single authorised user in OCPA.
    /// </summary>
    [Table("Users")]
    public class User
    {
        /// <summary>
        /// Table unique ID of this user.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// Username of this user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The hashed password of this user.
        /// </summary>
        public string HashedPassword { get; set; }

        /// <summary>
        /// A comma delimited set of permissions that this user has.
        /// </summary>
        public string Permissions { get; set; }

        /// <summary>
        /// This user's GUID.
        /// </summary>
        public string GUID { get; set; }
    }

    /// <summary>
    /// Represents a single authorised user. These users can edit freely.
    /// </summary>
    public class UserDB : IUserMapper
    {
        /// <summary>
        /// Returns a user (if they exist) from the given GUID identifier.
        /// </summary>
        public ClaimsPrincipal GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            //Grab the user from the database with the given GUID.
            string guid = identifier.ToString();
            var user = DBMan.Instance.FindWithQuery<User>("SELECT * FROM Users WHERE GUID=?", guid);

            //Does the user exist?
            if (user == null) { return null; }

            //Yes, create a claims principal from that.
            var identity = new GenericIdentity(user.Username);
            if (user.Permissions != null && user.Permissions != "")
            {
                foreach (var perm in user.Permissions.Split(','))
                {
                    identity.AddClaim(new Claim(perm, ""));
                }
            }

            //Return.
            return new ClaimsPrincipal(identity);
        }

        /// <summary>
        /// Validates this user's credentials based on a username and password.
        /// </summary>
        public static Guid? ValidateUser(string username, string password)
        {
            //Are either of the fields nulled?
            if (username == null || password == null) { return null; }

            //Does the user exist?
            User user = DBMan.Instance.FindWithQuery<User>("SELECT * FROM Users WHERE Username=?", username);
            if (user == null) { return null; }

            //Is the password correct?
            if (!Hash.Verify(password, user.HashedPassword, DBMan.Settings.PasswordHashStrength))
            {
                return null;
            }

            //Everything checks out, return a GUID.
            return new Guid(user.GUID);
        }
    }
}