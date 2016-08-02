using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WhereToGo {
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
	public Player(PlayerModel model) {
		_model = model;
	}
	public abstract WhereToGo SelectWhereToGo (Game game);
	public abstract int SelectUsedHumans (Game game, WhereToGo whereToGo);
	public abstract bool UseGetAnyResourceFromTopCard (Game game);
	public abstract Resource ChooseResourceToReceive (Game game);
	public abstract int GetUsedInstrumentSlotInd (Game game, Resource receivedReceource, int points); // -1 if not using any.
	public abstract bool UseInstrumentOnce (Game game, Resource receivedReceource, int points, BuiltCard card);
	public abstract bool BuildCard (Game game, int cardInd);
	public abstract Resource GetUsedResourceForCardBuilding(Game game);
	public abstract bool BuildHouse (Game game, int houseInd);
	public abstract Resource GetUsedResourceForHouseBuilding(Game game, HouseToBuild house, List<Resource> spendResources);
	public abstract bool LeaveHungry (Game game);
}
