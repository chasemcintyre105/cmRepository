using RuleEngine;

namespace RuleEngineAddons.RulePanel {

    public class RemoveStackObjectEffect : IRemoveStackObjectEffect {

        private string StackID;
        private ActionStackItemProfile Item;
        private bool CheckIsAtTop;
        private ActionStackManager ActionStackManager;

        public override Effect Init(params object[] parameters) {
            StackID = (string) parameters[0];
            Item = (ActionStackItemProfile) parameters[1];

            if (parameters.Length > 2)
                CheckIsAtTop = (bool) parameters[2];
            else
                CheckIsAtTop = false;

            ActionStackManager = RuleEngineController.E.GetManager<ActionStackManager>();
            return this;
        }

        public override void Apply() {
            Assert.True("StackID is valid", ActionStackManager.Stacks.ContainsKey(StackID));

            if (CheckIsAtTop)
                Assert.True("Item is last item added", ActionStackManager.Stacks[StackID].HasItemAtTop_TS(Item.ID));
            
            // Move the GameObject into the reserve stack
            Item.Object.transform.SetParent(ActionStackManager.ReserveStack.RectTransform);

        }

        public override object[] GetEffectData() {
            return new object[] { StackID, Item.ID, CheckIsAtTop };
        }

    }

}
