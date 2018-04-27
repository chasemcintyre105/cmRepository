namespace RuleEngineAddons.TurnBased {

    public class Turn {

		public static int MAX_COLLISIONS_PER_TURN = 10;

		public Turn() {
			CollisionResolutions = 0;
		}

		public Player player;
		public int number;
		public int CollisionResolutions;

	}
	
}