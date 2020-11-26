using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Common
{
    public class TempHelper
    {
        public string Id { get; set; }

        public IDictionary<string, object> Items { get; set; }

        public TempHelper(string helperId, IDictionary<string, object> items)
        {
            Id = helperId;
            Items = items;
        }

        public void Reset()
        {
            Items = new Dictionary<string, object>();
        }

        public object Get(string key, object defaultValue)
        {
            if (Items.ContainsKey(key))
            {
                return Items[key];
            }
            return defaultValue;
        }

        public void Set(string key, object value)
        {
            if (!IsEnabled())
            {
                return;
            }
            Items[key] = value;
        }

        public void EnabledRefresh(int? enabledNextMinutes = null)
        {
            _enabledTo = DateTimeOffset.Now.AddMinutes(enabledNextMinutes ?? EnableForNextMinutes);
        }

        public bool IsEnabled()
        {
            if (_enabledTo == null)
            {
                return false;
            }
            return _enabledTo.Value > DateTimeOffset.Now;
        }

        private DateTimeOffset? _enabledTo;

        #region factory

        public static IDictionary<string, TempHelper> Helpers = new Dictionary<string, TempHelper>(StringComparer.OrdinalIgnoreCase);
        public static TempHelper Get(string helperKey, bool createIfNotExist)
        {
            if (Helpers.TryGetValue(helperKey, out var helper))
            {
                return helper;
            }

            if (!createIfNotExist)
            {
                return null;
            }

            helper = new TempHelper(helperKey, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));
            Helpers[helperKey] = helper;
            return helper;
        }
        public static void ResetAll()
        {
            Helpers.Clear();
        }

        public const string KeyDefault = "Default";
        public static TempHelper Default => Get(KeyDefault, true);
        public static int EnableForNextMinutes = 10;

        #endregion
    }
    
    public static class TempHelperExtForDebugInfo
    {
        private static string KeyDebugInfo = "DebugInfo";
        public static IList<string> ShowDebugInfos(this TempHelper itemsHelper, bool autoReset = true)
        {
            itemsHelper.EnabledRefresh();
            var debugInfos = GetDebugInfos(itemsHelper);
            if (autoReset)
            {
                itemsHelper.Reset();
            }
            return debugInfos;
        }
        public static void AppendDebugInfos(this TempHelper itemsHelper, object msg)
        {
            if (msg == null)
            {
                return;
            }

            if (!itemsHelper.IsEnabled())
            {
                return;
            }

            var infos = GetDebugInfos(itemsHelper);
            infos.Add(msg.ToString());
        }
        public static void ClearDebugInfos(this TempHelper itemsHelper)
        {
            var infos = GetDebugInfos(itemsHelper);
            infos.Clear();
        }
        private static IList<string> GetDebugInfos(TempHelper itemsHelper)
        {
            var infos = itemsHelper.Get(KeyDebugInfo, null) as IList<string>;
            if (infos == null)
            {
                infos = new List<string>();
                itemsHelper.Set(KeyDebugInfo, infos);
            }
            return infos;
        }
    }
}
