using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenCPA.Data
{
    /// <summary>
    /// Represents a single edit on the OpenCPA.
    /// </summary>
    [Table("Edits")]
    public class Edit
    {
        /// <summary>
        /// The table unique ID of this edit.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The string message for this edit.
        /// </summary>
        public string EditMessage { get; set; }

        /// <summary>
        /// The link to the edited resource, relative to root.
        /// </summary>
        public string RelativeLink { get; set; }
    }
}