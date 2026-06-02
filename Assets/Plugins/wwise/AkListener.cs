using UnityEngine;

public class AkListener : MonoBehaviour
{
	public int listenerId;

	private Vector3 m_Position;

	private Vector3 m_Top;

	private Vector3 m_Front;

	private void Update()
	{
		if (!(m_Position == base.transform.position) || !(m_Front == base.transform.forward) || !(m_Top == base.transform.up))
		{
			m_Position = base.transform.position;
			m_Front = base.transform.forward;
			m_Top = base.transform.up;
#if SOUND_WWISE
			AkSoundEngine.SetListenerPosition(base.transform.forward.x, base.transform.forward.y, base.transform.forward.z, base.transform.up.x, base.transform.up.y, base.transform.up.z, base.transform.position.x, base.transform.position.y, base.transform.position.z, (uint)listenerId);
#endif
		}
	}
}
