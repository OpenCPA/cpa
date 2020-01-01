using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite;

namespace OpenCPA.Data
{
    /// <summary>
    /// Represents a single album in the OpenCPA database.
    /// </summary>
    [Table("Albums")]
    public class Album
    {
        /// <summary>
        /// The table unique ID of this album.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// The English name of the album.
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// The name of the album in it's native language.
        /// </summary>
        public string NativeName { get; set; }

        /// <summary>
        /// The ID of the artist which created this album.
        /// </summary>
        public int Artist { get; set; }

        /// <summary>
        /// A comma delimited list of track IDs in this album.
        /// </summary>
        public string Tracks { get; set; }

        /// <summary>
        /// The release year of the album.
        /// </summary>
        public int ReleaseYear { get; set; }

        /// <summary>
        /// The album art GUID.
        /// </summary>
        public string AlbumArtGUID { get; set; }
    }
}