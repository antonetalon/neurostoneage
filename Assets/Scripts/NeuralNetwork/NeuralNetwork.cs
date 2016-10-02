using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class NeuralNetwork {

	public int LayersCount { get; private set; }
	private int[] _layerSizes;
	private double[][,] _weights;

	public NeuralNetwork(int[] layerSizes) {
		LayersCount = layerSizes.Length;
		_layerSizes = layerSizes;
		_weights = new double[LayersCount-1][,];
		for (int layerInd=0;layerInd<_weights.Length;layerInd++) {
			_weights[layerInd] = new double[layerSizes[layerInd],layerSizes[layerInd+1]];
			for (int neuronInd1 = 0; neuronInd1 < _layerSizes [layerInd]; neuronInd1++) {
				for (int neuronInd2 = 0; neuronInd2 < _layerSizes [layerInd + 1]; neuronInd2++)
					_weights [layerInd][neuronInd1, neuronInd2] = (double)Game.RandomValue;
			}
		}
		_neuronValues = new double[LayersCount][];
		for (int layerInd=0;layerInd<LayersCount;layerInd++)
			_neuronValues[layerInd] = new double[_layerSizes[layerInd]];
	}

	[NonSerialized]
	double[][] _neuronValues;
	public double[] Think(double[] inputs) {
		System.Diagnostics.Debug.Assert (inputs.Length == _layerSizes [0]);
		for (int layerInd = 0; layerInd < _weights.Length; layerInd++) {
			for (int neuronInd = 0; neuronInd < _layerSizes [layerInd]; neuronInd++) {
				_neuronValues [layerInd][neuronInd] = layerInd==0?inputs[neuronInd]:0;
			}
		}
		for (int layerInd = 1; layerInd < LayersCount; layerInd++) {
			for (int neuronInd1 = 0; neuronInd1 < _layerSizes [layerInd]; neuronInd1++) {
				// Sum values.
				for (int neuronInd2 = 0; neuronInd2 < _layerSizes [layerInd-1]; neuronInd2++)
					_neuronValues [layerInd] [neuronInd1] += _weights [layerInd-1] [neuronInd2, neuronInd1] * _neuronValues [layerInd - 1][neuronInd2];
				// Apply activation function.
				_neuronValues[layerInd][neuronInd1] = FAct(_neuronValues[layerInd][neuronInd1]);
			}
		}
		return _neuronValues [LayersCount - 1];
	}
	private double FAct(double sum) {
		return 1/(1+Math.Exp(-sum));
	}

	/*public void AddNeuron(int layerInd) {
	}
	public void RemoveNeuron(int layerInd, int ind) {
	}*/
}
