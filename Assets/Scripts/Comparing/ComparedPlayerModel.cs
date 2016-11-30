using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class ComparedPlayerModel {
	public readonly Player Player;
	const int SuccessCount = 10;
	public List<float> Success;
	public readonly string Name;
	public string Comment;
	public ComparedPlayerModel(Player player, string name) {
		this.Player = player;
		this.Name = name;
		Success = new List<float> ();
	}
	public void AddSuccess(float success) {
		if (Success.Count >= SuccessCount)
			Success.RemoveAt (0);
		Success.Add (success);
	}
}
