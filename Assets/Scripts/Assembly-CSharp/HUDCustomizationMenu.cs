using UnityEngine;

public class HUDCustomizationMenu : MonoBehaviour
{
	private void Awake()
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
	}

	public void RestoreCustomDefaults()
	{
		Globals.m_HUD.RestoreCustomDefaults();
		SoundManager.TriggerEvent("Play_UI_Select", Globals.m_PlayerController.gameObject);
	}

	public void QuitCustomizationAndSave()
	{
		Globals.m_HUD.QuitCustomizationAndSave();
		PauseTabs.HideTabsDuringCustomization(false);
		SoundManager.TriggerEvent("Play_UI_Select", Globals.m_PlayerController.gameObject);
	}

	public void CancelCustomization()
	{
		Globals.m_HUD.CancelCustomization();
		PauseTabs.HideTabsDuringCustomization(false);
		SoundManager.TriggerEvent("Play_UI_Select", Globals.m_PlayerController.gameObject);
	}
}
