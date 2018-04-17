using System;

namespace RuleEngine {

    public class ExpressionCreator : Creator {

        private Expression Expression;
        private int currentIndex = 0;

        public ExpressionCreator(Engine E, Creator Parent, Expression Expression) : base(E, Parent) {
            this.Expression = Expression;
        }

        public ExpressionCreator SetAndConfigure<X>() where X : Expression {
            X newExpression = (X) Activator.CreateInstance(typeof(X), new object[] { E });
            Set(newExpression);
            return new ExpressionCreator(E, this, newExpression);
        }

        public ExpressionCreator Set<X>(params Value[] values) where X : Expression {
            SetAndConfigure<X>().Set(values).ReturnTo<ExpressionCreator>();
            return this;
        }

        public ExpressionCreator Set(params Value[] values) {
            for (int i = 0; i < values.Length; i++)
                Set(values[i]);
            currentIndex += values.Length;
            return this;
        }

        public ExpressionCreator Set(Value value) {
            Expression.ArgumentList.SetArgument(currentIndex++, value);
            return this;
        }

        public ExpressionCreator AccessExpression(Action<Expression> callback) {
            callback.Invoke(Expression);
            return this;
        }

        public C ReturnTo<C>() where C : Creator {

            if (Parent.GetType() != typeof(C))
                throw new Exception(ToString() + " with parent " + Parent.ToString() + " was incorrectly terminated. Found: " + typeof(C).Name + " Expected: " + Parent.GetType().Name);

            return (C) Parent;
        }

        public override string ToString() {
            return "ExpressionCreator (" + Expression.GetType().Name + ")";
        }

    }

}
