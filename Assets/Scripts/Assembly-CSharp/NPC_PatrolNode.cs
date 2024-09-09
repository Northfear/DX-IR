using UnityEngine;

public class NPC_PatrolNode : MonoBehaviour
{
	public enum PatrolEvent
	{
		None = -1,
		TurnBack = 0,
		Idle = 1,
		Waypoint = 2,
		Dialog = 3
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
		Right = 1,
		Left = 2
	}

	public NPC_PatrolNode m_ForwardConnection;

	public NPC_PatrolNode m_ReverseConnection;

	public PatrolEvent m_PatrolEvent = PatrolEvent.None;

	public EventChance m_EventChance = EventChance.Never;

	public float m_MinIdle = 5f;

	public float m_MaxIdle = 10f;

	public WaypointDirection m_WaypointDirection;

	public string m_WaypointAnimation = string.Empty;

	public string m_DialogAnimation = string.Empty;

	public AudioClip m_DialogClip;
}
