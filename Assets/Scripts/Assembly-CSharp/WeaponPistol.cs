using UnityEngine;

public class WeaponPistol : WeaponBase
{
	protected override void Awake()
	{
		m_WeaponType = WeaponType.Pistol;
		m_WeaponItemID = WeaponItemID.Pistol;
		m_FireSound3rdEventName = "Play_Pistol_3rd_Fire";
		m_FireSound3rdEventNameStop = "Stop_Pistol_3rd_Fire";
		m_FireSound1stEventName = "Play_Pistol_1st_Fire";
		m_FireSound1stEventNameStop = "Stop_Pistol_1st_Fire";
		m_ReloadSound3rdEventName = "Play_Pistol_3rd_Reload";
		m_ReloadSound1stEventName = "Play_Pistol_1st_Reload";
		m_HolsterSoundEventName = "Play_Pistol_1st_Holster";
		m_DrawSoundEventName = "Play_Pistol_1st_Draw";
		m_CrouchModelLeftPosition = new Vector3(0.06653196f, -0.3416758f, 0.1464483f);
		m_StandModelLeftPosition = new Vector3(0.3187732f, -0.4162331f, 0.2563784f);
		m_StandModelRightPosition = new Vector3(-0.4092249f, -0.3235845f, 0.236013f);
		m_UpOverModelLeftPosition = new Vector3(-0.202106f, -0.4807356f, 0.1598782f);
		m_UpOverModelLeftRotation = new Vector3(356.7158f, 348.6215f, 358.0946f);
		m_UpOverModelRightPosition = new Vector3(0.06287808f, -0.5529872f, 0.1495789f);
		m_UpOverModelRightRotation = new Vector3(358.2082f, 3.512516f, 357.6178f);
		base.Awake();
	}

	public override string GetFirstPersonModelAnimName(FirstPersonAnimation anim)
	{
		switch (anim)
		{
		case FirstPersonAnimation.Idle:
			return "PistolIdle";
		case FirstPersonAnimation.Fire:
			return "PistolFire";
		case FirstPersonAnimation.Reload:
			return "PistolReload";
		case FirstPersonAnimation.Walk:
			return "PistolWalk";
		case FirstPersonAnimation.Draw:
			return "PistolHolster";
		case FirstPersonAnimation.Holster:
			return "PistolHolster";
		default:
			return base.GetFirstPersonModelAnimName(anim);
		}
	}

	public override string GetFirstPersonWeaponAnimName(FirstPersonAnimation anim)
	{
		switch (anim)
		{
		case FirstPersonAnimation.None:
			return "none";
		case FirstPersonAnimation.Idle:
			return "idle";
		case FirstPersonAnimation.Fire:
			return "fire";
		case FirstPersonAnimation.Reload:
			return "reload";
		case FirstPersonAnimation.Walk:
			return "walk";
		case FirstPersonAnimation.Draw:
			return "holster";
		case FirstPersonAnimation.Holster:
			return "holster";
		case FirstPersonAnimation.ThrowGrenade:
			return null;
		default:
			return null;
		}
	}
}
