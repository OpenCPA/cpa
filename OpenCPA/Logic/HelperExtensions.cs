using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace OpenCPA
{
    public class ViewExtensions
    {
        /// <summary>
        /// De-entifies a unicode set.
        /// </summary>
        public static string EntityToUnicode(string html)
        {
            var regex = new Regex("(&(#)?[a-zA-Z0-9]{2,11};)");
            foreach (Match match in regex.Matches(html))
            {
                var unicode = HttpUtility.HtmlDecode(match.Value);
                html = html.Replace(match.Value, unicode);
            }
            return html;
        }
    }
}