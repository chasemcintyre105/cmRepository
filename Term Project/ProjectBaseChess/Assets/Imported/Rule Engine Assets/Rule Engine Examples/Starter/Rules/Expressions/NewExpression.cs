using System;
using System.Collections.Generic;
using RuleEngine;
using UnityEngine;

namespace RuleEngineExamples.StarterProject {
	
	public class NewExpression : Expression {

        public override string GetSelectionPanelCategory() {
			return "Expressions";
		}

		public NewExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
        }
        
		public override void DefineArguments() {
            // No arguments, for an example of how to define arguments, see the NewStatement class
		}

		public override string GetDescription() {
			return "This is a new expression";
		}

        public override string ToString() {
            return "NewExpression";
        }
        
        public override Type GetReturnType() {
            return typeof(NewObjectValue);
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            // Do something thread safe here as this will be executed in a secondary thread that cannot directly interact with the Unity API

            Debug.Log("NewExpression executed, creating NewObject and NewObjectValue container...");

            NewObjectRegistry registry = (NewObjectRegistry) E.ObjectRegistries[typeof(NewObject)];

            NewObject newObject = new NewObject(registry.GenerateObjectUID_TS(), registry.GetObjectTypeByID_TS("New object type"));
            registry.RegisterObject_TS(newObject);

            // Return the promised value
            return registry.CreateObjectValue_TS(newObject); // The Type of this must match the Type returned by GetReturnType above
        }

    }

}