using UnityEngine;

public class AudioTriggerToggle : MonoBehaviour
{
	public bool m_EnableOnTrigger = true;

	public Collider[] m_CollidersToToggle;

	private void OnTriggerEnter(Collider other)
	{
		if (m_CollidersToToggle != null)
		{
			for (int i = 0; i < m_CollidersToToggle.Length; i++)
			{
				m_CollidersToToggle[i].enabled = m_EnableOnTrigger;
			}
		}
	}
}
