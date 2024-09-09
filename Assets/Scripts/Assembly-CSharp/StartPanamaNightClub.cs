using UnityEngine;

public class StartPanamaNightClub : MonoBehaviour
{
	public Transform m_Exit;

	public Transform m_Kaspar;

	public GameObject m_KasparIntroConversation;

	public Transform m_KasparFinal;

	public GameObject m_KasparFinalConversation;

	private void Start()
	{
		if (!Globals.m_PanamaHubStarted)
		{
			Debug.Log("Progression Cheat: Advance to Havok meeting.");
			Globals.m_PanamaHubStarted = true;
			Globals.m_PanamaVisitedDoctor = true;
			Globals.m_PanamaDoctorMissionComplete = true;
			Globals.m_PanamaVisitedLimb = true;
			Globals.m_PanamaVisitedCobra = true;
			Globals.m_PanamaDestroyedRiezol = true;
			Globals.m_PanamaLimbMissionComplete = true;
			Globals.m_PanamaVisitedHavok = true;
			Globals.m_PanamaVIPFound = true;
		}
		if (Globals.m_PanamaLimbMissionComplete && !Globals.m_PanamaVisitedHavok)
		{
			Globals.m_PrimaryObjective = m_Kaspar;
			m_KasparIntroConversation.SetActiveRecursively(true);
		}
		else if (Globals.m_PanamaVIPFound && !Globals.m_PanamaVIPMissionComplete)
		{
			Globals.m_PrimaryObjective = m_KasparFinal;
			m_KasparFinalConversation.SetActiveRecursively(true);
		}
		else
		{
			Globals.m_PrimaryObjective = m_Exit;
		}
	}

	private void KasparInvestigate()
	{
		Globals.m_PanamaVisitedHavok = true;
		Globals.m_PrimaryObjective = m_Exit;
	}

	private void AcceptBelltowerMission()
	{
		Globals.m_PanamaVIPMissionComplete = true;
		Globals.m_PanamaVTOLMissionStarted = true;
		Globals.m_PrimaryObjective = m_Exit;
	}
}
