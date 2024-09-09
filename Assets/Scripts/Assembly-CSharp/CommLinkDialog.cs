using System;
using Fabric;
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

		public AudioClip m_CommAudio;

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

	private AudioSource m_LinkAudioSource;

	private string m_LastCharacterSubtitle = string.Empty;

	private AudioSource m_PlayerAudioSource;

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
		return !(m_This == null) && m_This.m_LinkAudioSource.clip != null;
	}

	public static bool PlayerTalking()
	{
		return !(m_This == null) && m_This.m_PlayerAudioSource.clip != null;
	}

	private void Awake()
	{
		m_This = this;
		m_LinkAudioSource = base.gameObject.AddComponent<AudioSource>();
		m_PlayerAudioSource = base.gameObject.AddComponent<AudioSource>();
	}

	public static void PlayDialog(int Index)
	{
		if (m_This.m_CommDialogs != null && Index >= 0 && Index < m_This.m_CommDialogs.Length && !(m_This.m_CommDialogs[Index].m_CharacterPortrait == null) && m_This.m_CommDialogs[Index].m_DialogEvents != null)
		{
			m_This.m_DialogIndex = Index;
			m_This.m_EventTimer = 0f;
			m_This.m_EventIndex = 0;
		}
	}

	public static void Paused(bool paused)
	{
		if (m_This == null)
		{
			return;
		}
		if (paused)
		{
			if (m_This.m_LinkAudioSource.clip != null)
			{
				m_This.m_LinkAudioSource.Pause();
			}
			if (m_This.m_PlayerAudioSource.clip != null)
			{
				m_This.m_PlayerAudioSource.Pause();
			}
			m_This.m_CommPaused = true;
		}
		else
		{
			if (m_This.m_LinkAudioSource.clip != null)
			{
				m_This.m_LinkAudioSource.Play();
			}
			if (m_This.m_PlayerAudioSource.clip != null)
			{
				m_This.m_PlayerAudioSource.Play();
			}
			m_This.m_CommPaused = false;
		}
	}

	private void Update()
	{
		if (m_DialogIndex >= 0)
		{
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
		if (m_LinkAudioSource.clip != null && !m_LinkAudioSource.isPlaying && !m_CommPaused)
		{
			Globals.m_HUD.HideCommLinkSubtitle();
			m_LinkAudioSource.clip = null;
		}
		if (m_PlayerAudioSource.clip != null && !m_PlayerAudioSource.isPlaying && !m_CommPaused)
		{
			Globals.m_HUD.HidePlayerSubtitle();
			m_PlayerAudioSource.clip = null;
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
				EventManager.Instance.PostEvent("Comm_Incoming", EventAction.PlaySound, null);
			}
			break;
		case DialogEventType.CloseLink:
			if (m_LinkOpen)
			{
				Globals.m_HUD.CloseCommLink();
				m_LinkOpen = false;
				EventManager.Instance.PostEvent("Comm_End", EventAction.PlaySound, null);
			}
			break;
		case DialogEventType.PlayerDialog:
			m_LastPlayerSubtitle = m_CommDialogs[m_DialogIndex].m_DialogEvents[m_EventIndex].m_CommSubtitle;
			Globals.m_HUD.DisplayPlayerSubtitle(m_LastPlayerSubtitle);
			m_PlayerAudioSource.Stop();
			m_PlayerAudioSource.clip = m_CommDialogs[m_DialogIndex].m_DialogEvents[m_EventIndex].m_CommAudio;
			m_PlayerAudioSource.Play();
			break;
		case DialogEventType.LinkDialog:
			if (!m_LinkOpen)
			{
				Globals.m_HUD.OpenCommLink(m_CommDialogs[m_DialogIndex].m_CharacterPortrait, m_CommDialogs[m_DialogIndex].m_CharacterName);
				m_LinkOpen = true;
				EventManager.Instance.PostEvent("Comm_Incoming", EventAction.PlaySound, null);
			}
			m_LastCharacterSubtitle = m_CommDialogs[m_DialogIndex].m_DialogEvents[m_EventIndex].m_CommSubtitle;
			Globals.m_HUD.DisplayLinkSubtitle(m_LastCharacterSubtitle);
			m_LinkAudioSource.Stop();
			m_LinkAudioSource.clip = m_CommDialogs[m_DialogIndex].m_DialogEvents[m_EventIndex].m_CommAudio;
			m_LinkAudioSource.Play();
			break;
		}
	}
}
