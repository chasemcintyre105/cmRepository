using System;

namespace RuleEngine {

    public class IfElseStatementCreator : Creator {

        private IfElseStatement IfElseStatement;
        private bool ExpressionSet = false;
        private bool TrueBlockSet = false;
        private bool ElseBlockSet = false;

        public IfElseStatementCreator(Engine E, Creator Parent, IfElseStatement IfElseStatement) : base(E, Parent) {
            this.IfElseStatement = IfElseStatement;
        }

        public ExpressionCreator SetAndConfigure<X>() where X : Expression {
            ExpressionSet = true;
            X newExpression = (X) Activator.CreateInstance(typeof(X), new object[] { E });

            IfElseStatement.SetExpression(newExpression);
            return new ExpressionCreator(E, this, newExpression);
        }

        public IfElseStatementCreator Set<E>() where E : Expression {
            SetAndConfigure<E>().ReturnTo<IfElseStatementCreator>();
            return this;
        }

        public IfElseStatementCreator Set<X>(params Value[] values) where X : Expression {
            SetAndConfigure<X>().Set(values).ReturnTo<IfElseStatementCreator>();
            return this;
        }

        public BlockCreator StartTrueBlock() {
            TrueBlockSet = true;
            return new BlockCreator(E, this, IfElseStatement.GetTrueBlock());
        }

        public BlockCreator StartElseBlock() {
            ElseBlockSet = true;
            return new BlockCreator(E, this, IfElseStatement.GetTrueBlock());
        }

        public C EndIfElseStatement<C>() where C : Creator {
            Assert.True("If-Else Statement was completed", ExpressionSet && TrueBlockSet && ElseBlockSet);

            if (!(ExpressionSet && TrueBlockSet && ElseBlockSet))
                throw new Exception("IfElseStatement was not completed. Not completed: " + (ExpressionSet ? "Expression not set. " : "") + (TrueBlockSet ? "True block not set. " : "") + (ElseBlockSet ? "Else block not set." : ""));

            if (Parent.GetType() != typeof(C))
                throw new Exception(ToString() + " with parent " + Parent.ToString() + " was incorrectly terminated. Found: " + typeof(C).Name + " Expected: " + Parent.GetType().Name);

            return (C) Parent;
        }

        public override string ToString() {
            return "IfElseStatementCreator";
        }

    }

}
