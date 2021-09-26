using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OAuthSample.Demo
{
    public class OAuthHelper
    {
        public static OAuthHelper Instance = new OAuthHelper();
        public IDictionary<string, object> Bags { get; set; } = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }
}
