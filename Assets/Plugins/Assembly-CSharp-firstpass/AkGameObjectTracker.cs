using UnityEngine;

[AddComponentMenu("Wwise/Game Object Tracker")]
public class AkGameObjectTracker : AkGameObject
{
	private Vector3 m_Position;

	private Vector3 m_Forward;

	private bool m_bHasMoved;

	public bool HasMovedInLastFrame()
	{
		return m_bHasMoved;
	}

	private void Update()
	{
		if (m_Position == base.transform.position && m_Forward == base.transform.forward)
		{
			m_bHasMoved = false;
			return;
		}
		m_Position = base.transform.position;
		m_Forward = base.transform.forward;
		m_bHasMoved = true;
		AkSoundEngine.SetObjectPosition(base.gameObject, base.transform.position.x, base.transform.position.y, base.transform.position.z, base.transform.forward.x, base.transform.forward.y, base.transform.forward.z);
	}
}
