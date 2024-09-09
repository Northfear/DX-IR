using Fabric;
using UnityEngine;

public class HUDCustomizationMenu : MonoBehaviour
{
	public void RestoreCustomDefaults()
	{
		Globals.m_HUD.RestoreCustomDefaults();
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}

	public void QuitCustomizationAndSave()
	{
		Globals.m_HUD.QuitCustomizationAndSave();
		PauseTabs.HideTabsDuringCustomization(false);
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}

	public void CancelCustomization()
	{
		Globals.m_HUD.CancelCustomization();
		PauseTabs.HideTabsDuringCustomization(false);
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}
}
