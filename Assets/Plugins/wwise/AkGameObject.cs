using UnityEngine;

public class AkGameObject : MonoBehaviour
{
	private void Awake()
	{
#if SOUND_WWISE
		AkSoundEngine.RegisterGameObj(base.gameObject, base.gameObject.name);
		AkSoundEngine.SetObjectPosition(base.gameObject, base.transform.position.x, base.transform.position.y, base.transform.position.z, base.transform.forward.x, base.transform.forward.y, base.transform.forward.z);
#endif
	}

	private void OnDestroy()
	{
#if SOUND_WWISE
		if (AkSoundEngine.IsInitialized())
		{
			AkSoundEngine.UnregisterGameObj(base.gameObject);
		}
#endif
	}
}
