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

	private StoreItem m_ItemChosen = StoreItem.None;

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

	public void CmbtAmmoInfo()
	{
		m_ItemChosen = StoreItem.CmbtAmmo;
		string desc = "Titanium-cored, fin-stabilized, discarding-sabot, .303 caliber rounds manufactured for use in any military-field combat rifle.\n\nManufacturer: Osprey GmbH\n\nInventory size: 1x2\n\nStack: 20";
		InfoPanel.OpenInfoPanel(m_ItemChosen, "C.RIFLE AMMO", "[#00E4B8]V50", m_IconRenderers[(int)m_ItemChosen].material, "[#EDA723]COMBAT RIFLE AMMO (20)", desc);
	}

	public void CrbwAmmoInfo()
	{
		m_ItemChosen = StoreItem.CrbwAmmo;
		string desc = "Polycarbonate, carbide-tipped fin-stabilized bolts, optimized to offer stealth, grace, and accuracy when hand-loaded into a crossbow.\n\nManufacturer: Kaiga Ltd.\n\nInventory size: 1x3\n\nStack: 5";
		InfoPanel.OpenInfoPanel(m_ItemChosen, "C.BOW AMMO", "[#00E4B8]V75", m_IconRenderers[(int)m_ItemChosen].material, "[#EDA723]CROSSBOW AMMO (5)", desc);
	}

	public void GrenadeInfo()
	{
		m_ItemChosen = StoreItem.Grenade;
		string desc = "The M1-A Frag is an anti-personnel weapon that is designed to disperse shrapnel upon exploding. The body is grooved and segmented to helps to create sharp fragments when the grenade explodes.\n\nThe M1-A Frag is armed by a firing pin and does not have security lever like standard grenade. The M1-A Frag is a defensive grenade unlike offensive grenades used in 40 mm grenade launchers.";
		InfoPanel.OpenInfoPanel(m_ItemChosen, "FRAG GRENADE", "[#00E4B8]V150", m_IconRenderers[(int)m_ItemChosen].material, "[#EDA723]M1-A FRAG GRENADE", desc);
	}

	public void PraxisKitInfo()
	{
		m_ItemChosen = StoreItem.PraxisKit;
		string desc = "After somebody receives mechanical augmentations, it is common for many of the new features to be locked so that the body does not get overwhelmed and eventually reject the augment.\n\nTraditionally, these abilities unlock themselves through prolonged usage of the augmentations, thus allowing the body to become accustomed to the new features slowly, however Praxis kits, distributed mainly from L.I.M.B clinics, allow this system to be bypassed manually and release new features at the users own free will.";
		InfoPanel.OpenInfoPanel(m_ItemChosen, "PRAXIS KIT", "[#00E4B8]V10,000", m_IconRenderers[(int)m_ItemChosen].material, "[#EDA723]PRAXIS KIT", desc);
	}

	public void EnergyBarInfo()
	{
		m_ItemChosen = StoreItem.EnergyBar;
		string desc = "Cyberboost Proenergy is an artificial food energy source, packed with proteins and carbohydrates. It is 90% fat free with no sugar added.\n\nWhen consumed, Cyberboost bars replenish one energy cell in full.";
		InfoPanel.OpenInfoPanel(m_ItemChosen, "ENERGY BAR", "[#00E4B8]V25", m_IconRenderers[(int)m_ItemChosen].material, "[#EDA723]CYBERBOOST PROENERGY BAR", desc);
	}

	public void BoozeInfo()
	{
		m_ItemChosen = StoreItem.Booze;
		string desc = "Beer is an alcoholic beverage produced through brewing and fermenting grains such as barley or wheat. The process of brewing sterilizes beer, making it a very popular drink for thousands of years around the world.\n\nIt's delicious.";
		InfoPanel.OpenInfoPanel(m_ItemChosen, "BEER", "[#00E4B8]V5", m_IconRenderers[(int)m_ItemChosen].material, "[#EDA723]BEER", desc);
	}

	public void CombatRifleInfo()
	{
		m_ItemChosen = StoreItem.CombatRifle;
		string desc = "The FR-27 SFR is a modern fully automatic combat rifle developed by Steiner Bisley. Instead of firing bullets, the FR-27 SFR fires flechettes.\n\nThe flechettes that it uses are faster and more powerful than conventional bullets (an effect of being larger, heavier rounds), these rounds are fin-stabilized and fired by a smoothbore barrel. This power comes with a cost though as only 20 rounds can be held in a single magazine.";
		InfoPanel.OpenInfoPanel(m_ItemChosen, "COMBAT RIFLE", "[#00E4B8]V1,260", m_IconRenderers[(int)m_ItemChosen].material, "[#EDA723]FR-27 SFR COMBAT RIFLE", desc);
	}

	public void CrossbowInfo()
	{
		m_ItemChosen = StoreItem.Crossbow;
		string desc = "The Xbow XHII is a light and foldable crossbow manufactured by Stasiuk Arms Inc. It has a 4 pulleys system and specially shaped bow segment. \n\nThis weapon uses Guillaume Tell Golden Arrows, which are polycarbonate, carbide-tipped, fin-stabilized bolts and are manufactured by Kaiga Ltd, a Russian weapon manufacturing company.";
		InfoPanel.OpenInfoPanel(m_ItemChosen, "CROSSBOW", "[#00E4B8]V2,000", m_IconRenderers[(int)m_ItemChosen].material, "[#EDA723]XBOW XHII CROSSBOW", desc);
	}

	public void AutoHackInfo()
	{
		m_ItemChosen = StoreItem.AutoHack;
		string desc = "The Auto Hack uses modified microwave pulse technology, the Keybreaker Security Countermeasure unit can neutralize 98% of conventional electronic and digital locking systems.\n\nThis one-shot device fires a directed high-energy burst into the circuitry of the security unit, overloading it and forcing an immediate disengage; once activated, the discharge consumes the Keybreaker's internal capacitor and renders the device useless.";
		InfoPanel.OpenInfoPanel(m_ItemChosen, "AUTO HACK", "[#00E4B8]V500", m_IconRenderers[(int)m_ItemChosen].material, "[#EDA723]AUTOMATIC HACKING DEVICE", desc);
	}

	public void Credits500Info()
	{
		m_ItemChosen = StoreItem.Credits500;
		string desc = "Credits are global virtual currency. Credits are the official currency used in Germany, France, The United States, Egypt, Canada and the Peoples Republic of China. They are mostly used digitally through the Internet and ATMs, but despite what their name implies, physical financial transactions can also be made through the use of Tangible Credit Chits - Green vouchers that may hold anywhere from cr50 to several hundred credits.\n\nPack Value: [#EDA723]500";
		InfoPanel.OpenInfoPanel(m_ItemChosen, "CREDITS", "[#00E4B8]$0.99", null, "[#EDA723]500 CREDITS", desc);
	}

	public void Credits2500Info()
	{
		m_ItemChosen = StoreItem.Credits2500;
		string desc = "Credits are global virtual currency. Credits are the official currency used in Germany, France, The United States, Egypt, Canada and the Peoples Republic of China. They are mostly used digitally through the Internet and ATMs, but despite what their name implies, physical financial transactions can also be made through the use of Tangible Credit Chits - Green vouchers that may hold anywhere from cr50 to several hundred credits.\n\nPack Value: [#EDA723]2500";
		InfoPanel.OpenInfoPanel(m_ItemChosen, "CREDITS", "[#00E4B8]$1.99", null, "[#EDA723]2,500 CREDITS", desc);
	}

	public void Credits10000Info()
	{
		m_ItemChosen = StoreItem.Credits10000;
		string desc = "Credits are global virtual currency. Credits are the official currency used in Germany, France, The United States, Egypt, Canada and the Peoples Republic of China. They are mostly used digitally through the Internet and ATMs, but despite what their name implies, physical financial transactions can also be made through the use of Tangible Credit Chits - Green vouchers that may hold anywhere from cr50 to several hundred credits.\n\nPack Value: [#EDA723]10000";
		InfoPanel.OpenInfoPanel(m_ItemChosen, "CREDITS", "[#00E4B8]$2.99", null, "[#EDA723]10,000 CREDITS", desc);
	}

	public void Credits50000Info()
	{
		m_ItemChosen = StoreItem.Credits50000;
		string desc = "Credits are global virtual currency. Credits are the official currency used in Germany, France, The United States, Egypt, Canada and the Peoples Republic of China. They are mostly used digitally through the Internet and ATMs, but despite what their name implies, physical financial transactions can also be made through the use of Tangible Credit Chits - Green vouchers that may hold anywhere from cr50 to several hundred credits.\n\nPack Value: [#EDA723]50000";
		InfoPanel.OpenInfoPanel(m_ItemChosen, "CREDITS", "[#00E4B8]$4.99", null, "[#EDA723]50,000 CREDITS", desc);
	}

	public void CmbtAmmoBuy()
	{
		OpenPopUpToBuy(StoreItem.CmbtAmmo);
	}

	public void CrbwAmmoBuy()
	{
		OpenPopUpToBuy(StoreItem.CrbwAmmo);
	}

	public void GrenadeBuy()
	{
		OpenPopUpToBuy(StoreItem.Grenade);
	}

	public void PraxisKitBuy()
	{
		OpenPopUpToBuy(StoreItem.PraxisKit);
	}

	public void EnergyBarBuy()
	{
		OpenPopUpToBuy(StoreItem.EnergyBar);
	}

	public void BoozeBuy()
	{
		OpenPopUpToBuy(StoreItem.Booze);
	}

	public void CombatRifleBuy()
	{
		OpenPopUpToBuy(StoreItem.CombatRifle);
	}

	public void CrossbowBuy()
	{
		OpenPopUpToBuy(StoreItem.Crossbow);
	}

	public void AutoHackBuy()
	{
		OpenPopUpToBuy(StoreItem.AutoHack);
	}

	public void Credits500Buy()
	{
		OpenPopUpToBuy(StoreItem.Credits500);
	}

	public void Credits2500Buy()
	{
		OpenPopUpToBuy(StoreItem.Credits2500);
	}

	public void Credits10000Buy()
	{
		OpenPopUpToBuy(StoreItem.Credits10000);
	}

	public void Credits50000Buy()
	{
		OpenPopUpToBuy(StoreItem.Credits50000);
	}

	public static void OpenPopUpToBuy(StoreItem item)
	{
		m_This.m_ItemChosen = item;
		switch (m_This.m_ItemChosen)
		{
		case StoreItem.CmbtAmmo:
			PopUpPanel.OpenPopUp("[#FFFFFF]Are you sure you want to purchase [#EDA723]Combat Rifle Ammo (20) [#FFFFFF]for [#00E4B8]50V[#FFFFFF]?", 50, m_This.PopUpResult);
			break;
		case StoreItem.CrbwAmmo:
			PopUpPanel.OpenPopUp("[#FFFFFF]Are you sure you want to purchase [#EDA723]Crossbow Ammo (5) [#FFFFFF]for [#00E4B8]75V[#FFFFFF]?", 75, m_This.PopUpResult);
			break;
		case StoreItem.Grenade:
			PopUpPanel.OpenPopUp("[#FFFFFF]Are you sure you want to purchase a [#EDA723]Grenade [#FFFFFF]for [#00E4B8]150V[#FFFFFF]?", 150, m_This.PopUpResult);
			break;
		case StoreItem.PraxisKit:
			PopUpPanel.OpenPopUp("[#FFFFFF]Are you sure you want to purchase a [#EDA723]Praxis Kit [#FFFFFF]for [#00E4B8]10,000V[#FFFFFF]?", 10000, m_This.PopUpResult);
			break;
		case StoreItem.EnergyBar:
			PopUpPanel.OpenPopUp("[#FFFFFF]Are you sure you want to purchase an [#EDA723]Energy Bar [#FFFFFF]for [#00E4B8]25V[#FFFFFF]?", 25, m_This.PopUpResult);
			break;
		case StoreItem.Booze:
			PopUpPanel.OpenPopUp("[#FFFFFF]Are you sure you want to purchase [#EDA723]Booze [#FFFFFF]for [#00E4B8]5V[#FFFFFF]?", 5, m_This.PopUpResult);
			break;
		case StoreItem.CombatRifle:
			PopUpPanel.OpenPopUp("[#FFFFFF]Are you sure you want to purchase a [#EDA723]Combat Rifle [#FFFFFF]for [#00E4B8]1,260V[#FFFFFF]?", 1260, m_This.PopUpResult);
			break;
		case StoreItem.Crossbow:
			PopUpPanel.OpenPopUp("[#FFFFFF]Are you sure you want to purchase a [#EDA723]Crossbow [#FFFFFF]for [#00E4B8]2,000V[#FFFFFF]?", 2000, m_This.PopUpResult);
			break;
		case StoreItem.AutoHack:
			PopUpPanel.OpenPopUp("[#FFFFFF]Are you sure you want to purchase an [#EDA723]Auto Hack [#FFFFFF]for [#00E4B8]500V[#FFFFFF]?", 500, m_This.PopUpResult);
			break;
		case StoreItem.Credits500:
			PopUpPanel.OpenPopUp("[#FFFFFF]Are you sure you want to purchase [#EDA723]500 CREDITS [#FFFFFF]for [#00E4B8]$0.99[#FFFFFF]?", 0, m_This.PopUpResult, true);
			break;
		case StoreItem.Credits2500:
			PopUpPanel.OpenPopUp("[#FFFFFF]Are you sure you want to purchase [#EDA723]2,500 CREDITS [#FFFFFF]for [#00E4B8]$1.99[#FFFFFF]?", 0, m_This.PopUpResult, true);
			break;
		case StoreItem.Credits10000:
			PopUpPanel.OpenPopUp("[#FFFFFF]Are you sure you want to purchase [#EDA723]10,000 CREDITS [#FFFFFF]for [#00E4B8]$2.99[#FFFFFF]?", 0, m_This.PopUpResult, true);
			break;
		case StoreItem.Credits50000:
			PopUpPanel.OpenPopUp("[#FFFFFF]Are you sure you want to purchase [#EDA723]50,000 CREDITS [#FFFFFF]for [#00E4B8]$4.99[#FFFFFF]?", 0, m_This.PopUpResult, true);
			break;
		}
	}

	public void PopUpResult(bool choice)
	{
		if (choice)
		{
			if (m_ItemChosen == StoreItem.AutoHack)
			{
				Globals.m_Inventory.m_AutoHacks++;
				Globals.m_Inventory.m_Credits -= 500;
			}
			else if (m_ItemChosen == StoreItem.Booze)
			{
				Globals.m_Inventory.m_Booze++;
				Globals.m_Inventory.m_Credits -= 5;
			}
			else if (m_ItemChosen == StoreItem.CmbtAmmo)
			{
				Globals.m_PlayerController.AddAmmo(0, 20);
				Globals.m_Inventory.m_Credits -= 50;
			}
			else if (m_ItemChosen == StoreItem.CombatRifle)
			{
				Globals.m_Inventory.m_Credits -= 1260;
			}
			else if (m_ItemChosen == StoreItem.CrbwAmmo)
			{
				Globals.m_PlayerController.AddAmmo(1, 5);
				Globals.m_Inventory.m_Credits -= 75;
			}
			else if (m_ItemChosen == StoreItem.EnergyBar)
			{
				Globals.m_Inventory.m_EnergyBars++;
				Globals.m_Inventory.m_Credits -= 25;
			}
			else if (m_ItemChosen == StoreItem.PraxisKit)
			{
				Globals.m_Inventory.m_PraxisKits++;
				Globals.m_Inventory.m_Credits -= 10000;
			}
			else if (m_ItemChosen == StoreItem.Grenade)
			{
				Globals.m_Inventory.m_Grenades[Globals.m_PlayerController.m_CurrentGrenadeType]++;
				Globals.m_Inventory.m_Credits -= 150;
			}
			else if (m_ItemChosen == StoreItem.Crossbow)
			{
				Globals.m_Inventory.m_Credits -= 2000;
			}
			else if (m_ItemChosen == StoreItem.Credits500)
			{
				Globals.m_Inventory.m_Credits += 500;
			}
			else if (m_ItemChosen == StoreItem.Credits2500)
			{
				Globals.m_Inventory.m_Credits += 2500;
			}
			else if (m_ItemChosen == StoreItem.Credits10000)
			{
				Globals.m_Inventory.m_Credits += 10000;
			}
			else if (m_ItemChosen == StoreItem.Credits50000)
			{
				Globals.m_Inventory.m_Credits += 50000;
			}
			PauseTabs.UpdateCreditsValue();
		}
		m_ItemChosen = StoreItem.None;
	}
}
