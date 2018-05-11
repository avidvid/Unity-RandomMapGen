using UnityEngine;
using System.Collections;

public class RandomMapTester : MonoBehaviour {

	[Header("Map Dimensions")]
	public int mapWidth = 20;
	public int mapHeight = 20;

	[Space]
	[Header("Vizualize Map")]
	public GameObject mapContainer;
	public GameObject tilePrefab;
	public Vector2 tileSize = new Vector2(16,16);

	[Space]
	[Header("Map Sprites")]
	public Texture2D islandTexture;
	public Texture2D fowTexture;


	[Space]
	[Header("Player")]
	public GameObject playerPrefab;
	public GameObject player;
	//1 single tile 
	//3 one each side 
	//5 2 each side 
	//only odd numbers
	public int viewDistance = 3;

	[Space]
	[Header("Decorate Map")]
	[Range(0, .9f)]
	public float erodePercent = .5f;
	public int erodeIterations = 2;
	[Range(0, .9f)]
	public float treePercent = .3f;
	[Range(0, .9f)]
	public float hillPercent = .2f;
	[Range(0, .9f)]
	public float mountainsPercent = .1f;
	[Range(0, .9f)]
	public float townPercent = .05f;
	[Range(0, .9f)]
	public float monsterPercent = .1f;
	[Range(0, .9f)]
	public float lakePercent = .05f;

	public Map map;

	private int tmpX;
	private int tmpY;

	//To keep the texture of tiles and fow 
	private Sprite[] islandTileSprites;
	private Sprite[] fowTileSprites;

	// Use this for initialization
	void Start () {

		//islandTexture is the path of the textures 
		islandTileSprites = Resources.LoadAll<Sprite> (islandTexture.name);
		fowTileSprites = Resources.LoadAll<Sprite> (fowTexture.name);

		Reset ();
	}

	public void Reset(){
		map = new Map ();
		MakeMap ();
		//Give delay before adding player by using coroutine
		StartCoroutine (AddPlayer ());
	
	}


	IEnumerator AddPlayer(){
		//waiting untill the end of the frame to add the player 
		yield return new WaitForEndOfFrame ();
		CreatePlayer ();

		//To remove the fog at the begining of the game 
		//VisitTile (map.castleTile.id);

	}
	
	public void MakeMap(){
		map.NewMap (mapWidth, mapHeight);
		//Debug.Log ("Created a new " + map.columns + "x" + map.rows + " map");
		map.CreateIsland (
			erodePercent,
			erodeIterations,
			treePercent,
			hillPercent,
			mountainsPercent,
			townPercent,
			monsterPercent,
			lakePercent
		);
		CreateGrid ();

		CenterMap (map.castleTile.id);
	}

	void CreateGrid(){
		ClearMapContainer ();

		//islandTexture is the path of the textures 
		//no need for this we have it seperately on thestart method now 
		//Sprite[] sprites = Resources.LoadAll<Sprite> (islandTexture.name);

		//Total number of tiles
		var total = map.tiles.Length;
		var maxColumns = map.columns;
		var column = 0;
		var row = 0;

		for (var i = 0; i < total; i++) {

			//Find the column that we are creating 
			column = i % maxColumns;


			//location of the new tile that we are crreating
			//right
			var newX = column * tileSize.x;
			//down
			var newY = -row * tileSize.y;

			//Go is the new tile
			var go = Instantiate (tilePrefab);
			go.name = "Tile " + i;
			//Nest the game object inside the container 
			go.transform.SetParent (mapContainer.transform);
			go.transform.position = new Vector3 (newX, newY, 0);

			DecorateTile (i);

			//next row
			if (column == (maxColumns - 1)) {
				row++;
			}

		}

	}

	private void DecorateTile(int tileID){
		//autotileID get calculated in tile class and is the binary id of 0000 to 1111
		var tile = map.tiles [tileID];
		var spriteID = tile.autotileID;
		var go = mapContainer.transform.GetChild (tileID).gameObject;


		//For sprite that we don't want to show like water ... 
		//so no tile sprite means the background show up which can be water grasss or what ever else 
		if (spriteID >= 0) {
			var sr = go.GetComponent<SpriteRenderer> ();
			if (tile.visited) {
				sr.sprite = islandTileSprites [spriteID];
			} else {
				//Calcualte new values for the tile of the neighbors 
				tile.CalculateFoWAutotileID ();
				//using min to make sure we don't go out of range  (because there are mmore island tiles than fog tiles)
				sr.sprite = fowTileSprites [Mathf.Min(tile.fowAutotileID,fowTileSprites.Length-1)];
			}
		}

	}


	public void CreatePlayer(){
		//Create an instance of player
		player = Instantiate (playerPrefab);
		player.name = "Player";
		//Set the parent and location 
		player.transform.SetParent (mapContainer.transform);

		//inisiate the player At the castle 
// Option 1: no controller 
//		//finding location of the player  
//		//Calcualte the postion of the casle map.castleTile.id = index ,map.columns=width
//		PosUtil.CalculatePos (map.castleTile.id, map.columns, out tmpX, out tmpY);
//		//Taking account the size of the tile to find correct location for the player
//		tmpX *= (int)tileSize.x;
//		tmpY *= -(int)tileSize.y;
//		player.transform.position = new Vector3(tmpX,tmpY,0);

// Option 2: with controller 
		//ConfigurableJoint the Controller 
		var controller = player.GetComponent<MapMovementController> ();
		controller.map = map;
		controller.tileSize = tileSize;
		controller.tileActionCallback += TileActionCallback;

		var moveScript = Camera.main.GetComponent<MoveCamera> ();
		moveScript.target = player;


		//by having this in the last line it guranties the fow works after the start and restart 
		controller.MoveTo (map.castleTile.id);

	}


	void TileActionCallback(int type)
	{
		Debug.Log ("On tyle type " + type);
		var tileID = player.GetComponent<MapMovementController> ().currentTile;

		VisitTile (tileID);
	}

	void ClearMapContainer(){

		var children = mapContainer.transform.GetComponentsInChildren<Transform> ();
		//On delete the index of all child goes down so we need to do the reverce delete not from 0 to N
		for (var i = children.Length - 1; i > 0; i--) {
			//Desroy only workd on game object so we need to pass a game object insetad of transform
			Destroy (children [i].gameObject);
		}

	}

	//centerize the Camera on any tile that we give it Touch the method
	void CenterMap(int index){

		var camPos = Camera.main.transform.position;
		var width = map.columns;



		PosUtil.CalculatePos (index, width,out tmpX,out tmpY);
		camPos.x = tmpX * tileSize.x;
		camPos.y = -tmpY * tileSize.y;

		//Before using position utility 
		//camPos.x = (index % width) * tileSize.x;
		//camPos.y = -((index / width) * tileSize.y);

		Camera.main.transform.position = camPos;

	}


	//when we visit a Tile we decorate it AndroidJNI also WebCamFlags redecorate the neighbor tiles 
	//to have apropriate FogMode 
	void VisitTile(int index)
	{

		int column, newX, newY, row = 0;

		//Calculate position of current tile
		PosUtil.CalculatePos (index, map.columns, out tmpX, out tmpY);

		//Half way of the field of view 
		var half = Mathf.FloorToInt (viewDistance / 2f);
		//Shifiting the area that we are going to loop all that area 
		tmpX -= half;
		tmpY -= half;

		//find total tile to visit
		var total = viewDistance * viewDistance;
		//Calculate the column
		var maxColumns = viewDistance - 1; 

		for (int i = 0; i < total; i++) {

			//Find the index OperatingSystemFamily Tile int this Area 
			column = i % viewDistance;
			newX = column + tmpX;
			newY = row + tmpY;
			PosUtil.CalculateIndex(newX,newY, map.columns, out index);

			if (index > -1 && index <map.tiles.Length) {
				var tile = map.tiles [index];
				tile.visited = true;

				DecorateTile (index);

				foreach (var neighbor in tile.neighbors) {
					if (neighbor !=null) {
						if (!neighbor.visited) {
							neighbor.CalculateFoWAutotileID ();
							DecorateTile (neighbor.id);  
						}
					}
				}
			}
			if (column == maxColumns) {
				row++;
			}
		}
	}


}
