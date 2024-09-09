using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public EnemyType m_EnemyType = EnemyType.None;

	public Ethnicity m_Ethnicity;

	public EnemySquad m_EnemySquad = EnemySquad.A;

	public SpawnCondition m_SpawnCondition = SpawnCondition.None;

	public WeaponType m_WeaponType = WeaponType.None;

	public PatrolNode m_StartingPatrolNode;

	public bool SingleSpawn = true;

	[HideInInspector]
	public GameObject m_EnemyPrefab;

	[HideInInspector]
	public GameObject m_WeaponPrefab;

	private void Awake()
	{
		if (m_EnemyType == EnemyType.None || m_EnemyPrefab == null)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (m_SpawnCondition == SpawnCondition.AtStart && m_EnemyType != EnemyType.None && m_EnemyPrefab != null)
		{
			Spawn();
		}
	}

	public void Spawn()
	{
		GameObject gameObject = Object.Instantiate(m_EnemyPrefab, base.transform.position, base.transform.rotation) as GameObject;
		gameObject.name = base.name;
		Enemy_Base component = gameObject.GetComponent<Enemy_Base>();
		component.m_EnemySquad = m_EnemySquad;
		component.m_Ethnicity = m_Ethnicity;
		component.m_StartingNode = m_StartingPatrolNode;
		if (m_WeaponType != WeaponType.None && m_WeaponPrefab != null)
		{
			gameObject = Object.Instantiate(m_WeaponPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			WeaponBase component2 = gameObject.GetComponent<WeaponBase>();
			component.AssignWeapon(component2);
		}
		if (SingleSpawn)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
