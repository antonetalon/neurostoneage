using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HumanTurnView : MonoBehaviour {

	#region Where to go
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
		List<WhereToGo> availableTargets = game.GetAvailableTargets ();
		_field.SetActive (availableTargets.Contains(WhereToGo.Field));
		_housing.SetActive (availableTargets.Contains(WhereToGo.Housing));
		_instrument.SetActive (availableTargets.Contains(WhereToGo.Instrument));
		_food.SetActive (availableTargets.Contains(WhereToGo.Food));
		_forest.SetActive (availableTargets.Contains(WhereToGo.Forest));
		_clay.SetActive (availableTargets.Contains(WhereToGo.Clay));
		_stone.SetActive (availableTargets.Contains(WhereToGo.Stone));
		_gold.SetActive (availableTargets.Contains(WhereToGo.Gold));
		_house1.SetActive (availableTargets.Contains(WhereToGo.House1));
		_house2.SetActive (availableTargets.Contains(WhereToGo.House2));
		_house3.SetActive (availableTargets.Contains(WhereToGo.House3));
		_house4.SetActive (availableTargets.Contains(WhereToGo.House4));
		_card1.SetActive (availableTargets.Contains(WhereToGo.Card1));
		_card2.SetActive (availableTargets.Contains(WhereToGo.Card2));
		_card3.SetActive (availableTargets.Contains(WhereToGo.Card3));
		_card4.SetActive (availableTargets.Contains(WhereToGo.Card4));
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
	#endregion

	#region Select used human
	[SerializeField] GameObject _selectHumansCount;
	[SerializeField] List<Image> _humans;
	[SerializeField] List<Sprite> _playerSprites;
	public int SelectedHumansCount { get; private set; }
	public void ShowHumansCount(Game game, WhereToGo target, PlayerModel.Color color) {
		SelectedHumansCount = -1;
		_selectHumansCount.SetActive (true);
		int maxHumans = game.GetAvailableHumansCountFor (target);
		for (int i = 0; i < _humans.Count; i++) {
			_humans [i].gameObject.SetActive (i < maxHumans);
			_humans[i].sprite = _playerSprites[(int)color];
		}
	}
	public void OnHumansCountPressed(GameObject sender) {
		SelectedHumansCount = -1;
		for (int i = 0; i < _humans.Count; i++) {
			if (sender == _humans [i].gameObject) {
				SelectedHumansCount = i;
				break;
			}
		}
		_selectHumansCount.SetActive (false);
	}
	#endregion

	#region Select use any resource
	[SerializeField] GameObject _selectAnyResource;
	public bool SelectedAnyResourceUse { get; private set; }
	public bool SelectedAnyResourceDontUse { get; private set; }
	public void ShowSelectAnyResource() {
		SelectedAnyResourceUse = false;
		SelectedAnyResourceDontUse = false;
		_selectAnyResource.SetActive (true);
	}
	public void OnAnyResourceUse() {
		SelectedAnyResourceUse = true;
		_selectHumansCount.SetActive (false);
	}
	public void OnAnyResourceDontUse() {
		SelectedAnyResourceDontUse = true;
		_selectHumansCount.SetActive (false);
	}
	#endregion

	#region Select resource
	[SerializeField] GameObject _selectResource;
	[SerializeField] List<Image> _resourceImages;
	public Resource SelectedResource { get; private set; }
	public void ShowSelectResource() {
		SelectedResource = Resource.None;
		_selectResource.SetActive (true);
	}
	public void OnResourceSelectionPressed(GameObject sender) {
		_selectResource.SetActive (false);
		for (int i = 0; i < _resourceImages.Count; i++) {
			if (_resourceImages [i].gameObject == sender)
				SelectedResource = (Resource)i;
		}
	}
	#endregion
}
