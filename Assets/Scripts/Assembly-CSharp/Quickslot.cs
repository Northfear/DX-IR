public class Quickslot
{
	public int m_CategoryID;

	public int m_ItemID;

	public bool IsValid()
	{
		if (m_CategoryID < 0 || m_CategoryID >= 6 || m_ItemID < 0)
		{
			return false;
		}
		if (m_ItemID >= Globals.m_Inventory.m_Items[m_CategoryID].Length)
		{
			return false;
		}
		return true;
	}
}
