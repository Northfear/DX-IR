using UnityEngine;

public class AkGlobalSoundEngineTerminator : MonoBehaviour
{
	private static AkGlobalSoundEngineTerminator ms_Instance;

	private void Awake()
	{
		if (!(ms_Instance != null))
		{
			Object.DontDestroyOnLoad(this);
			ms_Instance = this;
		}
	}

	private void OnDestroy()
	{
		Terminate();
	}

	private void OnApplicationQuit()
	{
	}

	private void Terminate()
	{
		if (!(ms_Instance == null))
		{
			if (AkSoundEngine.IsInitialized())
			{
				AkSoundEngine.Term();
				AkCallbackManager.Term();
			}
			ms_Instance = null;
		}
	}
}
