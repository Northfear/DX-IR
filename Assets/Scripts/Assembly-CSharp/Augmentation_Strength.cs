using System;

[Serializable]
public class Augmentation_Strength : AugmentationContainer
{
	public enum StrengthAugs
	{
		MoveHeavyObject = 0,
		BreakThroughWall = 1,
		Total = 2
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

	public bool GetCanBreakThroughWall()
	{
		if (m_UpgradeData[1].m_Purchased)
		{
			return m_UpgradeData[1].m_CanBreakThroughWall;
		}
		return false;
	}
}
