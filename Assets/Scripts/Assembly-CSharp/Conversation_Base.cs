using Fabric;
using UnityEngine;

public class Conversation_Base : MonoBehaviour
{
	public enum DialogChoiceEnum
	{
		DialogChoice1 = 0,
		DialogChoice2 = 1,
		DialogChoice3 = 2,
		DialogChoice4 = 3,
		DialogChoice5 = 4
	}

	public string m_ConversationName;

	public ConversationInfo m_ConversationInfo;

	public PersuasionInfo m_PersuasionInfo;

	public DialogSequence[] m_PreChoiceDialogSequence;

	public Camera m_ConversationCamera;

	protected DialogChoice m_SelectedDialogChoice;

	protected bool m_ConversationInitiated;

	protected bool m_PreConversation;

	protected int m_PreSequenceIndex;

	public void SetSelectedDialogChoice(DialogChoiceEnum choice)
	{
		switch (choice)
		{
		case DialogChoiceEnum.DialogChoice1:
			m_SelectedDialogChoice = m_ConversationInfo.m_DialogChoice1;
			break;
		case DialogChoiceEnum.DialogChoice2:
			m_SelectedDialogChoice = m_ConversationInfo.m_DialogChoice2;
			break;
		case DialogChoiceEnum.DialogChoice3:
			m_SelectedDialogChoice = m_ConversationInfo.m_DialogChoice3;
			break;
		case DialogChoiceEnum.DialogChoice4:
			m_SelectedDialogChoice = m_ConversationInfo.m_DialogChoice4;
			break;
		default:
			m_SelectedDialogChoice = m_ConversationInfo.m_DialogChoice5;
			break;
		}
	}

	public void SkipCurrentDialogSequence()
	{
		Globals.m_ConversationSystem.StopCurrentDialog();
		if (m_PreConversation)
		{
			MoveToNextPreSequenceDialog();
		}
		else if (m_ConversationInitiated)
		{
			MoveToNextPostSequenceDialog();
		}
	}

	public virtual void BeginConversation(bool postSequence)
	{
		Globals.m_ConversationSystem.SetupConvsersation(this);
		Globals.m_ConversationSystem.SetupChoicePressed(null);
		if (m_PreChoiceDialogSequence.Length > 0)
		{
			Globals.m_ConversationSystem.BringInDialogPanel(true);
			m_PreConversation = true;
			m_PreSequenceIndex = -1;
		}
		else
		{
			Globals.m_ConversationSystem.BringInConversationPanel(postSequence);
		}
	}

	public virtual void DialogChoicePressed(DialogChoiceEnum choice)
	{
		SetSelectedDialogChoice(choice);
		Globals.m_ConversationSystem.SetupChoicePressed(m_SelectedDialogChoice);
		EventManager.Instance.PostEvent("UI_Toggle", EventAction.PlaySound, null, base.gameObject);
	}

	public virtual void DialogChoiceSelected()
	{
		m_ConversationInitiated = true;
		m_SelectedDialogChoice.ResetDialog();
		Globals.m_ConversationSystem.BringInDialogPanel(false);
		Globals.m_ConversationSystem.AddPersuasionLevel(m_SelectedDialogChoice.m_PersuasionAmount);
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}

	protected virtual void MoveToNextConversation()
	{
		int persuasionLevel = Globals.m_ConversationSystem.GetPersuasionLevel();
		bool flag = false;
		if (m_SelectedDialogChoice == null)
		{
			flag = true;
		}
		else if (persuasionLevel >= m_SelectedDialogChoice.m_RequiredPersuasionLevel)
		{
			if (m_SelectedDialogChoice.m_ConnectingConversation == null)
			{
				flag = true;
			}
		}
		else if (m_SelectedDialogChoice.m_FailedConnectingConversation == null)
		{
			flag = true;
		}
		if (flag)
		{
			EndConversation();
		}
		else if (persuasionLevel >= m_SelectedDialogChoice.m_RequiredPersuasionLevel)
		{
			m_SelectedDialogChoice.m_ConnectingConversation.BeginConversation(true);
		}
		else
		{
			m_SelectedDialogChoice.m_FailedConnectingConversation.BeginConversation(true);
		}
		if (m_SelectedDialogChoice.m_ConversationTriggerClass != null && m_SelectedDialogChoice.m_TriggerFunctionName != string.Empty)
		{
			m_SelectedDialogChoice.m_ConversationTriggerClass.Invoke(m_SelectedDialogChoice.m_TriggerFunctionName, 0f);
		}
		m_SelectedDialogChoice = null;
		m_ConversationInitiated = false;
	}

	protected virtual void EndConversation()
	{
		Globals.m_ConversationSystem.EndConversation();
	}

	protected virtual void UpdateConversation()
	{
		if (m_SelectedDialogChoice != null && !m_SelectedDialogChoice.Update())
		{
			MoveToNextConversation();
		}
	}

	protected virtual void UpdatePreConversation()
	{
		if (!Globals.m_ConversationSystem.IsFaceFXAnimating())
		{
			MoveToNextPreSequenceDialog();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (m_PreConversation)
		{
			UpdatePreConversation();
		}
		else if (m_ConversationInitiated)
		{
			UpdateConversation();
		}
	}

	private void MoveToNextPreSequenceDialog()
	{
		m_PreSequenceIndex++;
		if (m_PreSequenceIndex != m_PreChoiceDialogSequence.Length)
		{
			Globals.m_ConversationSystem.SetupDialog(m_PreChoiceDialogSequence[m_PreSequenceIndex]);
			return;
		}
		m_PreConversation = false;
		Globals.m_ConversationSystem.BringInConversationPanel(true);
	}

	private void MoveToNextPostSequenceDialog()
	{
		if (!m_SelectedDialogChoice.MoveToNextDialogSequence())
		{
			MoveToNextConversation();
		}
	}
}
