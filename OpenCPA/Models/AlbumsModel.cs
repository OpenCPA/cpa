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
    public class AlbumsModel : CPAModel
    {
        public string PageName { get; set; } = "Albums";
        public List<Album> Albums { get; set; }

        public AlbumsModel(List<Album> albums)
        {
            Albums = albums;
        }
    }
}