﻿using System;

namespace NbSites.Web.PermissionChecks
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class PermissionCheckAttribute : Attribute
    {
        public string PermissionId { get; set; }
        public string OverridePermissionIds { get; set; }
    }
}