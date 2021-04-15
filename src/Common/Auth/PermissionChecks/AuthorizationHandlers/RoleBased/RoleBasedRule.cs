using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Auth.PermissionChecks.AuthorizationHandlers.RoleBased
{
    public class RoleBasedRule
    {
        public string PermissionId { get; set; }
        public string Rule { get; set; }

        public static RoleBasedRule Create(string permissionId, RoleBasedRuleExpression expression)
        {
            return new RoleBasedRule { PermissionId = permissionId, Rule = expression.ToRule() };
        }
    }

    public static class RoleBasedRuleExtensions
    {
        public static RoleBasedRuleExpression ToExpression(this RoleBasedRule rule)
        {
            return RoleBasedRuleExpression.ParseRule(rule.Rule);
        }
        
        public static List<RoleBasedRule> GetRoleBasedRules(this IDictionary<string, RoleBasedRule> rules, params string[] permissionIds)
        {
            var list = new List<RoleBasedRule>();
            if (permissionIds.Length == 0)
            {
                return list;
            }

            foreach (var permissionId in permissionIds)
            {
                if (rules.TryGetValue(permissionId, out var theRule))
                {
                    list.Add(theRule);
                }
            }
            return list;
        }

        public static PermissionCheckResult CheckRoleBasedRules(this PermissionCheckContext checkContext, params RoleBasedRule[] roleBasedRules)
        {
            var result = roleBasedRules.Select(checkContext.CheckRoleBasedRule).Combine();
            return result;
        }
        
        private static PermissionCheckResult CheckRoleBasedRule(this PermissionCheckContext checkContext, RoleBasedRule rule)
        {
            if (!checkContext.MatchPermissionId(rule.PermissionId))
            {
                return PermissionCheckResult.NotSure
                    .WithMessage($"规则中没有发现匹配的规则: {rule.PermissionId} not found in [{string.Join(',', checkContext.NeedCheckPermissionIds)}] ")
                    .WithData(rule.PermissionId);
            }
            
            var ruleExpression = rule.ToExpression();
            var userContext = checkContext.UserContext;

            var msg = $"userContext:[{userContext.User}],[{userContext.Roles.JoinToOneValue()}] ? rule:[{rule.Rule}]";
            if (ruleExpression.ValidateNeedGuest())
            {
                return PermissionCheckResult.Allowed.WithMessage("访客规则 => 满足 " + msg).WithData(rule.PermissionId);
            }

            var hasLogin = userContext.IsLogin();
            if (!hasLogin)
            {
                return PermissionCheckResult.Forbidden.WithMessage("需要登录 => 不满足 " + msg).WithData(rule.PermissionId);
            }

            if (ruleExpression.ValidateNeedLogin())
            {
                return PermissionCheckResult.Allowed.WithMessage("需要登录 => 满足 " + msg).WithData(rule.PermissionId);
            }

            if (ruleExpression.ValidateNeedAnyOfUsersOrRoles(userContext.User, userContext.Roles.JoinToOneValue()))
            {
                return PermissionCheckResult.Allowed.WithMessage("满足 " + msg).WithData(rule.PermissionId);
            }
            return PermissionCheckResult.Forbidden.WithMessage("不满足 " + msg).WithData(rule.PermissionId);
        }
    }
}
