using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Enemy_Base : CharacterBase
{
	public enum EnemyState
	{
		None = -1,
		Patrol = 0,
		Suspicious = 1,
		Alarmed = 2,
		Combat = 3,
		Hostile = 4,
		Death = 5,
		WarningPlayer = 6,
		TransitionToSuspicious = 7,
		TransitionToAlarmed = 8,
		Total = 9
	}

	public enum PatrolState
	{
		None = -1,
		StandingIdle = 0,
		Patrolling = 1,
		TurningToWaypoint = 2,
		AtWaypoint = 3,
		TurningFromWaypoint = 4,
		WaitingToRePatrol = 5,
		WalkBackToRePatrol = 6,
		TransitionToRePatrol = 7,
		ReturnToPatrolImmediate = 8,
		Total = 9
	}

	public enum SuspiciousState
	{
		None = -1,
		PostTransition = 0,
		TurnToWalk = 1,
		WalkToDisturbance = 2,
		ScanArea = 3,
		Total = 4
	}

	public enum AlarmedState
	{
		None = -1,
		PostTransition = 0,
		HandleNewAlarmedLocation = 1,
		TurnToRun = 2,
		RunToDisturbance = 3,
		SearchForSource = 4,
		WaitingForPathToCover = 5,
		StandAtTheReady = 6,
		RunToCover = 7,
		EnteringCover = 8,
		InCover = 9,
		AimFromCover = 10,
		ExitingCover = 11,
		Total = 12
	}

	public enum CombatState
	{
		None = -1,
		WaitingForCover = 0,
		ChargePlayer = 1,
		StandAndFire = 2,
		RunToCover = 3,
		StandAtCover = 4,
		EnterCover = 5,
		InCover = 6,
		CoverFiring = 7,
		ExitingCover = 8,
		Staggering = 9,
		Total = 10
	}

	public enum HostileState
	{
		None = -1,
		WaitToStartSearching = 0,
		TurnToStartSearching = 1,
		RunToSearch = 2,
		Searching = 3,
		EnteringCover = 4,
		FiringFromCover = 5,
		ExitingCover = 6,
		StandReady = 7,
		Total = 8
	}

	public enum WarningState
	{
		None = -1,
		StandingIdle = 0,
		Total = 1
	}

	public enum AimDirection
	{
		None = -1,
		Center = 0,
		Up = 1,
		Down = 2,
		Left = 3,
		Right = 4,
		UpLeft = 5,
		UpRight = 6,
		DownLeft = 7,
		DownRight = 8,
		Total = 9
	}

	public enum TurnDirection
	{
		Forward = 0,
		BackLeft = 1,
		BackRight = 2,
		Left = 3,
		Right = 4
	}

	public enum CoverStatus
	{
		None = -1,
		CrouchingLeft = 0,
		CrouchingRight = 1,
		StandingLeft = 2,
		StandingRight = 3,
		Total = 4
	}

	public enum CoverFireStatus
	{
		None = -1,
		TransitionIn = 0,
		WaitToFire = 1,
		Fire = 2,
		WaitToHide = 3,
		TransitionOut = 4,
		ThrowingGrenade = 5,
		Total = 6
	}

	public enum CoverFireDirection
	{
		None = -1,
		Side = 0,
		Over = 1,
		Total = 2
	}

	public enum CoverFireAnimations
	{
		None = -1,
		CrouchLeft = 0,
		CrouchLeftOver = 1,
		CrouchRight = 2,
		CrouchRightOver = 3,
		StandLeft = 4,
		StandRight = 5,
		Total = 6
	}

	private const float m_DeathFadeTime = 5f;

	public Animation m_Animator;

	public Rigidbody m_Rigidbody;

	public CapsuleCollider m_Collider;

	public NavMeshAgent m_NavAgent;

	public Renderer m_Renderer;

	public Material m_DeathMaterial;

	protected bool m_DeadBodyFound;

	protected SphereCollider m_DeathCollider;

	public Transform m_StunGunEffectAttachment;

	protected EnemyType m_EnemyType = EnemyType.None;

	[HideInInspector]
	public EnemySquad m_EnemySquad;

	[HideInInspector]
	public Ethnicity m_Ethnicity;

	[HideInInspector]
	public Vector3 m_SpawnLocation;

	[HideInInspector]
	public bool m_Neutral;

	private bool m_PlayerWarned;

	protected EnemyState m_EnemyState = EnemyState.None;

	protected PatrolState m_PatrolState = PatrolState.None;

	protected SuspiciousState m_SuspiciousState = SuspiciousState.None;

	protected AlarmedState m_AlarmedState = AlarmedState.None;

	protected CombatState m_CombatState = CombatState.None;

	protected HostileState m_HostileState = HostileState.None;

	protected WarningState m_WarningState = WarningState.None;

	private float m_StateTimer;

	private float m_EventTimer;

	private float m_CooldownTimer;

	private float m_MoveTimer;

	private float m_DisturbanceIgnoreTimer;

	public float m_StrollSpeed = 1f;

	public float m_WalkSpeed = 0.9f;

	public float m_RunSpeed = 4f;

	public float m_StrollTurnSpeed = 120f;

	public float m_WalkTurnSpeed = 210f;

	public float m_RunTurnSpeed = 300f;

	public float m_PatrolTurnSpeed = 5f;

	private float m_CurrentPatrolWalkSpeed = 1f;

	private float m_CurrentPatrolTurnSpeed = 5f;

	[HideInInspector]
	public bool m_Paused;

	private NavMeshPath m_NavMeshPath;

	private bool m_WaitingForPathToClear;

	private Vector3 m_PreBlockVelocity = Vector3.zero;

	private float m_ContinuePathTimer;

	[HideInInspector]
	public PatrolNode m_StartingNode;

	private PatrolNode m_TargetNode;

	private bool m_PatrolForward = true;

	private PatrolNode m_EventNode;

	private PatrolNode.WaypointDirection m_WPDir;

	private Vector3 m_LastPatrolledPosition = Vector3.zero;

	[HideInInspector]
	public NearbyCover m_ChosenCover;

	private NearbyCover m_LastChosenCover;

	private float m_PlayerLostDelayToInvestigate = 15f;

	private float m_PlayerLostDelayToStopFiring = 5f;

	protected CoverFireStatus m_CoverFireStatus = CoverFireStatus.None;

	protected CoverFireDirection m_CoverFireDirection = CoverFireDirection.None;

	private bool m_FiringFromCover;

	private bool m_SearchSearchNode;

	[HideInInspector]
	public SearchNode m_ChosenSearchNode;

	[HideInInspector]
	public bool m_DoneSearching;

	protected CoverStatus m_CoverStatus = CoverStatus.None;

	private string m_CoverTransitionInAnimation;

	private bool m_NavMoving;

	private Vector3 m_AudioDisturbanceLocation = Vector3.zero;

	public float m_MinInvestigateTime = 10f;

	public float m_MaxInvestigateTime = 15f;

	[HideInInspector]
	public WeaponBase m_Weapon;

	public GameObject m_WeaponAttachRight;

	public GameObject m_WeaponAttachLeft;

	public GameObject m_RigMotion;

	private Vector3 m_ChestLocation = Vector3.zero;

	public float m_VisualForwardThresholdInDegrees = 130f;

	private float m_VisualForwardThresholdDot;

	public float m_VisualDistanceThreshold = 20f;

	private float m_VisualDistanceThresholdSqr;

	private bool m_VisualSensesChecked;

	private float m_VisualSensingTimer;

	public float m_VisualThresholdToEnterCombat = 0.75f;

	public float m_VisualThresholdToInvestigate = 0.2f;

	public float m_VisualCombatDistanceThreshold = 30f;

	private float m_VisualCombatDistanceThresholdSqr;

	[HideInInspector]
	public bool m_SeePlayerThisFrame;

	[HideInInspector]
	public bool m_LostPlayer;

	private Vector3 m_ToSource = Vector3.zero;

	private Vector3 m_ToSourceNormalizedOnXZ = Vector3.zero;

	private float m_ToSourceDot;

	private float m_ToSourceRightDot;

	private float m_ToSourceUpDot;

	private TurnDirection m_SourceDirection;

	private float m_AngleWhenTargetCrouched = 50f;

	public Transform m_Waist;

	protected string m_TransitionalAnimation;

	protected string m_PosedAnimation;

	[HideInInspector]
	public float m_CurrentAnimationSpeed = 1f;

	public float m_LeftAimingExtentInDegrees = 46.18f;

	public float m_RightAimingExtentInDegrees = 45.7f;

	public float m_UpAimingExtentInDegrees = 37f;

	public float m_DownAimingExtentInDegrees = 35.95f;

	public Vector4[] m_InCoverAimingExtentInDegrees = new Vector4[6];

	private int m_CoverAimingIndex;

	protected AnimationState[] m_ActiveAimAnimations;

	protected AnimationState[] m_CBRAimDirections = new AnimationState[9];

	protected AnimationState[][] m_InCoverAimDirections = new AnimationState[6][];

	protected AnimationState m_CBRFiring;

	protected AnimationState m_CBRReload;

	private GameObject m_CompoundTransitionAnimator;

	private GameObject m_CompoundTransitionTracker;

	private bool m_TrackCompoundTransition;

	private string m_CompoundTransitionAnimationFrom;

	private Vector3 m_CompoundTransitionPosition = Vector3.zero;

	private Quaternion m_CompoundTransitionRotation = Quaternion.identity;

	private Vector3 m_FireDirection = Vector3.zero;

	private bool m_WaitingToReload;

	public float m_MinBurstDuration = 0.75f;

	public float m_MaxBurstDuration = 1.5f;

	public float m_MinBurstCooldown = 2f;

	public float m_MaxBurstCooldown = 3.5f;

	private float m_BurstTimer;

	private bool m_Bursting;

	private GrenadeFrag m_Grenade;

	private float m_ThrowGrenadeTimer = -1f;

	public CapsuleCollider m_CapsuleCollider;

	public float m_ColliderHeightStanding = 2f;

	public float m_ColliderHeightCrouching = 1f;

	public float m_ColliderHeightCrouchLeaning = 1.5f;

	public float m_BulletAdjustThresholdInDegrees = 15f;

	private float m_BulletAdjustDotThreshold;

	private float m_MaxAccuracyVariation = 8f;

	private float m_CurrentAccuracyVariance;

	private float m_AccuracyAdjustmentSpeed = 3f;

	private bool m_WantToExitCover;

	[HideInInspector]
	public bool m_WantToPatrol;

	private bool m_WantsToInvestigate;

	private Vector3 m_WantsToInvestigateLocation = Vector3.zero;

	private bool m_WantsToEnterCover;

	private bool m_WantsNewCover;

	private float m_MaxPCRangeSqr = 500f;

	private int m_CurrentVOPriority = -1;

	private uint m_CurrentVO;

	private float m_StaggerCooldown;

	public bool InCombat()
	{
		return m_EnemyState == EnemyState.Combat;
	}

	public bool IsAlarmed()
	{
		return (m_EnemyState == EnemyState.Alarmed && !m_WantToPatrol) || m_EnemyState == EnemyState.TransitionToAlarmed;
	}

	public bool IsSuspicious()
	{
		return m_EnemyState == EnemyState.Suspicious || m_EnemyState == EnemyState.TransitionToSuspicious;
	}

	public bool IsHostile()
	{
		return m_EnemyState == EnemyState.Hostile && !m_WantToPatrol;
	}

	public bool IsPassive()
	{
		return m_EnemyState == EnemyState.Patrol || m_EnemyState == EnemyState.WarningPlayer;
	}

	public override bool IsDead()
	{
		return base.IsDead() || m_EnemyState == EnemyState.Death;
	}

	public override bool IsInCover()
	{
		if (m_EnemyState == EnemyState.Alarmed)
		{
			return m_AlarmedState == AlarmedState.EnteringCover || m_AlarmedState == AlarmedState.InCover || m_AlarmedState == AlarmedState.AimFromCover;
		}
		if (m_EnemyState == EnemyState.Combat)
		{
			return m_CombatState == CombatState.EnterCover || m_CombatState == CombatState.InCover || m_CombatState == CombatState.CoverFiring;
		}
		if (m_EnemyState == EnemyState.Hostile)
		{
			return m_HostileState == HostileState.EnteringCover || m_HostileState == HostileState.FiringFromCover;
		}
		return false;
	}

	public override bool ExposedInCover()
	{
		if (m_EnemyState == EnemyState.Alarmed)
		{
			return m_AlarmedState == AlarmedState.AimFromCover;
		}
		if (m_EnemyState == EnemyState.Combat)
		{
			return m_CombatState == CombatState.CoverFiring && m_CoverFireStatus != CoverFireStatus.TransitionOut;
		}
		return false;
	}

	public override Vector3 GetCoverNormal()
	{
		if (m_ChosenCover != null)
		{
			return -m_ChosenCover.m_Cover.transform.forward;
		}
		return Vector3.zero;
	}

	public override Collider GetCoverCollider()
	{
		if (m_ChosenCover != null)
		{
			return m_ChosenCover.m_Cover.m_CoverCollider;
		}
		return null;
	}

	public override int GetGlobalLayer()
	{
		return 9;
	}

	public override WeaponType GetEquippedWeaponType()
	{
		if (m_Weapon != null)
		{
			return m_Weapon.m_WeaponType;
		}
		return WeaponType.None;
	}

	public WeaponBase GetWeapon()
	{
		return m_Weapon;
	}

	public virtual void Awake()
	{
	}

	protected override void Start()
	{
		base.Start();
		m_NavMeshPath = new NavMeshPath();
		m_VisualForwardThresholdDot = Mathf.Cos((float)Math.PI / 180f * (m_VisualForwardThresholdInDegrees * 0.5f));
		m_VisualDistanceThresholdSqr = m_VisualDistanceThreshold * m_VisualDistanceThreshold;
		m_VisualCombatDistanceThresholdSqr = m_VisualCombatDistanceThreshold * m_VisualCombatDistanceThreshold;
		m_BulletAdjustDotThreshold = Mathf.Cos((float)Math.PI / 180f * m_BulletAdjustThresholdInDegrees);
		LoadAnimations();
		m_CompoundTransitionAnimator = UnityEngine.Object.Instantiate(m_RigMotion.transform.parent.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
		m_CompoundTransitionAnimator.name = base.gameObject.name + "_CTTracker";
		m_CompoundTransitionTracker = m_CompoundTransitionAnimator.transform.GetChild(0).gameObject;
		UnityEngine.Object.Destroy(m_CompoundTransitionTracker.transform.GetChild(0).gameObject);
		m_CompoundTransitionAnimator.SetActiveRecursively(false);
		m_NavAgent.updateRotation = false;
		if (!GameManager.m_SaveLoaded)
		{
			m_TargetNode = m_StartingNode;
			if (m_TargetNode != null)
			{
				PatrolNode patrolNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
				base.transform.position = m_TargetNode.transform.position;
				if (patrolNode != null)
				{
					Vector3 vector = patrolNode.transform.position - m_TargetNode.transform.position;
					vector.y = 0f;
					base.transform.rotation = Quaternion.LookRotation(vector.normalized);
				}
			}
			RayClampToGround();
			StartPatrol(false);
		}
		m_LastPatrolledPosition = base.transform.position;
	}

	protected virtual void LoadAnimations()
	{
		m_CBRAimDirections[0] = m_Animator["Aim_Stand_Center"];
		m_CBRAimDirections[3] = m_Animator["Aim_Stand_Left"];
		m_CBRAimDirections[4] = m_Animator["Aim_Stand_Right"];
		m_CBRAimDirections[1] = m_Animator["Aim_Stand_Up"];
		m_CBRAimDirections[2] = m_Animator["Aim_Stand_Down"];
		m_CBRAimDirections[5] = m_Animator["Aim_Stand_UpLeft"];
		m_CBRAimDirections[6] = m_Animator["Aim_Stand_UpRight"];
		m_CBRAimDirections[7] = m_Animator["Aim_Stand_DownLeft"];
		m_CBRAimDirections[8] = m_Animator["Aim_Stand_DownRight"];
		for (int i = 0; i < 9; i++)
		{
			m_CBRAimDirections[i].AddMixingTransform(m_Waist, true);
			m_CBRAimDirections[i].blendMode = AnimationBlendMode.Blend;
			m_CBRAimDirections[i].weight = 1f;
			m_CBRAimDirections[i].layer = 10;
		}
		m_InCoverAimDirections[0] = new AnimationState[9];
		m_InCoverAimDirections[0][0] = m_Animator["Aim_CrouchLeft_Center"];
		m_InCoverAimDirections[0][3] = m_Animator["Aim_CrouchLeft_Left"];
		m_InCoverAimDirections[0][4] = m_Animator["Aim_CrouchLeft_Right"];
		m_InCoverAimDirections[0][1] = m_Animator["Aim_CrouchLeft_Up"];
		m_InCoverAimDirections[0][2] = m_Animator["Aim_CrouchLeft_Down"];
		m_InCoverAimDirections[0][5] = m_Animator["Aim_CrouchLeft_UpLeft"];
		m_InCoverAimDirections[0][6] = m_Animator["Aim_CrouchLeft_UpRight"];
		m_InCoverAimDirections[0][7] = m_Animator["Aim_CrouchLeft_DownLeft"];
		m_InCoverAimDirections[0][8] = m_Animator["Aim_CrouchLeft_DownRight"];
		m_InCoverAimDirections[1] = new AnimationState[9];
		m_InCoverAimDirections[1][0] = m_Animator["Aim_CrouchLeftOver_Center"];
		m_InCoverAimDirections[1][3] = m_Animator["Aim_CrouchLeftOver_Left"];
		m_InCoverAimDirections[1][4] = m_Animator["Aim_CrouchLeftOver_Right"];
		m_InCoverAimDirections[1][1] = m_Animator["Aim_CrouchLeftOver_Up"];
		m_InCoverAimDirections[1][2] = m_Animator["Aim_CrouchLeftOver_Down"];
		m_InCoverAimDirections[1][5] = m_Animator["Aim_CrouchLeftOver_UpLeft"];
		m_InCoverAimDirections[1][6] = m_Animator["Aim_CrouchLeftOver_UpRight"];
		m_InCoverAimDirections[1][7] = m_Animator["Aim_CrouchLeftOver_DownLeft"];
		m_InCoverAimDirections[1][8] = m_Animator["Aim_CrouchLeftOver_DownRight"];
		m_InCoverAimDirections[2] = new AnimationState[9];
		m_InCoverAimDirections[2][0] = m_Animator["Aim_CrouchRight_Center"];
		m_InCoverAimDirections[2][3] = m_Animator["Aim_CrouchRight_Left"];
		m_InCoverAimDirections[2][4] = m_Animator["Aim_CrouchRight_Right"];
		m_InCoverAimDirections[2][1] = m_Animator["Aim_CrouchRight_Up"];
		m_InCoverAimDirections[2][2] = m_Animator["Aim_CrouchRight_Down"];
		m_InCoverAimDirections[2][5] = m_Animator["Aim_CrouchRight_UpLeft"];
		m_InCoverAimDirections[2][6] = m_Animator["Aim_CrouchRight_UpRight"];
		m_InCoverAimDirections[2][7] = m_Animator["Aim_CrouchRight_DownLeft"];
		m_InCoverAimDirections[2][8] = m_Animator["Aim_CrouchRight_DownRight"];
		m_InCoverAimDirections[3] = new AnimationState[9];
		m_InCoverAimDirections[3][0] = m_Animator["Aim_CrouchRightOver_Center"];
		m_InCoverAimDirections[3][3] = m_Animator["Aim_CrouchRightOver_Left"];
		m_InCoverAimDirections[3][4] = m_Animator["Aim_CrouchRightOver_Right"];
		m_InCoverAimDirections[3][1] = m_Animator["Aim_CrouchRightOver_Up"];
		m_InCoverAimDirections[3][2] = m_Animator["Aim_CrouchRightOver_Down"];
		m_InCoverAimDirections[3][5] = m_Animator["Aim_CrouchRightOver_UpLeft"];
		m_InCoverAimDirections[3][6] = m_Animator["Aim_CrouchRightOver_UpRight"];
		m_InCoverAimDirections[3][7] = m_Animator["Aim_CrouchRightOver_DownLeft"];
		m_InCoverAimDirections[3][8] = m_Animator["Aim_CrouchRightOver_DownRight"];
		for (int j = 0; j < 6; j++)
		{
			if (j != 4 && j != 5)
			{
				for (int k = 0; k < 9; k++)
				{
					m_InCoverAimDirections[j][k].AddMixingTransform(m_Waist, true);
					m_InCoverAimDirections[j][k].blendMode = AnimationBlendMode.Blend;
					m_InCoverAimDirections[j][k].weight = 1f;
					m_InCoverAimDirections[j][k].layer = 10;
				}
			}
		}
		m_CBRFiring = m_Animator["Weapon_Fire"];
		m_CBRFiring.AddMixingTransform(m_Waist, true);
		m_CBRFiring.blendMode = AnimationBlendMode.Additive;
		m_CBRFiring.layer = 20;
		m_Animator["Aim_CrouchLeft_FireLoop"].AddMixingTransform(m_Waist, true);
		m_Animator["Aim_CrouchLeft_FireLoop"].blendMode = AnimationBlendMode.Additive;
		m_Animator["Aim_CrouchLeft_FireLoop"].layer = 20;
		m_Animator["Aim_CrouchLeftOver_FireLoop"].AddMixingTransform(m_Waist, true);
		m_Animator["Aim_CrouchLeftOver_FireLoop"].blendMode = AnimationBlendMode.Additive;
		m_Animator["Aim_CrouchLeftOver_FireLoop"].layer = 20;
		m_Animator["Aim_CrouchRight_FireLoop"].AddMixingTransform(m_Waist, true);
		m_Animator["Aim_CrouchRight_FireLoop"].blendMode = AnimationBlendMode.Additive;
		m_Animator["Aim_CrouchRight_FireLoop"].layer = 20;
		m_Animator["Aim_CrouchRightOver_FireLoop"].AddMixingTransform(m_Waist, true);
		m_Animator["Aim_CrouchRightOver_FireLoop"].blendMode = AnimationBlendMode.Additive;
		m_Animator["Aim_CrouchRightOver_FireLoop"].layer = 20;
		m_CBRReload = m_Animator["Weapon_Reload"];
		m_CBRReload.AddMixingTransform(m_Waist, true);
		m_CBRReload.blendMode = AnimationBlendMode.Additive;
		m_CBRReload.layer = 30;
		m_Animator["Damaged_DartIdle"].AddMixingTransform(m_Waist, true);
		m_Animator["Damaged_DartIdle"].layer = 40;
	}

	public virtual void InvestigateAlarmingSound(Vector3 sourceLocation, bool FromWeaponFire = false, bool FromSave = false)
	{
		if (m_EnemyState == EnemyState.Combat || ((m_EnemyState == EnemyState.Alarmed || m_EnemyState == EnemyState.TransitionToAlarmed) && m_DisturbanceIgnoreTimer < 4f))
		{
			return;
		}
		if (m_NavMoving)
		{
			m_NavAgent.Stop(true);
			m_NavMoving = false;
			m_NavAgent.updateRotation = false;
		}
		m_AudioDisturbanceLocation = sourceLocation;
		m_DisturbanceIgnoreTimer = 0f;
		bool atLeastOneOtherSquadmate = Globals.m_AIDirector.EnemyIsAlarmed(this, m_AudioDisturbanceLocation);
		m_EnemyState = EnemyState.TransitionToAlarmed;
		m_PlayerWarned = false;
		m_StateTimer = 60f;
		DetermineSourceDirection(m_AudioDisturbanceLocation);
		if (!FromSave)
		{
			PlayDisturbance(true, false, m_SourceDirection);
			if (FromWeaponFire)
			{
				PlayVO(AudioEvent.HeardWeaponFire, atLeastOneOtherSquadmate);
			}
			else
			{
				PlayVO("Play_VO_Alarmed_Audio", 70);
			}
		}
		EnableAimBlending(false, null);
		SwapWeaponHands(true, true);
	}

	public virtual void SupportAlarmingInvestigation(Vector3 sourceLocation)
	{
		if (m_EnemyState == EnemyState.Combat)
		{
			return;
		}
		m_AudioDisturbanceLocation = sourceLocation;
		m_DisturbanceIgnoreTimer = 0f;
		if (m_NavMoving)
		{
			m_NavAgent.Stop(true);
			m_NavMoving = false;
			m_NavAgent.updateRotation = false;
		}
		LinkedList<NearbyCover> coverList = Globals.m_AIDirector.GetCoverList(m_AudioDisturbanceLocation);
		LinkedListNode<NearbyCover> linkedListNode = coverList.First;
		NearbyCover nearbyCover = null;
		float num = 1000000f;
		while (linkedListNode != null)
		{
			if (linkedListNode.Value.m_Enemy == null && linkedListNode.Value.m_DistanceSqr >= 0f && linkedListNode.Value.m_DistanceSqr <= 10000f)
			{
				float sqrMagnitude = (linkedListNode.Value.m_Cover.transform.position - base.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					nearbyCover = linkedListNode.Value;
					num = sqrMagnitude;
				}
			}
			linkedListNode = linkedListNode.Next;
		}
		if (nearbyCover != null)
		{
			m_ChosenCover = nearbyCover;
			m_ChosenCover.m_Enemy = this;
		}
		DetermineSourceDirection(m_AudioDisturbanceLocation);
		PlayDisturbance(true, false, m_SourceDirection);
		m_EnemyState = EnemyState.Alarmed;
		m_PlayerWarned = false;
		m_AlarmedState = AlarmedState.WaitingForPathToCover;
		m_StateTimer = 60f;
		if (m_ChosenCover != null)
		{
			Vector3 position = m_ChosenCover.m_Cover.transform.position;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(position, out hit, 0.5f, m_NavAgent.walkableMask))
			{
				position = hit.position;
			}
			m_NavAgent.updateRotation = false;
			m_NavAgent.updatePosition = false;
			m_NavMoving = true;
			m_NavAgent.ResetPath();
			m_NavAgent.enabled = false;
			m_NavAgent.enabled = true;
			m_NavAgent.CalculatePath(position, m_NavMeshPath);
		}
		EnableAimBlending(false, null);
		SwapWeaponHands(true, true);
	}

	public virtual void UpdateAlarmingSoundLocation(Vector3 sourceLocation)
	{
		if (m_TransitionalAnimation == null)
		{
			if (m_NavMoving)
			{
				m_NavAgent.Stop(true);
				m_NavMoving = false;
				m_NavAgent.updateRotation = false;
			}
			m_AudioDisturbanceLocation = sourceLocation;
			m_DisturbanceIgnoreTimer = 0f;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(m_AudioDisturbanceLocation, out hit, 5f, m_NavAgent.walkableMask))
			{
				m_AudioDisturbanceLocation = hit.position;
			}
			if (m_TransitionalAnimation != null)
			{
				m_EnemyState = EnemyState.Alarmed;
				m_PlayerWarned = false;
				m_AlarmedState = AlarmedState.HandleNewAlarmedLocation;
				m_NavAgent.updateRotation = false;
				m_NavAgent.updatePosition = false;
				m_NavMoving = true;
				m_NavAgent.ResetPath();
				m_NavAgent.enabled = false;
				m_NavAgent.enabled = true;
				m_NavAgent.CalculatePath(m_AudioDisturbanceLocation, m_NavMeshPath);
			}
			else
			{
				m_EnemyState = EnemyState.TransitionToAlarmed;
				m_PlayerWarned = false;
			}
			m_StateTimer = 60f;
			EnableAimBlending(false, null);
			SwapWeaponHands(true, true);
		}
	}

	public virtual void CancelSupport()
	{
		if (m_AlarmedState == AlarmedState.EnteringCover || m_AlarmedState == AlarmedState.InCover || m_AlarmedState == AlarmedState.AimFromCover)
		{
			m_StateTimer = 5f;
			m_WantToExitCover = true;
		}
		m_WantToPatrol = true;
	}

	public virtual void UpdateAlarmed()
	{
		switch (m_AlarmedState)
		{
		case AlarmedState.PostTransition:
			UpdateAlarmedPostTransition();
			break;
		case AlarmedState.TurnToRun:
			UpdateAlarmedTurnToRun();
			break;
		case AlarmedState.RunToDisturbance:
			UpdateAlarmedRunToDisturbance();
			break;
		case AlarmedState.SearchForSource:
			UpdateAlarmedSearchForSource();
			break;
		case AlarmedState.WaitingForPathToCover:
			UpdateWaitingForPathToCover();
			break;
		case AlarmedState.StandAtTheReady:
			UpdateStandAtTheReady();
			break;
		case AlarmedState.RunToCover:
			UpdateRunToCover();
			break;
		case AlarmedState.EnteringCover:
			UpdateAlarmedEnteringCover();
			break;
		case AlarmedState.InCover:
			UpdateAlarmedInCover();
			break;
		case AlarmedState.AimFromCover:
			UpdateAlarmedAimFromCover();
			break;
		case AlarmedState.ExitingCover:
			UpdateAlarmedExitingCover();
			break;
		case AlarmedState.HandleNewAlarmedLocation:
			UpdateAlarmedHandleNewLocation();
			break;
		}
		m_StateTimer -= Time.deltaTime;
		if (CheckVisualSenses())
		{
			return;
		}
		if (m_WantToExitCover)
		{
			if (m_StateTimer <= 0f && m_TransitionalAnimation == null && m_AlarmedState == AlarmedState.InCover)
			{
				EnableAimBlending(false, null);
				SwapWeaponHands(true, true);
				if (m_CoverStatus == CoverStatus.CrouchingLeft)
				{
					m_TransitionalAnimation = "Exit_CrouchLeft";
				}
				else if (m_CoverStatus == CoverStatus.CrouchingRight)
				{
					m_TransitionalAnimation = "Exit_CrouchRight";
				}
				else if (m_CoverStatus == CoverStatus.StandingLeft)
				{
					m_TransitionalAnimation = "Exit_StandLeft";
				}
				else
				{
					m_TransitionalAnimation = "Exit_StandRight";
				}
				if (m_CBRReload != null)
				{
					m_Animator.Stop(m_CBRReload.name);
				}
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
				m_CoverStatus = CoverStatus.None;
				m_AlarmedState = AlarmedState.ExitingCover;
				m_WantToExitCover = false;
				m_WantToPatrol = !m_WantsToInvestigate;
				AdjustCollider();
				if (m_ChosenCover != null)
				{
					m_LastChosenCover = m_ChosenCover;
					m_ChosenCover.m_Enemy = null;
					m_ChosenCover = null;
				}
			}
		}
		else if (m_WantsToInvestigate)
		{
			if (m_TransitionalAnimation == null)
			{
				m_AlarmedState = AlarmedState.PostTransition;
				m_AudioDisturbanceLocation = m_WantsToInvestigateLocation;
				NavMeshHit hit;
				if (NavMesh.SamplePosition(m_AudioDisturbanceLocation, out hit, 5f, m_NavAgent.walkableMask))
				{
					m_AudioDisturbanceLocation = hit.position;
				}
				EnableAimBlending(false, null);
				m_Animator.CrossFade("Idle_Alert");
				m_NavAgent.Stop(true);
				m_NavAgent.updateRotation = false;
				m_NavAgent.updatePosition = false;
				m_NavMoving = true;
				m_NavAgent.ResetPath();
				m_NavAgent.enabled = false;
				m_NavAgent.enabled = true;
				m_NavAgent.CalculatePath(m_AudioDisturbanceLocation, m_NavMeshPath);
				m_StateTimer = 60f;
				m_WantsToInvestigate = false;
			}
		}
		else if (m_WantToPatrol)
		{
			if (m_TransitionalAnimation == null)
			{
				EnableAimBlending(false, null);
				m_Animator.CrossFade("Idle_Alert");
				ResumePatrol();
				m_WantToPatrol = false;
			}
		}
		else if (m_StateTimer <= 0f && m_TransitionalAnimation == null && Globals.m_AIDirector.AmILeader(this))
		{
			EnableAimBlending(false, null);
			m_Animator.CrossFade("Idle_Alert");
			ResumePatrol();
			Globals.m_AIDirector.EnemyIsNoLongerAlarmed(this);
		}
	}

	private void UpdateAlarmedHandleNewLocation()
	{
		if (m_NavAgent.pathPending)
		{
			return;
		}
		m_SourceDirection = TurnDirection.Forward;
		if (m_NavMeshPath.corners.Length > 1)
		{
			DetermineSourceDirection(m_NavMeshPath.corners[1]);
		}
		string transitionalAnimation = m_TransitionalAnimation;
		if (m_SourceDirection == TurnDirection.BackLeft)
		{
			m_TransitionalAnimation = "Turn_Left180";
		}
		else if (m_SourceDirection == TurnDirection.BackRight)
		{
			m_TransitionalAnimation = "Turn_Right180";
		}
		else if (m_SourceDirection == TurnDirection.Left)
		{
			m_TransitionalAnimation = "Turn_Left90";
		}
		else if (m_SourceDirection == TurnDirection.Right)
		{
			m_TransitionalAnimation = "Turn_Right90";
		}
		else
		{
			m_TransitionalAnimation = null;
		}
		if (m_TransitionalAnimation != null)
		{
			if (transitionalAnimation != null)
			{
				BeginCompoundTransition(transitionalAnimation, m_TransitionalAnimation);
			}
			m_PosedAnimation = m_TransitionalAnimation + "_Pose";
			m_Animator.CrossFade(m_TransitionalAnimation);
		}
		m_NavAgent.Stop(true);
		m_AlarmedState = AlarmedState.TurnToRun;
	}

	public virtual void UpdateAlarmedPostTransition()
	{
		if (!m_NavAgent.pathPending)
		{
			m_SourceDirection = TurnDirection.Forward;
			if (m_NavMeshPath.corners.Length > 1)
			{
				DetermineSourceDirection(m_NavMeshPath.corners[1]);
			}
			if (m_SourceDirection == TurnDirection.BackLeft)
			{
				m_TransitionalAnimation = "Turn_Left180";
			}
			else if (m_SourceDirection == TurnDirection.BackRight)
			{
				m_TransitionalAnimation = "Turn_Right180";
			}
			else if (m_SourceDirection == TurnDirection.Left)
			{
				m_TransitionalAnimation = "Turn_Left90";
			}
			else if (m_SourceDirection == TurnDirection.Right)
			{
				m_TransitionalAnimation = "Turn_Right90";
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
			m_NavAgent.Stop(true);
			m_AlarmedState = AlarmedState.TurnToRun;
		}
	}

	public virtual void UpdateAlarmedTurnToRun()
	{
		if (m_TransitionalAnimation != null && m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			return;
		}
		m_AlarmedState = AlarmedState.RunToDisturbance;
		if (m_TransitionalAnimation != null)
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
		}
		m_Animator.CrossFade("Walk_ForwardGunUp");
		m_PosedAnimation = null;
		m_TransitionalAnimation = null;
		m_NavAgent.speed = m_WalkSpeed;
		m_NavAgent.angularSpeed = m_WalkTurnSpeed;
		m_NavAgent.updateRotation = true;
		m_NavAgent.updatePosition = true;
		m_NavAgent.SetPath(m_NavMeshPath);
		m_NavAgent.Resume();
	}

	public virtual void UpdateAlarmedRunToDisturbance()
	{
		if (m_NavMoving && m_NavAgent.remainingDistance <= 1f && m_NavAgent.remainingDistance != float.PositiveInfinity)
		{
			m_AlarmedState = AlarmedState.SearchForSource;
			m_StateTimer = UnityEngine.Random.Range(m_MinInvestigateTime, m_MaxInvestigateTime);
			m_NavAgent.Stop(true);
			m_NavMoving = false;
			m_NavAgent.updateRotation = false;
			if (m_EnemyType == EnemyType.Ogre)
			{
				m_TransitionalAnimation = null;
				m_PosedAnimation = null;
				m_Animator.CrossFade("Idle_Investigate");
			}
			else
			{
				m_TransitionalAnimation = "Investigate_Nothing";
				m_PosedAnimation = null;
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
		}
	}

	public virtual void UpdateAlarmedSearchForSource()
	{
		if (m_EnemyType == EnemyType.Ogre)
		{
			return;
		}
		if (m_TransitionalAnimation == null)
		{
			m_EventTimer -= Time.deltaTime;
			if (m_EventTimer <= 0f)
			{
				m_TransitionalAnimation = "Investigate_Nothing";
				m_PosedAnimation = null;
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
		}
		else if (!m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_Animator.CrossFade("Idle_Alert");
			m_EventTimer = UnityEngine.Random.Range(4f, 10f);
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
		}
	}

	public virtual void UpdateWaitingForPathToCover()
	{
		if (m_TransitionalAnimation != null && !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_Animator.CrossFade("Idle_Alert");
			m_PosedAnimation = null;
			m_TransitionalAnimation = null;
		}
		if (m_TransitionalAnimation == null)
		{
			if (m_ChosenCover == null)
			{
				m_AlarmedState = AlarmedState.StandAtTheReady;
				EnableAimBlending(true, m_CBRAimDirections);
			}
			else if (!m_NavAgent.pathPending)
			{
				m_AlarmedState = AlarmedState.RunToCover;
				m_NavAgent.speed = m_RunSpeed;
				m_NavAgent.angularSpeed = m_RunTurnSpeed;
				m_NavAgent.SetPath(m_NavMeshPath);
				m_NavAgent.Resume();
				m_NavAgent.updateRotation = false;
				m_NavAgent.updatePosition = true;
				EnableAimBlending(true, m_CBRAimDirections);
			}
		}
	}

	public virtual void UpdateStandAtTheReady()
	{
		if (m_TransitionalAnimation != null && !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_Animator.CrossFade("Idle_Alert");
			m_PosedAnimation = null;
			m_TransitionalAnimation = null;
		}
		DetermineSourceDirection(m_AudioDisturbanceLocation);
		float num = 57.29578f * Mathf.Acos(m_ToSourceDot);
		float num2 = 90f - 57.29578f * Mathf.Acos(m_ToSourceUpDot);
		float hPerc = Mathf.Min(Mathf.Abs(num / ((!(m_ToSourceRightDot >= 0f)) ? m_LeftAimingExtentInDegrees : m_RightAimingExtentInDegrees)), 1f);
		float vPerc = Mathf.Clamp(num2 / ((!(m_ToSourceUpDot >= 0f)) ? m_DownAimingExtentInDegrees : m_UpAimingExtentInDegrees), -1f, 1f);
		TurnWhileStanding(num);
		BlendAiming(hPerc, vPerc);
	}

	public virtual void UpdateRunToCover()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (m_NavAgent.remainingDistance <= 0.5f && m_NavAgent.remainingDistance != float.PositiveInfinity)
		{
			if ((m_ChosenCover.m_Cover.m_CoverType & 5) != 0)
			{
				m_CoverStatus = CoverStatus.CrouchingLeft;
				m_TransitionalAnimation = "Enter_CrouchLeft";
			}
			else if ((m_ChosenCover.m_Cover.m_CoverType & 6) != 0)
			{
				m_CoverStatus = CoverStatus.CrouchingRight;
				m_TransitionalAnimation = "Enter_CrouchRight";
			}
			else if ((m_ChosenCover.m_Cover.m_CoverType & 8) != 0)
			{
				m_CoverStatus = CoverStatus.StandingLeft;
				m_TransitionalAnimation = "Enter_StandLeft";
			}
			else
			{
				m_CoverStatus = CoverStatus.StandingRight;
				m_TransitionalAnimation = "Enter_StandRight";
			}
			EnableAimBlending(false, null);
			m_AlarmedState = AlarmedState.EnteringCover;
			m_Animator.CrossFade(m_TransitionalAnimation);
			if (m_NavMoving)
			{
				m_NavAgent.Stop(true);
				m_NavMoving = false;
				m_NavAgent.updateRotation = false;
			}
			return;
		}
		Vector3 lhs = ((!(m_NavAgent.remainingDistance <= 1.5f) || m_NavAgent.remainingDistance == float.PositiveInfinity) ? (m_AudioDisturbanceLocation - m_RigMotion.transform.position).normalized : m_ChosenCover.m_Cover.transform.forward);
		Vector3 velocity = m_NavAgent.velocity;
		velocity.y = 0f;
		velocity.Normalize();
		float f = Vector3.Dot(lhs, velocity);
		float num = Vector3.Dot(lhs, new Vector3(velocity.z, 0f, 0f - velocity.x));
		float num2 = 57.29578f * Mathf.Acos(f);
		Vector3 forward;
		if (num >= 0f)
		{
			if (num2 <= m_RightAimingExtentInDegrees)
			{
				forward = velocity;
				m_Animator.CrossFade("Run_ForwardAiming");
			}
			else if (num2 >= 180f - m_RightAimingExtentInDegrees)
			{
				forward = -velocity;
				m_Animator.CrossFade("Run_BackAiming");
			}
			else
			{
				forward = new Vector3(velocity.z, 0f, 0f - velocity.x);
				m_Animator.CrossFade("Run_LeftStrafeAiming");
			}
		}
		else if (num2 <= m_LeftAimingExtentInDegrees)
		{
			forward = velocity;
			m_Animator.CrossFade("Run_ForwardAiming");
		}
		else if (num2 >= 180f - m_LeftAimingExtentInDegrees)
		{
			forward = -velocity;
			m_Animator.CrossFade("Run_BackAiming");
		}
		else
		{
			forward = new Vector3(0f - velocity.z, 0f, velocity.x);
			m_Animator.CrossFade("Run_RightStrafeAiming");
		}
		Quaternion to = Quaternion.LookRotation(forward);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, 180f * Time.deltaTime);
		DetermineSourceDirection(m_AudioDisturbanceLocation);
		num2 = 57.29578f * Mathf.Acos(m_ToSourceDot);
		float num3 = 90f - 57.29578f * Mathf.Acos(m_ToSourceUpDot);
		float hPerc = Mathf.Min(Mathf.Abs(num2 / ((!(m_ToSourceRightDot >= 0f)) ? m_LeftAimingExtentInDegrees : m_RightAimingExtentInDegrees)), 1f);
		float vPerc = Mathf.Clamp(num3 / ((!(m_ToSourceUpDot >= 0f)) ? m_DownAimingExtentInDegrees : m_UpAimingExtentInDegrees), -1f, 1f);
		BlendAiming(hPerc, vPerc);
	}

	public virtual void UpdateAlarmedEnteringCover()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (m_TransitionalAnimation != null && !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			if (m_CoverStatus == CoverStatus.CrouchingLeft)
			{
				m_Animator.CrossFade("Idle_CrouchLeft");
			}
			else if (m_CoverStatus == CoverStatus.CrouchingRight)
			{
				m_Animator.CrossFade("Idle_CrouchRight");
			}
			else if (m_CoverStatus == CoverStatus.StandingLeft)
			{
				m_Animator.CrossFade("Idle_StandLeft");
			}
			else
			{
				m_Animator.CrossFade("Idle_StandRight");
			}
			m_PosedAnimation = null;
			m_TransitionalAnimation = null;
		}
		Vector3 vector = m_ChosenCover.m_Cover.transform.position - base.transform.position;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		float num = m_RunSpeed * Time.deltaTime;
		bool flag = false;
		if (magnitude <= num)
		{
			base.transform.position = m_ChosenCover.m_Cover.transform.position;
			flag = true;
		}
		else
		{
			base.transform.position += vector.normalized * num;
		}
		Quaternion quaternion = Quaternion.LookRotation(m_ChosenCover.m_Cover.transform.forward);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, 180f * Time.deltaTime);
		if (flag && Quaternion.Angle(base.transform.rotation, quaternion) <= 5f)
		{
			base.transform.rotation = quaternion;
			m_AlarmedState = AlarmedState.InCover;
			m_CoverFireStatus = CoverFireStatus.None;
			AdjustCollider();
			if (m_CoverStatus == CoverStatus.CrouchingLeft || m_CoverStatus == CoverStatus.StandingLeft)
			{
				SwapWeaponHands(false, true);
			}
		}
	}

	public virtual void UpdateAlarmedInCover()
	{
		if (m_CoverFireStatus == CoverFireStatus.TransitionOut)
		{
			m_StateTimer = 0f;
			return;
		}
		m_AlarmedState = AlarmedState.AimFromCover;
		m_CoverFireStatus = CoverFireStatus.TransitionIn;
		if (m_CoverStatus == CoverStatus.CrouchingLeft)
		{
			float num = -1f;
			if ((m_ChosenCover.m_Cover.m_CoverType & 4) != 0)
			{
				num = 0.5f;
			}
			if (UnityEngine.Random.value <= num)
			{
				m_CoverAimingIndex = 1;
				m_CoverFireDirection = CoverFireDirection.Over;
				m_CoverTransitionInAnimation = "Aim_CrouchLeftOver_FireIn";
			}
			else
			{
				m_CoverAimingIndex = 0;
				m_CoverFireDirection = CoverFireDirection.Side;
				m_CoverTransitionInAnimation = "Aim_CrouchLeft_FireIn";
			}
		}
		else if (m_CoverStatus == CoverStatus.CrouchingRight)
		{
			float num2 = -1f;
			if ((m_ChosenCover.m_Cover.m_CoverType & 4) != 0)
			{
				num2 = 0.5f;
			}
			if (UnityEngine.Random.value <= num2)
			{
				m_CoverAimingIndex = 3;
				m_CoverFireDirection = CoverFireDirection.Over;
				m_CoverTransitionInAnimation = "Aim_CrouchRightOver_FireIn";
			}
			else
			{
				m_CoverAimingIndex = 2;
				m_CoverFireDirection = CoverFireDirection.Side;
				m_CoverTransitionInAnimation = "Aim_CrouchRight_FireIn";
			}
		}
		else if (m_CoverStatus == CoverStatus.StandingLeft)
		{
			m_CoverAimingIndex = 4;
			m_CoverFireDirection = CoverFireDirection.Side;
			m_CoverTransitionInAnimation = "Aim_StandLeft_FireIn";
		}
		else
		{
			m_CoverAimingIndex = 5;
			m_CoverFireDirection = CoverFireDirection.Side;
			m_CoverTransitionInAnimation = "Aim_StandRight_FireIn";
		}
		m_Animator.Play(m_CoverTransitionInAnimation);
		AdjustCollider();
	}

	public virtual void UpdateAlarmedAimFromCover()
	{
		if (m_CoverFireStatus == CoverFireStatus.TransitionIn)
		{
			if (!m_Animator.IsPlaying(m_CoverTransitionInAnimation))
			{
				EnableAimBlending(true, m_InCoverAimDirections[m_CoverAimingIndex]);
				m_CoverFireStatus = CoverFireStatus.WaitToFire;
				m_EventTimer = UnityEngine.Random.Range(2f, 3f);
			}
		}
		else if (m_CoverFireStatus == CoverFireStatus.WaitToFire && (m_WantToExitCover || m_WantsToInvestigate))
		{
			EnableAimBlending(false, null);
			if (m_CoverAimingIndex == 0)
			{
				m_Animator.Play("Aim_CrouchLeft_FireOut");
			}
			else if (m_CoverAimingIndex == 1)
			{
				m_Animator.Play("Aim_CrouchLeftOver_FireOut");
			}
			else if (m_CoverAimingIndex == 2)
			{
				m_Animator.Play("Aim_CrouchRight_FireOut");
			}
			else if (m_CoverAimingIndex == 3)
			{
				m_Animator.Play("Aim_CrouchRightOver_FireOut");
			}
			else if (m_CoverAimingIndex == 4)
			{
				m_Animator.Play("Aim_StandLeft_FireOut");
			}
			else
			{
				m_Animator.Play("Aim_StandRight_FireOut");
			}
			if (m_CoverStatus == CoverStatus.CrouchingLeft)
			{
				m_Animator.CrossFadeQueued("Idle_CrouchLeft");
			}
			else if (m_CoverStatus == CoverStatus.CrouchingRight)
			{
				m_Animator.CrossFadeQueued("Idle_CrouchRight");
			}
			else if (m_CoverStatus == CoverStatus.StandingLeft)
			{
				m_Animator.CrossFadeQueued("Idle_StandLeft");
			}
			else
			{
				m_Animator.CrossFadeQueued("Idle_StandRight");
			}
			m_AlarmedState = AlarmedState.InCover;
			m_CoverFireStatus = CoverFireStatus.TransitionOut;
			m_EventTimer = UnityEngine.Random.Range(5f, 10f);
			AdjustCollider();
		}
		DetermineSourceDirection(Globals.m_AIDirector.m_LastKnownPlayerPosition);
		float num = 57.29578f * Mathf.Acos(m_ToSourceDot);
		float num2 = 90f - 57.29578f * Mathf.Acos(m_ToSourceUpDot);
		int num3 = 0;
		num3 = ((m_CoverStatus == CoverStatus.CrouchingLeft) ? ((m_CoverFireDirection != CoverFireDirection.Side) ? 1 : 0) : ((m_CoverStatus == CoverStatus.CrouchingRight) ? ((m_CoverFireDirection != CoverFireDirection.Side) ? 3 : 2) : ((m_CoverStatus != CoverStatus.StandingLeft) ? 5 : 4)));
		float hPerc = Mathf.Min(Mathf.Abs(num / ((!(m_ToSourceRightDot >= 0f)) ? m_InCoverAimingExtentInDegrees[num3].y : m_InCoverAimingExtentInDegrees[num3].x)), 1f);
		float vPerc = Mathf.Clamp(num2 / ((!(m_ToSourceUpDot >= 0f)) ? m_InCoverAimingExtentInDegrees[num3].w : m_InCoverAimingExtentInDegrees[num3].z), -1f, 1f);
		BlendAiming(hPerc, vPerc);
	}

	public virtual void UpdateAlarmedExitingCover()
	{
		if (m_TransitionalAnimation == null || !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_Animator.CrossFade("Idle_Alert");
			m_PosedAnimation = null;
			m_TransitionalAnimation = null;
		}
	}

	public virtual void EnterCombat(bool FirstSight = false, DamageType FromDamageType = DamageType.None)
	{
		if (m_NavMoving)
		{
			m_NavAgent.Stop(true);
			m_NavMoving = false;
			m_NavAgent.updateRotation = false;
		}
		m_EnemyState = EnemyState.Combat;
		m_PlayerWarned = false;
		m_StateTimer = m_PlayerLostDelayToInvestigate;
		m_MoveTimer = -1f;
		m_LostPlayer = false;
		if (FirstSight)
		{
			Globals.m_AIDirector.EnemyEnteredCombat(this, Globals.m_PlayerController.transform.position);
		}
		if (FromDamageType != DamageType.None && m_StaggerCooldown <= 0f && UnityEngine.Random.value <= 0.2f && (m_CBRReload == null || !m_Animator.IsPlaying(m_CBRReload.name)))
		{
			Stagger();
			return;
		}
		bool flag = FromDamageType != DamageType.None || UnityEngine.Random.value <= 0.7f;
		Vector3 playerToEnemy = base.transform.position - Globals.m_PlayerController.transform.position;
		if (flag && playerToEnemy.sqrMagnitude <= 70f)
		{
			flag = false;
		}
		if (flag)
		{
			FindCover(playerToEnemy);
		}
		DetermineSourceDirection(Globals.m_PlayerController.transform.position);
		if (FirstSight)
		{
			PlayVO("Play_VO_Alarmed_Visual", 75);
			if (m_EnemyType != EnemyType.Ogre || m_EnemyState == EnemyState.Patrol || m_EnemyState == EnemyState.WarningPlayer)
			{
				PlayDisturbance(true, true, m_SourceDirection);
			}
			else
			{
				m_Animator.CrossFade("Idle_Alert");
			}
		}
		else
		{
			string transitionalAnimation = m_TransitionalAnimation;
			if (m_SourceDirection == TurnDirection.BackLeft)
			{
				m_TransitionalAnimation = "Turn_Left180";
			}
			else if (m_SourceDirection == TurnDirection.BackRight)
			{
				m_TransitionalAnimation = "Turn_Right180";
			}
			else if (m_SourceDirection == TurnDirection.Left)
			{
				m_TransitionalAnimation = "Turn_Left90";
			}
			else if (m_SourceDirection == TurnDirection.Right)
			{
				m_TransitionalAnimation = "Turn_Right90";
			}
			else
			{
				m_TransitionalAnimation = null;
			}
			if (m_TransitionalAnimation != null)
			{
				if (transitionalAnimation != null)
				{
					BeginCompoundTransition(transitionalAnimation, m_TransitionalAnimation);
				}
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
			else
			{
				if (transitionalAnimation != null)
				{
					BeginCompoundTransition(transitionalAnimation, "Idle_Alert");
				}
				m_Animator.CrossFade("Idle_Alert");
			}
		}
		m_CombatState = CombatState.WaitingForCover;
		m_LastChosenCover = null;
		if (m_ChosenCover != null)
		{
			Vector3 position = m_ChosenCover.m_Cover.transform.position;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(position, out hit, 0.5f, m_NavAgent.walkableMask))
			{
				position = hit.position;
			}
			m_NavAgent.updateRotation = false;
			m_NavAgent.updatePosition = false;
			m_NavMoving = true;
			m_NavAgent.ResetPath();
			m_NavAgent.enabled = false;
			m_NavAgent.enabled = true;
			m_NavAgent.CalculatePath(position, m_NavMeshPath);
		}
		EnableAimBlending(true, m_CBRAimDirections);
		Globals.m_AIDirector.UpdatePlayerKnownPosition();
	}

	public virtual void RefreshPlayersKnownLocation()
	{
		Globals.m_AIDirector.UpdatePlayerKnownPosition();
	}

	public virtual void UpdateCombat()
	{
		CalculateFireDirection();
		float stateTimer = m_StateTimer;
		m_StateTimer -= Time.deltaTime;
		CheckCombatSenses();
		if (m_MoveTimer > 0f)
		{
			m_MoveTimer -= Time.deltaTime;
			if (m_MoveTimer <= 0f)
			{
				m_WantsNewCover = true;
			}
		}
		if (m_StateTimer <= 0f && stateTimer > 0f)
		{
			m_LostPlayer = true;
			if (Globals.m_AIDirector.EnemyLostTrackOfPlayer(this))
			{
				return;
			}
		}
		switch (m_CombatState)
		{
		case CombatState.WaitingForCover:
			UpdateCombatWaitingForCover();
			break;
		case CombatState.ChargePlayer:
			UpdateCombatChargePlayer();
			break;
		case CombatState.StandAndFire:
			UpdateStandAndFire();
			break;
		case CombatState.RunToCover:
			UpdateCombatRunToCover();
			break;
		case CombatState.StandAtCover:
			UpdateCombatStandAtCover();
			break;
		case CombatState.EnterCover:
			UpdateCombatEnterCover();
			break;
		case CombatState.InCover:
			UpdateCombatInCover();
			break;
		case CombatState.CoverFiring:
			UpdateCombatCoverFiring();
			break;
		case CombatState.ExitingCover:
			UpdateCombatExitingCover();
			break;
		case CombatState.Staggering:
			UpdateCombatStaggering();
			break;
		}
		if (m_StateTimer <= m_PlayerLostDelayToStopFiring && m_CooldownTimer <= 0f && !m_WantsNewCover && m_MoveTimer <= 0f && Globals.m_AIDirector.SquadmateSeesPlayer(this))
		{
			m_MoveTimer = UnityEngine.Random.Range(1f, 3f);
		}
	}

	public virtual void UpdateCombatStaggering()
	{
		if (m_TransitionalAnimation != null && m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			return;
		}
		bool flag = UnityEngine.Random.value <= 0.7f;
		Vector3 playerToEnemy = base.transform.position - Globals.m_PlayerController.transform.position;
		if (flag && playerToEnemy.sqrMagnitude <= 70f)
		{
			flag = false;
		}
		if (flag)
		{
			FindCover(playerToEnemy);
		}
		m_Animator.CrossFade("Idle_Alert");
		m_CombatState = CombatState.WaitingForCover;
		m_LastChosenCover = null;
		if (m_ChosenCover != null)
		{
			Vector3 position = m_ChosenCover.m_Cover.transform.position;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(position, out hit, 0.5f, m_NavAgent.walkableMask))
			{
				position = hit.position;
			}
			m_NavAgent.updateRotation = false;
			m_NavAgent.updatePosition = false;
			m_NavMoving = true;
			m_NavAgent.ResetPath();
			m_NavAgent.enabled = false;
			m_NavAgent.enabled = true;
			m_NavAgent.CalculatePath(position, m_NavMeshPath);
		}
		EnableAimBlending(true, m_CBRAimDirections);
		Globals.m_AIDirector.UpdatePlayerKnownPosition();
	}

	public virtual void UpdateCombatWaitingForCover()
	{
		if (m_TransitionalAnimation != null && !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_Animator.CrossFade("Idle_Alert");
			m_PosedAnimation = null;
			m_TransitionalAnimation = null;
		}
		if (m_TransitionalAnimation == null)
		{
			if (m_ChosenCover == null || (!m_NavAgent.pathPending && m_NavMeshPath.status == NavMeshPathStatus.PathInvalid))
			{
				m_CombatState = CombatState.StandAndFire;
				m_EventTimer = UnityEngine.Random.Range(20f, 30f);
				m_WaitingForPathToClear = false;
				EnableAimBlending(true, m_CBRAimDirections);
			}
			else if (!m_NavAgent.pathPending)
			{
				m_CombatState = CombatState.RunToCover;
				m_NavAgent.speed = m_RunSpeed;
				m_NavAgent.angularSpeed = m_RunTurnSpeed;
				m_NavAgent.SetPath(m_NavMeshPath);
				m_NavAgent.Resume();
				m_NavAgent.updateRotation = false;
				m_NavAgent.updatePosition = true;
				EnableAimBlending(true, m_CBRAimDirections);
			}
		}
	}

	public virtual void UpdateStandAndFire()
	{
		if (m_TransitionalAnimation != null && !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_Animator.CrossFade("Idle_Alert");
			m_PosedAnimation = null;
			m_TransitionalAnimation = null;
		}
		DetermineSourceDirection(Globals.m_AIDirector.m_LastKnownPlayerPosition);
		float num = 57.29578f * Mathf.Acos(m_ToSourceDot);
		float num2 = 90f - 57.29578f * Mathf.Acos(m_ToSourceUpDot);
		if (Globals.m_PlayerController.m_Stance == PlayerController.Stance.Crouch)
		{
			num2 -= Mathf.Lerp(m_AngleWhenTargetCrouched, 0f, Mathf.InverseLerp(0f, 17f, m_ToSource.sqrMagnitude));
		}
		float hPerc = Mathf.Min(Mathf.Abs(num / ((!(m_ToSourceRightDot >= 0f)) ? m_LeftAimingExtentInDegrees : m_RightAimingExtentInDegrees)), 1f);
		float vPerc = Mathf.Clamp(num2 / ((!(m_ToSourceUpDot >= 0f)) ? m_DownAimingExtentInDegrees : m_UpAimingExtentInDegrees), -1f, 1f);
		TurnWhileStanding(num);
		BlendAiming(hPerc, vPerc);
		UpdateBursting();
		if (m_WaitingForPathToClear)
		{
			m_ContinuePathTimer -= Time.deltaTime;
			if (m_ContinuePathTimer <= 0f)
			{
				m_WaitingForPathToClear = false;
				if (!SquadmateInTheWay(m_PreBlockVelocity, 3f))
				{
					m_CombatState = CombatState.ChargePlayer;
					m_NavAgent.updateRotation = false;
					m_NavAgent.updatePosition = true;
					m_NavMoving = true;
					m_NavAgent.Resume();
				}
			}
			return;
		}
		m_EventTimer -= Time.deltaTime;
		if (!m_WantsNewCover && !m_WantsToEnterCover && !(m_EventTimer <= 0f))
		{
			return;
		}
		m_WantsToEnterCover = false;
		m_EventTimer = UnityEngine.Random.Range(5f, 12f);
		Vector3 playerToEnemy = base.transform.position - Globals.m_PlayerController.transform.position;
		FindCover(playerToEnemy);
		if (m_ChosenCover != null)
		{
			Vector3 position = m_ChosenCover.m_Cover.transform.position;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(position, out hit, 0.5f, m_NavAgent.walkableMask))
			{
				position = hit.position;
			}
			m_CombatState = CombatState.WaitingForCover;
			m_NavAgent.updateRotation = false;
			m_NavAgent.updatePosition = false;
			m_NavMoving = true;
			m_NavAgent.ResetPath();
			m_NavAgent.enabled = false;
			m_NavAgent.enabled = true;
			m_NavAgent.CalculatePath(position, m_NavMeshPath);
		}
		else if (m_WantsNewCover)
		{
			if (SquadmateInTheWay(base.transform.forward, 3f))
			{
				m_WaitingForPathToClear = true;
				m_PreBlockVelocity = base.transform.forward;
				m_ContinuePathTimer = 4f;
			}
			else
			{
				m_CombatState = CombatState.ChargePlayer;
				Vector3 vector = Globals.m_AIDirector.m_LastKnownPlayerPosition;
				NavMeshHit hit2;
				if (NavMesh.SamplePosition(vector, out hit2, 5f, m_NavAgent.walkableMask))
				{
					vector = hit2.position;
				}
				m_NavAgent.updateRotation = false;
				m_NavAgent.updatePosition = false;
				m_NavMoving = false;
				m_NavAgent.ResetPath();
				m_NavAgent.enabled = false;
				m_NavAgent.enabled = true;
				m_NavAgent.CalculatePath(vector, m_NavMeshPath);
			}
		}
		m_CooldownTimer = UnityEngine.Random.Range(5f, 8f);
		m_WantsNewCover = false;
	}

	public virtual void UpdateCombatChargePlayer()
	{
		if (!m_NavMoving)
		{
			if (!m_NavAgent.pathPending)
			{
				m_NavAgent.speed = m_RunSpeed;
				m_NavAgent.angularSpeed = m_RunTurnSpeed;
				m_NavAgent.SetPath(m_NavMeshPath);
				m_NavAgent.Resume();
				m_NavAgent.updateRotation = false;
				m_NavAgent.updatePosition = true;
				m_NavMoving = true;
			}
		}
		else
		{
			if (Time.timeScale == 0f)
			{
				return;
			}
			Vector3 normalized = (Globals.m_AIDirector.m_LastKnownPlayerPosition - m_RigMotion.transform.position).normalized;
			Vector3 velocity = m_NavAgent.velocity;
			velocity.y = 0f;
			velocity.Normalize();
			float f = Vector3.Dot(normalized, velocity);
			float num = Vector3.Dot(normalized, new Vector3(velocity.z, 0f, 0f - velocity.x));
			float num2 = 57.29578f * Mathf.Acos(f);
			Vector3 forward;
			if (num >= 0f)
			{
				if (num2 <= m_RightAimingExtentInDegrees)
				{
					forward = velocity;
					m_Animator.CrossFade("Run_ForwardAiming");
				}
				else if (num2 >= 180f - m_RightAimingExtentInDegrees)
				{
					forward = -velocity;
					m_Animator.CrossFade("Run_BackAiming");
				}
				else
				{
					forward = new Vector3(velocity.z, 0f, 0f - velocity.x);
					m_Animator.CrossFade("Run_LeftStrafeAiming");
				}
			}
			else if (num2 <= m_LeftAimingExtentInDegrees)
			{
				forward = velocity;
				m_Animator.CrossFade("Run_ForwardAiming");
			}
			else if (num2 >= 180f - m_LeftAimingExtentInDegrees)
			{
				forward = -velocity;
				m_Animator.CrossFade("Run_BackAiming");
			}
			else
			{
				forward = new Vector3(0f - velocity.z, 0f, velocity.x);
				m_Animator.CrossFade("Run_RightStrafeAiming");
			}
			Quaternion to = Quaternion.LookRotation(forward);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, 180f * Time.deltaTime);
			DetermineSourceDirection(Globals.m_AIDirector.m_LastKnownPlayerPosition);
			num2 = 57.29578f * Mathf.Acos(m_ToSourceDot);
			float num3 = 90f - 57.29578f * Mathf.Acos(m_ToSourceUpDot);
			if (Globals.m_PlayerController.m_Stance == PlayerController.Stance.Crouch)
			{
				num3 -= Mathf.Lerp(m_AngleWhenTargetCrouched, 0f, Mathf.InverseLerp(0f, 17f, m_ToSource.sqrMagnitude));
			}
			float hPerc = Mathf.Min(Mathf.Abs(num2 / ((!(m_ToSourceRightDot >= 0f)) ? m_LeftAimingExtentInDegrees : m_RightAimingExtentInDegrees)), 1f);
			float vPerc = Mathf.Clamp(num3 / ((!(m_ToSourceUpDot >= 0f)) ? m_DownAimingExtentInDegrees : m_UpAimingExtentInDegrees), -1f, 1f);
			BlendAiming(hPerc, vPerc);
			UpdateBursting();
			if ((m_NavAgent.remainingDistance <= 1f && m_NavAgent.remainingDistance != float.PositiveInfinity) || (Globals.m_PlayerController.transform.position - base.transform.position).sqrMagnitude <= 5f)
			{
				m_NavAgent.Stop(true);
				m_NavMoving = false;
				m_NavAgent.updateRotation = false;
				m_Animator.CrossFade("Idle_Alert");
				m_CombatState = CombatState.StandAndFire;
				m_WaitingForPathToClear = false;
				m_CooldownTimer = 1f;
				m_EventTimer = UnityEngine.Random.Range(10f, 20f);
			}
			else if (SquadmateInTheWay(velocity, 2f))
			{
				m_NavAgent.Stop(true);
				m_NavMoving = false;
				m_NavAgent.updateRotation = false;
				m_Animator.CrossFade("Idle_Alert");
				m_CombatState = CombatState.StandAndFire;
				m_WaitingForPathToClear = true;
				m_PreBlockVelocity = velocity;
				m_ContinuePathTimer = 4f;
				m_CooldownTimer = 1f;
				m_EventTimer = 6f;
			}
		}
	}

	public virtual void UpdateCombatRunToCover()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (m_NavAgent.remainingDistance <= 0.5f && m_NavAgent.remainingDistance != float.PositiveInfinity)
		{
			if (UnityEngine.Random.value <= 0.5f)
			{
				m_CombatState = CombatState.StandAtCover;
				m_WantsToEnterCover = false;
				m_Animator.CrossFade("Idle_Alert");
				m_EventTimer = UnityEngine.Random.Range(10f, 20f);
			}
			else
			{
				m_CoverStatus = DetermineChosenCoverType();
				if (m_CoverStatus == CoverStatus.CrouchingLeft)
				{
					m_TransitionalAnimation = "Enter_CrouchLeft";
				}
				else if (m_CoverStatus == CoverStatus.CrouchingRight)
				{
					m_TransitionalAnimation = "Enter_CrouchRight";
				}
				else if (m_CoverStatus == CoverStatus.StandingLeft)
				{
					m_TransitionalAnimation = "Enter_StandLeft";
				}
				else
				{
					m_TransitionalAnimation = "Enter_StandRight";
				}
				if (m_Bursting)
				{
					StopBurstFire();
				}
				EnableAimBlending(false, null);
				m_CombatState = CombatState.EnterCover;
				if (m_CBRReload != null)
				{
					m_Animator.Stop(m_CBRReload.name);
				}
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
			if (m_NavMoving)
			{
				m_NavAgent.Stop(true);
				m_NavMoving = false;
				m_NavAgent.updateRotation = false;
			}
			return;
		}
		Vector3 lhs = ((!(m_NavAgent.remainingDistance <= 1.5f) || m_NavAgent.remainingDistance == float.PositiveInfinity) ? (Globals.m_AIDirector.m_LastKnownPlayerPosition - m_RigMotion.transform.position).normalized : m_ChosenCover.m_Cover.transform.forward);
		Vector3 velocity = m_NavAgent.velocity;
		velocity.y = 0f;
		velocity.Normalize();
		float f = Vector3.Dot(lhs, velocity);
		float num = Vector3.Dot(lhs, new Vector3(velocity.z, 0f, 0f - velocity.x));
		float num2 = 57.29578f * Mathf.Acos(f);
		Vector3 forward;
		if (num >= 0f)
		{
			if (num2 <= m_RightAimingExtentInDegrees)
			{
				forward = velocity;
				m_Animator.CrossFade("Run_ForwardAiming");
			}
			else if (num2 >= 180f - m_RightAimingExtentInDegrees)
			{
				forward = -velocity;
				m_Animator.CrossFade("Run_BackAiming");
			}
			else
			{
				forward = new Vector3(velocity.z, 0f, 0f - velocity.x);
				m_Animator.CrossFade("Run_LeftStrafeAiming");
			}
		}
		else if (num2 <= m_LeftAimingExtentInDegrees)
		{
			forward = velocity;
			m_Animator.CrossFade("Run_ForwardAiming");
		}
		else if (num2 >= 180f - m_LeftAimingExtentInDegrees)
		{
			forward = -velocity;
			m_Animator.CrossFade("Run_BackAiming");
		}
		else
		{
			forward = new Vector3(0f - velocity.z, 0f, velocity.x);
			m_Animator.CrossFade("Run_RightStrafeAiming");
		}
		Quaternion to = Quaternion.LookRotation(forward);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, 180f * Time.deltaTime);
		DetermineSourceDirection(Globals.m_AIDirector.m_LastKnownPlayerPosition);
		num2 = 57.29578f * Mathf.Acos(m_ToSourceDot);
		float num3 = 90f - 57.29578f * Mathf.Acos(m_ToSourceUpDot);
		if (Globals.m_PlayerController.m_Stance == PlayerController.Stance.Crouch)
		{
			num3 -= Mathf.Lerp(m_AngleWhenTargetCrouched, 0f, Mathf.InverseLerp(0f, 17f, m_ToSource.sqrMagnitude));
		}
		float hPerc = Mathf.Min(Mathf.Abs(num2 / ((!(m_ToSourceRightDot >= 0f)) ? m_LeftAimingExtentInDegrees : m_RightAimingExtentInDegrees)), 1f);
		float vPerc = Mathf.Clamp(num3 / ((!(m_ToSourceUpDot >= 0f)) ? m_DownAimingExtentInDegrees : m_UpAimingExtentInDegrees), -1f, 1f);
		BlendAiming(hPerc, vPerc);
		UpdateBursting();
		if ((Globals.m_PlayerController.transform.position - base.transform.position).sqrMagnitude <= 8f)
		{
			m_NavAgent.Stop(true);
			m_NavMoving = false;
			m_NavAgent.updateRotation = false;
			m_Animator.CrossFade("Idle_Alert");
			m_CombatState = CombatState.StandAndFire;
			m_EventTimer = UnityEngine.Random.Range(10f, 20f);
			m_WaitingForPathToClear = false;
			if (m_ChosenCover != null)
			{
				m_LastChosenCover = m_ChosenCover;
				m_ChosenCover.m_Enemy = null;
				m_ChosenCover = null;
			}
		}
	}

	public virtual void UpdateCombatStandAtCover()
	{
		DetermineSourceDirection(Globals.m_AIDirector.m_LastKnownPlayerPosition);
		float num = 57.29578f * Mathf.Acos(m_ToSourceDot);
		float num2 = 90f - 57.29578f * Mathf.Acos(m_ToSourceUpDot);
		if (Globals.m_PlayerController.m_Stance == PlayerController.Stance.Crouch)
		{
			num2 -= Mathf.Lerp(m_AngleWhenTargetCrouched, 0f, Mathf.InverseLerp(0f, 17f, m_ToSource.sqrMagnitude));
		}
		float hPerc = Mathf.Min(Mathf.Abs(num / ((!(m_ToSourceRightDot >= 0f)) ? m_LeftAimingExtentInDegrees : m_RightAimingExtentInDegrees)), 1f);
		float vPerc = Mathf.Clamp(num2 / ((!(m_ToSourceUpDot >= 0f)) ? m_DownAimingExtentInDegrees : m_UpAimingExtentInDegrees), -1f, 1f);
		bool flag = TurnWhileStanding(num);
		BlendAiming(hPerc, vPerc);
		UpdateBursting();
		if (flag)
		{
			m_CombatState = CombatState.StandAndFire;
			m_EventTimer = UnityEngine.Random.Range(10f, 20f);
			m_WaitingForPathToClear = false;
			m_WantToExitCover = false;
			if (m_ChosenCover != null)
			{
				m_LastChosenCover = m_ChosenCover;
				m_ChosenCover.m_Enemy = null;
				m_ChosenCover = null;
			}
			return;
		}
		m_EventTimer -= Time.deltaTime;
		if (m_WantsToEnterCover || m_EventTimer <= 0f)
		{
			m_CoverStatus = DetermineChosenCoverType();
			if (m_CoverStatus == CoverStatus.CrouchingLeft)
			{
				m_TransitionalAnimation = "Enter_CrouchLeft";
			}
			else if (m_CoverStatus == CoverStatus.CrouchingRight)
			{
				m_TransitionalAnimation = "Enter_CrouchRight";
			}
			else if (m_CoverStatus == CoverStatus.StandingLeft)
			{
				m_TransitionalAnimation = "Enter_StandLeft";
			}
			else
			{
				m_TransitionalAnimation = "Enter_StandRight";
			}
			if (m_Bursting)
			{
				StopBurstFire();
			}
			EnableAimBlending(false, null);
			m_CombatState = CombatState.EnterCover;
			m_WantsToEnterCover = false;
			if (m_CBRReload != null)
			{
				m_Animator.Stop(m_CBRReload.name);
			}
			m_Animator.CrossFade(m_TransitionalAnimation);
		}
	}

	public virtual void UpdateCombatEnterCover()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (m_TransitionalAnimation != null && !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			if (m_CoverStatus == CoverStatus.CrouchingLeft)
			{
				m_Animator.CrossFade("Idle_CrouchLeft");
			}
			else if (m_CoverStatus == CoverStatus.CrouchingRight)
			{
				m_Animator.CrossFade("Idle_CrouchRight");
			}
			else if (m_CoverStatus == CoverStatus.StandingLeft)
			{
				m_Animator.CrossFade("Idle_StandLeft");
			}
			else
			{
				m_Animator.CrossFade("Idle_StandRight");
			}
			m_PosedAnimation = null;
			m_TransitionalAnimation = null;
		}
		Vector3 vector = m_ChosenCover.m_Cover.transform.position - base.transform.position;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		float num = m_RunSpeed * Time.deltaTime;
		bool flag = false;
		if (magnitude <= num)
		{
			base.transform.position = m_ChosenCover.m_Cover.transform.position;
			flag = true;
		}
		else
		{
			base.transform.position += vector.normalized * num;
		}
		Quaternion quaternion = Quaternion.LookRotation(m_ChosenCover.m_Cover.transform.forward);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, 180f * Time.deltaTime);
		if (flag && Quaternion.Angle(base.transform.rotation, quaternion) <= 5f)
		{
			base.transform.rotation = quaternion;
			m_CombatState = CombatState.InCover;
			m_CoverFireStatus = CoverFireStatus.None;
			m_EventTimer = UnityEngine.Random.Range(3f, 8f);
			AdjustCollider();
			m_WantToExitCover = false;
			if (m_CoverStatus == CoverStatus.CrouchingLeft || m_CoverStatus == CoverStatus.StandingLeft)
			{
				SwapWeaponHands(false, true);
			}
		}
	}

	public virtual void UpdateCombatInCover()
	{
		Vector3 vector = Globals.m_PlayerController.transform.position - base.transform.position;
		if (m_WantsNewCover || m_WantToExitCover || vector.sqrMagnitude <= 8f || Vector3.Dot(Vector3.Normalize(new Vector3(vector.x, 0f, vector.z)), base.transform.forward) <= 0f)
		{
			EnableAimBlending(false, null);
			SwapWeaponHands(true, true);
			if (m_CoverStatus == CoverStatus.CrouchingLeft)
			{
				m_TransitionalAnimation = "Exit_CrouchLeft";
			}
			else if (m_CoverStatus == CoverStatus.CrouchingRight)
			{
				m_TransitionalAnimation = "Exit_CrouchRight";
			}
			else if (m_CoverStatus == CoverStatus.StandingLeft)
			{
				m_TransitionalAnimation = "Exit_StandLeft";
			}
			else
			{
				m_TransitionalAnimation = "Exit_StandRight";
			}
			if (m_CBRReload != null)
			{
				m_Animator.Stop(m_CBRReload.name);
			}
			m_PosedAnimation = m_TransitionalAnimation + "_Pose";
			m_Animator.CrossFade(m_TransitionalAnimation);
			m_CoverStatus = CoverStatus.None;
			m_CombatState = CombatState.ExitingCover;
			m_WantToExitCover = false;
			AdjustCollider();
			m_CooldownTimer = 5f;
			if (m_ChosenCover != null)
			{
				m_LastChosenCover = m_ChosenCover;
				m_ChosenCover.m_Enemy = null;
				m_ChosenCover = null;
			}
			return;
		}
		if ((m_WaitingToReload || m_Weapon.GetPercentageBulletsInClip() <= 0.2f) && m_Weapon.m_WeaponState != WeaponBase.WeaponState.Reloading)
		{
			if (m_CBRReload != null)
			{
				if (m_CoverStatus == CoverStatus.CrouchingRight || m_CoverStatus == CoverStatus.StandingRight)
				{
					m_Animator.Play(m_CBRReload.name);
				}
				if (m_CBRReload != null)
				{
					m_Weapon.Reload(m_CBRReload.length);
				}
			}
			else
			{
				m_Weapon.Reload(0f);
			}
			m_WaitingToReload = false;
			return;
		}
		m_EventTimer -= Time.deltaTime;
		if (!(m_EventTimer <= 0f) || m_Weapon.m_WeaponState == WeaponBase.WeaponState.Reloading)
		{
			return;
		}
		bool flag = UnityEngine.Random.value <= 0.2f && Globals.m_AIDirector.m_GrenadeCooldownTimer <= 0f && vector.sqrMagnitude <= 400f;
		m_CombatState = CombatState.CoverFiring;
		m_CoverFireStatus = (flag ? CoverFireStatus.ThrowingGrenade : CoverFireStatus.TransitionIn);
		if (m_CoverStatus == CoverStatus.CrouchingLeft)
		{
			float num = -1f;
			if ((m_ChosenCover.m_Cover.m_CoverType & 4) != 0)
			{
				num = 0.5f;
			}
			if (UnityEngine.Random.value <= num || (m_ChosenCover.m_Cover.m_CoverType & 1) == 0)
			{
				m_CoverAimingIndex = 1;
				m_CoverFireDirection = CoverFireDirection.Over;
				m_CoverTransitionInAnimation = ((!flag) ? "Aim_CrouchLeftOver_FireIn" : "ThrowGrenade_CrouchLeftOver");
				m_ThrowGrenadeTimer = 0.85f;
			}
			else
			{
				m_CoverAimingIndex = 0;
				m_CoverFireDirection = CoverFireDirection.Side;
				m_CoverTransitionInAnimation = ((!flag) ? "Aim_CrouchLeft_FireIn" : "ThrowGrenade_CrouchLeft");
				m_ThrowGrenadeTimer = 0.85f;
			}
		}
		else if (m_CoverStatus == CoverStatus.CrouchingRight)
		{
			float num2 = -1f;
			if ((m_ChosenCover.m_Cover.m_CoverType & 4) != 0)
			{
				num2 = 0.5f;
			}
			if (UnityEngine.Random.value <= num2 || (m_ChosenCover.m_Cover.m_CoverType & 2) == 0)
			{
				m_CoverAimingIndex = 3;
				m_CoverFireDirection = CoverFireDirection.Over;
				m_CoverTransitionInAnimation = ((!flag) ? "Aim_CrouchRightOver_FireIn" : "ThrowGrenade_CrouchRightOver");
				m_ThrowGrenadeTimer = 0.85f;
			}
			else
			{
				m_CoverAimingIndex = 2;
				m_CoverFireDirection = CoverFireDirection.Side;
				m_CoverTransitionInAnimation = ((!flag) ? "Aim_CrouchRight_FireIn" : "ThrowGrenade_CrouchRight");
				m_ThrowGrenadeTimer = 0.85f;
			}
		}
		else if (m_CoverStatus == CoverStatus.StandingLeft)
		{
			m_CoverAimingIndex = 4;
			m_CoverFireDirection = CoverFireDirection.Side;
			m_CoverTransitionInAnimation = ((!flag) ? "Aim_StandLeft_FireIn" : "ThrowGrenade_StandLeft");
			m_ThrowGrenadeTimer = 0.5f;
		}
		else
		{
			m_CoverAimingIndex = 5;
			m_CoverFireDirection = CoverFireDirection.Side;
			m_CoverTransitionInAnimation = ((!flag) ? "Aim_StandRight_FireIn" : "ThrowGrenade_StandRight");
			m_ThrowGrenadeTimer = 0.5f;
		}
		m_Animator.Play(m_CoverTransitionInAnimation);
		AdjustCollider();
		if (!flag)
		{
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_FragGrenadePrefab, Vector3.zero, Quaternion.identity) as GameObject;
		m_Grenade = gameObject.GetComponent<GrenadeFrag>();
		if ((bool)m_Grenade)
		{
			if (m_CoverStatus == CoverStatus.CrouchingLeft && m_CoverFireDirection == CoverFireDirection.Over)
			{
				SwapWeaponHands(true, false);
				m_Grenade.transform.parent = m_WeaponAttachLeft.transform;
			}
			else if (m_CoverStatus == CoverStatus.CrouchingRight && m_CoverFireDirection == CoverFireDirection.Over)
			{
				SwapWeaponHands(false, false);
				m_Grenade.transform.parent = m_WeaponAttachRight.transform;
			}
			else if (m_CoverStatus == CoverStatus.CrouchingLeft || m_CoverStatus == CoverStatus.StandingLeft)
			{
				m_Grenade.transform.parent = m_WeaponAttachRight.transform;
			}
			else
			{
				m_Grenade.transform.parent = m_WeaponAttachLeft.transform;
			}
			m_Grenade.transform.localPosition = Vector3.zero;
			m_Grenade.transform.localRotation = Quaternion.identity;
			m_Grenade.m_Owner = this;
			Globals.m_AIDirector.EnemyThrowingGrenade(this);
		}
		if (m_CoverStatus == CoverStatus.CrouchingLeft)
		{
			m_Animator.CrossFadeQueued("Idle_CrouchLeft");
		}
		else if (m_CoverStatus == CoverStatus.CrouchingRight)
		{
			m_Animator.CrossFadeQueued("Idle_CrouchRight");
		}
		else if (m_CoverStatus == CoverStatus.StandingLeft)
		{
			m_Animator.CrossFadeQueued("Idle_StandLeft");
		}
		else
		{
			m_Animator.CrossFadeQueued("Idle_StandRight");
		}
		PlayVO("Play_VO_Grenade_Throw", 72);
	}

	public virtual void UpdateCombatCoverFiring()
	{
		if (m_CoverFireStatus == CoverFireStatus.ThrowingGrenade)
		{
			if (m_ThrowGrenadeTimer > 0f)
			{
				m_ThrowGrenadeTimer -= Time.deltaTime;
				if (m_ThrowGrenadeTimer <= 0f)
				{
					m_Grenade.Throw(Globals.m_AIDirector.m_LastKnownPlayerPosition);
					m_Grenade = null;
				}
			}
			if (!m_Animator.IsPlaying(m_CoverTransitionInAnimation))
			{
				m_CombatState = CombatState.InCover;
				m_CoverFireStatus = CoverFireStatus.TransitionOut;
				m_EventTimer = UnityEngine.Random.Range(5f, 10f);
				AdjustCollider();
				if (m_CoverStatus == CoverStatus.CrouchingLeft || m_CoverStatus == CoverStatus.StandingLeft)
				{
					SwapWeaponHands(false, true);
				}
			}
			return;
		}
		if (m_CoverFireStatus == CoverFireStatus.TransitionIn)
		{
			if (!m_Animator.IsPlaying(m_CoverTransitionInAnimation))
			{
				EnableAimBlending(true, m_InCoverAimDirections[m_CoverAimingIndex]);
				m_CoverFireStatus = CoverFireStatus.WaitToFire;
				m_EventTimer = UnityEngine.Random.Range(0.5f, 1.5f);
				m_FiringFromCover = false;
			}
		}
		else if (m_CoverFireStatus == CoverFireStatus.WaitToFire)
		{
			m_EventTimer -= Time.deltaTime;
			if (m_EventTimer <= 0f)
			{
				if (SquadmateInLoS())
				{
					m_CoverFireStatus = CoverFireStatus.WaitToHide;
					m_EventTimer = 1f;
				}
				else
				{
					if (m_StateTimer >= m_PlayerLostDelayToInvestigate - m_PlayerLostDelayToStopFiring)
					{
						m_FiringFromCover = true;
						m_Weapon.StartFire();
						if (m_CoverAimingIndex == 0)
						{
							m_Animator.Play("Aim_CrouchLeft_FireLoop");
						}
						else if (m_CoverAimingIndex == 1)
						{
							m_Animator.Play("Aim_CrouchLeftOver_FireLoop");
						}
						else if (m_CoverAimingIndex == 2)
						{
							m_Animator.Play("Aim_CrouchRight_FireLoop");
						}
						else if (m_CoverAimingIndex == 3)
						{
							m_Animator.Play("Aim_CrouchRightOver_FireLoop");
						}
						else if (m_CoverAimingIndex == 4)
						{
							m_Animator.Play("Aim_StandLeft_FireLoop");
						}
						else
						{
							m_Animator.Play("Aim_StandRight_FireLoop");
						}
					}
					m_CoverFireStatus = CoverFireStatus.Fire;
					m_EventTimer = UnityEngine.Random.Range(2f, 3.5f);
					m_CurrentAccuracyVariance = m_MaxAccuracyVariation;
				}
			}
		}
		else if (m_CoverFireStatus == CoverFireStatus.Fire)
		{
			bool flag = !SquadmateInLoS();
			if (flag && !m_FiringFromCover)
			{
				if (m_StateTimer >= m_PlayerLostDelayToInvestigate - m_PlayerLostDelayToStopFiring)
				{
					m_FiringFromCover = true;
					m_Weapon.StartFire();
					if (m_CoverAimingIndex == 0)
					{
						m_Animator.Play("Aim_CrouchLeft_FireLoop");
					}
					else if (m_CoverAimingIndex == 1)
					{
						m_Animator.Play("Aim_CrouchLeftOver_FireLoop");
					}
					else if (m_CoverAimingIndex == 2)
					{
						m_Animator.Play("Aim_CrouchRight_FireLoop");
					}
					else if (m_CoverAimingIndex == 3)
					{
						m_Animator.Play("Aim_CrouchRightOver_FireLoop");
					}
					else if (m_CoverAimingIndex == 4)
					{
						m_Animator.Play("Aim_StandLeft_FireLoop");
					}
					else
					{
						m_Animator.Play("Aim_StandRight_FireLoop");
					}
					if (m_EventTimer <= 0.5f)
					{
						m_EventTimer += 1f;
					}
				}
				m_CurrentAccuracyVariance = m_MaxAccuracyVariation;
			}
			m_EventTimer -= Time.deltaTime;
			if (m_EventTimer <= 0f || m_WaitingToReload || !flag)
			{
				m_Weapon.EndFire();
				if (m_CoverAimingIndex == 0)
				{
					m_Animator.Stop("Aim_CrouchLeft_FireLoop");
				}
				else if (m_CoverAimingIndex == 1)
				{
					m_Animator.Stop("Aim_CrouchLeftOver_FireLoop");
				}
				else if (m_CoverAimingIndex == 2)
				{
					m_Animator.Stop("Aim_CrouchRight_FireLoop");
				}
				else if (m_CoverAimingIndex == 3)
				{
					m_Animator.Stop("Aim_CrouchRightOver_FireLoop");
				}
				else if (m_CoverAimingIndex == 4)
				{
					m_Animator.Stop("Aim_StandLeft_FireLoop");
				}
				else
				{
					m_Animator.Stop("Aim_StandRight_FireLoop");
				}
				m_CoverFireStatus = CoverFireStatus.WaitToHide;
				m_EventTimer = UnityEngine.Random.Range(1f, 2f);
			}
		}
		else if (m_CoverFireStatus == CoverFireStatus.WaitToHide)
		{
			m_EventTimer -= Time.deltaTime;
			if (m_EventTimer <= 0f)
			{
				EnableAimBlending(false, null);
				if (m_CoverAimingIndex == 0)
				{
					m_Animator.Play("Aim_CrouchLeft_FireOut");
				}
				else if (m_CoverAimingIndex == 1)
				{
					m_Animator.Play("Aim_CrouchLeftOver_FireOut");
				}
				else if (m_CoverAimingIndex == 2)
				{
					m_Animator.Play("Aim_CrouchRight_FireOut");
				}
				else if (m_CoverAimingIndex == 3)
				{
					m_Animator.Play("Aim_CrouchRightOver_FireOut");
				}
				else if (m_CoverAimingIndex == 4)
				{
					m_Animator.Play("Aim_StandLeft_FireOut");
				}
				else
				{
					m_Animator.Play("Aim_StandRight_FireOut");
				}
				if (m_CoverStatus == CoverStatus.CrouchingLeft)
				{
					m_Animator.CrossFadeQueued("Idle_CrouchLeft");
				}
				else if (m_CoverStatus == CoverStatus.CrouchingRight)
				{
					m_Animator.CrossFadeQueued("Idle_CrouchRight");
				}
				else if (m_CoverStatus == CoverStatus.StandingLeft)
				{
					m_Animator.CrossFadeQueued("Idle_StandLeft");
				}
				else
				{
					m_Animator.CrossFadeQueued("Idle_StandRight");
				}
				m_CombatState = CombatState.InCover;
				m_CoverFireStatus = CoverFireStatus.TransitionOut;
				m_EventTimer = UnityEngine.Random.Range(5f, 10f);
				AdjustCollider();
			}
		}
		DetermineSourceDirection(Globals.m_AIDirector.m_LastKnownPlayerPosition);
		float num = 57.29578f * Mathf.Acos(m_ToSourceDot);
		float num2 = 90f - 57.29578f * Mathf.Acos(m_ToSourceUpDot);
		if ((m_CoverStatus == CoverStatus.StandingLeft || m_CoverStatus == CoverStatus.StandingRight) && Globals.m_PlayerController.m_Stance == PlayerController.Stance.Crouch)
		{
			num2 -= Mathf.Lerp(m_AngleWhenTargetCrouched, 0f, Mathf.InverseLerp(0f, 17f, m_ToSource.sqrMagnitude));
		}
		int num3 = 0;
		num3 = ((m_CoverStatus == CoverStatus.CrouchingLeft) ? ((m_CoverFireDirection != CoverFireDirection.Side) ? 1 : 0) : ((m_CoverStatus == CoverStatus.CrouchingRight) ? ((m_CoverFireDirection != CoverFireDirection.Side) ? 3 : 2) : ((m_CoverStatus != CoverStatus.StandingLeft) ? 5 : 4)));
		float hPerc = Mathf.Min(Mathf.Abs(num / ((!(m_ToSourceRightDot >= 0f)) ? m_InCoverAimingExtentInDegrees[num3].y : m_InCoverAimingExtentInDegrees[num3].x)), 1f);
		float vPerc = Mathf.Clamp(num2 / ((!(m_ToSourceUpDot >= 0f)) ? m_InCoverAimingExtentInDegrees[num3].w : m_InCoverAimingExtentInDegrees[num3].z), -1f, 1f);
		BlendAiming(hPerc, vPerc);
		Vector3 vector = Globals.m_PlayerController.transform.position - base.transform.position;
		if (!m_WantsNewCover && !m_WantToExitCover && !(vector.sqrMagnitude <= 8f) && !(Vector3.Dot(Vector3.Normalize(new Vector3(vector.x, 0f, vector.z)), base.transform.forward) <= 0f))
		{
			return;
		}
		if (m_CoverFireStatus == CoverFireStatus.Fire)
		{
			m_Weapon.EndFire();
			if (m_CoverAimingIndex == 0)
			{
				m_Animator.Stop("Aim_CrouchLeft_FireLoop");
			}
			else if (m_CoverAimingIndex == 1)
			{
				m_Animator.Stop("Aim_CrouchLeftOver_FireLoop");
			}
			else if (m_CoverAimingIndex == 2)
			{
				m_Animator.Stop("Aim_CrouchRight_FireLoop");
			}
			else if (m_CoverAimingIndex == 3)
			{
				m_Animator.Stop("Aim_CrouchRightOver_FireLoop");
			}
			else if (m_CoverAimingIndex == 4)
			{
				m_Animator.Stop("Aim_StandLeft_FireLoop");
			}
			else
			{
				m_Animator.Stop("Aim_StandRight_FireLoop");
			}
		}
		m_WantToExitCover = true;
		m_CoverFireStatus = CoverFireStatus.WaitToHide;
		m_EventTimer = 0f;
	}

	public virtual void UpdateCombatExitingCover()
	{
		if (m_TransitionalAnimation == null || !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_Animator.CrossFade("Idle_Alert");
			m_PosedAnimation = null;
			m_TransitionalAnimation = null;
			m_CombatState = CombatState.StandAndFire;
			m_EventTimer = UnityEngine.Random.Range(20f, 30f);
			m_WaitingForPathToClear = false;
			EnableAimBlending(true, m_CBRAimDirections);
		}
	}

	public virtual bool TurnWhileStanding(float HAngle)
	{
		if (m_TransitionalAnimation != null)
		{
			return false;
		}
		if (m_ToSourceRightDot >= 0f)
		{
			if (m_ToSourceDot < 0f && HAngle >= 180f - m_RightAimingExtentInDegrees)
			{
				m_TransitionalAnimation = "Turn_Right180";
			}
			else if (HAngle >= m_RightAimingExtentInDegrees)
			{
				m_TransitionalAnimation = "Turn_Right90";
			}
		}
		else if (m_ToSourceDot < 0f && HAngle >= 180f - m_RightAimingExtentInDegrees)
		{
			m_TransitionalAnimation = "Turn_Left180";
		}
		else if (HAngle >= m_LeftAimingExtentInDegrees)
		{
			m_TransitionalAnimation = "Turn_Left90";
		}
		if (m_TransitionalAnimation != null)
		{
			m_PosedAnimation = m_TransitionalAnimation + "_Pose";
			m_Animator.CrossFade(m_TransitionalAnimation);
			return true;
		}
		return false;
	}

	public virtual void BlendAiming(float HPerc, float VPerc)
	{
		if (m_ActiveAimAnimations == null || float.IsNaN(HPerc) || HPerc == float.PositiveInfinity || HPerc == float.NegativeInfinity || float.IsNaN(VPerc) || VPerc == float.PositiveInfinity || VPerc == float.NegativeInfinity)
		{
			return;
		}
		for (int i = 0; i < 9; i++)
		{
			m_ActiveAimAnimations[i].weight = 0f;
		}
		if (m_ToSourceRightDot > 0f)
		{
			if (VPerc > 0f)
			{
				m_ActiveAimAnimations[0].weight = BiLerp(1f, 0f, 0f, 0f, HPerc, VPerc);
				m_ActiveAimAnimations[4].weight = BiLerp(0f, 1f, 0f, 0f, HPerc, VPerc);
				m_ActiveAimAnimations[1].weight = BiLerp(0f, 0f, 1f, 0f, HPerc, VPerc);
				m_ActiveAimAnimations[6].weight = BiLerp(0f, 0f, 0f, 1f, HPerc, VPerc);
			}
			else
			{
				m_ActiveAimAnimations[2].weight = BiLerp(1f, 0f, 0f, 0f, HPerc, 1f + VPerc);
				m_ActiveAimAnimations[8].weight = BiLerp(0f, 1f, 0f, 0f, HPerc, 1f + VPerc);
				m_ActiveAimAnimations[0].weight = BiLerp(0f, 0f, 1f, 0f, HPerc, 1f + VPerc);
				m_ActiveAimAnimations[4].weight = BiLerp(0f, 0f, 0f, 1f, HPerc, 1f + VPerc);
			}
		}
		else if (VPerc > 0f)
		{
			m_ActiveAimAnimations[3].weight = BiLerp(1f, 0f, 0f, 0f, 1f - HPerc, VPerc);
			m_ActiveAimAnimations[0].weight = BiLerp(0f, 1f, 0f, 0f, 1f - HPerc, VPerc);
			m_ActiveAimAnimations[5].weight = BiLerp(0f, 0f, 1f, 0f, 1f - HPerc, VPerc);
			m_ActiveAimAnimations[1].weight = BiLerp(0f, 0f, 0f, 1f, 1f - HPerc, VPerc);
		}
		else
		{
			m_ActiveAimAnimations[7].weight = BiLerp(1f, 0f, 0f, 0f, 1f - HPerc, 1f + VPerc);
			m_ActiveAimAnimations[2].weight = BiLerp(0f, 1f, 0f, 0f, 1f - HPerc, 1f + VPerc);
			m_ActiveAimAnimations[3].weight = BiLerp(0f, 0f, 1f, 0f, 1f - HPerc, 1f + VPerc);
			m_ActiveAimAnimations[0].weight = BiLerp(0f, 0f, 0f, 1f, 1f - HPerc, 1f + VPerc);
		}
	}

	public virtual void UpdateBursting()
	{
		if (m_Weapon == null)
		{
			return;
		}
		if (m_WaitingToReload)
		{
			if (m_Bursting)
			{
				StopBurstFire();
			}
			if (!m_Animator.IsPlaying("Damaged_HitRightShoulder") && !m_Animator.IsPlaying("Damaged_ConcussionGrenade") && !m_Animator.IsPlaying("Damaged_DartIdle"))
			{
				if (m_CBRReload != null)
				{
					m_Animator.Play(m_CBRReload.name);
					m_Weapon.Reload(m_CBRReload.length);
				}
				else
				{
					m_Weapon.Reload(0f);
				}
				m_WaitingToReload = false;
			}
			return;
		}
		m_BurstTimer -= Time.deltaTime;
		if (m_Weapon.m_WeaponState == WeaponBase.WeaponState.Reloading)
		{
			return;
		}
		float num = Vector3.Dot(m_Weapon.m_MuzzleFlashAttachEnemy.transform.forward, (Globals.m_PlayerController.GetChestLocation() - m_Weapon.m_MuzzleFlashAttachEnemy.transform.position).normalized);
		if (m_Bursting)
		{
			float num2 = Mathf.Clamp(Mathf.InverseLerp(100f, 12f, m_ToSource.sqrMagnitude), 0f, 1f);
			m_BurstTimer += Time.deltaTime * (num2 * 2f);
			if (m_BurstTimer <= 0f || SquadmateInLoS() || num < 0.94f || m_StateTimer < m_PlayerLostDelayToInvestigate - m_PlayerLostDelayToStopFiring)
			{
				StopBurstFire();
			}
		}
		else if (m_BurstTimer <= 0f && !SquadmateInLoS() && num >= 0.94f && m_StateTimer >= m_PlayerLostDelayToInvestigate - m_PlayerLostDelayToStopFiring)
		{
			StartBurstFire();
		}
	}

	public virtual void StartBurstFire()
	{
		if (!(m_Weapon == null) && !m_Bursting)
		{
			m_Bursting = true;
			m_BurstTimer = UnityEngine.Random.Range(m_MinBurstDuration, m_MaxBurstDuration);
			m_Animator.CrossFade("Weapon_Fire");
			m_CurrentAccuracyVariance = m_MaxAccuracyVariation;
			m_Weapon.StartFire();
		}
	}

	public virtual void StopBurstFire()
	{
		if (!(m_Weapon == null))
		{
			if (m_Bursting)
			{
				m_Weapon.EndFire();
				m_Bursting = false;
				m_BurstTimer = UnityEngine.Random.Range(m_MinBurstCooldown, m_MaxBurstCooldown);
			}
			m_Animator.Stop("Weapon_Fire");
			if (m_Weapon.GetPercentageBulletsInClip() <= 0.2f)
			{
				m_WaitingToReload = true;
			}
		}
	}

	public override void WeaponWantsReload()
	{
		if (m_Weapon != null && m_Weapon.m_WeaponState != WeaponBase.WeaponState.Reloading)
		{
			m_WaitingToReload = true;
			StopBurstFire();
		}
	}

	public override void WeaponDoneReloading()
	{
	}

	public virtual void SearchAPosition(Vector3 PositionToSearch, bool SetStateTimer = true)
	{
		m_WantToPatrol = false;
		m_WantsToEnterCover = false;
		m_WantsNewCover = false;
		m_WantToExitCover = false;
		m_WantsToInvestigate = false;
		m_DoneSearching = false;
		m_ChosenSearchNode = null;
		m_SearchSearchNode = false;
		if (m_Bursting)
		{
			StopBurstFire();
		}
		if (m_Weapon != null)
		{
			m_Weapon.EndFire();
		}
		SetStateBasedOnPreviousState();
		if (SetStateTimer)
		{
			m_StateTimer = UnityEngine.Random.Range(15f, 30f);
		}
		else if (m_StateTimer <= 3f)
		{
			m_StateTimer += 5f;
		}
		if (m_TransitionalAnimation == null)
		{
			m_Animator.CrossFade("Idle_Alert");
		}
		NavMeshHit hit;
		if (NavMesh.SamplePosition(PositionToSearch, out hit, 5f, m_NavAgent.walkableMask))
		{
			PositionToSearch = hit.position;
		}
		m_NavAgent.Stop(true);
		m_NavAgent.updateRotation = false;
		m_NavAgent.updatePosition = false;
		m_NavMoving = true;
		m_NavAgent.ResetPath();
		m_NavAgent.enabled = false;
		m_NavAgent.enabled = true;
		m_NavAgent.CalculatePath(PositionToSearch, m_NavMeshPath);
		if (m_HostileState != HostileState.EnteringCover && m_ChosenCover != null)
		{
			m_ChosenCover.m_Enemy = null;
			m_ChosenCover = null;
		}
		EnableAimBlending(false, null);
	}

	public virtual void SearchANode(SearchNode OverriddenNode = null)
	{
		m_WantToPatrol = false;
		m_WantsToEnterCover = false;
		m_WantsNewCover = false;
		m_WantToExitCover = false;
		m_WantsToInvestigate = false;
		m_DoneSearching = false;
		m_SearchSearchNode = true;
		if (OverriddenNode != null)
		{
			m_ChosenSearchNode = OverriddenNode;
		}
		else
		{
			m_ChosenSearchNode = Globals.m_AIDirector.GetSearchNode();
		}
		if (m_Bursting)
		{
			StopBurstFire();
		}
		if (m_Weapon != null)
		{
			m_Weapon.EndFire();
		}
		SetStateBasedOnPreviousState();
		m_StateTimer = UnityEngine.Random.Range(15f, 30f);
		if (m_TransitionalAnimation == null)
		{
			m_Animator.CrossFade("Idle_Alert");
		}
		m_NavAgent.Stop(true);
		m_NavMoving = false;
		if (m_ChosenSearchNode != null)
		{
			Vector3 position = m_ChosenSearchNode.transform.position;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(position, out hit, 0.5f, m_NavAgent.walkableMask))
			{
				position = hit.position;
			}
			m_NavAgent.updateRotation = false;
			m_NavAgent.updatePosition = false;
			m_NavMoving = true;
			m_NavAgent.ResetPath();
			m_NavAgent.enabled = false;
			m_NavAgent.enabled = true;
			m_NavAgent.CalculatePath(position, m_NavMeshPath);
		}
		if (m_HostileState != HostileState.EnteringCover && m_ChosenCover != null)
		{
			m_ChosenCover.m_Enemy = null;
			m_ChosenCover = null;
		}
		EnableAimBlending(false, null);
	}

	public virtual void CancelSearch(bool IAmLeader)
	{
		if (IAmLeader)
		{
			PlayVO("Play_VO_All_Clear", 40);
		}
		m_WantToPatrol = true;
		m_EventTimer = UnityEngine.Random.Range(0.5f, 2.5f);
	}

	public virtual void UpdateHostile()
	{
		switch (m_HostileState)
		{
		case HostileState.WaitToStartSearching:
			UpdateHostileWaitToStartSearching();
			break;
		case HostileState.TurnToStartSearching:
			UpdateHostileTurnToStartSearching();
			break;
		case HostileState.RunToSearch:
			UpdateHostileRunToSearch();
			break;
		case HostileState.Searching:
			UpdateHostileSearching();
			break;
		case HostileState.EnteringCover:
			UpdateHostileEnteringCover();
			break;
		case HostileState.FiringFromCover:
			UpdateHostileFiringFromCover();
			break;
		case HostileState.ExitingCover:
			UpdateHostileExitingCover();
			break;
		case HostileState.StandReady:
			UpdateHostileStandReady();
			break;
		}
		m_StateTimer -= Time.deltaTime;
		if (!m_DoneSearching && m_StateTimer <= 0f && Globals.m_AIDirector.AmILeader(this))
		{
			m_DoneSearching = true;
			Globals.m_AIDirector.EnemyFinishedSearching(this, true);
		}
		if (!CheckVisualSenses() && m_WantToPatrol)
		{
			m_EventTimer -= Time.deltaTime;
			if (m_TransitionalAnimation == null && m_EventTimer <= 0f)
			{
				m_WantToPatrol = false;
				m_Animator.CrossFade("Idle_Alert");
				ResumePatrol();
			}
		}
	}

	public virtual void UpdateHostileWaitToStartSearching()
	{
		if (m_TransitionalAnimation != null && !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_Animator.CrossFade("Idle_Alert");
			m_PosedAnimation = null;
			m_TransitionalAnimation = null;
		}
		if (m_TransitionalAnimation != null)
		{
			return;
		}
		if (m_SearchSearchNode && m_ChosenSearchNode == null)
		{
			m_HostileState = HostileState.StandReady;
			m_NavAgent.Stop(true);
			m_NavMoving = false;
			m_DoneSearching = true;
			Globals.m_AIDirector.EnemyFinishedSearching(this, false);
		}
		else if (m_NavMeshPath.status == NavMeshPathStatus.PathInvalid)
		{
			m_HostileState = HostileState.StandReady;
			m_NavAgent.Stop(true);
			m_NavMoving = false;
			SearchANode(null);
		}
		else if (!m_NavAgent.pathPending)
		{
			m_SourceDirection = TurnDirection.Forward;
			if (m_NavMeshPath.corners.Length > 1)
			{
				DetermineSourceDirection(m_NavMeshPath.corners[1]);
			}
			if (m_SourceDirection == TurnDirection.BackLeft)
			{
				m_TransitionalAnimation = "Turn_Left180";
			}
			else if (m_SourceDirection == TurnDirection.BackRight)
			{
				m_TransitionalAnimation = "Turn_Right180";
			}
			else if (m_SourceDirection == TurnDirection.Left)
			{
				m_TransitionalAnimation = "Turn_Left90";
			}
			else if (m_SourceDirection == TurnDirection.Right)
			{
				m_TransitionalAnimation = "Turn_Right90";
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
			m_NavAgent.Stop(true);
			m_HostileState = HostileState.TurnToStartSearching;
		}
	}

	public virtual void UpdateHostileTurnToStartSearching()
	{
		if (m_TransitionalAnimation != null && m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			return;
		}
		m_HostileState = HostileState.RunToSearch;
		if (m_TransitionalAnimation != null)
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
		}
		m_Animator.CrossFade("Walk_ForwardGunUp");
		m_PosedAnimation = null;
		m_TransitionalAnimation = null;
		m_NavAgent.speed = m_WalkSpeed;
		m_NavAgent.angularSpeed = m_WalkTurnSpeed;
		m_NavAgent.updateRotation = true;
		m_NavAgent.updatePosition = true;
		m_NavAgent.SetPath(m_NavMeshPath);
		m_NavAgent.Resume();
	}

	public virtual void UpdateHostileRunToSearch()
	{
		if (m_NavMoving && m_NavAgent.remainingDistance <= 0.5f && m_NavAgent.remainingDistance != float.PositiveInfinity)
		{
			m_HostileState = HostileState.Searching;
			m_MoveTimer = UnityEngine.Random.Range(10f, 15f);
			m_NavAgent.Stop(true);
			m_NavMoving = false;
			m_NavAgent.updateRotation = false;
			if (m_EnemyType == EnemyType.Ogre)
			{
				m_TransitionalAnimation = null;
				m_PosedAnimation = null;
				m_Animator.CrossFade("Idle_Investigate");
			}
			else
			{
				m_TransitionalAnimation = "Investigate_Nothing";
				m_PosedAnimation = null;
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
		}
	}

	public virtual void UpdateHostileSearching()
	{
		if (m_EnemyType == EnemyType.Ogre)
		{
			m_MoveTimer -= Time.deltaTime;
			if (m_MoveTimer <= 0f)
			{
				SearchANode(null);
			}
			return;
		}
		m_MoveTimer -= Time.deltaTime;
		if (m_TransitionalAnimation == null)
		{
			if (m_MoveTimer <= 0f)
			{
				SearchANode(null);
			}
		}
		else if (!m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_Animator.CrossFade("Idle_Investigate");
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
		}
	}

	public virtual void UpdateHostileStandReady()
	{
	}

	private void SetStateBasedOnPreviousState()
	{
		m_HostileState = HostileState.WaitToStartSearching;
		if (InCombat())
		{
			if (m_CombatState == CombatState.CoverFiring)
			{
				m_HostileState = HostileState.FiringFromCover;
				m_Weapon.EndFire();
				if (m_CoverAimingIndex == 0)
				{
					m_Animator.Stop("Aim_CrouchLeft_FireLoop");
				}
				else if (m_CoverAimingIndex == 1)
				{
					m_Animator.Stop("Aim_CrouchLeftOver_FireLoop");
				}
				else if (m_CoverAimingIndex == 2)
				{
					m_Animator.Stop("Aim_CrouchRight_FireLoop");
				}
				else if (m_CoverAimingIndex == 3)
				{
					m_Animator.Stop("Aim_CrouchRightOver_FireLoop");
				}
				else if (m_CoverAimingIndex == 4)
				{
					m_Animator.Stop("Aim_StandLeft_FireLoop");
				}
				else
				{
					m_Animator.Stop("Aim_StandRight_FireLoop");
				}
				if (m_CoverAimingIndex == 0)
				{
					m_CoverTransitionInAnimation = "Aim_CrouchLeft_FireOut";
				}
				else if (m_CoverAimingIndex == 1)
				{
					m_CoverTransitionInAnimation = "Aim_CrouchLeftOver_FireOut";
				}
				else if (m_CoverAimingIndex == 2)
				{
					m_CoverTransitionInAnimation = "Aim_CrouchRight_FireOut";
				}
				else if (m_CoverAimingIndex == 3)
				{
					m_CoverTransitionInAnimation = "Aim_CrouchRightOver_FireOut";
				}
				else if (m_CoverAimingIndex == 4)
				{
					m_CoverTransitionInAnimation = "Aim_StandLeft_FireOut";
				}
				else
				{
					m_CoverTransitionInAnimation = "Aim_StandRight_FireOut";
				}
				m_Animator.Play(m_CoverTransitionInAnimation);
				m_CoverFireStatus = CoverFireStatus.TransitionOut;
				AdjustCollider();
			}
			else if (m_CombatState == CombatState.InCover)
			{
				m_HostileState = HostileState.ExitingCover;
				SwapWeaponHands(true, true);
				if (m_CoverStatus == CoverStatus.CrouchingLeft)
				{
					m_TransitionalAnimation = "Exit_CrouchLeft";
				}
				else if (m_CoverStatus == CoverStatus.CrouchingRight)
				{
					m_TransitionalAnimation = "Exit_CrouchRight";
				}
				else if (m_CoverStatus == CoverStatus.StandingLeft)
				{
					m_TransitionalAnimation = "Exit_StandLeft";
				}
				else
				{
					m_TransitionalAnimation = "Exit_StandRight";
				}
				if (m_CBRReload != null)
				{
					m_Animator.Stop(m_CBRReload.name);
				}
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
				m_CoverStatus = CoverStatus.None;
				AdjustCollider();
			}
			else if (m_CombatState == CombatState.EnterCover)
			{
				m_HostileState = HostileState.EnteringCover;
			}
			else if (m_CombatState == CombatState.ExitingCover)
			{
				m_HostileState = HostileState.ExitingCover;
			}
		}
		else if (IsAlarmed())
		{
			if (m_AlarmedState == AlarmedState.AimFromCover)
			{
				m_HostileState = HostileState.FiringFromCover;
				if (m_CoverAimingIndex == 0)
				{
					m_CoverTransitionInAnimation = "Aim_CrouchLeft_FireOut";
				}
				else if (m_CoverAimingIndex == 1)
				{
					m_CoverTransitionInAnimation = "Aim_CrouchLeftOver_FireOut";
				}
				else if (m_CoverAimingIndex == 2)
				{
					m_CoverTransitionInAnimation = "Aim_CrouchRight_FireOut";
				}
				else if (m_CoverAimingIndex == 3)
				{
					m_CoverTransitionInAnimation = "Aim_CrouchRightOver_FireOut";
				}
				else if (m_CoverAimingIndex == 4)
				{
					m_CoverTransitionInAnimation = "Aim_StandLeft_FireOut";
				}
				else
				{
					m_CoverTransitionInAnimation = "Aim_StandRight_FireOut";
				}
				m_Animator.Play(m_CoverTransitionInAnimation);
				m_CoverFireStatus = CoverFireStatus.TransitionOut;
				AdjustCollider();
			}
			else if (m_AlarmedState == AlarmedState.InCover)
			{
				m_HostileState = HostileState.ExitingCover;
				SwapWeaponHands(true, true);
				if (m_CoverStatus == CoverStatus.CrouchingLeft)
				{
					m_TransitionalAnimation = "Exit_CrouchLeft";
				}
				else if (m_CoverStatus == CoverStatus.CrouchingRight)
				{
					m_TransitionalAnimation = "Exit_CrouchRight";
				}
				else if (m_CoverStatus == CoverStatus.StandingLeft)
				{
					m_TransitionalAnimation = "Exit_StandLeft";
				}
				else
				{
					m_TransitionalAnimation = "Exit_StandRight";
				}
				if (m_CBRReload != null)
				{
					m_Animator.Stop(m_CBRReload.name);
				}
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
				m_CoverStatus = CoverStatus.None;
				AdjustCollider();
			}
			else if (m_AlarmedState == AlarmedState.EnteringCover)
			{
				m_HostileState = HostileState.EnteringCover;
			}
			else if (m_AlarmedState == AlarmedState.ExitingCover)
			{
				m_HostileState = HostileState.ExitingCover;
			}
		}
		m_EnemyState = EnemyState.Hostile;
		m_PlayerWarned = false;
	}

	public virtual void UpdateHostileEnteringCover()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (m_TransitionalAnimation != null && !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			if (m_CoverStatus == CoverStatus.CrouchingLeft)
			{
				m_Animator.CrossFade("Idle_CrouchLeft");
			}
			else if (m_CoverStatus == CoverStatus.CrouchingRight)
			{
				m_Animator.CrossFade("Idle_CrouchRight");
			}
			else if (m_CoverStatus == CoverStatus.StandingLeft)
			{
				m_Animator.CrossFade("Idle_StandLeft");
			}
			else
			{
				m_Animator.CrossFade("Idle_StandRight");
			}
			m_PosedAnimation = null;
			m_TransitionalAnimation = null;
		}
		Vector3 vector = m_ChosenCover.m_Cover.transform.position - base.transform.position;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		float num = m_WalkSpeed * Time.deltaTime;
		bool flag = false;
		if (magnitude <= num)
		{
			base.transform.position = m_ChosenCover.m_Cover.transform.position;
			flag = true;
		}
		else
		{
			base.transform.position += vector.normalized * num;
		}
		Quaternion quaternion = Quaternion.LookRotation(m_ChosenCover.m_Cover.transform.forward);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, 180f * Time.deltaTime);
		if (flag && Quaternion.Angle(base.transform.rotation, quaternion) <= 5f)
		{
			base.transform.rotation = quaternion;
			m_HostileState = HostileState.ExitingCover;
			SwapWeaponHands(true, true);
			if (m_CoverStatus == CoverStatus.CrouchingLeft)
			{
				m_TransitionalAnimation = "Exit_CrouchLeft";
			}
			else if (m_CoverStatus == CoverStatus.CrouchingRight)
			{
				m_TransitionalAnimation = "Exit_CrouchRight";
			}
			else if (m_CoverStatus == CoverStatus.StandingLeft)
			{
				m_TransitionalAnimation = "Exit_StandLeft";
			}
			else
			{
				m_TransitionalAnimation = "Exit_StandRight";
			}
			if (m_CBRReload != null)
			{
				m_Animator.Stop(m_CBRReload.name);
			}
			m_PosedAnimation = m_TransitionalAnimation + "_Pose";
			m_Animator.CrossFade(m_TransitionalAnimation);
			m_CoverStatus = CoverStatus.None;
			m_CoverFireStatus = CoverFireStatus.None;
			AdjustCollider();
			if (m_ChosenCover != null)
			{
				m_ChosenCover.m_Enemy = null;
				m_ChosenCover = null;
			}
		}
	}

	public virtual void UpdateHostileFiringFromCover()
	{
		if (m_CoverTransitionInAnimation == null || !m_Animator.IsPlaying(m_CoverTransitionInAnimation))
		{
			m_HostileState = HostileState.ExitingCover;
			SwapWeaponHands(true, true);
			if (m_CoverStatus == CoverStatus.CrouchingLeft)
			{
				m_TransitionalAnimation = "Exit_CrouchLeft";
			}
			else if (m_CoverStatus == CoverStatus.CrouchingRight)
			{
				m_TransitionalAnimation = "Exit_CrouchRight";
			}
			else if (m_CoverStatus == CoverStatus.StandingLeft)
			{
				m_TransitionalAnimation = "Exit_StandLeft";
			}
			else
			{
				m_TransitionalAnimation = "Exit_StandRight";
			}
			if (m_CBRReload != null)
			{
				m_Animator.Stop(m_CBRReload.name);
			}
			m_PosedAnimation = m_TransitionalAnimation + "_Pose";
			m_Animator.CrossFade(m_TransitionalAnimation);
			m_CoverStatus = CoverStatus.None;
			m_CoverFireStatus = CoverFireStatus.None;
			AdjustCollider();
		}
	}

	public virtual void UpdateHostileExitingCover()
	{
		if (m_TransitionalAnimation == null || !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_Animator.CrossFade("Idle_Alert");
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
			m_HostileState = HostileState.WaitToStartSearching;
		}
	}

	public virtual void StartPatrol(bool FromSave = false)
	{
		m_EnemyState = EnemyState.Patrol;
		m_PlayerWarned = false;
		if (m_TargetNode != null)
		{
			m_PatrolState = PatrolState.Patrolling;
			if (FromSave)
			{
				PatrolNode patrolNode = ((!m_PatrolForward) ? m_TargetNode.m_ForwardConnection : m_TargetNode.m_ReverseConnection);
				if (patrolNode != null && patrolNode.m_RunToNextNode)
				{
					m_CurrentPatrolWalkSpeed = 4f;
					m_CurrentPatrolTurnSpeed = 10f;
					m_Animator.CrossFade("Run_Forward");
				}
				else
				{
					m_CurrentPatrolWalkSpeed = m_StrollSpeed;
					m_CurrentPatrolTurnSpeed = m_PatrolTurnSpeed;
					m_Animator.CrossFade("Walk_Forward");
				}
			}
			else
			{
				m_CurrentPatrolWalkSpeed = m_StrollSpeed;
				m_CurrentPatrolTurnSpeed = m_PatrolTurnSpeed;
				m_Animator.CrossFade("Walk_Forward");
			}
		}
		else
		{
			m_PatrolState = PatrolState.StandingIdle;
			m_Animator.CrossFade("Idle_Standing");
		}
	}

	public virtual void ResumePatrol()
	{
		m_CurrentPatrolWalkSpeed = m_StrollSpeed;
		m_CurrentPatrolTurnSpeed = m_PatrolTurnSpeed;
		m_EnemyState = EnemyState.Patrol;
		m_PlayerWarned = false;
		m_PatrolState = PatrolState.WaitingToRePatrol;
		m_StateTimer = UnityEngine.Random.Range(1.5f, 3.5f);
		Vector3 targetPosition = m_LastPatrolledPosition;
		NavMeshHit hit;
		if (NavMesh.SamplePosition(m_LastPatrolledPosition, out hit, 0.5f, m_NavAgent.walkableMask))
		{
			targetPosition = hit.position;
		}
		m_NavAgent.updateRotation = false;
		m_NavAgent.updatePosition = false;
		m_NavMoving = true;
		m_NavAgent.ResetPath();
		m_NavAgent.enabled = false;
		m_NavAgent.enabled = true;
		m_NavAgent.CalculatePath(targetPosition, m_NavMeshPath);
	}

	public virtual void ResumePatrolImmediate()
	{
		m_CurrentPatrolWalkSpeed = m_StrollSpeed;
		m_CurrentPatrolTurnSpeed = m_PatrolTurnSpeed;
		m_EnemyState = EnemyState.Patrol;
		m_PlayerWarned = false;
		if (m_TargetNode == null)
		{
			m_PatrolState = PatrolState.StandingIdle;
			m_Animator.CrossFade("Idle_Standing");
		}
		else
		{
			m_PatrolState = PatrolState.ReturnToPatrolImmediate;
			m_StateTimer = UnityEngine.Random.Range(1.5f, 3.5f);
		}
	}

	public virtual void UpdatePatrol()
	{
		switch (m_PatrolState)
		{
		case PatrolState.StandingIdle:
			UpdatePatrolStandingIdle();
			break;
		case PatrolState.Patrolling:
			UpdatePatrolPatrolling();
			break;
		case PatrolState.TurningToWaypoint:
			UpdateTurningToWaypoint();
			break;
		case PatrolState.AtWaypoint:
			UpdateAtWaypoint();
			break;
		case PatrolState.TurningFromWaypoint:
			UpdateTurningFromWaypoint();
			break;
		case PatrolState.WaitingToRePatrol:
			UpdateWaitingToRePatrol();
			break;
		case PatrolState.TransitionToRePatrol:
			UpdateTransitionToRePatrol();
			break;
		case PatrolState.WalkBackToRePatrol:
			UpdateWalkBackToRePatrol();
			break;
		case PatrolState.ReturnToPatrolImmediate:
			UpdateReturnToPatrolImmediate();
			break;
		}
		CheckVisualSenses();
	}

	public virtual void UpdatePatrolStandingIdle()
	{
		if (!(m_EventNode != null))
		{
			return;
		}
		m_StateTimer -= Time.deltaTime;
		if (!(m_StateTimer <= 0f))
		{
			return;
		}
		if (m_EventNode.m_PatrolEvent == PatrolNode.PatrolEvent.Waypoint)
		{
			m_WPDir = m_EventNode.m_WaypointDirection;
			if (!m_PatrolForward)
			{
				if (m_WPDir == PatrolNode.WaypointDirection.Forward)
				{
					m_WPDir = PatrolNode.WaypointDirection.Back;
				}
				else if (m_WPDir == PatrolNode.WaypointDirection.Back)
				{
					m_WPDir = PatrolNode.WaypointDirection.Forward;
				}
				else if (m_WPDir == PatrolNode.WaypointDirection.Right)
				{
					m_WPDir = PatrolNode.WaypointDirection.Left;
				}
				else if (m_WPDir == PatrolNode.WaypointDirection.Left)
				{
					m_WPDir = PatrolNode.WaypointDirection.Right;
				}
			}
			m_TransitionalAnimation = null;
			if (m_WPDir == PatrolNode.WaypointDirection.Back)
			{
				m_TransitionalAnimation = ((!(UnityEngine.Random.value <= 0.5f)) ? "TurnPatrol_West180" : "TurnPatrol_East180");
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
			else if (m_WPDir == PatrolNode.WaypointDirection.Left)
			{
				m_TransitionalAnimation = "TurnPatrol_West90";
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
			else if (m_WPDir == PatrolNode.WaypointDirection.Right)
			{
				m_TransitionalAnimation = "TurnPatrol_East90";
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
			m_PatrolState = PatrolState.TurningToWaypoint;
		}
		else if (m_EventNode.m_PatrolEvent == PatrolNode.PatrolEvent.TurnBack)
		{
			m_TransitionalAnimation = ((!(UnityEngine.Random.value <= 0.5f)) ? "TurnPatrol_West180" : "TurnPatrol_East180");
			m_PosedAnimation = m_TransitionalAnimation + "_Pose";
			m_Animator.CrossFade(m_TransitionalAnimation);
			m_PatrolState = PatrolState.TurningFromWaypoint;
		}
		else if (m_TargetNode != null)
		{
			m_CurrentPatrolWalkSpeed = m_StrollSpeed;
			m_CurrentPatrolTurnSpeed = m_PatrolTurnSpeed;
			m_Animator.CrossFade("Walk_Forward");
			m_PatrolState = PatrolState.Patrolling;
			m_EventNode = null;
		}
		else
		{
			m_EventNode = null;
		}
	}

	public virtual void UpdatePatrolPatrolling()
	{
		Vector3 vector = m_TargetNode.transform.position - base.transform.position;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		float num = m_CurrentPatrolWalkSpeed * Time.deltaTime;
		if (magnitude <= num)
		{
			base.transform.position = m_TargetNode.transform.position;
			m_LastPatrolledPosition = base.transform.position;
			num -= magnitude;
			if (PatrolEventTriggered())
			{
				m_StateTimer = UnityEngine.Random.Range(m_EventNode.m_MinIdle, m_EventNode.m_MaxIdle);
				if (m_EventNode.m_PatrolEvent == PatrolNode.PatrolEvent.TurnBack)
				{
					m_PatrolForward = !m_PatrolForward;
				}
				m_TargetNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
				m_PatrolState = PatrolState.StandingIdle;
				m_Animator.CrossFade("Idle_Standing");
				return;
			}
			if (m_EnemyType != EnemyType.Ogre)
			{
				if (m_TargetNode.m_RunToNextNode)
				{
					m_CurrentPatrolWalkSpeed = 4f;
					m_CurrentPatrolTurnSpeed = 10f;
					m_Animator.CrossFade("Run_Forward");
				}
				else
				{
					m_CurrentPatrolWalkSpeed = m_StrollSpeed;
					m_CurrentPatrolTurnSpeed = m_PatrolTurnSpeed;
					m_Animator.CrossFade("Walk_Forward");
				}
			}
			m_TargetNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
			if (!(m_TargetNode != null))
			{
				m_PatrolState = PatrolState.StandingIdle;
				m_Animator.CrossFade("Idle_Standing");
				return;
			}
			vector = m_TargetNode.transform.position - base.transform.position;
			vector.y = 0f;
		}
		float maxDegreesDelta = ((!(magnitude <= 1f)) ? m_CurrentPatrolTurnSpeed : (m_CurrentPatrolTurnSpeed * 2f));
		Quaternion to = Quaternion.LookRotation(vector.normalized);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, maxDegreesDelta);
		m_NavAgent.Move(base.transform.forward * num);
		m_LastPatrolledPosition = base.transform.position;
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
					m_TransitionalAnimation = "Cyclebreaker_SouthEast";
					break;
				case 1:
					m_TransitionalAnimation = "Cyclebreaker_SouthWest";
					break;
				case 2:
					m_TransitionalAnimation = "Cyclebreaker_East";
					break;
				default:
					m_TransitionalAnimation = "Cyclebreaker_West";
					break;
				}
				m_Animator.CrossFade(m_TransitionalAnimation);
				m_Animator.CrossFadeQueued("Walk_Forward");
				return false;
			}
			m_EventNode = m_TargetNode;
			return true;
		}
		return false;
	}

	public virtual void UpdateTurningToWaypoint()
	{
		if (m_TransitionalAnimation == null || !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_TransitionalAnimation = "Waypoint_DeskSearch";
			m_Animator.CrossFade(m_TransitionalAnimation);
			m_PosedAnimation = null;
			if (m_EventNode.m_Dialog != null && m_EventNode.m_Dialog.Length > 0)
			{
				PlayVO(m_EventNode.m_Dialog, 10);
			}
			m_PatrolState = PatrolState.AtWaypoint;
		}
	}

	public virtual void UpdateAtWaypoint()
	{
		if (m_TransitionalAnimation != null && m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			return;
		}
		RecalibrateBase();
		if (m_PosedAnimation != null)
		{
			m_Animator.Play(m_PosedAnimation);
		}
		m_PosedAnimation = null;
		m_TransitionalAnimation = null;
		if (m_WPDir == PatrolNode.WaypointDirection.Back)
		{
			if (m_TargetNode != null)
			{
				m_TransitionalAnimation = ((!(UnityEngine.Random.value <= 0.5f)) ? "TurnWalk_West180" : "TurnWalk_East180");
			}
			else
			{
				m_TransitionalAnimation = ((!(UnityEngine.Random.value <= 0.5f)) ? "TurnPatrol_West180" : "TurnPatrol_East180");
			}
			m_PosedAnimation = m_TransitionalAnimation + "_Pose";
			m_Animator.CrossFade(m_TransitionalAnimation);
		}
		else if (m_WPDir == PatrolNode.WaypointDirection.Right)
		{
			if (m_TargetNode != null)
			{
				m_TransitionalAnimation = "TurnWalk_West90";
			}
			else
			{
				m_TransitionalAnimation = "TurnPatrol_West90";
			}
			m_PosedAnimation = m_TransitionalAnimation + "_Pose";
			m_Animator.CrossFade(m_TransitionalAnimation);
		}
		else if (m_WPDir == PatrolNode.WaypointDirection.Left)
		{
			if (m_TargetNode != null)
			{
				m_TransitionalAnimation = "TurnWalk_East90";
			}
			else
			{
				m_TransitionalAnimation = "TurnPatrol_East90";
			}
			m_PosedAnimation = m_TransitionalAnimation + "_Pose";
			m_Animator.CrossFade(m_TransitionalAnimation);
		}
		else
		{
			m_Animator.CrossFade("Idle_Standing");
		}
		m_PatrolState = PatrolState.TurningFromWaypoint;
	}

	public virtual void UpdateTurningFromWaypoint()
	{
		if (m_TransitionalAnimation == null || !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_CurrentPatrolWalkSpeed = m_StrollSpeed;
			m_CurrentPatrolTurnSpeed = m_PatrolTurnSpeed;
			if (m_TargetNode != null)
			{
				m_PatrolState = PatrolState.Patrolling;
				m_Animator.CrossFade("Walk_Forward");
			}
			else
			{
				m_PatrolState = PatrolState.StandingIdle;
				m_Animator.CrossFade("Idle_Standing");
			}
			m_PosedAnimation = null;
			m_TransitionalAnimation = null;
			m_EventNode = null;
		}
	}

	public virtual void UpdateWaitingToRePatrol()
	{
		m_StateTimer -= Time.deltaTime;
		if (!m_NavAgent.pathPending && m_StateTimer <= 0f)
		{
			m_CurrentPatrolWalkSpeed = m_StrollSpeed;
			m_CurrentPatrolTurnSpeed = m_PatrolTurnSpeed;
			m_SourceDirection = TurnDirection.Forward;
			if (m_NavMeshPath.corners.Length > 1)
			{
				DetermineSourceDirection(m_NavMeshPath.corners[1]);
			}
			if (m_SourceDirection == TurnDirection.BackLeft)
			{
				m_TransitionalAnimation = "TurnWalk_West180";
			}
			else if (m_SourceDirection == TurnDirection.BackRight)
			{
				m_TransitionalAnimation = "TurnWalk_East180";
			}
			else if (m_SourceDirection == TurnDirection.Left)
			{
				m_TransitionalAnimation = "TurnWalk_West90";
			}
			else if (m_SourceDirection == TurnDirection.Right)
			{
				m_TransitionalAnimation = "TurnWalk_East90";
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
			m_NavAgent.Stop(true);
			m_PatrolState = PatrolState.TransitionToRePatrol;
		}
	}

	public virtual void UpdateTransitionToRePatrol()
	{
		if (m_TransitionalAnimation != null && m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			return;
		}
		m_PatrolState = PatrolState.WalkBackToRePatrol;
		if (m_TransitionalAnimation != null)
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
		}
		m_CurrentPatrolWalkSpeed = m_StrollSpeed;
		m_CurrentPatrolTurnSpeed = m_PatrolTurnSpeed;
		m_Animator.CrossFade("Walk_Forward", 0.1f);
		m_PosedAnimation = null;
		m_TransitionalAnimation = null;
		m_NavAgent.speed = m_StrollSpeed;
		m_NavAgent.angularSpeed = m_StrollTurnSpeed;
		m_NavAgent.updateRotation = true;
		m_NavAgent.updatePosition = true;
		m_NavAgent.SetPath(m_NavMeshPath);
		m_NavAgent.Resume();
	}

	public virtual void UpdateWalkBackToRePatrol()
	{
		if (m_NavMoving)
		{
			if (m_NavAgent.remainingDistance <= 0.5f && m_NavAgent.remainingDistance != float.PositiveInfinity)
			{
				m_NavAgent.Stop(true);
				m_NavMoving = false;
				m_NavAgent.updateRotation = false;
				m_NavAgent.updatePosition = true;
				m_CurrentPatrolWalkSpeed = m_StrollSpeed;
				m_CurrentPatrolTurnSpeed = m_PatrolTurnSpeed;
				if (m_TargetNode != null)
				{
					m_PatrolState = PatrolState.Patrolling;
					m_Animator.CrossFade("Walk_Forward");
				}
				else
				{
					m_PatrolState = PatrolState.StandingIdle;
					m_Animator.CrossFade("Idle_Standing");
				}
			}
		}
		else
		{
			m_CurrentPatrolWalkSpeed = m_StrollSpeed;
			m_CurrentPatrolTurnSpeed = m_PatrolTurnSpeed;
			if (m_TargetNode != null)
			{
				m_PatrolState = PatrolState.Patrolling;
				m_Animator.CrossFade("Walk_Forward");
			}
			else
			{
				m_PatrolState = PatrolState.StandingIdle;
				m_Animator.CrossFade("Idle_Standing");
			}
		}
	}

	public virtual void UpdateReturnToPatrolImmediate()
	{
		m_StateTimer -= Time.deltaTime;
		if (!(m_StateTimer <= 0f))
		{
			return;
		}
		if (m_TransitionalAnimation == null)
		{
			DetermineSourceDirection(m_TargetNode.transform.position);
			if (m_SourceDirection == TurnDirection.BackLeft)
			{
				m_TransitionalAnimation = "TurnWalk_West180";
			}
			else if (m_SourceDirection == TurnDirection.BackRight)
			{
				m_TransitionalAnimation = "TurnWalk_East180";
			}
			else if (m_SourceDirection == TurnDirection.Left)
			{
				m_TransitionalAnimation = "TurnWalk_West90";
			}
			else if (m_SourceDirection == TurnDirection.Right)
			{
				m_TransitionalAnimation = "TurnWalk_East90";
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
		if (m_TransitionalAnimation != null && m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			return;
		}
		if (m_TransitionalAnimation != null)
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
		}
		m_CurrentPatrolWalkSpeed = m_StrollSpeed;
		m_CurrentPatrolTurnSpeed = m_PatrolTurnSpeed;
		m_PatrolState = PatrolState.Patrolling;
		m_Animator.CrossFade("Walk_Forward");
	}

	public virtual void InvestigateSuspiciousSound(Vector3 sourceLocation, bool WasVisual = false, bool FromSave = false)
	{
		if (IsHostile())
		{
			return;
		}
		if (IsAlarmed())
		{
			InvestigateAlarmingSound(sourceLocation, false, false);
		}
		else
		{
			if (IsSuspicious() && !WasVisual && m_DisturbanceIgnoreTimer < 5f)
			{
				return;
			}
			if (m_NavMoving)
			{
				m_NavAgent.Stop(true);
				m_NavMoving = false;
				m_NavAgent.updateRotation = false;
			}
			m_AudioDisturbanceLocation = sourceLocation;
			m_DisturbanceIgnoreTimer = 0f;
			Globals.m_AIDirector.EnemyIsSuspicious(this);
			m_EnemyState = EnemyState.TransitionToSuspicious;
			m_PlayerWarned = false;
			m_StateTimer = 60f;
			DetermineSourceDirection(m_AudioDisturbanceLocation);
			if (!FromSave)
			{
				PlayDisturbance(m_ToSource.sqrMagnitude <= 12f, WasVisual, m_SourceDirection);
				if (WasVisual)
				{
					PlayVO("Play_VO_Suspicious_Visual", 50);
				}
				else
				{
					PlayVO("Play_VO_Suspicious_Audio", 50);
				}
			}
		}
	}

	public virtual void UpdateSuspicious()
	{
		switch (m_SuspiciousState)
		{
		case SuspiciousState.PostTransition:
			UpdateSuspiciousPostTransition();
			break;
		case SuspiciousState.TurnToWalk:
			UpdateSuspiciousTurnToWalk();
			break;
		case SuspiciousState.WalkToDisturbance:
			UpdateSuspiciousWalkToDisturbance();
			break;
		case SuspiciousState.ScanArea:
			UpdateSuspiciousScanArea();
			break;
		}
		m_StateTimer -= Time.deltaTime;
		if (!CheckVisualSenses() && m_StateTimer <= 0f && m_TransitionalAnimation == null)
		{
			m_Animator.CrossFade("Idle_Investigate");
			ResumePatrol();
			Globals.m_AIDirector.EnemyIsNoLongerSuspicious(this);
			PlayVO("Play_VO_All_Clear", 40);
		}
	}

	public virtual void UpdateSuspiciousPostTransition()
	{
		if (!m_NavAgent.pathPending)
		{
			m_SourceDirection = TurnDirection.Forward;
			if (m_NavMeshPath.corners.Length > 1)
			{
				DetermineSourceDirection(m_NavMeshPath.corners[1]);
			}
			if (m_SourceDirection == TurnDirection.BackLeft)
			{
				m_TransitionalAnimation = "TurnWalk_West180";
			}
			else if (m_SourceDirection == TurnDirection.BackRight)
			{
				m_TransitionalAnimation = "TurnWalk_East180";
			}
			else if (m_SourceDirection == TurnDirection.Left)
			{
				m_TransitionalAnimation = "TurnWalk_West90";
			}
			else if (m_SourceDirection == TurnDirection.Right)
			{
				m_TransitionalAnimation = "TurnWalk_East90";
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
			m_NavAgent.Stop(true);
			m_SuspiciousState = SuspiciousState.TurnToWalk;
		}
	}

	public virtual void UpdateSuspiciousTurnToWalk()
	{
		if (m_TransitionalAnimation != null && m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			return;
		}
		m_SuspiciousState = SuspiciousState.WalkToDisturbance;
		if (m_TransitionalAnimation != null)
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
		}
		if (m_EnemyType == EnemyType.Ogre)
		{
			m_Animator.CrossFade("Walk_ForwardGunUp", 0.1f);
		}
		else
		{
			m_Animator.CrossFade("Walk_Forward", 0.1f);
		}
		m_PosedAnimation = null;
		m_TransitionalAnimation = null;
		m_NavAgent.speed = m_StrollSpeed;
		m_NavAgent.angularSpeed = m_StrollTurnSpeed;
		m_NavAgent.updateRotation = true;
		m_NavAgent.updatePosition = true;
		m_NavAgent.SetPath(m_NavMeshPath);
		m_NavAgent.Resume();
	}

	public virtual void UpdateSuspiciousWalkToDisturbance()
	{
		if (m_NavMoving && m_NavAgent.remainingDistance <= 1f && m_NavAgent.remainingDistance != float.PositiveInfinity)
		{
			m_SuspiciousState = SuspiciousState.ScanArea;
			m_StateTimer = UnityEngine.Random.Range(m_MinInvestigateTime, m_MaxInvestigateTime);
			m_NavAgent.Stop(true);
			m_NavMoving = false;
			m_NavAgent.updateRotation = false;
			if (m_EnemyType == EnemyType.Ogre)
			{
				m_TransitionalAnimation = null;
				m_PosedAnimation = null;
				m_Animator.CrossFade("Idle_Investigate");
			}
			else
			{
				m_TransitionalAnimation = "Investigate_Search03";
				m_PosedAnimation = null;
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
		}
	}

	public virtual void UpdateSuspiciousScanArea()
	{
		if (m_EnemyType == EnemyType.Ogre)
		{
			return;
		}
		if (m_TransitionalAnimation == null)
		{
			m_EventTimer -= Time.deltaTime;
			if (m_EventTimer <= 0f)
			{
				m_TransitionalAnimation = "Investigate_Search03";
				m_PosedAnimation = null;
				m_Animator.CrossFade(m_TransitionalAnimation);
			}
		}
		else if (!m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_Animator.CrossFade("Idle_Investigate");
			m_EventTimer = UnityEngine.Random.Range(4f, 10f);
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (!m_Paused)
		{
			m_SeePlayerThisFrame = false;
			m_VisualSensesChecked = false;
			m_ChestLocation = m_RigMotion.transform.position + Vector3.up * 1.5f;
			switch (m_EnemyState)
			{
			case EnemyState.Patrol:
				UpdatePatrol();
				break;
			case EnemyState.TransitionToSuspicious:
				UpdateTransitionToSuspicious();
				break;
			case EnemyState.Suspicious:
				UpdateSuspicious();
				break;
			case EnemyState.TransitionToAlarmed:
				UpdateTransitionToAlarmed();
				break;
			case EnemyState.Alarmed:
				UpdateAlarmed();
				break;
			case EnemyState.Combat:
				UpdateCombat();
				break;
			case EnemyState.Hostile:
				UpdateHostile();
				break;
			case EnemyState.Death:
				UpdateDeath();
				break;
			case EnemyState.WarningPlayer:
				UpdateWarning();
				break;
			}
			m_DisturbanceIgnoreTimer += Time.deltaTime;
			m_CooldownTimer -= Time.deltaTime;
			RayClampToGround();
			if (!m_VisualSensesChecked)
			{
				m_VisualSensingTimer = 0f;
			}
			m_CurrentAccuracyVariance = Mathf.Max(m_CurrentAccuracyVariance - Time.deltaTime * m_AccuracyAdjustmentSpeed, 0f);
			if (m_CurrentVO != 0 && !SoundManager.IsEventPlaying(m_CurrentVO))
			{
				m_CurrentVO = 0u;
			}
			m_StaggerCooldown -= Time.deltaTime;
		}
	}

	public virtual void LateUpdate()
	{
		if (m_TrackCompoundTransition)
		{
			m_RigMotion.transform.position = m_CompoundTransitionPosition + m_CompoundTransitionTracker.transform.localPosition;
			m_RigMotion.transform.rotation = m_CompoundTransitionRotation * m_CompoundTransitionTracker.transform.localRotation;
			if (!m_Animator.IsPlaying(m_CompoundTransitionAnimationFrom))
			{
				base.transform.position = m_CompoundTransitionPosition;
				base.transform.rotation = m_CompoundTransitionRotation;
				m_RigMotion.transform.localPosition = m_CompoundTransitionTracker.transform.localPosition;
				m_RigMotion.transform.localRotation = m_CompoundTransitionTracker.transform.localRotation;
				m_CompoundTransitionAnimator.SetActiveRecursively(false);
				m_TrackCompoundTransition = false;
			}
		}
	}

	public virtual void UpdateDeath()
	{
		if (m_DeathCollider != null)
		{
			if (!m_DeadBodyFound && Globals.m_AIDirector.CheckVisualSenses(m_DeathCollider, DisturbanceEvent.MajorVisual, AudioEvent.DeadBodyFound))
			{
				m_DeadBodyFound = true;
			}
			Globals.m_AIDirector.ShowBodyToCameras(m_DeathCollider);
		}
		if (m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			return;
		}
		m_StateTimer -= Time.deltaTime;
		m_Renderer.material.SetFloat("_DissolvePower", m_StateTimer / 5f);
		if (m_Weapon != null && m_Weapon.m_RendererThirdPersonEnemy != null)
		{
			m_Weapon.m_RendererThirdPersonEnemy.material.SetFloat("_DissolvePower", m_StateTimer / 5f);
		}
		if (m_StateTimer <= 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			if (m_CompoundTransitionAnimator != null)
			{
				UnityEngine.Object.Destroy(m_CompoundTransitionAnimator);
			}
		}
	}

	public virtual void UpdateTransitionToSuspicious()
	{
		CheckVisualSenses();
		if (m_TransitionalAnimation == null || !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			m_EnemyState = EnemyState.Suspicious;
			m_PlayerWarned = false;
			m_SuspiciousState = SuspiciousState.PostTransition;
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_Animator.CrossFade("Idle_Standing");
			m_PosedAnimation = null;
			m_TransitionalAnimation = null;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(m_AudioDisturbanceLocation, out hit, 0.5f, m_NavAgent.walkableMask))
			{
				m_AudioDisturbanceLocation = hit.position;
			}
			m_NavAgent.updateRotation = false;
			m_NavAgent.updatePosition = false;
			m_NavMoving = true;
			m_NavAgent.ResetPath();
			m_NavAgent.enabled = false;
			m_NavAgent.enabled = true;
			m_NavAgent.CalculatePath(m_AudioDisturbanceLocation, m_NavMeshPath);
		}
	}

	public virtual void UpdateTransitionToAlarmed()
	{
		CheckVisualSenses();
		if (m_TransitionalAnimation == null || !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			m_EnemyState = EnemyState.Alarmed;
			m_PlayerWarned = false;
			m_AlarmedState = AlarmedState.PostTransition;
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_Animator.CrossFade("Idle_Alert");
			m_PosedAnimation = null;
			m_TransitionalAnimation = null;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(m_AudioDisturbanceLocation, out hit, 0.5f, m_NavAgent.walkableMask))
			{
				m_AudioDisturbanceLocation = hit.position;
			}
			m_NavAgent.updateRotation = false;
			m_NavAgent.updatePosition = false;
			m_NavMoving = true;
			m_NavAgent.ResetPath();
			m_NavAgent.enabled = false;
			m_NavAgent.enabled = true;
			m_NavAgent.CalculatePath(m_AudioDisturbanceLocation, m_NavMeshPath);
		}
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
			m_Weapon.m_ModelThirdPersonEnemy.SetActiveRecursively(true);
			m_Weapon.m_ModelThirdPersonEnemy.transform.parent = m_WeaponAttachRight.transform;
			m_Weapon.m_ModelThirdPersonEnemy.transform.localPosition = Vector3.zero;
			m_Weapon.m_ModelThirdPersonEnemy.transform.localRotation = Quaternion.identity;
			m_Weapon.SetEnemy();
		}
	}

	public virtual void DetermineSourceDirection(Vector3 sourceLocation)
	{
		m_ToSource = sourceLocation - m_RigMotion.transform.position;
		m_ToSourceNormalizedOnXZ = Vector3.Normalize(new Vector3(m_ToSource.x, 0f, m_ToSource.z));
		m_ToSourceDot = Vector3.Dot(m_ToSourceNormalizedOnXZ, m_RigMotion.transform.forward);
		m_ToSourceRightDot = Vector3.Dot(m_ToSourceNormalizedOnXZ, m_RigMotion.transform.right);
		m_ToSourceUpDot = m_ToSource.normalized.y;
		if (Mathf.Abs(m_ToSourceDot) >= Mathf.Abs(m_ToSourceRightDot))
		{
			if (m_ToSourceDot >= 0f)
			{
				m_SourceDirection = TurnDirection.Forward;
			}
			else
			{
				m_SourceDirection = ((!(m_ToSourceRightDot >= 0f)) ? TurnDirection.BackLeft : TurnDirection.BackRight);
			}
		}
		else if (m_ToSourceRightDot >= 0f)
		{
			m_SourceDirection = TurnDirection.Right;
		}
		else
		{
			m_SourceDirection = TurnDirection.Left;
		}
	}

	public static float BiLerp(float v0, float v1, float v2, float v3, float x, float y)
	{
		return Mathf.Lerp(Mathf.Lerp(v0, v1, x), Mathf.Lerp(v2, v3, x), y);
	}

	private void EnableAimBlending(bool enable, AnimationState[] AimDirections = null)
	{
		if (enable)
		{
			if (m_ActiveAimAnimations == AimDirections)
			{
				return;
			}
			if (m_ActiveAimAnimations != null)
			{
				for (int i = 0; i < 9; i++)
				{
					m_ActiveAimAnimations[i].enabled = false;
				}
			}
			m_ActiveAimAnimations = AimDirections;
			if (m_ActiveAimAnimations != null)
			{
				for (int j = 0; j < 9; j++)
				{
					m_ActiveAimAnimations[j].enabled = true;
					m_ActiveAimAnimations[j].weight = 0f;
				}
			}
			return;
		}
		if (m_ActiveAimAnimations != null)
		{
			for (int k = 0; k < 9; k++)
			{
				m_ActiveAimAnimations[k].enabled = false;
			}
		}
		m_ActiveAimAnimations = null;
	}

	public virtual bool CheckVisualSenses()
	{
		if (m_EnemyState == EnemyState.Death)
		{
			return false;
		}
		if (Globals.m_PlayerController == null)
		{
			return false;
		}
		Vector3 vector = Globals.m_PlayerController.transform.position - m_RigMotion.transform.position;
		Vector3 lhs = Vector3.Normalize(new Vector3(vector.x, 0f, vector.z));
		float num = Vector3.Dot(lhs, m_RigMotion.transform.forward);
		if (Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.Firing && Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.Outside && Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.CoverDive && Vector3.Dot(lhs, Globals.m_PlayerController.m_CoverNormal) >= 0.3f)
		{
			return ReadVisualSensingTimer(true, false);
		}
		if (!Globals.m_AugmentCloaking.enabled)
		{
			if (num >= m_VisualForwardThresholdDot && vector.sqrMagnitude <= m_VisualDistanceThresholdSqr)
			{
				Ray ray = new Ray(m_ChestLocation, Globals.m_PlayerController.GetChestLocation() - m_ChestLocation);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 100f, 82177) && (hitInfo.collider.gameObject.layer == 14 || (Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Firing && hitInfo.collider == Globals.m_PlayerController.m_CoverCollider)))
				{
					m_VisualSensingTimer += Time.deltaTime;
					return ReadVisualSensingTimer(false, vector.sqrMagnitude <= 9f);
				}
			}
			else if (vector.sqrMagnitude <= 2f && (double)num >= -0.5)
			{
				m_VisualSensingTimer += Time.deltaTime;
				return ReadVisualSensingTimer(false, true);
			}
		}
		return ReadVisualSensingTimer(true, false);
	}

	public virtual void CheckCombatSenses()
	{
		if (m_EnemyState == EnemyState.Death)
		{
			return;
		}
		Vector3 vector = Globals.m_PlayerController.transform.position - m_RigMotion.transform.position;
		Vector3 lhs = Vector3.Normalize(new Vector3(vector.x, 0f, vector.z));
		float num = Vector3.Dot(lhs, m_RigMotion.transform.forward);
		if (vector.sqrMagnitude >= m_VisualCombatDistanceThresholdSqr || (Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.Firing && Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.Outside && Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.CoverDive && Vector3.Dot(lhs, Globals.m_PlayerController.m_CoverNormal) >= 0.3f) || num < 0f || Globals.m_AugmentCloaking.enabled)
		{
			return;
		}
		Ray ray = new Ray(m_ChestLocation, Globals.m_PlayerController.GetChestLocation() - m_ChestLocation);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, 82177) && (hitInfo.collider.gameObject.layer == 14 || (Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Firing && hitInfo.collider == Globals.m_PlayerController.m_CoverCollider)))
		{
			Globals.m_AIDirector.UpdatePlayerKnownPosition();
			m_SeePlayerThisFrame = true;
			m_LostPlayer = false;
			m_StateTimer = m_PlayerLostDelayToInvestigate;
			if (!m_WantsNewCover && m_MoveTimer <= 0f && vector.sqrMagnitude >= m_MaxPCRangeSqr)
			{
				m_MoveTimer = 1f;
			}
		}
	}

	public virtual void FindCover(Vector3 PlayerToEnemy)
	{
		if (m_EnemyState == EnemyState.Death || m_EnemyType == EnemyType.Ogre)
		{
			return;
		}
		LinkedList<NearbyCover> coverList = Globals.m_AIDirector.GetCoverList(Globals.m_PlayerController.transform.position);
		LinkedListNode<NearbyCover> linkedListNode = coverList.First;
		NearbyCover nearbyCover = null;
		float num = 1000000f;
		while (linkedListNode != null)
		{
			if (linkedListNode.Value != m_LastChosenCover && linkedListNode.Value.m_Enemy == null)
			{
				LinkedListNode<Enemy_Base> linkedListNode2 = Globals.m_AIDirector.GetFirstEnemy((int)m_EnemySquad);
				while (true)
				{
					if (linkedListNode2 != null)
					{
						if ((linkedListNode2.Value.transform.position - linkedListNode.Value.m_Cover.transform.position).sqrMagnitude <= 2f)
						{
							break;
						}
						linkedListNode2 = linkedListNode2.Next;
						continue;
					}
					if (!(linkedListNode.Value.m_DistanceSqr >= 0f) || !(linkedListNode.Value.m_DistanceSqr <= 10000f))
					{
						break;
					}
					float sqrMagnitude = (linkedListNode.Value.m_Cover.transform.position - base.transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						Vector3 lhs = linkedListNode.Value.m_Cover.transform.position - Globals.m_PlayerController.transform.position;
						if (lhs.sqrMagnitude <= PlayerToEnemy.sqrMagnitude && Vector3.Dot(lhs, PlayerToEnemy) > 0f)
						{
							nearbyCover = linkedListNode.Value;
							num = sqrMagnitude;
						}
					}
					break;
				}
			}
			linkedListNode = linkedListNode.Next;
		}
		if (nearbyCover != null)
		{
			m_ChosenCover = nearbyCover;
			m_ChosenCover.m_Enemy = this;
		}
	}

	public virtual bool ReadVisualSensingTimer(bool ResetTimer, bool ForceSee = false)
	{
		if (m_EnemyState == EnemyState.Death)
		{
			return false;
		}
		bool result = false;
		m_VisualSensesChecked = true;
		HostilityLevel playerHostilityLevel = HostilityZone.GetPlayerHostilityLevel();
		if (m_VisualSensingTimer >= m_VisualThresholdToEnterCombat || (ForceSee && m_VisualSensingTimer >= m_VisualThresholdToInvestigate))
		{
			if (m_Neutral && playerHostilityLevel != HostilityLevel.Hostile && m_EnemyState != EnemyState.Hostile && m_EnemyState != EnemyState.Combat && m_EnemyState != EnemyState.Alarmed)
			{
				if (playerHostilityLevel == HostilityLevel.Warning && !m_PlayerWarned && (m_RigMotion.transform.position - Globals.m_PlayerController.transform.position).sqrMagnitude <= 12f && WarnPlayer())
				{
					result = true;
				}
				if (m_EnemyState == EnemyState.WarningPlayer)
				{
					result = true;
				}
			}
			else
			{
				EnterCombat(true, DamageType.None);
				result = true;
			}
		}
		else if (m_VisualSensingTimer >= m_VisualThresholdToInvestigate && ResetTimer && (!m_Neutral || HostilityZone.IsPlayerInHostileTerritory()))
		{
			InvestigateSuspiciousSound(Globals.m_PlayerController.transform.position, true, false);
			result = true;
		}
		if (ResetTimer)
		{
			m_VisualSensingTimer = 0f;
		}
		return result;
	}

	public virtual float CheckAudioSenses(Vector3 sourceLocation, float sourceRadiusSqr, DisturbanceEvent sourceEvent)
	{
		if (m_EnemyState == EnemyState.Death)
		{
			return -1f;
		}
		if (m_Neutral && !HostilityZone.IsPlayerInHostileTerritory())
		{
			return -1f;
		}
		float sqrMagnitude = (m_RigMotion.transform.position - sourceLocation).sqrMagnitude;
		if (sqrMagnitude <= sourceRadiusSqr)
		{
			if (sourceEvent == DisturbanceEvent.MinorAudio)
			{
				Vector3 direction = sourceLocation - GetChestLocation();
				RaycastHit[] array = Physics.RaycastAll(GetChestLocation(), direction, direction.magnitude, 65793);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].collider.tag == "SoundBlocker" || array[i].collider.gameObject.layer == 16)
					{
						return -1f;
					}
				}
			}
			return sqrMagnitude;
		}
		return -1f;
	}

	public virtual float CheckSpecificVisualSenses(Collider collider)
	{
		if (m_EnemyState == EnemyState.Death)
		{
			return -1f;
		}
		Vector3 vector = collider.transform.position - m_RigMotion.transform.position;
		Vector3 lhs = Vector3.Normalize(new Vector3(vector.x, 0f, vector.z));
		float num = Vector3.Dot(lhs, m_RigMotion.transform.forward);
		if (num >= m_VisualForwardThresholdDot && vector.sqrMagnitude <= m_VisualDistanceThresholdSqr)
		{
			Ray ray = new Ray(m_ChestLocation, collider.transform.position - m_ChestLocation);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 100f, 66305) && hitInfo.collider.gameObject.layer == 9)
			{
				return vector.sqrMagnitude;
			}
		}
		return -1f;
	}

	protected virtual void PlayDisturbance(bool Major, bool Visual, TurnDirection Direction)
	{
		string transitionalAnimation = m_TransitionalAnimation;
		switch (Direction)
		{
		case TurnDirection.BackLeft:
			m_TransitionalAnimation = "Disturbance_Major_AudioSouthWest";
			break;
		case TurnDirection.BackRight:
			m_TransitionalAnimation = "Disturbance_Major_AudioSouthEast";
			break;
		case TurnDirection.Left:
			m_TransitionalAnimation = "Disturbance_Major_AudioWest";
			break;
		case TurnDirection.Right:
			m_TransitionalAnimation = "Disturbance_Major_AudioEast";
			break;
		default:
			m_TransitionalAnimation = "Disturbance_Major_AudioNorth";
			break;
		}
		if (transitionalAnimation != null)
		{
			BeginCompoundTransition(transitionalAnimation, m_TransitionalAnimation);
		}
		m_PosedAnimation = m_TransitionalAnimation + "_Pose";
		m_Animator.CrossFade(m_TransitionalAnimation);
	}

	private void RecalibrateBase()
	{
		Vector3 position = m_RigMotion.transform.position;
		Quaternion rotation = m_RigMotion.transform.rotation;
		base.transform.position = position;
		base.transform.rotation = rotation;
		m_RigMotion.transform.localPosition = Vector3.zero;
		m_RigMotion.transform.localRotation = Quaternion.identity;
	}

	protected void BeginCompoundTransition(string AnimationFrom, string AnimationTo)
	{
		m_CompoundTransitionAnimator.SetActiveRecursively(true);
		m_TrackCompoundTransition = true;
		m_CompoundTransitionAnimationFrom = AnimationFrom;
		m_CompoundTransitionPosition = m_RigMotion.transform.position;
		m_CompoundTransitionRotation = m_RigMotion.transform.rotation;
		m_CompoundTransitionTracker.transform.localPosition = Vector3.zero;
		m_CompoundTransitionTracker.transform.localRotation = Quaternion.identity;
		m_CompoundTransitionAnimator.animation.Play(AnimationTo);
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

	private bool SquadmateInTheWay(Vector3 MovementVector, float RayLength)
	{
		Ray ray = new Ray(GetChestLocation(), MovementVector);
		return Physics.Raycast(ray, RayLength, 512);
	}

	private bool SquadmateInLoS()
	{
		for (LinkedListNode<Enemy_Base> linkedListNode = Globals.m_AIDirector.GetFirstEnemy((int)m_EnemySquad); linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			Vector3 vector = linkedListNode.Value.m_ChestLocation - m_Weapon.m_MuzzleFlashAttachEnemy.transform.position;
			if (Vector3.Dot(m_FireDirection, vector.normalized) >= 0.965f)
			{
				return true;
			}
		}
		return false;
	}

	private void DropWeapon()
	{
		m_Weapon.Drop(base.transform.position);
		m_Weapon = null;
	}

	private void SwapWeaponHands(bool UseRightHand, bool ZeroOut = true)
	{
		if (!(m_Weapon == null))
		{
			if (UseRightHand)
			{
				m_Weapon.m_ModelThirdPersonEnemy.transform.parent = m_WeaponAttachRight.transform;
			}
			else
			{
				m_Weapon.m_ModelThirdPersonEnemy.transform.parent = m_WeaponAttachLeft.transform;
			}
			if (ZeroOut)
			{
				m_Weapon.m_ModelThirdPersonEnemy.transform.localPosition = Vector3.zero;
				m_Weapon.m_ModelThirdPersonEnemy.transform.localRotation = Quaternion.identity;
			}
		}
	}

	public virtual void PromotedToLeader(Enemy_Base OldLeader)
	{
		if (m_EnemyState == EnemyState.Alarmed)
		{
			if (m_AlarmedState == AlarmedState.EnteringCover || m_AlarmedState == AlarmedState.InCover || m_AlarmedState == AlarmedState.AimFromCover)
			{
				m_StateTimer = 5f;
				m_WantToExitCover = true;
			}
			m_WantsToInvestigate = true;
			m_WantsToInvestigateLocation = OldLeader.transform.position;
		}
		else if (m_EnemyState == EnemyState.Hostile)
		{
			m_StateTimer = OldLeader.m_StateTimer;
		}
	}

	public virtual void AdjustCollider()
	{
		if (m_CapsuleCollider == null)
		{
			return;
		}
		if (m_CoverStatus == CoverStatus.CrouchingLeft)
		{
			if (m_CoverFireStatus == CoverFireStatus.TransitionIn || m_CoverFireStatus == CoverFireStatus.Fire || m_CoverFireStatus == CoverFireStatus.WaitToFire || m_CoverFireStatus == CoverFireStatus.WaitToHide)
			{
				m_CapsuleCollider.height = m_ColliderHeightCrouchLeaning;
				if (m_CoverFireDirection == CoverFireDirection.Over)
				{
					m_CapsuleCollider.center = new Vector3(0.5f, m_CapsuleCollider.height * 0.5f, 0f);
				}
				else
				{
					m_CapsuleCollider.center = new Vector3(-0.4f, m_CapsuleCollider.height * 0.5f, 0f);
				}
			}
			else
			{
				m_CapsuleCollider.height = m_ColliderHeightCrouching;
				m_CapsuleCollider.center = new Vector3(0f, m_CapsuleCollider.height * 0.5f, 0f);
			}
		}
		else if (m_CoverStatus == CoverStatus.CrouchingRight)
		{
			if (m_CoverFireStatus == CoverFireStatus.TransitionIn || m_CoverFireStatus == CoverFireStatus.Fire || m_CoverFireStatus == CoverFireStatus.WaitToFire || m_CoverFireStatus == CoverFireStatus.WaitToHide)
			{
				m_CapsuleCollider.height = m_ColliderHeightCrouchLeaning;
				if (m_CoverFireDirection == CoverFireDirection.Over)
				{
					m_CapsuleCollider.center = new Vector3(-0.5f, m_CapsuleCollider.height * 0.5f, 0f);
				}
				else
				{
					m_CapsuleCollider.center = new Vector3(0.4f, m_CapsuleCollider.height * 0.5f, 0f);
				}
			}
			else
			{
				m_CapsuleCollider.height = m_ColliderHeightCrouching;
				m_CapsuleCollider.center = new Vector3(0f, m_CapsuleCollider.height * 0.5f, 0f);
			}
		}
		else if (m_CoverStatus == CoverStatus.StandingLeft)
		{
			m_CapsuleCollider.height = m_ColliderHeightStanding;
			if (m_CoverFireStatus == CoverFireStatus.TransitionIn || m_CoverFireStatus == CoverFireStatus.Fire || m_CoverFireStatus == CoverFireStatus.WaitToFire || m_CoverFireStatus == CoverFireStatus.WaitToHide)
			{
				m_CapsuleCollider.center = new Vector3(-0.5f, m_CapsuleCollider.height * 0.5f, 0.2f);
			}
			else
			{
				m_CapsuleCollider.center = new Vector3(0f, m_CapsuleCollider.height * 0.5f, 0f);
			}
		}
		else if (m_CoverStatus == CoverStatus.StandingRight)
		{
			m_CapsuleCollider.height = m_ColliderHeightStanding;
			if (m_CoverFireStatus == CoverFireStatus.TransitionIn || m_CoverFireStatus == CoverFireStatus.Fire || m_CoverFireStatus == CoverFireStatus.WaitToFire || m_CoverFireStatus == CoverFireStatus.WaitToHide)
			{
				m_CapsuleCollider.center = new Vector3(0.5f, m_CapsuleCollider.height * 0.5f, 0.2f);
			}
			else
			{
				m_CapsuleCollider.center = new Vector3(0f, m_CapsuleCollider.height * 0.5f, 0f);
			}
		}
		else
		{
			m_CapsuleCollider.height = m_ColliderHeightStanding;
			m_CapsuleCollider.center = new Vector3(0f, m_CapsuleCollider.height * 0.5f, 0f);
		}
	}

	private CoverStatus DetermineChosenCoverType()
	{
		if ((m_ChosenCover.m_Cover.m_CoverType & 5) != 0 && (m_ChosenCover.m_Cover.m_CoverType & 2) == 0)
		{
			return CoverStatus.CrouchingLeft;
		}
		if ((m_ChosenCover.m_Cover.m_CoverType & 6) != 0)
		{
			return CoverStatus.CrouchingRight;
		}
		if ((m_ChosenCover.m_Cover.m_CoverType & 8) != 0)
		{
			return CoverStatus.StandingLeft;
		}
		return CoverStatus.StandingRight;
	}

	private void CalculateFireDirection()
	{
		if (!(m_Weapon == null))
		{
			Vector3 vector = Globals.m_PlayerController.GetChestLocation() - m_Weapon.m_MuzzleFlashAttachEnemy.transform.position;
			vector.Normalize();
			m_FireDirection = m_Weapon.m_MuzzleFlashAttachEnemy.transform.forward;
			if (Vector3.Dot(m_FireDirection, vector) >= m_BulletAdjustDotThreshold)
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
		if (vector.sqrMagnitude <= 1f && Vector3.Dot(vector.normalized, m_Weapon.m_MuzzleFlashAttachEnemy.transform.forward) >= 0.65f)
		{
			PlayTracer = false;
			return new Ray(GetChestLocation(), Globals.m_PlayerController.GetChestLocation() - GetChestLocation());
		}
		Quaternion quaternion = Quaternion.Euler(UnityEngine.Random.Range(0f - m_CurrentAccuracyVariance, m_CurrentAccuracyVariance), UnityEngine.Random.Range(0f - m_CurrentAccuracyVariance, m_CurrentAccuracyVariance), 0f);
		vector = quaternion * m_FireDirection;
		return new Ray(m_Weapon.m_MuzzleFlashAttachEnemy.transform.position, vector);
	}

	public override void HitByTranquilizer()
	{
		if (m_EnemyState != EnemyState.Death)
		{
			base.HitByTranquilizer();
			if (m_EnemyState != EnemyState.Combat)
			{
				m_Animator.CrossFade("Damaged_DartIdle");
				PlayVO("Play_VO_Damage_Tranq", 66);
			}
		}
	}

	private void Stagger()
	{
		m_StaggerCooldown = 10f;
		m_CombatState = CombatState.Staggering;
		if (m_Bursting)
		{
			StopBurstFire();
		}
		EnableAimBlending(false, null);
		string transitionalAnimation = m_TransitionalAnimation;
		m_TransitionalAnimation = "Damaged_HitRightShoulder";
		if (transitionalAnimation != null)
		{
			BeginCompoundTransition(transitionalAnimation, m_TransitionalAnimation);
		}
		m_Animator.CrossFade(m_TransitionalAnimation);
		Globals.m_AIDirector.UpdatePlayerKnownPosition();
	}

	private void Stunned(Vector3 sourceLocation)
	{
		if (!InCombat())
		{
			if (m_NavMoving)
			{
				m_NavAgent.Stop(true);
				m_NavMoving = false;
				m_NavAgent.updateRotation = false;
			}
			m_EnemyState = EnemyState.Combat;
			m_PlayerWarned = false;
			m_StateTimer = m_PlayerLostDelayToInvestigate;
			m_MoveTimer = -1f;
			m_LostPlayer = false;
			Globals.m_AIDirector.EnemyEnteredCombat(this, sourceLocation);
		}
		m_StaggerCooldown = 10f;
		m_CombatState = CombatState.Staggering;
		if (m_Bursting)
		{
			StopBurstFire();
		}
		EnableAimBlending(false, null);
		Vector3 vector = sourceLocation - m_RigMotion.transform.position;
		vector.y = 0f;
		base.transform.position = m_RigMotion.transform.position;
		base.transform.rotation = Quaternion.LookRotation(vector.normalized);
		m_Animator.Stop();
		m_TransitionalAnimation = "Damaged_ConcussionGrenade";
		m_Animator.Play(m_TransitionalAnimation);
		m_PosedAnimation = null;
		Globals.m_AIDirector.UpdatePlayerKnownPosition();
	}

	public override bool TakeDamage(DamageData data)
	{
		if (m_EnemyState == EnemyState.Death)
		{
			return true;
		}
		if (data.m_Headshot && (!m_HeadProtected || data.m_DamageType == DamageType.ArmorPiercing))
		{
			data.m_Damage *= 6;
		}
		else
		{
			data.m_Headshot = false;
		}
		bool flag = base.TakeDamage(data);
		if (!flag)
		{
			if (data.m_DamageType == DamageType.Concussion)
			{
				Stunned(data.m_SourceLocation);
			}
			else if (!InCombat())
			{
				EnterCombat(true, data.m_DamageType);
			}
			else if (m_StaggerCooldown <= 0f && UnityEngine.Random.value <= 0.2f && (m_CombatState == CombatState.ChargePlayer || m_CombatState == CombatState.StandAndFire || m_CombatState == CombatState.StandAtCover || m_CombatState == CombatState.RunToCover) && (m_CBRReload == null || !m_Animator.IsPlaying(m_CBRReload.name)))
			{
				Stagger();
			}
			else if (m_CombatState == CombatState.StandAtCover && m_CooldownTimer <= 0f)
			{
				m_WantsToEnterCover = true;
			}
			else if (m_CombatState == CombatState.StandAndFire && m_CooldownTimer <= 0f)
			{
				m_WantsToEnterCover = true;
			}
		}
		return flag;
	}

	public override void Die(DamageData data)
	{
		if (m_EnemyState == EnemyState.Death)
		{
			return;
		}
		base.Die(data);
		Globals.m_AIDirector.EnemyKilled(this);
		m_EnemyState = EnemyState.Death;
		m_PlayerWarned = false;
		if (m_Bursting)
		{
			StopBurstFire();
		}
		if (m_CBRReload != null)
		{
			m_Animator.Stop(m_CBRReload.name);
		}
		EnableAimBlending(false, null);
		if ((bool)m_Collider)
		{
			m_Collider.enabled = false;
		}
		m_NavAgent.Stop(true);
		m_NavAgent.updateRotation = false;
		PlayDeathAnimation(data);
		if (m_ChosenCover != null)
		{
			m_LastChosenCover = m_ChosenCover;
			m_ChosenCover.m_Enemy = null;
			m_ChosenCover = null;
		}
		m_StateTimer = 5f;
		m_Renderer.material = m_DeathMaterial;
		if (Globals.m_PlayerController.m_TargetedEnemy == base.gameObject)
		{
			Globals.m_PlayerController.m_TargetedEnemy = null;
		}
		if (m_Weapon != null)
		{
			m_Weapon.EndFire();
			if (m_EnemyType == EnemyType.Ogre)
			{
				m_Weapon.SetDeathMaterial();
			}
			else
			{
				DropWeapon();
			}
		}
		if (m_Grenade != null)
		{
			Globals.m_AIDirector.RemoveGrenade(m_Grenade);
			m_Grenade = null;
		}
		m_DeathCollider = m_Waist.gameObject.AddComponent<SphereCollider>();
		m_DeathCollider.isTrigger = true;
		m_DeathCollider.center = Vector3.zero;
		m_DeathCollider.radius = 0.5f;
		m_DeathCollider.enabled = true;
	}

	protected virtual void PlayDeathAnimation(DamageData data)
	{
		string transitionalAnimation = m_TransitionalAnimation;
		if (data.m_DamageType == DamageType.NonLethal)
		{
			m_TransitionalAnimation = "Death_DartFallForward";
			PlayVO("Play_Enemy_Death_Tranq_FWD", 100);
			if (transitionalAnimation != null)
			{
				BeginCompoundTransition(transitionalAnimation, m_TransitionalAnimation);
			}
			m_Animator.CrossFade(m_TransitionalAnimation);
			return;
		}
		if (data.m_DamageType == DamageType.StunGun)
		{
			m_TransitionalAnimation = "Death_StunGun_FallFWD";
			PlayVO("Play_Enemy_Death_StunGun_FallFWD", 100);
			if (transitionalAnimation != null)
			{
				BeginCompoundTransition(transitionalAnimation, m_TransitionalAnimation);
			}
			m_Animator.CrossFade(m_TransitionalAnimation);
			GameObject gameObject = UnityEngine.Object.Instantiate(Globals.m_This.m_StunGunEffect) as GameObject;
			gameObject.transform.parent = m_StunGunEffectAttachment;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			return;
		}
		if (data.m_DamageType == DamageType.Explosive)
		{
			Vector3 lhs = data.m_SourceLocation - m_RigMotion.transform.position;
			lhs.y = 0f;
			if (Vector3.Dot(lhs, m_RigMotion.transform.forward) > 0f)
			{
				base.transform.position = m_RigMotion.transform.position;
				base.transform.rotation = Quaternion.LookRotation(lhs.normalized);
				m_TransitionalAnimation = "Death_Explosion";
			}
			else
			{
				base.transform.position = m_RigMotion.transform.position;
				base.transform.rotation = Quaternion.LookRotation(-lhs.normalized);
				m_TransitionalAnimation = "Death_Back_Heavy";
			}
			m_Animator.Stop();
			m_Animator.Play(m_TransitionalAnimation);
			PlayVO("Play_Enemy_Death_Explosion", 100);
			return;
		}
		Vector3 lhs2 = data.m_SourceLocation - m_RigMotion.transform.position;
		float sqrMagnitude = lhs2.sqrMagnitude;
		lhs2.y = 0f;
		bool flag = Vector3.Dot(lhs2, m_RigMotion.transform.forward) > 0f;
		if (data.m_Headshot)
		{
			if (UnityEngine.Random.value <= 0.5f)
			{
				m_TransitionalAnimation = "Death_Head_Back_Heavy";
				PlayVO("Play_Enemy_Death_Head_Back_Heavy", 100);
			}
			else
			{
				m_TransitionalAnimation = "Death_Head_Back_Light";
				PlayVO("Play_Enemy_Death_Head_Back_Light", 100);
			}
			if (transitionalAnimation != null)
			{
				BeginCompoundTransition(transitionalAnimation, m_TransitionalAnimation);
			}
			m_Animator.CrossFade(m_TransitionalAnimation);
			return;
		}
		if (data.m_SourceEnemy != null && data.m_SourceEnemy.GetEquippedWeaponType() == WeaponType.Shotgun && sqrMagnitude <= 9f)
		{
			if (flag)
			{
				base.transform.position = m_RigMotion.transform.position;
				base.transform.rotation = Quaternion.LookRotation(lhs2.normalized);
				m_TransitionalAnimation = "Death_Chest_Front_Heavy";
				PlayVO("Play_Enemy_Death_Chest_Front_Heavy", 100);
			}
			else
			{
				base.transform.position = m_RigMotion.transform.position;
				base.transform.rotation = Quaternion.LookRotation(-lhs2.normalized);
				m_TransitionalAnimation = "Death_Back_Heavy";
				PlayVO("Play_Enemy_Death_Back_Heavy", 100);
			}
			m_Animator.Stop();
			m_Animator.Play(m_TransitionalAnimation);
			PlayVO("Play_Enemy_Death_Explosion", 100);
			return;
		}
		int num = ((!(UnityEngine.Random.value <= 0.5f)) ? 1 : 0);
		Ray ray = new Ray(m_RigMotion.transform.position + Vector3.up * 0.25f, (num != 0) ? (-m_RigMotion.transform.forward) : m_RigMotion.transform.forward);
		if (Physics.Raycast(ray, 2f, 6374145))
		{
			num = (num + 1) % 2;
			ray.direction = ((num != 0) ? (-m_RigMotion.transform.forward) : m_RigMotion.transform.forward);
			if (Physics.Raycast(ray, 2f, 6374145))
			{
				num = -1;
				m_TransitionalAnimation = "Death_FallLeft";
				PlayVO("Play_Enemy_Death_LeftFall", 100);
			}
		}
		switch (num)
		{
		case 0:
			m_TransitionalAnimation = ((!(UnityEngine.Random.value <= 0.35f)) ? "Death_Back_Light" : "Death_Drop");
			PlayVO("Play_Enemy_Death_Back_Light", 100);
			break;
		case 1:
			m_TransitionalAnimation = ((!(UnityEngine.Random.value <= 0.35f)) ? "Death_Front_Stomach_Light" : "Death_Front");
			PlayVO("Play_Enemy_Death_Front_Stomach_Light", 100);
			break;
		}
		if (transitionalAnimation != null)
		{
			BeginCompoundTransition(transitionalAnimation, m_TransitionalAnimation);
		}
		m_Animator.CrossFade(m_TransitionalAnimation);
	}

	public virtual void Removed()
	{
		if (Globals.m_PlayerController.m_TargetedEnemy == base.gameObject)
		{
			Globals.m_PlayerController.m_TargetedEnemy = null;
		}
		if (m_ChosenCover != null)
		{
			m_LastChosenCover = m_ChosenCover;
			m_ChosenCover.m_Enemy = null;
			m_ChosenCover = null;
		}
		if (m_Weapon != null)
		{
			StopBurstFire();
			m_Weapon.EndFire();
		}
		if (m_Grenade != null)
		{
			Globals.m_AIDirector.RemoveGrenade(m_Grenade);
			m_Grenade = null;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void BeginTakedownOnEnemy()
	{
		EnableAimBlending(false, null);
		if ((bool)m_Collider)
		{
			m_Collider.enabled = false;
		}
		UnityEngine.Object.Destroy(m_ShadowObject);
		m_NavAgent.Stop(true);
		m_NavAgent.updatePosition = false;
		if (Globals.m_PlayerController.m_TargetedEnemy == base.gameObject)
		{
			Globals.m_PlayerController.m_TargetedEnemy = null;
		}
	}

	public void TakedownLethal()
	{
		Globals.m_AIDirector.EnemyKilled(this);
		m_EnemyState = EnemyState.Death;
		m_PlayerWarned = false;
		m_CurrentHealth = 0;
		if (m_Bursting)
		{
			StopBurstFire();
		}
		m_StateTimer = 3.5f;
		DropWeapon();
	}

	public void TakedownNonLethal()
	{
		Globals.m_AIDirector.EnemyKilled(this);
		m_EnemyState = EnemyState.Death;
		m_PlayerWarned = false;
		m_CurrentHealth = 0;
		if (m_Bursting)
		{
			StopBurstFire();
		}
		m_StateTimer = 3.5f;
		DropWeapon();
	}

	public virtual void PlayVO(string eventName, int Priority)
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

	public virtual void PlayVO(AudioEvent VOEvent, bool AtLeastOneOtherSquadmate)
	{
		switch (VOEvent)
		{
		case AudioEvent.LostVisuals:
			PlayVO("Play_VO_Agitated_Lost_Visual", 76);
			break;
		case AudioEvent.DeadBodyFound:
			if (AtLeastOneOtherSquadmate)
			{
				PlayVO("Play_VO_Dead_Body", 71);
			}
			break;
		case AudioEvent.HeardWeaponFire:
			PlayVO("Play_VO_Weapon_Fire", 73);
			break;
		}
	}

	public override Vector3 GetChestLocation()
	{
		return m_Waist.position + Vector3.up * 0f;
	}

	protected override void AttachShadowObject()
	{
		m_ShadowObject.transform.parent = null;
		m_ShadowObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	protected override void UpdateShadowObjectPosition()
	{
		Vector3 center = m_MeshRenderer.bounds.center;
		center.y = base.transform.position.y;
		m_ShadowObject.transform.position = center;
	}

	public XmlElement SaveGame(XmlElement root, XmlDocument doc)
	{
		if (m_CurrentHealth <= 0)
		{
			return null;
		}
		XmlElement xmlElement = null;
		XmlNodeList elementsByTagName = root.GetElementsByTagName("Enemies");
		foreach (XmlNode item in elementsByTagName)
		{
			if (item.Name == "Enemies")
			{
				xmlElement = (XmlElement)item;
				break;
			}
		}
		if (xmlElement == null)
		{
			return null;
		}
		XmlElement xmlElement2 = (XmlElement)xmlElement.AppendChild(doc.CreateElement("Enemy"));
		xmlElement2.SetAttribute("Spawn_Position", m_SpawnLocation.x + "," + m_SpawnLocation.y + "," + m_SpawnLocation.z);
		xmlElement2.SetAttribute("Position", base.transform.position.x + "," + base.transform.position.y + "," + base.transform.position.z);
		xmlElement2.SetAttribute("RigRotation", m_RigMotion.transform.rotation.x + "," + m_RigMotion.transform.rotation.y + "," + m_RigMotion.transform.rotation.z + "," + m_RigMotion.transform.rotation.w);
		xmlElement2.SetAttribute("Rotation", base.transform.rotation.x + "," + base.transform.rotation.y + "," + base.transform.rotation.z + "," + base.transform.rotation.w);
		xmlElement2.SetAttribute("CurrentHealth", m_CurrentHealth.ToString());
		xmlElement2.SetAttribute("EnemyState", m_EnemyState.ToString());
		xmlElement2.SetAttribute("PatrolState", m_PatrolState.ToString());
		xmlElement2.SetAttribute("SuspiciousState", m_SuspiciousState.ToString());
		xmlElement2.SetAttribute("AlarmedState", m_AlarmedState.ToString());
		xmlElement2.SetAttribute("CombatState", m_CombatState.ToString());
		xmlElement2.SetAttribute("HostileState", m_HostileState.ToString());
		xmlElement2.SetAttribute("WarningState", m_WarningState.ToString());
		xmlElement2.SetAttribute("StateTimer", m_StateTimer.ToString());
		xmlElement2.SetAttribute("EventTimer", m_EventTimer.ToString());
		xmlElement2.SetAttribute("CooldownTimer", m_CooldownTimer.ToString());
		xmlElement2.SetAttribute("MoveTimer", m_MoveTimer.ToString());
		xmlElement2.SetAttribute("DisturbanceIgnoreTimer", m_DisturbanceIgnoreTimer.ToString());
		if (m_NavMeshPath != null && m_NavMeshPath.corners != null && m_NavMeshPath.corners.Length > 0)
		{
			xmlElement2.SetAttribute("NavMeshPathDestination", m_NavMeshPath.corners[m_NavMeshPath.corners.Length - 1].x + "," + m_NavMeshPath.corners[m_NavMeshPath.corners.Length - 1].y + "," + m_NavMeshPath.corners[m_NavMeshPath.corners.Length - 1].z);
		}
		if (m_TargetNode != null)
		{
			xmlElement2.SetAttribute("TargetPatrolNode", m_TargetNode.transform.position.x + "," + m_TargetNode.transform.position.y + "," + m_TargetNode.transform.position.z);
		}
		xmlElement2.SetAttribute("PatrolForward", m_PatrolForward.ToString());
		if (m_EventNode != null)
		{
			xmlElement2.SetAttribute("EventNode", m_EventNode.transform.position.x + "," + m_EventNode.transform.position.y + "," + m_EventNode.transform.position.z);
		}
		xmlElement2.SetAttribute("WPDir", m_WPDir.ToString());
		if (m_EnemyState == EnemyState.Patrol)
		{
			switch (m_PatrolState)
			{
			case PatrolState.TurningToWaypoint:
			case PatrolState.TurningFromWaypoint:
				xmlElement2.SetAttribute("WaypointAnimTime", m_Animator[m_TransitionalAnimation].time.ToString());
				xmlElement2.SetAttribute("TurningWaypointAnim", m_TransitionalAnimation);
				xmlElement2.SetAttribute("TurningWaypointPosedAnim", m_PosedAnimation);
				break;
			case PatrolState.AtWaypoint:
				xmlElement2.SetAttribute("WaypointAnimTime", m_Animator[m_TransitionalAnimation].time.ToString());
				break;
			}
		}
		if (m_EnemyState == EnemyState.Combat)
		{
			if (m_ChosenCover != null)
			{
				xmlElement2.SetAttribute("ChosenCover", m_ChosenCover.m_Cover.transform.position.x + "," + m_ChosenCover.m_Cover.transform.position.y + "," + m_ChosenCover.m_Cover.transform.position.z);
			}
			if (m_LastChosenCover != null)
			{
				xmlElement2.SetAttribute("LastChosenCover", m_LastChosenCover.m_Cover.transform.position.x + "," + m_LastChosenCover.m_Cover.transform.position.y + "," + m_LastChosenCover.m_Cover.transform.position.z);
			}
			xmlElement2.SetAttribute("CoverStatus", m_CoverStatus.ToString());
		}
		return xmlElement2;
	}

	public void LoadGame(XmlElement el)
	{
		Vector3 sourceLocation = Vector3.zero;
		EnemyState enemyState = EnemyState.None;
		float time = 0f;
		string transitionalAnimation = string.Empty;
		string posedAnimation = string.Empty;
		foreach (XmlAttribute attribute in el.Attributes)
		{
			switch (attribute.Name)
			{
			case "Position":
			{
				string[] array = attribute.InnerText.Split(',');
				base.transform.position = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				break;
			}
			case "Rotation":
			{
				string[] array = attribute.InnerText.Split(',');
				base.transform.rotation = new Quaternion(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
				break;
			}
			case "RigRotation":
			{
				string[] array = attribute.InnerText.Split(',');
				m_RigMotion.transform.rotation = new Quaternion(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
				break;
			}
			case "CurrentHealth":
				m_CurrentHealth = int.Parse(attribute.InnerText);
				break;
			case "EnemyState":
				enemyState = (EnemyState)(int)Enum.Parse(typeof(EnemyState), attribute.InnerText);
				break;
			case "PatrolState":
				m_PatrolState = (PatrolState)(int)Enum.Parse(typeof(PatrolState), attribute.InnerText);
				break;
			case "CombatState":
				m_CombatState = (CombatState)(int)Enum.Parse(typeof(CombatState), attribute.InnerText);
				break;
			case "StateTimer":
				m_StateTimer = float.Parse(attribute.InnerText);
				break;
			case "EventTimer":
				m_EventTimer = float.Parse(attribute.InnerText);
				break;
			case "NavMeshPathDestination":
			{
				string[] array = attribute.InnerText.Split(',');
				sourceLocation = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				break;
			}
			case "TargetPatrolNode":
			{
				string[] array = attribute.InnerText.Split(',');
				Vector3 vector = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				PatrolNode[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(PatrolNode)) as PatrolNode[];
				PatrolNode[] array4 = array2;
				foreach (PatrolNode patrolNode2 in array4)
				{
					if (patrolNode2.transform.position == vector)
					{
						m_TargetNode = patrolNode2;
						break;
					}
				}
				break;
			}
			case "PatrolForward":
				m_PatrolForward = bool.Parse(attribute.InnerText);
				break;
			case "EventNode":
			{
				string[] array = attribute.InnerText.Split(',');
				Vector3 vector = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				PatrolNode[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(PatrolNode)) as PatrolNode[];
				PatrolNode[] array3 = array2;
				foreach (PatrolNode patrolNode in array3)
				{
					if (patrolNode.transform.position == vector)
					{
						m_EventNode = patrolNode;
						break;
					}
				}
				break;
			}
			case "WPDir":
				m_WPDir = (PatrolNode.WaypointDirection)(int)Enum.Parse(typeof(PatrolNode.WaypointDirection), attribute.InnerText);
				break;
			case "WaypointAnimTime":
				time = float.Parse(attribute.InnerText);
				break;
			case "TurningWaypointAnim":
				transitionalAnimation = attribute.InnerText;
				break;
			case "TurningWaypointPosedAnim":
				posedAnimation = attribute.InnerText;
				break;
			case "ChosenCover":
			{
				string[] array = attribute.InnerText.Split(',');
				Vector3 vector = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				for (LinkedListNode<NearbyCover> linkedListNode = Globals.m_AIDirector.m_CoverNodes.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					if (linkedListNode.Value.m_Cover.transform.position == vector)
					{
						m_ChosenCover = linkedListNode.Value;
						break;
					}
				}
				break;
			}
			case "LastChosenCover":
			{
				string[] array = attribute.InnerText.Split(',');
				Vector3 vector = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				for (LinkedListNode<NearbyCover> linkedListNode = Globals.m_AIDirector.m_CoverNodes.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					if (linkedListNode.Value.m_Cover.transform.position == vector)
					{
						m_ChosenCover = linkedListNode.Value;
						break;
					}
				}
				break;
			}
			case "CoverStatus":
				m_CoverStatus = (CoverStatus)(int)Enum.Parse(typeof(CoverStatus), attribute.InnerText);
				break;
			}
		}
		switch (enemyState)
		{
		case EnemyState.Patrol:
			m_EnemyState = EnemyState.Patrol;
			switch (m_PatrolState)
			{
			case PatrolState.StandingIdle:
				m_Animator.CrossFade("Idle_Standing");
				break;
			case PatrolState.AtWaypoint:
				m_TransitionalAnimation = "Waypoint_DeskSearch";
				m_Animator.CrossFade("Waypoint_DeskSearch");
				m_Animator["Waypoint_DeskSearch"].time = time;
				break;
			case PatrolState.TurningToWaypoint:
			case PatrolState.TurningFromWaypoint:
				m_TransitionalAnimation = transitionalAnimation;
				m_PosedAnimation = posedAnimation;
				m_Animator.CrossFade(transitionalAnimation);
				m_Animator[transitionalAnimation].time = time;
				RecalibrateBase();
				break;
			default:
				StartPatrol(true);
				break;
			}
			break;
		case EnemyState.Suspicious:
			InvestigateSuspiciousSound(sourceLocation, false, true);
			break;
		case EnemyState.Alarmed:
			InvestigateAlarmingSound(sourceLocation, false, true);
			break;
		case EnemyState.Combat:
			switch (m_CombatState)
			{
			case CombatState.StandAtCover:
			case CombatState.InCover:
			case CombatState.CoverFiring:
				if (m_CoverStatus == CoverStatus.CrouchingLeft)
				{
					m_Animator.CrossFade("Idle_CrouchLeft");
				}
				else if (m_CoverStatus == CoverStatus.CrouchingRight)
				{
					m_Animator.CrossFade("Idle_CrouchRight");
				}
				else if (m_CoverStatus == CoverStatus.StandingLeft)
				{
					m_Animator.CrossFade("Idle_StandLeft");
				}
				else
				{
					m_Animator.CrossFade("Idle_StandRight");
				}
				AdjustCollider();
				if (m_CoverStatus == CoverStatus.CrouchingLeft || m_CoverStatus == CoverStatus.StandingLeft)
				{
					SwapWeaponHands(false, true);
				}
				m_WantsNewCover = false;
				m_WantToExitCover = false;
				m_EnemyState = EnemyState.Combat;
				m_CombatState = CombatState.InCover;
				Globals.m_AIDirector.EnemyEnteredCombat(this, Globals.m_PlayerController.transform.position);
				break;
			default:
				StartPatrol(false);
				break;
			}
			break;
		default:
			StartPatrol(false);
			break;
		}
	}

	private bool WarnPlayer()
	{
		if (m_TransitionalAnimation != null)
		{
			return false;
		}
		if (m_EnemyState == EnemyState.Suspicious)
		{
			Globals.m_AIDirector.EnemyIsNoLongerSuspicious(this);
		}
		else if (m_EnemyState == EnemyState.Alarmed)
		{
			Globals.m_AIDirector.EnemyIsNoLongerAlarmed(this);
		}
		m_Animator.CrossFade("Idle_Standing");
		m_PlayerWarned = true;
		if (m_NavMoving)
		{
			m_NavAgent.Stop(true);
			m_NavMoving = false;
			m_NavAgent.updateRotation = false;
		}
		m_EnemyState = EnemyState.WarningPlayer;
		m_WarningState = WarningState.StandingIdle;
		PlayVO("Play_VO_Zone_Warning", 95);
		return true;
	}

	public virtual void UpdateWarning()
	{
		if (m_WarningState == WarningState.StandingIdle)
		{
			UpdateWarningStandingIdle();
		}
		bool flag = CheckVisualSenses();
		if (m_TransitionalAnimation == null && (!flag || HostilityZone.GetPlayerHostilityLevel() == HostilityLevel.None))
		{
			m_Animator.CrossFade("Idle_Standing");
			ResumePatrolImmediate();
		}
	}

	public virtual void UpdateWarningStandingIdle()
	{
		if (m_TransitionalAnimation != null && !m_Animator.IsPlaying(m_TransitionalAnimation))
		{
			RecalibrateBase();
			if (m_PosedAnimation != null)
			{
				m_Animator.Play(m_PosedAnimation);
			}
			m_Animator.CrossFade("Idle_Standing");
			m_PosedAnimation = null;
			m_TransitionalAnimation = null;
		}
		DetermineSourceDirection(Globals.m_PlayerController.transform.position);
		WarningTurn(57.29578f * Mathf.Acos(m_ToSourceDot));
	}

	public virtual void WarningTurn(float HAngle)
	{
		if (m_TransitionalAnimation != null)
		{
			return;
		}
		if (m_ToSourceRightDot >= 0f)
		{
			if (m_ToSourceDot < 0f && HAngle >= 180f - m_RightAimingExtentInDegrees)
			{
				m_TransitionalAnimation = "TurnPatrol_East180";
			}
			else if (HAngle >= m_RightAimingExtentInDegrees)
			{
				m_TransitionalAnimation = "TurnPatrol_East90";
			}
		}
		else if (m_ToSourceDot < 0f && HAngle >= 180f - m_RightAimingExtentInDegrees)
		{
			m_TransitionalAnimation = "TurnPatrol_West180";
		}
		else if (HAngle >= m_LeftAimingExtentInDegrees)
		{
			m_TransitionalAnimation = "TurnPatrol_West90";
		}
		if (m_TransitionalAnimation != null)
		{
			m_PosedAnimation = m_TransitionalAnimation + "_Pose";
			m_Animator.CrossFade(m_TransitionalAnimation);
		}
	}
}
