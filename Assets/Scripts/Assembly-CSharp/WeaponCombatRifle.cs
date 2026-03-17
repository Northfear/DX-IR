public class WeaponCombatRifle : WeaponBase
{
	protected override void Awake()
	{
		m_WeaponType = WeaponType.CombatRifle;
		m_WeaponItemID = WeaponItemID.CombatRifle;
		m_FireSound3rdEventName = "Play_CombatRifle_3rd_Fire";
		m_FireSound3rdEventNameStop = "Stop_CombatRifle_3rd_Fire";
		m_FireSound1stEventName = "Play_CombatRifle_1st_Fire";
		m_FireSound1stEventNameStop = "Stop_CombatRifle_1st_Fire";
		m_ReloadSound3rdEventName = "Play_CombatRifle_3rd_Reload";
		m_ReloadSound1stEventName = "Play_CombatRifle_1st_Reload";
		m_HolsterSoundEventName = "Play_CombatRifle_1st_Holster";
		m_DrawSoundEventName = "Play_CombatRifle_1st_Draw";
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
