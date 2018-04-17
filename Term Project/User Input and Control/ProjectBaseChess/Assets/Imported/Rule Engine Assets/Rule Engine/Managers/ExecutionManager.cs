using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngine {

    public class ExecutionManager : IManager {

        protected bool RuleExecutorRegistrationFinalised = false;

        public Dictionary<Type, int> RuleExecutorTypeToID { get; private set; }
        public List<Type> RuleExecutorTypes { get; private set; }
        public Dictionary<RuleType, List<CompiledRuleExecutable>> CompiledRules { get; private set; }

        protected Dictionary<Type, VisitAction[]> VisitFunctionsByRuleComponentTypeAndRuleExecutorID;

        public delegate void VisitAction(RuleExecutor RuleExecutor, RuleComponent obj);

        public override void Preinit() {
            CompiledRules = new Dictionary<RuleType, List<CompiledRuleExecutable>>();
            VisitFunctionsByRuleComponentTypeAndRuleExecutorID = new Dictionary<Type, VisitAction[]>();
            RuleExecutorTypeToID = new Dictionary<Type, int>();
            RuleExecutorTypes = new List<Type>();
        }

        public override void Init() {
        }

        public virtual void RegisterRuleExecutorType<V>() where V : RuleExecutor {
            Assert.False("RuleExecutor registration has not yet been finalised", RuleExecutorRegistrationFinalised);
            Type type = typeof(V);
            Assert.False("RuleExecutor is not yet registered", RuleExecutorTypeToID.ContainsKey(type));
            RuleExecutorTypeToID.Add(type, RuleExecutorTypes.Count);
            RuleExecutorTypes.Add(type);
        }

        public virtual void RegisterVisitFunction<E, V>(VisitAction visit) where E : RuleComponent where V : RuleExecutor {
            Assert.True("RuleExecutor registration has been finalised", RuleExecutorRegistrationFinalised);
            VisitAction[] actions = null;

            if (!VisitFunctionsByRuleComponentTypeAndRuleExecutorID.TryGetValue(typeof(E), out actions)) {
                actions = new VisitAction[GetRuleExecutorCount()];
                VisitFunctionsByRuleComponentTypeAndRuleExecutorID.Add(typeof(E), actions);
            }

            actions[RuleExecutorTypeToID[typeof(V)]] = visit;
        }

        public virtual void RegisterVisitFunction(Type RuleComponentType, Type RuleExecutorType, VisitAction visit) {
            Assert.True("RuleExecutor registration has been finalised", RuleExecutorRegistrationFinalised);
            VisitAction[] actions = null;

            if (!VisitFunctionsByRuleComponentTypeAndRuleExecutorID.TryGetValue(RuleComponentType, out actions)) {
                actions = new VisitAction[GetRuleExecutorCount()];
                VisitFunctionsByRuleComponentTypeAndRuleExecutorID.Add(RuleComponentType, actions);
            }

            actions[RuleExecutorTypeToID[RuleExecutorType]] = visit;
        }

        public virtual VisitAction GetVisitAction<V>(Type RuleComponentType) where V : RuleExecutor {
            return _GetVisitAction(RuleExecutorTypeToID[typeof(V)], RuleComponentType);
        }

        public virtual VisitAction GetVisitAction(RuleExecutor RuleExecutor, Type RuleComponentType) {
            return _GetVisitAction(RuleExecutorTypeToID[RuleExecutor.GetType()], RuleComponentType);
        }

        public virtual VisitAction[] GetVisitActions(Type RuleComponentType) {

            VisitAction[] actions = new VisitAction[RuleExecutorTypes.Count];
            for (int i = 0; i < actions.Length; i++) {
                actions[i] = _GetVisitAction(i, RuleComponentType);
            }

            return actions;
        }

        public virtual VisitAction _GetVisitAction(int RuleExecutorID, Type RuleComponentType) {
            VisitAction[] tmp = null;

            if (VisitFunctionsByRuleComponentTypeAndRuleExecutorID.TryGetValue(RuleComponentType, out tmp) && tmp[RuleExecutorID] != null) {
                return tmp[RuleExecutorID];
            }

            if (RuleComponentType.IsSubclassOf(typeof(Expression)) && VisitFunctionsByRuleComponentTypeAndRuleExecutorID.TryGetValue(typeof(Expression), out tmp) && tmp[RuleExecutorID] != null) {
                return tmp[RuleExecutorID];
            }

            if (RuleComponentType.IsSubclassOf(typeof(Value)) && VisitFunctionsByRuleComponentTypeAndRuleExecutorID.TryGetValue(typeof(Value), out tmp) && tmp[RuleExecutorID] != null) {
                return tmp[RuleExecutorID];
            }

            if (RuleComponentType.IsSubclassOf(typeof(Statement)) && VisitFunctionsByRuleComponentTypeAndRuleExecutorID.TryGetValue(typeof(Statement), out tmp) && tmp[RuleExecutorID] != null) {
                return tmp[RuleExecutorID];
            }

            if (RuleComponentType.IsSubclassOf(typeof(RuleComponent)) && VisitFunctionsByRuleComponentTypeAndRuleExecutorID.TryGetValue(typeof(RuleComponent), out tmp) && tmp[RuleExecutorID] != null) {
                return tmp[RuleExecutorID];
            }

            Assert.Never("No visit action was registered for " + RuleExecutorTypes[RuleExecutorID].Name + " and " + RuleComponentType.Name);
            return null;

        }

        public virtual int GetIDOfRuleExecutorType(Type RuleExecutorType) {
            int ID;
            Assert.True("The type is a registered RuleExecutor type", RuleExecutorTypeToID.TryGetValue(RuleExecutorType, out ID));
            return ID;
        }

        public virtual int GetRuleExecutorCount() {
            return RuleExecutorTypes.Count;
        }

        public virtual void FinaliseRuleExecutorRegistration() {
            RuleExecutorRegistrationFinalised = true;
        }

        public virtual void VerifyRulesAsynchronously(Action<RuleVerifyingExecutor> callback = null) {
            SelectiveDebug.LogRuleSet("VerifyRules Async");

            VerifyRulesJob job = new VerifyRulesJob();
			job.E = E;
            job.RuleExecutor = new RuleVerifyingExecutor(E);

            E.ThreadController.ExecuteThreadedJob(job, delegate {
				if (callback != null)
					callback.Invoke(job.RuleExecutor);
			});

		}

        public virtual void CompileRules() {
            SelectiveDebug.LogRuleSet("CompileRules - Synchronous");

            SelectiveDebug.LogRuleExecutor("Starting CompileRules");
            SelectiveDebug.StartTimer("CompileRules");

            foreach (RuleList list in E.RuleManager.RuleTypeToList.Values) {
                for (int i = 0; i < list.Count; i++) {
                    list[i].CompiledRule = list[i].Block.NewCRE();
                }
            }

            SelectiveDebug.StopTimer("CompileRules");
            SelectiveDebug.LogRuleExecutor("Finished CompileRules");

        }
        
        public virtual void CompileRulesAsynchronously(Action callback = null) {

            CompileRulesJob job = new CompileRulesJob();
            job.ExecutionManager = E.ExecutionManager;
            
            E.ThreadController.ExecuteThreadedJob(job, delegate {

                if (callback != null)
                    callback.Invoke();

            });

        }

    }

}

