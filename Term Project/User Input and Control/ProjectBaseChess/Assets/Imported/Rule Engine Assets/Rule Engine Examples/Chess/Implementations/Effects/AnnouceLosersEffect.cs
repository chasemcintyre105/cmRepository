using RuleEngine;
using RuleEngineAddons.TurnBased;
using System.Collections.Generic;

namespace RuleEngineExamples.Chess {

    public class AnnouceLosersEffect : IAnnouceLosersEffect {

        public List<Player> Losers;
        public bool IsMuliplayer;

        public override Effect Init(params object[] parameters) {
            Losers = (List<Player>) parameters[0];
            IsMuliplayer = (bool) parameters[1];
            return this;
        }

        public override void Apply() {
			Assert.True("There is at least one ", Losers.Count != 0);

            GUIManager GUIManager = RuleEngineController.E.GetManager<GUIManager>();

            GUIManager.WinnerDialog.Activate();
            GUIManager.WinnerDialog.SetText(GUIManager.WinnerDialog.GetStringForMultiplayPlayer(Losers, false));

        }

        public override object[] GetEffectData() {
            return new object[] { Losers, IsMuliplayer };
        }

    }

}
