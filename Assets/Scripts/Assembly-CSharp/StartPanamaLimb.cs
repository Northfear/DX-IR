using UnityEngine;

public class StartPanamaLimb : MonoBehaviour
{
	public Transform m_FrontDesk;

	public GameObject m_FrontDeskConversation;

	public Transform m_Exit;

	public Transform m_Camila;

	public GameObject m_CamilaConversation;

	public Transform m_CamilaComplete;

	public GameObject m_CamilaConversationComplete;

	private void Start()
	{
		if (Globals.m_PanamaVisitedDoctor && !Globals.m_PanamaVisitedLimb)
		{
			Globals.m_PrimaryObjective = m_FrontDesk;
			m_CamilaConversation.SetActiveRecursively(false);
		}
		else if (Globals.m_PanamaDestroyedRiezol && !Globals.m_PanamaLimbMissionComplete)
		{
			Object.Destroy(m_FrontDeskConversation);
			m_CamilaConversationComplete.SetActiveRecursively(true);
			Globals.m_PrimaryObjective = m_CamilaComplete;
		}
		else
		{
			Globals.m_PrimaryObjective = m_Exit;
			Object.Destroy(m_FrontDeskConversation);
		}
	}

	private void AccessCamila()
	{
		m_CamilaConversation.SetActiveRecursively(true);
		Globals.m_PrimaryObjective = m_Camila;
	}

	private void AcceptRizeolMission()
	{
		Globals.m_PrimaryObjective = m_Exit;
		Globals.m_PanamaVisitedLimb = true;
	}

	private void RiezolMissionComplete()
	{
		Globals.m_PrimaryObjective = m_FrontDesk;
		Globals.m_PanamaLimbMissionComplete = true;
		Globals.m_PrimaryObjective = m_Exit;
	}
}
