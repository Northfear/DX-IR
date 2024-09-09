using System;
using System.Collections.Generic;
using Fabric;
using PreviewLabs;
using UnityEngine;

public class MainHUD : MonoBehaviour
{
	[Serializable]
	public class QuickSlotWeapon
	{
		[HideInInspector]
		public Vector3 m_OriginalLocalPosition = Vector3.zero;

		public UIButton m_Root;

		public PackedSprite m_WeaponIcon;

		public PackedSprite m_ActiveSprite;

		public PackedSprite m_InactiveSprite;

		public SpriteText m_ClipText;

		public SpriteText m_AmmoText;

		public Collider m_Collider;
	}

	[Serializable]
	public class QuickSlotItem
	{
		[HideInInspector]
		public Vector3 m_OriginalLocalPosition = Vector3.zero;

		public UIButton m_Root;

		public PackedSprite m_ItemIcon;

		public PackedSprite m_ActiveSprite;

		public PackedSprite m_InactiveSprite;

		public SpriteText m_Quantity;

		public Collider m_Collider;
	}

	public struct GrenadeIndicatorInfo
	{
		public PackedSprite m_Root;

		public Transform m_Pivot;

		public PackedSprite m_Glow;

		public PackedSprite m_Arrow;
	}

	public struct ObjectiveMarkerInfo
	{
		public PackedSprite m_Root;

		public Transform m_Pivot;

		public PackedSprite m_Arrow;

		public PackedSprite m_Plaque;

		public SpriteText m_Number;

		public SpriteText m_Distance;
	}

	public enum CoverFlipButtonSide
	{
		Left = 0,
		Right = 1
	}

	public const int m_MaxEnergy = 3;

	public const int m_TotalQuickWeaponSlots = 4;

	public const int m_TotalGrenadeMenuSlots = 3;

	public const int m_TotalQuickItemSlots = 4;

	[HideInInspector]
	public bool m_Showing;

	public SpriteText m_CurrentHealth;

	public SpriteText m_CurrentArmor;

	public SpriteText m_FPS;

	public Color m_BrightHUDColor;

	public Color m_DarkHUDColor;

	public Color m_EnergyFullColor;

	public Color m_EnergyEmptyColor;

	public Color m_EnergyChargingColor;

	public PackedSprite[] m_HealthUnits;

	private float m_CurrentChargeTimeOffset;

	private float m_ChargeTimeFlashDuration = 0.5f;

	private float m_ChargeTimeCycleDuration = 1.25f;

	public PackedSprite[] m_EnergyContainers = new PackedSprite[3];

	public PackedSprite[] m_EnergyBars = new PackedSprite[3];

	private float m_CurrentFlashTime;

	private float m_FlashTimeTotalDuration = 0.8f;

	public Color m_BackgroundNormal = new Color(0f, 0f, 0f, 0.19f);

	public Color m_BackgroundFlash = new Color(0.5f, 0f, 0f, 1f);

	private Color m_CurrentFlashColor = Color.white;

	public PackedSprite m_EnergyParent;

	public PackedSprite m_EnergyBackground;

	public PackedSprite m_CoverButtonParent;

	public UIButton m_CoverButtonEnter;

	public UIButton m_CoverButtonExit;

	public PackedSprite m_StanceBackground;

	public UIButton m_StanceStanding;

	public UIButton m_StanceCrouching;

	public UIPanel m_HUDPanel;

	public GameObject m_PassThruButton;

	public UIButton m_CoverFlipButtonLeft;

	public UIButton m_CoverFlipButtonRight;

	public UIButton m_CoverFlipInnerButtonLeft;

	public UIButton m_CoverFlipInnerButtonRight;

	public UIButton m_CoverFlipOuterButtonLeft;

	public UIButton m_CoverFlipOuterButtonRight;

	public float m_QuickMenuHoldTime = 0.2f;

	public float m_QuickSlotAnimateInDuration = 0.25f;

	public float m_QuickSlotAnimateInDelay = 0.1f;

	public float m_QuickSlotAnimateOutDuration = 0.2f;

	public GameObject m_DraggableWeaponParent;

	public QuickSlotWeapon[] m_QuickSlotWeapon = new QuickSlotWeapon[4];

	private int m_QuickWeaponPointerID;

	private int m_QuickWeaponPointerCameraID;

	private bool m_QuickWeaponsOpen;

	private bool m_ReloadPressed;

	private float m_ReloadHoldTime;

	private int m_PreviousWeaponSelected = -1;

	public PackedSprite[] m_GrenadeMenuBackground = new PackedSprite[3];

	public UIButton[] m_GrenadeMenuSlot = new UIButton[3];

	public SpriteText[] m_GrenadeMenuSlotText = new SpriteText[3];

	private int m_GrenadeMenuPointerID;

	private int m_GrenadeMenuPointerCameraID;

	private bool m_GrenadeMenuOpen;

	private bool m_GrenadeMenuPressed;

	private float m_GrenadeMenuHoldTime = 0.2f;

	private float m_GrenadeMenuHoldTimer;

	private int m_PreviousGrenadeSelected = -1;

	public GameObject[] m_GrenadeIcon = new GameObject[3];

	public UIButton m_WeaponReload;

	public SpriteText m_CurrentAmmo;

	public SpriteText m_TotalAmmo;

	public GameObject m_CombatRifle;

	public GameObject m_Crossbow;

	public GameObject m_Shotgun;

	private float m_GrenadeWarningTimer = -1f;

	public UIButton m_GrenadeButton;

	public SpriteText m_GrenadeQuantity;

	public GameObject m_DraggableItemParent;

	public QuickSlotItem[] m_QuickSlotItem = new QuickSlotItem[4];

	private int m_QuickItemPointerID;

	private int m_QuickItemPointerCameraID;

	private bool m_QuickItemsOpen;

	private bool m_ItemPressed;

	private float m_ItemHoldTime;

	private int m_PreviousItemSelected = -1;

	private float m_ItemWarningTimer = -1f;

	public UIButton m_ItemButton;

	public SpriteText m_CurrentItemQuantity;

	public GameObject m_CloakingIcon;

	public GameObject m_EnergyBarIcon;

	public GameObject m_BoozeIcon;

	private float m_AudioPauseEventDelay = -1f;

	private bool m_Customizing;

	public Collider m_CustomizationBlocker;

	public UIButton[] m_DraggableElements;

	private Vector3[] m_DraggablePositions;

	private Vector3[] m_DraggableDefaultPositions;

	public Renderer m_RadarMaskRenderer;

	public Renderer m_RadarIconRenderer;

	public Vector2 m_RadarWorldRange = new Vector2(40f, 31.2f);

	private float m_RadarWorldRangeSqr;

	public Color m_HostileRadarColor = new Color(0.93f, 0.12f, 0.12f, 1f);

	public Color m_AlarmedRadarColor = new Color(0.82f, 0.4f, 0.05f, 1f);

	public Color m_PassiveRadarColor = new Color(0.18f, 0.87f, 0.2f, 1f);

	public PackedSprite m_RadarRoot;

	public PackedSprite m_EnemyIconPrefab;

	public PackedSprite m_TurretIconPrefab;

	private LinkedList<PackedSprite> m_EnemyIcons = new LinkedList<PackedSprite>();

	private LinkedList<PackedSprite> m_TurretIcons = new LinkedList<PackedSprite>();

	private Vector2 ObjectDir;

	private Vector2 ToObject;

	private Vector2 ToObjectRelative;

	public GameObject m_DraggableRadarParent;

	public PackedSprite m_WarningIndicator;

	public SpriteText m_WarningIndicatorText;

	private float m_WarningIndicatorAlpha;

	private Color m_WarningIndicatorColor = Globals.m_ClearWhite;

	private float m_MinArrowScale = 0.5f;

	private float m_MaxArrowScale = 1f;

	public GameObject m_GrenadeIndicatorPrefab;

	public int m_GrenadeIndicatorBuffer = 55;

	private LinkedList<GrenadeIndicatorInfo> m_GrenadeIndicators = new LinkedList<GrenadeIndicatorInfo>();

	public GameObject m_ObjectiveMarkerPrefab;

	public Color m_PrimaryObjectiveColor = new Color(0.93f, 0.65f, 0.14f, 1f);

	public Color m_SecondaryObjectiveColor = new Color(0f, 0.89f, 0.72f, 1f);

	private int m_MarkerBuffer = 55;

	private ObjectiveMarkerInfo m_PrimaryObjectiveInfo;

	private ObjectiveMarkerInfo m_SecondaryObjectiveInfo;

	public Texture2D m_EmptyPortraitTexture;

	public GameObject m_DraggableCommLink;

	public GameObject m_CommLinkParent;

	public SimpleSprite m_CommPortrait;

	public PackedSprite m_CommNameBackground;

	public SpriteText m_CommName;

	public PackedSprite m_CommSubtitleBackground;

	public SpriteText m_CommSubtitle;

	private bool m_CommScrambling;

	private int m_ScrambleLetterIndex;

	private string m_TargetCommName = string.Empty;

	private float[] m_LetterTimers;

	private float m_WordTimer = -1f;

	public PackedSprite m_PlayerSubtitleBackground;

	public SpriteText m_PlayerSubtitle;

	public PackedSprite m_EnergyWarningBackground;

	public PackedSprite m_EnergyWarningIcon;

	public SpriteText m_EnergyWarningText;

	private float m_EnergyWarningTimer = -1f;

	public PackedSprite m_AmmoWarningBackground;

	public PackedSprite m_AmmoWarningIcon;

	public SpriteText m_AmmoWarningText;

	public PackedSprite m_AmmoWarningIconOverWeapon;

	private float m_WarningOverWeaponTimer;

	private void Awake()
	{
		Globals.m_HUD = this;
		m_RadarWorldRangeSqr = Mathf.Max(m_RadarWorldRange.x, m_RadarWorldRange.y);
		m_RadarWorldRangeSqr *= m_RadarWorldRangeSqr;
	}

	private void Start()
	{
		base.transform.localPosition = new Vector3(0f, 0f, 15f);
		TurnOnCoverButton(false);
		TurnOnCoverFlipButton(false, CoverFlipButtonSide.Left);
		TurnOnCoverFlipButton(false, CoverFlipButtonSide.Right);
		TurnOnCoverInnerFlipButton(false, CoverFlipButtonSide.Left);
		TurnOnCoverInnerFlipButton(false, CoverFlipButtonSide.Right);
		TurnOnCoverOuterFlipButton(false, CoverFlipButtonSide.Left);
		TurnOnCoverOuterFlipButton(false, CoverFlipButtonSide.Right);
		SetupQuickWeapons();
		CloseQuickWeapons(true);
		SetupQuickItems();
		CloseQuickItems(true);
		CloseGrenadeMenu(true);
		SetWeaponIcon(Globals.m_PlayerController.GetEquippedWeaponType());
		SetItemIcon();
		SetGrenadeIcon();
		SetStance();
		SetupObjectiveMarkers();
		m_Showing = true;
		m_Customizing = false;
		m_CustomizationBlocker.enabled = m_Customizing;
		if (m_DraggableElements != null)
		{
			if (m_DraggableElements.Length > 0)
			{
				m_DraggableDefaultPositions = new Vector3[m_DraggableElements.Length];
				m_DraggablePositions = new Vector3[m_DraggableElements.Length];
			}
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < m_DraggableElements.Length; i++)
			{
				if (m_DraggableElements[i] != null)
				{
					m_DraggableElements[i].AddDragDropDelegate(OnEZDragDrop);
					m_DraggableElements[i].collider.enabled = false;
					m_DraggableDefaultPositions[i] = m_DraggableElements[i].transform.localPosition;
					if (PreviewLabs.PlayerPrefs.HasKey(m_DraggableElements[i].name))
					{
						string[] array = PreviewLabs.PlayerPrefs.GetString(m_DraggableElements[i].name).Split(',');
						zero.x = float.Parse(array[0]);
						zero.y = float.Parse(array[1]);
						zero.z = float.Parse(array[2]);
						m_DraggableElements[i].transform.localPosition = zero;
					}
					m_DraggablePositions[i] = m_DraggableElements[i].transform.localPosition;
				}
			}
		}
		ReAdjustQuickWeaponMenu();
		ReAdjustQuickItemMenu();
		ReAdjustCommLink();
		HidePlayerSubtitle();
		ReAdjustWarningIndicatorPosition();
		CloseCommLink();
		if (m_RadarMaskRenderer != null)
		{
			m_RadarMaskRenderer.sharedMaterial.renderQueue = 3010;
		}
		if (m_RadarIconRenderer != null)
		{
			m_RadarIconRenderer.sharedMaterial.renderQueue = 3020;
		}
		m_EnergyWarningBackground.gameObject.SetActiveRecursively(false);
	}

	private void Update()
	{
		if (!m_QuickWeaponsOpen && m_ReloadPressed)
		{
			m_ReloadHoldTime += Time.deltaTime;
			if (m_ReloadHoldTime >= m_QuickMenuHoldTime)
			{
				OpenQuickWeapons();
			}
		}
		if (m_QuickWeaponsOpen)
		{
			UpdateQuickWeaponsInput();
		}
		if (!m_QuickItemsOpen && m_ItemPressed)
		{
			m_ItemHoldTime += Time.deltaTime;
			if (m_ItemHoldTime >= m_QuickMenuHoldTime)
			{
				OpenQuickItems();
			}
		}
		if (m_QuickItemsOpen)
		{
			UpdateQuickItemsInput();
		}
		if (!m_GrenadeMenuOpen && m_GrenadeMenuPressed)
		{
			m_GrenadeMenuHoldTimer += Time.deltaTime;
			if (m_GrenadeMenuHoldTimer >= m_GrenadeMenuHoldTime)
			{
				OpenGrenadeMenu();
			}
		}
		if (m_GrenadeMenuOpen)
		{
			UpdateGrenadesInput();
		}
		if (m_AudioPauseEventDelay >= 0f)
		{
			m_AudioPauseEventDelay -= Time.deltaTime;
			if (m_AudioPauseEventDelay < 0f)
			{
				EventManager.Instance.PostEvent("Pause", EventAction.PauseSound, null, base.gameObject);
			}
		}
		UpdatePlayerRadar();
		if (m_Customizing)
		{
			ReAdjustWarningIndicatorPosition();
			ReAdjustCommLink();
		}
		else if (m_CommScrambling)
		{
			UpdateCommScramble();
		}
		UpdateFlashColor();
	}

	private void UpdateFlashColor()
	{
		m_CurrentFlashTime += Time.deltaTime;
		if (m_CurrentFlashTime >= m_FlashTimeTotalDuration)
		{
			m_CurrentFlashTime = 0f;
		}
		if (m_CurrentFlashTime < m_FlashTimeTotalDuration * 0.5f)
		{
			m_CurrentFlashColor = Color.Lerp(m_BackgroundNormal, m_BackgroundFlash, m_CurrentFlashTime / (m_FlashTimeTotalDuration * 0.5f));
		}
		else
		{
			m_CurrentFlashColor = Color.Lerp(m_BackgroundFlash, m_BackgroundNormal, Mathf.InverseLerp(m_FlashTimeTotalDuration * 0.5f, m_FlashTimeTotalDuration, m_CurrentFlashTime));
		}
	}

	private void LateUpdate()
	{
		UpdateGrenadeIndicators();
		UpdateWarningIndicator();
		UpdateEnergyBackgroundFlash();
		UpdateAmmoWarnings();
		UpdateEnergyWarning();
		UpdateItemButtonFlash();
		UpdateGrenadeButtonFlash();
		UpdateObjectiveMarkers();
	}

	public void Display(bool display, bool animate, bool ForCustomization = false)
	{
		if (Globals.m_HUD == null || Globals.m_HUD.m_HUDPanel == null)
		{
			return;
		}
		if (display)
		{
			if (animate)
			{
				Globals.m_HUD.m_HUDPanel.BringIn();
			}
			else
			{
				Globals.m_HUD.m_HUDPanel.BringInImmediate();
			}
			SetStance();
			SetWeaponIcon(Globals.m_PlayerController.GetEquippedWeaponType());
			SetItemIcon();
			SetGrenadeIcon();
			CloseQuickWeapons(true);
			CloseQuickItems(true);
			CloseGrenadeMenu(true);
			TurnOnCoverButton(false);
			TurnOnCoverFlipButton(false, CoverFlipButtonSide.Left);
			TurnOnCoverFlipButton(false, CoverFlipButtonSide.Right);
			TurnOnCoverInnerFlipButton(false, CoverFlipButtonSide.Left);
			TurnOnCoverInnerFlipButton(false, CoverFlipButtonSide.Right);
			TurnOnCoverOuterFlipButton(false, CoverFlipButtonSide.Left);
			TurnOnCoverOuterFlipButton(false, CoverFlipButtonSide.Right);
			m_WarningIndicatorAlpha = 0f;
			if (CommLinkDialog.LinkOpen())
			{
				OpenCommLinkImmediate(false);
			}
			else
			{
				CloseCommLink();
			}
			if (CommLinkDialog.CharacterTalking())
			{
				DisplayLinkSubtitleImmediate();
			}
			else
			{
				HideCommLinkSubtitle();
			}
			if (CommLinkDialog.PlayerTalking())
			{
				DisplayPlayerSubtitleImmediate();
			}
			else
			{
				HidePlayerSubtitle();
			}
			if (m_EnergyWarningTimer < 0f)
			{
				m_EnergyWarningBackground.gameObject.SetActiveRecursively(false);
			}
			m_Showing = true;
		}
		else
		{
			if (animate)
			{
				Globals.m_HUD.m_HUDPanel.Dismiss();
			}
			else
			{
				Globals.m_HUD.m_HUDPanel.DismissImmediate();
			}
			m_Showing = false;
		}
	}

	public void EnablePassThruInput(bool enable)
	{
		if ((bool)m_PassThruButton)
		{
			m_PassThruButton.SetActiveRecursively(enable);
		}
	}

	public void SetCurrentAmmo(int ammo)
	{
		if ((bool)m_CurrentAmmo)
		{
			m_CurrentAmmo.Text = ammo.ToString();
		}
	}

	public void SetTotalAmmo(int ammo)
	{
		if ((bool)m_TotalAmmo)
		{
			m_TotalAmmo.Text = ammo.ToString();
		}
	}

	public void SetCurrentHealth(int health)
	{
		if ((bool)m_CurrentHealth)
		{
			m_CurrentHealth.Text = health.ToString();
			m_CurrentHealth.Color = m_BrightHUDColor;
			m_CurrentHealth.Hide(!m_Showing);
		}
		int num = health / 10;
		if (num == 0)
		{
			num = 1;
		}
		for (int i = 0; i < m_HealthUnits.Length; i++)
		{
			if (i < num)
			{
				m_HealthUnits[i].Color = m_BrightHUDColor;
			}
			else
			{
				m_HealthUnits[i].Color = m_DarkHUDColor;
			}
			m_HealthUnits[i].Hide(!m_Showing);
		}
	}

	public void SetCurrentEnergy(float TotalEnergy, bool Depleting = false)
	{
		Color color = ((!Depleting) ? m_EnergyChargingColor : m_EnergyEmptyColor);
		m_CurrentChargeTimeOffset += Time.deltaTime;
		if (m_CurrentChargeTimeOffset >= m_ChargeTimeCycleDuration)
		{
			m_CurrentChargeTimeOffset = 0f;
		}
		color = ((m_CurrentChargeTimeOffset <= m_ChargeTimeFlashDuration) ? Color.Lerp(m_EnergyFullColor, color, m_CurrentChargeTimeOffset / m_ChargeTimeFlashDuration) : ((!(m_CurrentChargeTimeOffset <= m_ChargeTimeFlashDuration * 2f)) ? m_EnergyFullColor : Color.Lerp(color, m_EnergyFullColor, Mathf.InverseLerp(m_ChargeTimeFlashDuration, m_ChargeTimeFlashDuration * 2f, m_CurrentChargeTimeOffset))));
		int num = (int)Globals.m_PlayerController.GetMaxEnergy();
		for (int i = 0; i < 3; i++)
		{
			if (i >= num)
			{
				m_EnergyContainers[i].Hide(true);
				m_EnergyBars[i].Hide(true);
				m_EnergyBars[i].Hide(true);
				continue;
			}
			m_EnergyContainers[i].Hide(false);
			m_EnergyBars[i].Hide(false);
			m_EnergyBars[i].Hide(false);
			if (TotalEnergy >= (float)(i + 1))
			{
				m_EnergyContainers[i].SetColor(m_EnergyFullColor);
				m_EnergyBars[i].SetColor(m_EnergyFullColor);
				m_EnergyBars[i].transform.localScale = Vector3.one;
			}
			else if (Depleting)
			{
				if (TotalEnergy > (float)i)
				{
					if (TotalEnergy - Mathf.Floor(TotalEnergy) < 0.5f)
					{
						m_EnergyContainers[i].SetColor(color);
						m_EnergyBars[i].SetColor(color);
					}
					else
					{
						m_EnergyContainers[i].SetColor(m_EnergyFullColor);
						m_EnergyBars[i].SetColor(m_EnergyFullColor);
					}
					m_EnergyBars[i].transform.localScale = new Vector3(TotalEnergy - Mathf.Floor(TotalEnergy), 1f, 1f);
				}
				else
				{
					m_EnergyContainers[i].SetColor(m_EnergyEmptyColor);
					m_EnergyBars[i].SetColor(m_EnergyEmptyColor);
					m_EnergyBars[i].transform.localScale = new Vector3(0f, 1f, 1f);
				}
			}
			else if (TotalEnergy > (float)i)
			{
				m_EnergyContainers[i].SetColor(color);
				m_EnergyBars[i].SetColor(color);
				m_EnergyBars[i].transform.localScale = new Vector3(TotalEnergy - Mathf.Floor(TotalEnergy), 1f, 1f);
			}
			else
			{
				m_EnergyContainers[i].SetColor(m_EnergyEmptyColor);
				m_EnergyBars[i].SetColor(m_EnergyEmptyColor);
				m_EnergyBars[i].transform.localScale = new Vector3(0f, 1f, 1f);
			}
		}
	}

	private void UpdateEnergyBackgroundFlash()
	{
		if (Globals.m_PlayerController.GetCurrentEnergy() <= 0f)
		{
			Color currentFlashColor = m_CurrentFlashColor;
			currentFlashColor.a = m_EnergyParent.Color.a * currentFlashColor.a;
			m_EnergyBackground.SetColor(currentFlashColor);
		}
		else
		{
			Color backgroundNormal = m_BackgroundNormal;
			backgroundNormal.a = m_EnergyParent.Color.a * backgroundNormal.a;
			m_EnergyBackground.SetColor(backgroundNormal);
		}
	}

	public void SetCurrentArmor(int armor)
	{
		if ((bool)m_CurrentArmor)
		{
			m_CurrentArmor.Text = armor.ToString();
			m_CurrentArmor.Color = m_BrightHUDColor;
		}
	}

	public void ReloadTapped()
	{
		Globals.m_PlayerController.ReloadButtonTapped();
	}

	public void ItemButtonTapped()
	{
		if (m_QuickItemsOpen)
		{
			return;
		}
		if (Globals.m_Inventory.m_CurrentItem == ItemType.Aug_Cloaking)
		{
			Globals.m_PlayerController.ToggleCloaking();
		}
		else if (Globals.m_Inventory.m_CurrentItem == ItemType.Booze)
		{
			if (Globals.m_Inventory.m_Booze > 0)
			{
				if (!Globals.m_PlayerController.AtMaxHealth())
				{
					Globals.m_PlayerController.UseItem(ItemType.Booze);
					Globals.m_Inventory.m_Booze--;
				}
				else
				{
					EventManager.Instance.PostEvent("UI_Toggle", EventAction.PlaySound, null, base.gameObject);
				}
			}
			else
			{
				m_ItemWarningTimer = 0f;
				EventManager.Instance.PostEvent("UI_Error", EventAction.PlaySound, null, base.gameObject);
			}
		}
		else if (Globals.m_Inventory.m_CurrentItem == ItemType.EnergyBar)
		{
			if (Globals.m_Inventory.m_EnergyBars > 0)
			{
				if (!Globals.m_PlayerController.AtMaxEnergy())
				{
					Globals.m_PlayerController.UseItem(ItemType.EnergyBar);
					Globals.m_Inventory.m_EnergyBars--;
				}
				else
				{
					EventManager.Instance.PostEvent("UI_Toggle", EventAction.PlaySound, null, base.gameObject);
				}
			}
			else
			{
				m_ItemWarningTimer = 0f;
				EventManager.Instance.PostEvent("UI_Error", EventAction.PlaySound, null, base.gameObject);
			}
		}
		SetItemIcon();
	}

	public void ReloadPressed(POINTER_INFO ptr)
	{
		m_ReloadHoldTime = 0f;
		m_ReloadPressed = true;
		m_QuickWeaponPointerID = ptr.id;
		m_QuickWeaponPointerCameraID = UIManager.instance.GetCameraID(ptr.camera);
	}

	public void ItemButtonPressed(POINTER_INFO ptr)
	{
		m_ItemHoldTime = 0f;
		m_ItemPressed = true;
		m_QuickItemPointerID = ptr.id;
		m_QuickItemPointerCameraID = UIManager.instance.GetCameraID(ptr.camera);
	}

	public void GrenadeButtonPressed(POINTER_INFO ptr)
	{
		m_GrenadeMenuHoldTimer = 0f;
		m_GrenadeMenuPressed = true;
		m_GrenadeMenuPointerID = ptr.id;
		m_GrenadeMenuPointerCameraID = UIManager.instance.GetCameraID(ptr.camera);
	}

	public void UserReleased()
	{
		m_ReloadPressed = false;
		m_ItemPressed = false;
		CloseQuickWeapons(false);
		CloseQuickItems(false);
		CloseGrenadeMenu(false);
	}

	private void SetupQuickWeapons()
	{
		for (int i = 0; i < 4; i++)
		{
			m_QuickSlotWeapon[i].m_OriginalLocalPosition = m_QuickSlotWeapon[i].m_Root.transform.localPosition;
		}
	}

	private void SetupQuickItems()
	{
		for (int i = 0; i < 4; i++)
		{
			m_QuickSlotItem[i].m_OriginalLocalPosition = m_QuickSlotItem[i].m_Root.transform.localPosition;
		}
	}

	private void OpenQuickWeapons()
	{
		if (m_QuickWeaponsOpen)
		{
			return;
		}
		Vector3 begin = Vector3.zero;
		for (int i = 0; i < 4; i++)
		{
			m_QuickSlotWeapon[i].m_Root.gameObject.SetActiveRecursively(true);
			m_QuickSlotWeapon[i].m_Root.SetColor(Globals.m_ClearWhite);
			m_QuickSlotWeapon[i].m_ActiveSprite.SetColor(Globals.m_ClearWhite);
			m_QuickSlotWeapon[i].m_InactiveSprite.SetColor(Globals.m_ClearWhite);
			m_QuickSlotWeapon[i].m_AmmoText.SetColor(Globals.m_ClearWhite);
			m_QuickSlotWeapon[i].m_ClipText.SetColor(Globals.m_ClearWhite);
			if (m_QuickSlotWeapon[i].m_WeaponIcon != null)
			{
				m_QuickSlotWeapon[i].m_WeaponIcon.SetColor(Globals.m_ClearWhite);
			}
			AnimatePosition.Do(m_QuickSlotWeapon[i].m_Root.gameObject, EZAnimation.ANIM_MODE.FromTo, begin, m_QuickSlotWeapon[i].m_OriginalLocalPosition, EZAnimation.sinusOut, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			FadeSpriteAlpha.Do(m_QuickSlotWeapon[i].m_Root, EZAnimation.ANIM_MODE.FromTo, Globals.m_ClearWhite, Color.white, EZAnimation.linear, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			FadeSpriteAlpha.Do(m_QuickSlotWeapon[i].m_ActiveSprite, EZAnimation.ANIM_MODE.FromTo, Globals.m_ClearWhite, Color.white, EZAnimation.linear, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			FadeSpriteAlpha.Do(m_QuickSlotWeapon[i].m_InactiveSprite, EZAnimation.ANIM_MODE.FromTo, Globals.m_ClearWhite, Color.white, EZAnimation.linear, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			FadeTextAlpha.Do(m_QuickSlotWeapon[i].m_ClipText, EZAnimation.ANIM_MODE.FromTo, Globals.m_ClearWhite, Color.white, EZAnimation.linear, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			FadeTextAlpha.Do(m_QuickSlotWeapon[i].m_AmmoText, EZAnimation.ANIM_MODE.FromTo, Globals.m_ClearWhite, Color.white, EZAnimation.linear, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			if (m_QuickSlotWeapon[i].m_WeaponIcon != null)
			{
				FadeSpriteAlpha.Do(m_QuickSlotWeapon[i].m_WeaponIcon, EZAnimation.ANIM_MODE.FromTo, Globals.m_ClearWhite, Color.white, EZAnimation.linear, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			}
			begin = m_QuickSlotWeapon[i].m_OriginalLocalPosition;
		}
		m_QuickSlotWeapon[0].m_ClipText.Text = Globals.m_PlayerController.GetWeaponCurrentClip(0).ToString();
		m_QuickSlotWeapon[0].m_AmmoText.Text = Globals.m_PlayerController.GetWeaponTotalAmmo(0).ToString();
		m_QuickSlotWeapon[1].m_ClipText.Text = Globals.m_PlayerController.GetWeaponCurrentClip(1).ToString();
		m_QuickSlotWeapon[1].m_AmmoText.Text = Globals.m_PlayerController.GetWeaponTotalAmmo(1).ToString();
		m_QuickSlotWeapon[2].m_ClipText.Text = Globals.m_PlayerController.GetWeaponCurrentClip(2).ToString();
		m_QuickSlotWeapon[2].m_AmmoText.Text = Globals.m_PlayerController.GetWeaponTotalAmmo(2).ToString();
		m_QuickWeaponsOpen = true;
		m_PreviousWeaponSelected = -1;
		EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null, base.gameObject);
	}

	private void OpenQuickItems()
	{
		if (m_QuickItemsOpen)
		{
			return;
		}
		Color brightHUDColor = m_BrightHUDColor;
		brightHUDColor.a = 0f;
		Vector3 begin = Vector3.zero;
		for (int i = 0; i < 4; i++)
		{
			m_QuickSlotItem[i].m_Root.gameObject.SetActiveRecursively(true);
			m_QuickSlotItem[i].m_Root.SetColor(brightHUDColor);
			m_QuickSlotItem[i].m_ActiveSprite.SetColor(Globals.m_ClearWhite);
			m_QuickSlotItem[i].m_InactiveSprite.SetColor(Globals.m_ClearWhite);
			m_QuickSlotItem[i].m_Quantity.SetColor(Globals.m_ClearWhite);
			if (m_QuickSlotItem[i].m_ItemIcon != null)
			{
				m_QuickSlotItem[i].m_ItemIcon.SetColor(Globals.m_ClearWhite);
			}
			AnimatePosition.Do(m_QuickSlotItem[i].m_Root.gameObject, EZAnimation.ANIM_MODE.FromTo, begin, m_QuickSlotItem[i].m_OriginalLocalPosition, EZAnimation.sinusOut, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			FadeSpriteAlpha.Do(m_QuickSlotItem[i].m_Root, EZAnimation.ANIM_MODE.FromTo, brightHUDColor, m_BrightHUDColor, EZAnimation.linear, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			FadeSpriteAlpha.Do(m_QuickSlotItem[i].m_ActiveSprite, EZAnimation.ANIM_MODE.FromTo, Globals.m_ClearWhite, Color.white, EZAnimation.linear, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			FadeSpriteAlpha.Do(m_QuickSlotItem[i].m_InactiveSprite, EZAnimation.ANIM_MODE.FromTo, Globals.m_ClearWhite, Color.white, EZAnimation.linear, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			FadeTextAlpha.Do(m_QuickSlotItem[i].m_Quantity, EZAnimation.ANIM_MODE.FromTo, Globals.m_ClearWhite, Color.white, EZAnimation.linear, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			if (m_QuickSlotItem[i].m_ItemIcon != null)
			{
				FadeSpriteAlpha.Do(m_QuickSlotItem[i].m_ItemIcon, EZAnimation.ANIM_MODE.FromTo, Globals.m_ClearWhite, Color.white, EZAnimation.linear, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			}
			begin = m_QuickSlotItem[i].m_OriginalLocalPosition;
		}
		m_QuickSlotItem[1].m_Quantity.Text = Globals.m_Inventory.m_EnergyBars.ToString();
		m_QuickSlotItem[2].m_Quantity.Text = Globals.m_Inventory.m_Booze.ToString();
		m_QuickItemsOpen = true;
		m_PreviousItemSelected = -1;
		EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null, base.gameObject);
	}

	private void OpenGrenadeMenu()
	{
		m_GrenadeMenuBackground[0].gameObject.SetActiveRecursively(true);
		m_GrenadeMenuBackground[1].gameObject.SetActiveRecursively(true);
		m_GrenadeMenuBackground[2].gameObject.SetActiveRecursively(true);
		Color backgroundNormal = m_BackgroundNormal;
		backgroundNormal.a = m_RadarRoot.Color.a * backgroundNormal.a;
		m_GrenadeMenuSlot[0].SetColor(backgroundNormal);
		m_GrenadeMenuSlot[1].SetColor(backgroundNormal);
		m_GrenadeMenuSlot[2].SetColor(backgroundNormal);
		m_GrenadeMenuSlotText[0].Text = Globals.m_Inventory.m_Grenades[0].ToString();
		m_GrenadeMenuSlotText[1].Text = Globals.m_Inventory.m_Grenades[1].ToString();
		m_GrenadeMenuSlotText[2].Text = Globals.m_Inventory.m_Grenades[2].ToString();
		m_GrenadeMenuOpen = true;
		EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null, base.gameObject);
	}

	private void CloseGrenadeMenu(bool ForceClose = false)
	{
		m_GrenadeMenuBackground[0].gameObject.SetActiveRecursively(false);
		m_GrenadeMenuBackground[1].gameObject.SetActiveRecursively(false);
		m_GrenadeMenuBackground[2].gameObject.SetActiveRecursively(false);
		m_GrenadeMenuOpen = false;
		m_GrenadeMenuPressed = false;
		SetGrenadeIcon();
	}

	private void CloseQuickWeapons(bool ForceClose = false)
	{
		if (!m_QuickWeaponsOpen && !ForceClose)
		{
			return;
		}
		Color brightHUDColor = m_BrightHUDColor;
		brightHUDColor.a = 0f;
		for (int i = 0; i < 4; i++)
		{
			FadeSpriteAlpha fadeSpriteAlpha = FadeSpriteAlpha.Do(m_QuickSlotWeapon[i].m_Root, EZAnimation.ANIM_MODE.To, brightHUDColor, EZAnimation.linear, m_QuickSlotAnimateOutDuration, 0f, null, QuickWeaponRootFaded);
			FadeSpriteAlpha.Do(m_QuickSlotWeapon[i].m_ActiveSprite, EZAnimation.ANIM_MODE.To, Globals.m_ClearWhite, EZAnimation.linear, m_QuickSlotAnimateOutDuration, 0f, null, null);
			FadeSpriteAlpha.Do(m_QuickSlotWeapon[i].m_InactiveSprite, EZAnimation.ANIM_MODE.To, Globals.m_ClearWhite, EZAnimation.linear, m_QuickSlotAnimateOutDuration, 0f, null, null);
			FadeTextAlpha.Do(m_QuickSlotWeapon[i].m_ClipText, EZAnimation.ANIM_MODE.To, Globals.m_ClearWhite, EZAnimation.linear, m_QuickSlotAnimateOutDuration, 0f, null, null);
			FadeTextAlpha.Do(m_QuickSlotWeapon[i].m_AmmoText, EZAnimation.ANIM_MODE.To, Globals.m_ClearWhite, EZAnimation.linear, m_QuickSlotAnimateOutDuration, 0f, null, null);
			if (m_QuickSlotWeapon[i].m_WeaponIcon != null)
			{
				FadeSpriteAlpha.Do(m_QuickSlotWeapon[i].m_WeaponIcon, EZAnimation.ANIM_MODE.To, Globals.m_ClearWhite, EZAnimation.linear, m_QuickSlotAnimateOutDuration, 0f, null, null);
			}
			if (ForceClose)
			{
				fadeSpriteAlpha.End();
			}
		}
		m_QuickWeaponsOpen = false;
		if (!ForceClose)
		{
			EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null, base.gameObject);
		}
	}

	private void CloseQuickItems(bool ForceClose = false)
	{
		if (!m_QuickItemsOpen && !ForceClose)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			FadeSpriteAlpha fadeSpriteAlpha = FadeSpriteAlpha.Do(m_QuickSlotItem[i].m_Root, EZAnimation.ANIM_MODE.To, Globals.m_ClearWhite, EZAnimation.linear, m_QuickSlotAnimateOutDuration, 0f, null, QuickItemRootFaded);
			FadeSpriteAlpha.Do(m_QuickSlotItem[i].m_ActiveSprite, EZAnimation.ANIM_MODE.To, Globals.m_ClearWhite, EZAnimation.linear, m_QuickSlotAnimateOutDuration, 0f, null, null);
			FadeSpriteAlpha.Do(m_QuickSlotItem[i].m_InactiveSprite, EZAnimation.ANIM_MODE.To, Globals.m_ClearWhite, EZAnimation.linear, m_QuickSlotAnimateOutDuration, 0f, null, null);
			FadeTextAlpha.Do(m_QuickSlotItem[i].m_Quantity, EZAnimation.ANIM_MODE.To, Globals.m_ClearWhite, EZAnimation.linear, m_QuickSlotAnimateOutDuration, 0f, null, null);
			if (m_QuickSlotItem[i].m_ItemIcon != null)
			{
				FadeSpriteAlpha.Do(m_QuickSlotItem[i].m_ItemIcon, EZAnimation.ANIM_MODE.To, Globals.m_ClearWhite, EZAnimation.linear, m_QuickSlotAnimateOutDuration, 0f, null, null);
			}
			if (ForceClose)
			{
				fadeSpriteAlpha.End();
			}
		}
		m_QuickItemsOpen = false;
		if (!ForceClose)
		{
			EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null, base.gameObject);
		}
	}

	public void QuickWeaponRootFaded(EZAnimation anim)
	{
		(anim.GetSubject() as UIButton).gameObject.SetActiveRecursively(false);
	}

	public void QuickItemRootFaded(EZAnimation anim)
	{
		(anim.GetSubject() as UIButton).gameObject.SetActiveRecursively(false);
	}

	private void UpdateQuickWeaponsInput()
	{
		POINTER_INFO ptr;
		UIManager.instance.GetPointer(m_QuickWeaponPointerID, m_QuickWeaponPointerCameraID, out ptr);
		int num = -1;
		Ray ray = UIManager.instance.uiCameras[m_QuickWeaponPointerCameraID].camera.ScreenPointToRay(ptr.devicePos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, ptr.rayDepth, ptr.layerMask))
		{
			for (int i = 0; i < 4; i++)
			{
				if (hitInfo.collider == m_QuickSlotWeapon[i].m_Collider)
				{
					num = i;
					break;
				}
			}
		}
		for (int j = 0; j < 4; j++)
		{
			m_QuickSlotWeapon[j].m_ActiveSprite.Hide(num != j);
			m_QuickSlotWeapon[j].m_InactiveSprite.Hide(num == j);
		}
		if (num >= 0 && num != m_PreviousWeaponSelected)
		{
			EventManager.Instance.PostEvent("UI_Toggle", EventAction.PlaySound, null, base.gameObject);
		}
		m_PreviousWeaponSelected = num;
	}

	private void UpdateQuickItemsInput()
	{
		POINTER_INFO ptr;
		UIManager.instance.GetPointer(m_QuickItemPointerID, m_QuickItemPointerCameraID, out ptr);
		int num = -1;
		Ray ray = UIManager.instance.uiCameras[m_QuickItemPointerCameraID].camera.ScreenPointToRay(ptr.devicePos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, ptr.rayDepth, ptr.layerMask))
		{
			for (int i = 0; i < 4; i++)
			{
				if (hitInfo.collider == m_QuickSlotItem[i].m_Collider)
				{
					num = i;
					break;
				}
			}
		}
		for (int j = 0; j < 4; j++)
		{
			m_QuickSlotItem[j].m_ActiveSprite.Hide(num != j);
			m_QuickSlotItem[j].m_InactiveSprite.Hide(num == j);
		}
		if (num >= 0 && num != m_PreviousItemSelected)
		{
			EventManager.Instance.PostEvent("UI_Toggle", EventAction.PlaySound, null, base.gameObject);
		}
		m_PreviousItemSelected = num;
	}

	private void UpdateGrenadesInput()
	{
		POINTER_INFO ptr;
		UIManager.instance.GetPointer(m_GrenadeMenuPointerID, m_GrenadeMenuPointerCameraID, out ptr);
		int num = -1;
		Ray ray = UIManager.instance.uiCameras[m_GrenadeMenuPointerCameraID].camera.ScreenPointToRay(ptr.devicePos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, ptr.rayDepth, ptr.layerMask))
		{
			for (int i = 0; i < 3; i++)
			{
				if (hitInfo.collider == m_GrenadeMenuSlot[i].gameObject.collider)
				{
					num = i;
					break;
				}
			}
		}
		for (int j = 0; j < 3; j++)
		{
			Color color = m_BackgroundNormal;
			if (num == j)
			{
				color = m_BrightHUDColor;
			}
			color.a = m_RadarRoot.Color.a * color.a;
			m_GrenadeMenuSlot[j].SetColor(color);
		}
		if (num >= 0 && num != m_PreviousGrenadeSelected)
		{
			EventManager.Instance.PostEvent("UI_Toggle", EventAction.PlaySound, null, base.gameObject);
		}
		m_PreviousGrenadeSelected = num;
	}

	public void Weapon1Released(POINTER_INFO ptr)
	{
		Globals.m_PlayerController.SetWeapon(0);
		m_ReloadPressed = false;
		CloseQuickWeapons(false);
	}

	public void Weapon2Released(POINTER_INFO ptr)
	{
		Globals.m_PlayerController.SetWeapon(1);
		m_ReloadPressed = false;
		CloseQuickWeapons(false);
	}

	public void Weapon3Released(POINTER_INFO ptr)
	{
		Globals.m_PlayerController.SetWeapon(2);
		m_ReloadPressed = false;
		CloseQuickWeapons(false);
	}

	public void Weapon4Released(POINTER_INFO ptr)
	{
		m_ReloadPressed = false;
		CloseQuickWeapons(false);
	}

	public void Item1Released(POINTER_INFO ptr)
	{
		Globals.m_Inventory.m_CurrentItem = ItemType.Aug_Cloaking;
		SetItemIcon();
		m_ItemPressed = false;
		CloseQuickItems(false);
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}

	public void Item2Released(POINTER_INFO ptr)
	{
		Globals.m_Inventory.m_CurrentItem = ItemType.EnergyBar;
		SetItemIcon();
		m_ItemPressed = false;
		CloseQuickItems(false);
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}

	public void Item3Released(POINTER_INFO ptr)
	{
		Globals.m_Inventory.m_CurrentItem = ItemType.Booze;
		SetItemIcon();
		m_ItemPressed = false;
		CloseQuickItems(false);
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}

	public void Item4Released(POINTER_INFO ptr)
	{
		m_ItemPressed = false;
		CloseQuickItems(false);
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}

	public void Grenade1Released(POINTER_INFO ptr)
	{
		Globals.m_PlayerController.m_CurrentGrenadeType = 0;
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}

	public void Grenade2Released(POINTER_INFO ptr)
	{
		Globals.m_PlayerController.m_CurrentGrenadeType = 1;
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}

	public void Grenade3Released(POINTER_INFO ptr)
	{
		Globals.m_PlayerController.m_CurrentGrenadeType = 2;
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}

	public void GrenadeButtonTapped()
	{
		if (Globals.m_Inventory.m_Grenades[Globals.m_PlayerController.m_CurrentGrenadeType] > 0)
		{
			Globals.m_PlayerController.GrenadeButtonTapped();
			Globals.m_Inventory.m_Grenades[Globals.m_PlayerController.m_CurrentGrenadeType]--;
			SetGrenadeIcon();
		}
		else
		{
			m_GrenadeWarningTimer = 0f;
			EventManager.Instance.PostEvent("UI_Error", EventAction.PlaySound, null, base.gameObject);
		}
	}

	public void Kickback(float Strength)
	{
		base.transform.localScale = Vector3.one;
		PunchScale.Do(base.gameObject, new Vector3(Strength, Strength, Strength), 0.35f, 0f, null, null);
	}

	public void TurnOnCoverButton(bool Enable)
	{
		TurnOnCoverButton(Enable, true);
	}

	public void TurnOnCoverButton(bool Enable, bool ToEnter)
	{
		if ((bool)m_CoverButtonParent)
		{
			m_CoverButtonParent.Hide(!Enable);
		}
		SetCoverState(ToEnter, Enable);
	}

	public void TurnOnCoverFlipButton(bool Enable, CoverFlipButtonSide side)
	{
		if (side == CoverFlipButtonSide.Left)
		{
			m_CoverFlipButtonLeft.Hide(!Enable || m_Customizing);
		}
		else
		{
			m_CoverFlipButtonRight.Hide(!Enable || m_Customizing);
		}
	}

	public void TurnOnCoverInnerFlipButton(bool Enable, CoverFlipButtonSide side)
	{
		if (side == CoverFlipButtonSide.Left)
		{
			m_CoverFlipInnerButtonLeft.Hide(!Enable || m_Customizing);
		}
		else
		{
			m_CoverFlipInnerButtonRight.Hide(!Enable || m_Customizing);
		}
	}

	public void TurnOnCoverOuterFlipButton(bool Enable, CoverFlipButtonSide side)
	{
		if (side == CoverFlipButtonSide.Left)
		{
			m_CoverFlipOuterButtonLeft.Hide(!Enable || m_Customizing);
		}
		else
		{
			m_CoverFlipOuterButtonRight.Hide(!Enable || m_Customizing);
		}
	}

	public void SetCoverState(bool ToEnter, bool Enable = true)
	{
		m_CoverButtonEnter.Hide(!Enable || !ToEnter);
		m_CoverButtonExit.Hide(!Enable || ToEnter);
	}

	public void PassThruPressed(POINTER_INFO ptr)
	{
		Globals.m_PlayerController.ScreenPress(ptr);
	}

	public void PassThruReleased(POINTER_INFO ptr)
	{
		Globals.m_PlayerController.ScreenRelease(ptr);
	}

	public void CoverButtonTapped()
	{
		Globals.m_PlayerController.CoverButtonTapped();
	}

	public void ArmorButtonTapped()
	{
		Globals.m_PlayerController.ArmorButtonTapped();
	}

	public void FirePressed(POINTER_INFO ptr)
	{
		Globals.m_PlayerController.FireButtonPress(ptr);
	}

	public void FireReleased(POINTER_INFO ptr)
	{
		Globals.m_PlayerController.FireButtonRelease(ptr);
	}

	public void StanceButtonTapped()
	{
		Globals.m_PlayerController.StanceButtonTapped();
		SetStance();
	}

	public void PauseTapped()
	{
		Time.timeScale = 0f;
		PauseTabs.OpenTabs();
		Globals.m_HUD.Display(false, true);
		EventManager.Instance.PostEvent("UI_Swish", EventAction.PlaySound, null, base.gameObject);
		EventManager.Instance.PostEvent("DynamicMixer", EventAction.AddPreset, "Pause");
		EventManager.Instance.PostEvent("Pause", EventAction.PauseSound, null, base.gameObject);
		GameManager.GamePaused(true);
		CommLinkDialog.Paused(true);
	}

	public void CoverFlipTapped()
	{
		Globals.m_PlayerController.CoverFlipButtonTapped();
	}

	public void CoverInnerFlipTapped()
	{
		Globals.m_PlayerController.CoverInnerFlipButtonTapped();
	}

	public void CoverOuterFlipTapped()
	{
		Globals.m_PlayerController.CoverOuterFlipButtonTapped();
	}

	private void SetStance()
	{
		m_StanceStanding.Hide(Globals.m_PlayerController.m_Stance != PlayerController.Stance.Stand);
		m_StanceCrouching.Hide(Globals.m_PlayerController.m_Stance != PlayerController.Stance.Crouch);
	}

	public void TurnOnStanceButton(bool Enabled)
	{
		if (Enabled)
		{
			m_StanceBackground.Hide(false);
			SetStance();
		}
		else
		{
			m_StanceBackground.Hide(true);
			m_StanceStanding.Hide(true);
			m_StanceCrouching.Hide(true);
		}
	}

	public void SetWeaponIcon(WeaponType Weapon)
	{
		m_CombatRifle.active = Weapon == WeaponType.CombatRifle;
		m_Crossbow.active = Weapon == WeaponType.Crossbow;
		m_Shotgun.active = Weapon == WeaponType.Shotgun;
	}

	public void SetGrenadeIcon()
	{
		m_GrenadeIcon[0].active = Globals.m_PlayerController.m_CurrentGrenadeType == 0;
		m_GrenadeIcon[1].active = Globals.m_PlayerController.m_CurrentGrenadeType == 1;
		m_GrenadeIcon[2].active = Globals.m_PlayerController.m_CurrentGrenadeType == 2;
		m_GrenadeQuantity.Text = Globals.m_Inventory.m_Grenades[Globals.m_PlayerController.m_CurrentGrenadeType].ToString();
	}

	public void SetItemIcon()
	{
		if (Globals.m_Inventory.m_CurrentItem == ItemType.Aug_Cloaking)
		{
			m_CurrentItemQuantity.Hide(true);
			m_CloakingIcon.active = true;
			m_EnergyBarIcon.active = false;
			m_BoozeIcon.active = false;
		}
		else if (Globals.m_Inventory.m_CurrentItem == ItemType.EnergyBar)
		{
			m_CurrentItemQuantity.Hide(false);
			m_CurrentItemQuantity.Text = Globals.m_Inventory.m_EnergyBars.ToString();
			m_CloakingIcon.active = false;
			m_EnergyBarIcon.active = true;
			m_BoozeIcon.active = false;
		}
		else if (Globals.m_Inventory.m_CurrentItem == ItemType.Booze)
		{
			m_CurrentItemQuantity.Hide(false);
			m_CurrentItemQuantity.Text = Globals.m_Inventory.m_Booze.ToString();
			m_CloakingIcon.active = false;
			m_EnergyBarIcon.active = false;
			m_BoozeIcon.active = true;
		}
	}

	public void StartCustomization()
	{
		if (m_Customizing)
		{
			return;
		}
		Display(true, true, true);
		if (m_DraggableElements != null)
		{
			for (int i = 0; i < m_DraggableElements.Length; i++)
			{
				if (m_DraggableElements[i] != null)
				{
					m_DraggableElements[i].gameObject.SetActiveRecursively(true);
					m_DraggableElements[i].collider.enabled = true;
					m_DraggablePositions[i] = m_DraggableElements[i].transform.localPosition;
				}
			}
		}
		m_Customizing = true;
		m_CustomizationBlocker.enabled = m_Customizing;
		SetStance();
		TurnOnCoverButton(true, true);
		SetWeaponIcon(Globals.m_PlayerController.GetEquippedWeaponType());
		SetItemIcon();
		SetGrenadeIcon();
		OpenCommLinkImmediate(true);
		HidePlayerSubtitle();
		CloseQuickWeapons(true);
		CloseQuickItems(true);
		CloseGrenadeMenu(true);
	}

	public void OnEZDragDrop(EZDragDropParams data)
	{
		if (data.evt == EZDragDropEvent.Dropped)
		{
			data.dragObj.DropHandled = true;
		}
	}

	public void RestoreCustomDefaults()
	{
		if (m_DraggableElements == null)
		{
			return;
		}
		for (int i = 0; i < m_DraggableElements.Length; i++)
		{
			if (m_DraggableElements[i] != null)
			{
				m_DraggableElements[i].transform.localPosition = m_DraggableDefaultPositions[i];
			}
		}
	}

	public void QuitCustomizationAndSave()
	{
		if (m_DraggableElements != null)
		{
			for (int i = 0; i < m_DraggableElements.Length; i++)
			{
				if (m_DraggableElements[i] != null)
				{
					m_DraggablePositions[i] = m_DraggableElements[i].transform.localPosition;
					m_DraggableElements[i].collider.enabled = false;
					PreviewLabs.PlayerPrefs.SetString(m_DraggableElements[i].name, m_DraggableElements[i].transform.localPosition.x + "," + m_DraggableElements[i].transform.localPosition.y + "," + m_DraggableElements[i].transform.localPosition.z);
				}
			}
			PreviewLabs.PlayerPrefs.Flush();
		}
		Display(false, true);
		ReAdjustQuickWeaponMenu();
		ReAdjustQuickItemMenu();
		ReAdjustCommLink();
		ReAdjustWarningIndicatorPosition();
		m_Customizing = false;
		m_CustomizationBlocker.enabled = m_Customizing;
	}

	public void CancelCustomization()
	{
		if (m_DraggableElements != null)
		{
			for (int i = 0; i < m_DraggableElements.Length; i++)
			{
				if (m_DraggableElements[i] != null)
				{
					m_DraggableElements[i].transform.localPosition = m_DraggablePositions[i];
					m_DraggableElements[i].collider.enabled = false;
				}
			}
		}
		Display(false, true);
		ReAdjustQuickWeaponMenu();
		ReAdjustQuickItemMenu();
		ReAdjustCommLink();
		ReAdjustWarningIndicatorPosition();
		m_Customizing = false;
		m_CustomizationBlocker.enabled = m_Customizing;
	}

	private void ReAdjustQuickWeaponMenu()
	{
		float num = ((!(m_DraggableWeaponParent.transform.localPosition.y >= 0f)) ? 1f : (-1f));
		for (int i = 0; i < 4; i++)
		{
			m_QuickSlotWeapon[i].m_OriginalLocalPosition.y = num * Mathf.Abs(m_QuickSlotWeapon[i].m_OriginalLocalPosition.y);
		}
	}

	private void ReAdjustQuickItemMenu()
	{
		float num = ((!(m_DraggableItemParent.transform.localPosition.y >= 0f)) ? 1f : (-1f));
		for (int i = 0; i < 4; i++)
		{
			m_QuickSlotItem[i].m_OriginalLocalPosition.y = num * Mathf.Abs(m_QuickSlotItem[i].m_OriginalLocalPosition.y);
		}
	}

	private void ReAdjustCommLink()
	{
		if (m_DraggableCommLink.transform.localPosition.x <= 0f)
		{
			if (m_CommNameBackground.anchor != SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT)
			{
				m_CommName.transform.localPosition += new Vector3(m_CommNameBackground.width, 0f, 0f);
				m_CommSubtitle.transform.localPosition += new Vector3(m_CommSubtitleBackground.width, 0f, 0f);
				Vector3 localPosition = m_CommNameBackground.transform.localPosition;
				localPosition.x = Mathf.Abs(localPosition.x);
				m_CommNameBackground.transform.localPosition = localPosition;
				localPosition = m_CommSubtitleBackground.transform.localPosition;
				localPosition.x = Mathf.Abs(localPosition.x);
				m_CommSubtitleBackground.transform.localPosition = localPosition;
				m_CommNameBackground.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT);
				m_CommSubtitleBackground.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT);
			}
		}
		else if (m_CommNameBackground.anchor != SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT)
		{
			m_CommName.transform.localPosition -= new Vector3(m_CommNameBackground.width, 0f, 0f);
			m_CommSubtitle.transform.localPosition -= new Vector3(m_CommSubtitleBackground.width, 0f, 0f);
			Vector3 localPosition = m_CommNameBackground.transform.localPosition;
			localPosition.x = Mathf.Abs(localPosition.x) * -1f;
			m_CommNameBackground.transform.localPosition = localPosition;
			localPosition = m_CommSubtitleBackground.transform.localPosition;
			localPosition.x = Mathf.Abs(localPosition.x) * -1f;
			m_CommSubtitleBackground.transform.localPosition = localPosition;
			m_CommNameBackground.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT);
			m_CommSubtitleBackground.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT);
		}
	}

	private void ReAdjustWarningIndicatorPosition()
	{
		int num = ((!(m_DraggableRadarParent.transform.localPosition.y >= 0f)) ? 1 : (-1));
		Vector3 localPosition = m_WarningIndicator.transform.localPosition;
		localPosition.y = (float)num * Mathf.Abs(localPosition.y);
		m_WarningIndicator.transform.localPosition = localPosition;
	}

	public void DisplayEnergyWarning()
	{
		m_EnergyWarningTimer = 0f;
		m_EnergyWarningBackground.gameObject.SetActiveRecursively(true);
	}

	private void UpdateEnergyWarning()
	{
		if (m_EnergyWarningTimer >= 0f)
		{
			m_EnergyWarningTimer += Time.deltaTime;
			if (m_EnergyWarningTimer >= 3f)
			{
				m_EnergyWarningBackground.gameObject.SetActiveRecursively(false);
				m_EnergyWarningTimer = -1f;
				return;
			}
			Color white = Color.white;
			white.a = m_RadarRoot.Color.a * Mathf.Lerp(0.5f, 1f, Mathf.PingPong(3f * m_EnergyWarningTimer, 1f));
			m_EnergyWarningBackground.SetColor(white);
			m_EnergyWarningBackground.Hide(m_Customizing);
			m_EnergyWarningIcon.SetColor(white);
			m_EnergyWarningIcon.Hide(m_Customizing);
			m_EnergyWarningText.SetColor(white);
			m_EnergyWarningText.Hide(m_Customizing);
		}
	}

	private void UpdateAmmoWarnings()
	{
		if (Globals.m_PlayerController.NoAmmoForCurrentWeapon() && !m_Customizing)
		{
			if (!m_AmmoWarningBackground.gameObject.active)
			{
				m_AmmoWarningBackground.gameObject.SetActiveRecursively(true);
				m_AmmoWarningIconOverWeapon.gameObject.active = true;
			}
			m_AmmoWarningIcon.SetUVs(m_AmmoWarningIcon.animations[0].GetFrame(1).uvs);
			m_AmmoWarningIconOverWeapon.SetUVs(m_AmmoWarningIconOverWeapon.animations[0].GetFrame(1).uvs);
			m_AmmoWarningText.Text = "NO AMMO";
		}
		else if (Globals.m_PlayerController.LowAmmoForCurrentWeaponClip() && !m_Customizing)
		{
			if (!m_AmmoWarningBackground.gameObject.active)
			{
				m_AmmoWarningBackground.gameObject.SetActiveRecursively(true);
				m_AmmoWarningIconOverWeapon.gameObject.active = true;
			}
			m_AmmoWarningIcon.SetUVs(m_AmmoWarningIcon.animations[0].GetFrame(0).uvs);
			m_AmmoWarningIconOverWeapon.SetUVs(m_AmmoWarningIconOverWeapon.animations[0].GetFrame(0).uvs);
			m_AmmoWarningText.Text = "LOW AMMO";
		}
		else if (m_AmmoWarningBackground.gameObject.active)
		{
			m_AmmoWarningBackground.gameObject.SetActiveRecursively(false);
			m_AmmoWarningIconOverWeapon.gameObject.active = false;
		}
		Color white = Color.white;
		white.a = m_RadarRoot.Color.a * Mathf.Lerp(0.5f, 1f, Mathf.PingPong(2f * Time.time, 1f));
		m_AmmoWarningBackground.SetColor(white);
		m_AmmoWarningIcon.SetColor(white);
		m_AmmoWarningText.SetColor(white);
		m_WarningOverWeaponTimer += Time.deltaTime;
		if (m_WarningOverWeaponTimer >= 0.9f)
		{
			m_WarningOverWeaponTimer = 0f;
		}
		white.a = 0f;
		if (m_WarningOverWeaponTimer <= 0.25f)
		{
			white.a = Mathf.Lerp(0f, 1f, m_WarningOverWeaponTimer / 0.25f);
		}
		else if (m_WarningOverWeaponTimer <= 0.5f)
		{
			white.a = Mathf.Lerp(1f, 0f, Mathf.InverseLerp(0.25f, 0.5f, m_WarningOverWeaponTimer));
		}
		m_AmmoWarningIconOverWeapon.SetColor(white);
		Color color = m_BrightHUDColor;
		if (m_WeaponReload.controlState != UIButton.CONTROL_STATE.ACTIVE)
		{
			color = ((!m_AmmoWarningBackground.gameObject.active) ? m_BackgroundNormal : m_CurrentFlashColor);
		}
		color.a = m_RadarRoot.Color.a * color.a;
		m_WeaponReload.SetColor(color);
	}

	private void UpdateItemButtonFlash()
	{
		Color color = m_BrightHUDColor;
		if (m_ItemButton.controlState != UIButton.CONTROL_STATE.ACTIVE)
		{
			color = m_BackgroundNormal;
			if (Globals.m_Inventory.m_CurrentItem == ItemType.Aug_Cloaking)
			{
				if (Globals.m_AugmentCloaking.enabled)
				{
					color = Globals.m_AugmentCloaking.m_DesiredHUDColor;
				}
			}
			else if (m_ItemWarningTimer >= 0f)
			{
				m_ItemWarningTimer += Time.deltaTime;
				if (m_ItemWarningTimer >= 1.7f)
				{
					m_ItemWarningTimer = -1f;
				}
				float t = Mathf.PingPong(4.5f * m_ItemWarningTimer, 1f);
				color = Color.Lerp(m_BackgroundNormal, m_BackgroundFlash, t);
			}
		}
		color.a = m_RadarRoot.Color.a * color.a;
		m_ItemButton.SetColor(color);
	}

	private void UpdateGrenadeButtonFlash()
	{
		Color color = m_BrightHUDColor;
		if (m_GrenadeButton.controlState != UIButton.CONTROL_STATE.ACTIVE)
		{
			if (m_GrenadeWarningTimer >= 0f)
			{
				m_GrenadeWarningTimer += Time.deltaTime;
				if (m_GrenadeWarningTimer >= 1.7f)
				{
					m_GrenadeWarningTimer = -1f;
				}
				float t = Mathf.PingPong(4.5f * m_GrenadeWarningTimer, 1f);
				color = Color.Lerp(m_BackgroundNormal, m_BackgroundFlash, t);
			}
			else
			{
				color = m_BackgroundNormal;
			}
		}
		color.a = m_RadarRoot.Color.a * color.a;
		m_GrenadeButton.SetColor(color);
	}

	private void UpdateGrenadeIndicators()
	{
		float a = m_RadarRoot.Color.a;
		float num = Mathf.Lerp(m_MinArrowScale, m_MaxArrowScale, Mathf.PingPong(3f * Time.time, 1f));
		float y = Mathf.Lerp(-0.06f, 0.04f, Mathf.InverseLerp(m_MinArrowScale, m_MaxArrowScale, num));
		Color white = Color.white;
		LinkedListNode<GrenadeIndicatorInfo> linkedListNode = m_GrenadeIndicators.First;
		for (LinkedListNode<GrenadeFrag> linkedListNode2 = Globals.m_AIDirector.GetFirstGrenade(); linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
		{
			if (linkedListNode2.Value.m_HUDTrackable)
			{
				float num2 = (linkedListNode2.Value.transform.position - Globals.m_PlayerController.transform.position).sqrMagnitude / (linkedListNode2.Value.m_MaxRadiusSqr * 1.5f);
				if (!(num2 > 1f))
				{
					num2 = Mathf.InverseLerp(0.5f, 1f, num2);
					if (linkedListNode == null)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(m_GrenadeIndicatorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
						gameObject.transform.parent = Globals.m_HUD.transform;
						gameObject.transform.localRotation = Quaternion.identity;
						GrenadeIndicatorInfo value = default(GrenadeIndicatorInfo);
						value.m_Root = gameObject.GetComponent<PackedSprite>();
						value.m_Pivot = value.m_Root.transform.GetChild(0).transform;
						value.m_Glow = value.m_Pivot.GetChild(0).GetComponent<PackedSprite>();
						value.m_Arrow = value.m_Glow.transform.GetChild(0).GetComponent<PackedSprite>();
						linkedListNode = m_GrenadeIndicators.AddLast(value);
					}
					linkedListNode.Value.m_Root.gameObject.SetActiveRecursively(true);
					linkedListNode.Value.m_Root.PlayAnim(0, (int)linkedListNode2.Value.m_GrenadeType);
					linkedListNode.Value.m_Root.PauseAnim();
					Vector3 center = linkedListNode2.Value.collider.bounds.center;
					center = Globals.m_CameraController.camera.WorldToScreenPoint(center);
					center.x -= (float)Screen.width * 0.5f;
					center.y -= (float)Screen.height * 0.5f;
					if (center.z < 0f)
					{
						center *= -1f;
						center.y = -20000f;
					}
					Vector3 vector = center;
					vector.x = Mathf.Clamp(vector.x, (float)(-Screen.width) * 0.5f + (float)m_GrenadeIndicatorBuffer, (float)Screen.width * 0.5f - (float)m_GrenadeIndicatorBuffer);
					if (vector.y > (float)Screen.height * 0.5f - 2f * (float)m_GrenadeIndicatorBuffer)
					{
						vector.y -= m_GrenadeIndicatorBuffer;
					}
					else
					{
						vector.y += m_GrenadeIndicatorBuffer;
					}
					vector.y = Mathf.Clamp(vector.y, (float)(-Screen.height) * 0.5f + (float)m_GrenadeIndicatorBuffer, (float)Screen.height * 0.5f - (float)m_GrenadeIndicatorBuffer);
					white.a = a * (1f - num2);
					linkedListNode.Value.m_Root.SetColor(white);
					linkedListNode.Value.m_Glow.SetColor(white);
					linkedListNode.Value.m_Arrow.SetColor(white);
					center -= vector;
					num2 = 57.29578f * Mathf.Atan2(center.y, center.x);
					vector *= m_RadarRoot.worldUnitsPerScreenPixel;
					vector.z = -1f;
					linkedListNode.Value.m_Root.transform.localPosition = vector;
					linkedListNode.Value.m_Pivot.localRotation = Quaternion.Euler(0f, 0f, num2 - 90f);
					linkedListNode.Value.m_Arrow.transform.localPosition = new Vector3(0f, y, -0.1f);
					linkedListNode.Value.m_Arrow.transform.localScale = new Vector3(num, num, num);
					linkedListNode = linkedListNode.Next;
				}
			}
		}
		while (linkedListNode != null)
		{
			linkedListNode.Value.m_Root.gameObject.SetActiveRecursively(false);
			linkedListNode = linkedListNode.Next;
		}
	}

	private void UpdateWarningIndicator()
	{
		float a = m_RadarRoot.Color.a;
		Color color = m_BrightHUDColor;
		switch ((m_Customizing || !m_Showing) ? WarningLevel.Alarmed : Globals.m_AIDirector.GetWarningLevel())
		{
		case WarningLevel.Hostile:
			m_WarningIndicatorAlpha = 1f;
			m_WarningIndicatorColor = m_HostileRadarColor;
			color = m_HostileRadarColor;
			m_WarningIndicatorText.Text = "HOSTILE";
			break;
		case WarningLevel.Alarmed:
			m_WarningIndicatorAlpha = 1f;
			m_WarningIndicatorColor = m_BrightHUDColor;
			m_WarningIndicatorText.Text = "ALARMED";
			break;
		default:
			m_WarningIndicatorAlpha = Mathf.Max(m_WarningIndicatorAlpha - 0.5f * Time.deltaTime, 0f);
			break;
		}
		m_WarningIndicatorColor.a = a * m_WarningIndicatorAlpha;
		color.a = a;
		m_RadarRoot.SetColor(color);
		m_WarningIndicator.SetColor(m_WarningIndicatorColor);
		m_WarningIndicatorText.SetColor(m_WarningIndicatorColor);
	}

	private void UpdatePlayerRadar()
	{
		Vector2 camForward = new Vector2(Globals.m_CameraController.transform.forward.x, Globals.m_CameraController.transform.forward.z);
		camForward.Normalize();
		Vector2 camRight = new Vector2(Globals.m_CameraController.transform.right.x, Globals.m_CameraController.transform.right.z);
		camRight.Normalize();
		UpdateEnemyIcons(camForward, camRight);
		UpdateTurretIcons(camForward, camRight);
	}

	private void UpdateEnemyIcons(Vector2 CamForward, Vector2 CamRight)
	{
		LinkedListNode<PackedSprite> linkedListNode = m_EnemyIcons.First;
		for (int i = 0; i < 8; i++)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode2 = Globals.m_AIDirector.GetFirstEnemy(i); linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
			{
				ToObject.x = linkedListNode2.Value.m_RigMotion.transform.position.x - Globals.m_CameraController.transform.position.x;
				ToObject.y = linkedListNode2.Value.m_RigMotion.transform.position.z - Globals.m_CameraController.transform.position.z;
				if (!(ToObject.sqrMagnitude >= m_RadarWorldRangeSqr))
				{
					ToObjectRelative.x = Vector2.Dot(CamRight, ToObject);
					if (!(ToObjectRelative.x > m_RadarWorldRange.x) && !(ToObjectRelative.x < 0f - m_RadarWorldRange.x))
					{
						ToObjectRelative.x /= m_RadarWorldRange.x;
						ToObjectRelative.x *= m_RadarRoot.width * 0.5f;
						ToObjectRelative.y = Vector2.Dot(CamForward, ToObject);
						if (!(ToObjectRelative.y > m_RadarWorldRange.y) && !(ToObjectRelative.y < 0f - m_RadarWorldRange.y))
						{
							ToObjectRelative.y /= m_RadarWorldRange.y;
							ToObjectRelative.y *= m_RadarRoot.height * 0.5f;
							if (linkedListNode == null)
							{
								GameObject gameObject = UnityEngine.Object.Instantiate(m_EnemyIconPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
								gameObject.transform.parent = m_RadarRoot.transform;
								linkedListNode = m_EnemyIcons.AddLast(gameObject.GetComponent<PackedSprite>());
							}
							linkedListNode.Value.gameObject.active = true;
							linkedListNode.Value.transform.localPosition = new Vector3(ToObjectRelative.x, ToObjectRelative.y, -0.05f);
							ObjectDir.x = linkedListNode2.Value.m_RigMotion.transform.forward.x;
							ObjectDir.y = linkedListNode2.Value.m_RigMotion.transform.forward.z;
							ObjectDir.Normalize();
							float num = Mathf.Atan2(ObjectDir.y, ObjectDir.x) - Mathf.Atan2(CamForward.y, CamForward.x);
							linkedListNode.Value.transform.localRotation = Quaternion.Euler(0f, 0f, 57.29578f * num);
							if (linkedListNode2.Value.InCombat())
							{
								linkedListNode.Value.SetColor(m_HostileRadarColor);
							}
							else if (linkedListNode2.Value.IsPassive())
							{
								linkedListNode.Value.SetColor(m_PassiveRadarColor);
							}
							else
							{
								linkedListNode.Value.SetColor(m_AlarmedRadarColor);
							}
							linkedListNode = linkedListNode.Next;
						}
					}
				}
			}
		}
		while (linkedListNode != null)
		{
			linkedListNode.Value.gameObject.active = false;
			linkedListNode = linkedListNode.Next;
		}
	}

	private void UpdateTurretIcons(Vector2 CamForward, Vector2 CamRight)
	{
		LinkedListNode<PackedSprite> linkedListNode = m_TurretIcons.First;
		for (LinkedListNode<Turret> linkedListNode2 = Globals.m_AIDirector.GetFirstTurret(); linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
		{
			ToObject.x = linkedListNode2.Value.transform.position.x - Globals.m_CameraController.transform.position.x;
			ToObject.y = linkedListNode2.Value.transform.position.z - Globals.m_CameraController.transform.position.z;
			if (!(ToObject.sqrMagnitude >= m_RadarWorldRangeSqr))
			{
				ToObjectRelative.x = Vector2.Dot(CamRight, ToObject);
				if (!(ToObjectRelative.x > m_RadarWorldRange.x) && !(ToObjectRelative.x < 0f - m_RadarWorldRange.x))
				{
					ToObjectRelative.x /= m_RadarWorldRange.x;
					ToObjectRelative.x *= m_RadarRoot.width * 0.5f;
					ToObjectRelative.y = Vector2.Dot(CamForward, ToObject);
					if (!(ToObjectRelative.y > m_RadarWorldRange.y) && !(ToObjectRelative.y < 0f - m_RadarWorldRange.y))
					{
						ToObjectRelative.y /= m_RadarWorldRange.y;
						ToObjectRelative.y *= m_RadarRoot.height * 0.5f;
						if (linkedListNode == null)
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(m_TurretIconPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
							gameObject.transform.parent = m_RadarRoot.transform;
							linkedListNode = m_TurretIcons.AddLast(gameObject.GetComponent<PackedSprite>());
						}
						linkedListNode.Value.gameObject.active = true;
						linkedListNode.Value.transform.localPosition = new Vector3(ToObjectRelative.x, ToObjectRelative.y, -0.05f);
						ObjectDir.x = linkedListNode2.Value.m_HorizontalRotator.transform.forward.x;
						ObjectDir.y = linkedListNode2.Value.m_HorizontalRotator.transform.forward.z;
						ObjectDir.Normalize();
						float num = Mathf.Atan2(ObjectDir.y, ObjectDir.x) - Mathf.Atan2(CamForward.y, CamForward.x);
						linkedListNode.Value.transform.localRotation = Quaternion.Euler(0f, 0f, 57.29578f * num);
						if (linkedListNode2.Value.IsAttacking())
						{
							linkedListNode.Value.SetColor(m_HostileRadarColor);
						}
						else if (linkedListNode2.Value.IsPassive())
						{
							linkedListNode.Value.SetColor(m_PassiveRadarColor);
						}
						else
						{
							linkedListNode.Value.SetColor(m_AlarmedRadarColor);
						}
						linkedListNode = linkedListNode.Next;
					}
				}
			}
		}
		while (linkedListNode != null)
		{
			linkedListNode.Value.gameObject.active = false;
			linkedListNode = linkedListNode.Next;
		}
	}

	public void OpenCommLink(Texture2D CharPortrait, string CharName)
	{
		m_CommLinkParent.SetActiveRecursively(true);
		m_CommPortrait.renderer.sharedMaterial.mainTexture = CharPortrait;
		m_TargetCommName = CharName;
		m_CommScrambling = true;
		m_CommName.Text = string.Empty;
		m_ScrambleLetterIndex = -1;
		m_WordTimer = 0.5f;
		m_LetterTimers = new float[m_TargetCommName.Length];
		for (int i = 0; i < m_LetterTimers.Length; i++)
		{
			m_LetterTimers[i] = -1f;
		}
		m_CommSubtitle.Text = string.Empty;
		m_CommName.SetColor(Color.white);
		m_CommSubtitle.SetColor(Color.white);
		m_CommPortrait.transform.localScale = Vector3.zero;
		AnimateScale.Do(m_CommPortrait.gameObject, EZAnimation.ANIM_MODE.FromTo, new Vector3(1f, 0f, 1f), new Vector3(1f, 1f, 1f), EZAnimation.linear, 0.1f, 0f, null, null);
		m_CommNameBackground.transform.localScale = Vector3.zero;
		AnimateScale.Do(m_CommNameBackground.gameObject, EZAnimation.ANIM_MODE.FromTo, new Vector3(0f, 1f, 1f), new Vector3(1f, 1f, 1f), EZAnimation.spring, 0.4f, 0.3f, null, null);
		m_CommSubtitleBackground.transform.localScale = Vector3.zero;
		AnimateScale.Do(m_CommSubtitleBackground.gameObject, EZAnimation.ANIM_MODE.FromTo, new Vector3(0f, 1f, 1f), new Vector3(1f, 1f, 1f), EZAnimation.spring, 0.4f, 0.5f, null, null);
	}

	private void OpenCommLinkImmediate(bool ForCustomization)
	{
		m_CommLinkParent.SetActiveRecursively(true);
		m_CommPortrait.transform.localScale = Vector3.one;
		m_CommNameBackground.transform.localScale = Vector3.one;
		m_CommSubtitleBackground.transform.localScale = Vector3.one;
		if (ForCustomization)
		{
			m_CommPortrait.renderer.sharedMaterial.mainTexture = m_EmptyPortraitTexture;
			m_CommName.Text = "-offline-";
			m_CommSubtitle.Text = "...";
			m_CommName.SetColor(Color.white);
			m_CommSubtitle.SetColor(Color.white);
		}
		else
		{
			m_CommPortrait.renderer.sharedMaterial.mainTexture = CommLinkDialog.GetCurrentCharacterPortrait();
			m_CommName.Text = CommLinkDialog.GetCurrentCharacterName();
			m_CommSubtitle.Text = CommLinkDialog.GetLastCharacterSubtitle();
			m_CommName.SetColor(Color.white);
			m_CommSubtitle.SetColor(Color.white);
		}
	}

	public void CloseCommLink()
	{
		m_CommLinkParent.SetActiveRecursively(false);
	}

	public void HideCommLinkSubtitle()
	{
		m_CommSubtitleBackground.gameObject.SetActiveRecursively(false);
	}

	public void DisplayLinkSubtitle(string subtitle)
	{
		if (!m_CommSubtitleBackground.gameObject.active)
		{
			m_CommSubtitleBackground.gameObject.SetActiveRecursively(true);
			m_CommSubtitle.SetColor(Color.white);
			m_CommSubtitleBackground.transform.localScale = Vector3.zero;
			AnimateScale.Do(m_CommSubtitleBackground.gameObject, EZAnimation.ANIM_MODE.FromTo, new Vector3(0f, 1f, 1f), new Vector3(1f, 1f, 1f), EZAnimation.spring, 0.4f, 0.5f, null, null);
		}
		m_CommSubtitle.Text = subtitle;
	}

	public void DisplayLinkSubtitleImmediate()
	{
		m_CommSubtitleBackground.gameObject.SetActiveRecursively(true);
		m_CommSubtitle.SetColor(Color.white);
		m_CommSubtitleBackground.transform.localScale = Vector3.one;
		m_CommSubtitle.Text = CommLinkDialog.GetLastCharacterSubtitle();
	}

	public void DisplayPlayerSubtitle(string subtitle)
	{
		m_PlayerSubtitleBackground.gameObject.SetActiveRecursively(true);
		m_PlayerSubtitle.Text = subtitle;
		m_PlayerSubtitle.SetColor(Color.white);
	}

	public void DisplayPlayerSubtitleImmediate()
	{
		m_PlayerSubtitleBackground.gameObject.SetActiveRecursively(true);
		m_PlayerSubtitle.Text = CommLinkDialog.GetLastPlayerSubtitle();
		m_PlayerSubtitle.SetColor(Color.white);
	}

	public void HidePlayerSubtitle()
	{
		m_PlayerSubtitleBackground.gameObject.SetActiveRecursively(false);
	}

	private void UpdateCommScramble()
	{
		if (m_WordTimer >= 0f)
		{
			m_WordTimer -= Time.deltaTime;
			if (m_WordTimer <= 0f)
			{
				m_ScrambleLetterIndex++;
				m_CommName.Text += (char)UnityEngine.Random.Range(33, 93);
				m_LetterTimers[m_ScrambleLetterIndex] = UnityEngine.Random.Range(0.07f, 0.18f);
				m_WordTimer = -1f;
				if (m_ScrambleLetterIndex < m_TargetCommName.Length - 1)
				{
					m_WordTimer = UnityEngine.Random.Range(0.03f, 0.06f);
				}
			}
		}
		char[] array = m_CommName.Text.ToCharArray();
		for (int i = 0; i < m_LetterTimers.Length && i <= m_ScrambleLetterIndex; i++)
		{
			if (m_LetterTimers[i] >= 0f)
			{
				array[i] = (char)UnityEngine.Random.Range(33, 93);
				m_LetterTimers[i] -= Time.deltaTime;
				if (m_LetterTimers[i] <= 0f)
				{
					array[i] = m_TargetCommName[i];
					m_LetterTimers[i] = -1f;
				}
			}
		}
		m_CommName.Text = new string(array);
		if (string.Compare(m_CommName.Text, m_TargetCommName, false) == 0)
		{
			m_CommScrambling = false;
		}
	}

	private void SetupObjectiveMarkers()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_ObjectiveMarkerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.parent = Globals.m_HUD.transform;
		gameObject.transform.localRotation = Quaternion.identity;
		m_PrimaryObjectiveInfo = default(ObjectiveMarkerInfo);
		m_PrimaryObjectiveInfo.m_Root = gameObject.GetComponent<PackedSprite>();
		m_PrimaryObjectiveInfo.m_Pivot = m_PrimaryObjectiveInfo.m_Root.transform.FindChild("Pivot");
		m_PrimaryObjectiveInfo.m_Arrow = m_PrimaryObjectiveInfo.m_Pivot.GetChild(0).GetComponent<PackedSprite>();
		m_PrimaryObjectiveInfo.m_Plaque = m_PrimaryObjectiveInfo.m_Root.transform.FindChild("Sprite_Plaque").gameObject.GetComponent<PackedSprite>();
		m_PrimaryObjectiveInfo.m_Distance = m_PrimaryObjectiveInfo.m_Plaque.transform.FindChild("Text_Distance").gameObject.GetComponent<SpriteText>();
		m_PrimaryObjectiveInfo.m_Number = m_PrimaryObjectiveInfo.m_Plaque.transform.FindChild("Text_Number").gameObject.GetComponent<SpriteText>();
		m_PrimaryObjectiveInfo.m_Plaque.SetCamera(Globals.m_HUDRoot.m_HUDCamera2D);
		m_PrimaryObjectiveInfo.m_Distance.SetCamera(Globals.m_HUDRoot.m_HUDCamera2D);
		m_PrimaryObjectiveInfo.m_Number.SetCamera(Globals.m_HUDRoot.m_HUDCamera2D);
		gameObject = UnityEngine.Object.Instantiate(m_ObjectiveMarkerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.parent = Globals.m_HUD.transform;
		gameObject.transform.localRotation = Quaternion.identity;
		m_SecondaryObjectiveInfo = default(ObjectiveMarkerInfo);
		m_SecondaryObjectiveInfo.m_Root = gameObject.GetComponent<PackedSprite>();
		m_SecondaryObjectiveInfo.m_Pivot = m_SecondaryObjectiveInfo.m_Root.transform.FindChild("Pivot");
		m_SecondaryObjectiveInfo.m_Arrow = m_SecondaryObjectiveInfo.m_Pivot.GetChild(0).GetComponent<PackedSprite>();
		m_SecondaryObjectiveInfo.m_Plaque = m_SecondaryObjectiveInfo.m_Root.transform.FindChild("Sprite_Plaque").gameObject.GetComponent<PackedSprite>();
		m_SecondaryObjectiveInfo.m_Distance = m_SecondaryObjectiveInfo.m_Plaque.transform.FindChild("Text_Distance").gameObject.GetComponent<SpriteText>();
		m_SecondaryObjectiveInfo.m_Number = m_SecondaryObjectiveInfo.m_Plaque.transform.FindChild("Text_Number").gameObject.GetComponent<SpriteText>();
		m_SecondaryObjectiveInfo.m_Plaque.SetCamera(Globals.m_HUDRoot.m_HUDCamera2D);
		m_SecondaryObjectiveInfo.m_Distance.SetCamera(Globals.m_HUDRoot.m_HUDCamera2D);
		m_SecondaryObjectiveInfo.m_Number.SetCamera(Globals.m_HUDRoot.m_HUDCamera2D);
	}

	private void UpdateObjectiveMarkers()
	{
		float a = m_RadarRoot.Color.a;
		float t = Mathf.PingPong(3f * Time.time, 1f);
		float num = Mathf.Lerp(0.7f, 1f, t);
		float x = Mathf.Lerp(0.25f, 0.35f, t);
		Vector3 vector;
		Vector3 vector2;
		Vector3 localPosition;
		Color primaryObjectiveColor;
		if (Globals.m_PrimaryObjective == null)
		{
			m_PrimaryObjectiveInfo.m_Root.gameObject.SetActiveRecursively(false);
		}
		else
		{
			m_PrimaryObjectiveInfo.m_Root.gameObject.SetActiveRecursively(true);
			vector = Globals.m_CameraController.camera.WorldToScreenPoint(Globals.m_PrimaryObjective.position);
			vector.x -= (float)Screen.width * 0.5f;
			vector.y -= (float)Screen.height * 0.5f;
			if (vector.z < 0f)
			{
				vector *= -1f;
				vector.y = -20000f;
			}
			vector2 = vector;
			vector2.x = Mathf.Clamp(vector2.x, (float)(-Screen.width) * 0.5f + (float)m_MarkerBuffer, (float)Screen.width * 0.5f - (float)m_MarkerBuffer);
			vector2.y = Mathf.Clamp(vector2.y, (float)(-Screen.height) * 0.5f + (float)m_MarkerBuffer, (float)Screen.height * 0.5f - (float)m_MarkerBuffer);
			localPosition = m_PrimaryObjectiveInfo.m_Plaque.transform.localPosition;
			if (vector2.x - 5f >= 0f && localPosition.x >= 0f)
			{
				localPosition.x = -1.413036f;
			}
			else if (vector2.x + 5f < 0f && localPosition.x < 0f)
			{
				localPosition.x = 0.15f;
			}
			m_PrimaryObjectiveInfo.m_Plaque.transform.localPosition = localPosition;
			primaryObjectiveColor = m_PrimaryObjectiveColor;
			primaryObjectiveColor.a = a * 0.75f;
			m_PrimaryObjectiveInfo.m_Root.SetColor(primaryObjectiveColor);
			m_PrimaryObjectiveInfo.m_Plaque.SetColor(primaryObjectiveColor);
			t = (Globals.m_PrimaryObjective.position - Globals.m_CameraController.transform.position).magnitude;
			m_PrimaryObjectiveInfo.m_Distance.SetColor(primaryObjectiveColor);
			m_PrimaryObjectiveInfo.m_Distance.Text = (int)t + "M";
			m_PrimaryObjectiveInfo.m_Number.SetColor(primaryObjectiveColor);
			m_PrimaryObjectiveInfo.m_Number.Text = "M1";
			if (vector.x != vector2.x || vector.y != vector2.y)
			{
				m_PrimaryObjectiveInfo.m_Pivot.gameObject.SetActiveRecursively(true);
				m_PrimaryObjectiveInfo.m_Arrow.SetColor(primaryObjectiveColor);
				vector -= vector2;
				t = 57.29578f * Mathf.Atan2(vector.y, vector.x);
				m_PrimaryObjectiveInfo.m_Pivot.localRotation = Quaternion.Euler(0f, 0f, t);
				m_PrimaryObjectiveInfo.m_Arrow.transform.localPosition = new Vector3(x, 0f, -0.1f);
				m_PrimaryObjectiveInfo.m_Arrow.transform.localScale = new Vector3(num, num, num);
			}
			else
			{
				m_PrimaryObjectiveInfo.m_Pivot.gameObject.SetActiveRecursively(false);
			}
			vector2 *= m_RadarRoot.worldUnitsPerScreenPixel;
			vector2.z = -1f;
			m_PrimaryObjectiveInfo.m_Root.transform.localPosition = vector2;
		}
		if (Globals.m_SecondaryObjective == null)
		{
			m_SecondaryObjectiveInfo.m_Root.gameObject.SetActiveRecursively(false);
			return;
		}
		m_SecondaryObjectiveInfo.m_Root.gameObject.SetActiveRecursively(true);
		vector = Globals.m_CameraController.camera.WorldToScreenPoint(Globals.m_SecondaryObjective.position);
		vector.x -= (float)Screen.width * 0.5f;
		vector.y -= (float)Screen.height * 0.5f;
		if (vector.z < 0f)
		{
			vector *= -1f;
			vector.y = -20000f;
		}
		vector2 = vector;
		vector2.x = Mathf.Clamp(vector2.x, (float)(-Screen.width) * 0.5f + (float)m_MarkerBuffer, (float)Screen.width * 0.5f - (float)m_MarkerBuffer);
		vector2.y = Mathf.Clamp(vector2.y, (float)(-Screen.height) * 0.5f + (float)m_MarkerBuffer, (float)Screen.height * 0.5f - (float)m_MarkerBuffer);
		localPosition = m_SecondaryObjectiveInfo.m_Plaque.transform.localPosition;
		if (vector2.x - 5f >= 0f && localPosition.x >= 0f)
		{
			localPosition.x = -1.413036f;
		}
		else if (vector2.x + 5f < 0f && localPosition.x < 0f)
		{
			localPosition.x = 0.15f;
		}
		m_SecondaryObjectiveInfo.m_Plaque.transform.localPosition = localPosition;
		primaryObjectiveColor = m_SecondaryObjectiveColor;
		primaryObjectiveColor.a = a * 0.75f;
		m_SecondaryObjectiveInfo.m_Root.SetColor(primaryObjectiveColor);
		m_SecondaryObjectiveInfo.m_Plaque.SetColor(primaryObjectiveColor);
		t = (Globals.m_SecondaryObjective.position - Globals.m_CameraController.transform.position).magnitude;
		m_SecondaryObjectiveInfo.m_Distance.SetColor(primaryObjectiveColor);
		m_SecondaryObjectiveInfo.m_Distance.Text = (int)t + "M";
		m_SecondaryObjectiveInfo.m_Number.SetColor(primaryObjectiveColor);
		m_SecondaryObjectiveInfo.m_Number.Text = "S1";
		if (vector.x != vector2.x || vector.y != vector2.y)
		{
			m_SecondaryObjectiveInfo.m_Pivot.gameObject.SetActiveRecursively(true);
			m_SecondaryObjectiveInfo.m_Arrow.SetColor(primaryObjectiveColor);
			vector -= vector2;
			t = 57.29578f * Mathf.Atan2(vector.y, vector.x);
			m_SecondaryObjectiveInfo.m_Pivot.localRotation = Quaternion.Euler(0f, 0f, t);
			m_SecondaryObjectiveInfo.m_Arrow.transform.localPosition = new Vector3(x, 0f, -0.1f);
			m_SecondaryObjectiveInfo.m_Arrow.transform.localScale = new Vector3(num, num, num);
		}
		else
		{
			m_SecondaryObjectiveInfo.m_Pivot.gameObject.SetActiveRecursively(false);
		}
		vector2 *= m_RadarRoot.worldUnitsPerScreenPixel;
		vector2.z = -1f;
		m_SecondaryObjectiveInfo.m_Root.transform.localPosition = vector2;
	}
}
