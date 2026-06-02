using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public enum GameState
	{
		Startup = 0,
		Menu = 1,
		FadeBlackToGameLoad = 2,
		FadeInGameLoad = 3,
		FadeOutGameLoad = 4,
		Game = 5,
		Paused = 6,
		FadeBlackToMenuLoad = 7,
		FadeInMenuLoad = 8,
		FadeOutMenuLoad = 9
	}

	public enum FullscreenQuadType
	{
		Normal = 0,
		Multiply = 1,
		Add = 2
	}

	[Serializable]
	public class LevelData
	{
		public string m_UnityScene = string.Empty;

		public bool m_IncludeInBuild = true;

		public string m_DisplayName = string.Empty;

		public string m_LoadingImage = string.Empty;

		public string m_MusicEvent = string.Empty;

		public string[] m_SoundBanks;
	}

	public delegate void PauseHandler(bool Paused);

	public delegate void SceneLoadHandler();

	public delegate void SmartVisionHandler(bool Enabled);

	public delegate XmlElement SaveGameHandler(XmlElement root, XmlDocument doc);

	public delegate XmlElement LoadGameHandler(XmlElement root);

	public static GameManager m_This;

	public static int m_FrameNum;

	[HideInInspector]
	public GameState m_GameState;

	public string m_MenuScene = "Menu";

	public string m_MenuLoadingImage = "LoadingInProgress";

	private string m_EmptyScene = "EmptyScene";

	public LevelData[] m_GameScenes;

	public static string m_LevelToLoad;

	public static string m_LevelLoadingImage;

	[HideInInspector]
	public int m_LastLoadedLevel = -1;

	[HideInInspector]
	public string m_NameOfGameObjectToSpawnAt;

	public string m_MenuResLow = "GUI/Menu_LowRes";

	public string m_MenuResHigh = "GUI/Menu_HighRes";

	public string m_HUDResLow = "GUI/HUD_LowRes";

	public string m_HUDResHigh = "GUI/HUD_HighRes";

	private bool m_RenderBlackTexture;

	private Color m_BlackTextureColor = Color.black;

	private Texture2D m_LoadingTexture;

	private bool m_RenderLoadingTexture;

	private Color m_LoadingTextureColor = Color.white;

	public static bool m_LoadSaveAfterLevelLoad;

	public static bool m_LoadSceneSetupAfterLevelLoad;

	public static bool m_JustLoadedLevel;

	public static bool m_SaveLoaded;

	public static float m_SaveTextTimer;

	public static float m_SaveTextTime;

	[HideInInspector]
	public Location m_CurrentLocation = Location.None;

	[HideInInspector]
	public LinkedList<MediaLog>[] m_MediaLogs = new LinkedList<MediaLog>[6];

	private static XmlDocument m_SaveXML;

	public static event PauseHandler OnPause;

	public static event SceneLoadHandler OnSceneLoad;

	public static event SmartVisionHandler OnSmartVisionToggled;

	public static event SaveGameHandler OnSaveGame;

	public static event LoadGameHandler OnLoadGame;

	public static bool IsGamePaused()
	{
		return m_This == null || m_This.m_GameState == GameState.Paused;
	}

	public static void SetGameObjectToSpawnAt(string spawnPoint)
	{
		m_This.m_NameOfGameObjectToSpawnAt = spawnPoint;
	}

	public static string GetGameObjectToSpawnAt()
	{
		return m_This.m_NameOfGameObjectToSpawnAt;
	}

	public static void SmartVisionToggled(bool Enabled)
	{
		GameManager.OnSmartVisionToggled(Enabled);
	}

	private void Awake()
	{
		m_This = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		ResetSaveXML();
		m_SaveTextTimer = 0f;
		AllocateMediaLogs();
	}

	private void ResetSaveXML()
	{
		m_SaveXML = new XmlDocument();
		m_SaveXML.AppendChild(m_SaveXML.CreateElement("DeusExMobileSaveGame"));
	}

	private void Start()
	{
		LoadPlatformSpecificMenu();
	}

	public static void AddMediaLog(MediaType type, string to, string from, string subject, string body)
	{
		if (m_This == null || m_This.m_CurrentLocation <= Location.None || m_This.m_CurrentLocation >= Location.Total)
		{
			return;
		}
		for (LinkedListNode<MediaLog> linkedListNode = m_This.m_MediaLogs[(int)m_This.m_CurrentLocation].First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (string.Compare(linkedListNode.Value.m_Subject, subject, false) == 0)
			{
				return;
			}
		}
		MediaLog mediaLog = new MediaLog();
		mediaLog.m_MediaType = type;
		mediaLog.m_Read = false;
		mediaLog.m_To = to;
		mediaLog.m_From = from;
		mediaLog.m_Subject = subject;
		mediaLog.m_Body = body;
		m_This.m_MediaLogs[(int)m_This.m_CurrentLocation].AddFirst(mediaLog);
	}

	public static void LoadHUD()
	{
		if (!(m_This == null))
		{
			m_This.LoadPlatformSpecificHUD();
		}
	}

	public static void ReloadCurrentLevel()
	{
		Time.timeScale = 1f;
		LoadLevel(m_This.m_LastLoadedLevel);
	}

	private void AllocateMediaLogs()
	{
		for (int i = 0; i < 6; i++)
		{
			m_MediaLogs[i] = new LinkedList<MediaLog>();
		}
	}

	public static void LoadMenu()
	{
		LoadLevel(-1);
	}

	public static void LoadLevel(string LevelName)
	{
		for (int i = 0; i < m_This.m_GameScenes.Length; i++)
		{
			if (m_This.m_GameScenes[i].m_UnityScene == LevelName || m_This.m_GameScenes[i].m_DisplayName == LevelName)
			{
				LoadLevel(i);
				break;
			}
		}
	}

	public static void LoadLevel(int LevelID)
	{
		if (m_This.m_LastLoadedLevel == -1)
		{
			m_This.AllocateMediaLogs();
			Globals.ResetStoryProgressionVars();
			Globals.m_Inventory.m_ActiveItemQuickslot = 0;
			Globals.m_Inventory.m_ActiveWeaponQuickslot = 0;
			Globals.m_Inventory.m_ActiveGrenadeQuickslot = 0;
			Globals.m_Inventory.LoadDefaultInventory();
		}
		if (m_This.m_LastLoadedLevel != -1 && !m_LoadSaveAfterLevelLoad)
		{
			SaveCurrentSceneData((XmlElement)m_SaveXML.ChildNodes[0]);
		}
		m_SaveLoaded = false;
		m_JustLoadedLevel = true;
		if (!m_LoadSaveAfterLevelLoad && LevelID != -1 && m_This.m_LastLoadedLevel != -1)
		{
			XmlNodeList elementsByTagName = m_SaveXML.GetElementsByTagName(m_This.m_GameScenes[LevelID].m_UnityScene);
			if (elementsByTagName.Count > 0)
			{
				m_LoadSceneSetupAfterLevelLoad = true;
			}
		}
		Time.timeScale = 1f;
		if (GameManager.OnSceneLoad != null)
		{
			GameManager.OnSceneLoad();
		}
		SoundManager.TriggerEvent("STOP_ALL", Globals.m_This.gameObject);
		Globals.m_AIDirector.ClearAll();
		Globals.m_InteractiveObjectManager.ClearAll();
		InteractiveObject_Pickup.m_LevelPickups.Clear();
		InteractiveObject_Pickup.m_SpawnedPickups.Clear();
		InteractiveObject_Pickup.m_DestroyedPickups.Clear();
		NPC_Base.m_DestroyedNPCs.Clear();
		List<string> list = new List<string>();
		if (m_This.m_LastLoadedLevel != -1)
		{
			string[] soundBanks = m_This.m_GameScenes[m_This.m_LastLoadedLevel].m_SoundBanks;
			foreach (string text in soundBanks)
			{
				bool flag = false;
				if (LevelID != -1)
				{
					string[] soundBanks2 = m_This.m_GameScenes[LevelID].m_SoundBanks;
					foreach (string text2 in soundBanks2)
					{
						if (text2 == text)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					SoundManager.UnloadSoundBank(text);
				}
				else
				{
					list.Add(text);
				}
			}
		}
		m_This.m_LastLoadedLevel = LevelID;
		if (LevelID != -1)
		{
			SoundManager.SetLevelMusic(m_This.m_GameScenes[LevelID].m_MusicEvent);
		}
		else
		{
			SoundManager.SetLevelMusic(string.Empty);
		}
		if (LevelID != -1)
		{
			string[] soundBanks3 = m_This.m_GameScenes[LevelID].m_SoundBanks;
			foreach (string text3 in soundBanks3)
			{
				bool flag2 = false;
				foreach (string item in list)
				{
					if (text3 == item)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					SoundManager.LoadSoundBank(text3);
				}
			}
		}
		if (LevelID < 0)
		{
			m_LevelToLoad = m_This.m_MenuScene;
			m_LevelLoadingImage = m_This.m_MenuLoadingImage;
			m_This.m_GameState = GameState.FadeBlackToMenuLoad;
		}
		else
		{
			m_LevelToLoad = m_This.m_GameScenes[LevelID].m_UnityScene;
			m_LevelLoadingImage = m_This.m_GameScenes[LevelID].m_LoadingImage;
			m_This.m_GameState = GameState.FadeBlackToGameLoad;
		}
		UIManager.instance.blockInput = true;
		m_This.m_BlackTextureColor.a = 0f;
		m_This.m_RenderBlackTexture = true;
	}

	private void LoadPlatformSpecificHUD()
	{
		if (Globals.m_HUDRoot == null)
		{
			if (Globals.m_ResolutionRank == Globals.ResolutionRank.Low)
			{
				UnityEngine.Object.Instantiate(Resources.Load(m_This.m_HUDResLow), Vector3.zero, Quaternion.identity);
			}
			else
			{
				UnityEngine.Object.Instantiate(Resources.Load(m_This.m_HUDResHigh), Vector3.zero, Quaternion.identity);
			}
		}
	}

	private void LoadPlatformSpecificMenu()
	{
		if (Globals.m_MenuRoot == null)
		{
			if (Globals.m_ResolutionRank == Globals.ResolutionRank.Low)
			{
				UnityEngine.Object.Instantiate(Resources.Load(m_MenuResLow), Vector3.zero, Quaternion.identity);
			}
			else
			{
				UnityEngine.Object.Instantiate(Resources.Load(m_MenuResHigh), Vector3.zero, Quaternion.identity);
			}
		}
	}

	public static void GamePaused(bool Paused)
	{
		if (Paused)
		{
			m_This.m_GameState = GameState.Paused;
			if (GameManager.OnPause != null)
			{
				GameManager.OnPause(true);
			}
		}
		else
		{
			m_This.m_GameState = GameState.Game;
			if (GameManager.OnPause != null)
			{
				GameManager.OnPause(false);
			}
		}
	}

	public static void SaveCurrentSceneData(XmlElement root)
	{
		XmlNodeList elementsByTagName = root.GetElementsByTagName(Application.loadedLevelName);
		XmlElement xmlElement = (XmlElement)elementsByTagName[0];
		if (xmlElement != null)
		{
			xmlElement.RemoveAll();
		}
		else
		{
			xmlElement = (XmlElement)root.AppendChild(m_SaveXML.CreateElement(Application.loadedLevelName));
		}
		xmlElement.AppendChild(m_SaveXML.CreateElement("InteractiveObjects"));
		GameManager.OnSaveGame(xmlElement, m_SaveXML);
		xmlElement.AppendChild(m_SaveXML.CreateElement("Enemies"));
		for (int i = 0; i < 8; i++)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = Globals.m_AIDirector.m_Squads[i].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value.SaveGame(xmlElement, m_SaveXML);
			}
		}
		xmlElement.AppendChild(m_SaveXML.CreateElement("NPCs"));
		for (LinkedListNode<NPC_Base> linkedListNode2 = Globals.m_AIDirector.m_NPCs.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
		{
			linkedListNode2.Value.SaveGame(xmlElement, m_SaveXML);
		}
		XmlElement xmlElement2 = (XmlElement)xmlElement.AppendChild(m_SaveXML.CreateElement("DestroyedNPCs"));
		for (LinkedListNode<Vector3> linkedListNode3 = NPC_Base.m_DestroyedNPCs.First; linkedListNode3 != null; linkedListNode3 = linkedListNode3.Next)
		{
			XmlElement xmlElement3 = (XmlElement)xmlElement2.AppendChild(m_SaveXML.CreateElement("DeadNPC"));
			xmlElement3.SetAttribute("Spawn_Position", linkedListNode3.Value.x + "," + linkedListNode3.Value.y + "," + linkedListNode3.Value.z);
		}
		xmlElement.AppendChild(m_SaveXML.CreateElement("Projectiles"));
		for (LinkedListNode<ProjectileBase> linkedListNode4 = ProjectileBase.m_Projectiles.First; linkedListNode4 != null; linkedListNode4 = linkedListNode4.Next)
		{
			linkedListNode4.Value.SaveGame(xmlElement, m_SaveXML);
		}
		xmlElement.AppendChild(m_SaveXML.CreateElement("Grenades"));
		for (LinkedListNode<GrenadeFrag> linkedListNode5 = Globals.m_AIDirector.m_ActiveGrenades.First; linkedListNode5 != null; linkedListNode5 = linkedListNode5.Next)
		{
			linkedListNode5.Value.SaveGame(xmlElement, m_SaveXML);
		}
		xmlElement.AppendChild(m_SaveXML.CreateElement("Mines"));
		for (LinkedListNode<Mine> linkedListNode6 = Mine.m_Mines.First; linkedListNode6 != null; linkedListNode6 = linkedListNode6.Next)
		{
			linkedListNode6.Value.SaveGame(xmlElement, m_SaveXML);
		}
		XmlElement xmlElement4 = (XmlElement)xmlElement.AppendChild(m_SaveXML.CreateElement("SpawnedPickups"));
		for (LinkedListNode<InteractiveObject_Pickup> linkedListNode7 = InteractiveObject_Pickup.m_SpawnedPickups.First; linkedListNode7 != null; linkedListNode7 = linkedListNode7.Next)
		{
			if (linkedListNode7.Value.m_WeaponBase != null && linkedListNode7.Value.m_WeaponBase.m_User == null)
			{
				XmlElement xmlElement5 = (XmlElement)xmlElement4.AppendChild(m_SaveXML.CreateElement("Pickup"));
				xmlElement5.SetAttribute("Type", linkedListNode7.Value.m_PickupMainType.ToString());
				xmlElement5.SetAttribute("WeaponType", linkedListNode7.Value.m_WeaponBase.m_WeaponType.ToString());
				xmlElement5.SetAttribute("Position", linkedListNode7.Value.gameObject.transform.position.x + "," + linkedListNode7.Value.gameObject.transform.position.y + "," + linkedListNode7.Value.gameObject.transform.position.z);
				xmlElement5.SetAttribute("Rotation", linkedListNode7.Value.gameObject.transform.rotation.x + "," + linkedListNode7.Value.gameObject.transform.rotation.y + "," + linkedListNode7.Value.gameObject.transform.rotation.z + "," + linkedListNode7.Value.gameObject.transform.rotation.w);
			}
		}
		XmlElement xmlElement6 = (XmlElement)xmlElement.AppendChild(m_SaveXML.CreateElement("DestroyedPickups"));
		for (LinkedListNode<Vector3> linkedListNode8 = InteractiveObject_Pickup.m_DestroyedPickups.First; linkedListNode8 != null; linkedListNode8 = linkedListNode8.Next)
		{
			XmlElement xmlElement7 = (XmlElement)xmlElement6.AppendChild(m_SaveXML.CreateElement("Pickup"));
			xmlElement7.SetAttribute("Spawn_Position", linkedListNode8.Value.x + "," + linkedListNode8.Value.y + "," + linkedListNode8.Value.z);
		}
	}

	public static void GameSaved(bool lowtext = false)
	{
		Globals.m_HUD.ShowSavingText();
		if (lowtext)
		{
			Vector3 position = Globals.m_HUD.m_SavingText.transform.position;
			position.y = -2.5f;
			Globals.m_HUD.m_SavingText.transform.position = position;
		}
		XmlElement xmlElement = (XmlElement)m_SaveXML.ChildNodes[0];
		XmlNodeList elementsByTagName = xmlElement.GetElementsByTagName("GameManager");
		XmlElement xmlElement2 = (XmlElement)elementsByTagName[0];
		if (xmlElement2 != null)
		{
			xmlElement2.RemoveAll();
			xmlElement.RemoveChild(xmlElement2);
		}
		elementsByTagName = xmlElement.GetElementsByTagName("PlayerSpecific");
		XmlElement xmlElement3 = (XmlElement)elementsByTagName[0];
		if (xmlElement3 != null)
		{
			xmlElement3.RemoveAll();
			xmlElement.RemoveChild(xmlElement3);
		}
		elementsByTagName = xmlElement.GetElementsByTagName("MediaLogs");
		XmlElement xmlElement4 = (XmlElement)elementsByTagName[0];
		if (xmlElement4 != null)
		{
			xmlElement4.RemoveAll();
			xmlElement.RemoveChild(xmlElement4);
		}
		XmlElement xmlElement5 = (XmlElement)xmlElement.AppendChild(m_SaveXML.CreateElement("GameManager"));
		xmlElement5.SetAttribute("Game_Version", Globals.m_This.m_VersionMajor + "." + Globals.m_This.m_VersionMinor + "." + Globals.m_This.m_VersionMicro);
		xmlElement5.SetAttribute("Save_Version", "1");
		xmlElement5.SetAttribute("Scene", Application.loadedLevelName);
		xmlElement3 = (XmlElement)xmlElement.AppendChild(m_SaveXML.CreateElement("PlayerSpecific"));
		Globals.m_This.SaveGame(xmlElement3, m_SaveXML);
		Globals.m_Inventory.SaveGame(xmlElement3, m_SaveXML);
		XmlElement xmlElement6 = (XmlElement)xmlElement.AppendChild(m_SaveXML.CreateElement("MediaLogs"));
		for (int i = 0; i < 6; i++)
		{
			for (LinkedListNode<MediaLog> linkedListNode = m_This.m_MediaLogs[i].First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				xmlElement4 = (XmlElement)xmlElement6.AppendChild(m_SaveXML.CreateElement("MediaLog"));
				xmlElement4.SetAttribute("Location", i.ToString());
				xmlElement4.SetAttribute("MediaType", linkedListNode.Value.m_MediaType.ToString());
				xmlElement4.SetAttribute("Read", linkedListNode.Value.m_Read.ToString());
				xmlElement4.SetAttribute("To", linkedListNode.Value.m_To);
				xmlElement4.SetAttribute("From", linkedListNode.Value.m_From);
				xmlElement4.SetAttribute("Subject", linkedListNode.Value.m_Subject);
				xmlElement4.SetAttribute("Body", linkedListNode.Value.m_Body);
			}
		}
		Globals.m_PlayerController.SaveGame(xmlElement3, m_SaveXML);
		SaveCurrentSceneData(xmlElement);
		FileStream fileStream = new FileStream(Application.persistentDataPath + "/dxmsave.xml", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
		m_SaveXML.Save(fileStream);
		fileStream.Close();
		Debug.Log("GAME SAVED");
	}

	public static void GameLoaded()
	{
		m_This.ResetSaveXML();
		if (File.Exists(Application.persistentDataPath + "/dxmsave.xml"))
		{
			FileStream fileStream = new FileStream(Application.persistentDataPath + "/dxmsave.xml", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			m_SaveXML.Load(fileStream);
			fileStream.Close();
			XmlNodeList elementsByTagName = m_SaveXML.GetElementsByTagName("GameManager");
			string innerText = elementsByTagName[0].Attributes.GetNamedItem("Scene").InnerText;
			m_LoadSaveAfterLevelLoad = true;
			if (innerText == Application.loadedLevelName)
			{
				ReloadCurrentLevel();
			}
			else
			{
				LoadLevel(innerText);
			}
		}
		else if (m_This.m_LastLoadedLevel != -1)
		{
			ReloadCurrentLevel();
		}
	}

	private void LoadEnemies(XmlElement root)
	{
		XmlNodeList elementsByTagName = root.GetElementsByTagName("Enemies");
		foreach (XmlNode item in elementsByTagName)
		{
			if (!(item.Name == "Enemies"))
			{
				continue;
			}
			foreach (XmlNode childNode in item.ChildNodes)
			{
				if (!(childNode.Name == "Enemy"))
				{
					continue;
				}
				foreach (XmlAttribute attribute in childNode.Attributes)
				{
					if (!(attribute.Name == "Spawn_Position"))
					{
						continue;
					}
					string[] array = attribute.InnerText.Split(',');
					Vector3 vector = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
					for (LinkedListNode<EnemySpawner> linkedListNode = EnemySpawner.m_Spawners.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
					{
						if (linkedListNode.Value.transform.position == vector)
						{
							linkedListNode.Value.gameObject.SetActiveRecursively(true);
							Enemy_Base enemy_Base = linkedListNode.Value.Spawn();
							enemy_Base.LoadGame((XmlElement)childNode);
						}
					}
				}
			}
		}
	}

	private void LoadNPCs(XmlElement root)
	{
		XmlNodeList elementsByTagName = root.GetElementsByTagName("NPCs");
		foreach (XmlNode item in elementsByTagName)
		{
			if (!(item.Name == "NPCs"))
			{
				continue;
			}
			foreach (XmlNode childNode in item.ChildNodes)
			{
				if (!(childNode.Name == "NPC"))
				{
					continue;
				}
				foreach (XmlAttribute attribute2 in childNode.Attributes)
				{
					if (!(attribute2.Name == "Spawn_Position"))
					{
						continue;
					}
					string[] array = attribute2.InnerText.Split(',');
					Vector3 vector = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
					NPC_Base[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(NPC_Base)) as NPC_Base[];
					NPC_Base[] array3 = array2;
					foreach (NPC_Base nPC_Base in array3)
					{
						if (nPC_Base.m_SpawnPosition == vector)
						{
							nPC_Base.LoadGame((XmlElement)childNode);
							break;
						}
					}
				}
			}
		}
		XmlNodeList elementsByTagName2 = root.GetElementsByTagName("DestroyedNPCs");
		foreach (XmlNode item2 in elementsByTagName2)
		{
			if (!(item2.Name == "DestroyedNPCs"))
			{
				continue;
			}
			foreach (XmlNode childNode2 in item2.ChildNodes)
			{
				if (!(childNode2.Name == "DeadNPC"))
				{
					continue;
				}
				string attribute = ((XmlElement)childNode2).GetAttribute("Spawn_Position");
				string[] array4 = attribute.Split(',');
				Vector3 vector2 = new Vector3(float.Parse(array4[0]), float.Parse(array4[1]), float.Parse(array4[2]));
				NPC_Base[] array5 = UnityEngine.Object.FindObjectsOfType(typeof(NPC_Base)) as NPC_Base[];
				NPC_Base[] array6 = array5;
				foreach (NPC_Base nPC_Base2 in array6)
				{
					if (nPC_Base2.m_SpawnPosition == vector2)
					{
						UnityEngine.Object.Destroy(nPC_Base2.gameObject);
						break;
					}
				}
			}
		}
	}

	private void LoadProjectiles(XmlElement root)
	{
		XmlNodeList elementsByTagName = root.GetElementsByTagName("Projectiles");
		foreach (XmlNode item in elementsByTagName)
		{
			if (!(item.Name == "Projectiles"))
			{
				continue;
			}
			foreach (XmlNode childNode in item.ChildNodes)
			{
				if (!(childNode.Name == "Projectile"))
				{
					continue;
				}
				foreach (XmlAttribute attribute in childNode.Attributes)
				{
					if (attribute.Name == "Class")
					{
						ProjectileBase projectileBase = null;
						switch (attribute.InnerText)
						{
						case "ProjectileMiniRPGRocket":
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_MiniRPGProjectilePrefab, Vector3.zero, Quaternion.identity) as GameObject;
							projectileBase = gameObject.GetComponent<ProjectileBase>();
							break;
						}
						case "Projectile_BoxRobotRocket":
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_BoxRobotRocketProjectilePrefab, Vector3.zero, Quaternion.identity) as GameObject;
							projectileBase = gameObject.GetComponent<ProjectileBase>();
							break;
						}
						case "ProjectileCrossbow":
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_CrossbowProjectilePrefab, Vector3.zero, Quaternion.identity) as GameObject;
							projectileBase = gameObject.GetComponent<ProjectileBase>();
							break;
						}
						case "ProjectilePlasma":
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_PlasmaRifleProjectilePrefab, Vector3.zero, Quaternion.identity) as GameObject;
							projectileBase = gameObject.GetComponent<ProjectileBase>();
							break;
						}
						}
						if (projectileBase != null)
						{
							projectileBase.LoadGame((XmlElement)childNode);
						}
					}
				}
			}
		}
	}

	private void LoadGrenades(XmlElement root)
	{
		XmlNodeList elementsByTagName = root.GetElementsByTagName("Grenades");
		foreach (XmlNode item in elementsByTagName)
		{
			if (!(item.Name == "Grenades"))
			{
				continue;
			}
			foreach (XmlNode childNode in item.ChildNodes)
			{
				if (!(childNode.Name == "Grenade"))
				{
					continue;
				}
				foreach (XmlAttribute attribute in childNode.Attributes)
				{
					if (attribute.Name == "GrenadeType")
					{
						GrenadeFrag grenadeFrag = null;
						switch (attribute.InnerText)
						{
						case "Frag":
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_FragGrenadePrefab, Vector3.zero, Quaternion.identity) as GameObject;
							grenadeFrag = gameObject.GetComponent<GrenadeFrag>();
							break;
						}
						case "EMP":
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_EMPGrenadePrefab, Vector3.zero, Quaternion.identity) as GameObject;
							grenadeFrag = gameObject.GetComponent<GrenadeFrag>();
							break;
						}
						case "Concussion":
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_ConcussionGrenadePrefab, Vector3.zero, Quaternion.identity) as GameObject;
							grenadeFrag = gameObject.GetComponent<GrenadeFrag>();
							break;
						}
						}
						grenadeFrag.LoadGame((XmlElement)childNode);
					}
				}
			}
		}
	}

	private void LoadMines(XmlElement root)
	{
		for (LinkedListNode<Mine> linkedListNode = Mine.m_Mines.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			UnityEngine.Object.Destroy(linkedListNode.Value.gameObject);
		}
		XmlNodeList elementsByTagName = root.GetElementsByTagName("Mines");
		foreach (XmlNode item in elementsByTagName)
		{
			if (!(item.Name == "Mines"))
			{
				continue;
			}
			foreach (XmlNode childNode in item.ChildNodes)
			{
				if (!(childNode.Name == "Mine"))
				{
					continue;
				}
				foreach (XmlAttribute attribute in childNode.Attributes)
				{
					if (attribute.Name == "DamageType")
					{
						Mine mine = null;
						switch (attribute.InnerText)
						{
						case "Explosive":
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_FragMinePrefab, Vector3.zero, Quaternion.identity) as GameObject;
							mine = gameObject.GetComponent<Mine>();
							break;
						}
						case "EMP":
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_EMPMinePrefab, Vector3.zero, Quaternion.identity) as GameObject;
							mine = gameObject.GetComponent<Mine>();
							break;
						}
						case "Concussion":
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_ConcussionMinePrefab, Vector3.zero, Quaternion.identity) as GameObject;
							mine = gameObject.GetComponent<Mine>();
							break;
						}
						}
						mine.LoadGame((XmlElement)childNode);
					}
				}
			}
		}
	}

	private void LoadPickups(XmlElement root)
	{
		XmlNodeList elementsByTagName = root.GetElementsByTagName("SpawnedPickups");
		foreach (XmlNode item in elementsByTagName)
		{
			if (!(item.Name == "SpawnedPickups"))
			{
				continue;
			}
			foreach (XmlNode childNode in item.ChildNodes)
			{
				if (!(childNode.Name == "Pickup"))
				{
					continue;
				}
				GameObject gameObject = null;
				foreach (XmlNode attribute2 in childNode.Attributes)
				{
					switch (attribute2.Name)
					{
					case "WeaponType":
					{
						WeaponType weaponType = (WeaponType)(int)Enum.Parse(typeof(WeaponType), attribute2.InnerText);
						gameObject = UnityEngine.Object.Instantiate(Globals.m_PlayerController.m_WeaponList[(int)weaponType]) as GameObject;
						WeaponBase component = gameObject.GetComponent<WeaponBase>();
						component.Drop(Vector3.zero);
						break;
					}
					case "Position":
						if (gameObject != null)
						{
							string[] array = attribute2.InnerText.Split(',');
							gameObject.transform.position = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
						}
						break;
					case "Rotation":
						if (gameObject != null)
						{
							string[] array = attribute2.InnerText.Split(',');
							gameObject.transform.rotation = new Quaternion(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
						}
						break;
					}
				}
			}
		}
		XmlNodeList elementsByTagName2 = root.GetElementsByTagName("DestroyedPickups");
		foreach (XmlNode item2 in elementsByTagName2)
		{
			if (!(item2.Name == "DestroyedPickups"))
			{
				continue;
			}
			foreach (XmlNode childNode2 in item2.ChildNodes)
			{
				if (!(childNode2.Name == "Pickup"))
				{
					continue;
				}
				string attribute = ((XmlElement)childNode2).GetAttribute("Spawn_Position");
				string[] array2 = attribute.Split(',');
				Vector3 vector = new Vector3(float.Parse(array2[0]), float.Parse(array2[1]), float.Parse(array2[2]));
				for (LinkedListNode<InteractiveObject_Pickup> linkedListNode = InteractiveObject_Pickup.m_LevelPickups.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					if (linkedListNode.Value.m_SpawnPosition == vector)
					{
						linkedListNode.Value.MarkForDelete();
						break;
					}
				}
			}
		}
	}

	private void Update()
	{
		bool bActiveGame = m_GameState == GameState.FadeOutGameLoad || m_GameState == GameState.Game || m_GameState == GameState.FadeBlackToGameLoad;
		bool bActiveHacking = Globals.m_This != null && Globals.m_HackingGlobals != null && Globals.m_HackingGlobals.m_ActiveTerminal != null;
		bool bIsPaused = Time.timeScale == 0f;
		bool bHUDEnabled = (Globals.m_HUD == null || !Globals.m_HUD.enabled || !Globals.m_HUD.m_Showing);
		bool bIsSpeaking = Globals.m_ConversationSystem != null && Globals.m_ConversationSystem.IsSpeaking();
		
		bool bTakedownActive = false;
		if (Globals.m_PlayerController != null)
		{
			bTakedownActive = Globals.m_PlayerController.m_CameraMode == PlayerController.CameraMode.Takedown;
		}

		bool bClimbingLadder = false;
		if (Globals.m_PlayerController != null)
		{
			bClimbingLadder = Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Ladder_ClimbDown || Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Ladder_ClimbUp || Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Ladder_EnterBottom || Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Ladder_EnterTop || Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Ladder_ExitBottom || Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Ladder_ExitTop || Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Ladder_Idle;
		}
		
		bool bScreenLocked = ((bActiveGame && !bIsPaused && !bHUDEnabled && !bIsSpeaking) || bTakedownActive || bClimbingLadder);
		
		if (Screen.lockCursor != bScreenLocked)
		{
			Screen.lockCursor = bScreenLocked;
		}

		if (m_SaveTextTimer > 0f)
		{
			m_SaveTextTimer -= Time.realtimeSinceStartup - m_SaveTextTime;
			m_SaveTextTime = Time.realtimeSinceStartup;
			if (m_SaveTextTimer < 1f)
			{
				Color primaryObjectiveColor = Globals.m_HUD.m_PrimaryObjectiveColor;
				primaryObjectiveColor.a = m_SaveTextTimer / 1f;
				Globals.m_HUD.m_SavingText.SetColor(primaryObjectiveColor);
				Globals.m_HUD.m_SavingTextText.SetColor(new Color(1f, 1f, 1f, primaryObjectiveColor.a));
			}
			if (m_SaveTextTimer <= 0f)
			{
				m_SaveTextTimer = 0f;
				Globals.m_HUD.m_SavingText.gameObject.SetActiveRecursively(false);
			}
		}
		if ((m_LoadSaveAfterLevelLoad || m_LoadSceneSetupAfterLevelLoad || m_JustLoadedLevel) && Globals.m_PlayerController != null && m_FrameNum == 2)
		{
			if (m_LoadSaveAfterLevelLoad)
			{
				XmlNodeList elementsByTagName = m_SaveXML.GetElementsByTagName("PlayerSpecific");
				XmlElement root = (XmlElement)elementsByTagName[0];
				Globals.m_This.LoadGame(root);
				Globals.m_Inventory.LoadGame(root);
				elementsByTagName = m_SaveXML.GetElementsByTagName("MediaLogs");
				XmlElement xmlElement = (XmlElement)elementsByTagName[0];
				foreach (XmlElement childNode in xmlElement.ChildNodes)
				{
					int num = int.Parse(childNode.GetAttribute("Location"));
					MediaLog mediaLog = new MediaLog();
					mediaLog.m_MediaType = (MediaType)(int)Enum.Parse(typeof(MediaType), childNode.GetAttribute("MediaType"));
					mediaLog.m_Read = bool.Parse(childNode.GetAttribute("Read"));
					mediaLog.m_To = childNode.GetAttribute("To");
					mediaLog.m_From = childNode.GetAttribute("From");
					mediaLog.m_Subject = childNode.GetAttribute("Subject");
					mediaLog.m_Body = childNode.GetAttribute("Body");
					m_This.m_MediaLogs[num].AddLast(mediaLog);
				}
				Globals.m_PlayerController.LoadGame(root);
			}
			if (m_LoadSceneSetupAfterLevelLoad || m_LoadSaveAfterLevelLoad)
			{
				XmlNodeList elementsByTagName = m_SaveXML.GetElementsByTagName(Application.loadedLevelName);
				if (elementsByTagName.Count > 0)
				{
					m_SaveLoaded = true;
					XmlElement root = (XmlElement)elementsByTagName[0];
					GameManager.OnLoadGame(root);
					LoadEnemies(root);
					LoadNPCs(root);
					LoadProjectiles(root);
					LoadGrenades(root);
					LoadMines(root);
					LoadPickups(root);
				}
			}
			if (Globals.m_Autosave && !m_LoadSaveAfterLevelLoad)
			{
				GameSaved(false);
			}
			m_LoadSaveAfterLevelLoad = false;
			m_LoadSceneSetupAfterLevelLoad = false;
			m_JustLoadedLevel = false;
		}
		switch (m_GameState)
		{
		case GameState.Startup:
			Update_Startup();
			break;
		case GameState.Menu:
			Update_Menu();
			break;
		case GameState.FadeBlackToGameLoad:
			Update_FadeBlackToGameLoad();
			break;
		case GameState.FadeInGameLoad:
			Update_FadeInGameLoad();
			break;
		case GameState.FadeOutGameLoad:
			Update_FadeOutGameLoad();
			break;
		case GameState.Game:
			Update_Game();
			break;
		case GameState.Paused:
			Update_Paused();
			break;
		case GameState.FadeBlackToMenuLoad:
			Update_FadeBlackToMenuLoad();
			break;
		case GameState.FadeInMenuLoad:
			Update_FadeInMenuLoad();
			break;
		case GameState.FadeOutMenuLoad:
			Update_FadeOutMenuLoad();
			break;
		}
		m_FrameNum++;
	}

	private void Update_Startup()
	{
		Application.LoadLevelAdditive(m_MenuScene);
		m_GameState = GameState.Menu;
	}

	private void Update_Menu()
	{
	}

	private void Update_FadeBlackToGameLoad()
	{
		m_BlackTextureColor.a += 1.5f * Time.deltaTime;
		if (m_BlackTextureColor.a >= 1f)
		{
			m_BlackTextureColor.a = 1f;
			if (Globals.m_ResolutionRank == Globals.ResolutionRank.Low)
			{
				m_LoadingTexture = Resources.Load("GUI/LoadingImages/LowRes/" + m_LevelLoadingImage) as Texture2D;
			}
			else
			{
				m_LoadingTexture = Resources.Load("GUI/LoadingImages/HighRes/" + m_LevelLoadingImage) as Texture2D;
			}
			m_LoadingTextureColor.a = 0f;
			m_RenderLoadingTexture = true;
			m_GameState = GameState.FadeInGameLoad;
		}
	}

	private void Update_FadeInGameLoad()
	{
		m_LoadingTextureColor.a += 1f * Time.deltaTime;
		if (m_LoadingTextureColor.a >= 1f)
		{
			m_LoadingTextureColor.a = 1f;
			m_RenderBlackTexture = false;
			SoundManager.StopAll();
			UIManager.instance.ClearCameras();
			Application.LoadLevel(m_EmptyScene);
			m_GameState = GameState.FadeOutGameLoad;
			UIManager.instance.blockInput = false;
		}
	}

	private void Update_FadeOutGameLoad()
	{
		if (Globals.m_This != null && (m_LevelToLoad == "Turntable" || m_FrameNum >= 4))
		{
			m_LoadingTextureColor.a -= 1f * Time.deltaTime;
		}
		else
		{
			m_LoadingTextureColor.a = 1f;
		}
		if (m_LoadingTextureColor.a <= 0f)
		{
			m_LoadingTextureColor.a = 0f;
			m_RenderLoadingTexture = false;
			m_LoadingTexture = null;
			m_GameState = GameState.Game;
		}
	}

	private void Update_Game()
	{
	}

	private void Update_Paused()
	{
	}

	private void Update_FadeBlackToMenuLoad()
	{
		m_BlackTextureColor.a += 1.5f * Time.deltaTime;
		if (m_BlackTextureColor.a >= 1f)
		{
			m_BlackTextureColor.a = 1f;
			if (Globals.m_ResolutionRank == Globals.ResolutionRank.Low)
			{
				m_LoadingTexture = Resources.Load("GUI/LoadingImages/LowRes/" + m_LevelLoadingImage) as Texture2D;
			}
			else
			{
				m_LoadingTexture = Resources.Load("GUI/LoadingImages/HighRes/" + m_LevelLoadingImage) as Texture2D;
			}
			m_LoadingTextureColor.a = 0f;
			m_RenderLoadingTexture = true;
			m_GameState = GameState.FadeInMenuLoad;
		}
	}

	private void Update_FadeInMenuLoad()
	{
		m_LoadingTextureColor.a += 1f * Time.deltaTime;
		if (m_LoadingTextureColor.a >= 1f)
		{
			m_LoadingTextureColor.a = 1f;
			m_RenderBlackTexture = false;
			UIManager.instance.ClearCameras();
			UIManager.instance.blockInput = true;
			Application.LoadLevel(m_EmptyScene);
			m_GameState = GameState.FadeOutMenuLoad;
			UIManager.instance.blockInput = false;
		}
	}

	private void Update_FadeOutMenuLoad()
	{
		LoadPlatformSpecificMenu();
		m_LoadingTextureColor.a -= 1f * Time.deltaTime;
		if (m_LoadingTextureColor.a <= 0f)
		{
			m_LoadingTextureColor.a = 0f;
			m_RenderLoadingTexture = false;
			m_LoadingTexture = null;
			m_GameState = GameState.Menu;
		}
	}

	private void OnGUI()
	{
		if (Event.current.type == EventType.Repaint)
		{
			if (m_RenderBlackTexture)
			{
				GUI.color = m_BlackTextureColor;
				GUI.DrawTexture(Globals.m_FullScreenRect, Globals.m_This.m_WhiteTexture);
				GUI.color = Color.white;
			}
			if (m_RenderLoadingTexture)
			{
				GUI.color = m_LoadingTextureColor;
				GUI.DrawTexture(Globals.m_FullScreenRect, m_LoadingTexture, ScaleMode.ScaleAndCrop);
				GUI.color = Color.white;
			}
		}
	}

	public static PackedSprite CreateFullscreenQuad(FullscreenQuadType type = FullscreenQuadType.Normal, float zOffset = 0.1f)
	{
		PackedSprite packedSprite;
		switch (type)
		{
		case FullscreenQuadType.Multiply:
			packedSprite = UnityEngine.Object.Instantiate(Globals.m_This.m_FullscreenQuadMult) as PackedSprite;
			break;
		case FullscreenQuadType.Add:
			packedSprite = UnityEngine.Object.Instantiate(Globals.m_This.m_FullscreenQuadAdd) as PackedSprite;
			break;
		default:
			packedSprite = UnityEngine.Object.Instantiate(Globals.m_This.m_FullscreenQuad) as PackedSprite;
			break;
		}
		packedSprite.transform.parent = Globals.m_HUD.transform.parent.transform;
		packedSprite.transform.localPosition = new Vector3(0f, 0f, Globals.m_HUD.transform.localPosition.z + zOffset);
		packedSprite.transform.localRotation = Quaternion.identity;
		packedSprite.width = Globals.m_FullScreenRect.width * Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel;
		packedSprite.height = Globals.m_FullScreenRect.height * Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel;
		return packedSprite;
	}
}
