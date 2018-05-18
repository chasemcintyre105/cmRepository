using System;
using System.Collections.Generic;

namespace RuleEngine {

	public class RuleSorterRegistrationAnchor : Anchor {

        public RuleSorterRegistrationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public void RegisterRuleSorter(string name, RuleManager.RuleSorter sorter) {
            initialiser.GetEngine().RuleManager.RegisterRuleSorter(name, sorter);
        }

        public override string GetDescription() {
            return "An anchor that allows for the registration of variables for use in rules.";
        }

    }

}