using RuleEngine;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

	public class BoardManager : IManager {

        protected readonly object _boardLock;

        protected Queue<CollisionProfile> Collisions;

        public float sqrPositioningTolerance { get; protected set; }
        
        public UnitObjectRegistry UnitRegistry;
        public TileObjectRegistry TileRegistry;
        public PositionObjectRegistry PositionRegistry;

        protected ModificationManager ModificationManager;

        protected List<Position> PlacemarkerPositions;
        protected IBoardObject placingObject;

        protected TurnController BoardController;

        public BoardManager() {
            _boardLock = new object();
        }

        public override void Preinit() {
            Collisions = new Queue<CollisionProfile>();
            UnitRegistry = new UnitObjectRegistry(E);
            TileRegistry = new TileObjectRegistry(E);
            PositionRegistry = new PositionObjectRegistry(E);

            E.RegisterNewObjectRegistry(UnitRegistry);
            E.RegisterNewObjectRegistry(TileRegistry);
            E.RegisterNewObjectRegistry(PositionRegistry);
        }

        public override void Init() {
            ModificationManager = E.ModificationManager;
            BoardController = E.GetController<TurnController>();
            sqrPositioningTolerance = BoardController.PositioningTolerance * BoardController.PositioningTolerance;
        }
        
        public virtual CollisionProfile PopNextUnitCollision_TS() {
			lock (_boardLock) {
				return (Collisions.Count > 0) ? Collisions.Dequeue() : null;
			}
		}

        public virtual bool DetectUnitCollisions_TS() {
			lock (_boardLock) {
                foreach (Position position in PositionRegistry.AllObjectsEnumerable_TS()) {
					if (position.HasUnitCollisions_TS()) {

						// Add just the next unit in the list as a collision, ignore the rest
						Unit firstCollidingUnit = position.GetFirstCollidingUnit_TS();
						CollisionProfile collision = new CollisionProfile(E,
						                                                  firstCollidingUnit.lastOffset_TS,
						                                                  firstCollidingUnit, 
						                                                  position.GetUnit_TS());

						// Register this collision
						if (!Collisions.Contains(collision))
							Collisions.Enqueue(collision);
								
					}
                }

                return Collisions.Count > 0;
			}
		}


        // Threadsafe BoardObject functions

        public virtual Position GetPosition_TS(Vector3 v) {
			return PositionRegistry.GetClosestAtOffsetWithinTolerance_TS(v, sqrPositioningTolerance);
		}

        public virtual Position GetPosition_TS(float x, float y) {
			return PositionRegistry.GetClosestAtOffsetWithinTolerance_TS(new Vector3(x, y), sqrPositioningTolerance);
		}

        public virtual bool IsPositionWithinTolerance_TS(Vector3 v) {
			return PositionRegistry.IsOffsetRegisteredWithinTolerance_TS(v, sqrPositioningTolerance);
		}

        public virtual List<Position> FindAllPositionsWithinTolerance_TS(Vector3 v) {
            return PositionRegistry.GetAllAtOffsetWithinTolerance_TS(v, sqrPositioningTolerance);
        }

        public virtual void RegisterTile_TS(Tile tile) {
			TileRegistry.RegisterObject_TS(tile);
		}

        public virtual void RegisterPosition_TS(Position position) {
			PositionRegistry.RegisterObject_TS(position);
		}

        // Threadsafe UnitManager functions

        public virtual bool ContainsUnit_TS(Unit unit) {
			lock (_boardLock) {
				return UnitRegistry.IsRegistered_TS(unit);
			}
		}

        // Threadsafe modification Stack functions
        public virtual void ApplyGameModification_TS(GameModification mod) {

            // Let the modifcation gather any necessary references specific to the modifcation
            // This needs to be outside of the lock since it is possible that it could call back to a TS function in the BoardManager and cause a deadlock
            mod.ValidateModifcation_TS();

            ModificationManager.ApplyModification_TS(mod);

        }

        public virtual void PlaceNewObject() {
            Assert.NotNull("placingObject", placingObject);

            if (placingObject is Tile) {

                Tile tile = placingObject as Tile;

                // Create an instance of the tile type
                new TileCreator_TS(E, tile.type, tile.GetOffset_TS()).Finalise();

            } else
                Assert.Never("Unknown placemarker object");

            HidePossibleTilePlacements();

            // Run movement rules to take into account the new object
            ((TurnBasedExecutionManager) E.ExecutionManager).ExecuteMovementRules(delegate () {

                // Return to the game
                E.GetManager<TurnManager>().TurnStateMachine.IssueCommand(TurnEvent.Wait_For_Input);

            });

        }

        public virtual void ShowPossibleTilePlacements(IBoardObject placingObject) {

            this.placingObject = placingObject;

            // Find all possible positions
            List<Vector3> possiblePositions = new List<Vector3>();
            foreach (Tile tile in TileRegistry.AllObjectsEnumerable_TS()) { 
                Vector3 tileOffset = tile.GetOffset_TS();

                foreach (Vector3 positionOffset in tile.type.PositionTypes.Keys) {

                    TileType.PositionProfile profile = tile.type.PositionTypes[positionOffset];

                    foreach (Vector3 possibleNeighbourOffset in profile.Neighbours) {
                        possiblePositions.Add(tileOffset + possibleNeighbourOffset);
                    }

                    foreach (Vector3 possibleNeighbourOffset in profile.Displacements) {
                        possiblePositions.Add(tileOffset + possibleNeighbourOffset);
                    }

                }

            }

            // Add placemarker positions
            PlacemarkerPositions = new List<Position>();
            Position tmpPos;
            foreach (Vector3 offset in possiblePositions) {

                // Select only offsets that don't already have a position
                if (E.GetManager<BoardManager>().GetPosition_TS(offset) == null) {

                    new PositionCreator_TS(E, BoardController.PositionTypePlacemarker, offset)
                        .SetAsPlacemarker(placingObject)
                        .Finalise(out tmpPos);

                    E.EffectFactory.EnqueueNewEffect<IAddPositionEffect>(tmpPos);

                    PlacemarkerPositions.Add(tmpPos);

                }

            }
            
        }

        public virtual void HidePossibleTilePlacements() {

            GameObject.Destroy(placingObject.GetGameObject());
            placingObject = null;

            // Destroy all the placemarker positions
            foreach (Position position in PlacemarkerPositions) {
                GameObject.Destroy(position.GetGameObject());
            }

        }

    }

}