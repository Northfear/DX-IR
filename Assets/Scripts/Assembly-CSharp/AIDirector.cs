using System;
using System.Collections.Generic;
using Fabric;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
	public GameObject m_FragGrenadePrefab;

	public GameObject m_EMPGrenadePrefab;

	public GameObject m_ConcussionGrenadePrefab;

	private Squad[] m_Squads = new Squad[8];

	[HideInInspector]
	public bool m_AnySuspiciousEnemies;

	[HideInInspector]
	public bool m_AnyHostileEnemies;

	[HideInInspector]
	public bool m_AnyAlarmedEnemies;

	[HideInInspector]
	public bool m_AnyInCombatEnemies;

	private bool m_CoverCalculatedThisFrame;

	private LinkedList<NearbyCover> m_CoverNodes = new LinkedList<NearbyCover>();

	public float m_MaxCoverDistance = 20f;

	private float m_MaxCoverDistanceSqr;

	public float m_ValidCoverAngle = 45f;

	[HideInInspector]
	public float m_MinCoverFacingDot;

	private LinkedList<NearbyCover> m_NearbyCover = new LinkedList<NearbyCover>();

	public float m_MaxSearchDistance = 25f;

	private float m_MaxSearchDistanceSqr;

	private LinkedList<NearbySearch> m_SearchNodes = new LinkedList<NearbySearch>();

	private LinkedList<NearbySearch> m_NearbySearch = new LinkedList<NearbySearch>();

	[HideInInspector]
	public Vector3 m_LastKnownPlayerPosition = Vector3.zero;

	public float m_MinimumAlertSoundInterval = 60f;

	private float m_AlertSoundTimer;

	[HideInInspector]
	public float m_GrenadeCooldownTimer;

	private LinkedList<GrenadeFrag> m_ActiveGrenades = new LinkedList<GrenadeFrag>();

	private LinkedList<Turret> m_Turrets = new LinkedList<Turret>();

	private LinkedList<Sentry> m_Sentries = new LinkedList<Sentry>();

	private LinkedList<NPC_Base> m_NPCs = new LinkedList<NPC_Base>();

	private LinkedList<SecurityCamera> m_SecurityCameras = new LinkedList<SecurityCamera>();

	private void Awake()
	{
		Globals.m_AIDirector = this;
		m_MaxSearchDistanceSqr = m_MaxSearchDistance * m_MaxSearchDistance;
		m_MaxCoverDistanceSqr = m_MaxCoverDistance * m_MaxCoverDistance;
		m_MinCoverFacingDot = Mathf.Cos((float)Math.PI / 180f * m_ValidCoverAngle);
		for (int i = 0; i < 8; i++)
		{
			m_Squads[i] = new Squad();
		}
	}

	private void Update()
	{
		m_AlertSoundTimer -= Time.deltaTime;
		m_GrenadeCooldownTimer -= Time.deltaTime;
	}

	private void LateUpdate()
	{
		m_CoverCalculatedThisFrame = false;
	}

	public WarningLevel GetWarningLevel()
	{
		if (m_AnyInCombatEnemies)
		{
			return WarningLevel.Hostile;
		}
		if (m_AnyAlarmedEnemies)
		{
			return WarningLevel.Alarmed;
		}
		if (m_AnyHostileEnemies)
		{
			return WarningLevel.Alarmed;
		}
		if (m_AnySuspiciousEnemies)
		{
			return WarningLevel.Alarmed;
		}
		return WarningLevel.Passive;
	}

	public LinkedList<NearbyCover> GetCoverList(Vector3 ThreatLocation)
	{
		if (!m_CoverCalculatedThisFrame)
		{
			GatherNearbyCover(ThreatLocation);
			m_CoverCalculatedThisFrame = true;
		}
		return m_NearbyCover;
	}

	private void GatherNearbyCover(Vector3 ThreatLocation)
	{
		m_NearbyCover.Clear();
		for (LinkedListNode<NearbyCover> linkedListNode = m_CoverNodes.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			Vector3 vector = ThreatLocation - linkedListNode.Value.m_Cover.transform.position;
			if (vector.sqrMagnitude <= m_MaxCoverDistanceSqr && Vector3.Dot(vector.normalized, linkedListNode.Value.m_Cover.transform.forward) >= m_MinCoverFacingDot)
			{
				linkedListNode.Value.m_DistanceSqr = vector.sqrMagnitude;
				m_NearbyCover.AddLast(linkedListNode.Value);
			}
		}
	}

	public void AddCoverNode(CoverNode node)
	{
		NearbyCover nearbyCover = new NearbyCover();
		nearbyCover.m_Cover = node;
		nearbyCover.m_Enemy = null;
		m_CoverNodes.AddLast(nearbyCover);
	}

	public SearchNode GetSearchNode()
	{
		if (m_NearbySearch.First != null)
		{
			SearchNode searchNode = m_NearbySearch.First.Value.m_SearchNode;
			m_NearbySearch.RemoveFirst();
			return searchNode;
		}
		return null;
	}

	public void AddSearchNode(SearchNode node)
	{
		NearbySearch nearbySearch = new NearbySearch();
		nearbySearch.m_SearchNode = node;
		m_SearchNodes.AddLast(nearbySearch);
	}

	private void GatherSearchNodes(Vector3 PositionToSearchAround)
	{
		m_NearbySearch.Clear();
		for (LinkedListNode<NearbySearch> linkedListNode = m_SearchNodes.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			linkedListNode.Value.m_DistanceSqr = (PositionToSearchAround - linkedListNode.Value.m_SearchNode.transform.position).sqrMagnitude;
			if (linkedListNode.Value.m_DistanceSqr <= m_MaxSearchDistanceSqr)
			{
				if (m_NearbySearch.First == null)
				{
					m_NearbySearch.AddLast(linkedListNode.Value);
				}
				else
				{
					for (LinkedListNode<NearbySearch> linkedListNode2 = m_NearbySearch.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
					{
						if (linkedListNode2.Next == null || linkedListNode2.Next.Value.m_DistanceSqr >= linkedListNode.Value.m_DistanceSqr)
						{
							m_NearbySearch.AddAfter(linkedListNode2, linkedListNode.Value);
							break;
						}
					}
				}
			}
		}
	}

	public void AddGrenade(GrenadeFrag grenade)
	{
		m_ActiveGrenades.AddLast(grenade);
	}

	public void RemoveGrenade(GrenadeFrag grenade)
	{
		m_ActiveGrenades.Remove(grenade);
	}

	public LinkedListNode<GrenadeFrag> GetFirstGrenade()
	{
		return m_ActiveGrenades.First;
	}

	public void GrenadeDetonated(GrenadeFrag grenade)
	{
		Vector3 vector = grenade.transform.position + Vector3.up * 0.1f;
		float sqrMagnitude = (Globals.m_PlayerController.transform.position - grenade.transform.position).sqrMagnitude;
		Ray ray = new Ray(vector, Globals.m_PlayerController.GetChestLocation() - vector);
		RaycastHit hitInfo;
		if (sqrMagnitude <= grenade.m_MinRadiusSqr)
		{
			Globals.m_PlayerController.TakeDamage(grenade.CalculateDamage(sqrMagnitude), grenade.gameObject, grenade.GetDamageType());
		}
		else if (sqrMagnitude <= grenade.m_MaxRadiusSqr && Physics.Raycast(ray, out hitInfo, grenade.m_MaxRadius * 1.1f, 16641) && hitInfo.collider.gameObject.layer == 14)
		{
			Globals.m_PlayerController.TakeDamage(grenade.CalculateDamage(sqrMagnitude), grenade.gameObject, grenade.GetDamageType());
		}
		if (grenade.m_GrenadeType == GrenadeType.Frag || grenade.m_GrenadeType == GrenadeType.Concussion)
		{
			for (int i = 0; i < 8; i++)
			{
				for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[i].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					sqrMagnitude = (linkedListNode.Value.transform.position - grenade.transform.position).sqrMagnitude;
					if (sqrMagnitude <= grenade.m_MinRadiusSqr)
					{
						linkedListNode.Value.TakeDamage(grenade.CalculateDamage(sqrMagnitude), grenade.gameObject, grenade.GetDamageType());
					}
					else if (sqrMagnitude <= grenade.m_MaxRadiusSqr)
					{
						ray.direction = linkedListNode.Value.GetChestLocation() - vector;
						if (Physics.Raycast(ray, out hitInfo, grenade.m_MaxRadius * 1.1f, 769) && hitInfo.collider.gameObject.layer == 9)
						{
							linkedListNode.Value.TakeDamage(grenade.CalculateDamage(sqrMagnitude), grenade.gameObject, grenade.GetDamageType());
						}
					}
				}
			}
		}
		if (grenade.m_GrenadeType == GrenadeType.EMP)
		{
			for (LinkedListNode<Turret> linkedListNode2 = m_Turrets.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
			{
				sqrMagnitude = (linkedListNode2.Value.transform.position - grenade.transform.position).sqrMagnitude;
				if (sqrMagnitude <= grenade.m_MaxRadiusSqr)
				{
					linkedListNode2.Value.TakeDamage(grenade.CalculateDamage(sqrMagnitude), grenade.gameObject, grenade.GetDamageType());
				}
			}
			for (LinkedListNode<Sentry> linkedListNode3 = m_Sentries.First; linkedListNode3 != null; linkedListNode3 = linkedListNode3.Next)
			{
				sqrMagnitude = (linkedListNode3.Value.transform.position - grenade.transform.position).sqrMagnitude;
				if (sqrMagnitude <= grenade.m_MaxRadiusSqr)
				{
					linkedListNode3.Value.TakeDamage(grenade.CalculateDamage(sqrMagnitude), grenade.gameObject, grenade.GetDamageType());
				}
			}
		}
		m_ActiveGrenades.Remove(grenade);
		CheckAudioSenses(grenade.transform.position, 20f, DisturbanceEvent.MajorAudio);
		ScareNearbyNPCs(grenade.transform.position, 20f);
	}

	public void EnemySpawned(Enemy_Base enemy)
	{
		m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.AddLast(enemy);
	}

	public LinkedListNode<Enemy_Base> GetFirstEnemy(int SquadID)
	{
		return m_Squads[SquadID].m_EnemyUnits.First;
	}

	public void TurretSpawned(Turret turret)
	{
		m_Turrets.AddLast(turret);
	}

	public LinkedListNode<Turret> GetFirstTurret()
	{
		return m_Turrets.First;
	}

	public void TurretDestroyed(Turret turret)
	{
		m_Turrets.Remove(turret);
	}

	public void SentrySpawned(Sentry sentry)
	{
		m_Sentries.AddLast(sentry);
	}

	public LinkedListNode<Sentry> GetFirstSentry()
	{
		return m_Sentries.First;
	}

	public void SentryDestroyed(Sentry sentry)
	{
		m_Sentries.Remove(sentry);
	}

	public void SecurityCameraSpawned(SecurityCamera camera)
	{
		m_SecurityCameras.AddLast(camera);
	}

	public LinkedListNode<SecurityCamera> GetFirstSecurityCamera()
	{
		return m_SecurityCameras.First;
	}

	public void SecurityCameratDestroyed(SecurityCamera camera)
	{
		m_SecurityCameras.Remove(camera);
	}

	public void TriggerAlarm(EnemySquad SquadToAlert)
	{
		m_LastKnownPlayerPosition = Globals.m_PlayerController.transform.position;
		bool flag = false;
		for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[(int)SquadToAlert].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (!flag)
			{
				linkedListNode.Value.InvestigateAlarmingSound(Globals.m_PlayerController.transform.position);
				flag = true;
			}
			else
			{
				linkedListNode.Value.SupportAlarmingInvestigation(Globals.m_PlayerController.transform.position);
			}
		}
	}

	public void UpdatePlayerKnownPosition()
	{
		m_LastKnownPlayerPosition = Globals.m_PlayerController.transform.position;
	}

	public void NPCSpawned(NPC_Base npc)
	{
		m_NPCs.AddLast(npc);
	}

	public void NPCKilled(NPC_Base npc)
	{
		m_NPCs.Remove(npc);
	}

	public void ScareNearbyNPCs(Vector3 sourceLocation, float sourceRadius)
	{
		sourceRadius *= sourceRadius;
		for (LinkedListNode<NPC_Base> linkedListNode = m_NPCs.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if ((linkedListNode.Value.transform.position - sourceLocation).sqrMagnitude <= sourceRadius)
			{
				linkedListNode.Value.Cower();
			}
		}
	}

	public void ClearAll()
	{
		for (int i = 0; i < 8; i++)
		{
			m_Squads[i].m_Suspicious = false;
			m_Squads[i].m_Alarmed = false;
			m_Squads[i].m_InCombat = false;
			m_Squads[i].m_Hostile = false;
			m_Squads[i].m_EnemyUnits.Clear();
		}
		m_AnySuspiciousEnemies = false;
		m_AnyHostileEnemies = false;
		m_AnyAlarmedEnemies = false;
		m_AnyInCombatEnemies = false;
		m_CoverNodes.Clear();
		m_SearchNodes.Clear();
		for (LinkedListNode<GrenadeFrag> linkedListNode = m_ActiveGrenades.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (linkedListNode.Value != null)
			{
				UnityEngine.Object.Destroy(linkedListNode.Value.gameObject);
			}
		}
		m_ActiveGrenades.Clear();
		m_Turrets.Clear();
		m_Sentries.Clear();
		m_NPCs.Clear();
	}

	public void SetEnemiesPaused(bool paused)
	{
		for (int i = 0; i < 8; i++)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[i].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value.m_Paused = paused;
				if ((bool)linkedListNode.Value.animation)
				{
					foreach (AnimationState item in linkedListNode.Value.animation)
					{
						if (paused)
						{
							item.speed = 0f;
						}
						else
						{
							item.speed = linkedListNode.Value.m_CurrentAnimationSpeed;
						}
					}
				}
			}
		}
	}

	public bool HasSquadmates(Enemy_Base enemy)
	{
		if (enemy.m_EnemySquad == EnemySquad.Solo)
		{
			return false;
		}
		int num = 0;
		for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (linkedListNode.Value != enemy)
			{
				num++;
			}
		}
		return num > 0;
	}

	public bool AmILeader(Enemy_Base enemy)
	{
		return enemy.m_EnemySquad == EnemySquad.Solo || m_Squads[(int)enemy.m_EnemySquad].m_Leader == enemy;
	}

	public bool SquadmateSeesPlayer(Enemy_Base enemy)
	{
		for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (linkedListNode.Value != enemy && linkedListNode.Value.m_SeePlayerThisFrame)
			{
				return true;
			}
		}
		return false;
	}

	public void RemoveSquad(EnemySquad SquadID)
	{
		while (m_Squads[(int)SquadID].m_EnemyUnits.First != null)
		{
			RemoveEnemy(m_Squads[(int)SquadID].m_EnemyUnits.First.Value, false);
		}
		UpdateEnemyAwareness();
	}

	public void RemoveEnemy(Enemy_Base enemy, bool UpdateAwareness = true)
	{
		enemy.Removed();
		m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.Remove(enemy);
		if (UpdateAwareness)
		{
			UpdateEnemyAwareness();
		}
	}

	public void EnemyThrowingGrenade(Enemy_Base enemy)
	{
		m_GrenadeCooldownTimer = UnityEngine.Random.Range(15f, 25f);
	}

	public bool EnemyIsSuspicious(Enemy_Base enemy)
	{
		if (!m_AnyInCombatEnemies && !m_AnyAlarmedEnemies && !m_AnySuspiciousEnemies && !m_AnyHostileEnemies)
		{
			EventManager.Instance.PostEvent("Music_Level", EventAction.SetSwitch, "Stress");
			if (m_AlertSoundTimer <= 0f)
			{
				EventManager.Instance.PostEvent("EnemyState_Alarmed", EventAction.PlaySound, null, base.gameObject);
				m_AlertSoundTimer = m_MinimumAlertSoundInterval;
			}
		}
		m_Squads[(int)enemy.m_EnemySquad].m_Suspicious = true;
		m_AnySuspiciousEnemies = true;
		if (enemy.m_EnemySquad != 0)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value != enemy)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool EnemyIsNoLongerSuspicious(Enemy_Base enemy)
	{
		bool result = false;
		if (enemy.m_EnemySquad != 0)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value != enemy)
				{
					result = true;
				}
			}
		}
		UpdateEnemyAwareness();
		return result;
	}

	public bool EnemyIsAlarmed(Enemy_Base enemy, Vector3 sourceLocation)
	{
		if (!m_AnyInCombatEnemies && !m_AnyAlarmedEnemies && !m_AnySuspiciousEnemies && !m_AnyHostileEnemies)
		{
			EventManager.Instance.PostEvent("Music_Level", EventAction.SetSwitch, "Stress");
			EventManager.Instance.PostEvent("Stop_Conversation", EventAction.StopSound, null, base.gameObject);
			if (m_AlertSoundTimer <= 0f)
			{
				EventManager.Instance.PostEvent("EnemyState_Alarmed", EventAction.PlaySound, null, base.gameObject);
				m_AlertSoundTimer = m_MinimumAlertSoundInterval;
			}
		}
		if (!m_Squads[(int)enemy.m_EnemySquad].m_Alarmed && !m_Squads[(int)enemy.m_EnemySquad].m_InCombat)
		{
			m_Squads[(int)enemy.m_EnemySquad].m_Leader = enemy;
		}
		m_Squads[(int)enemy.m_EnemySquad].m_Alarmed = true;
		m_AnyAlarmedEnemies = true;
		bool result = false;
		if (enemy.m_EnemySquad != 0)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value != enemy)
				{
					linkedListNode.Value.SupportAlarmingInvestigation(sourceLocation);
					result = true;
				}
			}
		}
		return result;
	}

	public bool EnemyIsNoLongerAlarmed(Enemy_Base enemy)
	{
		bool result = false;
		if (enemy.m_EnemySquad != 0)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value != enemy)
				{
					linkedListNode.Value.CancelSupport();
					result = true;
				}
			}
		}
		UpdateEnemyAwareness();
		return result;
	}

	public bool EnemyEnteredCombat(Enemy_Base enemy)
	{
		if (!m_AnyInCombatEnemies)
		{
			EventManager.Instance.PostEvent("Music_Level", EventAction.SetSwitch, "Combat");
			EventManager.Instance.PostEvent("Stop_Conversation", EventAction.StopSound, null, base.gameObject);
			if (m_AlertSoundTimer <= 0f)
			{
				EventManager.Instance.PostEvent("EnemyState_Alarmed", EventAction.PlaySound, null, base.gameObject);
				m_AlertSoundTimer = m_MinimumAlertSoundInterval;
			}
			m_GrenadeCooldownTimer = UnityEngine.Random.Range(10f, 15f);
		}
		if (!m_Squads[(int)enemy.m_EnemySquad].m_Alarmed && !m_Squads[(int)enemy.m_EnemySquad].m_InCombat)
		{
			m_Squads[(int)enemy.m_EnemySquad].m_Leader = enemy;
		}
		m_Squads[(int)enemy.m_EnemySquad].m_InCombat = true;
		m_AnyInCombatEnemies = true;
		bool result = false;
		if (enemy.m_EnemySquad != 0)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value != enemy)
				{
					linkedListNode.Value.EnterCombat(false, DamageType.None);
					result = true;
				}
			}
		}
		return result;
	}

	public bool EnemyLostTrackOfPlayer(Enemy_Base enemy)
	{
		bool flag = true;
		if (enemy.m_EnemySquad != 0)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value != enemy && !linkedListNode.Value.m_LostPlayer)
				{
					flag = false;
				}
			}
		}
		if (flag)
		{
			SendSquadToSearch(enemy, m_LastKnownPlayerPosition, AudioEvent.LostVisuals);
		}
		return flag;
	}

	private void SendSquadToSearch(Enemy_Base enemy, Vector3 Location, AudioEvent VOEvent)
	{
		GatherSearchNodes(Location);
		LinkedListNode<NearbySearch> linkedListNode = m_NearbySearch.First;
		while (linkedListNode != null && !((Location - linkedListNode.Value.m_SearchNode.transform.position).sqrMagnitude <= 4f))
		{
			linkedListNode = linkedListNode.Next;
		}
		if (linkedListNode == null)
		{
			enemy.SearchAPosition(Location);
		}
		else
		{
			enemy.SearchANode(linkedListNode.Value.m_SearchNode);
			m_NearbySearch.Remove(linkedListNode);
		}
		bool atLeastOneOtherSquadmate = false;
		if (enemy.m_EnemySquad != 0)
		{
			m_Squads[(int)enemy.m_EnemySquad].m_Leader = enemy;
			for (LinkedListNode<Enemy_Base> linkedListNode2 = m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
			{
				if (linkedListNode2.Value != enemy)
				{
					atLeastOneOtherSquadmate = true;
					linkedListNode2.Value.SearchANode(null);
				}
			}
		}
		enemy.PlayVO(VOEvent, atLeastOneOtherSquadmate);
		UpdateEnemyAwareness();
	}

	public void EnemyFinishedSearching(Enemy_Base enemy, bool ForceFinish = false)
	{
		bool flag = true;
		LinkedListNode<Enemy_Base> linkedListNode = null;
		if (enemy.m_EnemySquad != 0)
		{
			for (linkedListNode = m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value != enemy && !linkedListNode.Value.m_DoneSearching)
				{
					flag = false;
					break;
				}
			}
		}
		if (flag || ForceFinish)
		{
			if (enemy.m_EnemySquad == EnemySquad.Solo)
			{
				enemy.CancelSearch(true);
			}
			else
			{
				for (linkedListNode = m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					linkedListNode.Value.CancelSearch(m_Squads[(int)enemy.m_EnemySquad].m_Leader == linkedListNode.Value);
				}
			}
		}
		UpdateEnemyAwareness();
	}

	public void EnemyKilled(Enemy_Base enemy)
	{
		if (enemy.m_EnemySquad != 0 && AmILeader(enemy))
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value != enemy)
				{
					m_Squads[(int)enemy.m_EnemySquad].m_Leader = linkedListNode.Value;
					linkedListNode.Value.PromotedToLeader(enemy);
					break;
				}
			}
		}
		m_Squads[(int)enemy.m_EnemySquad].m_EnemyUnits.Remove(enemy);
		UpdateEnemyAwareness();
	}

	private void UpdateEnemyAwareness()
	{
		bool anySuspiciousEnemies = m_AnySuspiciousEnemies;
		bool anyAlarmedEnemies = m_AnyAlarmedEnemies;
		bool anyInCombatEnemies = m_AnyInCombatEnemies;
		bool anyHostileEnemies = m_AnyHostileEnemies;
		m_AnySuspiciousEnemies = false;
		m_AnyAlarmedEnemies = false;
		m_AnyInCombatEnemies = false;
		m_AnyHostileEnemies = false;
		for (int i = 0; i < 8; i++)
		{
			m_Squads[i].m_Suspicious = false;
			m_Squads[i].m_Alarmed = false;
			m_Squads[i].m_InCombat = false;
			m_Squads[i].m_Hostile = false;
			for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[i].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value.InCombat())
				{
					m_Squads[i].m_InCombat = true;
					m_AnyInCombatEnemies = true;
				}
				if (linkedListNode.Value.IsHostile())
				{
					m_Squads[i].m_Hostile = true;
					m_AnyHostileEnemies = true;
				}
				if (linkedListNode.Value.IsAlarmed())
				{
					m_Squads[i].m_Alarmed = true;
					m_AnyAlarmedEnemies = true;
				}
				if (linkedListNode.Value.IsSuspicious())
				{
					m_Squads[i].m_Suspicious = true;
					m_AnySuspiciousEnemies = true;
				}
			}
		}
		if (m_AnyInCombatEnemies)
		{
			if (!anyInCombatEnemies)
			{
				EventManager.Instance.PostEvent("Music_Level", EventAction.SetSwitch, "Combat");
				if (m_AlertSoundTimer <= 0f)
				{
					EventManager.Instance.PostEvent("EnemyState_Alarmed", EventAction.PlaySound, null, base.gameObject);
					m_AlertSoundTimer = m_MinimumAlertSoundInterval;
				}
			}
		}
		else if (m_AnySuspiciousEnemies || m_AnyAlarmedEnemies || m_AnyHostileEnemies)
		{
			if (!anyAlarmedEnemies && !anySuspiciousEnemies && !anyHostileEnemies)
			{
				EventManager.Instance.PostEvent("Music_Level", EventAction.SetSwitch, "Stress");
				if (!anyInCombatEnemies && m_AlertSoundTimer <= 0f)
				{
					EventManager.Instance.PostEvent("EnemyState_Alarmed", EventAction.PlaySound, null, base.gameObject);
					m_AlertSoundTimer = m_MinimumAlertSoundInterval;
				}
			}
		}
		else if (anyInCombatEnemies || anySuspiciousEnemies || anyAlarmedEnemies || anyHostileEnemies)
		{
			EventManager.Instance.PostEvent("Music_Level", EventAction.SetSwitch, "Ambient");
		}
	}

	public void CheckAudioSenses(Vector3 sourceLocation, float sourceRadius, DisturbanceEvent sourceEvent, bool WeaponFire = false)
	{
		sourceRadius *= sourceRadius;
		if (sourceEvent != 0 && sourceEvent != DisturbanceEvent.MajorAudio)
		{
			return;
		}
		float num = -1f;
		float num2 = -1f;
		LinkedListNode<Enemy_Base> linkedListNode = null;
		LinkedListNode<Enemy_Base> linkedListNode2 = null;
		LinkedList<Enemy_Base> linkedList = new LinkedList<Enemy_Base>();
		for (int i = 0; i < 8; i++)
		{
			if (m_Squads[i].m_EnemyUnits.First == null || (m_Squads[i].m_InCombat && i != 0))
			{
				continue;
			}
			linkedListNode = m_Squads[i].m_EnemyUnits.First;
			num = -1f;
			linkedListNode2 = null;
			linkedList.Clear();
			while (linkedListNode != null)
			{
				num2 = linkedListNode.Value.CheckAudioSenses(sourceLocation, sourceRadius, sourceEvent);
				if (num2 > 0f)
				{
					linkedList.AddLast(linkedListNode.Value);
					if (num2 < num || num < 0f)
					{
						num = num2;
						linkedListNode2 = linkedListNode;
					}
				}
				linkedListNode = linkedListNode.Next;
			}
			for (linkedListNode = linkedList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value.InCombat())
				{
					linkedListNode.Value.RefreshPlayersKnownLocation();
				}
				else if (linkedListNode.Value.IsHostile())
				{
					if (linkedListNode.Value == linkedListNode2.Value)
					{
						linkedListNode.Value.SearchAPosition(sourceLocation, false);
					}
				}
				else if (linkedListNode.Value == linkedListNode2.Value)
				{
					if (linkedListNode.Value.IsAlarmed())
					{
						linkedListNode.Value.UpdateAlarmingSoundLocation(sourceLocation);
					}
					else if (sourceEvent == DisturbanceEvent.MinorAudio)
					{
						linkedListNode.Value.InvestigateSuspiciousSound(sourceLocation);
					}
					else
					{
						linkedListNode.Value.InvestigateAlarmingSound(sourceLocation, true);
					}
				}
			}
		}
	}

	public bool CheckVisualSenses(Collider collider, DisturbanceEvent sourceEvent, AudioEvent VOEvent)
	{
		if (sourceEvent != DisturbanceEvent.MinorVisual && sourceEvent != DisturbanceEvent.MajorVisual)
		{
			return false;
		}
		float num = -1f;
		float num2 = -1f;
		LinkedListNode<Enemy_Base> linkedListNode = null;
		LinkedListNode<Enemy_Base> linkedListNode2 = null;
		bool result = false;
		for (int i = 0; i < 8; i++)
		{
			if (m_Squads[i].m_EnemyUnits.First == null || ((m_Squads[i].m_InCombat || m_Squads[i].m_Hostile) && i != 0))
			{
				continue;
			}
			linkedListNode = m_Squads[i].m_EnemyUnits.First;
			num = -1f;
			linkedListNode2 = null;
			while (linkedListNode != null && (i != 0 || (!linkedListNode.Value.IsHostile() && !linkedListNode.Value.InCombat())))
			{
				num2 = linkedListNode.Value.CheckSpecificVisualSenses(collider);
				if (num2 > 0f && (num2 < num || num < 0f))
				{
					num = num2;
					linkedListNode2 = linkedListNode;
				}
				linkedListNode = linkedListNode.Next;
			}
			if (linkedListNode2 != null)
			{
				if (sourceEvent == DisturbanceEvent.MinorVisual)
				{
					linkedListNode2.Value.InvestigateSuspiciousSound(collider.transform.position, true);
				}
				else if (i != 0 || (!linkedListNode2.Value.InCombat() && !linkedListNode2.Value.IsHostile()))
				{
					EventManager.Instance.PostEvent("Stop_Conversation", EventAction.StopSound, null, base.gameObject);
					SendSquadToSearch(linkedListNode2.Value, collider.transform.position, VOEvent);
				}
				result = true;
			}
		}
		return result;
	}
}
