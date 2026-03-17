using UnityEngine;

public class MainPanel : MonoBehaviour
{
	public GameObject m_StartGameButton;

	public SpriteText m_Version;

	private void Start()
	{
		SetVersion();
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
		SoundManager.TriggerEvent("Play_Music_Menu", base.gameObject);
	}

	public void StartPressed()
	{
		OptionsPanel.ResetControls();
		Globals.m_MenuRoot.m_PanelManager.BringIn("OptionsPanel", UIPanelManager.MENU_DIRECTION.Forwards);
	}

	public void SetVersion()
	{
		if ((bool)m_Version)
		{
			m_Version.Text = "v " + Globals.m_This.m_VersionMajor + "." + Globals.m_This.m_VersionMinor + "." + Globals.m_This.m_VersionMicro;
		}
	}
}
