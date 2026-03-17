using System;

[Serializable]
public class Augmentation_InfoLink : AugmentationContainer
{
	public enum InfoLinkAugs
	{
		Link = 0,
		Total = 1
	}

	public InfoLinkData[] m_UpgradeData;

	public override AugData[] GetAugData()
	{
		return m_UpgradeData;
	}

	public override AugData GetAugData(int idx)
	{
		return m_UpgradeData[idx];
	}
}
