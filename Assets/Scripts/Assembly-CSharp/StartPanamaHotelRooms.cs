using UnityEngine;

public class StartPanamaHotelRooms : MonoBehaviour
{
	public Transform m_Elevator;

	public Transform m_RooftopAccess;

	public Transform m_VIP;

	public GameObject m_VIPGroup;

	private void Start()
	{
		if (Globals.m_PanamaVisitedHavok && !Globals.m_PanamaVIPFound)
		{
			m_VIPGroup.SetActiveRecursively(true);
			Globals.m_PrimaryObjective = m_VIP;
		}
		else if (Globals.m_PanamaVTOLMissionComplete)
		{
			Globals.m_PrimaryObjective = m_RooftopAccess;
		}
		else
		{
			Globals.m_PrimaryObjective = m_Elevator;
		}
	}

	private void BodyFound()
	{
		Globals.m_PanamaVIPFound = true;
		Globals.m_PrimaryObjective = m_Elevator;
	}
}
