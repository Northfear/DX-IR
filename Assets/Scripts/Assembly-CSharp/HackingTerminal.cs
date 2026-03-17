using UnityEngine;

public class HackingTerminal : MonoBehaviour
{
	public delegate void CallbackDelegate();

	public UIPanel m_TerminalPanel;

	public UIPanel m_TouchPadPanel;

	public UIPanel m_LockoutPanel;

	public Camera m_terminalCamera;

	public SpriteText m_terminalText;

	public SpriteText m_PasscodeText;

	public SpriteText m_NoPasscodeText;

	public SpriteText m_HackText;

	public SpriteText m_SecondsRemainingText;

	protected string m_currentPasscode = string.Empty;

	protected HackingTerminal_Info m_info;

	protected bool m_PasscodeKnown;

	protected InteractiveObject_HackingTerminal m_HackingTerminal;

	protected CallbackDelegate m_correctPasscodeCallback;

	protected CallbackDelegate m_incorrectPasscodeCallback;

	protected CallbackDelegate m_hackingButtonPressedCallback;

	protected CallbackDelegate m_closeButtonPressedCallback;

	public void RegisterHackingTerminal(InteractiveObject_HackingTerminal terminal)
	{
		m_HackingTerminal = terminal;
	}

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
		SoundManager.TriggerEvent("Play_Keypad_Open", base.gameObject);
		ShowTerminal();
	}

	public void ShowTerminal()
	{
		m_TerminalPanel.BringIn();
		if (!m_PasscodeKnown)
		{
			m_PasscodeText.Hide(true);
			m_NoPasscodeText.Hide(false);
			m_HackText.Hide(false);
		}
		else
		{
			m_PasscodeText.Text = m_info.m_correctPasscode;
			m_PasscodeText.Hide(false);
			m_NoPasscodeText.Hide(true);
			m_HackText.Hide(false);
		}
		if (m_HackingTerminal.GetAttemptsLeft() > 0)
		{
			EnableLockout(false);
		}
		else
		{
			EnableLockout(true);
		}
	}

	public void EnableLockout(bool tf)
	{
		m_HackText.Hide(tf);
		if (tf)
		{
			m_TouchPadPanel.Dismiss();
			m_LockoutPanel.BringIn();
		}
		else
		{
			m_TouchPadPanel.BringIn();
			m_LockoutPanel.Dismiss();
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
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
	}

	public void BackspaceButtonPressed()
	{
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
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
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
	}

	public void CloseTerminal(bool closedFromTerminal)
	{
		if (closedFromTerminal)
		{
			m_TerminalPanel.AddTempTransitionDelegate(FinishedClosingTerminal);
			m_TerminalPanel.Dismiss();
			SoundManager.TriggerEvent("Play_Keypad_Close", base.gameObject);
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
		if (m_HackingTerminal.GetAttemptsLeft() > 0)
		{
			m_TerminalPanel.Dismiss();
			if (m_hackingButtonPressedCallback != null)
			{
				m_hackingButtonPressedCallback();
			}
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
	}

	public virtual void Update()
	{
		if ((bool)m_HackingTerminal && m_HackingTerminal.GetAttemptsLeft() <= 0)
		{
			m_SecondsRemainingText.Text = string.Format("{0:0.0}", m_HackingTerminal.GetAlarmTimeLeft()) + " SECONDS REMAINING";
		}
	}
}
