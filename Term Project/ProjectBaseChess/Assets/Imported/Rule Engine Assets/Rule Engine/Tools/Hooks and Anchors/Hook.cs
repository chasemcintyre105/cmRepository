using System;

namespace RuleEngine {

    public class Hook {
    }

	public class Hook<A> : Hook where A : Anchor {

        private Action<A> action;

        public Hook(Action<A> action) {
            this.action = action;
        }

        public void Process(A anchor) {
            action(anchor);
        }
        
    }

}