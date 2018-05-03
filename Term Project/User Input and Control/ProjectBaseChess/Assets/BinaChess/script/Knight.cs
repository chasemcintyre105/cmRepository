using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BinaChess
{
	public class Knight : Chessman
	{

		public override bool[,] PossibleMove ()
		{

			bool[,] r = new bool[8, 8];

			//Upleft
			KnightMove (CurrentX - 1, CurrentY + 2, ref r);

			//Upright
			KnightMove (CurrentX + 1, CurrentY + 2, ref r);

			//rightup
			KnightMove (CurrentX + 2, CurrentY + 1, ref r);

			//rightdown
			KnightMove (CurrentX + 2, CurrentY - 1, ref r);

			//DownLeft
			KnightMove (CurrentX - 1, CurrentY - 2, ref r);

			//DownRight
			KnightMove (CurrentX + 1, CurrentY - 2, ref r);

			//Leftup
			KnightMove (CurrentX - 2, CurrentY + 1, ref r);

			//Leftdown
			KnightMove (CurrentX - 2, CurrentY - 1, ref r);


			return r;
		}

		public void KnightMove (int x, int y, ref bool[,] r)
		{

			Chessman c; 

			if (x >= 0 && x < 8 && y >= 0 && y < 8) {

				c = BoardManager.Instance.Chessmans [x, y];
				if (c == null)
					r [x, y] = true;
				else {
					if (c.isWhite != isWhite)
						r [x, y] = true;
				
				}
			}
		}
	}
}