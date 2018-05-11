using UnityEngine;
using System.Collections;

public class PixelPerfectCamera : MonoBehaviour {

	public static float pixelToUnits = 1f;
	public static float scale = 1f;

	public Vector2 nativeResolution = new Vector2 (160, 144);

	void Awake(){

		var camera = GetComponent<Camera> ();

		//if it is 2D
		if (camera.orthographic) {

			var dir = Screen.height;
			var res = nativeResolution.y;

			scale = dir / res;
			pixelToUnits *= scale;

			camera.orthographicSize = (dir / 2.0f) / pixelToUnits;

		}

	}
}
