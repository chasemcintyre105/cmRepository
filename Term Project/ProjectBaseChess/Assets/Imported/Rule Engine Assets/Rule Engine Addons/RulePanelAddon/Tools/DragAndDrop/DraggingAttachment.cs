using RuleEngine;
using UnityEngine;
using UnityEngine.UI;

namespace RuleEngineAddons.RulePanel {

    public class DraggingAttachment : MonoBehaviour {

		public static readonly float transparencyMultiplier = 0.5f;

		public static DraggingAttachment StartDraggingObject(GameObject obj) {
			GameObject newObj = (GameObject) GameObject.Instantiate(obj);
			newObj.transform.SetParent(RuleEngineController.E.GetController<RulePanelController>().OrganisationalObjects.Canvas.transform);
			newObj.name = "Dragging object: " + obj.name;

			Assert.Null("Object does not already hold a DragginAttachment", newObj.GetComponent<DraggingAttachment>());
            
			DraggingAttachment attachment = newObj.AddComponent<DraggingAttachment>();
			
			// Make click events pass through this object so that they are handled normally
			CanvasGroup canvasGroup = newObj.AddComponent<CanvasGroup>();
			canvasGroup.blocksRaycasts = false;
			canvasGroup.interactable = false;
			
			// Make transparent
			Image imageComponent = newObj.GetComponent<Image>();
			if (imageComponent != null) {

				// Change transparency of gui element
				Color iColour = imageComponent.color;
				iColour.a = (float) transparencyMultiplier * iColour.a;
				imageComponent.color = iColour;

				Text textComponent = newObj.GetComponentInChildren<Text>();
				iColour = textComponent.color;
				iColour.a = (float) transparencyMultiplier * iColour.a;
				textComponent.color = iColour;
			} else {
				Material material = newObj.GetComponent<Material>();
				if (material != null) {
					// Change transparency of game object
					Color mColour = material.color;
					mColour.a = (float) transparencyMultiplier * mColour.a;
					material.color = mColour;
				}
			}

			return attachment;
		}

		void Update() {
			transform.position = Input.mousePosition;
		}

		public void StopDragging() {
			Destroy(gameObject);
		}

	}

}