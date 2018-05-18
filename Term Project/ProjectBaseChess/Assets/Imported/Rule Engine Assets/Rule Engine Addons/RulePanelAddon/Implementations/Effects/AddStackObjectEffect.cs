using RuleEngine;

namespace RuleEngineAddons.RulePanel {

    public class AddStackObjectEffect : IAddStackObjectEffect {

        private string StackID;
        private ActionStackItemProfile Item;
        private int? position = null;
        private ActionStackManager ActionStackManager;

        public override Effect Init(params object[] parameters) {
            StackID = (string) parameters[0];
            Item = (ActionStackItemProfile) parameters[1];

            if (parameters.Length > 2) {
                position = (int) parameters[2];
            }

            ActionStackManager = RuleEngineController.E.GetManager<ActionStackManager>();
            return this;
        }

        public override void Apply() {
            Assert.True("StackID is valid", ActionStackManager.Stacks.ContainsKey(StackID));

            ActionStackProfile stack = ActionStackManager.Stacks[StackID];

            switch (Item.Type) {
            case ActionStackItemProfile.StackOptionType.Label:
                stack.AddLabelItem(Item);
                break;
            case ActionStackItemProfile.StackOptionType.Button:
                stack.AddButtonItem(Item);
                break;
            default:
                Assert.Never("Stack option type not supported: " + Item.Type);
                break;
            }

            if (position == null)
                stack.AddGameObjectAtTheTop(Item);
            else
                stack.AddGameObjectAtPosition(Item, position.Value);

        }

        public override object[] GetEffectData() {
            return new object[] { StackID, Item, position };
        }

    }

}
