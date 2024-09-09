using System.Collections.Generic;

public class Squad
{
	public bool m_Suspicious;

	public bool m_Alarmed;

	public bool m_InCombat;

	public bool m_Hostile;

	public LinkedList<Enemy_Base> m_EnemyUnits = new LinkedList<Enemy_Base>();

	public Enemy_Base m_Leader;
}
