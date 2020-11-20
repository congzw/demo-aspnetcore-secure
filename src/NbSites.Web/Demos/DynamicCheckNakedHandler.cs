using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace NbSites.Web.Demos
{
    public class DynamicCheckNakedHandler : AuthorizationHandler<DynamicCheckRequirement>
    {
        private readonly IOptionsSnapshot<DynamicCheckOptions> _settingSnapshot;

        public DynamicCheckNakedHandler(IOptionsSnapshot<DynamicCheckOptions> settingSnapshot)
        {
            _settingSnapshot = settingSnapshot;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicCheckRequirement requirement)
        {
            if (_settingSnapshot.Value.Naked)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

    }
}
