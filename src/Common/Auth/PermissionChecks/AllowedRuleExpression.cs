using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Auth.PermissionChecks
{
    public class AllowedRuleExpression
    {
        private static readonly char[] Separators = { ',' };
        private static readonly string RuleAny = "*";
        private static readonly string RuleExist = "?";

        public AllowedRuleExpression(string expressionValue)
        {
            Value = expressionValue;
        }

        public string Value { get; }

        public static List<string> Split(string theValue)
        {
            var items = new List<string>();
            if (string.IsNullOrWhiteSpace(theValue))
            {
                return items;
            }
            items = theValue.Split(Separators, StringSplitOptions.RemoveEmptyEntries).ToList();
            return items;
        }

        public bool AllowExist()
        {
            return Value == RuleExist;
        }

        public bool AllowAny()
        {
            return Value.Contains(RuleAny);
        }

        public bool AllowAnyOf(params string[] ofValues)
        {
            if (AllowAny())
            {
                return true;
            }

            if (ofValues == null || ofValues.Length == 0)
            {
                return false;
            }

            var ruleValues = Value.SplitToValues(Separators);
            if (ruleValues == null || ruleValues.Count == 0)
            {
                return false;
            }

            return ofValues.Any(checkValue => ruleValues.MyContains(checkValue));
        }

        public bool AllowAnyOfValue(string ofValue)
        {
            var theItems = Split(ofValue).ToArray();
            return AllowAnyOf(theItems);
        }

        public static AllowedRuleExpression Empty => new AllowedRuleExpression(RuleExist);

        public static AllowedRuleExpression Any => new AllowedRuleExpression(RuleAny);

        public static AllowedRuleExpression Create(string values)
        {
            return new AllowedRuleExpression(values);
        }
    }
}