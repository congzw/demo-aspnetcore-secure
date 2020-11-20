using System;
using System.Collections.Generic;

namespace NbSites.Web.Demos
{
    public static class DynamicCheckRuleExtensions
    {
        public static IDictionary<string, DynamicCheckRule> AddOrUpdate(this IDictionary<string, DynamicCheckRule> rules, DynamicCheckRule theRule)
        {
            rules.AddOrUpdate(theRule.CheckFeatureId, theRule.AllowedUsers, theRule.AllowedRoles);
            return rules;
        }
        public static IDictionary<string, DynamicCheckRule> AddOrUpdate(this IDictionary<string, DynamicCheckRule> rules, string id, string allowedUsers, string allowedRoles)
        {
            var theRule = rules.GetRule(id, true);
            theRule.AllowedUsers = allowedUsers;
            theRule.AllowedRoles = allowedRoles;
            return rules;
        }
        public static DynamicCheckRule GetRule(this IDictionary<string, DynamicCheckRule> rules, string id, bool autoCreate)
        {
            var exist = rules.TryGetValue(id, out var theRule);
            if (!exist)
            {
                if (!autoCreate)
                {
                    return null;
                }
                theRule = new DynamicCheckRule
                {
                    CheckFeatureId = id
                };
                rules[id] = theRule;
            }

            return theRule;
        }

        public static MessageResult CheckAllowed(this IDictionary<string, DynamicCheckRule> rules, string checkFeatureId, CurrentUserContext ctx)
        {
            if (checkFeatureId == null) throw new ArgumentNullException(nameof(checkFeatureId));
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));

            //AllowedUsers,AllowedRoles
            //"*", "*"  => allowed anonymous
            //"", "*"  => allowed any Login user    
            //"bob", "*"  => allowed bob only   
            //"bob", "Admin"  => allowed bob, or Admin Role only    
            //"", "Admin"  => allowed Admin Role only
            //"", "Admin,Super"  => allowed Admin,Super only
            
            var result = new MessageResult();
            result.Data = checkFeatureId;

            var checkFeatureRule = rules.GetRule(checkFeatureId, false);
            if (checkFeatureRule == null)
            {
                //没有特别限制, 默认放行
                result.Success = true;
                result.Message = $"没有特别限制: {checkFeatureId}";
                return result;
            }

            var stepDesc = $"校验登录: {checkFeatureId} => is user '{ctx.User}' login?";


            //"", "*"  => allowed any Login user    
            if (DynamicCheckRuleExpression.Create(checkFeatureRule.AllowedUsers).DenyAll() && DynamicCheckRuleExpression.Create(checkFeatureRule.AllowedRoles).AllowAny())
            {
                //任何登录的用户都可以
                var isMember = !string.IsNullOrWhiteSpace(ctx.User);
                if (!isMember)
                {
                    //登录条件不满足，拦截!
                    result.Success = false;
                    result.Message = stepDesc;
                    return result;
                }
            }

            stepDesc = $"校验用户: is user '{ctx.User}' in '{checkFeatureRule.AllowedUsers}'?";
            var checkAllowedUsers = checkFeatureRule.CheckAllowedUsers(ctx);
            if (checkAllowedUsers)
            {
                //用户条件满足
                result.Success = true;
                result.Message = stepDesc;
                return result;
            }

            stepDesc = $"校验角色: {checkFeatureId} => is role '{string.Join(',', ctx.Roles)}' in '{checkFeatureRule.AllowedRoles}'";
            var checkAllowedRoles = checkFeatureRule.CheckAllowedRoles(ctx);
            if (checkAllowedRoles)
            {
                //角色条件满足，放行
                result.Success = true;
                result.Message = stepDesc;
                return result;
            }

            result.Message = $"权限不足: {stepDesc}";
            return result;
        }

        public static DynamicCheckRule SetNeedLogin(this DynamicCheckRule theRule)
        {
            theRule.AllowedUsers = DynamicCheckRuleExpression.None.Value;
            theRule.AllowedRoles = DynamicCheckRuleExpression.Any.Value;
            return theRule;
        }

        public static DynamicCheckRule SetNeedGuest(this DynamicCheckRule theRule)
        {
            theRule.AllowedUsers = DynamicCheckRuleExpression.Any.Value;
            theRule.AllowedRoles = DynamicCheckRuleExpression.Any.Value;
            return theRule;
        }

        public static DynamicCheckRule SetNeedUsersOrRoles(this DynamicCheckRule theRule, DynamicCheckRuleExpression allowedUsers, DynamicCheckRuleExpression allowedRoles)
        {
            theRule.AllowedUsers = allowedUsers.Value;
            theRule.AllowedRoles = allowedRoles.Value;
            return theRule;
        }

        private static bool CheckAllowedUsers(this DynamicCheckRule checkFeatureRule, CurrentUserContext ctx)
        {
            if (DynamicCheckRuleExpression.Create(checkFeatureRule.AllowedUsers).AllowAny())
            {
                return true;
            }

            if (DynamicCheckRuleExpression.Create(checkFeatureRule.AllowedUsers).DenyAll())
            {
                return false;
            }

            return DynamicCheckRuleExpression.Create(checkFeatureRule.AllowedUsers).MatchRule(ctx.User);
        }

        private static bool CheckMember(this DynamicCheckRule checkFeatureRule, CurrentUserContext ctx)
        {
            if (DynamicCheckRuleExpression.Create(checkFeatureRule.AllowedUsers).DenyAll() && DynamicCheckRuleExpression.Create(checkFeatureRule.AllowedRoles).AllowAny())
            {
                //任何登录的用户都可以
                return !string.IsNullOrWhiteSpace(ctx.User);
            }
            return false;
        }

        private static bool CheckAllowedRoles(this DynamicCheckRule checkFeatureRule, CurrentUserContext ctx)
        {
            if (DynamicCheckRuleExpression.Create(checkFeatureRule.AllowedRoles).AllowAny())
            {
                return true;
            }

            if (DynamicCheckRuleExpression.Create(checkFeatureRule.AllowedRoles).DenyAll())
            {
                return false;
            }

            return DynamicCheckRuleExpression.Create(checkFeatureRule.AllowedRoles).MatchRule(ctx.Roles.ToArray());
        }
    }
}