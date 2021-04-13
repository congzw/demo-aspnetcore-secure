using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Auth.PermissionChecks
{
    public interface IPermissionCheckService
    {
        Task<PermissionCheckResult> CheckAsync(PermissionCheckContext checkContext);
    }

    public class PermissionCheckService : IPermissionCheckService
    {
        private readonly IList<IPermissionCheckLogicProvider> _providers;
        public PermissionCheckService(IEnumerable<IPermissionCheckLogicProvider> providers)
        {
            _providers = providers.OrderBy(x => x.Order).ToList();
        }


        public Task<PermissionCheckResult> CheckAsync(PermissionCheckContext checkContext)
        {
            //todo
            return PermissionCheckResult.NotSure.AsTask();
        }
    }
}
