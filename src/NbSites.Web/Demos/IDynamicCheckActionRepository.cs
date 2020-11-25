using System.Collections.Generic;

namespace NbSites.Web.Demos
{
    public interface IDynamicCheckActionRepository
    {
        IList<DynamicCheckAction> GetActions();
        void SaveAll(IList<DynamicCheckAction> actions);
    }

    public class DynamicCheckActionRepository : IDynamicCheckActionRepository
    {
        public IList<DynamicCheckAction> Actions { get; set; } = new List<DynamicCheckAction>();

        public DynamicCheckActionRepository()
        {
            Actions.Add(DynamicCheckAction.Create(ConstActionIds.UnsureActionId, KnownFeatureIds.UnsureActionA, "某个迟决定的Action"));
            Actions.Add(DynamicCheckAction.Create(ConstActionIds.UnsureActionId2, KnownFeatureIds.UnsureActionA, "某个迟决定的Action2"));
            Actions.Add(DynamicCheckAction.Create(ConstActionIds.SpecialAction, KnownFeatureIds.DemoOp, "演示用的Action"));
        }

        public IList<DynamicCheckAction> GetActions()
        {
            //use dictionary like this:
            //var checkActions = Actions.ToDictionary(x => x.ActionId);
            return Actions;
        }

        public void SaveAll(IList<DynamicCheckAction> actions)
        {
            //no need
        }
    }
}