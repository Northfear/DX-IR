using UnityEngine;

public class Inventory : MonoBehaviour
{
	public int m_Credits = 500;

	public int m_PraxisExp;

	public int m_PraxisNeededToLevel = 4000;

	public int m_TotalExp;

	public int m_PraxisKits;

	public int m_EnergyBars;

	public int m_Booze;

	public int[] m_Grenades = new int[3];

	public int m_AutoHacks;

	public int m_Nukes;

	public int m_StopWorms;

	[HideInInspector]
	public ItemType m_CurrentItem;

	private void Awake()
	{
		Globals.m_Inventory = this;
		m_Grenades = new int[3];
		m_Grenades[0] = 2;
		m_Grenades[1] = 2;
		m_Grenades[2] = 2;
	}
}
