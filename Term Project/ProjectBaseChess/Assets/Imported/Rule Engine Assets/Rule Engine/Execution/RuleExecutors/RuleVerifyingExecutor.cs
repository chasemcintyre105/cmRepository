using System.Collections.Generic;
using UnityEngine;

namespace RuleEngine {

    public class RuleVerifyingExecutor : RuleExecutor {

		public float ObjectCount;
		public bool MustRevertToPreviousRuleSet;
		public List<ArgumentAccessor> VoidsToRemove;

		public RuleVerifyingExecutor(Engine E) : base (E) {
            UseCompiledRules = false;
			VoidsToRemove = new List<ArgumentAccessor>();
        }

		public override void Start() {
			MustRevertToPreviousRuleSet = false;
		}
		
		public override void Finish() {

		}
		
		public override void StartRule() {

		}
		
		public override void FinishRule() {
            if (MustRevertToPreviousRuleSet)
                StopExecutingRules = true;
		}
        
        public Variable RegisterVariable(ObjectTypeValue objType) {
            List<Variable> variableList;

            // Get the current rule's enumerable variable list
            if (!CurrentRuleContext.Rule.VariablesByType.TryGetValue(objType.GetType(), out variableList)) {
                variableList = new List<Variable>();
                CurrentRuleContext.Rule.VariablesByType.Add(objType.GetType(), variableList);
            }

            // Create the enumerable variable and register it with current rule's enumerable variable list
            Variable enumerableVariable = new Variable(E, "a " + objType.ToString(), objType, null);
            variableList.Add(enumerableVariable);
            CurrentRuleContext.Rule.Variables.Add(enumerableVariable);

            return enumerableVariable;
        }
        
        // Check all argument objects
        protected void CheckAndVisitAllArguments(RuleComponent obj) {
            for (int i = obj.ArgumentList.argsByOrder.Count - 1; i >= 0; i--) {

                // Check that the reference has been set
                if (obj.ArgumentList.argsByOrder[i].reference == null)
                    Debug.LogError("Rule verification problem: " + "Argument " + i + " (" + obj.ArgumentList.argsByOrder[i].name + ") of " + obj.GetType().Name + " is not set. (" + RuleType.ID + " rule " + CurrentRuleContext.RuleListIndex + ")");
                else
                    ArgumentAccept(obj, i);

            }
        }

        // Checks that this instance of this object does not appear twice
        protected void VerifyThisObject(RuleComponent obj) {
            if (obj.GetType() == typeof(VoidStatement)) {
                VisitedObject vo = visitedObjectsStack.Peek();
                VoidsToRemove.Add(new ArgumentAccessor(vo.obj, vo.index));
            } else {
                // Count this object
                ObjectCount++;
            }
		}

        public static void DefaultVisit(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleVerifyingExecutor v = ((RuleVerifyingExecutor) RuleExecutor);

            // Verify the object
            v.VerifyThisObject(obj);
            v.CheckAndVisitAllArguments(obj);
            
        }

        public static void VisitNonNullValue(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleVerifyingExecutor v = (RuleVerifyingExecutor) RuleExecutor;
            v.VerifyThisObject(obj);

            // If the object is an object type that should become a variable so it can be iterated
            VisitedObject vo = v.visitedObjectsStack.Peek();
            if (vo.obj.ArgumentList.argsByOrder[vo.index].TransformObjectTypeIntoVariable &&
                TypeFunctions.IsSameOrSubclassOf(obj.GetType(), typeof(ObjectTypeValue))) {

                // Declare the variable in the rule and insert a clone into the rule set
                Variable newVar = v.RegisterVariable((ObjectTypeValue) obj);

                // Replace the object type value in the rule with the enumerable variable
                v.ReplaceObjectInParentWith(obj, new ArgumentAccessor(vo.obj, vo.index), newVar);

            }

        }

        public static void VisitVoid(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleVerifyingExecutor v = (RuleVerifyingExecutor) RuleExecutor;

            // This the void value for removal
            VisitedObject vo = v.visitedObjectsStack.Peek();
            v.VoidsToRemove.Add(new ArgumentAccessor(vo.obj, vo.index));

        }

        public static void VisitNullValue(RuleExecutor RuleExecutor, RuleComponent obj) {

            // Null values are not allow in the rule set
            ((RuleVerifyingExecutor) RuleExecutor).MustRevertToPreviousRuleSet = true;

        }

    }

}

