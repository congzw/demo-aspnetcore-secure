using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace NbSites.Web.PermissionChecks.ConfigBased
{
    public class ConfigBasedHandler : AuthorizationHandler<PermissionCheckRequirement>
    {
        private readonly IOptionsSnapshot<PermissionCheckOptions> _settingSnapshot;

        public ConfigBasedHandler(IOptionsSnapshot<PermissionCheckOptions> settingSnapshot)
        {
            _settingSnapshot = settingSnapshot;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionCheckRequirement requirement)
        {
            if (_settingSnapshot.Value.Naked)
            {
                //配置为裸奔模式，主体权限控制直接放行
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }

    }
}
