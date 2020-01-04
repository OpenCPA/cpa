using Nancy.ViewEngines.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenCPA
{
    /// <summary>
    /// Razor config project-wide.
    /// </summary>
    public class RazorConfig : IRazorConfiguration
    {
        public IEnumerable<string> GetAssemblyNames()
        {
            yield return "OpenCPA.Models";
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            yield return "OpenCPA.Models";
        }

        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }
    }
}