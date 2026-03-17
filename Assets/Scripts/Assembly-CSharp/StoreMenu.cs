using UnityEngine;

public class StoreMenu : MonoBehaviour
{
	public enum StorePanel
	{
		None = -1,
		Premium = 0,
		Booster = 1,
		Items = 2,
		Total = 3
	}

	public enum StoreItem
	{
		None = -1,
		CmbtAmmo = 0,
		CrbwAmmo = 1,
		Grenade = 2,
		PraxisKit = 3,
		EnergyBar = 4,
		Booze = 5,
		CombatRifle = 6,
		Crossbow = 7,
		AutoHack = 8,
		Credits500 = 9,
		Credits2500 = 10,
		Credits10000 = 11,
		Credits50000 = 12,
		Total = 13
	}

	public static StoreMenu m_This;

	public UIPanel[] m_StorePanels = new UIPanel[3];

	public UIPanelTab[] m_StoreTabs = new UIPanelTab[3];

	private StorePanel m_PanelToOpen = StorePanel.None;

	public Renderer[] m_IconRenderers = new Renderer[13];

	private void Awake()
	{
		m_This = this;
	}

	public static void StoreOpening(StorePanel startPanel = StorePanel.Items)
	{
		if (!(m_This == null))
		{
			m_This.m_PanelToOpen = startPanel;
			for (int i = 0; i < 3; i++)
			{
				m_This.m_StoreTabs[i].Value = m_This.m_PanelToOpen == (StorePanel)i;
			}
		}
	}

	private void LateUpdate()
	{
		if (m_PanelToOpen != StorePanel.None)
		{
			if (m_This.m_PanelToOpen == StorePanel.Booster)
			{
				m_This.OpenBooster();
			}
			else if (m_This.m_PanelToOpen == StorePanel.Premium)
			{
				m_This.OpenPremium();
			}
			else
			{
				m_This.OpenItems();
			}
			m_PanelToOpen = StorePanel.None;
		}
	}

	public void OpenBooster()
	{
		m_StorePanels[1].gameObject.SetActiveRecursively(true);
		m_StorePanels[0].gameObject.SetActiveRecursively(false);
		m_StorePanels[2].gameObject.SetActiveRecursively(false);
	}

	public void OpenPremium()
	{
		m_StorePanels[1].gameObject.SetActiveRecursively(false);
		m_StorePanels[0].gameObject.SetActiveRecursively(true);
		m_StorePanels[2].gameObject.SetActiveRecursively(false);
	}

	public void OpenItems()
	{
		m_StorePanels[1].gameObject.SetActiveRecursively(false);
		m_StorePanels[0].gameObject.SetActiveRecursively(false);
		m_StorePanels[2].gameObject.SetActiveRecursively(true);
	}
}
