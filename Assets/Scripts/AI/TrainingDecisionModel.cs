using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class TrainingDecisionModel {
	public int[] Inputs;
	public int Output;
	public float Reward;
	public readonly DecisionType Type;
	public readonly int PlayerInd;
	public TrainingDecisionModel(DecisionType type, int[] inputs, int playerInd) {
		this.Type = type;
		this.Inputs = inputs;
		this.PlayerInd = playerInd;
	}
}
public enum DecisionType {
	SelectWhereToGo,
	SelectUsedHumans,
	ChooseItemToReceiveFromCharityCard,
	GetUsedInstrumentSlotInd,
	LeaveHungry
}
