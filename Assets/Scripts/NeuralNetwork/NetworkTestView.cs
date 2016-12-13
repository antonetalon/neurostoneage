using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text;
using System;

public class NetworkTestView : MonoBehaviour {

	[SerializeField] GameView _gameView;

	AINeuralPlayer2 _brain;
	AINeuralPlayer2 _prevBrain;
	int _trainingsCount;

	#region Manage data
	public void CreateRandomBrainPressed() {
		_brain = new AINeuralPlayer2();
		_prevBrain = (AINeuralPlayer2)_brain.Clone ();
		_trainingsCount = 0;
		Debug.Log ("Created ai");
	}
	public void OnCleanTrainingPressed() {
		_trainingModels.Clear ();
		UpdateView ();
		InitTrainers ();
	}

	public void OnRevertToPrevPressed() {
		_brain = (AINeuralPlayer2)_prevBrain.Clone ();
		UpdateView ();
	}
	#endregion

	#region Save/Load
	[SerializeField] ComparerView _savedModelsComparer;
	public void OnSavePlayerPressed() {
		_savedModelsComparer.SavePlayer (_brain, "Jorge");
	}
	const string FilePath = "SavedTraining";
	public void SaveTraining() {
		using (Stream stream = File.Open(FilePath, FileMode.Create))
		{
			var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			binaryFormatter.Serialize(stream, _trainingModels);
			binaryFormatter.Serialize(stream, _brain);
		}
		Debug.Log ("Training saved");
	}
	public void LoadTraining() {
		using (Stream stream = File.Open(FilePath, FileMode.Open))
		{
			var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			_trainingModels = (List<TrainingDecisionModel>)binaryFormatter.Deserialize(stream);
			_brain = (AINeuralPlayer2)binaryFormatter.Deserialize (stream);
			_prevBrain = (AINeuralPlayer2)_brain.Clone ();
		}
		UpdateView ();
		InitTrainers ();
		Debug.Log ("Training loaded");
	}
	#endregion

	#region Play
	public void PlayAndAddTraining() {

		List<Player> players = new List<Player> () {
			new HumanPlayer (_gameView.TurnView),
			_brain,
			_brain.Clone(),
			_brain.Clone()
		};
		_gameView.gameObject.SetActive (true);
		Game game = new Game (players);
		_gameView.Init (game);
		Thread thread = new Thread (new ThreadStart(()=>{game.Play (()=>{
			CompositionRoot.Instance.ExecuteInMainThread(()=>{
				//_view.gameObject.SetActive (false);
				Debug.Log("Match ended");
				foreach (var trainingController in game.TrainingControllers) {
					foreach (var trainingModel in trainingController.TrainingModels)
						_trainingModels.Add(trainingModel);
				}
				UpdateView();
				InitTrainers ();
			});
		});}));
		thread.Start ();
	}
	[SerializeField] InputField _gamesCountInput;
	[SerializeField] Text _meanScore;
	public void OnPlayWithAIAndAddTraining() {
		int count = int.Parse (_gamesCountInput.text);
		long sumScore = 0;
		long scoresCount = 0;
		List<Player> players = new List<Player> () {
			_brain,
			_brain.Clone(),
			_brain.Clone(),
			_brain.Clone()
		};
		Game game = new Game (players);
		Thread thread = new Thread (new ThreadStart(()=>{
			for (int i=0;i<count;i++) {
				bool matchEnded = false;
				game.Play (()=>{
					for (int j=0;j<4;j++) {
						scoresCount++;
						sumScore += game.PlayerModels[j].Score;
					}
					foreach (var trainingController in game.TrainingControllers) {
						foreach (var trainingModel in trainingController.TrainingModels)
							_trainingModels.Add(trainingModel);
					}
					matchEnded = true;
				});
				while (!matchEnded)
					System.Threading.Thread.Sleep (100);
				UpdateProgress(i+1,count);
			}
			CompositionRoot.Instance.ExecuteInMainThread(()=>{
				Debug.Log(count.ToString() + " AI matches ended");
				InitTrainers ();
				UpdateView();
				float meanScore = sumScore/(float)scoresCount;
				_meanScore.text = meanScore.ToString("###.00");
			});
		}));
		thread.Start ();
	}
	#endregion



	public void OnPlayNamedPlayersPressed() {
		/**const string human = "Human";
		List<string> names = new List<string> () { "AlanScoreValues", "StueRandom", "Rodriges", human };
		int[] wins = new int[4];
		int count = int.Parse (_gamesCountLabel.text);

		List<Player> players = new List<Player> ();
		foreach (string name in names) {
			if (name == human)
				players.Add (new HumanPlayer (_gameView.TurnView));
			else
				players.Add(PlayerSerializer.LoadPlayer(name));
		}

		Thread thread = new Thread (new ThreadStart(()=>{

			for (int i=0;i<count;i++) {
				UpdateProgress(i,count);
				Game game = new Game (players);
				game.Play (null);
				game.
			}
			UpdateProgress(count,count);

			Dictionary<string, int> meanRewards = new Dictionary<DecisionType, float>();
			foreach (var decision in decisions)
				meanRewards.Add(decision, sumRewards[decision]/(float)countRewards[decision]);

			CompositionRoot.Instance.ExecuteInMainThread(()=>{
				Debug.Log("Success calced");
				_successLabel.text = string.Format("wheretogo = {0:#0.##}\n humans count = {1:#0.##}\n instruments = {2:#0.##}\n charity = {3:#0.##}\n hungry = {4:#0.##}\n",
					meanRewards[DecisionType.SelectWhereToGo], meanRewards[DecisionType.SelectUsedHumans],
					meanRewards[DecisionType.SelectInstruments], meanRewards[DecisionType.SelectCharity], meanRewards[DecisionType.SelectLeaveHungry]);
				UpdateView();
			});
		}));
		thread.Start ();*/
	}

	List<TrainingDecisionModel> _trainingModels = new List<TrainingDecisionModel>();

	[SerializeField] Text _trainingModelsCount;
	public void UpdateView() {
		_trainingModelsCount.text = _trainingModels.Count.ToString ();
	}

	[SerializeField] GameObject _progressParent;
	[SerializeField] Slider _progress;
	[SerializeField] Text _progressText;
	public void UpdateProgress(int completedCount, int totalCount) {
		CompositionRoot.Instance.ExecuteInMainThread (() => {
			_progressParent.SetActive (completedCount < totalCount);
			float progress = completedCount / (float)totalCount;
			_progress.value = progress;
			_progressText.text = string.Format ("{0}/{1}={2:00}%", completedCount, totalCount, progress*100);
		});
	}

	[SerializeField] OneDeciderTrainingView _whereToGoTrainer;
	[SerializeField] OneDeciderTrainingView _humanCountTrainer;
	private void InitTrainers() {
		_whereToGoTrainer.Init (DecisionType.SelectWhereToGo, _trainingModels, _brain);
		_humanCountTrainer.Init (DecisionType.SelectUsedHumans, _trainingModels, _brain);
	}
}

