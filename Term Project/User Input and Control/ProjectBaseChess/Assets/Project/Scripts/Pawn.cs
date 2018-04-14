using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
	public class Pawn : Chessman
	{
		public override bool[,] PossibleMove ()
		{
			bool[,] r = new bool [8, 8];
			Chessman c, c2;

			// White team move
			if (isWhite) {
				//Diagonal Left
				if (CurrentX != 0 && CurrentY != 7) { //if piece is at current position
					c = BoardManager.Instance.Chessmans [CurrentX - 1, CurrentY + 1]; //get piece
					if (c != null && !c.isWhite)//capture
					r [CurrentX - 1, CurrentY + 1] = true;
				}

				//Diagonal Right
				if (CurrentX != 0 && CurrentY != 7) { //if piece is at current position
					c = BoardManager.Instance.Chessmans [CurrentX + 1, CurrentY + 1]; //get piece
					if (c != null && !c.isWhite)//capture
					r [CurrentX + 1, CurrentY + 1] = true;
				}

				//Middle
				if (CurrentY != 7) {
					c = BoardManager.Instance.Chessmans [CurrentX, CurrentY + 1]; //only able to move if no other piece infront (Pawn rule)
					if (c == null)
						r [CurrentX, CurrentY + 1] = true;
				}

				//Middle on First move (two tiles)
				if (CurrentY == 1) {
					c = BoardManager.Instance.Chessmans [CurrentX, CurrentY + 1]; //only able to move if no other piece infront (Pawn rule)
					c2 = BoardManager.Instance.Chessmans [CurrentX, CurrentY + 2]; //only able to move if no other piece infront two tiles (Pawn rule)
					if (c == null & c2 == null)
						r [CurrentX, CurrentY + 2] = true;
				}
			} else { //Black Team Pawn Movement
				//Diagonal Left
				if (CurrentX != 0 && CurrentY != 0) { //if piece is at current position
					c = BoardManager.Instance.Chessmans [CurrentX - 1, CurrentY - 1]; //get piece
					if (c != null && c.isWhite) //capture
					r [CurrentX - 1, CurrentY + 1] = true;
				}

				//Diagonal Right
				if (CurrentX != 7 && CurrentY != 0) { //if piece is at current position
					c = BoardManager.Instance.Chessmans [CurrentX + 1, CurrentY - 1]; //get piece
					if (c != null && c.isWhite)//capture
					r [CurrentX + 1, CurrentY - 1] = true;
				}

				//Middle
				if (CurrentY != 0) {
					c = BoardManager.Instance.Chessmans [CurrentX, CurrentY - 1]; //only able to move if no other piece infront (Pawn rule)
					if (c == null)
						r [CurrentX, CurrentY - 1] = true;
				}

				//Middle on First move (two tiles)
				if (CurrentY == 6) {
					c = BoardManager.Instance.Chessmans [CurrentX, CurrentY - 1]; //only able to move if no other piece infront (Pawn rule)
					c2 = BoardManager.Instance.Chessmans [CurrentX, CurrentY - 2]; //only able to move if no other piece infront two tiles (Pawn rule)
					if (c == null & c2 == null)
						r [CurrentX, CurrentY - 2] = true;
				}

			}

			return r;
		}
	}
}