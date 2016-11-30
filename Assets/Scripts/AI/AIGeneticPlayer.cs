using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class AIGeneticPlayer:Player {
	const int AdditionalNeuronsCount = 5;
	private static int Indicator(bool condition) {
		return condition ? 1 : 0;
	}

	private AIGeneticPlayer() {
	}

	public static AIGeneticPlayer CreateRandom() {
		AIGeneticPlayer player = new AIGeneticPlayer ();
		player._whereToGoDecider = GetRandomDecider(WhereToGoInputsCount, AdditionalNeuronsCount, 16);
		player._humansCountDecider = GetRandomDecider(GetUsedHumansInputsCount, AdditionalNeuronsCount, 9);
		player._useAnyResourceFromTopCardDecider = GetRandomDecider(GetAnyResourceFromTopCardInputsCount, AdditionalNeuronsCount, 2);
		player._resourceFromTopCardDecider = GetRandomDecider(GetResourceFromTopCardInputsCount, AdditionalNeuronsCount, 4);
		player._resourceFromCharityDecider = GetRandomDecider(GetResourceFromCharityInputsCount, AdditionalNeuronsCount, 6);
		player._resourceFromInstrumentsDecider = GetRandomDecider(GetResourceFromInstrumentsInputsCount, AdditionalNeuronsCount, 4);
		player._leaveHungryDecider = GetRandomDecider(GetLeaveHungryInputsCount, AdditionalNeuronsCount, 2);
		return player;
	}
	public static AIGeneticPlayer CreateFromCrossover(AIGeneticPlayer parent1, AIGeneticPlayer parent2) {
		const float MutationRate = 0.005f;
		AIGeneticPlayer player = new AIGeneticPlayer ();
		player._whereToGoDecider = GetDeciderFromCrossover(parent1._whereToGoDecider, parent2._whereToGoDecider, MutationRate, WhereToGoInputsCount, AdditionalNeuronsCount, 16);
		player._humansCountDecider = GetDeciderFromCrossover(parent1._humansCountDecider, parent2._humansCountDecider, MutationRate, GetUsedHumansInputsCount, AdditionalNeuronsCount, 9);
		player._useAnyResourceFromTopCardDecider = GetDeciderFromCrossover(parent1._useAnyResourceFromTopCardDecider, parent2._useAnyResourceFromTopCardDecider, MutationRate, GetAnyResourceFromTopCardInputsCount, AdditionalNeuronsCount, 2);
		player._resourceFromTopCardDecider = GetDeciderFromCrossover(parent1._resourceFromTopCardDecider, parent2._resourceFromTopCardDecider, MutationRate, GetResourceFromTopCardInputsCount, AdditionalNeuronsCount, 4);
		player._resourceFromCharityDecider = GetDeciderFromCrossover(parent1._resourceFromCharityDecider, parent2._resourceFromCharityDecider, MutationRate, GetResourceFromCharityInputsCount, AdditionalNeuronsCount, 6);
		player._resourceFromInstrumentsDecider = GetDeciderFromCrossover(parent1._resourceFromInstrumentsDecider, parent2._resourceFromInstrumentsDecider, MutationRate, GetResourceFromInstrumentsInputsCount, AdditionalNeuronsCount, 4);
		player._leaveHungryDecider = GetDeciderFromCrossover(parent1._leaveHungryDecider, parent2._leaveHungryDecider, MutationRate, GetLeaveHungryInputsCount, AdditionalNeuronsCount, 2);
		return player;
	}
	private static Decider GetRandomDecider(int inputsCount, int neuronsCount, int outputsCount) {
		var decider = new Decider (inputsCount, neuronsCount, outputsCount);
		decider.SetRandomValues ();
		return decider;
	}
	private static Decider GetDeciderFromCrossover(Decider parent1, Decider parent2, float MutationRate, int inputsCount, int neuronsCount, int outputsCount) {
		if (UnityEngine.Random.value < MutationRate)
			return GetRandomDecider (inputsCount, neuronsCount, outputsCount);
		if (UnityEngine.Random.value < 0.5f)
			return parent1;
		else
			return parent2;
	}

	public override Player Clone ()
	{
		AIGeneticPlayer clone = new AIGeneticPlayer ();
		clone._whereToGoDecider = _whereToGoDecider.Clone ();
		clone._humansCountDecider = _humansCountDecider.Clone ();
		clone._useAnyResourceFromTopCardDecider = _useAnyResourceFromTopCardDecider.Clone ();
		clone._resourceFromTopCardDecider = _resourceFromTopCardDecider.Clone ();
		clone._resourceFromCharityDecider = _resourceFromCharityDecider.Clone ();
		clone._resourceFromInstrumentsDecider = _resourceFromInstrumentsDecider.Clone ();
		clone._leaveHungryDecider = _leaveHungryDecider.Clone ();
		return clone;
	}

	const int WhereToGoInputsCount = 17;
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
		inputs [i] = Indicator(Game.EnoughResourcesForBuilding (game, _model, 0)); i++;
		inputs [i] = Indicator(Game.EnoughResourcesForBuilding (game, _model, 1)); i++;
		inputs [i] = Indicator(Game.EnoughResourcesForBuilding (game, _model, 2)); i++;
		inputs [i] = Indicator(Game.EnoughResourcesForBuilding (game, _model, 3)); i++;
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
		max = Mathf.Min (max, _model.AvailableHumans);
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
		inputs [i] = Mathf.Max(0, _model.HumansCount-_model.FieldsCount-_model.Food); i++; // Needed food.

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
		int availableSlot1Instruments = _model.GetAvailableInstruments (0);
		int availableSlot2Instruments = _model.GetAvailableInstruments (1);
		int availableSlot3Instruments = _model.GetAvailableInstruments (2);
		int availableTop4Instruments =  _model.GetAvailableOnceInstruments (0);
		int availableTop3Instruments =  _model.GetAvailableOnceInstruments (1);
		int availableTop2Instruments =  _model.GetAvailableOnceInstruments (2);

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