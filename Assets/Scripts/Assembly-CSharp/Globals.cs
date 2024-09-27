using UnityEngine;

public class Globals : MonoBehaviour
{
	public enum MemoryRank
	{
		Low = 0,
		High = 1
	}

	public enum PowerRank
	{
		Low = 0,
		High = 1
	}

	public enum ResolutionRank
	{
		Low = 0,
		High = 1
	}

	public enum EffectsRank
	{
		Usable = 0,
		NotUsable = 1
	}

	public const int m_LayerCover = 8;

	public const int m_LayerEnemies = 9;

	public const int m_LayerGUIMenu2D = 10;

	public const int m_LayerGUIMenu3D = 11;

	public const int m_LayerGUIGame2D = 12;

	public const int m_LayerGUIGame3D = 13;

	public const int m_LayerPlayer = 14;

	public const int m_LayerFirstPersonWeapon = 15;

	public const int m_LayerInteractiveObject = 16;

	public const int m_LayerTapCollision = 17;

	public const int m_LayerGrenade = 18;

	public const int m_LayerBulletWhiz = 19;

	public const int m_LayerNPC = 21;

	public const int m_LayerSecurityCamera = 22;

	public const int m_RaycastJustCover = 256;

	public const int m_RaycastEnvironment = 257;

	public const int m_RaycastEnemies = 512;

	public const int m_RaycastPlayer = 16384;

	public const int m_RaycastInteractiveObject = 65536;

	public const int m_RaycastTapCollision = 131072;

	public const int m_RaycastNPC = 2097152;

	public const int m_RaycastSecurityCamera = 4194304;

	public const float m_PlayerCoverVisualDot = 0.3f;

	public static Globals m_This = null;

	public static MemoryRank m_MemoryRank = MemoryRank.High;

	public static PowerRank m_PowerRank = PowerRank.High;

	public static ResolutionRank m_ResolutionRank = ResolutionRank.High;

	public static EffectsRank m_EffectsRank = EffectsRank.NotUsable;

	public static Rect m_FullScreenRect;

	public static Vector2 m_ScreenCenter;

	public static Color m_ClearWhite = new Color(1f, 1f, 1f, 0f);

	public static PlayerController m_PlayerController;

	public static Inventory m_Inventory;

	public static AugmentCloaking m_AugmentCloaking;

	public static CameraController m_CameraController;

	public static MenuRoot m_MenuRoot = null;

	public static HUDRoot m_HUDRoot = null;

	public static MainHUD m_HUD = null;

	public static AIDirector m_AIDirector = null;

	public static ConversationSystem m_ConversationSystem = null;

	public static InteractiveObjectManager m_InteractiveObjectManager = null;

	public static Hacking_Globals m_HackingGlobals = null;

	public static AugmentationData m_AugmentationData = null;

	public static bool m_DisableTapToMove = false;

	public static bool m_Bloom = false;

	public static bool m_AutoRotate = false;

	public static float m_TapTimeLimit = 0.2f;

	public static bool m_ShowFPS = false;

	public static Transform m_PrimaryObjective = null;

	public static Transform m_SecondaryObjective = null;

	public Color m_PassiveGlow = new Color(0f, 1f, 0f, 1f);

	public Color m_AlarmedGlow = new Color(1f, 1f, 0f, 1f);

	public Color m_HostileGlow = new Color(1f, 0f, 0f, 1f);

	public static bool m_PanamaHubStarted = false;

	public static bool m_PanamaVisitedDoctor = false;

	public static bool m_PanamaDoctorMissionComplete = false;

	public static bool m_PanamaVisitedLimb = false;

	public static bool m_PanamaVisitedCobra = false;

	public static bool m_PanamaDestroyedRiezol = false;

	public static bool m_PanamaLimbMissionComplete = false;

	public static bool m_PanamaVisitedHavok = false;

	public static bool m_PanamaVIPFound = false;

	public static bool m_PanamaVIPMissionComplete = false;

	public static bool m_PanamaVTOLMissionStarted = false;

	public static bool m_PanamaVTOLMissionComplete = false;

	public Texture2D m_WhiteTexture;

	public GameObject m_FragExplosion;

	public GameObject m_EMPExplosion;

	public GameObject m_ConcussionExplosion;

	public GameObject m_ShadowProjectorObject;

	public PackedSprite m_FullscreenQuad;

	public PackedSprite m_FullscreenQuadMult;

	public PackedSprite m_FullscreenQuadAdd;

	public GameObject m_EnemyTargetIndicator;

	public static PackedSprite m_DamageDirectionIndicator = null;

	public static PackedSprite m_DamageDirectionArrow = null;

	private void Awake()
	{
		m_This = this;
		DetermineDeviceRank();
#if UNITY_IPHONE
		if (iPhone.generation == iPhoneGeneration.iPad2Gen || iPhone.generation == iPhoneGeneration.iPhone4S)
		{
			QualitySettings.antiAliasing = 2;
		}
#else
		QualitySettings.antiAliasing = 2;
#endif
		Shader.globalMaximumLOD = 1000;
	}

	private void Start()
	{
		m_FullScreenRect = new Rect(0f, 0f, Screen.width, Screen.height);
		m_ScreenCenter = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
		if (m_ResolutionRank == ResolutionRank.Low)
		{
			GameObject gameObject = Resources.Load("GUI/LowRes/HUD_DamageDirectionIndicator") as GameObject;
			m_DamageDirectionIndicator = gameObject.GetComponent<PackedSprite>();
			gameObject = Resources.Load("GUI/LowRes/HUD_DamageDirectionArrow") as GameObject;
			m_DamageDirectionArrow = gameObject.GetComponent<PackedSprite>();
		}
		else
		{
			GameObject gameObject = Resources.Load("GUI/HighRes/HUD_DamageDirectionIndicator") as GameObject;
			m_DamageDirectionIndicator = gameObject.GetComponent<PackedSprite>();
			gameObject = Resources.Load("GUI/HighRes/HUD_DamageDirectionArrow") as GameObject;
			m_DamageDirectionArrow = gameObject.GetComponent<PackedSprite>();
		}
	}

	private void DetermineDeviceRank()
	{
#if UNITY_IPHONE
		if (iPhone.generation != iPhoneGeneration.iPad1Gen && iPhone.generation != iPhoneGeneration.iPad2Gen && iPhone.generation != iPhoneGeneration.iPad3Gen)
		{
			if (iPhone.generation == iPhoneGeneration.iPhone3G)
			{
				m_ResolutionRank = ResolutionRank.Low;
			}
			else if (iPhone.generation == iPhoneGeneration.iPhone3GS)
			{
				m_ResolutionRank = ResolutionRank.Low;
			}
			else if (iPhone.generation == iPhoneGeneration.iPhone4)
			{
				m_ResolutionRank = ResolutionRank.Low;
			}
			else if (iPhone.generation == iPhoneGeneration.iPhone4S)
			{
				m_ResolutionRank = ResolutionRank.Low;
			}
			else if (iPhone.generation == iPhoneGeneration.iPhone)
			{
				m_ResolutionRank = ResolutionRank.Low;
			}
			else if (iPhone.generation == iPhoneGeneration.iPodTouch1Gen)
			{
				m_ResolutionRank = ResolutionRank.Low;
			}
			else if (iPhone.generation == iPhoneGeneration.iPodTouch2Gen)
			{
				m_ResolutionRank = ResolutionRank.Low;
			}
			else if (iPhone.generation == iPhoneGeneration.iPodTouch3Gen)
			{
				m_ResolutionRank = ResolutionRank.Low;
			}
			else if (iPhone.generation == iPhoneGeneration.iPodTouch4Gen)
			{
				m_ResolutionRank = ResolutionRank.Low;
			}
		}
#else
		m_ResolutionRank = ResolutionRank.High;
#endif
	}

	public static CharacterBase FindCharacterBase(Transform node)
	{
		CharacterBase characterBase = null;
		while (node != null)
		{
			characterBase = node.gameObject.GetComponent<CharacterBase>();
			if (characterBase != null)
			{
				break;
			}
			node = node.parent;
		}
		return characterBase;
	}

	public static SecurityCamera FindSecurityCameraComponent(Transform node)
	{
		SecurityCamera securityCamera = null;
		while (node != null)
		{
			securityCamera = node.gameObject.GetComponent<SecurityCamera>();
			if (securityCamera != null)
			{
				break;
			}
			node = node.parent;
		}
		return securityCamera;
	}
}
