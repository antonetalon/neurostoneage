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
	AINeuralPlayer _prevBrain;
	int _trainingsCount;

	#region Buttons
	[SerializeField] ComparerView _savedModelsComparer;
	public void OnSavePlayerPressed() {
		_savedModelsComparer.SavePlayer (_brain, "Jorge");
		//PlayerSerializer.SavePlayer ("Jorge", _brain);
	}
	public void CreateRandomBrainPressed() {
		//_brain = new NeuralNetwork (new int[4] { 2, 4, 4, 1 });
		_brain = new AINeuralPlayer();
		_prevBrain = (AINeuralPlayer)_brain.Clone ();
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
			_prevBrain = (AINeuralPlayer)_brain.Clone ();
		}
		UpdateView ();
		Debug.Log ("Training loaded");

		List<float> cardValuesNoRes = new List<float> ();
		List<float> cardValuesResPres = new List<float> ();
		foreach (var training in _trainingModels) {
			if (training.Type != DecisionType.SelectWhereToGo)
				continue;
			WhereToGo target = (WhereToGo)(training.Output+1);
			if (target != WhereToGo.House1 && target != WhereToGo.House2 && target != WhereToGo.House3 && target != WhereToGo.House4)
				continue;
			int ind = training.Output - 3;
			if (training.Inputs [ind] > 0.5f)
				cardValuesResPres.Add (training.RewardPercent);
			else
				cardValuesNoRes.Add (training.RewardPercent);
		}
		StringBuilder sb = new StringBuilder ();
		sb.AppendFormat ("has res = ");
		foreach (float val in cardValuesResPres)
			sb.AppendFormat ("{0:#0.##}, ", val);
		sb.AppendFormat ("\nno  res = ");
		foreach (float val in cardValuesNoRes)
			sb.AppendFormat ("{0:#0.##}, ", val);
		Debug.Log (sb.ToString());

		/*StringBuilder sb = new StringBuilder ();
		Dictionary<int, List<float>> values = new Dictionary<int, List<float>> ();
		foreach (var training in _trainingModels) {
			if (training.Type != DecisionType.SelectCharity)
				continue;
			if (!values.ContainsKey (training.Output))
				values.Add (training.Output, new List<float> ());
			values [training.Output].Add (training.RewardPercent);
		}
		foreach (var item in values)
			item.Value.Sort ();
		foreach (var item in values) {
			sb.AppendFormat ("{0} has values of\n", item.Key);
			foreach (float val in item.Value)
				sb.AppendFormat ("{0:#0.##}, ", val);
			sb.AppendLine ();
		}
		Debug.Log (sb.ToString());*/
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
		int count = int.Parse (_gamesCountInput.text);
		DecisionType decision = (DecisionType)(_decisionSelector.value+1);
		Train (count, decision, null);
	}
	private void Train(int loopsCount, DecisionType decisionType, System.Action onFinished) {
		_prevBrain = (AINeuralPlayer)_brain.Clone ();
		float speed = float.Parse (_learningSpeedInput.text);
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

	[SerializeField] InputField _gamesCountInput;
	[SerializeField] InputField _learningSpeedInput;
	public void OnPlayWithAIAndAddTraining() {
		int count = int.Parse (_gamesCountInput.text);

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
				UpdateProgress(i+1,count);
			}
			CompositionRoot.Instance.ExecuteInMainThread(()=>{
				Debug.Log(count.ToString() + " AI matches ended");
				UpdateView();
			});
		}));
		thread.Start ();
	}

	[SerializeField] Text _successLabel;
	[SerializeField] Text _changeSignLabel;
	public void OnCalcSuccessPressed () {
		CalcSuccess ();
	}
	private void CalcSuccess() {

		List<DecisionType> decisions = new List<DecisionType>() { DecisionType.SelectWhereToGo, DecisionType.SelectUsedHumans, DecisionType.SelectInstruments, DecisionType.SelectCharity, DecisionType.SelectLeaveHungry };

		Dictionary<DecisionType, float> errors = new Dictionary<DecisionType, float>();
		Dictionary<DecisionType, int> counts = new Dictionary<DecisionType, int>();
		Dictionary<DecisionType, int> changeSignCounts = new Dictionary<DecisionType, int>();

		foreach (var decision in decisions) {
			errors.Add(decision, 0);
			counts.Add(decision, 0);
			changeSignCounts.Add (decision, 0);
		}

		foreach (TrainingDecisionModel training in _trainingModels) {			
			double output = _brain.GetDecider (training.Type).Think (training.Inputs)[training.Output];
			double prevOutput = _prevBrain.GetDecider (training.Type).Think (training.Inputs)[training.Output];
			errors[training.Type] += Mathf.Abs( training.RewardPercent - (float)output);
			counts[training.Type]++;
			if ((output - training.RewardPercent) * (prevOutput - training.RewardPercent) < 0)
				changeSignCounts [training.Type]++;
		}

		Dictionary<DecisionType, float> meanErrors = new Dictionary<DecisionType, float>();
		foreach (var decision in decisions)
			meanErrors.Add(decision, errors[decision]/(float)counts[decision]);
		
		Debug.Log("Success calced");
		_successLabel.text = string.Format("Errors:\nwheretogo = {0:#0.####}\n humans count = {1:#0.####}\n charity = {3:#0.####}\n instruments = {2:#0.####}\n hungry = {4:#0.####}\n",
			meanErrors[DecisionType.SelectWhereToGo], meanErrors[DecisionType.SelectUsedHumans],
			meanErrors[DecisionType.SelectInstruments], meanErrors[DecisionType.SelectCharity], meanErrors[DecisionType.SelectLeaveHungry]);

		_changeSignLabel.text = string.Format("change signs:\nwheretogo = {0:#0.####}\n humans count = {1:#0.####}\n charity = {3:#0.####}\n instruments = {2:#0.####}\n hungry = {4:#0.####}\n",
			changeSignCounts[DecisionType.SelectWhereToGo]/(float)counts[DecisionType.SelectWhereToGo], changeSignCounts[DecisionType.SelectUsedHumans]/(float)counts[DecisionType.SelectUsedHumans],
			changeSignCounts[DecisionType.SelectInstruments]/(float)counts[DecisionType.SelectInstruments], changeSignCounts[DecisionType.SelectCharity]/(float)counts[DecisionType.SelectCharity],
			changeSignCounts[DecisionType.SelectLeaveHungry]/(float)counts[DecisionType.SelectLeaveHungry]);
	}

	public void OnCleanTrainingPressed() {
		/*int ind = -1;
		for (int i = 0; i < _trainingModels.Count; i++) {
			if (_trainingModels [i].Type == DecisionType.SelectCharity) {
				ind = i;
				break;
			}
		}
		var model = _trainingModels [ind];
		_trainingModels.Clear ();
		_trainingModels.Add (model);*/

		_trainingModels.Clear ();
		UpdateView ();
	}

	public void OnRevertToPrevPressed() {
		_brain = (AINeuralPlayer)_prevBrain.Clone ();
		UpdateView ();
	}

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
	#endregion

	List<TrainingDecisionModel> _trainingModels = new List<TrainingDecisionModel>();

	[SerializeField] Text _trainingModelsCount;
	public void UpdateView() {
		CalcSuccess ();
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

