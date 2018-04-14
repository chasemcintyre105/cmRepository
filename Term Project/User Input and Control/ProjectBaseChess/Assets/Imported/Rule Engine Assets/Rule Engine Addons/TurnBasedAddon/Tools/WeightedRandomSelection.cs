using RuleEngine;
using Random = UnityEngine.Random;

namespace RuleEngineAddons.TurnBased {

	public class WeightedRandomSelection {

        private float total;
        private float chosenNumber;
        private float runningTotal;
        private bool result = false;

        public WeightedRandomSelection(float total) {
            this.total = total;
            chosenNumber = Random.Range(0, total);
            runningTotal = 0;
        }

        public bool Check(float increment) {
            Assert.False("Random selection is not finished", result);

            runningTotal += increment;

            Assert.True("running total does not exceed total", runningTotal <= total);

            result = chosenNumber <= runningTotal;
            return result;
        }
        
	}
}

