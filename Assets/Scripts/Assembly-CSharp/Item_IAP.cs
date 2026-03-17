using UnityEngine;

public class Item_IAP
{
	public bool m_Valid;

	public int m_MinSupportedVersion;

	public int m_MaxSupportedVersion;

	public string m_TypeName;

	public string m_Name;

	public string m_Description;

	public int m_AppStoreID;

	public string m_TexturePath;

	public Texture m_Texture;

	public Purchasable[] m_Purchasables;
}
