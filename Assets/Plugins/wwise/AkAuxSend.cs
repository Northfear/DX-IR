using UnityEngine;

public class AkAuxSend : MonoBehaviour
{
	public string auxBusName;

	public float rollOffDistance;

	private uint m_auxBusID;

	public uint GetAuxBusID()
	{
		return m_auxBusID;
	}

	public virtual float GetAuxSendValueForPosition(Vector3 in_pos)
	{
		return 1f;
	}

	private void Awake()
	{
#if SOUND_WWISE
		m_auxBusID = AkSoundEngine.GetIDFromString(auxBusName);
#endif
	}
}
