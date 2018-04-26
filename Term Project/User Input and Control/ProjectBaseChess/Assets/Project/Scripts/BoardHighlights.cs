using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Project
{
	public class BoardHighlights : MonoBehaviour //Selection Highlighter
	{
		public static BoardHighlights Instance{ set; get; }

		public GameObject highlightPrefab;
		private List<GameObject> highlights;

		private void Start ()
		{
			Instance = this;
			highlights = new List <GameObject> ();
		}

		private GameObject GetHighlightObject ()
		{
			GameObject go = highlights.Find (g => !g.activeSelf);

			if (go == null) {
				go = Instantiate (highlightPrefab);
				highlights.Add (go);
			}

			return go;
		}

		public void HighlightAllowedMoves (bool[,] moves) //Multi Dimensional array with booleans, will highlight allowed moves
		{
			for (int i = 0; i < 8; i++) {
				for (int j = 0; j < 8; j++) {
					if (moves [i, j]) {
						GameObject go = GetHighlightObject ();
						go.SetActive (true);
						go.transform.position = new Vector3 (i + 0.5f, 0, j + 0.5f); // highlight prefab must be size of 1 tile. Tile = 1 meter, so 0.5f
					}
				}
			}
		}

		public void HideHighlights () //responsible for turning off highlights if not needed
		{
			foreach (GameObject go in highlights)
				go.SetActive (false);
		}
	}
}