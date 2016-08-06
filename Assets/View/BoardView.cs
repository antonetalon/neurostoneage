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

	int _foodMinersCount;
	int _forestMinersCount;
	int _clayMinersCount;
	int _stoneMinersCount;
	int _goldMinersCount;

	public void UpdateView(Game game) {
		_foodMinersCount = 0;
		_forestMinersCount = 0;
		_clayMinersCount = 0;
		_stoneMinersCount = 0;
		_goldMinersCount = 0;
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
		_houseHeapCounts [0].text = game.HousesHeapCount1.ToString ();
		_houseHeapCounts [1].text = game.HousesHeapCount2.ToString ();
		_houseHeapCounts [2].text = game.HousesHeapCount3.ToString ();
		_houseHeapCounts [3].text = game.HousesHeapCount4.ToString ();
		_cardHeapCount.text = game.CardsHeapCount.ToString ();
		for (int i = 0; i < _firstPlayerMarks.Count; i++)
			_firstPlayerMarks [i].SetActive (i==game.FirstPlayerInd);
		_turnCount.text = (game.TurnInd+1).ToString ();
	}
	private void ShowPlayer(PlayerModel player) {
		ShowPlayerOnResource (player, player.SpentOnFood, _foodMiners, ref _foodMinersCount);
		ShowPlayerOnResource (player, player.SpentOnForest, _forestMiners, ref _forestMinersCount);
		ShowPlayerOnResource (player, player.SpentOnClay, _clayMiners, ref _clayMinersCount);
		ShowPlayerOnResource (player, player.SpentOnStone, _stoneMiners, ref _stoneMinersCount);
		ShowPlayerOnResource (player, player.SpentOnGold, _goldMiners, ref _goldMinersCount);
		ShowPlayer (_instrumentMiner, player.CurrColor);
		_instrumentMiner.gameObject.SetActive (player.SpentOnInstruments > 0);
		ShowPlayer (_fieldMiner, player.CurrColor);

		_fieldMiner.gameObject.SetActive (player.SpentOnFields > 0);
		ShowPlayer (_housingMiners[0], player.CurrColor);

		_housingMiners[0].gameObject.SetActive (player.SpentOnHousing > 0);
		ShowPlayer (_housingMiners[1], player.CurrColor);
		_housingMiners[1].gameObject.SetActive (player.SpentOnHousing > 0);

		ShowPlayer (_houseMiners[0], player.CurrColor);
		_houseMiners[0].gameObject.SetActive (player.SpentOnBuilding1 > 0);
		ShowPlayer (_houseMiners[1], player.CurrColor);
		_houseMiners[1].gameObject.SetActive (player.SpentOnBuilding2 > 0);
		ShowPlayer (_houseMiners[2], player.CurrColor);
		_houseMiners[2].gameObject.SetActive (player.SpentOnBuilding3 > 0);
		ShowPlayer (_houseMiners[3], player.CurrColor);
		_houseMiners[3].gameObject.SetActive (player.SpentOnBuilding4 > 0);

		ShowPlayer (_cardMiners[0], player.CurrColor);
		_cardMiners[0].gameObject.SetActive (player.SpentOnCard1 > 0);
		ShowPlayer (_cardMiners[1], player.CurrColor);
		_cardMiners[1].gameObject.SetActive (player.SpentOnCard2 > 0);
		ShowPlayer (_cardMiners[2], player.CurrColor);
		_cardMiners[2].gameObject.SetActive (player.SpentOnCard3 > 0);
		ShowPlayer (_cardMiners[3], player.CurrColor);
		_cardMiners[3].gameObject.SetActive (player.SpentOnCard4 > 0);
	}
	private void ShowPlayerOnResource(PlayerModel player, int count, List<Image> views, ref int usedCounts) {
		for (int i = 0; i < count; i++) {
			ShowPlayer (views[usedCounts], player.CurrColor);
			usedCounts++;
		}
	}
	private void ShowPlayer(Image pic, PlayerModel.Color color) {
		pic.sprite = _playerSprites [(int)color];
	}
}
