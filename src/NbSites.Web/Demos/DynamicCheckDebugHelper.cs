using System.Collections.Generic;

namespace NbSites.Web.Demos
{
    public class DynamicCheckDebugHelper
    {
        public List<MessageResult> CheckRuleResults { get; set; } = new List<MessageResult>();

        public static DynamicCheckDebugHelper Instance = new DynamicCheckDebugHelper();
    }
}
