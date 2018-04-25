using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Project
{
	public class BoardManager : MonoBehaviour
	{
		public static BoardManager Instance{ set; get; }
		//will give the pieces the information on when a specific move to a tile is legal or not
		private bool[,] allowedMoves{ set; get; }

		public Chessman[,] Chessmans{ set; get; }

		private Chessman selectedChessman;

		private const float TILE_SIZE = 1.0f;		//tile dimensions
		private const float TILE_OFFSET = 0.5f;

		private int selectionX = -1;
		private int selectionY = -1;

		public List<GameObject> chessmanPrefabs;
		private List<GameObject> activeChessman = new List<GameObject> ();

		public bool isWhiteTurn = true;

		private void Start ()
		{
			Instance = this;
			SpawnAllChessmans ();
		}

		private void Update ()
		{
			UpdateSelection ();
			DrawChessboard ();

			if (Input.GetMouseButtonDown (0)) { //for cursor selection
				if (selectionX >= 0 && selectionY >= 0) {
					if (selectedChessman == null) {
						SelectChessman (selectionX, selectionY);
					} else {
						MoveChessman (selectionX, selectionY);
					}
				}
			}
		}

		private void SelectChessman (int x, int y) //responsible for selecting a piece
		{
			if (Chessmans [x, y] == null) //check if piece is selected
			return;

			if (Chessmans [x, y].isWhite != isWhiteTurn) //check for which players turn
			return;

			allowedMoves = Chessmans [x, y].PossibleMove ();//figuring out allowed moves
			selectedChessman = Chessmans [x, y];
			BoardHighlights.Instance.HighlightAllowedMoves (allowedMoves); //PROBLEM LINE?

		}

		private void MoveChessman (int x, int y) //responsible for moving a piece
		{
			if (allowedMoves [x, y]) { //Move piece if possible
				Chessman c = Chessmans [x, y];

				if (c != null && c.isWhite != isWhiteTurn) {
					//Capture piece
					if (c.GetType () == typeof(King)) { //If the piece if a king
						// End the Game
						return;
					}
					activeChessman.Remove (c.gameObject); //Destroy the piece if captured
					Destroy (c.gameObject);
				}
				Chessmans [selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
				selectedChessman.transform.position = GetTileCenter (x, y);
				selectedChessman.SetPosition (x, y); // allows for moving the same piece twice
				Chessmans [x, y] = selectedChessman;
				isWhiteTurn = !isWhiteTurn; // responsible for turn order
			}

			BoardHighlights.Instance.Hidehighlights ();
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
			} else {
				selectionX = -1;
				selectionY = -1;
			}
		}

		private void SpawnChessman (int index, int x, int y) // Spawns pieces
		{
			GameObject go = Instantiate (chessmanPrefabs [index], GetTileCenter (x, y), Quaternion.Euler (90, 0, 0)) as GameObject;
			go.transform.SetParent (transform);
			Chessmans [x, y] = go.GetComponent<Chessman> ();
			Chessmans [x, y].SetPosition (x, y);
			activeChessman.Add (go);
		}

		private void SpawnAllChessmans () //Spawns the whole board at the same time
		{
			activeChessman = new List<GameObject> ();
			Chessmans = new Chessman[8, 8];

			SpawnChessman (0, 3, 0); //Spawns White King
			SpawnChessman (1, 4, 0); //Spawns White Queen
			SpawnChessman (2, 0, 0); //Spawns White Rooks
			SpawnChessman (2, 7, 0);
			SpawnChessman (3, 2, 0); //Spawns White Bishops
			SpawnChessman (3, 5, 0);
			SpawnChessman (4, 1, 0); //Spawns White Knights
			SpawnChessman (4, 6, 0);
			for (int i = 0; i < 8; i++)
				SpawnChessman (5, i, 1); //Spawn White Pawns

			SpawnChessman (6, 4, 7); //Spawns Black King
			SpawnChessman (7, 3, 7); //Spawns Black Queen
			SpawnChessman (8, 0, 7); //Spawns Black Rooks
			SpawnChessman (8, 7, 7);
			SpawnChessman (9, 2, 7); //Spawns Black Bishops
			SpawnChessman (9, 5, 7);
			SpawnChessman (10, 1, 7); //Spawns Black Knights
			SpawnChessman (10, 6, 7);
			for (int i = 0; i < 8; i++)
				SpawnChessman (11, i, 6); //Spawn Black Pawns	

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
	}
}