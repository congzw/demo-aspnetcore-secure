//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Reflection;
//using Common;
//using Microsoft.Extensions.Logging;

//// ReSharper disable once CheckNamespace
//namespace NbSites.Web.PermissionChecks
//{
//    public interface IPermissionCheckLogHelper
//    {
//        void Log(LogLevel logLevel, object msg);
//    }

//    public class PermissionCheckLogHelper : IPermissionCheckLogHelper
//    {
//        private readonly ILogger<PermissionCheckLogHelper> _logger;

//        public PermissionCheckLogHelper(ILogger<PermissionCheckLogHelper> logger)
//        {
//            _logger = logger;
//        }

//        public void Log(LogLevel logLevel, object msg)
//        {
//            TempLogHelper.Log(logLevel, "PermissionCheck >>>> " + msg);
//            _logger.Log(logLevel, "PermissionCheck >>>> " + msg);
//        }

//        //for extensions
//        private static readonly TempPermissionCheckLogHelper TempLogHelper = TempPermissionCheckLogHelper.Default;
//        public static Func<IPermissionCheckLogHelper> Resolve = () => new Lazy<IPermissionCheckLogHelper>(TempPermissionCheckLogHelper.Default).Value;
//    }

//    public static class PermissionCheckLogHelperExtensions
//    {
//        public static void LogInformation(this IPermissionCheckLogHelper helper, object msg)
//        {
//            helper.Log(LogLevel.Information, msg);
//        }
//    }

//    public class TempPermissionCheckLogHelper : IPermissionCheckLogHelper
//    {
//        private TempPermissionCheckLogHelper()
//        {
//        }

//        public void Log(LogLevel logLevel, object msg)
//        {
//            string preFix = null;
//            var invokeMethod = TryGetWantedInvokeMethod(new StackTrace(), 1);
//            if (invokeMethod != null)
//            {
//                var typeName = invokeMethod.DeclaringType?.Name;
//                preFix += $"{typeName}.{invokeMethod.Name}() => ";
//            }
//            //todo: msg to json
//            var log = preFix + msg;
//            Trace.WriteLine(log);
//            TempHelper.Default.AppendDebugInfos(log);
//        }

//        private MethodBase TryGetWantedInvokeMethod(StackTrace stackTrace, int startIndex, int maxDeep = 10)
//        {
//            if (startIndex > maxDeep)
//            {
//                return null;
//            }

//            var stackFrame = stackTrace?.GetFrame(startIndex);
//            if (stackFrame == null)
//            {
//                return null;
//            }
//            var invokeMethod = stackFrame.GetMethod();
//            if (invokeMethod == null)
//            {
//                return null;
//            }

//            var reflectedType = invokeMethod.ReflectedType;
//            if (reflectedType == null)
//            {
//                return null;
//            }

//            if (IgnoreTraceClassNames.Any(x => x.Equals(reflectedType.Name, StringComparison.OrdinalIgnoreCase)))
//            {
//                return TryGetWantedInvokeMethod(stackTrace, startIndex + 1);
//            }

//            return invokeMethod;
//        }


//        public static readonly TempPermissionCheckLogHelper Default = new TempPermissionCheckLogHelper();

//        public static IList<string> IgnoreTraceClassNames = new List<string>() { "TempPermissionCheckLogHelper", "PermissionCheckLogHelperExtensions", "PermissionCheckLogHelper" };
//    }
//}
