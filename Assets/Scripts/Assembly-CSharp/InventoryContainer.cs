using UnityEngine;

public class InventoryContainer : MonoBehaviour
{
	public Transform[] m_ItemAttachments;

	[HideInInspector]
	public InventoryItem[] m_InventoryItems;

	private void Awake()
	{
		if (m_ItemAttachments != null && m_ItemAttachments.Length > 0)
		{
			m_InventoryItems = new InventoryItem[m_ItemAttachments.Length];
		}
	}
}
