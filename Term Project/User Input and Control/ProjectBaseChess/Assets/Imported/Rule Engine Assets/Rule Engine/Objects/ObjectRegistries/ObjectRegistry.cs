using System;
using System.Collections.Generic;

namespace RuleEngine {

    public abstract class ObjectRegistry {

        public abstract Type GetRegistryObjectType();

        public SpecificObjectRegistry<Obj, ObjType, ObjValue, ObjTypeValue> Specify<Obj, ObjType, ObjValue, ObjTypeValue>() where Obj : IObject
                                                                                                                            where ObjType : IObjectType
                                                                                                                            where ObjValue : ObjectValue
                                                                                                                            where ObjTypeValue : ObjectTypeValue {
            return (SpecificObjectRegistry<Obj, ObjType, ObjValue, ObjTypeValue>) this;
        }

        public abstract List<ObjectValue> GenerateListOfNewBaseObjectValuesByTypeID(string typeid);

    }

    public class SpecificObjectRegistry<Obj, ObjType, ObjValue, ObjTypeValue> : ObjectRegistry where Obj : IObject 
                                                                                               where ObjType : IObjectType
                                                                                               where ObjValue : ObjectValue
                                                                                               where ObjTypeValue : ObjectTypeValue {

		protected readonly object _registryLock;

        private Engine E;
        private int objectIDTicker = 0;

        // Types
        protected Dictionary<string, ObjType> ObjectTypesByID;

        // Type values
        protected Dictionary<string, ObjTypeValue> ObjectTypeValuesByID;

        // Instances
        protected Dictionary<int, Obj> ObjectsByID;
        protected Dictionary<string, List<Obj>> ObjectsByTypeID;

        // Instance values
        protected Dictionary<int, ObjValue> ObjectValuesByID;
        protected Dictionary<string, List<ObjValue>> ObjectValuesByTypeID;

        public readonly string ObjectName;

        protected ObjectValueCreator NewObjectValue;
        protected ObjectTypeValueCreator NewObjectTypeValue;

        public delegate ObjValue ObjectValueCreator(Engine E, Obj o, ObjTypeValue otv);
        public delegate ObjTypeValue ObjectTypeValueCreator(Engine E, ObjType ot);

        public SpecificObjectRegistry(Engine E, ObjectValueCreator NewObjectValue, ObjectTypeValueCreator NewObjectTypeValue) {
			_registryLock = new object();

            this.E = E;

            ObjectsByID = new Dictionary<int, Obj>();
            ObjectsByTypeID = new Dictionary<string, List<Obj>>();
            ObjectValuesByID = new Dictionary<int, ObjValue>();
            ObjectValuesByTypeID = new Dictionary<string, List<ObjValue>>();
            ObjectTypesByID = new Dictionary<string, ObjType>();
            ObjectTypeValuesByID = new Dictionary<string, ObjTypeValue>();

			ObjectName = typeof(Obj).Name;
            this.NewObjectValue = NewObjectValue;
            this.NewObjectTypeValue = NewObjectTypeValue;

        }

        public override Type GetRegistryObjectType() {
            return typeof(Obj);
        }


        // Object registration
        public virtual void RegisterObject_TS(Obj obj) {
			lock (_registryLock) {
                registerObject(obj);
            }
        }

        public virtual void UnregisterObject_TS(Obj obj) {
            lock (_registryLock) {
                unregisterObject(obj);
            }
        }

        protected void registerObject(Obj obj) {
            int id = obj.GetID();

            if (ObjectsByID.ContainsKey(id))
                throw new Exception(ObjectName + " is already registered: " + id + " (type: " + obj.GetObjectType().GetID() + ")");

            ObjectsByID.Add(id, obj);

            // Register by type
            string typeid = obj.GetObjectType().GetID();
            List<Obj> objectList = null;

            // Make sure there is a list for this type
            if (!ObjectsByTypeID.TryGetValue(typeid, out objectList)) {
                objectList = new List<Obj>();
                ObjectsByTypeID.Add(typeid, objectList);
            }

            objectList.Add(obj);

            // Register a new object value
            if (!ObjectTypeValuesByID.ContainsKey(typeid))
                throw new Exception(ObjectName + " type not registered: " + typeid);

            ObjValue objectValue = NewObjectValue(E, obj, ObjectTypeValuesByID[obj.GetObjectType().GetID()]);

            ObjectValuesByID.Add(id, objectValue);

            // And by type
            List<ObjValue> valueList = null;
            if (!ObjectValuesByTypeID.TryGetValue(typeid, out valueList)) {
                valueList = new List<ObjValue>();
                ObjectValuesByTypeID.Add(typeid, valueList);
            }

            valueList.Add(objectValue);
		}

        protected void unregisterObject(Obj obj) {
            int id = obj.GetID();

            if (!ObjectsByID.ContainsKey(id))
                throw new Exception(ObjectName + " is already not registered: " + id + " (type: " + obj.GetObjectType().GetID() + ")");

            ObjectsByID.Remove(id);

            // Unregister by type
            string typeid = obj.GetObjectType().GetID();
            List<Obj> objectList = null;

            if (!ObjectsByTypeID.TryGetValue(typeid, out objectList))
                throw new Exception(ObjectName + " was not correctly registered by type: " + id + ", " + typeid);

            objectList.Remove(obj);

            // Unregister object value
            ObjValue objectValue = null;
            if (!ObjectValuesByID.TryGetValue(id, out objectValue))
                throw new Exception(ObjectName + " value as not correctly registered for object: " + id);

            ObjectValuesByID.Remove(id);

            // And by type
            List<ObjValue> valueList = null;
            if (!ObjectValuesByTypeID.TryGetValue(typeid, out valueList))
                throw new Exception(ObjectName + " type was not registered for ObjectValues: " + typeid);

            valueList.Remove(objectValue);
        }

        public virtual bool IsRegistered_TS(Obj obj) {
			lock (_registryLock) {
				return ObjectsByID.ContainsKey(obj.GetID());
			}
		}


        // Type registration
        public virtual void RegisterObjectType_TS(ObjType type) {
            lock (_registryLock) {
                registerType(type);
            }
        }

        protected void registerType(ObjType type) {
            string typeid = type.GetID();

            if (ObjectTypesByID.ContainsKey(typeid))
                throw new Exception(ObjectName + " type already registered: " + typeid);

            ObjectTypesByID.Add(typeid, type);
			ObjectsByTypeID.Add(typeid, new List<Obj>());
            ObjectValuesByTypeID.Add(typeid, new List<ObjValue>());

            // Register the corresponding type object value
            ObjectTypeValuesByID.Add(typeid, NewObjectTypeValue(E, type));

        }


        // Retrieval methods
        public virtual int GenerateObjectUID_TS() {
            lock (_registryLock) {
                return objectIDTicker++;
            }
        }

        public virtual Obj GetObjectByID_TS(int id) {
            lock (_registryLock) {
                if (!ObjectsByID.ContainsKey(id))
                    throw new Exception(ObjectName + " not registered: " + id);

                return ObjectsByID[id];
            }
        }

        public virtual ObjType GetObjectTypeByID_TS(string typeid) {
            lock (_registryLock) {
                if (!ObjectTypesByID.ContainsKey(typeid))
                    throw new Exception(ObjectName + " type not registered: " + typeid);

                return ObjectTypesByID[typeid];
            }
        }

        public virtual ObjTypeValue GetObjectTypeValueByID_TS(string typeid) {
            lock (_registryLock) {
                if (!ObjectTypeValuesByID.ContainsKey(typeid))
                    throw new Exception(ObjectName + " type value not register: " + typeid);

                return ObjectTypeValuesByID[typeid];
            }
        }

        public virtual int GetObjectCount_TS() {
            lock (_registryLock) {
                return ObjectsByID.Values.Count;
            }
        }

        public virtual int GetObjectTypeCount_TS() {
            lock (_registryLock) {
                return ObjectTypesByID.Values.Count;
            }
        }

        public virtual List<Obj> GenerateNewListOfAllObjects_TS() {
            lock (_registryLock) {
                return new List<Obj>(ObjectsByID.Values);
            }
        }

        public virtual List<Obj> GenerateNewListOfAllObjectsByType_TS(ObjType type) {
            return GenerateNewListOfAllObjectsByTypeID_TS(type.GetID());
        }

        public virtual List<Obj> GenerateNewListOfAllObjectsByTypeID_TS(string typeid) {
            lock (_registryLock) {
                List<Obj> list = null;

                if (!ObjectsByTypeID.TryGetValue(typeid, out list))
                    throw new Exception(ObjectName + " is not registered by type: " + typeid);

                return new List<Obj>(list);
            }
        }

        public virtual List<ObjType> GenerateNewListOfAllObjectTypes_TS() {
            lock (_registryLock) {
                return new List<ObjType>(ObjectTypesByID.Values);
            }
        }


        // Instantiation methods
        public virtual ObjValue CreateObjectValueByID_TS(int id) {
            lock (_registryLock) {
                if (!ObjectsByID.ContainsKey(id))
                    throw new Exception(ObjectName + " not registered: " + id);

                return CreateObjectValue_TS(ObjectsByID[id]);
            }
        }

        public virtual ObjValue CreateObjectValue_TS(Obj obj) {
            lock (_registryLock) {
                string typeid = obj.GetObjectType().GetID();

                if (!ObjectTypeValuesByID.ContainsKey(typeid))
                    throw new Exception(ObjectName + " type not registered: " + typeid);

                return NewObjectValue(E, obj, ObjectTypeValuesByID[obj.GetObjectType().GetID()]);
            }
        }


        // Iteration of objects
        public virtual IEnumerable<Obj> AllObjectsEnumerable_TS() {
            lock (_registryLock) {
                foreach (Obj o in ObjectsByID.Values)
                    yield return o;
            }
        }

        public virtual IEnumerable<Obj> AllObjectsByTypeEnumerable_TS(ObjType type) {
            lock (_registryLock) {
                string typeid = type.GetID();
                List<Obj> list = null;

                if (!ObjectsByTypeID.TryGetValue(typeid, out list))
                    throw new Exception(ObjectName + " is not registered by type: " + typeid);

                foreach (Obj o in list)
                    yield return o;
            }
        }

        public virtual IEnumerable<Obj> AllObjectsByTypeIDEnumerable_TS(string typeid) {
            lock (_registryLock) {
                List<Obj> list = null;

                if (!ObjectsByTypeID.TryGetValue(typeid, out list))
                    throw new Exception(ObjectName + " is not registered by type: " + typeid);

                foreach (Obj o in list)
                    yield return o;
            }
        }

        // Iteration of object types
        public virtual IEnumerable<ObjType> AllObjectTypesEnumerable_TS() {
            lock (_registryLock) {
                foreach (ObjType ot in ObjectTypesByID.Values)
                    yield return ot;
            }
        }


        // Iteration of object values
        public virtual IEnumerable<ObjValue> AllObjectValuesEnumerable_TS() {
            lock (_registryLock) {
                foreach (ObjValue ot in ObjectValuesByID.Values)
                    yield return ot;
            }
        }

        public virtual IEnumerable<ObjValue> AllObjectValuesByTypeEnumerable_TS(ObjType type) {
            lock (_registryLock) {
                string typeid = type.GetID();
                List<ObjValue> list = null;

                if (!ObjectValuesByTypeID.TryGetValue(typeid, out list))
                    throw new Exception(ObjectName + " value is not registered by type: " + typeid);

                foreach (ObjValue ov in list)
                    yield return ov;
            }
        }

        public virtual IEnumerable<ObjValue> AllObjectValuesByTypeIDEnumerable_TS(string typeid) {
            lock (_registryLock) {
                List<ObjValue> list = null;

                if (!ObjectValuesByTypeID.TryGetValue(typeid, out list))
                    throw new Exception(ObjectName + " value is not registered by type: " + typeid);

                foreach (ObjValue ov in list)
                    yield return ov;
            }
        }


        // Iteration of object type values
        public virtual IEnumerable<ObjTypeValue> AllObjectTypeValuesEnumerable_TS() {
            lock (_registryLock) {
                foreach (ObjTypeValue otv in ObjectTypeValuesByID.Values)
                    yield return otv;
            }
        }


        // Generation of object values
        public virtual List<ObjectValue> GenerateListOfNewBaseObjectValuesByType(ObjType type) {
            return GenerateListOfNewBaseObjectValuesByTypeID(type.GetID());
        }

        public override List<ObjectValue> GenerateListOfNewBaseObjectValuesByTypeID(string typeid) {
            List<ObjectValue> list = new List<ObjectValue>();

            if (!ObjectsByTypeID.ContainsKey(typeid))
                throw new Exception(ObjectName + " type is not registered: " + typeid);

            foreach (Obj obj in ObjectsByTypeID[typeid])
                list.Add(NewObjectValue(E, obj, ObjectTypeValuesByID[obj.GetObjectType().GetID()]));

            return list;
        }

        public virtual List<ObjValue> GenerateListOfNewObjectValuesByType(ObjType type) {
            return GenerateListOfNewObjectValuesByTypeID(type.GetID());
        }

        public virtual List<ObjValue> GenerateListOfNewObjectValuesByTypeID(string typeid) {
            List<ObjValue> list = new List<ObjValue>();

            if (!ObjectsByTypeID.ContainsKey(typeid))
                throw new Exception(ObjectName + " type is not registered: " + typeid);

            foreach (Obj obj in ObjectsByTypeID[typeid])
                list.Add(NewObjectValue(E, obj, ObjectTypeValuesByID[obj.GetObjectType().GetID()]));

            return list;
        }

        public virtual List<ObjectValue> GenerateListOfNewBaseObjectValues() {
            List<ObjectValue> list = new List<ObjectValue>();

            foreach (Obj obj in ObjectsByID.Values)
                list.Add(NewObjectValue(E, obj, ObjectTypeValuesByID[obj.GetObjectType().GetID()]));

            return list;
        }

        public virtual List<ObjValue> GenerateListOfNewObjectValues() {
            List<ObjValue> list = new List<ObjValue>();

            foreach (Obj obj in ObjectsByID.Values)
                list.Add(NewObjectValue(E, obj, ObjectTypeValuesByID[obj.GetObjectType().GetID()]));

            return list;
        }


    }

}
