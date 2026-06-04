using System;
using UnityEngine;

public class CommLinkDialog : MonoBehaviour
{
	public enum DialogEventType
	{
		None = -1,
		PlayerDialog = 0,
		LinkDialog = 1,
		OpenLink = 2,
		CloseLink = 3
	}

	[Serializable]
	public class CommDialogEvent
	{
		public DialogEventType m_EventType = DialogEventType.None;

		public float m_TriggerDelay = -1f;

		public string m_CommAudioEvent = string.Empty;

		public string m_CommSubtitle = string.Empty;
	}

	[Serializable]
	public class CommDialog
	{
		public Texture2D m_CharacterPortrait;

		public string m_CharacterName = string.Empty;

		public CommDialogEvent[] m_DialogEvents;
	}

	private static CommLinkDialog m_This;

	public CommDialog[] m_CommDialogs;

	private int m_DialogIndex = -1;

	private int m_EventIndex;

	private float m_EventTimer;

	private bool m_LinkOpen;

	private bool m_CommPaused;

	private uint m_LinkAudioPlayingID;

	private string m_LastCharacterSubtitle = string.Empty;

	private uint m_PlayerAudioPlayingID;

	private string m_LastPlayerSubtitle = string.Empty;

	public static bool LinkOpen()
	{
		return !(m_This == null) && m_This.m_LinkOpen;
	}

	public static Texture2D GetCurrentCharacterPortrait()
	{
		return m_This.m_CommDialogs[m_This.m_DialogIndex].m_CharacterPortrait;
	}

	public static string GetCurrentCharacterName()
	{
		return m_This.m_CommDialogs[m_This.m_DialogIndex].m_CharacterName;
	}

	public static string GetLastCharacterSubtitle()
	{
		return m_This.m_LastCharacterSubtitle;
	}

	public static string GetLastPlayerSubtitle()
	{
		return m_This.m_LastPlayerSubtitle;
	}

	public static bool CharacterTalking()
	{
#if !WITH_WWISE
		if (m_This == null)
		{
			return false;
		}
		
		if (m_This.m_DialogIndex < 0)
		{
			return false;
		}
		
		int activeEventIndex = m_This.m_EventIndex - 1;

		if (activeEventIndex < 0)
		{
			return false;
		}

		if (activeEventIndex >= m_This.m_CommDialogs[m_This.m_DialogIndex].m_DialogEvents.Length)
		{
			activeEventIndex = m_This.m_CommDialogs[m_This.m_DialogIndex].m_DialogEvents.Length - 1;
		}
		
		if (activeEventIndex >= 0)
		{
			if (m_This.m_CommDialogs[m_This.m_DialogIndex].m_DialogEvents[activeEventIndex].m_EventType != DialogEventType.LinkDialog)
			{
				return false;
			}
			
			return m_This.m_EventTimer < m_This.m_CommDialogs[m_This.m_DialogIndex].m_DialogEvents[activeEventIndex].m_TriggerDelay + 5f;
		}
#endif
		return !(m_This == null) && m_This.m_LinkAudioPlayingID != 0;
	}

	public static bool PlayerTalking()
	{
#if !WITH_WWISE
		if (m_This == null)
		{
			return false;
		}

		if (m_This.m_DialogIndex < 0)
		{
			return false;
		}
		
		int activeEventIndex = m_This.m_EventIndex - 1;

		if (activeEventIndex < 0)
		{
			return false;
		}

		if (activeEventIndex >= m_This.m_CommDialogs[m_This.m_DialogIndex].m_DialogEvents.Length)
		{
			activeEventIndex = m_This.m_CommDialogs[m_This.m_DialogIndex].m_DialogEvents.Length - 1;
		}
		
		if (activeEventIndex >= 0)
		{
			if (m_This.m_CommDialogs[m_This.m_DialogIndex].m_DialogEvents[activeEventIndex].m_EventType != DialogEventType.PlayerDialog)
			{
				return false;
			}
			
			return m_This.m_EventTimer < m_This.m_CommDialogs[m_This.m_DialogIndex].m_DialogEvents[activeEventIndex].m_TriggerDelay + 5f;
		}
#endif
		return !(m_This == null) && m_This.m_PlayerAudioPlayingID != 0;
	}

	private void Awake()
	{
		m_This = this;
	}

	public static void PlayDialog(int Index)
	{
		if (m_This.m_CommDialogs != null && Index >= 0 && Index < m_This.m_CommDialogs.Length && !(m_This.m_CommDialogs[Index].m_CharacterPortrait == null) && m_This.m_CommDialogs[Index].m_DialogEvents != null)
		{
			// Close any existing link before starting new dialog
			if (m_This.m_LinkOpen)
			{
				Globals.m_HUD.CloseCommLink();
				m_This.m_LinkOpen = false;
			}
			
			m_This.m_DialogIndex = Index;
			m_This.m_EventTimer = 0f;
			m_This.m_EventIndex = 0;
		}
	}

	public static void Paused(bool paused)
	{
		if (!(m_This == null))
		{
			if (paused)
			{
				m_This.m_CommPaused = true;
			}
			else
			{
				m_This.m_CommPaused = false;
			}
		}
	}

	private void Update()
	{
		if ((!CharacterTalking() || (m_LinkAudioPlayingID != 0 && !SoundManager.IsEventPlaying(m_LinkAudioPlayingID))) && !m_CommPaused)
		{
			Globals.m_HUD.HideCommLinkSubtitle();
			m_LinkAudioPlayingID = 0u;
		}
	
		if ((!PlayerTalking() || (m_PlayerAudioPlayingID != 0 && !SoundManager.IsEventPlaying(m_PlayerAudioPlayingID))) && !m_CommPaused)
		{
			Globals.m_HUD.HidePlayerSubtitle();
			m_PlayerAudioPlayingID = 0u;
		}

		if (m_DialogIndex < 0)
		{
			return;
		}

		m_EventTimer += Time.deltaTime;

		while (m_EventIndex < m_CommDialogs[m_DialogIndex].m_DialogEvents.Length)
		{
			if (m_CommDialogs[m_DialogIndex].m_DialogEvents[m_EventIndex].m_EventType != DialogEventType.None)
			{
				if (!(m_EventTimer >= m_CommDialogs[m_DialogIndex].m_DialogEvents[m_EventIndex].m_TriggerDelay))
				{
					break;
				}
				TriggerEvent(DialogEventType.None);
			}
			m_EventIndex++;
		}

		if (m_EventIndex >= m_CommDialogs[m_DialogIndex].m_DialogEvents.Length && !CharacterTalking() && !PlayerTalking())
		{
			if (m_LinkOpen)
			{
				TriggerEvent(DialogEventType.CloseLink);
				m_LinkOpen = false;
			}
			m_DialogIndex = -1;
		}
	}

	private void TriggerEvent(DialogEventType eventType = DialogEventType.None)
	{
		switch (eventType)
		{
		case DialogEventType.None:
			eventType = m_CommDialogs[m_DialogIndex].m_DialogEvents[m_EventIndex].m_EventType;
			break;
		default:
			return;
		case DialogEventType.CloseLink:
			break;
		}
		switch (eventType)
		{
		case DialogEventType.OpenLink:
			if (!m_LinkOpen)
			{
				Globals.m_HUD.OpenCommLink(m_CommDialogs[m_DialogIndex].m_CharacterPortrait, m_CommDialogs[m_DialogIndex].m_CharacterName);
				m_LinkOpen = true;
				SoundManager.TriggerEvent("Play_Comm_Incoming", base.gameObject);
			}
			break;
		case DialogEventType.CloseLink:
			if (m_LinkOpen)
			{
				Globals.m_HUD.CloseCommLink();
				m_LinkOpen = false;
				SoundManager.TriggerEvent("Play_Comm_End", base.gameObject);
			}
			break;
		case DialogEventType.PlayerDialog:
			m_LastPlayerSubtitle = m_CommDialogs[m_DialogIndex].m_DialogEvents[m_EventIndex].m_CommSubtitle;
			Globals.m_HUD.DisplayPlayerSubtitle(m_LastPlayerSubtitle);
			m_PlayerAudioPlayingID = SoundManager.TriggerEvent(m_CommDialogs[m_DialogIndex].m_DialogEvents[m_EventIndex].m_CommAudioEvent);
			break;
		case DialogEventType.LinkDialog:
			if (!m_LinkOpen)
			{
				Globals.m_HUD.OpenCommLink(m_CommDialogs[m_DialogIndex].m_CharacterPortrait, m_CommDialogs[m_DialogIndex].m_CharacterName);
				m_LinkOpen = true;
				SoundManager.TriggerEvent("Play_Comm_Incoming", base.gameObject);
			}
			m_LastCharacterSubtitle = m_CommDialogs[m_DialogIndex].m_DialogEvents[m_EventIndex].m_CommSubtitle;
			Globals.m_HUD.DisplayLinkSubtitle(m_LastCharacterSubtitle);
			m_LinkAudioPlayingID = SoundManager.TriggerEvent(m_CommDialogs[m_DialogIndex].m_DialogEvents[m_EventIndex].m_CommAudioEvent);
			break;
		}
	}
}
