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
		{ModelChangeType.ApplyGoToFields, new List<ResourceType>(){ ResourceType.Fields, ResourceType.UnspentFields }},
		{ModelChangeType.ApplyGoToHousing, new List<ResourceType>(){ ResourceType.HumansCount }},
		{ModelChangeType.ApplyGoToHouse, new List<ResourceType>(){ ResourceType.HousesCount, ResourceType.Score }},
		{ModelChangeType.ApplyGoToCard, new List<ResourceType>(){ ResourceType.InstrumentsOnce, ResourceType.Charity, ResourceType.Score, 
				ResourceType.SpentOnForest, ResourceType.SpentOnClay, ResourceType.SpentOnStone, ResourceType.SpentOnGold,
				ResourceType.Food,ResourceType.Forest, ResourceType.Clay, ResourceType.Stone, ResourceType.Gold,
				ResourceType.HumanMultiplier, ResourceType.InstrumentsMultiplier, ResourceType.HousesMultiplier, ResourceType.FieldsMultiplier, 
				ResourceType.SciencesIn1stLine, ResourceType.SciencesIn2ndLine, ResourceType.Any2ResourcesFromCard, ResourceType.OneCardBottomMore, 
				ResourceType.Fields, ResourceType.UnspentFields, ResourceType.Instruments }},
		{ModelChangeType.ApplyingInstruments, new List<ResourceType>(){ResourceType.DicePoints}},
		{ModelChangeType.ResourcesMining, new List<ResourceType>(){ ResourceType.Food, ResourceType.Forest, ResourceType.Clay, ResourceType.Stone, ResourceType.Gold}},
		{ModelChangeType.ReceiveAny2ResFromCard, new List<ResourceType>(){ ResourceType.Forest, ResourceType.Clay, ResourceType.Stone, ResourceType.Gold, ResourceType.Food }},
		{ModelChangeType.BonusFromOwnCharity, new List<ResourceType>(){ResourceType.Forest, ResourceType.Clay, ResourceType.Stone, ResourceType.Gold, ResourceType.Instruments, ResourceType.Fields, ResourceType.UnspentFields }},
		{ModelChangeType.BonusFromOthersCharity, new List<ResourceType>(){ ResourceType.Forest, ResourceType.Clay, ResourceType.Stone, ResourceType.Gold, ResourceType.Instruments, ResourceType.Fields, ResourceType.UnspentFields }},
		{ModelChangeType.ApplyCardFromOtherCard, new List<ResourceType>(){ ResourceType.HumanMultiplier, ResourceType.InstrumentsMultiplier, ResourceType.HousesMultiplier, ResourceType.FieldsMultiplier, 
				ResourceType.SciencesIn1stLine, ResourceType.SciencesIn2ndLine }},
		{ModelChangeType.Feeding, new List<ResourceType>(){ ResourceType.Score}}
	};
	private static Dictionary<ModelChangeType, List<ResourceType>> AllowedDecs = new Dictionary<ModelChangeType, List<ResourceType>>() {
		{ModelChangeType.StartGame, new List<ResourceType>(){/*its ok*/ }},
		{ModelChangeType.StartRound, new List<ResourceType>(){/*its ok*/ }},
		{ModelChangeType.EndTurn, new List<ResourceType>(){ ResourceType.AvailableHumans, ResourceType.InstrumentsAvailable, ResourceType.UnspentFields }},
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
				ResourceType.SpentOnCard1, ResourceType.SpentOnCard2, ResourceType.SpentOnCard3, ResourceType.SpentOnCard4, ResourceType.InstrumentsAvailable }},
		{ModelChangeType.ApplyGoToMining, new List<ResourceType>(){ ResourceType.SpentOnFood, ResourceType.SpentOnForest, ResourceType.SpentOnClay, ResourceType.SpentOnStone, ResourceType.SpentOnGold }},
		{ModelChangeType.ApplyingInstruments, new List<ResourceType>(){ ResourceType.InstrumentsAvailable, ResourceType.InstrumentsOnce }},
		{ModelChangeType.ResourcesMining, new List<ResourceType>(){ ResourceType.DicePoints }},
		{ModelChangeType.ReceiveAny2ResFromCard, new List<ResourceType>(){ ResourceType.Any2ResourcesFromCard }},
		{ModelChangeType.BonusFromOwnCharity, new List<ResourceType>(){ ResourceType.Charity, ResourceType.InstrumentsAvailable }},
		{ModelChangeType.BonusFromOthersCharity, new List<ResourceType>(){ ResourceType.InstrumentsAvailable }},
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
		foreach (ResourceType res in Enum.GetValues(typeof(ResourceType))) {
			if (stateAfter.GetCount (res) < 0)
				Debug.Log ("WTF");
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
		if (i > 0)
			endPrevEvent = _events [i - 1].StateAfter;
		else
			endPrevEvent = new PlayerModelTrainingDump ();
		//Debug.LogFormat ("After {0}-th turn {1}", i - 1, endPrevEvent.ToString ());

		PlayerModelTrainingDump startCurrEvent = _events[i].StateBefore;
		//Debug.LogFormat ("Before {0}-th turn {1}", i, startCurrEvent.ToString());

		PlayerModelTrainingDump delta = ModelChangeEvent.GetDelta (endPrevEvent, startCurrEvent);
		if (!delta.IsEmpty)
			Debug.LogFormat ("NON-NULL DELTA = {0}", delta.ToString());

//		Debug.LogFormat ("event[{0}] type = {2} delta = {1}", i, _events[i].Delta.ToString(), _events[i].Type);

		List<ResourceType> addedRes = new List<ResourceType> ();
		List<ResourceType> removedRes = new List<ResourceType> ();
		foreach (ResourceType res in Enum.GetValues(typeof(ResourceType))) {
			if (_events [i].Delta.GetCount (res) > 0)
				addedRes.Add (res);
			if (_events [i].Delta.GetCount (res) < 0)
				removedRes.Add (res);
		}
		foreach (var item in addedRes) {
			if (!AllowedIncs [_events [i].Type].Contains (item))
				Debug.LogFormat ("Violates constraint, cant add {0}", item);
		}
		foreach (var item in removedRes) {
			if (!AllowedDecs [_events [i].Type].Contains (item))
				Debug.LogFormat ("Violates constraint, cant remove {0}", item);
		}
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
	private ResourceType GetSpendingOnWhereToGo(WhereToGo target) {
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
		return addedResource;
	}
	public void OnAfterWhereToGoSelected(TrainingDecisionModel trainingModel, WhereToGo target) {
		ResourceType addedResource = GetSpendingOnWhereToGo (target);
		OnAfterDecisionPrivate(trainingModel, ModelChangeType.WhereToGoSelected, new Dictionary<ResourceType, int>() { {addedResource, 1} });
		CheckLastEventsConsistency (1);
	}
	public void OnAfterHumansCountSelected(TrainingDecisionModel trainingModel, WhereToGo target) {
		ResourceType removedResource = GetSpendingOnWhereToGo (target);
		var resourceChange = new Dictionary<ResourceType, int> () { { removedResource, -1 } };
		if (target == WhereToGo.Food || target == WhereToGo.Forest || target == WhereToGo.Clay || target == WhereToGo.Stone || target == WhereToGo.Gold)
			OnAfterDecisionPrivate(trainingModel, ModelChangeType.SetSpentHumans, resourceChange);
		else
			OnAfterModelChangePrivate(ModelChangeType.SetSpentHumans, resourceChange);
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
		OnAfterModelChangePrivate(ModelChangeType.ApplyGoToMining, new Dictionary<ResourceType, int>() { 
			{ResourceType.DicePoints, dotsSum}
		});
		CheckLastEventsConsistency (1);
	}
	public void OnAfterInstrumentsApplied(TrainingDecisionModel trainingModel, int instrumentsAdded) {
		OnAfterDecisionPrivate(trainingModel, ModelChangeType.ApplyingInstruments, new Dictionary<ResourceType, int>() { {ResourceType.DicePoints, instrumentsAdded} });
		CheckLastEventsConsistency (1);
	}
	public void OnAfterResourcesMined() {
		PlayerModel player = GetPlayer (_game);
		OnAfterModelChangePrivate(ModelChangeType.ResourcesMining, new Dictionary<ResourceType, int>() { {ResourceType.DicePoints, -_beforeState.GetCount(ResourceType.DicePoints)} });
		CheckLastEventsConsistency (1);
	}
	public void OnHumanRemovedFromCard(int cardSlotInd) {
		OnHumanRemovedFromCardPrivate (cardSlotInd);
		CheckLastEventsConsistency (1);
	}
	public void OnHumanRemovedFromCardPrivate(int cardSlotInd) {
		OnAfterModelChangePrivate(ModelChangeType.ApplyGoToCard);
	}
	public void OnAfterCardBought(int cardSlotInd, CardToBuild card) {
		OnHumanRemovedFromCardPrivate (cardSlotInd);
		ModelChangeEvent cardBuyEvent = _events [_events.Count - 1];

		if (card.TopFeature == TopCardFeature.RandomForEveryone)
			cardBuyEvent.StateAfter.ChangeCount (ResourceType.Charity, 1);

		if (card.TopFeature == TopCardFeature.Score) {
			if (UsedScoreSources == ScoreSources.TopCardFeature || UsedScoreSources == ScoreSources.Any)
				cardBuyEvent.StateAfter.ChangeCount (ResourceType.Score, card.TopFeatureParam);
		}

		if (card.TopFeature == TopCardFeature.ResourceAny)
			cardBuyEvent.StateAfter.ChangeCount (ResourceType.Any2ResourcesFromCard, 1);

		if (card.TopFeature == TopCardFeature.OneCardMore)
			cardBuyEvent.StateAfter.ChangeCount (ResourceType.OneCardBottomMore, 1);

		cardBuyEvent.UpdateDelta ();
		CheckLastEventsConsistency (1);
	}
	public void OnAfterCharityReceived(TrainingDecisionModel trainingModel, bool own) {
		ModelChangeType type = own ? ModelChangeType.BonusFromOwnCharity : ModelChangeType.BonusFromOthersCharity;
		Dictionary<ResourceType, int> additionalRes = own ? new Dictionary<ResourceType, int> (){ { ResourceType.Charity, -1 } } : null;
		OnAfterDecisionPrivate (trainingModel, type, additionalRes);
		CheckLastEventsConsistency (1);
	}
	public void OnAfterCardFromCardApplied() {
		OnAfterModelChangePrivate (ModelChangeType.ApplyCardFromOtherCard, new Dictionary<ResourceType, int> (){ { ResourceType.OneCardBottomMore, -1 }});
		CheckLastEventsConsistency (1);
	}

	public void OnHumanRemovedFromHouse(int houseSlotInd) {
		OnAfterModelChangePrivate(ModelChangeType.ApplyGoToHouse);
		CheckLastEventsConsistency (1);
	}
	public void OnAfterHouseBought(int houseSlotInd, List<Resource> spentResources) {
		int scoreFromResources = 0;
		foreach (var res in spentResources)
			scoreFromResources += Game.GetResourceCost (res);
		//OnHumanRemovedFromHouse (houseSlotInd);
		OnAfterModelChangePrivate(ModelChangeType.ApplyGoToHouse);
		ModelChangeEvent houseBuyEvent = _events [_events.Count - 1];

		if (UsedScoreSources == ScoreSources.Houses || UsedScoreSources == ScoreSources.Any)
			houseBuyEvent.StateAfter.ChangeCount (ResourceType.Score, scoreFromResources);

		houseBuyEvent.UpdateDelta ();
		CheckLastEventsConsistency (1);
	}
	public void OnAfterFeeding(TrainingDecisionModel trainingModel, bool hungry) {
		Dictionary<ResourceType, int> additionalResources = null;
		if (!hungry) {
			if (UsedScoreSources == ScoreSources.Feeding || UsedScoreSources == ScoreSources.Any) {
				int scoreToAdd = 10;
				additionalResources = new Dictionary<ResourceType, int> (){ { ResourceType.Score, scoreToAdd } };
			}
		}
		OnAfterDecisionPrivate (trainingModel, ModelChangeType.Feeding, additionalResources);
		CheckLastEventsConsistency (1);
	}
	private PlayerModelTrainingDump GetDump(PlayerModel player) {
		PlayerModelTrainingDump res = new PlayerModelTrainingDump ();
		res.Add (ResourceType.HumansCount, player.HumansCount);
		res.Add (ResourceType.AvailableHumans, player.AvailableHumans);
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
	public enum ScoreSources { 
		Any,
		Feeding,
		Houses,
		TopCardFeature,
		Science,
		FieldMultiplier,
		HumanMultiplier,
		HouseMultiplier,
		InstrumentMultiplier,
		Resources
	}
	public const ScoreSources UsedScoreSources = ScoreSources.Houses;
	/*public void OnEndGame(bool winner) {
		foreach (TrainingDecisionModel training in _trainingModels) {
			training.RewardPercent = winner ? 1 : 0;
		}
	}*/
	public void OnEndGame(bool winner) {

		/*StringBuilder sb = new StringBuilder ("Training controller log start\n");
		sb.AppendFormat("Player ind = {0}\n", PlayerInd);
		foreach (var gameEvent in _events)
			sb.Append (gameEvent.GetString (_events));
		sb.AppendLine ("Training controller log end");
		Debug.Log (sb.ToString());*/
		// Calc game events causes.
		for (int eventInd = 0; eventInd < _events.Count; eventInd++) {
			FindEventCauses (eventInd);
		}
		//LogEventsWithCauses("Causes calced");
		// Transitive closure for causality matrix.
		for (int eventInd = 0; eventInd < _events.Count; eventInd++) {
			ModelChangeEvent[] causesArray = new ModelChangeEvent[_events [eventInd].Causes.Keys.Count];
			_events [eventInd].Causes.Keys.CopyTo (causesArray, 0);
			for (int causeIndInCauses = 0; causeIndInCauses < causesArray.Length; causeIndInCauses++) {
				ModelChangeEvent currCause = causesArray [causeIndInCauses];
				int causeInd = _events.IndexOf (currCause);
				if (causeInd == eventInd)
					continue; // No transitiveness.
				float weightToTransit = _events[eventInd].Causes[currCause];
				_events[eventInd].Causes.Remove(currCause);
				foreach (var causeOfCauseItem in currCause.Causes) {
					float currWeight = weightToTransit;//causeOfCauseItem.Value * weightToTransit;
					if (_events [eventInd].Causes.ContainsKey (causeOfCauseItem.Key))
						_events [eventInd].Causes [causeOfCauseItem.Key] += currWeight;
					else
						_events [eventInd].Causes.Add(causeOfCauseItem.Key, currWeight);
				}
			}
		}
		//LogEventsWithCauses("Transitive causes calced");
		// Give score values to events.
		PlayerModel model = GetPlayer(_game);
		foreach (ModelChangeEvent currEvent in _events) {
			int score = currEvent.Delta.GetCount (ResourceType.Score);
			currEvent.ScoreValue = score; // Score from houses(resources), top card features, feeding.
		}
		Dictionary<ResourceType, float> resourceScoreValues = new Dictionary<ResourceType, float>();
		// Score from science line 1 and 2.
		if (UsedScoreSources == ScoreSources.Science || UsedScoreSources == ScoreSources.Any) {
			int scienceScoreRow1 = model.GetScienceScore (0);
			resourceScoreValues.Add (ResourceType.SciencesIn1stLine, scienceScoreRow1);
		}
		if (UsedScoreSources == ScoreSources.Science || UsedScoreSources == ScoreSources.Any) {
			int scienceScoreRow2 = model.GetScienceScore (1);
			resourceScoreValues.Add (ResourceType.SciencesIn2ndLine, scienceScoreRow2);
		}
		// Score from field multipliers.
		if (UsedScoreSources == ScoreSources.FieldMultiplier || UsedScoreSources == ScoreSources.Any) {
			float scoreFromFieldMultipliers = model.FieldsMultiplier * model.FieldsCount;
			resourceScoreValues.Add (ResourceType.Fields, scoreFromFieldMultipliers * 0.5f);
			resourceScoreValues.Add (ResourceType.FieldsMultiplier, scoreFromFieldMultipliers * 0.5f);
		}
		// Score from human multipliers.
		if (UsedScoreSources == ScoreSources.HumanMultiplier || UsedScoreSources == ScoreSources.Any) {
			float scoreFromHumanMultipliers = model.HumansMultiplier * model.HumansCount;
			resourceScoreValues.Add (ResourceType.HumansCount, scoreFromHumanMultipliers * 0.5f);
			resourceScoreValues.Add (ResourceType.HumanMultiplier, scoreFromHumanMultipliers * 0.5f);
		}
		// Score from house multipliers.
		if (UsedScoreSources == ScoreSources.HouseMultiplier || UsedScoreSources == ScoreSources.Any) {
			float scoreFromHouseMultipliers = model.HouseMultiplier * model.Houses.Count;
			resourceScoreValues.Add (ResourceType.HousesCount, scoreFromHouseMultipliers * 0.5f);
			resourceScoreValues.Add (ResourceType.HousesMultiplier, scoreFromHouseMultipliers * 0.5f);
		}
		// Score from instrument multipliers.
		if (UsedScoreSources == ScoreSources.InstrumentMultiplier || UsedScoreSources == ScoreSources.Any) {
			float scoreFromInstrumentsMultipliers = model.InstrumentsMultiplier * (model.InstrumentsCountSlot1 + model.InstrumentsCountSlot2 + model.InstrumentsCountSlot3);
			resourceScoreValues.Add (ResourceType.Instruments, scoreFromInstrumentsMultipliers * 0.5f);
			resourceScoreValues.Add (ResourceType.InstrumentsMultiplier, scoreFromInstrumentsMultipliers * 0.5f);
		}
		// Score from resources.
		if (UsedScoreSources == ScoreSources.Resources || UsedScoreSources == ScoreSources.Any) {
			resourceScoreValues.Add (ResourceType.Forest, model.Forest);
			resourceScoreValues.Add (ResourceType.Clay, model.Clay);
			resourceScoreValues.Add (ResourceType.Stone, model.Stone);
			resourceScoreValues.Add (ResourceType.Gold, model.Gold);
		}
		// Add all scores to corresponding turns.
		Dictionary<ResourceType, int> resourceCounts = new Dictionary<ResourceType, int>();
		foreach (var item in resourceScoreValues) {
			int count = 0;
			foreach (ModelChangeEvent currEvent in _events) {
				count += currEvent.Delta.GetCount (item.Key);
			}
			resourceCounts.Add (item.Key, count);
		}
		foreach (ModelChangeEvent currEvent in _events) {
			if (float.IsNaN (currEvent.ScoreValue))
				Debug.Log ("WTF");
			foreach (var item in resourceScoreValues) {
				int resInc = currEvent.Delta.GetCount (item.Key);
				if (resInc>0)
					currEvent.ScoreValue += resInc / (float)resourceCounts[item.Key] * item.Value;
				if (float.IsNaN (currEvent.ScoreValue))
					Debug.Log ("WTF");
			}
		}
		//LogEventsWithCauses("Score values given");
		// Distribute score values for each turn considering causation.
		foreach (ModelChangeEvent currEvent in _events) {
			float score = currEvent.ScoreValue;
			currEvent.ScoreValue = 0;
			foreach (var causeItem in currEvent.Causes) {
				causeItem.Key.ScoreValue += causeItem.Value * score;
				if (float.IsNaN (score) || float.IsNaN (causeItem.Value))
					Debug.Log ("WTF");
			}
		}
		//LogEventsWithCauses("Score values distributed");
		// Fill training model rewards.

		foreach (ModelChangeEvent currEvent in _events) {
			GameDecizionEvent decision = currEvent as GameDecizionEvent;
			if (decision == null)
				continue;
			decision.DecisionTraining.RewardPercent = decision.ScoreValue<float.Epsilon?0:(winner?1:0.5f);
			if (decision.DecisionTraining.RewardPercent < 0)
				Debug.Log ("WTF");
		}

		/*// Hack
		foreach (TrainingDecisionModel training in _trainingModels) {
			if (training.Type != DecisionType.SelectWhereToGo)
				continue;
			WhereToGo output = (WhereToGo)(training.Output+1);
			if (output == WhereToGo.Food || output == WhereToGo.Forest || output == WhereToGo.Clay || output == WhereToGo.Stone || output == WhereToGo.Gold)
				training.RewardPercent *= 2;
		}*/

		/*StringBuilder sb = new StringBuilder ("All training outputs = \n");
		foreach (TrainingDecisionModel training in _trainingModels) {
			switch (training.Type) {
			case DecisionType.SelectCharity: sb.AppendFormat ("{0:0.00}, charity selected {1}\n", training.RewardPercent, training.Output); break;
			case DecisionType.SelectInstruments: sb.AppendFormat ("{0:0.00}, instruments selected {1}\n", training.RewardPercent, training.Output); break;
			case DecisionType.SelectLeaveHungry: sb.AppendFormat ("{0:0.00}, leave hungry selected {1}\n", training.RewardPercent, training.Output); break;
			case DecisionType.SelectUsedHumans: sb.AppendFormat ("{0:0.00}, used humans selected {1}\n", training.RewardPercent, training.Output+1); break;
			case DecisionType.SelectWhereToGo: sb.AppendFormat ("{0:0.00}, where to go selected {1}\n", training.RewardPercent, (WhereToGo)(training.Output+1)); break;
			}

		}
		Debug.Log (sb.ToString());*/
	}
	private void LogEventsWithCauses(string title) {
		StringBuilder sb = new StringBuilder(title + ":\n");
		for (int eventInd = 0; eventInd < _events.Count; eventInd++) {
			string typeString = _events [eventInd].Type.ToString();
			GameDecizionEvent decision = _events [eventInd] as GameDecizionEvent;
			if (decision != null && decision.Type == ModelChangeType.WhereToGoSelected)
				typeString += ", " + ((WhereToGo)(decision.DecisionTraining.Output+1)).ToString ();
			sb.AppendFormat ("{0}-th event, type={1}, Score={2:0.00}", eventInd, typeString, _events [eventInd].ScoreValue);
			if (_events [eventInd].Causes.Count == 0)
				sb.AppendLine (" has no causes");
			else {
				sb.Append (", causes = (");
				foreach (var item in _events [eventInd].Causes)
					sb.AppendFormat ("{0}, {1}-th event, mass={2:0.00}, ", item.Key.Type, _events.IndexOf(item.Key), item.Value);
				sb.Append (")");
			}
			sb.AppendLine ();
		}
		Debug.Log (sb.ToString());
	}
	void FindEventCauses(int eventInd) {
		ModelChangeEvent currEvent = _events [eventInd];
		currEvent.Causes = new Dictionary<ModelChangeEvent, float>();
		if (currEvent is GameProxyEvent) {
			GameProxyEvent proxyEvent = currEvent as GameProxyEvent;
			currEvent.Causes.Add (proxyEvent.Cause, 1);
			return;
		}

		List<int> causeInds = new List<int> ();
		List<ResourceType> requiredResources = new List<ResourceType> ();
		foreach (ResourceType res in Enum.GetValues(typeof(ResourceType))) {
			int requiredCount = -currEvent.Delta.GetCount (res);
			for (int i=0; i<requiredCount;i++)
				requiredResources.Add (res);
		}
		foreach (ResourceType requiredRes in requiredResources) {
			bool causeFound = false;
			for (int i = 0; i < eventInd; i++) {
				if (_events [i].Delta.GetCount (requiredRes) <= 0)
					continue;
				// i-th event gives requiredRes to current.
				currEvent.Delta.Add (requiredRes, 1);
				_events [i].Delta.Add (requiredRes, -1);
				//if (i!=0 && _events [i].Type != ModelChangeType.StartRound) // Start game and round is not a cause.
				causeInds.Add (i);
				causeFound = true;
				break;
			}
			if (!causeFound)
				Debug.Log ("WTF");
		}
		if (currEvent is GameDecizionEvent)
			causeInds.Add (eventInd); // Decisions are not part of previous actions, they are causality sources.
		if (causeInds.Count == 0)
			causeInds.Add (eventInd); // Actions without any other cause considered self-caused.
		
		// Find cause weights.
		for (int i = 0; i < causeInds.Count; i++) {
			int duplicates = 1;
			for (int j = causeInds.Count-1; j > i; j--) {
				if (causeInds [i] == causeInds [j]) {
					duplicates++;
					causeInds.RemoveAt (j);
				}
			}
			currEvent.Causes.Add (_events [causeInds[i]], duplicates);
		}
		float sum = 0;
		foreach (float weight in currEvent.Causes.Values)
			sum += weight;
		for (int causeInd = 0;causeInd<causeInds.Count;causeInd++)
			currEvent.Causes [_events[causeInds[causeInd]]] /= sum;
	}
}
