using Fabric;
using UnityEngine;

public class IntroSecurity : MonoBehaviour
{
	public GameObject m_ElevatorObject;

	public GameObject m_ElevatorCollision;

	public float m_ElevatorOpenDelay = 8f;

	private bool m_ElevatorOpen;

	public GameObject m_InitialPatrol;

	private void Update()
	{
		if (m_ElevatorOpenDelay > 0f)
		{
			m_ElevatorOpenDelay -= Time.deltaTime;
		}
		else if (!m_ElevatorOpen)
		{
			m_ElevatorOpen = true;
			m_ElevatorObject.animation.Play();
			Object.Destroy(m_ElevatorCollision, 2f);
			m_InitialPatrol.SetActiveRecursively(true);
			EventManager.Instance.PostEvent("Elevator_Loop", EventAction.StopSound, null, Globals.m_PlayerController.gameObject);
			EventManager.Instance.PostEvent("Elevator_Stop", EventAction.PlaySound, null, base.gameObject);
		}
	}
}
