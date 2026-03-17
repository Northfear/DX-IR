using System;

[Serializable]
public class Augmentation_Armor : AugmentationContainer
{
	public enum ArmorAugs
	{
		ReductionAmount1 = 0,
		ReductionAmount2 = 1,
		ReductionAmount3 = 2,
		Total = 3
	}

	public ArmorData[] m_UpgradeData;

	public override AugData[] GetAugData()
	{
		return m_UpgradeData;
	}

	public override AugData GetAugData(int idx)
	{
		return m_UpgradeData[idx];
	}

	public bool IsDamageReductionPurchased()
	{
		if (m_UpgradeData[0].m_Purchased || m_UpgradeData[1].m_Purchased || m_UpgradeData[2].m_Purchased)
		{
			return true;
		}
		return false;
	}

	public float DamageReductionAmount()
	{
		if (m_UpgradeData[2].m_Purchased)
		{
			return m_UpgradeData[2].m_DamageReductionPercentAmount;
		}
		if (m_UpgradeData[1].m_Purchased)
		{
			return m_UpgradeData[1].m_DamageReductionPercentAmount;
		}
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_DamageReductionPercentAmount;
		}
		return 0f;
	}
}
