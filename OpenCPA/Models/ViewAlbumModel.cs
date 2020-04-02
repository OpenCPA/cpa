using OpenCPA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenCPA.Models
{
    /// <summary>
    /// Model for viewing an album.
    /// </summary>
    public class ViewAlbumModel : CPAModel
    {
        public string PageName { get; set; }
        public Album Album;
        public Artist Artist;
        public List<Track> Tracks;

        public ViewAlbumModel(Album album, Artist artist, List<Track> tracks)
        {
            Album = album;
            Artist = artist;
            Tracks = tracks;
            PageName = album.EnglishName;
        }
    }
}