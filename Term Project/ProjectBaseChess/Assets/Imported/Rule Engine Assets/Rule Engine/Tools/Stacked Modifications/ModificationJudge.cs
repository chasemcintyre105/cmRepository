using System.Collections.Generic;

namespace RuleEngine {

    public interface ModificationJudge<M> where M : Modification {

		void JudgeStack(Stack<M> stack);

	}

}