using UnityEngine;

public class StartPanamaDowntown : MonoBehaviour
{
	public Transform m_BoardwalkAccess;

	public Transform m_LimbDoor;

	public Transform m_CorpDoor;

	public Transform m_HighRiseDoor;

	public GameObject m_LeaveLimbTrigger;

	private void Start()
	{
		if (Globals.m_PanamaVisitedDoctor && !Globals.m_PanamaVisitedLimb)
		{
			Globals.m_PrimaryObjective = m_LimbDoor;
		}
		if (Globals.m_PanamaVisitedLimb && !Globals.m_PanamaVisitedCobra)
		{
			Globals.m_PrimaryObjective = m_BoardwalkAccess;
		}
		if (Globals.m_PanamaDestroyedRiezol && !Globals.m_PanamaLimbMissionComplete)
		{
			Globals.m_PrimaryObjective = m_LimbDoor;
		}
		if (Globals.m_PanamaLimbMissionComplete && !Globals.m_PanamaVisitedHavok)
		{
			Globals.m_PrimaryObjective = m_BoardwalkAccess;
			m_LeaveLimbTrigger.active = true;
		}
		else
		{
			m_LeaveLimbTrigger.active = false;
		}
		if (Globals.m_PanamaVisitedHavok && !Globals.m_PanamaVIPFound)
		{
			Globals.m_PrimaryObjective = m_HighRiseDoor;
		}
		if (Globals.m_PanamaVIPFound && !Globals.m_PanamaVIPMissionComplete)
		{
			Globals.m_PrimaryObjective = m_BoardwalkAccess;
		}
		if (Globals.m_PanamaVTOLMissionStarted && !Globals.m_PanamaVTOLMissionComplete)
		{
			Globals.m_PrimaryObjective = m_CorpDoor;
		}
		if (Globals.m_PanamaVTOLMissionComplete)
		{
			Globals.m_PrimaryObjective = m_HighRiseDoor;
		}
	}
}
