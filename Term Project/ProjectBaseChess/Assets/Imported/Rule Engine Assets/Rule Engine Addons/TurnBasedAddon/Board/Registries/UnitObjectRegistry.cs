using RuleEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

    public class UnitObjectRegistry : BoardObjectRegistry<Unit, UnitType, UnitObjectValue, UnitObjectTypeValue> {

        protected Dictionary<Player, Dictionary<string, GameObject>> UnitTemplatesByPlayerAndUnitID { get; private set; }
        protected List<UnitType> GroupingUnitTypes { get; private set; }
        protected List<UnitType> IndividualUnitTypes { get; private set; }

        private static ObjectValueCreator CreateUnitValue = delegate (Engine E, Unit o, UnitObjectTypeValue otv) {
            return new UnitObjectValue(E, otv, o);
        };

        private static ObjectTypeValueCreator CreateUnitTypeValue = delegate (Engine E, UnitType ot) {
            return new UnitObjectTypeValue(E, ot);
        };

        public UnitObjectRegistry(Engine E) : base(E, CreateUnitValue, CreateUnitTypeValue, false) {

            UnitTemplatesByPlayerAndUnitID = new Dictionary<Player, Dictionary<string, GameObject>>();
            GroupingUnitTypes = new List<UnitType>();
            IndividualUnitTypes = new List<UnitType>();

        }

        // Overridden registration methods
        public override void RegisterObjectType_TS(UnitType type) {
            lock (_registryLock) {
                registerType(type);
                registerUnitType(type);
            }
        }

        protected void registerUnitType(UnitType type) {

            // Register according to grouping or individual
            if (type.IsGroupingType)
                GroupingUnitTypes.Add(type);
            else
                IndividualUnitTypes.Add(type);
            
        }


        // New retrieval methods
        public IEnumerable<UnitType> AllGroupingTypesEnumerable_TS() {
            lock (_registryLock) {
                foreach (UnitType type in GroupingUnitTypes)
                    yield return type;
            }
        }

        public IEnumerable<UnitType> AllIndividualTypesEnumerable_TS() {
            lock (_registryLock) {
                foreach (UnitType type in IndividualUnitTypes)
                    yield return type;
            }
        }

        public void SetUnitTypeTemplate_TS(Player player, string unitTypeID, GameObject template) {
            lock (_registryLock) {
                Dictionary<string, GameObject> unitLookup = null;

                if (!UnitTemplatesByPlayerAndUnitID.TryGetValue(player, out unitLookup)) {
                    unitLookup = new Dictionary<string, GameObject>();
                    UnitTemplatesByPlayerAndUnitID.Add(player, unitLookup);
                }

                if (unitLookup.ContainsKey(unitTypeID))
                    throw new Exception("Unit type already has a template: " + unitTypeID);

                unitLookup.Add(unitTypeID, template);
            }
        }

        public bool HasUnitTypeTemplate_TS(Player player, string unitID) {
            lock (_registryLock) {
                Dictionary<string, GameObject> unitLookup = null;

                if (!UnitTemplatesByPlayerAndUnitID.TryGetValue(player, out unitLookup))
                    return false;

                return unitLookup.ContainsKey(unitID);
            }
        }

        public bool HasUnitTypeTemplate_TS(Player player, string unitID, GameObject template) {
            lock (_registryLock) {
                GameObject existingTemplate = null;
                Dictionary<string, GameObject> unitLookup = null;

                if (!UnitTemplatesByPlayerAndUnitID.TryGetValue(player, out unitLookup))
                    return false;

                if (!unitLookup.TryGetValue(unitID, out existingTemplate))
                    return false;

                return existingTemplate.Equals(template);
            }
        }

        public GameObject GetUnitTypeTemplateByPlayerAndUnitTypeID_TS(Player player, string unitTypeID) {
            lock (_registryLock) {
                GameObject template = null;
                Dictionary<string, GameObject> unitLookup = null;

                if (!UnitTemplatesByPlayerAndUnitID.TryGetValue(player, out unitLookup))
                    throw new Exception("Player " + player.Name + " not registered with unit template lookup");

                if (!unitLookup.TryGetValue(unitTypeID, out template))
                    throw new Exception("Unit type " + unitTypeID + " not registered with unit template lookup and player " + player.Name + ".");

                return template;
            }
        }

    }

}
