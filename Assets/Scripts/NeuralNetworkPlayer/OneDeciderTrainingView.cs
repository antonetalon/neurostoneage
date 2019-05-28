using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;

public class OneDeciderTrainingView : MonoBehaviour {
	
	[SerializeField] Text _trainingsDataSize;
	[SerializeField] Text _performedTrainingsCount;
	[SerializeField] Text _error;
	List<TrainingDecisionModel> _trainingModels;
	NeuralDecisionNetwork _decider;
	public void Init(DecisionType type, List<TrainingDecisionModel> trainingModels, AINeuralPlayer2 player) {
		_trainingModels = new List<TrainingDecisionModel> ();
		foreach (var model in trainingModels) {
			if (model.Type == type && model.RewardPercent>0.5f)
				_trainingModels.Add (model);
		}
		_trainingsDataSize.text = _trainingModels.Count.ToString ();
		_decider = player.GetChooserDecider(type);
		_trainingsCount = 0;
	}
	bool _isTraining;
	int _trainingsCount;
	public void OnToggleTrainPressed() {
		_isTraining = !_isTraining;
		if (!_isTraining)
			return;

		// Train thread.
		Thread thread = new Thread (new ThreadStart(()=>{
			while (_isTraining) {
				TrainingDecisionModel currTraining = _trainingModels[_trainingsCount%_trainingModels.Count];
				float currSpeed = 0.0005f;//*currTraining.RewardPercent*0.01f;
				_decider.Train(currTraining.Inputs, currTraining.Output, currTraining.Options, currSpeed);
				_trainingsCount++;
				CompositionRoot.Instance.ExecuteInMainThread (() => {
					_performedTrainingsCount.text = _trainingsCount.ToString();
				});
			}
		}));
		thread.Start ();

		// Calc success thread.
		Thread thread2 = new Thread (new ThreadStart(()=>{
			while (_isTraining) {
				float error = GetError();
				System.Threading.Thread.Sleep(300);
				CompositionRoot.Instance.ExecuteInMainThread (() => {
					_error.text = error.ToString ("00.0000");
					Debug.Log("success updated, success = " + error.ToString());
				});
			}
		}));
		thread2.Start ();
	}

	private float GetError() {
		NeuralDecisionNetwork decider = _decider.Clone();
		int successes = 0;
		for (int i = 0; i < _trainingModels.Count; i++) {
			if (_trainingModels [i].Output == decider.Think (_trainingModels [i].Inputs, _trainingModels [i].Options))
				successes++;
		}
		return ((_trainingModels.Count - successes) / (float)_trainingModels.Count);
	}
}
