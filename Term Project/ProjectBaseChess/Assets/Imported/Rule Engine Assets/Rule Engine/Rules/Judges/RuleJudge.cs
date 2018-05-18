using System.Collections.Generic;

namespace RuleEngine {

    /*
	 * Determines what and how many rule changes can be made before the players turn is up.
	 */
    public abstract class RuleJudge : ModificationJudge<RuleModification> {

        public abstract void JudgeStack(Stack<RuleModification> stack);
        public abstract void ClearJudgement();
        public abstract bool AreAllValuesCompleted();
        public abstract bool IsNewRuleAllowed();
        public abstract bool IsRemovingARuleAllowed();
        public abstract bool IsNewStatementAllowed();
        public abstract bool IsRemovingAnObjectAllowed();
        public abstract bool IsReplacingAnObjectAllowed();
        public abstract bool IsSwappingObjectsAllowed();
        public abstract bool IsAccessingARule();
        public abstract Rule GetAccessedRule();
        public abstract bool IsOnlyOneStatementAccessible();
        public abstract RuleComponent GetAccessibleRuleComponent();
        public abstract bool AreBlockContainingStatementsAllowed();
        public abstract bool IsRuleComponentAllowed(Rule currentRule, RuleComponent questionableObject);
        public abstract Judgement GetJudgement();

    }

}

