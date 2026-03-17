using UnityEngine;

public class InfoPanel : MonoBehaviour
{
	public delegate void ExitCallback();

	public static InfoPanel m_This;

	public UIPanel m_Panel;

	private int m_CategoryID;

	private int m_ItemID;

	public UIScrollList m_DescList;

	public InventoryItem m_Item;

	public SpriteRoot m_AlphaTracker;

	public GameObject m_WeaponSection;

	public PackedSprite[] m_FireRate;

	public PackedSprite[] m_Reload;

	public PackedSprite[] m_Damage;

	public PackedSprite[] m_Capacity;

	public SimpleSprite m_DamageEnhancerIcon;

	public SpriteText m_DamageEnhancerQuantity;

	public SimpleSprite m_FireRateEnhancerIcon;

	public SpriteText m_FireRateEnhancerQuantity;

	public SimpleSprite m_ReloadEnhancerIcon;

	public SpriteText m_ReloadEnhancerQuantity;

	public SimpleSprite m_AmmoEnhancerIcon;

	public SpriteText m_AmmoEnhancerQuantity;

	public Color m_MarkOn = new Color(0.929f, 0.655f, 0.137f, 1f);

	public Color m_MarkBoosted = new Color(0f, 200f, 0f, 1f);

	public Color m_MarkBoostable = new Color(0f, 128f, 0f, 1f);

	public Color m_MarkOff = new Color(0.5f, 0.5f, 0.5f, 1f);

	public BTButton m_UseButton;

	public SpriteText m_ReturnText;

	private ExitCallback m_Callback;

	private void Awake()
	{
		m_This = this;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
	}

	public static void OpenInfoPanel(int Category, int ItemID, ExitCallback callback, string returnText)
	{
		if (!(m_This == null))
		{
			m_This.m_CategoryID = Category;
			m_This.m_ItemID = ItemID;
			SoundManager.TriggerEvent("Play_UI_Window");
			SoundManager.TriggerEvent("Play_UI_Select");
			Globals.m_HUDRoot.m_PanelManager.BringIn("InfoPanel", UIPanelManager.MENU_DIRECTION.Forwards);
			m_This.m_DamageEnhancerIcon.renderer.material.mainTexture = Globals.m_Inventory.m_Items[3][8].m_Texture;
			m_This.m_DamageEnhancerQuantity.Text = Globals.m_Inventory.m_Items[3][8].m_Quantity.ToString();
			m_This.m_FireRateEnhancerIcon.renderer.material.mainTexture = Globals.m_Inventory.m_Items[3][9].m_Texture;
			m_This.m_FireRateEnhancerQuantity.Text = Globals.m_Inventory.m_Items[3][9].m_Quantity.ToString();
			m_This.m_ReloadEnhancerIcon.renderer.material.mainTexture = Globals.m_Inventory.m_Items[3][10].m_Texture;
			m_This.m_ReloadEnhancerQuantity.Text = Globals.m_Inventory.m_Items[3][10].m_Quantity.ToString();
			m_This.m_AmmoEnhancerIcon.renderer.material.mainTexture = Globals.m_Inventory.m_Items[3][11].m_Texture;
			m_This.m_AmmoEnhancerQuantity.Text = Globals.m_Inventory.m_Items[3][11].m_Quantity.ToString();
			m_This.SetupItemData(true);
			m_This.m_ReturnText.Text = returnText;
			m_This.m_Callback = callback;
			PauseTabs.SetMenu(7, true);
		}
	}

	private void SetupItemData(bool ClearAlpha)
	{
		Globals.m_Inventory.FillInInventoryItem(m_CategoryID, m_ItemID, m_Item, ClearAlpha);
		m_Item.m_PurchaseButton.SetValueChangedDelegate(PurchasePressed);
		UIListItemContainer uIListItemContainer = m_DescList.GetItem(0) as UIListItemContainer;
		UISlider slider = m_DescList.slider;
		UIScrollKnob knob = slider.GetKnob();
		if (m_CategoryID == 7)
		{
			uIListItemContainer.Text = Globals.m_Inventory.m_IAPs[m_ItemID].m_Description;
		}
		else
		{
			uIListItemContainer.Text = Globals.m_Inventory.m_Items[m_CategoryID][m_ItemID].m_Description;
		}
		float num = (uIListItemContainer.TopLeftEdge.y - uIListItemContainer.BottomRightEdge.y) / m_DescList.viewableArea.y;
		knob.gameObject.active = num > 1f;
		if (knob.gameObject.active)
		{
			knob.SetSize(Mathf.Max(slider.width * (1f / num), 2f), knob.height);
			slider.stopKnobFromEdge = knob.width * 0.5f;
			slider.SetSize(slider.width, slider.height);
		}
		if (m_CategoryID != 1 || !(Globals.m_Inventory.m_Items[m_CategoryID][m_ItemID] as Item_Weapon).m_IsGun)
		{
			m_WeaponSection.SetActiveRecursively(false);
		}
		m_UseButton.gameObject.SetActiveRecursively(m_CategoryID == 3 && Globals.m_Inventory.m_Items[m_CategoryID][m_ItemID].m_Assignable && Globals.m_Inventory.m_Items[m_CategoryID][m_ItemID].m_Quantity > 0);
	}

	private void LateUpdate()
	{
		if (m_CategoryID == 1)
		{
			UpdateWeaponMarks();
		}
	}

	private void UpdateWeaponMarks()
	{
		Item_Weapon item_Weapon = Globals.m_Inventory.m_Items[m_CategoryID][m_ItemID] as Item_Weapon;
		if (!item_Weapon.m_IsGun)
		{
			return;
		}
		if (m_FireRate != null)
		{
			for (int i = 0; i < m_FireRate.Length; i++)
			{
				Color color = ((item_Weapon.m_MinFireRateRank < i + 1) ? ((item_Weapon.m_CurrentFireRateRank < i + 1) ? ((item_Weapon.m_MaxFireRateRank < i + 1) ? m_MarkOff : m_MarkBoostable) : m_MarkBoosted) : m_MarkOn);
				color.a *= m_AlphaTracker.Color.a;
				m_FireRate[i].SetColor(color);
			}
		}
		if (m_Reload != null)
		{
			for (int j = 0; j < m_Reload.Length; j++)
			{
				Color color = ((item_Weapon.m_MinReloadRank < j + 1) ? ((item_Weapon.m_CurrentReloadRank < j + 1) ? ((item_Weapon.m_MaxReloadRank < j + 1) ? m_MarkOff : m_MarkBoostable) : m_MarkBoosted) : m_MarkOn);
				color.a *= m_AlphaTracker.Color.a;
				m_Reload[j].SetColor(color);
			}
		}
		if (m_Damage != null)
		{
			for (int k = 0; k < m_Damage.Length; k++)
			{
				Color color = ((item_Weapon.m_MinDamageRank < k + 1) ? ((item_Weapon.m_CurrentDamageRank < k + 1) ? ((item_Weapon.m_MaxDamageRank < k + 1) ? m_MarkOff : m_MarkBoostable) : m_MarkBoosted) : m_MarkOn);
				color.a *= m_AlphaTracker.Color.a;
				m_Damage[k].SetColor(color);
			}
		}
		if (m_Capacity != null)
		{
			for (int l = 0; l < m_Capacity.Length; l++)
			{
				Color color = ((item_Weapon.m_MinAmmoRank < l + 1) ? ((item_Weapon.m_CurrentAmmoRank < l + 1) ? ((item_Weapon.m_MaxAmmoRank < l + 1) ? m_MarkOff : m_MarkBoostable) : m_MarkBoosted) : m_MarkOn);
				color.a *= m_AlphaTracker.Color.a;
				m_Capacity[l].SetColor(color);
			}
		}
	}

	public void PurchasePressed(IUIObject obj)
	{
		SoundManager.TriggerEvent("Play_UI_Select");
		SoundManager.TriggerEvent("Play_UI_Window");
		if (m_CategoryID == 7)
		{
			Globals.m_Inventory.PurchaseItem(m_CategoryID, m_ItemID);
		}
		else if (!PurchaseConfirmationPanel.OpenPurchaseConfirmation(m_CategoryID, m_ItemID, PurchaseCallback, false))
		{
			Globals.m_Inventory.PurchaseItem(m_CategoryID, m_ItemID);
			SetupItemData(false);
		}
	}

	private void PurchaseCallback(bool PurchaseMade)
	{
		Globals.m_HUDRoot.m_PanelManager.BringIn("InfoPanel", UIPanelManager.MENU_DIRECTION.Forwards);
		if (PurchaseMade)
		{
			Globals.m_Inventory.PurchaseItem(m_CategoryID, m_ItemID);
		}
		SetupItemData(true);
	}

	public void ExitPressed()
	{
		Globals.m_HUDRoot.m_PanelManager.DismissImmediate(UIPanelManager.MENU_DIRECTION.Forwards);
		SoundManager.TriggerEvent("Play_UI_Window");
		SoundManager.TriggerEvent("Play_UI_Select");
		if (m_Callback != null)
		{
			m_Callback();
		}
	}

	public void UsePressed()
	{
		if (m_CategoryID == 3)
		{
			Globals.m_Inventory.UseUtilityItem(m_ItemID);
			SetupItemData(false);
		}
	}

	public void DamageEnhancerPressed()
	{
		if (Globals.m_Inventory.m_Items[3][8].m_Quantity > 0)
		{
			Item_Weapon item_Weapon = Globals.m_Inventory.m_Items[m_CategoryID][m_ItemID] as Item_Weapon;
			if (item_Weapon.m_CurrentDamageRank < item_Weapon.m_MaxDamageRank)
			{
				item_Weapon.m_CurrentDamageRank++;
				Globals.m_Inventory.AdjustItemQuantity(3, 8, -1);
				m_DamageEnhancerQuantity.Text = Globals.m_Inventory.m_Items[3][8].m_Quantity.ToString();
				item_Weapon.m_FinalDamage = (int)Mathf.Lerp(item_Weapon.m_MinDamage, item_Weapon.m_MaxDamage, Mathf.InverseLerp(item_Weapon.m_MinDamageRank, item_Weapon.m_MaxDamageRank, item_Weapon.m_CurrentDamageRank));
			}
		}
	}

	public void FireRateEnhancerPressed()
	{
		if (Globals.m_Inventory.m_Items[3][9].m_Quantity > 0)
		{
			Item_Weapon item_Weapon = Globals.m_Inventory.m_Items[m_CategoryID][m_ItemID] as Item_Weapon;
			if (item_Weapon.m_CurrentFireRateRank < item_Weapon.m_MaxFireRateRank)
			{
				item_Weapon.m_CurrentFireRateRank++;
				Globals.m_Inventory.AdjustItemQuantity(3, 9, -1);
				m_FireRateEnhancerQuantity.Text = Globals.m_Inventory.m_Items[3][9].m_Quantity.ToString();
				item_Weapon.m_FinalFireRate = (int)Mathf.Lerp(item_Weapon.m_MinFireRate, item_Weapon.m_MaxFireRate, Mathf.InverseLerp(item_Weapon.m_MinFireRateRank, item_Weapon.m_MaxFireRateRank, item_Weapon.m_CurrentFireRateRank));
			}
		}
	}

	public void ReloadEnhancerPressed()
	{
		if (Globals.m_Inventory.m_Items[3][10].m_Quantity > 0)
		{
			Item_Weapon item_Weapon = Globals.m_Inventory.m_Items[m_CategoryID][m_ItemID] as Item_Weapon;
			if (item_Weapon.m_CurrentReloadRank < item_Weapon.m_MaxReloadRank)
			{
				item_Weapon.m_CurrentReloadRank++;
				Globals.m_Inventory.AdjustItemQuantity(3, 10, -1);
				m_ReloadEnhancerQuantity.Text = Globals.m_Inventory.m_Items[3][10].m_Quantity.ToString();
				item_Weapon.m_FinalReload = (int)Mathf.Lerp(item_Weapon.m_MinReload, item_Weapon.m_MaxReload, Mathf.InverseLerp(item_Weapon.m_MinReloadRank, item_Weapon.m_MaxReloadRank, item_Weapon.m_CurrentReloadRank));
			}
		}
	}

	public void AmmoEnhancerPressed()
	{
		if (Globals.m_Inventory.m_Items[3][11].m_Quantity > 0)
		{
			Item_Weapon item_Weapon = Globals.m_Inventory.m_Items[m_CategoryID][m_ItemID] as Item_Weapon;
			if (item_Weapon.m_CurrentAmmoRank < item_Weapon.m_MaxAmmoRank)
			{
				item_Weapon.m_CurrentAmmoRank++;
				Globals.m_Inventory.AdjustItemQuantity(3, 11, -1);
				m_AmmoEnhancerQuantity.Text = Globals.m_Inventory.m_Items[3][11].m_Quantity.ToString();
				item_Weapon.m_AmmoPerClip = (int)Mathf.Lerp(item_Weapon.m_MinAmmo, item_Weapon.m_MaxAmmo, Mathf.InverseLerp(item_Weapon.m_MinAmmoRank, item_Weapon.m_MaxAmmoRank, item_Weapon.m_CurrentAmmoRank));
			}
		}
	}
}
