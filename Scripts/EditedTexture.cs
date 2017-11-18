using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shiva.TextureEditor {
	public class EditedTexture : MonoBehaviour {
		private bool initFlag = false;

		public Texture orgTexture;
		public Texture editedTexture;

		public string textureFilePath;


		// Use this for initialization
		void Start () {
			
			Init ();
		}

		public void Restore(){
			TextureEditorManager master = FindObjectOfType<TextureEditorManager> ();

			MeshRenderer mr = GetComponent<MeshRenderer> ();
			Material mat = master.targetSharedMaterial ? mr.sharedMaterial : mr.material;

			mat.mainTexture = orgTexture;
			textureFilePath = null;
			editedTexture = null;
		}

		private void Init(){
			if (initFlag) {
				return;
			}
			initFlag = true;

			TextureEditorManager master = FindObjectOfType<TextureEditorManager> ();

			MeshRenderer mr = GetComponent<MeshRenderer> ();
			if (master.targetSharedMaterial) {
				orgTexture = mr.sharedMaterial.mainTexture;
			} else {
				orgTexture = mr.material.mainTexture;
			}

		}

		public void SetTextureTo(Material mat){
			if (editedTexture != null) {
				mat.mainTexture = editedTexture;
			} else {
				mat.mainTexture = orgTexture;
			}
		}

		public void Set(TextureEntry entry){
			Init ();

			TextureEditorManager master = FindObjectOfType<TextureEditorManager> ();

			MeshRenderer mr = GetComponent<MeshRenderer> ();
			entry.SetTextureTo (master.targetSharedMaterial ? mr.sharedMaterial : mr.material, orgTexture);

			textureFilePath = entry.TexturePath;
			editedTexture = entry.rawImage.texture;
		}

	}
}
