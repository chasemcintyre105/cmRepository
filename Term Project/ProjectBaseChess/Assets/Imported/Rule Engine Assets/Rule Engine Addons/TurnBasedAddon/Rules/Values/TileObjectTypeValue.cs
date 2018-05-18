using System;
using System.Collections.Generic;
using UnityEngine;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

	public class TileObjectTypeValue : ObjectTypeValue {

        private TileObjectRegistry _TileObjectRegistry;
        protected TileObjectRegistry TileObjectRegistry
        {
            get
            {
                if (_TileObjectRegistry == null)
                    _TileObjectRegistry = E.ObjectRegistries[typeof(Tile)] as TileObjectRegistry;

                return _TileObjectRegistry;
            }
        }
        
		public override string GetSelectionPanelCategory() {
			return "Tile";
		}
        
		public override bool IsEqualTo (Value value) {
			if (value.GetType() != GetType())
				return false;

            TileObjectTypeValue otherObject = (TileObjectTypeValue) value;

			return TileType.ID.Equals(otherObject.TileType.ID);
		}

		public TileObjectTypeValue(Engine E, TileType TileType) : base(E, TileType) {
			this.TileType = TileType;
			ApplicableTemplates = new List<GameObject>();
		}

		public TileObjectTypeValue(Engine E, GameObject Template, TileType TileType) : base(E, TileType) {
			this.TemplateRepresentative = Template;
			this.TileType = TileType;
			ApplicableTemplates = new List<GameObject>();
			ApplicableTemplates.Add(Template);
		}

		public TileObjectTypeValue(Engine E, GameObject Template, TileType TileType, List<GameObject> ApplicableTemplates) : base(E, TileType) {
			this.TemplateRepresentative = Template;
			this.ApplicableTemplates = ApplicableTemplates;
			this.TileType = TileType;
		}
		 
		public readonly TileType TileType;
		public GameObject TemplateRepresentative;
		public readonly List<GameObject> ApplicableTemplates;

        public override List<ObjectValue> GetAllInstances() {
            //List<ObjectValue> list = new List<ObjectValue>();
            //foreach (Tile tile in E.GetManager<BoardManager>().GetAllTiles_TS()) {
            //    if (tile.type.Equals(TileType))
            //        list.Add((ObjectValue) new TileObjectValue(E, tile));
            //}
            //return list;
            return TileObjectRegistry.GenerateListOfNewBaseObjectValuesByType((TileType) TypeInstance);
        }

        public override string GetDescription() {
            return "The type of a tile. In this case " + TileType;
		}

        public override string ToString() {
            return "TileObjectTypeValue: " + TileType.GetID();
        }

    }

}