using UnityEngine;

public class AkGameObject : MonoBehaviour
{
	private void Awake()
	{
		AkSoundEngine.RegisterGameObj(base.gameObject, base.gameObject.name);
		AkSoundEngine.SetObjectPosition(base.gameObject, base.transform.position.x, base.transform.position.y, base.transform.position.z, base.transform.forward.x, base.transform.forward.y, base.transform.forward.z);
	}

	private void OnDestroy()
	{
		if (AkSoundEngine.IsInitialized())
		{
			AkSoundEngine.UnregisterGameObj(base.gameObject);
		}
	}
}
