using System;
using System.Collections.Generic;

namespace RuleEngine {

    public abstract class RuleExecutor {

        public struct VisitedObject {
            public RuleComponent obj;
            public int index;
            public VisitedObject(RuleComponent obj, int index) {
                Assert.NotNull("VisitedObject.obj", obj);
                Assert.NotNull("VisitedObject.index", index);
                this.obj = obj;
                this.index = index;
            }
        }
        public Stack<VisitedObject> visitedObjectsStack;

        protected readonly int ID;

        protected bool ErrorOccured;
        protected string ErrorMessage;
        protected bool ErrorIsImportant;
        protected RuleComponent LastEvaluationResult; // The result of the last evaluation only. It is expected that it will be used immediately
		protected bool LastRuleInType; // Used to add something at the end of a list of rules, always true if visiting all rules

        public Engine E;
        public RuleType RuleType { get; protected set; }
		public RuleContext CurrentRuleContext { get; protected set; }
        public bool SkipRule;
        public bool StopExecutingRules;
        public bool UseCompiledRules;
        public string RuleSorter;
        public string RuleVariableIterator;
        public Stack<RuleComponent> StackTrace;

        public RuleExecutor(Engine E) {
			this.E = E;
            ID = E.ExecutionManager.GetIDOfRuleExecutorType(GetType());
            RuleType = null;
            UseCompiledRules = false;
            RuleSorter = "Sequential";
            RuleVariableIterator = "NoChange";
        }
        
        protected void RuleExecutorSetup() {

            // Initialise objects and values relevant to the rule executor object stack, rule iteration, etc...
            visitedObjectsStack = new Stack<VisitedObject>();
            StopExecutingRules = false;
            LastModificationMade = null;
            LastRuleInType = true;

            if (UseCompiledRules && ExecuteCompiledNonNullValue == null) {

                // Set the values of VisitNullValue, VisitVoid and VisitNonNullValue
                ExecuteCompiledNonNullValue = E.ExecutionManager.GetVisitAction(this, typeof(Value));
                ExecuteCompiledVisitNullValue = E.ExecutionManager.GetVisitAction(this, typeof(NullValue));
                ExecuteCompiledVoid = E.ExecutionManager.GetVisitAction(this, typeof(VoidStatement));

            }

            // Per rule set setup
            Start();

        }

        protected void RuleExecutorTeardown() {

            // Per rule set clean up
            Finish();
            
            Assert.True("The visited object stack does not contain any items", visitedObjectsStack.Count == 0);

        }

        public void ExecuteRule(RuleType RuleType, int index, bool LastRuleInType = true) {
            RuleType backup = this.RuleType;
            this.RuleType = RuleType;

            RuleList list = null;
            Assert.True("The rule type is valid", E.RuleManager.RuleTypeToList.TryGetValue(RuleType, out list));
            Assert.True("The index exists in the list associated with the rule type", index < list.Count);

            ExecuteRule(list[index], LastRuleInType);

            this.RuleType = backup;
        }

        public void ExecuteRule(Rule Rule, bool LastRuleInType = true) {
            RuleExecutorSetup();

            this.LastRuleInType = LastRuleInType;

            CurrentRuleContext = new RuleContext() {
                Rule = Rule,
                RuleList = new RuleList(Rule),
                RuleListIndex = 0
            };

            ExecuteIndividualRuleByContext();

            RuleExecutorTeardown();
        }
        
        public void ExecuteRulesForEachRuleType() {
            Assert.Null("Rule type is not set. This is a requirement for being able to iterate through all rule types on a rule executor.", RuleType);

            RuleType backup = RuleType;

            foreach (RuleType type in E.RuleManager.RuleTypeToList.Keys) {
                RuleType = type;
                ExecuteRules();
            }

            RuleType = backup;
        }

		public void ExecuteRules() {
            SelectiveDebug.LogRuleSet(RuleType.ToString());
            Assert.NotNull("RuleType is not null.", RuleType);

            RuleExecutorSetup();

            // Iterate through the rules and visit them in the order determined by the sorting pattern selected
            RuleManager.RuleSorter sorter = E.RuleManager.GetRuleSorter(RuleSorter);

            RuleList list = E.RuleManager.RuleTypeToList[RuleType];
            foreach (int ruleIndex in sorter(this, RuleType)) {

                // Stop the process if the flag is set
                if (StopExecutingRules) {
                    break;
                }

                Rule rule = list[ruleIndex];

                CurrentRuleContext = new RuleContext() {
                    Rule = rule,
                    RuleList = list,
                    RuleListIndex = ruleIndex
                };

                ExecuteIndividualRuleByContext();

            }

            RuleExecutorTeardown();

		}

        private void ExecuteIndividualRuleByContext() {

            ResetExecutor();
            
            RuleManager.RuleVariableIterator iterator = E.RuleManager.GetRuleVariableIterator(RuleVariableIterator);
            IEnumerator<bool> continueIterating = iterator(this, CurrentRuleContext.Rule).GetEnumerator();

            while (continueIterating.MoveNext() && continueIterating.Current)
                _ExecuteRule();

        }

        private void _ExecuteRule() {

            SelectiveDebug.ResetExecutionIndent();

            // Pre-rule visiting callback so that this part of the loop can be influenced by the caller
            if (BeforeStartRule != null)
				BeforeStartRule.Invoke(CurrentRuleContext.Rule);
			
			StartRule(); // Per rule setup

			if (!SkipRule) {
                if (UseCompiledRules) {

                    if (CurrentRuleContext.Rule.CompiledRule == null)
                        throw new Exception("Rule " + CurrentRuleContext.Rule.Name + " has not been compiled");

                    StackTrace = new Stack<RuleComponent>();
                    try {
                        CurrentRuleContext.Rule.CompiledRule.CalculateValue<VoidStatement>(this);
                    } catch (RuleException exception) { // Catch RuleEngine runtime exceptions
                        HandleRuleException(exception, RuleType, CurrentRuleContext.Rule, visitedObjectsStack);
                    }

                } else {

                    visitedObjectsStack.Push(new VisitedObject(CurrentRuleContext.Rule.Block, CurrentRuleContext.RuleListIndex));
                    CurrentRuleContext.Rule.Block.accept[ID](this, CurrentRuleContext.Rule.Block);
                    visitedObjectsStack.Pop();

                }
			} else {
                SelectiveDebug.LogExecution("Skipping rule");
				SkipRule = false;
			}

			FinishRule(); // Per rule clean up
			
			// Post-rule visiting callback so that this part of the loop can be influenced by the caller
			if (AfterFinishRule != null)
				AfterFinishRule.Invoke(CurrentRuleContext.Rule);

		}

        protected virtual void ResetExecutor() {
            ErrorOccured = false;
            ErrorMessage = "";
            LastEvaluationResult = NullValue.Instance;
            SkipRule = false;
        }
        
        private void HandleRuleException(RuleException e, RuleType RuleType,Rule rule, Stack<RuleExecutor.VisitedObject> stackTrace) {
            string type = "";
            switch (e.GetSeverity()) {
            case RuleException.Severity.Error:
                type = "Error";
                break;
            case RuleException.Severity.Skipped:
                type = "Skipped";
                break;
            }
            string stackTraceString = RuleType.ID + " rule " + ": " + rule.Name + "\n" + StackTraceToString(stackTrace);

            E.EffectFactory.EnqueueNewEffect<IRuleExceptionEffect>(type, e.Message, stackTraceString);
        }

        private string StackTraceToString(Stack<RuleExecutor.VisitedObject> stackTrace) {
            string str = "";
            bool notFirst = false;
            while (stackTrace.Count != 0) {
                RuleComponent obj = stackTrace.Pop().obj;
                if (notFirst) {
                    str = "\n" + str;
                } else {
                    notFirst = true;
                }
                str = obj.GetType().Name + str;
            }
            return str;
        }

        public void ReplaceObjectInParentWith(RuleComponent original, ArgumentAccessor originalAccessor, RuleComponent replacer) {
			Assert.True("Can be set as argument", originalAccessor.CanBeSetAsArgument(replacer));
			originalAccessor.Argument = replacer.NewRef();
		}
		
		public void AllArgumentAccept(RuleComponent obj) {
			for (int i = 0; i < obj.ArgumentList.argsByOrder.Count; i++)
				ArgumentAccept(obj, i);
		}

		public void ArgumentAccept(RuleComponent obj, int index) {
            SelectiveDebug.ExecutionIndentAdd(1);
			visitedObjectsStack.Push(new VisitedObject(obj, index));
            
            RuleComponent arg = obj.ArgumentList.GetArgument(index).Instance();
            arg.accept[ID](this, arg);

			visitedObjectsStack.Pop();
            SelectiveDebug.ExecutionIndentAdd(-1);
        }
		
		public Modification LastModificationMade { get; protected set; }
		
		protected void SetEvaluationResult(RuleComponent result) {
			LastEvaluationResult = result;
		}

		// Error functions
		public void DeclareError(string message) {
			ErrorOccured = true;
			ErrorIsImportant = false;
			ErrorMessage = message;
		}
		
		public bool ErrorWasGenerated() {
			return ErrorOccured;
		}
		
		public bool ErrorWasImportant() {
			return ErrorIsImportant;
		}
		
		public string GetErrorMessage() {
			return ErrorMessage;
		}

		// Callbacks
		public Action<Rule> BeforeStartRule;
		public Action<Rule> AfterFinishRule;

		// Starts and finishes
		public abstract void Start();
		public abstract void Finish();
		public abstract void StartRule();
		public abstract void FinishRule();
        
        public ExecutionManager.VisitAction ExecuteCompiledNonNullValue;
        public ExecutionManager.VisitAction ExecuteCompiledVisitNullValue;
        public ExecutionManager.VisitAction ExecuteCompiledVoid;
        
    }

}
