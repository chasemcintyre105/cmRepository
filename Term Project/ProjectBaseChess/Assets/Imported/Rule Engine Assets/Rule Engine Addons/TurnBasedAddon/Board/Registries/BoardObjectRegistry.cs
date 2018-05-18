using RuleEngine;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

    public class BoardObjectRegistry<Obj, ObjType, ObjValue, ObjTypeValue> : SpecificObjectRegistry<Obj, ObjType, ObjValue, ObjTypeValue> where Obj : IBoardObject
                                                                                                                                          where ObjType : IBoardObjectType
                                                                                                                                          where ObjValue : ObjectValue
                                                                                                                                          where ObjTypeValue : ObjectTypeValue {
        
		protected Dictionary<Vector3, List<Obj>> ObjectsByOffset;

		protected bool OnlyOneAtEachOffset = false;

		public BoardObjectRegistry(Engine E, ObjectValueCreator NewObjectValue, ObjectTypeValueCreator NewObjectTypeValue, bool OnlyOneAtEachOffset) : base (E, NewObjectValue, NewObjectTypeValue) {
			this.OnlyOneAtEachOffset = OnlyOneAtEachOffset;

			ObjectsByOffset = new Dictionary<Vector3, List<Obj>>();
		}

        // Overrided object registration methods
        public override void RegisterObject_TS(Obj obj) {
            lock (_registryLock) {
                registerObject(obj);
                registerByOffset(obj);
            }
        }

        public override void UnregisterObject_TS(Obj obj) {
            lock (_registryLock) {
                unregisterObject(obj);
                unregisterByOffset(obj);
            }
        }

        protected void registerByOffset(Obj obj) {
            Vector3 offset = obj.GetOffset_TS();
            List<Obj> list;

            if (ObjectsByOffset.TryGetValue(offset, out list)) {
                if (OnlyOneAtEachOffset)
                    Assert.True("Tried to add to an offset that would violate OnlyOneAtEachOffset principle", list.Count == 0);
                else
                    Assert.True("Lookup does not yet have the " + ObjectName, !list.Contains(obj));
            } else {
                list = new List<Obj>();
                ObjectsByOffset.Add(offset, list);
            }

            list.Add(obj);
        }

        protected void unregisterByOffset(Obj obj) {
            Vector3 offset = obj.GetOffset_TS();

            Assert.True("Lookup does not yet have the " + ObjectName, ObjectsByOffset.ContainsKey(offset));
            ObjectsByOffset[offset].Remove(obj);
        }

        // By offset methods
        public List<Obj> GetAllAtOffset_TS(Vector3 offset) {
			lock (_registryLock) {
				Assert.True("Lookup contains a list at offset " + offset.ToString(), ObjectsByOffset.ContainsKey(offset));
				return ObjectsByOffset[offset];
			}
		}

		public Obj GetOnlyAtOffset_TS(Vector3 offset) {
			lock (_registryLock) {
				Assert.True("OnlyOneAtEachOffset is set", OnlyOneAtEachOffset);
				Assert.True("Lookup contains a list at offset " + offset.ToString(), ObjectsByOffset.ContainsKey(offset));

				List<Obj> list = ObjectsByOffset[offset];
				if (list.Count == 0)
					return default(Obj);
				else if (list.Count == 1)
					return list[0];
				else
					Assert.Never("OnlyOneAtEachOffset declared in registry but LookupByOffset[" + offset + "] has " + list.Count + " entries.");

				return default(Obj);
			}
		}

		public Obj GetClosestAtOffsetWithinTolerance_TS(Vector3 offset, float sqrTolerance) {
			lock (_registryLock) {
                List<Obj> objs;

                // If there is at least one at the exact offset, use the first at the offset
                if (ObjectsByOffset.ContainsKey(offset)) {
                    objs = ObjectsByOffset[offset];
                    if (objs.Count > 0)
                        return objs[0];
                }

                objs = GetAllAtOffsetWithinTolerance(offset, sqrTolerance);

				if (objs.Count == 0)
					return default(Obj);
				else if (objs.Count == 1)
					return objs[0];
				else {
					float distance = sqrTolerance;
					Obj closestSoFar = default(Obj);

					foreach (Obj obj in objs) {
						float latestDistance = (obj.GetOffset_TS() - offset).sqrMagnitude;
						if (latestDistance < distance) {
							distance = latestDistance;
							closestSoFar = obj;
						}
					}

					Assert.NotNull("Closest object was found", closestSoFar);

					return closestSoFar;
				}
			}
		}

		public List<Obj> GetAllAtOffsetWithinTolerance_TS(Vector3 offset, float sqrTolerance) {
			lock (_registryLock) {
				return GetAllAtOffsetWithinTolerance(offset, sqrTolerance);
			}
		}
		
		private List<Obj> GetAllAtOffsetWithinTolerance(Vector3 offset, float sqrTolerance) {
			List<Obj> objs = new List<Obj>();
			
			// Search through the positions to find something close by within the tolerance distance
			foreach (Vector3 key in ObjectsByOffset.Keys) {
				if ((key - offset).sqrMagnitude < sqrTolerance) {
					objs.AddRange(ObjectsByOffset[key]);
				}
			}
			
			return objs;
		}

		public int GetCountAtOffset_TS(Vector3 offset) {
			lock (_registryLock) {
				Assert.True("Lookup contains a list at offset " + offset.ToString(), ObjectsByOffset.ContainsKey(offset));
				return ObjectsByOffset[offset].Count;
			}
		}
		
		public bool IsOffsetRegistered_TS(Vector3 v) {
			lock (_registryLock) {
				return ObjectsByOffset.ContainsKey(v);
			}
		}
		
		public bool IsOffsetRegisteredWithinTolerance_TS(Vector3 offset, float sqrTolerance) {
			lock (_registryLock) {
				List<Obj> objs = GetAllAtOffsetWithinTolerance(offset, sqrTolerance);
				return objs.Count > 0;
			}
		}

	}

}
