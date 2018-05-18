using System;

namespace RuleEngine {

    public class IfStatementCreator : Creator {

        private IfStatement IfStatement;
        private bool ExpressionSet = false;
        private bool BlockSet = false;

        public IfStatementCreator(Engine E, Creator Parent, IfStatement IfStatement) : base(E, Parent) {
            this.IfStatement = IfStatement;
        }

        public ExpressionCreator SetAndConfigure<X>() where X : Expression {
            ExpressionSet = true;
            X newExpression = (X) Activator.CreateInstance(typeof(X), new object[] { E });

            IfStatement.SetExpression(newExpression);
            return new ExpressionCreator(E, this, newExpression);
        }

        public ExpressionCreator SetAndConfigure<X>(out X newExpression) where X : Expression {
            ExpressionSet = true;
            newExpression = (X) Activator.CreateInstance(typeof(X), new object[] { E });

            IfStatement.SetExpression(newExpression);
            return new ExpressionCreator(E, this, newExpression);
        }

        public IfStatementCreator Set<E>() where E : Expression {
            SetAndConfigure<E>().ReturnTo<IfStatementCreator>();
            return this;
        }

        public IfStatementCreator Set<X>(params Value[] values) where X : Expression {
            SetAndConfigure<X>().Set(values).ReturnTo<IfStatementCreator>();
            return this;
        }

        public BlockCreator StartBlock() {
            BlockSet = true;
            return new BlockCreator(E, this, IfStatement.GetTrueBlock());
        }

        public C EndIfStatement<C>() where C : Creator {

            if (!(ExpressionSet && BlockSet))
                throw new Exception("IfElseStatement was not completed. Not completed: " + (ExpressionSet ? "Expression not set. " : "") + (BlockSet ? "Block not set. " : ""));

            if (Parent.GetType() != typeof(C))
                throw new Exception(ToString() + " with parent " + Parent.ToString() + " was incorrectly terminated. Found: " + typeof(C).Name + " Expected: " + Parent.GetType().Name);

            return (C) Parent;
        }

        public override string ToString() {
            return "IfStatementCreator";
        }

    }

}
