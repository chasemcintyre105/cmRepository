namespace RuleEngine {

    public class CREVoidWrapper : CREStatement {
        
        public CREVoidWrapper() : base (null, null, null) {}
        
        public override O CalculateValue<O>(RuleExecutor RuleExecutor) {
            RuleExecutor.ExecuteCompiledVoid(RuleExecutor, VoidStatement.Instance);
            Assert.True("Statements always return void", VoidStatement.Instance is O);
            return VoidStatement.Instance as O;
        }

    }

}
