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
		SciencesIn2ndLine,
		UnspentFields
	}
	public enum ModelChangeType {
		StartGame,
		StartRound,
		SetSpentHumans,
		ApplyGoToInstruments,
		ApplyGoToFields,
		ApplyGoToHousing,
		AnyResourcesFromCard,
		ApplyingInstruments,
		ResourcesMining,
		BonusFromOwnCharity,
		BonusFromOthersCharity,
		ResourcesFromCard,
		ApplyGoToHouse,
		ApplyGoToCard,
		ApplyCardGivenByCard,
		Feeding,
		EndTurn,
		AddHumansFromHousing,
		AddInstrumentsFromGoToInstruments,
		AddFoodFromFields
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
		public readonly int CardHouseInd;
		public ModelChangeEvent(PlayerModelTrainingDump stateBefore, PlayerModelTrainingDump stateAfter, ModelChangeType type, int cardHouseInd = -1) {
			this.Type = type;
			this.StateBefore = stateBefore;
			this.StateAfter = stateAfter;
			this.CardHouseInd = cardHouseInd;
			Delta = new PlayerModelTrainingDump ();
			foreach (ResourceType res in Enum.GetValues(typeof(ResourceType))) {
				int delta = StateAfter.GetCount (res) - StateBefore.GetCount (res);
				Delta.Add (res, delta);
			}
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
	public void OnAfterModelChange(ModelChangeType type, int cardHouseInd = -1) {
		PlayerModel player = GetPlayer (_game);	
		PlayerModelTrainingDump stateAfter = GetDump (player);
		ModelChangeEvent modelChange = new ModelChangeEvent (_beforeState, stateAfter, type, cardHouseInd);
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
		res.Add (ResourceType.UnspentFields, player.UnSpentFields);
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
		for (int eventInd = 0; eventInd < _events.Count; eventInd++) {
			FindEventCauses (eventInd);
		}
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
	void FindEventCauses(int eventInd) {
		List<List<GameEvent>> causes = new List<List<GameEvent>> ();
		GameDecizionEvent decision;
		WhereToGo target;
		ModelChangeEvent currEvent = _events [eventInd] as ModelChangeEvent;
		if (currEvent == null) {
			decision = _events [eventInd] as GameDecizionEvent;
			// Decisions have circular cause.
			causes.Add(new List<GameEvent>(){ decision });
		} else {			
			switch (currEvent.Type) {
			case ModelChangeType.AnyResourcesFromCard:
			case ModelChangeType.ApplyCardGivenByCard:
			case ModelChangeType.ResourcesFromCard:
				// Cause is going on that card decision.
				ModelChangeEvent prevApplyCard = GetClosestPastEvent (eventInd, (ModelChangeEvent gotoCardEvent) => {
					if (gotoCardEvent.Type != ModelChangeType.ApplyGoToCard)
						return false;
					return currEvent.CardHouseInd == gotoCardEvent.CardHouseInd;
				});
				if (prevApplyCard == null)
					Debug.LogError ("Apply card not found");
				causes.Add(new List<GameEvent>(){ prevApplyCard });
				break;
			case ModelChangeType.ApplyGoToCard:
			case ModelChangeType.ApplyGoToFields:
			case ModelChangeType.ApplyGoToHouse:
			case ModelChangeType.ApplyGoToHousing:
			case ModelChangeType.ApplyGoToInstruments:
				// Cause is prev decision where to go.
				target = GetTarget(currEvent.Type, currEvent.CardHouseInd);
				decision = GetPrevDecision (eventInd, DecisionType.SelectWhereToGo, (int)target);
				if (decision == null)
					Debug.LogError ("where to go not found");
				causes.Add(new List<GameEvent>(){ decision });
				break;
			case ModelChangeType.BonusFromOthersCharity:
				// Cause is charity selection.
				decision = GetPrevDecision (eventInd, DecisionType.SelectCharity, -1);
				if (decision == null)
					Debug.LogError ("charity selection not found");
				causes.Add(new List<GameEvent>(){ decision });
				break;
			case ModelChangeType.BonusFromOwnCharity:
				// Cause is charity selection and where to go selection on charity.
				decision = GetPrevDecision (eventInd, DecisionType.SelectCharity, -1);
				if (decision == null)
					Debug.LogError ("charity selection not found");
				target = GetTarget(currEvent.Type, currEvent.CardHouseInd);
				GameDecizionEvent decisionWhereToGo = GetPrevDecision (eventInd, DecisionType.SelectWhereToGo, (int)target);
				if (decision == null)
					Debug.LogError ("where to go not found");
				causes.Add(new List<GameEvent>(){ decision, decisionWhereToGo });
				break;
			case ModelChangeType.Feeding:
				//  Cause is curr turn selection on leaving hungry and all turns that give fields.
				decision = GetPrevCurrTurnDecision (eventInd, DecisionType.SelectLeaveHungry, -1);
				// TODO: Add giving fields.
				if (decision != null)
					causes.Add(new List<GameEvent>(){ decision });
				break;
			case ModelChangeType.ResourcesMining:
				// Causes are decisions where to go, spending humans, using instruments.
				GameDecizionEvent humansCountDecision = GetPrevDecision (eventInd, DecisionType.SelectUsedHumans, -1, (GameDecizionEvent currSpentHumansDesicion)=>{					
					if (currEvent.Delta.GetCount(ResourceType.SpentOnFood)!=0)
						return currEvent.Delta.GetCount(ResourceType.SpentOnFood)!=0;
					if (currEvent.Delta.GetCount(ResourceType.SpentOnForest)!=0)
						return currEvent.Delta.GetCount(ResourceType.SpentOnForest)!=0;
					if (currEvent.Delta.GetCount(ResourceType.SpentOnClay)!=0)
						return currEvent.Delta.GetCount(ResourceType.SpentOnClay)!=0;
					if (currEvent.Delta.GetCount(ResourceType.SpentOnStone)!=0)
						return currEvent.Delta.GetCount(ResourceType.SpentOnStone)!=0;
					if (currEvent.Delta.GetCount(ResourceType.SpentOnGold)!=0)
						return currEvent.Delta.GetCount(ResourceType.SpentOnGold)!=0;
					return false;
				});
				GameDecizionEvent whereToGoDecision = GetPrevDecision(humansCountDecision);
				if (whereToGoDecision == null)
					Debug.LogError ("where to go not found");
				GameDecizionEvent instrumentsDecision = GetNextDecision(humansCountDecision);
				if (instrumentsDecision == null)
					Debug.LogError ("instruments found");
				causes.Add(new List<GameEvent>(){ whereToGoDecision, humansCountDecision, instrumentsDecision });
				break;
			case ModelChangeType.SetSpentHumans:
				// Cause is prev decision - where to go.
				GameDecizionEvent whereToGoDecision2 = GetPrevDecision(humansCountDecision);
				if (whereToGoDecision2 == null)
					Debug.LogError ("where to go not found");
				causes.Add(new List<GameEvent>(){ whereToGoDecision, humansCountDecision });
				break;
			}

			// TODO: Add turns that gave resources.
		}
		// TODO: Add turns that add to next turns humans and instruments.
		// TODO: Calc weights and fill _events [eventInd].Causes
	}
	private WhereToGo  GetTarget(ModelChangeType type, int cardHouseInd) {
		switch (type) {
		case ModelChangeType.BonusFromOwnCharity:
		case ModelChangeType.ApplyGoToCard:
			switch (cardHouseInd) {
				default:
				case 0:	return WhereToGo.Card1;
				case 1:	return WhereToGo.Card2;
				case 2:	return WhereToGo.Card3;
				case 3:	return WhereToGo.Card4;
			}
			break;
		case ModelChangeType.ApplyGoToFields:
			return WhereToGo.Field;
			break;
		case ModelChangeType.ApplyGoToHouse:
			switch (cardHouseInd) {
				default:
				case 0:	return WhereToGo.House1;
				case 1:	return WhereToGo.House2;
				case 2:	return WhereToGo.House3;
				case 3:	return WhereToGo.House4;
			}
			break;
		case ModelChangeType.ApplyGoToHousing:
			return WhereToGo.Housing;
			break;
		case ModelChangeType.ApplyGoToInstruments:
			return WhereToGo.Instrument;
			break;
		}
	}
}
