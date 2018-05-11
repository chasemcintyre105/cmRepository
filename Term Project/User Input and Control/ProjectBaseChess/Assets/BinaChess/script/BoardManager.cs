using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BinaChess
{
	public class BoardManager : MonoBehaviour
	{
		public static BoardManager Instance{ set; get; }

		private bool[,] allowedMoves{ set; get; }

		public Chessman[,] Chessmans{ set; get; }

		private Chessman selectedChessman;



		private const float TILE_SIZE = 1.0f;
		private const float TILE_OFFSET = 0.5f;

		private int selectionX = -1;
		private int selectionY = -1;

		public List<GameObject> chessmanPrefabs;
		private List<GameObject> activeChessman;




		public int[] EnPassantMove{ set; get; }

		private Quaternion orientation = Quaternion.Euler (0, 90, 0);

		public bool isWhiteTurn = true;

		public GameObject ButtonPanels;
		public Button QueenButton;
		public Button RookButton;
		public Button BishopButton;
		public Button KnightButton;

		public GameObject EndGamePanel;

		public Text winText;

		private void Start ()
		{
			Instance = this;
			SpawnAllChessmans ();
			ButtonPanels = GameObject.Find("ButtonPanel");
			EndGamePanel = GameObject.Find ("EndGamePanel");
			EndGamePanel.SetActive (false);
			ButtonPanels.SetActive(false);

			winText.text = "";
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
					if (y == 7) {
						activeChessman.Remove (selectedChessman.gameObject);
						Destroy (selectedChessman.gameObject);
						ButtonPanels.SetActive (true);
						Button QWbtn = QueenButton.GetComponent<Button> ();
						QWbtn.onClick.AddListener(delegate{pawntoQueenWhite(x, y);});
						Button RWbtn = RookButton.GetComponent<Button> ();
						RWbtn.onClick.AddListener(delegate{pawntoRookWhite(x, y);});
						Button BWbtn = BishopButton.GetComponent<Button> ();
						BWbtn.onClick.AddListener(delegate{pawntoBishopWhite(x, y);});
						Button KWbtn = KnightButton.GetComponent<Button> ();
						KWbtn.onClick.AddListener(delegate{pawntoKnightWhite(x, y);});


						selectedChessman = Chessmans [x, y];

					} else if (y == 0) {
						activeChessman.Remove (selectedChessman.gameObject);
						Destroy (selectedChessman.gameObject);
						ButtonPanels.SetActive (true);
						Button QBbtn = QueenButton.GetComponent<Button> ();
						QBbtn.onClick.AddListener(delegate{pawntoQueenBlack(x, y);});
						Button RBbtn = RookButton.GetComponent<Button> ();
						RBbtn.onClick.AddListener(delegate{pawntoRookBlack(x, y);});
						Button BBbtn = BishopButton.GetComponent<Button> ();
						BBbtn.onClick.AddListener(delegate{pawntoBishopBlack(x, y);});
						Button KBbtn = KnightButton.GetComponent<Button> ();
						KBbtn.onClick.AddListener(delegate{pawntoKnightBlack(x, y);});


						selectedChessman = Chessmans [x, y];
					}

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

		private void UpdateSelection ()
		{
			if (!Camera.main)
				return;

			RaycastHit hit;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, LayerMask.GetMask ("ChessPlane"))) {
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

		private void SpawnAllChessmans ()
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
			origin.x += (TILE_SIZE * x) + TILE_OFFSET;
			origin.z += (TILE_SIZE * y) + TILE_OFFSET;
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

		//White
		void pawntoQueenWhite(int x, int y){

			Debug.Log ("Queen button is clicked");
			SpawnChessman (1, x, y);
			ButtonPanels.SetActive (false);
		}
		void pawntoRookWhite(int x, int y){

			Debug.Log ("Rook button is clicked");
			SpawnChessman (2, x, y);
			ButtonPanels.SetActive (false);
		}
		void pawntoBishopWhite(int x, int y){

			Debug.Log ("Bishop button is clicked");
			SpawnChessman (3, x, y);
			ButtonPanels.SetActive (false);
		}
		void pawntoKnightWhite(int x, int y){

			Debug.Log ("Knight button is clicked");
			SpawnChessman (4, x, y);
			ButtonPanels.SetActive (false);
		}

		//Black
		void pawntoQueenBlack(int x, int y){

			Debug.Log ("Queen button is clicked");
			SpawnChessman (7, x, y);
			ButtonPanels.SetActive (false);
		}
		void pawntoRookBlack(int x, int y){

			Debug.Log ("Rook button is clicked");
			SpawnChessman (8, x, y);
			ButtonPanels.SetActive (false);
		}
		void pawntoBishopBlack(int x, int y){

			Debug.Log ("Bishop button is clicked");
			SpawnChessman (9, x, y);
			ButtonPanels.SetActive (false);
		}
		void pawntoKnightBlack(int x, int y){

			Debug.Log ("Knight button is clicked");
			SpawnChessman (10, x, y);
			ButtonPanels.SetActive (false);
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

			EndGamePanel.SetActive (true);

			foreach (GameObject go in activeChessman)
				Destroy (go);

			isWhiteTurn = true;
			BoardHighlights.Instance.HideHighlights ();
			SpawnAllChessmans ();
		}
	}
}