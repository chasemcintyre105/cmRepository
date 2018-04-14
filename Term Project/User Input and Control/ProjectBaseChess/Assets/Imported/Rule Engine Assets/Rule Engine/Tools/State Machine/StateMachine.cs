using System;
using System.Collections.Generic;

namespace RuleEngine {

	public class StateMachine<Command, State> where Command : struct, IConvertible
	                                          where State : struct, IConvertible  {

		private Dictionary<StateTransition<Command, State>, State> transitions;

		public State CurrentState { get; private set; }
		private TransitionHandler<Command, State> handler;

		public StateMachine(State initialState, TransitionHandler<Command, State> handler) {
			transitions = new Dictionary<StateTransition<Command, State>, State>();
			CurrentState = initialState;
			this.handler = handler;
		}

        public void Init() {
            handler.Init();
        }

		public StateMachine<Command, State> AddTransition(State fromState, Command command, State toState) {
			transitions.Add(new StateTransition<Command, State>(fromState, command), toState);
			return this;
		}

		public State IssueCommand(Command command, Dictionary<string, object> properties = null) {
			StateTransition<Command, State> transition = new StateTransition<Command, State>(CurrentState, command);
			State nextState;
            
            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Transition not present: (state, command) -> (" + CurrentState.ToString() + ", " + command.ToString() + ")");

            SelectiveDebug.LogStateMachine(command.ToString() + ": " + CurrentState.ToString() + " -> " + nextState.ToString());
            CurrentState = nextState;

            // State change event
            handler.HandleTransition(command, properties);

            return CurrentState;
		}

	}

}