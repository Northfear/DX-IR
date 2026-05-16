using UnityEngine;

public class SoundManager
{
	private static bool m_Initialized;

	private static string m_LevelMusic = string.Empty;

	private static string m_LastMusic = string.Empty;

	private static GameObject m_GlobalObject;

	public static void Initialize(GameObject go)
	{
#if SOUND_WWISE
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXPlayer && AudioListener.volume != 0f)
		{
			m_GlobalObject = go;
			m_GlobalObject.AddComponent("AkGlobalSoundEngineInitializer");
			m_GlobalObject.AddComponent("AkGlobalSoundEngineTerminator");
			if (!AkSoundEngine.IsInitialized())
			{
				Debug.LogError("Error Initializing Wwise.");
			}
			else
			{
				m_Initialized = true;
			}
		}
#endif
	}

	public static void LoadSoundBank(string name)
	{
#if SOUND_WWISE
		if (m_Initialized)
		{
			uint out_bankID;
			AkSoundEngine.LoadBank(name, -1, out out_bankID);
		}
#endif
	}

	public static void UnloadSoundBank(string name)
	{
#if SOUND_WWISE
		if (m_Initialized)
		{
			AkSoundEngine.UnloadBank(name);
		}
#endif
	}

	public static uint TriggerEvent(string name, GameObject gameObject)
	{
#if SOUND_WWISE
		if (!m_Initialized)
		{
			return 0u;
		}
		if (gameObject == null)
		{
			Debug.LogError("Can't trigger sound event: " + name + ", gameObject is null.");
			return 0u;
		}
		return AkSoundEngine.PostEvent(name, gameObject, 65536u);
#else
		return 0u;
#endif
	}

	public static uint TriggerEvent(string name)
	{
#if SOUND_WWISE
		if (!m_Initialized)
		{
			return 0u;
		}
		return AkSoundEngine.PostEvent(name, m_GlobalObject, 65536u);
#else
		return 0u;
#endif
	}

	public static void StopEvent(uint playingID)
	{
#if SOUND_WWISE
		AkSoundEngine.StopPlayingID(playingID);
#endif
	}

	public static bool IsEventPlaying(uint playingID)
	{
#if SOUND_WWISE
		int out_puPosition;
		AKRESULT sourcePlayPosition = AkSoundEngine.GetSourcePlayPosition(playingID, out out_puPosition);
		if (sourcePlayPosition == AKRESULT.AK_Success && out_puPosition >= 0)
		{
			return true;
		}
#endif
		return false;
	}

	public static float GetEventCurrentPosition(uint playingID)
	{
#if SOUND_WWISE
		int out_puPosition;
		AKRESULT sourcePlayPosition = AkSoundEngine.GetSourcePlayPosition(playingID, out out_puPosition);
		if (sourcePlayPosition == AKRESULT.AK_Success)
		{
			return (float)out_puPosition / 1000f;
		}
#endif
		return -1f;
	}

	public static void SetLevelMusic(string name)
	{
		m_LevelMusic = name;
	}

	public static void PlayLevelMusic()
	{
		if (!(m_LevelMusic == string.Empty))
		{
			if (m_LastMusic != string.Empty)
			{
				TriggerEvent("Stop_" + m_LastMusic, m_GlobalObject);
			}
			TriggerEvent("Play_" + m_LevelMusic, m_GlobalObject);
		}
	}

	public static void StopLevelMusic()
	{
		if (!(m_LevelMusic == string.Empty))
		{
			TriggerEvent("Stop_" + m_LevelMusic, m_GlobalObject);
			m_LastMusic = string.Empty;
		}
	}

	public static void PlayMusic(string name)
	{
		if (m_LevelMusic != string.Empty)
		{
			TriggerEvent("Stop_" + m_LevelMusic, m_GlobalObject);
		}
		TriggerEvent("Play_" + name, m_GlobalObject);
		m_LastMusic = name;
	}

	public static void SetSoundSwitch(string name, string switchname, GameObject gameObject)
	{
#if SOUND_WWISE
		if (m_Initialized)
		{
			AkSoundEngine.SetSwitch(name, switchname, gameObject);
		}
#endif
	}

	public static void StopAll()
	{
#if SOUND_WWISE
		if (m_Initialized)
		{
			AkSoundEngine.StopAll();
		}
#endif
	}
}
