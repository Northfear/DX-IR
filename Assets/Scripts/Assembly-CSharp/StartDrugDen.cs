using UnityEngine;

public class StartDrugDen : MonoBehaviour
{
	public Transform m_Exit;

	public Transform m_ReizolShipment;

	public GameObject m_RiezolShipmentGroup;

	private void Start()
	{
		if (Globals.m_PanamaVisitedCobra && !Globals.m_PanamaDestroyedRiezol)
		{
			Globals.m_PrimaryObjective = m_ReizolShipment;
			return;
		}
		Globals.m_PrimaryObjective = m_Exit;
		Object.Destroy(m_RiezolShipmentGroup);
	}

	private void BombPlanted()
	{
		Globals.m_PanamaDestroyedRiezol = true;
		Globals.m_PrimaryObjective = m_Exit;
		Object.Destroy(m_RiezolShipmentGroup);
	}
}
