using System;

[Serializable]
public class Augmentation_HackingFortify : AugmentationContainer
{
	public enum HackingFortifyAugs
	{
		Fortify1 = 0,
		Fortify2 = 1,
		Fortify3 = 2,
		Total = 3
	}

	public HackingFortifyData[] m_UpgradeData;

	public override AugData[] GetAugData()
	{
		return m_UpgradeData;
	}

	public override AugData GetAugData(int idx)
	{
		return m_UpgradeData[idx];
	}

	public bool IsFortifyActive()
	{
		if (m_UpgradeData[2].m_Purchased)
		{
			return m_UpgradeData[2].m_FortifyActive;
		}
		if (m_UpgradeData[1].m_Purchased)
		{
			return m_UpgradeData[1].m_FortifyActive;
		}
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_FortifyActive;
		}
		return false;
	}

	public int GetFortifyIncreaseValue()
	{
		if (m_UpgradeData[2].m_Purchased)
		{
			return m_UpgradeData[2].m_FortifyIncreaseValue;
		}
		if (m_UpgradeData[1].m_Purchased)
		{
			return m_UpgradeData[1].m_FortifyIncreaseValue;
		}
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_FortifyIncreaseValue;
		}
		return 0;
	}

	public int GetFortifyProgramRating()
	{
		if (m_UpgradeData[2].m_Purchased)
		{
			return 3;
		}
		if (m_UpgradeData[1].m_Purchased)
		{
			return 2;
		}
		if (m_UpgradeData[0].m_Purchased)
		{
			return 1;
		}
		return 0;
	}
}
