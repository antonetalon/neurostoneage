using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;
using System.IO;

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
	
	// Update is called once per frame
	void Update () {
		SavePtIfPressed (0);
		SavePtIfPressed (1);
	}

	struct IntVec {
		public int X,Y;
	}
	List<IntVec> GoodPts = new List<IntVec>();
	List<IntVec> BadPts = new List<IntVec>();
	void SavePtIfPressed(int mouseButton) {
		if (!Input.GetMouseButtonUp (mouseButton))
			return;
		Vector2 localPoint;
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_view.rectTransform, Input.mousePosition, Camera.main, out localPoint))
			return;		
		localPoint.x *= Width / _view.rectTransform.rect.width;
		localPoint.y *= Height / _view.rectTransform.rect.height;
		if (Mathf.Abs (localPoint.x) > Width / 2 || Mathf.Abs (localPoint.y) > Height / 2)
			return;
		localPoint += new Vector2 (Width / 2, Height / 2);

		IntVec pt = new IntVec ();
		pt.X = Mathf.RoundToInt (localPoint.x);
		pt.Y = Height - 1 - Mathf.RoundToInt (localPoint.y);
		Color col;
		if (mouseButton == 0) {
			GoodPts.Add (pt);
			col = Color.green;
		} else {
			BadPts.Add (pt);
			col = Color.red;
		}
		_texture.SetPixel (pt.X, Height - pt.Y - 1, col);
		_texture.Apply ();
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
		InitTrainingViews ();
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
		InitTrainingViews ();
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
			});
		});}));
		thread.Start ();
	}
	#endregion

	List<TrainingDecisionModel> _trainingModels = new List<TrainingDecisionModel>();

	[SerializeField] Text _trainingModelsCount;
	[SerializeField] OneDeciderTrainingView _whereToGoTrainingView;
	[SerializeField] OneDeciderTrainingView _usedHumansTrainingView;
	[SerializeField] OneDeciderTrainingView _charityTrainingView;
	[SerializeField] OneDeciderTrainingView _instrumentsTrainingView;
	[SerializeField] OneDeciderTrainingView _hungryTrainingView;
	void InitTrainingViews() {
		_whereToGoTrainingView.Init (DecisionType.SelectWhereToGo, _trainingModels, _brain);
		_usedHumansTrainingView.Init (DecisionType.SelectUsedHumans, _trainingModels, _brain);
		_charityTrainingView.Init (DecisionType.SelectCharity, _trainingModels, _brain);
		_instrumentsTrainingView.Init (DecisionType.SelectInstruments, _trainingModels, _brain);
		_hungryTrainingView.Init (DecisionType.SelectLeaveHungry, _trainingModels, _brain);
	}
	public void UpdateView() {
		_trainingModelsCount.text = _trainingModels.Count.ToString ();
		_whereToGoTrainingView.UpdateView ();
		_usedHumansTrainingView.UpdateView ();
		_charityTrainingView.UpdateView ();
		_instrumentsTrainingView.UpdateView ();
		_hungryTrainingView.UpdateView ();
	}

	[SerializeField] GameObject _progressParent;
	[SerializeField] Slider _progress;
	[SerializeField] Text _progressText;
	public void UpdateProgress(int completedCount, int totalCount) {
		CompositionRoot.Instance.ExecuteInMainThread (() => {
			_progressParent.SetActive (completedCount < totalCount);
			float progress = completedCount / (float)totalCount;
			_progress.value = progress;
			_progressText.text = string.Format ("{0}/{1}={2}", completedCount, totalCount, progress);
		});
	}

	private float _prevProgress;
	public void OnCleanTrainingPressed() {
		_trainingModels.Clear ();
		InitTrainingViews ();
		UpdateView ();
	}
}

