using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;

public class OneDeciderTrainingView : MonoBehaviour {
	
	[SerializeField] NetworkTestView _view;
	[SerializeField] Text _loopsCountInput;
	[SerializeField] Text _speedInput;
	[SerializeField] Text _totalLoops;
	[SerializeField] Text _state;
	DecisionType _type;
	List<TrainingDecisionModel> _trainingModels;
	AINeuralPlayer _player;
	public void Init(DecisionType type, List<TrainingDecisionModel> trainingModels, AINeuralPlayer player) {
		_type = type;
		_trainingModels = new List<TrainingDecisionModel> ();
		foreach (var model in trainingModels) {
			if (model.Type == type || type == DecisionType.None)
				_trainingModels.Add (model);
		}
		_player = player;
		UpdateView ();
	}
	public void OnTrainPressed() {
		int loopsCount = int.Parse (_loopsCountInput.text);
		if (loopsCount <= 0)
			return;
		//Debug.LogFormat("Training {0} loops started", loopsCount);
		float speed = float.Parse(_speedInput.text); // 0.5f
		Thread thread = new Thread (new ThreadStart(()=>{
			for (int loopInd = 0; loopInd < loopsCount; loopInd++) {
				_view.UpdateProgress (loopInd+1, loopsCount);
				NeuralPlayerTrainerController.DoTrainingLoop(_type, _trainingModels, speed, _player);
			}
			CompositionRoot.Instance.ExecuteInMainThread (() => {
				_view.UpdateView ();
			});
		}));
		thread.Start ();
		//Debug.LogFormat("Training {0} loops finished", loopsCount);
	}
	enum ComparingState { Improved, Still, Degraded }
	public void OnTrainWhilePossiblePressed() {
		const int minLoopsCount = 400;
		const int step = 40;
		const float minSuccessibilityChange = 0.001f;
		//Debug.LogFormat("Training {0} loops started", loopsCount);

		Thread thread = new Thread (new ThreadStart(()=>{
			bool trainingPossible = true;
			int loopInd = 0;
			int loopsUntilSuccessCalc = step;
			float prevSuccess = GetSuccess();
			float learningSpeed = 0.5f;
			const float minLearningSpeed = 0.001f;
			const float maxLearningSpeed = 10;
			ComparingState prevState = ComparingState.Improved;
			do {
				loopInd++;
				NeuralPlayerTrainerController.DoTrainingLoop(_type, _trainingModels, learningSpeed, _player);

				loopsUntilSuccessCalc--;
				if (loopsUntilSuccessCalc==0) {
					loopsUntilSuccessCalc = step;
					float success = GetSuccess();
					ComparingState state;
					if (Mathf.Abs(success-prevSuccess)<minSuccessibilityChange) {
						// No change.
						learningSpeed *= 1.2f;
						state = ComparingState.Still;
						trainingPossible = false;
					} else if (success>prevSuccess) {
						// Improved success.
						//learningSpeed *= 1.1f;
						state = ComparingState.Improved;
					} else {
						// Success decreased.
						learningSpeed *= 0.1f;
						state = ComparingState.Degraded;
					}
					if (state == ComparingState.Degraded && prevState == ComparingState.Still)
						trainingPossible = false;
					Debug.Log("learning speed = " + learningSpeed.ToString());
					if (learningSpeed<minLearningSpeed || learningSpeed>maxLearningSpeed)
						trainingPossible = false;
					prevSuccess = success;
					prevState = state;
				}

				if (loopInd%10==0) {
					int shownCount = loopInd;
					CompositionRoot.Instance.ExecuteInMainThread (() => {
						_totalLoops.text = shownCount.ToString();
					});
				}

				if (loopInd<minLoopsCount)
					trainingPossible = true;
			} while (trainingPossible);
				
			CompositionRoot.Instance.ExecuteInMainThread (() => {
				_loopsCountInput.text = loopInd.ToString();
				_view.UpdateView ();
			});
		}));
		thread.Start ();
		//Debug.LogFormat("Training {0} loops finished", loopsCount);
	}

	private float GetSuccess() {
		int positiveTrainingCount = 0;
		int negativeTrainingCount = 0;
		int positiveSuccessesCount = 0;
		int negativeSuccessesCount = 0;
		GetSuccessfullTrainingsCount (_type,ref positiveTrainingCount,ref negativeTrainingCount,ref positiveSuccessesCount,ref negativeSuccessesCount);
		return (positiveSuccessesCount + negativeSuccessesCount) / (float)(positiveTrainingCount + negativeTrainingCount);
	}

	public void UpdateView() {
		if (_trainingModels.Count == 0) {
			_state.text = "no data";
			return;
		}
		int positiveTrainingCount = 0;
		int negativeTrainingCount = 0;
		int positiveSuccessesCount = 0;
		int negativeSuccessesCount = 0;
		GetSuccessfullTrainingsCount (_type,ref positiveTrainingCount,ref negativeTrainingCount,ref positiveSuccessesCount,ref negativeSuccessesCount);
		_state.text = string.Format ("pos {0}/{1}={2:P}\nneg {3}/{4}={5:P}", 
			positiveSuccessesCount, positiveTrainingCount, positiveSuccessesCount/(float)positiveTrainingCount,
			negativeSuccessesCount, negativeTrainingCount, negativeSuccessesCount/(float)negativeTrainingCount);
	}
	private void GetSuccessfullTrainingsCount(DecisionType type,
		ref int positiveTrainingCount,ref int negativeTrainingCount,ref int positiveSuccessesCount,ref int negativeSuccessesCount) {
		NeuralNetwork decider = _player.GetDecider (type);
		double[] inputs = null;
		foreach (TrainingDecisionModel model in _trainingModels) {
			if (model.Type != type)
				continue;
			bool isPositiveTraining = model.RewardPercent >= 0;
			if (inputs==null)
				inputs = new double[model.Inputs.Length];
			for (int i = 0; i < inputs.Length; i++)
				inputs [i] = model.Inputs [i];
			double[] outputs = decider.Think (inputs);
			int selectedInd = AINeuralPlayer.GetDecisionFromOutputs (outputs, model.Options);
			bool selectionEqualsTrainingTarget = selectedInd == model.Output;
			bool trainingSuccess = selectionEqualsTrainingTarget == isPositiveTraining;
			if (isPositiveTraining) {
				positiveTrainingCount++;
				if (trainingSuccess)
					positiveSuccessesCount++;
			} else {
				negativeTrainingCount++;
				if (trainingSuccess)
					negativeSuccessesCount++;
			}
		}
	}


}
