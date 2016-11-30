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
[Serializable]
public abstract class Player {
	[NonSerialized]
	protected PlayerModel _model;
	public PlayerModel Model { get { return _model; } }
	public PlayerModel.Color Color { get { return _model.CurrColor; } }
	public Player() {
	}
	public void Init(PlayerModel model) {
		_model = model;
	}
	public delegate void OnInstrumentsToUseSelected(bool useSlot0, bool useSlot1, bool useSlot2, bool useOnce4Slot, bool useOnce3Slot, bool useOnce2Slot);
	public abstract void SelectWhereToGo (Game game, Action<WhereToGo> onComplete);
	public abstract void SelectUsedHumans (Game game, WhereToGo whereToGo, Action<int> onComplete);
	public abstract void UseGetAnyResourceFromTopCard (Game game, Action<bool> onComplete);
	public abstract void ChooseResourceToReceiveFromTopCard (Game game, Action<Resource> onComplete);
	public abstract void GetUsedInstrumentSlotInd (Game game, Resource receivedReceource, int points, OnInstrumentsToUseSelected onComplete); // -1 if not using any.
	public abstract void BuildCard (Game game, int cardInd, Action<bool> onComplete);
	public abstract void GetUsedResourceForCardBuilding(Game game, CardToBuild card, List<Resource> alreadySelectedResources, Action<Resource> onComplete);
	public abstract void ChooseItemToReceiveFromCharityCard (Game game, List<int> randoms, Action<int> onComplete);
	public abstract void BuildHouse (Game game, int houseInd, Action<bool> onComplete);
	public abstract void GetUsedResourceForHouseBuilding(Game game, HouseToBuild house, List<Resource> options, List<Resource> spendResources, Action<Resource> onComplete);
	public abstract void LeaveHungry (Game game, int eatenResources, Action<bool> onComplete);
	public abstract Player Clone ();
}
public class HumanPlayer:Player {
	HumanTurnView _turnView;
	public HumanPlayer(HumanTurnView turnView) {
		_turnView = turnView;
	}
	public override Player Clone () {
		return new HumanPlayer (_turnView);
	}
	public override void SelectWhereToGo (Game game, Action<WhereToGo> onComplete)
	{
		_turnView.SelectPlayer (game, _model);
		_turnView.ShowWhereToGo (game, _model);
		while (true) {
			System.Threading.Thread.Sleep (1000);
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
				return;
			}
		}
	}
	public override void SelectUsedHumans (Game game, WhereToGo whereToGo, Action<int> onComplete)
	{
		_turnView.SelectPlayer (game, _model);
		_turnView.ShowHumansCount (game, _model, whereToGo, Color);
		while (true) {
			System.Threading.Thread.Sleep (1000);
			if (_turnView.SelectedHumansCount != -1) {
				onComplete (_turnView.SelectedHumansCount);
				return;
			}
		}
	}


	public override void UseGetAnyResourceFromTopCard (Game game, Action<bool> onComplete)
	{
		_turnView.SelectPlayer (game, _model);
		_turnView.ShowSelectAnyResource ();
		while (true) {
			System.Threading.Thread.Sleep (1000);
			if (_turnView.SelectedAnyResourceDontUse || _turnView.SelectedAnyResourceUse) {
				onComplete (_turnView.SelectedAnyResourceUse);
				return;
			}
		}
	}
	public override void ChooseResourceToReceiveFromTopCard (Game game, Action<Resource> onComplete) {
		_turnView.SelectPlayer (game, _model);
		_turnView.ShowSelectResource ();
		while (true) {
			System.Threading.Thread.Sleep (1000);
			if (_turnView.SelectedResource != Resource.None) {
				onComplete (_turnView.SelectedResource);
				return;
			}
		}
	}
	public override void ChooseItemToReceiveFromCharityCard (Game game, List<int> randoms, Action<int> onComplete)
	{
		_turnView.SelectPlayer (game, _model);
		_turnView.ShowSelectingItemFromCharityCard(_model, randoms);
		while (true) {
			System.Threading.Thread.Sleep (1000);
			if (_turnView.SelectedItemFromCharityCard!=-1) {
				onComplete (_turnView.SelectedItemFromCharityCard);
				return;
			}
		}
	}

	public override void GetUsedInstrumentSlotInd (Game game, Resource receivedReceource, int points, OnInstrumentsToUseSelected onComplete)
	{
		_turnView.SelectPlayer (game, _model);
		_turnView.ShowSelectInstruments(_model, receivedReceource, points);
		while (true) {
			System.Threading.Thread.Sleep (1000);
			if (_turnView.SelectingInstrumentsDone) {
				onComplete (_turnView.InstrumentSlot0Used, _turnView.InstrumentSlot1Used, _turnView.InstrumentSlot2Used, 
					_turnView.Instrument4OnceUsed, _turnView.Instrument3OnceUsed, _turnView.Instrument2OnceUsed);
				return;
			}
		}
	}
	public override void BuildCard (Game game, int cardInd, Action<bool> onComplete)
	{
		_turnView.SelectPlayer (game, _model);
		_turnView.ShowBuildCard(cardInd);
		while (true) {
			System.Threading.Thread.Sleep (1000);
			if (_turnView.SelectingBuildCardDone) {
				onComplete (_turnView.SelectedBuildCard);
				return;
			}
		}
	}
	public override void GetUsedResourceForCardBuilding (Game game, CardToBuild card, List<Resource> alreadySelectedResources, Action<Resource> onComplete)
	{
		_turnView.SelectPlayer (game, _model);
		_turnView.ShowSelectResourceForCard(_model, alreadySelectedResources);
		while (true) {
			System.Threading.Thread.Sleep (1000);
			if (_turnView.SelectedResourceForCardBuilding!=Resource.None) {
				onComplete (_turnView.SelectedResourceForCardBuilding);
				return;
			}
		}
	}
	public override void BuildHouse (Game game, int houseInd, Action<bool> onComplete)
	{
		_turnView.SelectPlayer (game, _model);
		_turnView.ShowSelectBuildingHouse(houseInd);
		while (true) {
			System.Threading.Thread.Sleep (1000);
			if (_turnView.SelectingBuildHouseDone) {
				onComplete (_turnView.SelectedToBuildHouse);
				return;
			}
		}
	}
	public override void GetUsedResourceForHouseBuilding (Game game, HouseToBuild house, List<Resource> options, List<Resource> spendResources, Action<Resource> onComplete)
	{
		_turnView.SelectPlayer (game, _model);
		_turnView.ShowSelectResourceForBuildingHouse(house, options, spendResources);
		while (true) {
			System.Threading.Thread.Sleep (1000);
			if (_turnView.ResourceForHouseBuildingSelectionDone) {
				onComplete (_turnView.SelectedResourceForHouseBuilding);
				return;
			}
		}
	}
	public override void LeaveHungry (Game game, int eatenResources, Action<bool> onComplete)
	{
		_turnView.SelectPlayer (game, _model);
		_turnView.ShowSelectingLeavingHungry(eatenResources);
		while (true) {
			System.Threading.Thread.Sleep (1000);
			if (_turnView.SelectingLeavingHungryDone) {
				onComplete (_turnView.SelecedLeaveHungry);
				return;
			}
		}
	}
}