using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

public class NeuralPlayerTrainerController {

	public static void Train(int loopsCount, List<TrainingDecisionModel> trainingModels, float speed, AINeuralPlayer player, Action<float> onProgressUpdated) {
		
		Thread thread = new Thread (new ThreadStart(()=>{
			float totalComplexity = player.GetDecider(DecisionType.SelectWhereToGo).Complexity + 
				player.GetDecider(DecisionType.SelectUsedHumans).Complexity + 
				player.GetDecider(DecisionType.SelectCharity).Complexity + 
				player.GetDecider(DecisionType.SelectInstruments).Complexity + 
				player.GetDecider(DecisionType.SelectLeaveHungry).Complexity;

			float progress = 0;

			TrainOneDecider(DecisionType.SelectUsedHumans, loopsCount, trainingModels, speed, player, onProgressUpdated, ref progress, totalComplexity);
			TrainOneDecider(DecisionType.SelectCharity, loopsCount, trainingModels, speed, player, onProgressUpdated, ref progress, totalComplexity);
			TrainOneDecider(DecisionType.SelectInstruments, loopsCount, trainingModels, speed, player, onProgressUpdated, ref progress, totalComplexity);
			TrainOneDecider(DecisionType.SelectLeaveHungry, loopsCount, trainingModels, speed, player, onProgressUpdated, ref progress, totalComplexity);
			TrainOneDecider(DecisionType.SelectWhereToGo, loopsCount, trainingModels, speed, player, onProgressUpdated, ref progress, totalComplexity);
		}));
		thread.Start ();
	}
	private static void TrainOneDecider(DecisionType type, int loopsCount, List<TrainingDecisionModel> trainingModels, float speed, AINeuralPlayer player, Action<float> onProgressUpdated, ref float progress, float totalComplexity) {
		float currComplexity = player.GetDecider(type).Complexity;
		for (int loopInd = 0; loopInd < loopsCount; loopInd++) {
			if (onProgressUpdated!=null)
				onProgressUpdated(progress + (loopInd+1)/(float)loopsCount*currComplexity/totalComplexity);
			DoTrainingLoop(type, trainingModels, speed, player);
		}
		progress += currComplexity/totalComplexity;
	}
	public static void DoTrainingLoop(DecisionType type, List<TrainingDecisionModel> trainingModels, float speed, AINeuralPlayer player) {
		foreach (var training in trainingModels) {
			if (type != DecisionType.None && training.Type != type)
				continue;

			int output = training.Output;
			float reward = training.RewardPercent;
			string options = "";
			foreach (var option in training.Options)
				options += option.ToString()+";";
			NeuralNetwork decider = player.GetDecider (training.Type);
			int existingOutput = AINeuralPlayer.GetDecisionFromOutputs(decider.Think (training.Inputs), training.Options);
			//if ((existingOutput == training.Output) == (training.RewardPercent > 0))
			//	continue;
			float nu = training.RewardPercent*speed;
			if (nu < 0)
				nu *= 1/(float)WinToLooseImportancyRatio;
			double[] idealOutputs = new double[decider.OutputLength];
			for (int optionInd = 0; optionInd < idealOutputs.Length; optionInd++) {
				if (optionInd == training.Output)
					idealOutputs [optionInd] = 1;
				else
					idealOutputs [optionInd] = 0;
			}
			decider.Train (training.Inputs, idealOutputs, training.Options, nu);
		}
	}
	public const float WinToLooseImportancyRatio = 4;
}
