using System;

[Serializable]
public class Augmentation_AimStabilization : AugmentationContainer
{
	private enum AimStabilizationAugs
	{
		MotionControl1 = 0,
		MotionControl2 = 1
	}

	public AimStabilizationData[] m_UpgradeData;

	public override AugData[] GetAugData()
	{
		return m_UpgradeData;
	}

	public override AugData GetAugData(int idx)
	{
		return m_UpgradeData[idx];
	}

	public float GetKickScaler()
	{
		if (m_UpgradeData[1].m_Purchased)
		{
			return m_UpgradeData[1].m_KickScaler;
		}
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_KickScaler;
		}
		return 1f;
	}
}
