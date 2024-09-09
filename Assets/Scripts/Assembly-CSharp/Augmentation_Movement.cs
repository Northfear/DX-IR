using System;

[Serializable]
public class Augmentation_Movement : AugmentationContainer
{
	private enum MovementAugs
	{
		Movement = 0
	}

	public MovementData[] m_UpgradeData;

	public override AugData[] GetAugData()
	{
		return m_UpgradeData;
	}

	public override AugData GetAugData(int idx)
	{
		return m_UpgradeData[idx];
	}

	public int GetMoveSilentlyNoiseRadius()
	{
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_MoveSilentlyNoiseRadius;
		}
		return 10;
	}
}
