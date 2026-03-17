using UnityEngine;

public class BoxBot_Spawner : MonoBehaviour
{
	public SpawnCondition m_SpawnCondition = SpawnCondition.None;

	public BoxBot_PatrolNode m_StartingPatrolNode;

	public bool m_SingleSpawn = true;

	public EnemySquad m_EnemySquad = EnemySquad.A;

	public MachineAwareness m_BoxRobotAwareness;

	public bool m_SpawnNeutral;

	public InteractiveObject_Domination m_ConnectedDominationPanel;

	public GameObject m_BoxBotPrefab;

	private void Awake()
	{
		if (m_BoxBotPrefab == null)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (m_SpawnCondition == SpawnCondition.AtStart && m_BoxBotPrefab != null)
		{
			Spawn();
		}
	}

	public void Spawn()
	{
		GameObject gameObject = Object.Instantiate(m_BoxBotPrefab, base.transform.position, base.transform.rotation) as GameObject;
		gameObject.name = base.name;
		BoxRobot component = gameObject.GetComponent<BoxRobot>();
		if ((bool)component)
		{
			component.m_EnemySquad = m_EnemySquad;
			component.m_TargetNode = m_StartingPatrolNode;
			component.m_Neutral = m_SpawnNeutral;
			component.m_Awareness = m_BoxRobotAwareness;
		}
		if (m_SingleSpawn)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
