using UnityEngine;

public class OpenDoor : MonoBehaviour
{
	private bool m_DoorOpen;

	public GameObject m_DoorObject;

	private void OnTriggerEnter(Collider other)
	{
		if (!m_DoorOpen && other.gameObject.tag == "Player")
		{
			m_DoorObject.animation.Play();
			m_DoorOpen = true;
			SoundManager.TriggerEvent("Play_Door_Metal_Slide_Open", base.gameObject);
		}
	}
}
