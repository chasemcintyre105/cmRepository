using System;
using System.Collections.Generic;

namespace RuleEngine {

	public class RuleVariableIteratorRegistrationAnchor : Anchor {

        public RuleVariableIteratorRegistrationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public void RegisterRuleVariableIterator(string name, RuleManager.RuleVariableIterator iterator) {
            initialiser.GetEngine().RuleManager.RegisterRuleVariableIterator(name, iterator);
        }

        public override string GetDescription() {
            return "An anchor that allows for the registration of variables for use in rules.";
        }

    }

}