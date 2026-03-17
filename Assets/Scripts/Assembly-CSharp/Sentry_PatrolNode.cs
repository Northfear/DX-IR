using UnityEngine;

public class Sentry_PatrolNode : MonoBehaviour
{
	public enum PatrolEvent
	{
		None = -1,
		TurnBack = 0,
		Idle = 1
	}

	public enum EventChance
	{
		Always = 0,
		Often = 1,
		Sometimes = 2,
		Rarely = 3,
		Once = 4,
		Never = 5
	}

	public Sentry_PatrolNode m_ForwardConnection;

	public Sentry_PatrolNode m_ReverseConnection;

	public PatrolEvent m_PatrolEvent = PatrolEvent.None;

	public EventChance m_EventChance = EventChance.Never;

	public float m_MinIdle = 5f;

	public float m_MaxIdle = 10f;
}
