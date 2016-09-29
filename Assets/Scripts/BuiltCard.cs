using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BuiltCard {
	public readonly CardToBuild Card;
	public BuiltCard(CardToBuild card) {
		this.Card = card; 
	}
}
