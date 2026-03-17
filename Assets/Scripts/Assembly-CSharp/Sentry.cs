using System;
using System.Collections.Generic;
using UnityEngine;

public class Sentry : CharacterBase
{
	public enum SentryState
	{
		None = -1,
		Patrolling = 0,
		Alarmed = 1,
		Hostile = 2,
		Dying = 3,
		Deactivated = 4,
		Total = 5
	}

	public enum PatrolState
	{
		None = -1,
		StandingIdle = 0,
		WalkingPatrol = 1,
		TurnToWalkToRePatrol = 2,
		WalkBackToRePatrol = 3,
		TurnToPatrol = 4,
		Total = 5
	}

	public enum AlarmedState
	{
		None = -1,
		TurnToInvestigate = 0,
		WalkToInvestigate = 1,
		Investigate = 2,
		Total = 3
	}

	public enum HostileState
	{
		None = -1,
		StandingIdle = 0,
		MoveToLocation = 1,
		TurnToInvestigate = 2,
		WalkToInvestigate = 3,
		Investigate = 4,
		Total = 5
	}

	public enum TurretTargetting
	{
		Neutral = 0,
		Target = 1,
		Spin = 2
	}

	private SentryState m_SentryState = SentryState.None;

	private PatrolState m_PatrolState = PatrolState.None;

	private AlarmedState m_AlarmedState = AlarmedState.None;

	private HostileState m_HostileState = HostileState.None;

	[HideInInspector]
	public EnemySquad m_EnemySquad = EnemySquad.None;

	[HideInInspector]
	public MachineAwareness m_Awareness;

	[HideInInspector]
	public bool m_Neutral;

	public Collider m_Collider;

	public NavMeshAgent m_NavAgent;

	private NavMeshPath m_NavMeshPath = new NavMeshPath();

	protected float m_PatrolSpeed = 1.5f;

	protected float m_PatrolTurnSpeed = 5f;

	protected float m_IdleTurnSpeed = 50f;

	protected float m_WalkSpeed = 1.5f;

	protected float m_WalkTurnSpeed = 150f;

	protected float m_HostileSpeed = 1.5f;

	protected float m_HostileTurnSpeed = 300f;

	protected float m_StateTimer;

	protected float m_VisualSensingTimer;

	protected float m_LostPlayerTimer;

	protected float m_InvestigateDelay;

	[HideInInspector]
	public Sentry_PatrolNode m_TargetNode;

	public bool m_PatrolForward = true;

	private Sentry_PatrolNode m_EventNode;

	protected Vector3 m_LastPatrolledPosition = Vector3.zero;

	public GameObject m_HorizontalRotator;

	public GameObject m_VerticalRotator;

	public float m_ViewingAngleInDegrees = 90f;

	private float m_ViewingAngleDot;

	private float m_DistanceThreshold = 18f;

	private float m_DistanceThresholdSqr;

	private TurretTargetting m_TargettingType;

	private CharacterBase m_Target;

	private Vector3 m_TargetLocation = Vector3.zero;

	public float m_DegreesPerSecond = 90f;

	private bool m_Firing;

	public float m_FireRate = 0.2f;

	private float m_FiringTimer;

	public int m_BulletDamage = 10;

	public GameObject m_RaycastObject;

	public ParticleSystem m_LeftMuzzleFlash;

	public ParticleSystem m_RightMuzzleFlash;

	[HideInInspector]
	public bool m_Paused;

	public MachineAwareness GetMachineAwareness()
	{
		return m_Awareness;
	}

	public bool IsAttacking()
	{
		return m_SentryState == SentryState.Hostile;
	}

	public bool IsAlarmed()
	{
		return m_SentryState == SentryState.Alarmed;
	}

	public bool IsPassive()
	{
		return m_SentryState == SentryState.Patrolling;
	}

	public bool IsDeactivated()
	{
		return m_SentryState == SentryState.Deactivated;
	}

	private void Awake()
	{
		m_DistanceThresholdSqr = m_DistanceThreshold * m_DistanceThreshold;
		m_ViewingAngleDot = Mathf.Cos((float)Math.PI / 180f * (m_ViewingAngleInDegrees * 0.5f));
		m_LeftMuzzleFlash.Stop();
		m_RightMuzzleFlash.Stop();
	}

	protected override void Start()
	{
		base.Start();
		m_DamageModifiers[0] = 0.1f;
		m_DamageModifiers[1] = 0f;
		m_DamageModifiers[2] = 0.6f;
		m_DamageModifiers[3] = 5f;
		m_DamageModifiers[4] = 0f;
		m_DamageModifiers[6] = 1.25f;
		m_DamageModifiers[5] = 5f;
		m_CharacterType = CharacterType.Machine;
		Globals.m_AIDirector.SentrySpawned(this);
		m_SentryState = SentryState.Patrolling;
		m_PatrolState = PatrolState.StandingIdle;
		m_TargettingType = TurretTargetting.Neutral;
		if (m_TargetNode != null)
		{
			Sentry_PatrolNode sentry_PatrolNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
			base.transform.position = m_TargetNode.transform.position;
			if (sentry_PatrolNode != null)
			{
				m_PatrolState = PatrolState.WalkingPatrol;
				Vector3 vector = sentry_PatrolNode.transform.position - m_TargetNode.transform.position;
				vector.y = 0f;
				base.transform.rotation = Quaternion.LookRotation(vector.normalized);
			}
		}
		RayClampToGround();
		m_LastPatrolledPosition = base.transform.position;
		m_NavAgent.Stop(true);
		m_NavAgent.updateRotation = false;
		m_NavAgent.updatePosition = true;
		if (m_Awareness == MachineAwareness.Deactivated)
		{
			m_SentryState = SentryState.Deactivated;
			Vector3 forward = m_VerticalRotator.transform.localRotation * Vector3.forward;
			forward.y = 0f;
			forward.Normalize();
			forward.y = -1f;
			m_VerticalRotator.transform.localRotation = Quaternion.LookRotation(forward);
		}
	}

	private void UpdateSentryAlarmed()
	{
		EndFiring();
		switch (m_AlarmedState)
		{
		case AlarmedState.TurnToInvestigate:
			UpdateSentryTurnToInvestigate();
			break;
		case AlarmedState.WalkToInvestigate:
			UpdateSentryWalkToInvestigate();
			break;
		case AlarmedState.Investigate:
			UpdateSentryInvestigate();
			break;
		}
	}

	private void UpdateSentryTurnToInvestigate()
	{
		if (m_NavMeshPath.corners == null || m_NavMeshPath.corners.Length <= 0)
		{
			m_AlarmedState = AlarmedState.Investigate;
			return;
		}
		Vector3 vector = m_NavMeshPath.corners[1] - base.transform.position;
		vector.y = 0f;
		Quaternion quaternion = Quaternion.LookRotation(vector.normalized);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, m_IdleTurnSpeed * Time.deltaTime);
		if (Quaternion.Dot(base.transform.rotation, quaternion) >= 0.9f)
		{
			m_AlarmedState = AlarmedState.WalkToInvestigate;
			m_NavAgent.speed = m_PatrolSpeed;
			m_NavAgent.angularSpeed = m_PatrolTurnSpeed;
			m_NavAgent.updateRotation = true;
			m_NavAgent.updatePosition = true;
			m_NavAgent.SetPath(m_NavMeshPath);
			m_NavAgent.Resume();
		}
	}

	private void UpdateSentryWalkToInvestigate()
	{
		if (m_NavAgent.remainingDistance != float.PositiveInfinity && m_NavAgent.remainingDistance <= 0.1f)
		{
			m_NavAgent.Stop(true);
			m_NavAgent.updatePosition = true;
			m_AlarmedState = AlarmedState.Investigate;
			m_TargettingType = TurretTargetting.Spin;
			m_StateTimer = UnityEngine.Random.Range(8f, 12f);
		}
	}

	private void UpdateSentryInvestigate()
	{
		m_StateTimer -= Time.deltaTime;
		if (m_StateTimer <= 0f)
		{
			m_SentryState = SentryState.Patrolling;
			m_PatrolState = PatrolState.TurnToWalkToRePatrol;
			m_TargettingType = TurretTargetting.Neutral;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(m_LastPatrolledPosition, out hit, 1f, m_NavAgent.walkableMask))
			{
				m_LastPatrolledPosition = hit.position;
			}
			m_NavAgent.updateRotation = false;
			m_NavAgent.updatePosition = false;
			m_NavAgent.ResetPath();
			m_NavAgent.enabled = false;
			m_NavAgent.enabled = true;
			m_NavAgent.CalculatePath(m_LastPatrolledPosition, m_NavMeshPath);
		}
	}

	private void UpdateSentryHostile()
	{
		switch (m_HostileState)
		{
		case HostileState.StandingIdle:
			UpdateSentryHostileStandingIdle();
			break;
		case HostileState.MoveToLocation:
			UpdateSentryHostileMoveToLocation();
			break;
		case HostileState.TurnToInvestigate:
			UpdateSentryHostileTurnToInvestigate();
			break;
		case HostileState.WalkToInvestigate:
			UpdateSentryHostileWalkToInvestigate();
			break;
		case HostileState.Investigate:
			UpdateSentryHostileInvestigate();
			break;
		}
		UpdateFiring();
	}

	private void UpdateFiring()
	{
		float num = Vector3.Dot(m_RaycastObject.transform.forward, (m_TargetLocation - m_RaycastObject.transform.position).normalized);
		m_FiringTimer -= Time.deltaTime;
		if (m_Firing)
		{
			if (m_FiringTimer <= 0f)
			{
				FireBullet();
				m_FiringTimer = m_FireRate;
			}
			if (num < 0.96f || m_LostPlayerTimer >= 1f)
			{
				EndFiring();
			}
		}
		else if (num >= 0.96f && m_LostPlayerTimer <= 0.25f)
		{
			StartFiring();
		}
	}

	private void UpdateSentryHostileStandingIdle()
	{
		if (m_LostPlayerTimer >= 5f && m_StateTimer <= 0f)
		{
			m_HostileState = HostileState.TurnToInvestigate;
			m_NavAgent.Stop(true);
			Vector3 vector = m_TargetLocation;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(vector, out hit, 1f, m_NavAgent.walkableMask))
			{
				vector = hit.position;
			}
			m_NavAgent.updateRotation = false;
			m_NavAgent.updatePosition = false;
			m_NavAgent.ResetPath();
			m_NavAgent.enabled = false;
			m_NavAgent.enabled = true;
			m_NavAgent.CalculatePath(vector, m_NavMeshPath);
		}
	}

	private void UpdateSentryHostileMoveToLocation()
	{
		if (m_NavAgent.remainingDistance != float.PositiveInfinity && m_NavAgent.remainingDistance <= 0.1f)
		{
			m_NavAgent.Stop(true);
			m_NavAgent.updatePosition = true;
			if (m_LostPlayerTimer < 5f)
			{
				m_StateTimer = 2f;
				m_HostileState = HostileState.StandingIdle;
			}
			else
			{
				m_HostileState = HostileState.Investigate;
				m_TargettingType = TurretTargetting.Spin;
				m_StateTimer = UnityEngine.Random.Range(8f, 12f);
			}
		}
	}

	private void UpdateSentryHostileTurnToInvestigate()
	{
		if (m_NavMeshPath.corners == null || m_NavMeshPath.corners.Length <= 0)
		{
			m_HostileState = HostileState.StandingIdle;
			m_StateTimer = 2f;
			return;
		}
		Vector3 vector = m_NavMeshPath.corners[1] - base.transform.position;
		vector.y = 0f;
		Quaternion quaternion = Quaternion.LookRotation(vector.normalized);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, m_IdleTurnSpeed * Time.deltaTime);
		float num = Mathf.Abs(Quaternion.Dot(base.transform.rotation, quaternion));
		if (num >= 0.9f)
		{
			m_HostileState = HostileState.WalkToInvestigate;
			m_NavAgent.speed = m_HostileSpeed;
			m_NavAgent.angularSpeed = m_HostileTurnSpeed;
			m_NavAgent.updateRotation = true;
			m_NavAgent.updatePosition = true;
			m_NavAgent.SetPath(m_NavMeshPath);
			m_NavAgent.Resume();
		}
	}

	private void UpdateSentryHostileWalkToInvestigate()
	{
		if (m_NavAgent.remainingDistance != float.PositiveInfinity && m_NavAgent.remainingDistance <= 0.1f)
		{
			m_NavAgent.Stop(true);
			m_NavAgent.updatePosition = true;
			m_HostileState = HostileState.Investigate;
			m_TargettingType = TurretTargetting.Spin;
			m_StateTimer = UnityEngine.Random.Range(8f, 12f);
		}
	}

	private void UpdateSentryHostileInvestigate()
	{
		m_StateTimer -= Time.deltaTime;
		if (m_StateTimer <= 0f)
		{
			m_SentryState = SentryState.Patrolling;
			m_PatrolState = PatrolState.TurnToWalkToRePatrol;
			m_TargettingType = TurretTargetting.Neutral;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(m_LastPatrolledPosition, out hit, 1f, m_NavAgent.walkableMask))
			{
				m_LastPatrolledPosition = hit.position;
			}
			m_NavAgent.updateRotation = false;
			m_NavAgent.updatePosition = false;
			m_NavAgent.ResetPath();
			m_NavAgent.enabled = false;
			m_NavAgent.enabled = true;
			m_NavAgent.CalculatePath(m_LastPatrolledPosition, m_NavMeshPath);
		}
	}

	private void UpdateSentryPatrolling()
	{
		EndFiring();
		switch (m_PatrolState)
		{
		case PatrolState.StandingIdle:
			UpdateSentryStandingIdle();
			break;
		case PatrolState.WalkingPatrol:
			UpdateSentryWalkingPatrol();
			break;
		case PatrolState.TurnToWalkToRePatrol:
			UpdateSentryTurnToWalkToRePatrol();
			break;
		case PatrolState.WalkBackToRePatrol:
			UpdateSentryWalkBackToRePatrol();
			break;
		case PatrolState.TurnToPatrol:
			UpdateSentryTurnToPatrol();
			break;
		}
	}

	private void UpdateSentryStandingIdle()
	{
		if (!(m_StateTimer <= 0f))
		{
			return;
		}
		if (m_EventNode != null)
		{
			if (m_EventNode.m_PatrolEvent == Sentry_PatrolNode.PatrolEvent.TurnBack && m_TargetNode != null)
			{
				m_PatrolState = PatrolState.TurnToPatrol;
			}
			else if (m_TargetNode != null)
			{
				m_PatrolState = PatrolState.WalkingPatrol;
			}
			m_TargettingType = TurretTargetting.Neutral;
			m_EventNode = null;
		}
		else if (m_TargetNode != null)
		{
			m_PatrolState = PatrolState.WalkingPatrol;
			m_TargettingType = TurretTargetting.Neutral;
		}
	}

	private void UpdateSentryWalkingPatrol()
	{
		Vector3 vector = m_TargetNode.transform.position - base.transform.position;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		float num = m_PatrolSpeed * Time.deltaTime;
		if (magnitude <= num)
		{
			base.transform.position = m_TargetNode.transform.position;
			m_LastPatrolledPosition = base.transform.position;
			num -= magnitude;
			if (SentryPatrolEventTriggered())
			{
				m_StateTimer = UnityEngine.Random.Range(m_EventNode.m_MinIdle, m_EventNode.m_MaxIdle);
				if (m_EventNode.m_PatrolEvent == Sentry_PatrolNode.PatrolEvent.TurnBack)
				{
					m_PatrolForward = !m_PatrolForward;
				}
				m_TargetNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
				m_PatrolState = PatrolState.StandingIdle;
				m_TargettingType = TurretTargetting.Spin;
				return;
			}
			m_TargetNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
			if (!(m_TargetNode != null))
			{
				m_PatrolState = PatrolState.StandingIdle;
				return;
			}
			vector = m_TargetNode.transform.position - base.transform.position;
			vector.y = 0f;
		}
		float maxDegreesDelta = ((!(magnitude <= 1f)) ? m_PatrolTurnSpeed : (m_PatrolTurnSpeed * 2f));
		Quaternion to = Quaternion.LookRotation(vector.normalized);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, maxDegreesDelta);
		m_NavAgent.Move(base.transform.forward * num);
		m_LastPatrolledPosition = base.transform.position;
	}

	private void UpdateSentryTurnToWalkToRePatrol()
	{
		if (m_NavMeshPath.corners == null || m_NavMeshPath.corners.Length <= 0)
		{
			m_PatrolState = PatrolState.StandingIdle;
			m_TargettingType = TurretTargetting.Neutral;
			return;
		}
		Vector3 vector = ((m_NavMeshPath.corners.Length > 1) ? m_NavMeshPath.corners[1] : m_LastPatrolledPosition) - base.transform.position;
		vector.y = 0f;
		Quaternion quaternion = Quaternion.LookRotation(vector.normalized);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, m_IdleTurnSpeed * Time.deltaTime);
		float num = Mathf.Abs(Quaternion.Dot(base.transform.rotation, quaternion));
		if (num >= 0.98f)
		{
			m_PatrolState = PatrolState.WalkBackToRePatrol;
			m_NavAgent.speed = m_WalkSpeed;
			m_NavAgent.angularSpeed = m_WalkTurnSpeed;
			m_NavAgent.updateRotation = true;
			m_NavAgent.updatePosition = true;
			m_NavAgent.SetPath(m_NavMeshPath);
			m_NavAgent.Resume();
		}
	}

	private void UpdateSentryWalkBackToRePatrol()
	{
		if (m_NavAgent.remainingDistance != float.PositiveInfinity && m_NavAgent.remainingDistance <= 0.1f)
		{
			m_NavAgent.Stop(true);
			m_NavAgent.updatePosition = true;
			if (m_TargetNode != null)
			{
				m_PatrolState = PatrolState.TurnToPatrol;
			}
			else
			{
				m_PatrolState = PatrolState.StandingIdle;
			}
		}
	}

	private void UpdateSentryTurnToPatrol()
	{
		if (m_TargetNode == null)
		{
			m_SentryState = SentryState.Patrolling;
			m_PatrolState = PatrolState.StandingIdle;
			return;
		}
		Vector3 vector = m_TargetNode.transform.position - base.transform.position;
		vector.y = 0f;
		Quaternion quaternion = Quaternion.LookRotation(vector.normalized);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, m_IdleTurnSpeed * Time.deltaTime);
		if (Mathf.Abs(Quaternion.Dot(base.transform.rotation, quaternion)) >= 0.98f)
		{
			m_PatrolState = PatrolState.WalkingPatrol;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (Time.timeScale != 0f && !(Time.deltaTime <= 0f) && !m_Paused)
		{
			switch (m_SentryState)
			{
			case SentryState.Patrolling:
				UpdateSentryPatrolling();
				break;
			case SentryState.Alarmed:
				UpdateSentryAlarmed();
				break;
			case SentryState.Hostile:
				UpdateSentryHostile();
				break;
			case SentryState.Dying:
				UpdateSentryDying();
				break;
			case SentryState.Deactivated:
				UpdateSentryDeactivated();
				break;
			}
			RayClampToGround();
			m_StateTimer -= Time.deltaTime;
			m_InvestigateDelay -= Time.deltaTime;
			m_LostPlayerTimer += Time.deltaTime;
			CheckVisualSenses();
		}
	}

	private void LateUpdate()
	{
		if (Time.timeScale != 0f && !(Time.deltaTime <= 0f))
		{
			if (m_SentryState == SentryState.Dying || m_SentryState == SentryState.Deactivated)
			{
				Vector3 forward = m_VerticalRotator.transform.localRotation * Vector3.forward;
				forward.y = 0f;
				forward.Normalize();
				forward.y = -1f;
				Quaternion to = Quaternion.LookRotation(forward);
				m_VerticalRotator.transform.localRotation = Quaternion.RotateTowards(m_VerticalRotator.transform.localRotation, to, 30f * Time.deltaTime);
			}
			else if (m_TargettingType == TurretTargetting.Neutral)
			{
				m_HorizontalRotator.transform.localRotation = Quaternion.RotateTowards(m_HorizontalRotator.transform.localRotation, Quaternion.identity, m_DegreesPerSecond * Time.deltaTime);
				m_VerticalRotator.transform.localRotation = Quaternion.RotateTowards(m_VerticalRotator.transform.localRotation, Quaternion.identity, m_DegreesPerSecond * Time.deltaTime);
			}
			else if (m_TargettingType == TurretTargetting.Target)
			{
				Vector3 forward2 = m_TargetLocation - m_RaycastObject.transform.position;
				forward2.y = 0f;
				Quaternion to2 = ((!(forward2.magnitude <= 0.5f)) ? Quaternion.LookRotation(forward2) : Quaternion.identity);
				m_HorizontalRotator.transform.rotation = Quaternion.RotateTowards(m_HorizontalRotator.transform.rotation, to2, m_DegreesPerSecond * Time.deltaTime);
				forward2 = m_TargetLocation - m_RaycastObject.transform.position;
				forward2.y = 0f;
				forward2.z = forward2.magnitude;
				forward2.x = 0f;
				forward2.y = m_TargetLocation.y - m_RaycastObject.transform.position.y;
				to2 = ((!(forward2.magnitude <= 0.5f)) ? Quaternion.LookRotation(forward2) : Quaternion.identity);
				m_VerticalRotator.transform.localRotation = Quaternion.RotateTowards(m_VerticalRotator.transform.localRotation, to2, m_DegreesPerSecond * Time.deltaTime);
			}
			else
			{
				Vector3 eulerAngles = m_HorizontalRotator.transform.localRotation.eulerAngles;
				eulerAngles.y += m_DegreesPerSecond * Time.deltaTime;
				m_HorizontalRotator.transform.localRotation = Quaternion.Euler(eulerAngles);
				m_VerticalRotator.transform.localRotation = Quaternion.RotateTowards(m_VerticalRotator.transform.localRotation, Quaternion.identity, m_DegreesPerSecond * Time.deltaTime);
			}
		}
	}

	private void UpdateSentryDying()
	{
		m_StateTimer -= Time.deltaTime;
		if (m_StateTimer <= 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void UpdateSentryDeactivated()
	{
	}

	public void Dominate(MachineAwareness awareness)
	{
		if (m_SentryState != SentryState.Dying)
		{
			m_Awareness = awareness;
			if (awareness == MachineAwareness.Deactivated && m_SentryState != SentryState.Deactivated)
			{
				m_SentryState = SentryState.Deactivated;
				m_Target = null;
				EndFiring();
				m_NavAgent.Stop(true);
				m_NavAgent.updateRotation = false;
			}
			else if (awareness != MachineAwareness.Deactivated && m_SentryState == SentryState.Deactivated)
			{
				ResumePatrol();
			}
		}
	}

	private void ResumePatrol()
	{
		if ((m_LastPatrolledPosition - base.transform.position).magnitude <= 0.5f)
		{
			m_SentryState = SentryState.Patrolling;
			m_PatrolState = PatrolState.TurnToPatrol;
			m_TargettingType = TurretTargetting.Neutral;
			m_NavAgent.updatePosition = true;
			return;
		}
		m_SentryState = SentryState.Patrolling;
		m_PatrolState = PatrolState.TurnToWalkToRePatrol;
		m_TargettingType = TurretTargetting.Neutral;
		NavMeshHit hit;
		if (NavMesh.SamplePosition(m_LastPatrolledPosition, out hit, 1f, m_NavAgent.walkableMask))
		{
			m_LastPatrolledPosition = hit.position;
		}
		m_NavAgent.updateRotation = false;
		m_NavAgent.updatePosition = false;
		m_NavAgent.ResetPath();
		m_NavAgent.enabled = false;
		m_NavAgent.enabled = true;
		m_NavAgent.CalculatePath(m_LastPatrolledPosition, m_NavMeshPath);
	}

	private void RayClampToGround()
	{
		Ray ray = new Ray(base.transform.position + Vector3.up * 1.5f, -Vector3.up);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 3f, 257))
		{
			Vector3 position = base.transform.position;
			position.y = hitInfo.point.y;
			base.transform.position = position;
		}
	}

	private bool SentryPatrolEventTriggered()
	{
		if (m_TargetNode.m_PatrolEvent == Sentry_PatrolNode.PatrolEvent.None || m_TargetNode.m_EventChance == Sentry_PatrolNode.EventChance.Never)
		{
			return false;
		}
		float num = -1f;
		switch (m_TargetNode.m_EventChance)
		{
		case Sentry_PatrolNode.EventChance.Always:
			num = 1f;
			break;
		case Sentry_PatrolNode.EventChance.Often:
			num = 0.75f;
			break;
		case Sentry_PatrolNode.EventChance.Sometimes:
			num = 0.5f;
			break;
		case Sentry_PatrolNode.EventChance.Rarely:
			num = 0.25f;
			break;
		case Sentry_PatrolNode.EventChance.Once:
			num = 1f;
			m_TargetNode.m_EventChance = Sentry_PatrolNode.EventChance.Never;
			break;
		}
		if (UnityEngine.Random.value <= num)
		{
			m_EventNode = m_TargetNode;
			return true;
		}
		return false;
	}

	private void StartFiring()
	{
		if (!m_Firing)
		{
			m_Firing = true;
			SoundManager.TriggerEvent("Play_Security_Robot_Fire", base.gameObject);
			m_LeftMuzzleFlash.Play();
			m_RightMuzzleFlash.Play();
		}
	}

	private void EndFiring()
	{
		if (m_Firing)
		{
			m_Firing = false;
			SoundManager.TriggerEvent("Stop_Security_Robot_Fire", base.gameObject);
			m_LeftMuzzleFlash.Stop();
			m_RightMuzzleFlash.Stop();
			m_LeftMuzzleFlash.Clear();
			m_RightMuzzleFlash.Clear();
		}
	}

	private void FireBullet()
	{
		Ray ray = new Ray(m_RaycastObject.transform.position, m_RaycastObject.transform.forward);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, 17153) && (hitInfo.collider.gameObject.layer == 14 || hitInfo.collider.gameObject.layer == 9 || hitInfo.collider.gameObject.layer == 21))
		{
			CharacterBase characterBase = Globals.FindCharacterBase(hitInfo.transform);
			if ((bool)characterBase)
			{
				characterBase.TakeDamage(new DamageData(this, base.transform.position, m_BulletDamage, DamageType.Normal, false));
			}
		}
	}

	private bool CheckVisualSenses()
	{
		if (m_SentryState == SentryState.Dying || m_SentryState == SentryState.Deactivated)
		{
			return false;
		}
		if (m_Awareness == MachineAwareness.Hostile)
		{
			if (Globals.m_PlayerController == null || !Globals.m_PlayerController.gameObject.active)
			{
				m_VisualSensingTimer = 0f;
				return false;
			}
			if (Globals.m_AugmentCloaking.enabled)
			{
				return ProcessVisualTimer(true);
			}
			if (CanSeeEnemy(Globals.m_PlayerController) > 0f)
			{
				m_TargettingType = TurretTargetting.Target;
				m_Target = Globals.m_PlayerController;
				m_TargetLocation = m_Target.GetChestLocation();
				m_VisualSensingTimer += Time.deltaTime;
				m_LostPlayerTimer = 0f;
				return ProcessVisualTimer(false);
			}
			return ProcessVisualTimer(true);
		}
		if (m_Awareness == MachineAwareness.Friendly)
		{
			bool flag = false;
			if (m_Target != null && m_Target.IsDead())
			{
				flag = true;
				m_Target = null;
			}
			if (m_Target != null && CanSeeEnemy(m_Target) > 0f)
			{
				m_TargetLocation = m_Target.GetChestLocation();
				m_VisualSensingTimer += Time.deltaTime;
				m_LostPlayerTimer = 0f;
				return ProcessVisualTimer(false);
			}
			m_Target = null;
			float num = -1f;
			for (int i = 0; i < 8; i++)
			{
				for (LinkedListNode<Enemy_Base> linkedListNode = Globals.m_AIDirector.GetFirstEnemy(i); linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					if (!linkedListNode.Value.IsDead())
					{
						float num2 = CanSeeEnemy(linkedListNode.Value);
						if (num2 > 0f && (num2 < num || num < 0f))
						{
							m_Target = linkedListNode.Value;
							m_TargetLocation = m_Target.GetChestLocation();
							num = num2;
						}
					}
				}
			}
			if (m_Target != null)
			{
				m_TargettingType = TurretTargetting.Target;
				m_VisualSensingTimer += Time.deltaTime;
				m_LostPlayerTimer = 0f;
				return ProcessVisualTimer(false);
			}
			if (flag)
			{
				ResumePatrol();
				m_VisualSensingTimer = 0f;
				return false;
			}
		}
		return ProcessVisualTimer(true);
	}

	private float CanSeeEnemy(CharacterBase enemy)
	{
		Vector3 vector = enemy.GetChestLocation() - m_RaycastObject.transform.position;
		Vector3 normalized = vector.normalized;
		float num = Vector3.Dot(normalized, m_RaycastObject.transform.forward);
		if (enemy.IsInCover() && Vector3.Dot(normalized, enemy.GetCoverNormal()) >= 0.3f)
		{
			return -1f;
		}
		if (num >= m_ViewingAngleDot && vector.sqrMagnitude <= m_DistanceThresholdSqr)
		{
			Ray ray = new Ray(m_RaycastObject.transform.position, enemy.GetChestLocation() - m_RaycastObject.transform.position);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 30f, 82689) && (hitInfo.collider.gameObject.layer == enemy.GetGlobalLayer() || (hitInfo.collider == enemy.GetCoverCollider() && enemy.ExposedInCover())))
			{
				return vector.sqrMagnitude;
			}
		}
		return -1f;
	}

	public virtual bool ProcessVisualTimer(bool ResetTimer)
	{
		if (m_SentryState == SentryState.Hostile || m_Target == null)
		{
			return false;
		}
		bool result = false;
		if (m_VisualSensingTimer >= 0.75f && (m_Target != Globals.m_PlayerController || !m_Neutral || HostilityZone.IsPlayerInHostileTerritory()))
		{
			EnterCombat(true, m_Target.GetChestLocation());
			result = true;
		}
		else if (m_VisualSensingTimer >= 0.2f && ResetTimer && (m_Target != Globals.m_PlayerController || !m_Neutral || HostilityZone.IsPlayerInHostileTerritory()))
		{
			Investigate(m_Target.GetChestLocation());
			result = true;
		}
		if (ResetTimer)
		{
			m_VisualSensingTimer = 0f;
		}
		return result;
	}

	private void Investigate(Vector3 location)
	{
		if (!(m_InvestigateDelay > 0f))
		{
			m_NavAgent.Stop(true);
			m_SentryState = SentryState.Alarmed;
			m_AlarmedState = AlarmedState.TurnToInvestigate;
			m_TargetLocation = location;
			m_TargettingType = TurretTargetting.Target;
			m_InvestigateDelay = 3f;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(location, out hit, 1f, m_NavAgent.walkableMask))
			{
				location = hit.position;
			}
			m_NavAgent.updateRotation = false;
			m_NavAgent.updatePosition = false;
			m_NavAgent.ResetPath();
			m_NavAgent.enabled = false;
			m_NavAgent.enabled = true;
			m_NavAgent.CalculatePath(location, m_NavMeshPath);
		}
	}

	public void EnterCombat(bool FirstSight, Vector3 location)
	{
		if (m_SentryState == SentryState.Hostile)
		{
			return;
		}
		if (!FirstSight)
		{
			if (m_SentryState == SentryState.Deactivated)
			{
				Dominate(MachineAwareness.Hostile);
			}
			return;
		}
		m_NavAgent.Stop(true);
		m_SentryState = SentryState.Hostile;
		m_HostileState = HostileState.StandingIdle;
		m_StateTimer = 2f;
		if (FirstSight)
		{
			Globals.m_AIDirector.SentryEnteredCombat(this, location);
		}
		m_Neutral = false;
	}

	public void Destroy()
	{
		Globals.m_AIDirector.SentryDestroyed(this);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public override Ray WeaponRequestForBulletRay(out bool PlayTracer)
	{
		PlayTracer = true;
		return new Ray(Vector3.zero, Vector3.one);
	}

	public override void WeaponWantsReload()
	{
	}

	public override void WeaponDoneReloading()
	{
	}

	public override Vector3 GetChestLocation()
	{
		return base.transform.position + Vector3.up * 0.5f;
	}

	public override bool TakeDamage(DamageData data)
	{
		if (m_SentryState == SentryState.Dying)
		{
			return true;
		}
		bool flag = base.TakeDamage(data);
		if (!flag && m_SentryState != SentryState.Deactivated)
		{
			Globals.m_AIDirector.UpdatePlayerKnownPosition();
			EnterCombat(true, data.m_SourceLocation);
			m_TargettingType = TurretTargetting.Target;
			m_Target = Globals.m_PlayerController;
			m_TargetLocation = m_Target.GetChestLocation();
			m_LostPlayerTimer = 0f;
		}
		return flag;
	}

	public override void Die(DamageData data)
	{
		base.Die(data);
		Globals.m_AIDirector.SentryDestroyed(this);
		m_SentryState = SentryState.Dying;
		EndFiring();
		if ((bool)m_Collider)
		{
			m_Collider.enabled = false;
		}
		m_NavAgent.Stop(true);
		m_NavAgent.updateRotation = false;
		m_StateTimer = 10f;
		if (Globals.m_PlayerController.m_TargetedEnemy == base.gameObject)
		{
			Globals.m_PlayerController.m_TargetedEnemy = null;
		}
		SoundManager.TriggerEvent("Play_Security_Robot_Death", base.gameObject);
	}

	protected override void AttachShadowObject()
	{
		UnityEngine.Object.Destroy(m_ShadowObject);
	}
}
