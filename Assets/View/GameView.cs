using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameView : MonoBehaviour {
	[SerializeField] BoardView _board;
	[SerializeField] List<PlayerView> _players;
	[SerializeField] GameObject _gameEndedParent;
	public HumanTurnView TurnView;
	Game _game;
	public void Init(Game game) {
		CompositionRoot.Instance.ExecuteInMainThread (() => {
			_game = game;
			_game.OnChanged += UpdateView;
			_game.OnTurnEnded += UpdateWinnerView;
			_gameEndedParent.SetActive (false);
		});
	}
	void Destroy() {
		_game.OnChanged -= UpdateView;
		_game.OnTurnEnded -= UpdateWinnerView;
	}
	void UpdateView() {
		CompositionRoot.Instance.ExecuteInMainThread (() => {
			_board.UpdateView (_game);
			for (int i = 0; i < _players.Count; i++)
				_players [i].UpdateView (_game.PlayerModels [i]);	
		});
	}
	void UpdateWinnerView() {
		CompositionRoot.Instance.ExecuteInMainThread (() => {
			if (_game.GetEnded ()) {
				// Show winner.
				PlayerModel player = null;
				int winnerInd = _game.WinnerInd;
				player = _game.PlayerModels [winnerInd];
				TurnView.SelectPlayer (_game, player);
				_gameEndedParent.SetActive (true);
			} else
				_gameEndedParent.SetActive (false);
		});
	}
	void Update() {
		if (_game != null)
			_game.Update ();
	}
	public void OnClosePressed() {
		gameObject.SetActive (false);
	}
}
