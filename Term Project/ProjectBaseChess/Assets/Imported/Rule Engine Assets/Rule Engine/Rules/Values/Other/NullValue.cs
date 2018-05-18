using System;

namespace RuleEngine {

	public class NullValue : Value {

		public static NullValue Instance = new NullValue();
        
		public override string GetSelectionPanelCategory() {
			return "Null";
		}
        
		public override bool IsEqualTo (Value v) {
			return v.GetType() != typeof(NullValue);
		}

		private NullValue() : base (RuleEngineController.E) {
            Editability = RuleComponentEditability.Editable;
        }

        public override string GetDescription() {
			return "A value that represents the absence of a value";
		}

        public override string ToString() { return "Null Value"; }

    }

}