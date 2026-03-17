using System;

[Serializable]
public class Augmentation_Cloaking : AugmentationContainer
{
	public enum CloakingAugs
	{
		CloakingDuration1 = 0,
		CloakingDuration2 = 1,
		CloakingDuration3 = 2,
		Total = 3
	}

	public CloakingData[] m_UpgradeData;

	public override AugData[] GetAugData()
	{
		return m_UpgradeData;
	}

	public override AugData GetAugData(int idx)
	{
		return m_UpgradeData[idx];
	}

	public float GetDurationPerEnergyCell()
	{
		if (m_UpgradeData[2].m_Purchased)
		{
			return m_UpgradeData[2].m_DurationPerEnergyCell;
		}
		if (m_UpgradeData[1].m_Purchased)
		{
			return m_UpgradeData[1].m_DurationPerEnergyCell;
		}
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_DurationPerEnergyCell;
		}
		return 3f;
	}
}
