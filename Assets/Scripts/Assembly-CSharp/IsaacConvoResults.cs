using UnityEngine;

public class IsaacConvoResults : MonoBehaviour
{
	public InteractiveObject_HackingTerminal m_LinkedHackingTerminalForEmail;

	private void LearnPassword()
	{
		if ((bool)m_LinkedHackingTerminalForEmail)
		{
			m_LinkedHackingTerminalForEmail.LearnedPassword();
		}
		Globals.m_SecondaryObjective = null;
	}

	private void ConversationEnded()
	{
		Globals.m_SecondaryObjective = null;
	}
}
