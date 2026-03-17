using UnityEngine;

public class DamageData
{
	public CharacterBase m_SourceEnemy;

	public Vector3 m_SourceLocation = Vector3.zero;

	public int m_Damage;

	public DamageType m_DamageType;

	public bool m_Headshot;

	public DamageData(CharacterBase enemy, Vector3 location, int damage, DamageType type = DamageType.Normal, bool headshot = false)
	{
		m_SourceEnemy = enemy;
		m_SourceLocation = location;
		m_Damage = damage;
		m_DamageType = type;
		m_Headshot = headshot;
	}
}
