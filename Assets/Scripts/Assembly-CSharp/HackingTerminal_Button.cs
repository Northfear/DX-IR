using UnityEngine;

public class HackingTerminal_Button : MonoBehaviour
{
	public ButtonType m_buttonType;

	public string m_buttonKey = string.Empty;

	public void ButtonPressed()
	{
		HackingTerminal hackingTerminal = null;
		if (HackingTerminal_NumberPad.m_this != null)
		{
			hackingTerminal = HackingTerminal_NumberPad.m_this;
		}
		if (hackingTerminal != null)
		{
			if (m_buttonType == ButtonType.Key)
			{
				hackingTerminal.PressButton(m_buttonKey);
			}
			else if (m_buttonType == ButtonType.Backspace)
			{
				hackingTerminal.BackspaceButtonPressed();
			}
			else if (m_buttonType == ButtonType.Close)
			{
				hackingTerminal.CloseButtonPressed();
			}
			else if (m_buttonType == ButtonType.Hack)
			{
				hackingTerminal.HackingButtonPressed();
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
