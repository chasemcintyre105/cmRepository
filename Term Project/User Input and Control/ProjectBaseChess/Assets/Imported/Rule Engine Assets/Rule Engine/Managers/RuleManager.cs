using System;
using System.Collections.Generic;

namespace RuleEngine {

    public class RuleManager : IManager {

        public Dictionary<RuleType, RuleList> RuleTypeToList { get; private set; }
        public Dictionary<string, Rule> RulesByUID { get; private set; }
        public Dictionary<string, RuleType> RuleIDToType { get; private set; }
        public Dictionary<RuleType, bool> NoAdditionalRules { get; private set; }
		public Dictionary<RuleType, List<RuleComponent>> SpecificVariables { get; private set; }
		public ModificationStack<RuleModification> ModStack { get; private set; }
		public RuleJudge RuleJudge { get; private set; }
        protected Dictionary<string, RuleSorter> RuleSorters;
        protected Dictionary<string, RuleVariableIterator> RuleVariableIterators;

        public delegate IEnumerable<int> RuleSorter(RuleExecutor RuleExecutor, RuleType RuleType);
        public delegate IEnumerable<bool> RuleVariableIterator(RuleExecutor RuleExecutor, Rule Rule);

        public override void Preinit() {
            RuleTypeToList = new Dictionary<RuleType, RuleList>();
            RulesByUID = new Dictionary<string, Rule>();
            RuleIDToType = new Dictionary<string, RuleType>();
            NoAdditionalRules = new Dictionary<RuleType, bool>();
            SpecificVariables = new Dictionary<RuleType, List<RuleComponent>>();
            ModStack = new ModificationStack<RuleModification>();
            RuleSorters = new Dictionary<string, RuleSorter>();
            RuleVariableIterators = new Dictionary<string, RuleVariableIterator>();
        }

        public override void Init() {

            // Setup rule objects with some initial values and lists (in the case of lists of lists)
            foreach (RuleType type in GetAllRuleTypes()) {
                RuleTypeToList[type] = new RuleList();
                SpecificVariables[type] = new List<RuleComponent>();
                NoAdditionalRules[type] = false;
            }

        }

        public virtual RuleType RegisterRuleType(string ID, bool NoAdditionalRulesByPlayer = false) {
            if (RuleIDToType.ContainsKey(ID))
                throw new Exception("Rule type ID is already registered: " + ID);

            RuleType newType = new RuleType(ID);

            RuleTypeToList[newType] = new RuleList();
            RuleIDToType.Add(ID, newType);
            SpecificVariables[newType] = new List<RuleComponent>();
            NoAdditionalRules[newType] = NoAdditionalRulesByPlayer;

            return newType;
        }

        public virtual RuleType GetRuleType(string ID) {
            RuleType type;
            Assert.True("Rule type exists", RuleIDToType.TryGetValue(ID, out type));
            return type;
        }

        public virtual IEnumerable<RuleType> GetAllRuleTypes() {
            return RuleIDToType.Values;
        }

        public virtual int RuleTypeCount() {
            return RuleIDToType.Values.Count;
        }

        public virtual void SetRuleJudge(RuleJudge RuleJudge) {
			Assert.Null("Existing RuleJudge", this.RuleJudge);
			Assert.NotNull("New RuleJudge", RuleJudge);

			this.RuleJudge = RuleJudge;
			ModStack.SetJudge(RuleJudge);
		}

        public virtual void RegisterRuleSorter(string name, RuleSorter sorter) {
            Assert.False("Rule Sorter is not yet registered: " + name, RuleSorters.ContainsKey(name));
            RuleSorters.Add(name, sorter);
        }

        public virtual RuleSorter GetRuleSorter(string name) {
            RuleSorter sorter = null;
            Assert.True("Rule Sorter is registered: " + name, RuleSorters.TryGetValue(name, out sorter));
            return sorter;
        }

        public virtual void RegisterRuleVariableIterator(string name, RuleVariableIterator iterator) {
            Assert.False("Rule Variable Iterator is not yet registered: " + name, RuleVariableIterators.ContainsKey(name));
            RuleVariableIterators.Add(name, iterator);
        }

        public virtual RuleVariableIterator GetRuleVariableIterator(string name) {
            RuleVariableIterator iterator = null;
            Assert.True("Rule Variable Iterator is registered: " + name, RuleVariableIterators.TryGetValue(name, out iterator));
            return iterator;
        }

    }

}
