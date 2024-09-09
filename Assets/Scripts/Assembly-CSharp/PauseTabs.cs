using Fabric;
using UnityEngine;

public class PauseTabs : MonoBehaviour
{
	public enum PauseWindow
	{
		None = -1,
		Mission = 0,
		Inventory = 1,
		Augmentations = 2,
		Map = 3,
		Media = 4,
		Premium = 5,
		Pause = 6,
		Total = 7
	}

	public static PauseTabs m_This;

	private float m_TimeThisFrame;

	private float m_deltaTime;

	public UIPanel m_Panel;

	public GameObject m_InputBlocker;

	public UIPanelTab[] m_PanelTabs = new UIPanelTab[7];

	private PauseWindow m_TargetMenu = PauseWindow.Pause;

	public PackedSprite m_MenuTitleDoodad;

	public SpriteText m_MenuTitle;

	private bool m_TitleJumbling;

	private int m_JumbleLetterIndex;

	private string m_TargetTitle = string.Empty;

	private float[] m_LetterTimers;

	private float m_WordTimer = -1f;

	public PackedSprite[] m_Doodads;

	private float m_AudioUnpauseEventDelay = -1f;

	public SpriteText m_CreditsValue;

	public SpriteText m_PraxisValue;

	public SpriteText m_ExperienceValue;

	public static float GetDeltaTime()
	{
		return (!(m_This == null)) ? m_This.m_deltaTime : 0f;
	}

	private void Awake()
	{
		m_This = this;
		if ((bool)m_Panel)
		{
			m_Panel.DismissImmediate();
		}
	}

	private void Start()
	{
		base.transform.localPosition = new Vector3(0f, 0f, 5f);
		Globals.m_HUDRoot.m_PanelManager.transform.localPosition = new Vector3(0f, 0f, 10f);
		if (m_PanelTabs == null)
		{
			return;
		}
		for (int i = 0; i < 7; i++)
		{
			if (m_PanelTabs[i] != null)
			{
				m_PanelTabs[i].AddValueChangedDelegate(PanelTabPressed);
			}
		}
	}

	private void Update()
	{
		float timeThisFrame = m_TimeThisFrame;
		m_TimeThisFrame = Time.realtimeSinceStartup;
		m_deltaTime = m_TimeThisFrame - timeThisFrame;
		if (m_AudioUnpauseEventDelay >= 0f)
		{
			m_AudioUnpauseEventDelay -= m_deltaTime;
			if (m_AudioUnpauseEventDelay < 0f)
			{
				EventManager.Instance.PostEvent("Pause", EventAction.UnpauseSound, null, base.gameObject);
			}
		}
		if (!m_TitleJumbling)
		{
			return;
		}
		if (m_WordTimer >= 0f)
		{
			m_WordTimer -= m_deltaTime;
			if (m_WordTimer <= 0f)
			{
				m_JumbleLetterIndex++;
				m_MenuTitle.Text += (char)Random.Range(33, 93);
				m_LetterTimers[m_JumbleLetterIndex] = Random.Range(0.07f, 0.18f);
				m_WordTimer = -1f;
				if (m_JumbleLetterIndex < m_TargetTitle.Length - 1)
				{
					m_WordTimer = Random.Range(0.03f, 0.06f);
				}
			}
		}
		char[] array = m_MenuTitle.Text.ToCharArray();
		for (int i = 0; i < m_LetterTimers.Length && i <= m_JumbleLetterIndex; i++)
		{
			if (m_LetterTimers[i] >= 0f)
			{
				array[i] = (char)Random.Range(33, 93);
				m_LetterTimers[i] -= m_deltaTime;
				if (m_LetterTimers[i] <= 0f)
				{
					array[i] = m_TargetTitle[i];
					m_LetterTimers[i] = -1f;
				}
			}
		}
		m_MenuTitle.Text = new string(array);
		if (string.Compare(m_MenuTitle.Text, m_TargetTitle, false) == 0)
		{
			m_TitleJumbling = false;
		}
	}

	public static void OpenTabs()
	{
		if (m_This == null || m_This.m_Panel == null)
		{
			return;
		}
		Globals.m_HUD.Display(false, true);
		m_This.m_Panel.BringIn();
		m_This.m_InputBlocker.active = true;
		if (m_This.m_TargetMenu == PauseWindow.None)
		{
			m_This.m_TargetMenu = PauseWindow.Pause;
		}
		if (m_This.m_TargetMenu != PauseWindow.Premium && m_This.m_TargetMenu != PauseWindow.Augmentations)
		{
			m_This.m_TargetMenu = PauseWindow.Pause;
		}
		for (int i = 0; i < 7; i++)
		{
			if (m_This.m_PanelTabs[i] != null)
			{
				m_This.m_PanelTabs[i].Value = m_This.m_TargetMenu == (PauseWindow)i;
				m_This.m_PanelTabs[i].SetState((m_This.m_TargetMenu != (PauseWindow)i) ? 1 : 0);
			}
		}
		if (m_This.m_Doodads != null)
		{
			for (int j = 0; j < m_This.m_Doodads.Length; j++)
			{
				if (m_This.m_Doodads[j] != null)
				{
					m_This.m_Doodads[j].PlayAnim(0, Random.Range(0, 20));
				}
			}
		}
		if (m_This.m_TargetMenu == PauseWindow.Augmentations)
		{
			EventManager.Instance.PostEvent("UI_Pulse", EventAction.PlaySound, null, m_This.gameObject);
		}
		if (m_This.m_TargetMenu == PauseWindow.Mission)
		{
			Globals.m_HUDRoot.m_PanelManager.BringIn("MissionMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		}
		else if (m_This.m_TargetMenu == PauseWindow.Inventory)
		{
			Globals.m_HUDRoot.m_PanelManager.BringIn("InventoryMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		}
		else if (m_This.m_TargetMenu == PauseWindow.Augmentations)
		{
			Globals.m_HUDRoot.m_PanelManager.BringIn("AugmentationMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		}
		else if (m_This.m_TargetMenu == PauseWindow.Map)
		{
			Globals.m_HUDRoot.m_PanelManager.BringIn("MapMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		}
		else if (m_This.m_TargetMenu == PauseWindow.Media)
		{
			Globals.m_HUDRoot.m_PanelManager.BringIn("MediaMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		}
		else if (m_This.m_TargetMenu == PauseWindow.Premium)
		{
			Globals.m_HUDRoot.m_PanelManager.BringIn("PremiumMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		}
		else
		{
			Globals.m_HUDRoot.m_PanelManager.BringIn("PauseMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		}
		m_This.SetMenuTitle();
		UpdateCreditsValue();
		UpdatePraxisValue();
		UpdateExperienceValue();
	}

	private void SetMenuTitle()
	{
		if (m_MenuTitleDoodad != null)
		{
			m_MenuTitleDoodad.PlayAnim(0);
		}
		if (m_TargetMenu == PauseWindow.Mission)
		{
			m_TargetTitle = "Mission Logs";
		}
		else if (m_TargetMenu == PauseWindow.Inventory)
		{
			m_TargetTitle = "Inventory";
		}
		else if (m_TargetMenu == PauseWindow.Augmentations)
		{
			m_TargetTitle = "Augmentations";
		}
		else if (m_TargetMenu == PauseWindow.Map)
		{
			m_TargetTitle = "Map";
		}
		else if (m_TargetMenu == PauseWindow.Media)
		{
			m_TargetTitle = "Media Logs";
		}
		else if (m_TargetMenu == PauseWindow.Premium)
		{
			m_TargetTitle = "Market";
		}
		else
		{
			m_TargetTitle = "Pause Menu";
		}
		m_TitleJumbling = true;
		m_MenuTitle.Text = string.Empty;
		m_JumbleLetterIndex = -1;
		m_WordTimer = 0f;
		m_LetterTimers = new float[m_TargetTitle.Length];
		for (int i = 0; i < m_LetterTimers.Length; i++)
		{
			m_LetterTimers[i] = -1f;
		}
		if (m_TargetMenu == PauseWindow.Premium)
		{
			StoreMenu.StoreOpening(StoreMenu.StorePanel.Items);
		}
		else if (m_TargetMenu == PauseWindow.Augmentations)
		{
			AugMenu.AugsOpening(true);
		}
	}

	public void ExitTapped()
	{
		m_InputBlocker.active = false;
		m_This.m_Panel.Dismiss();
		Globals.m_HUDRoot.m_PanelManager.Dismiss(UIPanelManager.MENU_DIRECTION.Forwards);
		Time.timeScale = 1f;
		EventManager.Instance.PostEvent("UI_Swish", EventAction.PlaySound, null, base.gameObject);
		EventManager.Instance.PostEvent("DynamicMixer", EventAction.RemovePreset, "Pause");
		m_AudioUnpauseEventDelay = 0f;
		GameManager.GamePaused(false);
		CommLinkDialog.Paused(false);
		Globals.m_HUD.Display(true, true);
	}

	private void PanelTabPressed(IUIObject obj)
	{
		PauseWindow pauseWindow = PauseWindow.None;
		for (int i = 0; i < 7; i++)
		{
			if (m_PanelTabs[i].Value)
			{
				pauseWindow = (PauseWindow)i;
				break;
			}
		}
		if (pauseWindow != m_TargetMenu)
		{
			m_TargetMenu = pauseWindow;
			SetMenuTitle();
		}
	}

	public static void HideTabsDuringCustomization(bool Hide)
	{
		if (m_This == null || m_This.m_Panel == null)
		{
			return;
		}
		if (Hide)
		{
			m_This.m_Panel.Dismiss();
			m_This.m_InputBlocker.active = false;
			return;
		}
		m_This.m_Panel.BringIn();
		m_This.m_InputBlocker.active = true;
		m_This.m_TargetMenu = PauseWindow.Pause;
		Globals.m_HUDRoot.m_PanelManager.BringIn("PauseMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		for (int i = 0; i < 7; i++)
		{
			if (m_This.m_PanelTabs[i] != null)
			{
				m_This.m_PanelTabs[i].Value = m_This.m_TargetMenu == (PauseWindow)i;
			}
		}
		if (m_This.m_Doodads != null)
		{
			for (int j = 0; j < m_This.m_Doodads.Length; j++)
			{
				if (m_This.m_Doodads[j] != null)
				{
					m_This.m_Doodads[j].PlayAnim(0, Random.Range(0, 20));
				}
			}
		}
		m_This.SetMenuTitle();
	}

	public static void UpdateCreditsValue()
	{
		if (!(m_This == null) && !(m_This.m_CreditsValue == null) && !(Globals.m_Inventory == null))
		{
			m_This.m_CreditsValue.Text = Globals.m_Inventory.m_Credits.ToString();
		}
	}

	public static void UpdatePraxisValue()
	{
		if (!(m_This == null) && !(m_This.m_PraxisValue == null) && !(Globals.m_Inventory == null))
		{
			m_This.m_PraxisValue.Text = (Globals.m_Inventory.m_PraxisNeededToLevel - Globals.m_Inventory.m_PraxisExp).ToString();
		}
	}

	public static void UpdateExperienceValue()
	{
		if (!(m_This == null) && !(m_This.m_ExperienceValue == null) && !(Globals.m_Inventory == null))
		{
			m_This.m_ExperienceValue.Text = Globals.m_Inventory.m_TotalExp.ToString();
		}
	}
}
