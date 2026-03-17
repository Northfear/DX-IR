using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public enum FileState
	{
		None = -1,
		WaitingToDownload = 0,
		DownloadingFile = 1,
		LoadingTextures = 2,
		Done = 3,
		Error = 4,
		Total = 5
	}

	public const int m_PraxisExpToLevel = 4000;

	public string m_URLRoot = "http://www.n-fusion.com/DeusExMobile/";

	public string m_StoreFileName = "Store.xml";

	public string m_InventoryFileName = "Inventory_v1.xml";

	public string m_GameplayFileName = "Gameplay_v1.xml";

	public TextAsset m_DefaultInventory;

	public TextAsset m_DefaultGameplay;

	private WWW m_StoreWWW;

	private WWW m_InventoryWWW;

	private WWW m_GameplayWWW;

	private FileState m_StoreFileState;

	private float m_LastTimeStoreWasLoaded = -1f;

	[HideInInspector]
	public List<Item_IAP> m_IAPs = new List<Item_IAP>();

	private FileState m_InventoryFileState;

	private FileState m_GameplayFileState;

	[HideInInspector]
	public Item_Base[][] m_Items;

	[HideInInspector]
	public int m_ActiveItemQuickslot;

	[HideInInspector]
	public Quickslot[] m_ItemQuickslots = new Quickslot[4];

	[HideInInspector]
	public int m_ActiveWeaponQuickslot;

	[HideInInspector]
	public Quickslot[] m_WeaponQuickslots = new Quickslot[4];

	[HideInInspector]
	public int m_ActiveGrenadeQuickslot;

	[HideInInspector]
	public Quickslot[] m_GrenadeQuickslots = new Quickslot[6];

	public bool StoreLoading()
	{
		return m_StoreFileState == FileState.WaitingToDownload || m_StoreFileState == FileState.DownloadingFile || m_StoreFileState == FileState.LoadingTextures;
	}

	public bool StoreLoaded()
	{
		return m_StoreFileState == FileState.Done;
	}

	public bool StoreLoadingError()
	{
		return m_StoreFileState == FileState.Error;
	}

	public Quickslot GetActiveItemQuickslot()
	{
		return m_ItemQuickslots[m_ActiveItemQuickslot];
	}

	public int GetCredits()
	{
		return m_Items[5][0].m_Quantity;
	}

	public int GetPraxisExperience()
	{
		return m_Items[5][1].m_Quantity;
	}

	public int GetTotalExperience()
	{
		return m_Items[5][2].m_Quantity;
	}

	public int GetWeaponAmmo(WeaponItemID weapon)
	{
		Item_Weapon item_Weapon = m_Items[1][(int)weapon] as Item_Weapon;
		return GetItemQuantity(2, item_Weapon.m_AmmoItemID);
	}

	public void SetWeaponAmmo(WeaponItemID weapon, int ammo)
	{
		Item_Weapon item_Weapon = m_Items[1][(int)weapon] as Item_Weapon;
		m_Items[2][item_Weapon.m_AmmoItemID].m_Quantity = ammo;
	}

	public int GetItemQuantity(int category, int item)
	{
		if (category < 0 || category >= 6 || item < 0)
		{
			return 0;
		}
		if (category < 0 || item < 0 || category >= 6 || item >= m_Items[category].Length)
		{
			return 0;
		}
		return m_Items[category][item].m_Quantity;
	}

	public int GetItemMaxQuantity(int category, int item)
	{
		if (category < 0 || category >= 6 || item < 0)
		{
			return -1;
		}
		if (category < 0 || item < 0 || category >= 6 || item >= m_Items[category].Length)
		{
			return -1;
		}
		return m_Items[category][item].m_MaxQuantity;
	}

	public Texture GetItemTexture(int category, int item)
	{
		if (category < 0 || category >= 6 || item < 0)
		{
			return null;
		}
		if (category < 0 || item < 0 || category >= 6 || item >= m_Items[category].Length)
		{
			return null;
		}
		return m_Items[category][item].m_Texture;
	}

	public string GetItemCostAsString(int category, int item)
	{
		if (category < 0 || item < 0 || category >= 6 || item >= m_Items[category].Length)
		{
			return null;
		}
		return m_Items[category][item].m_Cost.ToString("N0");
	}

	public Texture GetWeaponQuickSlotItemTexture()
	{
		return GetWeaponQuickSlotItemTexture(m_ActiveWeaponQuickslot);
	}

	public Texture GetWeaponQuickSlotItemTexture(int quickslot)
	{
		if (quickslot < 0 || quickslot >= 4)
		{
			return null;
		}
		return GetItemTexture(m_WeaponQuickslots[quickslot].m_CategoryID, m_WeaponQuickslots[quickslot].m_ItemID);
	}

	public Texture GetItemQuickSlotItemTexture()
	{
		return GetItemQuickSlotItemTexture(m_ActiveItemQuickslot);
	}

	public Texture GetItemQuickSlotItemTexture(int quickslot)
	{
		if (quickslot < 0 || quickslot >= 4)
		{
			return null;
		}
		return GetItemTexture(m_ItemQuickslots[quickslot].m_CategoryID, m_ItemQuickslots[quickslot].m_ItemID);
	}

	public Texture GetGrenadeQuickSlotItemTexture()
	{
		return GetGrenadeQuickSlotItemTexture(m_ActiveGrenadeQuickslot);
	}

	public Texture GetGrenadeQuickSlotItemTexture(int quickslot)
	{
		if (quickslot < 0 || quickslot >= 6)
		{
			return null;
		}
		return GetItemTexture(m_GrenadeQuickslots[quickslot].m_CategoryID, m_GrenadeQuickslots[quickslot].m_ItemID);
	}

	public Item_Weapon GetWeaponQuickslotItem(int quickslot)
	{
		if (quickslot < 0 || quickslot >= 4)
		{
			return null;
		}
		if (m_WeaponQuickslots[quickslot].m_CategoryID < 0 || m_WeaponQuickslots[quickslot].m_ItemID < 0)
		{
			return null;
		}
		return m_Items[m_WeaponQuickslots[quickslot].m_CategoryID][m_WeaponQuickslots[quickslot].m_ItemID] as Item_Weapon;
	}

	public Item_Base GetItemQuickslotItem(int quickslot)
	{
		if (quickslot < 0 || quickslot >= 4)
		{
			return null;
		}
		if (m_ItemQuickslots[quickslot].m_CategoryID < 0 || m_ItemQuickslots[quickslot].m_ItemID < 0)
		{
			return null;
		}
		return m_Items[m_ItemQuickslots[quickslot].m_CategoryID][m_ItemQuickslots[quickslot].m_ItemID];
	}

	public int GetWeaponQuickSlotItemQuantity()
	{
		return GetWeaponQuickSlotItemQuantity(m_ActiveWeaponQuickslot);
	}

	public int GetWeaponQuickSlotItemQuantity(int quickslot)
	{
		if (quickslot < 0 || quickslot >= 4)
		{
			return 0;
		}
		return GetItemQuantity(m_WeaponQuickslots[quickslot].m_CategoryID, m_WeaponQuickslots[quickslot].m_ItemID);
	}

	public int GetItemQuickSlotItemQuantity()
	{
		return GetItemQuickSlotItemQuantity(m_ActiveItemQuickslot);
	}

	public int GetItemQuickSlotItemQuantity(int quickslot)
	{
		if (quickslot < 0 || quickslot >= 4)
		{
			return 0;
		}
		return GetItemQuantity(m_ItemQuickslots[quickslot].m_CategoryID, m_ItemQuickslots[quickslot].m_ItemID);
	}

	public int GetGrenadeQuickSlotItemQuantity()
	{
		return GetGrenadeQuickSlotItemQuantity(m_ActiveGrenadeQuickslot);
	}

	public int GetGrenadeQuickSlotItemQuantity(int quickslot)
	{
		if (quickslot < 0 || quickslot >= 6)
		{
			return 0;
		}
		return GetItemQuantity(m_GrenadeQuickslots[quickslot].m_CategoryID, m_GrenadeQuickslots[quickslot].m_ItemID);
	}

	public int GetItemQuickSlotItemMaxQuantity()
	{
		return GetItemQuickSlotItemMaxQuantity(m_ActiveItemQuickslot);
	}

	public int GetItemQuickSlotItemMaxQuantity(int quickslot)
	{
		if (quickslot < 0 || quickslot >= 4)
		{
			return 0;
		}
		return GetItemMaxQuantity(m_ItemQuickslots[quickslot].m_CategoryID, m_ItemQuickslots[quickslot].m_ItemID);
	}

	public void AdjustCredits(int adj)
	{
		m_Items[5][0].m_Quantity = Mathf.Max(m_Items[5][0].m_Quantity + adj, 0);
	}

	public void AdjustPraxisExp(int adj)
	{
		m_Items[5][1].m_Quantity += adj;
		m_Items[5][2].m_Quantity += adj;
		while (m_Items[5][1].m_Quantity >= 4000)
		{
			m_Items[5][1].m_Quantity -= 4000;
			m_Items[3][4].m_Quantity++;
		}
	}

	public void AdjustItemQuantity(int category, int item, int adj)
	{
		m_Items[category][item].m_Quantity += adj;
	}

	public bool IsEquipped(int Category, int Item)
	{
		for (int i = 0; i < 4; i++)
		{
			if (m_WeaponQuickslots[i].m_CategoryID == Category && m_WeaponQuickslots[i].m_ItemID == Item)
			{
				return true;
			}
		}
		for (int j = 0; j < 4; j++)
		{
			if (m_ItemQuickslots[j].m_CategoryID == Category && m_ItemQuickslots[j].m_ItemID == Item)
			{
				return true;
			}
		}
		return false;
	}

	private void Awake()
	{
		Globals.m_Inventory = this;
		InitializeInventory();
		InitializeQuickslots();
	}

	public bool SelectActiveWeaponQuickslot(int slot)
	{
		if (slot < 0 || slot >= 4)
		{
			return false;
		}
		int categoryID = m_WeaponQuickslots[slot].m_CategoryID;
		int itemID = m_WeaponQuickslots[slot].m_ItemID;
		if (categoryID < 0 || categoryID >= 6)
		{
			return false;
		}
		if (itemID < 0 || itemID >= m_Items[categoryID].Length)
		{
			return false;
		}
		if (Globals.m_PlayerController.SetWeapon(slot, false))
		{
			m_ActiveWeaponQuickslot = slot;
			return true;
		}
		return false;
	}

	public bool SelectActiveItemQuickslot(int slot)
	{
		if (slot < 0 || slot >= 4)
		{
			return false;
		}
		int categoryID = m_ItemQuickslots[slot].m_CategoryID;
		int itemID = m_ItemQuickslots[slot].m_ItemID;
		if (categoryID < 0 || categoryID >= 6)
		{
			return false;
		}
		if (itemID < 0 || itemID >= m_Items[categoryID].Length)
		{
			return false;
		}
		m_ActiveItemQuickslot = slot;
		return true;
	}

	public bool SelectActiveGrenadeQuickslot(int slot)
	{
		if (slot < 0 || slot >= 6)
		{
			return false;
		}
		int categoryID = m_GrenadeQuickslots[slot].m_CategoryID;
		int itemID = m_GrenadeQuickslots[slot].m_ItemID;
		if (categoryID < 0 || categoryID >= 6)
		{
			return false;
		}
		if (itemID < 0 || itemID >= m_Items[categoryID].Length)
		{
			return false;
		}
		m_ActiveGrenadeQuickslot = slot;
		return true;
	}

	public bool UseItemQuickslot()
	{
		if (m_ActiveItemQuickslot < 0 || m_ActiveItemQuickslot >= 4)
		{
			return false;
		}
		int categoryID = m_ItemQuickslots[m_ActiveItemQuickslot].m_CategoryID;
		int itemID = m_ItemQuickslots[m_ActiveItemQuickslot].m_ItemID;
		if (categoryID < 0 || categoryID >= 6)
		{
			return false;
		}
		if (itemID < 0 || itemID >= m_Items[categoryID].Length)
		{
			return false;
		}
		switch (categoryID)
		{
		case 3:
			return UseUtilityItem(itemID);
		case 4:
			return UseAugmentation(itemID);
		default:
			return false;
		}
	}

	public bool UseGrenadeQuickslot()
	{
		if (m_ActiveGrenadeQuickslot < 0 || m_ActiveGrenadeQuickslot >= 6)
		{
			return false;
		}
		int categoryID = m_GrenadeQuickslots[m_ActiveGrenadeQuickslot].m_CategoryID;
		int itemID = m_GrenadeQuickslots[m_ActiveGrenadeQuickslot].m_ItemID;
		if (categoryID < 0 || categoryID >= 6)
		{
			return false;
		}
		if (itemID < 0 || itemID >= m_Items[categoryID].Length)
		{
			return false;
		}
		if (m_Items[categoryID][itemID].m_Quantity <= 0)
		{
			return false;
		}
		if (Globals.m_PlayerController.UseGrenade(ItemIDToGrenadeType((WeaponItemID)itemID)))
		{
			m_Items[categoryID][itemID].m_Quantity--;
			return true;
		}
		return false;
	}

	public bool UseUtilityItem(int Item)
	{
		if (m_Items[3][Item].m_Quantity <= 0)
		{
			return false;
		}
		switch (Item)
		{
		case 0:
			if (Globals.m_PlayerController.UseEnergyBar())
			{
				m_Items[3][Item].m_Quantity--;
				return true;
			}
			break;
		case 1:
		case 2:
		case 3:
			if (Globals.m_PlayerController.UseBooze())
			{
				m_Items[3][Item].m_Quantity--;
				return true;
			}
			break;
		}
		return false;
	}

	public bool UseAugmentation(int Item)
	{
		switch (Item)
		{
		case 7:
			Globals.m_PlayerController.ToggleCloaking();
			return true;
		case 9:
			Globals.m_PlayerController.ArmorButtonTapped();
			return true;
		case 10:
			Globals.m_PlayerController.ToggleSeeThroughWalls();
			return true;
		default:
			return false;
		}
	}

	private void Update()
	{
		UpdateStoreLoading();
		UpdateInventoryLoading();
		UpdateGameplayLoading();
	}

	private void UpdateStoreLoading()
	{
		switch (m_StoreFileState)
		{
		case FileState.WaitingToDownload:
			m_StoreWWW = new WWW(m_URLRoot + m_StoreFileName);
			m_StoreFileState = FileState.DownloadingFile;
			break;
		case FileState.DownloadingFile:
			if (m_StoreWWW.isDone && m_InventoryFileState == FileState.Done)
			{
				if (m_StoreWWW.error == null)
				{
					ParseStore(m_StoreWWW.text);
					m_StoreFileState = FileState.LoadingTextures;
				}
				else
				{
					m_StoreFileState = FileState.Error;
				}
				m_StoreWWW = null;
			}
			break;
		case FileState.LoadingTextures:
			LoadStoreTextures();
			m_StoreFileState = FileState.Done;
			break;
		}
	}

	private void UpdateInventoryLoading()
	{
		switch (m_InventoryFileState)
		{
		case FileState.WaitingToDownload:
			m_InventoryWWW = new WWW(m_URLRoot + m_InventoryFileName);
			m_InventoryFileState = FileState.DownloadingFile;
			break;
		case FileState.DownloadingFile:
			if (m_InventoryWWW.isDone)
			{
				if (m_InventoryWWW.error == null)
				{
					ParseInventory(m_InventoryWWW.text);
					File.WriteAllText(Application.persistentDataPath + "/" + m_InventoryFileName, m_InventoryWWW.text);
				}
				else if (File.Exists(Application.persistentDataPath + "/" + m_InventoryFileName))
				{
					ParseInventory(File.ReadAllText(Application.persistentDataPath + "/" + m_InventoryFileName));
				}
				else
				{
					ParseInventory(m_DefaultInventory.text);
				}
				m_InventoryFileState = FileState.LoadingTextures;
				m_InventoryWWW = null;
			}
			break;
		case FileState.LoadingTextures:
			LoadInventoryTextures();
			m_InventoryFileState = FileState.Done;
			break;
		}
	}

	private void UpdateGameplayLoading()
	{
		switch (m_GameplayFileState)
		{
		case FileState.WaitingToDownload:
			m_GameplayWWW = new WWW(m_URLRoot + m_GameplayFileName);
			m_GameplayFileState = FileState.DownloadingFile;
			break;
		case FileState.DownloadingFile:
			if (m_GameplayWWW.isDone && m_InventoryFileState == FileState.Done)
			{
				if (m_GameplayWWW.error == null)
				{
					ParseGameplay(m_GameplayWWW.text);
					File.WriteAllText(Application.persistentDataPath + "/" + m_GameplayFileName, m_GameplayWWW.text);
				}
				else if (File.Exists(Application.persistentDataPath + "/" + m_GameplayFileName))
				{
					ParseGameplay(File.ReadAllText(Application.persistentDataPath + "/" + m_GameplayFileName));
				}
				else
				{
					ParseGameplay(m_DefaultGameplay.text);
				}
				m_GameplayFileState = FileState.Done;
				m_GameplayWWW = null;
			}
			break;
		}
	}

	private void ParseStore(string xml)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xml);
		XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("Table");
		XmlNodeList elementsByTagName2 = (elementsByTagName[0] as XmlElement).GetElementsByTagName("Row");
		for (int i = 1; i < elementsByTagName2.Count; i++)
		{
			XmlElement xmlElement = (elementsByTagName2[i] as XmlElement).FirstChild as XmlElement;
			Item_IAP item_IAP = new Item_IAP();
			item_IAP.m_Valid = true;
			item_IAP.m_AppStoreID = int.Parse(xmlElement.FirstChild.InnerText);
			xmlElement = xmlElement.NextSibling as XmlElement;
			item_IAP.m_TypeName = xmlElement.FirstChild.InnerText;
			xmlElement = xmlElement.NextSibling as XmlElement;
			item_IAP.m_Name = xmlElement.FirstChild.InnerText;
			xmlElement = xmlElement.NextSibling as XmlElement;
			item_IAP.m_Description = xmlElement.FirstChild.InnerText;
			xmlElement = xmlElement.NextSibling as XmlElement;
			item_IAP.m_TexturePath = xmlElement.FirstChild.InnerText;
			xmlElement = xmlElement.NextSibling as XmlElement;
			item_IAP.m_MinSupportedVersion = int.Parse(xmlElement.FirstChild.InnerText);
			xmlElement = xmlElement.NextSibling as XmlElement;
			try
			{
				item_IAP.m_MaxSupportedVersion = int.Parse(xmlElement.FirstChild.InnerText);
			}
			catch
			{
				item_IAP.m_MaxSupportedVersion = 999999;
			}
			XmlElement xmlElement2 = xmlElement.NextSibling as XmlElement;
			int num = 0;
			while (xmlElement2 != null)
			{
				num++;
				xmlElement2 = xmlElement2.NextSibling as XmlElement;
			}
			num /= 3;
			item_IAP.m_Purchasables = new Purchasable[num];
			for (int j = 0; j < num; j++)
			{
				item_IAP.m_Purchasables[j] = new Purchasable();
				xmlElement = xmlElement.NextSibling as XmlElement;
				int enumValue = Globals.GetEnumValue<CategoryID>(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				if (enumValue >= 0)
				{
					int enumValue2 = GetEnumValue(enumValue, xmlElement.FirstChild.InnerText);
					xmlElement = xmlElement.NextSibling as XmlElement;
					if (enumValue2 >= 0)
					{
						int quantity = int.Parse(xmlElement.FirstChild.InnerText);
						item_IAP.m_Purchasables[j].m_CategoryID = enumValue;
						item_IAP.m_Purchasables[j].m_ItemID = enumValue2;
						item_IAP.m_Purchasables[j].m_Quantity = quantity;
					}
				}
				else
				{
					xmlElement = xmlElement.NextSibling as XmlElement;
				}
			}
			if (item_IAP.m_MinSupportedVersion > Globals.m_This.m_VersionMajor || item_IAP.m_MaxSupportedVersion < Globals.m_This.m_VersionMajor)
			{
				item_IAP.m_Valid = false;
			}
			m_IAPs.Add(item_IAP);
		}
	}

	private void ParseInventory(string xml)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xml);
		XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("Table");
		int num = -1;
		int num2 = -1;
		num = 0;
		XmlNodeList elementsByTagName2 = (elementsByTagName[0] as XmlElement).GetElementsByTagName("Row");
		for (int i = 1; i < elementsByTagName2.Count; i++)
		{
			XmlElement xmlElement = (elementsByTagName2[i] as XmlElement).FirstChild as XmlElement;
			num2 = Globals.GetEnumValue<BundleItemID>(xmlElement.FirstChild.InnerText);
			if (num2 < 0)
			{
				continue;
			}
			m_Items[num][num2].m_Valid = true;
			xmlElement = xmlElement.NextSibling as XmlElement;
			m_Items[num][num2].m_TypeName = xmlElement.FirstChild.InnerText;
			xmlElement = xmlElement.NextSibling as XmlElement;
			m_Items[num][num2].m_Name = xmlElement.FirstChild.InnerText;
			xmlElement = xmlElement.NextSibling as XmlElement;
			m_Items[num][num2].m_Description = xmlElement.FirstChild.InnerText;
			xmlElement = xmlElement.NextSibling as XmlElement;
			m_Items[num][num2].m_TexturePath = xmlElement.FirstChild.InnerText;
			xmlElement = xmlElement.NextSibling as XmlElement;
			m_Items[num][num2].m_Cost = int.Parse(xmlElement.FirstChild.InnerText);
			XmlElement xmlElement2 = xmlElement.NextSibling as XmlElement;
			int num3 = 0;
			while (xmlElement2 != null)
			{
				num3++;
				xmlElement2 = xmlElement2.NextSibling as XmlElement;
			}
			num3 /= 3;
			(m_Items[num][num2] as Item_Bundle).m_Purchasables = new Purchasable[num3];
			for (int j = 0; j < num3; j++)
			{
				(m_Items[num][num2] as Item_Bundle).m_Purchasables[j] = new Purchasable();
				xmlElement = xmlElement.NextSibling as XmlElement;
				int enumValue = Globals.GetEnumValue<CategoryID>(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				if (enumValue >= 0)
				{
					int enumValue2 = GetEnumValue(enumValue, xmlElement.FirstChild.InnerText);
					xmlElement = xmlElement.NextSibling as XmlElement;
					if (enumValue2 >= 0)
					{
						int quantity = int.Parse(xmlElement.FirstChild.InnerText);
						(m_Items[num][num2] as Item_Bundle).m_Purchasables[j].m_CategoryID = enumValue;
						(m_Items[num][num2] as Item_Bundle).m_Purchasables[j].m_ItemID = enumValue2;
						(m_Items[num][num2] as Item_Bundle).m_Purchasables[j].m_Quantity = quantity;
					}
				}
				else
				{
					xmlElement = xmlElement.NextSibling as XmlElement;
				}
			}
		}
		num = 1;
		elementsByTagName2 = (elementsByTagName[1] as XmlElement).GetElementsByTagName("Row");
		for (int k = 1; k < elementsByTagName2.Count; k++)
		{
			XmlElement xmlElement = (elementsByTagName2[k] as XmlElement).FirstChild as XmlElement;
			num2 = Globals.GetEnumValue<WeaponItemID>(xmlElement.FirstChild.InnerText);
			if (num2 >= 0)
			{
				m_Items[num][num2].m_Valid = true;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_TypeName = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Name = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Description = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_TexturePath = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				(m_Items[num][num2] as Item_Weapon).m_IsGun = string.Compare(xmlElement.FirstChild.InnerText, "Yes", true) == 0;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Browsable = string.Compare(xmlElement.FirstChild.InnerText, "Yes", true) == 0;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Assignable = string.Compare(xmlElement.FirstChild.InnerText, "Yes", true) == 0;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Cost = int.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Quantity = int.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_MaxQuantity = int.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_PurchaseQuantity = int.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				(m_Items[num][num2] as Item_Weapon).m_AmmoItemID = Globals.GetEnumValue<AmmoItemID>(xmlElement.FirstChild.InnerText);
			}
		}
		num = 2;
		elementsByTagName2 = (elementsByTagName[2] as XmlElement).GetElementsByTagName("Row");
		for (int l = 1; l < elementsByTagName2.Count; l++)
		{
			XmlElement xmlElement = (elementsByTagName2[l] as XmlElement).FirstChild as XmlElement;
			num2 = Globals.GetEnumValue<AmmoItemID>(xmlElement.FirstChild.InnerText);
			if (num2 >= 0)
			{
				m_Items[num][num2].m_Valid = true;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_TypeName = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Name = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Description = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_TexturePath = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Browsable = string.Compare(xmlElement.FirstChild.InnerText, "Yes", true) == 0;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Assignable = string.Compare(xmlElement.FirstChild.InnerText, "Yes", true) == 0;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Cost = int.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Quantity = int.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_MaxQuantity = int.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_PurchaseQuantity = int.Parse(xmlElement.FirstChild.InnerText);
			}
		}
		num = 3;
		elementsByTagName2 = (elementsByTagName[3] as XmlElement).GetElementsByTagName("Row");
		for (int m = 1; m < elementsByTagName2.Count; m++)
		{
			XmlElement xmlElement = (elementsByTagName2[m] as XmlElement).FirstChild as XmlElement;
			num2 = Globals.GetEnumValue<UtilityItemID>(xmlElement.FirstChild.InnerText);
			if (num2 >= 0)
			{
				m_Items[num][num2].m_Valid = true;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_TypeName = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Name = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Description = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_TexturePath = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Browsable = string.Compare(xmlElement.FirstChild.InnerText, "Yes", true) == 0;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Assignable = string.Compare(xmlElement.FirstChild.InnerText, "Yes", true) == 0;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Cost = int.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Quantity = int.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_MaxQuantity = int.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_PurchaseQuantity = int.Parse(xmlElement.FirstChild.InnerText);
			}
		}
		num = 4;
		elementsByTagName2 = (elementsByTagName[4] as XmlElement).GetElementsByTagName("Row");
		for (int n = 1; n < elementsByTagName2.Count; n++)
		{
			XmlElement xmlElement = (elementsByTagName2[n] as XmlElement).FirstChild as XmlElement;
			num2 = Globals.GetEnumValue<AugmentationItemID>(xmlElement.FirstChild.InnerText);
			if (num2 >= 0)
			{
				m_Items[num][num2].m_Valid = true;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Name = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Description = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_TexturePath = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Assignable = string.Compare(xmlElement.FirstChild.InnerText, "Yes", true) == 0;
			}
		}
		num = 5;
		elementsByTagName2 = (elementsByTagName[5] as XmlElement).GetElementsByTagName("Row");
		for (int num4 = 1; num4 < elementsByTagName2.Count; num4++)
		{
			XmlElement xmlElement = (elementsByTagName2[num4] as XmlElement).FirstChild as XmlElement;
			num2 = Globals.GetEnumValue<MiscItemID>(xmlElement.FirstChild.InnerText);
			if (num2 >= 0)
			{
				m_Items[num][num2].m_Valid = true;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Name = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Description = xmlElement.FirstChild.InnerText;
				xmlElement = xmlElement.NextSibling as XmlElement;
				m_Items[num][num2].m_Quantity = int.Parse(xmlElement.FirstChild.InnerText);
			}
		}
	}

	private void ParseGameplay(string xml)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xml);
		XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("Table");
		XmlNodeList elementsByTagName2 = (elementsByTagName[0] as XmlElement).GetElementsByTagName("Row");
		for (int i = 1; i < elementsByTagName2.Count; i++)
		{
			XmlElement xmlElement = (elementsByTagName2[i] as XmlElement).FirstChild as XmlElement;
			int enumValue = Globals.GetEnumValue<WeaponItemID>(xmlElement.FirstChild.InnerText);
			if (enumValue >= 0)
			{
				Item_Weapon item_Weapon = m_Items[1][enumValue] as Item_Weapon;
				xmlElement = xmlElement.NextSibling as XmlElement;
				string[] array = xmlElement.FirstChild.InnerText.Split(" -,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				item_Weapon.m_MinDamageRank = int.Parse(array[0]);
				item_Weapon.m_MaxDamageRank = int.Parse(array[1]);
				item_Weapon.m_CurrentDamageRank = item_Weapon.m_MinDamageRank;
				xmlElement = xmlElement.NextSibling as XmlElement;
				item_Weapon.m_MinDamage = int.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				item_Weapon.m_MaxDamage = int.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				array = xmlElement.FirstChild.InnerText.Split(" -,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				item_Weapon.m_MinFireRateRank = int.Parse(array[0]);
				item_Weapon.m_MaxFireRateRank = int.Parse(array[1]);
				item_Weapon.m_CurrentFireRateRank = item_Weapon.m_MinFireRateRank;
				xmlElement = xmlElement.NextSibling as XmlElement;
				item_Weapon.m_MinFireRate = float.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				item_Weapon.m_MaxFireRate = float.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				array = xmlElement.FirstChild.InnerText.Split(" -,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				item_Weapon.m_MinReloadRank = int.Parse(array[0]);
				item_Weapon.m_MaxReloadRank = int.Parse(array[1]);
				item_Weapon.m_CurrentReloadRank = item_Weapon.m_MinReloadRank;
				xmlElement = xmlElement.NextSibling as XmlElement;
				item_Weapon.m_MinReload = float.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				item_Weapon.m_MaxReload = float.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				array = xmlElement.FirstChild.InnerText.Split(" -,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				item_Weapon.m_MinAmmoRank = int.Parse(array[0]);
				item_Weapon.m_MaxAmmoRank = int.Parse(array[1]);
				item_Weapon.m_CurrentAmmoRank = item_Weapon.m_MinAmmoRank;
				xmlElement = xmlElement.NextSibling as XmlElement;
				item_Weapon.m_MinAmmo = int.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				item_Weapon.m_MaxAmmo = int.Parse(xmlElement.FirstChild.InnerText);
				xmlElement = xmlElement.NextSibling as XmlElement;
				item_Weapon.m_MinEnemyPCRSqr = float.Parse(xmlElement.FirstChild.InnerText);
				item_Weapon.m_MinEnemyPCRSqr *= item_Weapon.m_MinEnemyPCRSqr;
				xmlElement = xmlElement.NextSibling as XmlElement;
				item_Weapon.m_MaxEnemyPCRSqr = float.Parse(xmlElement.FirstChild.InnerText);
				item_Weapon.m_MaxEnemyPCRSqr *= item_Weapon.m_MaxEnemyPCRSqr;
				item_Weapon.m_FinalDamage = item_Weapon.m_MinDamage;
				item_Weapon.m_FinalFireRate = item_Weapon.m_MinFireRate;
				item_Weapon.m_FinalReload = item_Weapon.m_MinReload;
				item_Weapon.m_AmmoPerClip = item_Weapon.m_MinAmmo;
				item_Weapon.m_CurrentAmmoInClip = Mathf.Min(item_Weapon.m_AmmoPerClip, m_Items[2][item_Weapon.m_AmmoItemID].m_Quantity);
				m_Items[2][item_Weapon.m_AmmoItemID].m_Quantity = Mathf.Max(m_Items[2][item_Weapon.m_AmmoItemID].m_Quantity - item_Weapon.m_CurrentAmmoInClip, 0);
			}
		}
	}

	private int GetEnumValue(int Category, string name)
	{
		switch (Category)
		{
		case 0:
			return Globals.GetEnumValue<BundleItemID>(name);
		case 1:
			return Globals.GetEnumValue<WeaponItemID>(name);
		case 2:
			return Globals.GetEnumValue<AmmoItemID>(name);
		case 3:
			return Globals.GetEnumValue<UtilityItemID>(name);
		case 4:
			return Globals.GetEnumValue<AugmentationItemID>(name);
		case 5:
			return Globals.GetEnumValue<MiscItemID>(name);
		default:
			return -1;
		}
	}

	public static int ItemIDToWeaponType(WeaponItemID itemID)
	{
		switch (itemID)
		{
		case WeaponItemID.CombatRifle:
			return 0;
		case WeaponItemID.Crossbow:
			return 1;
		case WeaponItemID.Shotgun:
			return 2;
		case WeaponItemID.Pistol:
			return 3;
		case WeaponItemID.PlasmaRifle:
			return 4;
		case WeaponItemID.StunGun:
			return 5;
		case WeaponItemID.MiniRPG:
			return 6;
		case WeaponItemID.HeavyRifle:
			return 7;
		default:
			return -1;
		}
	}

	public static int ItemIDToGrenadeType(WeaponItemID itemID)
	{
		switch (itemID)
		{
		case WeaponItemID.FragGrenade:
			return 0;
		case WeaponItemID.EMPGrenade:
			return 1;
		case WeaponItemID.ConcussionGrenade:
			return 2;
		case WeaponItemID.FragMine:
			return 3;
		case WeaponItemID.EMPMine:
			return 4;
		case WeaponItemID.ConcussionMine:
			return 5;
		default:
			return -1;
		}
	}

	public static int GrenadeTypeToItemID(GrenadeType type)
	{
		switch (type)
		{
		case GrenadeType.Frag:
			return 3;
		case GrenadeType.EMP:
			return 4;
		case GrenadeType.Concussion:
			return 5;
		case GrenadeType.FragMine:
			return 10;
		case GrenadeType.EMPMine:
			return 11;
		case GrenadeType.ConcussionMine:
			return 12;
		default:
			return -1;
		}
	}

	public static int WeaponTypeToItemID(WeaponType weaponType)
	{
		switch (weaponType)
		{
		case WeaponType.CombatRifle:
			return 0;
		case WeaponType.Crossbow:
			return 1;
		case WeaponType.Shotgun:
			return 2;
		case WeaponType.Pistol:
			return 6;
		case WeaponType.PlasmaRifle:
			return 7;
		case WeaponType.StunGun:
			return 8;
		case WeaponType.MiniRPG:
			return 9;
		case WeaponType.HeavyRifle:
			return 13;
		default:
			return -1;
		}
	}

	public bool InitializeStore(bool ForceLoad)
	{
		if (StoreLoading())
		{
			return true;
		}
		if (!ForceLoad && m_LastTimeStoreWasLoaded > 0f && Time.realtimeSinceStartup - m_LastTimeStoreWasLoaded < 10f)
		{
			return false;
		}
		m_LastTimeStoreWasLoaded = Time.realtimeSinceStartup;
		m_IAPs.Clear();
		m_StoreFileState = FileState.WaitingToDownload;
		return true;
	}

	private void InitializeInventory()
	{
		m_Items = new Item_Base[6][];
		m_Items[0] = new Item_Bundle[4];
		m_Items[1] = new Item_Weapon[14];
		m_Items[2] = new Item_Ammo[7];
		m_Items[3] = new Item_Utility[12];
		m_Items[4] = new Item_Augmentation[11];
		m_Items[5] = new Item_Misc[3];
		for (int i = 0; i < 4; i++)
		{
			m_Items[0][i] = new Item_Bundle();
		}
		for (int j = 0; j < 14; j++)
		{
			m_Items[1][j] = new Item_Weapon();
		}
		for (int k = 0; k < 7; k++)
		{
			m_Items[2][k] = new Item_Ammo();
		}
		for (int l = 0; l < 12; l++)
		{
			m_Items[3][l] = new Item_Utility();
		}
		for (int m = 0; m < 11; m++)
		{
			m_Items[4][m] = new Item_Augmentation();
		}
		for (int n = 0; n < 3; n++)
		{
			m_Items[5][n] = new Item_Misc();
		}
	}

	private void InitializeQuickslots()
	{
		m_WeaponQuickslots[0] = new Quickslot();
		m_WeaponQuickslots[0].m_CategoryID = 1;
		m_WeaponQuickslots[0].m_ItemID = 0;
		m_WeaponQuickslots[1] = new Quickslot();
		m_WeaponQuickslots[1].m_CategoryID = 1;
		m_WeaponQuickslots[1].m_ItemID = 1;
		m_WeaponQuickslots[2] = new Quickslot();
		m_WeaponQuickslots[2].m_CategoryID = 1;
		m_WeaponQuickslots[2].m_ItemID = 2;
		m_WeaponQuickslots[3] = new Quickslot();
		m_WeaponQuickslots[3].m_CategoryID = 1;
		m_WeaponQuickslots[3].m_ItemID = 6;
		m_ItemQuickslots[0] = new Quickslot();
		m_ItemQuickslots[0].m_CategoryID = -1;
		m_ItemQuickslots[0].m_ItemID = -1;
		m_ItemQuickslots[1] = new Quickslot();
		m_ItemQuickslots[1].m_CategoryID = -1;
		m_ItemQuickslots[1].m_ItemID = -1;
		m_ItemQuickslots[2] = new Quickslot();
		m_ItemQuickslots[2].m_CategoryID = -1;
		m_ItemQuickslots[2].m_ItemID = -1;
		m_ItemQuickslots[3] = new Quickslot();
		m_ItemQuickslots[3].m_CategoryID = -1;
		m_ItemQuickslots[3].m_ItemID = -1;
		for (int i = 0; i < 6; i++)
		{
			m_GrenadeQuickslots[i] = new Quickslot();
			m_GrenadeQuickslots[i].m_CategoryID = 1;
			m_GrenadeQuickslots[i].m_ItemID = GrenadeTypeToItemID((GrenadeType)i);
		}
	}

	private void LoadStoreTextures()
	{
		for (int i = 0; i < m_IAPs.Count; i++)
		{
			if (m_IAPs[i].m_Valid)
			{
				m_IAPs[i].m_Texture = Resources.Load(m_IAPs[i].m_TexturePath) as Texture;
				if (m_IAPs[i].m_Texture == null)
				{
					m_IAPs[i].m_Valid = false;
				}
			}
		}
	}

	private void LoadInventoryTextures()
	{
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < m_Items[i].Length; j++)
			{
				if (m_Items[i][j].m_Valid)
				{
					m_Items[i][j].m_Texture = Resources.Load(m_Items[i][j].m_TexturePath) as Texture;
					if (m_Items[i][j].m_Texture == null)
					{
						m_Items[i][j].m_Valid = false;
					}
				}
			}
		}
	}

	public void FillInInventoryItem(int Category, int ItemID, InventoryItem listing, bool ClearAlpha = false)
	{
		if (Category == 7)
		{
			Item_IAP item_IAP = m_IAPs[ItemID];
			listing.m_GroupButton.RenderCamera = Globals.m_HUDRoot.m_HUDCamera2D;
			listing.m_GroupButton.collider.enabled = false;
			listing.m_TypeName.Text = item_IAP.m_TypeName;
			listing.m_ItemName.Text = item_IAP.m_Name;
			listing.m_ItemIcon.renderer.material.mainTexture = item_IAP.m_Texture;
			listing.m_GroupButton.Data = ItemID;
			listing.m_PurchaseButton.Data = ItemID;
			if (listing.m_InfoButton != null)
			{
				listing.m_InfoButton.Data = ItemID;
			}
			listing.m_EquippedBox.Hide(true);
			listing.m_NoneBox.Hide(true);
			listing.m_OwnedBox.Hide(true);
			listing.m_Quantity.Hide(true);
			listing.m_PurchaseQuantity.Hide(true);
			listing.m_AmmoIcon.Hide(true);
			listing.m_AmmoText.Hide(true);
			Color brightHUD = Globals.m_This.m_BrightHUD;
			if (ClearAlpha)
			{
				brightHUD.a = 0f;
			}
			listing.m_GroupButton.SetColor(brightHUD);
			listing.m_PurchaseButton.gameObject.active = true;
			listing.m_PurchaseButton.collider.enabled = true;
			listing.m_PurchaseEquipped.Hide(true);
			listing.m_PurchaseText.Text = "[#20FFFF]BUY";
			return;
		}
		Item_Base item_Base = m_Items[Category][ItemID];
		listing.m_GroupButton.RenderCamera = Globals.m_HUDRoot.m_HUDCamera2D;
		listing.m_GroupButton.collider.enabled = false;
		listing.m_TypeName.Text = item_Base.m_TypeName;
		listing.m_ItemName.Text = item_Base.m_Name;
		listing.m_ItemIcon.renderer.material.mainTexture = item_Base.m_Texture;
		listing.m_GroupButton.Data = ItemID;
		listing.m_PurchaseButton.Data = ItemID;
		if (listing.m_InfoButton != null)
		{
			listing.m_InfoButton.Data = ItemID;
		}
		if (Category == 1 && (item_Base as Item_Weapon).m_IsGun)
		{
			listing.m_EquippedBox.Hide(true);
			listing.m_Quantity.Hide(true);
			listing.m_PurchaseQuantity.Hide(true);
			listing.m_AmmoIcon.Hide(false);
			listing.m_AmmoText.Hide(false);
			int itemQuantity = GetItemQuantity(2, (item_Base as Item_Weapon).m_AmmoItemID);
			listing.m_AmmoText.Text = itemQuantity.ToString();
			listing.m_NoneBox.Hide(itemQuantity > 0);
			listing.m_OwnedBox.Hide(itemQuantity <= 0);
			if (IsEquipped(Category, ItemID))
			{
				Color equipped = Globals.m_This.m_Equipped;
				if (ClearAlpha)
				{
					equipped.a = 0f;
				}
				listing.m_GroupButton.SetColor(equipped);
				listing.m_PurchaseButton.gameObject.active = false;
				listing.m_PurchaseEquipped.Hide(false);
				listing.m_PurchaseText.Text = "EQUIPPED";
				return;
			}
			Color brightHUD2 = Globals.m_This.m_BrightHUD;
			if (ClearAlpha)
			{
				brightHUD2.a = 0f;
			}
			listing.m_GroupButton.SetColor(brightHUD2);
			if (item_Base.m_Quantity > 0)
			{
				listing.m_PurchaseButton.gameObject.active = true;
				listing.m_PurchaseButton.collider.enabled = false;
				listing.m_PurchaseEquipped.Hide(true);
				listing.m_PurchaseText.Text = "OWNED";
			}
			else if (item_Base.m_MaxQuantity > 1 && item_Base.m_Quantity >= item_Base.m_MaxQuantity)
			{
				listing.m_PurchaseButton.gameObject.active = true;
				listing.m_PurchaseButton.collider.enabled = false;
				listing.m_PurchaseEquipped.Hide(true);
				listing.m_PurchaseText.Text = "[#20FFFF]MAX";
			}
			else
			{
				listing.m_PurchaseButton.gameObject.active = true;
				listing.m_PurchaseButton.collider.enabled = true;
				listing.m_PurchaseEquipped.Hide(true);
				listing.m_PurchaseText.Text = "[#20FFFF]" + GetItemCostAsString(Category, ItemID);
			}
			return;
		}
		listing.m_AmmoIcon.Hide(true);
		listing.m_AmmoText.Hide(true);
		listing.m_Quantity.Hide(false);
		listing.m_Quantity.Text = item_Base.m_Quantity.ToString();
		if (item_Base.m_MaxQuantity > 1 && item_Base.m_Quantity >= item_Base.m_MaxQuantity)
		{
			listing.m_PurchaseButton.gameObject.active = true;
			listing.m_PurchaseButton.collider.enabled = false;
			listing.m_PurchaseEquipped.Hide(true);
			listing.m_PurchaseText.Text = "[#20FFFF]MAX";
		}
		else
		{
			listing.m_PurchaseButton.gameObject.active = true;
			listing.m_PurchaseButton.collider.enabled = true;
			listing.m_PurchaseEquipped.Hide(true);
			listing.m_PurchaseText.Text = "[#20FFFF]" + GetItemCostAsString(Category, ItemID);
		}
		listing.m_PurchaseQuantity.Hide(item_Base.m_PurchaseQuantity <= 1);
		listing.m_PurchaseQuantity.Text = "x" + item_Base.m_PurchaseQuantity;
		if (IsEquipped(Category, ItemID))
		{
			Color equipped2 = Globals.m_This.m_Equipped;
			if (ClearAlpha)
			{
				equipped2.a = 0f;
			}
			listing.m_GroupButton.SetColor(equipped2);
			listing.m_EquippedBox.Hide(item_Base.m_Quantity <= 0);
			listing.m_NoneBox.Hide(item_Base.m_Quantity > 0);
			listing.m_OwnedBox.Hide(true);
		}
		else
		{
			Color brightHUD3 = Globals.m_This.m_BrightHUD;
			if (ClearAlpha)
			{
				brightHUD3.a = 0f;
			}
			listing.m_GroupButton.SetColor(brightHUD3);
			listing.m_EquippedBox.Hide(true);
			listing.m_NoneBox.Hide(item_Base.m_Quantity > 0);
			listing.m_OwnedBox.Hide(item_Base.m_Quantity <= 0);
		}
	}

	public bool PurchaseItem(int Category, int ItemID)
	{
		switch (Category)
		{
		case 7:
		{
			Item_IAP item_IAP = m_IAPs[ItemID];
			if (item_IAP != null)
			{
				for (int i = 0; i < item_IAP.m_Purchasables.Length; i++)
				{
					Globals.m_Inventory.AdjustItemQuantity(item_IAP.m_Purchasables[i].m_CategoryID, item_IAP.m_Purchasables[i].m_ItemID, item_IAP.m_Purchasables[i].m_Quantity);
				}
			}
			break;
		}
		case 0:
		{
			Item_Bundle item_Bundle = m_Items[Category][ItemID] as Item_Bundle;
			if (item_Bundle != null)
			{
				Globals.m_Inventory.AdjustCredits(-item_Bundle.m_Cost);
				for (int j = 0; j < item_Bundle.m_Purchasables.Length; j++)
				{
					Globals.m_Inventory.AdjustItemQuantity(item_Bundle.m_Purchasables[j].m_CategoryID, item_Bundle.m_Purchasables[j].m_ItemID, item_Bundle.m_Purchasables[j].m_Quantity);
				}
			}
			break;
		}
		default:
		{
			Item_Base item_Base = m_Items[Category][ItemID];
			if (item_Base != null)
			{
				Globals.m_Inventory.AdjustCredits(-item_Base.m_Cost);
				Globals.m_Inventory.AdjustItemQuantity(Category, ItemID, item_Base.m_PurchaseQuantity);
			}
			break;
		}
		}
		return true;
	}

	public static void UnlockAll()
	{
		if (Globals.m_Inventory == null)
		{
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < Globals.m_Inventory.m_Items[i].Length; j++)
			{
				if (Globals.m_Inventory.m_Items[i][j].m_MaxQuantity < 0)
				{
					Globals.m_Inventory.m_Items[i][j].m_Quantity++;
				}
				else if (Globals.m_Inventory.m_Items[i][j].m_MaxQuantity == 0)
				{
					Globals.m_Inventory.m_Items[i][j].m_Quantity = 0;
				}
				else
				{
					Globals.m_Inventory.m_Items[i][j].m_Quantity = Globals.m_Inventory.m_Items[i][j].m_MaxQuantity;
				}
			}
		}
		Globals.m_Inventory.m_Items[5][0].m_Quantity = 50000;
		Globals.m_Inventory.m_Items[5][1].m_Quantity = 0;
		Globals.m_Inventory.m_Items[5][2].m_Quantity = 0;
		for (int k = 0; k < 14; k++)
		{
			AugmentationContainer augmentationContainer = Globals.m_AugmentationData.GetAugmentationContainer((AugmentationData.Augmentations)k);
			if (augmentationContainer != null && augmentationContainer.GetAugData() != null)
			{
				for (int l = 0; l < augmentationContainer.GetAugData().Length; l++)
				{
					augmentationContainer.GetAugData(l).m_Purchased = true;
				}
			}
		}
	}

	public XmlElement SaveGame(XmlElement root, XmlDocument doc)
	{
		XmlElement xmlElement = (XmlElement)root.AppendChild(doc.CreateElement("Inventory"));
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < m_Items[i].Length; j++)
			{
				XmlElement xmlElement2 = (XmlElement)xmlElement.AppendChild(doc.CreateElement("Item"));
				xmlElement2.SetAttribute("Category", i.ToString());
				xmlElement2.SetAttribute("Index", j.ToString());
				xmlElement2.SetAttribute("Quantity", m_Items[i][j].m_Quantity.ToString());
				if (i == 1)
				{
					xmlElement2.SetAttribute("CurrentAmmoInClip", (m_Items[i][j] as Item_Weapon).m_CurrentAmmoInClip.ToString());
				}
			}
		}
		XmlElement xmlElement3 = (XmlElement)xmlElement.AppendChild(doc.CreateElement("ItemQuickSlots"));
		xmlElement3.SetAttribute("Active_Item_Quickslot", m_ActiveItemQuickslot.ToString());
		for (int k = 0; k < 4; k++)
		{
			XmlElement xmlElement4 = (XmlElement)xmlElement3.AppendChild(doc.CreateElement("Slot"));
			xmlElement4.SetAttribute("Category", m_ItemQuickslots[k].m_CategoryID.ToString());
			xmlElement4.SetAttribute("ItemID", m_ItemQuickslots[k].m_ItemID.ToString());
		}
		XmlElement xmlElement5 = (XmlElement)xmlElement.AppendChild(doc.CreateElement("WeaponQuickSlots"));
		xmlElement5.SetAttribute("Active_Weapon_Quickslot", m_ActiveWeaponQuickslot.ToString());
		for (int l = 0; l < 4; l++)
		{
			XmlElement xmlElement6 = (XmlElement)xmlElement5.AppendChild(doc.CreateElement("Slot"));
			xmlElement6.SetAttribute("Category", m_WeaponQuickslots[l].m_CategoryID.ToString());
			xmlElement6.SetAttribute("ItemID", m_WeaponQuickslots[l].m_ItemID.ToString());
		}
		XmlElement xmlElement7 = (XmlElement)xmlElement.AppendChild(doc.CreateElement("GrenadeQuickSlots"));
		xmlElement7.SetAttribute("Active_Grenade_Quickslot", m_ActiveGrenadeQuickslot.ToString());
		for (int m = 0; m < 6; m++)
		{
			XmlElement xmlElement8 = (XmlElement)xmlElement7.AppendChild(doc.CreateElement("Slot"));
			xmlElement8.SetAttribute("Category", m_GrenadeQuickslots[m].m_CategoryID.ToString());
			xmlElement8.SetAttribute("ItemID", m_GrenadeQuickslots[m].m_ItemID.ToString());
		}
		return xmlElement;
	}

	public void LoadGame(XmlElement root)
	{
		XmlNodeList elementsByTagName = root.GetElementsByTagName("Inventory");
		foreach (XmlNode item in elementsByTagName)
		{
			foreach (XmlNode childNode in item.ChildNodes)
			{
				int num = 0;
				switch (childNode.Name)
				{
				case "Item":
				{
					int num2 = -1;
					int num3 = -1;
					foreach (XmlAttribute attribute in childNode.Attributes)
					{
						switch (attribute.Name)
						{
						case "Category":
							num2 = int.Parse(attribute.InnerText);
							break;
						case "Index":
							num3 = int.Parse(attribute.InnerText);
							break;
						case "Quantity":
							if (num2 != -1 && num3 != -1)
							{
								m_Items[num2][num3].m_Quantity = int.Parse(attribute.InnerText);
							}
							break;
						case "CurrentAmmoInClip":
							if (num2 != -1 && num3 != -1)
							{
								(m_Items[num2][num3] as Item_Weapon).m_CurrentAmmoInClip = int.Parse(attribute.InnerText);
							}
							break;
						}
					}
					break;
				}
				case "ItemQuickSlots":
					foreach (XmlAttribute attribute2 in childNode.Attributes)
					{
						switch (attribute2.Name)
						{
						case "Active_Item_Quickslot":
							m_ActiveItemQuickslot = int.Parse(attribute2.InnerText);
							break;
						}
					}
					num = 0;
					foreach (XmlNode childNode2 in childNode.ChildNodes)
					{
						if (childNode2.Name == "Slot")
						{
							foreach (XmlAttribute attribute3 in childNode2.Attributes)
							{
								switch (attribute3.Name)
								{
								case "Category":
									m_ItemQuickslots[num].m_CategoryID = int.Parse(attribute3.InnerText);
									break;
								case "ItemID":
									m_ItemQuickslots[num].m_ItemID = int.Parse(attribute3.InnerText);
									break;
								}
							}
						}
						num++;
					}
					break;
				case "WeaponQuickSlots":
					foreach (XmlAttribute attribute4 in childNode.Attributes)
					{
						switch (attribute4.Name)
						{
						case "Active_Weapon_Quickslot":
							m_ActiveWeaponQuickslot = int.Parse(attribute4.InnerText);
							break;
						}
					}
					num = 0;
					foreach (XmlNode childNode3 in childNode.ChildNodes)
					{
						if (childNode3.Name == "Slot")
						{
							foreach (XmlAttribute attribute5 in childNode3.Attributes)
							{
								switch (attribute5.Name)
								{
								case "Category":
									m_WeaponQuickslots[num].m_CategoryID = int.Parse(attribute5.InnerText);
									break;
								case "ItemID":
									m_WeaponQuickslots[num].m_ItemID = int.Parse(attribute5.InnerText);
									break;
								}
							}
						}
						num++;
					}
					break;
				case "GrenadeQuickSlots":
					foreach (XmlAttribute attribute6 in childNode.Attributes)
					{
						switch (attribute6.Name)
						{
						case "Active_Grenade_Quickslot":
							m_ActiveGrenadeQuickslot = int.Parse(attribute6.InnerText);
							break;
						}
					}
					num = 0;
					foreach (XmlNode childNode4 in childNode.ChildNodes)
					{
						if (childNode4.Name == "Slot")
						{
							foreach (XmlAttribute attribute7 in childNode4.Attributes)
							{
								switch (attribute7.Name)
								{
								case "Category":
									m_GrenadeQuickslots[num].m_CategoryID = int.Parse(attribute7.InnerText);
									break;
								case "ItemID":
									m_GrenadeQuickslots[num].m_ItemID = int.Parse(attribute7.InnerText);
									break;
								}
							}
						}
						num++;
					}
					break;
				}
			}
		}
		Globals.m_HUD.Display(false, false, false);
		Globals.m_HUD.Display(true, true, false);
	}

	public void LoadDefaultInventory()
	{
		if (File.Exists(Application.persistentDataPath + "/" + m_InventoryFileName))
		{
			ParseInventory(File.ReadAllText(Application.persistentDataPath + "/" + m_InventoryFileName));
		}
		else
		{
			ParseInventory(m_DefaultInventory.text);
		}
	}
}
