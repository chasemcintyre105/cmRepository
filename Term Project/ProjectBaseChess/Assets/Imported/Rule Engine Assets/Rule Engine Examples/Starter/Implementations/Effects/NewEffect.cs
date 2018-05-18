using RuleEngine;

namespace RuleEngineExamples.StarterProject {

    public class NewEffect : INewEffect {

        private object firstParameter;
        private object secondParameter;
        private object thirdParameterOptional = null;

        public override Effect Init(params object[] parameters) {
            firstParameter = (object) parameters[0];
            secondParameter = (object) parameters[1];

            if (parameters.Length > 2)
                thirdParameterOptional = (string) parameters[2];

            return this;
        }

        public override void Apply() {
            // This is always executed in the main thread so that the Unity API is available.
        }

        public override object[] GetEffectData() {
            return new object[] { firstParameter, secondParameter, thirdParameterOptional };
        }

    }

}
