using UnityEngine;
using System.Collections;

public static class Config {
	public const int PointsPerFood = 2;
	public const int PointsPerForest = 3;
	public const int PointsPerClay = 4;
	public const int PointsPerStone = 5;
	public const int PointsPerGold = 6;
	public static int GetPointsPerResource(Resource res) {
		switch (res) {
			case Resource.Food: return PointsPerFood;
			case Resource.Forest: return PointsPerForest;
			case Resource.Clay: return PointsPerClay;
			case Resource.Stone: return PointsPerStone;
			case Resource.Gold: return PointsPerGold;
			default: return 0;
		}
	}
}
