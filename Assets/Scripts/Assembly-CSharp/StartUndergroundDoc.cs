using UnityEngine;

public class StartUndergroundDoc : MonoBehaviour
{
	public Transform m_Doctor;

	public Transform m_Exit;

	private void Start()
	{
		if (!Globals.m_PanamaVisitedDoctor)
		{
			Globals.m_PrimaryObjective = m_Doctor;
		}
		else
		{
			Globals.m_PrimaryObjective = m_Exit;
		}
	}

	private void VisitDoctor()
	{
		Debug.Log("Visit doctor quest complete.\n+500 experience points!");
		Globals.m_PanamaVisitedDoctor = true;
		Globals.m_PrimaryObjective = m_Exit;
	}
}
