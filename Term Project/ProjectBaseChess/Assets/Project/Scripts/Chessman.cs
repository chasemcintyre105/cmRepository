using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Project
{
	public abstract class Chessman : MonoBehaviour //Chessman class for pieces
	{
		public int CurrentX{ set; get; }

		public int CurrentY{ set; get; }

		public bool isWhite;

		public void SetPosition (int x, int y)
		{
			CurrentX = x;
			CurrentY = y;
		}

		public virtual bool[,] PossibleMove () //Multi dimensional bool, determine possible moves
		{
			return new bool[8, 8];
		}
	}
}