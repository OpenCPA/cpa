using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Nancy;
using OpenCPA.Data;
using OpenCPA.Database;
using OpenCPA.Models;

namespace OpenCPA
{
    /// <summary>
    /// The main module of OpenCPA. Handles the homepage.
    /// </summary>
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            //Homepage.
            Get("/", (req) => {

                return View["index", new IndexModel()
                {
                    Alert = DBMan.Settings.MainPageAlert
                }];
            });

            //Albums page.
            Get("/albums", (req) =>
            {
                //If there's a search term use that instead.
                if (Request.Query.search != null && Regex.IsMatch(Request.Query.search.ToString(), "^[A-Za-z0-9 ]{1,50}$"))
                {
                    List<Album> searched = DBMan.Instance.Query<Album>("SELECT * FROM Albums WHERE (lower(EnglishName) LIKE '%" + Request.Query.search.ToString() +"%')");
                    return View["albums", new AlbumsModel(searched)];
                }

                //Get the letter to sort by.
                string sortLetter = "a";
                if (Request.Query.sort != null && Regex.IsMatch(Request.Query.sort.Value, "^[a-zA-Z]$"))
                {
                    sortLetter = Request.Query.sort.ToString();
                }

                //Get the albums starting with the given letter.
                var albums = DBMan.Instance.Query<Album>("SELECT * FROM Albums WHERE (lower(EnglishName) LIKE '" + sortLetter.ToLower() + "%')");
                return View["albums", new AlbumsModel(albums)];
            });

            //Artists page.
            Get("/artists", (req) =>
            {
                //If there's a search term use that instead.
                if (Request.Query.search != null && Regex.IsMatch(Request.Query.search.ToString(), "^[A-Za-z0-9 ]{1,50}$"))
                {
                    List<Artist> searched = DBMan.Instance.Query<Artist>("SELECT * FROM Artists WHERE (lower(EnglishName) LIKE '%" + Request.Query.search.ToString() + "%')");
                    return View["artists", new ArtistsModel(searched)];
                }

                //Get the letter to sort by.
                string sortLetter = "a";
                if (Request.Query.sort != null && Regex.IsMatch(Request.Query.sort.Value, "^[a-zA-Z]$"))
                {
                    sortLetter = Request.Query.sort.ToString();
                }

                //Get the albums starting with the given letter.
                var artists = DBMan.Instance.Query<Artist>("SELECT * FROM Artists WHERE (lower(EnglishName) LIKE '" + sortLetter.ToLower() + "%')");
                return View["artists", new ArtistsModel(artists)];
            });
        }
    }
}