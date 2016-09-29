using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.UI;
using System.Threading;

public class PopulationManager : MonoBehaviour {
	const int GamesPerPlayer = 10;
	const float DeathRate = 0.02f;
	const float ReproductivePercent = 0.1f;

	[Serializable]
	class PopulationItem {
		public AIGeneticPlayer Player;
		public int AverageScore;
		public int MaxScore;
	}
	[Serializable]
	class Population {
		public List<PopulationItem> Players;
		public int Generation;
		public Population(int count) {
			Generation = 0;
			Players = new  List<PopulationItem> ();
			for (int i = 0; i < count; i++) {
				var item = new PopulationItem();
				item.Player = AIGeneticPlayer.CreateRandom();
				Players.Add (item);
			}
		}
	}
	Population _population;
	public void CreatePressed() {
		const int count = 200;
		_population = new Population (count);
		UpdateView ();
		Debug.Log ("Population created");
	}
	const string FilePath = "SavedPopulation";
	public void Save() {
		using (Stream stream = File.Open(FilePath, FileMode.Create))
		{
			var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			binaryFormatter.Serialize(stream, _population);
		}
		Debug.Log ("Population saved");
	}
	public void Load() {
		using (Stream stream = File.Open(FilePath, FileMode.Open))
		{
			var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			_population = (Population)binaryFormatter.Deserialize(stream);
		}
		UpdateView ();
		Debug.Log ("Population loaded");
	}
	[SerializeField] Text _generation;
	[SerializeField] Text _bestAverage;
	[SerializeField] LineRenderer _lineAverage;
	[SerializeField] LineRenderer _lineMax;
	[SerializeField] Text _bestGameModel;
	private void UpdateView() {
		_generation.text = _population.Generation.ToString ();
		int max = int.MinValue;
		_lineAverage.SetVertexCount (_population.Players.Count);
		_lineMax.SetVertexCount (_population.Players.Count);
		const float maxX = 1000;
		float xScale = maxX / _population.Players.Count;
		for (int i = 0; i < _population.Players.Count; i++) {
			if (max < _population.Players [i].AverageScore)
				max = _population.Players [i].AverageScore;
			_lineAverage.SetPosition (i, new Vector3 (i*xScale, _population.Players[i].AverageScore, 0));
			_lineMax.SetPosition (i, new Vector3 (i*xScale, _population.Players[i].MaxScore, 0));
		}
		_bestAverage.text = max.ToString ();
		if (_bestGamePlayerModel != null)
			_bestGameModel.text = _bestGamePlayerModel.ToString ();
		else
			_bestGameModel.text = "no data";
	}

	[SerializeField] GameObject _isNotRunningParent;
	[SerializeField] GameObject _isRunningParent;
	[SerializeField] Text _generationsCountInput;
	public void RunPressed() {
		int count;
		if (int.TryParse (_generationsCountInput.text, out count))
			StartCoroutine (RunningGeneration (count));
		else
			StartCoroutine (RunningGeneration (1));
	}

	PlayerModel _bestGamePlayerModel;
	private IEnumerator RunningGeneration(int generationsCount) {
		for (int generationInd=0;generationInd<generationsCount;generationInd++) {
			Thread thread = new Thread (new ThreadStart(()=>{
				
				PlayGeneration(_population.Players, GamesPerPlayer, (maxScores, averageScores, model)=>{
					for (int i=0;i<_population.Players.Count;i++) {
						_population.Players[i].MaxScore = maxScores[i];
						_population.Players[i].AverageScore = averageScores[i];
						_bestGamePlayerModel = model;
					}
				});
			}));
			thread.Start ();
			while (thread.IsAlive)
				yield return new WaitForEndOfFrame ();
			_population.Generation++;

			_population.Players.Sort((player1, player2)=>{
				return player1.AverageScore.CompareTo(player2.AverageScore);
			});

			// Remove worst genes.
			int DeathCount = Mathf.RoundToInt(DeathRate*_population.Players.Count);
			for (int i=0;i<DeathCount;i++)
				_population.Players.RemoveAt(0);

			// Add best genes.
			int ReproductiveCount = Mathf.RoundToInt(ReproductivePercent*_population.Players.Count);
			for (int i=0;i<DeathCount;i++) {
				int parent1Ind = _population.Players.Count - UnityEngine.Random.Range(0, ReproductiveCount) - 1;
				int parent2Ind = _population.Players.Count - UnityEngine.Random.Range(0, ReproductiveCount) - 1;
				AIGeneticPlayer child = AIGeneticPlayer.CreateFromCrossover(_population.Players[parent1Ind].Player, _population.Players[parent2Ind].Player);
				PopulationItem childItem = new PopulationItem();
				childItem.Player = child;
				_population.Players.Insert(0, childItem);
			}
			UpdateView ();
		}
	}

	private void PlayGeneration(List<PopulationItem> players, int gamesPerPlayer, System.Action<List<int>, List<int>, PlayerModel> onComplete) {
		List<int> maxScores = new List<int> ();
		List<int> sumScores = new List<int> ();
		PlayerModel bestGameModel = null;
		for (int i = 0; i < players.Count; i++) {
			maxScores.Add (int.MinValue);
			sumScores.Add (0);
		}

		for (int i=0;i<gamesPerPlayer;i++) {
			List<AIGeneticPlayer> playerNotPlayedCurrGame = new List<AIGeneticPlayer> ();
			foreach (var player in players)
				playerNotPlayedCurrGame.Add (player.Player);
			while (playerNotPlayedCurrGame.Count>0) {
				List<Player> currGamePlayers = new List<Player> ();
				for (int j = 0; j < 4; j++) {
					int ind = Game.RandomRange(0, playerNotPlayedCurrGame.Count);
					currGamePlayers.Add (playerNotPlayedCurrGame [ind]);
					playerNotPlayedCurrGame.RemoveAt (ind);
				}
				Game game = new Game (currGamePlayers);
				bool processEnded = false;
				game.Play (()=>{ processEnded = true; });
				while (!processEnded)
					System.Threading.Thread.Sleep (1000);
				for (int j = 0; j < 4; j++) {
					int ind = -1;
					for (int k = 0; k < players.Count; k++) {
						if (currGamePlayers [j] == players [k].Player) {
							ind = k;
							break;
						}
					}
					int score = players [ind].Player.Model.Score;
					if (score > maxScores [ind])
						maxScores [ind] = score;
					if (bestGameModel == null || bestGameModel.Score < players [ind].Player.Model.Score)
						bestGameModel = players [ind].Player.Model;
					sumScores[ind] += score;
				}
				//Debug.LogFormat ("Game {0} played", (players.Count - playerNotPlayedCurrGame.Count)/4);
			}
			Debug.LogFormat ("Round {0} played", i);
		}

		int maxMaxScore = int.MinValue;
		int maxSumScore = 0;
		int bestPlayerInd = -1;
		for (int i = 0; i < players.Count; i++) {
			if (maxScores [i] > maxMaxScore)
				maxMaxScore = maxScores [i];
			if (maxSumScore < sumScores [i]) {
				maxSumScore = sumScores [i];
				bestPlayerInd = i;
			}
			sumScores [i] /= gamesPerPlayer;
		}

		Debug.LogFormat("Max score = {0}, max average score = {1}", maxMaxScore, maxSumScore/gamesPerPlayer);
		onComplete (maxScores, sumScores, bestGameModel);
	}

	[SerializeField] GameView _view;
	public void PlayWithBestPressed() {
		int bestAverage = int.MinValue;
		int firstBest = -1;
		int secondBest = -1;
		int thirdBest = -1;
		for (int i = 0; i < _population.Players.Count; i++) {
			if (bestAverage < _population.Players[i].AverageScore) {
				bestAverage = _population.Players[i].AverageScore;
				thirdBest = secondBest;
				secondBest = firstBest;
				firstBest = i;
			}
		}

		List<Player> players = new List<Player> () {
			new HumanPlayer (_view.TurnView),
			_population.Players[firstBest].Player,
			_population.Players[secondBest].Player,
			_population.Players[thirdBest].Player
		};
		_view.gameObject.SetActive (true);
		Game game = new Game (players);
		_view.Init (game);
		Thread thread = new Thread (new ThreadStart(()=>{game.Play (()=>{
			CompositionRoot.Instance.ExecuteInMainThread(()=>{
				//_view.gameObject.SetActive (false);
				Debug.Log("Match ended");
			});
		});}));
		thread.Start ();
	}
}
