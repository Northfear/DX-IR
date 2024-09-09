using Fabric;
using UnityEngine;

public class MainPanel : MonoBehaviour
{
	public GameObject m_StartGameButton;

	public SpriteText m_Version;

	private void Start()
	{
		SetVersion("v 0.0.7");
		GameManager.m_LastLevelCombatRifleCurrentAmmo = -1;
		GameManager.m_LastLevelCombatRifleAmmo = -1;
		GameManager.m_LastLevelCrossbowCurrentAmmo = -1;
		GameManager.m_LastLevelCrossbowAmmo = -1;
		GameManager.m_LastLevelShotgunCurrentAmmo = -1;
		GameManager.m_LastLevelShotgunAmmo = -1;
		EventManager.Instance.PostEvent("Music_Menu", EventAction.SetSwitch, "MainMenu");
		EventManager.Instance.PostEvent("Music_Menu", EventAction.PlaySound, null);
	}

	public void StartPressed()
	{
		OptionsPanel.ResetControls();
		Globals.m_MenuRoot.m_PanelManager.BringIn("OptionsPanel", UIPanelManager.MENU_DIRECTION.Forwards);
	}

	public void SetVersion(string version)
	{
		if ((bool)m_Version)
		{
			m_Version.Text = version;
		}
	}
}
