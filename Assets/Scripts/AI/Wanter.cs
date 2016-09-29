using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Wanter {
	float[] _inToOutCoefs;
	float[,] _inToAdditionalCoefs;
	float[] _additionalToOutCoefs;
	float _constWanting;

	float[] _additionals;
	public Wanter(int inputsCount, int additionalNeuronsCount) {
		_inToOutCoefs = new float[inputsCount];
		_inToAdditionalCoefs = new float[inputsCount,additionalNeuronsCount];
		_additionalToOutCoefs = new float[additionalNeuronsCount];
		_additionals = new float[additionalNeuronsCount];
	}
	public void SetRandomValues() {
		for (int i = 0; i < _inToOutCoefs.Length; i++) {
			_inToOutCoefs [i] = GetRandomCoef();
			for (int j = 0; j < _additionalToOutCoefs.Length; j++)
				_inToAdditionalCoefs [i, j] = GetRandomCoef();
		}
		for (int j = 0; j < _additionalToOutCoefs.Length; j++)
			_additionalToOutCoefs [j] = GetRandomCoef();
		_constWanting = GetRandomCoef ();
	}
	private float GetRandomCoef() {
		return Game.RandomValue - 0.5f;
	}
	public float GetValue(int[] inputs) {
		float wanting = _constWanting;
		for (int j = 0; j < _additionalToOutCoefs.Length; j++)
			_additionals [j] = 0;
		for (int i = 0; i < _inToOutCoefs.Length; i++) {
			wanting += _inToOutCoefs [i] * inputs [i];
			for (int j = 0; j < _additionalToOutCoefs.Length; j++)
				_additionals [j] += _inToAdditionalCoefs [i, j] * inputs [i];
		}
		for (int j = 0; j < _additionalToOutCoefs.Length; j++)
			wanting += _additionalToOutCoefs [j] * _additionals [j];
		return wanting;
	}
}
