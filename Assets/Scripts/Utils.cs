using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils {

	public static void Shuffle<T>(List<T> list) {
		List<T> source = new List<T> ();
		foreach (var item in list)
			source.Add (item);
		list.Clear ();
		foreach (var item in source) {
			int ind = Game.RandomRange (0, list.Count);
			list.Insert (ind, item);
		}
	}
}
