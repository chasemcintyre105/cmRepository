using System;

namespace RuleEngine {

    public class CallbackEffect : Effect {

		public Action callback;
        
        public Effect Init(Action callback) {
            this.callback = callback;
            return this;
        }
        
        public override void Apply() {
			callback.Invoke();
		}

        public override object[] GetEffectData() {
            return new object[0];
        }

    }

}