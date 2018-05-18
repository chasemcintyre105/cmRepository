using System.Collections.Generic;

namespace RuleEngine {

    public class ModificationStack<M> where M : Modification {

		private Stack<M> stack; 
		private ModificationJudge<M> judge;
		private LinkedList<M> clearedModifications;

		public ModificationStack() {
			stack = new Stack<M>();
			clearedModifications = new LinkedList<M>();
		}

		public void SetJudge(ModificationJudge<M> judge) {
			Assert.Null("ModificationJudge", this.judge);
			this.judge = judge;
		}

		public void ApplyModification(M mod) {
			mod.ApplyModification();
			stack.Push(mod);
			JudgeStack();
		}
		
		public void JudgeStack() {
			if (judge != null)
				judge.JudgeStack(stack);
		}

        private M UndoBlockingModification;

        public void SetUndoBlock(M mod) {
            Assert.True("Undo blocking modification is in the stack", stack.Contains(mod));
            Assert.Null("There is not yet an undo block", UndoBlockingModification);

            UndoBlockingModification = mod;

        }

        public void ClearUndoBlock() {
            Assert.NotNull("There is already an undo block", UndoBlockingModification);

            UndoBlockingModification = null;

        }

		public void UndoAllModifications() {
            if (UndoBlockingModification == null) {
                while (stack.Count > 0) {
                    stack.Pop().UndoModification();
                }
            } else {
                while (stack.Peek() != UndoBlockingModification) {
                    stack.Pop().UndoModification();
                }
            }
		}

		public void UndoModificationsUpToAndIncluding(M mod) {
            if (UndoBlockingModification == null) {
                while (stack.Count > 0) {
                    stack.Pop().UndoModification();
                    if (stack.Count > 0 && stack.Peek().Equals(mod))
                        break;
                }
            } else {
                while (stack.Peek() != UndoBlockingModification) {
                    stack.Pop().UndoModification();
                    if (stack.Count > 0 && stack.Peek().Equals(mod))
                        break;
                }
            }
		}
		
		public void UndoModificationsUpTo(M mod) {
            if (UndoBlockingModification == null) {
                while (stack.Count > 0 && !stack.Peek().Equals(mod)) {
                    stack.Pop().UndoModification();
                }
            } else {
                while (stack.Peek() != UndoBlockingModification && !stack.Peek().Equals(mod)) {
                    stack.Pop().UndoModification();
                }
            }
		}

		public void ClearModifications() {
            while (stack.Count > 0) {
                // Store cleared modifications
                clearedModifications.AddLast(stack.Pop());
			}
		}

		public LinkedList<M> GetClearedModifications() {
			return clearedModifications;
		}

		public M Peek() {
			if (stack.Count > 0)
				return stack.Peek();
			else
				return default(M);
		}

	}

}

