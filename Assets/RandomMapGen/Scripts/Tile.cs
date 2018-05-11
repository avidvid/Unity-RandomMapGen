using UnityEngine;
using System.Collections;
using System;
using System.Text;


//to do 8 sided bit shifteing to fix the bleedng 
public enum Sides{
	Bottom,
	Right,
	Left,
	Top
}

public class Tile {

	public int id = 0;
	public Tile[] neighbors = new Tile[4];
	public int autotileID;
	public bool visited;
	public int fowAutotileID;

	public void AddNeighbor(Sides side, Tile tile){
		neighbors [(int)side] = tile;
		CalculateAutotileID ();
	}

	public void RemoveNeighbor(Tile tile){

		var total = neighbors.Length;
		for (var i = 0; i < total; i++) {
			if (neighbors [i] != null) {
				//We only remove tiles with similar id to prevent deadlocks 
				if (neighbors [i].id == tile.id) {
					neighbors [i] = null;
				}
			}
		}
		CalculateAutotileID ();
	}

	//Remove the refrence of the removed tile from the neighbors array 
	public void ClearNeighbors(){
		
		var total = neighbors.Length;
		for (var i = 0; i < total; i++) {
			var tile = neighbors [i];
			if (tile != null) {
				tile.RemoveNeighbor (this);
				neighbors [i] = null;
			}
		}

		CalculateAutotileID ();
	}

	private void CalculateAutotileID(){

		//StringBuilder allow concat string and is more memory efficient 
		var sideValues = new StringBuilder ();
		//0000 no naibors 
		//0001 top neighbr only 
		foreach (Tile tile in neighbors) {
			sideValues.Append (tile == null ? "0" : "1");
		}
		autotileID = Convert.ToInt32 (sideValues.ToString (), 2);

	}


	//For each tile we calculate how the surrunding tile fog of War should be 
	public void CalculateFoWAutotileID(){

		//StringBuilder allow concat string and is more memory efficient 
		var sideValues = new StringBuilder ();
		//0000 no naibors 
		//0001 top neighbr only 
		foreach (Tile tile in neighbors) {
			if (tile == null) {
				sideValues.Append ("1");
			} else {
				sideValues.Append (tile.visited ? "0" : "1" );
			}
		}
		fowAutotileID = Convert.ToInt32 (sideValues.ToString (), 2);

	}


}
