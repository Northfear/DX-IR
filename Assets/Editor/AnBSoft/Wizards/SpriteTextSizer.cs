//-----------------------------------------------------------------
//  Copyright 2009 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Threading;


public class SpriteTextSizer : ScriptableWizard
{
	public bool onlyApplyToSelected = false;	// Only apply settings to the selected texts
	public bool applyToAllInScene = false;		// Applies the sizing to all texts in the scene
	public bool applyToAllPrefabs = true;		// Applies sizing to all prefabs in the project folder
	public bool disablePixelPerfect = false;	// When set, disables pixel perfect on all texts processed. If this is not set, any texts with pixel perfect enabled will not be changed.

	//public int targetScreenWidth = 480;
	public int targetScreenHeight = 320;

	public Camera renderCamera;

	string onlySelectedHelp = "Only apply sizing to currently selected text.";
	string allInSceneHelp = "Will apply sizing to all SpriteTexts in this scene. WARNING: This can override the character size settings in any prefab instances! Try checking \"Apply To All Prefabs\" instead.";
	string allPrefabsHelp = "scan the entire project folder for prefabs and apply sizing to them.";

	ArrayList texts = new ArrayList();


	// Loads previous settings from PlayerPrefs
	void LoadSettings()
	{
		onlyApplyToSelected = 1 == PlayerPrefs.GetInt("SpriteTextSizer.onlyApplyToSelected", onlyApplyToSelected ? 1 : 0);
		applyToAllInScene = 1 == PlayerPrefs.GetInt("SpriteTextSizer.applyToAllInScene", applyToAllInScene ? 1 : 0);
		applyToAllPrefabs = 1 == PlayerPrefs.GetInt("SpriteTextSizer.applyToAllPrefabs", applyToAllPrefabs ? 1 : 0);
		disablePixelPerfect = 1 == PlayerPrefs.GetInt("SpriteTextSizer.disablePixelPerfect", disablePixelPerfect ? 1 : 0);
		targetScreenHeight = PlayerPrefs.GetInt("SpriteTextSizer.targetScreenHeight", targetScreenHeight);

		string camName = PlayerPrefs.GetString("SpriteTextSizer.camName");
		if (!System.String.IsNullOrEmpty(camName))
		{
			GameObject go = GameObject.Find(camName);

			if (go != null)
				renderCamera = go.GetComponent(typeof(Camera)) as Camera;
		}
	}

	// Saves settings to PlayerPrefs
	void SaveSettings()
	{
		PlayerPrefs.SetInt("SpriteTextSizer.onlyApplyToSelected", onlyApplyToSelected ? 1 : 0);
		PlayerPrefs.SetInt("SpriteTextSizer.applyToAllInScene", applyToAllInScene ? 1 : 0);
		PlayerPrefs.SetInt("SpriteTextSizer.applyToAllPrefabs", applyToAllPrefabs ? 1 : 0);
		PlayerPrefs.SetInt("SpriteTextSizer.disablePixelPerfect", disablePixelPerfect ? 1 : 0);
		PlayerPrefs.SetInt("SpriteTextSizer.targetScreenHeight", targetScreenHeight);
		PlayerPrefs.SetString("SpriteTextSizer.camName", renderCamera.name);
	}


	[UnityEditor.MenuItem("Tools/A&B Software/Size SpriteTexts")]
	static void StartSizingTexts()
	{
		SpriteTextSizer ss = (SpriteTextSizer)ScriptableWizard.DisplayWizard("Size SpriteTexts", typeof(SpriteTextSizer), "Ok");
		ss.LoadSettings();
	}


	// Updates the wizard:
	void OnWizardUpdate()
	{
		if (renderCamera == null)
			renderCamera = Camera.main;

		if (onlyApplyToSelected)
		{
			// See if we have a valid selection:
			Object[] o = Selection.GetFiltered(typeof(SpriteText), SelectionMode.Unfiltered);
			if (o != null)
				if (o.Length != 0)
				{
					// Uncheck other options:
					applyToAllInScene = false;
					applyToAllPrefabs = false;
					helpString = onlySelectedHelp;
					errorString = "";
					isValid = true;
					return;
				}

			// Else we don't have a valid selection, so uncheck:
			onlyApplyToSelected = false;
		}

		helpString = "";

		if (applyToAllInScene)
			helpString = allInSceneHelp;

		if (applyToAllPrefabs)
		{
			if (helpString.Length != 0)
				helpString += "  Will also ";
			else
				helpString += "Will ";

			helpString += allPrefabsHelp;
		}

		if (helpString.Length == 0)
		{
			isValid = false;
			errorString = "Nothing to do!";
		}
		else
		{
			isValid = true;
			errorString = "";
		}
	}


	// Let's do this thing!:
	void OnWizardCreate()
	{
		float worldUnitsPerScreenPixel;
		int skipped = 0; // How many texts had to be skipped because they were prefabs?

		if (disablePixelPerfect)
			disablePixelPerfect = EditorUtility.DisplayDialog("Are you sure?", "Are you sure you wish to disable pixel-perfect on all selected texts?", "Yes", "No");

		// Get our desired texts:
		FindSpriteTexts();


		if (renderCamera.isOrthoGraphic)
		{
			// Use orthographic logic:
			worldUnitsPerScreenPixel = (renderCamera.orthographicSize * 2f) / targetScreenHeight;

			// Now set their sizes:
			for (int i = 0; i < texts.Count; ++i)
			{
				SpriteText text = (SpriteText)texts[i];
				float pxSize = (float)FontStore.GetFont(text.font).PixelSize;

				if (disablePixelPerfect)
					text.pixelPerfect = false;

				text.SetCharacterSize(pxSize * worldUnitsPerScreenPixel);

				EditorUtility.SetDirty(((SpriteText)texts[i]).gameObject);
			}
		}
		else
		{
			// Use perspective logic:
			float dist;
			Plane nearPlane;

			// Now set their sizes:
			for (int i = 0; i < texts.Count; ++i)
			{
#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9)
#if (UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9)
				PrefabType ptype = PrefabUtility.GetPrefabType(((SpriteText)texts[i]).gameObject);
#else
				PrefabType ptype = EditorUtility.GetPrefabType(((SpriteText)texts[i]).gameObject);
#endif
				// We can't do prefabs with perspective cameras:
				if (ptype == PrefabType.Prefab || ptype == PrefabType.ModelPrefab)
				{
					++skipped;
					Debug.LogWarning("SpriteText \"" + ((SpriteText)texts[i]).name + "\" skipped because it is a prefab and the selected camera is perspective. Size cannot be calculated for perspective cameras without the object actually being positioned in front of the camera.\n Either use an orthographic camera, or use an instance of the prefab in the scene instead.");
					continue;
				}
#endif
				SpriteText text = (SpriteText)texts[i];
				nearPlane = new Plane(renderCamera.transform.forward, renderCamera.transform.position);
				float pxSize = (float)FontStore.GetFont(text.font).PixelSize;

				// Determine the world distance between two vertical
				// screen pixels for this camera:
				dist = nearPlane.GetDistanceToPoint(text.transform.position);
				worldUnitsPerScreenPixel = Vector3.Distance(renderCamera.ScreenToWorldPoint(new Vector3(0, 1, dist)), renderCamera.ScreenToWorldPoint(new Vector3(0, 0, dist)));

				if (disablePixelPerfect)
					text.pixelPerfect = false;

				text.SetCharacterSize(pxSize * worldUnitsPerScreenPixel);

				EditorUtility.SetDirty(((SpriteText)texts[i]).gameObject);
			}
		}



		// See if we need to advise the user to reload the scene:
		if (applyToAllPrefabs)
			EditorUtility.DisplayDialog("NOTE", "You may need to reload the current scene for prefab instances to reflect your changes.", "OK");

		Debug.Log((texts.Count - skipped) + " texts sized.");

		// Save our settings for next time:
		SaveSettings();
	}


	// Finds all desired texts
	void FindSpriteTexts()
	{
		texts.Clear();

		if (onlyApplyToSelected)
		{
			Object[] o = Selection.GetFiltered(typeof(SpriteText), SelectionMode.Unfiltered);
			texts.AddRange(o);
			return;
		}

		if (applyToAllInScene)
		{
			// Get all packed texts in the scene:
			Object[] o = FindObjectsOfType(typeof(SpriteText));

			for (int i = 0; i < o.Length; ++i)
			{
				if (applyToAllPrefabs)
				{
					// Check to see if this is a prefab instance,
					// and if so, don't use it since we'll be updating
					// the prefab itself anyway.
#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9)
#if (UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9)
					if (PrefabType.PrefabInstance != PrefabUtility.GetPrefabType(o[i]))
#else
					if (PrefabType.PrefabInstance != EditorUtility.GetPrefabType(o[i]))
#endif
#endif
					texts.Add(o[i]);
				}
				else
					texts.Add(o[i]);
			}
		}

		// See if we need to scan the Assets folder for sprite objects
		if (applyToAllPrefabs)
			ScanProjectFolder(texts);
	}


	// Scans the project folder, looking for sprite prefabs
	void ScanProjectFolder(ArrayList texts)
	{
		string[] files;
		GameObject obj;
		Component[] c;

		// Stack of folders:
		Stack stack = new Stack();

		// Add root directory:
		stack.Push(Application.dataPath);

		// Continue while there are folders to process
		while (stack.Count > 0)
		{
			// Get top folder:
			string dir = (string)stack.Pop();

			try
			{
				// Get a list of all prefabs in this folder:
				files = Directory.GetFiles(dir, "*.prefab");

				// Process all prefabs:
				for (int i = 0; i < files.Length; ++i)
				{
					// Make the file path relative to the assets folder:
					files[i] = files[i].Substring(Application.dataPath.Length - 6);

					obj = (GameObject)AssetDatabase.LoadAssetAtPath(files[i], typeof(GameObject));

					if (obj != null)
					{
						c = obj.GetComponentsInChildren(typeof(SpriteText), true);

						for (int j = 0; j < c.Length; ++j)
							texts.Add(c[j]);
					}
				}

				// Add all subfolders in this folder:
				foreach (string dn in Directory.GetDirectories(dir))
				{
					stack.Push(dn);
				}
			}
			catch
			{
				// Error
				Debug.LogError("Could not access folder: \"" + dir + "\"");
			}
		}
	}
}
