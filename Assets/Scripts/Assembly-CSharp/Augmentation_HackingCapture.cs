using System;

[Serializable]
public class Augmentation_HackingCapture : AugmentationContainer
{
	public enum HackingAugs
	{
		Capture1 = 0,
		Capture2 = 1,
		Capture3 = 2,
		Capture4 = 3,
		Capture5 = 4,
		CameraDomination = 5,
		RobotDomination = 6,
		Total = 7
	}

	public HackingCaptureData[] m_UpgradeData;

	public override AugData[] GetAugData()
	{
		return m_UpgradeData;
	}

	public override AugData GetAugData(int idx)
	{
		return m_UpgradeData[idx];
	}

	public int GetCaptureProgramRating()
	{
		if (m_UpgradeData[4].m_Purchased)
		{
			return m_UpgradeData[4].m_CaptureProgramRating;
		}
		if (m_UpgradeData[3].m_Purchased)
		{
			return m_UpgradeData[3].m_CaptureProgramRating;
		}
		if (m_UpgradeData[2].m_Purchased)
		{
			return m_UpgradeData[2].m_CaptureProgramRating;
		}
		if (m_UpgradeData[1].m_Purchased)
		{
			return m_UpgradeData[1].m_CaptureProgramRating;
		}
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_CaptureProgramRating;
		}
		return 0;
	}

	public bool IsDoorDominationActive()
	{
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_DoorDominationActive;
		}
		return false;
	}

	public bool IsCameraDominationActive()
	{
		if (m_UpgradeData[5].m_Purchased)
		{
			return m_UpgradeData[5].m_CameraDominationActive;
		}
		return false;
	}

	public bool IsRobotDominationActive()
	{
		if (m_UpgradeData[6].m_Purchased)
		{
			return m_UpgradeData[6].m_RobotDominationActive;
		}
		return false;
	}

	public bool IsTurretDominationActive()
	{
		if (m_UpgradeData[6].m_Purchased)
		{
			return m_UpgradeData[6].m_TurretDominationActive;
		}
		return false;
	}
}
