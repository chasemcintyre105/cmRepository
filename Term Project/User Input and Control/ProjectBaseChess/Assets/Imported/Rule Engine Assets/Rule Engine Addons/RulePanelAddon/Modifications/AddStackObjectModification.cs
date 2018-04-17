using RuleEngine;

namespace RuleEngineAddons.RulePanel {

    public class AddStackObjectModification : StackModification {
		
        private string StackID;
        private ActionStackItemProfile Item;
        private ActionStackManager ActionStackManager;

        public AddStackObjectModification(Engine E, string StackID, ActionStackItemProfile Item) : base(E) {
            this.StackID = StackID;
            this.Item = Item;
            ActionStackManager = E.GetManager<ActionStackManager>();
        }
        
        protected override void Apply() {
            Assert.True("StackID is valid", ActionStackManager.Stacks.ContainsKey(StackID));
            ActionStackManager.Stacks[StackID].AddItem_TS(Item);
        }
		
		protected override void Undo() {
            Assert.True("StackID is valid", ActionStackManager.Stacks.ContainsKey(StackID));
            Assert.True("Item is top most", ActionStackManager.Stacks[StackID].HasItemAtTop_TS(Item.ID));
            ActionStackManager.Stacks[StackID].RemoveItem_TS(Item);
        }
		
		public override string ToString () {
			return "AddStackObjectModification: " + Item.ID.ToString();
		}

    }
	
}

