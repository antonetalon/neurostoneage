using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HumanTurnView : MonoBehaviour {

	private List<System.Action> _executeAtMainThread = new List<System.Action>();
	public void ExecuteInMainThread(System.Action action) {
		_executeAtMainThread.Add (action);
	}
	void Update() {
		for (int i = 0; i < _executeAtMainThread.Count; i++)
			_executeAtMainThread [i] ();
		_executeAtMainThread.Clear ();
	}

	#region Player selection
	[SerializeField] List<GameObject> _playerSelections;
	public void SelectPlayer(Game game, PlayerModel player) {
		ExecuteInMainThread (() => {
			int ind = game.PlayerModels.IndexOf (player);
			for (int i = 0; i < _playerSelections.Count; i++)
				_playerSelections [i].SetActive (i == ind);
		});
	}
	#endregion

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
		ExecuteInMainThread (() => {
			_whereToGo.SetActive (true);
			List<WhereToGo> availableTargets = game.GetAvailableTargets (player);
			_field.SetActive (availableTargets.Contains (WhereToGo.Field));
			_housing.SetActive (availableTargets.Contains (WhereToGo.Housing));
			_instrument.SetActive (availableTargets.Contains (WhereToGo.Instrument));
			_food.SetActive (availableTargets.Contains (WhereToGo.Food));
			_forest.SetActive (availableTargets.Contains (WhereToGo.Forest));
			_clay.SetActive (availableTargets.Contains (WhereToGo.Clay));
			_stone.SetActive (availableTargets.Contains (WhereToGo.Stone));
			_gold.SetActive (availableTargets.Contains (WhereToGo.Gold));
			_house1.SetActive (availableTargets.Contains (WhereToGo.House1));
			_house2.SetActive (availableTargets.Contains (WhereToGo.House2));
			_house3.SetActive (availableTargets.Contains (WhereToGo.House3));
			_house4.SetActive (availableTargets.Contains (WhereToGo.House4));
			_card1.SetActive (availableTargets.Contains (WhereToGo.Card1));
			_card2.SetActive (availableTargets.Contains (WhereToGo.Card2));
			_card3.SetActive (availableTargets.Contains (WhereToGo.Card3));
			_card4.SetActive (availableTargets.Contains (WhereToGo.Card4));
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
		});
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
		ExecuteInMainThread (() => {
			SelectedHumansCount = -1;
			_selectHumansCount.SetActive (true);
			int maxHumans = Mathf.Min (game.GetAvailableHumansCountFor (target), player.UnspentHumanCount);
			for (int i = 0; i < _humans.Count; i++) {
				_humans [i].gameObject.SetActive (i < maxHumans);
				_humans [i].sprite = _playerSprites [(int)color];
			}
		});
	}
	public void OnHumansCountPressed(GameObject sender) {
		SelectedHumansCount = -1;
		for (int i = 0; i < _humans.Count; i++) {
			if (sender == _humans [i].gameObject) {
				SelectedHumansCount = i+1;
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
		ExecuteInMainThread (() => {
			SelectedAnyResourceUse = false;
			SelectedAnyResourceDontUse = false;
			_selectAnyResource.SetActive (true);
		});
	}
	public void OnAnyResourceUse() {
		SelectedAnyResourceUse = true;
		_selectAnyResource.SetActive (false);
	}
	public void OnAnyResourceDontUse() {
		SelectedAnyResourceDontUse = true;
		_selectAnyResource.SetActive (false);
	}
	#endregion

	#region Select resource
	[SerializeField] GameObject _selectResource;
	[SerializeField] List<Image> _resourceImages;
	public Resource SelectedResource { get; private set; }
	public void ShowSelectResource() {
		ExecuteInMainThread (() => {
			SelectedResource = Resource.None;
			_selectResource.SetActive (true);
		});
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
	private int _pointsCount;
	PlayerModel _player;
	public void ShowSelectInstruments(PlayerModel player, Resource receivedReceource, int points) {
		ExecuteInMainThread (() => {
			_selectInstruments.SetActive (true);
			SelectingInstrumentsDone = false;
			_player = player;
			_pointsCount = points;
			_points.text = points.ToString ();
			_receivedResource.sprite = _resourceSprites [(int)receivedReceource];
			_cost.text = Game.GetResourceCost (receivedReceource).ToString ();

			_instrumentsImages [0].gameObject.SetActive (!player.InstrumentsSlot1Used && player.InstrumentsCountSlot1 > 0);
			_instrumentsCounts [0].text = player.InstrumentsCountSlot1.ToString ();
			InstrumentSlot0Used = false;
			_instrumentsImages [1].gameObject.SetActive (!player.InstrumentsSlot2Used && player.InstrumentsCountSlot2 > 0);
			_instrumentsCounts [1].text = player.InstrumentsCountSlot2.ToString ();
			InstrumentSlot1Used = false;
			_instrumentsImages [2].gameObject.SetActive (!player.InstrumentsSlot3Used && player.InstrumentsCountSlot3 > 0);
			_instrumentsCounts [2].text = player.InstrumentsCountSlot3.ToString ();
			InstrumentSlot2Used = false;

			_instrumentsImages [3].gameObject.SetActive (player.Top4Instruments != null && !player.Top4Instruments.Card.TopUsed);
			//_instrumentsCounts [3].text = 4.ToString ();
			Instrument4OnceUsed = false;
			_instrumentsImages [4].gameObject.SetActive (player.Top3Instruments != null && !player.Top3Instruments.Card.TopUsed);
			//_instrumentsCounts [4].text = 3.ToString ();
			Instrument3OnceUsed = false;
			_instrumentsImages [5].gameObject.SetActive (player.Top2Instruments != null && !player.Top2Instruments.Card.TopUsed);
			//_instrumentsCounts [2].text = 2.ToString ();
			Instrument2OnceUsed = false;
		});
	}
	public bool InstrumentSlot0Used { get; private set; }
	public bool InstrumentSlot1Used { get; private set; }
	public bool InstrumentSlot2Used { get; private set; }
	public bool Instrument4OnceUsed { get; private set; }
	public bool Instrument3OnceUsed { get; private set; }
	public bool Instrument2OnceUsed { get; private set; }
	public void OnInstrumentUsed(GameObject sender) {
		if (_instrumentsImages [0].gameObject == sender) {
			InstrumentSlot0Used = true;
			_pointsCount += _player.InstrumentsCountSlot1;
			_instrumentsImages [0].gameObject.SetActive (false);
		}
		if (_instrumentsImages [1].gameObject == sender) {
			InstrumentSlot1Used = true;
			_pointsCount += _player.InstrumentsCountSlot2;
			_instrumentsImages [1].gameObject.SetActive (false);
		}
		if (_instrumentsImages [2].gameObject == sender) {
			InstrumentSlot2Used = true;
			_pointsCount += _player.InstrumentsCountSlot3;
			_instrumentsImages [2].gameObject.SetActive (false);
		}
		if (_instrumentsImages [3].gameObject == sender){
			Instrument4OnceUsed = true;
			_pointsCount += 4;
			_instrumentsImages [3].gameObject.SetActive (false);
		}
		if (_instrumentsImages [4].gameObject == sender){
			Instrument3OnceUsed = true;
			_pointsCount += 3;
			_instrumentsImages [4].gameObject.SetActive (false);
		}
		if (_instrumentsImages [5].gameObject == sender){
			Instrument2OnceUsed = true;
			_pointsCount += 2;
			_instrumentsImages [5].gameObject.SetActive (false);
		}
		_points.text = _pointsCount.ToString();
	}
	public void OnInstrumentsUsingDone() {
		_selectInstruments.SetActive (false);
		SelectingInstrumentsDone = true;
	}
	#endregion

	#region Charity card
	[SerializeField] GameObject _charityCardSelectingParent;
	[SerializeField] List<Image> _charityCardButtons;
	[SerializeField] List<Sprite> _charityCardSprites;
	public int SelectedItemFromCharityCard { get; private set; }
	List<int> _charityRandoms;
	public void ShowSelectingItemFromCharityCard(PlayerModel model, List<int> randoms) {
		ExecuteInMainThread (() => {
			_charityRandoms = randoms;
			SelectedItemFromCharityCard = -1;
			_charityCardSelectingParent.SetActive (true);
			for (int i = 0; i < _charityCardButtons.Count; i++) {
				_charityCardButtons [i].gameObject.SetActive (randoms.Count > i);
				if (i < randoms.Count) {
					_charityCardButtons [i].sprite = _charityCardSprites [randoms [i]];
					_charityCardButtons [i].SetNativeSize ();
				}
			}
		});
	}
	public void OnItemFromCharityCardSelected(GameObject sender) {
		int ind = -1;
		for (int i = 0; i < _charityCardButtons.Count; i++) {
			if (_charityCardButtons [i].gameObject == sender) {
				ind = i;
				break;
			}
		}
		SelectedItemFromCharityCard = _charityRandoms[ind];
		_charityCardSelectingParent.SetActive (false);
	}
	#endregion

	#region Select whether to build card
	[SerializeField] GameObject _buildCardSelectingParent;
	[SerializeField] GameObject _buildCardSelectedFalse;
	[SerializeField] GameObject _buildCardSelectedTrue;
	[SerializeField] List<GameObject> _cardToBuildSelectionParent;
	public bool SelectingBuildCardDone { get; private set; }
	public bool SelectedBuildCard { get; private set; }
	public void ShowBuildCard(int cardInd) {
		ExecuteInMainThread (() => {
			SelectingBuildCardDone = false;
			_buildCardSelectingParent.SetActive (true);
			for (int i = 0; i < _cardToBuildSelectionParent.Count; i++)
				_cardToBuildSelectionParent [i].SetActive (i == cardInd);
		});
	}
	public void OnBuildCardSelected(GameObject sender) {
		SelectingBuildCardDone = true;
		_buildCardSelectingParent.SetActive (false);
		if (_buildCardSelectedTrue == sender)
			SelectedBuildCard = true;
		else
			SelectedBuildCard = false;
	}
	#endregion

	#region Selecting resource for card ind
	[SerializeField] GameObject _selectingResourceForCardBuyingParent;
	[SerializeField] List<GameObject> _selectedResourcesForCardBuildingButtons;
	public Resource SelectedResourceForCardBuilding { get; private set; }
	public void ShowSelectResourceForCard(PlayerModel model, List<Resource> alreadySelectedResources) {
		ExecuteInMainThread (() => {
			SelectedResourceForCardBuilding = Resource.None;
			_selectingResourceForCardBuyingParent.SetActive (true);
			Dictionary<Resource, int> spentResourcesDict = new Dictionary<Resource, int> ();
			spentResourcesDict.Add (Resource.Forest, 0);
			spentResourcesDict.Add (Resource.Clay, 0);
			spentResourcesDict.Add (Resource.Stone, 0);
			spentResourcesDict.Add (Resource.Gold, 0);
			foreach (var res in alreadySelectedResources)
				spentResourcesDict [res]++;
			for (int ind = 0; ind < _selectedResourcesForCardBuildingButtons.Count; ind++) {
				Resource res = (Resource)(ind + 1);
				bool hasResource = model.GetResourceCount (res) > spentResourcesDict [res];
				_selectedResourcesForCardBuildingButtons [ind].SetActive (hasResource);
			}
		});
	}
	public void OnResourceForBuyingCardSelected(GameObject sender) {
		int ind = _selectedResourcesForCardBuildingButtons.IndexOf (sender);
		SelectedResourceForCardBuilding = (Resource)(ind+1);
		_selectingResourceForCardBuyingParent.SetActive (false);
	}
	#endregion

	#region Selecting whether to build house
	[SerializeField] GameObject _buildingHouseSelectingParent;
	[SerializeField] GameObject _selectedToBuildHouse;
	[SerializeField] List<GameObject> _houseToBuild;
	public bool SelectingBuildHouseDone { get; private set; }
	public bool SelectedToBuildHouse { get; private set; }
	public void ShowSelectBuildingHouse(int houseInd) {
		ExecuteInMainThread (() => {
			SelectingBuildHouseDone = false;
			_buildingHouseSelectingParent.SetActive (true);
			for (int i = 0; i < _houseToBuild.Count; i++)
				_houseToBuild [i].SetActive (i == houseInd);
		});
	}
	public void OnBuildHouseSelected(GameObject sender) {
		if (sender == _selectedToBuildHouse)
			SelectedToBuildHouse = true;
		else
			SelectedToBuildHouse = false;
		SelectingBuildHouseDone = true;
		_buildingHouseSelectingParent.SetActive (false);
	}
	#endregion

	#region Get resource for house building
	[SerializeField] GameObject _gettingResourceForHouseBuildingParent;
	[SerializeField] List<GameObject> _selectedResourceForHouseBuildingButtons;
	[SerializeField] List<Image> _usedResources;
	public Resource SelectedResourceForHouseBuilding { get; private set; }
	public void ShowSelectResourceForBuildingHouse(HouseToBuild house, List<Resource> options, List<Resource> spentResources) {
		ExecuteInMainThread (() => {
			SelectedResourceForHouseBuilding = Resource.None;
			_gettingResourceForHouseBuildingParent.SetActive (true);
			for (int ind = 0; ind < _selectedResourceForHouseBuildingButtons.Count; ind++) {
				Resource res = (Resource)(ind + 1);
				_selectedResourceForHouseBuildingButtons [ind].SetActive (options.Contains (res));
			}
			for (int ind = 0; ind < _usedResources.Count; ind++) {
				if (ind >= spentResources.Count)
					_usedResources [ind].gameObject.SetActive (false);
				else {
					_usedResources [ind].gameObject.SetActive (true);
					_usedResources [ind].sprite = _resourceSprites [(int)spentResources [ind]];
				}
			}
		});
	}
	public void OnResourceForHouseBuildingSelected(GameObject sender) {
		SelectedResourceForHouseBuilding = (Resource)(_selectedResourceForHouseBuildingButtons.IndexOf(sender)+1);
		_gettingResourceForHouseBuildingParent.SetActive (false);
	}
	#endregion

	#region Selecting whether to leave hungry
	[SerializeField] GameObject _leavingHungryParent;
	[SerializeField] GameObject _selectedToLeaveHungry;
	[SerializeField] Text _eatenResourcesCount;
	public bool SelectingLeavingHungryDone { get; private set; }
	public bool SelecedLeaveHungry { get; private set; }
	public void ShowSelectingLeavingHungry(int eatenResources) {
		ExecuteInMainThread (() => {
			SelectingLeavingHungryDone = false;
			_leavingHungryParent.SetActive (true);
			_eatenResourcesCount.text = eatenResources.ToString ();
		});
	}
	public void OnLeaveHungrySelected(GameObject sender) {
		SelectingLeavingHungryDone = true;
		if (sender == _selectedToLeaveHungry)
			SelecedLeaveHungry = true;
		else
			SelecedLeaveHungry = false;
		_leavingHungryParent.SetActive (false);
	}
	#endregion
}
