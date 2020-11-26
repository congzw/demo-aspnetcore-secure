using System.Collections.Generic;
using System.Linq;

namespace NbSites.Web.PermissionChecks.RoleBased
{
    public interface IPermissionRuleActionPoolInitService
    {
        void Refresh(PermissionRuleActionPool pool);
    }

    public class PermissionRuleActionPoolInitService : IPermissionRuleActionPoolInitService
    {
        private readonly IList<IPermissionRuleActionProvider> _providers;

        public PermissionRuleActionPoolInitService(IEnumerable<IPermissionRuleActionProvider> providers)
        {
            _providers = providers.OrderBy(x => x.Order).ToList();
        }

        public void Refresh(PermissionRuleActionPool pool)
        {
            foreach (var provider in _providers)
            {
                provider.SetRuleActions(pool);
            }
        }
    }
}