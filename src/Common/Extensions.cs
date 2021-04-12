using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public static partial class Extensions
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

        public static string JoinToOneValue(this IEnumerable<string> values, char separator = ',')
        {
            return string.Join(separator, values.Select(x => x));
        }

        public static IList<string> SplitToValues(this string itemsValue, char[] chars = null)
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

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, bool condition)
        {
            if (condition)
            {
                source = source.Where(predicate);
            }
            return source;
        }

        public static Task<T> AsTask<T>(this T value)
        {
            return FromResult(value);
        }

        public static Task<T> FromResult<T>(T result)
        {
#if NET40
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
#else
            return Task.FromResult(result);
#endif
        }

        public static T WithPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> memberExpression, TValue value)
        {
            if (memberExpression.Body is MemberExpression memberSelectorExpression)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    property.SetValue(target, value, null);
                }
            }

            return target;
        }

        #region hack for deplay

        public static Task AsDelay(this TimeSpan value)
        {
            return Delay(value);
        }

        public static Task Delay(TimeSpan span)
        {
#if NET40
            return Delay(span, CancellationToken.None);
#else
            return Task.Delay(span);
#endif
        }

        public static Task Delay(TimeSpan span, CancellationToken token)
        {
#if NET40
            return Delay((int)span.TotalMilliseconds, token);
#else
            return Task.Delay(span, token);
#endif
        }

        private static Task Delay(int milliseconds, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<object>();
            token.Register(() => tcs.TrySetCanceled());
            var timer = new Timer(_ => tcs.TrySetResult(null));
            timer.Change(milliseconds, -1);
            return tcs.Task;
        }

        #endregion
    }
}
