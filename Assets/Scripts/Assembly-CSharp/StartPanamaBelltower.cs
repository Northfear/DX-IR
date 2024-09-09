using UnityEngine;

public class StartPanamaBelltower : MonoBehaviour
{
	public Transform m_Elevator;

	public Transform m_SecurityConsole;

	private void Start()
	{
		if (Globals.m_PanamaVTOLMissionStarted && !Globals.m_PanamaVTOLMissionComplete)
		{
			Globals.m_PrimaryObjective = m_SecurityConsole;
		}
		else
		{
			Globals.m_PrimaryObjective = m_Elevator;
		}
	}

	private void HackSecurity()
	{
		Globals.m_PanamaVTOLMissionComplete = true;
		Globals.m_PrimaryObjective = m_Elevator;
	}
}
