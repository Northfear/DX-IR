using UnityEngine;

public class PurchaseConfirmationPanel : MonoBehaviour
{
	public enum ConfirmationState
	{
		None = -1,
		PurchaseProcessing = 0,
		Total = 1
	}

	public delegate void OptionChosen(bool choice);

	public static PurchaseConfirmationPanel m_This;

	private ConfirmationState m_State = ConfirmationState.None;

	private int m_CreditsCategory = -1;

	private int m_CreditsItemID = -1;

	private int m_PurchaseCategory = -1;

	private int m_PurchaseItemID = -1;

	public BTButton m_Window;

	public SpriteText m_Title;

	public SpriteText m_Desc;

	public PackedSprite m_Yes;

	public PackedSprite m_No;

	private Vector3 m_OriginalTogglePosition;

	public UIStateToggleBtn m_ConfirmationToggle;

	private OptionChosen m_Callback;

	private bool m_CreditsPurchased;

	private void Awake()
	{
		m_This = this;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
		m_OriginalTogglePosition = m_ConfirmationToggle.transform.localPosition;
	}

	public static bool OpenStorePurchaseConfirmation(int itemID, OptionChosen callback)
	{
		m_This.m_State = ConfirmationState.None;
		m_This.m_Callback = callback;
		m_This.m_CreditsCategory = -1;
		m_This.m_CreditsItemID = -1;
		m_This.m_PurchaseCategory = 7;
		m_This.m_PurchaseItemID = itemID;
		SoundManager.TriggerEvent("Play_UI_Window");
		Globals.m_HUDRoot.m_PanelManager.BringIn("PurchaseConfirmationPanel", UIPanelManager.MENU_DIRECTION.Forwards);
		if ((bool)m_This.m_Window)
		{
			AnimateScale.Do(m_This.m_Window.gameObject, EZAnimation.ANIM_MODE.FromTo, new Vector3(0.7f, 0.7f, 0.7f), new Vector3(1f, 1f, 1f), EZAnimation.spring, 0.4f, 0f, null, null);
		}
		m_This.m_Yes.gameObject.SetActiveRecursively(true);
		m_This.m_No.gameObject.SetActiveRecursively(true);
		m_This.m_ConfirmationToggle.transform.localPosition = new Vector3(10000f, 10000f, -10000f);
		Item_IAP item_IAP = Globals.m_Inventory.m_IAPs[m_This.m_PurchaseItemID];
		m_This.m_Title.Hide(false);
		m_This.m_Title.Text = "Buy " + LocalizationManager.LocalizeString(item_IAP.m_Name) + "?";
		m_This.m_Desc.Text = "[#D94040]YOUR GAME WILL BE SAVED AFTER THE PURCHASE.";
		SoundManager.TriggerEvent("Play_UI_Select");
		return true;
	}

	public static bool OpenPurchaseConfirmation(int category, int itemID, OptionChosen callback, bool ForceShow = false)
	{
		m_This.m_State = ConfirmationState.None;
		m_This.m_Callback = callback;
		bool flag = m_This.m_CreditsCategory != -1 || ForceShow;
		int num = -1;
		m_This.m_CreditsCategory = -1;
		m_This.m_CreditsItemID = -1;
		m_This.m_PurchaseCategory = category;
		m_This.m_PurchaseItemID = itemID;
		if (Globals.m_Inventory.GetCredits() < Globals.m_Inventory.m_Items[category][itemID].m_Cost)
		{
			if (Globals.m_Inventory.StoreLoading())
			{
				PopUpPanel.OpenPopUp("You do not have enough credits to purchase that item.", MessageButtons.Ok, m_This.NoCreditsCallback);
				return true;
			}
			int num2 = Globals.m_Inventory.m_Items[category][itemID].m_Cost - Globals.m_Inventory.GetCredits();
			for (int i = 0; i < Globals.m_Inventory.m_IAPs.Count; i++)
			{
				Item_IAP item_IAP = Globals.m_Inventory.m_IAPs[i];
				for (int j = 0; j < item_IAP.m_Purchasables.Length; j++)
				{
					if (item_IAP.m_Purchasables[j].m_CategoryID == 5 && item_IAP.m_Purchasables[j].m_ItemID == 0 && item_IAP.m_Purchasables[j].m_Quantity >= num2 && (num < 0 || item_IAP.m_Purchasables[j].m_Quantity < num))
					{
						num = item_IAP.m_Purchasables[j].m_Quantity;
						m_This.m_CreditsItemID = i;
					}
				}
			}
			if (m_This.m_CreditsItemID < 0)
			{
				PopUpPanel.OpenPopUp("You do not have enough credits to purchase that item.", MessageButtons.Ok, m_This.NoCreditsCallback);
				return true;
			}
			m_This.m_CreditsCategory = 7;
		}
		else if (!Globals.m_DisplayPurchaseConfirmation && !flag)
		{
			return false;
		}
		SoundManager.TriggerEvent("Play_UI_Window");
		Globals.m_HUDRoot.m_PanelManager.BringIn("PurchaseConfirmationPanel", UIPanelManager.MENU_DIRECTION.Forwards);
		if ((bool)m_This.m_Window)
		{
			AnimateScale.Do(m_This.m_Window.gameObject, EZAnimation.ANIM_MODE.FromTo, new Vector3(0.7f, 0.7f, 0.7f), new Vector3(1f, 1f, 1f), EZAnimation.spring, 0.4f, 0f, null, null);
		}
		m_This.m_Yes.gameObject.SetActiveRecursively(true);
		m_This.m_No.gameObject.SetActiveRecursively(true);
		if (m_This.m_CreditsCategory == -1)
		{
			m_This.m_ConfirmationToggle.SetToggleState((!Globals.m_DisplayPurchaseConfirmation) ? 1 : 0);
			m_This.m_ConfirmationToggle.transform.localPosition = m_This.m_OriginalTogglePosition;
		}
		else
		{
			m_This.m_ConfirmationToggle.transform.localPosition = new Vector3(10000f, 10000f, -10000f);
		}
		if (m_This.m_CreditsCategory != -1)
		{
			m_This.m_Title.Hide(false);
			m_This.m_Title.Text = "Not enough credits!";
			m_This.m_Desc.Text = "Would you like to buy [#EDA723]" + num + " [#FFFFFF]Credits?  [#D94040]YOUR GAME WILL BE SAVED AFTER THE PURCHASE.";
			SoundManager.TriggerEvent("Play_UI_Error");
		}
		else
		{
			Item_Base item_Base = Globals.m_Inventory.m_Items[m_This.m_PurchaseCategory][m_This.m_PurchaseItemID];
			if (item_Base.m_PurchaseQuantity <= 1)
			{
				m_This.m_Title.Hide(false);
				m_This.m_Title.Text = "Buy " + LocalizationManager.LocalizeString(item_Base.m_TypeName) + "?";
				m_This.m_Desc.Text = "Are you sure you want to buy [#EDA723]" + LocalizationManager.LocalizeString(item_Base.m_Name) + " [#FFFFFF]for [#00FFFF]" + Globals.m_Inventory.GetItemCostAsString(m_This.m_PurchaseCategory, m_This.m_PurchaseItemID) + "[#FFFFFF]?";
			}
			else
			{
				m_This.m_Title.Hide(false);
				m_This.m_Title.Text = "Buy " + item_Base.m_PurchaseQuantity + " x " + LocalizationManager.LocalizeString(item_Base.m_TypeName) + "?";
				m_This.m_Desc.Text = "Are you sure you want to buy [#EDA723]" + item_Base.m_PurchaseQuantity + " x " + LocalizationManager.LocalizeString(item_Base.m_Name) + " [#FFFFFF]for [#00FFFF]" + Globals.m_Inventory.GetItemCostAsString(m_This.m_PurchaseCategory, m_This.m_PurchaseItemID) + "[#FFFFFF]?";
			}
			SoundManager.TriggerEvent("Play_UI_Select");
		}
		return true;
	}

	private void Update()
	{
		if (m_State != ConfirmationState.PurchaseProcessing)
		{
			return;
		}
		if (m_PurchaseCategory == 7)
		{
			Globals.m_Inventory.PurchaseItem(m_PurchaseCategory, m_PurchaseItemID);
			GameManager.GameSaved(true);
			SoundManager.TriggerEvent("Play_UI_Transaction");
			SoundManager.TriggerEvent("Play_UI_Window");
			Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
			if (m_Callback != null)
			{
				m_Callback(true);
			}
			m_State = ConfirmationState.None;
		}
		else
		{
			Globals.m_Inventory.PurchaseItem(m_CreditsCategory, m_CreditsItemID);
			m_CreditsPurchased = true;
			if (!OpenPurchaseConfirmation(m_PurchaseCategory, m_PurchaseItemID, m_Callback, false))
			{
				YesPressed();
			}
			m_State = ConfirmationState.None;
		}
	}

	public void YesPressed()
	{
		if (m_CreditsPurchased)
		{
			GameManager.GameSaved(true);
			m_CreditsPurchased = false;
		}
		if (m_CreditsCategory != -1 || m_PurchaseCategory == 7)
		{
			m_State = ConfirmationState.PurchaseProcessing;
			m_Title.Hide(true);
			m_Desc.Text = "Processing...";
			m_Yes.gameObject.SetActiveRecursively(false);
			m_No.gameObject.SetActiveRecursively(false);
			m_This.m_ConfirmationToggle.transform.localPosition = new Vector3(10000f, 10000f, -10000f);
		}
		else
		{
			SoundManager.TriggerEvent("Play_UI_Transaction");
			SoundManager.TriggerEvent("Play_UI_Window");
			Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
			if (m_Callback != null)
			{
				m_Callback(true);
			}
		}
	}

	public void NoPressed()
	{
		if (m_CreditsPurchased)
		{
			GameManager.GameSaved(true);
			m_CreditsPurchased = false;
		}
		m_CreditsCategory = -1;
		SoundManager.TriggerEvent("Play_UI_Negative");
		SoundManager.TriggerEvent("Play_UI_Window");
		Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
		if (m_Callback != null)
		{
			m_Callback(false);
		}
	}

	public void NoCreditsCallback(bool option)
	{
		m_CreditsCategory = -1;
		SoundManager.TriggerEvent("Play_UI_Negative");
		SoundManager.TriggerEvent("Play_UI_Window");
		Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
		if (m_Callback != null)
		{
			m_Callback(false);
		}
	}

	public void TogglePressed()
	{
		SoundManager.TriggerEvent("Play_UI_Toggle");
		Globals.m_DisplayPurchaseConfirmation = !Globals.m_DisplayPurchaseConfirmation;
	}
}
