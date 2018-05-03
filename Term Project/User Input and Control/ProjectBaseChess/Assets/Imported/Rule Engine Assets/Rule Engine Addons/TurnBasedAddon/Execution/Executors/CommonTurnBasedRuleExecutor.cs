using System.Collections.Generic;
using RuleEngine;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

    public abstract class CommonTurnBasedRuleExecutor : TurnBasedRuleExecutor {

		public CommonTurnBasedRuleExecutor(Engine E, Player player) : base (E) {
			CurrentPlayer = player;
			playersDeclaredWinners = new List<Player>();
			playersDeclaredLosers = new List<Player>();
            ModificationManager = E.ModificationManager;
            TurnManager = E.GetManager<TurnManager>();
            UseCompiledRules = true;
        }

		public Player CurrentPlayer;
		public List<Player> playersDeclaredWinners;
		public List<Player> playersDeclaredLosers;
		public bool MoveCancelled;

        protected ModificationManager ModificationManager;
        protected TurnManager TurnManager;

        protected abstract void _Start();
		public override void Start() {

			MoveCancelled = false;
			_Start();

		}

        protected abstract void _Finish();
		public override void Finish() {

            _Finish();
		}
        
        public void ApplyMod(GameModification mod) {
            E.GetManager<BoardManager>().ApplyGameModification_TS(mod);
            LastModificationMade = mod;
        }

        public virtual Vector3 GetUnitPositionWithinExecutionContext(UnitObjectValue unit) {
            return unit.GetInstance().GetOffset_TS();
        }

        public virtual void SetUnitPositionWithinExecutionContext(UnitObjectValue unit, Vector3 pos) {
            unit.GetInstance().SetOffset_TS(pos);
        }

        public static void VisitNonNullValue(RuleExecutor RuleExecutor, RuleComponent obj) {

        }

        public static void VisitNullValue(RuleExecutor RuleExecutor, RuleComponent obj) {
            Assert.Never("Malformed rules: found Null value when executing rules.");
        }
        
        public static void VisitVoid(RuleExecutor RuleExecutor, RuleComponent obj) {
            Assert.Never("Malformed rules: found Void when executing rules.");
        }

    }

}

