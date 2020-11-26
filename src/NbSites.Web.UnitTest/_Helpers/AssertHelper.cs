using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable once CheckNamespace
namespace Common
{
    public class AssertHelper
    {
        public static readonly string OkMark = "✔";
        public static readonly string KoMark = "✘";

        public static void ShouldThrows<T>(Action action) where T : Exception
        {
            T expectedEx = null;
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    var baseException = ex.GetBaseException();
                    WriteLineOk("抛出了异常:" + baseException.Message);
                    expectedEx = baseException as T;
                }
                else
                {
                    WriteLineOk("抛出了异常:" + ex.Message);
                    expectedEx = ex as T;
                }
            }
            Assert.IsNotNull(expectedEx, PrefixKo("没有发现应该抛出的异常: " + typeof(T).Name));
        }
        public static string PrefixOk(string value)
        {
            return OkMark + " " + value;
        }
        public static string PrefixKo(string value)
        {
            return KoMark + " " + value;
        }
        public static void WriteLineOk(string message)
        {
            WriteLine(PrefixOk(message));
        }
        public static void WriteLineKo(string message)
        {
            WriteLine(PrefixKo(message));
        }
        public static void WriteLine(string message)
        {
            WriteLine(message, null);
        }
        public static void WriteLineForShouldBeTrue(bool result)
        {
            var message = "Should be true";
            if (result)
            {
                WriteLineOk(message);
            }
            else
            {
                WriteLineKo(message);
            }
        }
        public static void WriteLineForShouldBeFalse(bool result)
        {
            var message = "Should be false";
            if (!result)
            {
                WriteLineOk(message);
            }
            else
            {
                WriteLineKo(message);
            }
        }
        public static void WriteLineForShouldBeNull(object result)
        {
            var message = "Should be null";
            if (result == null)
            {
                WriteLineOk(message);
            }
            else
            {
                WriteLineKo(message + ": " + result);
            }
        }
        public static void WriteLineForShouldBeNotNull(object result)
        {
            var message = "Should be not null";
            if (result != null)
            {
                WriteLineOk(message);
            }
            else
            {
                WriteLineKo(message + ": null");
            }
        }

        public static void WriteLine(string message, string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                Trace.WriteLine(message);
            }
            else
            {
                Trace.WriteLine(prefix + message);
            }
        }
    }
}
