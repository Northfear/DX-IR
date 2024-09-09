using UnityEngine;

public class StartPanamaSlums : MonoBehaviour
{
	public Transform m_DoctorDoor;

	public Transform m_BoardwalkAccess;

	public GameObject m_LeaveDocTrigger;

	public Transform m_DrugDenDoor;

	private void Start()
	{
		if (!Globals.m_PanamaVisitedDoctor)
		{
			Globals.m_PrimaryObjective = m_DoctorDoor;
		}
		else
		{
			if (!Globals.m_PanamaVisitedLimb)
			{
				Globals.m_PrimaryObjective = m_BoardwalkAccess;
			}
			if (Globals.m_PanamaVisitedCobra && !Globals.m_PanamaDestroyedRiezol)
			{
				Globals.m_PrimaryObjective = m_DrugDenDoor;
			}
			if (Globals.m_PanamaDestroyedRiezol && !Globals.m_PanamaLimbMissionComplete)
			{
				Globals.m_PrimaryObjective = m_BoardwalkAccess;
			}
		}
		if (Globals.m_PanamaVisitedDoctor && !Globals.m_PanamaVisitedLimb)
		{
			m_LeaveDocTrigger.SetActiveRecursively(true);
		}
		else
		{
			Object.Destroy(m_LeaveDocTrigger);
		}
	}
}
