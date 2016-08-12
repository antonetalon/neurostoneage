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
	public PlayerModel.Color Color { get { return _model.CurrColor; } }
	public Player() {
	}
	public void Init(PlayerModel model) {
		_model = model;
	}
	public abstract IEnumerator SelectWhereToGo (Game game, Action<WhereToGo> onComplete);
	public abstract IEnumerator SelectUsedHumans (Game game, WhereToGo whereToGo, Action<int> onComplete);
	public abstract IEnumerator UseGetAnyResourceFromTopCard (Game game, Action<bool> onComplete);
	public abstract IEnumerator ChooseResourceToReceiveFromTopCard (Game game, Action<Resource> onComplete);
	public abstract IEnumerator GetUsedInstrumentSlotInd (Game game, Resource receivedReceource, int points, Action<int> onComplete); // -1 if not using any.
	public abstract IEnumerator UseInstrumentOnce (Game game, Resource receivedReceource, int points, BuiltCard card, Action<bool> onComplete);
	public abstract IEnumerator BuildCard (Game game, int cardInd, Action<bool> onComplete);
	public abstract IEnumerator GetUsedResourceForCardBuilding(Game game, Action<Resource> onComplete);
	public abstract IEnumerator BuildHouse (Game game, int houseInd, Action<bool> onComplete);
	public abstract IEnumerator GetUsedResourceForHouseBuilding(Game game, HouseToBuild house, List<Resource> spendResources, Action<Resource> onComplete);
	public abstract IEnumerator LeaveHungry (Game game, Action<bool> onComplete);
}
public class HumanPlayer:Player {
	HumanTurnView _turnView;
	public HumanPlayer(HumanTurnView turnView) {
		_turnView = turnView;
	}
	public override IEnumerator SelectWhereToGo (Game game, Action<WhereToGo> onComplete)
	{
		_turnView.ShowWhereToGo (game);
		while (true) {
			yield return new WaitForEndOfFrame ();
			WhereToGo res = WhereToGo.None;
			if (_turnView.FieldPressed)
				res = WhereToGo.Field;
			if (_turnView.HousingPressed)
				res = WhereToGo.Housing;
			if (_turnView.InstrumentPressed)
				res = WhereToGo.Instrument;
			if (_turnView.FoodPressed)
				res = WhereToGo.Food;
			if (_turnView.ForestPressed)
				res = WhereToGo.Forest;
			if (_turnView.ClayPressed)
				res = WhereToGo.Clay;
			if (_turnView.StonePressed)
				res = WhereToGo.Stone;
			if (_turnView.GoldPressed)
				res = WhereToGo.Gold;
			if (_turnView.House1Pressed)
				res = WhereToGo.House1;
			if (_turnView.House2Pressed)
				res = WhereToGo.House2;
			if (_turnView.House3Pressed)
				res = WhereToGo.House3;
			if (_turnView.House4Pressed)
				res = WhereToGo.House4;
			if (_turnView.Card1Pressed)
				res = WhereToGo.Card1;
			if (_turnView.Card2Pressed)
				res = WhereToGo.Card2;
			if (_turnView.Card3Pressed)
				res = WhereToGo.Card3;
			if (_turnView.Card4Pressed)
				res = WhereToGo.Card4;
			if (res != WhereToGo.None) {
				onComplete (res);
				yield break;
			}
		}
	}
	public override IEnumerator SelectUsedHumans (Game game, WhereToGo whereToGo, Action<int> onComplete)
	{
		_turnView.ShowHumansCount (game, whereToGo, Color);
		while (true) {
			yield return new WaitForEndOfFrame ();
			if (_turnView.SelectedHumansCount != -1) {
				onComplete (_turnView.SelectedHumansCount);
				yield break;
			}
		}
	}
	public override IEnumerator UseGetAnyResourceFromTopCard (Game game, Action<bool> onComplete)
	{
		_turnView.ShowSelectAnyResource ();
		while (true) {
			yield return new WaitForEndOfFrame ();
			if (_turnView.SelectedAnyResourceDontUse || _turnView.SelectedAnyResourceUse) {
				onComplete (_turnView.SelectedAnyResourceUse);
				yield break;
			}
		}
	}
	public override IEnumerator ChooseResourceToReceiveFromTopCard (Game game, Action<Resource> onComplete)
	{
		_turnView.ShowSelectResource ();
		while (true) {
			yield return new WaitForEndOfFrame ();
			if (_turnView.SelectedResource != Resource.None) {
				onComplete (_turnView.SelectedResource);
				yield break;
			}
		}
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