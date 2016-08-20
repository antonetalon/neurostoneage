using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public int UnspentHumanCount {
		get {
			return HumansCount - SpentOnHousing - SpentOnFields - SpentOnInstruments - SpentOnFood - SpentOnForest
			- SpentOnClay - SpentOnStone - SpentOnGold - SpentOnBuilding1 - SpentOnBuilding2 - SpentOnBuilding3
			- SpentOnBuilding4 - SpentOnCard1 - SpentOnCard2 - SpentOnCard3 - SpentOnCard4;
		}
	}

	public int FieldsCount { get; private set; }
	public int InstrumentsCountSlot1 { get; private set; }
	public int InstrumentsCountSlot2 { get; private set; }
	public int InstrumentsCountSlot3 { get; private set; }
	public bool InstrumentsSlot1Used { get; private set; }
	public bool InstrumentsSlot2Used { get; private set; }
	public bool InstrumentsSlot3Used { get; private set; }

	public int Food { get; private set; }
	public int Forest { get; private set; }
	public int Clay { get; private set; }
	public int Stone { get; private set; }
	public int Gold { get; private set; }
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
		return _sciencesCount [sci] >= row;
	}
	public int GetScienceScore(int row) {
		int sum = 0;
		foreach (var count in _sciencesCount.Values) {
			if (count <= row + 1)
				sum++;
		}
		return sum * sum;
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
		CurrColor = color;
		HumansCount = 5;
		FieldsCount = 0;
		InstrumentsCountSlot1 =  2; // debug.
		InstrumentsCountSlot2 = 1; // debug.
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
	}

	#region Selecting where to go - turn phase 1.
	public void GoToHousing() {
		SpentOnHousing = 2;
	}
	public void GoToFields() {
		SpentOnFields = 1;
	}
	public void GoToInstruments() {
		SpentOnInstruments = 1;
	}
	public void GoToFood(int count) {
		SpentOnFood+=count;
	}
	public void GoToForest(int count) {
		SpentOnForest+=count;
	}
	public void GoToClay(int count) {
		SpentOnClay+=count;
	}
	public void GoToStone(int count) {
		SpentOnStone+=count;
	}
	public void GoToGold(int count) {
		SpentOnGold+=count;
	}
	public void GoToBuilding1() {
		SpentOnBuilding1++;
	}
	public void GoToBuilding2() {
		SpentOnBuilding2++;
	}
	public void GoToBuilding3() {
		SpentOnBuilding3++;
	}
	public void GoToBuilding4() {
		SpentOnBuilding4++;
	}
	public void GoToCard1() {
		SpentOnCard1++;
	}
	public void GoToCard2() {
		SpentOnCard2++;
	}
	public void GoToCard3() {
		SpentOnCard3++;
	}
	public void GoToCard4() {
		SpentOnCard4++;
	}
	#endregion

	#region Applying where went - turn phase 2.
	public void ApplyGoToHousing() {
		SpentOnHousing = 0;
		HumansCount++;
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
	public void ApplyGoToFood(int pointsSum) {
		if (SpentOnFood == 0)
			return;
		SpentOnFood = 0;
		int inc = pointsSum / Config.PointsPerFood;
		Food += inc;
	}
	public void ApplyGoToForest(int pointsSum) {
		if (SpentOnForest == 0)
			return;
		SpentOnForest = 0;
		int inc = pointsSum / Config.PointsPerForest;
		Forest += inc;
	}
	public void ApplyGoToClay(int pointsSum) {
		if (SpentOnClay == 0)
			return;
		SpentOnClay = 0;
		int inc = pointsSum / Config.PointsPerClay;
		Clay += inc;
	}
	public void ApplyGoToStone(int pointsSum) {
		if (SpentOnStone == 0)
			return;
		SpentOnStone = 0;
		int inc = pointsSum / Config.PointsPerStone;
		Stone += inc;
	}
	public void ApplyGoToGold(int pointsSum) {
		if (SpentOnGold == 0)
			return;
		SpentOnGold = 0;
		int inc = pointsSum / Config.PointsPerGold;
		Gold += inc;
	}
	public void ApplyGoToBuilding1(bool build, List<Resource> spentResources) {
		if (SpentOnBuilding1 == 0)
			return;
		SpentOnBuilding1 = 0;
		ApplyGoToBuilding (build, spentResources);
	}
	public void ApplyGoToBuilding2(bool build, List<Resource> spentResources) {
		if (SpentOnBuilding2 == 0)
			return;
		SpentOnBuilding2 = 0;
		ApplyGoToBuilding (build, spentResources);
	}
	public void ApplyGoToBuilding3(bool build, List<Resource> spentResources) {
		if (SpentOnBuilding3 == 0)
			return;
		SpentOnBuilding3 = 0;
		ApplyGoToBuilding (build, spentResources);
	}
	public void ApplyGoToBuilding4(bool build, List<Resource> spentResources) {
		if (SpentOnBuilding4 == 0)
			return;
		SpentOnBuilding4 = 0;
		ApplyGoToBuilding (build, spentResources);
	}
	private void ApplyGoToBuilding(bool build, List<Resource> spentResources) {
		if (!build)
			return;
		BuiltHouse house = new BuiltHouse (spentResources);
		_houses.Add (house);
		SubtractResources (spentResources);
		Score += HouseMultiplier;
		Score += house.Score;
	}
	public void ApplyGoToCard1(bool build, List<Resource> spentResources, CardToBuild card) {
		if (SpentOnCard1 == 0)
			return;
		SpentOnCard1 = 0;
		ApplyGoToCard (build, spentResources, card);
	}
	public void ApplyGoToCard2(bool build, List<Resource> spentResources, CardToBuild card) {
		if (SpentOnCard2 == 0)
			return;
		SpentOnCard2 = 0;
		ApplyGoToCard (build, spentResources, card);
	}
	public void ApplyGoToCard3(bool build, List<Resource> spentResources, CardToBuild card) {
		if (SpentOnCard3 == 0)
			return;
		SpentOnCard3 = 0;
		ApplyGoToCard (build, spentResources, card);
	}
	public void ApplyGoToCard4(bool build, List<Resource> spentResources, CardToBuild card) {
		if (SpentOnCard4 == 0)
			return;
		SpentOnCard4 = 0;
		ApplyGoToCard (build, spentResources, card);
	}
	private void ApplyGoToCard(bool build, List<Resource> spentResources, CardToBuild card) {
		if (!build)
			return;
		SubtractResources (spentResources);
		AddCard (card, true);
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
		} else {
			if (!card.TopUsed)
				card.UseTop ();
		}
	}
	public void ApplyCardTopRandomToAllSelection(int selection) {
		switch (selection) {
			case 1: AddResource (Resource.Forest, 1); break;
			case 2: AddResource (Resource.Clay, 1); break;
			case 3: AddResource (Resource.Stone, 1); break;
			case 4: AddResource (Resource.Gold, 1); break;
			case 5: AddInstrument (); break;
			case 6: AddField(); break;
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
		int neededFood = HumansCount - FieldsCount;
		if (neededFood <= 0)
			return;
		if (neededFood <= Food) {
			Food -= neededFood;
			return;
		}
		if (neededFood > Food + eatenResources.Count || leaveHungry) {
			Food = 0;
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
		if (res!=Resource.Food)
			Score += delta;
		switch (res) {
		case Resource.Food: Food+=delta; break;
		case Resource.Forest: Forest+=delta; break;
		case Resource.Clay: Clay+=delta; break;
		case Resource.Stone: Stone+=delta; break;
		case Resource.Gold:	Gold += delta; break;
		}
	}
	void AddInstrument() {
		if (InstrumentsCountSlot3 < InstrumentsCountSlot2)
			InstrumentsCountSlot3++;
		else if (InstrumentsCountSlot2 < InstrumentsCountSlot1)
			InstrumentsCountSlot2++;
		else
			InstrumentsCountSlot1++;
		Score += InstrumentsMultiplier;
	}
	private void AddField() {
		FieldsCount++;
		Score += FieldsMultiplier;
	}
	private void AddInstrumentsMultiplier() {
		InstrumentsMultiplier++;
		Score += InstrumentsCountSlot1 + InstrumentsCountSlot2 + InstrumentsCountSlot3;
	}
	private void AddHousesMultiplier() {
		HouseMultiplier++;
		Score += Houses.Count;
	}
	private void AddHumansMultiplier() {
		HumansMultiplier++;
		Score += HumansCount;
	}
	private void AddFieldsMultiplier() {
		FieldsMultiplier++;
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
		Score += incScore;
	}
}
