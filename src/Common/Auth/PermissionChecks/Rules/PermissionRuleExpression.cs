using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Auth.PermissionChecks.Rules
{
    public class PermissionRuleExpression
    {
        private static readonly char[] Separators = { ',' };
        private static readonly string RuleAny = "*";
        private static readonly string RuleNone = "";

        public PermissionRuleExpression(string expressionValue)
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

        public bool DenyAll()
        {
            return string.IsNullOrWhiteSpace(Value);
        }

        public bool AllowAny()
        {
            return !string.IsNullOrWhiteSpace(Value) && Value.Contains(Any.Value);
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

        public static PermissionRuleExpression None => new PermissionRuleExpression(RuleNone);

        public static PermissionRuleExpression Any => new PermissionRuleExpression(RuleAny);

        public static PermissionRuleExpression Create(string values)
        {
            return new PermissionRuleExpression(values);
        }
    }
}