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
}