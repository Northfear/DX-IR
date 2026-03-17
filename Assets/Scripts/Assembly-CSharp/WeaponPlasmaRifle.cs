using UnityEngine;

public class WeaponPlasmaRifle : WeaponBase
{
	public GameObject m_PlasmaVentFX;

	public GameObject m_PlasmaVentFX_FP;

	protected override void Awake()
	{
		m_WeaponType = WeaponType.PlasmaRifle;
		m_WeaponItemID = WeaponItemID.PlasmaRifle;
		m_FireSound3rdEventName = "Play_PlasmaRifle_3rd_Fire";
		m_FireSound3rdEventNameStop = "Stop_PlasmaRifle_3rd_Fire";
		m_FireSound1stEventName = "Play_PlasmaRifle_1st_Fire";
		m_FireSound1stEventNameStop = "Stop_PlasmaRifle_1st_Fire";
		m_ReloadSound3rdEventName = "Play_PlasmaRifle_3rd_Reload";
		m_ReloadSound1stEventName = "Play_PlasmaRifle_1st_Reload";
		m_HolsterSoundEventName = "Play_PlasmaRifle_1st_Holster";
		m_DrawSoundEventName = "Play_PlasmaRifle_1st_Draw";
		m_CrouchModelLeftPosition = new Vector3(0.05523365f, -0.3970734f, 0.2231815f);
		m_CrouchModelLeftRotation = new Vector3(354.4166f, 350.3462f, 1.9612f);
		m_StandModelLeftPosition = new Vector3(0.2837372f, -0.3945174f, 0.209f);
		m_StandModelLeftRotation = new Vector3(354.428f, 350.0138f, 1.993508f);
		base.Awake();
	}

	protected override void FireBullet()
	{
		base.FireBullet();
		GameObject gameObject = ((!(m_User == Globals.m_PlayerController) || Globals.m_PlayerController.m_CameraMode != PlayerController.CameraMode.First) ? ((GameObject)Object.Instantiate(m_PlasmaVentFX, new Vector3(0f, 0f, 0f), Quaternion.identity)) : ((GameObject)Object.Instantiate(m_PlasmaVentFX_FP, new Vector3(0f, 0f, 0f), Quaternion.identity)));
		gameObject.transform.parent = m_MuzzleFlashAttachObject.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
	}

	public override string GetFirstPersonModelAnimName(FirstPersonAnimation anim)
	{
		switch (anim)
		{
		case FirstPersonAnimation.Idle:
			return "PlasmaRifleIdle";
		case FirstPersonAnimation.Fire:
			return "PlasmaRifleFire";
		case FirstPersonAnimation.Reload:
			return "PlasmaRifleReload";
		case FirstPersonAnimation.Walk:
			return "PlasmaRifleWalk";
		case FirstPersonAnimation.Draw:
			return "PlasmaRifleDraw";
		case FirstPersonAnimation.Holster:
			return "PlasmaRifleHolster";
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
		default:
			return null;
		}
	}
}
