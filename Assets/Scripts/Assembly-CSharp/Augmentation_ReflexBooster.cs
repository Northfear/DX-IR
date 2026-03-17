using System;

[Serializable]
public class Augmentation_ReflexBooster : AugmentationContainer
{
	public enum ReflexBoosterAugs
	{
		MultipleTakedown = 0,
		Total = 1
	}

	public ReflexBoosterData[] m_UpgradeData;

	public override AugData[] GetAugData()
	{
		return m_UpgradeData;
	}

	public override AugData GetAugData(int idx)
	{
		return m_UpgradeData[idx];
	}

	public bool IsMultipleTakedownActive()
	{
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_MultipleTakedownActive;
		}
		return false;
	}
}
