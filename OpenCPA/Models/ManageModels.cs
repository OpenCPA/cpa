using System.Reflection;

namespace OpenCPA.Models
{
    /// <summary>
    /// Represents dynamic data related to the management pages.
    /// </summary>
    public class ManageModel : CPAModel
    {
        public string PageName { get; set; } = "Manage";
        public string Version
        {
            get
            {
                return "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }
}