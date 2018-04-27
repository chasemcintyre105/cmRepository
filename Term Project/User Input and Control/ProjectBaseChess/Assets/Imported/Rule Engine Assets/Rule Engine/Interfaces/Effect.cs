
namespace RuleEngine {

	public class Effect {
        
        public Effect() {}

        public virtual void Apply() { }
        public virtual object[] GetEffectData() { return new object[0]; }

        public virtual Effect Init(params object[] parameters) {
            return this;
        }

        public void Enqueue() {
            SelectiveDebug.LogEffect("Enqueuing effect: " + ToString());

            RuleEngineController.E.EffectController.EnqueueEffect_TS(this);
        }

        public void EnqueueWithDelay(float delayInSeconds) {
            SelectiveDebug.LogEffect("Enqueuing effect with delay: " + ToString());

            RuleEngineController.E.EffectController.EnqueueEffectWithDelay_TS(this, delayInSeconds);
        }

        public override string ToString() {
            string str = GetType().ToString() + " (";
            object[] data = GetEffectData();
            for (int i = 0; i < data.Length; i++) {
                if (i != 0)
                    str += ", ";
                str += data[i];
            }
            return str + ")";
        }

    }

}
