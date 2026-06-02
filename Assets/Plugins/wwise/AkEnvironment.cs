using UnityEngine;

public class AkEnvironment : MonoBehaviour
{
	public string environmentName;

	public float rollOffDistance;

	private uint m_EnvID;

	public uint GetEnvID()
	{
		return m_EnvID;
	}

	public virtual float GetEnvValueForPosition(Vector3 in_pos)
	{
		return 1f;
	}

	private void Awake()
	{
#if SOUND_WWISE
		m_EnvID = AkSoundEngine.GetIDFromString(environmentName);
#endif
	}
}
