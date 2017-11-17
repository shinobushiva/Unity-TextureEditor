using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Shiva.TextureEditor {
	public class TextureEntry : MonoBehaviour {


		public RawImage rawImage;


		private string texturePath;

		public string TexturePath {
			get {
				return texturePath;
			}
			set {
				texturePath = value;
				StartCoroutine (Routine (this, texturePath));
			}
		}

		private Texture2D texture;

		private Texture2D Texture {
			get {
				return texture;
			}
			set {
				texture = value;
				rawImage.texture = value;
			}
		}

		private IEnumerator Routine(TextureEntry te, string filepath) {
			string uri = new Uri(filepath).AbsoluteUri;
			var loader = new WWW(uri);
			yield return loader;
			te.Texture = loader.texture;
		}


		void Reset(){
			rawImage = GetComponentInChildren<RawImage> ();
			GetComponentInChildren<Text> ().text = "";
		}

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
