public class Enemy_Gangster : Enemy_Base
{
	public override void Awake()
	{
		m_EnemyType = EnemyType.Gangster;
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		m_DamageModifiers[0] = 1f;
		m_DamageModifiers[1] = 1f;
		m_DamageModifiers[2] = 1f;
		m_DamageModifiers[3] = 0f;
		m_DamageModifiers[4] = 0f;
		m_DamageModifiers[6] = 1f;
		m_DamageModifiers[5] = 1f;
	}
}
