using UnityEngine;

public class StartPanamaTrainStation : MonoBehaviour
{
	public Transform m_StationExit;

	public GameObject m_StartupTrigger;

	public Transform m_Cobra;

	public GameObject m_CobraConversation;

	public GameObject m_EnemyGroup;

	private void Start()
	{
		if (!Globals.m_PanamaHubStarted)
		{
			Globals.m_PanamaHubStarted = true;
			Globals.m_PrimaryObjective = m_StationExit;
			return;
		}
		Object.Destroy(m_StartupTrigger);
		if (Globals.m_PanamaVisitedLimb && !Globals.m_PanamaVisitedCobra)
		{
			Globals.m_PrimaryObjective = m_Cobra;
			m_CobraConversation.SetActiveRecursively(true);
		}
		else
		{
			Globals.m_PrimaryObjective = m_StationExit;
		}
	}

	private void CobraDealDone()
	{
		Globals.m_PanamaVisitedCobra = true;
		Globals.m_PrimaryObjective = m_StationExit;
		if (m_EnemyGroup != null)
		{
			m_EnemyGroup.SetActiveRecursively(true);
		}
	}
}
