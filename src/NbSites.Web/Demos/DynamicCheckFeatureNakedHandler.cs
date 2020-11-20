using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace NbSites.Web.Demos
{
    public class DynamicCheckFeatureNakedHandler : AuthorizationHandler<DynamicCheckFeatureRequirement>
    {
        private readonly IOptionsSnapshot<DynamicCheckPolicyOptions> _settingSnapshot;

        public DynamicCheckFeatureNakedHandler(IOptionsSnapshot<DynamicCheckPolicyOptions> settingSnapshot)
        {
            _settingSnapshot = settingSnapshot;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicCheckFeatureRequirement requirement)
        {
            if (_settingSnapshot.Value.Naked)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

    }
}
