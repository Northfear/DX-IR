public class WeaponCombatRifle : WeaponBase
{
	protected override void Awake()
	{
		m_WeaponType = WeaponType.CombatRifle;
		base.Awake();
	}

	public override string GetFirstPersonModelAnimName(FirstPersonAnimation anim)
	{
		switch (anim)
		{
		case FirstPersonAnimation.Idle:
			return "CombatRifleIdle";
		case FirstPersonAnimation.Fire:
			return "CombatRifleFire";
		case FirstPersonAnimation.Reload:
			return "CombatRifleReload";
		case FirstPersonAnimation.Walk:
			return "CombatRifleWalk";
		case FirstPersonAnimation.Draw:
			return "CombatRifleHolster";
		case FirstPersonAnimation.Holster:
			return "CombatRifleHolster";
		default:
			return base.GetFirstPersonModelAnimName(anim);
		}
	}
}
