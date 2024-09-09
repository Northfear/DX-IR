using System;
using UnityEngine;

[Serializable]
public class AugmentationContainer
{
	public string m_Name = "Default";

	public Texture m_IconTexture;

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
