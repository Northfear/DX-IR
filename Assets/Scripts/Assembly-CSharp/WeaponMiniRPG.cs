using UnityEngine;

public class WeaponMiniRPG : WeaponBase
{
	protected override void Awake()
	{
		m_WeaponType = WeaponType.MiniRPG;
		m_WeaponItemID = WeaponItemID.MiniRPG;
		m_FireSound3rdEventName = "Play_MiniRPG_3rd_Fire";
		m_FireSound3rdEventNameStop = "Stop_MiniRPG_3rd_Fire";
		m_FireSound1stEventName = "Play_MiniRPG_1st_Fire";
		m_FireSound1stEventNameStop = "Stop_MiniRPG_1st_Fire";
		m_ReloadSound3rdEventName = "Play_MiniRPG_3rd_Reload";
		m_ReloadSound1stEventName = "Play_MiniRPG_1st_Reload";
		m_HolsterSoundEventName = "Play_MiniRPG_1st_Holster";
		m_DrawSoundEventName = "Play_MiniRPG_1st_Draw";
		m_StandModelLeftRotation = new Vector3(354.2772f, 353.2472f, 1.553542f);
		m_UpOverModelLeftPosition = new Vector3(-0.184895f, -0.4613907f, 0.1588104f);
		m_UpOverModelLeftRotation = new Vector3(0.3618607f, 348.8264f, 358.0927f);
		base.Awake();
	}

	public override string GetFirstPersonModelAnimName(FirstPersonAnimation anim)
	{
		switch (anim)
		{
		case FirstPersonAnimation.Idle:
			return "RPGIdle";
		case FirstPersonAnimation.Fire:
			return "RPGFire";
		case FirstPersonAnimation.Reload:
			return "RPGReload";
		case FirstPersonAnimation.Walk:
			return "RPGWalk";
		case FirstPersonAnimation.Draw:
			return "RPGHolster";
		case FirstPersonAnimation.Holster:
			return "RPGHolster";
		default:
			return base.GetFirstPersonModelAnimName(anim);
		}
	}
}
