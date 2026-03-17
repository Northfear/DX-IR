using System;

[Serializable]
public class Augmentation_IcarusLanding : AugmentationContainer
{
	public enum IcarusLandingAugs
	{
		Landing = 0,
		Total = 1
	}

	public IcarusLandingData[] m_UpgradeData;

	public override AugData[] GetAugData()
	{
		return m_UpgradeData;
	}

	public override AugData GetAugData(int idx)
	{
		return m_UpgradeData[idx];
	}
}
