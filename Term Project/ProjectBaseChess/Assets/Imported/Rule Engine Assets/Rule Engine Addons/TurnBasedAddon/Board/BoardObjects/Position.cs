using RuleEngine;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

    public class Position : IBoardObject {

		private readonly object _positionLock;

        public readonly int ID;
        public readonly PositionType type;

        public bool Placemarker;
        public IBoardObject placingObject;

        private UnitObjectRegistry UnitRegistry;
        private Vector3 offset;
        private GameObject gameObject;
        private IBoardObjectAttachment Attachment;
		private LinkedList<Unit> occupyingUnits;
        private Dictionary<Vector3, Tile> adjacentTiles;

		public Position(int ID, UnitObjectRegistry UnitRegistry, PositionType type, Vector3 relativePositionOffsetFromTile, Tile associatedTile) {
            this.UnitRegistry = UnitRegistry;
            this.type = type;
			this.offset = associatedTile.GetOffset_TS() + relativePositionOffsetFromTile;
            this.ID = ID;

            _positionLock = new object();
			
			occupyingUnits = new LinkedList<Unit>();
			adjacentTiles = new Dictionary<Vector3, Tile>();
            Placemarker = false;

            adjacentTiles.Add(-relativePositionOffsetFromTile, associatedTile);

		}

        public Position(int ID, UnitObjectRegistry UnitRegistry, PositionType type, Vector3 absoluteOffset) {
            this.UnitRegistry = UnitRegistry;
            this.type = type;
            this.offset = absoluteOffset;
            this.ID = ID;

            _positionLock = new object();

            occupyingUnits = new LinkedList<Unit>();
            adjacentTiles = new Dictionary<Vector3, Tile>();
            Placemarker = false;

        }

        public int GetID() {
            return ID;
        }

        public IObjectType GetObjectType() {
            return type;
        }

        // Get a list of all possible absolute offsets where a neighbouring position could lie from this position
        public List<Vector3> GetAbsoluteNeighbourOffsets() {
            List<Vector3> neighbours = new List<Vector3>();

            // Add neighbours as defined by the type of the tiles this position is adjacent to
            foreach (Tile tile in adjacentTiles.Values) {
                foreach (Vector3 v in tile.type.PositionTypes.Keys) {
                    if (tile.GetOffset_TS() + v == offset) {

                        // Add neighbours
                        foreach (Vector3 neighbour in tile.type.PositionTypes[v].Neighbours) {
                            neighbours.Add(tile.GetOffset_TS() + neighbour);
                        }

                        // Add displaced neighbours
                        foreach (Vector3 displacedNeighbour in tile.type.PositionTypes[v].Displacements) {
                            neighbours.Add(tile.GetOffset_TS() + displacedNeighbour);
                        }

                        break;
                    }
                }
            }
            
            return neighbours;
        }

        public void AddAdjacentTile_TS(Tile tile) {
            lock (_positionLock) {
                Assert.True("Does not already have adjacent tile", !adjacentTiles.ContainsKey(tile.GetOffset_TS() - offset));
                adjacentTiles.Add(tile.GetOffset_TS() - offset, tile);
            }
        }

        // Leaving out for now
        //public void AddDisplacment_TS(Vector3 v) {
        //    lock (_positionLock) {
        //        displacedPositions.Add(v);
        //    }
        //}

        //public bool HasDisplacement_TS(Vector3 v) {
        //    lock (_positionLock) {
        //        return displacedPositions.Contains(v);
        //    }
        //}

        public Dictionary<Vector3, Tile> GetAllAdjacentTiles_TS() {
            lock (_positionLock) {
                return adjacentTiles;
            }
        }

        public bool TryGetAdjacentTileAtSameOffset(out Tile tile) {
            lock (_positionLock) {
                foreach (Vector3 v in adjacentTiles.Keys) {
                    if (v == Vector3.zero) {
                        tile = adjacentTiles[v];
                        return true;
                    }
                }
                tile = null;
                return false;
            } 
        }

        //public List<Vector3> GetAllDisplacement_TS() {
        //    lock (_positionLock) {
        //        return displacedPositions;
        //    }
        //}

        public LinkedListNode<Unit> FindNodeBeforeUnit_TS(Unit unit) {
			lock (_positionLock) {
				return occupyingUnits.Find(unit).Previous;
			}
		}
		
		public void InsertNodeAfterUnit_TS(Unit unit, LinkedListNode<Unit> nodeBefore) {
			lock (_positionLock) {
				if (nodeBefore == null) {
					occupyingUnits.AddFirst(unit);
				} else {
					occupyingUnits.AddAfter(nodeBefore, unit);
				}

                UnitRegistry.RegisterObject_TS(unit);
            }
		}

		public bool HasUnit_TS(Unit unit) {
			lock (_positionLock) {
				return occupyingUnits.Contains(unit);
			}
		}

		public Unit GetFirstCollidingUnit_TS() {
			lock (_positionLock) {
				Assert.True("Has collision", occupyingUnits.Count > 1);
				return occupyingUnits.First.Next.Value;
			}
		}

		public bool HasUnitCollisions_TS() {
			lock (_positionLock) {
				return occupyingUnits.Count > 1;
			}
		}

		public Unit GetUnit_TS() {
			lock (_positionLock) {
				if (occupyingUnits.First != null)
					return occupyingUnits.First.Value;
				else
					return null;
			}
		}

		public Unit GetLastUnit_TS() {
			lock (_positionLock) {
				return occupyingUnits.Last.Value;
			}
		}

		public void RemoveUnit_TS(Unit unit) {
			lock (_positionLock) {
				Assert.True("The unit is at this location", occupyingUnits.Contains(unit));
				occupyingUnits.Remove(unit);

                UnitRegistry.UnregisterObject_TS(unit);
            }
		}

		// Used to undo actions
		public void AddFirst_UndoAction_TS(Unit unit) {
			lock (_positionLock) {
				Assert.True("The unit is not already at this location", !occupyingUnits.Contains(unit));
				occupyingUnits.AddFirst(unit);

                UnitRegistry.RegisterObject_TS(unit);
            }
		}

		// Add unit normally and let collisions take place
		public void AddLast_TS(Unit unit) {
			lock (_positionLock) {
				Assert.True("The unit is not already at this location", !occupyingUnits.Contains(unit));
				occupyingUnits.AddLast(unit);
				unit.SetOffset_TS(offset);

                UnitRegistry.RegisterObject_TS(unit);
            }
		}

		// Remove unit from this location normally and let a colliding unit replace it
		public Unit RemoveFirst_TS() {
			lock (_positionLock) {
				Assert.NotNull("The first unit is not null", occupyingUnits.First);
				Unit removedUnit = occupyingUnits.First.Value;

				occupyingUnits.RemoveFirst();

                UnitRegistry.UnregisterObject_TS(removedUnit);

                return removedUnit;
			}
		}

		// Used to undo actions
		public Unit RemoveLast_UndoAction_TS() {
			lock (_positionLock) {
				Assert.NotNull("The last unit is not null", occupyingUnits.Last);
				Unit removedUnit = occupyingUnits.Last.Value;

				occupyingUnits.RemoveLast();

                UnitRegistry.UnregisterObject_TS(removedUnit);

                return removedUnit;
			}
		}

		// IBoardObject methods
		public IBoardObjectType GetBoardObjectType() {
			return type;
		}

		public void SetBoardObjectAttachment(IBoardObjectAttachment value) {
			Attachment = value;
		}

		public IBoardObjectAttachment GetBoardObjectAttachment() { 
			return Attachment;
		}

        public void SetGameObject(GameObject gameObject) {
            this.gameObject = gameObject;
        }

        public GameObject GetGameObject() { 
			return gameObject; 
		}
		
		public void SetOffset_TS(Vector3 position) {
			lock (_positionLock) {
				this.offset = position; 
			}
		}
		
		public Vector3 GetOffset_TS() { 
			lock (_positionLock) {
				return offset;
			}
		}


		public override string ToString() {
			return "Position (" + offset.x + ", " + offset.y + ")";
		}

        public override int GetHashCode() {
            return offset.GetHashCode();
        }

    }
}