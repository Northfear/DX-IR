using UnityEngine;

public class SentrySpawner : MonoBehaviour
{
	public SpawnCondition m_SpawnCondition = SpawnCondition.None;

	public Sentry_PatrolNode m_StartingPatrolNode;

	public bool m_SingleSpawn = true;

	public GameObject m_SentryPrefab;

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
			component.m_TargetNode = m_StartingPatrolNode;
		}
		if (m_SingleSpawn)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
