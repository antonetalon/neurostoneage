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
		List<Player> players = new List<Player> () {
			new HumanPlayer (_view.TurnView),
			new AIPlayer(),
			new AIPlayer(),
			new AIPlayer()
		};
		_game = new Game (players);
		_view.Init (_game);
		StartCoroutine (_game.Play ());

	}

}
