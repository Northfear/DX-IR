using System;

[Serializable]
public class Augmentation_SmartVision : AugmentationContainer
{
	public enum SmartVisionAugs
	{
		SeeThroughWalls = 0,
		Total = 1
	}

	public SmartVisionData[] m_UpgradeData;

	public override AugData[] GetAugData()
	{
		return m_UpgradeData;
	}

	public override AugData GetAugData(int idx)
	{
		return m_UpgradeData[idx];
	}

	public bool IsSeeThroughWallsActive()
	{
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_SeeThroughWallsActive;
		}
		return false;
	}

	public float GetDurationPerEnergyCell()
	{
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_DurationPerEnergyCell;
		}
		return 3f;
	}
}
