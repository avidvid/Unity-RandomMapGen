using UnityEngine;
using System.Collections;
using System;

//Allow us to move around the map 
public class MapMovementController : MonoBehaviour {

	public Map map;
	public Vector2 tileSize;
	public int currentTile;
	public float speed = 1f;
	public bool moving;
	public int[] blockedTileTypes;

	//Will be define and load later type of the tile on map
	public delegate void TileAction(int Type);
	public TileAction tileActionCallback;

	//Event of movement 
	public delegate void MoveAction();
	public MoveAction moveActionCallback;

	private float moveTime;
	private Vector2 startPos;
	private Vector2 endPos;
	private int tmpIndex;
	private int tmpX;
	private int tmpY;

	public void MoveTo(int index, bool animate = false){

		if (!CanMove (index))
			return;

		if (moveActionCallback != null)
			moveActionCallback ();

		currentTile = index;

		PosUtil.CalculatePos (index, map.columns, out tmpX, out tmpY);

		tmpX *= (int)tileSize.x;
		tmpY *= -(int)tileSize.y;

		var newPos = new Vector3 (tmpX, tmpY, 0);
		if (!animate) {
			transform.position = newPos;

			if (tileActionCallback != null)
				tileActionCallback (map.tiles [currentTile].autotileID);

		} else {
			startPos = transform.position;
			endPos = newPos;
			moveTime = 0;
			moving = true;
		}
	}

	public void MoveInDirection(Vector2 dir){
		PosUtil.CalculatePos (currentTile, map.columns, out tmpX, out tmpY);

		tmpX += (int)dir.x;
		tmpY += (int)dir.y;

		PosUtil.CalculateIndex (tmpX, tmpY, map.columns, out tmpIndex);

		Debug.Log ("Move to tile "+tmpIndex);
		MoveTo (tmpIndex, true);
	}

	void Update(){

		if (moving) {

			moveTime += Time.deltaTime;
			if (moveTime > speed) {
				moving = false;
				transform.position = endPos;

				if (tileActionCallback != null)
					tileActionCallback (map.tiles [currentTile].autotileID);
			}

			//calculate the value of between to number over the time 
			transform.position = Vector2.Lerp (startPos, endPos, moveTime / speed);
		}

	}

	bool CanMove(int index){

		//For index out of range error 
		if (index < 0 || index >= map.tiles.Length)
			return false;

		var tileType = map.tiles [index].autotileID;
		//Move only if it is not moving 
		if (moving || Array.IndexOf(blockedTileTypes, tileType) > -1)
			return false;

		return true;
	}
}
