public class WeaponShotgun : WeaponBase
{
	private const int m_NumBulletsFired = 8;

	protected override void Awake()
	{
		m_WeaponType = WeaponType.Shotgun;
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
