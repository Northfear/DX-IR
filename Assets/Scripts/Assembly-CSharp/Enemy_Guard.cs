public class Enemy_Guard : Enemy_Base
{
	public override void Awake()
	{
		m_EnemyType = EnemyType.Guard;
		base.Awake();
	}
}
