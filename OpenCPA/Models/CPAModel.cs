using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenCPA
{
    /// <summary>
    /// The interface that all page model classes should be based on.
    /// </summary>
    public abstract class CPAModel
    {
        /// <summary>
        /// The page name that will be described in the page title.
        /// </summary>
        string PageName { get; set; }
    }
}