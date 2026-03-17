using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
	public GameObject m_FragGrenadePrefab;

	public GameObject m_EMPGrenadePrefab;

	public GameObject m_ConcussionGrenadePrefab;

	public GameObject m_FragMinePrefab;

	public GameObject m_EMPMinePrefab;

	public GameObject m_ConcussionMinePrefab;

	public GameObject m_MiniRPGProjectilePrefab;

	public GameObject m_CrossbowProjectilePrefab;

	public GameObject m_PlasmaRifleProjectilePrefab;

	public GameObject m_BoxRobotRocketProjectilePrefab;

	[HideInInspector]
	public Squad[] m_Squads = new Squad[8];

	[HideInInspector]
	public bool m_AnySuspiciousEnemies;

	[HideInInspector]
	public bool m_AnyHostileEnemies;

	[HideInInspector]
	public bool m_AnyAlarmedEnemies;

	[HideInInspector]
	public bool m_AnyInCombatEnemies;

	private bool m_CoverCalculatedThisFrame;

	[HideInInspector]
	public LinkedList<NearbyCover> m_CoverNodes = new LinkedList<NearbyCover>();

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

	[HideInInspector]
	public LinkedList<GrenadeFrag> m_ActiveGrenades = new LinkedList<GrenadeFrag>();

	[HideInInspector]
	public LinkedList<Turret> m_Turrets = new LinkedList<Turret>();

	[HideInInspector]
	public LinkedList<Sentry> m_Sentries = new LinkedList<Sentry>();

	[HideInInspector]
	public LinkedList<NPC_Base> m_NPCs = new LinkedList<NPC_Base>();

	[HideInInspector]
	public LinkedList<SecurityCamera> m_SecurityCameras = new LinkedList<SecurityCamera>();

	[HideInInspector]
	public LinkedList<BoxRobot> m_BoxRobots = new LinkedList<BoxRobot>();

	private void Awake()
	{
		Globals.m_AIDirector = this;
		GameManager.OnSaveGame += SaveGame;
		GameManager.OnLoadGame += LoadGame;
		m_MaxSearchDistanceSqr = m_MaxSearchDistance * m_MaxSearchDistance;
		m_MaxCoverDistanceSqr = m_MaxCoverDistance * m_MaxCoverDistance;
		m_MinCoverFacingDot = Mathf.Cos((float)Math.PI / 180f * m_ValidCoverAngle);
		for (int i = 0; i < 8; i++)
		{
			m_Squads[i] = new Squad();
		}
	}

	private void OnDestroy()
	{
		GameManager.OnSaveGame -= SaveGame;
		GameManager.OnLoadGame -= LoadGame;
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
		DamageData data = new DamageData(grenade.m_Owner, grenade.transform.position + Vector3.up * 0.1f, (int)grenade.m_Damage, grenade.GetDamageType(), false);
		Globals.DealSplashDamage(data, grenade.m_MinRadius, grenade.m_MaxRadius);
		m_ActiveGrenades.Remove(grenade);
		CheckAudioSenses(grenade.transform.position, 20f, DisturbanceEvent.MajorAudio, false);
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

	public void SentryEnteredCombat(Sentry sentryDrone, Vector3 threatLocation)
	{
		for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[(int)sentryDrone.m_EnemySquad].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			linkedListNode.Value.EnterCombat(false, DamageType.None);
		}
		for (LinkedListNode<BoxRobot> linkedListNode2 = m_BoxRobots.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
		{
			if (linkedListNode2.Value.m_EnemySquad == sentryDrone.m_EnemySquad)
			{
				linkedListNode2.Value.EnterCombat(false, threatLocation);
			}
		}
		for (LinkedListNode<Sentry> linkedListNode3 = m_Sentries.First; linkedListNode3 != null; linkedListNode3 = linkedListNode3.Next)
		{
			if (linkedListNode3.Value != sentryDrone && linkedListNode3.Value.m_EnemySquad == sentryDrone.m_EnemySquad)
			{
				linkedListNode3.Value.EnterCombat(false, threatLocation);
			}
		}
	}

	public void BoxRobotSpawned(BoxRobot boxbot)
	{
		m_BoxRobots.AddLast(boxbot);
	}

	public LinkedListNode<BoxRobot> GetFirstBoxRobot()
	{
		return m_BoxRobots.First;
	}

	public void BoxRobotDestroyed(BoxRobot boxbot)
	{
		m_BoxRobots.Remove(boxbot);
	}

	public void BoxRobotEnteredCombat(BoxRobot boxbot, Vector3 threatLocation)
	{
		for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[(int)boxbot.m_EnemySquad].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			linkedListNode.Value.EnterCombat(false, DamageType.None);
		}
		for (LinkedListNode<BoxRobot> linkedListNode2 = m_BoxRobots.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
		{
			if (linkedListNode2.Value != boxbot && linkedListNode2.Value.m_EnemySquad == boxbot.m_EnemySquad)
			{
				linkedListNode2.Value.EnterCombat(false, threatLocation);
			}
		}
		for (LinkedListNode<Sentry> linkedListNode3 = m_Sentries.First; linkedListNode3 != null; linkedListNode3 = linkedListNode3.Next)
		{
			if (linkedListNode3.Value.m_EnemySquad == boxbot.m_EnemySquad)
			{
				linkedListNode3.Value.EnterCombat(false, threatLocation);
			}
		}
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
				linkedListNode.Value.InvestigateAlarmingSound(Globals.m_PlayerController.transform.position, false, false);
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
				linkedListNode.Value.Cower(false);
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
		m_BoxRobots.Clear();
		m_SecurityCameras.Clear();
		m_NPCs.Clear();
	}

	public void SetEnemiesPaused(bool paused)
	{
		for (int i = 0; i < 8; i++)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = m_Squads[i].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value.m_Paused = paused;
				foreach (AnimationState item in linkedListNode.Value.m_Animator)
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
		for (LinkedListNode<BoxRobot> linkedListNode2 = m_BoxRobots.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
		{
			linkedListNode2.Value.m_Paused = paused;
			foreach (AnimationState item2 in linkedListNode2.Value.m_Animator)
			{
				if (paused)
				{
					item2.speed = 0f;
				}
				else
				{
					item2.speed = linkedListNode2.Value.m_CurrentAnimationSpeed;
				}
			}
		}
		for (LinkedListNode<NPC_Base> linkedListNode3 = m_NPCs.First; linkedListNode3 != null; linkedListNode3 = linkedListNode3.Next)
		{
			linkedListNode3.Value.m_Paused = paused;
			foreach (AnimationState item3 in linkedListNode3.Value.m_NPCAnimator)
			{
				if (paused)
				{
					item3.speed = 0f;
				}
				else
				{
					item3.speed = linkedListNode3.Value.m_CurrentAnimationSpeed;
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

	public void RemoveNPC(NPC_Base npc)
	{
		npc.Removed();
		m_NPCs.Remove(npc);
	}

	public void EnemyThrowingGrenade(Enemy_Base enemy)
	{
		m_GrenadeCooldownTimer = UnityEngine.Random.Range(15f, 25f);
	}

	public bool EnemyIsSuspicious(Enemy_Base enemy)
	{
		if (!m_AnyInCombatEnemies && !m_AnyAlarmedEnemies && !m_AnySuspiciousEnemies && !m_AnyHostileEnemies)
		{
			SoundManager.SetSoundSwitch("Music_Gameplay", "Stress", base.gameObject);
			if (m_AlertSoundTimer <= 0f)
			{
				SoundManager.TriggerEvent("Play_EnemyState_Alarmed", base.gameObject);
				m_AlertSoundTimer = m_MinimumAlertSoundInterval;
			}
		}
		m_Squads[(int)enemy.m_EnemySquad].m_Suspicious = true;
		m_AnySuspiciousEnemies = true;
		if (enemy.m_EnemySquad != EnemySquad.Solo)
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
		if (enemy.m_EnemySquad != EnemySquad.Solo)
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
			SoundManager.SetSoundSwitch("Music_Gameplay", "Stress", base.gameObject);
			SoundManager.TriggerEvent("Stop_Conversation", base.gameObject);
			if (m_AlertSoundTimer <= 0f)
			{
				SoundManager.TriggerEvent("Play_EnemyState_Alarmed", base.gameObject);
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
		if (enemy.m_EnemySquad != EnemySquad.Solo)
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
		if (enemy.m_EnemySquad != EnemySquad.Solo)
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

	public bool EnemyEnteredCombat(Enemy_Base enemy, Vector3 threatLocation)
	{
		if (!m_AnyInCombatEnemies)
		{
			SoundManager.SetSoundSwitch("Music_Gameplay", "Combat", base.gameObject);
			SoundManager.TriggerEvent("Stop_Conversation", base.gameObject);
			if (m_AlertSoundTimer <= 0f)
			{
				SoundManager.TriggerEvent("Play_EnemyState_Alarmed", base.gameObject);
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
		if (enemy.m_EnemySquad != EnemySquad.Solo)
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
		for (LinkedListNode<BoxRobot> linkedListNode2 = m_BoxRobots.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
		{
			if (linkedListNode2.Value.m_EnemySquad == enemy.m_EnemySquad)
			{
				linkedListNode2.Value.EnterCombat(false, threatLocation);
			}
		}
		for (LinkedListNode<Sentry> linkedListNode3 = m_Sentries.First; linkedListNode3 != null; linkedListNode3 = linkedListNode3.Next)
		{
			if (linkedListNode3.Value.m_EnemySquad == enemy.m_EnemySquad)
			{
				linkedListNode3.Value.EnterCombat(false, threatLocation);
			}
		}
		return result;
	}

	public bool EnemyLostTrackOfPlayer(Enemy_Base enemy)
	{
		bool flag = true;
		if (enemy.m_EnemySquad != EnemySquad.Solo)
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
			enemy.SearchAPosition(Location, true);
		}
		else
		{
			enemy.SearchANode(linkedListNode.Value.m_SearchNode);
			m_NearbySearch.Remove(linkedListNode);
		}
		bool atLeastOneOtherSquadmate = false;
		if (enemy.m_EnemySquad != EnemySquad.Solo)
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
		if (enemy.m_EnemySquad != EnemySquad.Solo)
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
		if (enemy.m_EnemySquad != EnemySquad.Solo && AmILeader(enemy))
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
				SoundManager.SetSoundSwitch("Music_Gameplay", "Combat", base.gameObject);
				if (m_AlertSoundTimer <= 0f)
				{
					SoundManager.TriggerEvent("Play_EnemyState_Alarmed", base.gameObject);
					m_AlertSoundTimer = m_MinimumAlertSoundInterval;
				}
			}
		}
		else if (m_AnySuspiciousEnemies || m_AnyAlarmedEnemies || m_AnyHostileEnemies)
		{
			if (!anyAlarmedEnemies && !anySuspiciousEnemies && !anyHostileEnemies)
			{
				SoundManager.SetSoundSwitch("Music_Gameplay", "Stress", base.gameObject);
				if (!anyInCombatEnemies && m_AlertSoundTimer <= 0f)
				{
					SoundManager.TriggerEvent("Play_EnemyState_Alarmed", base.gameObject);
					m_AlertSoundTimer = m_MinimumAlertSoundInterval;
				}
			}
		}
		else if (anyInCombatEnemies || anySuspiciousEnemies || anyAlarmedEnemies || anyHostileEnemies)
		{
			SoundManager.SetSoundSwitch("Music_Gameplay", "Ambient", base.gameObject);
		}
	}

	public void CheckAudioSenses(Vector3 sourceLocation, float sourceRadius, DisturbanceEvent sourceEvent, bool WeaponFire = false)
	{
		sourceRadius *= sourceRadius;
		if (sourceEvent != DisturbanceEvent.MinorAudio && sourceEvent != DisturbanceEvent.MajorAudio)
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
						linkedListNode.Value.InvestigateSuspiciousSound(sourceLocation, false, false);
					}
					else
					{
						linkedListNode.Value.InvestigateAlarmingSound(sourceLocation, true, false);
					}
				}
			}
		}
	}

	public void ShowBodyToCameras(Collider collider)
	{
		for (LinkedListNode<SecurityCamera> linkedListNode = m_SecurityCameras.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			linkedListNode.Value.CheckBody(collider);
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
					linkedListNode2.Value.InvestigateSuspiciousSound(collider.transform.position, true, false);
				}
				else if (i != 0 || (!linkedListNode2.Value.InCombat() && !linkedListNode2.Value.IsHostile()))
				{
					SoundManager.TriggerEvent("Stop_Conversation", base.gameObject);
					SendSquadToSearch(linkedListNode2.Value, collider.transform.position, VOEvent);
				}
				result = true;
			}
		}
		return result;
	}

	public XmlElement SaveGame(XmlElement root, XmlDocument doc)
	{
		XmlElement xmlElement = (XmlElement)root.AppendChild(doc.CreateElement("AIDirector"));
		xmlElement.SetAttribute("LastKnownPlayerPosition", m_LastKnownPlayerPosition.x + "," + m_LastKnownPlayerPosition.y + "," + m_LastKnownPlayerPosition.z);
		return null;
	}

	public XmlElement LoadGame(XmlElement root)
	{
		XmlNodeList elementsByTagName = root.GetElementsByTagName("AIDirector");
		foreach (XmlNode item in elementsByTagName)
		{
			if (!(item.Name == "AIDirector"))
			{
				continue;
			}
			foreach (XmlAttribute attribute in item.Attributes)
			{
				switch (attribute.Name)
				{
				case "LastKnownPlayerPosition":
				{
					string[] array = attribute.InnerText.Split(',');
					m_LastKnownPlayerPosition = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
					break;
				}
				}
			}
			return (XmlElement)item;
		}
		return null;
	}
}
