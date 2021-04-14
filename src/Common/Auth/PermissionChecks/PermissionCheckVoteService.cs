using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Auth.PermissionChecks
{
    public interface IPermissionCheckVoteService
    {
        Task<PermissionCheckResult> CheckAsync(PermissionCheckContext checkContext);
    }

    public class PermissionCheckVoteService : IPermissionCheckVoteService
    {
        private readonly CurrentUserContext _userContext;
        private readonly IList<IPermissionCheckLogicProvider> _providers;
        public PermissionCheckVoteService(IEnumerable<IPermissionCheckLogicProvider> providers, CurrentUserContext userContext)
        {
            _userContext = userContext;
            _providers = providers.OrderBy(x => x.Order).ToList();
        }


        public async Task<PermissionCheckResult> CheckAsync(PermissionCheckContext checkContext)
        {
            var results = new List<PermissionCheckResult>();
            foreach (var logicProvider in _providers)
            {
                var shouldCare = await logicProvider.ShouldCareAsync(_userContext, checkContext);
                if (shouldCare)
                {
                    var permissionCheckResult = await logicProvider.CheckPermissionAsync(_userContext, checkContext);
                    results.Add(permissionCheckResult);
                }
            }
            return results.Combine();

        }
    }
}
