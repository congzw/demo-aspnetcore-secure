﻿namespace NbSites.Web.PermissionChecks
{
    //todo： refactor
    public class KnownPermissionIds
    {
        //for demo purpose!
        public const string PortalEntry = "PortalEntry";
        public const string AdminOp = "AdminOp";
        public const string SuperOp = "SuperOp";
        public const string LoginOp = "LoginOp";
        public const string GuestOp = "GuestOp";
        public const string LeaderOp = "LeaderOp";
        public const string VodOp = "VodOp";
        public const string LiveOp = "LiveOp";
        public const string DemoOp = "DemoOp";
        public const string UnsureActionA = "UnsureActionA";
    }

    public class KnownActionIds
    {
        public const string UnsureActionId = "NbSites.Web.Controllers.SimpleController.Unsure";
        public const string UnsureActionId2 = "NbSites.Web.Controllers.SimpleController.Unsure2";
        public const string SpecialAction = "NbSites.Web.Controllers.SimpleController.SpecialAction";
    }
}
