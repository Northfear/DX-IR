using Fabric;
using UnityEngine;

public class PopUpPanel : MonoBehaviour
{
	public delegate void OptionChosen(bool choice);

	public static PopUpPanel m_This;

	public UIPanel m_Panel;

	public PackedSprite m_Window;

	public SpriteText m_Message;

	private int m_CostForPurchase;

	private OptionChosen m_Callback;

	private bool m_CompoundedMessage;

	private int m_CreditsToPurchase = 500;

	private bool m_CreditsBeingPurchased;

	private void Awake()
	{
		m_This = this;
	}

	public static void OpenPopUp(string message, int Price, OptionChosen callback = null, bool CreditsPurchase = false)
	{
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null);
		EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null);
		if (!(m_This == null))
		{
			Globals.m_HUDRoot.m_PanelManager.BringIn("PopUpPanel", UIPanelManager.MENU_DIRECTION.Forwards);
			if ((bool)m_This.m_Window)
			{
				AnimateScale.Do(m_This.m_Window.gameObject, EZAnimation.ANIM_MODE.FromTo, new Vector3(0.7f, 0.7f, 0.7f), new Vector3(1f, 1f, 1f), EZAnimation.spring, 0.4f, 0f, null, null);
			}
			if ((bool)m_This.m_Message)
			{
				m_This.m_Message.Text = message;
			}
			m_This.m_CostForPurchase = Price;
			m_This.m_Callback = callback;
			m_This.m_CompoundedMessage = false;
			m_This.m_CreditsBeingPurchased = CreditsPurchase;
		}
	}

	public static void OpenCompoundedPopUp(string message)
	{
		if (!(m_This == null))
		{
			Globals.m_HUDRoot.m_PanelManager.BringIn("PopUpPanel", UIPanelManager.MENU_DIRECTION.Forwards);
			if ((bool)m_This.m_Window)
			{
				AnimateScale.Do(m_This.m_Window.gameObject, EZAnimation.ANIM_MODE.FromTo, new Vector3(0.7f, 0.7f, 0.7f), new Vector3(1f, 1f, 1f), EZAnimation.spring, 0.4f, 0f, null, null);
			}
			if ((bool)m_This.m_Message)
			{
				m_This.m_Message.Text = message;
			}
			m_This.m_CompoundedMessage = true;
		}
	}

	public void NoPressed()
	{
		EventManager.Instance.PostEvent("UI_Select", EventAction.PlaySound, null, base.gameObject);
		EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null, base.gameObject);
		Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
		Globals.m_HUDRoot.m_PanelManager.BringIn("PremiumMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		StoreMenu.StoreOpening(m_CreditsBeingPurchased ? StoreMenu.StorePanel.Booster : StoreMenu.StorePanel.Items);
		if (m_Callback != null)
		{
			m_Callback(false);
		}
	}

	public void YesPressed()
	{
		if (m_CompoundedMessage)
		{
			Globals.m_Inventory.m_Credits += m_CreditsToPurchase;
			Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
			Globals.m_HUDRoot.m_PanelManager.BringIn("PremiumMenu", UIPanelManager.MENU_DIRECTION.Forwards);
			StoreMenu.StoreOpening(StoreMenu.StorePanel.Items);
			PauseTabs.UpdateCreditsValue();
			if (m_Callback != null)
			{
				m_Callback(true);
			}
			EventManager.Instance.PostEvent("UI_Transaction", EventAction.PlaySound, null, base.gameObject);
			EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null);
			return;
		}
		if (m_CostForPurchase <= Globals.m_Inventory.m_Credits || m_CreditsBeingPurchased)
		{
			Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
			Globals.m_HUDRoot.m_PanelManager.BringIn("PremiumMenu", UIPanelManager.MENU_DIRECTION.Forwards);
			StoreMenu.StoreOpening(m_CreditsBeingPurchased ? StoreMenu.StorePanel.Booster : StoreMenu.StorePanel.Items);
			if (m_Callback != null)
			{
				m_Callback(true);
			}
			EventManager.Instance.PostEvent("UI_Transaction", EventAction.PlaySound, null, base.gameObject);
			EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null, base.gameObject);
			return;
		}
		Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
		int num = m_CostForPurchase - Globals.m_Inventory.m_Credits;
		string text;
		if (num <= 500)
		{
			text = "$0.99";
			m_CreditsToPurchase = 500;
		}
		else if (num <= 2500)
		{
			text = "$1.99";
			m_CreditsToPurchase = 2500;
		}
		else if (num <= 10000)
		{
			text = "$2.99";
			m_CreditsToPurchase = 10000;
		}
		else
		{
			text = "$4.99";
			m_CreditsToPurchase = 50000;
		}
		string text2 = "[#FFFFFF]You don't have enough credits!\n";
		string text3 = text2;
		text2 = text3 + "[#FFFFFF]Would you like to purchase [#00E4B8]" + m_CreditsToPurchase + "V [#FFFFFF]for [#EDA723]" + text + "[#FFFFFF] to complete the transaction?";
		OpenCompoundedPopUp(text2);
		EventManager.Instance.PostEvent("UI_Error", EventAction.PlaySound, null, base.gameObject);
		EventManager.Instance.PostEvent("UI_Window", EventAction.PlaySound, null);
	}
}
