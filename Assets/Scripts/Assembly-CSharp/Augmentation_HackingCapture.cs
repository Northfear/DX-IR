using System;

[Serializable]
public class Augmentation_HackingCapture : AugmentationContainer
{
	private enum HackingAugs
	{
		Capture1 = 0,
		Capture2 = 1,
		Capture3 = 2,
		Capture4 = 3,
		Capture5 = 4
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

	public int GetStealthProgramRating()
	{
		return 0;
	}
}
