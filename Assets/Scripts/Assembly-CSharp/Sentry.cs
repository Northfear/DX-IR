using System;
using Fabric;
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
		Total = 4
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

	public float m_DistanceThreshold = 15f;

	private float m_DistanceThresholdSqr;

	private TurretTargetting m_TargettingType;

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
		m_CharacterType = CharacterType.Machine;
		m_MaxHealth = 10000;
		m_CurrentHealth = 10000;
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
			if (num < 0.96f || m_LostPlayerTimer >= 2f)
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
		Vector3 vector = m_NavMeshPath.corners[1] - base.transform.position;
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
		if (Time.timeScale != 0f && !m_Paused)
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
		if (Time.timeScale != 0f)
		{
			if (m_SentryState == SentryState.Dying)
			{
				Vector3 forward = m_VerticalRotator.transform.localRotation * Vector3.forward;
				forward.y = 0f;
				forward.Normalize();
				forward.y = -1f;
				Quaternion to = Quaternion.LookRotation(forward);
				m_VerticalRotator.transform.localRotation = Quaternion.RotateTowards(m_VerticalRotator.transform.localRotation, to, 40f * Time.deltaTime);
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
			EventManager.Instance.PostEvent("Security_Robot_Fire_Loop", EventAction.PlaySound, null, base.gameObject);
			m_LeftMuzzleFlash.Play();
			m_RightMuzzleFlash.Play();
		}
	}

	private void EndFiring()
	{
		if (m_Firing)
		{
			m_Firing = false;
			EventManager.Instance.PostEvent("Security_Robot_Fire_Loop", EventAction.StopSound, null, base.gameObject);
			EventManager.Instance.PostEvent("Security_Robot_Fire_Tail", EventAction.PlaySound, null, base.gameObject);
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
			CharacterBase component = hitInfo.collider.gameObject.GetComponent<CharacterBase>();
			if ((bool)component)
			{
				component.TakeDamage(m_BulletDamage, base.gameObject);
			}
		}
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

	public override bool TakeDamage(int Damage, GameObject Damager, DamageType Type = DamageType.Normal)
	{
		return base.TakeDamage(Damage, Damager, Type);
	}

	public override void Die(GameObject Damager, DamageType Type = DamageType.Normal)
	{
		base.Die(Damager, Type);
		Globals.m_AIDirector.SentryDestroyed(this);
		m_SentryState = SentryState.Dying;
		EndFiring();
		if ((bool)m_Collider)
		{
			m_Collider.enabled = false;
		}
		m_NavAgent.Stop(true);
		m_NavAgent.updateRotation = false;
		m_StateTimer = 6f;
		if (Globals.m_PlayerController.m_TargetedEnemy == base.gameObject)
		{
			Globals.m_PlayerController.m_TargetedEnemy = null;
		}
		EventManager.Instance.PostEvent("Security_Robot_Death", EventAction.PlaySound, null, base.gameObject);
	}

	protected override void AttachShadowObject()
	{
		UnityEngine.Object.Destroy(m_ShadowObject);
	}

	public virtual bool CheckVisualSenses()
	{
		if (m_SentryState == SentryState.Dying)
		{
			return false;
		}
		if (Globals.m_PlayerController == null || !Globals.m_PlayerController.gameObject.active)
		{
			m_VisualSensingTimer = 0f;
			return false;
		}
		if (Globals.m_AugmentCloaking.enabled)
		{
			m_VisualSensingTimer = 0f;
			return false;
		}
		Vector3 vector = Globals.m_PlayerController.transform.position - m_RaycastObject.transform.position;
		Vector3 lhs = new Vector3(vector.x, 0f, vector.z);
		float sqrMagnitude = lhs.sqrMagnitude;
		lhs.Normalize();
		float num = Vector3.Dot(lhs, m_RaycastObject.transform.forward);
		if (Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.Firing && Globals.m_PlayerController.m_CoverState != 0 && Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.CoverDive && Vector3.Dot(lhs, Globals.m_PlayerController.m_CoverNormal) >= 0.3f)
		{
			return ProcessVisualTimer(true);
		}
		if (num >= m_ViewingAngleDot && sqrMagnitude <= 400f)
		{
			Ray ray = new Ray(m_RaycastObject.transform.position, Globals.m_PlayerController.GetChestLocation() - m_RaycastObject.transform.position);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 100f, 82177) && (hitInfo.collider.gameObject.layer == 14 || (Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Firing && hitInfo.collider == Globals.m_PlayerController.m_CoverCollider)))
			{
				m_TargetLocation = Globals.m_PlayerController.GetChestLocation();
				m_TargettingType = TurretTargetting.Target;
				m_VisualSensingTimer += Time.deltaTime;
				m_LostPlayerTimer = 0f;
				return ProcessVisualTimer(false);
			}
		}
		return ProcessVisualTimer(true);
	}

	public virtual bool ProcessVisualTimer(bool ResetTimer)
	{
		if (m_SentryState == SentryState.Hostile)
		{
			return false;
		}
		bool result = false;
		if (m_VisualSensingTimer >= 0.75f)
		{
			EnterCombat(Globals.m_PlayerController.GetChestLocation());
			result = true;
		}
		else if (m_VisualSensingTimer >= 0.2f && ResetTimer)
		{
			Investigate(Globals.m_PlayerController.GetChestLocation());
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

	private void EnterCombat(Vector3 location)
	{
		if (m_SentryState != SentryState.Hostile)
		{
			m_NavAgent.Stop(true);
			m_SentryState = SentryState.Hostile;
			m_HostileState = HostileState.StandingIdle;
			m_StateTimer = 2f;
		}
	}
}
