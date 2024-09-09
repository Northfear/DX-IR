using System;

[Serializable]
public class Augmentation_Strength : AugmentationContainer
{
	private enum StrengthAugs
	{
		MoveHeavyObject = 0
	}

	public StrengthData[] m_UpgradeData;

	public override AugData[] GetAugData()
	{
		return m_UpgradeData;
	}

	public override AugData GetAugData(int idx)
	{
		return m_UpgradeData[idx];
	}

	public bool GetCanMoveHeavyObject()
	{
		if (m_UpgradeData[0].m_Purchased)
		{
			return m_UpgradeData[0].m_CanMoveHeavyObject;
		}
		return false;
	}
}
