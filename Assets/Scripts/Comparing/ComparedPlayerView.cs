using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ComparedPlayerView : MonoBehaviour {

	[SerializeField] List<Text> _successLabels;
	[SerializeField] Text _name;
	[SerializeField] InputField _comment;

	ComparedPlayerModel _model;
	Action _onRemovePressed; 
	public void Init(ComparedPlayerModel model, Action onRemovePressed) {
		_model = model;
		_onRemovePressed = onRemovePressed;
		UpdateView ();
	}

	public void OnRemovePressed() {
		_onRemovePressed ();
	}
	public void OnCommentChanged(string text) {
		_model.Comment = _comment.text;
	}
	public void UpdateView() {
		for (int i = 0; i < _successLabels.Count; i++) {
			if (_model.Success.Count <= i)
				break;
			_successLabels [i].text = (_model.Success [_model.Success.Count - i - 1]).ToString ("##%");
		}
		_name.text = _model.Name;
		_comment.text = _model.Comment;
	}
}
