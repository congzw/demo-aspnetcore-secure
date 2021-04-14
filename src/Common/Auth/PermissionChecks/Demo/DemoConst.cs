using Common.Auth.PermissionChecks.AuthorizationHandlers.RoleBased;
using Common.Auth.PermissionChecks.ControlPoints;

namespace Common.Auth.PermissionChecks.Demo
{
    public class DemoConst
    {
        public const string AppNamespace = "JwtAndCookie.";
        public const string PageController = "Controllers.DemoController.";
        public const string ApiController = "Controllers.DemoApiController.";

        public class PermissionIds
        {
            //演示用的Permission列表
            public const string GuestOp = "GuestOp";
            public const string LoginOp = "LoginOp";
            public const string AdminOp = "AdminOp";
            public const string SuperOp = "SuperOp";
            public const string SmartOp = "SmartOp";
            public const string DemoBasedOp = "DemoBasedOp";
        }
        
        public class PageIds
        {
            public const string GuestOp = AppNamespace + PageController + "GuestOp";
            public const string LoginOp = AppNamespace + PageController + "LoginOp";
            public const string AdminOp = AppNamespace + PageController + "AdminOp";
            public const string SuperOp = AppNamespace + PageController + "SuperOp";
            public const string SmartOp = AppNamespace + PageController + "SmartOp";
        }

        public class ApiIds
        {
            public const string GuestOp = AppNamespace + ApiController + "GuestOp";
            public const string LoginOp = AppNamespace + ApiController + "LoginOp";
            public const string AdminOp = AppNamespace + ApiController + "AdminOp";
            public const string SuperOp = AppNamespace + ApiController + "SuperOp";
            public const string SmartOp = AppNamespace + ApiController + "SmartOp";
        }

        public class Permissions
        {
            public static Permission NeedGuest = Permission.Create(PermissionIds.GuestOp, "游客可用");
            public static Permission NeedLogin = Permission.Create(PermissionIds.LoginOp, "登录可用");
            public static Permission NeedAdmin = Permission.Create(PermissionIds.AdminOp, "Admin可用");
            public static Permission NeedSuper = Permission.Create(PermissionIds.SuperOp, "Super可用");
            public static Permission Smart = Permission.Create(PermissionIds.SmartOp, "动态配置");
        }

        public class EndPoints
        {
            public static EndPoint PageGuestOp = EndPoint.Create(PageIds.GuestOp, "页面：游客可用");
            public static EndPoint PageLoginOp = EndPoint.Create(PageIds.GuestOp, "页面：登录可用");
            public static EndPoint PageAdminOp = EndPoint.Create(PageIds.GuestOp, "页面：Admin可用");
            public static EndPoint PageSuperOp = EndPoint.Create(PageIds.GuestOp, "页面：Super可用");
            public static EndPoint PageSmartOp = EndPoint.Create(PageIds.SmartOp, "页面：动态配置");
            
            public static EndPoint ApiGuestOp = EndPoint.Create(ApiIds.GuestOp, "Api：游客可用");
            public static EndPoint ApiLoginOp = EndPoint.Create(ApiIds.GuestOp, "Api：登录可用");
            public static EndPoint ApiAdminOp = EndPoint.Create(ApiIds.GuestOp, "Api：Admin可用");
            public static EndPoint ApiSuperOp = EndPoint.Create(ApiIds.GuestOp, "Api：Super可用");
            public static EndPoint ApiSmartOp = EndPoint.Create(ApiIds.SmartOp, "Api：动态配置");
        }

        public static void Init(ControlPointRegistry registry)
        {
            registry.SetPermission(Permissions.NeedGuest);
            registry.SetPermission(Permissions.NeedLogin);
            registry.SetPermission(Permissions.NeedAdmin);
            registry.SetPermission(Permissions.NeedSuper);
            registry.SetPermission(Permissions.Smart);

            registry.SetEndPoint(EndPoints.PageGuestOp);
            registry.SetEndPoint(EndPoints.PageLoginOp);
            registry.SetEndPoint(EndPoints.PageAdminOp);
            registry.SetEndPoint(EndPoints.PageSuperOp);
            registry.SetEndPoint(EndPoints.PageSmartOp);
            
            registry.SetEndPoint(EndPoints.ApiGuestOp);
            registry.SetEndPoint(EndPoints.ApiLoginOp);
            registry.SetEndPoint(EndPoints.ApiAdminOp);
            registry.SetEndPoint(EndPoints.ApiSuperOp);
            registry.SetEndPoint(EndPoints.ApiSmartOp);

            registry.SetEndPointPermission(EndPointPermission.Create(PageIds.SmartOp, PermissionIds.SmartOp));
            registry.SetEndPointPermission(EndPointPermission.Create(ApiIds.SmartOp, PermissionIds.SmartOp));

            registry.SetPermissionRule(PermissionRule.Create(PageIds.SmartOp, ))
            RoleBasedPermissionRule.Create(PageIds.SmartOp, null, null);
        }
    }

}
