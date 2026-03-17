using UnityEngine;

public class SoundManager
{
	private static bool m_Initialized;

	private static string m_LevelMusic = string.Empty;

	private static string m_LastMusic = string.Empty;

	private static GameObject m_GlobalObject;

	public static void Initialize(GameObject go)
	{
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
	}

	public static void LoadSoundBank(string name)
	{
		if (m_Initialized)
		{
			uint out_bankID;
			AkSoundEngine.LoadBank(name, -1, out out_bankID);
		}
	}

	public static void UnloadSoundBank(string name)
	{
		if (m_Initialized)
		{
			AkSoundEngine.UnloadBank(name);
		}
	}

	public static uint TriggerEvent(string name, GameObject gameObject)
	{
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
	}

	public static uint TriggerEvent(string name)
	{
		if (!m_Initialized)
		{
			return 0u;
		}
		return AkSoundEngine.PostEvent(name, m_GlobalObject, 65536u);
	}

	public static void StopEvent(uint playingID)
	{
		AkSoundEngine.StopPlayingID(playingID);
	}

	public static bool IsEventPlaying(uint playingID)
	{
		int out_puPosition;
		AKRESULT sourcePlayPosition = AkSoundEngine.GetSourcePlayPosition(playingID, out out_puPosition);
		if (sourcePlayPosition == AKRESULT.AK_Success && out_puPosition >= 0)
		{
			return true;
		}
		return false;
	}

	public static float GetEventCurrentPosition(uint playingID)
	{
		int out_puPosition;
		AKRESULT sourcePlayPosition = AkSoundEngine.GetSourcePlayPosition(playingID, out out_puPosition);
		if (sourcePlayPosition == AKRESULT.AK_Success)
		{
			return (float)out_puPosition / 1000f;
		}
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
		if (m_Initialized)
		{
			AkSoundEngine.SetSwitch(name, switchname, gameObject);
		}
	}

	public static void StopAll()
	{
		if (m_Initialized)
		{
			AkSoundEngine.StopAll();
		}
	}
}
