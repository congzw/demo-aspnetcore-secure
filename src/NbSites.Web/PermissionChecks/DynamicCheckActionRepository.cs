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
            Actions.Add(DynamicCheckAction.Create(KnownActionIds.UnsureActionId, KnownPermissionIds.UnsureActionA, "某个迟决定的Action"));
            Actions.Add(DynamicCheckAction.Create(KnownActionIds.UnsureActionId2, KnownPermissionIds.UnsureActionA, "某个迟决定的Action2"));
            Actions.Add(DynamicCheckAction.Create(KnownActionIds.SpecialAction, KnownPermissionIds.DemoOp, "演示用的Action"));
        }

        public IList<DynamicCheckAction> Actions { get; set; } = new List<DynamicCheckAction>();

        public IList<DynamicCheckAction> GetActions()
        {
            return Actions;
        }

        public void SaveAll(IList<DynamicCheckAction> actions)
        {
        }
    }
}