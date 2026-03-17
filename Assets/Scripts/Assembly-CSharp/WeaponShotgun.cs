public class WeaponShotgun : WeaponBase
{
	private const int m_NumBulletsFired = 8;

	protected override void Awake()
	{
		m_WeaponType = WeaponType.Shotgun;
		m_WeaponItemID = WeaponItemID.Shotgun;
		m_FireSound3rdEventName = "Play_Shotgun_3rd_Fire";
		m_FireSound3rdEventNameStop = "Stop_Shotgun_3rd_Fire";
		m_FireSound1stEventName = "Play_Shotgun_1st_Fire";
		m_FireSound1stEventNameStop = "Stop_Shotgun_1st_Fire";
		m_ReloadSound3rdEventName = "Play_Shotgun_3rd_Reload";
		m_ReloadSound1stEventName = "Play_Shotgun_1st_Reload";
		m_HolsterSoundEventName = "Play_Shotgun_1st_Holster";
		m_DrawSoundEventName = "Play_Shotgun_1st_Draw";
		base.Awake();
	}

	protected override void FireBullet()
	{
		m_CurrentAmmo--;
		for (int i = 0; i < 8; i++)
		{
			FireHitScanBullet();
		}
		PlayerFiredBullet();
	}

	public override string GetFirstPersonModelAnimName(FirstPersonAnimation anim)
	{
		switch (anim)
		{
		case FirstPersonAnimation.Idle:
			return "ShotgunIdle";
		case FirstPersonAnimation.Fire:
			return "ShotgunFire";
		case FirstPersonAnimation.Reload:
			return "ShotgunReload";
		case FirstPersonAnimation.Walk:
			return "ShotgunWalk";
		case FirstPersonAnimation.Draw:
			return "ShotgunHolster";
		case FirstPersonAnimation.Holster:
			return "ShotgunHolster";
		default:
			return base.GetFirstPersonModelAnimName(anim);
		}
	}
}
