using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class NetworkTestView : MonoBehaviour {

	[SerializeField] RawImage _view;
	Texture2D _texture;
	const int Width = 100;
	const int Height = 100;
	// Use this for initialization
	void Start () {
		_texture = new Texture2D (Width, Height);
		Color32[] colors = _texture.GetPixels32 ();
		for (int i = 0; i < colors.Length; i++)
			colors [i] = new Color32 (0, 0, 0, 255);
		_texture.SetPixels32 (colors);
		_texture.Apply ();
		_view.texture = _texture;
	}
	
	// Update is called once per frame
	void Update () {
		SavePtIfPressed (0);
		SavePtIfPressed (1);
	}

	struct IntVec {
		public int X,Y;
	}
	List<IntVec> GoodPts = new List<IntVec>();
	List<IntVec> BadPts = new List<IntVec>();
	void SavePtIfPressed(int mouseButton) {
		if (!Input.GetMouseButtonUp (mouseButton))
			return;
		Vector2 localPoint;
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_view.rectTransform, Input.mousePosition, Camera.main, out localPoint))
			return;
		localPoint.x *= Width / _view.rectTransform.rect.width;
		localPoint.y *= Height / _view.rectTransform.rect.height;
		localPoint += new Vector2 (Width / 2, Height / 2);

		IntVec pt = new IntVec ();
		pt.X = Mathf.RoundToInt (localPoint.x);
		pt.Y = Mathf.RoundToInt (localPoint.y);
		Color col;
		if (mouseButton == 0) {
			GoodPts.Add (pt);
			col = Color.green;
		} else {
			BadPts.Add (pt);
			col = Color.red;
		}
		_texture.SetPixel (pt.X, pt.Y, col);
		_texture.Apply ();
	}
}
