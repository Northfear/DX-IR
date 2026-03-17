using System;

[Serializable]
public class Augmentation_Energy : AugmentationContainer
{
	public enum EnergyAugs
	{
		Energy1 = 0,
		Energy2 = 1,
		Energy3 = 2,
		RechargeRate1 = 3,
		RechargeRate2 = 4,
		RechargeCapacityUpdate = 5,
		Total = 6
	}

	public EnergyData[] m_UpgradeData;

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
		if (m_UpgradeData[2].m_Purchased)
		{
			return m_UpgradeData[2].m_EnergyContainerAmount;
		}
		if (m_UpgradeData[1].m_Purchased)
		{
			return m_UpgradeData[1].m_EnergyContainerAmount;
		}
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_EnergyContainerAmount;
		}
		return 2;
	}

	public float GetEnergyRechargeRate()
	{
		if (m_UpgradeData[4].m_Purchased)
		{
			return m_UpgradeData[4].m_EnergyRechargeRate;
		}
		if (m_UpgradeData[3].m_Purchased)
		{
			return m_UpgradeData[3].m_EnergyRechargeRate;
		}
		return 0.05f;
	}

	public int GetRechargeCapacity()
	{
		if (m_UpgradeData[5].m_Purchased)
		{
			return m_UpgradeData[5].m_RechargeCapacity;
		}
		return 1;
	}
}
