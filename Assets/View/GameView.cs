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
		_game = game;
		_game.OnChanged += UpdateView;
		_game.OnTurnEnded += UpdateWinnerView;
		_gameEndedParent.SetActive (false);
	}
	void Destroy() {
		_game.OnChanged -= UpdateView;
		_game.OnTurnEnded -= UpdateWinnerView;
	}
	void UpdateView() {
		_board.UpdateView (_game);
		for (int i = 0; i < _players.Count; i++)
			_players [i].UpdateView (_game.PlayerModels [i]);	
	}
	void UpdateWinnerView() {
		
		if (_game.GetEnded ()) {
			// Show winner.
			PlayerModel player = null;
			int winnerInd = -1;
			if (_game.PlayerModels [0].Score >= _game.PlayerModels [1].Score && _game.PlayerModels [0].Score >= _game.PlayerModels [2].Score && _game.PlayerModels [0].Score >= _game.PlayerModels [3].Score)
				winnerInd = 0;
			else if (_game.PlayerModels [1].Score >= _game.PlayerModels [2].Score && _game.PlayerModels [1].Score >= _game.PlayerModels [3].Score)
				winnerInd = 1;
			else if (_game.PlayerModels [2].Score >= _game.PlayerModels [3].Score)
				winnerInd = 2;
			else
				winnerInd = 3;
			player = _game.PlayerModels [winnerInd];
			TurnView.SelectPlayer(_game, player);
			_gameEndedParent.SetActive (true);
		} else
			_gameEndedParent.SetActive (false);
	}
}
