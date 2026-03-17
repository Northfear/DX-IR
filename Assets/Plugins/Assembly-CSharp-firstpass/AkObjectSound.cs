using UnityEngine;

public class AkObjectSound : MonoBehaviour
{
	public string m_EventName = string.Empty;

	public bool m_Moving;

	private void Start()
	{
		if (m_Moving)
		{
			base.gameObject.AddComponent<AkGameObjectTracker>();
		}
		SoundManager.TriggerEvent(m_EventName, base.gameObject);
	}
}
