public class Enemy_RiotCop : Enemy_Base
{
	public override void Awake()
	{
		m_EnemyType = EnemyType.RiotCop;
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		m_DamageModifiers[0] = 0.8f;
		m_DamageModifiers[1] = 1f;
		m_DamageModifiers[2] = 0.9f;
		m_DamageModifiers[3] = 0f;
		m_DamageModifiers[4] = 0f;
		m_DamageModifiers[6] = 1f;
		m_DamageModifiers[5] = 1f;
	}
}
