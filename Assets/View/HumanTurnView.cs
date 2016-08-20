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

	public void ShowWhereToGo(Game game, PlayerModel player) {
		_whereToGo.SetActive (true);
		List<WhereToGo> availableTargets = game.GetAvailableTargets (player);
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
	public void ShowHumansCount(Game game, PlayerModel player, WhereToGo target, PlayerModel.Color color) {
		SelectedHumansCount = -1;
		_selectHumansCount.SetActive (true);
		int maxHumans = Mathf.Min( game.GetAvailableHumansCountFor (target), player.UnspentHumanCount );
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

	#region Select instruments to use
	[SerializeField] GameObject _selectInstruments;
	[SerializeField] List<Image> _instrumentsImages;
	[SerializeField] List<Text> _instrumentsCounts;
	[SerializeField] Image _receivedResource;
	[SerializeField] List<Sprite> _resourceSprites;
	[SerializeField] Text _points;
	[SerializeField] Text _cost;
	public bool SelectingInstrumentsDone { get; private set; }
	public void ShowSelectInstruments(PlayerModel player, Resource receivedReceource, int points) {
		_selectInstruments.SetActive (true);
		SelectingInstrumentsDone = false;
		_points.text = points.ToString();
		_receivedResource.sprite = _resourceSprites [(int)receivedReceource];
		_cost.text = Game.GetResourceCost (receivedReceource).ToString ();

		_instrumentsImages[0].gameObject.SetActive(!player.InstrumentsSlot1Used && player.InstrumentsCountSlot1>0);
		_instrumentsCounts [0].text = player.InstrumentsCountSlot1.ToString ();
		InstrumentSlot0Used = false;
		_instrumentsImages[1].gameObject.SetActive(!player.InstrumentsSlot2Used && player.InstrumentsCountSlot2>0);
		_instrumentsCounts [1].text = player.InstrumentsCountSlot2.ToString ();
		InstrumentSlot1Used = false;
		_instrumentsImages[2].gameObject.SetActive(!player.InstrumentsSlot3Used && player.InstrumentsCountSlot3>0);
		_instrumentsCounts [2].text = player.InstrumentsCountSlot3.ToString ();
		InstrumentSlot2Used = false;

		_instrumentsImages[3].gameObject.SetActive(player.Top4Instruments!=null && !player.Top4Instruments.Card.TopUsed);
		//_instrumentsCounts [3].text = 4.ToString ();
		Instrument4OnceUsed = false;
		_instrumentsImages[4].gameObject.SetActive(player.Top3Instruments!=null && !player.Top3Instruments.Card.TopUsed);
		//_instrumentsCounts [4].text = 3.ToString ();
		Instrument3OnceUsed = false;
		_instrumentsImages[2].gameObject.SetActive(player.Top2Instruments!=null && !player.Top2Instruments.Card.TopUsed);
		//_instrumentsCounts [2].text = 2.ToString ();
		Instrument2OnceUsed = false;
	}
	public bool InstrumentSlot0Used { get; private set; }
	public bool InstrumentSlot1Used { get; private set; }
	public bool InstrumentSlot2Used { get; private set; }
	public bool Instrument4OnceUsed { get; private set; }
	public bool Instrument3OnceUsed { get; private set; }
	public bool Instrument2OnceUsed { get; private set; }
	public void OnInstrumentUsed(GameObject sender) {
		if (_instrumentsImages [0].gameObject == sender)
			InstrumentSlot0Used = true;
		if (_instrumentsImages [1].gameObject == sender)
			InstrumentSlot1Used = true;
		if (_instrumentsImages [2].gameObject == sender)
			InstrumentSlot2Used = true;
		if (_instrumentsImages [3].gameObject == sender)
			Instrument4OnceUsed = true;
		if (_instrumentsImages [4].gameObject == sender)
			Instrument3OnceUsed = true;
		if (_instrumentsImages [5].gameObject == sender)
			Instrument2OnceUsed = true;
	}
	public void OnInstrumentsUsingDone() {
		_selectInstruments.SetActive (false);
		SelectingInstrumentsDone = true;
	}
	#endregion
}
