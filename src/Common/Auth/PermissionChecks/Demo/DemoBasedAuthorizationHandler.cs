//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;

//namespace Common.Auth.PermissionChecks.Demo
//{
//    public class DemoBasedAuthorizationHandler : AuthorizationHandler<PermissionCheckRequirement>
//    {
//        public static bool Allowed = false;

//        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionCheckRequirement requirement)
//        {
//            if (Allowed)
//            {
//                //根据演示flag放行
//                context.Succeed(requirement);
//                return Task.CompletedTask;
//            }
//            return Task.CompletedTask;
//        }
//    }
//}
