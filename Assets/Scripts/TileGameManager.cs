using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGameManager : MonoBehaviour {

	public static TileGameManager Instance;

	public Tile emptyTile;   
	[SerializeField]
	public List<Tile> tileList;

	public bool isShufflingOn = true;
	
	[Range (0.01f, 1.0f)]
	public float animationTime = 1.0f;

	private List<Vector3> tilePositionList;
	
	void Awake ()
	{
		Instance = this;
	}
	
	void Start () {

		tilePositionList = new List<Vector3> ();
		for (int i = 0; i < tileList.Count; i++)
		{
			tileList [i].sequentialIndex = i;
			tilePositionList.Add (tileList [i].gameObject.transform.position);
		}
		StartCoroutine (ShuffleTile ());
	}
	
	IEnumerator ShuffleTile ()
	{
		yield return new WaitForSeconds (animationTime);

		int randomNumberOne;
		int randomNumberTwo;
		for (int i = 0; i < 50; i++)
		{
			randomNumberOne = Random.Range (0, 16);
			randomNumberTwo = Random.Range (0, 16);
			
			while (randomNumberOne == randomNumberTwo)
			{
				randomNumberTwo = Random.Range (0, 16);
			};

			SwapTile (randomNumberOne, randomNumberTwo);

			yield return new WaitForSeconds (animationTime / 4);
			yield return new WaitForEndOfFrame ();
			yield return new WaitForEndOfFrame ();
		}		
		isShufflingOn = false;
	}

	#region SwapTileLogic
	void SwapTile (int indexOne, int indexTwo)
	{
		iTween.MoveTo (tileList [indexOne].gameObject, tilePositionList [indexTwo], animationTime);
		iTween.MoveTo (tileList [indexTwo].gameObject, tilePositionList [indexOne], animationTime);
		
		Tile temp = tileList [indexOne];
		tileList [indexOne] = tileList [indexTwo];
		tileList [indexTwo] = temp;
		
		SwapTileData (tileList[indexOne], tileList[indexTwo]);
	}
	
	void SwapTileData (Tile tileOne, Tile tileTwo)
	{
		Swap (ref tileOne.index, ref tileTwo.index);
		Swap (ref tileOne.row, ref tileTwo.row);
		Swap (ref tileOne.column, ref tileTwo.column);
	}

	void Swap (ref int v1, ref int v2)
	{
		int temp = v1;
		v1 = v2;
		v2 = temp;
	}
	#endregion

	#region Tile Input Logic
	public void TilePressed (Tile selectedTile)
	{
		if (isShufflingOn)
			return;

		int emptyTileIndex = Tile.IndexFor (emptyTile.row, emptyTile.column);

		if (selectedTile.row == emptyTile.row)
		{
			//Same Row selected.
			int tileDistance = selectedTile.column - emptyTile.column;
			bool direction = tileDistance > 0;

			tileDistance = direction ? tileDistance : - tileDistance;

			for (int i = 0; i < tileDistance; i++)
			{
				int nextIndex = direction ? emptyTileIndex + 1 : emptyTileIndex - 1;
				SwapTile (emptyTileIndex, nextIndex);
				emptyTileIndex += direction ? 1 : -1;
			}
		}
		else if (selectedTile.column == emptyTile.column)
		{
			//Same Column selected.
			int tileDistance = selectedTile.row - emptyTile.row;
			bool direction = tileDistance > 0;

			tileDistance = direction ? tileDistance : - tileDistance;

			for (int i = 0; i < tileDistance; i++)
			{
				int nextIndex = direction ? emptyTileIndex + 4 : emptyTileIndex - 4;
				SwapTile (emptyTileIndex, nextIndex);
				emptyTileIndex += direction ? 4 : -4;
			}
		}

		StartCoroutine(CheckGameEnd ());
	}
	#endregion

	#region Validation
	IEnumerator CheckGameEnd ()
	{
		if (IsAllTilesValid())
		{
			GameObject resultObject = Instantiate( Resources.Load ("Effects/Result", typeof(GameObject)),
			                                      Camera.main.ScreenToWorldPoint ( new Vector3(Screen.width / 2, Screen.height / 2, 40)),
			                                      Quaternion.identity) as GameObject;
			iTween.PunchScale (resultObject, Vector3.one * 2, 2.0f);
			isShufflingOn = true;
			yield return new WaitForSeconds (5);
			DestroyImmediate (resultObject);
			Resources.UnloadUnusedAssets ();
			StartCoroutine (ShuffleTile ());
		}
		yield return null;
	}

	public bool IsAllTilesValid ()
	{
		for (int i = 0; i < tileList.Count; i++)
		{
			if (i != tileList [i].sequentialIndex)
				return false;
		}
		return true;
	}
	#endregion
}
