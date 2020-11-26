using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace NbSites.Web.PermissionChecks
{
    public static class StringExtensions
    {
        public static bool MyEquals(this string value, string value2, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var valueFix = string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                valueFix = value.Trim();
            }

            var value2Fix = string.Empty;
            if (!string.IsNullOrWhiteSpace(value2))
            {
                value2Fix = value2.Trim();
            }

            return valueFix.Equals(value2Fix, comparison);
        }

        public static bool MyContains(this IEnumerable<string> values, string toCheck, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            foreach (var value in values)
            {
                if (value.MyEquals(toCheck, comparison))
                {
                    return true;
                }
            }
            return false;
        }

        public static string MyFind(this IEnumerable<string> values, string toCheck, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            foreach (var value in values)
            {
                if (value.MyEquals(toCheck, comparison))
                {
                    return value;
                }
            }
            return null;
        }

        public static IList<string> MySplit(this string itemsValue, char[] chars = null)
        {
            chars ??= new[] { ',' };
            var items = new List<string>();
            if (string.IsNullOrWhiteSpace(itemsValue))
            {
                return items;
            }

            var strings = itemsValue.Split(chars, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in strings)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    items.Add(item);
                }
            }

            return items;
        }

        public static string MyJoin(this IEnumerable<string> items, char separator = ',', bool autoTrim = true)
        {
            if (items == null)
            {
                return string.Empty;
            }

            var list = items.ToList();
            if (list.Count == 0)
            {
                return string.Empty;
            }

            if (!autoTrim)
            {
                return string.Join(separator, list);
            }
            var trimItems = list.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();
            return string.Join(separator, trimItems);
        }
    }
}
