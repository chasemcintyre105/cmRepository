using UnityEngine;
using UnityEngine.UI;

namespace RuleEngineAddons.RulePanel {

    public class JudgeNotificationAttachment : MonoBehaviour {

		private Image _Image;
		public Image Image {
            get {
                if (_Image == null)
                    _Image = GetComponent<Image>();

                return _Image;
            }
        }

		private bool _Available = true;

		public bool Available {
			get {
				return _Available;
			}
			set {
				_Available = value;
				if (value) {
					Image.color = new Color(0, 0, 0, 0.1f);
                } else {
					Image.color = new Color(1, 1, 1, 0);
				}
			}
		}

	}

}

