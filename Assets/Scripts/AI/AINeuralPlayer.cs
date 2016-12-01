using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class AINeuralPlayer:Player {
	private static int Indicator(bool condition) {
		return condition ? 1 : 0;
	}

	public AINeuralPlayer() {
		InitWhereToGo ();
		InitGetUsedHumans ();
		InitCharity ();
		InitInstruments ();
		InitHungry ();
	}
	public override Player Clone() {
		AINeuralPlayer clone = new AINeuralPlayer ();
		clone._whereToGoDecider = _whereToGoDecider.Clone();
		clone._getUsedHumansDecider = _getUsedHumansDecider.Clone();
		clone._charityDecider = _charityDecider.Clone();
		clone._instrumentsDecider = _instrumentsDecider.Clone();
		clone._hungryDecider = _hungryDecider.Clone();
		return clone;
	}

	public static int[] GetInputs(DecisionType type, Game game, PlayerModel player, Resource receivedRecource, WhereToGo whereToGo) {
		switch (type) {
		default:
		case DecisionType.SelectWhereToGo: return GetWhereToGoInputs (game, player);
		case DecisionType.SelectUsedHumans: return GetUsedHumansInputs (game, player, whereToGo);
		case DecisionType.SelectCharity: return GetCharityInputs (game, player);
		case DecisionType.SelectInstruments: return GetInstrumentsInputs (game, player, receivedRecource);
		case DecisionType.SelectLeaveHungry: return GetHungryInputs (game, player);
		}
	}
	public static List<int> GetOptionInds(DecisionType type, Game game, PlayerModel player, List<int> randoms, int points, Resource receivedRecource, WhereToGo whereToGo) {
		switch (type) {
		default:
		case DecisionType.SelectWhereToGo: return GetWhereToGoOptionInds (game, player);
		case DecisionType.SelectUsedHumans: return GetUsedHumansOptionInds (game, player, whereToGo);
		case DecisionType.SelectCharity: return GetCharityOptionInds (game, player, randoms);
		case DecisionType.SelectInstruments: return GetInstrumentsOptionInds (game, player, randoms, points, receivedRecource);
		case DecisionType.SelectLeaveHungry: return GetHungryOptionInds (game, player);
		}
	}
	public static int GetDecisionFromOutputs(double[] outputs, List<int> optionInds) {
		double maxAllowedOutput = double.MinValue;
		int maxAllowedOutputInd = -1;
		for (int i = 0; i < outputs.Length; i++) {
			if (optionInds.Contains(i) && maxAllowedOutput<outputs[i]) {
				maxAllowedOutput = outputs [i];
				maxAllowedOutputInd = i;
			}
		}
		if (maxAllowedOutputInd == -1)
			Debug.Log ("hi");
		return maxAllowedOutputInd;
	}
	private int GetDecisionInd(DecisionType type, Game game, PlayerModel player, NeuralNetwork decider, List<int> randoms, int points, Resource receivedRecource, WhereToGo whereToGo) {
		List<int> optionInds = GetOptionInds (type, game, _model, randoms, points, receivedRecource, whereToGo);
		int[] inputs = GetInputs (type, game, _model, receivedRecource, whereToGo);
		double[] inputsDouble = new double[inputs.Length];
		for (int i = 0; i < inputs.Length; i++)
			inputsDouble [i] = inputs [i];
		double[] outputs = decider.Think (inputsDouble);
		int decisionInd = GetDecisionFromOutputs (outputs, optionInds);
		return decisionInd;
	}
	public NeuralNetwork GetDecider(DecisionType type) {
		switch (type) {
			case DecisionType.SelectWhereToGo: return _whereToGoDecider;
			case DecisionType.SelectUsedHumans: return _getUsedHumansDecider;
			case DecisionType.SelectInstruments: return _instrumentsDecider;
			case DecisionType.SelectCharity: return _charityDecider;
			case DecisionType.SelectLeaveHungry: return _hungryDecider;
			default: return null;
		}
	}

	#region Where to go
	private void InitWhereToGo() {
		_whereToGoDecider = new NeuralNetwork (new int[4]{ 81, 50, 30, 16 });
	}
	private static int[] GetWhereToGoInputs(Game game, PlayerModel player) {
		int[] inputs = new int[81];
		int i = 0;
		inputs [i] = game.TurnInd; i++;
		inputs [i] = player.Food; i++;
		inputs [i] = player.Forest; i++;
		inputs [i] = player.Stone; i++;
		inputs [i] = player.Gold; i++;
		inputs [i] = Indicator(Game.EnoughResourcesForBuilding (game, player, 0)); i++;
		inputs [i] = Indicator(Game.EnoughResourcesForBuilding (game, player, 1)); i++;
		inputs [i] = Indicator(Game.EnoughResourcesForBuilding (game, player, 2)); i++;
		inputs [i] = Indicator(Game.EnoughResourcesForBuilding (game, player, 3)); i++;
		inputs [i] = player.HumansCount; i++;
		inputs [i] = player.FieldsCount; i++;
		inputs [i] = player.InstrumentsCountSlot1+player.InstrumentsCountSlot2+player.InstrumentsCountSlot3; i++;
		inputs [i] = player.HouseMultiplier; i++;
		inputs [i] = player.FieldsMultiplier; i++;
		inputs [i] = player.InstrumentsMultiplier; i++;
		inputs [i] = player.GetScienceScore(0); i++;
		inputs [i] = player.Score; i++;

		List<CardToBuild> cards = new List<CardToBuild> ();
		cards.Add (game.GetAvailableCard (0));
		cards.Add (game.GetAvailableCard (1));
		cards.Add (game.GetAvailableCard (2));
		cards.Add (game.GetAvailableCard (3));
		// Card i not owned science exists.
		inputs [i] = Indicator( cards [0].BottomFeature == BottomCardFeature.Science && player.ScienceExists ((Science)cards [0].BottomFeatureParam, 0) ); i++;
		inputs [i] = Indicator( cards [1].BottomFeature == BottomCardFeature.Science && player.ScienceExists ((Science)cards [1].BottomFeatureParam, 0) ); i++;
		inputs [i] = Indicator( cards [2].BottomFeature == BottomCardFeature.Science && player.ScienceExists ((Science)cards [2].BottomFeatureParam, 0) ); i++;
		inputs [i] = Indicator( cards [3].BottomFeature == BottomCardFeature.Science && player.ScienceExists ((Science)cards [3].BottomFeatureParam, 0) ); i++;
		// Card i science score addition for row 0.
		inputs [i] = inputs [i-4]>0.5f?player.GetSciencesCount(0)*player.GetSciencesCount(0):0; i++;
		inputs [i] = inputs [i-4]>0.5f?player.GetSciencesCount(0)*player.GetSciencesCount(0):0; i++;
		inputs [i] = inputs [i-4]>0.5f?player.GetSciencesCount(0)*player.GetSciencesCount(0):0; i++;
		inputs [i] = inputs [i-4]>0.5f?player.GetSciencesCount(0)*player.GetSciencesCount(0):0; i++;
		// Card i houses, fields, humans, instruments multipliers.
		inputs [i] = cards[0].BottomFeature == BottomCardFeature.FieldMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[0].BottomFeature == BottomCardFeature.HouseMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[0].BottomFeature == BottomCardFeature.HumanMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[0].BottomFeature == BottomCardFeature.InstrumentsMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[1].BottomFeature == BottomCardFeature.FieldMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[1].BottomFeature == BottomCardFeature.HouseMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[1].BottomFeature == BottomCardFeature.HumanMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[1].BottomFeature == BottomCardFeature.InstrumentsMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[2].BottomFeature == BottomCardFeature.FieldMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[2].BottomFeature == BottomCardFeature.HouseMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[2].BottomFeature == BottomCardFeature.HumanMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[2].BottomFeature == BottomCardFeature.InstrumentsMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[3].BottomFeature == BottomCardFeature.FieldMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[3].BottomFeature == BottomCardFeature.HouseMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[3].BottomFeature == BottomCardFeature.HumanMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = cards[3].BottomFeature == BottomCardFeature.InstrumentsMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		// Card i has charity.
		inputs [i] = Indicator( cards[0].TopFeature == TopCardFeature.RandomForEveryone ); i++;
		inputs [i] = Indicator( cards[1].TopFeature == TopCardFeature.RandomForEveryone ); i++;
		inputs [i] = Indicator( cards[2].TopFeature == TopCardFeature.RandomForEveryone ); i++;
		inputs [i] = Indicator( cards[3].TopFeature == TopCardFeature.RandomForEveryone ); i++;
		// Card i forest, clay, stone, gold amount - random and const aggregated.
		inputs [i] = GetResourceExpectedCount(cards[0],Resource.Food); i++;
		inputs [i] = GetResourceExpectedCount(cards[0],Resource.Forest); i++;
		inputs [i] = GetResourceExpectedCount(cards[0],Resource.Clay); i++;
		inputs [i] = GetResourceExpectedCount(cards[0],Resource.Stone); i++;
		inputs [i] = GetResourceExpectedCount(cards[0],Resource.Gold); i++;
		inputs [i] = GetResourceExpectedCount(cards[1],Resource.Food); i++;
		inputs [i] = GetResourceExpectedCount(cards[1],Resource.Forest); i++;
		inputs [i] = GetResourceExpectedCount(cards[1],Resource.Clay); i++;
		inputs [i] = GetResourceExpectedCount(cards[1],Resource.Stone); i++;
		inputs [i] = GetResourceExpectedCount(cards[1],Resource.Gold); i++;
		inputs [i] = GetResourceExpectedCount(cards[2],Resource.Food); i++;
		inputs [i] = GetResourceExpectedCount(cards[2],Resource.Forest); i++;
		inputs [i] = GetResourceExpectedCount(cards[2],Resource.Clay); i++;
		inputs [i] = GetResourceExpectedCount(cards[2],Resource.Stone); i++;
		inputs [i] = GetResourceExpectedCount(cards[2],Resource.Gold); i++;
		inputs [i] = GetResourceExpectedCount(cards[3],Resource.Food); i++;
		inputs [i] = GetResourceExpectedCount(cards[3],Resource.Forest); i++;
		inputs [i] = GetResourceExpectedCount(cards[3],Resource.Clay); i++;
		inputs [i] = GetResourceExpectedCount(cards[3],Resource.Stone); i++;
		inputs [i] = GetResourceExpectedCount(cards[3],Resource.Gold); i++;
		// Card i instruments and once instruments.
		inputs [i] = cards[0].TopFeature == TopCardFeature.InstrumentsForever? 1:0; i++;
		inputs [i] = cards[0].TopFeature == TopCardFeature.InstrumentsOnce? cards[0].TopFeatureParam:0; i++;
		inputs [i] = cards[1].TopFeature == TopCardFeature.InstrumentsForever? 1:0; i++;
		inputs [i] = cards[1].TopFeature == TopCardFeature.InstrumentsOnce? cards[1].TopFeatureParam:0; i++;
		inputs [i] = cards[2].TopFeature == TopCardFeature.InstrumentsForever? 1:0; i++;
		inputs [i] = cards[2].TopFeature == TopCardFeature.InstrumentsOnce? cards[2].TopFeatureParam:0; i++;
		inputs [i] = cards[3].TopFeature == TopCardFeature.InstrumentsForever? 1:0; i++;
		inputs [i] = cards[3].TopFeature == TopCardFeature.InstrumentsOnce? cards[3].TopFeatureParam:0; i++;
		// House i min, max score.
		inputs [i] = GetHouseMaxScore(game.GetHouse(0)); i++;
		inputs [i] = GetHouseMinScore(game.GetHouse(0)); i++;
		inputs [i] = GetHouseMaxScore(game.GetHouse(1)); i++;
		inputs [i] = GetHouseMinScore(game.GetHouse(1)); i++;
		inputs [i] = GetHouseMaxScore(game.GetHouse(2)); i++;
		inputs [i] = GetHouseMinScore(game.GetHouse(2)); i++;
		inputs [i] = GetHouseMaxScore(game.GetHouse(3)); i++;
		inputs [i] = GetHouseMinScore(game.GetHouse(3)); i++;
		return inputs;
	}
	private static int GetHouseMaxScore(HouseToBuild house) {
		if (house.StaticCost != null)
			return GetHouseStaticCost (house);
		else {
			int sum = 0;
			if (house.DifferentResourcesCount >= 1)
				sum += Game.GetResourceCost (Resource.Gold);
			if (house.DifferentResourcesCount >= 2)
				sum += Game.GetResourceCost (Resource.Stone);
			if (house.DifferentResourcesCount >= 3)
				sum += Game.GetResourceCost (Resource.Clay);
			if (house.DifferentResourcesCount >= 4)
				sum += Game.GetResourceCost (Resource.Forest);
			sum += Mathf.Max (0, house.MaxResourcesCount - house.DifferentResourcesCount)* Game.GetResourceCost (Resource.Gold);
			return sum;
		}
	}
	private static int GetHouseMinScore(HouseToBuild house) {
		if (house.StaticCost != null)
			return GetHouseStaticCost (house);
		else {
			int sum = 0;
			if (house.DifferentResourcesCount >= 1)
				sum += Game.GetResourceCost (Resource.Forest);
			if (house.DifferentResourcesCount >= 2)
				sum += Game.GetResourceCost (Resource.Clay);
			if (house.DifferentResourcesCount >= 3)
				sum += Game.GetResourceCost (Resource.Stone);
			if (house.DifferentResourcesCount >= 4)
				sum += Game.GetResourceCost (Resource.Gold);
			sum += Mathf.Max (0, house.MaxResourcesCount - house.DifferentResourcesCount)* Game.GetResourceCost (Resource.Forest);
			return sum;
		}
	}
	private static int GetHouseStaticCost(HouseToBuild house) {
		int sum = 0;
		foreach (var res in house.StaticCost)
			sum += Game.GetResourceCost (res);
		return sum;
	}
	private static int GetResourceExpectedCount(CardToBuild card, Resource res) {
		int count = 0;
		switch (card.TopFeature) {
		case TopCardFeature.ResourceAny:
			if (res == Resource.Gold)
				count += 2;
			break;
		case TopCardFeature.ResourceConstClay:
			if (res == Resource.Clay)
				count += card.TopFeatureParam;
			break;
		case TopCardFeature.ResourceConstFood:
			if (res == Resource.Food)
				count += card.TopFeatureParam;
			break;
		case TopCardFeature.ResourceConstGold:
			if (res == Resource.Gold)
				count += card.TopFeatureParam;
			break;
		case TopCardFeature.ResourceConstStone:
			if (res == Resource.Stone)
				count += card.TopFeatureParam;
			break;
		case TopCardFeature.ResourceRandomForest:
			if (res == Resource.Forest)
				count += (int)(card.TopFeatureParam*3.5f/Game.GetResourceCost(res));
			break;
		case TopCardFeature.ResourceRandomGold:
			if (res == Resource.Gold)
				count += (int)(card.TopFeatureParam*3.5f/Game.GetResourceCost(res));
			break;
		case TopCardFeature.ResourceRandomStone:
			if (res == Resource.Forest)
				count += (int)(card.TopFeatureParam*3.5f/Game.GetResourceCost(res));
			break;
		}
		return count;
	}
	private static List<int> GetWhereToGoOptionInds(Game game, PlayerModel player) {
		List<WhereToGo> options = game.GetAvailableTargets (player);
		List<int> optionInds = new List<int> ();
		for (int j = 0; j < options.Count; j++)
			optionInds.Add ((int)options[j]-1);
		if (optionInds.Count == 0) {
			Debug.Log ("hi");
			options = game.GetAvailableTargets (player);
		}
		return optionInds;
	}
	NeuralNetwork _whereToGoDecider;
	public override void SelectWhereToGo (Game game, Action<WhereToGo> onComplete) {
		int decisionInd = GetDecisionInd (DecisionType.SelectWhereToGo, game, _model, _whereToGoDecider, null, -1, Resource.None, WhereToGo.None );
		WhereToGo res = (WhereToGo)(decisionInd + 1);
		onComplete (res);
	}
	#endregion

	#region Get used humans
	private void InitGetUsedHumans() {
		_getUsedHumansDecider = new NeuralNetwork (new int[4]{ 9, 6, 6, 7 });
	}
	private static int[] GetUsedHumansInputs(Game game, PlayerModel player, WhereToGo whereToGo) {
		// Can only be resource mining turn.
		Resource receivedResource = Game.GetResourceFromTarget (whereToGo);
		int resourceCost = Game.GetResourceCost (receivedResource);

		int[] inputs = new int[9];
		int i = 0;
		inputs [i] = game.TurnInd; i++;
		inputs [i] = resourceCost; i++;
		inputs [i] = Indicator(receivedResource == Resource.Food); i++;
		inputs [i] = Indicator(receivedResource == Resource.Forest); i++;
		inputs [i] = Indicator(receivedResource == Resource.Clay); i++;
		inputs [i] = Indicator(receivedResource == Resource.Stone); i++;
		inputs [i] = Indicator(receivedResource == Resource.Gold); i++;
		inputs [i] = player.InstrumentsCountSlot1+player.InstrumentsCountSlot2+player.InstrumentsCountSlot3; i++;
		inputs [i] = Mathf.Max(0, player.HumansCount-player.FieldsCount-player.Food); i++; // Needed food.
		return inputs;
	}
	private static List<int> GetUsedHumansOptionInds(Game game, PlayerModel player, WhereToGo whereToGo) {
		int max = game.GetMaxHumansCountFor (whereToGo);
		int min = game.GetMinHumansCountFor (whereToGo);
		int freePlacesCount = game.GetAvailableHumansCountFor (whereToGo);
		max = Mathf.Min (max, freePlacesCount);
		max = Mathf.Min (max, player.AvailableHumans);

		List<int> optionInds = new List<int> ();
		for (int j = min; j <= max; j++)
			optionInds.Add (j - 1);
		return optionInds;
	}
	NeuralNetwork _getUsedHumansDecider;

	const int GetUsedHumansInputsCount = 9;
	private Decider _humansCountDecider;
	public override void SelectUsedHumans (Game game, WhereToGo whereToGo, Action<int> onComplete) {
		int decisionInd = GetDecisionInd (DecisionType.SelectUsedHumans, game, _model, _getUsedHumansDecider, null, -1, Resource.None, whereToGo);
		onComplete (decisionInd+1);
	}
	#endregion

	public override void UseGetAnyResourceFromTopCard (Game game, Action<bool> onComplete) {
		// Drop this decision.
		onComplete (true);
	}
	public override void ChooseResourceToReceiveFromTopCard (Game game, Action<Resource> onComplete) {
		// Drop this decision.
		onComplete (Resource.Gold);
	}

	#region Charity
	NeuralNetwork _charityDecider;
	private void InitCharity() {
		_charityDecider = new NeuralNetwork (new int[4]{ 7, 5, 5, 6 });
	}
	private static int[] GetCharityInputs(Game game, PlayerModel player) {
		int[] inputs = new int[7];
		int i = 0;
		inputs [i] = game.TurnInd; i++;
		inputs [i] = player.Forest; i++;
		inputs [i] = player.Clay; i++;
		inputs [i] = player.Stone; i++;
		inputs [i] = player.Gold; i++;
		inputs [i] = player.InstrumentsCountSlot1+player.InstrumentsCountSlot2+player.InstrumentsCountSlot3; i++;
		inputs [i] = player.FieldsCount; i++;
		return inputs;
	}
	private static List<int> GetCharityOptionInds(Game game, PlayerModel player, List<int> randoms) {
		List<int> optionInds = new List<int> ();
		for (int j = 0; j < 6; j++) {
			if (randoms.Contains (j))
				optionInds.Add (j);
		}
		return optionInds;
	}

	public override void ChooseItemToReceiveFromCharityCard (Game game, List<int> randoms, Action<int> onComplete) {
		int decisionInd = GetDecisionInd (DecisionType.SelectCharity, game, _model, _charityDecider, randoms, -1, Resource.None, WhereToGo.None);
		onComplete (decisionInd);
	}
	#endregion

	#region Instruments
	NeuralNetwork _instrumentsDecider;
	private void InitInstruments() {
		_instrumentsDecider = new NeuralNetwork (new int[4]{ 10, 5, 5, 4 });
	}
	private static int[] GetInstrumentsInputs(Game game, PlayerModel player, Resource receivedRecource) {
		int availableSlot1Instruments = player.InstrumentsSlot1Used ? 0 : player.InstrumentsCountSlot1;
		int availableSlot2Instruments = player.InstrumentsSlot2Used ? 0 : player.InstrumentsCountSlot2;
		int availableSlot3Instruments = player.InstrumentsSlot3Used ? 0 : player.InstrumentsCountSlot3;
		int availableTop4Instruments =  (player.Top4Instruments != null && !player.Top4Instruments.Card.TopUsed) ? 4 : 0;
		int availableTop3Instruments =  (player.Top3Instruments != null && !player.Top3Instruments.Card.TopUsed) ? 3 : 0;
		int availableTop2Instruments =  (player.Top2Instruments != null && !player.Top2Instruments.Card.TopUsed) ? 2 : 0;
		int cost = Game.GetResourceCost (receivedRecource);
		int[] inputs = new int[10];
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
		inputs [i] = player.GetResourceCount(receivedRecource); i++;
		return inputs;
	}
	private static List<int> GetInstrumentsOptionInds(Game game, PlayerModel player, List<int> randoms, int points, Resource receivedRecource) {
		int availableSlot1Instruments = player.InstrumentsSlot1Used ? 0 : player.InstrumentsCountSlot1;
		int availableSlot2Instruments = player.InstrumentsSlot2Used ? 0 : player.InstrumentsCountSlot2;
		int availableSlot3Instruments = player.InstrumentsSlot3Used ? 0 : player.InstrumentsCountSlot3;
		int availableTop4Instruments =  (player.Top4Instruments != null && !player.Top4Instruments.Card.TopUsed) ? 4 : 0;
		int availableTop3Instruments =  (player.Top3Instruments != null && !player.Top3Instruments.Card.TopUsed) ? 3 : 0;
		int availableTop2Instruments =  (player.Top2Instruments != null && !player.Top2Instruments.Card.TopUsed) ? 2 : 0;

		List<int> optionInds = new List<int> ();

		int cost = Game.GetResourceCost (receivedRecource);
		int receivedFromDices = points / cost;
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
		return optionInds;
	}

	public override void GetUsedInstrumentSlotInd (Game game, Resource receivedRecource, int points, OnInstrumentsToUseSelected onComplete) {
		int decision = GetDecisionInd (DecisionType.SelectInstruments, game, _model, _instrumentsDecider, null, points, receivedRecource, WhereToGo.None);

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
		int notUsedPoints = points - receivedFromDices * cost;
		int maxInstrumentsPoints = availableSlot1Instruments + availableSlot2Instruments + availableSlot3Instruments
			+ availableTop4Instruments + availableTop3Instruments + availableTop2Instruments;

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
	#endregion

	public override void BuildCard (Game game, int cardInd, Action<bool> onComplete) {
		// Always build card if possible.
		onComplete (true);
	}

	static List<Resource> _resourcesForCardBuilding = new List<Resource>() { Resource.Forest, Resource.Clay, Resource.Stone, Resource.Gold };
	public override void GetUsedResourceForCardBuilding (Game game, CardToBuild card, List<Resource> alreadySelectedResources, Action<Resource> onComplete) {
		// Drop this decision.
		int[] remainingResources = Game.GetRemainingResourcesAfterHousesBuilding (game, _model);
		foreach (Resource res in alreadySelectedResources)
			remainingResources [(int)res]--;
		foreach (var res in _resourcesForCardBuilding) {
			if (remainingResources [(int)res] > 0) {
				onComplete (res);
				return;
			}
		}
		remainingResources [(int)Resource.Forest] = _model.Forest;
		remainingResources [(int)Resource.Clay] = _model.Clay;
		remainingResources [(int)Resource.Stone] = _model.Stone;
		remainingResources [(int)Resource.Gold] = _model.Gold;
		foreach (Resource res in alreadySelectedResources)
			remainingResources [(int)res]--;
		foreach (var res in _resourcesForCardBuilding) {
			if (remainingResources [(int)res] > 0) {
				onComplete (res);
				return;
			}
		}
		onComplete (Resource.None);
	}

	public override void BuildHouse (Game game, int houseInd, Action<bool> onComplete) {
		onComplete (true); // Always build house if possible.
	}
	public override void GetUsedResourceForHouseBuilding (Game game, HouseToBuild house, List<Resource> options, List<Resource> spendResources, Action<Resource> onComplete) {
		// Drop this decision.
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

	#region Leave hungry
	NeuralNetwork _hungryDecider;
	private void InitHungry() {
		_hungryDecider = new NeuralNetwork (new int[4]{ 5, 3, 3, 2 });
	}
	private static int[] GetHungryInputs(Game game, PlayerModel player) {
		int resourcesToEat = player.HumansCount - player.FieldsCount - player.Food;
		int eatenForest = Mathf.Min (resourcesToEat, player.Forest);
		resourcesToEat -= eatenForest;
		int eatenClay = Mathf.Min (resourcesToEat, player.Clay);
		resourcesToEat -= eatenClay;
		int eatenStone = Mathf.Min (resourcesToEat, player.Stone);
		resourcesToEat -= eatenStone;
		int eatenGold = Mathf.Min (resourcesToEat, player.Gold);
		resourcesToEat -= eatenGold;

		int[] inputs = new int[5];
		int i = 0;
		inputs [i] = game.TurnInd; i++;
		inputs [i] = eatenForest; i++;
		inputs [i] = eatenClay; i++;
		inputs [i] = eatenStone; i++;
		inputs [i] = eatenGold; i++;
		return inputs;
	}
	private static List<int> GetHungryOptionInds(Game game, PlayerModel player) {
		List<int> optionInds = new List<int> ();
		optionInds.Add (0);
		optionInds.Add (1);
		return optionInds;
	}

	public override void LeaveHungry (Game game, int eatenResources, Action<bool> onComplete) {
		int decisionInd = GetDecisionInd (DecisionType.SelectLeaveHungry, game, _model, _hungryDecider, null, -1, Resource.None, WhereToGo.None);
		bool selectTrue = decisionInd == 1;
		onComplete (selectTrue);
	}
	#endregion
}