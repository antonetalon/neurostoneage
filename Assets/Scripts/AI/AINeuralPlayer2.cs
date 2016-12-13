using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class AINeuralPlayer2:Player {
	private static int Indicator(bool condition) {
		return condition ? 1 : 0;
	}

	public AINeuralPlayer2() {
		InitWhereToGo ();
		InitGetUsedHumans ();
	}
	public override Player Clone() {
		AINeuralPlayer2 clone = new AINeuralPlayer2 ();
		clone._whereToGoDecider = _whereToGoDecider.Clone();
		clone._getUsedHumansDecider = _getUsedHumansDecider.Clone();
		return clone;
	}
		
	private int GetDecisionInd(DecisionType type, Game game, PlayerModel player, List<int> randoms, int points, Resource receivedRecource, WhereToGo whereToGo) {
		List<int> optionInds = AINeuralPlayer.GetOptionInds (type, game, _model, randoms, points, receivedRecource, whereToGo);
		int[] inputs = AINeuralPlayer.GetInputs (type, game, _model, receivedRecource, whereToGo);
		double[] inputsDouble = new double[inputs.Length];
		for (int i = 0; i < inputs.Length; i++)
			inputsDouble [i] = inputs [i];
		NeuralDecisionNetwork chooser = GetChooserDecider (type);
		int decisionInd = chooser.Think (inputs, optionInds);
		return decisionInd;
	}
	public NeuralDecisionNetwork GetChooserDecider(DecisionType type) {
		switch (type) {
			case DecisionType.SelectWhereToGo: return _whereToGoDecider;
			case DecisionType.SelectUsedHumans: return _getUsedHumansDecider;
			default: return null;
		}
	}

	#region Where to go
	private void InitWhereToGo() {
		_whereToGoDecider = new NeuralDecisionNetwork (81, 16, new int[2]{ 50, 30 });
	}
	NeuralDecisionNetwork _whereToGoDecider;
	public override void SelectWhereToGo (Game game, Action<WhereToGo> onComplete) {
		int decisionInd = GetDecisionInd (DecisionType.SelectWhereToGo, game, _model, null, -1, Resource.None, WhereToGo.None );
		WhereToGo res = (WhereToGo)(decisionInd + 1);
		onComplete (res);
	}
	#endregion

	#region Get used humans
	private void InitGetUsedHumans() {
		_getUsedHumansDecider = new NeuralDecisionNetwork (9, 10, new int[2]{ 6, 6 });
	}
	NeuralDecisionNetwork _getUsedHumansDecider;

	const int GetUsedHumansInputsCount = 9;
	private NeuralDecisionNetwork _humansCountDecider;
	public override void SelectUsedHumans (Game game, WhereToGo whereToGo, Action<int> onComplete) {
		int decisionInd = GetDecisionInd (DecisionType.SelectUsedHumans, game, _model, null, -1, Resource.None, whereToGo);
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

	public override void ChooseItemToReceiveFromCharityCard (Game game, List<int> randoms, Action<int> onComplete) {
		// Drop this decision.
		int max = -1;
		foreach (int curr in randoms) {
			if (curr > max)
				max = curr;
		}
		onComplete (max);
	}

	#region Instruments
	public override void GetUsedInstrumentSlotInd (Game game, Resource receivedRecource, int points, OnInstrumentsToUseSelected onComplete) {
		List<int> options = AINeuralPlayer.GetInstrumentsOptionInds (game, _model, null, points, receivedRecource);
		int decision = options [options.Count - 1];

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

	public override void GetUsedResourceForCardBuilding (Game game, CardToBuild card, List<Resource> alreadySelectedResources, Action<Resource> onComplete) {
		// Drop this decision.
		int[] remainingResources = Game.GetRemainingResourcesAfterHousesBuilding (game, _model);
		foreach (Resource res in alreadySelectedResources)
			remainingResources [(int)res]--;
		foreach (var res in AINeuralPlayer.ResourcesForCardBuilding) {
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
		foreach (var res in AINeuralPlayer.ResourcesForCardBuilding) {
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
	public override void LeaveHungry (Game game, int eatenResources, Action<bool> onComplete) {
		// Drop this decision.
		onComplete (false);
	}
}