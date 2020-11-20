using System.Collections.Generic;

namespace NbSites.Web.Demos
{
    public class DynamicCheckContext
    {
        public string User { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}