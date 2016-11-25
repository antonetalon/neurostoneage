using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text;

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

	public void OnTestPressed() {
		StringBuilder sb = new StringBuilder ();
		// Find go to food wanting with 0 food.
		NeuralNetwork decider = _brain.GetDecider (DecisionType.SelectWhereToGo);
		int[] inputs = GetWhereToGoInputs (0);
		double[] outputs = decider.Think (inputs);
		sb.AppendLine ("0 food wanting = " + outputs[3].ToString());
		// Find go to food wanting with 1000 food.
		inputs = GetWhereToGoInputs (1000);
		outputs = decider.Think (inputs);
		sb.AppendLine ("1000 food wanting = " + outputs[3].ToString());
		// Train.
		_whereToGoTrainingView.Train(()=>{
			// Find go to food wanting with 0 food.
			decider = _brain.GetDecider (DecisionType.SelectWhereToGo);
			inputs = GetWhereToGoInputs (0);
			outputs = decider.Think (inputs);
			sb.AppendLine ("0 food wanting = " + outputs[3].ToString());
			// Find go to food wanting with 1000 food.
			inputs = GetWhereToGoInputs (1000);
			outputs = decider.Think (inputs);
			sb.AppendLine ("1000 food wanting = " + outputs[3].ToString());
		});
	}


	private static int[] GetWhereToGoInputs(int food) {
		int[] inputs = new int[81];
		int i = 0;
		inputs [i] = 0;i++;//game.TurnInd; i++;
		inputs [i] = food;i++;//player.Food; i++;
		inputs [i] = 0;i++;//player.Forest; i++;
		inputs [i] = 0;i++;//player.Stone; i++;
		inputs [i] = 0;i++;//player.Gold; i++;
		inputs [i] = 0;i++;//Indicator(Game.EnoughResourcesForBuilding (game, player, 0)); i++;
		inputs [i] = 0;i++;//Indicator(Game.EnoughResourcesForBuilding (game, player, 1)); i++;
		inputs [i] = 0;i++;//Indicator(Game.EnoughResourcesForBuilding (game, player, 2)); i++;
		inputs [i] = 0;i++;//Indicator(Game.EnoughResourcesForBuilding (game, player, 3)); i++;
		inputs [i] = 5;i++;//player.HumansCount; i++;
		inputs [i] = 0;i++;//player.FieldsCount; i++;
		inputs [i] = 0;i++;//player.InstrumentsCountSlot1+player.InstrumentsCountSlot2+player.InstrumentsCountSlot3; i++;
		inputs [i] = 1;i++;//player.HouseMultiplier; i++;
		inputs [i] = 1;i++;//player.FieldsMultiplier; i++;
		inputs [i] = 1;i++;//player.InstrumentsMultiplier; i++;
		inputs [i] = 0;i++;//player.GetScienceScore(0); i++;
		inputs [i] = 100;i++;//player.Score; i++;

		// Card i not owned science exists.
		inputs [i] = 0;i++;//Indicator( cards [0].BottomFeature == BottomCardFeature.Science && player.ScienceExists ((Science)cards [0].BottomFeatureParam, 0) ); i++;
		inputs [i] = 0;i++;//Indicator( cards [1].BottomFeature == BottomCardFeature.Science && player.ScienceExists ((Science)cards [1].BottomFeatureParam, 0) ); i++;
		inputs [i] = 0;i++;//Indicator( cards [2].BottomFeature == BottomCardFeature.Science && player.ScienceExists ((Science)cards [2].BottomFeatureParam, 0) ); i++;
		inputs [i] = 0;i++;//Indicator( cards [3].BottomFeature == BottomCardFeature.Science && player.ScienceExists ((Science)cards [3].BottomFeatureParam, 0) ); i++;
		// Card i science score addition for row 0.
		inputs [i] = 0;i++;//inputs [i-4]>0.5f?player.GetSciencesCount(0)*player.GetSciencesCount(0):0; i++;
		inputs [i] = 0;i++;//inputs [i-4]>0.5f?player.GetSciencesCount(0)*player.GetSciencesCount(0):0; i++;
		inputs [i] = 0;i++;//inputs [i-4]>0.5f?player.GetSciencesCount(0)*player.GetSciencesCount(0):0; i++;
		inputs [i] = 0;i++;//inputs [i-4]>0.5f?player.GetSciencesCount(0)*player.GetSciencesCount(0):0; i++;
		// Card i houses, fields, humans, instruments multipliers.
		inputs [i] = 0;i++;//cards[0].BottomFeature == BottomCardFeature.FieldMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[0].BottomFeature == BottomCardFeature.HouseMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[0].BottomFeature == BottomCardFeature.HumanMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[0].BottomFeature == BottomCardFeature.InstrumentsMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[1].BottomFeature == BottomCardFeature.FieldMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[1].BottomFeature == BottomCardFeature.HouseMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[1].BottomFeature == BottomCardFeature.HumanMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[1].BottomFeature == BottomCardFeature.InstrumentsMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[2].BottomFeature == BottomCardFeature.FieldMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[2].BottomFeature == BottomCardFeature.HouseMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[2].BottomFeature == BottomCardFeature.HumanMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[2].BottomFeature == BottomCardFeature.InstrumentsMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[3].BottomFeature == BottomCardFeature.FieldMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[3].BottomFeature == BottomCardFeature.HouseMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[3].BottomFeature == BottomCardFeature.HumanMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		inputs [i] = 0;i++;//cards[3].BottomFeature == BottomCardFeature.InstrumentsMultiplier ? cards[0].BottomFeatureParam : 0; i++;
		// Card i has charity.
		inputs [i] = 0;i++;//Indicator( cards[0].TopFeature == TopCardFeature.RandomForEveryone ); i++;
		inputs [i] = 0;i++;//Indicator( cards[1].TopFeature == TopCardFeature.RandomForEveryone ); i++;
		inputs [i] = 0;i++;//Indicator( cards[2].TopFeature == TopCardFeature.RandomForEveryone ); i++;
		inputs [i] = 0;i++;//Indicator( cards[3].TopFeature == TopCardFeature.RandomForEveryone ); i++;
		// Card i forest, clay, stone, gold amount - random and const aggregated.
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[0],Resource.Food); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[0],Resource.Forest); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[0],Resource.Clay); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[0],Resource.Stone); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[0],Resource.Gold); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[1],Resource.Food); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[1],Resource.Forest); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[1],Resource.Clay); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[1],Resource.Stone); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[1],Resource.Gold); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[2],Resource.Food); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[2],Resource.Forest); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[2],Resource.Clay); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[2],Resource.Stone); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[2],Resource.Gold); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[3],Resource.Food); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[3],Resource.Forest); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[3],Resource.Clay); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[3],Resource.Stone); i++;
		inputs [i] = 0;i++;//GetResourceExpectedCount(cards[3],Resource.Gold); i++;
		// Card i instruments and once instruments.
		inputs [i] = 0;i++;//cards[0].TopFeature == TopCardFeature.InstrumentsForever? 1:0; i++;
		inputs [i] = 0;i++;//cards[0].TopFeature == TopCardFeature.InstrumentsOnce? cards[0].TopFeatureParam:0; i++;
		inputs [i] = 0;i++;//cards[1].TopFeature == TopCardFeature.InstrumentsForever? 1:0; i++;
		inputs [i] = 0;i++;//cards[1].TopFeature == TopCardFeature.InstrumentsOnce? cards[1].TopFeatureParam:0; i++;
		inputs [i] = 0;i++;//cards[2].TopFeature == TopCardFeature.InstrumentsForever? 1:0; i++;
		inputs [i] = 0;i++;//cards[2].TopFeature == TopCardFeature.InstrumentsOnce? cards[2].TopFeatureParam:0; i++;
		inputs [i] = 0;i++;//cards[3].TopFeature == TopCardFeature.InstrumentsForever? 1:0; i++;
		inputs [i] = 0;i++;//cards[3].TopFeature == TopCardFeature.InstrumentsOnce? cards[3].TopFeatureParam:0; i++;
		// House i min, max score.
		inputs [i] = 0;i++;//GetHouseMaxScore(game.GetHouse(0)); i++;
		inputs [i] = 0;i++;//GetHouseMinScore(game.GetHouse(0)); i++;
		inputs [i] = 0;i++;//GetHouseMaxScore(game.GetHouse(1)); i++;
		inputs [i] = 0;i++;//GetHouseMinScore(game.GetHouse(1)); i++;
		inputs [i] = 0;i++;//GetHouseMaxScore(game.GetHouse(2)); i++;
		inputs [i] = 0;i++;//GetHouseMinScore(game.GetHouse(2)); i++;
		inputs [i] = 0;i++;//GetHouseMaxScore(game.GetHouse(3)); i++;
		inputs [i] = 0;i++;//GetHouseMinScore(game.GetHouse(3)); i++;
		return inputs;
	}
}

