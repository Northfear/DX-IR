using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public EnemyType m_EnemyType = EnemyType.None;

	public Ethnicity m_Ethnicity;

	public EnemySquad m_EnemySquad = EnemySquad.A;

	public SpawnCondition m_SpawnCondition = SpawnCondition.None;

	public WeaponType m_WeaponType = WeaponType.None;

	public PatrolNode m_StartingPatrolNode;

	public bool m_SpawnNeutral;

	[HideInInspector]
	public Enemy_Base m_EnemyPrefab;

	[HideInInspector]
	public Animation m_EnemyAnimator;

	[HideInInspector]
	public WeaponBase m_WeaponPrefab;

	public static LinkedList<EnemySpawner> m_Spawners = new LinkedList<EnemySpawner>();

	private void Awake()
	{
		if (m_EnemyType == EnemyType.None || m_EnemyPrefab == null || m_WeaponPrefab == null || m_EnemyAnimator == null)
		{
			Debug.LogWarning("EnemySpawner not setup correctly: " + base.name);
			Object.Destroy(base.gameObject);
			return;
		}
		m_Spawners.AddLast(this);
		if (GameManager.m_LoadSaveAfterLevelLoad || GameManager.m_LoadSceneSetupAfterLevelLoad)
		{
			base.gameObject.SetActiveRecursively(false);
		}
	}

	private void OnDestroy()
	{
		m_Spawners.Remove(this);
	}

	private void Start()
	{
		if (m_SpawnCondition == SpawnCondition.AtStart && m_EnemyType != EnemyType.None && m_EnemyPrefab != null && m_WeaponPrefab != null && m_EnemyAnimator != null)
		{
			Spawn();
		}
	}

	public Enemy_Base Spawn()
	{
		if (m_EnemyPrefab == null || m_WeaponPrefab == null || m_EnemyAnimator == null)
		{
			return null;
		}
		GameObject gameObject = Object.Instantiate(m_EnemyPrefab.gameObject, base.transform.position, base.transform.rotation) as GameObject;
		gameObject.name = base.name;
		Enemy_Base component = gameObject.GetComponent<Enemy_Base>();
		component.m_EnemySquad = m_EnemySquad;
		component.m_Ethnicity = m_Ethnicity;
		component.m_StartingNode = m_StartingPatrolNode;
		component.m_Neutral = m_SpawnNeutral;
		component.m_SpawnLocation = base.transform.position;
		Globals.m_AIDirector.EnemySpawned(component);
		gameObject = Object.Instantiate(m_WeaponPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
		WeaponBase component2 = gameObject.GetComponent<WeaponBase>();
		component.AssignWeapon(component2);
		m_EnemyAnimator.transform.parent = component.m_Animator.transform.parent;
		m_EnemyAnimator.transform.localPosition = component.m_Animator.transform.localPosition;
		m_EnemyAnimator.transform.localRotation = component.m_Animator.transform.localRotation;
		m_EnemyAnimator.transform.localScale = component.m_Animator.transform.localScale;
		while (component.m_Animator.transform.GetChildCount() > 0)
		{
			component.m_Animator.transform.GetChild(0).parent = m_EnemyAnimator.transform;
		}
		Object.Destroy(component.m_Animator.gameObject);
		component.m_Animator = m_EnemyAnimator;
		Object.Destroy(base.gameObject);
		return component;
	}
}
