using System;
using System.Collections.Generic;

namespace NbSites.Web.Demos
{
    public static class MyDictionaryExtensions
    {
        public static IDictionary<TKey, T> AddOrUpdate<TKey, T>(this IDictionary<TKey, T> dictionary, T theRule, Func<T, TKey> keySelector)
        {
            var theKey = keySelector(theRule);
            dictionary[theKey] = theRule;
            return dictionary;
        }

        public static T GetRule<TKey, T>(this IDictionary<TKey, T> dictionary, TKey key)
        {
            dictionary.TryGetValue(key, out var theRule);
            return theRule;
        }
    }
}