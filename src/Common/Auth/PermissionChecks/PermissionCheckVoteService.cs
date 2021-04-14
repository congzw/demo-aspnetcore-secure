using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Auth.PermissionChecks.ControlPoints;

namespace Common.Auth.PermissionChecks
{
    public interface IPermissionCheckVoteService
    {
        Task<PermissionCheckResult> CheckAsync(PermissionCheckContext checkContext);
    }

    public class PermissionCheckVoteService : IPermissionCheckVoteService
    {
        private readonly CurrentUserContext _userContext;
        private readonly ControlPointRegistry _registry;
        private readonly IList<IPermissionCheckLogicProvider> _providers;
        public PermissionCheckVoteService(IEnumerable<IPermissionCheckLogicProvider> providers, CurrentUserContext userContext, ControlPointRegistry registry)
        {
            _userContext = userContext;
            _registry = registry;
            _providers = providers.OrderBy(x => x.Order).ToList();
        }


        public async Task<PermissionCheckResult> CheckAsync(PermissionCheckContext checkContext)
        {
            var results = new List<PermissionCheckResult>();
            foreach (var logicProvider in _providers)
            {
                var shouldCare = await logicProvider.ShouldCareAsync(checkContext);
                if (shouldCare)
                {
                    var permissionCheckResult = await logicProvider.CheckPermissionAsync(checkContext);
                    results.Add(permissionCheckResult);
                }
            }
            return results.Combine();
        }
    }
}
