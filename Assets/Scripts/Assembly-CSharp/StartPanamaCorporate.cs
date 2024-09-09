using UnityEngine;

public class StartPanamaCorporate : MonoBehaviour
{
	public Transform m_Exit;

	public Transform m_Elevator;

	private void Start()
	{
		if (Globals.m_PanamaVTOLMissionStarted && !Globals.m_PanamaVTOLMissionComplete)
		{
			Globals.m_PrimaryObjective = m_Elevator;
		}
		else
		{
			Globals.m_PrimaryObjective = m_Exit;
		}
	}
}
