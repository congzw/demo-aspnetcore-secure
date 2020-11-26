using System.Collections.Generic;
using System.Linq;

namespace NbSites.Web.PermissionChecks.RoleBased
{
    public interface IPermissionRuleActionPoolInitService
    {
        void Refresh();
    }

    public class PermissionRuleActionPoolInitService : IPermissionRuleActionPoolInitService
    {
        private readonly IPermissionRuleActionPool _pool;
        private readonly IList<IPermissionRuleActionProvider> _providers;

        public PermissionRuleActionPoolInitService(IEnumerable<IPermissionRuleActionProvider> providers, IPermissionRuleActionPool pool)
        {
            _pool = pool;
            _providers = providers.OrderBy(x => x.Order).ToList();
        }

        public void Refresh()
        {
            foreach (var provider in _providers)
            {
                provider.SetRuleActions(_pool);
            }
        }
    }
}