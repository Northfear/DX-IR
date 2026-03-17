using UnityEngine;

public class WeaponCrossbow : WeaponBase
{
	protected override void Awake()
	{
		m_WeaponType = WeaponType.Crossbow;
		m_WeaponItemID = WeaponItemID.Crossbow;
		m_FireSound3rdEventName = "Play_Crossbow_3rd_Fire";
		m_FireSound3rdEventNameStop = "Stop_Crossbow_3rd_Fire";
		m_FireSound1stEventName = "Play_Crossbow_1st_Fire";
		m_FireSound1stEventNameStop = "Stop_Crossbow_1st_Fire";
		m_ReloadSound3rdEventName = "Play_Crossbow_3rd_Reload";
		m_ReloadSound1stEventName = "Play_Crossbow_1st_Reload";
		m_HolsterSoundEventName = "Play_Crossbow_1st_Holster";
		m_DrawSoundEventName = "Play_Crossbow_1st_Draw";
		base.Awake();
	}

	protected override void PlayerUpdate()
	{
		base.PlayerUpdate();
		if (m_CurrentAmmo <= 0 && Globals.m_PlayerController.GetAmmoForCurrentWeapon() <= 0 && m_ModelThirdPersonPlayer.active)
		{
			m_ModelThirdPersonPlayer.animation["CRB_TP_Empty_idle"].wrapMode = WrapMode.Loop;
			m_ModelThirdPersonPlayer.animation.Play("CRB_TP_Empty_idle");
		}
	}

	protected override void FireBullet()
	{
		base.FireBullet();
	}

	public override string GetFirstPersonModelAnimName(FirstPersonAnimation anim)
	{
		switch (anim)
		{
		case FirstPersonAnimation.Idle:
			return "CrossbowIdle";
		case FirstPersonAnimation.Fire:
			return "CrossbowFire";
		case FirstPersonAnimation.Reload:
			return "CrossbowReload";
		case FirstPersonAnimation.Walk:
			return "CrossbowWalk";
		case FirstPersonAnimation.Draw:
			return "CrossbowDraw";
		case FirstPersonAnimation.Holster:
			return "CrossbowHolster";
		default:
			return base.GetFirstPersonModelAnimName(anim);
		}
	}

	public override float GetFirstPersonAnimSpeedMod(FirstPersonAnimation anim)
	{
		switch (anim)
		{
		case FirstPersonAnimation.Idle:
			return 1f;
		case FirstPersonAnimation.Fire:
			return 1f;
		case FirstPersonAnimation.Reload:
			return 1f;
		case FirstPersonAnimation.Walk:
			return 1f;
		case FirstPersonAnimation.Draw:
			return 1f;
		case FirstPersonAnimation.Holster:
			return 1f;
		case FirstPersonAnimation.None:
			return 1f;
		default:
			return 1f;
		}
	}

	public override string GetFirstPersonWeaponAnimName(FirstPersonAnimation anim)
	{
		if (m_CurrentAmmo == 0 && Globals.m_PlayerController.GetAmmoForCurrentWeapon() == 0)
		{
			switch (anim)
			{
			case FirstPersonAnimation.Idle:
				return "Idle_Empty";
			case FirstPersonAnimation.Fire:
				return "Fire";
			case FirstPersonAnimation.Reload:
				return "Reload";
			case FirstPersonAnimation.Walk:
				return "Walk_Empty";
			case FirstPersonAnimation.Draw:
				return "Draw_Empty";
			case FirstPersonAnimation.Holster:
				return "Holster_Empty";
			case FirstPersonAnimation.None:
				return "none";
			}
		}
		else
		{
			switch (anim)
			{
			case FirstPersonAnimation.Idle:
				return "Idle";
			case FirstPersonAnimation.Fire:
				return "Fire";
			case FirstPersonAnimation.Reload:
				return "Reload";
			case FirstPersonAnimation.Walk:
				return "Walk";
			case FirstPersonAnimation.Draw:
				return "Draw";
			case FirstPersonAnimation.Holster:
				return "Holster";
			case FirstPersonAnimation.None:
				return "none";
			}
		}
		return null;
	}
}
