using System;

namespace RuleEngine {

	public abstract class Modification {
		
		public DateTime Timestamp { get; private set; }
		private bool applied = false;

		public void ApplyModification() {
			Assert.False("This modifcation has not already been applied", applied);
			applied = true;

			Timestamp = DateTime.Now;
			Apply();
		}
		
		public void UndoModification() {
			Assert.True("This modifcation has been applied", applied);
			applied = false;

			Undo();
		}

		protected abstract void Apply();
		protected abstract void Undo();

	}

}

