using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
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
            Get("/", (req) => {
                return View["index", new IndexModel()];
            });
        }
    }
}