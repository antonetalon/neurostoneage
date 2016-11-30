using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;

public class ComparerView : MonoBehaviour {

	public void OnSavePressed() {
		PlayerSerializer.SaveComparer (_model);
	}
	public void OnEnable() {
		_itemViews = new List<ComparedPlayerView> ();
		_itemPrefab.gameObject.SetActive (false);
		_model = PlayerSerializer.LoadComparer ();
		UpdateView ();
	}
	[SerializeField] GameObject _detailsParent;
	public void ShowHideDetails() {
		_detailsParent.SetActive (!_detailsParent.activeSelf);
		UpdateView ();
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

	public void OnCalcSuccessPressed() {
		int[] wins = new int[4];
		const int GamesPerPlayer = 100;
		int count = _model.Items.Count * GamesPerPlayer / 4;
		System.Random rand = new System.Random ();

		Thread thread = new Thread (new ThreadStart(()=>{
			int[] gamesCount = new int[_model.Items.Count];
			int[] winsCount = new int[_model.Items.Count];
			for (int i=0;i<count;i++) {
				UpdateProgress(i,count);
				List<Player> players = new List<Player> ();
				int[] inds = new int[4];
				for (int j=0;j<4;j++) {
					inds[j] = rand.Next()%_model.Items.Count;
					Player player = _model.Items[inds[j]].Player.Clone();
					players.Add(player);
				}
				Game game = new Game (players);
				game.Play (null);
				// Get winner.
				winsCount[inds[game.WinnerInd]]++;
				for (int j=0;j<4;j++)
					gamesCount[inds[j]]++;
			}
			UpdateProgress(count,count);
			// Save win rate.
			for (int j=0;j<_model.Items.Count;j++) {
				float success = winsCount[j]/(float)gamesCount[j];
				if (gamesCount[j]==0)
					success = -1;
				_model.Items[j].AddSuccess(success);
			}
			CompositionRoot.Instance.ExecuteInMainThread (() => {
				UpdateView();
			});
		}));
		thread.Start ();
	}

	[SerializeField] ComparedPlayerView _itemPrefab;
	List<ComparedPlayerView> _itemViews;
	ComparerModel _model;
	private void UpdateView() {
		for (int i = 0; i < _itemViews.Count; i++)
			Destroy (_itemViews [i].gameObject);
		_itemViews.Clear ();
		for (int i = 0; i < _model.Items.Count; i++) {
			ComparedPlayerView item = Instantiate (_itemPrefab, _itemPrefab.transform.parent) as ComparedPlayerView;
			int removedInd = i;
			item.gameObject.SetActive (true);
			item.Init (_model.Items [i], () => {
				_model.Items.RemoveAt(removedInd);
				UpdateView();
			});
			_itemViews.Add (item);
		}
	}

	public void SavePlayer(Player player, string name) {
		ComparedPlayerModel model = new ComparedPlayerModel (player, name);
		_model.Items.Add (model);
		UpdateView ();
		OnSavePressed ();
	}
}
