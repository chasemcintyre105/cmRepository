using System;

namespace RuleEngine {

    public class BlockCreator : Creator {

        protected Block Block;

        public BlockCreator(Engine E, Creator Parent, Block Block) : base(E, Parent) {
            this.Block = Block;
        }

        public IfStatementCreator AddIfStatement() {
            IfStatement newIfStatement = new IfStatement(E);
            Block.AddStatement(newIfStatement.NewRef());
            return new IfStatementCreator(E, this, newIfStatement);
        }

        public IfElseStatementCreator AddIfElseStatement() {
            IfElseStatement newIfElseStatement = new IfElseStatement(E);
            Block.AddStatement(newIfElseStatement.NewRef());
            return new IfElseStatementCreator(E, this, newIfElseStatement);
        }

        public PreUntilLoopStatementCreator AddPreLoopUntilStatement() {
            PreUntilLoopStatement newPreUntilLoopStatement = new PreUntilLoopStatement(E);
            Block.AddStatement(newPreUntilLoopStatement.NewRef());
            return new PreUntilLoopStatementCreator(E, this, newPreUntilLoopStatement);
        }

        public PostUntilLoopStatementCreator AddPostLoopUntilStatement() {
            PostUntilLoopStatement newPostUntilLoopStatement = new PostUntilLoopStatement(E);
            Block.AddStatement(newPostUntilLoopStatement.NewRef());
            return new PostUntilLoopStatementCreator(E, this, newPostUntilLoopStatement);
        }

        public StatementCreator AddAndConfigure<S>() where S : Statement {
            Assert.True("Add does not accept this type of statement", typeof(S) != typeof(IfStatement) && typeof(S) != typeof(Block) && typeof(S) != typeof(IfElseStatement));

            S newStatement = (S) Activator.CreateInstance(typeof(S), new object[] { E });
            Block.AddStatement(newStatement.NewRef());

            return new StatementCreator(E, this, newStatement);
        }

        public BlockCreator Add<S>(params Value[] values) where S : Statement {
            AddAndConfigure<S>().Set(values).ReturnTo<BlockCreator>();
            return this;
        }

        public BlockCreator Add<S>() where S : Statement {
            AddAndConfigure<S>().ReturnTo<BlockCreator>();
            return this;
        }
        
        public C EndBlock<C>() where C : Creator {
            Assert.NotNull("Parent", Parent);

            if (Parent.GetType() != typeof(C))
                throw new Exception(ToString() + " with parent " + Parent.ToString() + " was incorrectly terminated. Found: " + typeof(C).Name + " Expected: " + Parent.GetType().Name);

            return (C) Parent;
        }

        public override string ToString() {
            return "BlockCreator";
        }

    }

}
