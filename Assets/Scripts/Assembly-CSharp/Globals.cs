using System;
using System.Collections.Generic;
using System.Xml;
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

	public enum StoryProgressionVars
	{
		PanamaHubStarted = 0,
		PanamaVisitedDoctor = 1,
		PanamaDoctorMissionComplete = 2,
		PanamaVisitedLimb = 3,
		PanamaVisitedCobra = 4,
		PanamaMadeDealWithGang = 5,
		PanamaKilledGangLeader = 6,
		PanamaDestroyedRiezol = 7,
		PanamaLimbMissionComplete = 8,
		PanamaVisitedHavok = 9,
		PanamaVIPFound = 10,
		PanamaVIPMissionComplete = 11,
		PanamaVTOLMissionStarted = 12,
		PanamaVTOLMissionComplete = 13,
		NYCHubStarted = 14,
		NYCVisitedHotelReception = 15,
		NYCVisitedAnna = 16,
		NYCVisitedIsaac = 17,
		NYCImplantWasRemoved = 18,
		NYCVisitedHackers = 19,
		NYCHaveUplink = 20,
		NYCPlantedUplink = 21,
		NYCHackedBank = 22,
		NYCAnnaWasAbducted = 23,
		NYCEscapedBelltowerRaid = 24,
		NYCVisitedPoliceBarricade = 25,
		NYCDiscoveredUndergroundRoute = 26,
		NYCInfiltratedNYSE = 27,
		NYCDisarmedBomb = 28,
		Total = 29
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

	public const int m_LayerHostilityZone = 23;

	public const int m_RaycastJustCover = 256;

	public const int m_RaycastEnvironment = 257;

	public const int m_RaycastEnemies = 512;

	public const int m_RaycastPlayer = 16384;

	public const int m_RaycastInteractiveObject = 65536;

	public const int m_RaycastTapCollision = 131072;

	public const int m_RaycastNPC = 2097152;

	public const int m_RaycastSecurityCamera = 4194304;

	public const int m_RaycastPhysicalObjects = 6374145;

	public const float m_PlayerCoverVisualDot = 0.3f;

	public const int m_TotalQuickslotItems = 4;

	public const int m_TotalQuickslotWeapons = 4;

	public static Globals m_This = null;

	public int m_VersionMajor;

	public int m_VersionMinor;

	public int m_VersionMicro = 1;

	public static MemoryRank m_MemoryRank = MemoryRank.High;

	public static PowerRank m_PowerRank = PowerRank.High;

	public static ResolutionRank m_ResolutionRank = ResolutionRank.High;

	public static EffectsRank m_EffectsRank = EffectsRank.NotUsable;

	public static string[] m_LocationNames = new string[6] { "Prologue", "Costa Rica", "Panama", "Moscow", "Australia", "New York" };

	public static Rect m_FullScreenRect;

	public static Vector2 m_ScreenCenter;

	public static Color m_ClearWhite = new Color(1f, 1f, 1f, 0f);

	public static PlayerController m_PlayerController;

	public static Inventory m_Inventory;

	public static bool m_DisplayPurchaseConfirmation = true;

	public static float m_PlayerYOffset = 0.1f;

	public static AugmentCloaking m_AugmentCloaking;

	public static AugmentSeeThroughWalls m_AugmentSeeThroughWalls;

	public static AugmentArmor m_AugmentArmor;

	public static CameraController m_CameraController;

	public static MenuRoot m_MenuRoot = null;

	public static HUDRoot m_HUDRoot = null;

	public static MainHUD m_HUD = null;

	public static AIDirector m_AIDirector = null;

	public static ConversationSystem m_ConversationSystem = null;

	public static InteractiveObjectManager m_InteractiveObjectManager = null;

	public static Hacking_Globals m_HackingGlobals = null;

	public static AugmentationData m_AugmentationData = null;

	public static bool m_DisableTapToMove = true;

	public static bool m_Bloom = false;

	public static bool m_AutoRotate = false;

	public static bool m_Autosave = true;

	public static float m_TapTimeLimit = 0.2f;

	public static bool m_ShowFPS = false;

	public static bool m_GodMode = false;

	public static Transform m_PrimaryObjective = null;

	public static Transform m_SecondaryObjective = null;

	public Color m_PassiveGlow = new Color(0f, 1f, 0f, 1f);

	public Color m_AlarmedGlow = new Color(1f, 1f, 0f, 1f);

	public Color m_HostileGlow = new Color(1f, 0f, 0f, 1f);

	public Color m_BrightHUD = new Color(0.929f, 0.655f, 0.137f, 1f);

	public Color m_Equipped = Color.green;

	public Color m_DarkHUD = new Color(0.196f, 0.137f, 0f, 1f);

	public Color m_BlackHUD = new Color(0f, 0f, 0f, 0.19f);

	public TextAsset m_MissionFile;

	public static Mission[] m_Missions = null;

	public static List<int> m_TrackedMissions = new List<int>();

	private static bool[] m_StoryProgressionVars = new bool[29];

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

	public GameObject m_StunGunEffect;

	public static int m_MasterTextureLimit = 0;

	public static int m_MouseSensitivity = 4;
	
	public static bool m_InvertCamera = false;

	public static bool GetStoryProgressionVar(StoryProgressionVars v)
	{
		return m_StoryProgressionVars[(int)v];
	}

	public static void SetStoryProgressionVar(StoryProgressionVars v, bool b)
	{
		m_StoryProgressionVars[(int)v] = b;
	}

	public static void ResetStoryProgressionVars()
	{
		foreach (int value in Enum.GetValues(typeof(StoryProgressionVars)))
		{
			if (value != 29)
			{
				m_StoryProgressionVars[value] = false;
			}
		}
	}

	private void Awake()
	{
		m_This = this;
		DetermineDeviceRank();
		LoadMissions();

#if UNITY_IPHONE
		if (iPhone.generation == iPhoneGeneration.iPad2Gen || iPhone.generation == iPhoneGeneration.iPhone4S)
		{
			Debug.Log("High fillrate device detected, enabling FSAA.");
			QualitySettings.antiAliasing = 2;
		}
		if (iPhone.generation == iPhoneGeneration.iPad2Gen || iPhone.generation == iPhoneGeneration.iPhone4 || iOSInfo.isIPadMini)
		{
			Debug.Log("Low RAM device detected, halving textures.");
			QualitySettings.DecreaseLevel();
		}
#endif
	}

	private void Start()
	{
		SoundManager.Initialize(base.gameObject);
		SoundManager.LoadSoundBank("Global.bnk");
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

	private void OnApplicationPause()
	{
	}

	private void OnApplicationFocus()
	{
	}

	private void OnApplicationQuit()
	{
	}

	private void DetermineDeviceRank()
	{
#if UNITY_IPHONE
		if (iPhone.generation != iPhoneGeneration.iPad1Gen && iPhone.generation != iPhoneGeneration.iPad2Gen && iPhone.generation != iPhoneGeneration.iPad3Gen && iPhone.generation != iPhoneGeneration.iPhone3G && iPhone.generation != iPhoneGeneration.iPhone3GS && iPhone.generation != iPhoneGeneration.iPhone4 && iPhone.generation != iPhoneGeneration.iPhone4S && iPhone.generation != iPhoneGeneration.iPhone && iPhone.generation != iPhoneGeneration.iPodTouch1Gen && iPhone.generation != iPhoneGeneration.iPodTouch2Gen && iPhone.generation != iPhoneGeneration.iPodTouch3Gen && iPhone.generation != iPhoneGeneration.iPodTouch4Gen)
		{
		}
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

	public static int GetEnumValue<EnumType>(string name)
	{
		if (name == null || name == string.Empty)
		{
			return -1;
		}
		string[] names = Enum.GetNames(typeof(EnumType));
		for (int i = 0; i < names.Length; i++)
		{
			if (string.Compare(name, names[i], true) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public static float BiLerp(float v0, float v1, float v2, float v3, float x, float y)
	{
		return Mathf.Lerp(Mathf.Lerp(v0, v1, x), Mathf.Lerp(v2, v3, x), y);
	}

	private static int CalculateSplashDamage(int damage, float distSqr, float maxRadiusSqr)
	{
		return (int)((float)damage * Mathf.Clamp01(1f - distSqr / maxRadiusSqr));
	}

	public static bool Approximately(float a, float b, float epsilon)
	{
		return a + epsilon >= b && a - epsilon <= b;
	}

	public static void DealSplashDamage(DamageData data, float minRadius, float maxRadius)
	{
		float sqrMagnitude = (m_PlayerController.transform.position - data.m_SourceLocation).sqrMagnitude;
		float num = maxRadius * maxRadius;
		float num2 = minRadius * minRadius;
		Ray ray = new Ray(data.m_SourceLocation, m_PlayerController.GetChestLocation() - data.m_SourceLocation);
		int damage = data.m_Damage;
		RaycastHit hitInfo;
		if (sqrMagnitude <= num2)
		{
			data.m_Damage = CalculateSplashDamage(damage, sqrMagnitude, num);
			m_PlayerController.TakeDamage(data);
		}
		else if (sqrMagnitude <= num && Physics.Raycast(ray, out hitInfo, maxRadius * 1.1f, 16641) && hitInfo.collider.gameObject.layer == 14)
		{
			data.m_Damage = CalculateSplashDamage(damage, sqrMagnitude, num);
			m_PlayerController.TakeDamage(data);
		}
		if (data.m_DamageType == DamageType.Explosive || data.m_DamageType == DamageType.EMP)
		{
			for (LinkedListNode<Mine> linkedListNode = Mine.m_Mines.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				sqrMagnitude = (linkedListNode.Value.transform.position - data.m_SourceLocation).sqrMagnitude;
				if (sqrMagnitude <= num)
				{
					linkedListNode.Value.Explode();
				}
			}
		}
		if (data.m_DamageType == DamageType.Explosive || data.m_DamageType == DamageType.Concussion)
		{
			for (int i = 0; i < 8; i++)
			{
				LinkedListNode<Enemy_Base> linkedListNode2 = null;
				LinkedListNode<Enemy_Base> linkedListNode3 = m_AIDirector.m_Squads[i].m_EnemyUnits.First;
				while (linkedListNode3 != null)
				{
					sqrMagnitude = (linkedListNode3.Value.transform.position - data.m_SourceLocation).sqrMagnitude;
					if (sqrMagnitude <= num2)
					{
						data.m_Damage = CalculateSplashDamage(damage, sqrMagnitude, num);
						linkedListNode3.Value.TakeDamage(data);
					}
					else if (sqrMagnitude <= num)
					{
						ray.direction = linkedListNode3.Value.GetChestLocation() - data.m_SourceLocation;
						if (Physics.Raycast(ray, out hitInfo, maxRadius * 1.1f, 769) && hitInfo.collider.gameObject.layer == 9)
						{
							data.m_Damage = CalculateSplashDamage(damage, sqrMagnitude, num);
							if (linkedListNode3.Value.TakeDamage(data))
							{
								linkedListNode3 = linkedListNode2;
							}
						}
					}
					if (linkedListNode3 == null)
					{
						linkedListNode2 = null;
						linkedListNode3 = m_AIDirector.m_Squads[i].m_EnemyUnits.First;
					}
					else
					{
						linkedListNode2 = linkedListNode3;
						linkedListNode3 = linkedListNode3.Next;
					}
				}
			}
			LinkedListNode<NPC_Base> linkedListNode4 = m_AIDirector.m_NPCs.First;
			LinkedListNode<NPC_Base> linkedListNode5 = null;
			while (linkedListNode4 != null)
			{
				sqrMagnitude = (linkedListNode4.Value.transform.position - data.m_SourceLocation).sqrMagnitude;
				if (sqrMagnitude <= num2)
				{
					data.m_Damage = CalculateSplashDamage(damage, sqrMagnitude, num);
					linkedListNode4.Value.TakeDamage(data);
				}
				else if (sqrMagnitude <= num)
				{
					ray.direction = linkedListNode4.Value.GetChestLocation() - data.m_SourceLocation;
					if (Physics.Raycast(ray, out hitInfo, maxRadius * 1.1f, 2097409) && hitInfo.collider.gameObject.layer == 21)
					{
						data.m_Damage = CalculateSplashDamage(damage, sqrMagnitude, num);
						if (linkedListNode4.Value.TakeDamage(data))
						{
							linkedListNode4 = linkedListNode5;
						}
					}
				}
				if (linkedListNode4 == null)
				{
					linkedListNode5 = null;
					linkedListNode4 = m_AIDirector.m_NPCs.First;
				}
				else
				{
					linkedListNode5 = linkedListNode4;
					linkedListNode4 = linkedListNode4.Next;
				}
			}
		}
		if (data.m_DamageType != DamageType.EMP && data.m_DamageType != DamageType.Explosive)
		{
			return;
		}
		for (LinkedListNode<Turret> linkedListNode6 = m_AIDirector.m_Turrets.First; linkedListNode6 != null; linkedListNode6 = linkedListNode6.Next)
		{
			sqrMagnitude = (linkedListNode6.Value.transform.position - data.m_SourceLocation).sqrMagnitude;
			if (sqrMagnitude <= num)
			{
				data.m_Damage = CalculateSplashDamage(damage, sqrMagnitude, num);
				linkedListNode6.Value.TakeDamage(data);
			}
		}
		for (LinkedListNode<Sentry> linkedListNode7 = m_AIDirector.m_Sentries.First; linkedListNode7 != null; linkedListNode7 = linkedListNode7.Next)
		{
			sqrMagnitude = (linkedListNode7.Value.transform.position - data.m_SourceLocation).sqrMagnitude;
			if (sqrMagnitude <= num)
			{
				data.m_Damage = CalculateSplashDamage(damage, sqrMagnitude, num);
				linkedListNode7.Value.TakeDamage(data);
			}
		}
		for (LinkedListNode<BoxRobot> linkedListNode8 = m_AIDirector.m_BoxRobots.First; linkedListNode8 != null; linkedListNode8 = linkedListNode8.Next)
		{
			sqrMagnitude = (linkedListNode8.Value.transform.position - data.m_SourceLocation).sqrMagnitude;
			if (sqrMagnitude <= num)
			{
				data.m_Damage = CalculateSplashDamage(damage, sqrMagnitude, num);
				linkedListNode8.Value.TakeDamage(data);
			}
		}
		for (LinkedListNode<SecurityCamera> linkedListNode9 = m_AIDirector.m_SecurityCameras.First; linkedListNode9 != null; linkedListNode9 = linkedListNode9.Next)
		{
			sqrMagnitude = (linkedListNode9.Value.transform.position - data.m_SourceLocation).sqrMagnitude;
			if (sqrMagnitude <= num)
			{
				linkedListNode9.Value.Destroy();
			}
		}
	}

	public XmlElement SaveGame(XmlElement root, XmlDocument doc)
	{
		XmlElement xmlElement = (XmlElement)root.AppendChild(doc.CreateElement("Globals"));
		foreach (int value in Enum.GetValues(typeof(StoryProgressionVars)))
		{
			if (value != 29)
			{
				XmlElement xmlElement2 = (XmlElement)xmlElement.AppendChild(doc.CreateElement("StoryProgressionVar"));
				xmlElement2.SetAttribute("Name", ((StoryProgressionVars)value).ToString());
				xmlElement2.SetAttribute("Value", m_StoryProgressionVars[value].ToString());
			}
		}
		xmlElement.SetAttribute("GodMode", m_GodMode.ToString());
		xmlElement.SetAttribute("ShowFPS", m_ShowFPS.ToString());
		return xmlElement;
	}

	public XmlElement LoadGame(XmlElement root)
	{
		XmlNodeList elementsByTagName = root.GetElementsByTagName("Globals");
		foreach (XmlNode item in elementsByTagName)
		{
			if (!(item.Name == "Globals"))
			{
				continue;
			}
			foreach (XmlNode childNode in item.ChildNodes)
			{
				switch (childNode.Name)
				{
				case "StoryProgressionVar":
				{
					string text = string.Empty;
					foreach (XmlAttribute attribute in childNode.Attributes)
					{
						switch (attribute.Name)
						{
						case "Name":
							text = attribute.InnerText;
							break;
						case "Value":
							if (text != string.Empty)
							{
								StoryProgressionVars storyProgressionVars = (StoryProgressionVars)(int)Enum.Parse(typeof(StoryProgressionVars), text);
								m_StoryProgressionVars[(int)storyProgressionVars] = bool.Parse(attribute.InnerText);
								text = string.Empty;
							}
							break;
						}
					}
					break;
				}
				}
			}
			foreach (XmlAttribute attribute2 in item.Attributes)
			{
				switch (attribute2.Name)
				{
				case "GodMode":
					m_GodMode = bool.Parse(attribute2.InnerText);
					break;
				case "ShowFPS":
					m_ShowFPS = bool.Parse(attribute2.InnerText);
					if (m_ShowFPS)
					{
						m_PlayerController.m_Camera.GetComponent<HUDFPS>().enabled = true;
					}
					else
					{
						m_PlayerController.m_Camera.GetComponent<HUDFPS>().enabled = false;
					}
					break;
				}
			}
			return (XmlElement)item;
		}
		return null;
	}

	private void LoadMissions()
	{
		m_Missions = new Mission[8];
		if (m_MissionFile == null)
		{
			return;
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(m_MissionFile.text);
		int num = 0;
		Location location = Location.None;
		for (XmlNode xmlNode = xmlDocument.FirstChild.NextSibling.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			location = (Location)GetEnumValue<Location>(xmlNode.Name);
			if (location != Location.None)
			{
				XmlNodeList childNodes = xmlNode.FirstChild.ChildNodes;
				if (childNodes != null && childNodes.Count > 0)
				{
					for (int i = 0; i < childNodes.Count; i++)
					{
						FillInMissions(childNodes[i], location, true, i);
					}
				}
				childNodes = xmlNode.FirstChild.NextSibling.ChildNodes;
				if (childNodes != null && childNodes.Count > 0)
				{
					for (int j = 0; j < childNodes.Count; j++)
					{
						FillInMissions(childNodes[j], location, false, j);
					}
				}
			}
		}
	}

	private void FillInMissions(XmlNode missionNode, Location location, bool primary, int localIndex)
	{
		int enumValue = GetEnumValue<MissionID>(missionNode.Attributes["ID"].Value);
		m_Missions[enumValue] = new Mission();
		m_Missions[enumValue].m_Location = location;
		m_Missions[enumValue].m_Name = missionNode.Attributes["Name"].Value;
		m_Missions[enumValue].m_Primary = primary;
		m_Missions[enumValue].m_LocalIndex = localIndex;
		m_Missions[enumValue].m_Tracked = false;
		m_Missions[enumValue].m_Status = MissionStatus.Unknown;
		XmlNodeList childNodes = missionNode.ChildNodes;
		if (childNodes != null && childNodes.Count > 0)
		{
			m_Missions[enumValue].m_Subs = new SubMission[childNodes.Count];
			for (int i = 0; i < childNodes.Count; i++)
			{
				m_Missions[enumValue].m_Subs[i] = new SubMission();
				m_Missions[enumValue].m_Subs[i].m_Status = MissionStatus.Unknown;
				m_Missions[enumValue].m_Subs[i].m_Name = childNodes[i].Attributes["Name"].Value;
				m_Missions[enumValue].m_Subs[i].m_Desc = childNodes[i].Attributes["Desc"].Value;
			}
		}
	}

	public static void ClearTrackedMissions()
	{
		m_TrackedMissions.Clear();
	}

	public static void TrackMission(int missionID, bool track)
	{
		for (int i = 0; i < m_TrackedMissions.Count; i++)
		{
			if (m_TrackedMissions[i] == missionID)
			{
				if (!track)
				{
					m_TrackedMissions.RemoveAt(i);
					m_Missions[missionID].m_Tracked = false;
				}
				return;
			}
		}
		if (track)
		{
			m_TrackedMissions.Add(missionID);
			m_Missions[missionID].m_Tracked = true;
		}
	}

	public static void SetMissionStatus(int missionID, MissionStatus status)
	{
		if (missionID > -1 && missionID < 8)
		{
			m_Missions[missionID].m_Status = status;
		}
	}

	public static void SetSubMissionStatus(int missionID, int subMissionID, MissionStatus status)
	{
		if (missionID > -1 && missionID < 8 && subMissionID >= 0 && subMissionID < m_Missions[missionID].m_Subs.Length)
		{
			m_Missions[missionID].m_Subs[subMissionID].m_Status = status;
		}
	}

	public static void SetMissionTransforms(int missionID, Transform[] transforms = null)
	{
		if (missionID <= -1 || missionID >= 8)
		{
			return;
		}
		m_Missions[missionID].m_Transforms = null;
		if (transforms != null && transforms.Length > 0)
		{
			m_Missions[missionID].m_Transforms = new Transform[transforms.Length];
			for (int i = 0; i < transforms.Length; i++)
			{
				m_Missions[missionID].m_Transforms[i] = transforms[i];
			}
		}
	}

	public static bool CanPause()
	{
		return GameManager.m_This.m_GameState == GameManager.GameState.Game && m_HUD.m_Showing && m_PlayerController.gameObject.activeSelf && !m_HUD.m_PauseButton.IsHidden() && !m_ConversationSystem.IsSpeaking() && m_PlayerController.m_CameraMode != PlayerController.CameraMode.Takedown;
	}
}
