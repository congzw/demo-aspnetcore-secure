using System;
using System.Collections.Generic;
using System.Linq;

namespace NbSites.Web.Demos
{
    public class DynamicCheckRuleExpression
    {
        private static readonly char[] Separators = { ',' };
        private static readonly string RuleAny = "*";
        private static readonly string RuleNone = "";

        public DynamicCheckRuleExpression(string ruleExpression)
        {
            Value = ruleExpression;
        }
        public string Value { get; }
        
        public bool DenyAll()
        {
            return string.IsNullOrWhiteSpace(Value);
        }

        public bool AllowAny()
        {
            return !string.IsNullOrWhiteSpace(Value) && Value.Contains(Any.Value);
        }

        public bool MatchRule(params string[] checkItems)
        {
            var ruleValues = Split(Value);
            return checkItems.Any(item => ruleValues.Contains(item, StringComparer.OrdinalIgnoreCase));
        }

        public static DynamicCheckRuleExpression None => new DynamicCheckRuleExpression(RuleNone);

        public static DynamicCheckRuleExpression Any => new DynamicCheckRuleExpression(RuleAny);

        public static DynamicCheckRuleExpression Create(string values)
        {
            return new DynamicCheckRuleExpression(values);
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
}