using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class CompositionRoot : MonoBehaviour {
	private Object thisLock = new Object();
	private List<System.Action> _executeAtMainThread = new List<System.Action>();
	public void ExecuteInMainThread(System.Action action) {
		lock (thisLock) {
			_executeAtMainThread.Add (action);
		}
	}
	void Update() {
		for (int i = 0; i < _executeAtMainThread.Count; i++) {
			if (_executeAtMainThread [i]!=null)
				_executeAtMainThread [i] ();
		}
		_executeAtMainThread.Clear ();
	}

	[SerializeField] GameView _view;
	Game _game;
	public static CompositionRoot Instance { get; private set; }
	void Awake() {
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		//PlayMatch ();
		//PlayGenerationAndMatchWithBest();
	}
	void PlayMatch() {
		List<Player> players = new List<Player> () {
			new HumanPlayer (_view.TurnView),
			AIGeneticPlayer.CreateRandom(),
			AIGeneticPlayer.CreateRandom(),
			new AIRandomPlayer()
		};
		_game = new Game (players);
		_view.Init (_game);
		Thread thread = new Thread (new ThreadStart(()=>{_game.Play (()=>{
			Debug.Log("Match ended");
		});}));
		thread.Start ();
	}
	/*void PlayGenerationAndMatchWithBest() {
		Thread thread = new Thread (new ThreadStart(()=>{
			List<Player> players = new List<Player> ();
			const int GamesPerPlayer = 3;
			const int PopulationSize = 200;
			for (int i = 0; i < PopulationSize; i++) {
				AIGeneticPlayer player = new AIGeneticPlayer ();
				players.Add (player);
			}
			int bestPlayerInd = -1;
			PlayGeneration(players, GamesPerPlayer, (ind)=>{
				bestPlayerInd = ind;

				List<Player> currGamePlayers = new List<Player> () {
					new HumanPlayer (_view.TurnView),
					new AIGeneticPlayer(),
					new AIGeneticPlayer(),
					players[bestPlayerInd]
				};

				_game = new Game (currGamePlayers);
				_view.Init (_game);
				_game.Play (()=>{
					Debug.Log("Game ended");
				});
			});
		}));
		thread.Start ();
	}*/


}
