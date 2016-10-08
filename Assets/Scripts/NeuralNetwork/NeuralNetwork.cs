using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class NeuralNetwork {

	public int LayersCount { get; private set; }
	private int[] _layerSizes;
	public int OutputLength { get { return _layerSizes [_layerSizes.Length - 1]; } }
	private double[][,] _weights;

	public NeuralNetwork(int[] layerSizes) {
		LayersCount = layerSizes.Length;
		_layerSizes = layerSizes;
		_weights = new double[LayersCount-1][,];
		_sigmas = new double[LayersCount-1][];
		for (int layerInd=0;layerInd<_weights.Length;layerInd++) {
			_weights[layerInd] = new double[layerSizes[layerInd]+1,layerSizes[layerInd+1]];
			_sigmas[layerInd] = new double[layerSizes[layerInd+1]];
			for (int neuronInd1 = 0; neuronInd1 < _layerSizes [layerInd]+1; neuronInd1++) {
				for (int neuronInd2 = 0; neuronInd2 < _layerSizes [layerInd + 1]; neuronInd2++)
					_weights [layerInd][neuronInd1, neuronInd2] = (double)Game.RandomValue-0.5d;
			}
		}
		_neuronOutputs = new double[LayersCount][];
		for (int layerInd = 0; layerInd < LayersCount; layerInd++)
			_neuronOutputs [layerInd] = new double[_layerSizes [layerInd] + 1];
		_neuronInputs = new double[LayersCount-1][];
		for (int layerInd = 0; layerInd < LayersCount-1; layerInd++)
			_neuronInputs [layerInd] = new double[_layerSizes [layerInd+1]];
	}
	private void InitIfNeeded() {
		if (_neuronOutputs != null)
			return;
		_sigmas = new double[LayersCount-1][];
		for (int layerInd=0;layerInd<_weights.Length;layerInd++)
			_sigmas[layerInd] = new double[_layerSizes[layerInd+1]];
		_neuronOutputs = new double[LayersCount][];
		for (int layerInd = 0; layerInd < LayersCount; layerInd++)
			_neuronOutputs [layerInd] = new double[_layerSizes [layerInd] + 1];
		_neuronInputs = new double[LayersCount-1][];
		for (int layerInd = 0; layerInd < LayersCount-1; layerInd++)
			_neuronInputs [layerInd] = new double[_layerSizes [layerInd+1]];
	}
	public NeuralNetwork Clone() {
		int[] layerSizes = new int[_layerSizes.Length];
		for (int i = 0; i < layerSizes.Length; i++)
			layerSizes [i] = _layerSizes [i];
		NeuralNetwork clone = new NeuralNetwork (layerSizes);
		for (int i = 0; i < _weights.Length; i++) {
			for (int j = 0; j < _weights [i].GetLength (0); j++) {
				for (int k = 0; k < _weights [i].GetLength (1); k++)
					clone._weights [i] [j, k] = _weights [i] [j, k];
			}
		}
		return clone;
	}

	[NonSerialized]
	double[][] _neuronOutputs;
	[NonSerialized]
	double[][] _neuronInputs;
	public double[] Think(double[] inputs) {
		InitIfNeeded ();
		System.Diagnostics.Debug.Assert (inputs.Length == _layerSizes [0]);
		// Setup initial outputs.
		for (int layerInd = 0; layerInd < LayersCount; layerInd++) {
			for (int neuronInd = 0; neuronInd <= _layerSizes [layerInd]; neuronInd++) {
				if (neuronInd == _layerSizes [layerInd])
					_neuronOutputs [layerInd] [neuronInd] = 1; // Bias neuron.
				else if (layerInd == 0)
					_neuronOutputs [0][neuronInd] = inputs[neuronInd]; // Retina neuron.
				else
					_neuronOutputs [layerInd] [neuronInd] = 0;
			}
		}
		// Setup initial inputs.
		for (int layerInd = 1; layerInd < LayersCount; layerInd++) {
			for (int neuronInd = 0; neuronInd < _layerSizes [layerInd]; neuronInd++)
				_neuronInputs [layerInd-1] [neuronInd] = 0;
		}
		// Forward propagation.
		for (int layerInd = 1; layerInd < LayersCount; layerInd++) {
			for (int neuronInd1 = 0; neuronInd1 < _layerSizes [layerInd]; neuronInd1++) {
				// Sum values.
				for (int neuronInd2 = 0; neuronInd2 < _layerSizes [layerInd-1]+1; neuronInd2++)
					_neuronInputs[layerInd-1] [neuronInd1] += _weights [layerInd-1] [neuronInd2, neuronInd1] * _neuronOutputs [layerInd - 1][neuronInd2];
				// Apply activation function.
				_neuronOutputs[layerInd][neuronInd1] = FAct(_neuronInputs[layerInd-1][neuronInd1]);
			}
		}
		return _neuronOutputs [LayersCount - 1];
	}
	private double FAct(double sum) {
		return 1/(1+Math.Exp(-sum));
	}
	private double FActDerivative(double sum) {
		double exp = Math.Exp (-sum);
		double expPlus1 = 1 + exp;
		return exp/(expPlus1*expPlus1);
	}

	public void Train(int[] inputs, double[] idealOutputs, List<int> consideredOutputs, float nu) {
		double[] inputDoubles = new double[inputs.Length];
		for (int i = 0; i < inputs.Length; i++)
			inputDoubles [i] = inputs [i];
		Train (inputDoubles, idealOutputs, consideredOutputs, nu);
	}

	public void Train(double[] inputs, double[] idealOutputs, float nu) {
		Train (inputs, idealOutputs, null, nu);
	}

	[NonSerialized]
	private double[][] _sigmas;
	public void Train(double[] inputs, double[] idealOutputs, List<int> consideredOutputs, float nu) {
		InitIfNeeded ();
		double[] outputs = Think (inputs);
		// Backward propagation.
		for (int layerInd = LayersCount - 1; layerInd >= 1; layerInd--) {
			for (int neuronInd1 = 0; neuronInd1 < _layerSizes [layerInd]; neuronInd1++) {
				if (layerInd == LayersCount - 1) {
					if (consideredOutputs == null || consideredOutputs.Contains (neuronInd1))
						_sigmas [layerInd - 1] [neuronInd1] = FActDerivative (_neuronInputs [layerInd - 1] [neuronInd1]) * (idealOutputs [neuronInd1] - outputs [neuronInd1]);
					else
						_sigmas [layerInd - 1] [neuronInd1] = 0;
				} else {
					double sum = 0;
					for (int neuronInd2 = 0; neuronInd2 < _layerSizes [layerInd + 1]; neuronInd2++)
						sum += _sigmas [layerInd] [neuronInd2] * _weights [layerInd] [neuronInd1, neuronInd2];
					_sigmas [layerInd-1] [neuronInd1] = FActDerivative (_neuronInputs [layerInd-1] [neuronInd1]) * sum;
				}					
			}
		}

		// Deltas applying.
		for (int layerInd=0;layerInd<_weights.Length;layerInd++) {
			for (int neuronInd1 = 0; neuronInd1 < _layerSizes [layerInd]+1; neuronInd1++) {
				for (int neuronInd2 = 0; neuronInd2 < _layerSizes [layerInd + 1]; neuronInd2++) {
					double delta = nu * _neuronOutputs [layerInd] [neuronInd1] * _sigmas [layerInd] [neuronInd2];
					_weights [layerInd] [neuronInd1, neuronInd2] += delta;
				}
			}
		}
	}

	/*public void AddNeuron(int layerInd) {
	}
	public void RemoveNeuron(int layerInd, int ind) {
	}*/
}
