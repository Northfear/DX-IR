using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
	public enum QuickslotMode
	{
		None = -1,
		Weapon = 0,
		Item = 1
	}

	public static InventoryPanel m_This;

	public UIScrollList m_ScrollList;

	[HideInInspector]
	public int m_ViewingCategory = -1;

	[HideInInspector]
	public int m_InspectedItem = -1;

	private bool m_WaitingForStoreToLoad;

	private float m_PreviousScrollPos;

	private List<int> m_ItemIDs = new List<int>();

	public UIRadioBtn m_StoreButton;

	public UIRadioBtn m_BundleButton;

	public UIRadioBtn m_WeaponButton;

	public UIRadioBtn m_AmmoButton;

	public UIRadioBtn m_UtilityButton;

	public InventoryContainer m_InventoryContainerPrefab;

	public InventoryItem m_InventoryItemPrefab;

	private List<UIRadioBtn> m_PageDots = new List<UIRadioBtn>();

	public UIRadioBtn m_PageDotPrefab;

	public Transform m_PageDotCenter;

	private float m_CurrentFlashTime;

	private QuickslotMode m_QuickslotMode = QuickslotMode.None;

	private int m_CurrentSlot = -1;

	public GameObject m_WeaponQuickslots;

	public PackedSprite[] m_WeaponQuickslotOutlines;

	public UIButton[] m_WeaponQuickslotButtons;

	public UIButton[] m_WeaponQuickslotRemoves;

	public SimpleSprite[] m_WeaponQuickslotIcons;

	public SpriteText[] m_WeaponQuickslotClips;

	public SpriteText[] m_WeaponQuickslotQuantities;

	public GameObject m_ItemQuickslots;

	public PackedSprite[] m_ItemQuickslotOutlines;

	public UIButton[] m_ItemQuickslotButtons;

	public UIButton[] m_ItemQuickslotRemoves;

	public SimpleSprite[] m_ItemQuickslotIcons;

	public SpriteText[] m_ItemQuickslotQuantities;

	private void Awake()
	{
		m_This = this;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = 0f;
		base.transform.localPosition = localPosition;
		m_StoreButton.Data = 7;
		m_StoreButton.SetValueChangedDelegate(RadioPressed);
		m_BundleButton.Data = 0;
		m_BundleButton.SetValueChangedDelegate(RadioPressed);
		m_WeaponButton.Data = 1;
		m_WeaponButton.SetValueChangedDelegate(RadioPressed);
		m_AmmoButton.Data = 2;
		m_AmmoButton.SetValueChangedDelegate(RadioPressed);
		m_UtilityButton.Data = 3;
		m_UtilityButton.SetValueChangedDelegate(RadioPressed);
		for (int i = 0; i < 4; i++)
		{
			m_WeaponQuickslotButtons[i].Data = i;
			m_WeaponQuickslotButtons[i].SetValueChangedDelegate(QuickslotButtonPressed);
			m_WeaponQuickslotRemoves[i].Data = i;
			m_WeaponQuickslotRemoves[i].SetValueChangedDelegate(QuickslotRemovePressed);
		}
		for (int j = 0; j < 4; j++)
		{
			m_ItemQuickslotButtons[j].Data = j;
			m_ItemQuickslotButtons[j].SetValueChangedDelegate(QuickslotButtonPressed);
			m_ItemQuickslotRemoves[j].Data = j;
			m_ItemQuickslotRemoves[j].SetValueChangedDelegate(QuickslotRemovePressed);
		}
	}

	private void Update()
	{
		if (m_ViewingCategory == 7 && m_WaitingForStoreToLoad)
		{
			if (Globals.m_Inventory.StoreLoaded())
			{
				PopulateScrollList();
				m_WaitingForStoreToLoad = false;
			}
			else if (Globals.m_Inventory.StoreLoadingError())
			{
				m_WaitingForStoreToLoad = false;
			}
		}
		UpdatePageDots();
		m_PreviousScrollPos = m_ScrollList.ScrollPosition;
	}

	private void LateUpdate()
	{
		m_CurrentFlashTime += PauseTabs.GetDeltaTime();
		if (m_CurrentFlashTime >= 1.2f)
		{
			m_CurrentFlashTime = 0f;
		}
		float t = 0f;
		if (m_CurrentFlashTime < 0.5f)
		{
			t = Mathf.Clamp01(m_CurrentFlashTime / 0.5f);
		}
		else if (m_CurrentFlashTime < 1f)
		{
			t = Mathf.Clamp01((1f - m_CurrentFlashTime) / 0.5f);
		}
		Color color = Color.Lerp(Color.black, Globals.m_This.m_BrightHUD, t);
		Color color2 = Color.Lerp(Globals.m_This.m_BlackHUD, Globals.m_This.m_BrightHUD, t);
		for (int i = 0; i < m_ScrollList.Count; i++)
		{
			InventoryContainer component = m_ScrollList.GetItem(i).gameObject.GetComponent<InventoryContainer>();
			for (int j = 0; j < component.m_ItemAttachments.Length; j++)
			{
				if (component.m_InventoryItems[j] != null)
				{
					if (m_QuickslotMode != QuickslotMode.None && Globals.m_Inventory.m_Items[m_ViewingCategory][m_ItemIDs[i * component.m_ItemAttachments.Length + j]].m_Assignable)
					{
						component.m_InventoryItems[j].m_ItemIcon.SetColor(color);
					}
					else
					{
						component.m_InventoryItems[j].m_ItemIcon.SetColor(Color.black);
					}
				}
			}
		}
		for (int k = 0; k < 4; k++)
		{
			if (m_WeaponQuickslotButtons[k].controlState == UIButton.CONTROL_STATE.OVER || m_WeaponQuickslotButtons[k].controlState == UIButton.CONTROL_STATE.ACTIVE)
			{
				m_WeaponQuickslotButtons[k].SetColor(Globals.m_This.m_BrightHUD);
			}
			else if (m_QuickslotMode != QuickslotMode.None && k == m_CurrentSlot)
			{
				m_WeaponQuickslotButtons[k].SetColor(color2);
			}
			else
			{
				m_WeaponQuickslotButtons[k].SetColor(Globals.m_This.m_BlackHUD);
			}
		}
		for (int l = 0; l < 4; l++)
		{
			if (m_ItemQuickslotButtons[l].controlState == UIButton.CONTROL_STATE.OVER || m_ItemQuickslotButtons[l].controlState == UIButton.CONTROL_STATE.ACTIVE)
			{
				m_ItemQuickslotButtons[l].SetColor(Globals.m_This.m_BrightHUD);
			}
			else if (m_QuickslotMode != QuickslotMode.None && l == m_CurrentSlot)
			{
				m_ItemQuickslotButtons[l].SetColor(color2);
			}
			else
			{
				m_ItemQuickslotButtons[l].SetColor(Globals.m_This.m_BlackHUD);
			}
		}
	}

	public static void InventoryOpening(int category = -1)
	{
		if (!(m_This == null))
		{
			if (category >= 0)
			{
				m_This.m_ViewingCategory = category;
				m_This.m_PreviousScrollPos = 0f;
			}
			if (m_This.m_ViewingCategory == 7)
			{
				m_This.m_StoreButton.ManuallySetValue(true);
			}
			else if (m_This.m_ViewingCategory == 0)
			{
				m_This.m_BundleButton.ManuallySetValue(true);
			}
			else if (m_This.m_ViewingCategory == 1)
			{
				m_This.m_WeaponButton.ManuallySetValue(true);
			}
			else if (m_This.m_ViewingCategory == 2)
			{
				m_This.m_AmmoButton.ManuallySetValue(true);
			}
			else
			{
				m_This.m_ViewingCategory = 3;
				m_This.m_UtilityButton.ManuallySetValue(true);
			}
			m_This.CancelQuickslotMode();
			m_This.PopulateScrollList();
		}
	}

	private void OpenStore()
	{
		if (!Globals.m_Inventory.InitializeStore(false))
		{
			m_WaitingForStoreToLoad = false;
			PopulateScrollList();
			return;
		}
		m_WaitingForStoreToLoad = true;
		while (m_ScrollList.Count > 0)
		{
			m_ScrollList.RemoveItem(0, true);
		}
		m_PreviousScrollPos = 0f;
		m_ScrollList.ScrollListTo(m_PreviousScrollPos);
		SetupPageDots();
	}

	private void PopulateScrollList()
	{
		if (m_InventoryItemPrefab == null || m_InventoryContainerPrefab == null || m_InventoryContainerPrefab.m_ItemAttachments == null || m_InventoryContainerPrefab.m_ItemAttachments.Length <= 0)
		{
			return;
		}
		m_ItemIDs.Clear();
		if (m_ViewingCategory == 7)
		{
			for (int i = 0; i < Globals.m_Inventory.m_IAPs.Count; i++)
			{
				if (Globals.m_Inventory.m_IAPs[i].m_Valid)
				{
					m_ItemIDs.Add(i);
				}
			}
		}
		else
		{
			for (int j = 0; j < Globals.m_Inventory.m_Items[m_ViewingCategory].Length; j++)
			{
				if (Globals.m_Inventory.m_Items[m_ViewingCategory][j].m_Valid && Globals.m_Inventory.m_Items[m_ViewingCategory][j].m_Browsable)
				{
					m_ItemIDs.Add(j);
				}
			}
		}
		int num = Mathf.CeilToInt((float)m_ItemIDs.Count / (float)m_InventoryContainerPrefab.m_ItemAttachments.Length);
		if (num <= 0)
		{
			while (m_ScrollList.Count > 0)
			{
				m_ScrollList.RemoveItem(0, true);
			}
			SetupPageDots();
			return;
		}
		while (m_ScrollList.Count >= num)
		{
			m_ScrollList.RemoveItem(num - 1, true);
		}
		while (m_ScrollList.Count < num)
		{
			m_ScrollList.CreateItem(m_InventoryContainerPrefab.gameObject);
		}
		for (int k = 0; k < m_ScrollList.Count; k++)
		{
			m_ScrollList.GetItem(k).Data = k;
		}
		for (int l = 0; l < m_ItemIDs.Count; l++)
		{
			InventoryContainer component = m_ScrollList.GetItem(Mathf.CeilToInt(l / m_InventoryContainerPrefab.m_ItemAttachments.Length)).gameObject.GetComponent<InventoryContainer>();
			int num2 = l % component.m_ItemAttachments.Length;
			InventoryItem inventoryItem = component.m_InventoryItems[num2];
			if (inventoryItem == null)
			{
				inventoryItem = (Object.Instantiate(m_InventoryItemPrefab.gameObject) as GameObject).GetComponent<InventoryItem>();
				component.m_InventoryItems[num2] = inventoryItem;
				inventoryItem.transform.parent = component.m_ItemAttachments[num2];
				inventoryItem.transform.localPosition = Vector3.zero;
				inventoryItem.transform.localRotation = Quaternion.identity;
			}
			Globals.m_Inventory.FillInInventoryItem(m_ViewingCategory, m_ItemIDs[l], inventoryItem, false);
			inventoryItem.m_GroupButton.SetValueChangedDelegate(AssignPressed);
			inventoryItem.m_InfoButton.SetValueChangedDelegate(InfoPressed);
			inventoryItem.m_PurchaseButton.SetValueChangedDelegate(PurchasePressed);
		}
		m_ScrollList.ScrollListTo(m_PreviousScrollPos);
		SetupPageDots();
	}

	private void SetupPageDots()
	{
		int count = m_ScrollList.Count;
		if (count <= 0)
		{
			for (int i = 0; i < m_PageDots.Count; i++)
			{
				Object.Destroy(m_PageDots[i]);
			}
			m_PageDots.Clear();
			return;
		}
		while (m_PageDots.Count > count)
		{
			Object.Destroy(m_PageDots[m_PageDots.Count - 1].gameObject);
			m_PageDots.RemoveAt(m_PageDots.Count - 1);
		}
		while (m_PageDots.Count < count)
		{
			GameObject gameObject = Object.Instantiate(m_PageDotPrefab.gameObject) as GameObject;
			m_PageDots.Add(gameObject.GetComponent<UIRadioBtn>());
		}
		float x = (m_PageDotPrefab.collider as BoxCollider).size.x;
		float num = x * 0.5f;
		float num2 = 0f - ((float)count * 0.5f * x + ((float)count * 0.5f - 0.5f) * num);
		for (int j = 0; j < m_PageDots.Count; j++)
		{
			m_PageDots[j].Data = j;
			m_PageDots[j].SetValueChangedDelegate(PageDotPressed);
			m_PageDots[j].transform.parent = m_PageDotCenter;
			m_PageDots[j].transform.localPosition = new Vector3(num2, 0f, 0f);
			m_PageDots[j].transform.localRotation = Quaternion.identity;
			num2 += x + num;
		}
		UpdatePageDots();
	}

	private void UpdatePageDots()
	{
		if (m_PageDots.Count > 0)
		{
			int index = Mathf.Clamp((int)(m_ScrollList.ScrollPosition * (float)m_PageDots.Count), 0, m_PageDots.Count - 1);
			m_PageDots[index].ManuallySetValue(true);
		}
	}

	public void RadioPressed(IUIObject obj)
	{
		int num = (int)obj.Data;
		if (num != m_ViewingCategory)
		{
			m_PreviousScrollPos = 0f;
			m_ViewingCategory = num;
			CancelQuickslotMode();
			if (m_ViewingCategory == 7)
			{
				OpenStore();
			}
			else
			{
				PopulateScrollList();
			}
		}
	}

	public void PageDotPressed(IUIObject obj)
	{
		m_ScrollList.ScrollToItem((int)obj.Data, m_ScrollList.minSnapDuration);
	}

	public void AssignPressed(IUIObject obj)
	{
		if (m_QuickslotMode == QuickslotMode.None)
		{
			return;
		}
		int num = (int)obj.Data;
		if (m_QuickslotMode == QuickslotMode.Weapon)
		{
			for (int i = 0; i < 4; i++)
			{
				if (Globals.m_Inventory.m_WeaponQuickslots[i].m_ItemID == num)
				{
					Globals.m_Inventory.m_WeaponQuickslots[i].m_CategoryID = -1;
					Globals.m_Inventory.m_WeaponQuickslots[i].m_ItemID = -1;
				}
			}
			Globals.m_Inventory.m_WeaponQuickslots[m_CurrentSlot].m_CategoryID = 1;
			Globals.m_Inventory.m_WeaponQuickslots[m_CurrentSlot].m_ItemID = num;
			if (Globals.m_Inventory.m_ActiveWeaponQuickslot == m_CurrentSlot)
			{
				Globals.m_PlayerController.DestroyCurrentWeapon();
				Globals.m_PlayerController.SetWeapon(m_CurrentSlot, false);
			}
		}
		else if (m_QuickslotMode == QuickslotMode.Item)
		{
			for (int j = 0; j < 4; j++)
			{
				if (Globals.m_Inventory.m_ItemQuickslots[j].m_CategoryID == m_ViewingCategory && Globals.m_Inventory.m_ItemQuickslots[j].m_ItemID == num)
				{
					Globals.m_Inventory.m_ItemQuickslots[j].m_CategoryID = -1;
					Globals.m_Inventory.m_ItemQuickslots[j].m_ItemID = -1;
				}
			}
			Globals.m_Inventory.m_ItemQuickslots[m_CurrentSlot].m_CategoryID = m_ViewingCategory;
			Globals.m_Inventory.m_ItemQuickslots[m_CurrentSlot].m_ItemID = num;
		}
		UpdateEquippedStatuses();
		CancelQuickslotMode();
		SoundManager.TriggerEvent("Play_UI_Quick_Slot_Equip");
	}

	public void QuickslotButtonPressed(IUIObject obj)
	{
		if (m_QuickslotMode != QuickslotMode.None)
		{
			CancelQuickslotMode();
			return;
		}
		m_QuickslotMode = ((m_ViewingCategory != 1) ? QuickslotMode.Item : QuickslotMode.Weapon);
		m_CurrentSlot = (int)obj.Data;
		m_CurrentFlashTime = 0f;
		if (m_QuickslotMode == QuickslotMode.Weapon)
		{
			m_WeaponQuickslots.collider.enabled = true;
			m_WeaponQuickslotRemoves[m_CurrentSlot].Hide(false);
			m_WeaponQuickslotRemoves[m_CurrentSlot].SetControlState(UIButton.CONTROL_STATE.NORMAL);
			FadeSpriteAlpha.Do(m_WeaponQuickslotRemoves[m_CurrentSlot], EZAnimation.ANIM_MODE.FromTo, Globals.m_ClearWhite, Color.white, EZAnimation.linear, 0.4f, 0f, null, null);
			AnimateRotation.Do(m_WeaponQuickslotRemoves[m_CurrentSlot].transform.parent.gameObject, EZAnimation.ANIM_MODE.FromTo, new Vector3(0f, 0f, -90f), Vector3.zero, EZAnimation.spring, 0.25f, 0f, null, null);
			SoundManager.TriggerEvent("Play_UI_Quick_Slot_Activate");
		}
		else if (m_QuickslotMode == QuickslotMode.Item)
		{
			m_ItemQuickslots.collider.enabled = true;
			m_ItemQuickslotRemoves[m_CurrentSlot].Hide(false);
			m_ItemQuickslotRemoves[m_CurrentSlot].SetControlState(UIButton.CONTROL_STATE.NORMAL);
			FadeSpriteAlpha.Do(m_ItemQuickslotRemoves[m_CurrentSlot], EZAnimation.ANIM_MODE.FromTo, Globals.m_ClearWhite, Color.white, EZAnimation.linear, 0.4f, 0f, null, null);
			AnimateRotation.Do(m_ItemQuickslotRemoves[m_CurrentSlot].transform.parent.gameObject, EZAnimation.ANIM_MODE.FromTo, new Vector3(0f, 0f, -90f), Vector3.zero, EZAnimation.spring, 0.25f, 0f, null, null);
			SoundManager.TriggerEvent("Play_UI_Quick_Slot_Activate");
		}
		for (int i = 0; i < m_ScrollList.Count; i++)
		{
			InventoryContainer component = m_ScrollList.GetItem(i).gameObject.GetComponent<InventoryContainer>();
			for (int j = 0; j < component.m_ItemAttachments.Length; j++)
			{
				if (component.m_InventoryItems[j] != null && Globals.m_Inventory.m_Items[m_ViewingCategory][m_ItemIDs[i * component.m_ItemAttachments.Length + j]].m_Assignable)
				{
					component.m_InventoryItems[j].m_GroupButton.collider.enabled = true;
				}
			}
		}
	}

	public void QuickslotRemovePressed(IUIObject obj)
	{
		if (m_QuickslotMode == QuickslotMode.Weapon)
		{
			Globals.m_Inventory.m_WeaponQuickslots[(int)obj.Data].m_CategoryID = -1;
			Globals.m_Inventory.m_WeaponQuickslots[(int)obj.Data].m_ItemID = -1;
		}
		else if (m_QuickslotMode == QuickslotMode.Item)
		{
			Globals.m_Inventory.m_ItemQuickslots[(int)obj.Data].m_CategoryID = -1;
			Globals.m_Inventory.m_ItemQuickslots[(int)obj.Data].m_ItemID = -1;
		}
		UpdateEquippedStatuses();
		CancelQuickslotMode();
		SoundManager.TriggerEvent("Play_UI_Quick_Slot_Unequip");
	}

	private void UpdateEquippedStatuses()
	{
		for (int i = 0; i < m_ScrollList.Count; i++)
		{
			InventoryContainer component = m_ScrollList.GetItem(i).gameObject.GetComponent<InventoryContainer>();
			for (int j = 0; j < component.m_ItemAttachments.Length; j++)
			{
				if (component.m_InventoryItems[j] != null)
				{
					Globals.m_Inventory.FillInInventoryItem(m_ViewingCategory, m_ItemIDs[i * component.m_ItemAttachments.Length + j], component.m_InventoryItems[j], false);
				}
			}
		}
	}

	public void CancelQuickslotMode()
	{
		SoundManager.TriggerEvent("Stop_UI_Quick_Slot_Glow");
		if (m_QuickslotMode != QuickslotMode.None)
		{
			for (int i = 0; i < m_ScrollList.Count; i++)
			{
				InventoryContainer component = m_ScrollList.GetItem(i).gameObject.GetComponent<InventoryContainer>();
				for (int j = 0; j < component.m_ItemAttachments.Length; j++)
				{
					if (component.m_InventoryItems[j] != null)
					{
						component.m_InventoryItems[j].m_GroupButton.collider.enabled = false;
					}
				}
			}
		}
		m_QuickslotMode = QuickslotMode.None;
		if (m_ViewingCategory == 1)
		{
			m_ItemQuickslots.SetActiveRecursively(false);
			m_WeaponQuickslots.SetActiveRecursively(true);
			for (int k = 0; k < 4; k++)
			{
				Item_Weapon weaponQuickslotItem = Globals.m_Inventory.GetWeaponQuickslotItem(k);
				if (weaponQuickslotItem != null)
				{
					m_WeaponQuickslotIcons[k].Hide(false);
					m_WeaponQuickslotIcons[k].renderer.material.mainTexture = weaponQuickslotItem.m_Texture;
					m_WeaponQuickslotClips[k].Hide(false);
					m_WeaponQuickslotClips[k].Text = weaponQuickslotItem.m_CurrentAmmoInClip.ToString();
					m_WeaponQuickslotQuantities[k].Hide(false);
					m_WeaponQuickslotQuantities[k].Text = Globals.m_Inventory.GetItemQuantity(2, weaponQuickslotItem.m_AmmoItemID).ToString();
				}
				else
				{
					m_WeaponQuickslotIcons[k].Hide(true);
					m_WeaponQuickslotClips[k].Hide(true);
					m_WeaponQuickslotQuantities[k].Hide(true);
				}
			}
		}
		else if (m_ViewingCategory == 3)
		{
			m_WeaponQuickslots.SetActiveRecursively(false);
			m_ItemQuickslots.SetActiveRecursively(true);
			for (int l = 0; l < 4; l++)
			{
				Item_Base itemQuickslotItem = Globals.m_Inventory.GetItemQuickslotItem(l);
				if (itemQuickslotItem != null)
				{
					m_ItemQuickslotIcons[l].Hide(false);
					m_ItemQuickslotIcons[l].renderer.material.mainTexture = itemQuickslotItem.m_Texture;
					if (Globals.m_Inventory.m_ItemQuickslots[l].m_CategoryID == 4)
					{
						m_ItemQuickslotIcons[l].transform.localScale = Vector3.one;
					}
					else
					{
						m_ItemQuickslotIcons[l].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
					}
					m_ItemQuickslotQuantities[l].Hide(itemQuickslotItem.m_MaxQuantity == 1);
					m_ItemQuickslotQuantities[l].Text = itemQuickslotItem.m_Quantity.ToString();
				}
				else
				{
					m_ItemQuickslotIcons[l].Hide(true);
					m_ItemQuickslotQuantities[l].Hide(true);
				}
			}
		}
		else
		{
			m_WeaponQuickslots.SetActiveRecursively(false);
			m_ItemQuickslots.SetActiveRecursively(false);
		}
		for (int m = 0; m < 4; m++)
		{
			m_WeaponQuickslotRemoves[m].Hide(true);
		}
		for (int n = 0; n < 4; n++)
		{
			m_ItemQuickslotRemoves[n].Hide(true);
		}
		m_WeaponQuickslots.collider.enabled = false;
		m_ItemQuickslots.collider.enabled = false;
	}

	public void InfoPressed(IUIObject obj)
	{
		CancelQuickslotMode();
		m_InspectedItem = (int)obj.Data;
		InfoPanel.OpenInfoPanel(m_ViewingCategory, m_InspectedItem, InfoCallback, "Return To Market");
		SoundManager.TriggerEvent("Play_UI_Select");
	}

	public void PurchasePressed(IUIObject obj)
	{
		CancelQuickslotMode();
		SoundManager.TriggerEvent("Play_UI_Select", base.gameObject);
		SoundManager.TriggerEvent("Play_UI_Window", base.gameObject);
		if (m_ViewingCategory == 7)
		{
			m_InspectedItem = (int)obj.Data;
			PurchaseConfirmationPanel.OpenStorePurchaseConfirmation(m_InspectedItem, PurchasedCallback);
			return;
		}
		m_InspectedItem = (int)obj.Data;
		if (!PurchaseConfirmationPanel.OpenPurchaseConfirmation(m_ViewingCategory, m_InspectedItem, PurchasedCallback, false))
		{
			Globals.m_Inventory.PurchaseItem(m_ViewingCategory, m_InspectedItem);
			PopulateScrollList();
		}
	}

	private void PurchasedCallback(bool PurchaseMade)
	{
		Globals.m_HUDRoot.m_PanelManager.BringIn("InventoryMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		if (PurchaseMade)
		{
			Globals.m_Inventory.PurchaseItem(m_ViewingCategory, m_InspectedItem);
			if (m_ViewingCategory == 7)
			{
				GameManager.GameSaved(true);
			}
		}
		CancelQuickslotMode();
		if (m_ViewingCategory == 7)
		{
			m_StoreButton.ManuallySetValue(true);
		}
		else if (m_ViewingCategory == 0)
		{
			m_BundleButton.ManuallySetValue(true);
		}
		else if (m_ViewingCategory == 1)
		{
			m_WeaponButton.ManuallySetValue(true);
		}
		else if (m_ViewingCategory == 2)
		{
			m_AmmoButton.ManuallySetValue(true);
		}
		else if (m_ViewingCategory == 3)
		{
			m_UtilityButton.ManuallySetValue(true);
		}
		if (m_ViewingCategory == 7)
		{
			OpenStore();
		}
		else
		{
			PopulateScrollList();
		}
		PauseTabs.SetMenu(1, false);
	}

	public void InfoCallback()
	{
		Globals.m_HUDRoot.m_PanelManager.BringIn("InventoryMenu", UIPanelManager.MENU_DIRECTION.Forwards);
		CancelQuickslotMode();
		if (m_ViewingCategory == 7)
		{
			m_StoreButton.ManuallySetValue(true);
		}
		else if (m_ViewingCategory == 0)
		{
			m_BundleButton.ManuallySetValue(true);
		}
		else if (m_ViewingCategory == 1)
		{
			m_WeaponButton.ManuallySetValue(true);
		}
		else if (m_ViewingCategory == 2)
		{
			m_AmmoButton.ManuallySetValue(true);
		}
		else if (m_ViewingCategory == 3)
		{
			m_UtilityButton.ManuallySetValue(true);
		}
		if (m_ViewingCategory == 7)
		{
			OpenStore();
		}
		else
		{
			PopulateScrollList();
		}
		PauseTabs.SetMenu(1, false);
	}
}
