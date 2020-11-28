using System.Collections.Generic;
using System.Linq;

namespace NbSites.Web.PermissionChecks.RoleBased
{
    public interface IPermissionRuleActionPoolService
    {
        void RefreshPool(IPermissionRuleActionPool pool);
    }

    public class PermissionRuleActionPoolService : IPermissionRuleActionPoolService
    {
        private readonly IList<IPermissionRuleActionProvider> _providers;

        public PermissionRuleActionPoolService(IEnumerable<IPermissionRuleActionProvider> providers)
        {
            _providers = providers.OrderBy(x => x.Order).ToList();
        }

        public void RefreshPool(IPermissionRuleActionPool pool)
        {
            foreach (var provider in _providers)
            {
                provider.SetRuleActions(pool);
            }
        }
    }
}