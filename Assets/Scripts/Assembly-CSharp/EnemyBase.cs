using System;
using UnityEngine;

public class EnemyBase : CharacterBase
{
	public enum EnemyState
	{
		None = -1,
		GuardingPost = 0,
		StandingIdle = 1,
		WalkingPatrol = 2,
		PerformingActivity = 3,
		Transitioning = 4,
		MovingToLocation = 5,
		SearchingLocation = 6,
		InCover = 7,
		LeaningFromCover = 8,
		Dying = 9,
		Total = 10
	}

	public enum Hostility
	{
		None = -1,
		Relaxed = 0,
		Suspicious = 1,
		Alarmed = 2,
		Covering = 3,
		Hostile = 4,
		ThreatLost = 5,
		Agitated = 6,
		Warning = 7,
		Total = 8
	}

	public enum Urgency
	{
		None = -1,
		NotUrgent = 0,
		Urgent = 1,
		Total = 2
	}

	public enum PostTransitionEvent
	{
		None = -1,
		TurnTowardLocation = 0,
		TurnTowardNavigation = 1,
		TurnTowardPlayer = 2
	}

	public enum AimingType
	{
		None = -1,
		StandingFree = 0,
		StandingLeft = 1,
		StandingRight = 2,
		CrouchLeft = 3,
		CrouchLeftOver = 4,
		CrouchRight = 5,
		CrouchRightOver = 6,
		Total = 7
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

	public struct SourceInfo
	{
		public Vector3 m_ToSource;

		public float m_ToSourceDistanceSqr;

		public Vector3 m_ToSourceNormalizedOnXZ;

		public float m_ToSourceOnXZDistanceSqr;

		public float m_ToSourceForwardDot;

		public float m_ToSourceRightDot;

		public float m_ToSourceUpDot;

		public Direction m_ToSourceDirection;
	}

	[Serializable]
	public class AimingExtents
	{
		public float m_LeftExtent = 46f;

		public float m_RightExtent = 46f;

		public float m_UpExtent = 35f;

		public float m_DownExtent = 35f;
	}

	protected const float m_DeathFadeTime = 3.5f;

	protected const float m_PatrolSpeed = 1f;

	protected const float m_PatrolTurnSpeed = 5f;

	protected const float m_WalkSpeed = 0.9f;

	protected const float m_WalkTurnSpeed = 120f;

	protected const float m_RunSpeed = 4f;

	protected const float m_RunTurnSpeed = 300f;

	protected EnemyType m_EnemyType = EnemyType.None;

	[HideInInspector]
	public EnemySquad_NEW m_EnemySquad;

	[HideInInspector]
	public Ethnicity m_Ethnicity = Ethnicity.Guard_White_01;

	[HideInInspector]
	public bool m_Neutral;

	protected EnemyState m_CurrentState;

	protected EnemyState m_TargetState = EnemyState.None;

	protected EnemyState m_DesiredState = EnemyState.None;

	protected Hostility m_Hostility;

	protected Urgency m_Urgency;

	protected PostTransitionEvent m_PostTransitionEvent = PostTransitionEvent.None;

	public Animation m_Animator;

	public CapsuleCollider m_Collider;

	public NavMeshAgent m_NavAgent;

	public Transform m_RigMotion;

	public Transform m_Waist;

	public Transform m_Neck;

	public Material m_DeathMaterial;

	protected SphereCollider m_DeathCollider;

	protected bool m_SeePlayerThisFrame;

	protected float m_RelaxedVisualConeDiameterInDegrees = 130f;

	protected float m_HostileVisualConeDiameterInDegrees = 170f;

	protected float m_RelaxedVisualConeDot;

	protected float m_HostileVisualConeDot;

	protected float m_StateChangeTimer = -1f;

	protected float m_EventTimer = -1f;

	protected float m_VisualSensingTimer;

	protected float m_DisturbanceTimer;

	protected float m_StaggerTimer;

	protected float m_FiringTimer;

	[HideInInspector]
	public bool m_Paused;

	protected DisturbanceID m_CurrentDisturbanceID = DisturbanceID.None;

	protected SourceInfo m_SourceInfo = default(SourceInfo);

	protected NavMeshPath m_NavMeshPath = new NavMeshPath();

	protected Vector3 m_TargetMoveToPosition = Vector3.zero;

	protected Vector3 m_TargetAimToPosition = Vector3.zero;

	[HideInInspector]
	public PatrolNode m_TargetNode;

	protected bool m_PatrolForward = true;

	protected PatrolNode m_EventNode;

	protected Vector3 m_LastPatrolledPosition = Vector3.zero;

	public bool m_CanTakeCover = true;

	[HideInInspector]
	public NearbyCover m_ChosenCover;

	[HideInInspector]
	public SearchNode m_ChosenSearchNode;

	protected byte m_AimingFlags;

	protected float m_GlobalAimBlendingWeight;

	protected int m_CurrentAimingType = -1;

	protected int m_CancelledAimType = -1;

	protected float[] m_AimingWeights = new float[9];

	public AimingExtents[] m_AimingExtents = new AimingExtents[7];

	protected AnimationState[][] m_AimingAnimations = new AnimationState[7][];

	protected WeaponBase m_Weapon;

	public GameObject m_WeaponAttachRight;

	public GameObject m_WeaponAttachLeft;

	protected bool m_WaitingToReload;

	protected bool m_Firing;

	protected Vector3 m_FireDirection = Vector3.one;

	protected AnimationState m_ReloadingAnimation;

	protected AnimationState[] m_FiringAnimations = new AnimationState[7];

	protected float m_MinFiringDuration = 0.75f;

	protected float m_MaxFiringDuration = 1.5f;

	protected float m_MinFiringCooldown = 2f;

	protected float m_MaxFiringCooldown = 3.5f;

	protected float m_MaxAccuracyVariation = 8f;

	protected float m_AccuracyRefinementRate = 3f;

	protected float m_CurrentAccuracyVariation;

	protected string m_TransitionalAnimation;

	protected string m_PosedAnimation;

	protected string m_UninterruptableAnimation;

	protected bool m_CompoundTransitioning;

	protected GameObject m_CompoundTransitionAnimator;

	protected GameObject m_CompoundTransitionTracker;

	protected string m_CompoundTransitionAnimation;

	protected Vector3 m_CompoundTransitionPosition = Vector3.zero;

	protected Quaternion m_CompoundTransitionRotation = Quaternion.identity;

	protected int m_CurrentVOPriority = -1;

	protected uint m_CurrentVO;

	public bool IsAlive()
	{
		return m_CurrentState != EnemyState.Dying;
	}

	public override WeaponType GetEquippedWeaponType()
	{
		if (m_Weapon != null)
		{
			return m_Weapon.m_WeaponType;
		}
		return WeaponType.None;
	}

	public virtual void Awake()
	{
		m_RelaxedVisualConeDot = Mathf.Cos((float)Math.PI / 180f * (m_RelaxedVisualConeDiameterInDegrees * 0.5f));
		m_HostileVisualConeDot = Mathf.Cos((float)Math.PI / 180f * (m_HostileVisualConeDiameterInDegrees * 0.5f));
		PopulateAimingAnimations();
		PopulateFiringAnimations();
		m_CompoundTransitionAnimator = UnityEngine.Object.Instantiate(m_RigMotion.parent.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
		m_CompoundTransitionAnimator.name = base.gameObject.name + "_CompoundTracker";
		m_CompoundTransitionTracker = m_CompoundTransitionAnimator.transform.GetChild(0).gameObject;
		UnityEngine.Object.Destroy(m_CompoundTransitionTracker.transform.GetChild(0).gameObject);
		m_CompoundTransitionAnimator.SetActiveRecursively(false);
	}

	protected override void Start()
	{
		base.Start();
		Globals_NEW.m_AIDirector.EnemySpawned(this);
		m_CurrentState = EnemyState.GuardingPost;
		m_DesiredState = EnemyState.None;
		m_Hostility = Hostility.Relaxed;
		m_Urgency = Urgency.NotUrgent;
		m_StateChangeTimer = -1f;
		m_EventTimer = -1f;
		if (m_TargetNode != null)
		{
			PatrolNode patrolNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
			base.transform.position = m_TargetNode.transform.position;
			if (patrolNode != null)
			{
				m_CurrentState = EnemyState.WalkingPatrol;
				Vector3 vector = patrolNode.transform.position - m_TargetNode.transform.position;
				vector.y = 0f;
				base.transform.rotation = Quaternion.LookRotation(vector.normalized);
			}
			else
			{
				Vector3 forward = m_TargetNode.transform.forward;
				forward.y = 0f;
				base.transform.rotation = Quaternion.LookRotation(forward.normalized);
			}
		}
		RayClampToGround();
		m_LastPatrolledPosition = base.transform.position;
		if (m_CurrentState == EnemyState.GuardingPost)
		{
			m_Animator.Play("ENY_M_Stand_CBR_standingIdle");
		}
		else
		{
			m_Animator.Play("ENY_M_Stand_CBR_walk");
		}
	}

	protected virtual void PopulateAimingAnimations()
	{
		m_AimingAnimations[0] = new AnimationState[9];
		m_AimingAnimations[0][0] = m_Animator["ENY_M_Stand_CBR_AimCenter"];
		m_AimingAnimations[0][1] = m_Animator["ENY_M_Stand_CBR_AimLeft"];
		m_AimingAnimations[0][2] = m_Animator["ENY_M_Stand_CBR_AimRight"];
		m_AimingAnimations[0][3] = m_Animator["ENY_M_Stand_CBR_AimUP"];
		m_AimingAnimations[0][4] = m_Animator["ENY_M_Stand_CBR_AimDown"];
		m_AimingAnimations[0][5] = m_Animator["ENY_M_Stand_CBR_AimUP_LEFT"];
		m_AimingAnimations[0][6] = m_Animator["ENY_M_Stand_CBR_AimUP_Right"];
		m_AimingAnimations[0][7] = m_Animator["ENY_M_Stand_CBR_AimDownLEFT"];
		m_AimingAnimations[0][8] = m_Animator["ENY_M_Stand_CBR_AimDownRight"];
		m_AimingAnimations[1] = new AnimationState[9];
		m_AimingAnimations[1][0] = m_Animator["ENY_M_StandLeft_CBR_Center"];
		m_AimingAnimations[1][1] = m_Animator["ENY_M_StandLeft_CBR_AimLeft"];
		m_AimingAnimations[1][2] = m_Animator["ENY_M_StandLeft_CBR_AimRight"];
		m_AimingAnimations[1][3] = m_Animator["ENY_M_StandLeft_CBR_AimUp"];
		m_AimingAnimations[1][4] = m_Animator["ENY_M_StandLeft_CBR_AimDown"];
		m_AimingAnimations[1][5] = m_Animator["ENY_M_StandLeft_CBR_AimUp_Left"];
		m_AimingAnimations[1][6] = m_Animator["ENY_M_StandLeft_CBR_AimUp_Right"];
		m_AimingAnimations[1][7] = m_Animator["ENY_M_StandLeft_CBR_AimDown_Left"];
		m_AimingAnimations[1][8] = m_Animator["ENY_M_StandLeft_CBR_AimDown_Right"];
		m_AimingAnimations[2] = new AnimationState[9];
		m_AimingAnimations[2][0] = m_Animator["ENY_M_StandRight_CBR_AimCenter"];
		m_AimingAnimations[2][1] = m_Animator["ENY_M_StandRight_CBR_AimLeft"];
		m_AimingAnimations[2][2] = m_Animator["ENY_M_StandRight_CBR_AimRight"];
		m_AimingAnimations[2][3] = m_Animator["ENY_M_StandRight_CBR_AimUp"];
		m_AimingAnimations[2][4] = m_Animator["ENY_M_StandRight_CBR_AimDown"];
		m_AimingAnimations[2][5] = m_Animator["ENY_M_StandRight_CBR_AimUp_Left"];
		m_AimingAnimations[2][6] = m_Animator["ENY_M_StandRight_CBR_AimUp_Right"];
		m_AimingAnimations[2][7] = m_Animator["ENY_M_StandRight_CBR_AimDown_Left"];
		m_AimingAnimations[2][8] = m_Animator["ENY_M_StandRight_CBR_AimDown_Right"];
		m_AimingAnimations[3] = new AnimationState[9];
		m_AimingAnimations[3][0] = m_Animator["ENY_M_CrouchLeft_CBR_AimCenter"];
		m_AimingAnimations[3][1] = m_Animator["ENY_M_CrouchLeft_CBR_AimLeft"];
		m_AimingAnimations[3][2] = m_Animator["ENY_M_CrouchLeft_CBR_AimRight"];
		m_AimingAnimations[3][3] = m_Animator["ENY_M_CrouchLeft_CBR_AimUp"];
		m_AimingAnimations[3][4] = m_Animator["ENY_M_CrouchLeft_CBR_AimDown"];
		m_AimingAnimations[3][5] = m_Animator["ENY_M_CrouchLeft_CBR_AimUp_Left"];
		m_AimingAnimations[3][6] = m_Animator["ENY_M_CrouchLeft_CBR_AimUp_Right"];
		m_AimingAnimations[3][7] = m_Animator["ENY_M_CrouchLeft_CBR_AimDown_Left"];
		m_AimingAnimations[3][8] = m_Animator["ENY_M_CrouchLeft_CBR_AimDown_Right"];
		m_AimingAnimations[4] = new AnimationState[9];
		m_AimingAnimations[4][0] = m_Animator["ENY_M_CrouchLeftUpOver_CBR_AimCenter"];
		m_AimingAnimations[4][1] = m_Animator["ENY_M_CrouchLeftUpOver_CBR_AimLeft"];
		m_AimingAnimations[4][2] = m_Animator["ENY_M_CrouchLeftUpOver_CBR_AimRight"];
		m_AimingAnimations[4][3] = m_Animator["ENY_M_CrouchLeftUpOver_CBR_AimUp"];
		m_AimingAnimations[4][4] = m_Animator["ENY_M_CrouchLeftUpOver_CBR_AimDown"];
		m_AimingAnimations[4][5] = m_Animator["ENY_M_CrouchLeftUpOver_CBR_AimUp_Left"];
		m_AimingAnimations[4][6] = m_Animator["ENY_M_CrouchLeftUpOver_CBR_AimUp_Right"];
		m_AimingAnimations[4][7] = m_Animator["ENY_M_CrouchLeftUpOver_CBR_AimDown_Left"];
		m_AimingAnimations[4][8] = m_Animator["ENY_M_CrouchLeftUpOver_CBR_AimDown_Right"];
		m_AimingAnimations[5] = new AnimationState[9];
		m_AimingAnimations[5][0] = m_Animator["ENY_M_CrouchRight_CBR_AimCenter"];
		m_AimingAnimations[5][1] = m_Animator["ENY_M_CrouchRight_CBR_AimLeft"];
		m_AimingAnimations[5][2] = m_Animator["ENY_M_CrouchRight_CBR_AimRight"];
		m_AimingAnimations[5][3] = m_Animator["ENY_M_CrouchRight_CBR_AimUp"];
		m_AimingAnimations[5][4] = m_Animator["ENY_M_CrouchRight_CBR_AimDown"];
		m_AimingAnimations[5][5] = m_Animator["ENY_M_CrouchRight_CBR_AimUp_Left"];
		m_AimingAnimations[5][6] = m_Animator["ENY_M_CrouchRight_CBR_AimUp_Right"];
		m_AimingAnimations[5][7] = m_Animator["ENY_M_CrouchRight_CBR_AimDown_Left"];
		m_AimingAnimations[5][8] = m_Animator["ENY_M_CrouchRight_CBR_AimDown_Right"];
		m_AimingAnimations[6] = new AnimationState[9];
		m_AimingAnimations[6][0] = m_Animator["ENY_M_CrouchRightUpOver_CBR_AimCenter"];
		m_AimingAnimations[6][1] = m_Animator["ENY_M_CrouchRightUpOver_CBR_AimLeft"];
		m_AimingAnimations[6][2] = m_Animator["ENY_M_CrouchRightUpOver_CBR_AimRight"];
		m_AimingAnimations[6][3] = m_Animator["ENY_M_CrouchRightUpOver_CBR_AimUp"];
		m_AimingAnimations[6][4] = m_Animator["ENY_M_CrouchRightUpOver_CBR_AimDown"];
		m_AimingAnimations[6][5] = m_Animator["ENY_M_CrouchRightUpOver_CBR_AimUp_Left"];
		m_AimingAnimations[6][6] = m_Animator["ENY_M_CrouchRightUpOver_CBR_AimUp_Right"];
		m_AimingAnimations[6][7] = m_Animator["ENY_M_CrouchRightUpOver_CBR_AimDown_Left"];
		m_AimingAnimations[6][8] = m_Animator["ENY_M_CrouchRightUpOver_CBR_AimDown_Right"];
		for (int i = 0; i < 7; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				m_AimingAnimations[i][j].AddMixingTransform(m_Waist, true);
				m_AimingAnimations[i][j].blendMode = AnimationBlendMode.Blend;
				m_AimingAnimations[i][j].weight = 1f;
				m_AimingAnimations[i][j].layer = 10;
				m_AimingAnimations[i][j].enabled = false;
			}
		}
	}

	protected virtual void PopulateFiringAnimations()
	{
		m_FiringAnimations[0] = m_Animator["ENY_M_Stand_CBR_fire"];
		m_FiringAnimations[1] = m_Animator["ENY_M_StandLeft_CBR_Fire_Loop"];
		m_FiringAnimations[2] = m_Animator["ENY_M_StandRight_CBR_Fire_Loop"];
		m_FiringAnimations[3] = m_Animator["ENY_M_CrouchLeft_CBR_Fire_Loop"];
		m_FiringAnimations[4] = m_Animator["ENY_M_CrouchLeft_UpOver_CBR_Fire_Loop"];
		m_FiringAnimations[5] = m_Animator["ENY_M_CrouchRight_CBR_Fire_Loop"];
		m_FiringAnimations[6] = m_Animator["ENY_M_CrouchRight_UpOver_CBR_Fire_Loop"];
		for (int i = 0; i < 7; i++)
		{
			m_FiringAnimations[i].AddMixingTransform(m_Waist, true);
			m_FiringAnimations[i].blendMode = AnimationBlendMode.Additive;
			m_FiringAnimations[i].layer = 20;
		}
		m_ReloadingAnimation = m_Animator["ENY_M_Stand_CBR_reload_blend"];
		m_ReloadingAnimation.AddMixingTransform(m_Waist, true);
		m_ReloadingAnimation.blendMode = AnimationBlendMode.Additive;
		m_ReloadingAnimation.layer = 30;
	}

	protected virtual void GuardCurrentPosition()
	{
		m_CurrentState = EnemyState.GuardingPost;
		m_TargetState = EnemyState.None;
		m_DesiredState = EnemyState.None;
		m_Hostility = Hostility.Relaxed;
		m_Urgency = Urgency.NotUrgent;
		m_StateChangeTimer = -1f;
		m_EventTimer = UnityEngine.Random.Range(10f, 20f);
		m_Animator.CrossFade("ENY_M_Stand_CBR_standingIdle");
	}

	public virtual void EnterCombat(DisturbanceID sourceID, Vector3 sourceLocation)
	{
		if (m_CurrentState == EnemyState.Dying || m_Hostility == Hostility.Hostile)
		{
			return;
		}
		m_CurrentDisturbanceID = DisturbanceID.None;
		if (sourceID != DisturbanceID.Misc_ManuallySet)
		{
		}
		m_Urgency = Urgency.NotUrgent;
		if (sourceID == DisturbanceID.Misc_TakingDamage || sourceID == DisturbanceID.Visual_Grenade || sourceID == DisturbanceID.Audio_GrenadeLanding)
		{
			m_Urgency = Urgency.Urgent;
		}
		m_Hostility = Hostility.Hostile;
		m_StateChangeTimer = -1f;
		if (FindCover(sourceLocation, m_Urgency, null))
		{
			if (CalculateNavPath(m_ChosenCover.m_Cover.transform.position, 0.5f))
			{
				m_TargetState = EnemyState.MovingToLocation;
				m_DesiredState = EnemyState.InCover;
				m_PostTransitionEvent = PostTransitionEvent.None;
				if (m_Urgency != Urgency.Urgent && !IsInCover() && m_UninterruptableAnimation == null)
				{
					m_CurrentState = EnemyState.Transitioning;
					GatherSourceInfo(sourceLocation);
					PlayDisturbanceAnimation(m_SourceInfo.m_ToSourceDirection, DisturbanceEvent.MajorVisual);
				}
			}
			else
			{
				m_ChosenCover.m_Enemy = null;
				m_ChosenCover = null;
			}
		}
		if (m_ChosenCover == null)
		{
			bool flag = false;
			Vector3 vector = sourceLocation - base.transform.position;
			if ((sourceID == DisturbanceID.Audio_GrenadeLanding || sourceID == DisturbanceID.Visual_Grenade) && CalculateNavPath(base.transform.position - vector.normalized * 7.5f, 3f))
			{
				flag = true;
				m_TargetState = EnemyState.MovingToLocation;
				m_DesiredState = EnemyState.StandingIdle;
				m_PostTransitionEvent = PostTransitionEvent.None;
			}
			if (!flag)
			{
				if (m_Weapon.ThreatWithinPCR(vector.sqrMagnitude))
				{
					m_TargetState = EnemyState.StandingIdle;
					m_DesiredState = EnemyState.StandingIdle;
					m_PostTransitionEvent = PostTransitionEvent.None;
					if (m_Urgency != Urgency.Urgent && !IsInCover() && m_UninterruptableAnimation == null)
					{
						m_CurrentState = EnemyState.Transitioning;
						GatherSourceInfo(sourceLocation);
						PlayDisturbanceAnimation(m_SourceInfo.m_ToSourceDirection, DisturbanceEvent.MajorVisual);
					}
					if (m_CurrentState == EnemyState.Transitioning)
					{
						m_DesiredState = EnemyState.None;
					}
				}
				else if (CalculateNavPath(sourceLocation - vector.normalized * m_Weapon.AveragePCR(), m_Weapon.HalfPCR()) || CalculateNavPath(sourceLocation, m_Weapon.AveragePCR()))
				{
					m_TargetState = EnemyState.MovingToLocation;
					m_DesiredState = EnemyState.StandingIdle;
					m_PostTransitionEvent = PostTransitionEvent.None;
					if (m_Urgency != Urgency.Urgent && !IsInCover() && m_UninterruptableAnimation == null)
					{
						m_CurrentState = EnemyState.Transitioning;
						GatherSourceInfo(sourceLocation);
						PlayDisturbanceAnimation(m_SourceInfo.m_ToSourceDirection, DisturbanceEvent.MajorVisual);
					}
				}
				else
				{
					m_TargetState = EnemyState.StandingIdle;
					m_DesiredState = EnemyState.StandingIdle;
					m_PostTransitionEvent = PostTransitionEvent.TurnTowardPlayer;
					if (m_Urgency != Urgency.Urgent && !IsInCover() && m_UninterruptableAnimation == null)
					{
						m_CurrentState = EnemyState.Transitioning;
						GatherSourceInfo(sourceLocation);
						PlayDisturbanceAnimation(m_SourceInfo.m_ToSourceDirection, DisturbanceEvent.MajorVisual);
					}
					if (m_CurrentState == EnemyState.Transitioning)
					{
						m_DesiredState = EnemyState.None;
					}
				}
			}
		}
		if (m_UninterruptableAnimation == null && !IsInCover())
		{
			EnableAiming(AimingType.StandingFree);
		}
	}

	public virtual void InvestigateDisturbance(Vector3 sourceLocation, DisturbanceEvent sourceEvent, DisturbanceID sourceID)
	{
		if (m_CurrentState == EnemyState.Dying || m_Hostility == Hostility.Hostile)
		{
			return;
		}
		if (sourceEvent == DisturbanceEvent.MajorAudio || sourceEvent == DisturbanceEvent.MajorVisual)
		{
			if (m_Hostility == Hostility.Relaxed || m_Hostility == Hostility.Warning || m_Hostility == Hostility.Suspicious || m_Hostility == Hostility.Covering)
			{
				m_Hostility = Hostility.Alarmed;
			}
		}
		else
		{
			m_Hostility = ((m_Hostility == Hostility.Relaxed || m_Hostility == Hostility.Warning) ? Hostility.Suspicious : m_Hostility);
			if (m_Hostility != Hostility.Suspicious)
			{
				sourceEvent = ((sourceEvent == DisturbanceEvent.MinorAudio) ? DisturbanceEvent.MajorAudio : DisturbanceEvent.MajorVisual);
			}
		}
		if (IsInCover())
		{
			if (CalculateNavPath(sourceLocation, 5f))
			{
				m_DesiredState = EnemyState.SearchingLocation;
				m_StateChangeTimer = -1f;
				m_Urgency = Urgency.NotUrgent;
				GatherSourceInfo(m_TargetMoveToPosition);
				m_CurrentDisturbanceID = sourceID;
				m_DisturbanceTimer = 4f;
			}
			else
			{
				Debug.LogWarning("Enemy couldn't find a path to investigate the disturbance!");
			}
		}
		else if (m_DisturbanceTimer <= 0f)
		{
			if (CalculateNavPath(sourceLocation, 5f))
			{
				m_CurrentState = EnemyState.Transitioning;
				m_TargetState = EnemyState.MovingToLocation;
				m_DesiredState = EnemyState.SearchingLocation;
				m_PostTransitionEvent = PostTransitionEvent.TurnTowardNavigation;
				m_StateChangeTimer = -1f;
				GatherSourceInfo(sourceLocation);
				m_Urgency = ((m_SourceInfo.m_ToSourceOnXZDistanceSqr <= 2.5f) ? Urgency.Urgent : Urgency.NotUrgent);
				PlayDisturbanceAnimation(m_SourceInfo.m_ToSourceDirection, sourceEvent);
				m_CurrentDisturbanceID = sourceID;
				m_DisturbanceTimer = 4f;
			}
			else
			{
				Debug.LogWarning("Enemy couldn't find a path to investigate the disturbance!");
			}
		}
		m_TargetAimToPosition = sourceLocation + 0.5f * Vector3.up;
	}

	protected void CheckDesire()
	{
		if (m_DesiredState == m_CurrentState)
		{
			m_DesiredState = EnemyState.None;
		}
		if (m_CurrentState == EnemyState.Dying)
		{
			m_DesiredState = EnemyState.None;
		}
		if (m_DesiredState != EnemyState.None && m_CurrentState != EnemyState.Transitioning && ClearToChangeStates())
		{
			switch (m_CurrentState)
			{
			case EnemyState.GuardingPost:
				TransitionFrom_GuardingPost();
				break;
			case EnemyState.StandingIdle:
				TransitionFrom_StandingIdle();
				break;
			case EnemyState.WalkingPatrol:
				TransitionFrom_WalkingPatrol();
				break;
			case EnemyState.PerformingActivity:
				TransitionFrom_PerformingActivity();
				break;
			case EnemyState.MovingToLocation:
				TransitionFrom_MovingToLocation();
				break;
			case EnemyState.SearchingLocation:
				TransitionFrom_SearchingLocation();
				break;
			case EnemyState.InCover:
				TransitionFrom_InCover();
				break;
			case EnemyState.LeaningFromCover:
				TransitionFrom_LeaningFromCover();
				break;
			case EnemyState.Transitioning:
				break;
			}
		}
	}

	protected void TransitionFrom_GuardingPost()
	{
		if (m_DesiredState == EnemyState.StandingIdle)
		{
			m_CurrentState = EnemyState.StandingIdle;
			m_DesiredState = EnemyState.None;
			m_StateChangeTimer = -1f;
			m_EventTimer = -1f;
			if (m_Hostility == Hostility.Covering || m_Hostility == Hostility.Hostile)
			{
				m_Animator.CrossFade("ENY_M_Stand_CBR_AlertIdle");
				EnableAiming(AimingType.StandingFree);
			}
			else
			{
				m_Animator.CrossFade("ENY_M_Stand_CBR_standingIdle");
			}
		}
		else if (m_DesiredState == EnemyState.WalkingPatrol || m_DesiredState == EnemyState.PerformingActivity)
		{
			if (m_TargetNode == null)
			{
				m_DesiredState = EnemyState.None;
				return;
			}
			m_CurrentState = EnemyState.Transitioning;
			m_TargetState = EnemyState.WalkingPatrol;
			m_DesiredState = EnemyState.None;
			GatherSourceInfo(m_TargetNode.transform.position);
			PlayTurnAnimation(m_SourceInfo.m_ToSourceDirection);
		}
		else if (m_DesiredState == EnemyState.MovingToLocation || m_DesiredState == EnemyState.SearchingLocation || m_DesiredState == EnemyState.InCover || m_DesiredState == EnemyState.LeaningFromCover)
		{
			if (CalculateNavPath(m_TargetMoveToPosition, 5f))
			{
				m_CurrentState = EnemyState.Transitioning;
				m_TargetState = EnemyState.MovingToLocation;
				m_DesiredState = ((m_DesiredState != EnemyState.MovingToLocation) ? m_DesiredState : EnemyState.None);
				m_PostTransitionEvent = PostTransitionEvent.None;
				if (m_NavMeshPath.corners != null && m_NavMeshPath.corners.Length > 1)
				{
					GatherSourceInfo(m_NavMeshPath.corners[1]);
				}
				else
				{
					GatherSourceInfo(m_TargetMoveToPosition);
				}
				PlayTurnAnimation(m_SourceInfo.m_ToSourceDirection);
			}
			else
			{
				m_DesiredState = EnemyState.None;
			}
		}
		else
		{
			m_DesiredState = EnemyState.None;
		}
	}

	protected void TransitionFrom_StandingIdle()
	{
	}

	protected void TransitionFrom_WalkingPatrol()
	{
		if (m_DesiredState == EnemyState.GuardingPost)
		{
			return;
		}
		if (m_DesiredState == EnemyState.StandingIdle)
		{
			m_CurrentState = EnemyState.StandingIdle;
			m_DesiredState = EnemyState.None;
			StopNavigation();
			if (m_Hostility == Hostility.Covering || m_Hostility == Hostility.Hostile)
			{
				m_Animator.CrossFade("ENY_M_Stand_CBR_AlertIdle");
				m_StateChangeTimer = -1f;
				m_EventTimer = -1f;
				EnableAiming(AimingType.StandingFree);
			}
			else
			{
				m_Animator.CrossFade("ENY_M_Stand_CBR_standingIdle");
				m_StateChangeTimer = -1f;
				m_EventTimer = -1f;
			}
		}
		else if (m_DesiredState != EnemyState.PerformingActivity && m_DesiredState != EnemyState.MovingToLocation && m_DesiredState != EnemyState.SearchingLocation && m_DesiredState != EnemyState.InCover && m_DesiredState != EnemyState.LeaningFromCover)
		{
			m_DesiredState = EnemyState.None;
		}
	}

	protected void TransitionFrom_PerformingActivity()
	{
		if (m_DesiredState == EnemyState.WalkingPatrol)
		{
			if (m_TargetNode != null)
			{
				m_CurrentState = EnemyState.Transitioning;
				m_TargetState = EnemyState.WalkingPatrol;
				m_DesiredState = EnemyState.None;
				GatherSourceInfo(m_TargetNode.transform.position);
				PlayTurnAnimation(m_SourceInfo.m_ToSourceDirection);
			}
			else
			{
				GuardCurrentPosition();
			}
		}
	}

	protected void TransitionFrom_MovingToLocation()
	{
		if (m_DesiredState == EnemyState.SearchingLocation)
		{
			m_CurrentState = EnemyState.SearchingLocation;
			m_DesiredState = EnemyState.None;
			StopNavigation();
			if (m_Hostility == Hostility.Relaxed || m_Hostility == Hostility.Warning || m_Hostility == Hostility.Suspicious)
			{
				m_StateChangeTimer = UnityEngine.Random.Range(7.5f, 15f);
				m_EventTimer = 1f;
				m_Animator.CrossFade("ENY_M_Stand_CBR_InvestigateIdle");
			}
			else
			{
				m_StateChangeTimer = UnityEngine.Random.Range(7.5f, 15f);
				m_EventTimer = 1f;
				m_Animator.CrossFade("ENY_M_Stand_CBR_AlertIdle");
			}
		}
		else if (m_DesiredState == EnemyState.WalkingPatrol)
		{
			StopNavigation();
			if (m_TargetNode != null)
			{
				m_CurrentState = EnemyState.Transitioning;
				m_TargetState = EnemyState.WalkingPatrol;
				m_DesiredState = EnemyState.None;
				GatherSourceInfo(m_TargetNode.transform.position);
				PlayTurnAnimation(m_SourceInfo.m_ToSourceDirection);
			}
			else
			{
				GuardCurrentPosition();
			}
		}
	}

	protected void TransitionFrom_SearchingLocation()
	{
		if (m_DesiredState != EnemyState.WalkingPatrol)
		{
			return;
		}
		if (CalculateNavPath(m_LastPatrolledPosition, 2f))
		{
			m_CurrentState = EnemyState.Transitioning;
			m_TargetState = EnemyState.MovingToLocation;
			m_PostTransitionEvent = PostTransitionEvent.None;
			if (m_NavMeshPath.corners != null && m_NavMeshPath.corners.Length > 1)
			{
				GatherSourceInfo(m_NavMeshPath.corners[1]);
			}
			else
			{
				GatherSourceInfo(m_TargetMoveToPosition);
			}
			PlayTurnAnimation(m_SourceInfo.m_ToSourceDirection);
		}
		else if (m_TargetNode != null && CalculateNavPath(m_TargetNode.transform.position, 1f))
		{
			m_CurrentState = EnemyState.Transitioning;
			m_TargetState = EnemyState.MovingToLocation;
			m_PostTransitionEvent = PostTransitionEvent.None;
			if (m_NavMeshPath.corners != null && m_NavMeshPath.corners.Length > 1)
			{
				GatherSourceInfo(m_NavMeshPath.corners[1]);
			}
			else
			{
				GatherSourceInfo(m_TargetMoveToPosition);
			}
			PlayTurnAnimation(m_SourceInfo.m_ToSourceDirection);
		}
		else
		{
			GuardCurrentPosition();
		}
	}

	protected void TransitionFrom_InCover()
	{
	}

	protected void TransitionFrom_LeaningFromCover()
	{
	}

	protected void TransitionFrom_Transitioning()
	{
		m_CurrentState = m_TargetState;
		switch (m_CurrentState)
		{
		case EnemyState.WalkingPatrol:
			m_Animator.CrossFade("ENY_M_Stand_CBR_walk");
			break;
		case EnemyState.PerformingActivity:
			PlayTransition(m_EventNode.m_WaypointAnimation, m_EventNode.m_AppendPoseAnimation);
			if (m_EventNode.m_Dialog != null && m_EventNode.m_Dialog.Length > 0)
			{
				PlayVO(m_EventNode.m_Dialog, 10);
			}
			if (m_TargetNode != null)
			{
				m_CurrentState = EnemyState.Transitioning;
				m_TargetState = EnemyState.WalkingPatrol;
				m_PostTransitionEvent = PostTransitionEvent.TurnTowardLocation;
				m_TargetMoveToPosition = m_TargetNode.transform.position;
			}
			else
			{
				m_CurrentState = EnemyState.Transitioning;
				m_TargetState = EnemyState.GuardingPost;
				m_PostTransitionEvent = PostTransitionEvent.TurnTowardLocation;
				m_TargetMoveToPosition = m_RigMotion.position - m_RigMotion.forward;
			}
			break;
		case EnemyState.GuardingPost:
			m_EventTimer = UnityEngine.Random.Range(10f, 20f);
			m_Animator.CrossFade("ENY_M_Stand_CBR_standingIdle");
			break;
		case EnemyState.MovingToLocation:
			ResumeNavigation();
			if (m_Hostility == Hostility.Hostile)
			{
				m_Animator.CrossFade("ENY_M_Stand_CBR_runFWD_Aim_CombatReady");
				EnableAiming(AimingType.StandingFree);
			}
			else if (m_Hostility == Hostility.Covering)
			{
				m_Animator.CrossFade("ENY_M_Stand_CBR_runFWD_Aim_CombatReady");
				EnableAiming(AimingType.StandingFree);
			}
			else if (m_Hostility == Hostility.Relaxed || m_Hostility == Hostility.Warning || m_Hostility == Hostility.Suspicious)
			{
				m_Animator.CrossFade("ENY_M_Stand_CBR_walk");
			}
			else
			{
				m_Animator.CrossFade("ENY_M_Stand_CBR_walkFWD_CombatReady");
			}
			break;
		case EnemyState.StandingIdle:
			if (m_Hostility == Hostility.Covering || m_Hostility == Hostility.Hostile)
			{
				m_Animator.CrossFade("ENY_M_Stand_CBR_AlertIdle");
				m_StateChangeTimer = -1f;
				m_EventTimer = -1f;
				EnableAiming(AimingType.StandingFree);
			}
			else
			{
				m_Animator.CrossFade("ENY_M_Stand_CBR_standingIdle");
				m_StateChangeTimer = -1f;
				m_EventTimer = -1f;
			}
			break;
		case EnemyState.Transitioning:
			break;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (m_Paused)
		{
			return;
		}
		if (m_UninterruptableAnimation != null && !m_Animator.IsPlaying(m_UninterruptableAnimation))
		{
			m_UninterruptableAnimation = null;
			if (m_Hostility == Hostility.Hostile && !IsInCover())
			{
				EnableAiming(AimingType.StandingFree);
			}
		}
		m_StateChangeTimer -= Time.deltaTime;
		m_DisturbanceTimer -= Time.deltaTime;
		m_StaggerTimer -= Time.deltaTime;
		CheckVisualCone();
		switch (m_CurrentState)
		{
		case EnemyState.GuardingPost:
			Update_GuardingPost();
			break;
		case EnemyState.WalkingPatrol:
			Update_WalkingPatrol();
			break;
		case EnemyState.PerformingActivity:
			Update_PerformingActivity();
			break;
		case EnemyState.StandingIdle:
			Update_StandingIdle();
			break;
		case EnemyState.Dying:
			Update_Dying();
			break;
		case EnemyState.Transitioning:
			Update_Transitioning();
			break;
		case EnemyState.MovingToLocation:
			Update_MovingToLocation();
			break;
		case EnemyState.SearchingLocation:
			Update_SearchingLocation();
			break;
		}
		CheckDesire();
		RayClampToGround();
		UpdateAimBlending();
		CalculateFireDirection();
		UpdateFiring();
	}

	protected virtual void LateUpdate()
	{
		if (m_CompoundTransitioning)
		{
			m_RigMotion.position = m_CompoundTransitionPosition + m_CompoundTransitionTracker.transform.localPosition;
			m_RigMotion.rotation = m_CompoundTransitionRotation * m_CompoundTransitionTracker.transform.localRotation;
			if (!m_Animator.IsPlaying(m_CompoundTransitionAnimation))
			{
				base.transform.position = m_CompoundTransitionPosition;
				base.transform.rotation = m_CompoundTransitionRotation;
				m_RigMotion.localPosition = m_CompoundTransitionTracker.transform.localPosition;
				m_RigMotion.localRotation = m_CompoundTransitionTracker.transform.localRotation;
				m_CompoundTransitionAnimator.SetActiveRecursively(false);
				m_CompoundTransitioning = false;
			}
		}
	}

	protected virtual void Update_GuardingPost()
	{
		if (m_TransitionalAnimation == null)
		{
			m_EventTimer -= Time.deltaTime;
			if (!(m_EventTimer <= 0f))
			{
			}
		}
		else if (!m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			m_Animator.CrossFade("ENY_M_Stand_CBR_standingIdle");
			m_EventTimer = UnityEngine.Random.Range(10f, 20f);
		}
	}

	protected virtual void Update_WalkingPatrol()
	{
		Vector3 vector = m_TargetNode.transform.position - base.transform.position;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		float num = 1f * Time.deltaTime;
		if (magnitude <= num)
		{
			base.transform.position = m_TargetNode.transform.position;
			m_LastPatrolledPosition = base.transform.position;
			num -= magnitude;
			if (PatrolEventTriggered())
			{
				m_CurrentState = EnemyState.StandingIdle;
				m_DesiredState = EnemyState.PerformingActivity;
				m_StateChangeTimer = UnityEngine.Random.Range(m_EventNode.m_MinIdle, m_EventNode.m_MaxIdle);
				m_EventTimer = -1f;
				if (m_EventNode.m_PatrolEvent == PatrolNode.PatrolEvent.TurnBack)
				{
					m_PatrolForward = !m_PatrolForward;
				}
				m_TargetNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
				m_Animator.CrossFade("ENY_M_Stand_CBR_standingIdle");
				return;
			}
			m_TargetNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
			if (!(m_TargetNode != null))
			{
				GuardCurrentPosition();
				return;
			}
			vector = m_TargetNode.transform.position - base.transform.position;
			vector.y = 0f;
		}
		float maxDegreesDelta = ((!(magnitude <= 1f)) ? 5f : 10f);
		Quaternion to = Quaternion.LookRotation(vector.normalized);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, maxDegreesDelta);
		m_NavAgent.Move(base.transform.forward * num);
		m_LastPatrolledPosition = base.transform.position;
	}

	protected virtual void Update_PerformingActivity()
	{
	}

	protected virtual void Update_StandingIdle()
	{
		if (m_Hostility == Hostility.Hostile && (m_AimingFlags & 0x33) != 0)
		{
			m_CurrentState = EnemyState.Transitioning;
			m_TargetState = EnemyState.StandingIdle;
			m_PostTransitionEvent = PostTransitionEvent.None;
			if ((m_AimingFlags & 0x10) != 0)
			{
				PlayTurnAnimation(Direction.BackLeft);
			}
			else if ((m_AimingFlags & 0x20) != 0)
			{
				PlayTurnAnimation(Direction.BackRight);
			}
			else if ((m_AimingFlags & 1) != 0)
			{
				PlayTurnAnimation(Direction.Left);
			}
			else
			{
				PlayTurnAnimation(Direction.Right);
			}
		}
	}

	protected virtual void Update_Dying()
	{
	}

	protected virtual void Update_MovingToLocation()
	{
	}

	protected virtual void Update_SearchingLocation()
	{
		if (m_StateChangeTimer <= 0f && (m_Hostility == Hostility.Relaxed || m_Hostility == Hostility.Warning || m_Hostility == Hostility.Suspicious))
		{
			m_DesiredState = EnemyState.WalkingPatrol;
			m_Hostility = Hostility.Relaxed;
			m_Urgency = Urgency.NotUrgent;
		}
		if (m_TransitionalAnimation == null)
		{
			m_EventTimer -= Time.deltaTime;
			if (m_EventTimer <= 0f && m_DesiredState == EnemyState.None)
			{
				if (m_Hostility == Hostility.Relaxed || m_Hostility == Hostility.Warning || m_Hostility == Hostility.Suspicious)
				{
					PlayTransition("ENY_M_Stand_CBR_INV_SearchAround0" + (UnityEngine.Random.Range(0, 6) + 1), false);
				}
				else
				{
					PlayTransition("ENY_M_Stand_CBR_Investigate_Nothing", false);
				}
			}
		}
		else if (!m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_Hostility == Hostility.Relaxed || m_Hostility == Hostility.Warning || m_Hostility == Hostility.Suspicious)
			{
				m_Animator.CrossFade("ENY_M_Stand_CBR_InvestigateIdle");
			}
			else
			{
				m_Animator.CrossFade("ENY_M_Stand_CBR_AlertIdle");
			}
			if (m_StateChangeTimer <= 1f)
			{
				m_StateChangeTimer = 1f;
			}
			m_EventTimer = 10000f;
		}
	}

	protected virtual void Update_Transitioning()
	{
		if (m_TransitionalAnimation != null && m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			return;
		}
		RecalibrateBase();
		if (m_PostTransitionEvent != PostTransitionEvent.None)
		{
			if (m_PostTransitionEvent == PostTransitionEvent.TurnTowardLocation)
			{
				GatherSourceInfo(m_TargetMoveToPosition);
				PlayTurnAnimation(m_SourceInfo.m_ToSourceDirection);
			}
			else if (m_PostTransitionEvent == PostTransitionEvent.TurnTowardNavigation)
			{
				if (m_NavMeshPath.corners != null && m_NavMeshPath.corners.Length > 1)
				{
					GatherSourceInfo(m_NavMeshPath.corners[1]);
				}
				else
				{
					GatherSourceInfo(m_TargetMoveToPosition);
				}
				PlayTurnAnimation(m_SourceInfo.m_ToSourceDirection);
			}
			else if (m_PostTransitionEvent == PostTransitionEvent.TurnTowardPlayer)
			{
				GatherSourceInfo(Globals_NEW.m_AIDirector.m_LastKnownPlayerPosition);
				PlayTurnAnimation(m_SourceInfo.m_ToSourceDirection);
			}
			m_PostTransitionEvent = PostTransitionEvent.None;
		}
		else
		{
			TransitionFrom_Transitioning();
		}
	}

	protected virtual void UpdateAimBlending()
	{
		if (m_CurrentAimingType >= 0)
		{
			m_GlobalAimBlendingWeight = Mathf.Clamp01(m_GlobalAimBlendingWeight + 1f * Time.deltaTime);
		}
		else if (m_CancelledAimType >= 0)
		{
			m_GlobalAimBlendingWeight -= 1f * Time.deltaTime;
			if (m_GlobalAimBlendingWeight <= 0f)
			{
				for (int i = 0; i < 9; i++)
				{
					m_AimingAnimations[m_CancelledAimType][i].enabled = false;
				}
				m_CancelledAimType = -1;
			}
		}
		if (m_CurrentAimingType < 0)
		{
			return;
		}
		if (m_Hostility == Hostility.Hostile)
		{
			m_TargetAimToPosition = Globals_NEW.m_AIDirector.m_LastKnownPlayerPosition;
			m_TargetAimToPosition.y -= Globals.m_PlayerYOffset;
			if (Globals.m_PlayerController.m_Stance == PlayerController.Stance.Crouch && m_SeePlayerThisFrame)
			{
				m_TargetAimToPosition.y -= 0.5f;
			}
			Vector3 vector = new Vector3(m_TargetAimToPosition.z - m_RigMotion.position.z, 0f, 0f - m_TargetAimToPosition.x + m_RigMotion.position.x);
			m_TargetAimToPosition -= vector.normalized * 0.2f;
		}
		GatherSourceInfo(m_TargetAimToPosition);
		float num = 57.29578f * Mathf.Acos(m_SourceInfo.m_ToSourceForwardDot);
		float num2 = 90f - 57.29578f * Mathf.Acos(m_SourceInfo.m_ToSourceUpDot);
		float num3 = Mathf.Clamp01(Mathf.Abs(num / ((!(m_SourceInfo.m_ToSourceRightDot >= 0f)) ? m_AimingExtents[m_CurrentAimingType].m_LeftExtent : m_AimingExtents[m_CurrentAimingType].m_RightExtent)));
		float num4 = Mathf.Clamp(num2 / ((!(m_SourceInfo.m_ToSourceUpDot >= 0f)) ? m_AimingExtents[m_CurrentAimingType].m_DownExtent : m_AimingExtents[m_CurrentAimingType].m_UpExtent), -1f, 1f);
		m_AimingFlags = 0;
		if (m_SourceInfo.m_ToSourceDirection == Direction.BackLeft)
		{
			m_AimingFlags |= 16;
		}
		else if (m_SourceInfo.m_ToSourceDirection == Direction.BackRight)
		{
			m_AimingFlags |= 32;
		}
		if (num3 >= 1f)
		{
			m_AimingFlags |= (byte)((!(m_SourceInfo.m_ToSourceRightDot >= 0f)) ? 1 : 2);
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
		for (int j = 0; j < 9; j++)
		{
			array[j] = 0f;
		}
		if (m_SourceInfo.m_ToSourceRightDot >= 0f)
		{
			if (num4 >= 0f)
			{
				array[0] = BiLerp(1f, 0f, 0f, 0f, num3, num4);
				array[2] = BiLerp(0f, 1f, 0f, 0f, num3, num4);
				array[3] = BiLerp(0f, 0f, 1f, 0f, num3, num4);
				array[6] = BiLerp(0f, 0f, 0f, 1f, num3, num4);
			}
			else
			{
				array[4] = BiLerp(1f, 0f, 0f, 0f, num3, 1f + num4);
				array[8] = BiLerp(0f, 1f, 0f, 0f, num3, 1f + num4);
				array[0] = BiLerp(0f, 0f, 1f, 0f, num3, 1f + num4);
				array[2] = BiLerp(0f, 0f, 0f, 1f, num3, 1f + num4);
			}
		}
		else if (num4 >= 0f)
		{
			array[1] = BiLerp(1f, 0f, 0f, 0f, 1f - num3, num4);
			array[0] = BiLerp(0f, 1f, 0f, 0f, 1f - num3, num4);
			array[5] = BiLerp(0f, 0f, 1f, 0f, 1f - num3, num4);
			array[3] = BiLerp(0f, 0f, 0f, 1f, 1f - num3, num4);
		}
		else
		{
			array[7] = BiLerp(1f, 0f, 0f, 0f, 1f - num3, 1f + num4);
			array[4] = BiLerp(0f, 1f, 0f, 0f, 1f - num3, 1f + num4);
			array[1] = BiLerp(0f, 0f, 1f, 0f, 1f - num3, 1f + num4);
			array[0] = BiLerp(0f, 0f, 0f, 1f, 1f - num3, 1f + num4);
		}
		for (int k = 0; k < 9; k++)
		{
			if (m_AimingWeights[k] < array[k])
			{
				m_AimingWeights[k] = Mathf.Min(m_AimingWeights[k] + 5f * Time.deltaTime, array[k]);
			}
			else if (m_AimingWeights[k] > array[k])
			{
				m_AimingWeights[k] = Mathf.Max(m_AimingWeights[k] - 5f * Time.deltaTime, array[k]);
			}
			m_AimingAnimations[m_CurrentAimingType][k].weight = m_GlobalAimBlendingWeight * m_AimingWeights[k];
		}
	}

	protected virtual void UpdateFiring()
	{
		if (m_Weapon == null)
		{
			return;
		}
		if (m_WaitingToReload)
		{
			if (m_Firing)
			{
				StopFiring();
			}
			if (m_UninterruptableAnimation == null)
			{
				m_Animator.Play(m_ReloadingAnimation.name);
				m_Weapon.Reload(m_ReloadingAnimation.length);
				m_WaitingToReload = false;
			}
			return;
		}
		m_FiringTimer -= Time.deltaTime;
		if (m_Weapon.m_WeaponState == WeaponBase.WeaponState.Reloading)
		{
			return;
		}
		if (m_Firing)
		{
			float num = Mathf.Clamp01(Mathf.InverseLerp(100f, 8f, (Globals.m_PlayerController.transform.position - m_RigMotion.transform.position).sqrMagnitude));
			m_FiringTimer += num * 2f * Time.deltaTime;
			if (!CanFireWeapon() || m_FiringTimer <= 0f || SquadmateInLoS())
			{
				StopFiring();
			}
		}
		else if (CanFireWeapon() && m_FiringTimer <= 0f && !SquadmateInLoS() && m_SeePlayerThisFrame)
		{
			StartFiring();
		}
	}

	private bool PatrolEventTriggered()
	{
		if (m_TargetNode.m_PatrolEvent == PatrolNode.PatrolEvent.None || m_TargetNode.m_EventChance == PatrolNode.EventChance.Never)
		{
			return false;
		}
		float num = -1f;
		switch (m_TargetNode.m_EventChance)
		{
		case PatrolNode.EventChance.Always:
			num = 1f;
			break;
		case PatrolNode.EventChance.Often:
			num = 0.75f;
			break;
		case PatrolNode.EventChance.Sometimes:
			num = 0.5f;
			break;
		case PatrolNode.EventChance.Rarely:
			num = 0.25f;
			break;
		case PatrolNode.EventChance.Once:
			num = 1f;
			m_TargetNode.m_EventChance = PatrolNode.EventChance.Never;
			break;
		}
		if (UnityEngine.Random.value <= num)
		{
			if (m_TargetNode.m_PatrolEvent == PatrolNode.PatrolEvent.Dialog)
			{
				if (m_TargetNode.m_Dialog != null && m_TargetNode.m_Dialog.Length > 0)
				{
					PlayVO(m_TargetNode.m_Dialog, 10);
				}
				return false;
			}
			if (m_TargetNode.m_PatrolEvent == PatrolNode.PatrolEvent.CycleBreaker)
			{
				switch (UnityEngine.Random.Range(0, 4))
				{
				case 0:
					m_TransitionalAnimation = "ENY_M_Stand_CBR_walkFWD_CycleBreaker_SouthEast1";
					break;
				case 1:
					m_TransitionalAnimation = "ENY_M_Stand_CBR_walkFWD_CycleBreaker_SouthWest1";
					break;
				case 2:
					m_TransitionalAnimation = "ENY_M_Stand_CBR_walkFWD_CycleBreaker_West1";
					break;
				default:
					m_TransitionalAnimation = "ENY_M_Stand_CBR_walkFWD_CycleBreakerEast1";
					break;
				}
				m_Animator.CrossFade(m_TransitionalAnimation);
				m_Animator.CrossFadeQueued("ENY_M_Stand_CBR_walk");
				return false;
			}
			m_EventNode = m_TargetNode;
			return true;
		}
		return false;
	}

	public override bool IsInCover()
	{
		return m_CurrentState == EnemyState.InCover || m_CurrentState == EnemyState.LeaningFromCover || (m_CurrentState == EnemyState.Transitioning && m_TargetState == EnemyState.InCover);
	}

	protected virtual bool CanFireWeapon()
	{
		if (m_CurrentState == EnemyState.Dying)
		{
			return false;
		}
		if (m_Hostility != Hostility.Hostile)
		{
			return false;
		}
		if (m_CurrentAimingType == -1)
		{
			return false;
		}
		if (m_UninterruptableAnimation != null)
		{
			return false;
		}
		if ((m_CurrentState == EnemyState.Transitioning && m_TargetState != EnemyState.SearchingLocation && m_TargetState != EnemyState.InCover) || m_CurrentState == EnemyState.InCover || m_CurrentState == EnemyState.SearchingLocation || m_CurrentState == EnemyState.PerformingActivity)
		{
			return false;
		}
		return true;
	}

	protected virtual bool ClearToChangeStates()
	{
		if (m_TransitionalAnimation != null)
		{
			return false;
		}
		if (m_Urgency == Urgency.NotUrgent && m_StateChangeTimer > 0f)
		{
			return false;
		}
		EnemyState currentState = m_CurrentState;
		if (currentState == EnemyState.MovingToLocation && (m_NavAgent.remainingDistance == float.PositiveInfinity || m_NavAgent.remainingDistance > 0.1f))
		{
			return false;
		}
		return true;
	}

	private void PlayTransition(string transAnim, bool AppendPose)
	{
		if (m_TransitionalAnimation != null)
		{
			m_CompoundTransitionAnimator.SetActiveRecursively(true);
			m_CompoundTransitioning = true;
			m_CompoundTransitionAnimation = m_TransitionalAnimation;
			m_CompoundTransitionPosition = m_RigMotion.position;
			m_CompoundTransitionRotation = m_RigMotion.rotation;
			m_CompoundTransitionTracker.transform.localPosition = Vector3.zero;
			m_CompoundTransitionTracker.transform.localRotation = Quaternion.identity;
			m_CompoundTransitionAnimator.animation.Play(transAnim);
		}
		m_TransitionalAnimation = transAnim;
		m_PosedAnimation = ((!AppendPose) ? null : (m_TransitionalAnimation + "_Pose"));
		m_Animator.CrossFade(m_TransitionalAnimation);
	}

	private void PlayTurnAnimation(Direction dir)
	{
		if ((m_Hostility == Hostility.Relaxed || m_Hostility == Hostility.Warning || m_Hostility == Hostility.Suspicious) && m_Urgency == Urgency.NotUrgent)
		{
			if (m_CurrentState == EnemyState.Transitioning)
			{
				if (m_TargetState == EnemyState.WalkingPatrol || m_TargetState == EnemyState.MovingToLocation)
				{
					switch (dir)
					{
					case Direction.Left:
						PlayTransition("ENY_M_Stand_CBR_PAT_walkStart_90West", true);
						break;
					case Direction.Right:
						PlayTransition("ENY_M_Stand_CBR_PAT_walkStart_90East", true);
						break;
					case Direction.BackLeft:
						PlayTransition("ENY_M_Stand_CBR_PAT_walkStart_180West", true);
						break;
					case Direction.BackRight:
						PlayTransition("ENY_M_Stand_CBR_PAT_walkStart_180East", true);
						break;
					}
				}
				else
				{
					switch (dir)
					{
					case Direction.Left:
						PlayTransition("ENY_M_Stand_CBR_PAT_Turn_90West", true);
						break;
					case Direction.Right:
						PlayTransition("ENY_M_Stand_CBR_PAT_Turn_90East", true);
						break;
					case Direction.BackLeft:
						PlayTransition("ENY_M_Stand_CBR_PAT_Turn_180West", true);
						break;
					case Direction.BackRight:
						PlayTransition("ENY_M_Stand_CBR_PAT_Turn_180East", true);
						break;
					}
				}
			}
			else if ((m_RigMotion.position - m_TargetMoveToPosition).sqrMagnitude >= 1f)
			{
				switch (dir)
				{
				case Direction.Left:
					PlayTransition("ENY_M_Stand_CBR_PAT_walkStart_90West", true);
					break;
				case Direction.Right:
					PlayTransition("ENY_M_Stand_CBR_PAT_walkStart_90East", true);
					break;
				case Direction.BackLeft:
					PlayTransition("ENY_M_Stand_CBR_PAT_walkStart_180West", true);
					break;
				case Direction.BackRight:
					PlayTransition("ENY_M_Stand_CBR_PAT_walkStart_180East", true);
					break;
				}
			}
			else
			{
				switch (dir)
				{
				case Direction.Left:
					PlayTransition("ENY_M_Stand_CBR_PAT_Turn_90West", true);
					break;
				case Direction.Right:
					PlayTransition("ENY_M_Stand_CBR_PAT_Turn_90East", true);
					break;
				case Direction.BackLeft:
					PlayTransition("ENY_M_Stand_CBR_PAT_Turn_180West", true);
					break;
				case Direction.BackRight:
					PlayTransition("ENY_M_Stand_CBR_PAT_Turn_180East", true);
					break;
				}
			}
		}
		else
		{
			switch (dir)
			{
			case Direction.Left:
				PlayTransition("ENY_M_Stand_CBR_Turn90Left", true);
				break;
			case Direction.Right:
				PlayTransition("ENY_M_Stand_CBR_Turn90Right", true);
				break;
			case Direction.BackLeft:
				PlayTransition("ENY_M_Stand_CBR_Turn180Left", true);
				break;
			case Direction.BackRight:
				PlayTransition("ENY_M_Stand_CBR_Turn180Right", true);
				break;
			}
		}
	}

	private void PlayDisturbanceAnimation(Direction dir, DisturbanceEvent type)
	{
		if (type == DisturbanceEvent.MajorAudio || (type == DisturbanceEvent.MinorAudio && m_Urgency != Urgency.NotUrgent))
		{
			switch (dir)
			{
			case Direction.Left:
				PlayTransition("ENY_M_Stand_CBR_MajorAudio_West", true);
				break;
			case Direction.Right:
				PlayTransition("ENY_M_Stand_CBR_MajorAudio_East", true);
				break;
			case Direction.BackLeft:
				PlayTransition("ENY_M_Stand_CBR_MajorAudio_SouthWest", true);
				break;
			case Direction.BackRight:
				PlayTransition("ENY_M_Stand_CBR_MajorAudio_SouthEast", true);
				break;
			default:
				PlayTransition("ENY_M_Stand_CBR_MajorAudio_North", true);
				break;
			}
		}
		else if (type == DisturbanceEvent.MajorVisual || (type == DisturbanceEvent.MinorVisual && m_Urgency != Urgency.NotUrgent))
		{
			switch (dir)
			{
			case Direction.Left:
				PlayTransition("ENY_M_Stand_CBR_MajorVisual_West", true);
				break;
			case Direction.Right:
				PlayTransition("ENY_M_Stand_CBR_MajorVisual_East", true);
				break;
			case Direction.BackLeft:
				PlayTransition("ENY_M_Stand_CBR_MajorAudio_SouthWest", true);
				break;
			case Direction.BackRight:
				PlayTransition("ENY_M_Stand_CBR_MajorAudio_SouthEast", true);
				break;
			default:
				PlayTransition("ENY_M_Stand_CBR_MajorVisual_North", true);
				break;
			}
		}
		else if (type == DisturbanceEvent.MinorAudio)
		{
			switch (dir)
			{
			case Direction.Left:
				PlayTransition("ENY_M_Stand_CBR_MinorAudio_West", true);
				break;
			case Direction.Right:
				PlayTransition("ENY_M_Stand_CBR_MinorAudio_East", true);
				break;
			case Direction.BackLeft:
				PlayTransition("ENY_M_Stand_CBR_MinorAudio_SouthWest", true);
				break;
			case Direction.BackRight:
				PlayTransition("ENY_M_Stand_CBR_MinorAudio_SouthEast", true);
				break;
			default:
				PlayTransition("ENY_M_Stand_CBR_MinorAudio_North", true);
				break;
			}
		}
		else
		{
			switch (dir)
			{
			case Direction.Left:
				PlayTransition("ENY_M_Stand_CBR_MinorVisual_West", true);
				break;
			case Direction.Right:
				PlayTransition("ENY_M_Stand_CBR_MinorVisual_East", true);
				break;
			case Direction.BackLeft:
				PlayTransition("ENY_M_Stand_CBR_MinorAudio_SouthWest", true);
				break;
			case Direction.BackRight:
				PlayTransition("ENY_M_Stand_CBR_MinorAudio_SouthEast", true);
				break;
			default:
				PlayTransition("ENY_M_Stand_CBR_MinorVisual_North", true);
				break;
			}
		}
	}

	private void RecalibrateBase()
	{
		if (m_TransitionalAnimation != null)
		{
			Vector3 position = m_RigMotion.position;
			Quaternion rotation = m_RigMotion.rotation;
			base.transform.position = position;
			base.transform.rotation = rotation;
			m_RigMotion.localPosition = Vector3.zero;
			m_RigMotion.localRotation = Quaternion.identity;
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
				m_PosedAnimation = null;
			}
			m_TransitionalAnimation = null;
		}
	}

	private void GatherSourceInfo(Vector3 sourceLocation)
	{
		m_SourceInfo.m_ToSource = sourceLocation - m_RigMotion.position;
		m_SourceInfo.m_ToSourceDistanceSqr = m_SourceInfo.m_ToSource.sqrMagnitude;
		m_SourceInfo.m_ToSourceNormalizedOnXZ = new Vector3(m_SourceInfo.m_ToSource.x, 0f, m_SourceInfo.m_ToSource.z);
		m_SourceInfo.m_ToSourceOnXZDistanceSqr = m_SourceInfo.m_ToSourceNormalizedOnXZ.sqrMagnitude;
		m_SourceInfo.m_ToSourceNormalizedOnXZ.Normalize();
		m_SourceInfo.m_ToSourceForwardDot = Vector3.Dot(m_SourceInfo.m_ToSourceNormalizedOnXZ, m_RigMotion.forward);
		m_SourceInfo.m_ToSourceRightDot = Vector3.Dot(m_SourceInfo.m_ToSourceNormalizedOnXZ, m_RigMotion.right);
		m_SourceInfo.m_ToSourceUpDot = m_SourceInfo.m_ToSource.normalized.y;
		if (Mathf.Abs(m_SourceInfo.m_ToSourceForwardDot) >= Mathf.Abs(m_SourceInfo.m_ToSourceRightDot))
		{
			if (m_SourceInfo.m_ToSourceForwardDot >= 0f)
			{
				m_SourceInfo.m_ToSourceDirection = Direction.Forward;
			}
			else if (m_SourceInfo.m_ToSourceRightDot >= 0f)
			{
				m_SourceInfo.m_ToSourceDirection = Direction.BackRight;
			}
			else
			{
				m_SourceInfo.m_ToSourceDirection = Direction.BackLeft;
			}
		}
		else if (m_SourceInfo.m_ToSourceRightDot >= 0f)
		{
			m_SourceInfo.m_ToSourceDirection = Direction.Right;
		}
		else
		{
			m_SourceInfo.m_ToSourceDirection = Direction.Left;
		}
	}

	public virtual bool CheckVisualCone()
	{
		m_SeePlayerThisFrame = false;
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
		Vector3 forward = m_Neck.forward;
		forward.y = 0f;
		forward.Normalize();
		Vector3 vector = Globals.m_PlayerController.transform.position - m_Neck.position;
		Vector3 lhs = new Vector3(vector.x, 0f, vector.z);
		float sqrMagnitude = lhs.sqrMagnitude;
		lhs.Normalize();
		float num = Vector3.Dot(lhs, forward);
		if (Globals.m_PlayerController.IsInCover() && Vector3.Dot(lhs, Globals.m_PlayerController.m_CoverNormal) >= 0.3f)
		{
			return ProcessVisualTimer(true);
		}
		float num2 = ((m_Hostility != Hostility.Hostile) ? m_RelaxedVisualConeDot : m_HostileVisualConeDot);
		if (num >= num2 && sqrMagnitude <= 400f)
		{
			Ray ray = new Ray(m_Neck.position, Globals.m_PlayerController.GetChestLocation() - m_Neck.position);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 100f, 82177) && (hitInfo.collider.gameObject.layer == 14 || (hitInfo.collider == Globals.m_PlayerController.m_CoverCollider && Globals.m_PlayerController.ExposedInCover())))
			{
				m_SeePlayerThisFrame = true;
				Globals_NEW.m_AIDirector.UpdatePlayerKnownPosition();
				m_VisualSensingTimer += Time.deltaTime;
				return ProcessVisualTimer(false);
			}
		}
		return ProcessVisualTimer(true);
	}

	public virtual bool ProcessVisualTimer(bool ResetTimer)
	{
		if (m_Hostility == Hostility.Hostile)
		{
			return false;
		}
		bool result = false;
		if (m_VisualSensingTimer >= 0.75f)
		{
			EnterCombat(DisturbanceID.Visual_InPlainSight, Globals.m_PlayerController.transform.position);
			result = true;
		}
		else if (m_VisualSensingTimer >= 0.2f && ResetTimer)
		{
			InvestigateDisturbance(Globals.m_PlayerController.transform.position, DisturbanceEvent.MinorVisual, DisturbanceID.Visual_InPlainSight);
			result = true;
		}
		if (ResetTimer)
		{
			m_VisualSensingTimer = 0f;
		}
		return result;
	}

	private bool CalculateNavPath(Vector3 targetLocation, float radius = 5f)
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

	private void ResumeNavigation()
	{
		if (m_Hostility == Hostility.Hostile)
		{
			m_NavAgent.speed = 4f;
			m_NavAgent.angularSpeed = 300f;
			m_NavAgent.updateRotation = false;
		}
		else if (m_Hostility == Hostility.Covering)
		{
			m_NavAgent.speed = 4f;
			m_NavAgent.angularSpeed = 300f;
			m_NavAgent.updateRotation = false;
		}
		else
		{
			if (m_Urgency == Urgency.NotUrgent)
			{
				m_NavAgent.speed = 0.9f;
				m_NavAgent.angularSpeed = 120f;
			}
			else
			{
				m_NavAgent.speed = 4f;
				m_NavAgent.angularSpeed = 300f;
			}
			m_NavAgent.updateRotation = true;
		}
		m_NavAgent.updatePosition = true;
		m_NavAgent.SetPath(m_NavMeshPath);
		m_NavAgent.Resume();
	}

	private void StopNavigation()
	{
		m_NavAgent.Stop(true);
		m_NavAgent.updatePosition = true;
	}

	private void EnableAiming(AimingType Type)
	{
		if (m_CurrentAimingType == (int)Type)
		{
			return;
		}
		if (Type == AimingType.None)
		{
			m_CancelledAimType = m_CurrentAimingType;
			m_CurrentAimingType = -1;
			return;
		}
		if (m_CurrentAimingType >= 0)
		{
			for (int i = 0; i < 9; i++)
			{
				m_AimingAnimations[m_CurrentAimingType][i].enabled = false;
			}
		}
		else
		{
			m_AimingFlags = 0;
			m_GlobalAimBlendingWeight = 0f;
			for (int j = 0; j < 9; j++)
			{
				m_AimingWeights[j] = 0f;
			}
		}
		m_CurrentAimingType = (int)Type;
		if (m_CurrentAimingType >= 0)
		{
			for (int k = 0; k < 9; k++)
			{
				m_AimingAnimations[m_CurrentAimingType][k].enabled = true;
				m_AimingAnimations[m_CurrentAimingType][k].weight = m_AimingWeights[k];
			}
		}
	}

	protected float BiLerp(float v0, float v1, float v2, float v3, float x, float y)
	{
		return Mathf.Lerp(Mathf.Lerp(v0, v1, x), Mathf.Lerp(v2, v3, x), y);
	}

	public virtual bool FindCover(Vector3 threatLocation, Urgency urgency, NearbyCover ignoredCover)
	{
		return m_ChosenCover != null;
	}

	public virtual void AssignWeapon(WeaponBase weapon)
	{
		if (!(weapon == null))
		{
			m_Weapon = weapon;
			m_Weapon.SetUser(this);
			m_Weapon.transform.parent = base.transform;
			m_Weapon.transform.localPosition = Vector3.zero;
			m_Weapon.transform.localRotation = Quaternion.identity;
			m_Weapon.m_ModelFirstPerson.SetActiveRecursively(false);
			m_Weapon.m_ModelThirdPersonPlayer.SetActiveRecursively(false);
			m_Weapon.m_ModelThirdPersonEnemy.transform.parent = m_WeaponAttachRight.transform;
			m_Weapon.m_ModelThirdPersonEnemy.transform.localPosition = Vector3.zero;
			m_Weapon.m_ModelThirdPersonEnemy.transform.localRotation = Quaternion.identity;
			m_Weapon.SetEnemy();
		}
	}

	private void SwapWeaponHands(bool UseRightHand, bool ZeroOut = true)
	{
	}

	private void DropWeapon()
	{
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

	private bool SquadmateInLoS()
	{
		return false;
	}

	private bool SquadmateInTheWay(Vector3 MovementVector, float RayLength)
	{
		return false;
	}

	public virtual void AdjustCollider()
	{
	}

	public void Stun(Vector3 sourceLocation, DamageType Type)
	{
	}

	public override bool TakeDamage(DamageData data)
	{
		bool flag = base.TakeDamage(data);
		if (!flag)
		{
			if (data.m_DamageType == DamageType.Concussion || data.m_DamageType == DamageType.Explosive)
			{
				Stun(data.m_SourceLocation, data.m_DamageType);
			}
			if (m_Hostility != Hostility.Hostile)
			{
				EnterCombat(DisturbanceID.Misc_TakingDamage, data.m_SourceLocation);
			}
		}
		return flag;
	}

	public override void Die(DamageData data)
	{
	}

	public virtual void Removed()
	{
	}

	public void PlayVO(string eventName, int Priority)
	{
		if (m_CurrentVO != 0)
		{
			if (Priority <= m_CurrentVOPriority)
			{
				return;
			}
			SoundManager.StopEvent(m_CurrentVO);
		}
		switch (m_Ethnicity)
		{
		case Ethnicity.Guard_Asian_01:
			SoundManager.SetSoundSwitch("Ethnicity", "Guard_Asian_01", base.gameObject);
			break;
		case Ethnicity.Guard_Asian_02:
			SoundManager.SetSoundSwitch("Ethnicity", "Guard_Asian_02", base.gameObject);
			break;
		case Ethnicity.Guard_White_01:
			SoundManager.SetSoundSwitch("Ethnicity", "Guard_White_01", base.gameObject);
			break;
		}
		m_CurrentVO = SoundManager.TriggerEvent(eventName, base.gameObject);
		m_CurrentVOPriority = Priority;
	}

	private void CalculateFireDirection()
	{
		if (!(m_Weapon == null))
		{
			m_CurrentAccuracyVariation = Mathf.Max(m_CurrentAccuracyVariation - m_AccuracyRefinementRate * Time.deltaTime, 0f);
			Vector3 vector = Globals.m_PlayerController.GetChestLocation() - m_Weapon.m_MuzzleFlashAttachEnemy.transform.position;
			vector.Normalize();
			m_FireDirection = m_Weapon.m_MuzzleFlashAttachEnemy.transform.forward;
			if (Vector3.Dot(m_FireDirection, vector) >= 0.966f)
			{
				m_FireDirection = vector;
			}
		}
	}

	public override Ray WeaponRequestForBulletRay(out bool PlayTracer)
	{
		PlayTracer = true;
		Vector3 vector = Globals.m_PlayerController.transform.position - m_RigMotion.transform.position;
		vector.y = 0f;
		if (vector.sqrMagnitude <= 1.5f && Vector3.Dot(vector.normalized, m_Weapon.m_MuzzleFlashAttachEnemy.transform.forward) >= 0.65f)
		{
			PlayTracer = false;
			return new Ray(GetChestLocation(), Globals.m_PlayerController.GetChestLocation() - GetChestLocation());
		}
		Quaternion quaternion = Quaternion.Euler(UnityEngine.Random.Range(0f - m_CurrentAccuracyVariation, m_CurrentAccuracyVariation), UnityEngine.Random.Range(0f - m_CurrentAccuracyVariation, m_CurrentAccuracyVariation), 0f);
		vector = quaternion * m_FireDirection;
		return new Ray(m_Weapon.m_MuzzleFlashAttachEnemy.transform.position, vector);
	}

	public override Vector3 GetChestLocation()
	{
		return m_Waist.position + Vector3.up * 0f;
	}

	protected override void AttachShadowObject()
	{
		m_ShadowObject.transform.parent = null;
		m_ShadowObject.transform.localRotation = Quaternion.identity;
	}

	protected override void UpdateShadowObjectPosition()
	{
		Vector3 center = m_MeshRenderer.bounds.center;
		center.y = base.transform.position.y;
		m_ShadowObject.transform.position = center;
	}

	protected virtual void StartFiring()
	{
		if (!(m_Weapon == null) && !m_Firing && m_CurrentAimingType != -1)
		{
			m_Firing = true;
			m_Animator.CrossFade(m_FiringAnimations[m_CurrentAimingType].name);
			m_FiringTimer = UnityEngine.Random.Range(m_MinFiringDuration, m_MaxFiringDuration);
			m_CurrentAccuracyVariation = m_MaxAccuracyVariation;
			m_Weapon.StartFire();
		}
	}

	protected virtual void StopFiring()
	{
		if (!(m_Weapon == null) && m_Firing)
		{
			m_Firing = false;
			for (int i = 0; i < 7; i++)
			{
				m_Animator.Stop(m_FiringAnimations[i].name);
			}
			m_FiringTimer = UnityEngine.Random.Range(m_MinFiringCooldown, m_MaxFiringCooldown);
			if (m_Weapon.LowOnAmmo())
			{
				m_WaitingToReload = true;
			}
			m_Weapon.EndFire();
		}
	}

	public override void WeaponWantsReload()
	{
		if (m_Weapon != null && m_Weapon.m_WeaponState != WeaponBase.WeaponState.Reloading)
		{
			m_WaitingToReload = true;
			StopFiring();
		}
	}

	public override void WeaponDoneReloading()
	{
	}

	public void BeginTakedownOnEnemy()
	{
	}

	public void TakedownLethal()
	{
	}

	public void TakedownNonLethal()
	{
	}
}
