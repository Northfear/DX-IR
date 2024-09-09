using Fabric;
using UnityEngine;

public class OpenVent : MonoBehaviour
{
	private bool m_VentOpen;

	public GameObject m_VentObject;

	private GameObject m_VentCollisionObject;

	private void Start()
	{
		if ((bool)m_VentObject)
		{
			m_VentCollisionObject = m_VentObject.transform.Find("vent_entrance").gameObject;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!m_VentOpen && other.gameObject.tag == "Player")
		{
			m_VentObject.animation.Play();
			m_VentCollisionObject.collider.enabled = false;
			m_VentOpen = true;
			EventManager.Instance.PostEvent("Vent_Open", EventAction.PlaySound, null, base.gameObject);
		}
	}
}
