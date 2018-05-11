using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private MapMovementController moveController;
	private Animator animator;

	// Use this for initialization
	void Start () {
		moveController = GetComponent<MapMovementController> ();
		moveController.moveActionCallback += OnMove;
		moveController.tileActionCallback += OnTile;

		animator = GetComponent<Animator> ();
		animator.speed = 0;
	}

	void OnMove(){
		animator.speed = 1;
	}

	void OnTile(int type){
		animator.speed = 0;
	}

	// Update is called once per frame
	void Update () {
		var dir = Vector2.zero;

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			//Debug.Log ("Up Key Pressed");
			dir.y = -1;
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			//Debug.Log ("Right Key Pressed");
			dir.x = 1;
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			//Debug.Log ("Down Key Pressed");
			dir.y = 1;
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			//Debug.Log ("Left Key Pressed");
			dir.x = -1;
		}

		if (dir.x != 0 || dir.y != 0)
			moveController.MoveInDirection (dir);

	}
}
