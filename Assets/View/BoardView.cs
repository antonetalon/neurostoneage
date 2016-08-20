using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BoardView : MonoBehaviour {
	[SerializeField] List<Image> _foodMiners;
	[SerializeField] List<Image> _forestMiners;
	[SerializeField] List<Image> _clayMiners;
	[SerializeField] List<Image> _stoneMiners;
	[SerializeField] List<Image> _goldMiners;
	[SerializeField] Image _instrumentMiner;
	[SerializeField] Image _fieldMiner;
	[SerializeField] List<Image> _housingMiners;
	[SerializeField] List<Image> _houseMiners;
	[SerializeField] List<Text> _houseHeapCounts;
	[SerializeField] List<Image> _cardMiners;
	[SerializeField] Text _cardHeapCount;
	[SerializeField] List<GameObject> _firstPlayerMarks;
	[SerializeField] Text _turnCount;
	[SerializeField] List<Sprite> _playerSprites;

	[SerializeField] List<Image> _availableCards;
	[SerializeField] List<Image> _availableHouses;
	[SerializeField] List<Sprite> _cardSpriteByInd;
	[SerializeField] List<Sprite> _houseSpriteByInd;

	int _foodMinersCount;
	int _forestMinersCount;
	int _clayMinersCount;
	int _stoneMinersCount;
	int _goldMinersCount;
	int _housingMinersCount;
	int _instrumentsMinersCount;
	int _fieldMinersCount;
	int _house1MinersCount;
	int _house2MinersCount;
	int _house3MinersCount;
	int _house4MinersCount;
	int _card1MinersCount;
	int _card2MinersCount;
	int _card3MinersCount;
	int _card4MinersCount;

	public void UpdateView(Game game) {
		_foodMinersCount = 0;
		_forestMinersCount = 0;
		_clayMinersCount = 0;
		_stoneMinersCount = 0;
		_goldMinersCount = 0;
		_housingMinersCount = 0;
		_instrumentsMinersCount = 0;
		_fieldMinersCount = 0;
		_house1MinersCount = 0;
		_house2MinersCount = 0;
		_house3MinersCount = 0;
		_house4MinersCount = 0;
		_card1MinersCount = 0;
		_card2MinersCount = 0;
		_card3MinersCount = 0;
		_card4MinersCount = 0;
		for (int i=0;i<game.PlayerModels.Count;i++)
			ShowPlayer (game.PlayerModels[i]);
		for (int i = 0; i < _foodMiners.Count; i++)
			_foodMiners [i].gameObject.SetActive (i < _foodMinersCount);
		for (int i = 0; i < _forestMiners.Count; i++)
			_forestMiners [i].gameObject.SetActive (i < _forestMinersCount);
		for (int i = 0; i < _clayMiners.Count; i++)
			_clayMiners [i].gameObject.SetActive (i < _clayMinersCount);
		for (int i = 0; i < _stoneMiners.Count; i++)
			_stoneMiners [i].gameObject.SetActive (i < _stoneMinersCount);
		for (int i = 0; i < _goldMiners.Count; i++)
			_goldMiners [i].gameObject.SetActive (i < _goldMinersCount);
		for (int i = 0; i < _housingMiners.Count; i++)
			_housingMiners [i].gameObject.SetActive (i < _housingMinersCount);
		_instrumentMiner.gameObject.SetActive (0 < _instrumentsMinersCount);
		_fieldMiner.gameObject.SetActive (0 < _fieldMinersCount);
		_instrumentMiner.gameObject.SetActive (0 < _instrumentsMinersCount);
		_houseMiners[0].gameObject.SetActive (0 < _house1MinersCount);
		_houseMiners[1].gameObject.SetActive (0 < _house2MinersCount);
		_houseMiners[2].gameObject.SetActive (0 < _house3MinersCount);
		_houseMiners[3].gameObject.SetActive (0 < _house4MinersCount);
		_cardMiners[0].gameObject.SetActive (0 < _card1MinersCount);
		_cardMiners[1].gameObject.SetActive (0 < _card2MinersCount);
		_cardMiners[2].gameObject.SetActive (0 < _card3MinersCount);
		_cardMiners[3].gameObject.SetActive (0 < _card4MinersCount);

		_houseHeapCounts [0].text = game.HousesHeapCount1.ToString ();
		_houseHeapCounts [1].text = game.HousesHeapCount2.ToString ();
		_houseHeapCounts [2].text = game.HousesHeapCount3.ToString ();
		_houseHeapCounts [3].text = game.HousesHeapCount4.ToString ();
		_cardHeapCount.text = game.CardsHeapCount.ToString ();
		for (int i = 0; i < _firstPlayerMarks.Count; i++)
			_firstPlayerMarks [i].SetActive (i==game.FirstPlayerInd);
		_turnCount.text = (game.TurnInd+1).ToString ();

		if (game.AvailableCardFor1Resource != null) {
			_availableCards [0].sprite = _cardSpriteByInd [game.AvailableCardFor1Resource.Ind];
			_availableCards [0].gameObject.SetActive (true);
		} else 
			_availableCards [0].gameObject.SetActive (false);
		if (game.AvailableCardFor2Resource != null) {
			_availableCards [1].sprite = _cardSpriteByInd [game.AvailableCardFor2Resource.Ind];
			_availableCards [1].gameObject.SetActive (true);
		} else 
			_availableCards [1].gameObject.SetActive (false);
		if (game.AvailableCardFor3Resource != null) {
			_availableCards [2].sprite = _cardSpriteByInd [game.AvailableCardFor3Resource.Ind];
			_availableCards [2].gameObject.SetActive (true);
		} else 
			_availableCards [2].gameObject.SetActive (false);
		if (game.AvailableCardFor4Resource != null) {
			_availableCards [3].sprite = _cardSpriteByInd [game.AvailableCardFor4Resource.Ind];
			_availableCards [3].gameObject.SetActive (true);
		} else 
			_availableCards [3].gameObject.SetActive (false);

		if (game.AvailableHouse1 != null) {
			_availableHouses [0].sprite = _houseSpriteByInd [game.AvailableHouse1.Ind];
			_availableHouses [0].gameObject.SetActive (true);
		} else 
			_availableHouses [0].gameObject.SetActive (false);
		if (game.AvailableHouse2 != null) {
			_availableHouses [1].sprite = _houseSpriteByInd [game.AvailableHouse2.Ind];
			_availableHouses [1].gameObject.SetActive (true);
		} else 
			_availableHouses [1].gameObject.SetActive (false);
		if (game.AvailableHouse3 != null) {
			_availableHouses [2].sprite = _houseSpriteByInd [game.AvailableHouse3.Ind];
			_availableHouses [2].gameObject.SetActive (true);
		} else 
			_availableHouses [2].gameObject.SetActive (false);
		if (game.AvailableHouse4 != null) {
			_availableHouses [3].sprite = _houseSpriteByInd [game.AvailableHouse4.Ind];
			_availableHouses [3].gameObject.SetActive (true);
		} else 
			_availableHouses [3].gameObject.SetActive (false);

	}
	private void ShowPlayer(PlayerModel player) {
		ShowPlayerOnResource (player, player.SpentOnFood, _foodMiners, ref _foodMinersCount);
		ShowPlayerOnResource (player, player.SpentOnForest, _forestMiners, ref _forestMinersCount);
		ShowPlayerOnResource (player, player.SpentOnClay, _clayMiners, ref _clayMinersCount);
		ShowPlayerOnResource (player, player.SpentOnStone, _stoneMiners, ref _stoneMinersCount);
		ShowPlayerOnResource (player, player.SpentOnGold, _goldMiners, ref _goldMinersCount);
		if (player.SpentOnInstruments > 0) {
			_instrumentMiner.gameObject.SetActive (true);
			_instrumentsMinersCount = 1;
			ShowPlayer (_instrumentMiner, player);
		}
		if (player.SpentOnFields > 0) {
			_fieldMiner.gameObject.SetActive (true);
			_fieldMinersCount = 1;
			ShowPlayer (_fieldMiner, player);
		}
		if (player.SpentOnHousing > 0) {
			_housingMiners[0].gameObject.SetActive (true);
			_housingMiners[1].gameObject.SetActive (true);
			_housingMinersCount = 2;
			ShowPlayer (_housingMiners[0], player);
			ShowPlayer (_housingMiners[1], player);
		}

		if (player.SpentOnBuilding1 > 0) {
			ShowPlayer (_houseMiners [0], player);
			_houseMiners [0].gameObject.SetActive (true);
			_house1MinersCount = 1;
		}
		if (player.SpentOnBuilding2 > 0) {
			ShowPlayer (_houseMiners [1], player);
			_houseMiners [1].gameObject.SetActive (true);
			_house2MinersCount = 1;
		}
		if (player.SpentOnBuilding3 > 0) {
			ShowPlayer (_houseMiners [2], player);
			_houseMiners [2].gameObject.SetActive (true);
			_house3MinersCount = 1;
		}
		if (player.SpentOnBuilding4 > 0) {
			ShowPlayer (_houseMiners [3], player);
			_houseMiners [3].gameObject.SetActive (true);
			_house4MinersCount = 1;
		}

		if (player.SpentOnCard1 > 0) {
			ShowPlayer (_cardMiners [0], player);
			_cardMiners [0].gameObject.SetActive (true);
			_card1MinersCount = 1;
		}
		if (player.SpentOnCard2 > 0) {
			ShowPlayer (_cardMiners [1], player);
			_cardMiners [1].gameObject.SetActive (true);
			_card2MinersCount = 1;
		}
		if (player.SpentOnCard3 > 0) {
			ShowPlayer (_cardMiners [2], player);
			_cardMiners [2].gameObject.SetActive (true);
			_card3MinersCount = 1;
		}
		if (player.SpentOnCard4 > 0) {
			ShowPlayer (_cardMiners [3], player);
			_cardMiners [3].gameObject.SetActive (true);
			_card4MinersCount = 1;
		}
	}
	private void ShowPlayerOnResource(PlayerModel player, int count, List<Image> views, ref int usedCounts) {
		for (int i = 0; i < count; i++) {
			ShowPlayer (views[usedCounts], player);
			usedCounts++;
		}
	}
	private void ShowPlayer(Image pic, PlayerModel player) {
		pic.sprite = _playerSprites [(int)player.CurrColor];
	}
	private void ShowPlayer(Image pic, PlayerModel.Color color) {
		pic.sprite = _playerSprites [(int)color];
	}
}
