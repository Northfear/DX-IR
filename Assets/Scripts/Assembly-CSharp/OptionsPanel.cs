using Fabric;
using UnityEngine;

public class OptionsPanel : MonoBehaviour
{
	public enum ToggleState
	{
		On = 0,
		Off = 1
	}

	private static OptionsPanel m_This;

	public GameObject m_BloomRoot;

	public UIStateToggleBtn m_ToggleBloom;

	public UIStateToggleBtn m_ToggleAutoRotate;

	public UIStateToggleBtn m_ToggleDisableTap;

	private void Awake()
	{
		m_This = this;
	}

	private void Start()
	{
		if (Globals.m_EffectsRank == Globals.EffectsRank.NotUsable)
		{
			Globals.m_Bloom = false;
			if (m_This.m_BloomRoot != null)
			{
				Object.Destroy(m_This.m_BloomRoot);
			}
		}
	}

	public static void ResetControls()
	{
		if (!(m_This == null))
		{
			if (m_This.m_ToggleBloom != null)
			{
				m_This.m_ToggleBloom.SetToggleState((!Globals.m_Bloom) ? 1 : 0);
			}
			if (m_This.m_ToggleAutoRotate != null)
			{
				m_This.m_ToggleAutoRotate.SetToggleState((!Globals.m_AutoRotate) ? 1 : 0);
			}
			if (m_This.m_ToggleDisableTap != null)
			{
				m_This.m_ToggleDisableTap.SetToggleState((!Globals.m_DisableTapToMove) ? 1 : 0);
			}
		}
	}

	public void BackPressed()
	{
		Globals.m_MenuRoot.m_PanelManager.BringIn("MainPanel", UIPanelManager.MENU_DIRECTION.Forwards);
	}

	public void PlayPressed()
	{
		Globals.m_MenuRoot.m_PanelManager.Dismiss(UIPanelManager.MENU_DIRECTION.Backwards);
		LevelSelectPanel.OpenPanel();
	}

	public void BloomPressed()
	{
		EventManager.Instance.PostEvent("UI_Toggle", EventAction.PlaySound, null, base.gameObject);
		Globals.m_Bloom = !Globals.m_Bloom;
		if (m_ToggleBloom != null)
		{
			m_ToggleBloom.SetToggleState((!Globals.m_Bloom) ? 1 : 0);
		}
	}

	public void AutoRotatePressed()
	{
		EventManager.Instance.PostEvent("UI_Toggle", EventAction.PlaySound, null, base.gameObject);
		Globals.m_AutoRotate = !Globals.m_AutoRotate;
		if (m_ToggleAutoRotate != null)
		{
			m_ToggleAutoRotate.SetToggleState((!Globals.m_AutoRotate) ? 1 : 0);
		}
	}

	public void DisableTapPressed()
	{
		EventManager.Instance.PostEvent("UI_Toggle", EventAction.PlaySound, null, base.gameObject);
		Globals.m_DisableTapToMove = !Globals.m_DisableTapToMove;
		if (m_ToggleDisableTap != null)
		{
			m_ToggleDisableTap.SetToggleState((!Globals.m_DisableTapToMove) ? 1 : 0);
		}
	}
}
