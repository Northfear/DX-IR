using System;

[Serializable]
public class Augmentation_SocialEnhancer : AugmentationContainer
{
	private enum SocialAugs
	{
		EmotionalEnhancer = 0
	}

	public SocialEnhancerData[] m_UpgradeData;

	public override AugData[] GetAugData()
	{
		return m_UpgradeData;
	}

	public override AugData GetAugData(int idx)
	{
		return m_UpgradeData[idx];
	}

	public bool IsSocialEnhancerActive()
	{
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_SocialEnhancerActive;
		}
		return false;
	}
}
