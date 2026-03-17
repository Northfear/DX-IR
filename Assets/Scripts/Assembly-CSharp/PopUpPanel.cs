using UnityEngine;

public class PopUpPanel : MonoBehaviour
{
	public delegate void OptionChosen(bool choice);

	public static PopUpPanel m_This;

	public UIPanel m_Panel;

	public BTButton m_Window;

	public SpriteText m_Message;

	public UIButton m_Yes;

	public UIButton m_No;

	public UIButton m_OK;

	public UIButton m_Cancel;

	private OptionChosen m_Callback;

	private void Awake()
	{
		m_This = this;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
	}

	public static void OpenPopUp(string message, MessageButtons buttons, OptionChosen callback = null)
	{
		if (!(m_This == null))
		{
			SoundManager.TriggerEvent("Play_UI_Select", Globals.m_PlayerController.gameObject);
			SoundManager.TriggerEvent("Play_UI_Window", Globals.m_PlayerController.gameObject);
			Globals.m_HUDRoot.m_PanelManager.BringIn("PopUpPanel", UIPanelManager.MENU_DIRECTION.Forwards);
			if ((bool)m_This.m_Window)
			{
				AnimateScale.Do(m_This.m_Window.gameObject, EZAnimation.ANIM_MODE.FromTo, new Vector3(0.7f, 0.7f, 0.7f), new Vector3(1f, 1f, 1f), EZAnimation.spring, 0.4f, 0f, null, null);
			}
			if ((bool)m_This.m_Message)
			{
				m_This.m_Message.Text = message;
			}
			if ((bool)m_This.m_Yes)
			{
				m_This.m_Yes.gameObject.SetActiveRecursively(buttons == MessageButtons.YesNo);
			}
			if ((bool)m_This.m_No)
			{
				m_This.m_No.gameObject.SetActiveRecursively(buttons == MessageButtons.YesNo);
			}
			if ((bool)m_This.m_OK)
			{
				m_This.m_OK.gameObject.SetActiveRecursively(buttons == MessageButtons.Ok);
			}
			if ((bool)m_This.m_Cancel)
			{
				m_This.m_Cancel.gameObject.SetActiveRecursively(buttons == MessageButtons.Cancel);
			}
			m_This.m_Callback = callback;
		}
	}

	public void YesOKPressed()
	{
		SoundManager.TriggerEvent("Play_UI_Positive", base.gameObject);
		SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
		Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
		if (m_Callback != null)
		{
			m_Callback(true);
		}
	}

	public void NoCancelPressed()
	{
		SoundManager.TriggerEvent("Play_UI_Negative", base.gameObject);
		SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
		Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
		if (m_Callback != null)
		{
			m_Callback(false);
		}
	}
}
