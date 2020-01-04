using OpenCPA.Data;
using OpenCPA.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenCPA.Models
{
    /// <summary>
    /// Model for the index page of the site.
    /// </summary>
    public class IndexModel : CPAModel
    {
        public string PageName { get; set; } = "Home";
        public static int NumArchivedGlobal { get; set; } = -1;
        public static DateTime StaticUpdateTime { get; set; } = DateTime.MinValue;
        public int NumArchived { get; set; } = 0;

        public IndexModel()
        {
            //Has the global archived count been set? If so, when?
            if (StaticUpdateTime.AddMinutes(10) < DateTime.Now)
            {
                //Needs to be updated.
                NumArchivedGlobal = DBMan.Instance.Query<Resource>("SELECT * FROM Resources").Count + DBMan.Instance.Query<Track>("SELECT * FROM Tracks").Count;
                StaticUpdateTime = DateTime.Now;
            }

            //Set the local properties.
            NumArchived = NumArchivedGlobal;
        }
    }
}