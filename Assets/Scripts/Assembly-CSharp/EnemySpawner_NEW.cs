using UnityEngine;

public class EnemySpawner_NEW : MonoBehaviour
{
	public EnemyType m_EnemyType = EnemyType.None;

	public Ethnicity m_Ethnicity;

	public EnemySquad_NEW m_EnemySquad;

	public SpawnCondition m_SpawnCondition = SpawnCondition.None;

	public WeaponType m_WeaponType = WeaponType.None;

	public PatrolNode m_StartingPatrolNode;

	public bool m_SpawnNeutral;

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

	public EnemyBase Spawn()
	{
		GameObject gameObject = Object.Instantiate(m_EnemyPrefab, base.transform.position, base.transform.rotation) as GameObject;
		gameObject.name = base.name;
		EnemyBase component = gameObject.GetComponent<EnemyBase>();
		component.m_EnemySquad = m_EnemySquad;
		component.m_Ethnicity = m_Ethnicity;
		component.m_TargetNode = m_StartingPatrolNode;
		component.m_Neutral = m_SpawnNeutral;
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
		return component;
	}
}
