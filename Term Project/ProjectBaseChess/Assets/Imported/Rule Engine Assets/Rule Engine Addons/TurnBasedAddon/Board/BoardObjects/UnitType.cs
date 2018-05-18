using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class UnitType : IBoardObjectType {
        
        public readonly string ID;
		public bool IsGroupingType;

		public UnitType(Engine E, string ID) {
			this.ID = ID;
            IsGroupingType = false;
        }
		
		public string GetID() {
			return ID;	
		}
        
		public override bool Equals(object ob) {
			return (ob is UnitType) && ((UnitType) ob).ID == ID;
		}
		
		public override int GetHashCode() {
			return ID.GetHashCode();
		}
        
		public override string ToString() {
			return ID;
		}
        
    }

}