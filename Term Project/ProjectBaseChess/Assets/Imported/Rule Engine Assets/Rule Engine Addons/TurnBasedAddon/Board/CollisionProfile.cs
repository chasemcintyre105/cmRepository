using RuleEngine;
using UnityEngine;

namespace RuleEngineAddons.TurnBased { 

	public class CollisionProfile {

        private BoardManager BoardManager;

		public Vector3 IncidentUnitsOriginalPosition;
		public Unit IncidentUnit;
		public Unit StationaryUnit;

		public CollisionProfile(Engine E, Vector3 IncidentUnitsOriginalPosition, Unit IncidentUnit, Unit StationaryUnit) {
			//this.E = E;
            BoardManager = E.GetManager<BoardManager>();
			this.IncidentUnit = IncidentUnit;
			this.StationaryUnit = StationaryUnit;
			this.IncidentUnitsOriginalPosition = IncidentUnitsOriginalPosition;
        }

		public override bool Equals(object obj) {

			if (obj == null) 
				return false;

			CollisionProfile p = obj as CollisionProfile;
			if (p == null) 
				return false;

			return (IncidentUnit == p.IncidentUnit) && (StationaryUnit == p.StationaryUnit);
		}

		public override int GetHashCode() {
			return IncidentUnit.GetHashCode() + 31 * StationaryUnit.GetHashCode();
		}
		
		public bool IsCollisionResolved() {

			if (!BoardManager.ContainsUnit_TS(IncidentUnit) || !BoardManager.ContainsUnit_TS(StationaryUnit))
				return true;

			return IncidentUnit.GetOffset_TS() != StationaryUnit.GetOffset_TS();
		}

	}

}
