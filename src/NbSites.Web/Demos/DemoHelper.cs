using System.Collections.Generic;

namespace NbSites.Web.Demos
{
    public class DemoHelper
    {
        public List<MessageResult> CheckRuleResults { get; set; } = new List<MessageResult>();
        
        public static DemoHelper Instance = new DemoHelper();
    }
}
