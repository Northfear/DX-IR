using UnityEngine;

public class Item_Base
{
	public bool m_Valid;

	public string m_TypeName;

	public string m_Name;

	public string m_Description;

	public int m_CategoryID;

	public int m_ItemID;

	public string m_TexturePath;

	public Texture m_Texture;

	public bool m_Browsable;

	public bool m_Assignable;

	public int m_Cost;

	public int m_Quantity;

	public int m_MaxQuantity;

	public int m_PurchaseQuantity;

	public Item_Base()
	{
		m_Valid = false;
		m_TypeName = string.Empty;
		m_Browsable = false;
		m_Assignable = false;
		m_Quantity = 0;
		m_MaxQuantity = -1;
		m_PurchaseQuantity = 1;
	}
}
