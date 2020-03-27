using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Config.Net;

namespace OpenCPA.Data
{
    /// <summary>
    /// Represents the settings for OpenCPA.
    /// </summary>
    public interface ICPASettings
    {
        /// <summary>
        /// The name of the database file on disk.
        /// </summary>
        [Option(DefaultValue = "opencpa.db")]
        string DBName { get; }

        /// <summary>
        /// The password hashing strength. Must be >=1000.
        /// </summary>
        [Option(DefaultValue = 2000)]
        int PasswordHashStrength { get; }

        /// <summary>
        /// Username of the default user.
        /// </summary>
        [Option(DefaultValue = "ocpadmin")]
        string DefaultUserUsername { get; }

        /// <summary>
        /// Password for the default user.
        /// </summary>
        [Option(DefaultValue = "changethisplease")]
        string DefaultUserPassword { get; }

        /// <summary>
        /// The default user's permissions.
        /// </summary>
        [Option(DefaultValue = "Admin")]
        string DefaultUserPermissions { get; }

        /// <summary>
        /// The amount of hours that a user's cookie is valid for.
        /// </summary>
        [Option(DefaultValue = 24)]
        int LoginTime { get; }

        /// <summary>
        /// The length of the password given to a user when their account password is reset.
        /// </summary>
        [Option(DefaultValue = 15)]
        int PasswordResetLength { get; }

        /// <summary>
        /// The amount of albums to display per page in the management console and on site search.
        /// </summary>
        [Option(DefaultValue = 2)]
        int AlbumsPerPage { get; }
    }
}
