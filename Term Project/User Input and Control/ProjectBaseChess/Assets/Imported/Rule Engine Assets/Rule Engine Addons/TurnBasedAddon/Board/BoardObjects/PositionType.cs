namespace RuleEngineAddons.TurnBased {

    public class PositionType : IBoardObjectType {

        public string ID;
        public bool interactable;

        public PositionType(string ID) {
            this.ID = ID;
            interactable = true;
        }
        
        public string GetID() {
			return ID;
        }
        
    }

}