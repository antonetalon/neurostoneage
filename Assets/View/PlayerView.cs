using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour {
	
	[SerializeField] List<Image> _humans;
	[SerializeField] Text _humansMultiplier;
	[SerializeField] List<Text> _instruments;
	[SerializeField] Text _instrumentsMultiplier;
	[SerializeField] Image _once4Instruments;
	[SerializeField] Image _once3Instruments;
	[SerializeField] Image _once2Instruments;
	[SerializeField] Image _once2Resource;
	[SerializeField] Text _fields;
	[SerializeField] Text _fieldsMultiplier;
	[SerializeField] Text _houses;
	[SerializeField] Text _housesMultiplier;
	[SerializeField] Text _food;
	[SerializeField] Text _forest;
	[SerializeField] Text _clay;
	[SerializeField] Text _stone;
	[SerializeField] Text _gold;
	[SerializeField] List<Image> _scirow1;
	[SerializeField] Text _scirow1Score;
	[SerializeField] List<Image> _scirow2;
	[SerializeField] Text _scirow2Score;
	[SerializeField] Text _hungryTurns;
	[SerializeField] Text _score;

	public void UpdateView(PlayerModel player) {
		for (int i = 0; i < _humans.Count; i++)
			_humans [i].gameObject.SetActive (i<player.AvailableHumans);
		_humansMultiplier.text = player.HumansMultiplier.ToString ();
		_instruments [0].text = player.InstrumentsCountSlot1.ToString ();
		_instruments [1].text = player.InstrumentsCountSlot2.ToString ();
		_instruments [2].text = player.InstrumentsCountSlot3.ToString ();
		_instrumentsMultiplier.text = player.InstrumentsMultiplier.ToString ();
		_once4Instruments.gameObject.SetActive (player.Top4Instruments != null);
		_once3Instruments.gameObject.SetActive (player.Top3Instruments != null);
		_once2Instruments.gameObject.SetActive (player.Top2Instruments != null);
		_once2Resource.gameObject.SetActive (player.Top2Resources != null);
		_fields.text = player.FieldsCount.ToString ();
		_fieldsMultiplier.text = player.FieldsMultiplier.ToString ();
		_houses.text = player.Houses.Count.ToString ();
		_housesMultiplier.text = player.HouseMultiplier.ToString ();
		_food.text = player.Food.ToString ();
		_forest.text = player.Forest.ToString ();
		_clay.text = player.Clay.ToString ();
		_stone.text = player.Stone.ToString ();
		_gold.text = player.Gold.ToString ();
		_scirow1 [0].gameObject.SetActive (player.ScienceExists(Science.Book,0));
		_scirow1 [1].gameObject.SetActive (player.ScienceExists(Science.Car,0));
		_scirow1 [2].gameObject.SetActive (player.ScienceExists(Science.Grass,0));
		_scirow1 [3].gameObject.SetActive (player.ScienceExists(Science.Pot,0));
		_scirow1 [4].gameObject.SetActive (player.ScienceExists(Science.Statue,0));
		_scirow1 [5].gameObject.SetActive (player.ScienceExists(Science.Music,0));
		_scirow1 [6].gameObject.SetActive (player.ScienceExists(Science.Clock,0));
		_scirow1 [7].gameObject.SetActive (player.ScienceExists(Science.Loom,0));
		_scirow1Score.text = player.GetScienceScore (0).ToString ();
		_scirow2 [0].gameObject.SetActive (player.ScienceExists(Science.Book,1));
		_scirow2 [1].gameObject.SetActive (player.ScienceExists(Science.Car,1));
		_scirow2 [2].gameObject.SetActive (player.ScienceExists(Science.Grass,1));
		_scirow2 [3].gameObject.SetActive (player.ScienceExists(Science.Pot,1));
		_scirow2 [4].gameObject.SetActive (player.ScienceExists(Science.Statue,1));
		_scirow2 [5].gameObject.SetActive (player.ScienceExists(Science.Music,1));
		_scirow2Score.text = player.GetScienceScore (1).ToString ();
		_hungryTurns.text = player.HungryTurnsCount.ToString ();
		_score.text = player.Score.ToString ();
	}

}
