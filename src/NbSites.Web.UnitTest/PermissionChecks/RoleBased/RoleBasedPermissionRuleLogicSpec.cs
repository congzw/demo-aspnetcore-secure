using System.Collections.Generic;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NbSites.Web.PermissionChecks.RoleBased
{
    [TestClass]
    public class RoleBasedPermissionRuleLogicSpec
    {
        [TestMethod]
        public void CheckRule_NoPermissionId_Should_Allowed()
        {
            var logic = new RoleBasedPermissionRuleLogic();
            var checkContext = MockHelper.CreatePermissionCheckContext(null, "");
            var guestRule = RoleBasedPermissionRule.CreateGuestRule("MockPermission");

            var result = logic.CheckRule(guestRule, checkContext);
            result.LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);
        }

        [TestMethod]
        public void CheckRule_NoMatchPermissionId_Should_NoCare()
        {
            var logic = new RoleBasedPermissionRuleLogic();
            var checkContext = MockHelper.CreatePermissionCheckContext("A, B", "");
            var guestRule = RoleBasedPermissionRule.CreateGuestRule("MockPermission");

            var result = logic.CheckRule(guestRule, checkContext);
            result.LogJson().Category.ShouldEqual(PermissionCheckResultCategory.NoCare);
        }
        
        [TestMethod]
        public void CheckRule_MatchPermissionId_Should_Ok()
        {
            var logic = new RoleBasedPermissionRuleLogic();
            var checkContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "");
            var guestRule = RoleBasedPermissionRule.CreateGuestRule("MockPermission");

            var result = logic.CheckRule(guestRule, checkContext);
            result.LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);
        }

        [TestMethod]
        public void CheckGuestRule_Anyone_Should_Ok()
        {
            var logic = new RoleBasedPermissionRuleLogic();
            var guestRule = RoleBasedPermissionRule.CreateGuestRule("MockPermission");

            var guestContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "");
            logic.CheckRule(guestRule, guestContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);

            var loginContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "bob");
            logic.CheckRule(guestRule, loginContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);
        }

        [TestMethod]
        public void CheckLoginRule_Should_Ok()
        {
            var logic = new RoleBasedPermissionRuleLogic();
            var loginRule = RoleBasedPermissionRule.CreateLoginRule("MockPermission");

            var guestContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "");
            logic.CheckRule(loginRule, guestContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Forbidden);

            var loginContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "bob");
            logic.CheckRule(loginRule, loginContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);
        }
        
        [TestMethod]
        public void CheckUserRule_Should_Ok()
        {
            var logic = new RoleBasedPermissionRuleLogic();
            var theRule = RoleBasedPermissionRule.Create("MockPermission", "bob, john", "");

            var bobContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "bob");
            logic.CheckRule(theRule, bobContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);

            var johnContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "john");
            logic.CheckRule(theRule, johnContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);

            var jackContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "jack");
            logic.CheckRule(theRule, jackContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Forbidden);
        }
        
        [TestMethod]
        public void CheckRoleRule_Should_Ok()
        {
            var logic = new RoleBasedPermissionRuleLogic();
            var theRule = RoleBasedPermissionRule.Create("MockPermission", "", "Admin, super");

            var bobContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "bob");
            logic.CheckRule(theRule, bobContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Forbidden);

            var johnContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "john", "Admin");
            logic.CheckRule(theRule, johnContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);

            var marryContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "marry", "Admin", "Super");
            logic.CheckRule(theRule, marryContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);

            var jackContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "jack", "FooRole");
            logic.CheckRule(theRule, jackContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Forbidden);
        }
        
        [TestMethod]
        public void CheckUserRoleRule_Should_Ok()
        {
            //result = allowedUsers || allowedRoles
            var logic = new RoleBasedPermissionRuleLogic();
            var theRule = RoleBasedPermissionRule.Create("MockPermission", "bob, john", "Admin, super");

            var bobContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "bob");
            logic.CheckRule(theRule, bobContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);

            var johnContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "john", "FooRole");
            logic.CheckRule(theRule, johnContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);
            
            var marryContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "marry", "Admin", "Super");
            logic.CheckRule(theRule, marryContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);
            
            var jackContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "jack", "FooRole");
            logic.CheckRule(theRule, jackContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Forbidden);
        }
        
        [TestMethod]
        public void CheckRulesAsOne_AllRules_Should_Satisfied()
        {
            //result = rule1 && rule2
            //result = allowedUsers || allowedRoles
            var logic = new RoleBasedPermissionRuleLogic();
            var theRule = RoleBasedPermissionRule.Create("MockPermission", "bob, john", "");
            var theRule2 = RoleBasedPermissionRule.Create("MockPermission", "", "Admin, super");
            var theRules = new List<RoleBasedPermissionRule>() { theRule, theRule2 };

            var bobContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "bob");
            logic.CheckRulesAsOne(theRules, bobContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Forbidden);

            var johnContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "john", "Admin");
            logic.CheckRulesAsOne(theRules, johnContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Allowed);

            var marryContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "marry", "Admin", "Super");
            logic.CheckRulesAsOne(theRules, marryContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Forbidden);

            var jackContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "jack", "FooRole");
            logic.CheckRulesAsOne(theRules, jackContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Forbidden);
            
            var tomContext = MockHelper.CreatePermissionCheckContext("A, B, MockPermission", "tom", "Leader");
            logic.CheckRulesAsOne(theRules, tomContext).LogJson().Category.ShouldEqual(PermissionCheckResultCategory.Forbidden);
        }
    }
}
