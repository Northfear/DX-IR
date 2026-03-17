using System;
using UnityEngine;

public class BoxRobot : CharacterBase
{
	public enum BoxRobotState
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
		TurningToRePatrol = 2,
		WalkingToRePatrol = 3,
		Total = 4
	}

	public enum AlarmedState
	{
		None = -1,
		StandingIdle = 0,
		TurnToInvestigate = 1,
		WalkingToInvestigate = 2,
		Total = 3
	}

	public enum HostileState
	{
		None = -1,
		StandingIdle = 0,
		FiringMissiles = 1,
		Dodging = 2,
		Moving = 3,
		Total = 4
	}

	public enum AimDirection
	{
		None = -1,
		Center = 0,
		Left = 1,
		Right = 2,
		Up = 3,
		Down = 4,
		UpLeft = 5,
		UpRight = 6,
		DownLeft = 7,
		DownRight = 8,
		Total = 9
	}

	public enum AimingFlag
	{
		None = 0,
		OutsideLeft = 1,
		OutsideRight = 2,
		OutsideAbove = 4,
		OutsideBelow = 8,
		BehindLeft = 0x10,
		BehindRight = 0x20
	}

	private const float m_DeathFadeTime = 3.5f;

	private BoxRobotState m_BoxRobotState = BoxRobotState.None;

	private PatrolState m_PatrolState = PatrolState.None;

	private AlarmedState m_AlarmedState = AlarmedState.None;

	private HostileState m_HostileState = HostileState.None;

	[HideInInspector]
	public EnemySquad m_EnemySquad = EnemySquad.None;

	[HideInInspector]
	public MachineAwareness m_Awareness;

	public Animation m_Animator;

	public NavMeshAgent m_NavAgent;

	public GameObject m_RigMotion;

	public Transform m_Core;

	private NavMeshPath m_NavMeshPath = new NavMeshPath();

	[HideInInspector]
	public bool m_Neutral;

	public Collider m_Collider;

	private SphereCollider m_DeathCollider;

	public Material m_DeathMaterial;

	private bool m_DeadBodyFound;

	private float m_WalkSpeed = 1.15f;

	private float m_StrafeTurnSpeed = 2f;

	private float m_NormalTurnSpeed = 3f;

	private float m_HostileTurnSpeed = 300f;

	private float m_StateTimer;

	private float m_VisualSensingTimer;

	private float m_LostPlayerTimer;

	private float m_InvestigateDelay;

	private float m_MissileTimer = -1f;

	private float m_DodgeTimer = -1f;

	private float m_EventTimer;

	public float m_DodgeChanceOnDamage = 0.15f;

	private bool m_DamagedThisFrame;

	private Vector3 m_TargetMoveToPosition = Vector3.zero;

	private Vector3 m_TargetAimToPosition = Vector3.zero;

	[HideInInspector]
	public Vector3 m_TargetMissileLocation = Vector3.zero;

	[HideInInspector]
	public BoxBot_PatrolNode m_TargetNode;

	public bool m_PatrolForward = true;

	private BoxBot_PatrolNode m_EventNode;

	private Vector3 m_LastPatrolledPosition = Vector3.zero;

	private bool m_SeePlayerThisFrame;

	public float m_ViewingAngleInDegrees = 120f;

	private float m_ViewingAngleDot;

	private float m_DistanceThreshold = 18f;

	private float m_DistanceThresholdSqr;

	private Vector3 m_ToSource = Vector3.zero;

	private Vector3 m_ToSourceNormalizedOnXZ = Vector3.zero;

	private float m_ToSourceDot;

	private float m_ToSourceRightDot;

	private float m_ToSourceUpDot;

	private Direction m_SourceDirection;

	private bool m_BringingInGuns;

	private bool m_ReadyToFire;

	private string m_StunnedAnimation;

	private bool m_Firing;

	public float m_FireRate = 0.2f;

	private float m_FiringTimer;

	public int m_BulletDamage = 10;

	private float m_AccuracyOffset = 0.5f;

	public GameObject m_RaycastObject;

	public ParticleSystem m_LeftMuzzleFlash;

	public ParticleSystem m_RightMuzzleFlash;

	public Transform m_LeftShoulder;

	public Transform m_RightShoulder;

	private string m_TransitionalAnimation;

	private string m_PosedAnimation;

	[HideInInspector]
	public float m_CurrentAnimationSpeed = 1f;

	private float m_TurnThresholdDot = 0.707f;

	private bool m_StrafingToFaceThreat;

	private Quaternion m_TargetStrafeDirection = Quaternion.identity;

	public float m_LeftAimingExtentInDegrees = 28f;

	public float m_RightAimingExtentInDegrees = 30f;

	public float m_UpAimingExtentInDegrees = 50f;

	public float m_DownAimingExtentInDegrees = 45f;

	private byte m_AimingFlags;

	private bool m_Aiming;

	private float m_GlobalAimingWeight;

	private float[] m_AimingWeights = new float[9];

	private AnimationState[] m_AimAnimations = new AnimationState[9];

	private AnimationState m_FiringAnimation;

	[HideInInspector]
	public bool m_Paused;

	private void Awake()
	{
		m_DistanceThresholdSqr = m_DistanceThreshold * m_DistanceThreshold;
		m_ViewingAngleDot = Mathf.Cos((float)Math.PI / 180f * (m_ViewingAngleInDegrees * 0.5f));
		m_LeftMuzzleFlash.Stop();
		m_RightMuzzleFlash.Stop();
		m_AimAnimations[0] = m_Animator["Center"];
		m_AimAnimations[1] = m_Animator["Left"];
		m_AimAnimations[2] = m_Animator["Right"];
		m_AimAnimations[3] = m_Animator["Up"];
		m_AimAnimations[4] = m_Animator["Down"];
		m_AimAnimations[5] = m_Animator["UpLeft"];
		m_AimAnimations[6] = m_Animator["UpRight"];
		m_AimAnimations[7] = m_Animator["DownLeft"];
		m_AimAnimations[8] = m_Animator["DownRight"];
		for (int i = 0; i < 9; i++)
		{
			m_AimAnimations[i].AddMixingTransform(m_LeftShoulder, true);
			m_AimAnimations[i].AddMixingTransform(m_RightShoulder, true);
			m_AimAnimations[i].blendMode = AnimationBlendMode.Blend;
			m_AimAnimations[i].weight = 1f;
			m_AimAnimations[i].layer = 10;
			m_AimAnimations[i].enabled = false;
		}
		m_FiringAnimation = m_Animator["CBT_Fire"];
		m_FiringAnimation.AddMixingTransform(m_LeftShoulder, true);
		m_FiringAnimation.AddMixingTransform(m_RightShoulder, true);
		m_FiringAnimation.blendMode = AnimationBlendMode.Additive;
		m_FiringAnimation.layer = 20;
		m_NavAgent.updateRotation = false;
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
		Globals.m_AIDirector.BoxRobotSpawned(this);
		if (m_Awareness == MachineAwareness.Deactivated)
		{
			m_BoxRobotState = BoxRobotState.Deactivated;
			m_Animator.Play("NCB_Deactivated");
		}
		else
		{
			m_BoxRobotState = BoxRobotState.Patrolling;
			m_PatrolState = PatrolState.StandingIdle;
			if (m_TargetNode != null)
			{
				BoxBot_PatrolNode boxBot_PatrolNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
				base.transform.position = m_TargetNode.transform.position;
				if (boxBot_PatrolNode != null)
				{
					m_PatrolState = PatrolState.WalkingPatrol;
					Vector3 vector = boxBot_PatrolNode.transform.position - m_TargetNode.transform.position;
					vector.y = 0f;
					base.transform.rotation = Quaternion.LookRotation(vector.normalized);
				}
			}
			if (m_PatrolState == PatrolState.WalkingPatrol)
			{
				m_Animator.Play("NCB_Walk_Forward");
			}
			else
			{
				m_Animator.Play("NCB_Idle");
			}
		}
		RayClampToGround();
		m_LastPatrolledPosition = base.transform.position;
	}

	private void Investigate(Vector3 threatLocation)
	{
		if (m_BoxRobotState == BoxRobotState.Dying || m_BoxRobotState == BoxRobotState.Deactivated || m_BoxRobotState == BoxRobotState.Hostile || m_InvestigateDelay > 0f)
		{
			return;
		}
		m_StateTimer = 10000f;
		m_EventTimer = UnityEngine.Random.Range(1f, 3f);
		m_InvestigateDelay = 3.5f;
		m_BoxRobotState = BoxRobotState.Alarmed;
		m_AlarmedState = AlarmedState.StandingIdle;
		m_TargetAimToPosition = threatLocation;
		StopNavigation();
		if (m_TransitionalAnimation == null)
		{
			DetermineSourceDirection(m_TargetAimToPosition);
			if (m_SourceDirection == Direction.BackLeft)
			{
				m_TransitionalAnimation = "NCB_TurnLeft180";
			}
			else if (m_SourceDirection == Direction.BackRight)
			{
				m_TransitionalAnimation = "NCB_TurnRight180";
			}
			else if (m_SourceDirection == Direction.Left)
			{
				m_TransitionalAnimation = "NCB_TurnLeft90";
			}
			else if (m_SourceDirection == Direction.Right)
			{
				m_TransitionalAnimation = "NCB_TurnRight90";
			}
			else
			{
				m_TransitionalAnimation = null;
			}
			if (m_TransitionalAnimation != null)
			{
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
			else
			{
				m_Animator.CrossFade("NCB_Idle");
			}
		}
	}

	private void UpdateBoxRobotAlarmed()
	{
		EndFiring();
		switch (m_AlarmedState)
		{
		case AlarmedState.StandingIdle:
			UpdateAlarmedStandingIdle();
			break;
		case AlarmedState.TurnToInvestigate:
			UpdateAlarmedTurnToInvestigate();
			break;
		case AlarmedState.WalkingToInvestigate:
			UpdateAlarmedWalkingToInvestigate();
			break;
		}
	}

	private void UpdateAlarmedStandingIdle()
	{
		if (m_TransitionalAnimation != null && !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
			m_Animator.CrossFade("NCB_Idle");
			DetermineSourceDirection(m_TargetAimToPosition);
			if (m_SourceDirection == Direction.BackLeft)
			{
				m_TransitionalAnimation = "NCB_TurnLeft180";
			}
			else if (m_SourceDirection == Direction.BackRight)
			{
				m_TransitionalAnimation = "NCB_TurnRight180";
			}
			else if (m_SourceDirection == Direction.Left)
			{
				m_TransitionalAnimation = "NCB_TurnLeft90";
			}
			else if (m_SourceDirection == Direction.Right)
			{
				m_TransitionalAnimation = "NCB_TurnRight90";
			}
			else
			{
				m_TransitionalAnimation = null;
			}
			if (m_TransitionalAnimation != null)
			{
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
		}
		if (m_TransitionalAnimation != null || !(m_EventTimer <= 0f))
		{
			return;
		}
		if (m_StateTimer <= 0f)
		{
			ReturnToPatrol();
			return;
		}
		m_EventTimer = UnityEngine.Random.Range(5f, 10f);
		if (CalculateNavPath(m_TargetAimToPosition, 20f))
		{
			m_AlarmedState = AlarmedState.TurnToInvestigate;
			if (m_NavMeshPath.corners != null && m_NavMeshPath.corners.Length > 1)
			{
				DetermineSourceDirection(m_NavMeshPath.corners[1]);
			}
			else
			{
				DetermineSourceDirection(m_TargetMoveToPosition);
			}
			if (m_SourceDirection == Direction.BackLeft)
			{
				m_TransitionalAnimation = "NCB_TurnLeft180";
			}
			else if (m_SourceDirection == Direction.BackRight)
			{
				m_TransitionalAnimation = "NCB_TurnRight180";
			}
			else if (m_SourceDirection == Direction.Left)
			{
				m_TransitionalAnimation = "NCB_TurnLeft90";
			}
			else if (m_SourceDirection == Direction.Right)
			{
				m_TransitionalAnimation = "NCB_TurnRight90";
			}
			else
			{
				m_TransitionalAnimation = null;
			}
			if (m_TransitionalAnimation != null)
			{
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
		}
	}

	private void UpdateAlarmedTurnToInvestigate()
	{
		if (m_TransitionalAnimation == null || !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
			m_AlarmedState = AlarmedState.WalkingToInvestigate;
			m_Animator.CrossFade("NCB_Walk_Forward");
			ResumeNavigation(true);
		}
	}

	private void UpdateAlarmedWalkingToInvestigate()
	{
		if (m_NavAgent.remainingDistance < 1f && m_NavAgent.remainingDistance != float.PositiveInfinity)
		{
			m_AlarmedState = AlarmedState.StandingIdle;
			StopNavigation();
			m_EventTimer = UnityEngine.Random.Range(5f, 10f);
			m_StateTimer = -1f;
			m_Animator.CrossFade("NCB_Idle");
		}
	}

	public void EnterCombat(bool FirstSight, Vector3 threatLocation)
	{
		if (m_BoxRobotState == BoxRobotState.Dying || m_BoxRobotState == BoxRobotState.Deactivated || m_BoxRobotState == BoxRobotState.Hostile)
		{
			return;
		}
		StopNavigation();
		m_BoxRobotState = BoxRobotState.Hostile;
		m_HostileState = HostileState.StandingIdle;
		m_StateTimer = UnityEngine.Random.Range(8f, 12f);
		m_TargetAimToPosition = threatLocation;
		m_LostPlayerTimer = 0f;
		m_MissileTimer = UnityEngine.Random.Range(15f, 25f);
		m_DodgeTimer = UnityEngine.Random.Range(2f, 5f);
		m_EventTimer = UnityEngine.Random.Range(5f, 10f);
		if (FirstSight)
		{
			Globals.m_AIDirector.BoxRobotEnteredCombat(this, threatLocation);
		}
		m_StrafingToFaceThreat = false;
		m_BringingInGuns = false;
		m_ReadyToFire = false;
		m_Neutral = false;
		if (m_TransitionalAnimation == null)
		{
			DetermineSourceDirection(m_TargetAimToPosition);
			if (m_SourceDirection == Direction.BackLeft)
			{
				m_TransitionalAnimation = "CBT_TurnLeft180";
			}
			else if (m_SourceDirection == Direction.BackRight)
			{
				m_TransitionalAnimation = "CBT_TurnRight180";
			}
			else if (m_SourceDirection == Direction.Left && Mathf.Abs(m_ToSourceRightDot) >= m_TurnThresholdDot)
			{
				m_TransitionalAnimation = "CBT_TurnLeft90";
			}
			else if (m_SourceDirection == Direction.Right && Mathf.Abs(m_ToSourceRightDot) >= m_TurnThresholdDot)
			{
				m_TransitionalAnimation = "CBT_TurnRight90";
			}
			else
			{
				m_TransitionalAnimation = "CBT_CombatEnter";
			}
			if (m_TransitionalAnimation != null)
			{
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
			m_BringingInGuns = true;
		}
	}

	private void UpdateBoxRobotHostile()
	{
		switch (m_HostileState)
		{
		case HostileState.StandingIdle:
			UpdateHostileStandingIdle();
			break;
		case HostileState.FiringMissiles:
			UpdateHostileFiringMissiles();
			break;
		case HostileState.Dodging:
			UpdateHostileDodging();
			break;
		case HostileState.Moving:
			UpdateHostileMoving();
			break;
		}
	}

	private void UpdateHostileStandingIdle()
	{
		if (m_TransitionalAnimation != null && !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
			m_Animator.CrossFade("CBT_Idle");
			if (m_BringingInGuns)
			{
				EnableAiming(true, false);
				m_ReadyToFire = true;
			}
		}
		if (m_TransitionalAnimation == null)
		{
			if (!m_ReadyToFire)
			{
				if (m_SourceDirection == Direction.BackLeft)
				{
					m_TransitionalAnimation = "CBT_TurnLeft180";
				}
				else if (m_SourceDirection == Direction.BackRight)
				{
					m_TransitionalAnimation = "CBT_TurnRight180";
				}
				else if (m_SourceDirection == Direction.Left)
				{
					m_TransitionalAnimation = "CBT_TurnLeft90";
				}
				else if (m_SourceDirection == Direction.Right)
				{
					m_TransitionalAnimation = "CBT_TurnRight90";
				}
				else
				{
					m_TransitionalAnimation = "CBT_CombatEnter";
				}
				if (m_TransitionalAnimation != null)
				{
					m_PosedAnimation = m_TransitionalAnimation + "_Pose";
					m_Animator.CrossFade(m_TransitionalAnimation);
				}
				m_BringingInGuns = true;
			}
			else
			{
				if (m_StrafingToFaceThreat)
				{
					base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, m_TargetStrafeDirection, m_StrafeTurnSpeed);
					float f = Quaternion.Dot(base.transform.rotation, m_TargetStrafeDirection);
					if (Mathf.Abs(f) >= 0.9995f)
					{
						m_StrafingToFaceThreat = false;
						m_Animator.CrossFade("CBT_Idle");
					}
				}
				if ((m_AimingFlags & 0x33) != 0)
				{
					if ((m_AimingFlags & 0x10) != 0)
					{
						m_TransitionalAnimation = "CBT_TurnLeft180";
					}
					else if ((m_AimingFlags & 0x20) != 0)
					{
						m_TransitionalAnimation = "CBT_TurnRight180";
					}
					else if ((m_AimingFlags & 1) != 0)
					{
						if (Mathf.Abs(m_ToSourceRightDot) >= m_TurnThresholdDot)
						{
							m_StrafingToFaceThreat = false;
							m_TransitionalAnimation = "CBT_TurnLeft90";
						}
						else
						{
							m_StrafingToFaceThreat = true;
							m_TargetStrafeDirection = Quaternion.LookRotation(m_ToSourceNormalizedOnXZ);
							m_Animator.CrossFade("CBT_Walk_Left");
						}
					}
					else if (Mathf.Abs(m_ToSourceRightDot) >= m_TurnThresholdDot)
					{
						m_StrafingToFaceThreat = false;
						m_TransitionalAnimation = "CBT_TurnRight90";
					}
					else
					{
						m_StrafingToFaceThreat = true;
						m_TargetStrafeDirection = Quaternion.LookRotation(m_ToSourceNormalizedOnXZ);
						m_Animator.CrossFade("CBT_Walk_Right");
					}
					if (m_TransitionalAnimation != null)
					{
						m_PosedAnimation = m_TransitionalAnimation + "_Pose";
						m_Animator.CrossFade(m_TransitionalAnimation);
					}
				}
			}
		}
		if (m_TransitionalAnimation == null)
		{
			if (m_LostPlayerTimer >= 30f)
			{
				EnableAiming(false, false);
				ReturnToPatrol();
				return;
			}
			if (m_MissileTimer <= 0f)
			{
				m_StrafingToFaceThreat = false;
				m_HostileState = HostileState.FiringMissiles;
				m_TransitionalAnimation = "CBT_MissileLaunch_WithEvents";
				m_PosedAnimation = "CBT_MissileLaunch_WithEvents_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
				EnableAiming(false, false);
				m_TargetMissileLocation = Globals.m_AIDirector.m_LastKnownPlayerPosition;
				return;
			}
			if (m_DodgeTimer <= 0f && m_DamagedThisFrame && UnityEngine.Random.value <= m_DodgeChanceOnDamage)
			{
				bool[] array = new bool[4];
				NavMeshHit hit;
				if (!Physics.Raycast(new Ray(base.transform.position + Vector3.up * 0.5f, -base.transform.right), 4.2f, 6374145) && NavMesh.SamplePosition(base.transform.position - base.transform.right * 4f, out hit, 0.5f, m_NavAgent.walkableMask))
				{
					array[0] = Globals.Approximately((base.transform.position - hit.position).magnitude, 4f, 0.15f);
				}
				if (!Physics.Raycast(new Ray(base.transform.position + Vector3.up * 0.5f, base.transform.right), 4.2f, 6374145) && NavMesh.SamplePosition(base.transform.position + base.transform.right * 4f, out hit, 0.5f, m_NavAgent.walkableMask))
				{
					array[1] = Globals.Approximately((base.transform.position - hit.position).magnitude, 4f, 0.15f);
				}
				if (!Physics.Raycast(new Ray(base.transform.position + Vector3.up * 0.5f, base.transform.forward), 4.2f, 6374145) && NavMesh.SamplePosition(base.transform.position + base.transform.forward * 4f, out hit, 0.5f, m_NavAgent.walkableMask))
				{
					array[2] = Globals.Approximately((base.transform.position - hit.position).magnitude, 4f, 0.15f);
				}
				if (!Physics.Raycast(new Ray(base.transform.position + Vector3.up * 0.5f, -base.transform.forward), 4.2f, 6374145) && NavMesh.SamplePosition(base.transform.position - base.transform.forward * 4f, out hit, 0.5f, m_NavAgent.walkableMask))
				{
					array[3] = Globals.Approximately((base.transform.position - hit.position).magnitude, 4f, 0.15f);
				}
				int num = ((!(UnityEngine.Random.value <= 0.5f)) ? 1 : 0);
				int i;
				for (i = 0; i < 4; i++)
				{
					if (array[num])
					{
						break;
					}
					num = (num + 1) % 4;
				}
				if (i < 4)
				{
					switch (num)
					{
					case 0:
						m_TransitionalAnimation = "CBT_Dodge_Left";
						break;
					case 1:
						m_TransitionalAnimation = "CBT_Dodge_Right";
						break;
					case 2:
						m_TransitionalAnimation = "CBT_Dodge_Front";
						break;
					default:
						m_TransitionalAnimation = "CBT_Dodge_Back";
						break;
					}
					m_StrafingToFaceThreat = false;
					m_HostileState = HostileState.Dodging;
					m_PosedAnimation = m_TransitionalAnimation + "_Pose";
					m_Animator.CrossFade(m_TransitionalAnimation);
					return;
				}
			}
		}
		if (m_TransitionalAnimation == null && !(m_EventTimer <= 0f))
		{
		}
	}

	private void UpdateHostileFiringMissiles()
	{
		if (m_TransitionalAnimation == null || !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
			m_Animator.CrossFade("CBT_Idle");
			EnableAiming(true, false);
			m_HostileState = HostileState.StandingIdle;
			m_MissileTimer = UnityEngine.Random.Range(15f, 25f);
			m_EventTimer = UnityEngine.Random.Range(5f, 10f);
		}
	}

	private void UpdateHostileDodging()
	{
		if (m_TransitionalAnimation == null || !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
			m_Animator.CrossFade("CBT_Idle");
			m_HostileState = HostileState.StandingIdle;
			m_EventTimer = UnityEngine.Random.Range(3f, 7f);
		}
	}

	private void UpdateHostileMoving()
	{
		if (m_EventTimer <= 0f && m_MissileTimer <= 0f)
		{
			StopNavigation();
			m_HostileState = HostileState.FiringMissiles;
			m_TransitionalAnimation = "CBT_MissileLaunch_WithEvents";
			m_PosedAnimation = "CBT_MissileLaunch_WithEvents_Pose";
			m_Animator.CrossFade(m_TransitionalAnimation);
			EnableAiming(false, false);
			m_TargetMissileLocation = Globals.m_AIDirector.m_LastKnownPlayerPosition;
			return;
		}
		if ((m_NavAgent.remainingDistance <= 0.5f && m_NavAgent.remainingDistance != float.PositiveInfinity) || (m_StateTimer <= 0f && m_SeePlayerThisFrame))
		{
			StopNavigation();
			m_HostileState = HostileState.StandingIdle;
			m_EventTimer = UnityEngine.Random.Range(3f, 7f);
			m_Animator.CrossFade("CBT_Idle");
			return;
		}
		Vector3 vector = Globals.m_AIDirector.m_LastKnownPlayerPosition - base.transform.position;
		vector.y = 0f;
		vector.Normalize();
		Vector3 velocity = m_NavAgent.velocity;
		velocity.y = 0f;
		velocity.Normalize();
		float num = Vector3.Dot(vector, velocity);
		float num2 = Vector3.Dot(vector, new Vector3(velocity.z, 0f, 0f - velocity.x));
		if (Mathf.Abs(num) >= Mathf.Abs(num2))
		{
			if (num >= 0f)
			{
				m_Animator.CrossFade("CBT_Walk_Forward");
			}
			else
			{
				m_Animator.CrossFade("CBT_Walk_Back");
			}
		}
		else if (num2 <= 0f)
		{
			m_Animator.CrossFade("CBT_Walk_Right");
		}
		else
		{
			m_Animator.CrossFade("CBT_Walk_Left");
		}
		Quaternion to = Quaternion.LookRotation(vector);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, 180f * Time.deltaTime);
	}

	private void ReturnToPatrol()
	{
		m_BoxRobotState = BoxRobotState.Patrolling;
		StopNavigation();
		if ((m_LastPatrolledPosition - base.transform.position).sqrMagnitude <= 3f)
		{
			if (m_TargetNode != null)
			{
				m_PatrolState = PatrolState.WalkingPatrol;
				DetermineSourceDirection(m_TargetNode.transform.position);
				if (m_SourceDirection == Direction.BackLeft)
				{
					m_TransitionalAnimation = "NCB_TurnLeft180";
				}
				else if (m_SourceDirection == Direction.BackRight)
				{
					m_TransitionalAnimation = "NCB_TurnRight180";
				}
				else if (m_SourceDirection == Direction.Left)
				{
					m_TransitionalAnimation = "NCB_TurnLeft90";
				}
				else if (m_SourceDirection == Direction.Right)
				{
					m_TransitionalAnimation = "NCB_TurnRight90";
				}
				else
				{
					m_TransitionalAnimation = null;
				}
				if (m_TransitionalAnimation != null)
				{
					m_PosedAnimation = m_TransitionalAnimation + "_Pose";
					m_Animator.CrossFade(m_TransitionalAnimation);
				}
				else
				{
					m_Animator.CrossFade("NCB_Walk_Forward");
				}
			}
			else
			{
				m_PatrolState = PatrolState.StandingIdle;
				m_Animator.CrossFade("NCB_Idle");
			}
		}
		else if (CalculateNavPath(m_LastPatrolledPosition, 20f))
		{
			if (m_NavMeshPath.corners != null && m_NavMeshPath.corners.Length > 1)
			{
				DetermineSourceDirection(m_NavMeshPath.corners[1]);
			}
			else
			{
				DetermineSourceDirection(m_TargetMoveToPosition);
			}
			if (m_SourceDirection == Direction.BackLeft)
			{
				m_TransitionalAnimation = "NCB_TurnLeft180";
			}
			else if (m_SourceDirection == Direction.BackRight)
			{
				m_TransitionalAnimation = "NCB_TurnRight180";
			}
			else if (m_SourceDirection == Direction.Left)
			{
				m_TransitionalAnimation = "NCB_TurnLeft90";
			}
			else if (m_SourceDirection == Direction.Right)
			{
				m_TransitionalAnimation = "NCB_TurnRight90";
			}
			else
			{
				m_TransitionalAnimation = null;
			}
			if (m_TransitionalAnimation != null)
			{
				m_PatrolState = PatrolState.TurningToRePatrol;
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
			else
			{
				m_PatrolState = PatrolState.WalkingToRePatrol;
				m_Animator.CrossFade("NCB_Walk_Forward");
				ResumeNavigation(true);
			}
		}
		else
		{
			m_PatrolState = PatrolState.StandingIdle;
			m_Animator.CrossFade("NCB_Idle");
		}
	}

	private void UpdateBoxRobotPatrolling()
	{
		EndFiring();
		switch (m_PatrolState)
		{
		case PatrolState.StandingIdle:
			UpdateBoxRobotStandingIdle();
			break;
		case PatrolState.WalkingPatrol:
			UpdateBoxRobotWalkingPatrol();
			break;
		case PatrolState.TurningToRePatrol:
			UpdateBoxRobotTurningToRePatrol();
			break;
		case PatrolState.WalkingToRePatrol:
			UpdateBoxRobotWalkingToRePatrol();
			break;
		}
	}

	private void UpdateBoxRobotStandingIdle()
	{
		if (!(m_StateTimer <= 0f))
		{
			return;
		}
		if (m_EventNode != null)
		{
			if (m_EventNode.m_PatrolEvent == BoxBot_PatrolNode.PatrolEvent.TurnBack && m_TargetNode != null)
			{
				m_PatrolState = PatrolState.WalkingPatrol;
				m_TransitionalAnimation = ((!(UnityEngine.Random.value <= 0.5f)) ? "NCB_TurnRight180" : "NCB_TurnLeft180");
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
			else if (m_TargetNode != null)
			{
				m_PatrolState = PatrolState.WalkingPatrol;
				m_Animator.CrossFade("NCB_Walk_Forward");
			}
			m_EventNode = null;
		}
		else if (m_TargetNode != null)
		{
			m_PatrolState = PatrolState.WalkingPatrol;
		}
	}

	private void UpdateBoxRobotWalkingPatrol()
	{
		if (m_TransitionalAnimation != null)
		{
			if (m_Animator.IsPlaying(m_TransitionalAnimation))
			{
				return;
			}
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
			if (!(m_TargetNode != null))
			{
				m_PatrolState = PatrolState.StandingIdle;
				m_Animator.CrossFade("NCB_Idle");
				return;
			}
			m_Animator.CrossFade("NCB_Walk_Forward");
		}
		Vector3 vector = m_TargetNode.transform.position - base.transform.position;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		float num = m_WalkSpeed * Time.deltaTime;
		if (magnitude <= num)
		{
			base.transform.position = m_TargetNode.transform.position;
			m_LastPatrolledPosition = base.transform.position;
			num -= magnitude;
			if (BoxRobotPatrolEventTriggered())
			{
				m_StateTimer = UnityEngine.Random.Range(m_EventNode.m_MinIdle, m_EventNode.m_MaxIdle);
				if (m_EventNode.m_PatrolEvent == BoxBot_PatrolNode.PatrolEvent.TurnBack)
				{
					m_PatrolForward = !m_PatrolForward;
				}
				m_TargetNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
				m_PatrolState = PatrolState.StandingIdle;
				m_Animator.CrossFade("NCB_Idle");
				return;
			}
			m_TargetNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
			if (!(m_TargetNode != null))
			{
				m_PatrolState = PatrolState.StandingIdle;
				m_Animator.CrossFade("NCB_Idle");
				return;
			}
			vector = m_TargetNode.transform.position - base.transform.position;
			vector.y = 0f;
			Vector3 right = base.transform.right;
			right.y = 0f;
			float num2 = Vector3.Dot(vector.normalized, right.normalized);
			if (num2 >= 0.85f)
			{
				m_TransitionalAnimation = "NCB_TurnRight90";
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
				return;
			}
			if (num2 <= -0.85f)
			{
				m_TransitionalAnimation = "NCB_TurnLeft90";
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
				return;
			}
		}
		float maxDegreesDelta = ((!(magnitude <= 1f)) ? m_NormalTurnSpeed : (m_NormalTurnSpeed * 2f));
		Quaternion to = Quaternion.LookRotation(vector.normalized);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, maxDegreesDelta);
		m_NavAgent.Move(base.transform.forward * num);
		m_LastPatrolledPosition = base.transform.position;
	}

	private void UpdateBoxRobotTurningToRePatrol()
	{
		if (m_TransitionalAnimation == null || !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
			m_PatrolState = PatrolState.WalkingToRePatrol;
			m_Animator.CrossFade("NCB_Walk_Forward");
			ResumeNavigation(true);
		}
	}

	private void UpdateBoxRobotWalkingToRePatrol()
	{
		if (!(m_NavAgent.remainingDistance < 1f) || m_NavAgent.remainingDistance == float.PositiveInfinity)
		{
			return;
		}
		StopNavigation();
		if (m_TargetNode != null)
		{
			m_PatrolState = PatrolState.WalkingPatrol;
			DetermineSourceDirection(m_TargetNode.transform.position);
			if (m_SourceDirection == Direction.BackLeft)
			{
				m_TransitionalAnimation = "NCB_TurnLeft180";
			}
			else if (m_SourceDirection == Direction.BackRight)
			{
				m_TransitionalAnimation = "NCB_TurnRight180";
			}
			else if (m_SourceDirection == Direction.Left)
			{
				m_TransitionalAnimation = "NCB_TurnLeft90";
			}
			else if (m_SourceDirection == Direction.Right)
			{
				m_TransitionalAnimation = "NCB_TurnRight90";
			}
			else
			{
				m_TransitionalAnimation = null;
			}
			if (m_TransitionalAnimation != null)
			{
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
			else
			{
				m_Animator.CrossFade("NCB_Walk_Forward");
			}
		}
		else
		{
			m_PatrolState = PatrolState.StandingIdle;
			m_Animator.CrossFade("NCB_Idle");
		}
	}

	protected override void Update()
	{
		base.Update();
		if (!m_Paused)
		{
			m_StateTimer -= Time.deltaTime;
			m_EventTimer -= Time.deltaTime;
			m_InvestigateDelay -= Time.deltaTime;
			m_LostPlayerTimer += Time.deltaTime;
			m_MissileTimer -= Time.deltaTime;
			m_DodgeTimer -= Time.deltaTime;
			if (m_StunnedAnimation != null && !m_Animator.IsPlaying(m_StunnedAnimation))
			{
				m_StunnedAnimation = null;
			}
			switch (m_BoxRobotState)
			{
			case BoxRobotState.Patrolling:
				UpdateBoxRobotPatrolling();
				break;
			case BoxRobotState.Alarmed:
				UpdateBoxRobotAlarmed();
				break;
			case BoxRobotState.Hostile:
				UpdateBoxRobotHostile();
				break;
			case BoxRobotState.Dying:
				UpdateBoxRobotDying();
				break;
			}
			CheckVisualSenses();
			RayClampToGround();
			UpdateAimBlending();
			UpdateFiring();
			m_DamagedThisFrame = false;
		}
	}

	private void UpdateBoxRobotDying()
	{
		if (m_DeathCollider != null)
		{
			if (!m_DeadBodyFound && Globals.m_AIDirector.CheckVisualSenses(m_DeathCollider, DisturbanceEvent.MajorVisual, AudioEvent.DeadBodyFound))
			{
				m_DeadBodyFound = true;
			}
			Globals.m_AIDirector.ShowBodyToCameras(m_DeathCollider);
		}
		if (!m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			m_StateTimer -= Time.deltaTime;
			m_MeshRenderer.material.SetFloat("_DissolvePower", m_StateTimer / 3.5f);
			if (m_StateTimer <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	private void UpdateAimBlending()
	{
		if (m_Aiming)
		{
			m_GlobalAimingWeight = Mathf.Clamp01(m_GlobalAimingWeight + 3f * Time.deltaTime);
			m_TargetAimToPosition = Globals.m_AIDirector.m_LastKnownPlayerPosition;
			m_TargetAimToPosition.y -= Globals.m_PlayerYOffset;
			m_TargetAimToPosition.y -= 1.5f;
			if (Globals.m_PlayerController.m_Stance == PlayerController.Stance.Crouch && m_SeePlayerThisFrame)
			{
				m_TargetAimToPosition.y -= 0.5f;
			}
			DetermineSourceDirection(m_TargetAimToPosition);
			float num = 57.29578f * Mathf.Acos(m_ToSourceDot);
			float num2 = 90f - 57.29578f * Mathf.Acos(m_ToSourceUpDot);
			float num3 = Mathf.Clamp01(Mathf.Abs(num / ((!(m_ToSourceRightDot >= 0f)) ? m_LeftAimingExtentInDegrees : m_RightAimingExtentInDegrees)));
			float num4 = Mathf.Clamp(num2 / ((!(m_ToSourceUpDot >= 0f)) ? m_DownAimingExtentInDegrees : m_UpAimingExtentInDegrees), -1f, 1f);
			m_AimingFlags = 0;
			if (m_SourceDirection == Direction.BackLeft)
			{
				m_AimingFlags |= 16;
			}
			else if (m_SourceDirection == Direction.BackRight)
			{
				m_AimingFlags |= 32;
			}
			if (num3 >= 1f)
			{
				m_AimingFlags |= (byte)((!(m_ToSourceRightDot >= 0f)) ? 1 : 2);
			}
			if (num4 <= -1f)
			{
				m_AimingFlags |= 8;
			}
			else if (num4 >= 1f)
			{
				m_AimingFlags |= 4;
			}
			float[] array = new float[9];
			for (int i = 0; i < 9; i++)
			{
				array[i] = 0f;
			}
			if (m_ToSourceRightDot >= 0f)
			{
				if (num4 >= 0f)
				{
					array[0] = Globals.BiLerp(1f, 0f, 0f, 0f, num3, num4);
					array[2] = Globals.BiLerp(0f, 1f, 0f, 0f, num3, num4);
					array[3] = Globals.BiLerp(0f, 0f, 1f, 0f, num3, num4);
					array[6] = Globals.BiLerp(0f, 0f, 0f, 1f, num3, num4);
				}
				else
				{
					array[4] = Globals.BiLerp(1f, 0f, 0f, 0f, num3, 1f + num4);
					array[8] = Globals.BiLerp(0f, 1f, 0f, 0f, num3, 1f + num4);
					array[0] = Globals.BiLerp(0f, 0f, 1f, 0f, num3, 1f + num4);
					array[2] = Globals.BiLerp(0f, 0f, 0f, 1f, num3, 1f + num4);
				}
			}
			else if (num4 >= 0f)
			{
				array[1] = Globals.BiLerp(1f, 0f, 0f, 0f, 1f - num3, num4);
				array[0] = Globals.BiLerp(0f, 1f, 0f, 0f, 1f - num3, num4);
				array[5] = Globals.BiLerp(0f, 0f, 1f, 0f, 1f - num3, num4);
				array[3] = Globals.BiLerp(0f, 0f, 0f, 1f, 1f - num3, num4);
			}
			else
			{
				array[7] = Globals.BiLerp(1f, 0f, 0f, 0f, 1f - num3, 1f + num4);
				array[4] = Globals.BiLerp(0f, 1f, 0f, 0f, 1f - num3, 1f + num4);
				array[1] = Globals.BiLerp(0f, 0f, 1f, 0f, 1f - num3, 1f + num4);
				array[0] = Globals.BiLerp(0f, 0f, 0f, 1f, 1f - num3, 1f + num4);
			}
			for (int j = 0; j < 9; j++)
			{
				if (m_AimingWeights[j] < array[j])
				{
					m_AimingWeights[j] = Mathf.Min(m_AimingWeights[j] + 5f * Time.deltaTime, array[j]);
				}
				else if (m_AimingWeights[j] > array[j])
				{
					m_AimingWeights[j] = Mathf.Max(m_AimingWeights[j] - 5f * Time.deltaTime, array[j]);
				}
				m_AimAnimations[j].weight = m_GlobalAimingWeight * m_AimingWeights[j];
			}
		}
		else
		{
			if (!(m_GlobalAimingWeight > 0f))
			{
				return;
			}
			m_GlobalAimingWeight -= 1f * Time.deltaTime;
			if (m_GlobalAimingWeight <= 0f)
			{
				for (int k = 0; k < 9; k++)
				{
					m_AimAnimations[k].enabled = false;
				}
			}
			else
			{
				for (int l = 0; l < 9; l++)
				{
					m_AimAnimations[l].weight = m_GlobalAimingWeight * m_AimingWeights[l];
				}
			}
		}
	}

	private void UpdateFiring()
	{
		float num = Vector3.Dot(m_RaycastObject.transform.forward, (Globals.m_PlayerController.GetChestLocation() - m_RaycastObject.transform.position).normalized);
		m_FiringTimer -= Time.deltaTime;
		if (m_Firing)
		{
			if (m_FiringTimer <= 0f)
			{
				FireBullet();
				m_FiringTimer = m_FireRate;
			}
			if (!m_Aiming || num < 0.96f || m_LostPlayerTimer >= 1f || m_BoxRobotState != BoxRobotState.Hostile || m_HostileState == HostileState.FiringMissiles || m_HostileState == HostileState.Dodging || !m_ReadyToFire || m_StunnedAnimation != null)
			{
				EndFiring();
				SoundManager.TriggerEvent("Stop_BoxGuard_Fire", base.gameObject);
			}
		}
		else if (m_Aiming && num >= 0.96f && m_LostPlayerTimer <= 0.25f && m_BoxRobotState == BoxRobotState.Hostile && m_HostileState != HostileState.FiringMissiles && m_HostileState != HostileState.Dodging && m_ReadyToFire && m_StunnedAnimation == null)
		{
			StartFiring();
			SoundManager.TriggerEvent("Play_BoxGuard_Fire", base.gameObject);
		}
	}

	public void Dominate(MachineAwareness awareness)
	{
		if (m_BoxRobotState != BoxRobotState.Dying)
		{
			m_Awareness = awareness;
			if (awareness == MachineAwareness.Deactivated && m_BoxRobotState != BoxRobotState.Deactivated)
			{
				m_BoxRobotState = BoxRobotState.Deactivated;
				StopNavigation();
				m_Animator.CrossFade("NCB_Deactivated");
				EnableAiming(false, false);
			}
			else if (awareness != MachineAwareness.Deactivated && m_BoxRobotState == BoxRobotState.Deactivated)
			{
				ReturnToPatrol();
			}
		}
	}

	private bool BoxRobotPatrolEventTriggered()
	{
		if (m_TargetNode.m_PatrolEvent == BoxBot_PatrolNode.PatrolEvent.None || m_TargetNode.m_EventChance == BoxBot_PatrolNode.EventChance.Never)
		{
			return false;
		}
		float num = -1f;
		switch (m_TargetNode.m_EventChance)
		{
		case BoxBot_PatrolNode.EventChance.Always:
			num = 1f;
			break;
		case BoxBot_PatrolNode.EventChance.Often:
			num = 0.75f;
			break;
		case BoxBot_PatrolNode.EventChance.Sometimes:
			num = 0.5f;
			break;
		case BoxBot_PatrolNode.EventChance.Rarely:
			num = 0.25f;
			break;
		case BoxBot_PatrolNode.EventChance.Once:
			num = 1f;
			m_TargetNode.m_EventChance = BoxBot_PatrolNode.EventChance.Never;
			break;
		}
		if (UnityEngine.Random.value <= num)
		{
			m_EventNode = m_TargetNode;
			return true;
		}
		return false;
	}

	private void DetermineSourceDirection(Vector3 sourceLocation)
	{
		m_ToSource = sourceLocation - m_RigMotion.transform.position;
		m_ToSourceNormalizedOnXZ = Vector3.Normalize(new Vector3(m_ToSource.x, 0f, m_ToSource.z));
		m_ToSourceDot = Vector3.Dot(m_ToSourceNormalizedOnXZ, -m_RigMotion.transform.up);
		m_ToSourceRightDot = Vector3.Dot(m_ToSourceNormalizedOnXZ, m_RigMotion.transform.right);
		m_ToSourceUpDot = m_ToSource.normalized.y;
		if (Mathf.Abs(m_ToSourceDot) >= Mathf.Abs(m_ToSourceRightDot))
		{
			if (m_ToSourceDot >= 0f)
			{
				m_SourceDirection = Direction.Forward;
			}
			else
			{
				m_SourceDirection = ((!(m_ToSourceRightDot >= 0f)) ? Direction.BackLeft : Direction.BackRight);
			}
		}
		else if (m_ToSourceRightDot >= 0f)
		{
			m_SourceDirection = Direction.Right;
		}
		else
		{
			m_SourceDirection = Direction.Left;
		}
	}

	private void RecalibrateBase()
	{
		Vector3 position = m_RigMotion.transform.position;
		Quaternion rotation = m_RigMotion.transform.rotation;
		base.transform.position = position;
		base.transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
		m_RigMotion.transform.localPosition = Vector3.zero;
		m_RigMotion.transform.localRotation = Quaternion.identity;
	}

	private bool CalculateNavPath(Vector3 targetLocation, float radius = 20f)
	{
		NavMeshHit hit;
		if (!NavMesh.SamplePosition(targetLocation, out hit, radius, m_NavAgent.walkableMask))
		{
			return false;
		}
		m_TargetMoveToPosition = hit.position;
		m_NavAgent.updateRotation = false;
		m_NavAgent.updatePosition = false;
		m_NavAgent.ResetPath();
		m_NavAgent.enabled = false;
		m_NavAgent.enabled = true;
		return m_NavAgent.CalculatePath(m_TargetMoveToPosition, m_NavMeshPath);
	}

	private void ResumeNavigation(bool UpdateRotation)
	{
		m_NavAgent.updatePosition = true;
		m_NavAgent.updateRotation = UpdateRotation;
		m_NavAgent.SetPath(m_NavMeshPath);
		m_NavAgent.Resume();
	}

	private void StopNavigation()
	{
		m_NavAgent.Stop(true);
		m_NavAgent.updatePosition = true;
		m_NavAgent.updateRotation = false;
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

	private void EnableAiming(bool Enable, bool Immediate = false)
	{
		m_Aiming = Enable;
		if (!m_Aiming)
		{
			if (Immediate)
			{
				m_GlobalAimingWeight = 0f;
			}
		}
		else
		{
			for (int i = 0; i < 9; i++)
			{
				m_AimAnimations[i].enabled = true;
			}
		}
	}

	private void StartFiring()
	{
		if (!m_Firing)
		{
			m_Firing = true;
			m_LeftMuzzleFlash.Play();
			m_RightMuzzleFlash.Play();
		}
	}

	private void EndFiring()
	{
		if (m_Firing)
		{
			m_Firing = false;
			m_LeftMuzzleFlash.Stop();
			m_RightMuzzleFlash.Stop();
			m_LeftMuzzleFlash.Clear();
			m_RightMuzzleFlash.Clear();
		}
	}

	private void FireBullet()
	{
		Vector3 vector = Globals.m_PlayerController.GetChestLocation() - m_RaycastObject.transform.position;
		Quaternion quaternion = Quaternion.Euler(UnityEngine.Random.Range(0f - m_AccuracyOffset, m_AccuracyOffset), UnityEngine.Random.Range(0f - m_AccuracyOffset, m_AccuracyOffset), 0f);
		vector = quaternion * vector.normalized;
		Ray ray = new Ray(m_RaycastObject.transform.position, vector);
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
		bool seePlayerThisFrame = m_SeePlayerThisFrame;
		m_SeePlayerThisFrame = false;
		if (m_BoxRobotState == BoxRobotState.Dying || m_BoxRobotState == BoxRobotState.Deactivated)
		{
			m_VisualSensingTimer = 0f;
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
		Vector3 vector = Globals.m_PlayerController.transform.position - m_RigMotion.transform.position;
		Vector3 lhs = new Vector3(vector.x, 0f, vector.z);
		float sqrMagnitude = lhs.sqrMagnitude;
		lhs.Normalize();
		float num = Vector3.Dot(lhs, -m_RigMotion.transform.up);
		if (Globals.m_PlayerController.IsInCover() && Vector3.Dot(lhs, Globals.m_PlayerController.m_CoverNormal) >= 0.3f)
		{
			return ProcessVisualTimer(true);
		}
		if (num >= m_ViewingAngleDot && sqrMagnitude <= 1200f)
		{
			Ray ray = new Ray(m_Core.position, Globals.m_PlayerController.GetChestLocation() - m_Core.position);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 100f, 82177) && (hitInfo.collider.gameObject.layer == 14 || (hitInfo.collider == Globals.m_PlayerController.m_CoverCollider && Globals.m_PlayerController.ExposedInCover())))
			{
				m_SeePlayerThisFrame = true;
				Globals.m_AIDirector.UpdatePlayerKnownPosition();
				m_LostPlayerTimer = 0f;
				m_VisualSensingTimer += Time.deltaTime;
				return ProcessVisualTimer(false);
			}
		}
		return ProcessVisualTimer(true);
	}

	public virtual bool ProcessVisualTimer(bool ResetTimer)
	{
		if (m_BoxRobotState == BoxRobotState.Hostile)
		{
			return false;
		}
		HostilityLevel playerHostilityLevel = HostilityZone.GetPlayerHostilityLevel();
		bool result = false;
		if (m_VisualSensingTimer >= 0.75f && (!m_Neutral || HostilityZone.IsPlayerInHostileTerritory()))
		{
			EnterCombat(true, Globals.m_PlayerController.transform.position);
			result = true;
		}
		else if (m_VisualSensingTimer >= 0.2f && ResetTimer && (!m_Neutral || HostilityZone.IsPlayerInHostileTerritory()))
		{
			Investigate(Globals.m_PlayerController.transform.position);
			result = true;
		}
		if (ResetTimer)
		{
			m_VisualSensingTimer = 0f;
		}
		return result;
	}

	private void Stun()
	{
		StopNavigation();
		bool flag = false;
		if (m_TransitionalAnimation != null)
		{
			flag = true;
			Vector3 position = m_RigMotion.transform.position;
			Quaternion rotation = m_RigMotion.transform.rotation;
			m_Animator.Stop(m_TransitionalAnimation);
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
			base.transform.position = position;
			base.transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
			m_RigMotion.transform.localPosition = Vector3.zero;
			m_RigMotion.transform.localRotation = Quaternion.identity;
		}
		if (m_BoxRobotState == BoxRobotState.Hostile && m_BringingInGuns)
		{
			m_TransitionalAnimation = "CBT_Stun";
			m_StunnedAnimation = m_TransitionalAnimation;
			m_PosedAnimation = null;
			if (flag)
			{
				m_Animator.Play(m_TransitionalAnimation);
			}
			else
			{
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
		}
		else
		{
			m_TransitionalAnimation = "NCB_Stun";
			m_StunnedAnimation = m_TransitionalAnimation;
			m_PosedAnimation = null;
			if (flag)
			{
				m_Animator.Play(m_TransitionalAnimation);
			}
			else
			{
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
		}
	}

	public override bool TakeDamage(DamageData data)
	{
		if (m_BoxRobotState == BoxRobotState.Dying)
		{
			return true;
		}
		bool flag = base.TakeDamage(data);
		if (!flag && m_BoxRobotState != BoxRobotState.Deactivated)
		{
			m_DamagedThisFrame = true;
			Globals.m_AIDirector.UpdatePlayerKnownPosition();
			if (data.m_DamageType == DamageType.EMP && m_StunnedAnimation == null)
			{
				Stun();
			}
			EnterCombat(true, data.m_SourceLocation);
		}
		return flag;
	}

	public override void Die(DamageData data)
	{
		base.Die(data);
		Globals.m_AIDirector.BoxRobotDestroyed(this);
		EnableAiming(false, false);
		EndFiring();
		StopNavigation();
		if ((bool)m_Collider)
		{
			m_Collider.enabled = false;
		}
		m_StateTimer = 10f;
		if (Globals.m_PlayerController.m_TargetedEnemy == base.gameObject)
		{
			Globals.m_PlayerController.m_TargetedEnemy = null;
		}
		m_MeshRenderer.material = m_DeathMaterial;
		bool flag = false;
		if (m_TransitionalAnimation != null)
		{
			flag = true;
			Vector3 position = m_RigMotion.transform.position;
			Quaternion rotation = m_RigMotion.transform.rotation;
			m_Animator.Stop(m_TransitionalAnimation);
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
			base.transform.position = position;
			base.transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
			m_RigMotion.transform.localPosition = Vector3.zero;
			m_RigMotion.transform.localRotation = Quaternion.identity;
		}
		if (m_BoxRobotState == BoxRobotState.Hostile && m_BringingInGuns)
		{
			m_TransitionalAnimation = "CBT_Death";
			m_PosedAnimation = null;
			if (flag)
			{
				m_Animator.Play(m_TransitionalAnimation);
			}
			else
			{
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
		}
		else
		{
			m_TransitionalAnimation = "NCB_Death";
			m_PosedAnimation = null;
			m_Animator.Play(m_TransitionalAnimation);
			if (flag)
			{
				m_Animator.Play(m_TransitionalAnimation);
			}
			else
			{
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
		}
		m_DeathCollider = m_Core.gameObject.AddComponent<SphereCollider>();
		m_DeathCollider.isTrigger = true;
		m_DeathCollider.center = Vector3.zero;
		m_DeathCollider.radius = 0.5f;
		m_DeathCollider.enabled = true;
		m_BoxRobotState = BoxRobotState.Dying;
	}

	public void Destroy()
	{
		Globals.m_AIDirector.BoxRobotDestroyed(this);
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
		return base.transform.position + Vector3.up * 1.5f;
	}

	protected override void AttachShadowObject()
	{
		UnityEngine.Object.Destroy(m_ShadowObject);
	}
}
