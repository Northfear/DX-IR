using UnityEngine;

public class DescriptionPanel : MonoBehaviour
{
	public delegate void OptionChosen();

	public static DescriptionPanel m_This;

	public UIPanel m_Panel;

	public SpriteText m_NameText;

	public SpriteText m_DescText;

	private OptionChosen m_Callback;

	private void Awake()
	{
		m_This = this;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
	}

	public static void OpenDescriptionPanel(string name, string desc, OptionChosen callback)
	{
		if (!(m_This == null))
		{
			SoundManager.TriggerEvent("Play_UI_Window", m_This.gameObject);
			SoundManager.TriggerEvent("Play_UI_Select", m_This.gameObject);
			m_This.m_NameText.Text = name;
			m_This.m_DescText.Text = desc;
			Globals.m_HUDRoot.m_PanelManager.BringIn("DescriptionPanel", UIPanelManager.MENU_DIRECTION.Forwards);
			m_This.m_Callback = callback;
			PauseTabs.SetMenu(8, true);
		}
	}

	public void ExitPressed()
	{
		Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
		SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
		if (m_Callback != null)
		{
			m_Callback();
		}
	}
}
