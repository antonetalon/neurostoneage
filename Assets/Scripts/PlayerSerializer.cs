using System.IO;
using UnityEngine;

public class PlayerSerializer {
	public static void SavePlayer(string name, Player player) {
		using (Stream stream = File.Open(name, FileMode.Create)) {
			var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			binaryFormatter.Serialize(stream, player);
		}
		Debug.Log (name + " player saved");
	}
	public static Player LoadPlayer(string name) {
		Player res = null;
		using (Stream stream = File.Open(name, FileMode.Open)) {
			var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			res = (Player)binaryFormatter.Deserialize (stream);
		}
		Debug.Log (name + " player loaded");
		return res;
	}

	public static void SaveComparer(ComparerModel model) {
		using (Stream stream = File.Open("ComparedPlayers", FileMode.Create)) {
			var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			binaryFormatter.Serialize(stream, model);
		}
		Debug.Log ("Comparer saved");
	}
	public static ComparerModel LoadComparer() {
		
		ComparerModel res = null;
		try {
			using (Stream stream = File.Open("ComparedPlayers", FileMode.Open)) {
				var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				res = (ComparerModel)binaryFormatter.Deserialize (stream);
			}
			Debug.Log ("Comparer loaded");
		} catch {
			res = new ComparerModel ();
		}
		return res;
	}
}