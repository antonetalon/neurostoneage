using UnityEngine;
using System.Collections;

public enum TopCardFeature {
	RandomForEveryone,//
	InstrumentsOnce,
	ResourceConstFood,//
	ResourceConstClay,//
	ResourceConstStone,//
	ResourceConstGold,//
	OneCardMore,
	ResourceRandomForest,
	ResourceRandomStone,
	ResourceRandomGold,
	InstrumentsForever,//
	Score,//
	Field,//
	ResourceAny//
}
public enum BottomCardFeature {
	InstrumentsMultiplier,
	HouseMultiplier,
	HumanMultiplier,
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
	public readonly TopCardFeature TopFeature;
	public readonly int TopFeatureParam;
	public readonly BottomCardFeature BottomFeature;
	public readonly int BottomFeatureParam;
	public bool TopUsed { get; private set; }
	public readonly int Ind;
	public CardToBuild(int ind, TopCardFeature topFeature, int topFeatureParam, BottomCardFeature bottomFeature, int bottomFeatureParam) {
		this.TopFeature = topFeature;
		this.TopFeatureParam = topFeatureParam;
		this.BottomFeature = bottomFeature;
		this.BottomFeatureParam = bottomFeatureParam;
		this.TopUsed = true;
		if (TopFeature == TopCardFeature.InstrumentsOnce || TopFeature == TopCardFeature.ResourceAny)
			TopUsed = false;
		this.Ind = ind;
	}
	public CardToBuild(int ind, TopCardFeature topFeature, int topFeatureParam, BottomCardFeature bottomFeature, Science science):
	this( ind, topFeature, topFeatureParam, bottomFeature, (int)science) {
	}
	public void UseTop() {
		TopUsed = true;
	}
}
