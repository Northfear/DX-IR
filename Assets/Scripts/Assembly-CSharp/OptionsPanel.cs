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

	public UIStateToggleBtn m_ToggleAutosave;

	public UIStateToggleBtn m_ToggleGodMode;

	public UIStateToggleBtn m_ToggleAutoRotate;

	public UIStateToggleBtn m_ToggleDisableTap;

	private void Awake()
	{
		m_This = this;
	}

	private void Start()
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
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
			if (m_This.m_ToggleAutosave != null)
			{
				m_This.m_ToggleAutosave.SetToggleState((!Globals.m_Autosave) ? 1 : 0);
			}
			if (m_This.m_ToggleGodMode != null)
			{
				m_This.m_ToggleGodMode.SetToggleState((!Globals.m_GodMode) ? 1 : 0);
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

	public void LoadPressed()
	{
		GameManager.GameLoaded();
	}

	public void BloomPressed()
	{
		SoundManager.TriggerEvent("Play_UI_Toggle", base.gameObject);
		Globals.m_Bloom = !Globals.m_Bloom;
		if (m_ToggleBloom != null)
		{
			m_ToggleBloom.SetToggleState((!Globals.m_Bloom) ? 1 : 0);
		}
	}

	public void AutosavePressed()
	{
		SoundManager.TriggerEvent("Play_UI_Toggle", base.gameObject);
		Globals.m_Autosave = !Globals.m_Autosave;
		if (m_ToggleAutosave != null)
		{
			m_ToggleAutosave.SetToggleState((!Globals.m_Autosave) ? 1 : 0);
		}
	}

	public void GodModePressed()
	{
		SoundManager.TriggerEvent("Play_UI_Toggle", base.gameObject);
		Globals.m_GodMode = !Globals.m_GodMode;
		if (m_ToggleGodMode != null)
		{
			m_ToggleGodMode.SetToggleState((!Globals.m_GodMode) ? 1 : 0);
		}
	}

	public void AutoRotatePressed()
	{
		SoundManager.TriggerEvent("Play_UI_Toggle", base.gameObject);
		Globals.m_AutoRotate = !Globals.m_AutoRotate;
		if (m_ToggleAutoRotate != null)
		{
			m_ToggleAutoRotate.SetToggleState((!Globals.m_AutoRotate) ? 1 : 0);
		}
	}

	public void DisableTapPressed()
	{
		SoundManager.TriggerEvent("Play_UI_Toggle", base.gameObject);
		Globals.m_DisableTapToMove = !Globals.m_DisableTapToMove;
		if (m_ToggleDisableTap != null)
		{
			m_ToggleDisableTap.SetToggleState((!Globals.m_DisableTapToMove) ? 1 : 0);
		}
	}
}
