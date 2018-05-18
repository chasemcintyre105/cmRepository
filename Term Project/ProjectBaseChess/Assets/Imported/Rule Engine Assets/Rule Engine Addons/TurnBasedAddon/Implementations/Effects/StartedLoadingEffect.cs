using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class StartedLoadingEffect : IStartedLoadingEffect {
        
        public override void Apply() {
            RuleEngineController.E.GetController<TurnController>().GameAcceptingUserInput = false;
        }

        public override object[] GetEffectData() {
            return new object[0];
        }

    }

}
