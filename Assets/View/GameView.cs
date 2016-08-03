using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameView : MonoBehaviour {
	[SerializeField] BoardView _board;
	[SerializeField] List<PlayerView> _players;
	Game _game;
	public void Init(Game game) {
		_game = game;
		_game.OnChanged += UpdateView;
	}
	void Destroy() {
		_game.OnChanged -= UpdateView;
	}
	void UpdateView() {
		_board.UpdateView (_game);
		for (int i = 0; i < _players.Count; i++)
			_players [i].UpdateView (_game.PlayerModels [i]);			
	}
}
