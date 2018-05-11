using UnityEngine;
using System.Collections;

public class PosUtil {

	public static void CalculateIndex(int x, int y, int width, out int index){
		index = x + y * width;
	}

	public static void CalculatePos(int index, int width, out int x, out int y){
		x = index % width;
		y = index / width;
	}

}
