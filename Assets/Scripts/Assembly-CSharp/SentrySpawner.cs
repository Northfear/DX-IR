using UnityEngine;

public class SentrySpawner : MonoBehaviour
{
	public SpawnCondition m_SpawnCondition = SpawnCondition.None;

	public Sentry_PatrolNode m_StartingPatrolNode;

	public bool m_SingleSpawn = true;

	public GameObject m_SentryPrefab;

	public EnemySquad m_EnemySquad = EnemySquad.A;

	public MachineAwareness m_SentryAwareness;

	public bool m_SpawnNeutral;

	public InteractiveObject_Domination m_ConnectedDominationPanel;

	private void Awake()
	{
		if (m_SentryPrefab == null)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (m_SpawnCondition == SpawnCondition.AtStart && m_SentryPrefab != null)
		{
			Spawn();
		}
	}

	public void Spawn()
	{
		GameObject gameObject = Object.Instantiate(m_SentryPrefab, base.transform.position, base.transform.rotation) as GameObject;
		gameObject.name = base.name;
		Sentry component = gameObject.GetComponent<Sentry>();
		if ((bool)component)
		{
			component.m_EnemySquad = m_EnemySquad;
			component.m_TargetNode = m_StartingPatrolNode;
			component.m_Awareness = m_SentryAwareness;
			component.m_Neutral = m_SpawnNeutral;
		}
		if ((bool)m_ConnectedDominationPanel)
		{
			m_ConnectedDominationPanel.RegisterSentry(component);
		}
		if (m_SingleSpawn)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
