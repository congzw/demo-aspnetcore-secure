using System;
using System.Collections.Generic;
using Common;

// ReSharper disable once CheckNamespace
namespace NbSites.Web.PermissionChecks
{
    public interface IPermissionCheckDebugHelper
    {
        Func<bool> Enabled { get; set; }
        void AppendPermissionCheckResults(params PermissionCheckResult[] results);
        IList<PermissionCheckResult> GetPermissionCheckResults(bool autoReset = true);
    }

    public class PermissionCheckDebugHelper : IPermissionCheckDebugHelper
    {
        private IList<PermissionCheckResult> Results { get; set; } = new List<PermissionCheckResult>();
        
        public Func<bool> Enabled { get; set; } = () => false;

        public void AppendPermissionCheckResults(params PermissionCheckResult[] results)
        {
            if (!Enabled())
            {
                return;
            }

            if (results == null || results.Length == 0)
            {
                return;
            }

            foreach (var result in results)
            {
                Results.Add(result);
            }
        }

        public IList<PermissionCheckResult> GetPermissionCheckResults(bool autoReset = true)
        {
            if (!Enabled())
            {
                return Results;
            }

            var copyResults = new List<PermissionCheckResult>();
            foreach (var result in Results)
            {
                copyResults.Add(result);
            }

            if (autoReset)
            {
                Results.Clear();
            }
            return copyResults;
        }
    }
}
