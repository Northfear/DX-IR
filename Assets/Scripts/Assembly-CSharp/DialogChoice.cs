using System;
using UnityEngine;

[Serializable]
public class DialogChoice
{
	public string m_ChoiceString;

	public string m_ChoiceHighlightString;

	public Conversation_Base m_ConnectingConversation;

	public DialogSequence[] m_DialogSequences;

	public MonoBehaviour m_ConversationTriggerClass;

	public string m_TriggerFunctionName;

	public Conversation_Base m_FailedConnectingConversation;

	public DialogSequence[] m_FailedDialogSequences;

	public MonoBehaviour m_FailedConversationTriggerClass;

	public string m_FailedTriggerFunctionName;

	public int m_PersuasionAmount;

	public int m_RequiredPersuasionLevel;

	private int m_CurrentSequenceIndex;

#if !WITH_WWISE
	bool m_InitialAnswerSkipped = false;
#endif

	public void ResetDialog()
	{
		m_CurrentSequenceIndex = -1;
	}

	public bool Update()
	{
#if !WITH_WWISE
		if (!Globals.m_ConversationSystem.IsFaceFXAnimating() || !m_InitialAnswerSkipped)
		{
			m_InitialAnswerSkipped = true;
			return MoveToNextDialogSequence();
		}
#else
		if (!Globals.m_ConversationSystem.IsFaceFXAnimating())
		{
			return MoveToNextDialogSequence();
		}
#endif
		return true;
	}

	public bool MoveToNextDialogSequence()
	{
		m_CurrentSequenceIndex++;
		if (Globals.m_ConversationSystem.GetPersuasionLevel() >= m_RequiredPersuasionLevel)
		{
			if (m_DialogSequences == null || m_CurrentSequenceIndex == m_DialogSequences.Length)
			{
				return false;
			}
			Globals.m_ConversationSystem.SetupDialog(m_DialogSequences[m_CurrentSequenceIndex]);
		}
		else
		{
			if (m_CurrentSequenceIndex == m_FailedDialogSequences.Length)
			{
				return false;
			}
			Globals.m_ConversationSystem.SetupDialog(m_FailedDialogSequences[m_CurrentSequenceIndex]);
		}
		return true;
	}
}
