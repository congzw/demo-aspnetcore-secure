using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NbSites.Web.PermissionChecks
{
    [TestClass]
    public class PermissionCheckResultSpec
    {
        [TestMethod]
        public void Combine_NullOrEmpty_Should_Allowed()
        {
            PermissionCheckResult.Combine(null).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);
            PermissionCheckResult.Combine().LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);
        }

        [TestMethod]
        public void Combine_AllNotSure_Should_NotSure()
        {
            PermissionCheckResult.Combine(
                PermissionCheckResult.NotSure, 
                PermissionCheckResult.NotSure)
                .LogJson().Category.ShouldEqual(PermissionCheckResultCategory.NotSure);
        }

        [TestMethod]
        public void Combine_HasAllowed_Should_Allowed()
        {
            PermissionCheckResult.Combine(
                    PermissionCheckResult.NotSure, 
                    PermissionCheckResult.Allowed)
                .LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);
        }

        [TestMethod]
        public void Combine_HasForbidden_Should_Forbidden()
        {
            PermissionCheckResult.Combine(
                PermissionCheckResult.NotSure,
                PermissionCheckResult.Allowed,
                PermissionCheckResult.Forbidden)
                .LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Forbidden);
        }
    }
}
