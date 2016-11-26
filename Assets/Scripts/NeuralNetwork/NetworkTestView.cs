using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text;
using System;

public class NetworkTestView : MonoBehaviour {

	[SerializeField] RawImage _view;
	[SerializeField] GameView _gameView;
	Texture2D _texture;
	const int Width = 100;
	const int Height = 100;
	// Use this for initialization
	void Start () {
		_texture = new Texture2D (Width, Height);
		Color32[] colors = _texture.GetPixels32 ();
		for (int i = 0; i < colors.Length; i++)
			colors [i] = new Color32 (0, 0, 0, 255);
		_texture.SetPixels32 (colors);
		_texture.Apply ();
		_view.texture = _texture;
		_progressParent.SetActive (false);
	}


	AINeuralPlayer _brain;
	int _trainingsCount;

	#region Buttons
	public void OnSavePlayerPressed() {
		PlayerSerializer.SavePlayer ("Jorge", _brain);
	}
	public void CreateRandomBrainPressed() {
		//_brain = new NeuralNetwork (new int[4] { 2, 4, 4, 1 });
		_brain = new AINeuralPlayer();
		_trainingsCount = 0;
		Debug.Log ("Created ai");
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
			_brain = (AINeuralPlayer)binaryFormatter.Deserialize (stream);
		}
		UpdateView ();
		Debug.Log ("Training loaded");
	}

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
			});
		});}));
		thread.Start ();
	}

	[SerializeField] Dropdown _decisionSelector;
	public void OnTrainPressed() {
		int count = int.Parse (_gamesCountLabel.text);
		DecisionType decision = (DecisionType)(_decisionSelector.value+1);
		Train (count, decision, null);
	}
	private void Train(int loopsCount, DecisionType decisionType, System.Action onFinished) {
		const float speed = 0.5f;
		Thread thread = new Thread (new ThreadStart(()=>{
			for (int loopInd = 0; loopInd < loopsCount; loopInd++) {
				UpdateProgress (loopInd+1, loopsCount);
				NeuralPlayerTrainerController.DoTrainingLoop(decisionType, _trainingModels, speed, _brain);
			}
			CompositionRoot.Instance.ExecuteInMainThread (() => {
				UpdateView ();
				if (onFinished!=null)
					onFinished();
			});
		}));
		thread.Start ();
	}

	[SerializeField] Text _gamesCountLabel;
	public void PlayWithAIAndAddTraining() {
		int count = int.Parse (_gamesCountLabel.text);

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
					foreach (var trainingController in game.TrainingControllers) {
						foreach (var trainingModel in trainingController.TrainingModels)
							_trainingModels.Add(trainingModel);
					}
					matchEnded = true;
				});
				while (!matchEnded)
					System.Threading.Thread.Sleep (100);
			}
			CompositionRoot.Instance.ExecuteInMainThread(()=>{
				Debug.Log(count.ToString() + " AI matches ended");
				UpdateView();
			});
		}));
		thread.Start ();
	}

	[SerializeField] Text _successLabel;
	public void OnCalcSuccessPressed() {
		int count = int.Parse (_gamesCountLabel.text);

		List<Player> players = new List<Player> () {
			_brain,
			_brain.Clone(),
			_brain.Clone(),
			_brain.Clone()
		};

		Thread thread = new Thread (new ThreadStart(()=>{

			List<DecisionType> decisions = new List<DecisionType>() { DecisionType.SelectWhereToGo, DecisionType.SelectUsedHumans, DecisionType.SelectInstruments, DecisionType.SelectCharity, DecisionType.SelectLeaveHungry };

			Dictionary<DecisionType, float> sumRewards = new Dictionary<DecisionType, float>();
			Dictionary<DecisionType, int> countRewards = new Dictionary<DecisionType, int>();

			foreach (var decision in decisions) {
				sumRewards.Add(decision, 0);
				countRewards.Add(decision, 0);
			}

			System.Random rand = new System.Random();

			for (int i=0;i<count;i++) {
				UpdateProgress(i,count);
				players.Clear();
				for (int playerInd=0;playerInd<4;playerInd++) {
					Player player;
					if (rand.NextDouble()>0.5)
						player = _brain.Clone();
					else
						player = new AIRandomPlayer();
					players.Add(player);
				}
				Game game = new Game (players);
				game.Play (null);
				foreach (var trainingController in game.TrainingControllers) {
					for (int ind = 0;ind<4;ind++) {
						if (players[ind] is AINeuralPlayer) {
							var trainingModel = trainingController.TrainingModels[ind];
							sumRewards[trainingModel.Type] += trainingModel.RewardPercent;
							countRewards[trainingModel.Type]++;
						}
					}
				}
			}
			UpdateProgress(count,count);

			Dictionary<DecisionType, float> meanRewards = new Dictionary<DecisionType, float>();
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
		thread.Start ();
	}

	public void OnCleanTrainingPressed() {
		_trainingModels.Clear ();
		UpdateView ();
	}
	#endregion

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



}

