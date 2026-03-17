using UnityEngine;

public class StartPanamaBelltower : MonoBehaviour
{
	public Transform m_HelipadAccess;

	public Transform m_ElevatorExit;

	public GameObject m_ElevatorTrigger;

	private void Start()
	{
		if (!Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaHubStarted))
		{
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaHubStarted, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedDoctor, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDoctorMissionComplete, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedLimb, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedCobra, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaDestroyedRiezol, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaLimbMissionComplete, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVisitedHavok, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVIPFound, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVIPMissionComplete, true);
			Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVTOLMissionStarted, true);
		}
		if (Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVTOLMissionStarted) && !Globals.GetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVTOLMissionComplete))
		{
			Globals.m_PrimaryObjective = m_HelipadAccess;
			m_ElevatorTrigger.active = false;
		}
		else
		{
			Globals.m_PrimaryObjective = m_ElevatorExit;
		}
	}

	private void HackSecurity()
	{
		Globals.SetStoryProgressionVar(Globals.StoryProgressionVars.PanamaVTOLMissionComplete, true);
		Globals.m_PrimaryObjective = m_ElevatorExit;
		m_ElevatorTrigger.active = true;
	}
}
