using System;

namespace RuleEngine {

    public class StatementCreator : Creator {

        private Statement Statement;
        private int currentIndex = 0;

        public StatementCreator(Engine E, Creator Parent, Statement Statement) : base(E, Parent) {
            this.Statement = Statement;
        }

        public ExpressionCreator SetAndConfigure<X>() where X : Expression {
            X newExpression = (X) Activator.CreateInstance(typeof(X), new object[] { E });
            Set(newExpression);
            return new ExpressionCreator(E, this, newExpression);
        }

        public StatementCreator Set<X>() where X : Expression {
            SetAndConfigure<X>().ReturnTo<StatementCreator>();
            return this;
        }

        public StatementCreator Set<X>(params Value[] values) where X : Expression {
            SetAndConfigure<X>().Set(values).ReturnTo<StatementCreator>();
            return this;
        }

        public StatementCreator Set(params Value[] values) {
            for (int i = 0; i < values.Length; i++)
                Set(values[i]);
            currentIndex += values.Length;
            return this;
        }

        public StatementCreator Set(Value value) {

            if (currentIndex >= Statement.ArgumentList.GetNumberOfArguments())
                throw new Exception(ToString() + " was given too many arguments. Only has " + Statement.ArgumentList.GetNumberOfArguments() + " arguments. Last argument given was " + value.ToString());

            Statement.ArgumentList.SetArgument(currentIndex++, value);
            return this;
        }

        public StatementCreator AccessStatement(Action<Statement> callback) {
            callback.Invoke(Statement);
            return this;
        }

        public C ReturnTo<C>() where C : Creator {

            if (Parent.GetType() != typeof(C))
                throw new Exception(ToString() + " with parent " + Parent.ToString() + " was incorrectly terminated. Found: " + typeof(C).Name + " Expected: " + Parent.GetType().Name);

            return (C) Parent;
        }

        public override string ToString() {
            return "StatementCreator (" + Statement.GetType().Name + ")";
        }

    }

}
