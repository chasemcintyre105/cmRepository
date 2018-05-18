using RuleEngine;
using UnityEngine;

namespace RuleEngineAddons.RulePanel {

    public class RemoveStackObjectModification : StackModification {

        private string StackID;
        private ActionStackItemProfile Item;
        private ActionStackManager ActionStackManager;

        private int position;

        public RemoveStackObjectModification(Engine E, string StackID, ActionStackItemProfile Item) : base(E) {
            this.StackID = StackID;
            this.Item = Item;
            ActionStackManager = E.GetManager<ActionStackManager>();
        }

        protected override void Apply() {
            Assert.True("StackID is valid", ActionStackManager.Stacks.ContainsKey(StackID));
            position = ActionStackManager.Stacks[StackID].GetPositionOfItem_TS(Item);
            ActionStackManager.Stacks[StackID].RemoveItem_TS(Item);
        }

        protected override void Undo() {
            ActionStackManager.Stacks[StackID].AddItem_TS(Item, position);
        }

        public override string ToString () {
			return "RemoveStackObjectModification: " + Item.ID.ToString();
		}

    }
	
}

