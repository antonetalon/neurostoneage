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
	public Wanter Clone() {
		Wanter clone = new Wanter (-1,-1);
		clone._inToOutCoefs = new float[_inToOutCoefs.Length];
		for (int i = 0; i < _inToOutCoefs.Length; i++)
			clone._inToOutCoefs [i] = _inToOutCoefs [i];
		for (int i = 0; i < _inToAdditionalCoefs.GetLength(0); i++) {
			for (int j = 0; j < _inToAdditionalCoefs.GetLength (1); j++)
				clone._inToAdditionalCoefs [i, j] = _inToAdditionalCoefs [i, j];
		}
		for (int i = 0; i < _additionalToOutCoefs.Length; i++)
			clone._additionalToOutCoefs [i] = _additionalToOutCoefs [i];
		clone._constWanting = _constWanting;
		for (int i = 0; i < _additionals.Length; i++)
			clone._additionals [i] = _additionals [i];
		return clone;
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
			_additionals [j] = 1;
		for (int i = 0; i < _inToOutCoefs.Length; i++) {
			wanting += _inToOutCoefs [i] * inputs [i];
			for (int j = 0; j < _additionalToOutCoefs.Length; j++)
				_additionals [j] *= Mathf.Pow(Mathf.Max(0, inputs [i])+1, _inToAdditionalCoefs [i, j]+0.5f);
		}
		for (int j = 0; j < _additionalToOutCoefs.Length; j++)
			wanting += _additionalToOutCoefs [j] * _additionals [j];
		return wanting;
	}
}
