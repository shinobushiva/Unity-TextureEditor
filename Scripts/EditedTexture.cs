using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shiva.TextureEditor {
	public class EditedTexture : MonoBehaviour {

		private  Texture orgTexture;

		public Texture editedTexture;
	//	private string textureFilePath;

		private TextureEditorManager master;

		// Use this for initialization
		void Start () {
			master = FindObjectOfType<TextureEditorManager> ();
			
			MeshRenderer mr = GetComponent<MeshRenderer> ();
			if (master.targetSharedMaterial) {
				orgTexture = mr.sharedMaterial.mainTexture;
			} else {
				orgTexture = mr.material.mainTexture;
			}
		}


		// Update is called once per frame
		void Update () {
			
		}

		public void Set(Texture tex){
			
			MeshRenderer mr = GetComponent<MeshRenderer> ();

			editedTexture = tex;
			if (master.targetSharedMaterial) {
				mr.sharedMaterial.mainTexture = editedTexture;
			} else {
				mr.material.mainTexture = editedTexture;
			}
		}

	}
}
