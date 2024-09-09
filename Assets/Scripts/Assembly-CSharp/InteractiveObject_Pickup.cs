using Fabric;
using UnityEngine;

public class InteractiveObject_Pickup : InteractiveObject_Base
{
	public enum PickupMainType
	{
		None = 0,
		Weapon = 1,
		Ammo = 2,
		Generic = 3,
		AmmoCombatRifle = 4,
		AmmoCrossbow = 5,
		Grenades = 6,
		Credits = 7
	}

	public enum PickupFabricEvent
	{
		PU_Generic = 0,
		PU_CombatRifle = 1,
		PU_Grenade = 2,
		PU_Ammo = 3
	}

	public WeaponBase m_WeaponBase;

	public PickupMainType m_PickupMainType;

	public PickupFabricEvent m_PickupSound;

	private void Update()
	{
	}

	public override bool EnableInteractiveObject()
	{
		if (!base.EnableInteractiveObject())
		{
			return false;
		}
		return true;
	}

	public override bool DisableInteractiveObject()
	{
		if (!base.DisableInteractiveObject())
		{
			return false;
		}
		return true;
	}

	public override bool InteractWithObject()
	{
		if (!base.InteractWithObject())
		{
			return false;
		}
		switch (m_PickupMainType)
		{
		case PickupMainType.Ammo:
			Globals.m_PlayerController.GiveAmmo(m_WeaponBase.GetWeaponType(), m_WeaponBase.GetAmmo());
			break;
		case PickupMainType.AmmoCombatRifle:
			Globals.m_PlayerController.GiveAmmo(WeaponType.CombatRifle, Random.Range(3, 10));
			break;
		case PickupMainType.AmmoCrossbow:
			Globals.m_PlayerController.GiveAmmo(WeaponType.Crossbow, Random.Range(1, 2));
			break;
		case PickupMainType.Grenades:
			Globals.m_Inventory.m_Grenades[Globals.m_PlayerController.m_CurrentGrenadeType]++;
			Globals.m_HUD.SetGrenadeIcon();
			break;
		case PickupMainType.Credits:
			Globals.m_Inventory.m_Credits += Random.Range(20, 150);
			break;
		}
		EventManager.Instance.PostEvent(m_PickupSound.ToString(), EventAction.PlaySound, null, Globals.m_PlayerController.gameObject);
		m_Active = false;
		m_MarkedForDelete = true;
		Globals.m_InteractiveObjectManager.DisableInteractivePopup(this);
		return true;
	}
}
