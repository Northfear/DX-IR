using UnityEngine;

public class StartPanamaHotelLobby : MonoBehaviour
{
	public Transform m_Exit;

	public Transform m_Elevator;

	private void Start()
	{
		if (Globals.m_PanamaVisitedHavok && !Globals.m_PanamaVIPFound)
		{
			Globals.m_PrimaryObjective = m_Elevator;
		}
		else if (Globals.m_PanamaVTOLMissionComplete)
		{
			Globals.m_PrimaryObjective = m_Elevator;
		}
		else
		{
			Globals.m_PrimaryObjective = m_Exit;
		}
	}
}
