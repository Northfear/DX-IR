public class WeaponHeavyRifle : WeaponBase
{
	protected override void Awake()
	{
		m_WeaponType = WeaponType.HeavyRifle;
		m_WeaponItemID = WeaponItemID.HeavyRifle;
		m_FireSound3rdEventName = "Play_CombatRifle_3rd_Fire";
		m_FireSound3rdEventNameStop = "Stop_CombatRifle_3rd_Fire";
		m_FireSound1stEventName = "Play_CombatRifle_1st_Fire";
		m_FireSound1stEventNameStop = "Stop_CombatRifle_1st_Fire";
		base.Awake();
	}
}
