using System.Collections.Generic;
using UnityEngine;

public class AIDirector_NEW : MonoBehaviour
{
	private Squad_NEW[] m_Squads = new Squad_NEW[7];

	[HideInInspector]
	public Vector3 m_LastKnownPlayerPosition = Vector3.zero;

	private void Awake()
	{
		Globals_NEW.m_AIDirector = this;
		for (int i = 0; i < 7; i++)
		{
			m_Squads[i] = new Squad_NEW();
		}
	}

	public LinkedListNode<EnemyBase> GetFirstEnemy(int SquadID)
	{
		return m_Squads[SquadID].m_EnemyUnits.First;
	}

	public void EnemySpawned(EnemyBase enemy)
	{
		m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.AddLast(enemy);
	}

	public void UpdatePlayerKnownPosition()
	{
		m_LastKnownPlayerPosition = Globals.m_PlayerController.transform.position;
	}
}
