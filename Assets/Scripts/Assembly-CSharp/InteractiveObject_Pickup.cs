using System.Collections.Generic;
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
		FragGrenade = 6,
		EMPGrenade = 7,
		ConcussionGrenade = 8,
		Credits = 9,
		FragMine = 10,
		EMPMine = 11,
		ConcussionMine = 12
	}

	public enum PickupSoundEvent
	{
		Play_PU_Generic = 0,
		Play_PU_CombatRifle = 1,
		Play_PU_Grenade = 2,
		Play_PU_Ammo = 3
	}

	public WeaponBase m_WeaponBase;

	public PickupMainType m_PickupMainType;

	public PickupSoundEvent m_PickupSound;

	public static LinkedList<InteractiveObject_Pickup> m_LevelPickups = new LinkedList<InteractiveObject_Pickup>();

	public static LinkedList<InteractiveObject_Pickup> m_SpawnedPickups = new LinkedList<InteractiveObject_Pickup>();

	public static LinkedList<Vector3> m_DestroyedPickups = new LinkedList<Vector3>();

	public override void MarkForDelete()
	{
		base.MarkForDelete();
		if (GetInstanceID() >= 0)
		{
			m_DestroyedPickups.AddLast(m_SpawnPosition);
		}
	}

	private void Start()
	{
		if (GetInstanceID() < 0)
		{
			m_SpawnedPickups.AddLast(this);
		}
		else
		{
			m_LevelPickups.AddLast(this);
		}
	}

	public override bool EnableInteractiveObject(GameObject livingEntity, bool calledByPlayer = true)
	{
		if (!base.EnableInteractiveObject(livingEntity, calledByPlayer))
		{
			return false;
		}
		return true;
	}

	public override bool DisableInteractiveObject(GameObject livingEntity, bool calledByPlayer = true)
	{
		if (!base.DisableInteractiveObject(livingEntity, calledByPlayer))
		{
			return false;
		}
		return true;
	}

	public override bool InteractWithObject(bool instant = false)
	{
		if (!base.InteractWithObject(instant))
		{
			return false;
		}
		switch (m_PickupMainType)
		{
		case PickupMainType.Ammo:
			Globals.m_PlayerController.GiveAmmo(m_WeaponBase.GetWeaponType(), m_WeaponBase.m_DroppedAmmo);
			break;
		case PickupMainType.AmmoCombatRifle:
			Globals.m_PlayerController.GiveAmmo(WeaponType.CombatRifle, Random.Range(3, 10));
			break;
		case PickupMainType.AmmoCrossbow:
			Globals.m_PlayerController.GiveAmmo(WeaponType.Crossbow, Random.Range(1, 2));
			break;
		case PickupMainType.FragGrenade:
			Globals.m_Inventory.AdjustItemQuantity(1, 3, 1);
			Globals.m_HUD.SetGrenadeIcon();
			break;
		case PickupMainType.EMPGrenade:
			Globals.m_Inventory.AdjustItemQuantity(1, 4, 1);
			Globals.m_HUD.SetGrenadeIcon();
			break;
		case PickupMainType.ConcussionGrenade:
			Globals.m_Inventory.AdjustItemQuantity(1, 5, 1);
			Globals.m_HUD.SetGrenadeIcon();
			break;
		case PickupMainType.Credits:
			Globals.m_Inventory.AdjustCredits(Random.Range(20, 150));
			break;
		case PickupMainType.FragMine:
			Globals.m_Inventory.AdjustItemQuantity(1, 10, 1);
			Globals.m_HUD.SetGrenadeIcon();
			SoundManager.TriggerEvent("Play_Mine_Deactivate");
			break;
		case PickupMainType.EMPMine:
			Globals.m_Inventory.AdjustItemQuantity(1, 11, 1);
			Globals.m_HUD.SetGrenadeIcon();
			SoundManager.TriggerEvent("Play_Mine_Deactivate");
			break;
		case PickupMainType.ConcussionMine:
			Globals.m_Inventory.AdjustItemQuantity(1, 12, 1);
			Globals.m_HUD.SetGrenadeIcon();
			SoundManager.TriggerEvent("Play_Mine_Deactivate");
			break;
		}
		SoundManager.TriggerEvent(m_PickupSound.ToString(), Globals.m_PlayerController.gameObject);
		m_Active = false;
		MarkForDelete();
		Globals.m_InteractiveObjectManager.DisableInteractivePopup(this);
		return true;
	}
}
