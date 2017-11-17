using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DataSaveLoad;
using System;
using System.IO;
using SFB;

// This uses UnityStandaloneFileBrowser
// https://github.com/gkngkc/UnityStandaloneFileBrowser
namespace Shiva.TextureEditor {
	public class TextureEditorManager : MonoBehaviour {

		public string folderName = "TextureEditor";
		public string textureFolder = "TextureEditor/Textures";


		public DataSaveLoadMaster dataSaveLoad;

		public GameObject texturePrefab;
		public RectTransform scrollContent;

		private TextureEntry currentTextureEntry;

		private ConfirmDialogUI dialog;


		private MeshRenderer pointedRenderer;
		private Texture orgTexture;
		private bool applying = false;

		public bool targetSharedMaterial = false;


#if UNITY_WEBGL && !UNITY_EDITOR
		public void OpenLoadTextureDialog(){
		}
#else
		public void OpenLoadTextureDialog(){

			string folder = dataSaveLoad.GetFolderPath (textureFolder);

			string[] paths = StandaloneFileBrowser.OpenFilePanel(
					"Selecte Textures", "", "png, jpg", true);

			foreach (string p in paths) {

				string path = p;
				Uri u = new Uri(path);
				//変換するURIがファイルを表していることを確認する
				if (u.IsFile)
				{
					path = u.LocalPath + Uri.UnescapeDataString(u.Fragment);
				}
				print (path);
				FileInfo fi = new FileInfo (path);
				print (fi.FullName);
				print (fi.Exists);

				string file = folder + "/" + Guid.NewGuid ().ToString () + fi.Extension;
				print (file);

				fi.CopyTo(file);
			}

			UpdateTextures ();
		}
#endif

		[System.Serializable] 
		public class SavedCamera
		{
			public string name;
			public string cameraName;
			public Vector3 position;
			public Quaternion rotation;
			public Vector3 localScale;
			public string createdDate;
		}

		void Reset(){
			dataSaveLoad = FindObjectOfType<DataSaveLoadMaster> ();
		}


		// Update is called once per frame
		void Update () {
			if (currentTextureEntry != null && !applying) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast (ray, out hit, 100)) {
					MeshRenderer r = hit.transform.GetComponent<MeshRenderer> ();

					if (pointedRenderer != r) {
						if (pointedRenderer != null) {
							if (targetSharedMaterial) {
								pointedRenderer.sharedMaterial.mainTexture = orgTexture;
							} else {
								pointedRenderer.material.mainTexture = orgTexture;
							}
							pointedRenderer = null;
						}
						pointedRenderer = r;

						if (targetSharedMaterial) {
							orgTexture = r.sharedMaterial.mainTexture;
							r.sharedMaterial.mainTexture = currentTextureEntry.rawImage.texture;
						} else {
							orgTexture = r.material.mainTexture;
							r.material.mainTexture = currentTextureEntry.rawImage.texture;
						}
					}
				} else if (pointedRenderer != null) {

					if (targetSharedMaterial) {
						pointedRenderer.sharedMaterial.mainTexture = orgTexture;
					} else {
						pointedRenderer.material.mainTexture = orgTexture;
					}
					pointedRenderer = null;
				}
			}

			if (Input.GetMouseButtonDown (0)) {
				if (pointedRenderer != null) {
					applying = true;
					dialog.Show("Apply this texture?", "Are you applying this texture?",
						"Apply", "Cancel", (x)=>{
							if(x){
								print("Approve");
								EditedTexture et = pointedRenderer.gameObject.AddComponent<EditedTexture>();
								if (targetSharedMaterial) {
									et.Set(pointedRenderer.sharedMaterial.mainTexture);
								}else{
									et.Set(pointedRenderer.material.mainTexture);
								}
								pointedRenderer = null;

								applying = false;
							} else {
								print("Cancel");
								applying = false;
							}
						});
				}
			}
			
		}

		void UnApplying(){
		}

		// Use this for initialization
		void Awake () {
			//			dataSaveLoad = GetComponent<DataSaveLoadMaster> ();
		}

		void Start () {

			dialog = FindObjectOfType<ConfirmDialogUI> ();

			string folder = dataSaveLoad.GetFolderPath (textureFolder);
			if (! new FileInfo (folder).Exists ) {
				DirectoryInfo fi = Directory.CreateDirectory (folder);
				print ("Created : " + fi.FullName);
			}

			//			dataSaveLoad.AddHandler(DataLoadCallback, typeof(SavedCamera));
			UpdateTextures ();
		}

		public void UpdateTextures(){

			string folder = dataSaveLoad.GetFolderPath (textureFolder);
			DirectoryInfo di = new DirectoryInfo (folder);

			List<string> files = new List<string>();
			foreach (string exp in new string[] { "*.jpg", "*.png" })
			{
				files.AddRange(System.IO.Directory.GetFiles(folder, exp));
			}
//			foreach (string f in files) {
//				print (f);
//			}

			TextureEntry[] entries = scrollContent.GetComponentsInChildren<TextureEntry> ();
			foreach (TextureEntry te in entries) {
				Destroy (te.gameObject);
			}
//
			foreach (string f in files) {

				GameObject pip = GameObject.Instantiate (texturePrefab);
				pip.transform.SetParent (scrollContent.transform, false);
				TextureEntry te = pip.GetComponentInChildren<TextureEntry> ();
				te.TexturePath = f;

				te.GetComponentInChildren<Button>().onClick.AddListener (() => {
					TextureSelected(te);
				});


//				PlaceItemEntry pie = pip.GetComponent<PlaceItemEntry> ();
//				pie.PlaceItem = pi;

//				pie.itemButton.onClick.AddListener (() => {
//					CreatePlaceItemInstance (pie.PlaceItem);
//				});
			}
		}

		public void TextureSelected(TextureEntry te){
			if (currentTextureEntry != null) {
				currentTextureEntry.GetComponentInChildren<Text> ().text = "";
			}
			currentTextureEntry = te;
			currentTextureEntry.GetComponentInChildren<Text> ().text = "selected";

		}

		public void DataLoadCallback(object o){

//			SavedCamera sc = o as SavedCamera;
//
//			if (sc.cameraName == cameraSwitcher.CurrentActive.c.name) {
//				cameraSwitcher.CurrentActive.transform.position = sc.position;
//				cameraSwitcher.CurrentActive.transform.rotation = sc.rotation;
//				cameraSwitcher.CurrentActive.transform.localScale = sc.localScale;
//			}
		}



		public void TakeScreenshot(){
			
		}

		public void ShowSaveUI(){
//			SavedCamera sc = new SavedCamera ();
//			sc.cameraName = cameraSwitcher.CurrentActive.c.name;
//			sc.position = cameraSwitcher.CurrentActive.transform.position;
//			sc.rotation = cameraSwitcher.CurrentActive.transform.rotation;
//			sc.localScale = cameraSwitcher.CurrentActive.transform.localScale;
//
//			dataSaveLoad.ShowSaveDialog (sc, folderName);
		}

		public void ShowLoadUI(){
			dataSaveLoad.ShowLoadDialog (typeof(SavedCamera), folderName);
		}


	}
}