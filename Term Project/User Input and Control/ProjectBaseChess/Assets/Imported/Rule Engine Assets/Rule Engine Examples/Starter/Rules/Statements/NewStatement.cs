using System.Collections.Generic;
using RuleEngine;
using UnityEngine;

namespace RuleEngineExamples.StarterProject {
	
	public class NewStatement : Statement {

        public override string GetSelectionPanelCategory() {
			return "Commands";
		}

		public NewStatement(Engine E) : base(E) {
			FillArgumentsWithNullValues();
        }
        
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Object", typeof(NewObjectValue), "The object.", false);
		}

		public override string GetDescription() {
			return "This is a new statement";
		}

        public override string ToString() {
            return "NewStatement";
        }

        public override void ExecuteStatement(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            // Do something thread safe here as this will be executed in a secondary thread that cannot directly interact with the Unity API

            Debug.Log("NewStatement executed: Contains " + argsByOrder[0].CalculateValue<NewObjectValue>(RuleExecutor));

        }
    }

}