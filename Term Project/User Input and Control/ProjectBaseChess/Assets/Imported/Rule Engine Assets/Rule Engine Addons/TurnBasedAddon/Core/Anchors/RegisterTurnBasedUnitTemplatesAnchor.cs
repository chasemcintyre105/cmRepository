using RuleEngine;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

    public class RegisterTurnBasedUnitTemplatesAnchor : Anchor {

        private Engine E;

        public RegisterTurnBasedUnitTemplatesAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
            E = initialiser.GetEngine();
        }

        public void AddUnitTypeTemplate(PlayerPosition position, string unitTypeID, GameObject template) {
            E.GetManager<BoardManager>().UnitRegistry.SetUnitTypeTemplate_TS(E.GetManager<PlayerManager>().PlayersByPosition[position], unitTypeID, template);
        }

        public void AddUnitTypeTemplate(Player player, string unitTypeID, GameObject template) {
            E.GetManager<BoardManager>().UnitRegistry.SetUnitTypeTemplate_TS(player, unitTypeID, template);
        }

        public override string GetDescription() {
            return "An anchor that allows for the registration of turn based unit templates.";
        }

    }

}