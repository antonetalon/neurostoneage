using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HouseToBuild {
	List<Resource> StaticCost;
	public int MinResourcesCount;
	public int MaxResourcesCount;
	public int DifferentResourcesCount;
	public HouseToBuild(List<Resource> cost) {
		StaticCost = cost;
		MinResourcesCount = -1;
		MaxResourcesCount = -1;
		DifferentResourcesCount = -1;
	}
	public HouseToBuild(int count, int differentCount) {
		StaticCost = null;
		MinResourcesCount = count;
		MaxResourcesCount = count;
		DifferentResourcesCount = differentCount;
	}
	public HouseToBuild(int minCount, int maxCount, int differentCount) {
		StaticCost = null;
		MinResourcesCount = minCount;
		MaxResourcesCount = maxCount;
		DifferentResourcesCount = differentCount;
	}
}
