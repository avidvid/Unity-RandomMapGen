using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(RandomMapTester))]
public class RandomMapTesterEditor : Editor {

	//MAke a Custom editor
	public override void OnInspectorGUI(){
		DrawDefaultInspector ();

		var script = (RandomMapTester)target;

		if (GUILayout.Button ("Generate Island")) {
			if (Application.isPlaying) {
				script.MakeMap ();
			}
		}

		if (GUILayout.Button ("Create Player")) {
			if (Application.isPlaying) {
				script.CreatePlayer ();
			}
		}
		if (GUILayout.Button ("Reset")) {
			if (Application.isPlaying) {
				script.Reset ();
			}
		}
	}

}
