namespace RuleEngine {

    public class ArrayIteration {

		public static int[] NewZeroedIntArray(int numberOfIndices) {
			int[] newArray = new int[numberOfIndices];
			return newArray;
		}

		public static bool IncrementCombinatorialArray(int[] array, int[] limits) {
			// Assume that all entries in limits are greater than zero and that all entries in array are between 0 and one less than in limits
			int addressingIndex = 0;
			bool overflow = true;
			while (overflow) {
				if (addressingIndex == array.Length)
					return true;
				array[addressingIndex]++;
				if (array[addressingIndex] >= limits[addressingIndex]) {
					array[addressingIndex] = 0;
					addressingIndex++;
				} else {
					overflow = false;
				}
			}
			return false;
		}
		
	}
}