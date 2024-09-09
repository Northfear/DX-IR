using Fabric;
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
			EventManager.Instance.PostEvent("Door_Metal_Slide_Open", EventAction.PlaySound, null, base.gameObject);
		}
	}
}
