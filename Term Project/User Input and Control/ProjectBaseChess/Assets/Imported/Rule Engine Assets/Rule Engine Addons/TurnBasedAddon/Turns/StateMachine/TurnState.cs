
namespace RuleEngineAddons.TurnBased {
	
	public enum TurnState {
		Waiting = 0,
		Making_Move = 1,
		Resolving_Collisions = 2,
		Switching_Players = 3,
		Finished = 4,
        Placing_Object = 5
	}

}