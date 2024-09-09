using Fabric;
using UnityEngine;

public class HackingTerminal : MonoBehaviour
{
	public delegate void CallbackDelegate();

	public UIPanel m_TerminalPanel;

	public Camera m_terminalCamera;

	public SpriteText m_terminalText;

	public SpriteText m_PasscodeText;

	public SpriteText m_NoPasscodeText;

	public SpriteText m_HackText;

	protected string m_currentPasscode = string.Empty;

	protected HackingTerminal_Info m_info;

	protected bool m_PasscodeKnown;

	protected CallbackDelegate m_correctPasscodeCallback;

	protected CallbackDelegate m_incorrectPasscodeCallback;

	protected CallbackDelegate m_hackingButtonPressedCallback;

	protected CallbackDelegate m_closeButtonPressedCallback;

	public void RegisterHackingTerminalInfo(HackingTerminal_Info info, bool passcodeKnown)
	{
		m_info = info;
		m_PasscodeKnown = passcodeKnown;
	}

	public void SetCorrectPasscodeCallback(CallbackDelegate del)
	{
		m_correctPasscodeCallback = del;
	}

	public void SetIncorrectPasscodeCallback(CallbackDelegate del)
	{
		m_incorrectPasscodeCallback = del;
	}

	public void SetHackingButtonPressedCallback(CallbackDelegate del)
	{
		m_hackingButtonPressedCallback = del;
	}

	public void SetCloseButtonPressedCallback(CallbackDelegate del)
	{
		m_closeButtonPressedCallback = del;
	}

	public void BringInTerminal()
	{
		if (UIManager.instance.GetCameraID(m_terminalCamera) == -1)
		{
			UIManager.instance.AddCamera(m_terminalCamera, 4096, 100f, 0);
		}
		EventManager.Instance.PostEvent("Keypad_Open", EventAction.PlaySound, null, base.gameObject);
		ShowTerminal();
	}

	public void ShowTerminal()
	{
		m_TerminalPanel.BringIn();
		if (!m_PasscodeKnown)
		{
			m_PasscodeText.Hide(true);
			m_NoPasscodeText.Hide(false);
		}
		else
		{
			m_PasscodeText.Text = m_info.m_correctPasscode;
			m_PasscodeText.Hide(false);
			m_NoPasscodeText.Hide(true);
		}
	}

	public void HideTerminal()
	{
		m_PasscodeText.Hide(true);
		m_NoPasscodeText.Hide(true);
		m_HackText.Hide(true);
		m_TerminalPanel.Dismiss();
	}

	public virtual void PressButton(string buttonKey)
	{
		m_currentPasscode += buttonKey;
		m_terminalText.Text = m_currentPasscode;
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}

	public void BackspaceButtonPressed()
	{
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
		if (m_currentPasscode.Length != 0)
		{
			m_currentPasscode = m_currentPasscode.Remove(m_currentPasscode.Length - 1);
			m_terminalText.Text = m_currentPasscode;
		}
	}

	public void CloseButtonPressed()
	{
		CloseTerminal(true);
		if (m_closeButtonPressedCallback != null)
		{
			m_closeButtonPressedCallback();
		}
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}

	public void CloseTerminal(bool closedFromTerminal)
	{
		if (closedFromTerminal)
		{
			m_TerminalPanel.AddTempTransitionDelegate(FinishedClosingTerminal);
			m_TerminalPanel.Dismiss();
			EventManager.Instance.PostEvent("Keypad_Close", EventAction.PlaySound, null);
		}
		else
		{
			FinishedClosingTerminal(m_TerminalPanel, null);
		}
	}

	public void FinishedClosingTerminal(UIPanelBase panel, EZTransition transition)
	{
		if (panel == m_TerminalPanel)
		{
			UIManager.instance.RemoveCamera(m_terminalCamera);
			Object.Destroy(base.gameObject);
		}
	}

	public void EnterButtonPressed()
	{
		if (m_currentPasscode == m_info.m_correctPasscode)
		{
			if (m_correctPasscodeCallback != null)
			{
				m_correctPasscodeCallback();
			}
		}
		else if (m_incorrectPasscodeCallback != null)
		{
			m_incorrectPasscodeCallback();
		}
	}

	public void HackingButtonPressed()
	{
		m_TerminalPanel.Dismiss();
		if (m_hackingButtonPressedCallback != null)
		{
			m_hackingButtonPressedCallback();
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
