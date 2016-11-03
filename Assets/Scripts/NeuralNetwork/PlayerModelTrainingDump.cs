using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

class PlayerModelTrainingDump {

	private Dictionary<ResourceType, int> Resources;
	public PlayerModelTrainingDump() {
		Resources = new Dictionary<ResourceType, int>();
		foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
			Resources.Add(type, 0);
	}
	public void Add(ResourceType type, int count) {
		Resources [type] += count;
	}
	public int GetCount(ResourceType type) {
		return Resources [type];
	}
	public int ChangeCount(ResourceType type, int delta) {
		return Resources [type] += delta;
	}
	public PlayerModelTrainingDump Clone() {
		PlayerModelTrainingDump clone = new PlayerModelTrainingDump ();
		foreach (var item in Resources)
			clone.Resources[item.Key] = item.Value;
		return clone;
	}
	public override string ToString ()
	{
		StringBuilder sb = new StringBuilder ();
		foreach (var item in Resources) {
			if (item.Value == 0)
				continue;

			sb.Append (item.Key.ToString ());
			sb.Append (" ");
			if (item.Value > 0)
				sb.Append ("+");
			sb.Append (item.Value.ToString ("000"));
			sb.Append (", ");
		}
		return sb.ToString ();
	}
}
