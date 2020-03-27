using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite;

namespace OpenCPA.Data
{
    /// <summary>
    /// Represents a single image or music resource on file.
    /// </summary>
    [Table("Resources")]
    public class Resource
    {
        /// <summary>
        /// The table unique ID of this resource.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// The globally unique ID of this resource.
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// The type of resource.
        /// </summary>
        public ResourceType Type { get; set; }
    }

    /// <summary>
    /// Describes a single type of resource on file.
    /// </summary>
    public enum ResourceType
    {
        IMAGE,
        AUDIO
    }
}