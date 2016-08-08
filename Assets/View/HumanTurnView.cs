﻿using UnityEngine;
using System.Collections;

public class HumanTurnView : MonoBehaviour {
	[SerializeField] GameObject _whereToGo;
	[SerializeField] GameObject _field;
	[SerializeField] GameObject _housing;
	[SerializeField] GameObject _instrument;
	[SerializeField] GameObject _food;
	[SerializeField] GameObject _forest;
	[SerializeField] GameObject _clay;
	[SerializeField] GameObject _stone;
	[SerializeField] GameObject _gold;
	[SerializeField] GameObject _house1;
	[SerializeField] GameObject _house2;
	[SerializeField] GameObject _house3;
	[SerializeField] GameObject _house4;
	[SerializeField] GameObject _card1;
	[SerializeField] GameObject _card2;
	[SerializeField] GameObject _card3;
	[SerializeField] GameObject _card4;

	public void ShowWhereToGo(Game game) {
		_whereToGo.SetActive (true);
		_field.SetActive (game.AvailableFieldPlaces>0);
		_housing.SetActive (game.AvailableHousingPlaces>0);
		_instrument.SetActive (game.AvailableInstrumentsPlaces>0);
		_food.SetActive (true);
		_forest.SetActive (game.AvailableForestPlaces>0);
		_clay.SetActive (game.AvailableClayPlaces>0);
		_stone.SetActive (game.AvailableStonePlaces>0);
		_gold.SetActive (game.AvailableGoldPlaces>0);
		_house1.SetActive (game.AvailableHouse1Places>0);
		_house2.SetActive (game.AvailableHouse2Places>0);
		_house3.SetActive (game.AvailableHouse3Places>0);
		_house4.SetActive (game.AvailableHouse4Places>0);
		_card1.SetActive (game.AvailableCard1Places>0);
		_card2.SetActive (game.AvailableCard2Places>0);
		_card3.SetActive (game.AvailableCard3Places>0);
		_card4.SetActive (game.AvailableCard4Places>0);
		FieldPressed = false;
		HousingPressed = false;
		InstrumentPressed = false;
		FoodPressed = false;
		ForestPressed = false;
		ClayPressed = false;
		StonePressed = false;
		GoldPressed = false;
		House1Pressed = false;
		House2Pressed = false;
		House3Pressed = false;
		House4Pressed = false;
		Card1Pressed = false;
		Card2Pressed = false;
		Card3Pressed = false;
		Card4Pressed = false;
	}

	public bool FieldPressed { get; private set; }
	public void OnFieldPressed() {
		FieldPressed = true;
		_whereToGo.SetActive (false);
	}
	public bool HousingPressed { get; private set; }
	public void OnHousingPressed() {
		HousingPressed = true;
		_whereToGo.SetActive (false);
	}
	public bool InstrumentPressed { get; private set; }
	public void OnInstrumentPressed() {
		InstrumentPressed = true;
		_whereToGo.SetActive (false);
	}
	public bool FoodPressed { get; private set; }
	public void OnFoodPressed() {
		FoodPressed = true;
		_whereToGo.SetActive (false);
	}
	public bool ForestPressed { get; private set; }
	public void OnForestdPressed() {
		ForestPressed = true;
		_whereToGo.SetActive (false);
	}
	public bool ClayPressed { get; private set; }
	public void OnClayPressed() {
		ClayPressed = true;
		_whereToGo.SetActive (false);
	}
	public bool StonePressed { get; private set; }
	public void OnStonePressed() {
		StonePressed = true;
		_whereToGo.SetActive (false);
	}
	public bool GoldPressed { get; private set; }
	public void OnGoldPressed() {
		GoldPressed = true;
		_whereToGo.SetActive (false);
	}
	public bool House1Pressed { get; private set; }
	public void OnHouse1Pressed() {
		House1Pressed = true;
		_whereToGo.SetActive (false);
	}
	public bool House2Pressed { get; private set; }
	public void OnHouse2Pressed() {
		House2Pressed = true;
		_whereToGo.SetActive (false);
	}
	public bool House3Pressed { get; private set; }
	public void OnHouse3Pressed() {
		House3Pressed = true;
		_whereToGo.SetActive (false);
	}
	public bool House4Pressed { get; private set; }
	public void OnHouse4Pressed() {
		House4Pressed = true;
		_whereToGo.SetActive (false);
	}
	public bool Card1Pressed { get; private set; }
	public void OnCard1Pressed() {
		Card1Pressed = true;
		_whereToGo.SetActive (false);
	}
	public bool Card2Pressed { get; private set; }
	public void OnCard2Pressed() {
		Card2Pressed = true;
		_whereToGo.SetActive (false);
	}
	public bool Card3Pressed { get; private set; }
	public void OnCard3Pressed() {
		Card3Pressed = true;
		_whereToGo.SetActive (false);
	}
	public bool Card4Pressed { get; private set; }
	public void OnCard4Pressed() {
		Card4Pressed = true;
		_whereToGo.SetActive (false);
	}

}
