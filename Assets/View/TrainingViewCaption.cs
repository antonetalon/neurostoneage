using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TrainingViewCaption : MonoBehaviour {
	[SerializeField] string _caption;
	[SerializeField] Text _captionText;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.name = _caption;
		_captionText.text = _caption;
	}
}
