using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NeuralDecisionNetwork {
	public int InputLength { get { return _networks [0].InputLength; } }
	public int OutputLength { get { return _networks.Count; } }
	private List<NeuralNetwork> _networks;
	public NeuralDecisionNetwork(int inputLayersCount, int decisionOptionsCount, int[] innerLayers) {
		_networks = new List<NeuralNetwork> ();
		int[] layerSizes = new int[innerLayers.Length+2];
		layerSizes [0] = inputLayersCount;
		layerSizes [layerSizes.Length - 1] = 1;
		for (int i = 0; i < innerLayers.Length; i++)
			layerSizes [i + 1] = innerLayers [i];
		for (int i = 0; i < decisionOptionsCount; i++)
			_networks.Add (new NeuralNetwork (layerSizes));
	}
	public int Think(int[] inputs, List<int> options) {
		double max = double.NegativeInfinity;
		int ind = -1;
		for (int i = 0; i < OutputLength; i++) {
			if (options != null && !options.Contains (i))
				continue;
			double curr = _networks [i].Think (inputs)[0];
			if (curr > max) {
				max = curr;
				ind = i;
			}
		}
		return ind;
	}
	public void Train(int[] inputs, int idealOutput, List<int> options, float nu) {
		double[] outputs = new double[OutputLength];
		double max = double.NegativeInfinity;
		int ind = -1;
		for (int i = 0; i < OutputLength; i++) {
			if (options != null && !options.Contains (i))
				continue;
			outputs[i] = _networks [i].Think (inputs)[0];
			if (outputs[i] > max) {
				max = outputs[i];
				ind = i;
			}
		}
		if (ind == idealOutput)
			return; // Dont teach anything if output is already correct.
		const double eps = 0.1f;
		double gap = max - outputs [idealOutput] + eps;
		double[] idealOutputs = new double[1];
		for (int i = 0; i < OutputLength; i++) {
			if (options != null && !options.Contains (i))
				continue;
			if (i == idealOutput) {
				idealOutputs [0] = 1;//outputs [i] + gap;
				_networks [i].Train (inputs, idealOutputs, null, nu);
			} else if (outputs [i] > outputs [idealOutput]) {
				idealOutputs [0] = 0;//outputs [i] - gap;
				_networks [i].Train (inputs, idealOutputs, null, nu);
			}
		}
	}
}
