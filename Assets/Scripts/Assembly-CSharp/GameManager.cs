using System;
using Fabric;
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
	}

	public static GameManager m_This;

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

	public static int m_LastLevelCombatRifleCurrentAmmo;

	public static int m_LastLevelCombatRifleAmmo;

	public static int m_LastLevelCrossbowCurrentAmmo;

	public static int m_LastLevelCrossbowAmmo;

	public static int m_LastLevelShotgunCurrentAmmo;

	public static int m_LastLevelShotgunAmmo;

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

	private void Awake()
	{
		m_This = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
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

	public static void LoadHUD()
	{
		if (!(m_This == null) && Globals.m_HUDRoot == null)
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

	public static void ReloadCurrentLevel()
	{
		Time.timeScale = 1f;
		FabricManager.Instance.Stop(0f);
		LoadLevel(m_This.m_LastLoadedLevel);
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
		Globals.m_AIDirector.ClearAll();
		Globals.m_InteractiveObjectManager.ClearAll();
		m_This.m_LastLoadedLevel = LevelID;
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

	public static void GamePaused(bool Paused)
	{
		if (Paused)
		{
			m_This.m_GameState = GameState.Paused;
		}
		else
		{
			m_This.m_GameState = GameState.Game;
		}
	}

	private void Update()
	{
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
			UIManager.instance.ClearCameras();
			Application.LoadLevel(m_EmptyScene);
			m_GameState = GameState.FadeOutGameLoad;
			UIManager.instance.blockInput = false;
		}
	}

	private void Update_FadeOutGameLoad()
	{
		m_LoadingTextureColor.a -= 1f * Time.deltaTime;
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
		if (UnityEngine.Event.current.type == EventType.Repaint)
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
