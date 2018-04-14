using System;

namespace RuleEngine {

    public class PreUntilLoopStatementCreator : Creator {

        private PreUntilLoopStatement UntilStatement;
        private bool ExpressionSet = false;
        private bool BlockSet = false;

        public PreUntilLoopStatementCreator(Engine E, Creator Parent, PreUntilLoopStatement UntilStatement) : base(E, Parent) {
            this.UntilStatement = UntilStatement;
        }

        public ExpressionCreator SetAndConfigure<X>() where X : Expression {
            ExpressionSet = true;
            X newExpression = (X) Activator.CreateInstance(typeof(X), new object[] { E });

            UntilStatement.SetExpression(newExpression);
            return new ExpressionCreator(E, this, newExpression);
        }

        public ExpressionCreator SetAndConfigure<X>(out X newExpression) where X : Expression {
            ExpressionSet = true;
            newExpression = (X) Activator.CreateInstance(typeof(X), new object[] { E });
            UntilStatement.SetExpression(newExpression);
            return new ExpressionCreator(E, this, newExpression);
        }

        public PreUntilLoopStatementCreator Set<E>() where E : Expression {
            SetAndConfigure<E>().ReturnTo<PreUntilLoopStatementCreator>();
            return this;
        }

        public PreUntilLoopStatementCreator Set<X>(params Value[] values) where X : Expression {
            SetAndConfigure<X>().Set(values).ReturnTo<PreUntilLoopStatementCreator>();
            return this;
        }

        public BlockCreator StartBlock() {
            BlockSet = true;
            return new BlockCreator(E, this, UntilStatement.GetLoopBlock());
        }

        public C EndPreUntilLoopStatement<C>() where C : Creator {

            if (!(ExpressionSet && BlockSet))
                throw new Exception("PreUntilLoopStatement was not completed. Not completed: " + (ExpressionSet ? "Expression not set. " : "") + (BlockSet ? "Block not set. " : ""));

            if (Parent.GetType() != typeof(C))
                throw new Exception(ToString() + " with parent " + Parent.ToString() + " was incorrectly terminated. Found: " + typeof(C).Name + " Expected: " + Parent.GetType().Name);

            return (C) Parent;
        }

        public override string ToString() {
            return "PreUntilLoopStatement";
        }

    }

}
