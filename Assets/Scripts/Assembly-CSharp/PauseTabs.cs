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
		Pause = 5,
		Total = 6,
		Info = 7,
		AugDesc = 8
	}

	public static PauseTabs m_This;

	private float m_TimeThisFrame;

	private float m_deltaTime;

	public UIPanel m_Panel;

	private bool m_Open;

	public GameObject m_InputBlocker;

	public UIPanelTab[] m_PanelTabs = new UIPanelTab[6];

	private PauseWindow m_TargetMenu = PauseWindow.Pause;

	public SpriteText m_MenuTitle;

	private bool m_TitleJumbling;

	private int m_JumbleLetterIndex;

	private string m_TargetTitle = string.Empty;

	private float[] m_LetterTimers;

	private float m_WordTimer = -1f;

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
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
		localPosition = Globals.m_HUDRoot.m_PanelManager.transform.localPosition;
		localPosition.x = 0f;
		Globals.m_HUDRoot.m_PanelManager.transform.localPosition = localPosition;
		if (m_PanelTabs == null)
		{
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			if (m_PanelTabs[i] != null)
			{
				m_PanelTabs[i].SetValueChangedDelegate(PanelTabPressed);
			}
		}
	}

	private void Update()
	{
		if (KeyboardInput.m_KeyboardEnabled)
		{
			if ((Input.GetKeyDown(KeyCode.Escape) || KeyboardInput.GetKeyDown(KeyboardInput.KeyName.Pause)) && !UIManager.instance.blockInput)
			{
				ExitTapped();
			}
		}

		float timeThisFrame = m_TimeThisFrame;
		m_TimeThisFrame = Time.realtimeSinceStartup;
		m_deltaTime = m_TimeThisFrame - timeThisFrame;

		if (m_AudioUnpauseEventDelay >= 0f)
		{
			m_AudioUnpauseEventDelay -= m_deltaTime;
			if (m_AudioUnpauseEventDelay < 0f)
			{
				SoundManager.TriggerEvent("Unpause", base.gameObject);
			}
		}

		if (m_TitleJumbling)
		{
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
		if (m_Open)
		{
			UpdateCreditsValue();
			UpdatePraxisValue();
			UpdateExperienceValue();
		}
	}

	public static void OpenJustTabs()
	{
		if (!(m_This == null) && !(m_This.m_Panel == null))
		{
			m_This.m_Open = true;
			Globals.m_HUD.Display(false, true, false);
			m_This.m_Panel.BringIn();
			m_This.m_InputBlocker.active = true;
		}
	}

	public static void OpenTabs()
	{
		if (m_This == null || m_This.m_Panel == null)
		{
			return;
		}
		m_This.m_Open = true;
		Globals.m_HUD.Display(false, true, false);
		m_This.m_Panel.BringIn();
		m_This.m_InputBlocker.active = true;
		if (m_This.m_TargetMenu == PauseWindow.None || m_This.m_TargetMenu == PauseWindow.AugDesc || m_This.m_TargetMenu == PauseWindow.Info)
		{
			m_This.m_TargetMenu = PauseWindow.Pause;
		}
		for (int i = 0; i < 6; i++)
		{
			if (m_This.m_PanelTabs[i] != null)
			{
				m_This.m_PanelTabs[i].Value = m_This.m_TargetMenu == (PauseWindow)i;
				m_This.m_PanelTabs[i].SetState((m_This.m_TargetMenu != (PauseWindow)i) ? 1 : 0);
			}
		}
		if (m_This.m_TargetMenu == PauseWindow.Augmentations)
		{
			SoundManager.TriggerEvent("Play_UI_Pulse", m_This.gameObject);
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
			Globals.m_HUDRoot.m_PanelManager.BringIn("MediaLogMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		}
		else if (m_This.m_TargetMenu == PauseWindow.Pause)
		{
			Globals.m_HUDRoot.m_PanelManager.BringIn("PauseMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		}
		m_This.SetMenuTitle(null, true);
	}

	public static void OpenTabs(PauseWindow window, object data)
	{
		if (m_This == null || m_This.m_Panel == null)
		{
			return;
		}
		m_This.m_Open = true;
		Globals.m_HUD.Display(false, true, false);
		m_This.m_Panel.BringIn();
		m_This.m_InputBlocker.active = true;
		if (window != PauseWindow.None)
		{
			m_This.m_TargetMenu = window;
		}
		else
		{
			m_This.m_TargetMenu = PauseWindow.Pause;
			data = null;
		}
		for (int i = 0; i < 6; i++)
		{
			if (m_This.m_PanelTabs[i] != null)
			{
				m_This.m_PanelTabs[i].Value = m_This.m_TargetMenu == (PauseWindow)i;
				m_This.m_PanelTabs[i].SetState((m_This.m_TargetMenu != (PauseWindow)i) ? 1 : 0);
			}
		}
		if (m_This.m_TargetMenu == PauseWindow.Augmentations)
		{
			SoundManager.TriggerEvent("Play_UI_Pulse", m_This.gameObject);
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
			Globals.m_HUDRoot.m_PanelManager.BringIn("MediaLogMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		}
		else if (m_This.m_TargetMenu == PauseWindow.Pause)
		{
			Globals.m_HUDRoot.m_PanelManager.BringIn("PauseMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		}
		m_This.SetMenuTitle(data, true);
	}

	private void SetMenuTitle(object data, bool CallOpenFunc = true)
	{
		if (m_TargetMenu == PauseWindow.Mission)
		{
			m_TargetTitle = "MISSION LOGS";
		}
		else if (m_TargetMenu == PauseWindow.Inventory)
		{
			m_TargetTitle = "INVENTORY";
		}
		else if (m_TargetMenu == PauseWindow.Augmentations)
		{
			m_TargetTitle = "AUGMENTATIONS";
		}
		else if (m_TargetMenu == PauseWindow.Map)
		{
			m_TargetTitle = "MAP";
		}
		else if (m_TargetMenu == PauseWindow.Media)
		{
			m_TargetTitle = "MEDIA LOGS";
		}
		else if (m_TargetMenu == PauseWindow.Pause)
		{
			m_TargetTitle = "PAUSE MENU";
		}
		else if (m_TargetMenu == PauseWindow.AugDesc)
		{
			m_TargetTitle = "AUG INFO";
		}
		else if (m_TargetMenu == PauseWindow.Info)
		{
			m_TargetTitle = "ITEM INFO";
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
		if (CallOpenFunc)
		{
			if (m_TargetMenu == PauseWindow.Inventory)
			{
				InventoryPanel.InventoryOpening((data != null) ? ((int)data) : (-1));
			}
			else if (m_TargetMenu == PauseWindow.Augmentations)
			{
				AugMenu.AugsOpening();
			}
			else if (m_TargetMenu == PauseWindow.Media)
			{
				MediaLogMenu.MediaLogOpening();
			}
			else if (m_TargetMenu == PauseWindow.Mission)
			{
				MissionMenu.MissionMenuOpening();
			}
			else if (m_TargetMenu == PauseWindow.Map)
			{
				MapMenu.MapMenuOpening();
			}
		}
	}

	public void ExitTapped()
	{
		Time.timeScale = 1f;
		GameManager.GamePaused(false);
		m_Open = false;
		m_InputBlocker.active = false;
		m_This.m_Panel.Dismiss();
		Globals.m_HUDRoot.m_PanelManager.Dismiss(UIPanelManager.MENU_DIRECTION.Forwards);
		SoundManager.TriggerEvent("Play_UI_Swish", base.gameObject);
		m_AudioUnpauseEventDelay = 0f;
		CommLinkDialog.Paused(false);
		Globals.m_HUD.Display(true, true, false);
	}

	private void PanelTabPressed(IUIObject obj)
	{
		if (!(obj as UIPanelTab).Value)
		{
			return;
		}
		PauseWindow pauseWindow = PauseWindow.None;
		for (int i = 0; i < 6; i++)
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
			SetMenuTitle(null, true);
		}
	}

	public static void SetMenu(int menuID, bool CallOpenFunc = true)
	{
		if (m_This.m_TargetMenu == (PauseWindow)menuID)
		{
			return;
		}
		m_This.m_TargetMenu = (PauseWindow)menuID;
		for (int i = 0; i < 6; i++)
		{
			if (m_This.m_PanelTabs[i] != null)
			{
				m_This.m_PanelTabs[i].Value = m_This.m_TargetMenu == (PauseWindow)i;
				m_This.m_PanelTabs[i].SetState((m_This.m_TargetMenu != (PauseWindow)i) ? 1 : 0);
			}
		}
		m_This.SetMenuTitle(null, CallOpenFunc);
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
		for (int i = 0; i < 6; i++)
		{
			if (m_This.m_PanelTabs[i] != null)
			{
				m_This.m_PanelTabs[i].Value = m_This.m_TargetMenu == (PauseWindow)i;
			}
		}
		m_This.SetMenuTitle(null, true);
	}

	public static void UpdateCreditsValue()
	{
		if (!(m_This == null) && !(m_This.m_CreditsValue == null) && !(Globals.m_Inventory == null))
		{
			m_This.m_CreditsValue.Text = Globals.m_Inventory.GetCredits().ToString();
		}
	}

	public static void UpdatePraxisValue()
	{
		if (!(m_This == null) && !(m_This.m_PraxisValue == null) && !(Globals.m_Inventory == null))
		{
			m_This.m_PraxisValue.Text = (4000 - Globals.m_Inventory.GetPraxisExperience()).ToString();
		}
	}

	public static void UpdateExperienceValue()
	{
		if (!(m_This == null) && !(m_This.m_ExperienceValue == null) && !(Globals.m_Inventory == null))
		{
			m_This.m_ExperienceValue.Text = Globals.m_Inventory.GetTotalExperience().ToString();
		}
	}
}
