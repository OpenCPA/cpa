using OpenCPA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenCPA.Models
{
    /// <summary>
    /// The model for the "Albums" page.
    /// </summary>
    public class ArtistsModel : CPAModel
    {
        public string PageName { get; set; } = "Albums";
        public List<Artist> Artists { get; set; }

        public ArtistsModel(List<Artist> artists)
        {
            Artists = artists;
        }
    }
}