using System.Collections.Generic;

namespace RuleEngine {

    public class RuleList : List<Rule> {

        public RuleList() {}

        public RuleList(Rule initialRule) {
            Add(initialRule);
        }

        public override string ToString() {
            return "RuleList: Count=" + Count;
        }

    }

}
