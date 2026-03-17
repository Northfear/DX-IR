using UnityEngine;

public class HostilityZone : MonoBehaviour
{
	public HostilityLevel m_HostilityLevel = HostilityLevel.None;

	public static bool m_ForcedHostile;

	public static int m_TotalHostileLevel;

	public static int m_TotalWarningLevel;

	public static bool IsPlayerInHostileTerritory()
	{
		return m_TotalHostileLevel > 0 || m_ForcedHostile;
	}

	public static bool IsPlayerInWarningTerritory()
	{
		return m_TotalWarningLevel > 0;
	}

	public static HostilityLevel GetPlayerHostilityLevel()
	{
		return (m_TotalHostileLevel > 0 || m_ForcedHostile) ? HostilityLevel.Hostile : ((m_TotalWarningLevel <= 0) ? HostilityLevel.None : HostilityLevel.Warning);
	}

	public static void ClearHostilityLevels()
	{
		m_ForcedHostile = false;
		m_TotalHostileLevel = 0;
		m_TotalWarningLevel = 0;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 14)
		{
			m_ForcedHostile = false;
			if (m_HostilityLevel == HostilityLevel.Hostile)
			{
				m_TotalHostileLevel++;
			}
			else if (m_HostilityLevel == HostilityLevel.Warning)
			{
				m_TotalWarningLevel++;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 14)
		{
			if (m_HostilityLevel == HostilityLevel.Hostile)
			{
				m_TotalHostileLevel = Mathf.Max(m_TotalHostileLevel - 1, 0);
				m_ForcedHostile = m_TotalHostileLevel == 0 && m_TotalWarningLevel == 0;
			}
			else if (m_HostilityLevel == HostilityLevel.Warning)
			{
				m_TotalWarningLevel = Mathf.Max(m_TotalWarningLevel - 1, 0);
				m_ForcedHostile = false;
			}
		}
	}
}
