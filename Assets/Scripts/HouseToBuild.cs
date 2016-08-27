using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HouseToBuild {
	public ReadonlyList<Resource> StaticCost;
	public int MinResourcesCount;
	public int MaxResourcesCount;
	public int DifferentResourcesCount;
	public readonly int Ind;
	public HouseToBuild(int ind, List<Resource> cost) {
		StaticCost = new ReadonlyList<Resource>(cost);
		MinResourcesCount = -1;
		MaxResourcesCount = -1;
		DifferentResourcesCount = -1;
		this.Ind = ind;
	}
	public HouseToBuild(int ind, int count, int differentCount) {
		StaticCost = null;
		MinResourcesCount = count;
		MaxResourcesCount = count;
		DifferentResourcesCount = differentCount;
		this.Ind = ind;
	}
	public HouseToBuild(int ind, int minCount, int maxCount, int differentCount) {
		StaticCost = null;
		MinResourcesCount = minCount;
		MaxResourcesCount = maxCount;
		DifferentResourcesCount = differentCount;
		this.Ind = ind;
	}
}
