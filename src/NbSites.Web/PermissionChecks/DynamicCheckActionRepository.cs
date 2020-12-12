using System.Collections.Generic;

namespace NbSites.Web.PermissionChecks
{
    public interface IDynamicCheckActionRepository
    {
        IList<DynamicCheckAction> GetActions();
        void SaveAll(IList<DynamicCheckAction> actions);
    }

    public class DynamicCheckActionRepository : IDynamicCheckActionRepository
    {
        public DynamicCheckActionRepository()
        {
            Actions.Add(DynamicCheckAction.Create(KnownActionIds.UnsureActionId, KnownPermissionIds.UnsureOp, "某个迟决定的Action"));
            Actions.Add(DynamicCheckAction.Create(KnownActionIds.UnsureActionId2, KnownPermissionIds.UnsureOp, "某个迟决定的Action2"));
        }

        public IList<DynamicCheckAction> Actions { get; set; } = new List<DynamicCheckAction>();

        public IList<DynamicCheckAction> GetActions()
        {
            //todo: read from real data source
            return Actions;
        }

        public void SaveAll(IList<DynamicCheckAction> actions)
        {
        }
    }
}