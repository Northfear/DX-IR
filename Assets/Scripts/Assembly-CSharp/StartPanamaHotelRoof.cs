using UnityEngine;

public class StartPanamaHotelRoof : MonoBehaviour
{
	public Transform m_Exit;

	public Transform m_VTOLAccess;

	public GameObject m_VTOLGroup;

	private void Start()
	{
		if (Globals.m_PanamaVTOLMissionComplete)
		{
			m_VTOLGroup.SetActiveRecursively(true);
			Globals.m_PrimaryObjective = m_VTOLAccess;
		}
		else
		{
			m_VTOLGroup.SetActiveRecursively(false);
			Globals.m_PrimaryObjective = m_Exit;
		}
	}
}
