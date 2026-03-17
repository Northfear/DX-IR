using System;
using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;

public class MainHUD : MonoBehaviour
{
	[Serializable]
	public class QuickSlotPiece
	{
		[HideInInspector]
		public Vector3 m_OriginalLocalPosition = Vector3.zero;

		public UIButton m_Root;

		public PackedSprite m_Fill;

		public SimpleSprite m_Icon;

		public SpriteText m_Quantity;

		public SpriteText m_Clip;

		public Collider m_Collider;
	}

	public struct GrenadeIndicatorInfo
	{
		public PackedSprite m_Root;

		public Transform m_Pivot;

		public PackedSprite m_Glow;

		public PackedSprite m_Arrow;
	}

	public enum CoverFlipButtonSide
	{
		Left = 0,
		Right = 1
	}

	public const int m_MaxEnergy = 5;

	public const int m_MaxArmor = 33;

	public UIPanel m_HUDPanel;

	[HideInInspector]
	public bool m_Showing;

	public PackedSprite m_SavingText;

	public SpriteText m_SavingTextText;

	public Color m_EnergyFullColor;

	public Color m_EnergyEmptyColor;

	public Color m_EnergyChargingColor;

	private float m_CurrentFlashTime;

	private float m_FlashTimeTotalDuration = 0.8f;

	public Color m_BackgroundFlash = new Color(0.5f, 0f, 0f, 1f);

	private Color m_CurrentFlashColor = Color.white;

	public SpriteText m_FPS;

	public SpriteText m_CurrentHealth;

	public PackedSprite[] m_HealthUnits;

	public PackedSprite m_HealthBackground;

	private float m_CurrentChargeTimeOffset;

	private float m_ChargeTimeFlashDuration = 0.5f;

	private float m_ChargeTimeCycleDuration = 1.25f;

	public PackedSprite[] m_EnergyContainers = new PackedSprite[5];

	public PackedSprite[] m_EnergyBars = new PackedSprite[5];

	public PackedSprite m_EnergyParent;

	public PackedSprite m_EnergyBackground;

	public float m_ArmorFillRate = 50f;

	private float m_CurrentArmor;

	private float m_TargetArmor;

	public PackedSprite[] m_ArmorUnits = new PackedSprite[33];

	public PackedSprite m_ArmorBackground;

	public UIButton m_CoverFlipButtonLeft;

	public UIButton m_CoverFlipButtonRight;

	public UIButton m_CoverFlipInnerButtonLeft;

	public UIButton m_CoverFlipInnerButtonRight;

	public UIButton m_CoverFlipOuterButtonLeft;

	public UIButton m_CoverFlipOuterButtonRight;

	public UIButton m_VaultButton;

	public PackedSprite m_CoverButtonParent;

	public UIButton m_CoverButtonEnter;

	public UIButton m_CoverButtonExit;

	public PackedSprite m_StanceBackground;

	public UIButton m_StanceStanding;

	public UIButton m_StanceCrouching;

	public GameObject m_PassThruButton;

	private float m_QuickMenuHoldTime = 0.2f;

	private float m_QuickSlotAnimateInDuration = 0.25f;

	private float m_QuickSlotAnimateInDelay = 0.1f;

	private float m_QuickSlotAnimateOutDuration = 0.2f;

	public GameObject m_DraggableWeaponParent;

	public QuickSlotPiece[] m_WeaponQuickSlots = new QuickSlotPiece[4];

	private int m_WeaponPointerID;

	private int m_WeaponPointerCameraID;

	private bool m_WeaponQuickSlotsOpen;

	private bool m_WeaponPressed;

	private float m_WeaponHoldTime;

	private int m_PreviousWeaponSelected = -1;

	public GameObject m_DraggableGrenadeParent;

	public QuickSlotPiece[] m_GrenadeQuickSlots = new QuickSlotPiece[6];

	private int m_GrenadePointerID;

	private int m_GrenadePointerCameraID;

	private bool m_GrenadeQuickSlotsOpen;

	private bool m_GrenadePressed;

	private float m_GrenadeHoldTime;

	private int m_PreviousGrenadeSelected = -1;

	public GameObject m_DraggableItemParent;

	public QuickSlotPiece[] m_ItemQuickSlots = new QuickSlotPiece[4];

	private int m_ItemPointerID;

	private int m_ItemPointerCameraID;

	private bool m_ItemQuickSlotsOpen;

	private bool m_ItemPressed;

	private float m_ItemHoldTime;

	private int m_PreviousItemSelected = -1;

	public UIButton m_WeaponButton;

	public SimpleSprite m_CurrentWeaponIcon;

	public SpriteText m_CurrentWeaponAmmo;

	public SpriteText m_CurrentWeaponTotalAmmo;

	public UIButton m_GrenadeButton;

	public SimpleSprite m_CurrentGrenadeIcon;

	public SpriteText m_CurrentGrenadeQuantity;

	public UIButton m_ItemButton;

	public SimpleSprite m_CurrentItemIcon;

	public SpriteText m_CurrentItemQuantity;

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

	public PackedSprite m_CameraIconPrefab;

	public PackedSprite m_SentryIconPrefab;

	private LinkedList<PackedSprite> m_EnemyIcons = new LinkedList<PackedSprite>();

	private LinkedList<PackedSprite> m_TurretIcons = new LinkedList<PackedSprite>();

	private LinkedList<PackedSprite> m_CameraIcons = new LinkedList<PackedSprite>();

	private LinkedList<PackedSprite> m_SentryIcons = new LinkedList<PackedSprite>();

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

	private LinkedList<ObjectiveMarkerInfo> m_ObjectiveMarkers = new LinkedList<ObjectiveMarkerInfo>();

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

	public BTButton m_PlayerSubtitleBackground;

	public SpriteText m_PlayerSubtitle;

	public PackedSprite m_EnergyWarningBackground;

	public PackedSprite m_EnergyWarningIcon;

	public SpriteText m_EnergyWarningText;

	private float m_EnergyWarningTimer = -1f;

	public BTButton m_AmmoWarningBackground;

	public PackedSprite m_AmmoWarningIcon;

	public SpriteText m_AmmoWarningText;

	public PackedSprite m_AmmoWarningIconOverWeapon;

	private float m_WarningOverWeaponTimer;

	private float m_GrenadeWarningTimer = -1f;

	private float m_ItemWarningTimer = -1f;

	private void Awake()
	{
		Globals.m_HUD = this;
		m_RadarWorldRangeSqr = Mathf.Max(m_RadarWorldRange.x, m_RadarWorldRange.y);
		m_RadarWorldRangeSqr *= m_RadarWorldRangeSqr;
	}

	private void Start()
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
		localPosition = m_SavingText.transform.localPosition;
		localPosition.x = 0f;
		m_SavingText.transform.localPosition = localPosition;
		TurnOnCoverButton(false);
		TurnOnCoverFlipButton(false, CoverFlipButtonSide.Left);
		TurnOnCoverFlipButton(false, CoverFlipButtonSide.Right);
		TurnOnCoverInnerFlipButton(false, CoverFlipButtonSide.Left);
		TurnOnCoverInnerFlipButton(false, CoverFlipButtonSide.Right);
		TurnOnCoverOuterFlipButton(false, CoverFlipButtonSide.Left);
		TurnOnCoverOuterFlipButton(false, CoverFlipButtonSide.Right);
		TurnOnVaultButton(false);
		m_SavingText.gameObject.SetActiveRecursively(false);
		SetupQuickWeapons();
		CloseQuickWeapons(true);
		SetupQuickGrenades();
		CloseQuickGrenades(true);
		SetupQuickItems();
		CloseQuickItems(true);
		SetWeaponIcon();
		SetItemIcon();
		SetGrenadeIcon();
		SetStance();
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
		ReAdjustQuickGrenadeMenu();
		HidePlayerSubtitle();
		ReAdjustWarningIndicatorPosition();
		ReAdjustCommLink();
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

	public void ShowSavingText()
	{
		m_SavingText.gameObject.SetActiveRecursively(true);
		m_SavingText.SetColor(m_PrimaryObjectiveColor);
		m_SavingTextText.SetColor(Color.white);
		GameManager.m_SaveTextTimer = 2f;
		GameManager.m_SaveTextTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		if (!m_WeaponQuickSlotsOpen && m_WeaponPressed)
		{
			m_WeaponHoldTime += Time.deltaTime;
			if (m_WeaponHoldTime >= m_QuickMenuHoldTime)
			{
				OpenQuickWeapons();
			}
		}
		if (m_WeaponQuickSlotsOpen)
		{
			UpdateQuickWeaponsInput();
		}
		if (!m_ItemQuickSlotsOpen && m_ItemPressed)
		{
			m_ItemHoldTime += Time.deltaTime;
			if (m_ItemHoldTime >= m_QuickMenuHoldTime)
			{
				OpenQuickItems();
			}
		}
		if (m_ItemQuickSlotsOpen)
		{
			UpdateQuickItemsInput();
		}
		if (!m_GrenadeQuickSlotsOpen && m_GrenadePressed)
		{
			m_GrenadeHoldTime += Time.deltaTime;
			if (m_GrenadeHoldTime >= m_QuickMenuHoldTime)
			{
				OpenGrenadeMenu();
			}
		}
		if (m_GrenadeQuickSlotsOpen)
		{
			UpdateQuickGrenadesInput();
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
			m_CurrentFlashColor = Color.Lerp(Globals.m_This.m_BlackHUD, m_BackgroundFlash, m_CurrentFlashTime / (m_FlashTimeTotalDuration * 0.5f));
		}
		else
		{
			m_CurrentFlashColor = Color.Lerp(m_BackgroundFlash, Globals.m_This.m_BlackHUD, Mathf.InverseLerp(m_FlashTimeTotalDuration * 0.5f, m_FlashTimeTotalDuration, m_CurrentFlashTime));
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
		UpdateWeaponQuickSlotFills();
		UpdateItemQuickSlotFills();
		UpdateGrenadeQuickSlotFills();
		UpdateArmorMeter();
		UpdateHealthBackground();
		UpdateArmorBackground();
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
			SetWeaponIcon();
			SetItemIcon();
			SetGrenadeIcon();
			CloseQuickWeapons(true);
			CloseQuickItems(true);
			CloseQuickGrenades(true);
			TurnOnCoverButton(false);
			TurnOnCoverFlipButton(false, CoverFlipButtonSide.Left);
			TurnOnCoverFlipButton(false, CoverFlipButtonSide.Right);
			TurnOnCoverInnerFlipButton(false, CoverFlipButtonSide.Left);
			TurnOnCoverInnerFlipButton(false, CoverFlipButtonSide.Right);
			TurnOnCoverOuterFlipButton(false, CoverFlipButtonSide.Left);
			TurnOnCoverOuterFlipButton(false, CoverFlipButtonSide.Right);
			TurnOnVaultButton(false);
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

	public void SetCurrentAmmo(int clipammo, int totalammo)
	{
		if ((bool)m_CurrentWeaponAmmo)
		{
			m_CurrentWeaponAmmo.Text = clipammo.ToString();
		}
		if ((bool)m_CurrentWeaponTotalAmmo)
		{
			m_CurrentWeaponTotalAmmo.Text = totalammo.ToString();
		}
	}

	public void SetCurrentHealth(int health)
	{
		if ((bool)m_CurrentHealth)
		{
			m_CurrentHealth.Text = health.ToString();
			m_CurrentHealth.Color = Globals.m_This.m_BrightHUD;
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
				m_HealthUnits[i].Color = Globals.m_This.m_BrightHUD;
			}
			else
			{
				m_HealthUnits[i].Color = Globals.m_This.m_DarkHUD;
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
		for (int i = 0; i < 5; i++)
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

	public void SetCurrentArmor(float ArmorAmount, bool InstantChange)
	{
		if (InstantChange)
		{
			m_TargetArmor = (m_CurrentArmor = ArmorAmount);
			SetupArmorMeterUnits(ArmorAmount);
		}
		else
		{
			m_TargetArmor = ArmorAmount;
		}
	}

	public void SetupArmorMeterUnits(float ArmorAmount)
	{
		int num = (int)(0.33f * ArmorAmount);
		for (int i = 0; i < 33; i++)
		{
			m_ArmorUnits[i].Hide((i >= num) ? true : false);
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
			Color blackHUD = Globals.m_This.m_BlackHUD;
			blackHUD.a = m_EnergyParent.Color.a * blackHUD.a;
			m_EnergyBackground.SetColor(blackHUD);
		}
	}

	private void UpdateHealthBackground()
	{
		Color blackHUD = Globals.m_This.m_BlackHUD;
		blackHUD.a = m_EnergyParent.Color.a * blackHUD.a;
		m_HealthBackground.SetColor(blackHUD);
	}

	private void UpdateArmorBackground()
	{
		Color blackHUD = Globals.m_This.m_BlackHUD;
		blackHUD.a = m_EnergyParent.Color.a * blackHUD.a;
		m_ArmorBackground.SetColor(blackHUD);
	}

	public void WeaponButtonTapped()
	{
		Globals.m_PlayerController.ReloadButtonTapped();
	}

	public void ItemButtonTapped()
	{
		if (!m_ItemQuickSlotsOpen)
		{
			if (Globals.m_Inventory.UseItemQuickslot())
			{
				SetItemIcon();
			}
			else
			{
				SoundManager.TriggerEvent("Play_UI_Error", base.gameObject);
			}
		}
	}

	public void GrenadeButtonTapped()
	{
		if (!m_GrenadeQuickSlotsOpen)
		{
			if (Globals.m_Inventory.UseGrenadeQuickslot())
			{
				SetGrenadeIcon();
			}
			else
			{
				SoundManager.TriggerEvent("Play_UI_Error", base.gameObject);
			}
		}
	}

	public void WeaponButtonPressed(POINTER_INFO ptr)
	{
		m_WeaponHoldTime = 0f;
		m_WeaponPressed = true;
		m_WeaponPointerID = ptr.id;
		m_WeaponPointerCameraID = UIManager.instance.GetCameraID(ptr.camera);
	}

	public void ItemButtonPressed(POINTER_INFO ptr)
	{
		m_ItemHoldTime = 0f;
		m_ItemPressed = true;
		m_ItemPointerID = ptr.id;
		m_ItemPointerCameraID = UIManager.instance.GetCameraID(ptr.camera);
	}

	public void GrenadeButtonPressed(POINTER_INFO ptr)
	{
		m_GrenadeHoldTime = 0f;
		m_GrenadePressed = true;
		m_GrenadePointerID = ptr.id;
		m_GrenadePointerCameraID = UIManager.instance.GetCameraID(ptr.camera);
	}

	public void UserReleased(Vector2 position, int fingerID)
	{
		POINTER_INFO ptr;
		if (m_WeaponPressed)
		{
			UIManager.instance.GetPointer(m_WeaponPointerID, m_WeaponPointerCameraID, out ptr);
			if (fingerID == ptr.id)
			{
				m_WeaponPressed = false;
				CloseQuickWeapons(false);
			}
		}
		if (m_ItemPressed)
		{
			UIManager.instance.GetPointer(m_ItemPointerID, m_ItemPointerCameraID, out ptr);
			if (fingerID == ptr.id)
			{
				m_ItemPressed = false;
				CloseQuickItems(false);
			}
		}
		if (m_GrenadePressed)
		{
			UIManager.instance.GetPointer(m_GrenadePointerID, m_GrenadePointerCameraID, out ptr);
			if (fingerID == ptr.id)
			{
				m_GrenadePressed = false;
				CloseQuickGrenades(false);
			}
		}
	}

	public void WeaponButtonReleased(POINTER_INFO ptr)
	{
		if (m_PreviousWeaponSelected >= 0 && Globals.m_Inventory.SelectActiveWeaponQuickslot(m_PreviousWeaponSelected))
		{
			SetWeaponIcon();
		}
		m_PreviousWeaponSelected = -1;
		m_WeaponPressed = false;
		CloseQuickWeapons(false);
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
	}

	public void ItemButtonReleased(POINTER_INFO ptr)
	{
		if (m_PreviousItemSelected >= 0 && Globals.m_Inventory.SelectActiveItemQuickslot(m_PreviousItemSelected))
		{
			SetItemIcon();
		}
		m_PreviousItemSelected = -1;
		m_ItemPressed = false;
		CloseQuickItems(false);
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
	}

	public void GrenadeButtonReleased(POINTER_INFO ptr)
	{
		if (m_PreviousGrenadeSelected >= 0 && Globals.m_Inventory.SelectActiveGrenadeQuickslot(m_PreviousGrenadeSelected))
		{
			SetGrenadeIcon();
		}
		m_PreviousGrenadeSelected = -1;
		m_GrenadePressed = false;
		CloseQuickGrenades(false);
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
	}

	public void SetWeaponIcon()
	{
		Item_Weapon weaponQuickslotItem = Globals.m_Inventory.GetWeaponQuickslotItem(Globals.m_Inventory.m_ActiveWeaponQuickslot);
		if (weaponQuickslotItem == null || weaponQuickslotItem.m_Texture == null)
		{
			m_CurrentWeaponIcon.Hide(true);
			m_CurrentWeaponAmmo.Hide(true);
			m_CurrentWeaponTotalAmmo.Hide(true);
			return;
		}
		m_CurrentWeaponIcon.Hide(false);
		m_CurrentWeaponAmmo.Hide(false);
		m_CurrentWeaponTotalAmmo.Hide(false);
		m_CurrentWeaponIcon.renderer.material.mainTexture = weaponQuickslotItem.m_Texture;
		m_CurrentWeaponAmmo.Text = weaponQuickslotItem.m_CurrentAmmoInClip.ToString();
		m_CurrentWeaponTotalAmmo.Text = Globals.m_Inventory.GetItemQuantity(2, weaponQuickslotItem.m_AmmoItemID).ToString();
	}

	public void SetItemIcon()
	{
		m_CurrentItemIcon.renderer.material.mainTexture = Globals.m_Inventory.GetItemQuickSlotItemTexture();
		if (m_CurrentItemIcon.renderer.material.mainTexture == null)
		{
			m_CurrentItemIcon.Hide(true);
			m_CurrentItemQuantity.Hide(true);
			return;
		}
		m_CurrentItemIcon.Hide(false);
		if (Globals.m_Inventory.GetItemQuickSlotItemMaxQuantity() != 1)
		{
			m_CurrentItemQuantity.Hide(false);
			m_CurrentItemQuantity.Text = Globals.m_Inventory.GetItemQuickSlotItemQuantity().ToString();
		}
		else
		{
			m_CurrentItemQuantity.Hide(true);
		}
		if (Globals.m_Inventory.m_ItemQuickslots[Globals.m_Inventory.m_ActiveItemQuickslot].m_CategoryID == 4)
		{
			m_CurrentItemIcon.transform.localScale = Vector3.one;
		}
		else
		{
			m_CurrentItemIcon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}
	}

	public void SetGrenadeIcon()
	{
		m_CurrentGrenadeIcon.renderer.material.mainTexture = Globals.m_Inventory.GetGrenadeQuickSlotItemTexture();
		if (m_CurrentGrenadeIcon.renderer.material.mainTexture == null)
		{
			m_CurrentGrenadeIcon.Hide(true);
			m_CurrentGrenadeQuantity.Hide(true);
		}
		else
		{
			m_CurrentGrenadeIcon.Hide(false);
			m_CurrentGrenadeQuantity.Hide(false);
			m_CurrentGrenadeQuantity.Text = Globals.m_Inventory.GetGrenadeQuickSlotItemQuantity().ToString();
		}
	}

	private void SetupQuickWeapons()
	{
		for (int i = 0; i < 4; i++)
		{
			m_WeaponQuickSlots[i].m_OriginalLocalPosition = m_WeaponQuickSlots[i].m_Root.transform.localPosition;
		}
	}

	private void SetupQuickItems()
	{
		for (int i = 0; i < 4; i++)
		{
			m_ItemQuickSlots[i].m_OriginalLocalPosition = m_ItemQuickSlots[i].m_Root.transform.localPosition;
		}
	}

	private void SetupQuickGrenades()
	{
		for (int i = 0; i < 6; i++)
		{
			m_GrenadeQuickSlots[i].m_OriginalLocalPosition = m_GrenadeQuickSlots[i].m_Root.transform.localPosition;
		}
	}

	private void OpenQuickWeapons()
	{
		if (m_WeaponQuickSlotsOpen)
		{
			return;
		}
		Vector3 begin = Vector3.zero;
		for (int i = 0; i < 4; i++)
		{
			m_WeaponQuickSlots[i].m_Root.gameObject.SetActiveRecursively(true);
			m_WeaponQuickSlots[i].m_Root.SetColor(Globals.m_ClearWhite);
			if (Globals.m_Inventory.m_WeaponQuickslots[i].IsValid())
			{
				m_WeaponQuickSlots[i].m_Icon.Hide(false);
				m_WeaponQuickSlots[i].m_Icon.renderer.material.mainTexture = Globals.m_Inventory.GetWeaponQuickSlotItemTexture(i);
				m_WeaponQuickSlots[i].m_Quantity.Hide(false);
				m_WeaponQuickSlots[i].m_Clip.Hide(false);
				m_WeaponQuickSlots[i].m_Clip.Text = Globals.m_Inventory.GetWeaponQuickslotItem(i).m_CurrentAmmoInClip.ToString();
				m_WeaponQuickSlots[i].m_Quantity.Text = Globals.m_Inventory.GetItemQuantity(2, Globals.m_Inventory.GetWeaponQuickslotItem(i).m_AmmoItemID).ToString();
			}
			else
			{
				m_WeaponQuickSlots[i].m_Icon.Hide(true);
				m_WeaponQuickSlots[i].m_Quantity.Hide(true);
				m_WeaponQuickSlots[i].m_Clip.Hide(true);
			}
			AnimatePosition.Do(m_WeaponQuickSlots[i].m_Root.gameObject, EZAnimation.ANIM_MODE.FromTo, begin, m_WeaponQuickSlots[i].m_OriginalLocalPosition, EZAnimation.sinusOut, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			FadeSpriteAlpha.Do(m_WeaponQuickSlots[i].m_Root, EZAnimation.ANIM_MODE.FromTo, Globals.m_ClearWhite, Color.white, EZAnimation.linear, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			begin = m_WeaponQuickSlots[i].m_OriginalLocalPosition;
		}
		m_WeaponQuickSlotsOpen = true;
		m_PreviousWeaponSelected = -1;
		SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
	}

	private void OpenQuickItems()
	{
		if (m_ItemQuickSlotsOpen)
		{
			return;
		}
		Color brightHUD = Globals.m_This.m_BrightHUD;
		brightHUD.a = 0f;
		Vector3 begin = Vector3.zero;
		for (int i = 0; i < 4; i++)
		{
			m_ItemQuickSlots[i].m_Root.gameObject.SetActiveRecursively(true);
			m_ItemQuickSlots[i].m_Root.SetColor(brightHUD);
			if (Globals.m_Inventory.m_ItemQuickslots[i].IsValid())
			{
				m_ItemQuickSlots[i].m_Icon.Hide(false);
				m_ItemQuickSlots[i].m_Icon.renderer.material.mainTexture = Globals.m_Inventory.GetItemQuickSlotItemTexture(i);
				if (Globals.m_Inventory.m_ItemQuickslots[i].m_CategoryID == 4)
				{
					m_ItemQuickSlots[i].m_Icon.transform.localScale = Vector3.one;
				}
				else
				{
					m_ItemQuickSlots[i].m_Icon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				}
				if (Globals.m_Inventory.GetItemQuickSlotItemMaxQuantity(i) != 1)
				{
					m_ItemQuickSlots[i].m_Quantity.Hide(false);
					m_ItemQuickSlots[i].m_Quantity.Text = Globals.m_Inventory.GetItemQuickSlotItemQuantity(i).ToString();
				}
				else
				{
					m_ItemQuickSlots[i].m_Quantity.Hide(true);
				}
			}
			else
			{
				m_ItemQuickSlots[i].m_Icon.Hide(true);
				m_ItemQuickSlots[i].m_Quantity.Hide(true);
			}
			AnimatePosition.Do(m_ItemQuickSlots[i].m_Root.gameObject, EZAnimation.ANIM_MODE.FromTo, begin, m_ItemQuickSlots[i].m_OriginalLocalPosition, EZAnimation.sinusOut, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			FadeSpriteAlpha.Do(m_ItemQuickSlots[i].m_Root, EZAnimation.ANIM_MODE.FromTo, brightHUD, Globals.m_This.m_BrightHUD, EZAnimation.linear, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			begin = m_ItemQuickSlots[i].m_OriginalLocalPosition;
		}
		m_ItemQuickSlotsOpen = true;
		m_PreviousItemSelected = -1;
		SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
	}

	private void OpenGrenadeMenu()
	{
		if (m_GrenadeQuickSlotsOpen)
		{
			return;
		}
		Color brightHUD = Globals.m_This.m_BrightHUD;
		brightHUD.a = 0f;
		Vector3 begin = Vector3.zero;
		for (int i = 0; i < 6; i++)
		{
			m_GrenadeQuickSlots[i].m_Root.gameObject.SetActiveRecursively(true);
			m_GrenadeQuickSlots[i].m_Root.SetColor(brightHUD);
			if (Globals.m_Inventory.m_GrenadeQuickslots[i].IsValid())
			{
				m_GrenadeQuickSlots[i].m_Icon.Hide(false);
				m_GrenadeQuickSlots[i].m_Icon.renderer.material.mainTexture = Globals.m_Inventory.GetGrenadeQuickSlotItemTexture(i);
				m_GrenadeQuickSlots[i].m_Quantity.Hide(false);
				m_GrenadeQuickSlots[i].m_Quantity.Text = Globals.m_Inventory.GetGrenadeQuickSlotItemQuantity(i).ToString();
			}
			else
			{
				m_GrenadeQuickSlots[i].m_Icon.Hide(true);
				m_GrenadeQuickSlots[i].m_Quantity.Hide(true);
			}
			AnimatePosition.Do(m_GrenadeQuickSlots[i].m_Root.gameObject, EZAnimation.ANIM_MODE.FromTo, begin, m_GrenadeQuickSlots[i].m_OriginalLocalPosition, EZAnimation.sinusOut, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			FadeSpriteAlpha.Do(m_GrenadeQuickSlots[i].m_Root, EZAnimation.ANIM_MODE.FromTo, brightHUD, Globals.m_This.m_BrightHUD, EZAnimation.linear, m_QuickSlotAnimateInDuration, (float)i * m_QuickSlotAnimateInDelay, null, null);
			begin = m_GrenadeQuickSlots[i].m_OriginalLocalPosition;
		}
		m_GrenadeQuickSlotsOpen = true;
		m_PreviousItemSelected = -1;
		SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
	}

	private void CloseQuickWeapons(bool ForceClose = false)
	{
		if (!m_WeaponQuickSlotsOpen && !ForceClose)
		{
			return;
		}
		Color brightHUD = Globals.m_This.m_BrightHUD;
		brightHUD.a = 0f;
		for (int i = 0; i < 4; i++)
		{
			FadeSpriteAlpha fadeSpriteAlpha = FadeSpriteAlpha.Do(m_WeaponQuickSlots[i].m_Root, EZAnimation.ANIM_MODE.To, brightHUD, EZAnimation.linear, m_QuickSlotAnimateOutDuration, 0f, null, QuickRootFaded);
			if (ForceClose)
			{
				fadeSpriteAlpha.End();
			}
		}
		m_WeaponQuickSlotsOpen = false;
		if (!ForceClose)
		{
			SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
		}
	}

	private void CloseQuickItems(bool ForceClose = false)
	{
		if (!m_ItemQuickSlotsOpen && !ForceClose)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			FadeSpriteAlpha fadeSpriteAlpha = FadeSpriteAlpha.Do(m_ItemQuickSlots[i].m_Root, EZAnimation.ANIM_MODE.To, Globals.m_ClearWhite, EZAnimation.linear, m_QuickSlotAnimateOutDuration, 0f, null, QuickRootFaded);
			if (ForceClose)
			{
				fadeSpriteAlpha.End();
			}
		}
		m_ItemQuickSlotsOpen = false;
		if (!ForceClose)
		{
			SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
		}
	}

	private void CloseQuickGrenades(bool ForceClose = false)
	{
		if (!m_GrenadeQuickSlotsOpen && !ForceClose)
		{
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			FadeSpriteAlpha fadeSpriteAlpha = FadeSpriteAlpha.Do(m_GrenadeQuickSlots[i].m_Root, EZAnimation.ANIM_MODE.To, Globals.m_ClearWhite, EZAnimation.linear, m_QuickSlotAnimateOutDuration, 0f, null, QuickRootFaded);
			if (ForceClose)
			{
				fadeSpriteAlpha.End();
			}
		}
		m_GrenadeQuickSlotsOpen = false;
		if (!ForceClose)
		{
			SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
		}
	}

	public void QuickRootFaded(EZAnimation anim)
	{
		(anim.GetSubject() as UIButton).gameObject.SetActiveRecursively(false);
	}

	private void UpdateQuickWeaponsInput()
	{
		POINTER_INFO ptr;
		UIManager.instance.GetPointer(m_WeaponPointerID, m_WeaponPointerCameraID, out ptr);
		int num = -1;
		Ray ray = UIManager.instance.uiCameras[m_WeaponPointerCameraID].camera.ScreenPointToRay(ptr.devicePos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, ptr.rayDepth, ptr.layerMask))
		{
			for (int i = 0; i < 4; i++)
			{
				if (hitInfo.collider == m_WeaponQuickSlots[i].m_Collider)
				{
					num = i;
					break;
				}
			}
		}
		if (num >= 0 && num != m_PreviousWeaponSelected)
		{
			SoundManager.TriggerEvent("Play_UI_Toggle", base.gameObject);
		}
		m_PreviousWeaponSelected = num;
	}

	private void UpdateQuickItemsInput()
	{
		POINTER_INFO ptr;
		UIManager.instance.GetPointer(m_ItemPointerID, m_ItemPointerCameraID, out ptr);
		int num = -1;
		Ray ray = UIManager.instance.uiCameras[m_ItemPointerCameraID].camera.ScreenPointToRay(ptr.devicePos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, ptr.rayDepth, ptr.layerMask))
		{
			for (int i = 0; i < 4; i++)
			{
				if (hitInfo.collider == m_ItemQuickSlots[i].m_Collider)
				{
					num = i;
					break;
				}
			}
		}
		if (num >= 0 && num != m_PreviousItemSelected)
		{
			SoundManager.TriggerEvent("Play_UI_Toggle", base.gameObject);
		}
		m_PreviousItemSelected = num;
	}

	private void UpdateQuickGrenadesInput()
	{
		POINTER_INFO ptr;
		UIManager.instance.GetPointer(m_GrenadePointerID, m_GrenadePointerCameraID, out ptr);
		int num = -1;
		Ray ray = UIManager.instance.uiCameras[m_GrenadePointerCameraID].camera.ScreenPointToRay(ptr.devicePos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, ptr.rayDepth, ptr.layerMask))
		{
			for (int i = 0; i < 6; i++)
			{
				if (hitInfo.collider == m_GrenadeQuickSlots[i].m_Collider)
				{
					num = i;
					break;
				}
			}
		}
		if (num >= 0 && num != m_PreviousGrenadeSelected)
		{
			SoundManager.TriggerEvent("Play_UI_Toggle", base.gameObject);
		}
		m_PreviousGrenadeSelected = num;
	}

	private void UpdateWeaponQuickSlotFills()
	{
		if (m_WeaponQuickSlotsOpen)
		{
			for (int i = 0; i < 4; i++)
			{
				Color white = Color.white;
				white.a *= m_WeaponQuickSlots[i].m_Root.Color.a;
				m_WeaponQuickSlots[i].m_Quantity.SetColor(white);
				m_WeaponQuickSlots[i].m_Clip.SetColor(white);
				m_WeaponQuickSlots[i].m_Icon.SetColor(white);
				white = ((m_PreviousWeaponSelected != i) ? Globals.m_This.m_BlackHUD : Globals.m_This.m_BrightHUD);
				white.a *= m_WeaponQuickSlots[i].m_Root.Color.a;
				m_WeaponQuickSlots[i].m_Fill.SetColor(white);
			}
		}
	}

	private void UpdateItemQuickSlotFills()
	{
		if (m_ItemQuickSlotsOpen)
		{
			Color white = Color.white;
			for (int i = 0; i < 4; i++)
			{
				white = Color.white;
				white.a *= m_ItemQuickSlots[i].m_Root.Color.a;
				m_ItemQuickSlots[i].m_Quantity.SetColor(white);
				m_ItemQuickSlots[i].m_Icon.SetColor(white);
				white = ((m_PreviousItemSelected != i) ? Globals.m_This.m_BlackHUD : Globals.m_This.m_BrightHUD);
				white.a *= m_ItemQuickSlots[i].m_Root.Color.a;
				m_ItemQuickSlots[i].m_Fill.SetColor(white);
			}
		}
	}

	private void UpdateGrenadeQuickSlotFills()
	{
		if (m_GrenadeQuickSlotsOpen)
		{
			Color white = Color.white;
			for (int i = 0; i < 6; i++)
			{
				white = Color.white;
				white.a *= m_GrenadeQuickSlots[i].m_Root.Color.a;
				m_GrenadeQuickSlots[i].m_Quantity.SetColor(white);
				m_GrenadeQuickSlots[i].m_Icon.SetColor(white);
				white = ((m_PreviousGrenadeSelected != i) ? Globals.m_This.m_BlackHUD : Globals.m_This.m_BrightHUD);
				white.a *= m_GrenadeQuickSlots[i].m_Root.Color.a;
				m_GrenadeQuickSlots[i].m_Fill.SetColor(white);
			}
		}
	}

	private void UpdateArmorMeter()
	{
		if (m_CurrentArmor != m_TargetArmor)
		{
			if (m_CurrentArmor < m_TargetArmor)
			{
				m_CurrentArmor += m_ArmorFillRate * Time.deltaTime;
				m_CurrentArmor = Mathf.Min(m_CurrentArmor, m_TargetArmor);
			}
			else
			{
				m_CurrentArmor -= m_ArmorFillRate * Time.deltaTime;
				m_CurrentArmor = Mathf.Max(m_CurrentArmor, m_TargetArmor);
			}
			SetupArmorMeterUnits(m_CurrentArmor);
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

	public void TurnOnVaultButton(bool Enable)
	{
		m_VaultButton.Hide(!Enable);
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
		GameManager.GamePaused(true);
		PauseTabs.OpenTabs();
		SoundManager.TriggerEvent("Play_UI_Swish", base.gameObject);
		SoundManager.TriggerEvent("Pause", base.gameObject);
		Globals.m_HUD.Display(false, true, false);
		CommLinkDialog.Paused(true);
	}

	public void NoAmmoPressed()
	{
		if (!(Globals.m_PlayerController.m_WeaponScript == null) && Globals.m_PlayerController.m_WeaponScript.m_WeaponItemID >= WeaponItemID.CombatRifle)
		{
			int ammoItemID = (Globals.m_Inventory.m_Items[1][(int)Globals.m_PlayerController.m_WeaponScript.m_WeaponItemID] as Item_Weapon).m_AmmoItemID;
			if (ammoItemID >= 0)
			{
				Time.timeScale = 0f;
				GameManager.GamePaused(true);
				PauseTabs.OpenJustTabs();
				InventoryPanel.m_This.m_ViewingCategory = 2;
				InfoPanel.OpenInfoPanel(2, ammoItemID, InventoryPanel.m_This.InfoCallback, "Return To Market");
				SoundManager.TriggerEvent("Play_UI_Swish", base.gameObject);
				SoundManager.TriggerEvent("Pause", base.gameObject);
				Globals.m_HUD.Display(false, true, false);
				CommLinkDialog.Paused(true);
			}
		}
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

	public void VaultTapped()
	{
		Globals.m_PlayerController.VaultTapped();
		SoundManager.TriggerEvent("Play_Vault", Globals.m_PlayerController.gameObject);
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
		SetWeaponIcon();
		SetItemIcon();
		SetGrenadeIcon();
		OpenCommLinkImmediate(true);
		HidePlayerSubtitle();
		CloseQuickWeapons(true);
		CloseQuickItems(true);
		CloseQuickGrenades(true);
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
		Display(false, true, false);
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
		Display(false, true, false);
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
			m_WeaponQuickSlots[i].m_OriginalLocalPosition.y = num * Mathf.Abs(m_WeaponQuickSlots[i].m_OriginalLocalPosition.y);
		}
	}

	private void ReAdjustQuickItemMenu()
	{
		float num = ((!(m_DraggableItemParent.transform.localPosition.y >= 0f)) ? 1f : (-1f));
		for (int i = 0; i < 4; i++)
		{
			m_ItemQuickSlots[i].m_OriginalLocalPosition.y = num * Mathf.Abs(m_ItemQuickSlots[i].m_OriginalLocalPosition.y);
		}
	}

	private void ReAdjustQuickGrenadeMenu()
	{
		float num = ((!(m_DraggableGrenadeParent.transform.localPosition.y >= 0f)) ? 1f : (-1f));
		for (int i = 0; i < 6; i++)
		{
			m_GrenadeQuickSlots[i].m_OriginalLocalPosition.y = num * Mathf.Abs(m_GrenadeQuickSlots[i].m_OriginalLocalPosition.y);
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
			m_AmmoWarningBackground.collider.enabled = true;
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
			m_AmmoWarningBackground.collider.enabled = false;
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
		Color color = Globals.m_This.m_BrightHUD;
		if (m_WeaponButton.controlState != UIButton.CONTROL_STATE.ACTIVE)
		{
			color = ((!m_AmmoWarningBackground.gameObject.active) ? Globals.m_This.m_BlackHUD : m_CurrentFlashColor);
		}
		color.a = m_RadarRoot.Color.a * color.a;
		m_WeaponButton.SetColor(color);
	}

	private void UpdateItemButtonFlash()
	{
		Color color = Globals.m_This.m_BrightHUD;
		if (m_ItemButton.controlState != UIButton.CONTROL_STATE.ACTIVE)
		{
			color = Globals.m_This.m_BlackHUD;
			if (Globals.m_Inventory.GetActiveItemQuickslot().m_CategoryID == 4 && Globals.m_Inventory.GetActiveItemQuickslot().m_ItemID == 7)
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
				color = Color.Lerp(Globals.m_This.m_BlackHUD, m_BackgroundFlash, t);
			}
		}
		color.a = m_RadarRoot.Color.a * color.a;
		m_ItemButton.SetColor(color);
	}

	private void UpdateGrenadeButtonFlash()
	{
		Color color = Globals.m_This.m_BrightHUD;
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
				color = Color.Lerp(Globals.m_This.m_BlackHUD, m_BackgroundFlash, t);
			}
			else
			{
				color = Globals.m_This.m_BlackHUD;
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
		Color color = Globals.m_This.m_BrightHUD;
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
			m_WarningIndicatorColor = Globals.m_This.m_BrightHUD;
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
		UpdateCameraIcons(camForward, camRight);
		UpdateSentryIcons(camForward, camRight);
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
							Color color = (linkedListNode2.Value.InCombat() ? m_HostileRadarColor : ((!linkedListNode2.Value.IsPassive()) ? m_AlarmedRadarColor : m_PassiveRadarColor));
							color.a = ((!(Mathf.Abs(linkedListNode2.Value.m_RigMotion.transform.position.y - Globals.m_CameraController.transform.position.y) >= 4f)) ? 1f : 0.4f);
							linkedListNode.Value.SetColor(color);
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
						ObjectDir.x = linkedListNode2.Value.m_FunctioningYaw.transform.forward.x;
						ObjectDir.y = linkedListNode2.Value.m_FunctioningYaw.transform.forward.z;
						ObjectDir.Normalize();
						float num = Mathf.Atan2(ObjectDir.y, ObjectDir.x) - Mathf.Atan2(CamForward.y, CamForward.x);
						linkedListNode.Value.transform.localRotation = Quaternion.Euler(0f, 0f, 57.29578f * num);
						Color color = (linkedListNode2.Value.IsAttacking() ? m_HostileRadarColor : ((!linkedListNode2.Value.IsPassive()) ? m_AlarmedRadarColor : m_PassiveRadarColor));
						color.a = ((!(Mathf.Abs(linkedListNode2.Value.transform.position.y - Globals.m_CameraController.transform.position.y) >= 4f)) ? 1f : 0.4f);
						linkedListNode.Value.SetColor(color);
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

	private void UpdateCameraIcons(Vector2 CamForward, Vector2 CamRight)
	{
		LinkedListNode<PackedSprite> linkedListNode = m_CameraIcons.First;
		for (LinkedListNode<SecurityCamera> linkedListNode2 = Globals.m_AIDirector.GetFirstSecurityCamera(); linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
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
							GameObject gameObject = UnityEngine.Object.Instantiate(m_CameraIconPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
							gameObject.transform.parent = m_RadarRoot.transform;
							linkedListNode = m_CameraIcons.AddLast(gameObject.GetComponent<PackedSprite>());
						}
						linkedListNode.Value.gameObject.active = true;
						linkedListNode.Value.transform.localPosition = new Vector3(ToObjectRelative.x, ToObjectRelative.y, -0.05f);
						ObjectDir.x = linkedListNode2.Value.m_FunctioningYaw.transform.forward.x;
						ObjectDir.y = linkedListNode2.Value.m_FunctioningYaw.transform.forward.z;
						ObjectDir.Normalize();
						float num = Mathf.Atan2(ObjectDir.y, ObjectDir.x) - Mathf.Atan2(CamForward.y, CamForward.x);
						linkedListNode.Value.transform.localRotation = Quaternion.Euler(0f, 0f, 57.29578f * num);
						Color color = (linkedListNode2.Value.IsAttacking() ? m_HostileRadarColor : ((!linkedListNode2.Value.IsPassive()) ? m_AlarmedRadarColor : m_PassiveRadarColor));
						color.a = ((!(Mathf.Abs(linkedListNode2.Value.transform.position.y - Globals.m_CameraController.transform.position.y) >= 4f)) ? 1f : 0.4f);
						linkedListNode.Value.SetColor(color);
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

	private void UpdateSentryIcons(Vector2 CamForward, Vector2 CamRight)
	{
		LinkedListNode<PackedSprite> linkedListNode = m_SentryIcons.First;
		for (LinkedListNode<Sentry> linkedListNode2 = Globals.m_AIDirector.GetFirstSentry(); linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
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
							GameObject gameObject = UnityEngine.Object.Instantiate(m_SentryIconPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
							gameObject.transform.parent = m_RadarRoot.transform;
							linkedListNode = m_SentryIcons.AddLast(gameObject.GetComponent<PackedSprite>());
						}
						linkedListNode.Value.gameObject.active = true;
						linkedListNode.Value.transform.localPosition = new Vector3(ToObjectRelative.x, ToObjectRelative.y, -0.05f);
						ObjectDir.x = linkedListNode2.Value.m_HorizontalRotator.transform.forward.x;
						ObjectDir.y = linkedListNode2.Value.m_HorizontalRotator.transform.forward.z;
						ObjectDir.Normalize();
						float num = Mathf.Atan2(ObjectDir.y, ObjectDir.x) - Mathf.Atan2(CamForward.y, CamForward.x);
						linkedListNode.Value.transform.localRotation = Quaternion.Euler(0f, 0f, 57.29578f * num);
						Color color = (linkedListNode2.Value.IsAttacking() ? m_HostileRadarColor : ((!linkedListNode2.Value.IsPassive()) ? m_AlarmedRadarColor : m_PassiveRadarColor));
						color.a = ((!(Mathf.Abs(linkedListNode2.Value.transform.position.y - Globals.m_CameraController.transform.position.y) >= 4f)) ? 1f : 0.4f);
						linkedListNode.Value.SetColor(color);
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

	private void UpdateObjectiveMarkers()
	{
		LinkedListNode<ObjectiveMarkerInfo> linkedListNode = m_ObjectiveMarkers.First;
		if (Globals.m_TrackedMissions != null)
		{
			float a = m_RadarRoot.Color.a;
			float t = Mathf.PingPong(3f * Time.time, 1f);
			float num = Mathf.Lerp(0.7f, 1f, t);
			float x = Mathf.Lerp(0.25f, 0.35f, t);
			for (int i = 0; i < Globals.m_TrackedMissions.Count; i++)
			{
				Mission mission = Globals.m_Missions[Globals.m_TrackedMissions[i]];
				if (mission == null || mission.m_Transforms == null || !mission.m_Tracked || mission.m_Status != MissionStatus.Acquired)
				{
					continue;
				}
				for (int j = 0; j < mission.m_Transforms.Length; j++)
				{
					if (!(mission.m_Transforms[j] == null))
					{
						if (linkedListNode == null)
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(m_ObjectiveMarkerPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
							gameObject.transform.parent = Globals.m_HUD.transform;
							gameObject.transform.localRotation = Quaternion.identity;
							linkedListNode = m_ObjectiveMarkers.AddLast(gameObject.GetComponent<ObjectiveMarkerInfo>());
						}
						linkedListNode.Value.m_Root.gameObject.SetActiveRecursively(true);
						Vector3 vector = Globals.m_CameraController.camera.WorldToScreenPoint(mission.m_Transforms[j].position);
						vector.x -= (float)Screen.width * 0.5f;
						vector.y -= (float)Screen.height * 0.5f;
						if (vector.z < 0f)
						{
							vector *= -1f;
							vector.y = -20000f;
						}
						Vector3 vector2 = vector;
						vector2.x = Mathf.Clamp(vector2.x, (float)(-Screen.width) * 0.5f + (float)m_MarkerBuffer, (float)Screen.width * 0.5f - (float)m_MarkerBuffer);
						vector2.y = Mathf.Clamp(vector2.y, (float)(-Screen.height) * 0.5f + (float)m_MarkerBuffer, (float)Screen.height * 0.5f - (float)m_MarkerBuffer);
						Vector3 localPosition = linkedListNode.Value.m_Plaque.transform.localPosition;
						if (vector2.x - 5f >= 0f && localPosition.x >= 0f)
						{
							localPosition.x = -1.413036f;
						}
						else if (vector2.x + 5f < 0f && localPosition.x < 0f)
						{
							localPosition.x = 0.15f;
						}
						linkedListNode.Value.m_Plaque.transform.localPosition = localPosition;
						Color color = ((!mission.m_Primary) ? m_SecondaryObjectiveColor : m_PrimaryObjectiveColor);
						color.a = a * 0.75f;
						linkedListNode.Value.m_Root.SetColor(color);
						linkedListNode.Value.m_Plaque.SetColor(color);
						t = (mission.m_Transforms[j].position - Globals.m_CameraController.transform.position).magnitude;
						linkedListNode.Value.m_Distance.SetColor(color);
						linkedListNode.Value.m_Distance.Text = (int)t + "m";
						linkedListNode.Value.m_Number.SetColor(color);
						linkedListNode.Value.m_Number.Text = ((!mission.m_Primary) ? "S" : "M") + (mission.m_LocalIndex + 1);
						if (vector.x != vector2.x || vector.y != vector2.y)
						{
							linkedListNode.Value.m_Pivot.gameObject.SetActiveRecursively(true);
							linkedListNode.Value.m_Arrow.SetColor(color);
							vector -= vector2;
							float z = 57.29578f * Mathf.Atan2(vector.y, vector.x);
							linkedListNode.Value.m_Pivot.localRotation = Quaternion.Euler(0f, 0f, z);
							linkedListNode.Value.m_Arrow.transform.localPosition = new Vector3(x, 0f, -0.1f);
							linkedListNode.Value.m_Arrow.transform.localScale = new Vector3(num, num, num);
						}
						else
						{
							linkedListNode.Value.m_Pivot.gameObject.SetActiveRecursively(false);
						}
						vector2 *= m_RadarRoot.worldUnitsPerScreenPixel;
						vector2.z = -1f;
						linkedListNode.Value.m_Root.transform.localPosition = vector2;
						linkedListNode = linkedListNode.Next;
					}
				}
			}
		}
		while (linkedListNode != null)
		{
			linkedListNode.Value.m_Root.gameObject.SetActiveRecursively(false);
			linkedListNode = linkedListNode.Next;
		}
	}
}
