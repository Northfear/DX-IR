using Fabric;
using UnityEngine;

public class DescriptionPanel : MonoBehaviour
{
	public static DescriptionPanel m_This;

	public UIPanel m_Panel;

	public SpriteText m_NameText;

	public SpriteText m_DescText;

	private void Awake()
	{
		m_This = this;
	}

	public static void OpenDescriptionPanel(string name, string desc)
	{
		if (!(m_This == null))
		{
			EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null);
			EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null);
			m_This.m_NameText.Text = name;
			m_This.m_DescText.Text = desc;
			Globals.m_HUDRoot.m_PanelManager.BringIn("DescriptionPanel", UIPanelManager.MENU_DIRECTION.Forwards);
		}
	}

	public void ExitPressed()
	{
		Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
		Globals.m_HUDRoot.m_PanelManager.BringIn("AugmentationMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		AugMenu.AugsOpening(false);
		EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null, base.gameObject);
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}
}
