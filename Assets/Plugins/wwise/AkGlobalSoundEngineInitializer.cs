using UnityEngine;

public class AkGlobalSoundEngineInitializer : MonoBehaviour
{
	public string basePath = AkBankPath.GetBasePath();

	public string language = "English(US)";

	public int defaultPoolSize = 4096;

	public int lowerPoolSize = 2048;

	public int streamingPoolSize = 1024;

	public float memoryCutoffThreshold = 0.9f;

	private static AkGlobalSoundEngineInitializer ms_Instance;

	private void Awake()
	{
		if (ms_Instance != null)
		{
			return;
		}
		Debug.Log("WwiseUnity: Initialize sound engine ...");
		AkMemSettings akMemSettings = new AkMemSettings();
		akMemSettings.uMaxNumPools = 20u;
		AkDeviceSettings akDeviceSettings = new AkDeviceSettings();
		AkSoundEngine.GetDefaultDeviceSettings(akDeviceSettings);
		AkStreamMgrSettings akStreamMgrSettings = new AkStreamMgrSettings();
		akStreamMgrSettings.uMemorySize = (uint)(streamingPoolSize * 1024);
		AkInitSettings akInitSettings = new AkInitSettings();
		AkSoundEngine.GetDefaultInitSettings(akInitSettings);
		akInitSettings.uDefaultPoolSize = (uint)(defaultPoolSize * 1024);
		AkPlatformInitSettings akPlatformInitSettings = new AkPlatformInitSettings();
		AkSoundEngine.GetDefaultPlatformInitSettings(akPlatformInitSettings);
		akPlatformInitSettings.uLEngineDefaultPoolSize = (uint)(lowerPoolSize * 1024);
		akPlatformInitSettings.fLEngineDefaultPoolRatioThreshold = memoryCutoffThreshold;
		AkMusicSettings akMusicSettings = new AkMusicSettings();
		AkSoundEngine.GetDefaultMusicSettings(akMusicSettings);
		AKRESULT aKRESULT = AkSoundEngine.Init(akMemSettings, akStreamMgrSettings, akDeviceSettings, akInitSettings, akPlatformInitSettings, akMusicSettings);
		if (aKRESULT != AKRESULT.AK_Success)
		{
			Debug.LogError("WwiseUnity: Failed to initialize the sound engine. Abort.");
			return;
		}
		AkBankPath.UsePlatformSpecificPath();
		string platformBasePath = AkBankPath.GetPlatformBasePath();
		if (!AkBankPath.Exists(platformBasePath))
		{
			Debug.LogError("WwiseUnity: Failed to find soundbank folder. Abort.");
			return;
		}
		AkSoundEngine.SetBasePath(platformBasePath);
		AkSoundEngine.SetCurrentLanguage(language);
		aKRESULT = AkCallbackManager.Init();
		if (aKRESULT != AKRESULT.AK_Success)
		{
			Debug.LogError("WwiseUnity: Failed to initialize Callback Manager. Terminate sound engine.");
			AkSoundEngine.Term();
			return;
		}
		AkCallbackManager.SetMonitoringCallback(ErrorLevel.ErrorLevel_All, null);
		Debug.Log("WwiseUnity: Sound engine initialized.");
		Object.DontDestroyOnLoad(this);
		ms_Instance = this;
		uint out_bankID;
		AkSoundEngine.LoadBank("Init.bnk", -1, out out_bankID);
	}

	private void OnDestroy()
	{
		ms_Instance = null;
	}

	private void OnApplicationQuit()
	{
	}

	private void OnEnable()
	{
		if (ms_Instance == null && AkSoundEngine.IsInitialized())
		{
			ms_Instance = this;
		}
	}

	private void LateUpdate()
	{
		if (ms_Instance != null)
		{
			AkCallbackManager.PostCallbacks();
			AkSoundEngine.RenderAudio();
		}
	}
}
