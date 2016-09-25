using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CompositionRoot : MonoBehaviour {

	[SerializeField] GameView _view;
	Game _game;
	public static CompositionRoot Instance { get; private set; }
	void Awake() {
		Instance = this;
	}

	// Use this for initialization
	void Start () {

		/*List<Player> players = new List<Player> () {
			new HumanPlayer (_view.TurnView),
			new AIGeneticPlayer(),
			new AIGeneticPlayer(),
			new AIRandomPlayer()
		};
		_game = new Game (players);
		_view.Init (_game);
		StartCoroutine (_game.Play ());

*/
		List<Player> players = new List<Player> ();
		const int GamesPerPlayer = 100;
		const int PopulationSize = 1000;
		for (int i = 0; i < PopulationSize; i++) {
			AIGeneticPlayer player = new AIGeneticPlayer ();
			players.Add (player);
		}
		int bestPlayerInd = -1;
		StartCoroutine (PlayGeneration(players, GamesPerPlayer, (ind)=>{
			bestPlayerInd = ind;
		
			List<Player> currGamePlayers = new List<Player> () {
				new HumanPlayer (_view.TurnView),
				new AIGeneticPlayer(),
				new AIGeneticPlayer(),
				players[bestPlayerInd]
			};
			_game = new Game (currGamePlayers);
			_view.Init (_game);
			StartCoroutine (_game.Play ());
		}));
	}

	public IEnumerator PlayGeneration(List<Player> players, int gamesPerPlayer, System.Action<int> onComplete) {
		List<int> maxScore = new List<int> ();
		List<int> sumScore = new List<int> ();
		for (int i = 0; i < players.Count; i++) {
			maxScore.Add (int.MinValue);
			sumScore.Add (0);
		}

		for (int i=0;i<gamesPerPlayer;i++) {
			List<Player> playerNotPlayedCurrGame = new List<Player> ();
			foreach (var player in players)
				playerNotPlayedCurrGame.Add (player);
			while (playerNotPlayedCurrGame.Count>0) {
				List<Player> currGamePlayers = new List<Player> ();
				for (int j = 0; j < 4; j++) {
					int ind = Random.Range (0, playerNotPlayedCurrGame.Count + 1);
					currGamePlayers.Add (playerNotPlayedCurrGame [ind]);
					playerNotPlayedCurrGame.RemoveAt (ind);
				}
				Game game = new Game (currGamePlayers);
				yield return CompositionRoot.Instance.StartCoroutine (game.Play ());
				for (int j = 0; j < 4; j++) {
					int ind = players.IndexOf (currGamePlayers [j]);
					int score = players [ind].Model.Score;
					if (score > maxScore [ind])
						maxScore [ind] = score;
					sumScore[ind] += score;
				}
			}
			Debug.LogFormat ("Round {0} playerd", i);
		}

		int maxMaxScore = int.MinValue;
		int maxSumScore = 0;
		int bestPlayerInd = -1;
		for (int i = 0; i < players.Count; i++) {
			if (maxScore [i] > maxMaxScore)
				maxMaxScore = maxScore [i];
			if (maxSumScore < sumScore [i]) {
				maxSumScore = sumScore [i];
				bestPlayerInd = i;
			}
		}

		Debug.LogFormat("Max score = {0}, max average score = {1}", maxMaxScore, maxSumScore/gamesPerPlayer);
		yield return new WaitForEndOfFrame ();
		onComplete (bestPlayerInd);
	}
}
