using RuleEngine;

namespace RuleEngineExamples.StarterProject {

    public class NewRuleExecutor : RuleExecutor {

        public NewRuleExecutor(Engine E) : base (E) {
            RuleType = E.RuleManager.GetRuleType(StarterAddon.NewRuleTypeID);
            UseCompiledRules = true;
        }

		public override void Start() {
        }

        public override void Finish() {
        }

        public override void StartRule() {
        }

        public override void FinishRule() {
        }

        public static void DefaultVisitFunction(RuleExecutor RuleExecutor, RuleComponent obj) {
            // When executing this rule type without compiling, this function will be used when encountering any rule component that doesn't have another registered function.
            // When compiled, the ExecuteStatement method of the rule component is used instead.
        }

        public static void StatementVisitFunction(RuleExecutor RuleExecutor, RuleComponent obj) {
            // When executing this rule type without compiling, this function will be used when encountering any statements that don't have any more specific registered functions.
            // When compiled, the ExecuteStatement method of the rule component is used instead.
        }

        public static void ExpressionVisitFunction(RuleExecutor RuleExecutor, RuleComponent obj) {
            // When executing this rule type without compiling, this function will be used when encountering any expressions that don't have any more specific registered functions.
        }

        public static void ValueVisitFunction(RuleExecutor RuleExecutor, RuleComponent obj) {
            // This function will be used when encountering any values that don't have any more specific registered functions.
            // This is used in compiled rules
        }

        public static void NullVisitFunction(RuleExecutor RuleExecutor, RuleComponent obj) {
            // This function will be used when encountering any null value.
            // This is used in compiled rules
        }

        public static void VoidVisitFunction(RuleExecutor RuleExecutor, RuleComponent obj) {
            // This function will be used when encountering any void statement.
            // This is used in compiled rules
        }

        public static void NewStatementVisitFunction(RuleExecutor RuleExecutor, RuleComponent obj) {
            // When executing this rule type without compiling, this function will be used when encountering any NewStatement.
        }

    }

}

