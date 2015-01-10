using UnityEngine;
using System.Collections;

[SerializeField]
public class Tile : MonoBehaviour {

	public int index;

	public int row;
	public int column;

	[HideInInspector]
	public int sequentialIndex;

	public static int IndexFor (int _row, int _column)
	{
		return _column + _row * 4;
	}

	public void TilePressed ()
	{
		TileGameManager.Instance.TilePressed (this);
	}
}
