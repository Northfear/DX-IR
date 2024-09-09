using Fabric;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public SpriteText m_GodModeText;

	public Color m_GodModeColor = new Color(1f, 1f, 0f, 1f);

	private void OnEnable()
	{
		if (m_GodModeText != null)
		{
			m_GodModeText.SetColor((!Globals.m_PlayerController.m_GodMode) ? Color.white : m_GodModeColor);
		}
	}

	public void FPSDisplayTapped()
	{
		Globals.m_PlayerController.m_Camera.GetComponent<HUDFPS>().enabled = !Globals.m_PlayerController.m_Camera.GetComponent<HUDFPS>().enabled;
		Globals.m_ShowFPS = !Globals.m_ShowFPS;
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}

	public void GodModeTapped()
	{
		Globals.m_PlayerController.m_GodMode = !Globals.m_PlayerController.m_GodMode;
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
		if (m_GodModeText != null)
		{
			m_GodModeText.SetColor((!Globals.m_PlayerController.m_GodMode) ? Color.white : m_GodModeColor);
		}
	}

	public void LoadTapped()
	{
		GameManager.ReloadCurrentLevel();
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
		EventManager.Instance.PostEvent("DynamicMixer", EventAction.RemovePreset, "Pause");
	}

	public void CustomizeHUDTapped()
	{
		PauseTabs.HideTabsDuringCustomization(true);
		Globals.m_HUD.StartCustomization();
		Globals.m_HUDRoot.m_PanelManager.BringIn("HUDCustomizationMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}

	public void QuitTapped()
	{
		Time.timeScale = 1f;
		GameManager.LoadMenu();
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
		EventManager.Instance.PostEvent("DynamicMixer", EventAction.RemovePreset, "Pause");
		EventManager.Instance.PostEvent("Pause", EventAction.UnpauseSound, null, base.gameObject);
		FabricManager.Instance.Stop(0f);
	}
}
