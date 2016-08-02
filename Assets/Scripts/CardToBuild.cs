using UnityEngine;
using System.Collections;

public enum TopCardFeature {
	RandomForEveryone,
	InstrumentsOnce,
	ResourceConstFood,
	ResourceConstClay,
	ResourceConstStone,
	ResourceConstGold,
	OneCardMore,
	ResourceRandomForest,
	ResourceRandomStone,
	ResourceRandomGold,
	InstrumentsForever,
	Score,
	Field,
	ResourceAny
}
public enum BottomCardFeature {
	InstrumentsMultiplier,
	HouseMultiplier,
	PeopleMultiplier,
	FieldMultiplier,
	Science
}
public enum Science {
	Pot = 0,
	Book = 1,
	Car = 2,
	Grass = 3,
	Statue = 4,
	Loom = 5,
	Music = 6,
	Clock = 7
}
public class CardToBuild {
	TopCardFeature TopFeature;
	int TopFeatureParam;
	BottomCardFeature BottomFeature;
	int BottomFeatureParam;
	public CardToBuild(TopCardFeature topFeature, int topFeatureParam, BottomCardFeature bottomFeature, int bottomFeatureParam) {
		this.TopFeature = topFeature;
		this.TopFeatureParam = topFeatureParam;
		this.BottomFeature = bottomFeature;
		this.BottomFeatureParam = bottomFeatureParam;
	}
	public CardToBuild(TopCardFeature topFeature, int topFeatureParam, BottomCardFeature bottomFeature, Science science):
	this( topFeature, topFeatureParam, bottomFeature, (int)science) {
	}
}
