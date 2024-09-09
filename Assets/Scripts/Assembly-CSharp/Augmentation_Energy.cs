using System;

[Serializable]
public class Augmentation_Energy : AugmentationContainer
{
	private enum EnergyAugs
	{
		Energy1 = 0,
		Energy2 = 1
	}

	public EnergyExpansionData[] m_UpgradeData;

	public override AugData[] GetAugData()
	{
		return m_UpgradeData;
	}

	public override AugData GetAugData(int idx)
	{
		return m_UpgradeData[idx];
	}

	public int GetEnergyContainerAmount()
	{
		if (m_UpgradeData[1].m_Purchased)
		{
			return m_UpgradeData[1].m_EnergyContainerAmount;
		}
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_EnergyContainerAmount;
		}
		return 1;
	}
}
