using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum WhereToGo {
	None, // For errors
	Field,
	Housing,
	Instrument,
	Food,
	Forest,
	Clay,
	Stone,
	Gold,
	House1,
	House2,
	House3,
	House4,
	Card1,
	Card2,
	Card3,
	Card4
}
public abstract class Player {
	protected PlayerModel _model;
	public Player() {
	}
	public void Init(PlayerModel model) {
		_model = model;
	}
	public abstract IEnumerator SelectWhereToGo (Game game, Action<WhereToGo> onComplete);
	public abstract IEnumerator SelectUsedHumans (Game game, WhereToGo whereToGo, Action<int> onComplete);
	public abstract IEnumerator UseGetAnyResourceFromTopCard (Game game, Action<bool> onComplete);
	public abstract IEnumerator ChooseResourceToReceive (Game game, Action<Resource> onComplete);
	public abstract IEnumerator GetUsedInstrumentSlotInd (Game game, Resource receivedReceource, int points, Action<int> onComplete); // -1 if not using any.
	public abstract IEnumerator UseInstrumentOnce (Game game, Resource receivedReceource, int points, BuiltCard card, Action<bool> onComplete);
	public abstract IEnumerator BuildCard (Game game, int cardInd, Action<bool> onComplete);
	public abstract IEnumerator GetUsedResourceForCardBuilding(Game game, Action<Resource> onComplete);
	public abstract IEnumerator BuildHouse (Game game, int houseInd, Action<bool> onComplete);
	public abstract IEnumerator GetUsedResourceForHouseBuilding(Game game, HouseToBuild house, List<Resource> spendResources, Action<Resource> onComplete);
	public abstract IEnumerator LeaveHungry (Game game, Action<bool> onComplete);
}
public class HumanPlayer:Player {
	public override IEnumerator SelectWhereToGo (Game game, Action<WhereToGo> onComplete)
	{
		onComplete (WhereToGo.Food);
		yield break;
	}
	public override IEnumerator SelectUsedHumans (Game game, WhereToGo whereToGo, Action<int> onComplete)
	{
		onComplete (7);
		yield break;
	}
	public override IEnumerator UseGetAnyResourceFromTopCard (Game game, Action<bool> onComplete)
	{
		throw new NotImplementedException ();
	}
	public override IEnumerator ChooseResourceToReceive (Game game, Action<Resource> onComplete)
	{
		throw new NotImplementedException ();
	}
	public override IEnumerator GetUsedInstrumentSlotInd (Game game, Resource receivedReceource, int points, Action<int> onComplete)
	{
		throw new NotImplementedException ();
	}
	public override IEnumerator UseInstrumentOnce (Game game, Resource receivedReceource, int points, BuiltCard card, Action<bool> onComplete)
	{
		throw new NotImplementedException ();
	}
	public override IEnumerator BuildCard (Game game, int cardInd, Action<bool> onComplete)
	{
		throw new NotImplementedException ();
	}
	public override IEnumerator GetUsedResourceForCardBuilding (Game game, Action<Resource> onComplete)
	{
		throw new NotImplementedException ();
	}
	public override IEnumerator BuildHouse (Game game, int houseInd, Action<bool> onComplete)
	{
		throw new NotImplementedException ();
	}
	public override IEnumerator GetUsedResourceForHouseBuilding (Game game, HouseToBuild house, List<Resource> spendResources, Action<Resource> onComplete)
	{
		throw new NotImplementedException ();
	}
	public override IEnumerator LeaveHungry (Game game, Action<bool> onComplete)
	{
		throw new NotImplementedException ();
	}
}