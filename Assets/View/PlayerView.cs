using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour {

	public void UpdateView(PlayerModel player) {
	}

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
}
