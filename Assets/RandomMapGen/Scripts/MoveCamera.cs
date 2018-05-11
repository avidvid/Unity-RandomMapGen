using UnityEngine;
using System.Collections;


//todo: add boundries to camera 


public class MoveCamera : MonoBehaviour {

	public float speed = 4f;
	public GameObject target;

	public bool mirror = false;

	private Vector3 startPos;
	private bool moving;


	//simlar to update() to calculating the distance just for performance to run it less than update()
	//Dragging the mouse 
	void FixedUpdate(){

		//Mouse clicked
		if (Input.GetMouseButtonDown (1)) {
			startPos = Input.mousePosition;
			moving = true;
		}

		//Mouse clided left and it was clicked and moved before then stop moving 
		if (Input.GetMouseButtonUp (1) && moving) {
			moving = false;
		}

		//while moving update the LocationInfo OperatingSystemFamily the camera 
		if (moving) {

			Vector3 pos = Camera.main.ScreenToViewportPoint (Input.mousePosition - startPos);
			Vector3 move = new Vector3 (-pos.x * speed, -pos.y * speed, 0);

			if (mirror) {
				move = -move;
			} 
			transform.Translate (move, Space.Self);

		}else if (target != null) {
			var pos = target.transform.position;
			pos.z = Camera.main.transform.position.z;

			Camera.main.transform.position = pos; 
		}

	}


}
