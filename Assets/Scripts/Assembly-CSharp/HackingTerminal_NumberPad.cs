using UnityEngine;

public class HackingTerminal_NumberPad : HackingTerminal
{
	public static HackingTerminal_NumberPad m_this;

	public int m_passcodeMaxLength = 4;

	private void Awake()
	{
		if (m_this == null)
		{
			m_this = this;
		}
	}

	private void Update()
	{
		if (!UIManager.instance.blockInput && Input.GetKeyDown(KeyCode.Escape))
		{
			CloseButtonPressed();
		}
		
		if (KeyboardInput.m_KeyboardEnabled)
		{
			if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
			{
				PressButton("0");
			}
			if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
			{
				PressButton("1");
			}
			if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
			{
				PressButton("2");
			}
			if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
			{
				PressButton("3");
			}
			if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
			{
				PressButton("4");
			}
			if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
			{
				PressButton("5");
			}
			if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
			{
				PressButton("6");
			}
			if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
			{
				PressButton("7");
			}
			if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
			{
				PressButton("8");
			}
			if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
			{
				PressButton("9");
			}
			if (Input.GetKeyDown(KeyCode.Backspace))
			{
				BackspaceButtonPressed();
			}
		}
	}

	public override void PressButton(string buttonKey)
	{
		if (m_currentPasscode.Length != m_passcodeMaxLength)
		{
			base.PressButton(buttonKey);
			if (m_currentPasscode == m_info.m_correctPasscode && m_correctPasscodeCallback != null)
			{
				m_correctPasscodeCallback();
			}
		}
	}
}
