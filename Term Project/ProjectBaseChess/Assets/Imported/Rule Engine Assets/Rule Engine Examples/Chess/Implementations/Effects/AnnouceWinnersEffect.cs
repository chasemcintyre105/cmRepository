using RuleEngine;
using RuleEngineAddons.TurnBased;
using System;
using System.Collections.Generic;

namespace RuleEngineExamples.Chess {

    public class AnnouceWinnersEffect : IAnnouceWinnersEffect {

        public List<Player> Winners;
        public bool IsMuliplayer;
        public Type NextLevelInitialiser;

        public override Effect Init(params object[] parameters) {
            Winners = (List<Player>) parameters[0];
            IsMuliplayer = (bool) parameters[1];
            NextLevelInitialiser = (Type) parameters[2];
            return this;
        }

        public override void Apply() {
			Assert.True("There is at least one winner", Winners.Count != 0);

            GUIManager GUIManager = RuleEngineController.E.GetManager<GUIManager>();

            GUIManager.WinnerDialog.Activate();
            GUIManager.WinnerDialog.SetText(GUIManager.WinnerDialog.GetStringForMultiplayPlayer(Winners, true));
            
        }

        public override object[] GetEffectData() {
            return new object[] { Winners, IsMuliplayer, NextLevelInitialiser };
        }

    }

}
