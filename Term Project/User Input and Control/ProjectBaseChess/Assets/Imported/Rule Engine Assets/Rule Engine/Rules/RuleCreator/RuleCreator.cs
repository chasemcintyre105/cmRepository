using System;
using System.Collections.Generic;

namespace RuleEngine {

    public class RuleCreator {

        private Engine E;
		private RuleList rules;
		private Rule NewRule;

        private bool RuleConstructed;

        public RuleCreator(Engine E, RuleList rules) {
            this.E = E;
			this.rules = rules;
            NewRule = new Rule(E);
			NewRule.Block.ArgumentList.argsByOrder.Clear();
        }

        public RuleCreator(Engine E, RuleType RuleType) {
            this.E = E;
			this.rules = E.RuleManager.RuleTypeToList[RuleType];
			NewRule = new Rule(E);
			NewRule.Block.ArgumentList.argsByOrder.Clear();
		}

        public RuleCreator SetName(string name) {
            NewRule.Name = name;
            return this;
        }

        public RuleCreator SetUID(string uid) {
            NewRule.UID = uid;
            return this;
        }

        public RuleCreator RegisterVariable(Variable variable) {
            List<Variable> variableList;

            // Get the current rule's enumerable variable list
            if (!NewRule.VariablesByType.TryGetValue(variable.VariableType.GetType(), out variableList)) {
                variableList = new List<Variable>();
                NewRule.VariablesByType.Add(variable.VariableType.GetType(), variableList);
            }

            // Create the enumerable variable and register it with current rule's enumerable variable list
            variableList.Add(variable);
            NewRule.Variables.Add(variable);

            return this;
		}

        public RuleCreator LockRule() {
            NewRule.Locked = true;
            return this;
        }

        public RuleCreator MakeRuleIrremovalable() {
            NewRule.Irremovalable = true;
            return this;
        }

        public void Finalise(out Rule rule) {
            Finalise();
            rule = NewRule;
        }

        public void Finalise() {
            Assert.True("Currently building rule", RuleConstructed);
			rules.Add(NewRule);
            if (NewRule.UID != null) {
                if (E.RuleManager.RulesByUID.ContainsKey(NewRule.UID))
                    throw new Exception("Rule UID has already been used: " + NewRule.UID);

                E.RuleManager.RulesByUID.Add(NewRule.UID, NewRule);
            }

        }
        
        public BaseBlockCreator StartRule() {
            BaseBlockCreator blockCreator = new BaseBlockCreator(E, NewRule.Block, this);
            RuleConstructed = true;
            return blockCreator;
        }

        public RuleCreator AddStatement(Statement statement) {
            NewRule.Block.AddStatement(statement);
            RuleConstructed = true;
            return this;
        }

    }

}
