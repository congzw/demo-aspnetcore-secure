using System.Collections.Generic;

namespace Common.Auth.PermissionChecks.Actions
{
    public interface IPermissionCheckActionRepository
    {
        IList<PermissionCheckAction> GetActions();
        void SaveAll(IList<PermissionCheckAction> actions);
    }
}