using System;

[Serializable]
public class AugData
{
	public int[] m_Parents;

	public string m_Name = "Default";

	public string m_ShortDescription = string.Empty;

	public string m_Description = string.Empty;

	public int m_Cost;

	public bool m_Purchased;

	public virtual void Purchase()
	{
		m_Purchased = true;
	}
}
