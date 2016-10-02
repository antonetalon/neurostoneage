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
		if (Mathf.Abs (localPoint.x) > Width / 2 || Mathf.Abs (localPoint.y) > Height / 2)
			return;
		localPoint += new Vector2 (Width / 2, Height / 2);

		IntVec pt = new IntVec ();
		pt.X = Mathf.RoundToInt (localPoint.x);
		pt.Y = Height - 1 - Mathf.RoundToInt (localPoint.y);
		Color col;
		if (mouseButton == 0) {
			GoodPts.Add (pt);
			col = Color.green;
		} else {
			BadPts.Add (pt);
			col = Color.red;
		}
		_texture.SetPixel (pt.X, Height - pt.Y - 1, col);
		_texture.Apply ();
	}

	public void OnShowRandomResultPressed() {
		NeuralNetwork brain = new NeuralNetwork (new int[4] { 2, 4, 4, 2 });
		Color32[] colors = _texture.GetPixels32 ();
		double[] input = new double[2];
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				input [0] = x;
				input [1] = y;
				double[] res = brain.Think (input);
				//Color32 col = new Color32((byte)(x*255/Width), (byte)(y*255/Height),0,255); // - test
				Color32 col = res[0] > res[1] ? new Color32 (120, 0, 0, 255) : new Color32 (0, 120, 0, 255);
				foreach (var coo in GoodPts) {
					if (x == coo.X && y == coo.Y) {
						col = new Color32 (0, 255, 0, 255);
						break;
					}
				}
				foreach (var coo in BadPts) {
					if (x == coo.X && y == coo.Y) {
						col = new Color32 (255, 0, 0, 255);
						break;
					}
				}
				colors [x + (Width - y - 1) * Width] = col;
			}
		}
		_texture.SetPixels32 (colors);
		_texture.Apply ();
	}
}
