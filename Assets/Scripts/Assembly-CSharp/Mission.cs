using UnityEngine;

public class Mission
{
	public Location m_Location = Location.None;

	public string m_Name = string.Empty;

	public bool m_Primary = true;

	public int m_LocalIndex = -1;

	public bool m_Tracked;

	public MissionStatus m_Status;

	public SubMission[] m_Subs;

	public Transform[] m_Transforms;
}
