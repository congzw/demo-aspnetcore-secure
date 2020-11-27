﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace NbSites.Web.PermissionChecks.ConfigBased
{
    public class ConfigBasedHandler : AuthorizationHandler<PermissionCheckRequirement>
    {
        private readonly IOptionsSnapshot<DynamicCheckOptions> _settingSnapshot;

        public ConfigBasedHandler(IOptionsSnapshot<DynamicCheckOptions> settingSnapshot)
        {
            _settingSnapshot = settingSnapshot;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionCheckRequirement requirement)
        {
            if (_settingSnapshot.Value.Naked)
            {
                //配置为裸奔模式，直接放行
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }

    }
}