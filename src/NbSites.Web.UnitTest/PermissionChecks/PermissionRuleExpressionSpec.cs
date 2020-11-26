using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NbSites.Web.PermissionChecks
{
    [TestClass]
    public class PermissionRuleExpressionSpec
    {
        [TestMethod]
        public void NoneRule_Should_AllowNone()
        {
            var noneRule = PermissionRuleExpression.None;
            noneRule.LogJson().DenyAll().ShouldTrue();
            
            noneRule.AllowAnyOf("A", "B").ShouldFalse();
            noneRule.AllowAnyOf("c").ShouldFalse();
            noneRule.AllowAnyOf("B").ShouldFalse();
            noneRule.AllowAnyOf("B", "C").ShouldFalse();
            noneRule.AllowAnyOf("A").ShouldFalse();
            noneRule.AllowAnyOf("A", "C").ShouldFalse();
            noneRule.AllowAnyOf("C").ShouldFalse();
            noneRule.AllowAnyOf("").ShouldFalse();
        }

        [TestMethod]
        public void AnyRule_Should_AllowAny()
        {
            var anyRule = PermissionRuleExpression.Any;
            anyRule.LogJson().AllowAny().ShouldTrue();

            anyRule.AllowAnyOf("A", "B").ShouldTrue();
            anyRule.AllowAnyOf("c").ShouldTrue();
            anyRule.AllowAnyOf("B").ShouldTrue();
            anyRule.AllowAnyOf("B", "C").ShouldTrue();
            anyRule.AllowAnyOf("A").ShouldTrue();
            anyRule.AllowAnyOf("A", "C").ShouldTrue();
            anyRule.AllowAnyOf("C").ShouldTrue();
        }
        
        [TestMethod]
        public void SomeRule_Should_AllowExist()
        {
            var someRule = PermissionRuleExpression.Create("A, C");
            someRule.LogJson().AllowAnyOf("A", "B").ShouldTrue();
            someRule.AllowAnyOf("B", "C").ShouldTrue();
            someRule.AllowAnyOf("c").ShouldTrue();
            someRule.AllowAnyOf("B").ShouldFalse();
            
            var anyRule = PermissionRuleExpression.Create("*, A");
            anyRule.LogJson().AllowAnyOf("A").ShouldTrue();
            anyRule.AllowAnyOf("A", "B").ShouldTrue();
            anyRule.AllowAnyOf("B").ShouldTrue();
        }
    }
}
