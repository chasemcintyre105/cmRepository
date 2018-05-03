using RuleEngine;
using RuleEngineAddons.RulePanel;
using RuleEngineAddons.TurnBased;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class TileAttachment : ITileAttachment {

		private Tile tile;
		public MeshAttachment MeshObject;

		public override void SetBoardObject(IBoardObject value) {
			Assert.Is<Tile>("BoardObject is of type Tile", value);

			tile = (Tile) value;
		}

        public override IBoardObject GetBoardObject() {
			Assert.NotNull("Tile", tile);

			return tile;
		}

        public override bool HasBoardObject() {
			return tile != null;
		}

        public override GameObject GetGameObject() {
			return gameObject;
		}

        public override void OnMouseDown() {}
        public override void OnMouseUp() {}

        public override void OnMouseEnter() {
        }

        public void OnMouseExit() {
        }

        public override void Hover() {

		}

        public override void Unhover() {

		}

        public override void PossibleAction() {
		}

        public override void UnpossibleAction() {
		}

    }
}