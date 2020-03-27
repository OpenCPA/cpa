using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite;

namespace OpenCPA.Data
{
    /// <summary>
    /// Represents a single artist in the OpenCPA database.
    /// </summary>
    [Table("Artists")]
    public class Artist
    {
        /// <summary>
        /// The table unique ID of the artist.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// Artist name (full, English).
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// Name in the artist's native language.
        /// </summary>
        public string NativeName { get; set; }

        /// <summary>
        /// Description of this artist.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The GUID of the profile picture resource for this artist.
        /// </summary>
        public string ProfileGUID { get; set; }
    }
}