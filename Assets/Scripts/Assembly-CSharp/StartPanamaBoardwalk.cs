using UnityEngine;

public class StartPanamaBoardwalk : MonoBehaviour
{
	public Transform m_SlumsAccess;

	public Transform m_DowntownAccess;

	public Transform m_TrainAccess;

	public Transform m_NightclubAccess;

	private void Start()
	{
		if (!Globals.m_PanamaVisitedDoctor)
		{
			Globals.m_PrimaryObjective = m_SlumsAccess;
			return;
		}
		if (!Globals.m_PanamaVisitedLimb)
		{
			Globals.m_PrimaryObjective = m_DowntownAccess;
		}
		if (Globals.m_PanamaVisitedLimb && !Globals.m_PanamaVisitedCobra)
		{
			Globals.m_PrimaryObjective = m_TrainAccess;
		}
		if (Globals.m_PanamaVisitedCobra && !Globals.m_PanamaDestroyedRiezol)
		{
			Globals.m_PrimaryObjective = m_SlumsAccess;
		}
		if (Globals.m_PanamaDestroyedRiezol && !Globals.m_PanamaLimbMissionComplete)
		{
			Globals.m_PrimaryObjective = m_DowntownAccess;
		}
		if (Globals.m_PanamaLimbMissionComplete && !Globals.m_PanamaVisitedHavok)
		{
			Globals.m_PrimaryObjective = m_NightclubAccess;
		}
		if (Globals.m_PanamaVisitedHavok && !Globals.m_PanamaVIPFound)
		{
			Globals.m_PrimaryObjective = m_DowntownAccess;
		}
		if (Globals.m_PanamaVIPFound && !Globals.m_PanamaVIPMissionComplete)
		{
			Globals.m_PrimaryObjective = m_NightclubAccess;
		}
		if (Globals.m_PanamaVTOLMissionStarted && !Globals.m_PanamaVTOLMissionComplete)
		{
			Globals.m_PrimaryObjective = m_DowntownAccess;
		}
	}
}
