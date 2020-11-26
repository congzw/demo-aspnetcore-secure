using System.Collections.Generic;
using Common;

// ReSharper disable once CheckNamespace
namespace NbSites.Web.PermissionChecks
{
    public class DynamicCheckDebugHelper
    {
        public List<MessageResult> CheckRuleResults { get; set; } = new List<MessageResult>();

        public static DynamicCheckDebugHelper Instance = new DynamicCheckDebugHelper();
    }
}
