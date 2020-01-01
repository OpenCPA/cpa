using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite;

namespace OpenCPA.Data
{
    /// <summary>
    /// Represents a single track by an artist.
    /// </summary>
    [Table("Tracks")]
    public class Track
    {
        /// <summary>
        /// The table unique ID of this track.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// The English name of this track.
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// The name of this track in its native language.
        /// </summary>
        public string NativeName { get; set; }

        /// <summary>
        /// Length of the track in seconds.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The ID of the artist that created this track.
        /// </summary>
        public int Artist { get; set; }

        /// <summary>
        /// Comma delimited IDs of the album(s) this track originates from.
        /// </summary>
        public int Album { get; set; }

        /// <summary>
        /// The original (first release) release year of this track.
        /// </summary>
        public int ReleaseYear { get; set; }

        /// <summary>
        /// The GUID of the file stored on disk.
        /// </summary>
        public string FileGUID { get; set; }
    }
}