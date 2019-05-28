using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

class ModelChangeEvent {
	public float ScoreValue;
	public Dictionary<ModelChangeEvent, float> Causes = new Dictionary<ModelChangeEvent, float>();

	public PlayerModelTrainingDump StateBefore;
	public PlayerModelTrainingDump StateAfter;
	public PlayerModelTrainingDump Delta;

	public readonly ModelChangeType Type;
	public ModelChangeEvent(PlayerModelTrainingDump stateBefore, PlayerModelTrainingDump stateAfter, ModelChangeType type) {
		this.Type = type;
		this.StateBefore = stateBefore;
		this.StateAfter = stateAfter;
		UpdateDelta();
	}

	public void UpdateDelta() {
		Delta = GetDelta (StateBefore, StateAfter);
	}

	public static PlayerModelTrainingDump GetDelta(PlayerModelTrainingDump stateBefore, PlayerModelTrainingDump stateAfter) {
		PlayerModelTrainingDump delta = new PlayerModelTrainingDump ();
		foreach (ResourceType res in Enum.GetValues(typeof(ResourceType))) {
			int deltaRes = stateAfter.GetCount (res) - stateBefore.GetCount (res);
			delta.Add (res, deltaRes);
		}
		return delta;
	}

	private string GetCausesString(List<ModelChangeEvent> events) {
		StringBuilder sb = new StringBuilder ("Score = ");
		sb.AppendLine (ScoreValue.ToString ());
		foreach (var item in Causes) {
			int ind = events.IndexOf (item.Key);
			sb.AppendFormat ("cause ind={0} val={1:0.####}\n", ind, item.Value);
		}
		if (Causes.Count == 0)
			sb.AppendLine ("No causes");
		return sb.ToString ();
	}
	public virtual string GetString (List<ModelChangeEvent> events)
	{
		return string.Format ("model change type = {0}, delta={1}\n {2}", Type.ToString(), Delta.ToString(), GetCausesString(events));
	}
}

class GameDecizionEvent : ModelChangeEvent {
	public TrainingDecisionModel DecisionTraining;
	public GameDecizionEvent(TrainingDecisionModel decisionTraining, PlayerModelTrainingDump stateBefore, PlayerModelTrainingDump stateAfter, ModelChangeType type):
		base(stateBefore, stateAfter, type) {
		this.DecisionTraining = decisionTraining;
	}
	public override string GetString (List<ModelChangeEvent> events)
	{
		return string.Format ("decision type = {0}, res={1}, {2}", DecisionTraining.Type.ToString (), DecisionTraining.Output, base.GetString (events));
	}
}

class GameProxyEvent : ModelChangeEvent {
	public readonly ModelChangeEvent Cause;
	public GameProxyEvent(ModelChangeEvent cause, PlayerModelTrainingDump stateBefore, PlayerModelTrainingDump stateAfter, ModelChangeType type):
		base(stateBefore, stateAfter, type) {
		this.Cause = cause;
	}
}