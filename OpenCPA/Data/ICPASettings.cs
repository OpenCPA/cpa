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
    }
}
