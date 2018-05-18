using System;
using UnityEngine;
using UnityEngine.UI;

namespace RuleEngineAddons.RulePanel {

    public class ActionStackItemProfile {

        public enum StackOptionType {
            Label,
            Button
        }

        public string ID;
        public StackOptionType Type;
        public Action OnClick = null;
        public bool DestroyOnClick;
        public bool Permanent;

        private GameObject _Object;
        private Text _Text;
        public GameObject Object {
            get {
                return _Object;
            }
            set {
                _Object = value;
                _Text = _Object.GetComponent<Text>();
            }
        }

        private string _Title;
        public string Title {
            get {
                return _Title;
            }
            set {
                _Title = value;
                if (_Text != null)
                    _Text.text = _Title;
            }
        }

    }

}
