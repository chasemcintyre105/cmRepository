using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BinaChess
{
	public class BoardManager : MonoBehaviour
	{

		public static BoardManager Instance{ set; get; }

		private bool[,] allowedMoves{ set; get; }

		public Chessman [,] Chessmans{ set; get; }

		private Chessman selectedChessman;

		private const float tile_size = 1.0f;
		private const float tile_offset = 0.5f;

		private int selectionX = -1;
		private int selectionY = -1;

		public int[] EnPassantMove{ set; get; }

		public List<GameObject> chessmanPrefabs;
		private List<GameObject> activeChessman;

		private Canvas CanvasObject;

		private Quaternion orientation = Quaternion.Euler (0, 90, 0);

		public bool isWhiteTurn = true;

		public GameObject ButtonPanels;
		/*public Button QButton;
	public Button RButton;
	public Button BButton;
	public Button KButton;*/

		public GameObject EndGamePanel;

		public Text winText;

		private void Start ()
		{

			Instance = this;
			SpawnAllChessmans ();
			ButtonPanels = GameObject.Find ("ButtonPanel");
			EndGamePanel = GameObject.Find ("EndGamePanel");
			EndGamePanel.SetActive (false);
			ButtonPanels.SetActive (false);

			winText.text = "";

			/*
		QButton = GetComponent<Button> ();
		RButton = GetComponent<Button> ();
		BButton = GetComponent<Button> ();
		KButton = GetComponent<Button> ();*/

		}


		private void Update ()
		{

			UpdateSelection ();
			DrawChessBoard ();

			if (Input.GetMouseButtonDown (0)) {
				if (selectionX >= 0 && selectionY >= 0) {

					if (selectedChessman == null) {

						//select the chessman
						SelectChessman (selectionX, selectionY);
					} else {
					
						//move the chessman
						MoveChessman (selectionX, selectionY);

					}
				}
			}
		}


		private void SelectChessman (int x, int y)
		{



			if (Chessmans [x, y] == null) //no piece
			return;

			if (Chessmans [x, y].isWhite != isWhiteTurn)
				return;

			bool hasAtleastOneMove = false;
			allowedMoves = Chessmans [x, y].PossibleMove ();
			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
					if (allowedMoves [i, j])
						hasAtleastOneMove = true;

			if (!hasAtleastOneMove)
				return;
		
			selectedChessman = Chessmans [x, y];
			//selectedChessman.transform.position = new Vector3 (selectedChessman.CurrentX, selectedChessman.transform.position.y + 0.25f, selectedChessman.CurrentY);
			BoardHighlights.Instance.HighlightAllowedMoves (allowedMoves);
		}



		private void MoveChessman (int x, int y)
		{

			if (allowedMoves [x, y]) {

				Chessman c = Chessmans [x, y];

				if (c != null && c.isWhite != isWhiteTurn) {

					//Capture a Piece

					//if it is the king 
					if (c.GetType () == typeof(King)) {
						//End the game
						EndGame ();
						return;
					}
					activeChessman.Remove (c.gameObject);
					Destroy (c.gameObject);
				}
				if (x == EnPassantMove [0] && y == EnPassantMove [1]) {
					if (isWhiteTurn) 
					//white turn
					c = Chessmans [x, y - 1];
					else 
					//black turn
					c = Chessmans [x, y + 1];

					activeChessman.Remove (c.gameObject);
					Destroy (c.gameObject);

				}
				EnPassantMove [0] = -1;
				EnPassantMove [1] = -1;

				if (selectedChessman.GetType () == typeof(Pawn)) {
				
					ChooseChessman (x, y);



					if (selectedChessman.CurrentY == 1 && y == 3) {
						EnPassantMove [0] = x;
						EnPassantMove [1] = y - 1;
					} else if (selectedChessman.CurrentY == 6 && y == 4) {
						EnPassantMove [0] = x;
						EnPassantMove [1] = y + 1;
					}
				}

				Chessmans [selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
				selectedChessman.transform.position = GetTileCenter (x, y);
				selectedChessman.SetPosition (x, y);
				Chessmans [x, y] = selectedChessman;
				isWhiteTurn = !isWhiteTurn;
			}
			
			BoardHighlights.Instance.HideHighlights ();
			selectedChessman = null;
		}

		private void ChooseChessman (int x, int y)
		{
		
			if (y == 7) {
				activeChessman.Remove (selectedChessman.gameObject);
				Destroy (selectedChessman.gameObject);
				SpawnChessman (1, x, y);
				selectedChessman = Chessmans [x, y];
				ButtonPanels.SetActive (true);


			} else if (y == 0) {
				activeChessman.Remove (selectedChessman.gameObject);
				Destroy (selectedChessman.gameObject);
				SpawnChessman (7, x, y);
				selectedChessman = Chessmans [x, y];
				ButtonPanels.SetActive (true);
			}


		}


		private void UpdateSelection ()
		{


			if (!Camera.main)
				return;

			RaycastHit hit;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, LayerMask.GetMask ("chessplane"))) {

				selectionX = (int)hit.point.x;
				selectionY = (int)hit.point.z;
				Debug.Log (Input.mousePosition);
			} else {

				selectionX = -1;
				selectionY = -1;

			}
		}


		private void SpawnChessman (int index, int x, int y)
		{

			GameObject go = Instantiate (chessmanPrefabs [index], GetTileCenter (x, y), orientation) as GameObject;
			go.transform.SetParent (transform);
			Chessmans [x, y] = go.GetComponent<Chessman> ();
			Chessmans [x, y].SetPosition (x, y);
			activeChessman.Add (go);

		}


		public void SpawnAllChessmans ()
		{

			activeChessman = new List<GameObject> ();
			Chessmans = new Chessman[8, 8];
			EnPassantMove = new int[2]{ -1, -1 }; 
			//spawn the white team

			//king
			SpawnChessman (0, 3, 0);

			//queen
			SpawnChessman (1, 4, 0);

			//Rook
			SpawnChessman (2, 0, 0);
			SpawnChessman (2, 7, 0);

			//bishop
			SpawnChessman (3, 2, 0);
			SpawnChessman (3, 5, 0);

			//knight
			SpawnChessman (4, 1, 0);
			SpawnChessman (4, 6, 0);

			//pawn
			for (int i = 0; i < 8; i++) {
				SpawnChessman (5, i, 1);

			}
			//spawn the black team

			//king
			SpawnChessman (6, 4, 7);

			//queen
			SpawnChessman (7, 3, 7);

			//Rook
			SpawnChessman (8, 0, 7);
			SpawnChessman (8, 7, 7);

			//bishop
			SpawnChessman (9, 2, 7);
			SpawnChessman (9, 5, 7);

			//knight
			SpawnChessman (10, 1, 7);
			SpawnChessman (10, 6, 7);

			//pawn
			for (int i = 0; i < 8; i++) {
				SpawnChessman (11, i, 6);
			}
		}


		


		private Vector3 GetTileCenter (int x, int y)
		{

			Vector3 origin = Vector3.zero;
			origin.x += (tile_size * x) + tile_offset;
			origin.z += (tile_size * y) + tile_offset;
			return origin;
		}

		private void DrawChessBoard ()
		{

			Vector3 widthLine = Vector3.right * 8;
			Vector3 heightLine = Vector3.forward * 8;

			for (int i = 0; i <= 8; i++) {

				Vector3 start = Vector3.forward * i;
				Debug.DrawLine (start, start + widthLine);

				for (int j = 0; j <= 8; j++) {

					start = Vector3.right * j;
					Debug.DrawLine (start, start + heightLine);
				
				}
			}

			//draw the selection

			if (selectionX >= 0 && selectionY >= 0) {

				Debug.DrawLine (
					Vector3.forward * selectionY + Vector3.right * selectionX,
					Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
				Debug.DrawLine (
					Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
					Vector3.forward * selectionY + Vector3.right * (selectionX + 1));	
		
			}
		}

		private void EndGame ()
		{


			if (isWhiteTurn) {
				Debug.Log ("White Team Wins!");
				winText.text = "White Team Wins!";
			} else {
				Debug.Log ("Black Team Wins!");
				winText.text = "Black Team Wins!";
			}

			foreach (GameObject go in activeChessman)
				Destroy (go);
		
			EndGamePanel.SetActive (true);
			isWhiteTurn = true;
			BoardHighlights.Instance.HideHighlights ();
			SpawnAllChessmans ();
		}



	}
}

