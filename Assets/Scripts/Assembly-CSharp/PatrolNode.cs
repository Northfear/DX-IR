using UnityEngine;

public class PatrolNode : MonoBehaviour
{
	public enum PatrolEvent
	{
		None = -1,
		TurnBack = 0,
		Idle = 1,
		Waypoint = 2,
		Dialog = 3,
		CycleBreaker = 4
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

	public enum WaypointDirection
	{
		Forward = 0,
		Back = 1,
		Right = 2,
		Left = 3
	}

	public PatrolNode m_ForwardConnection;

	public PatrolNode m_ReverseConnection;

	public PatrolEvent m_PatrolEvent = PatrolEvent.None;

	public EventChance m_EventChance = EventChance.Never;

	public float m_MinIdle = 5f;

	public float m_MaxIdle = 10f;

	public WaypointDirection m_WaypointDirection;

	public string m_WaypointAnimation = string.Empty;

	public bool m_AppendPoseAnimation = true;

	public string m_Dialog = string.Empty;
}
