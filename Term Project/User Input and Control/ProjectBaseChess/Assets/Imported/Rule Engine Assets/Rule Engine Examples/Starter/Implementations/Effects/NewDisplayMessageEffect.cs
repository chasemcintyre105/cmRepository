using RuleEngine;
using UnityEngine;

namespace RuleEngineExamples.StarterProject {

    public class NewDisplayMessageEffect : IDisplayMessageEffect {

        private string message;

        public override Effect Init(params object[] parameters) {
            message = (string) parameters[0];
            return this;
        }

        public override void Apply() {
            Debug.Log(message);
        }

        public override object[] GetEffectData() {
            return new object[] { message };
        }

    }

}
