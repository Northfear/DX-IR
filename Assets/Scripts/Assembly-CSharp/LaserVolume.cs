using UnityEngine;

public class LaserVolume : MonoBehaviour
{
	private bool m_LaserArmed = true;

	private bool m_AlarmActive;

	public GameObject m_AlarmResponseObject;

	private void OnTriggerEnter(Collider other)
	{
		if (m_LaserArmed && !m_AlarmActive && other.gameObject.tag == "Player")
		{
			m_AlarmActive = true;
			SoundManager.TriggerEvent("Play_Alarm", base.gameObject);
			if (m_AlarmResponseObject != null)
			{
				m_AlarmResponseObject.SetActiveRecursively(true);
			}
		}
	}
}
