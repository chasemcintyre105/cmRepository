using System;

namespace RuleEngine {

	public class StateTransition<Command, State> where Command : struct, IConvertible
	                                             where State : struct, IConvertible {
		
		public readonly State currentState;
		public readonly Command command;

		public StateTransition(State currentState, Command command) {
			this.currentState = currentState;
			this.command = command;
		}
		
		public override int GetHashCode() {
			return 17 + 31 * currentState.GetHashCode() + 31 * command.GetHashCode();
		}
		
		public override bool Equals(object obj) {
			StateTransition<Command, State> other = obj as StateTransition<Command, State>;
			return other != null && this.currentState.Equals(other.currentState) && this.command.Equals(other.command);
		}
		
	}

}