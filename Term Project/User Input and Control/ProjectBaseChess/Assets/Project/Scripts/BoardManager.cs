using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Project
{
	public class BoardManager : MonoBehaviour
	{
		public static BoardManager Instance{ set; get; }
		//will give the pieces the information on when a specific move to a tile is legal or not
		private bool[,] allowedMoves{ set; get; }

		public Chessman[,] Chessmans{ set; get; }

		private Chessman selectedChessman;

		public List<GameObject> highlights;

		private const float TILE_SIZE = 1.0f;		//tile dimensions
		private const float TILE_OFFSET = 0.5f;

		private int selectionX = -1;
		private int selectionY = -1;

		public List<GameObject> chessmanPrefabs;
		private List<GameObject> activeChessman;// = new List<GameObject> ();// new list not in Bina or tutorial

		private Material previousMat;	//not in Bina's
		public Material selectedMat;	//not in Bina's

		public int[] EnPassantMove{ set; get; }

		//		private Quaternion orientation = Quaternion.Euler (0, 90, 0);//Bina's Board
		//private Quaternion orientation = Quaternion.Euler (90, 0, 0);
		private Quaternion orientation = Quaternion.Euler (-90, 0, 0);

		public bool isWhiteTurn = true;

		public GameObject ButtonPanel;
		public Button QueenButton;
		public Button RookButton;
		public Button BishopButton;
		public Button KnightButton;

		public GameObject EndGamePanel;

		public Text winText;	

		private AudioSource audioSource;
		public AudioClip[] clips;
		private AudioClip clip;
		private AudioClip destroy;

		private void Start ()
		{
			Instance = this;
			SpawnAllChessmans ();
			ButtonPanel = GameObject.Find("ButtonPanel");
			EndGamePanel = GameObject.Find ("EndGamePanel");
			EndGamePanel.SetActive (false);
			ButtonPanel.SetActive(false);

			winText.text = "";
			audioSource = GetComponent<AudioSource> ();
			destroy = clips [5];			
		}

		private void Update ()
		{
			UpdateSelection ();
			DrawChessboard ();
			clip = clips[Random.Range (0, 4)];			

			if (Input.GetMouseButtonDown (0)) { //for cursor selection
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

		private void SelectChessman (int x, int y) //responsible for selecting a piece
		{
			if (Chessmans [x, y] == null) //check if piece exists
				return;

			if (Chessmans [x, y].isWhite != isWhiteTurn) //check for which players turn
				return;

			bool hasAtleastOneMove = false;
			allowedMoves = Chessmans [x, y].PossibleMove ();//figuring out allowed moves
			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
					if (allowedMoves [i, j])
						hasAtleastOneMove = true;

			if (!hasAtleastOneMove)
				return;

			selectedChessman = Chessmans [x, y]; 			
			//selectedChessman.transform.position = new Vector3 (selectedChessman.CurrentX, selectedChessman.transform.position.y + 0.25f, selectedChessman.CurrentY);
			previousMat = selectedChessman.GetComponent<MeshRenderer> ().material;//not in Bina's
			selectedMat.mainTexture = previousMat.mainTexture;//not in Bina's
			selectedChessman.GetComponent<MeshRenderer> ().material = selectedMat;//not in Bina's
			this.highlights = BoardHighlights.Instance.HighlightAllowedMoves (allowedMoves); //PROBLEM LINE?
			// BoardHighlights.Instance.HighlightAllowedMoves (allowedMoves); //from Bina
		}

		private void MoveChessman (int x, int y) //responsible for moving a piece
		{
			if (allowedMoves [x, y]) { //Move piece if possible
				audioSource.PlayOneShot (clip, 1.0f);

				Chessman c = Chessmans [x, y];

				if (c != null && c.isWhite != isWhiteTurn) {
					//Capture piece

					//if it is the king 				
					if (c.GetType () == typeof(King)) { //If the piece if a king
						// End the Game
						EndGame ();
						return;
					}
					activeChessman.Remove (c.gameObject); //Destroy the piece if captured
					Destroy (c.gameObject);
					audioSource.PlayOneShot (destroy, 1.0f);					
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
					audioSource.PlayOneShot (destroy, 1.0f);					
				}
				EnPassantMove [0] = -1;
				EnPassantMove [1] = -1;
				if (selectedChessman.GetType () == typeof(Pawn)) {
					if (y == 7) {
						activeChessman.Remove (selectedChessman.gameObject);
						Destroy (selectedChessman.gameObject);
						audioSource.PlayOneShot (destroy, 1.0f);						
						ButtonPanel.SetActive (true);
						Button QWbtn = QueenButton.GetComponent<Button> ();
						QWbtn.onClick.AddListener(delegate{pawntoQueenWhite(x, y);});
						Button RWbtn = RookButton.GetComponent<Button> ();
						RWbtn.onClick.AddListener(delegate{pawntoRookWhite(x, y);});
						Button BWbtn = BishopButton.GetComponent<Button> ();
						BWbtn.onClick.AddListener(delegate{pawntoBishopWhite(x, y);});
						Button KWbtn = KnightButton.GetComponent<Button> ();
						KWbtn.onClick.AddListener(delegate{pawntoKnightWhite(x, y);});

						SpawnChessman (1, x, y);//No longer in Bina's
						selectedChessman = Chessmans [x, y];

					} else if (y == 0) {
						activeChessman.Remove (selectedChessman.gameObject);
						Destroy (selectedChessman.gameObject);
						audioSource.PlayOneShot (destroy, 1.0f);						
						ButtonPanel.SetActive (true);
						Button QBbtn = QueenButton.GetComponent<Button> ();
						QBbtn.onClick.AddListener(delegate{pawntoQueenBlack(x, y);});
						Button RBbtn = RookButton.GetComponent<Button> ();
						RBbtn.onClick.AddListener(delegate{pawntoRookBlack(x, y);});
						Button BBbtn = BishopButton.GetComponent<Button> ();
						BBbtn.onClick.AddListener(delegate{pawntoBishopBlack(x, y);});
						Button KBbtn = KnightButton.GetComponent<Button> ();
						KBbtn.onClick.AddListener(delegate{pawntoKnightBlack(x, y);});						

						SpawnChessman (7, x, y);//No longer in Bina's
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
				selectedChessman.SetPosition (x, y); // allows for moving the same piece twice
				Chessmans [x, y] = selectedChessman;
				isWhiteTurn = !isWhiteTurn; // responsible for turn order
			}

			selectedChessman.GetComponent<MeshRenderer> ().material = previousMat; //not in Bina's
			BoardHighlights.Instance.HideHighlights (highlights);
			selectedChessman = null; //if move is not possible, unselect the piece
		}

		private void UpdateSelection () //for cursor selection
		{
			if (!Camera.main)
				return;

			RaycastHit hit;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 25.0f, LayerMask.GetMask ("ChessPlane"))) {
				selectionX = (int)hit.point.x;
				selectionY = (int)hit.point.z;
				//Debug.Log (Input.mousePosition);				
			} else {
				selectionX = -1;
				selectionY = -1;
			}
		}

		private void SpawnChessman (int index, int x, int y) // Spawns pieces
		{
			GameObject go = Instantiate (chessmanPrefabs [index], GetTileCenter (x, y), orientation) as GameObject;//This line is
			//			GameObject go = Instantiate (chessmanPrefabs [index], GetTileCenter (x, y), Quaternion.Euler (90, 0, 0)) as GameObject;// This line is not in bina or tutor
			go.transform.SetParent (transform);
			Chessmans [x, y] = go.GetComponent<Chessman> ();
			Chessmans [x, y].SetPosition (x, y);
			activeChessman.Add (go);
		}

		private void SpawnAllChessmans () //Spawns the whole board at the same time
		{
			activeChessman = new List<GameObject> ();
			Chessmans = new Chessman[8, 8];
			EnPassantMove = new int[2]{ -1, -1 }; 
			//spawn the white team

			//Spawns White King
			SpawnChessman (0, 3, 0);

			//Spawns White Queen
			SpawnChessman (1, 4, 0);

			//Spawns White Rooks 
			SpawnChessman (2, 0, 0);
			SpawnChessman (2, 7, 0);

			//Spawns White Bishops
			SpawnChessman (3, 2, 0);
			SpawnChessman (3, 5, 0);

			//Spawns White Knights
			SpawnChessman (4, 1, 0);
			SpawnChessman (4, 6, 0);

			//pawns
			for (int i = 0; i < 8; i++){
				SpawnChessman (5, i, 1); //Spawn White Pawns
			}
			//spawn the black team

			//king
			SpawnChessman (6, 4, 7); //Spawns Black King

			//queen
			SpawnChessman (7, 3, 7); //Spawns Black Queen

			//rook
			SpawnChessman (8, 0, 7); //Spawns Black Rooks
			SpawnChessman (8, 7, 7);

			//bishop
			SpawnChessman (9, 2, 7); //Spawns Black Bishops
			SpawnChessman (9, 5, 7);

			//knight
			SpawnChessman (10, 1, 7); //Spawns Black Knights
			SpawnChessman (10, 6, 7);

			//pawn
			for (int i = 0; i < 8; i++){
				SpawnChessman (11, i, 6); //Spawn Black Pawns	
			}
		}

		private Vector3 GetTileCenter (int x, int y)  //Spawn Position Function, gets the center point on the tile
		{
			Vector3 origin = Vector3.zero;
			origin.x += (TILE_SIZE * x) + TILE_OFFSET;
			origin.z += (TILE_SIZE * y) + TILE_OFFSET;
			return origin;
		}

		private void DrawChessboard () //Outline the board with vectors
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

			// Draw selection
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
			ButtonPanel.SetActive (false);
		}
		void pawntoRookWhite(int x, int y){

			Debug.Log ("Rook button is clicked");
			SpawnChessman (2, x, y);
			ButtonPanel.SetActive (false);
		}
		void pawntoBishopWhite(int x, int y){

			Debug.Log ("Bishop button is clicked");
			SpawnChessman (3, x, y);
			ButtonPanel.SetActive (false);
		}
		void pawntoKnightWhite(int x, int y){

			Debug.Log ("Knight button is clicked");
			SpawnChessman (4, x, y);
			ButtonPanel.SetActive (false);
		}

		//Black
		void pawntoQueenBlack(int x, int y){

			Debug.Log ("Queen button is clicked");
			SpawnChessman (7, x, y);
			ButtonPanel.SetActive (false);
		}
		void pawntoRookBlack(int x, int y){

			Debug.Log ("Rook button is clicked");
			SpawnChessman (8, x, y);
			ButtonPanel.SetActive (false);
		}
		void pawntoBishopBlack(int x, int y){

			Debug.Log ("Bishop button is clicked");
			SpawnChessman (9, x, y);
			ButtonPanel.SetActive (false);
		}
		void pawntoKnightBlack(int x, int y){

			Debug.Log ("Knight button is clicked");
			SpawnChessman (10, x, y);
			ButtonPanel.SetActive (false);
		}

		private void EndGame ()
		{

			if (isWhiteTurn){
				Debug.Log ("White Team Wins!");
				winText.text = "White Team Wins!";				
			}else{
				Debug.Log ("Black Team Wins!");
				winText.text = "Black Team Wins!";				
			}

			EndGamePanel.SetActive (true);

			foreach (GameObject go in activeChessman)
				Destroy (go);

			isWhiteTurn = true;
			BoardHighlights.Instance.HideHighlights (highlights);
			SpawnAllChessmans ();
		}
	}
}