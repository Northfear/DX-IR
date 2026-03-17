using System;

[Serializable]
public class AugmentationContainer
{
	public string m_Name = "Default";

	public AugMenu.AugLocation m_Location;

	public bool m_Passive;

	public virtual AugData[] GetAugData()
	{
		return null;
	}

	public virtual AugData GetAugData(int idx)
	{
		return null;
	}
}
