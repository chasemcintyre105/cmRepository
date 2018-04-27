using RuleEngine;
using RuleEngineAddons.TurnBased;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RuleEngineExamples.Chess {

    public class ConfigureNewBoardObjectEffect : IConfigureNewBoardObjectEffect {

        public Engine E;
        public IBoardObject obj;

        private TurnController BoardController;
        private ChessSettingsController CommonSettingsController;

        public override Effect Init(params object[] parameters) {
            E = (Engine) parameters[0];
            obj = (IBoardObject) parameters[1];
            BoardController = E.GetController<TurnController>();
            CommonSettingsController = E.GetController<ChessSettingsController>();
            return this;
        }

        public override void Apply() {
			if (obj is Tile)
				ConfigureNewTile(obj as Tile);
			else if (obj is Position)
				ConfigureNewPosition(obj as Position);
			else if (obj is Unit)
				ConfigureNewUnit(obj as Unit);
		}
		
		private void ConfigureNewTile(Tile tile) {

            // Create the game object for the tile and organise it in the hierarchy
            GameObject NewTile = GameObject.Instantiate<GameObject>(tile.type.Template);
            NewTile.transform.SetParent(BoardController.GameContainer.transform);
            tile.SetGameObject(NewTile);

            // Get a reference to the renderer
            Renderer renderer = NewTile.GetComponentInChildren<Renderer>();
            Assert.NotNull("Renderer", renderer);

            if (!tile.Placemarker) {

                // Set the attachment to anything with a renderer
                Assert.Null("Template does not contain a TileAttachment", NewTile.GetComponent<TileAttachment>());
                TileAttachment attachment = NewTile.AddComponent<TileAttachment>();

                // Attach a mesh attachment to the object inside this one with a renderer
                MeshAttachment meshAttachment = renderer.gameObject.AddComponent<MeshAttachment>();
                meshAttachment.attachment = attachment;
                meshAttachment.FollowOnMouseUp = true;
                meshAttachment.FollowOnMouseDown = true;

                // Give the objects a reference to each other
                attachment.SetBoardObject(tile);
                tile.SetBoardObjectAttachment(attachment);

                // Make the name more esthetic in the editor by removing "(Clone)"
                NewTile.name = NewTile.name.Remove(tile.GetGameObject().name.Length - 7) + " " + tile.GetOffset_TS();

            } else {

                PlacemarkerAttachment attachment =  NewTile.AddComponent<PlacemarkerAttachment>();

                attachment.SetEngine(E);

                attachment.SetBoardObject(tile);
                tile.SetBoardObjectAttachment(attachment);

                NewTile.name = "Tile Placemarker";

            }

            // Apply visual effect
            ApplyTileTint(tile);

            // Apply height modification
            Assert.True("A minimum can't be greater than a maximum", tile.type.MinimumHeight <= tile.type.MaximumHeight);
            if (tile.type.MinimumHeight == tile.type.MaximumHeight) {
                NewTile.transform.localScale = new Vector3(NewTile.transform.localScale.x, NewTile.transform.localScale.y, tile.type.MaximumHeight * NewTile.transform.localScale.z);
            } else {
                float r = Random.Range(tile.type.MinimumHeight, tile.type.MaximumHeight);
                NewTile.transform.localScale = new Vector3(NewTile.transform.localScale.x, NewTile.transform.localScale.y, r * NewTile.transform.localScale.z);
            }

            // Apply rotation modification
            tile.GetGameObject().transform.localRotation = Quaternion.identity;
            if (tile.type.AllowRotation) {
                NewTile.transform.Rotate(Vector3.forward, Random.Range(0f, 360f));
            }
            
        }

        private void ApplyTileTint(Tile tile) {
            bool a;

            a = (tile.GetOffset_TS().x + tile.GetOffset_TS().y) % 2 == 0;

            if (a) {
                tile.GetGameObject().GetComponentInChildren<Renderer>().material.color = CommonSettingsController.BoardTemplates.tileStates.TileTint1;
            } else {
                tile.GetGameObject().GetComponentInChildren<Renderer>().material.color = CommonSettingsController.BoardTemplates.tileStates.TileTint2;
            }

        }

        private void ConfigureNewPosition(Position position) {

			// Create the game object for the position and organise it in the hierarchy
			GameObject NewPosition = (GameObject) GameObject.Instantiate(CommonSettingsController.BoardTemplates.PositionTemplate);
			NewPosition.transform.SetParent(BoardController.GameContainer.transform);
            position.SetGameObject(NewPosition);

            // Get a reference to the renderer
            Renderer renderer = NewPosition.GetComponentInChildren<Renderer>();
            Assert.NotNull("Renderer", renderer);

            if (!position.Placemarker) {

                // Set the attachment
                Assert.Null("Template does not contain a PositionAttachment", NewPosition.GetComponent<PositionAttachment>());
                PositionAttachment attachment = NewPosition.AddComponent<PositionAttachment>();

                // Attach a mesh attachment to the object inside this one with a renderer
                MeshAttachment meshAttachment = renderer.gameObject.AddComponent<MeshAttachment>();
                meshAttachment.attachment = attachment;
                meshAttachment.FollowOnMouseUp = true;

                // Give the objects a reference to each other
                attachment.SetBoardObject(position);
                position.SetBoardObjectAttachment(attachment);

                // Set initial state
                attachment.UnpossibleAction();

                // Make the name more esthetic in the editor by removing "(Clone)"
                NewPosition.name = NewPosition.name.Remove(position.GetGameObject().name.Length - 7) + " " + position.GetOffset_TS();

                // If required, make the position not interactable
                if (!position.type.interactable) {
                    NewPosition.layer = 2; // 
                }

            } else {

                // Set the attachment
                PlacemarkerAttachment attachment = NewPosition.AddComponent<PlacemarkerAttachment>();

                // Give the attachment a reference to the engine
                attachment.SetEngine(E);

                // Give the objects a reference to each other
                attachment.SetBoardObject(position);
                position.SetBoardObjectAttachment(attachment);

                // Set initial state
                attachment.PossibleAction();

                // Give the object a better name in the editor
                NewPosition.name = "Position Placemarker";

            }

        }

		private void ConfigureNewUnit(Unit unit) {

            // Create the game object for the position and organise it in the hierarchy
			GameObject template = E.GetManager<BoardManager>().UnitRegistry.GetUnitTypeTemplateByPlayerAndUnitTypeID_TS(unit.player, unit.type.GetID());
            GameObject NewUnit = (GameObject) GameObject.Instantiate(template);
			NewUnit.transform.SetParent(BoardController.GameContainer.transform);
            unit.SetGameObject(NewUnit);

            // Get a reference to the renderer
            Renderer renderer = NewUnit.GetComponentInChildren<Renderer>();
            Assert.NotNull("Renderer", renderer);

            if (!unit.Placemarker) {

                // Add a unit attachment
                Assert.Null("Template does not contain a UnitAttachment", NewUnit.GetComponent<UnitAttachment>());
                UnitAttachment attachment = NewUnit.AddComponent<UnitAttachment>();

                // Attach a mesh attachment to the object inside this one with a renderer
                MeshAttachment meshAttachment = renderer.gameObject.AddComponent<MeshAttachment>();
                meshAttachment.attachment = attachment;
                meshAttachment.FollowOnMouseUp = true;

                // Give the objects a reference to each other
                attachment.SetBoardObject(unit);
                unit.SetBoardObjectAttachment(attachment);

                // Make the name more esthetic in the editor by removing "(Clone)"
                NewUnit.name = NewUnit.name.Remove(unit.GetGameObject().name.Length - 7);

            } else {

                PlacemarkerAttachment attachment = NewUnit.AddComponent<PlacemarkerAttachment>();

                attachment.SetEngine(E);

                attachment.SetBoardObject(unit);
                unit.SetBoardObjectAttachment(attachment);

                NewUnit.name = "Unit Placemarker";

            }

            // Add unit colour
            if (unit.player.PieceMaterial != null)
                renderer.material = unit.player.PieceMaterial;
            else
                renderer.material.color = unit.player.PieceColour;

            // Give the unit its template
            unit.template = template;

        }

        public override object[] GetEffectData() {
            return new object[] { obj };
        }

    }

}
