using System;

namespace NbSites.Web.Demos
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class DynamicCheckFeatureAttribute : Attribute
    {
        public string Id { get; set; }
        public string OverrideIds { get; set; }
    }
}