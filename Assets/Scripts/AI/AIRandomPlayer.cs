using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;


public class AIRandomPlayer:Player {
	System.Random rand;
	public AIRandomPlayer() {
		rand = new System.Random ();
	}
	private float RandomValue { get { return (float)rand.NextDouble (); } }
	private int RandomRange(int min, int max) { return rand.Next()%(max-min)+min; }
	public override void SelectWhereToGo (Game game, Action<WhereToGo> onComplete) {
		List<WhereToGo> options = game.GetAvailableTargets (_model);
		int ind = RandomRange (0, options.Count);
		onComplete (options [ind]);
	}
	public override void SelectUsedHumans (Game game, WhereToGo whereToGo, Action<int> onComplete) {
		int max = game.GetMaxHumansCountFor (whereToGo);
		int min = game.GetMinHumansCountFor (whereToGo);
		int freePlacesCount = game.GetAvailableHumansCountFor (whereToGo);
		int maxTotal = Mathf.Min (max, freePlacesCount);
		maxTotal = Mathf.Min (maxTotal, _model.UnspentHumanCount);
		if (maxTotal < min) {
			onComplete (0);
			return;
		}
			
		int count = RandomRange (min, maxTotal + 1);
		onComplete (count);
	}
	public override void UseGetAnyResourceFromTopCard (Game game, Action<bool> onComplete) {
		onComplete (GetRandomBool());
	}
	private bool GetRandomBool() {
		if (RandomValue> 0.5f)
			return true;
		else
			return false;
	}
	public override void ChooseResourceToReceiveFromTopCard (Game game, Action<Resource> onComplete) {
		int ind = RandomRange (0, 4);
		switch (ind) {
			default:
			case 0: onComplete (Resource.Forest); break;
			case 1: onComplete (Resource.Clay); break;
			case 2: onComplete (Resource.Stone); break;
			case 3: onComplete (Resource.Gold); break;
		}
	}
	public override void ChooseItemToReceiveFromCharityCard (Game game, List<int> randoms, Action<int> onComplete) {
		int ind = RandomRange (0, 5);
		onComplete (ind);
	}
	public override void GetUsedInstrumentSlotInd (Game game, Resource receivedReceource, int points, OnInstrumentsToUseSelected onComplete) {
		bool useSlot0 = !_model.InstrumentsSlot1Used && _model.InstrumentsCountSlot1 > 0;
		bool useSlot1 = !_model.InstrumentsSlot2Used && _model.InstrumentsCountSlot2 > 0;
		bool useSlot2 = !_model.InstrumentsSlot3Used && _model.InstrumentsCountSlot3 > 0;
		bool useOnceSlot4 = _model.Top4Instruments != null && !_model.Top4Instruments.Card.TopUsed;
		bool useOnceSlot3 = _model.Top3Instruments != null && !_model.Top3Instruments.Card.TopUsed;
		bool useOnceSlot2 = _model.Top2Instruments != null && !_model.Top2Instruments.Card.TopUsed;
		useSlot0 &= GetRandomBool ();
		useSlot1 &= GetRandomBool ();
		useSlot2 &= GetRandomBool ();
		useOnceSlot4 &= GetRandomBool ();
		useOnceSlot3 &= GetRandomBool ();
		useOnceSlot2 &= GetRandomBool ();
		onComplete (useSlot0, useSlot1, useSlot2, useOnceSlot4, useOnceSlot3, useOnceSlot2);
	}
	public override void BuildCard (Game game, int cardInd, Action<bool> onComplete) {
		onComplete (GetRandomBool ());
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
		foreach (Resource res in alreadySelectedResources) {
			int ind = options.IndexOf (res);
			options.RemoveAt (ind);
		}
		int index = RandomRange(0, options.Count);
		onComplete (options [index]);
	}
	public override void BuildHouse (Game game, int houseInd, Action<bool> onComplete) {
		onComplete (GetRandomBool ());
	}
	public override void GetUsedResourceForHouseBuilding (Game game, HouseToBuild house, List<Resource> options, List<Resource> spendResources, Action<Resource> onComplete) {
		onComplete (options [RandomRange (0, options.Count)]);
	}
	public override void LeaveHungry (Game game, int eatenResources, Action<bool> onComplete) {
		onComplete (GetRandomBool ());
	}
}