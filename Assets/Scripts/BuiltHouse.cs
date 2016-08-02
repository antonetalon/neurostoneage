using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuiltHouse {
	public readonly ReadonlyList<Resource> SpentResources;
	public readonly int Score;
	public BuiltHouse(List<Resource> spentResources) {
		SpentResources = new ReadonlyList<Resource> (spentResources);
		Score = 0;
		foreach (var resource in spentResources)
			Score += Config.GetPointsPerResource (resource);
	}
}
