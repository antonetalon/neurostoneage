using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class TrainingDecisionModel {
	public int[] Inputs;
	public List<int> Options;
	public int Output;
	public float RewardPercent;
	public readonly DecisionType Type;
	public readonly int PlayerInd;
	public TrainingDecisionModel(DecisionType type, int[] inputs, List<int> options, int playerInd) {
		this.Type = type;
		this.Inputs = inputs;
		this.Options = options;
		this.PlayerInd = playerInd;
	}
}
public enum DecisionType {
	None,
	SelectWhereToGo,
	SelectUsedHumans,
	SelectCharity,
	SelectInstruments,
	SelectLeaveHungry
}
