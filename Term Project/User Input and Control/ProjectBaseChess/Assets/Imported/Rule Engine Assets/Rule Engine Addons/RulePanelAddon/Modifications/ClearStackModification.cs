using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.RulePanel {

    public class ClearStackModification : StackModification {

        private string StackID;
        private ActionStackManager ActionStackManager;

        private List<ActionStackItemProfile> removedItems = new List<ActionStackItemProfile>();

        private bool? done = null; 

        public ClearStackModification(Engine E, string StackID) : base(E) {
            this.StackID = StackID;
            ActionStackManager = E.GetManager<ActionStackManager>();
        }

        protected override void Apply() {
            Assert.Null("Not started", done);
            done = true;

            ActionStackManager.Stacks[StackID].ForEachTemporaryItem_TS(delegate (ActionStackItemProfile Item) {
                removedItems.Add(Item);
            });

            foreach (ActionStackItemProfile Item in removedItems)
                ActionStackManager.Stacks[StackID].RemoveItem_TS(Item);

        }

        protected override void Undo() {
            Assert.True("Is started and is done", done != null && done.Value);
            done = false;

            foreach (ActionStackItemProfile Item in removedItems)
                ActionStackManager.Stacks[StackID].AddItem_TS(Item);

            removedItems = null;

        }
		
		public override string ToString () {
			return "ClearPositionGameModification";
		}

    }
	
}

