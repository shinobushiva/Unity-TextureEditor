using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DataSaveLoad;
using System;
using System.IO;
using SFB;
using UnityEngine.EventSystems;

// This uses UnityStandaloneFileBrowser
// https://github.com/gkngkc/UnityStandaloneFileBrowser
namespace Shiva.TextureEditor {
	public class TextureEditorManager : MonoBehaviour {

		public string folderName = "TextureEditor";
		public string textureFolder = "TextureEditor/Textures";
		public bool targetSharedMaterial = false;

		public DataSaveLoadMaster dataSaveLoad;
		private ConfirmDialogUI dialog;

		public GameObject texturePrefab;
		public RectTransform scrollContent;

		private TextureEntry currentTextureEntry;

		private MeshRenderer pointedRenderer;
		private Dictionary<Material, Texture> orgTextureMap = new Dictionary<Material, Texture>();
//		private Dictionary<Material, EditedTexture> matEditedMap = new Dictionary<Material, EditedTexture>();

		private bool applying = false;


#if UNITY_WEBGL && !UNITY_EDITOR
		public void OpenLoadTextureDialog(){
		}
#else
		public void OpenLoadTextureDialog(){

			string folder = dataSaveLoad.GetFolderPath (textureFolder);

			string[] paths = StandaloneFileBrowser.OpenFilePanel(
					"Selecte Textures", "", "", true);

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

		void Reset(){
			dataSaveLoad = FindObjectOfType<DataSaveLoadMaster> ();
		}

		private void ResetTexture(){
			if (pointedRenderer == null)
				return;
			
			Material mat = targetSharedMaterial ? pointedRenderer.sharedMaterial : pointedRenderer.material;
			EditedTexture et = pointedRenderer.GetComponent<EditedTexture> ();
			if (et) {
				et.SetTextureTo (mat);
			} else if (orgTextureMap.ContainsKey (mat)) {
				mat.mainTexture = orgTextureMap [mat];
			}

			pointedRenderer = null;
		}


		// Update is called once per frame
		void Update () {
			if (applying)
				return;

			if (EventSystem.current.IsPointerOverGameObject ()) {
				ResetTexture ();
				return;
			}

			if (currentTextureEntry != null) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast (ray, out hit, 100)) {
					MeshRenderer r = hit.transform.GetComponent<MeshRenderer> ();
					if (r == null) {
						ResetTexture ();
						return;
					}
						

					if (pointedRenderer != r) {
						if (pointedRenderer != null) {
							ResetTexture ();
						}
						pointedRenderer = r;

						Material mat = targetSharedMaterial ? r.sharedMaterial : r.material;
						if (!orgTextureMap.ContainsKey (mat)) {
							EditedTexture et = pointedRenderer.GetComponent<EditedTexture> ();
							if (et) {
								orgTextureMap [mat] = et.orgTexture;
							} else {
								orgTextureMap [mat] = mat.mainTexture;
							}
						}
						currentTextureEntry.SetTextureTo (mat, orgTextureMap [mat]);

					}
				} else if (pointedRenderer != null) {
					ResetTexture ();
					return;
				}
			}
			if (Input.GetMouseButtonDown (0)) {
				if (pointedRenderer != null) {
					applying = true;
					dialog.Show ("Apply this texture?", "Are you applying this texture?",
						"Apply", "Cancel", (x) => {
						if (x) {
							EditedTexture et = AddEditedTextureTo(pointedRenderer.gameObject);
							ResetTexture();
							et.Set (currentTextureEntry);
							pointedRenderer = null;

							applying = false;
						} else {
							applying = false;
						}
					});
				}
			}
		}

		private EditedTexture AddEditedTextureTo(GameObject go){
			EditedTexture[] ets = go.GetComponents<EditedTexture> ();
			foreach (var e in ets) {
				e.Restore ();
				Destroy (e);
			}
			EditedTexture et = go.AddComponent<EditedTexture> ();

			return et;
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

			//creating blank texture entry for reseting
			{
				GameObject pip = GameObject.Instantiate (texturePrefab);
				pip.transform.SetParent (scrollContent.transform, false);
				TextureEntry te = pip.GetComponentInChildren<TextureEntry> (true);
				te.TexturePath = null;

				te.GetComponentInChildren<Button> ().onClick.AddListener (() => {
					TextureSelected (te);
				});
			}

			//creating each texture entries
			foreach (string f in files) {

				GameObject pip = GameObject.Instantiate (texturePrefab);
				pip.transform.SetParent (scrollContent.transform, false);
				TextureEntry te = pip.GetComponentInChildren<TextureEntry> (true);
				te.TexturePath = f;

				te.GetComponentInChildren<Button>().onClick.AddListener (() => {
					TextureSelected(te);
				});
			}
		}

		public void TextureSelected(TextureEntry te){
			if (currentTextureEntry != null) {
				currentTextureEntry.GetComponentInChildren<Text> ().text = "";
			}
			currentTextureEntry = te;
			currentTextureEntry.GetComponentInChildren<Text> ().text = "selected";
		}

		public void DeleteTexture(){
			if (currentTextureEntry != null && currentTextureEntry.TexturePath != null) {
				new FileInfo (currentTextureEntry.TexturePath).Delete ();
				Destroy (currentTextureEntry.gameObject);
			}
		}




		[System.Serializable] 
		public class SavedEditedTexture
		{
			public string path;
			public string textureFilePath;
		}

		public void DataLoadCallback(object o){

			TextureEntry[] entries = scrollContent.GetComponentsInChildren<TextureEntry> ();
			Dictionary<string, TextureEntry> dict = new Dictionary<string, TextureEntry> ();
			foreach (var entry in entries) {
				if (entry.TexturePath == null)
					continue;
				dict [entry.TexturePath] = entry;
			}

			EditedTexture[] ets = FindObjectsOfType<EditedTexture> ();
			foreach (EditedTexture et in ets) {
				et.Restore ();
			}

			SavedEditedTexture[] sets = o as SavedEditedTexture[];
			foreach (var s in sets) {
				if(s.textureFilePath != null && dict.ContainsKey(s.textureFilePath)){
					GameObject go = GameObject.Find (s.path);
					EditedTexture et = AddEditedTextureTo (go);
					et.Set (dict [s.textureFilePath]);
				}
			}

			orgTextureMap.Clear ();
		}

		public void ShowSaveUI(){

			List<SavedEditedTexture> list = new List<SavedEditedTexture> ();

			EditedTexture[] ets = FindObjectsOfType<EditedTexture> ();
			foreach (EditedTexture et in ets) {
				SavedEditedTexture s = new SavedEditedTexture ();
				s.path = et.gameObject.GetFullPath ();
				s.textureFilePath = et.textureFilePath;
				list.Add (s);
			}

			dataSaveLoad.SetHandler(DataLoadCallback, typeof(SavedEditedTexture[]));
			dataSaveLoad.ShowSaveDialog (list.ToArray(), folderName);
		}

		public void ShowLoadUI(){
			dataSaveLoad.SetHandler(DataLoadCallback, typeof(SavedEditedTexture[]));
			dataSaveLoad.ShowLoadDialog (typeof(SavedEditedTexture[]), folderName);
		}


	}
}