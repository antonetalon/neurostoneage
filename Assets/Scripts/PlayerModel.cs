using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

[Serializable]
public class PlayerModel {
	public enum Color
	{
		Blue,
		Red,
		Yellow,
		Green
	}
	public int HumansCount { get; private set; }

	public int SpentOnHousing { get; private set; }
	public int SpentOnFields { get; private set; }
	public int SpentOnInstruments { get; private set; }
	public int SpentOnFood { get; private set; }
	public int SpentOnForest { get; private set; }
	public int SpentOnClay { get; private set; }
	public int SpentOnStone { get; private set; }
	public int SpentOnGold { get; private set; }
	public int SpentOnBuilding1 { get; private set; }
	public int SpentOnBuilding2 { get; private set; }
	public int SpentOnBuilding3 { get; private set; }
	public int SpentOnBuilding4 { get; private set; }
	public int SpentOnCard1 { get; private set; }
	public int SpentOnCard2 { get; private set; }
	public int SpentOnCard3 { get; private set; }
	public int SpentOnCard4 { get; private set; }
	public int UnSpentFields { get; private set; }

	public int GetSpentOnCard(int cardInd) {
		switch (cardInd) {
			case 0: return SpentOnCard1;
			case 1: return SpentOnCard2;
			case 2: return SpentOnCard3;
			case 3: return SpentOnCard4;
			default: return -1;
		}
	}
	private void SetSpentOnCard(int cardInd, int val) {
		switch (cardInd) {
			case 0: SpentOnCard1 = val; break;
			case 1: SpentOnCard2 = val; break;
			case 2: SpentOnCard3 = val; break;
			case 3: SpentOnCard4 = val; break;
		}
	}
	public int GetSpentOnHouse(int houseInd) {
		switch (houseInd) {
			case 0: return SpentOnBuilding1;
			case 1: return SpentOnBuilding2;
			case 2: return SpentOnBuilding3;
			case 3: return SpentOnBuilding4;
			default: return -1;
		}
	}
	public void SetSpentOnHouse(int houseInd, int count) {
		switch (houseInd) {
			case 0:	SpentOnBuilding1 = count; break;
			case 1: SpentOnBuilding2 = count; break;
			case 2: SpentOnBuilding3 = count; break;
			case 3: SpentOnBuilding4 = count; break;
		}
	}

	public int GetSpentHumansCountFor(WhereToGo target) {
		switch (target) {
			default: return 0;
			case WhereToGo.Card1: return SpentOnCard1;
			case WhereToGo.Card2: return SpentOnCard2;
			case WhereToGo.Card3: return SpentOnCard3;
			case WhereToGo.Card4: return SpentOnCard4;
			case WhereToGo.Field: return SpentOnFields;
			case WhereToGo.House1: return SpentOnBuilding1;
			case WhereToGo.House2: return SpentOnBuilding2;
			case WhereToGo.House3: return SpentOnBuilding3;
			case WhereToGo.House4: return SpentOnBuilding4;
			case WhereToGo.Instrument: return SpentOnInstruments;
			case WhereToGo.Clay: return SpentOnClay;
			case WhereToGo.Forest: return SpentOnForest;
			case WhereToGo.Gold: return SpentOnGold;
			case WhereToGo.Stone: return SpentOnStone;
			case WhereToGo.Food: return SpentOnFood;
			case WhereToGo.Housing: return SpentOnHousing;
		}
	}

	public int AvailableHumans { get; private set; }

	public int FieldsCount { get; private set; }
	public int InstrumentsCountSlot1 { get; private set; }
	public int InstrumentsCountSlot2 { get; private set; }
	public int InstrumentsCountSlot3 { get; private set; }
	public bool InstrumentsSlot1Used { get; private set; }
	public bool InstrumentsSlot2Used { get; private set; }
	public bool InstrumentsSlot3Used { get; private set; }

	public int GetAvailableInstruments(int slotInd) {
		switch (slotInd) {
			default: return 0;
			case 0: return InstrumentsSlot1Used ? 0 : InstrumentsCountSlot1;
			case 1: return InstrumentsSlot2Used ? 0 : InstrumentsCountSlot2;
			case 2: return InstrumentsSlot3Used ? 0 : InstrumentsCountSlot3;
		}
	}
	public int GetAvailableOnceInstruments(int slotInd) {
		switch (slotInd) {
			default: return 0;
			case 0: return (Top4Instruments != null && !Top4Instruments.Card.TopUsed) ? 4 : 0;
			case 1: return (Top3Instruments != null && !Top3Instruments.Card.TopUsed) ? 3 : 0;
			case 2: return (Top2Instruments != null && !Top2Instruments.Card.TopUsed) ? 2 : 0;
		}
	}

	public int Food { get; private set; }
	public int Forest { get; private set; }
	public int Clay { get; private set; }
	public int Stone { get; private set; }
	public int Gold { get; private set; }
	public int GetResourceCount(Resource res) {
		switch (res) {
			default: return -1;
			case Resource.Food: return Food;
			case Resource.Forest: return Forest;
			case Resource.Clay: return Clay;
			case Resource.Stone: return Stone;
			case Resource.Gold: return Gold;
		}
	}

	public ReadonlyList<BuiltHouse> Houses { get; private set; }
	List<BuiltHouse> _houses;
	public ReadonlyList<BuiltCard> Cards { get; private set; }
	List<BuiltCard> _cards;

	public int InstrumentsMultiplier { get; private set; }
	public int HouseMultiplier { get; private set; }
	public int HumansMultiplier { get; private set; }
	public int FieldsMultiplier { get; private set; }

	public int HungryTurnsCount { get; private set; }
	public int Score { get; private set; }

	private Dictionary<Science, int> _sciencesCount;
	public bool ScienceExists(Science sci, int row) {
		return _sciencesCount [sci] > row;
	}
	public int GetScienceScore(int row) {
		int sum = GetSciencesCount (row);
		return sum * sum;
	}
	public int GetSciencesCount(int row) {
		int sum = 0;
		foreach (var count in _sciencesCount.Values) {
			if (count > row)
				sum++;
		}
		return sum;
	}

	public BuiltCard Top4Instruments { 
		get {
			foreach (BuiltCard card in _cards) {
				if (card.Card.TopFeature == TopCardFeature.InstrumentsOnce && card.Card.TopFeatureParam == 4 && !card.Card.TopUsed)
					return card;
			}
			return null;
		}
	}
	public BuiltCard Top3Instruments { 
		get {
			foreach (BuiltCard card in _cards) {
				if (card == null || card.Card == null)
					Debug.Log ("hi");
				if (card.Card.TopFeature == TopCardFeature.InstrumentsOnce && card.Card.TopFeatureParam == 3 && !card.Card.TopUsed)
					return card;
			}
			return null;
		}
	}
	public BuiltCard Top2Instruments { 
		get {
			foreach (BuiltCard card in _cards) {
				if (card.Card.TopFeature == TopCardFeature.InstrumentsOnce && card.Card.TopFeatureParam == 2 && !card.Card.TopUsed)
					return card;
			}
			return null;
		}
	}
	public BuiltCard Top2Resources { 
		get {
			foreach (BuiltCard card in _cards) {
				if (card.Card.TopFeature == TopCardFeature.ResourceAny && !card.Card.TopUsed)
					return card;
			}
			return null;
		}
	}

	public Color CurrColor { get; private set; }
	public PlayerModel(Color color) {
		Score = 0;
		if (GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Any || GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.HumanMultiplier)
			Score += 5;
		CurrColor = color;
		HumansCount = 5;
		FieldsCount = 0;
		InstrumentsCountSlot1 = 0;
		InstrumentsCountSlot2 = 0;
		InstrumentsCountSlot3 = 0;
		Food = 12;
		Forest = 0;
		Clay = 0;
		Stone = 0;
		Gold = 0;
		_houses = new List<BuiltHouse> ();
		Houses = new ReadonlyList<BuiltHouse> (_houses);
		_cards = new List<BuiltCard> ();
		Cards = new ReadonlyList<BuiltCard> (_cards);
		HungryTurnsCount = 0;
		InstrumentsMultiplier = 1;
		HouseMultiplier = 1;
		HumansMultiplier = 1;
		FieldsMultiplier = 1;
		_sciencesCount = new Dictionary<Science, int> ();
		_sciencesCount.Add (Science.Book, 0);
		_sciencesCount.Add (Science.Car, 0);
		_sciencesCount.Add (Science.Clock, 0);
		_sciencesCount.Add (Science.Grass, 0);
		_sciencesCount.Add (Science.Loom, 0);
		_sciencesCount.Add (Science.Music, 0);
		_sciencesCount.Add (Science.Pot, 0);
		_sciencesCount.Add (Science.Statue, 0);
	}
	public void NewTurn() {
		InstrumentsSlot1Used = false;
		InstrumentsSlot2Used = false;
		InstrumentsSlot3Used = false;
		SpentOnHousing = 0;
		SpentOnFields = 0;
		SpentOnInstruments = 0;
		SpentOnFood = 0;
		SpentOnForest = 0;
		SpentOnClay = 0;
		SpentOnStone = 0;
		SpentOnGold = 0;
		SpentOnBuilding1 = 0;
		SpentOnBuilding2 = 0;
		SpentOnBuilding3 = 0;
		SpentOnBuilding4 = 0;
		SpentOnCard1 = 0;
		SpentOnCard2 = 0;
		SpentOnCard3 = 0;
		SpentOnCard4 = 0;
		AvailableHumans = HumansCount;
		UnSpentFields = FieldsCount;
	}

	#region Selecting where to go - turn phase 1.
	public void GoToHousing() {
		SpentOnHousing = 2;
		AvailableHumans -= 2;
	}
	public void GoToFields() {
		SpentOnFields = 1;
		AvailableHumans -= 1;
	}
	public void GoToInstruments() {
		SpentOnInstruments = 1;
		AvailableHumans -= 1;
	}
	public void GoToFood(int count) {
		SpentOnFood+=count;
		AvailableHumans -= count;
	}
	public void GoToForest(int count) {
		SpentOnForest+=count;
		AvailableHumans -= count;
	}
	public void GoToClay(int count) {
		SpentOnClay+=count;
		AvailableHumans -= count;
	}
	public void GoToStone(int count) {
		SpentOnStone+=count;
		AvailableHumans -= count;
	}
	public void GoToGold(int count) {
		SpentOnGold+=count;
		AvailableHumans -= count;
	}
	public void GoToBuilding1() {
		SpentOnBuilding1++;
		AvailableHumans--;
	}
	public void GoToBuilding2() {
		SpentOnBuilding2++;
		AvailableHumans--;
	}
	public void GoToBuilding3() {
		SpentOnBuilding3++;
		AvailableHumans--;
	}
	public void GoToBuilding4() {
		SpentOnBuilding4++;
		AvailableHumans--;
	}
	public void GoToCard1() {
		SpentOnCard1++;
		AvailableHumans--;
	}
	public void GoToCard2() {
		SpentOnCard2++;
		AvailableHumans--;
	}
	public void GoToCard3() {
		SpentOnCard3++;
		AvailableHumans--;
	}
	public void GoToCard4() {
		SpentOnCard4++;
		AvailableHumans--;
	}
	#endregion

	#region Applying where went - turn phase 2.
	public void ApplyGoToHousing() {
		HumansCount++;
		SpentOnHousing=0;
		if (GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Any || GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.HumanMultiplier)
			Score += HumansMultiplier;
	}
	public void ApplyGoToFields() {
		if (SpentOnFields == 0)
			return;
		SpentOnFields = 0;
		AddField ();
	}
	public void ApplyGoToInstruments() {
		if (SpentOnInstruments == 0)
			return;
		SpentOnInstruments = 0;
		AddInstrument ();
	}
	public void OnResourceDicesRolled(WhereToGo target) {
		switch (target) {
			case WhereToGo.Food: SpentOnFood = 0; break;
			case WhereToGo.Forest: SpentOnForest = 0; break;
			case WhereToGo.Clay: SpentOnClay = 0; break;
			case WhereToGo.Stone: SpentOnStone = 0; break;
			case WhereToGo.Gold: SpentOnGold = 0; break;
		}
	}
	public void ApplyGoToBuilding(int buildingInd, List<Resource> spentResources) {
		if (GetSpentOnHouse(buildingInd) == 0)
			return;
		SetSpentOnHouse (buildingInd, 0); 
		if (spentResources!=null)
			ApplyGoToBuilding (spentResources);
	}
	private void ApplyGoToBuilding(List<Resource> spentResources) {
		BuiltHouse house = new BuiltHouse (spentResources);
		_houses.Add (house);
		SubtractResources (spentResources);
		if (GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Any || GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.HouseMultiplier)
			Score += HouseMultiplier;
		if (GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Any || GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Houses)
			Score += house.Score;
	}
	public void ApplyGoToCard(bool build, int cardInd, List<Resource> spentResources, CardToBuild card) {
		if (GetSpentOnCard(cardInd) == 0)
			return;
		SetSpentOnCard (cardInd, 0);
		ApplyGoToCard (build, spentResources, card);
	}
	private void ApplyGoToCard(bool build, List<Resource> spentResources, CardToBuild card) {
		if (!build)
			return;
		SubtractResources (spentResources);
		AddCard (card, true);
	}
	public void ApplyEndTurn() {
		AvailableHumans = 0;
		InstrumentsSlot1Used = true;
		InstrumentsSlot2Used = true;
		InstrumentsSlot3Used = true;
		UnSpentFields = 0;
	}
	public void ApplyCardTopOneCardMore(CardToBuild cardFromStash) {
		AddCard (cardFromStash, false);
	}
	void AddCard(CardToBuild card, bool applyTop) {
		BuiltCard builtCard = new BuiltCard(card);
		_cards.Add (builtCard);
		switch (card.BottomFeature) {
		case BottomCardFeature.InstrumentsMultiplier:
			for (int i = 0; i < card.BottomFeatureParam; i++)
				AddInstrumentsMultiplier ();
			break;
		case BottomCardFeature.HouseMultiplier:
			for (int i = 0; i < card.BottomFeatureParam; i++)
				AddHousesMultiplier ();
			break;
		case BottomCardFeature.HumanMultiplier:
			for (int i = 0; i < card.BottomFeatureParam; i++)
				AddHumansMultiplier ();
			break;
		case BottomCardFeature.FieldMultiplier:
			for (int i = 0; i < card.BottomFeatureParam; i++)
				AddFieldsMultiplier ();
			break;
		case BottomCardFeature.Science:
			AddScience ((Science)card.BottomFeatureParam);
			break;
		}
		if (applyTop) {
			if (card.TopUsed) {
				switch (card.TopFeature) {
				case TopCardFeature.Score:
					if (GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Any || GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.TopCardFeature)
						Score += card.TopFeatureParam;
					break;
				case TopCardFeature.ResourceConstFood:
					AddResource (Resource.Food, card.TopFeatureParam);
					break;
				case TopCardFeature.ResourceConstClay:
					AddResource (Resource.Clay, card.TopFeatureParam);
					break;
				case TopCardFeature.ResourceConstStone:
					AddResource (Resource.Stone, card.TopFeatureParam);
					break;
				case TopCardFeature.ResourceConstGold:
					AddResource (Resource.Gold, card.TopFeatureParam);
					break;
				case TopCardFeature.InstrumentsForever:
					AddInstrument ();
					break;
				case TopCardFeature.Field:
					AddField ();
					break;
				//case TopCardFeature.OneCardMore: break;
				//case TopCardFeature.InstrumentsOnce: break;
				//case TopCardFeature.ResourceAny: break;
				//case TopCardFeature.RandomForEveryone: break;
				//case TopCardFeature.ResourceRandomForest: break;
				//case TopCardFeature.ResourceRandomStone: break;
				//case TopCardFeature.ResourceRandomGold: break;
				}
			} 
			if (card.TopFeature == TopCardFeature.ResourceRandomForest)
				SpentOnForest = card.TopFeatureParam;
			if (card.TopFeature == TopCardFeature.ResourceRandomStone)
				SpentOnStone = card.TopFeatureParam;
			if (card.TopFeature == TopCardFeature.ResourceRandomGold)
				SpentOnGold = card.TopFeatureParam;
		} else {
			if (!card.TopUsed)
				card.UseTop ();
		}
	}
	public bool GetHasAnyResourceFromCard() {
		foreach (BuiltCard card in _cards) {
			if (card.Card.TopFeature == TopCardFeature.ResourceAny && !card.Card.TopUsed)
				return true;
		}
		return false;
	}
	public void ApplyAnyResourceFromTopCard() {
		Top2Resources.Card.UseTop ();
	}
	public void ApplyUseInstrumentSlot(int slotInd) {
		switch (slotInd) {
		case 0: InstrumentsSlot1Used = true; break;
		case 1: InstrumentsSlot2Used = true; break;
		case 2: InstrumentsSlot3Used = true; break;
		}
	}
	#endregion

	#region Feeding - turn phase 3.
	public void Feed(bool leaveHungry, List<Resource> eatenResources) {
		int eatenFields = Mathf.Min (HumansCount, FieldsCount);
		UnSpentFields -= eatenFields;
		int neededFood = HumansCount - FieldsCount;
		if (neededFood <= 0)
			return;
		if (neededFood <= Food) {
			Food -= neededFood;
			return;
		}
		if (neededFood > Food + eatenResources.Count || leaveHungry) {
			Food = 0;
			if (GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Any || GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Feeding)
				Score -= 10;
			HungryTurnsCount++;
			return;
		}
		Food = 0;
		SubtractResources (eatenResources);
	}
	#endregion

	void SubtractResources(List<Resource> spentResources) {
		foreach (var res in spentResources)
			AddResource (res, -1);
	}

	public void AddResource(Resource res, int delta) {
		if (res != Resource.Food) {
			if (GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Any || GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Resources)
				Score += delta;
		}
		switch (res) {
		case Resource.Food: Food+=delta; break;
		case Resource.Forest: Forest+=delta; break;
		case Resource.Clay: Clay+=delta; break;
		case Resource.Stone: Stone+=delta; break;
		case Resource.Gold:	Gold += delta; break;
		}
	}
	public void AddInstrument() {
		if (InstrumentsCountSlot3 < InstrumentsCountSlot2) {
			InstrumentsCountSlot3++;
			InstrumentsSlot3Used = true;
		} else if (InstrumentsCountSlot2 < InstrumentsCountSlot1) {
			InstrumentsCountSlot2++;
			InstrumentsSlot2Used = true;
		} else {
			InstrumentsCountSlot1++;
			InstrumentsSlot1Used = true;
		}
		if (GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Any || GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.InstrumentMultiplier)
			Score += InstrumentsMultiplier;
	}
	public void AddField() {
		FieldsCount++;
		if (GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Any || GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.FieldMultiplier)
			Score += FieldsMultiplier;
		UnSpentFields++;
	}
	private void AddInstrumentsMultiplier() {
		InstrumentsMultiplier++;
		if (GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Any || GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.InstrumentMultiplier)
			Score += InstrumentsCountSlot1 + InstrumentsCountSlot2 + InstrumentsCountSlot3;
	}
	private void AddHousesMultiplier() {
		HouseMultiplier++;
		if (GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Any || GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.HouseMultiplier)
			Score += Houses.Count;
	}
	private void AddHumansMultiplier() {
		HumansMultiplier++;
		if (GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Any || GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.HumanMultiplier)
			Score += HumansCount;
	}
	private void AddFieldsMultiplier() {
		FieldsMultiplier++;
		if (GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Any || GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.FieldMultiplier)
			Score += FieldsCount;
	}
	private void AddScience(Science science) {		
		_sciencesCount [science]++;
		int complectInd = _sciencesCount [science];
		int complectCount = 0;
		foreach (var currCount in _sciencesCount.Values) {
			if (complectInd <= currCount)
				complectCount++;
		}
		int incScore = complectCount * complectCount - (complectCount - 1) * (complectCount - 1);
		if (GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Any || GameTrainingController.UsedScoreSources == GameTrainingController.ScoreSources.Science)
			Score += incScore;
	}

	public override string ToString ()
	{
		StringBuilder sb = new StringBuilder ();
		foreach (var house in Houses)
			sb.Append (string.Format(" + {0}", house.Score));
		
		return string.Format ("PlayerModel: \n" +
			"HumansCount*Multiplier={0}*{1}={2}, \n" +
			"FieldsCount*Multiplier={3}*{4}={5}, \n" +
			"Instruments*Multiplier=({6}+{7}+{8})*{9}={10}, \n" +
			"Food={11}*0 = {12},\n" +
			"Forest+Clay+Stone+Gold={13}+{14}+{15}+{16},\n" +
			"Houses*Multiplier={17}*{18}{19}, \n" +
			"Cards={20}, \n" +
			"HungryTurnsCount={21}*10={22},\n" +
			"Score={23}\n",
			HumansCount, HumansMultiplier, HumansCount*HumansMultiplier,
			FieldsCount, FieldsMultiplier, FieldsCount*FieldsMultiplier,
			InstrumentsCountSlot1, InstrumentsCountSlot2, InstrumentsCountSlot3, InstrumentsMultiplier, (InstrumentsCountSlot1+InstrumentsCountSlot2+InstrumentsCountSlot3)*InstrumentsMultiplier,
			Food, 0,
			Forest, Clay, Stone, Gold,
			Houses.Count, HouseMultiplier, sb.ToString(),
			Cards.Count,
			HungryTurnsCount, -HungryTurnsCount*10,
			Score
		);
	}
}
