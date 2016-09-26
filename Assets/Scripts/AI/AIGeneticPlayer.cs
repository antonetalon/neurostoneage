using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;


public class AIGeneticPlayer:Player {
	const int AdditionalNeuronsCount = 5;
	private static int Indicator(bool condition) {
		return condition ? 1 : 0;
	}

	public AIGeneticPlayer() {
		_whereToGoDecider = new Decider (WhereToGoInputsCount, AdditionalNeuronsCount, 16);_whereToGoDecider.SetRandomValues ();
		_humansCountDecider = new Decider (GetUsedHumansInputsCount, AdditionalNeuronsCount, 9);_humansCountDecider.SetRandomValues ();
		_useAnyResourceFromTopCardDecider = new Decider (GetAnyResourceFromTopCardInputsCount, AdditionalNeuronsCount, 2);_useAnyResourceFromTopCardDecider.SetRandomValues ();
		_resourceFromTopCardDecider = new Decider (GetResourceFromTopCardInputsCount, AdditionalNeuronsCount, 4);_resourceFromTopCardDecider.SetRandomValues ();
		_resourceFromCharityDecider = new Decider (GetResourceFromCharityInputsCount, AdditionalNeuronsCount, 6);_resourceFromCharityDecider.SetRandomValues ();
		_resourceFromInstrumentsDecider = new Decider (GetResourceFromInstrumentsInputsCount, AdditionalNeuronsCount, 4);_resourceFromInstrumentsDecider.SetRandomValues ();
		_leaveHungryDecider = new Decider (GetLeaveHungryInputsCount, AdditionalNeuronsCount, 2);_leaveHungryDecider.SetRandomValues ();
	}
	const int WhereToGoInputsCount = 13;
	private Decider _whereToGoDecider;
	public override void SelectWhereToGo (Game game, Action<WhereToGo> onComplete) {
		List<WhereToGo> options = game.GetAvailableTargets (_model);
		int[] inputs = new int[WhereToGoInputsCount];
		int i = 0;
		inputs [i] = game.TurnInd; i++;
		inputs [i] = _model.Food; i++;
		inputs [i] = _model.Forest; i++;
		inputs [i] = _model.Stone; i++;
		inputs [i] = _model.Gold; i++;
		inputs [i] = _model.HumansCount; i++;
		inputs [i] = _model.FieldsCount; i++;
		inputs [i] = _model.InstrumentsCountSlot1+_model.InstrumentsCountSlot2+_model.InstrumentsCountSlot3; i++;
		inputs [i] = _model.HouseMultiplier; i++;
		inputs [i] = _model.FieldsMultiplier; i++;
		inputs [i] = _model.InstrumentsMultiplier; i++;
		inputs [i] = _model.GetScienceScore(0); i++;
		inputs [i] = _model.Score; i++;
		List<int> optionInds = new List<int> ();
		for (int j = 0; j < options.Count; j++)
			optionInds.Add ((int)options[j]-1);
		int decision = _whereToGoDecider.GetDecision (inputs, optionInds);
		WhereToGo res = (WhereToGo)(decision + 1);
		onComplete (res);
	}

	const int GetUsedHumansInputsCount = 9;
	private Decider _humansCountDecider;
	public override void SelectUsedHumans (Game game, WhereToGo whereToGo, Action<int> onComplete) {
		int max = game.GetMaxHumansCountFor (whereToGo);
		int min = game.GetMinHumansCountFor (whereToGo);
		int freePlacesCount = game.GetAvailableHumansCountFor (whereToGo);
		max = Mathf.Min (max, freePlacesCount);
		max = Mathf.Min (max, _model.UnspentHumanCount);
		if (max <= min) {
			onComplete (min);
			return;
		}

		// Can only be resource mining turn.
		Resource receivedResource = Game.GetResourceFromTarget (whereToGo);
		int resourceCost = Game.GetResourceCost (receivedResource);

	
		List<int> optionInds = new List<int> ();
		for (int j = min; j <= max; j++)
			optionInds.Add (j - 1);
		int[] inputs = new int[GetUsedHumansInputsCount];
		int i = 0;
		inputs [i] = game.TurnInd; i++;
		inputs [i] = resourceCost; i++;
		inputs [i] = Indicator(receivedResource == Resource.Food); i++;
		inputs [i] = Indicator(receivedResource == Resource.Forest); i++;
		inputs [i] = Indicator(receivedResource == Resource.Clay); i++;
		inputs [i] = Indicator(receivedResource == Resource.Stone); i++;
		inputs [i] = Indicator(receivedResource == Resource.Gold); i++;
		inputs [i] = _model.InstrumentsCountSlot1+_model.InstrumentsCountSlot2+_model.InstrumentsCountSlot3; i++;
		inputs [i] = _model.HumansCount-_model.FieldsCount-_model.Food; i++; // Needed food.

		int count = _humansCountDecider.GetDecision (inputs, optionInds)+1;
		onComplete (count);
	}

	const int GetAnyResourceFromTopCardInputsCount = 6;
	private Decider _useAnyResourceFromTopCardDecider;
	public override void UseGetAnyResourceFromTopCard (Game game, Action<bool> onComplete) {
		List<int> optionInds = new List<int> ();
		optionInds.Add (0);
		optionInds.Add (1);

		int[] inputs = new int[GetAnyResourceFromTopCardInputsCount];
		int i = 0;
		inputs [i] = game.TurnInd; i++;
		inputs [i] = _model.Food; i++;
		inputs [i] = _model.Forest; i++;
		inputs [i] = _model.Clay; i++;
		inputs [i] = _model.Stone; i++;
		inputs [i] = _model.Gold; i++;

		bool selectedTrue = _useAnyResourceFromTopCardDecider.GetDecision (inputs, optionInds)==1;
		onComplete (selectedTrue);
	}

	const int GetResourceFromTopCardInputsCount = 6;
	private Decider _resourceFromTopCardDecider;
	public override void ChooseResourceToReceiveFromTopCard (Game game, Action<Resource> onComplete) {
		List<int> optionInds = new List<int> ();
		optionInds.Add (0);
		optionInds.Add (1);
		optionInds.Add (2);
		optionInds.Add (3);

		int[] inputs = new int[GetResourceFromTopCardInputsCount];
		int i = 0;
		inputs [i] = game.TurnInd; i++;
		inputs [i] = _model.Food; i++;
		inputs [i] = _model.Forest; i++;
		inputs [i] = _model.Clay; i++;
		inputs [i] = _model.Stone; i++;
		inputs [i] = _model.Gold; i++;

		int ind = _resourceFromTopCardDecider.GetDecision (inputs, optionInds);
		switch (ind) {
		default:
		case 0: onComplete (Resource.Forest); break;
		case 1: onComplete (Resource.Clay); break;
		case 2: onComplete (Resource.Stone); break;
		case 3: onComplete (Resource.Gold); break;
		}
	}

	const int GetResourceFromCharityInputsCount = 7;
	private Decider _resourceFromCharityDecider;
	public override void ChooseItemToReceiveFromCharityCard (Game game, List<int> randoms, Action<int> onComplete) {
		List<int> optionInds = new List<int> ();
		for (int j = 0; j < 6; j++) {
			if (randoms.Contains (j + 1))
				optionInds.Add (j);
		}

		int[] inputs = new int[GetResourceFromCharityInputsCount];
		int i = 0;
		inputs [i] = game.TurnInd; i++;
		inputs [i] = _model.Forest; i++;
		inputs [i] = _model.Clay; i++;
		inputs [i] = _model.Stone; i++;
		inputs [i] = _model.Gold; i++;
		inputs [i] = _model.InstrumentsCountSlot1+_model.InstrumentsCountSlot2+_model.InstrumentsCountSlot3; i++;
		inputs [i] = _model.FieldsCount; i++;

		int ind = _resourceFromCharityDecider.GetDecision (inputs, optionInds)+1;
		onComplete (ind);
	}

	const int GetResourceFromInstrumentsInputsCount = 10;
	private Decider _resourceFromInstrumentsDecider;
	public override void GetUsedInstrumentSlotInd (Game game, Resource receivedRecource, int points, OnInstrumentsToUseSelected onComplete) {
		int availableSlot1Instruments = _model.InstrumentsSlot1Used ? 0 : _model.InstrumentsCountSlot1;
		int availableSlot2Instruments = _model.InstrumentsSlot2Used ? 0 : _model.InstrumentsCountSlot2;
		int availableSlot3Instruments = _model.InstrumentsSlot3Used ? 0 : _model.InstrumentsCountSlot3;
		int availableTop4Instruments =  (_model.Top4Instruments != null && !_model.Top4Instruments.Card.TopUsed) ? 4 : 0;
		int availableTop3Instruments =  (_model.Top3Instruments != null && !_model.Top3Instruments.Card.TopUsed) ? 3 : 0;
		int availableTop2Instruments =  (_model.Top2Instruments != null && !_model.Top2Instruments.Card.TopUsed) ? 2 : 0;

		bool useSlot1 = availableSlot1Instruments > 0;
		bool useSlot2 = availableSlot2Instruments > 0;
		bool useSlot3 = availableSlot3Instruments > 0;
		bool useOnceSlot4 = availableTop4Instruments > 0;
		bool useOnceSlot3 = availableTop3Instruments > 0;
		bool useOnceSlot2 = availableTop2Instruments > 0;

		int cost = Game.GetResourceCost (receivedRecource);
		int receivedFromDices = points / cost;
		List<int> optionInds = new List<int> ();

		int notUsedPoints = points - receivedFromDices * cost;
		int maxInstrumentsPoints = availableSlot1Instruments + availableSlot2Instruments + availableSlot3Instruments
			+ availableTop4Instruments + availableTop3Instruments + availableTop2Instruments;

		optionInds.Add (0);  // Option to add 0 resource.
		if (maxInstrumentsPoints >= cost - notUsedPoints)
			optionInds.Add (1); // Option to add 1 resource.
		if (maxInstrumentsPoints >= 2*cost - notUsedPoints)
			optionInds.Add (2); // Option to add 2 resources.
		if (maxInstrumentsPoints >= 3*cost - notUsedPoints)
			optionInds.Add (3); // Option to add max and not less than 3 resources.

		int[] inputs = new int[GetResourceFromInstrumentsInputsCount];
		int i = 0;
		inputs [i] = game.TurnInd; i++;
		inputs [i] = cost; i++;
		inputs [i] = Indicator(receivedRecource == Resource.Food); i++;
		inputs [i] = Indicator(receivedRecource == Resource.Forest); i++;
		inputs [i] = Indicator(receivedRecource == Resource.Clay); i++;
		inputs [i] = Indicator(receivedRecource == Resource.Stone); i++;
		inputs [i] = Indicator(receivedRecource == Resource.Gold); i++;
		inputs [i] = availableSlot1Instruments + availableSlot2Instruments + availableSlot3Instruments; i++;
		inputs [i] = availableTop4Instruments + availableTop3Instruments + availableTop2Instruments; i++;
		inputs [i] = _model.GetResourceCount(receivedRecource); i++;

		int decision = _resourceFromInstrumentsDecider.GetDecision (inputs, optionInds);
		int pointsForDecision;
		switch (decision) {
			default:
			case 0: pointsForDecision = 0; break;
			case 1: pointsForDecision = cost - notUsedPoints; break;
			case 2: pointsForDecision = 2*cost - notUsedPoints; break;
			case 3: pointsForDecision = 3*cost - notUsedPoints; break;
		}

		int currPoints = maxInstrumentsPoints;
		if (availableTop4Instruments > 0 && pointsForDecision <= currPoints - availableTop4Instruments) {
			currPoints -= availableTop4Instruments;
			useOnceSlot4 = false;
		}
		if (availableTop3Instruments > 0 && pointsForDecision <= currPoints - availableTop3Instruments) {
			currPoints -= availableTop3Instruments;
			useOnceSlot3 = false;
		}
		if (availableTop2Instruments > 0 && pointsForDecision <= currPoints - availableTop2Instruments) {
			currPoints -= availableTop2Instruments;
			useOnceSlot2 = false;
		}
		if (availableSlot1Instruments > 0 && pointsForDecision <= currPoints - availableSlot1Instruments) {
			currPoints -= availableSlot1Instruments;
			useSlot1 = false;
		}
		if (availableSlot2Instruments > 0 && pointsForDecision <= currPoints - availableSlot2Instruments) {
			currPoints -= availableSlot2Instruments;
			useSlot2 = false;
		}
		if (availableSlot3Instruments > 0 && pointsForDecision <= currPoints - availableSlot3Instruments) {
			currPoints -= availableSlot3Instruments;
			useSlot3 = false;
		}

		onComplete (useSlot1, useSlot2, useSlot3, useOnceSlot4, useOnceSlot3, useOnceSlot2);
	}

	public override void BuildCard (Game game, int cardInd, Action<bool> onComplete) {
		// Always build card if possible.
		onComplete (true);
	}
	public override void GetUsedResourceForCardBuilding (Game game, CardToBuild card, List<Resource> alreadySelectedResources, Action<Resource> onComplete) {
		List<Resource> options = new List<Resource> ();
		for (int i = 0; i < _model.Forest; i++)
			options.Add (Resource.Forest);
		for (int i = 0; i < _model.Clay; i++)
			options.Add (Resource.Clay);
		for (int i = 0; i < _model.Stone; i++)
			options.Add (Resource.Stone);
		for (int i = 0; i < _model.Gold; i++)
			options.Add (Resource.Gold);
		foreach (Resource currRes in alreadySelectedResources) {
			int ind = options.IndexOf (currRes);
			options.RemoveAt (ind);
		}
		Resource res = Resource.None;
		if (options.Contains (Resource.Forest))
			res = Resource.Forest;
		else if (options.Contains (Resource.Clay))
			res = Resource.Clay;
		else if (options.Contains (Resource.Stone))
			res = Resource.Stone;
		else if (options.Contains (Resource.Gold))
			res = Resource.Gold;
		
		onComplete (res);
	}
	public override void BuildHouse (Game game, int houseInd, Action<bool> onComplete) {
		onComplete (true); // Always build house if possible.
	}
	public override void GetUsedResourceForHouseBuilding (Game game, HouseToBuild house, List<Resource> options, List<Resource> spendResources, Action<Resource> onComplete) {
		Resource res = Resource.None;
		if (options.Contains (Resource.Gold))
			res = Resource.Gold;
		else if (options.Contains (Resource.Stone))
			res = Resource.Stone;
		else if (options.Contains (Resource.Clay))
			res = Resource.Clay;
		else if (options.Contains (Resource.Forest))
			res = Resource.Forest;
		
		onComplete (res);
	}
	const int GetLeaveHungryInputsCount = 5;
	private Decider _leaveHungryDecider;
	public override void LeaveHungry (Game game, int eatenResources, Action<bool> onComplete) {
		List<int> optionInds = new List<int> ();
		optionInds.Add (0);
		optionInds.Add (1);

		int resourcesToEat = _model.HumansCount - _model.FieldsCount - _model.Food;
		int eatenForest = Mathf.Min (resourcesToEat, _model.Forest);
		resourcesToEat -= eatenForest;
		int eatenClay = Mathf.Min (resourcesToEat, _model.Clay);
		resourcesToEat -= eatenClay;
		int eatenStone = Mathf.Min (resourcesToEat, _model.Stone);
		resourcesToEat -= eatenStone;
		int eatenGold = Mathf.Min (resourcesToEat, _model.Gold);
		resourcesToEat -= eatenGold;

		int[] inputs = new int[GetLeaveHungryInputsCount];
		int i = 0;
		inputs [i] = game.TurnInd; i++;
		inputs [i] = eatenForest; i++;
		inputs [i] = eatenClay; i++;
		inputs [i] = eatenStone; i++;
		inputs [i] = eatenGold; i++;

		bool selectTrue = _leaveHungryDecider.GetDecision (inputs, optionInds)== 1;
		onComplete (selectTrue);
	}
}