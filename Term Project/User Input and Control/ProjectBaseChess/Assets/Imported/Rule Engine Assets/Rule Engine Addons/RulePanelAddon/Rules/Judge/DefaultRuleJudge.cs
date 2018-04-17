using RuleEngine;
using System;
using System.Collections.Generic;

namespace RuleEngineAddons.RulePanel {

	/*
	 * Allows total freedom in changing the rules, but still informs as to whether the rules are complete or not
	 */
	public class DefaultRuleJudge : RuleJudge {

        private class RuleJudgementProfile {
            public Rule rule;
            public Judgement judgement;
        }

        private Dictionary<Rule, RuleJudgementProfile> AssociatedRules;

        public DefaultRuleJudge(Engine E) {
            AssociatedRules = new Dictionary<Rule, RuleJudgementProfile>();
        }

		// Check the latest modification and determine whether the player has finished their turn or not.
		public override void JudgeStack(Stack<RuleModification> stack) {

			RuleModification mod = stack.Peek();

            Assert.NotNull("mod.AssociatedRule in DefaultRuleJudge", mod.AssociatedRule);

            RuleJudgementProfile currentRuleProfile = null;
            if (!AssociatedRules.TryGetValue(mod.AssociatedRule, out currentRuleProfile)) {
                currentRuleProfile = new RuleJudgementProfile() {
                    rule = mod.AssociatedRule,
                    judgement = Judgement.StillGoing
                };
                AssociatedRules.Add(mod.AssociatedRule, currentRuleProfile);
            }
            
            if (AreAllValuesCompletedIn(mod.AssociatedRule)) {
                currentRuleProfile.judgement = Judgement.Finished;
            } else {
                currentRuleProfile.judgement = Judgement.StillGoing;
            }

		}

        public bool AreAllValuesCompletedIn(Rule rule) {
            return ContainsNoNullOrVoidRecursive(rule.Block);
        }

        public override bool AreAllValuesCompleted() {
            foreach (RuleJudgementProfile ruleProfile in AssociatedRules.Values) {
                if (ruleProfile.judgement != Judgement.Finished)
                    return false;
            }

            return true;
		}

		private bool ContainsNoNullOrVoidRecursive(RuleComponent obj) {
			
			if (obj is NullValue || obj is VoidStatement)
				return false;

			foreach (Argument a in obj.ArgumentList.argsByOrder) {

				if (ContainsNoNullOrVoidRecursive(a.reference.Instance()) == false)
					return false;

			}

			return true;
		}

		public override bool IsNewRuleAllowed() {
			return true;
		}

		public override bool IsRemovingARuleAllowed() {
            return true;
        }

		public override bool IsNewStatementAllowed() {
            return true;
        }
		
		public override bool IsRemovingAnObjectAllowed() {
            return true;
        }
		
		public override bool IsReplacingAnObjectAllowed() {
            return true;
        }
		
		public override bool IsSwappingObjectsAllowed() {
            return true;
        }

		public override bool IsAccessingARule() {
			return false;
		}

		public override Rule GetAccessedRule() {
			return null;
		}

		public override bool IsOnlyOneStatementAccessible() {
			return false;
		}

		public override RuleComponent GetAccessibleRuleComponent() {
			return null;
		}

		public override bool AreBlockContainingStatementsAllowed() {
			return true;
		}

		public override bool IsRuleComponentAllowed(Rule currentRule, RuleComponent objectInQuestion) {
            return true;
		}

		private bool SearchArgumentsRecursively(RuleComponent potentialParent, RuleComponent potentialChild) {
			foreach (Argument a in potentialParent.ArgumentList.argsByOrder) {
				if (a.reference.Instance() == potentialChild)
					return true;
				if (SearchArgumentsRecursively(a.reference.Instance(), potentialChild))
					return true;
			}
			return false;
		}

		public override Judgement GetJudgement() {
			return AreAllValuesCompleted() ? Judgement.Finished : Judgement.StillGoing;
		}
		
		public override void ClearJudgement() {
            AssociatedRules.Clear();
        }

	}

}

