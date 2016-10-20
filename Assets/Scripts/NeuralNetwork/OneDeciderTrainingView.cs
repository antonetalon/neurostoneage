using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;

public class OneDeciderTrainingView : MonoBehaviour {

	[SerializeField] NetworkTestView _view;
	[SerializeField] Text _loopsCountInput;
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
	int totalLoops;
	const int threadsCount = 8;
	public void OnTrainPressed() {
		int loopsCount = int.Parse (_loopsCountInput.text);
		if (loopsCount <= 0)
			return;
		//Debug.LogFormat("Training {0} loops started", loopsCount);
		totalLoops = 0;

		for (int threadInd = 0; threadInd < threadsCount; threadInd++) {
			int ind = threadInd;
			Thread thread = new Thread (()=> TrainInOneThread(loopsCount, ind));
			thread.Start ();
		}
		//Debug.LogFormat("Training {0} loops finished", loopsCount);
	}
	private void TrainInOneThread(int loopsCount, int currThread) {
		Debug.LogFormat ("{0}-th thread started", currThread);
		NeuralNetwork decider = _player.GetDecider (_type);
		NeuralNetwork currThreadDecider = decider.Clone();
		for (int loopInd = 0; loopInd < loopsCount/threadsCount+1; loopInd++) {
			_view.UpdateProgress (totalLoops + 1, loopsCount);
			totalLoops++;
			Debug.LogFormat("{0}-th loop, {1}-th for curr thread, {2}-th thread", totalLoops, loopInd, currThread);
			currThreadDecider.CopyWeightsFrom(decider);
			foreach (var training in _trainingModels) {
				if (_type != DecisionType.None && training.Type != _type)
					continue;

				int output = training.Output;
				float reward = training.RewardPercent;
				string options = "";
				foreach (var option in training.Options)
					options += option.ToString () + ";";
				//					CompositionRoot.Instance.ExecuteInMainThread (() => {
				//						Debug.LogFormat("output={0} from {2}, reward={1}", output, reward, options);
				//					});
				int existingOutput = AINeuralPlayer.GetDecisionFromOutputs (currThreadDecider.Think (training.Inputs), training.Options);
				//if ((existingOutput == training.Output) == (training.RewardPercent > 0))
				//	continue;
				float learningSpeed = training.RewardPercent * 0.5f;
				if (learningSpeed < 0)
					//continue; // Do not do negative training.
					learningSpeed *= 0.15f; // Slower negative learning.
				double[] idealOutputs = new double[currThreadDecider.OutputLength];
				for (int optionInd = 0; optionInd < idealOutputs.Length; optionInd++) {
					if (optionInd == training.Output)
						idealOutputs [optionInd] = 1;
					else
						idealOutputs [optionInd] = 0;
				}
				currThreadDecider.Train (training.Inputs, idealOutputs, training.Options, learningSpeed, decider);
			}
		}
		CompositionRoot.Instance.ExecuteInMainThread (() => {
			_view.UpdateView ();
		});
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
		if (_type == DecisionType.None){
			GetSuccessfullTrainingsCount (DecisionType.SelectWhereToGo, ref positiveTrainingCount, ref negativeTrainingCount, ref positiveSuccessesCount, ref negativeSuccessesCount);
			GetSuccessfullTrainingsCount (DecisionType.SelectUsedHumans, ref positiveTrainingCount, ref negativeTrainingCount, ref positiveSuccessesCount, ref negativeSuccessesCount);
			GetSuccessfullTrainingsCount (DecisionType.SelectInstruments, ref positiveTrainingCount, ref negativeTrainingCount, ref positiveSuccessesCount, ref negativeSuccessesCount);
			GetSuccessfullTrainingsCount (DecisionType.SelectCharity, ref positiveTrainingCount, ref negativeTrainingCount, ref positiveSuccessesCount, ref negativeSuccessesCount);
			GetSuccessfullTrainingsCount (DecisionType.SelectLeaveHungry, ref positiveTrainingCount, ref negativeTrainingCount, ref positiveSuccessesCount, ref negativeSuccessesCount);
		}else
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
			bool isPositiveTraining = model.RewardPercent > 0;
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
