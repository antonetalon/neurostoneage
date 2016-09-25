using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Decider {
	Wanter[] _wanters;
	public Decider(int inputsCount, int additionalNeuronsCount, int outputsCount) {
		_wanters = new Wanter[outputsCount];
		for (int i = 0; i < _wanters.Length; i++)
			_wanters [i] = new Wanter (inputsCount, additionalNeuronsCount);
	}
	public void SetRandomValues() {
		for (int i = 0; i < _wanters.Length; i++)
			_wanters [i].SetRandomValues ();
	}
	public int GetDecision(int[] inputs, List<int> options) {
		int mostWantedOption = -1;
		float biggestWanting = float.MinValue;
		for (int i = 0; i < _wanters.Length; i++) {
			if (!options.Contains (i))
				continue;
			float wanting = _wanters [i].GetValue (inputs);
			if (wanting > biggestWanting) {
				mostWantedOption = i;
				biggestWanting = wanting;
			}
		}
		return mostWantedOption;
	}
}
