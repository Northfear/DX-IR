using Fabric;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
	public static InfoPanel m_This;

	public UIPanel m_Panel;

	private bool m_JustOpened;

	private StoreMenu.StoreItem m_Item = StoreMenu.StoreItem.None;

	private bool m_ItemIsCredits;

	public SpriteText m_WindowTitle;

	public SpriteText m_WindowCost;

	public SpriteText m_WindowCredits;

	public SimpleSprite m_WindowIcon;

	public SpriteText m_OfficialName;

	public SpriteText m_OfficialDesc;

	public GameObject m_SpecGroup;

	public PackedSprite[] m_FireRateMarks = new PackedSprite[10];

	public PackedSprite[] m_ReloadMarks = new PackedSprite[10];

	public PackedSprite[] m_DamageMarks = new PackedSprite[10];

	public PackedSprite[] m_AmmoMarks = new PackedSprite[10];

	private Color m_MarkOffColor = new Color(0.15f, 0.15f, 0.15f, 1f);

	private Color m_MarkOnColor = new Color(0.93f, 0.65f, 0.137f, 1f);

	private void Awake()
	{
		m_This = this;
	}

	public static void OpenInfoPanel(StoreMenu.StoreItem item, string shortName, string cost, Material itemMat, string fullName, string desc)
	{
		EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null);
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null);
		if (m_This == null)
		{
			return;
		}
		m_This.m_Item = item;
		m_This.m_ItemIsCredits = item == StoreMenu.StoreItem.Credits500 || item == StoreMenu.StoreItem.Credits2500 || item == StoreMenu.StoreItem.Credits10000 || item == StoreMenu.StoreItem.Credits50000;
		m_This.m_WindowTitle.Text = shortName;
		m_This.m_WindowCost.Text = cost;
		if (m_This.m_ItemIsCredits)
		{
			m_This.m_WindowCredits.Hide(false);
			m_This.m_WindowIcon.Hide(true);
			switch (item)
			{
			case StoreMenu.StoreItem.Credits500:
				m_This.m_WindowCredits.Text = "[#EDA723]500";
				break;
			case StoreMenu.StoreItem.Credits2500:
				m_This.m_WindowCredits.Text = "[#EDA723]2,500";
				break;
			case StoreMenu.StoreItem.Credits10000:
				m_This.m_WindowCredits.Text = "[#EDA723]10,000";
				break;
			default:
				m_This.m_WindowCredits.Text = "[#EDA723]50,000";
				break;
			}
		}
		else
		{
			m_This.m_WindowIcon.Hide(false);
			m_This.m_WindowCredits.Hide(true);
			m_This.m_WindowIcon.renderer.material = itemMat;
		}
		m_This.m_OfficialName.Text = fullName;
		m_This.m_OfficialDesc.Text = desc;
		if (item == StoreMenu.StoreItem.CombatRifle || item == StoreMenu.StoreItem.Crossbow || item == StoreMenu.StoreItem.Grenade)
		{
			m_This.m_SpecGroup.SetActiveRecursively(true);
			int num;
			switch (item)
			{
			case StoreMenu.StoreItem.CombatRifle:
				num = 8;
				break;
			case StoreMenu.StoreItem.Crossbow:
				num = 1;
				break;
			default:
				num = 1;
				break;
			}
			int num2 = num;
			int num3;
			switch (item)
			{
			case StoreMenu.StoreItem.CombatRifle:
				num3 = 4;
				break;
			case StoreMenu.StoreItem.Crossbow:
				num3 = 5;
				break;
			default:
				num3 = 2;
				break;
			}
			int num4 = num3;
			int num5;
			switch (item)
			{
			case StoreMenu.StoreItem.CombatRifle:
				num5 = 5;
				break;
			case StoreMenu.StoreItem.Crossbow:
				num5 = 9;
				break;
			default:
				num5 = 10;
				break;
			}
			int num6 = num5;
			int num7;
			switch (item)
			{
			case StoreMenu.StoreItem.CombatRifle:
				num7 = 7;
				break;
			case StoreMenu.StoreItem.Crossbow:
				num7 = 1;
				break;
			default:
				num7 = 1;
				break;
			}
			int num8 = num7;
			for (int i = 0; i < 10; i++)
			{
				m_This.m_FireRateMarks[i].SetColor((i >= num2) ? m_This.m_MarkOffColor : m_This.m_MarkOnColor);
				m_This.m_ReloadMarks[i].SetColor((i >= num4) ? m_This.m_MarkOffColor : m_This.m_MarkOnColor);
				m_This.m_DamageMarks[i].SetColor((i >= num6) ? m_This.m_MarkOffColor : m_This.m_MarkOnColor);
				m_This.m_AmmoMarks[i].SetColor((i >= num8) ? m_This.m_MarkOffColor : m_This.m_MarkOnColor);
			}
		}
		else
		{
			m_This.m_SpecGroup.SetActiveRecursively(false);
		}
		Globals.m_HUDRoot.m_PanelManager.BringIn("InfoPanel", UIPanelManager.MENU_DIRECTION.Forwards);
		m_This.m_JustOpened = true;
	}

	private void LateUpdate()
	{
		if (m_JustOpened)
		{
			if (m_Item != StoreMenu.StoreItem.CombatRifle && m_Item != StoreMenu.StoreItem.Crossbow && m_Item != StoreMenu.StoreItem.Grenade)
			{
				m_SpecGroup.SetActiveRecursively(false);
			}
			m_JustOpened = false;
		}
	}

	public void BuyPressed()
	{
		Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
		StoreMenu.OpenPopUpToBuy(m_Item);
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
		EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null, base.gameObject);
	}

	public void ExitPressed()
	{
		Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
		Globals.m_HUDRoot.m_PanelManager.BringIn("PremiumMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		StoreMenu.StoreOpening(m_ItemIsCredits ? StoreMenu.StorePanel.Booster : StoreMenu.StorePanel.Items);
		EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null, base.gameObject);
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
	}
}
