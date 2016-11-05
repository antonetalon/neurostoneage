using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class GameTrainingController {
	
	private static Dictionary<ModelChangeType, List<ResourceType>> AllowedIncs = new Dictionary<ModelChangeType, List<ResourceType>>() {
		{ModelChangeType.StartGame, new List<ResourceType>(){ ResourceType.Food, ResourceType.HumanMultiplier, ResourceType.InstrumentsMultiplier, 
				ResourceType.HousesMultiplier, ResourceType.FieldsMultiplier, ResourceType.HumansCount }},
		{ModelChangeType.StartRound, new List<ResourceType>(){ ResourceType.AvailableHumans }},
		{ModelChangeType.EndTurn, new List<ResourceType>(){/*its ok*/}},
		{ModelChangeType.AddHumansFromHousing, new List<ResourceType>(){ ResourceType.AvailableHumans }},
		{ModelChangeType.AddAvailableInstruments, new List<ResourceType>(){ ResourceType.InstrumentsAvailable }},
		{ModelChangeType.AddUnspentFields, new List<ResourceType>(){ ResourceType.UnspentFields }},
		{ModelChangeType.WhereToGoSelected, new List<ResourceType>(){ ResourceType.SpendingOnHousing,ResourceType.SpendingOnFields,ResourceType.SpendingOnInstruments,ResourceType.SpendingOnFood,
				ResourceType.SpendingOnForest,ResourceType.SpendingOnClay,ResourceType.SpendingOnStone,ResourceType.SpendingOnGold,ResourceType.SpendingOnBuilding1,
				ResourceType.SpendingOnBuilding2,ResourceType.SpendingOnBuilding3,ResourceType.SpendingOnBuilding4,ResourceType.SpendingOnCard1,ResourceType.SpendingOnCard2,
				ResourceType.SpendingOnCard3, ResourceType.SpendingOnCard4}},
		{ModelChangeType.SetSpentHumans, new List<ResourceType>(){ ResourceType.SpentOnHousing, ResourceType.SpentOnFields, ResourceType.SpentOnInstruments, 
				ResourceType.SpentOnFood, ResourceType.SpentOnForest, ResourceType.SpentOnClay, ResourceType.SpentOnStone, ResourceType.SpentOnGold, 
				ResourceType.SpentOnBuilding1, ResourceType.SpentOnBuilding2, ResourceType.SpentOnBuilding3, ResourceType.SpentOnBuilding4,
				ResourceType.SpentOnCard1, ResourceType.SpentOnCard2, ResourceType.SpentOnCard3, ResourceType.SpentOnCard4 }},
		{ModelChangeType.ApplyGoToMining, new List<ResourceType>(){ ResourceType.DicePoints }},
		{ModelChangeType.ApplyGoToInstruments, new List<ResourceType>(){ ResourceType.Instruments }},
		{ModelChangeType.ApplyGoToFields, new List<ResourceType>(){ ResourceType.Fields }},
		{ModelChangeType.ApplyGoToHousing, new List<ResourceType>(){ ResourceType.HumansCount }},
		{ModelChangeType.ApplyGoToHouse, new List<ResourceType>(){ ResourceType.HousesCount, ResourceType.Score }},
		{ModelChangeType.ApplyGoToCard, new List<ResourceType>(){ ResourceType.InstrumentsOnce, ResourceType.Charity, ResourceType.Score, 
				ResourceType.SpentOnForest, ResourceType.SpentOnClay, ResourceType.SpentOnStone, ResourceType.SpentOnGold,
				ResourceType.Food,ResourceType.Forest, ResourceType.Clay, ResourceType.Stone, ResourceType.Gold,
				ResourceType.HumanMultiplier, ResourceType.InstrumentsMultiplier, ResourceType.HousesMultiplier, ResourceType.HousesMultiplier, 
				ResourceType.SciencesIn1stLine, ResourceType.SciencesIn2ndLine, ResourceType.Any2ResourcesFromCard, ResourceType.OneCardBottomMore }},
		{ModelChangeType.ApplyingInstruments, new List<ResourceType>(){ResourceType.DicePoints}},
		{ModelChangeType.ResourcesMining, new List<ResourceType>(){ ResourceType.Food, ResourceType.Forest, ResourceType.Clay, ResourceType.Stone, ResourceType.Gold}},
		{ModelChangeType.ReceiveAny2ResFromCard, new List<ResourceType>(){ ResourceType.Forest, ResourceType.Clay, ResourceType.Stone, ResourceType.Gold, ResourceType.Food }},
		{ModelChangeType.BonusFromOwnCharity, new List<ResourceType>(){ResourceType.Forest, ResourceType.Clay, ResourceType.Stone, ResourceType.Gold, ResourceType.Instruments, ResourceType.Fields }},
		{ModelChangeType.BonusFromOthersCharity, new List<ResourceType>(){ ResourceType.Forest, ResourceType.Clay, ResourceType.Stone, ResourceType.Gold, ResourceType.Instruments, ResourceType.Fields }},
		{ModelChangeType.ApplyCardFromOtherCard, new List<ResourceType>(){ ResourceType.HumanMultiplier, ResourceType.InstrumentsMultiplier, ResourceType.HousesMultiplier, ResourceType.HousesMultiplier, 
				ResourceType.SciencesIn1stLine, ResourceType.SciencesIn2ndLine }},
		{ModelChangeType.Feeding, new List<ResourceType>(){ ResourceType.Score}}
	};
	private static Dictionary<ModelChangeType, List<ResourceType>> AllowedDecs = new Dictionary<ModelChangeType, List<ResourceType>>() {
		{ModelChangeType.StartGame, new List<ResourceType>(){/*its ok*/ }},
		{ModelChangeType.StartRound, new List<ResourceType>(){/*its ok*/ }},
		{ModelChangeType.EndTurn, new List<ResourceType>(){ ResourceType.AvailableHumans, ResourceType.InstrumentsAvailable }},
		{ModelChangeType.AddHumansFromHousing, new List<ResourceType>(){/*its ok*/ }},
		{ModelChangeType.AddAvailableInstruments, new List<ResourceType>(){/*its ok*/ }},
		{ModelChangeType.AddUnspentFields, new List<ResourceType>(){/*its ok*/ }},
		{ModelChangeType.WhereToGoSelected, new List<ResourceType>(){/*its ok*/ }},
		{ModelChangeType.SetSpentHumans, new List<ResourceType>(){ ResourceType.AvailableHumans, ResourceType.SpendingOnHousing,ResourceType.SpendingOnFields,ResourceType.SpendingOnInstruments,
				ResourceType.SpendingOnFood, ResourceType.SpendingOnForest,ResourceType.SpendingOnClay,ResourceType.SpendingOnStone,ResourceType.SpendingOnGold,ResourceType.SpendingOnBuilding1,
				ResourceType.SpendingOnBuilding2,ResourceType.SpendingOnBuilding3,ResourceType.SpendingOnBuilding4,ResourceType.SpendingOnCard1,ResourceType.SpendingOnCard2,
				ResourceType.SpendingOnCard3, ResourceType.SpendingOnCard4}},
		{ModelChangeType.ApplyGoToInstruments, new List<ResourceType>(){ResourceType.SpentOnInstruments, ResourceType.InstrumentsAvailable }},
		{ModelChangeType.ApplyGoToFields, new List<ResourceType>(){ ResourceType.SpentOnFields }},
		{ModelChangeType.ApplyGoToHousing, new List<ResourceType>(){ ResourceType.SpentOnHousing }},
		{ModelChangeType.ApplyGoToHouse, new List<ResourceType>(){ ResourceType.Forest, ResourceType.Clay, ResourceType.Stone, ResourceType.Gold, ResourceType.SpentOnBuilding1, 
				ResourceType.SpentOnBuilding2, ResourceType.SpentOnBuilding3, ResourceType.SpentOnBuilding4 }},
		{ModelChangeType.ApplyGoToCard, new List<ResourceType>(){ ResourceType.Forest, ResourceType.Clay, ResourceType.Stone, ResourceType.Gold, 
				ResourceType.SpentOnCard1, ResourceType.SpentOnCard2, ResourceType.SpentOnCard3, ResourceType.SpentOnCard4 }},
		{ModelChangeType.ApplyGoToMining, new List<ResourceType>(){ ResourceType.SpentOnFood, ResourceType.SpentOnForest, ResourceType.SpentOnClay, ResourceType.SpentOnStone, ResourceType.SpentOnGold }},
		{ModelChangeType.ApplyingInstruments, new List<ResourceType>(){ ResourceType.InstrumentsAvailable, ResourceType.InstrumentsOnce }},
		{ModelChangeType.ResourcesMining, new List<ResourceType>(){ ResourceType.DicePoints }},
		{ModelChangeType.ReceiveAny2ResFromCard, new List<ResourceType>(){ ResourceType.Any2ResourcesFromCard }},
		{ModelChangeType.BonusFromOwnCharity, new List<ResourceType>(){ ResourceType.Charity }},
		{ModelChangeType.BonusFromOthersCharity, new List<ResourceType>(){/*its ok*/ }},
		{ModelChangeType.ApplyCardFromOtherCard, new List<ResourceType>(){ ResourceType.OneCardBottomMore }},
		{ModelChangeType.Feeding, new List<ResourceType>(){ResourceType.Food, ResourceType.UnspentFields, ResourceType.Forest, ResourceType.Clay, ResourceType.Stone, ResourceType.Gold }}
	};
	private static List<ResourceType> externalResourcesList = new List<ResourceType>() { ResourceType.Charity, ResourceType.DicePoints, ResourceType.Any2ResourcesFromCard, 
		ResourceType.Score, ResourceType.OneCardBottomMore, ResourceType.SpendingOnHousing, ResourceType.SpendingOnFields, ResourceType.SpendingOnInstruments, 
		ResourceType.SpendingOnFood, ResourceType.SpendingOnForest, ResourceType.SpendingOnClay, ResourceType.SpendingOnStone, ResourceType.SpendingOnGold, 
		ResourceType.SpendingOnBuilding1, ResourceType.SpendingOnBuilding2, ResourceType.SpendingOnBuilding3, ResourceType.SpendingOnBuilding4, 
		ResourceType.SpendingOnCard1, ResourceType.SpendingOnCard2, ResourceType.SpendingOnCard3, ResourceType.SpendingOnCard4};

	private List<ModelChangeEvent> GetEventsWithResourceChange(ResourceType res, bool incOrDec) {
		List<ModelChangeEvent> resChangeEvents = new List<ModelChangeEvent> ();
		for (int i = 0; i < _events.Count; i++) {
			if (_events [i].Type == ModelChangeType.StartGame)
				continue; // Do not rely on start game in any case.
			int delta = _events [i].Delta.GetCount (res);
			if ((delta > 0 && incOrDec) || (delta < 0 && !incOrDec))
				resChangeEvents.Add (_events [i]);
		}
		return resChangeEvents;
	}

	private List<ModelChangeEvent> _events;
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
		_events = new List<ModelChangeEvent> ();
	}
	private PlayerModelTrainingDump _beforeState;
	public void OnBeforeModelChange() {	
		if (_events.Count > 0)
			_beforeState = _events [_events.Count - 1].StateAfter.Clone ();
		else {
			PlayerModel player = GetPlayer (_game);
			if (player != null)
				_beforeState = GetDump (player);
			else
				_beforeState = new PlayerModelTrainingDump ();
		}
	}
	public void OnAfterModelChange(ModelChangeType type, Dictionary<ResourceType, int> additionalResourceChanges = null) {
		OnAfterModelChangePrivate (type, additionalResourceChanges);
		CheckLastEventsConsistency (1);
	}
	private void OnAfterModelChangePrivate(ModelChangeType type, Dictionary<ResourceType, int> additionalResourceChanges = null) {
		PlayerModel player = GetPlayer (_game);	
		PlayerModelTrainingDump stateAfter = GetDump (player);
		if (additionalResourceChanges != null) {
			foreach (var item in additionalResourceChanges)
				stateAfter.ChangeCount (item.Key, item.Value);
		}
		ModelChangeEvent modelChange = new ModelChangeEvent (_beforeState, stateAfter, type);
		_events.Add (modelChange);
	}
	public void OnAfterDecision(TrainingDecisionModel trainingModel, ModelChangeType type, Dictionary<ResourceType, int> additionalResourceChanges = null) {
		OnAfterDecisionPrivate (trainingModel, type, additionalResourceChanges);
		CheckLastEventsConsistency (1);
	}
	private void OnAfterDecisionPrivate(TrainingDecisionModel trainingModel, ModelChangeType type, Dictionary<ResourceType, int> additionalResourceChanges = null) {
		PlayerModel player = GetPlayer (_game);	
		PlayerModelTrainingDump stateAfter = GetDump (player);
		if (additionalResourceChanges != null) {
			foreach (var item in additionalResourceChanges)
				stateAfter.ChangeCount (item.Key, item.Value);
		}
		GameDecizionEvent decision = new GameDecizionEvent (trainingModel, _beforeState, stateAfter, type);
		_events.Add (decision);
		_trainingModels.Add (trainingModel);
	}
	private void CheckLastEventsConsistency(int count) {
		for (int i = _events.Count - count; i < _events.Count; i++)
			CheckEventConsistency (i);
	}
	private void CheckEventConsistency (int i) {
		if (PlayerInd != 0)
			return; // Check only human player.
		// Checking that on start of curr event model was the same as at the end of prev.
		PlayerModelTrainingDump endPrevEvent = null;
		if (i > 0) {
			endPrevEvent = _events [i - 1].StateAfter;
			//Debug.LogFormat ("After {0}-th turn {1}", i - 1, endPrevEvent.ToString ());
		}

		PlayerModelTrainingDump startCurrEvent = _events[i].StateBefore;
		//Debug.LogFormat ("Before {0}-th turn {1}", i, startCurrEvent.ToString());

		if (endPrevEvent == null)
			return;
		PlayerModelTrainingDump delta = ModelChangeEvent.GetDelta (endPrevEvent, startCurrEvent);
		if (!delta.IsEmpty)
			Debug.LogFormat ("NON-NULL DELTA = {0}", delta.ToString());

		Debug.LogFormat ("event[{0}] type = {2} delta = {1}", i, _events[i].Delta.ToString(), _events[i].Type);
	}
	public void OnAfterStartTurn() {
		PlayerModel player = GetPlayer (_game);	
		// Other turn start.
		OnAfterModelChangePrivate(ModelChangeType.StartRound);
		ModelChangeEvent startTurnChange = _events [_events.Count - 1];
		PlayerModelTrainingDump stateAfterStartTurn = startTurnChange.StateAfter;
		int removedAvailableHumans = RemoveRenewableResource(ResourceType.HumansCount, ResourceType.AvailableHumans, 5, stateAfterStartTurn);
		int removedUnspentFields = RemoveRenewableResource(ResourceType.Fields, ResourceType.UnspentFields, 0, stateAfterStartTurn);
		int removedAvailableInstruments = RemoveRenewableResource(ResourceType.Instruments, ResourceType.InstrumentsAvailable, 0, stateAfterStartTurn);
		// Add humans from turns.
		AddLinkEventsForRenewableResources(ResourceType.HumansCount, ResourceType.AvailableHumans, 5, ModelChangeType.AddHumansFromHousing, removedAvailableHumans);
		// Add unspent fields.
		AddLinkEventsForRenewableResources(ResourceType.Fields, ResourceType.UnspentFields, 0, ModelChangeType.AddUnspentFields, removedUnspentFields);
		// Add instruments from turns.
		AddLinkEventsForRenewableResources(ResourceType.Instruments, ResourceType.InstrumentsAvailable, 0, ModelChangeType.AddAvailableInstruments, removedAvailableInstruments);
		// stateAfterStartTurn changed, need to update start turn event.
		startTurnChange.UpdateDelta ();
		CheckLastEventsConsistency (1+player.HumansCount-5+player.FieldsCount+player.InstrumentsCountSlot1+player.InstrumentsCountSlot2+player.InstrumentsCountSlot3);
	}
	public void OnAfterWhereToGoSelected(TrainingDecisionModel trainingModel, WhereToGo target) {
		ResourceType addedResource;
		switch (target) {
			case WhereToGo.Card1: addedResource = ResourceType.SpendingOnCard1; break;
			case WhereToGo.Card2: addedResource = ResourceType.SpendingOnCard2; break;
			case WhereToGo.Card3: addedResource = ResourceType.SpendingOnCard3; break;
			case WhereToGo.Card4: addedResource = ResourceType.SpendingOnCard4; break;
			case WhereToGo.Field: addedResource = ResourceType.SpendingOnFields; break;
			case WhereToGo.Food: addedResource = ResourceType.SpendingOnFood; break;
			case WhereToGo.Forest: addedResource = ResourceType.SpendingOnForest; break;
			case WhereToGo.Clay: addedResource = ResourceType.SpendingOnClay; break;
			case WhereToGo.Stone: addedResource = ResourceType.SpendingOnStone; break;
			case WhereToGo.Gold: addedResource = ResourceType.SpendingOnGold; break;
			case WhereToGo.House1: addedResource = ResourceType.SpendingOnBuilding1; break;
			case WhereToGo.House2: addedResource = ResourceType.SpendingOnBuilding2; break;
			case WhereToGo.House3: addedResource = ResourceType.SpendingOnBuilding3; break;
			case WhereToGo.House4: addedResource = ResourceType.SpendingOnBuilding4; break;
			case WhereToGo.Housing: addedResource = ResourceType.SpendingOnHousing; break;
			case WhereToGo.Instrument: addedResource = ResourceType.SpendingOnInstruments; break;
			default:
				Debug.LogError ("WTF");
				addedResource = ResourceType.Forest;
				break;
		}
		OnAfterDecisionPrivate(trainingModel, ModelChangeType.WhereToGoSelected, new Dictionary<ResourceType, int>() { {addedResource, 1} });
		CheckLastEventsConsistency (1);
	}
	public void OnAfterHumansCountSelected() {
		PlayerModelTrainingDump stateAfter = GetDump (GetPlayer (_game));
		PlayerModelTrainingDump delta = ModelChangeEvent.GetDelta (_beforeState, stateAfter);
		ResourceType removedResource;
		if (delta.GetCount (ResourceType.SpentOnCard1) > 0)
			removedResource = ResourceType.SpendingOnCard1;
		else if (delta.GetCount (ResourceType.SpentOnCard2) > 0)
			removedResource = ResourceType.SpendingOnCard2;
		else if (delta.GetCount (ResourceType.SpentOnCard3) > 0)
			removedResource = ResourceType.SpendingOnCard3;
		else if (delta.GetCount (ResourceType.SpentOnCard4) > 0)
			removedResource = ResourceType.SpendingOnCard4;		
		else if (delta.GetCount (ResourceType.SpentOnFields) > 0)
			removedResource = ResourceType.SpendingOnFields;
		else if (delta.GetCount (ResourceType.SpentOnFood) > 0)
			removedResource = ResourceType.SpendingOnFood;
		else if (delta.GetCount (ResourceType.SpentOnForest) > 0)
			removedResource = ResourceType.SpendingOnForest;
		else if (delta.GetCount (ResourceType.SpentOnClay) > 0)
			removedResource = ResourceType.SpendingOnClay;
		else if (delta.GetCount (ResourceType.SpentOnStone) > 0)
			removedResource = ResourceType.SpendingOnStone;
		else if (delta.GetCount (ResourceType.SpentOnGold) > 0)
			removedResource = ResourceType.SpendingOnGold;
		else if (delta.GetCount (ResourceType.SpentOnBuilding1) > 0)
			removedResource = ResourceType.SpendingOnBuilding1;
		else if (delta.GetCount (ResourceType.SpentOnBuilding2) > 0)
			removedResource = ResourceType.SpendingOnBuilding2;
		else if (delta.GetCount (ResourceType.SpentOnBuilding3) > 0)
			removedResource = ResourceType.SpendingOnBuilding3;
		else if (delta.GetCount (ResourceType.SpentOnBuilding4) > 0)
			removedResource = ResourceType.SpendingOnBuilding4;
		else if (delta.GetCount (ResourceType.SpentOnHousing) > 0)
			removedResource = ResourceType.SpendingOnHousing;
		else if (delta.GetCount (ResourceType.SpentOnInstruments) > 0)
			removedResource = ResourceType.SpendingOnInstruments;
		else {
			Debug.LogError ("WTF");
			removedResource = ResourceType.AvailableHumans;
		}

		OnAfterModelChangePrivate(ModelChangeType.SetSpentHumans, new Dictionary<ResourceType, int>() { {removedResource, -1} });
		CheckLastEventsConsistency (1);
	}
	private int RemoveRenewableResource(ResourceType renewableSource, ResourceType renewableDest, int startSourceCount, PlayerModelTrainingDump stateAfterStartTurn) {
		int sourceCount = stateAfterStartTurn.GetCount (renewableSource);
		int linkedCount = Mathf.Max(0, sourceCount - startSourceCount); 
		if (linkedCount>0)
			stateAfterStartTurn.ChangeCount(renewableDest, -linkedCount); // Remove human from start turn.
		return linkedCount;
	}
	private void AddLinkEventsForRenewableResources(ResourceType renewableSource, ResourceType renewableDest, int startSourceCount, ModelChangeType changeType, int linkedCount) {
		List<ModelChangeEvent> renewableSourceAddingEvents = GetEventsWithResourceChange(renewableSource, true);
		PlayerModelTrainingDump prevState = _events[_events.Count-1].StateAfter;
		int sourceCount = prevState.GetCount (renewableSource);
		if (linkedCount>0) {
			PlayerModelTrainingDump currState = prevState.Clone ();
			for (int i = startSourceCount; i < sourceCount; i++) {			
				// Create model change linked to renewableSource gaining turn.
				PlayerModelTrainingDump nextState = currState.Clone();
				nextState.ChangeCount (renewableDest, 1);
				GameProxyEvent addingRenewableResourceEvent = new GameProxyEvent (renewableSourceAddingEvents [i - startSourceCount], currState, nextState, changeType);
				_events.Add (addingRenewableResourceEvent);
				currState = nextState;
			}
		}
	}
	public void OnReceived2ResFromCard() {
		OnAfterModelChangePrivate(ModelChangeType.ReceiveAny2ResFromCard, new Dictionary<ResourceType, int>() { {ResourceType.Any2ResourcesFromCard, -1} });
		CheckLastEventsConsistency (1);
	}
	public void OnAfterResourceDicesRolled(int dotsSum) {
		OnAfterModelChangePrivate(ModelChangeType.ApplyGoToMining, new Dictionary<ResourceType, int>() { {ResourceType.DicePoints, dotsSum} });
		CheckLastEventsConsistency (1);
	}
	public void OnAfterInstrumentsApplied(TrainingDecisionModel trainingModel, int instrumentsAdded) {
		OnAfterDecisionPrivate(trainingModel, ModelChangeType.ApplyingInstruments, new Dictionary<ResourceType, int>() { {ResourceType.DicePoints, instrumentsAdded} });
		CheckLastEventsConsistency (1);
	}
	public void OnAfterResourcesMined() {
		PlayerModel player = GetPlayer (_game);
		OnAfterModelChangePrivate(ModelChangeType.ResourcesMining, new Dictionary<ResourceType, int>() { {ResourceType.DicePoints, _beforeState.GetCount(ResourceType.DicePoints)} });
		CheckLastEventsConsistency (1);
	}
	public void OnHumanRemovedFromCard(int cardSlotInd) {
		OnHumanRemovedFromCardPrivate (cardSlotInd);
		CheckLastEventsConsistency (1);
	}
	public void OnHumanRemovedFromCardPrivate(int cardSlotInd) {
		PlayerModel player = GetPlayer (_game);
		ResourceType res;
		switch (cardSlotInd) {
			default:
			case 0: res = ResourceType.SpendingOnCard1; break;
			case 1: res = ResourceType.SpendingOnCard2; break;
			case 2: res = ResourceType.SpendingOnCard3; break;
			case 3: res = ResourceType.SpendingOnCard4; break;
		}
		OnAfterModelChangePrivate(ModelChangeType.ApplyGoToCard, new Dictionary<ResourceType, int>() { {res, -1} });
	}
	public void OnAfterCardBought(int cardSlotInd, CardToBuild card) {
		OnHumanRemovedFromCardPrivate (cardSlotInd);
		ModelChangeEvent cardBuyEvent = _events [_events.Count - 1];

		if (card.TopFeature == TopCardFeature.RandomForEveryone)
			cardBuyEvent.StateAfter.ChangeCount (ResourceType.Charity, 1);

		if (card.TopFeature == TopCardFeature.Score)
			cardBuyEvent.StateAfter.ChangeCount (ResourceType.Score, card.TopFeatureParam);

		if (card.TopFeature == TopCardFeature.ResourceAny)
			cardBuyEvent.StateAfter.ChangeCount (ResourceType.Any2ResourcesFromCard, 1);

		if (card.TopFeature == TopCardFeature.OneCardMore)
			cardBuyEvent.StateAfter.ChangeCount (ResourceType.OneCardBottomMore, 1);

		cardBuyEvent.UpdateDelta ();
		CheckLastEventsConsistency (1);
	}
	public void OnAfterCharityReceived(TrainingDecisionModel trainingModel, bool own) {
		ModelChangeType type = own ? ModelChangeType.BonusFromOwnCharity : ModelChangeType.BonusFromOthersCharity;
		OnAfterDecisionPrivate (trainingModel, type, new Dictionary<ResourceType, int> (){ { ResourceType.Charity, -1 } });
		CheckLastEventsConsistency (1);
	}
	public void OnAfterCardFromCardApplied() {
		OnAfterModelChangePrivate (ModelChangeType.ApplyCardFromOtherCard, new Dictionary<ResourceType, int> (){ { ResourceType.OneCardBottomMore, -1 }});
		CheckLastEventsConsistency (1);
	}

	public void OnHumanRemovedFromHouse(int houseSlotInd) {
		PlayerModel player = GetPlayer (_game);
		ResourceType res;
		switch (houseSlotInd) {
			default:
			case 0: res = ResourceType.SpendingOnBuilding1; break;
			case 1: res = ResourceType.SpendingOnBuilding2; break;
			case 2: res = ResourceType.SpendingOnBuilding3; break;
			case 3: res = ResourceType.SpendingOnBuilding4; break;
		}
		OnAfterModelChangePrivate(ModelChangeType.ApplyGoToHouse, new Dictionary<ResourceType, int>() { {res, -1} });
		CheckLastEventsConsistency (1);
	}
	public void OnAfterHouseBought(int houseSlotInd, List<Resource> spentResources) {
		int scoreFromResources = 0;
		foreach (var res in spentResources)
			scoreFromResources += Game.GetResourceCost (res);
		OnHumanRemovedFromHouse (houseSlotInd);
		ModelChangeEvent houseBuyEvent = _events [_events.Count - 1];

		houseBuyEvent.StateAfter.ChangeCount (ResourceType.Score, scoreFromResources);

		houseBuyEvent.UpdateDelta ();
		CheckLastEventsConsistency (1);
	}
	public void OnAfterFeeding(TrainingDecisionModel trainingModel, bool hungry) {
		Dictionary<ResourceType, int> additionalResources = null;
		if (!hungry) {
			int scoreToAdd = 10;
			additionalResources = new Dictionary<ResourceType, int> (){{ ResourceType.Score, scoreToAdd } };
		}
		OnAfterDecisionPrivate (trainingModel, ModelChangeType.Feeding, additionalResources);
		CheckLastEventsConsistency (1);
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
		if (_events.Count > 0) {
			PlayerModelTrainingDump prevDump = _events [_events.Count - 1].StateAfter;
			foreach (var externalRes in externalResourcesList)
				res.Add (externalRes, prevDump.GetCount (externalRes));
		}
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
		List<List<ModelChangeEvent>> causes = new List<List<ModelChangeEvent>> ();

	}
	/*private WhereToGo GetTarget(ModelChangeType type, int cardHouseInd) {
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
	}*/
}
