using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public SpriteText m_GodModeText;

	public Color m_GodModeColor = new Color(1f, 1f, 0f, 1f);

	private void Awake()
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
	}

	private void OnEnable()
	{
		if (m_GodModeText != null)
		{
			m_GodModeText.SetColor((!Globals.m_GodMode) ? Color.white : m_GodModeColor);
		}
	}

	public static void PauseOpening()
	{
	}

	public void FPSDisplayTapped()
	{
		Globals.m_PlayerController.m_Camera.GetComponent<HUDFPS>().enabled = !Globals.m_PlayerController.m_Camera.GetComponent<HUDFPS>().enabled;
		Globals.m_ShowFPS = !Globals.m_ShowFPS;
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
	}

	public void GodModeTapped()
	{
		Globals.m_GodMode = !Globals.m_GodMode;
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
		if (m_GodModeText != null)
		{
			m_GodModeText.SetColor((!Globals.m_GodMode) ? Color.white : m_GodModeColor);
		}
	}

	public void UnlockAllTapped()
	{
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
		Inventory.UnlockAll();
	}

	private void LoadResult(bool result)
	{
		if (result)
		{
			GameManager.GameLoaded();
			SoundManager.TriggerEvent("Unpause", base.gameObject);
		}
		Globals.m_HUDRoot.m_PanelManager.BringIn("PauseMenu", UIPanelManager.MENU_DIRECTION.Forwards);
	}

	public void LoadTapped()
	{
		PopUpPanel.OpenPopUp("Load your previous savegame?", MessageButtons.YesNo, LoadResult);
	}

	public void SaveTapped()
	{
		GameManager.GameSaved(false);
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
	}

	public void CustomizeHUDTapped()
	{
		PauseTabs.HideTabsDuringCustomization(true);
		Globals.m_HUD.StartCustomization();
		Globals.m_HUDRoot.m_PanelManager.BringIn("HUDCustomizationMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		SoundManager.TriggerEvent("Play_UI_Select", Globals.m_PlayerController.gameObject);
	}

	public void QuitTapped()
	{
		Time.timeScale = 1f;
		GameManager.LoadMenu();
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
		SoundManager.TriggerEvent("Unpause", base.gameObject);
	}
}
