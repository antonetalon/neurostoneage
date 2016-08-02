using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game {

	public List<PlayerModel> Players;
	private List<HouseToBuild> _houseHeap1;
	private List<HouseToBuild> _houseHeap2;
	private List<HouseToBuild> _houseHeap3;
	private List<HouseToBuild> _houseHeap4;
	public int HousesHeapCount1 { get { return _houseHeap1.Count; } }
	public int HousesHeapCount2 { get { return _houseHeap2.Count; } }
	public int HousesHeapCount3 { get { return _houseHeap3.Count; } }
	public int HousesHeapCount4 { get { return _houseHeap4.Count; } }
	public HouseToBuild AvailableHouse1 { get { return HousesHeapCount1 > 0 ? _houseHeap1 [0] : null; } }
	public HouseToBuild AvailableHouse2 { get { return HousesHeapCount2 > 0 ? _houseHeap2 [0] : null; } }
	public HouseToBuild AvailableHouse3 { get { return HousesHeapCount3 > 0 ? _houseHeap3 [0] : null; } }
	public HouseToBuild AvailableHouse4 { get { return HousesHeapCount4 > 0 ? _houseHeap4 [0] : null; } }
	public CardToBuild AvailableCardFor1Resource;
	public CardToBuild AvailableCardFor2Resource;
	public CardToBuild AvailableCardFor3Resource;
	public CardToBuild AvailableCardFor4Resource;
	private List<CardToBuild> _cardsInHeap;
	public int CardsHeapCount { get { return _cardsInHeap.Count; } }

	public int AvailableInstrumentsPlaces;
	public int AvailableHousingPlaces;
	public int AvailableFieldPlaces;
	public int AvailableForestPlaces;
	public int AvailableClayPlaces;
	public int AvailableStonePlaces;
	public int AvailableGoldPlaces;
	public int AvaialbleCard1Places;
	public int AvaialbleCard2Places;
	public int AvaialbleCard3Places;
	public int AvaialbleCard4Places;
	public int AvaialbleBuilding1Places;
	public int AvaialbleBuilding2Places;
	public int AvaialbleBuilding3Places;
	public int AvaialbleBuilding4Places;

	public int TurnInd;

	public void NewGame() {
		TurnInd = 0;
		// Init buildings.
		List<HouseToBuild> allHouses = new List<HouseToBuild>();
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Forest, Resource.Forest, Resource.Clay }));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Forest, Resource.Clay, Resource.Gold }));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Clay, Resource.Clay, Resource.Stone }));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Clay, Resource.Stone, Resource.Stone}));
		allHouses.Add (new HouseToBuild (5, 1));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Forest, Resource.Clay, Resource.Clay}));
		allHouses.Add (new HouseToBuild (4, 1));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Forest, Resource.Clay, Resource.Stone}));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Forest, Resource.Stone, Resource.Stone}));
		allHouses.Add (new HouseToBuild (5, 3));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Forest, Resource.Stone, Resource.Gold}));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Stone, Resource.Stone, Resource.Gold}));
		allHouses.Add (new HouseToBuild (4, 3));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Clay, Resource.Clay, Resource.Gold}));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Forest, Resource.Forest, Resource.Stone}));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Forest, Resource.Clay, Resource.Stone}));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Forest, Resource.Clay, Resource.Gold}));
		allHouses.Add (new HouseToBuild (5, 2));
		allHouses.Add (new HouseToBuild (4, 2));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Clay, Resource.Stone, Resource.Gold}));
		allHouses.Add (new HouseToBuild (5, 4));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Forest, Resource.Forest, Resource.Gold}));
		allHouses.Add (new HouseToBuild (4, 4));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Clay, Resource.Stone, Resource.Gold}));
		allHouses.Add (new HouseToBuild (1, 7, 1));
		allHouses.Add (new HouseToBuild (1, 7, 1));
		allHouses.Add (new HouseToBuild (1, 7, 1));
		allHouses.Add (new HouseToBuild (new List<Resource> () { Resource.Forest, Resource.Stone, Resource.Gold }));
		Utils.Shuffle<HouseToBuild> (allHouses);
		for (int i = 0; i < allHouses.Count; i++) {
			int heapInd = i % 4;
			switch (heapInd) {
			case 0: _houseHeap1.Add (allHouses [i]); break;
			case 1: _houseHeap2.Add (allHouses [i]); break;
			case 2: _houseHeap3.Add (allHouses [i]); break;
			case 3: _houseHeap4.Add (allHouses [i]); break;
			}
		}
		// Cards.
		_cardsInHeap = new List<CardToBuild> ();
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.RandomForEveryone, 0, BottomCardFeature.InstrumentsMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.InstrumentsOnce, 4, BottomCardFeature.InstrumentsMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceConstFood, 7, BottomCardFeature.Science, Science.Pot));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceConstFood, 4, BottomCardFeature.HouseMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.OneCardMore, 0, BottomCardFeature.Science, Science.Book));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceConstClay, 1, BottomCardFeature.HumanMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.RandomForEveryone, 0, BottomCardFeature.Science, Science.Car));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceConstStone, 2, BottomCardFeature.Science, Science.Car));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.RandomForEveryone, 0, BottomCardFeature.Science, Science.Book));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceConstStone, 1, BottomCardFeature.HumanMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceConstFood, 5, BottomCardFeature.Science, Science.Grass));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.RandomForEveryone, 0, BottomCardFeature.Science, Science.Pot));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceRandomGold, 2, BottomCardFeature.Science, Science.Statue));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.InstrumentsForever, 1, BottomCardFeature.Science, Science.Statue));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.RandomForEveryone, 0, BottomCardFeature.HouseMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.InstrumentsOnce, 3, BottomCardFeature.InstrumentsMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.InstrumentsOnce, 2, BottomCardFeature.InstrumentsMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.RandomForEveryone, 0, BottomCardFeature.InstrumentsMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceConstFood, 1, BottomCardFeature.Science, Science.Loom));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceConstGold, 1, BottomCardFeature.HumanMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.RandomForEveryone, 0, BottomCardFeature.FieldMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.Score, 3, BottomCardFeature.Science, Science.Music));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.Field, 1, BottomCardFeature.Science, Science.Clock));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceRandomForest, 2, BottomCardFeature.HumanMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.RandomForEveryone, 0, BottomCardFeature.FieldMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceConstStone, 1, BottomCardFeature.FieldMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.Field, 1, BottomCardFeature.FieldMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.Score, 3, BottomCardFeature.Science, Science.Music));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceRandomStone, 2, BottomCardFeature.HumanMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceConstFood, 3, BottomCardFeature.FieldMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceAny, 2, BottomCardFeature.Science, Science.Grass));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceConstFood, 3, BottomCardFeature.Science, Science.Loom));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.RandomForEveryone, 0, BottomCardFeature.Science, Science.Clock));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.RandomForEveryone, 0, BottomCardFeature.HouseMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.Score, 3, BottomCardFeature.HouseMultiplier, 3));
		_cardsInHeap.Add (new CardToBuild(TopCardFeature.ResourceConstFood, 2, BottomCardFeature.HouseMultiplier, 2));
		Utils.Shuffle<CardToBuild> (_cardsInHeap);
	}
	public void NewTurn() {
		TurnInd++;
		AvailableInstrumentsPlaces = 1;
		AvailableHousingPlaces = 2;
		AvailableFieldPlaces = 1;
		AvailableForestPlaces = 7;
		AvailableClayPlaces = 7;
		AvailableStonePlaces = 7;
		AvailableGoldPlaces = 7;
		AvaialbleCard1Places = 1;
		AvaialbleCard2Places = 1;
		AvaialbleCard3Places = 1;
		AvaialbleCard4Places = 1;
		AvaialbleBuilding1Places = 1;
		AvaialbleBuilding2Places = 1;
		AvaialbleBuilding3Places = 1;
		AvaialbleBuilding4Places = 1;
		if (AvailableCardFor1Resource == null) {
			if (AvailableCardFor2Resource != null) {
				AvailableCardFor1Resource = AvailableCardFor2Resource;
				AvailableCardFor2Resource = null;
			} else if (AvailableCardFor3Resource != null) {
				AvailableCardFor1Resource = AvailableCardFor3Resource;
				AvailableCardFor3Resource = null;
			} else if (AvailableCardFor4Resource != null) {
				AvailableCardFor1Resource = AvailableCardFor4Resource;
				AvailableCardFor4Resource = null;
			} else {
				AvailableCardFor1Resource = _cardsInHeap [0];
				_cardsInHeap.RemoveAt (0);
			}				
		}
		if (AvailableCardFor2Resource == null) {
			if (AvailableCardFor3Resource != null) {
				AvailableCardFor2Resource = AvailableCardFor3Resource;
				AvailableCardFor3Resource = null;
			} else if (AvailableCardFor4Resource != null) {
				AvailableCardFor2Resource = AvailableCardFor4Resource;
				AvailableCardFor4Resource = null;
			} else {
				AvailableCardFor2Resource = _cardsInHeap [0];
				_cardsInHeap.RemoveAt (0);
			}				
		}
		if (AvailableCardFor3Resource == null) {
			if (AvailableCardFor4Resource != null) {
				AvailableCardFor3Resource = AvailableCardFor4Resource;
				AvailableCardFor4Resource = null;
			} else {
				AvailableCardFor3Resource = _cardsInHeap [0];
				_cardsInHeap.RemoveAt (0);
			}				
		}
		if (AvailableCardFor4Resource == null) {
			AvailableCardFor4Resource = _cardsInHeap [0];
			_cardsInHeap.RemoveAt (0);
		}
	}
}
