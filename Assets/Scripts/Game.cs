using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game {
	static System.Random rand = new System.Random();
	public static int RandomRange(int min, int max) { 
		if (max == min)
			return min;
		return rand.Next()%(max-min)+min;
	}
	public static float RandomValue { get { return (float)rand.NextDouble (); } }
	const bool Logging = false;
	public List<PlayerModel> PlayerModels;
	public List<Player> Players;
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

	public List<GameTrainingController> TrainingControllers;

	public CardToBuild GetAvailableCard(int cardInd) {
		switch (cardInd) {
			case 0: return AvailableCardFor1Resource;
			case 1: return AvailableCardFor2Resource;
			case 2: return AvailableCardFor3Resource;
			case 3: return AvailableCardFor4Resource;
			default: return null;
		}
	}
	public void	 RemoveAvailableCard(int cardInd) {
		switch (cardInd) {
			case 0: AvailableCardFor1Resource = null; break;
			case 1: AvailableCardFor2Resource = null; break;
			case 2: AvailableCardFor3Resource = null; break;
			case 3: AvailableCardFor4Resource = null; break;
		}
	}
	public HouseToBuild GetHouse(int ind) {
		switch (ind) {
			case 0: return AvailableHouse1;
			case 1: return AvailableHouse2;
			case 2: return AvailableHouse3;
			case 3: return AvailableHouse4;
			default: return null;
		}
	}
	private void RemoveAvailableHouse(int ind) {
		switch (ind) {
			case 0: _houseHeap1.RemoveAt(0); break;
			case 1: _houseHeap2.RemoveAt(0); break;
			case 2: _houseHeap3.RemoveAt(0); break;
			case 3: _houseHeap4.RemoveAt(0); break;
		}
	}
	private List<CardToBuild> _cardsInHeap;
	public int CardsHeapCount { get { return _cardsInHeap.Count; } }

	public int AvailableInstrumentsPlaces;
	public int AvailableHousingPlaces;
	public int AvailableFieldPlaces;
	public int AvailableForestPlaces;
	public int AvailableClayPlaces;
	public int AvailableStonePlaces;
	public int AvailableGoldPlaces;
	public int AvailableCard1Places;
	public int AvailableCard2Places;
	public int AvailableCard3Places;
	public int AvailableCard4Places;
	public int AvailableHouse1Places;
	public int AvailableHouse2Places;
	public int AvailableHouse3Places;
	public int AvailableHouse4Places;

	public event System.Action OnChanged;
	public event System.Action OnTurnEnded;
	bool _changed;
	private void SetChanged() {
		_changed = true;
	}
	public void Update() {
		if (!_changed)
			return;
		_changed = true;
		if (OnChanged != null)
			OnChanged ();
	}

	public int TurnInd;
	public int FirstPlayerInd { get { return TurnInd % Players.Count; } }

	public Game(List<Player> players) {

		WinnerInd = -1;
		TrainingControllers = new List<GameTrainingController> ();
		for (int i = 0; i < 4; i++)
			TrainingControllers.Add (new GameTrainingController (i, this));
		
		rand = new System.Random ();
		PlayerModels = new List<PlayerModel> ();
		this.Players = players;
		for (int i=0;i<4;i++) {
			TrainingControllers [i].OnBeforeModelChange ();
			var player = Players [i];
			PlayerModel model = new PlayerModel ((PlayerModel.Color)i);
			player.Init (model);
			PlayerModels.Add (model);
		}

		for (int i=0;i<4;i++)
			TrainingControllers [i].OnAfterModelChange (ModelChangeType.StartGame);
			
		TurnInd = -1;
		// Init buildings.
		List<HouseToBuild> allHouses = new List<HouseToBuild>();
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Forest, Resource.Forest, Resource.Clay }));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Forest, Resource.Clay, Resource.Gold }));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Clay, Resource.Clay, Resource.Stone }));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Clay, Resource.Stone, Resource.Stone}));
		allHouses.Add (new HouseToBuild (allHouses.Count, 5, 1));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Forest, Resource.Clay, Resource.Clay}));
		allHouses.Add (new HouseToBuild (allHouses.Count, 4, 1));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Forest, Resource.Clay, Resource.Stone}));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Forest, Resource.Stone, Resource.Stone}));
		allHouses.Add (new HouseToBuild (allHouses.Count, 5, 3));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Forest, Resource.Stone, Resource.Gold}));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Stone, Resource.Stone, Resource.Gold}));
		allHouses.Add (new HouseToBuild (allHouses.Count, 4, 3));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Clay, Resource.Clay, Resource.Gold}));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Forest, Resource.Forest, Resource.Stone}));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Forest, Resource.Clay, Resource.Stone}));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Forest, Resource.Clay, Resource.Gold}));
		allHouses.Add (new HouseToBuild (allHouses.Count, 5, 2));
		allHouses.Add (new HouseToBuild (allHouses.Count, 4, 2));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Clay, Resource.Stone, Resource.Gold}));
		allHouses.Add (new HouseToBuild (allHouses.Count, 5, 4));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Forest, Resource.Forest, Resource.Gold}));
		allHouses.Add (new HouseToBuild (allHouses.Count, 4, 4));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Clay, Resource.Stone, Resource.Gold}));
		allHouses.Add (new HouseToBuild (allHouses.Count, 1, 7, 1));
		allHouses.Add (new HouseToBuild (allHouses.Count, 1, 7, 1));
		allHouses.Add (new HouseToBuild (allHouses.Count, 1, 7, 1));
		allHouses.Add (new HouseToBuild (allHouses.Count, new List<Resource> () { Resource.Forest, Resource.Stone, Resource.Gold }));
		Utils.Shuffle<HouseToBuild> (allHouses);
		_houseHeap1 = new List<HouseToBuild> ();
		_houseHeap2 = new List<HouseToBuild> ();
		_houseHeap3 = new List<HouseToBuild> ();
		_houseHeap4 = new List<HouseToBuild> ();
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
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.RandomForEveryone, 0, BottomCardFeature.InstrumentsMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.InstrumentsOnce, 4, BottomCardFeature.InstrumentsMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceConstFood, 7, BottomCardFeature.Science, Science.Pot));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceConstFood, 4, BottomCardFeature.HouseMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.OneCardMore, 0, BottomCardFeature.Science, Science.Book));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceConstClay, 1, BottomCardFeature.HumanMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.RandomForEveryone, 0, BottomCardFeature.Science, Science.Car));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceConstStone, 2, BottomCardFeature.Science, Science.Car));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.RandomForEveryone, 0, BottomCardFeature.Science, Science.Book));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceConstStone, 1, BottomCardFeature.HumanMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceConstFood, 5, BottomCardFeature.Science, Science.Grass));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.RandomForEveryone, 0, BottomCardFeature.Science, Science.Pot));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceRandomGold, 2, BottomCardFeature.Science, Science.Statue));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.InstrumentsForever, 1, BottomCardFeature.Science, Science.Statue));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.RandomForEveryone, 0, BottomCardFeature.HouseMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.InstrumentsOnce, 3, BottomCardFeature.InstrumentsMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.InstrumentsOnce, 2, BottomCardFeature.InstrumentsMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.RandomForEveryone, 0, BottomCardFeature.InstrumentsMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceConstFood, 1, BottomCardFeature.Science, Science.Loom));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceConstGold, 1, BottomCardFeature.HumanMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.RandomForEveryone, 0, BottomCardFeature.FieldMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.Score, 3, BottomCardFeature.Science, Science.Music));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.Field, 1, BottomCardFeature.Science, Science.Clock));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceRandomForest, 2, BottomCardFeature.HumanMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.RandomForEveryone, 0, BottomCardFeature.FieldMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceConstStone, 1, BottomCardFeature.FieldMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.Field, 1, BottomCardFeature.FieldMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.Score, 3, BottomCardFeature.Science, Science.Music));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceRandomStone, 2, BottomCardFeature.HumanMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceConstFood, 3, BottomCardFeature.FieldMultiplier, 2));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceAny, 2, BottomCardFeature.Science, Science.Grass));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceConstFood, 3, BottomCardFeature.Science, Science.Loom));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.RandomForEveryone, 0, BottomCardFeature.Science, Science.Clock));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.RandomForEveryone, 0, BottomCardFeature.HouseMultiplier, 1));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.Score, 3, BottomCardFeature.HouseMultiplier, 3));
		_cardsInHeap.Add (new CardToBuild(_cardsInHeap.Count, TopCardFeature.ResourceConstFood, 2, BottomCardFeature.HouseMultiplier, 2));
		Utils.Shuffle<CardToBuild> (_cardsInHeap);

		SetChanged ();
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
		AvailableCard1Places = 1;
		AvailableCard2Places = 1;
		AvailableCard3Places = 1;
		AvailableCard4Places = 1;
		AvailableHouse1Places = 1;
		AvailableHouse2Places = 1;
		AvailableHouse3Places = 1;
		AvailableHouse4Places = 1;
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
		for (int i=0;i<4;i++) {
			TrainingControllers [i].OnBeforeModelChange ();
			PlayerModels[i].NewTurn ();
			TrainingControllers [i].OnAfterStartTurn ();
		}
		SetChanged ();
	}
	public bool IsEnded { get; private set; }
	private void CalcIsEnded() {
		WinnerInd = GetMaxScorePlayerInd ();
		const int MaxTurnsCount = 16;
		if (TurnInd >= MaxTurnsCount) {
			IsEnded = true;
			return;
		}
		// Condition 1 - cant prepare new card set.
		int requiredCards = 0;
		if (AvailableCardFor1Resource == null)
			requiredCards++;
		if (AvailableCardFor2Resource == null)
			requiredCards++;
		if (AvailableCardFor3Resource == null)
			requiredCards++;
		if (AvailableCardFor4Resource == null)
			requiredCards++;
		if (requiredCards > _cardsInHeap.Count) {
			IsEnded = true;
			return;
		}
		// Condition 2 - no houses in any heap.
		if (_houseHeap1.Count == 0) {
			IsEnded = true;
			return;
		}
		if (_houseHeap2.Count == 0) {
			IsEnded = true;
			return;
		}
		if (_houseHeap3.Count == 0) {
			IsEnded = true;
			return;
		}
		if (_houseHeap4.Count == 0) {
			IsEnded = true;
			return;
		}
		// Otherwise game continues.
		WinnerInd = -1;
	}
	public int WinnerInd { get; private set; }
	public int GetMaxScorePlayerInd() {
		int winnerInd = -1;
		if (PlayerModels [0].Score >= PlayerModels [1].Score && PlayerModels [0].Score >= PlayerModels [2].Score && PlayerModels [0].Score >= PlayerModels [3].Score)
			winnerInd = 0;
		else if (PlayerModels [1].Score >= PlayerModels [2].Score && PlayerModels [1].Score >= PlayerModels [3].Score)
			winnerInd = 1;
		else if (PlayerModels [2].Score >= PlayerModels [3].Score)
			winnerInd = 2;
		else
			winnerInd = 3;
		return winnerInd;
	}
	private int UnspentHumanCount {
		get {
			int count = 0;
			foreach (var player in PlayerModels)
				count += player.AvailableHumans;
			return count;
		}
	}
	static int[] _remainingResources = new int[5];
	public static int[] GetRemainingResourcesAfterHousesBuilding(Game game, PlayerModel model) {
		_remainingResources [(int)Resource.Forest] = model.Forest;
		_remainingResources [(int)Resource.Clay] = model.Clay;
		_remainingResources [(int)Resource.Stone] = model.Stone;
		_remainingResources [(int)Resource.Gold] = model.Gold;
		for (int slotInd = 0; slotInd < 4; slotInd++) {
			if (model.GetSpentOnHouse (slotInd) > 0)
				SubtractResourcesFromRemaining (game.GetHouse (slotInd));
		}
		return _remainingResources;
	}
	public static bool EnoughResourcesForBuilding(Game game, PlayerModel model, int buildingSlot) {
		GetRemainingResourcesAfterHousesBuilding (game, model);
		SubtractResourcesFromRemaining (game.GetHouse (buildingSlot));
		return _remainingResources [(int)Resource.Forest] >= 0 && _remainingResources [(int)Resource.Clay] >= 0 &&
			_remainingResources [(int)Resource.Stone] >= 0 && _remainingResources [(int)Resource.Gold] >= 0;
	}
	private static List<Resource> resourcesFromGoldToForest = new List<Resource>() { Resource.Gold, Resource.Stone, Resource.Clay, Resource.Forest };
	private static void SubtractResourcesFromRemaining(HouseToBuild house) {
		// Fast method of rough estimating resource enoughness for building.
		if (house.StaticCost != null) {
			foreach (var res in house.StaticCost)
				_remainingResources [(int)res]--;
		} else {
			int differentResCount = house.DifferentResourcesCount;
			foreach (var res in resourcesFromGoldToForest) {
				if (_remainingResources [(int)res] > 0) {
					_remainingResources [(int)res]--;
					differentResCount--;
				}
			}
			if (differentResCount > 0) {
				_remainingResources [(int)Resource.Forest] = -1;
				return;
			}
			int minResourcesCount = house.MinResourcesCount - house.DifferentResourcesCount;
			foreach (var res in resourcesFromGoldToForest) {
				while (minResourcesCount > 0 && _remainingResources [(int)res] > 0) {
					_remainingResources [(int)res]--;
					minResourcesCount--;
				}
			}
			if (minResourcesCount>0)
				_remainingResources [(int)Resource.Forest] = -1;
		}
	}
	public List<WhereToGo> GetAvailableTargets(PlayerModel player) {
		List<WhereToGo> targets = new List<WhereToGo> ();
		AddTargetIfAvailable (targets, WhereToGo.Card1, player);
		AddTargetIfAvailable (targets, WhereToGo.Card2, player);
		AddTargetIfAvailable (targets, WhereToGo.Card3, player);
		AddTargetIfAvailable (targets, WhereToGo.Card4, player);
		AddTargetIfAvailable (targets, WhereToGo.Clay, player);
		AddTargetIfAvailable (targets, WhereToGo.Field, player);
		AddTargetIfAvailable (targets, WhereToGo.Food, player);
		AddTargetIfAvailable (targets, WhereToGo.Forest, player);
		AddTargetIfAvailable (targets, WhereToGo.Gold, player);
		AddTargetIfAvailable (targets, WhereToGo.House1, player);
		AddTargetIfAvailable (targets, WhereToGo.House2, player);
		AddTargetIfAvailable (targets, WhereToGo.House3, player);
		AddTargetIfAvailable (targets, WhereToGo.House4, player);
		AddTargetIfAvailable (targets, WhereToGo.Housing, player);
		AddTargetIfAvailable (targets, WhereToGo.Instrument, player);
		AddTargetIfAvailable (targets, WhereToGo.Stone, player);
		return targets;
	}
	private void AddTargetIfAvailable(List<WhereToGo> targets, WhereToGo target, PlayerModel player) {
		switch (target) {
			case WhereToGo.Housing:
				if (player.HumansCount >= 10)
					return;
				break;
			case WhereToGo.Field:
				if (player.FieldsCount >= 10)
					return;
				break;
			case WhereToGo.Instrument:
				if (player.InstrumentsCountSlot3 >= 4)
					return;
				break;
		}

		if (GetAvailableHumansCountFor (target) > 0 && GetMinHumansCountFor (target) <= player.AvailableHumans && player.GetSpentHumansCountFor(target)==0)
			targets.Add (target);
	}
	public int GetMaxHumansCountFor(WhereToGo target) {
		switch (target) {
			default: 
				return 0;
			case WhereToGo.Card1: 
			case WhereToGo.Card2: 
			case WhereToGo.Card3: 
			case WhereToGo.Card4: 
			case WhereToGo.Field:
			case WhereToGo.House1:
			case WhereToGo.House2:
			case WhereToGo.House3:
			case WhereToGo.House4:
			case WhereToGo.Instrument:
				return 1;
			case WhereToGo.Clay:
			case WhereToGo.Forest:
			case WhereToGo.Gold:
			case WhereToGo.Stone:
				return 7;
			case WhereToGo.Food:
				return 40;
			case WhereToGo.Housing:
				return 2;
		}
	}
	public int GetMinHumansCountFor(WhereToGo target) {
		switch (target) {
		default: 
			return 1;
		case WhereToGo.Card1: 
		case WhereToGo.Card2: 
		case WhereToGo.Card3: 
		case WhereToGo.Card4: 
		case WhereToGo.Field:
		case WhereToGo.House1:
		case WhereToGo.House2:
		case WhereToGo.House3:
		case WhereToGo.House4:
		case WhereToGo.Instrument:
		case WhereToGo.Clay:
		case WhereToGo.Forest:
		case WhereToGo.Gold:
		case WhereToGo.Stone:
		case WhereToGo.Food:
			return 1;
		case WhereToGo.Housing:
			return 2;
		}
	}
	public static Resource GetResourceFromTarget(WhereToGo target) {
		switch (target) {
			default: return Resource.None;
			case WhereToGo.Food: return Resource.Food;
			case WhereToGo.Forest: return Resource.Forest;
			case WhereToGo.Clay: return Resource.Clay;
			case WhereToGo.Stone: return Resource.Stone;
			case WhereToGo.Gold: return Resource.Gold;
		}
	}
	public static int GetResourceCost(Resource res) {
		switch (res) {
			default: return int.MaxValue;
			case Resource.Food: return 2;
			case Resource.Forest: return 3;
			case Resource.Clay: return 4;
			case Resource.Stone: return 5;
			case Resource.Gold: return 6;
		}
	}
	public int GetSpentHumansCountFor(WhereToGo target) {
		int count = 0;
		foreach (var player in PlayerModels)
			count += player.GetSpentHumansCountFor(target);
		return count;
	}
	public int GetAvailableHumansCountFor(WhereToGo target) {
		return GetMaxHumansCountFor (target) - GetSpentHumansCountFor (target);
	}
	const int SleepTime = 100;
	public void Play(System.Action onEnded) {
		IsEnded = false;
		while (!IsEnded) {
			NewTurn ();
			//if (Logging) 
			//Debug.Log ("New turn, ind = " + TurnInd.ToString());
			int finishedTurnPlayersCount = 0;

			// First phase - selecting where to go.
			while (finishedTurnPlayersCount<Players.Count) {
				finishedTurnPlayersCount = 0;
				for (int i = 0; i < Players.Count; i++) {
					int currPlayerInd = (FirstPlayerInd + i) % Players.Count;
					Player currPlayer = Players [currPlayerInd];
					PlayerModel model = PlayerModels [currPlayerInd];
					WhereToGo target = WhereToGo.None;
					List<WhereToGo> availableTargets = GetAvailableTargets (model);
					if (availableTargets.Count > 0) {
						int[] inputs = AINeuralPlayer.GetInputs (DecisionType.SelectWhereToGo, this, model, Resource.None, WhereToGo.None);
						List<int> options = AINeuralPlayer.GetOptionInds (DecisionType.SelectWhereToGo, this, model, null, -1, Resource.None, WhereToGo.None);
						TrainingDecisionModel trainingModel = new TrainingDecisionModel (DecisionType.SelectWhereToGo, inputs, options, currPlayerInd);
						currPlayer.SelectWhereToGo (this, (WhereToGo tgt) => {
							target = tgt;
						});
						while (target== WhereToGo.None)
							System.Threading.Thread.Sleep (SleepTime);
						trainingModel.Output = (int)target - 1;
						TrainingControllers [currPlayerInd].OnBeforeModelChange ();
						TrainingControllers [currPlayerInd].OnAfterWhereToGoSelected (trainingModel, target);
					} else {
						if (availableTargets.Count == 0) {
							finishedTurnPlayersCount++;
							continue; // player finished spending humans.
						}
						target = availableTargets [0];
					}
					SetChanged ();

					int count = -1;
					int minCount = GetMinHumansCountFor (target);
					int maxCount = GetMaxHumansCountFor (target);
					TrainingDecisionModel trainingModelUsedHumans = new TrainingDecisionModel (DecisionType.SelectUsedHumans, 
						AINeuralPlayer.GetInputs (DecisionType.SelectUsedHumans, this, model, GetResourceFromTarget(target), target),
						AINeuralPlayer.GetOptionInds(DecisionType.SelectUsedHumans, this, model, null, -1, GetResourceFromTarget(target), target), currPlayerInd);
					
					if (maxCount - minCount > 0) {
						
						currPlayer.SelectUsedHumans (this, target, (int cnt) => {
							count = cnt;
						});
						while (count==-1)
							System.Threading.Thread.Sleep (SleepTime);
					} else
						count = maxCount;
					trainingModelUsedHumans.Output = count - 1;

					if (Logging) Debug.LogFormat("{2} go to {0} with {1}", target.ToString(), count.ToString(), currPlayer.Color.ToString());
					TrainingControllers [currPlayerInd].OnBeforeModelChange ();
					switch (target) {
						case WhereToGo.Card1: model.GoToCard1 (); break;
						case WhereToGo.Card2: model.GoToCard2 (); break;
						case WhereToGo.Card3: model.GoToCard3 (); break;
						case WhereToGo.Card4: model.GoToCard4 (); break;
						case WhereToGo.House1: model.GoToBuilding1 (); break;
						case WhereToGo.House2: model.GoToBuilding2 (); break;
						case WhereToGo.House3: model.GoToBuilding3 (); break;
						case WhereToGo.House4: model.GoToBuilding4 (); break;
						case WhereToGo.Food: model.GoToFood (count); break;
						case WhereToGo.Forest: model.GoToForest (count); break;
						case WhereToGo.Clay: model.GoToClay (count); break;
						case WhereToGo.Stone: model.GoToStone (count); break;
						case WhereToGo.Gold: model.GoToGold (count); break;
						case WhereToGo.Field: model.GoToFields (); break;
						case WhereToGo.Instrument: model.GoToInstruments (); break;
						case WhereToGo.Housing: model.GoToHousing (); break;
					}
					TrainingControllers [currPlayerInd].OnAfterHumansCountSelected (trainingModelUsedHumans, target);
					SetChanged ();
				}
			}

			// Second phase - using cards and applying where to go.
			for (int i = 0; i < Players.Count; i++) {
				int currPlayerInd = (FirstPlayerInd + i) % Players.Count;
				Player currPlayer = Players [currPlayerInd];
				PlayerModel model = PlayerModels [currPlayerInd];

				// Select resource from card.
				bool hasAnyResourceFromCard = model.GetHasAnyResourceFromCard ();
				if (hasAnyResourceFromCard) {
					bool selectedToUse = false;
					bool processEnded = false;
					currPlayer.UseGetAnyResourceFromTopCard (this, (bool use) => {
						selectedToUse = use;
						processEnded = true;
					});
					while (!processEnded)
						System.Threading.Thread.Sleep (SleepTime);
					if (Logging) {
						if (selectedToUse)
							Debug.Log ("Selected to use any resource from card");
						else
							Debug.Log ("Not selected to use any resource from card");
					}

					if (selectedToUse) {
						TrainingControllers [currPlayerInd].OnBeforeModelChange ();
						Resource resource = Resource.None;
						currPlayer.ChooseResourceToReceiveFromTopCard (this, (Resource res) => {
							resource = res;
						});
						while (resource == Resource.None)
							System.Threading.Thread.Sleep (SleepTime);						
						model.AddResource (resource, 1);
						resource = Resource.None;
						currPlayer.ChooseResourceToReceiveFromTopCard (this, (Resource res) => {
							resource = res;
						});
						while (resource==Resource.None)
							System.Threading.Thread.Sleep (SleepTime);
						if (Logging) Debug.Log ("Selected resource to receive = "+resource.ToString());
						model.AddResource (resource, 1);
						model.ApplyAnyResourceFromTopCard ();
						TrainingControllers [currPlayerInd].OnReceived2ResFromCard ();
						SetChanged ();
					}
				}

				// Receiving resources.
				List<WhereToGo> resourcesReceivingList = new List<WhereToGo>() { WhereToGo.Food, WhereToGo.Forest, WhereToGo.Clay, WhereToGo.Stone, WhereToGo.Gold };
				foreach (WhereToGo target in resourcesReceivingList)
					MineResourceIfHumansSpent (target, currPlayerInd);


				// Buy cards.
				for (int cardInd = 0; cardInd < 4; cardInd++) {
					if (model.GetSpentOnCard (cardInd) < 1)
						continue; // Should be spent on card.
					TrainingControllers [currPlayerInd].OnBeforeModelChange();
					CardToBuild card = GetAvailableCard (cardInd);

					int cardPrice = cardInd+1;
					int resourcesSum = model.Forest + model.Clay + model.Stone + model.Gold;
					if (cardPrice > resourcesSum) {
						model.ApplyGoToCard (false, cardInd, null, card);
						TrainingControllers [currPlayerInd].OnHumanRemovedFromCard (cardInd);
						SetChanged ();
						continue; //  Should have enough resources.
					}
					
					bool selectedToBuy = false;
					bool processEnded = false;
					currPlayer.BuildCard (this, cardInd, (bool use) => {
						selectedToBuy = use;
						processEnded = true;
					});
					while (!processEnded)
						System.Threading.Thread.Sleep (SleepTime);
					if (Logging) {
						if (selectedToBuy)
							Debug.Log ("Selected build card");
						else
							Debug.Log ("Not selected to build card");
					}

					if (!selectedToBuy) {
						model.ApplyGoToCard (false, cardInd, null, card);
						TrainingControllers [currPlayerInd].OnHumanRemovedFromCard (cardInd);
						SetChanged ();
						continue; // Player should want to build card.
					}

					// Define what resources to spend.
					List<Resource> spendResources = new List<Resource>();
					for (int resourceInd = 0; resourceInd < cardPrice; resourceInd++) {
						bool processEnded2 = false;
						currPlayer.GetUsedResourceForCardBuilding (this, GetAvailableCard(cardInd), spendResources, (Resource resource) => {
							spendResources.Add(resource);
							processEnded2 = true;
							int forest = model.Forest;
							foreach (Resource res in spendResources) {
								if (res == Resource.Forest)
									forest--;
							}
							if (forest<0)
								Debug.Log("WTF");
						});
						while (!processEnded2)
							System.Threading.Thread.Sleep (SleepTime);
					}

					if (Logging) {
						string log = "Selected to buy card with selected cards ";
						foreach (Resource res in spendResources)
							log += res.ToString () + " ";
						Debug.Log (log);
					}

					
					model.ApplyGoToCard (true, cardInd, spendResources, card);
					TrainingControllers [currPlayerInd].OnAfterCardBought (cardInd, card);
					RemoveAvailableCard (cardInd);
					SetChanged ();

					// Get top bonuses from cards.
					switch (card.TopFeature) {
					case TopCardFeature.RandomForEveryone: 
						List<int> options = new List<int> ();
						for (int optionInd = 0; optionInd < Players.Count; optionInd++)
							options.Add (RandomRange (0, 6));

						for (int playerIndShift = 0; playerIndShift < Players.Count; playerIndShift++) {
							int randomBonusPlayerInd = (currPlayerInd + playerIndShift) % Players.Count;
							Player currPlayer2 = Players [randomBonusPlayerInd];
							PlayerModel model2 = PlayerModels [randomBonusPlayerInd];
							int selectedOption = -1;
							var optionInds = AINeuralPlayer.GetOptionInds (DecisionType.SelectCharity, this, model2, options, -1, Resource.None, WhereToGo.None);

							TrainingDecisionModel trainingModel = new TrainingDecisionModel (DecisionType.SelectCharity, 
								AINeuralPlayer.GetInputs (DecisionType.SelectCharity, this, model2, Resource.None, WhereToGo.None),
								optionInds, randomBonusPlayerInd);
							
							currPlayer2.ChooseItemToReceiveFromCharityCard (this, options, (int option) => {
								selectedOption = option;
								if (!optionInds.Contains (option))
									Debug.Log ("WTF");
							});
							while (selectedOption==-1)
								System.Threading.Thread.Sleep (SleepTime);
							if (Logging) Debug.LogFormat ("Charity option {0} selected by player {1}", selectedOption, randomBonusPlayerInd);

							trainingModel.Output = selectedOption;
							if (!trainingModel.Options.Contains (trainingModel.Output))
								Debug.Log ("WTF");

							options.Remove (selectedOption);
							TrainingControllers [randomBonusPlayerInd].OnBeforeModelChange ();
							switch (selectedOption) {
								case 0:model2.AddResource (Resource.Forest, 1);break;
								case 1:model2.AddResource (Resource.Clay, 1);break;
								case 2:model2.AddResource (Resource.Stone, 1);break;
								case 3:model2.AddResource (Resource.Gold, 1);break;
								case 4:model2.AddInstrument ();break;
								case 5:model2.AddField ();break;
							}
							TrainingControllers [randomBonusPlayerInd].OnAfterCharityReceived (trainingModel, playerIndShift == 0);
							SetChanged ();
						}
						break;
					case TopCardFeature.OneCardMore:
						if (_cardsInHeap.Count > 0) {
							CardToBuild oneMoreCard = _cardsInHeap [0];
							_cardsInHeap.RemoveAt (0);
							TrainingControllers [currPlayerInd].OnBeforeModelChange ();
							model.ApplyCardTopOneCardMore (oneMoreCard);
							TrainingControllers [currPlayerInd].OnAfterCardFromCardApplied ();
							if (Logging)
								Debug.LogFormat ("Added one more card to player ind={0} with bottom={1} and param={2}", currPlayerInd, oneMoreCard.BottomFeature, oneMoreCard.BottomFeatureParam);
							SetChanged ();
						}
						break;
					case TopCardFeature.ResourceRandomForest:
					case TopCardFeature.ResourceRandomStone:
					case TopCardFeature.ResourceRandomGold:
						AddRandomResourceFromTopCard (card, model, currPlayer);
						break;
					}
				}

				// Build houses.
				for (int houseInd = 0; houseInd < 4; houseInd++) {
					if (model.GetSpentOnHouse (houseInd) == 0)
						continue; // Should spend numan there.
					TrainingControllers [currPlayerInd].OnBeforeModelChange();
					HouseToBuild house = GetHouse (houseInd);
					if (!EnoughResourcesForHouse (house, model)) {
						model.ApplyGoToBuilding(houseInd, null); 
						TrainingControllers [currPlayerInd].OnHumanRemovedFromHouse (houseInd);
						SetChanged ();
						continue; // Should be able to build.
					}

					bool selectedToBuild = false;
					bool processEnded = false;
					currPlayer.BuildHouse (this, houseInd, (bool build) => {
						selectedToBuild = build;
						processEnded = true;
					});
					while (!processEnded)
						System.Threading.Thread.Sleep (SleepTime);
					if (Logging) {
						if (selectedToBuild)
							Debug.Log ("Selected build house");
						else
							Debug.Log ("Not selected to build house");
					}
					if (!selectedToBuild) {
						model.ApplyGoToBuilding(houseInd, null); 
						TrainingControllers [currPlayerInd].OnHumanRemovedFromHouse (houseInd);
						SetChanged ();
						continue;
					}// Should want to build. 

					// Define resources to spend.
					List<Resource> spentResources = new List<Resource>();
					if (house.StaticCost != null) {
						foreach (var res in house.StaticCost)
							spentResources.Add (res);
					} else {
						for (int resourceInd = 0; resourceInd < house.MaxResourcesCount; resourceInd++) {
							List<Resource> options = GetSpendResourceOnHouseOptions (house, spentResources, model);
							Resource selectedRes = Resource.None;
							processEnded = false;
							currPlayer.GetUsedResourceForHouseBuilding (this, house, options, spentResources, (Resource res) => {
								selectedRes = res;
								processEnded = true;
							});
							while (!processEnded)
								System.Threading.Thread.Sleep (SleepTime);
							if (selectedRes == Resource.None)
								break; // No more resource spending.
							spentResources.Add(selectedRes);
						}
					}
					if (Logging) {
						string log = "Selected resources for house building ";
						foreach (Resource res in spentResources)
							log += res.ToString () + " ";
						Debug.Log (log);
					}

					// Spend resources.
					model.ApplyGoToBuilding(houseInd, spentResources);
					RemoveAvailableHouse (houseInd);
					TrainingControllers [currPlayerInd].OnAfterHouseBought (houseInd, spentResources);
					SetChanged ();
				}
			
				// Crafting instruments.
				if (model.SpentOnInstruments > 0) {
					TrainingControllers [currPlayerInd].OnBeforeModelChange ();
					model.ApplyGoToInstruments ();
					TrainingControllers [currPlayerInd].OnAfterModelChange (ModelChangeType.ApplyGoToInstruments);
					SetChanged ();
				}

				// Seeding fields.
				if (model.SpentOnFields > 0) {
					TrainingControllers [currPlayerInd].OnBeforeModelChange ();
					model.ApplyGoToFields ();
					TrainingControllers [currPlayerInd].OnAfterModelChange (ModelChangeType.ApplyGoToFields);
					SetChanged ();
				}

				// Housing.
				if (model.SpentOnHousing > 0) {
					TrainingControllers [currPlayerInd].OnBeforeModelChange ();
					model.ApplyGoToHousing ();
					TrainingControllers [currPlayerInd].OnAfterModelChange (ModelChangeType.ApplyGoToHousing);
					SetChanged ();
				}
			}

			// Third phase - feeding.
			for (int feededPlayerInd = 0; feededPlayerInd < Players.Count; feededPlayerInd++) {
				int currFedPlayerInd = (FirstPlayerInd + feededPlayerInd) % Players.Count;
				Player currFedPlayer = Players [currFedPlayerInd];
				PlayerModel modelForFeding = PlayerModels [currFedPlayerInd];

				int neededFood = Mathf.Max( modelForFeding.HumansCount - modelForFeding.FieldsCount );
				bool enoughFood = neededFood <= modelForFeding.Food;
				bool selectedToLeaveHungry = false;

				TrainingDecisionModel trainingModel = new TrainingDecisionModel (DecisionType.SelectLeaveHungry, 
					AINeuralPlayer.GetInputs (DecisionType.SelectLeaveHungry, this, modelForFeding, Resource.None, WhereToGo.None),
					AINeuralPlayer.GetOptionInds(DecisionType.SelectLeaveHungry, this, modelForFeding, null, -1, Resource.None, WhereToGo.None), currFedPlayerInd);
				
				if (!enoughFood) {
					int eatenResourcesCount = neededFood - modelForFeding.Food;
					bool enoughResources = modelForFeding.Forest + modelForFeding.Clay + modelForFeding.Stone + modelForFeding.Gold >= eatenResourcesCount;
					if (!enoughResources)
						selectedToLeaveHungry = true;
					else {
						
						bool processEnded = false;
						currFedPlayer.LeaveHungry (this, eatenResourcesCount, (bool leaveHungry) => {
							selectedToLeaveHungry = leaveHungry;
							processEnded = true;
						});
						while (!processEnded)
							System.Threading.Thread.Sleep (SleepTime);
					}
				}
				trainingModel.Output = selectedToLeaveHungry?1:0;
				if (Logging) {
					if (selectedToLeaveHungry)
						Debug.Log ("Selected to leave hungry");
					else
						Debug.Log ("Not selected to leave hungry");
				}

				List<Resource> eatenResources = new List<Resource> ();
				int neededResourceCount = Mathf.Max(0, modelForFeding.HumansCount - modelForFeding.FieldsCount - modelForFeding.Food);
				for (int ind = 0; ind < modelForFeding.Forest; ind++) {
					if (neededResourceCount == 0)
						break;
					eatenResources.Add (Resource.Forest);
					neededResourceCount--;
				}
				for (int ind = 0; ind < modelForFeding.Clay; ind++) {
					if (neededResourceCount == 0)
						break;
					eatenResources.Add (Resource.Clay);
					neededResourceCount--;
				}
				for (int ind = 0; ind < modelForFeding.Stone; ind++) {
					if (neededResourceCount == 0)
						break;
					eatenResources.Add (Resource.Stone);
					neededResourceCount--;
				}
				for (int ind = 0; ind < modelForFeding.Gold; ind++) {
					if (neededResourceCount == 0)
						break;
					eatenResources.Add (Resource.Gold);
					neededResourceCount--;
				}
				TrainingControllers [currFedPlayerInd].OnBeforeModelChange ();
				modelForFeding.Feed (selectedToLeaveHungry, eatenResources);
				TrainingControllers [currFedPlayerInd].OnAfterFeeding (trainingModel, selectedToLeaveHungry && !enoughFood);
				SetChanged ();
			}
		
			for (int i = 0; i < 4; i++) {
				TrainingControllers [i].OnBeforeModelChange ();
				PlayerModels [i].ApplyEndTurn ();
				TrainingControllers [i].OnAfterModelChange (ModelChangeType.EndTurn);
			}
			if (OnTurnEnded != null)
				OnTurnEnded ();
			CalcIsEnded ();
		}

		for (int i = 0; i < 4; i++)
			TrainingControllers [i].OnEndGame (i==WinnerInd);
		
		if (onEnded!=null)
			onEnded ();
	}

	private void MineResourceIfHumansSpent(WhereToGo target, int currPlayerInd) {
		Resource resource = GetResourceFromTarget (target);
		PlayerModel model = PlayerModels [currPlayerInd];
		Player currPlayer = Players [currPlayerInd];
		int spentHumans = model.GetSpentHumansCountFor (target);
		if (spentHumans == 0)
			return;
		int rand = 0;
		for (int j=0;j<spentHumans;j++)
			rand += RandomRange(1, 6);
		bool processEnded = false;

		TrainingDecisionModel trainingModel = new TrainingDecisionModel (DecisionType.SelectInstruments, 
			AINeuralPlayer.GetInputs (DecisionType.SelectInstruments, this, model, resource, target),
			AINeuralPlayer.GetOptionInds(DecisionType.SelectInstruments, this, model, null, rand, resource, target), currPlayerInd);

		int randFromDices = rand;

		TrainingControllers [currPlayerInd].OnBeforeModelChange ();
		model.OnResourceDicesRolled (target);
		TrainingControllers [currPlayerInd].OnAfterResourceDicesRolled (randFromDices);

		TrainingControllers[currPlayerInd].OnBeforeModelChange();

		currPlayer.GetUsedInstrumentSlotInd (this, resource, rand,
			(bool useSlot0, bool useSlot1, bool useSlot2, bool useOnce4Slot, bool useOnce3Slot, bool useOnce2Slot) => {
				processEnded = true;

				if (useSlot0 && !model.InstrumentsSlot1Used) {
					model.ApplyUseInstrumentSlot(0);
					rand += model.InstrumentsCountSlot1;
					if (Logging) Debug.LogFormat("Used instrument slot {0} with count {1}", 0, model.InstrumentsCountSlot1);
				}
				if (useSlot1 && !model.InstrumentsSlot2Used) {
					model.ApplyUseInstrumentSlot(1);
					rand += model.InstrumentsCountSlot2;
					if (Logging) Debug.LogFormat("Used instrument slot {0} with count {1}", 1, model.InstrumentsCountSlot2);
				}
				if (useSlot2 && !model.InstrumentsSlot3Used) {
					model.ApplyUseInstrumentSlot(2);
					rand += model.InstrumentsCountSlot3;
					if (Logging) Debug.LogFormat("Used instrument slot {0} with count {1}", 2, model.InstrumentsCountSlot3);
				}
				if (useOnce4Slot && !model.Top4Instruments.Card.TopUsed) {
					model.Top4Instruments.Card.UseTop();
					rand += 4;
					if (Logging) Debug.LogFormat("Used instrument slot {0} with count {1}", "top4", 4);
				}
				if (useOnce3Slot && !model.Top3Instruments.Card.TopUsed) {
					model.Top3Instruments.Card.UseTop();
					rand += 3;
					if (Logging) Debug.LogFormat("Used instrument slot {0} with count {1}", "top3", 3);
				}
				if (useOnce2Slot && !model.Top2Instruments.Card.TopUsed) {
					model.Top2Instruments.Card.UseTop();
					rand += 2;
					if (Logging) Debug.LogFormat("Used instrument slot {0} with count {1}", "top2", 2);
				}
			});
		while (!processEnded)
			System.Threading.Thread.Sleep (SleepTime);

		int cost = GetResourceCost (resource);
		int resourceCountFromDices = randFromDices / cost;
		int resourceCount = rand / cost;
		int resourcesFromInstruments = resourceCount - resourceCountFromDices;
		trainingModel.Output = Mathf.Min( 4, resourcesFromInstruments);

		TrainingControllers[currPlayerInd].OnAfterInstrumentsApplied(trainingModel, rand-randFromDices);

		TrainingControllers[currPlayerInd].OnBeforeModelChange();
		model.AddResource (resource, resourceCount);
		TrainingControllers [currPlayerInd].OnAfterResourcesMined ();
		SetChanged ();
	}

	private void AddRandomResourceFromTopCard(CardToBuild card, PlayerModel model, Player player) {
		int currPlayerInd = (int)player.Color;
		WhereToGo target = WhereToGo.None;
		switch (card.TopFeature) {
			case TopCardFeature.ResourceRandomForest: target = WhereToGo.Forest; break;
			case TopCardFeature.ResourceRandomStone: target = WhereToGo.Stone; break;
			case TopCardFeature.ResourceRandomGold: target = WhereToGo.Gold; break;
		}
		MineResourceIfHumansSpent (target, currPlayerInd);
	}

	private static List<Resource> GetSpendResourceOnHouseOptions(HouseToBuild house, List<Resource> spentResources, PlayerModel player) {
		if (house.StaticCost != null)
			return null;

		List<Resource> spentDifferentResources = new List<Resource> ();
		Dictionary<Resource, int> spentCounts = new Dictionary<Resource, int> ();
		spentCounts.Add (Resource.Forest, 0);
		spentCounts.Add (Resource.Clay, 0);
		spentCounts.Add (Resource.Stone, 0);
		spentCounts.Add (Resource.Gold, 0);
		foreach (var res in spentResources) {
			if (!spentDifferentResources.Contains (res))
				spentDifferentResources.Add (res);
			spentCounts [res]++;
		}

		List<Resource> options = new List<Resource> ();
		bool shouldSelectNotUsedPreviously = house.DifferentResourcesCount > spentDifferentResources.Count;
		// Select available resource.
		if (player.Forest > spentCounts [Resource.Forest] && (!shouldSelectNotUsedPreviously || !spentResources.Contains(Resource.Forest)))
			options.Add (Resource.Forest);
		if (player.Clay > spentCounts [Resource.Clay] && (!shouldSelectNotUsedPreviously || !spentResources.Contains(Resource.Clay)))
			options.Add (Resource.Clay);
		if (player.Stone > spentCounts [Resource.Stone] && (!shouldSelectNotUsedPreviously || !spentResources.Contains(Resource.Stone)))
			options.Add (Resource.Stone);
		if (player.Gold > spentCounts [Resource.Gold] && (!shouldSelectNotUsedPreviously || !spentResources.Contains(Resource.Gold)))
			options.Add (Resource.Gold);
		return options;
	}

	private static bool EnoughResourcesForHouse(HouseToBuild house, PlayerModel player) {
		if (house.StaticCost != null) {
			Dictionary<Resource, int> costResourceCounts = new Dictionary<Resource, int> ();
			foreach (var res in house.StaticCost) {
				if (costResourceCounts.ContainsKey (res))
					costResourceCounts [res]++;
				else
					costResourceCounts [res] = 1;
			}
			foreach (var res in costResourceCounts.Keys) {
				if (player.GetResourceCount (res) < costResourceCounts [res])
					return false;
			}
			return true;
		} else {
			if (house.MinResourcesCount > player.Forest + player.Clay + player.Stone + player.Gold)
				return false;

			List<Resource> differentAvailableResources = new List<Resource> ();
			if (player.GetResourceCount (Resource.Forest) > 0)
				differentAvailableResources.Add (Resource.Forest);
			if (player.GetResourceCount (Resource.Clay) > 0)
				differentAvailableResources.Add (Resource.Clay);
			if (player.GetResourceCount (Resource.Stone) > 0)
				differentAvailableResources.Add (Resource.Stone);
			if (player.GetResourceCount (Resource.Gold) > 0)
				differentAvailableResources.Add (Resource.Gold);
			return differentAvailableResources.Count >= house.DifferentResourcesCount;
		}
	}
}
