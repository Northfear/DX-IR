using System;

[Serializable]
public class Augmentation_HackingStealth : AugmentationContainer
{
	public enum HackingStealthAugs
	{
		Stealth1 = 0,
		Stealth2 = 1,
		Stealth3 = 2,
		Total = 3
	}

	public HackingStealthData[] m_UpgradeData;

	public override AugData[] GetAugData()
	{
		return m_UpgradeData;
	}

	public override AugData GetAugData(int idx)
	{
		return m_UpgradeData[idx];
	}

	public int GetStealthProgramRating()
	{
		if (m_UpgradeData[2].m_Purchased)
		{
			return m_UpgradeData[2].m_StealthProgramRating;
		}
		if (m_UpgradeData[1].m_Purchased)
		{
			return m_UpgradeData[1].m_StealthProgramRating;
		}
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_StealthProgramRating;
		}
		return 0;
	}
}
