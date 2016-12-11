using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;
using System;

public class TempTests : MonoBehaviour {
	NeuralDecisionNetwork _network;
	[SerializeField] Text _loopsCountLabel;
	private int _loopsCount = 0;
	System.Random _rand;
	public void CreateRandomAI() {
		_rand = new System.Random ();
		_network = new NeuralDecisionNetwork (81, 16, new int[2] { 50, 30 });
		Debug.Log ("created");
		_loopsCountLabel.text = _loopsCount.ToString ();
	}
	bool _isTraining;
	public void ToggleTraining() {
		_isTraining = !_isTraining;
		if (!_isTraining)
			return;
		Thread trainingThread = new Thread (new ThreadStart (() => {
			while (_isTraining) {
				//System.Threading.Thread.Sleep(100);
				TrainLoop();
				if (_loopsCount%20==0)
					CalcSuccess();
			}
		}));
		trainingThread.Start ();
	}
	void TrainLoop() {
		int[] inputs = new int[_network.InputLength];
		int output =  _rand.Next () % _network.OutputLength;
		for (int i = 0; i < inputs.Length; i++) {
			inputs [i] = _rand.Next () % 100;
		}
		inputs[0] = _rand.Next () % 2;
		if (inputs [0] > 0.5)
			output = 0;		
		_network.Train (inputs, output, null, 0.0005f);
		_loopsCount++;
		CompositionRoot.Instance.ExecuteInMainThread (() => {
			_loopsCountLabel.text = _loopsCount.ToString ();
		});
	}
	[SerializeField] Text _errorLabel;
	void CalcSuccess() {
		int[] inputs = new int[_network.InputLength];
		int output;
		double error = 0;
		int expCount = 1000;
		for (int i = 0; i < expCount; i++) {
			for (int j = 1; j < inputs.Length; j++)
				inputs [j] = _rand.Next () % 100;
			inputs[0] = 1;
			output = _network.Think (inputs, null);
			if (output != 0)
				error += 1;
		}
		error /= expCount;
		CompositionRoot.Instance.ExecuteInMainThread (() => {
			_errorLabel.text = error.ToString ("00.000");
		});
	}
}
