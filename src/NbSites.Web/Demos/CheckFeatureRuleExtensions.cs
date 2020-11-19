using System;
using System.Collections.Generic;
using System.Linq;

namespace NbSites.Web.Demos
{
    public class SimpleRuleExpression
    {
        private static readonly char[] Separators = { ',' };
        private static readonly string RuleAny = "*";
        private static readonly string RuleNone = "";

        public SimpleRuleExpression(string ruleExpress)
        {
            Rule = ruleExpress;
        }
        public string Rule { get; }
        
        public bool DenyAll()
        {
            return string.IsNullOrWhiteSpace(Rule);
        }

        public bool AllowAny()
        {
            return !string.IsNullOrWhiteSpace(Rule) && Rule.Contains(Any.Rule);
        }

        public bool MatchRule(params string[] checkItems)
        {
            var ruleValues = Split(Rule);
            return checkItems.Any(item => ruleValues.Contains(item, StringComparer.OrdinalIgnoreCase));
        }

        public static SimpleRuleExpression None => new SimpleRuleExpression(RuleNone);

        public static SimpleRuleExpression Any => new SimpleRuleExpression(RuleAny);

        public static SimpleRuleExpression Create(string values)
        {
            return new SimpleRuleExpression(values);
        }

        public static List<string> Split(string values)
        {
            var list = new List<string>();
            if (string.IsNullOrWhiteSpace(values))
            {
                return list;
            }
            list = values.Split(Separators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            return list;
        }
    }

    public static class CheckFeatureRuleExtensions
    {
        public static IDictionary<string, CheckFeatureRule> AddOrUpdate(this IDictionary<string, CheckFeatureRule> rules, CheckFeatureRule theRule)
        {
            rules.AddOrUpdate(theRule.CheckFeatureId, theRule.AllowedUsers, theRule.AllowedRoles);
            return rules;
        }
        public static IDictionary<string, CheckFeatureRule> AddOrUpdate(this IDictionary<string, CheckFeatureRule> rules, string id, string allowedUsers, string allowedRoles)
        {
            var theRule = rules.GetRule(id, true);
            theRule.AllowedUsers = allowedUsers;
            theRule.AllowedRoles = allowedRoles;
            return rules;
        }
        public static CheckFeatureRule GetRule(this IDictionary<string, CheckFeatureRule> rules, string id, bool autoCreate)
        {
            var exist = rules.TryGetValue(id, out var theRule);
            if (!exist)
            {
                if (!autoCreate)
                {
                    return null;
                }
                theRule = new CheckFeatureRule
                {
                    CheckFeatureId = id
                };
                rules[id] = theRule;
            }

            return theRule;
        }

        public static MessageResult CheckAllowed(this IDictionary<string, CheckFeatureRule> rules, string checkFeatureId, CheckFeatureContext ctx)
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
            if (SimpleRuleExpression.Create(checkFeatureRule.AllowedUsers).DenyAll() && SimpleRuleExpression.Create(checkFeatureRule.AllowedRoles).AllowAny())
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

        public static CheckFeatureRule SetNeedLogin(this CheckFeatureRule theRule)
        {
            theRule.AllowedUsers = SimpleRuleExpression.None.Rule;
            theRule.AllowedRoles = SimpleRuleExpression.Any.Rule;
            return theRule;
        }

        public static CheckFeatureRule SetNeedGuest(this CheckFeatureRule theRule)
        {
            theRule.AllowedUsers = SimpleRuleExpression.Any.Rule;
            theRule.AllowedRoles = SimpleRuleExpression.Any.Rule;
            return theRule;
        }

        public static CheckFeatureRule SetNeedUsersOrRoles(this CheckFeatureRule theRule, SimpleRuleExpression allowedUsers, SimpleRuleExpression allowedRoles)
        {
            theRule.AllowedUsers = allowedUsers.Rule;
            theRule.AllowedRoles = allowedRoles.Rule;
            return theRule;
        }

        private static bool CheckAllowedUsers(this CheckFeatureRule checkFeatureRule, CheckFeatureContext ctx)
        {
            if (SimpleRuleExpression.Create(checkFeatureRule.AllowedUsers).AllowAny())
            {
                return true;
            }

            if (SimpleRuleExpression.Create(checkFeatureRule.AllowedUsers).DenyAll())
            {
                return false;
            }

            return SimpleRuleExpression.Create(checkFeatureRule.AllowedUsers).MatchRule(ctx.User);
        }

        private static bool CheckMember(this CheckFeatureRule checkFeatureRule, CheckFeatureContext ctx)
        {
            if (SimpleRuleExpression.Create(checkFeatureRule.AllowedUsers).DenyAll() && SimpleRuleExpression.Create(checkFeatureRule.AllowedRoles).AllowAny())
            {
                //任何登录的用户都可以
                return !string.IsNullOrWhiteSpace(ctx.User);
            }
            return false;
        }

        private static bool CheckAllowedRoles(this CheckFeatureRule checkFeatureRule, CheckFeatureContext ctx)
        {
            if (SimpleRuleExpression.Create(checkFeatureRule.AllowedRoles).AllowAny())
            {
                return true;
            }

            if (SimpleRuleExpression.Create(checkFeatureRule.AllowedRoles).DenyAll())
            {
                return false;
            }

            return SimpleRuleExpression.Create(checkFeatureRule.AllowedRoles).MatchRule(ctx.Roles.ToArray());
        }
    }
}