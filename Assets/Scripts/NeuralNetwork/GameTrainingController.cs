using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class GameTrainingController {
	public enum ResourceType {
		HumansCount,
		AvailableHumans,
		SpentOnHousing,
		SpentOnFields,
		SpentOnInstruments,
		SpentOnFood,
		SpentOnForest,
		SpentOnClay,
		SpentOnStone,
		SpentOnGold,
		SpentOnBuilding1,
		SpentOnBuilding2,
		SpentOnBuilding3,
		SpentOnBuilding4,
		SpentOnCard1,
		SpentOnCard2,
		SpentOnCard3,
		SpentOnCard4,
		Food,
		Forest,
		Clay,
		Stone,
		Gold,
		Fields,
		Instruments,
		InstrumentsAvailable,
		InstrumentsOnce,
		HumanMultiplier,
		InstrumentsMultiplier,
		HousesMultiplier,
		FieldsMultiplier,
		HousesCount,
		SciencesIn1stLine,
		SciencesIn2ndLine
	}
	public enum ModelChangeType {
		StartGame,
		StartRound,
		SetSpentHumans,
		ApplyGoToInstruments,
		ApplyGoToFields,
		ApplyGoToHousing,
		AnyResourcesFromCard,
		ResourcesMining,
		BonusFromOwnCharity,
		BonusFromOthersCharity,
		ResourcesFromCard,
		ApplyGoToHouse,
		ApplyGoToCard,
		ApplyCardGivenByCard,
		Feeding
	}
	private class PlayerModelTrainingDump {
		
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
	private abstract class GameEvent
	{
		public int ScoreValue;
		public Dictionary<GameEvent, float> Causes = new Dictionary<GameEvent, float>();
		public virtual string GetString(List<GameEvent> events) {
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
	}
	private class GameDecizionEvent : GameEvent {
		public TrainingDecisionModel DecisionTraining;
		public GameDecizionEvent(TrainingDecisionModel decisionTraining) {
			this.DecisionTraining = decisionTraining;
		}
		public override string GetString (List<GameEvent> events)
		{
			return string.Format ("decision type = {0}, res={1}, {2}", DecisionTraining.Type.ToString (), DecisionTraining.Output, base.GetString (events));
		}
	}

	private class ModelChangeEvent : GameEvent {
		public PlayerModelTrainingDump StateBefore;
		public PlayerModelTrainingDump StateAfter;
		public PlayerModelTrainingDump Delta;

		public readonly ModelChangeType Type; 
		public ModelChangeEvent(PlayerModelTrainingDump stateBefore, PlayerModelTrainingDump stateAfter, ModelChangeType type) {
			this.Type = type;
			this.StateBefore = stateBefore;
			this.StateAfter = stateAfter;
			Delta = new PlayerModelTrainingDump ();
			foreach (ResourceType res in Enum.GetValues(typeof(ResourceType)))
				Delta.Add (res, StateAfter.GetCount (res) - StateBefore.GetCount (res));
		}

		public override string GetString (List<GameEvent> events)
		{
			return string.Format ("model change type = {0}, delta={1}\n {2}", Type.ToString(), Delta.ToString(), base.GetString (events));
		}
	}
	private List<GameEvent> _events;
	private PlayerModel GetPlayer(Game game) {
		if (game.PlayerModels == null || game.PlayerModels.Count<4)
			return null;
		return game.PlayerModels [PlayerInd];
	}
	List<TrainingDecisionModel> _trainingModels;
	public ReadonlyList<TrainingDecisionModel> TrainingModels { get; private set; }
	public readonly int PlayerInd;
	private Game _game;
	public GameTrainingController(int playerInd, Game game) {
		this.PlayerInd = playerInd;
		this._game = game;
		_trainingModels = new List<TrainingDecisionModel>();
		TrainingModels = new ReadonlyList<TrainingDecisionModel> (_trainingModels);
		_events = new List<GameEvent> ();
	}
	private PlayerModelTrainingDump _beforeState;
	public void OnBeforeModelChange() {	
		PlayerModel player = GetPlayer (_game);
		if (player != null)
			_beforeState = GetDump (player);
		else
			_beforeState = new PlayerModelTrainingDump ();
	}
	public void OnAfterModelChange(ModelChangeType type) {
		PlayerModel player = GetPlayer (_game);	
		PlayerModelTrainingDump stateAfter = GetDump (player);
		ModelChangeEvent modelChange = new ModelChangeEvent (_beforeState, stateAfter, type);
		_events.Add (modelChange);
	}
	public void OnAfterDecision(TrainingDecisionModel trainingModel) {
		GameDecizionEvent decision = new GameDecizionEvent (trainingModel);
		_events.Add (decision);
		_trainingModels.Add (trainingModel);
	}
	private PlayerModelTrainingDump GetDump(PlayerModel player) {
		PlayerModelTrainingDump res = new PlayerModelTrainingDump ();
		res.Add (ResourceType.HumansCount, player.HumansCount);
		res.Add (ResourceType.AvailableHumans, player.UnspentHumanCount);
		res.Add (ResourceType.SpentOnHousing, player.SpentOnHousing);
		res.Add (ResourceType.SpentOnFields, player.SpentOnFields);
		res.Add (ResourceType.SpentOnInstruments, player.SpentOnInstruments);
		res.Add (ResourceType.SpentOnFood, player.SpentOnFood);
		res.Add (ResourceType.SpentOnForest, player.SpentOnForest);
		res.Add (ResourceType.SpentOnClay, player.SpentOnClay);
		res.Add (ResourceType.SpentOnStone, player.SpentOnStone);
		res.Add (ResourceType.SpentOnGold, player.SpentOnGold);
		res.Add (ResourceType.SpentOnBuilding1, player.SpentOnBuilding1);
		res.Add (ResourceType.SpentOnBuilding2, player.SpentOnBuilding2);
		res.Add (ResourceType.SpentOnBuilding3, player.SpentOnBuilding3);
		res.Add (ResourceType.SpentOnBuilding4, player.SpentOnBuilding4);
		res.Add (ResourceType.SpentOnCard1, player.SpentOnCard1);
		res.Add (ResourceType.SpentOnCard2, player.SpentOnCard2);
		res.Add (ResourceType.SpentOnCard3, player.SpentOnCard3);
		res.Add (ResourceType.SpentOnCard4, player.SpentOnCard4);
		res.Add (ResourceType.Food, player.Food);
		res.Add (ResourceType.Forest, player.Forest);
		res.Add (ResourceType.Clay, player.Clay);
		res.Add (ResourceType.Stone, player.Stone);
		res.Add (ResourceType.Gold, player.Gold);
		res.Add (ResourceType.Fields, player.FieldsCount);
		res.Add (ResourceType.Instruments, player.InstrumentsCountSlot1+player.InstrumentsCountSlot2+player.InstrumentsCountSlot3);
		res.Add (ResourceType.InstrumentsAvailable, player.GetAvailableInstruments(0)+player.GetAvailableInstruments(1)+player.GetAvailableInstruments(2));
		res.Add (ResourceType.InstrumentsOnce, player.GetAvailableOnceInstruments(0)+player.GetAvailableOnceInstruments(1)+player.GetAvailableOnceInstruments(2));
		res.Add (ResourceType.HumanMultiplier, player.HumansMultiplier);
		res.Add (ResourceType.InstrumentsMultiplier, player.InstrumentsMultiplier);
		res.Add (ResourceType.HousesMultiplier, player.HouseMultiplier);
		res.Add (ResourceType.FieldsMultiplier, player.FieldsMultiplier);
		res.Add (ResourceType.HousesCount, player.Houses.Count);
		res.Add (ResourceType.SciencesIn1stLine, player.GetSciencesCount(0));
		res.Add (ResourceType.SciencesIn2ndLine, player.GetSciencesCount(1));

		return res;
	}

	public void OnEndGame() {

		StringBuilder sb = new StringBuilder ("Training controller log start\n");
		sb.AppendFormat("Player ind = {0}\n", PlayerInd);
		foreach (var gameEvent in _events)
			sb.Append (gameEvent.GetString (_events));
		sb.AppendLine ("Training controller log end");
		Debug.Log (sb.ToString());
		// Calc game events causes.
		// Trainsitive closure for causality matrix.
		// Give score values to events.
		// Calc score values for each turn.
		// Fill training model rewards.


		/*
		 // Fill rewards.
		float[] rewards = new float[4];
		int maxScore = int.MinValue;
		for (int i = 0; i < Players.Count; i++) {
			if (Players [i].Model.Score>maxScore)
				maxScore = Players [i].Model.Score;
		}
		if (maxScore != 0) {
			for (int i = 0; i < Players.Count; i++) {
				rewards [i] = Players [i].Model.Score / (float)maxScore;
				const float winnerThreshold = 0.9f;
				rewards [i] -= winnerThreshold;
				if (rewards [i] > 0)
					rewards [i] *= 1 / (1 - winnerThreshold);
				else
					rewards [i] *= 1 / winnerThreshold;
			}
		} else {
			for (int i = 0; i < Players.Count; i++)
				rewards [i] = 0;
		}
		foreach (var trainingModel in TrainingModels)
			trainingModel.RewardPercent = rewards[trainingModel.PlayerInd];
		 */
	}
}
