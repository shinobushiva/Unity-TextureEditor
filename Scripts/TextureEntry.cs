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
				if (texturePath != null) {
					GlobalCoroutine.Go(Routine (this, texturePath));
				}
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
				rawImage.color = Color.white;
			}
		}

		private IEnumerator Routine(TextureEntry te, string filepath) {

			print ("Set Texture Routine");
			
			string uri = new Uri(filepath).AbsoluteUri;
			var loader = new WWW(uri);
			yield return loader;
			print ("Set Texture");
			te.Texture = loader.texture;
		}

		public void SetTextureTo(Material mat, Texture org){
			if (texture != null) {
				mat.mainTexture = rawImage.texture;
			} else {
				mat.mainTexture = org;
			}
		}


		void Reset(){
			rawImage = GetComponentInChildren<RawImage> (true);
			GetComponentInChildren<Text> (true).text = "";
		}

		// Use this for initialization
		void Start () {
			Reset ();
		}

	}
}
