namespace NbSites.Web.PermissionChecks
{
    //todo： refactor
    public class KnownPermissionIds
    {
        //预置数据
        public const string GuestOp = "GuestOp";
        public const string LoginOp = "LoginOp";
        public const string AdminOp = "AdminOp";
        public const string SuperOp = "SuperOp";

        //for demo purpose!
        public const string PortalEntry = "PortalEntry";
        public const string LeaderOp = "LeaderOp";
        public const string VodOp = "VodOp";
        public const string LiveOp = "LiveOp";
        public const string UnsureOp = "UnsureOp";
        public const string DemoOp = "DemoOp";
        public const string DemoOp2 = "DemoOp2";
        public const string DemoOp3 = "DemoOp3";
    }

    public class KnownActionIds
    {
        //for demo purpose!
        public const string UnsureActionId = "NbSites.Web.Controllers.SimpleController.Unsure";
        public const string UnsureActionId2 = "NbSites.Web.Controllers.SimpleController.Unsure2";
    }
}
