using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class FinishedLoadingEffect : IFinishedLoadingEffect {
        
        public override void Apply() {
            RuleEngineController.E.GetController<TurnController>().GameAcceptingUserInput = true;
        }

        public override object[] GetEffectData() {
            return new object[0];
        }
        
	}

}
