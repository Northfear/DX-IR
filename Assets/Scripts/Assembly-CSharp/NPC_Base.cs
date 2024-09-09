using UnityEngine;

public class NPC_Base : CharacterBase
{
	public enum NPCState
	{
		None = -1,
		Patrolling = 0,
		Cowering = 1,
		Dying = 2,
		Total = 3
	}

	public enum PatrolState
	{
		None = -1,
		StandingIdle = 0,
		WalkingPatrol = 1,
		TurningToWaypoint = 2,
		AtWaypoint = 3,
		TurningTowardPatrol = 4,
		TalkingToPlayer = 5,
		Total = 6
	}

	private const float m_DeathFadeTime = 3.5f;

	public Animation m_BodyAnimator;

	public Animation m_HeadAnimator;

	public FaceFXControllerScript m_FaceFX;

	public Renderer m_BodyRenderer;

	public Renderer m_HeadRenderer;

	public NavMeshAgent m_NavAgent;

	public Transform m_Neck;

	private Quaternion m_NeckOffset = Quaternion.identity;

	public CapsuleCollider m_Collider;

	public bool m_Invulnerable;

	protected NPCState m_NPCState = NPCState.None;

	protected PatrolState m_PatrolState = PatrolState.None;

	protected float m_WalkSpeed = 1.5f;

	protected float m_WalkTurnSpeed = 5f;

	[HideInInspector]
	public bool m_Paused;

	[HideInInspector]
	public float m_CurrentAnimationSpeed = 1f;

	protected float m_StateTimer;

	public NPC_PatrolNode m_TargetNode;

	public bool m_PatrolForward = true;

	private NPC_PatrolNode m_EventNode;

	private NPC_PatrolNode.WaypointDirection m_WPDir;

	protected string m_TransitionalAnimation;

	protected string m_PosedAnimation;

	protected Quaternion m_OriginalTurnDirection = Quaternion.identity;

	protected Quaternion m_TargetTurnDirection = Quaternion.identity;

	private bool m_HasSpokenDialog;

	public string m_ExposedGunAnim;

	public AudioClip m_ExposedGunClip;

	public string m_InitialDialogAnim;

	public AudioClip m_InitialDialogClip;

	public string m_SecondaryDialogAnim;

	public AudioClip m_SecondaryDialogClip;

	public virtual void Awake()
	{
	}

	protected override void Start()
	{
		base.Start();
		Globals.m_AIDirector.NPCSpawned(this);
		m_NPCState = NPCState.Patrolling;
		m_PatrolState = PatrolState.StandingIdle;
		if (m_TargetNode != null)
		{
			NPC_PatrolNode nPC_PatrolNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
			base.transform.position = m_TargetNode.transform.position;
			if (nPC_PatrolNode != null)
			{
				m_PatrolState = PatrolState.WalkingPatrol;
				Vector3 vector = nPC_PatrolNode.transform.position - m_TargetNode.transform.position;
				vector.y = 0f;
				base.transform.rotation = Quaternion.LookRotation(vector.normalized);
			}
		}
		RayClampToGround();
		if (m_PatrolState == PatrolState.WalkingPatrol)
		{
			m_BodyAnimator.CrossFade("vanillaWalk");
			m_HeadAnimator.CrossFade("vanillaWalk");
		}
		else
		{
			m_BodyAnimator.CrossFade("StandingIdle");
			m_HeadAnimator.CrossFade("StandingIdle");
		}
		m_NavAgent.Stop(true);
		m_NavAgent.updateRotation = false;
		m_NavAgent.updatePosition = true;
	}

	protected override void Update()
	{
		base.Update();
		if (!m_Paused)
		{
			switch (m_NPCState)
			{
			case NPCState.Patrolling:
				UpdateNPCPatrolling();
				break;
			case NPCState.Cowering:
				UpdateNPCCowering();
				break;
			case NPCState.Dying:
				UpdateNPCDying();
				break;
			}
			RayClampToGround();
		}
	}

	private void LateUpdate()
	{
	}

	private void UpdateNPCPatrolling()
	{
		switch (m_PatrolState)
		{
		case PatrolState.StandingIdle:
			UpdateNPCStandingIdle();
			break;
		case PatrolState.WalkingPatrol:
			UpdateNPCWalkingPatrol();
			break;
		case PatrolState.TurningToWaypoint:
			UpdateNPCTurningToWaypoint();
			break;
		case PatrolState.AtWaypoint:
			UpdateNPCAtWaypoint();
			break;
		case PatrolState.TurningTowardPatrol:
			UpdateNPCTurningTowardPatrol();
			break;
		case PatrolState.TalkingToPlayer:
			UpdateNPCTalkingToPlayer();
			break;
		}
	}

	private void UpdateNPCStandingIdle()
	{
		m_StateTimer -= Time.deltaTime;
		if (!(m_StateTimer <= 0f))
		{
			return;
		}
		if (m_EventNode != null)
		{
			if (m_EventNode.m_PatrolEvent == NPC_PatrolNode.PatrolEvent.Waypoint)
			{
				m_WPDir = m_EventNode.m_WaypointDirection;
				if (!m_PatrolForward)
				{
					if (m_WPDir == NPC_PatrolNode.WaypointDirection.Right)
					{
						m_WPDir = NPC_PatrolNode.WaypointDirection.Left;
					}
					else if (m_WPDir == NPC_PatrolNode.WaypointDirection.Left)
					{
						m_WPDir = NPC_PatrolNode.WaypointDirection.Right;
					}
				}
				m_TransitionalAnimation = null;
				if (m_WPDir == NPC_PatrolNode.WaypointDirection.Left)
				{
					m_TransitionalAnimation = "TurnLeft";
					m_PosedAnimation = "TurnLeft_Pose";
					m_OriginalTurnDirection = base.transform.rotation;
					m_TargetTurnDirection = Quaternion.LookRotation(new Vector3(0f - base.transform.right.x, 0f, 0f - base.transform.right.z));
					m_BodyAnimator.CrossFade(m_TransitionalAnimation);
					m_HeadAnimator.CrossFade(m_TransitionalAnimation);
				}
				else if (m_WPDir == NPC_PatrolNode.WaypointDirection.Right)
				{
					m_TransitionalAnimation = "TurnRight";
					m_PosedAnimation = "TurnRight_Pose";
					m_OriginalTurnDirection = base.transform.rotation;
					m_TargetTurnDirection = Quaternion.LookRotation(new Vector3(base.transform.right.x, 0f, base.transform.right.z));
					m_BodyAnimator.CrossFade(m_TransitionalAnimation);
					m_HeadAnimator.CrossFade(m_TransitionalAnimation);
				}
				else
				{
					m_OriginalTurnDirection = base.transform.rotation;
					m_TargetTurnDirection = m_OriginalTurnDirection;
				}
				m_PatrolState = PatrolState.TurningToWaypoint;
			}
			else if (m_TargetNode != null)
			{
				m_BodyAnimator.CrossFade("vanillaWalk");
				m_HeadAnimator.CrossFade("vanillaWalk");
				m_PatrolState = PatrolState.WalkingPatrol;
				m_EventNode = null;
			}
			else
			{
				m_EventNode = null;
			}
		}
		else if (m_TargetNode != null)
		{
			m_BodyAnimator.CrossFade("vanillaWalk");
			m_HeadAnimator.CrossFade("vanillaWalk");
			m_PatrolState = PatrolState.WalkingPatrol;
		}
	}

	private void UpdateNPCWalkingPatrol()
	{
		Vector3 vector = m_TargetNode.transform.position - base.transform.position;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		float num = m_WalkSpeed * Time.deltaTime;
		if (magnitude <= num)
		{
			base.transform.position = m_TargetNode.transform.position;
			num -= magnitude;
			if (NPCPatrolEventTriggered())
			{
				m_StateTimer = Random.Range(m_EventNode.m_MinIdle, m_EventNode.m_MaxIdle);
				if (m_EventNode.m_PatrolEvent == NPC_PatrolNode.PatrolEvent.TurnBack)
				{
					m_PatrolForward = !m_PatrolForward;
				}
				m_TargetNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
				m_PatrolState = PatrolState.StandingIdle;
				m_BodyAnimator.CrossFade("StandingIdle");
				m_HeadAnimator.CrossFade("StandingIdle");
				return;
			}
			m_TargetNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
			if (!(m_TargetNode != null))
			{
				m_PatrolState = PatrolState.StandingIdle;
				m_BodyAnimator.CrossFade("StandingIdle");
				m_HeadAnimator.CrossFade("StandingIdle");
				return;
			}
			vector = m_TargetNode.transform.position - base.transform.position;
			vector.y = 0f;
		}
		float maxDegreesDelta = ((!(magnitude <= 1f)) ? m_WalkTurnSpeed : (m_WalkTurnSpeed * 2f));
		Quaternion to = Quaternion.LookRotation(vector.normalized);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, maxDegreesDelta);
		m_NavAgent.Move(base.transform.forward * num);
	}

	private void UpdateNPCTurningToWaypoint()
	{
		if (m_TransitionalAnimation != null)
		{
			float t = 1f;
			if (m_BodyAnimator.IsPlaying(m_TransitionalAnimation))
			{
				t = Mathf.Min(m_BodyAnimator[m_TransitionalAnimation].time / m_BodyAnimator[m_TransitionalAnimation].clip.length, 1f);
			}
			base.transform.rotation = Quaternion.Lerp(m_OriginalTurnDirection, m_TargetTurnDirection, t);
		}
		if (m_TransitionalAnimation == null || !m_BodyAnimator.IsPlaying(m_TransitionalAnimation))
		{
			if (m_PosedAnimation != null)
			{
				m_BodyAnimator.Play(m_PosedAnimation);
				m_HeadAnimator.Play(m_PosedAnimation);
			}
			m_TransitionalAnimation = m_EventNode.m_WaypointAnimation;
			m_PosedAnimation = null;
			m_BodyAnimator.CrossFade(m_TransitionalAnimation);
			m_HeadAnimator.CrossFade(m_TransitionalAnimation);
			if (m_EventNode.m_DialogClip != null && m_EventNode.m_DialogAnimation != null && m_EventNode.m_DialogAnimation.Length > 0)
			{
				SpeakDialog(m_EventNode.m_DialogAnimation, m_EventNode.m_DialogClip);
			}
			m_PatrolState = PatrolState.AtWaypoint;
		}
	}

	private void UpdateNPCAtWaypoint()
	{
		if (m_TransitionalAnimation == null || !m_BodyAnimator.IsPlaying(m_TransitionalAnimation))
		{
			m_TransitionalAnimation = null;
			if (m_WPDir == NPC_PatrolNode.WaypointDirection.Right)
			{
				m_TransitionalAnimation = "TurnLeft";
				m_PosedAnimation = "TurnLeft_Pose";
				m_OriginalTurnDirection = base.transform.rotation;
				m_TargetTurnDirection = Quaternion.LookRotation(new Vector3(0f - base.transform.right.x, 0f, 0f - base.transform.right.z));
				m_BodyAnimator.CrossFade(m_TransitionalAnimation);
				m_HeadAnimator.CrossFade(m_TransitionalAnimation);
			}
			else if (m_WPDir == NPC_PatrolNode.WaypointDirection.Left)
			{
				m_TransitionalAnimation = "TurnRight";
				m_PosedAnimation = "TurnRight_Pose";
				m_OriginalTurnDirection = base.transform.rotation;
				m_TargetTurnDirection = Quaternion.LookRotation(new Vector3(base.transform.right.x, 0f, base.transform.right.z));
				m_BodyAnimator.CrossFade(m_TransitionalAnimation);
				m_HeadAnimator.CrossFade(m_TransitionalAnimation);
			}
			else if (m_TargetNode != null)
			{
				m_BodyAnimator.CrossFade("vanillaWalk");
				m_HeadAnimator.CrossFade("vanillaWalk");
				m_PatrolState = PatrolState.WalkingPatrol;
			}
			else
			{
				m_BodyAnimator.CrossFade("StandingIdle");
				m_HeadAnimator.CrossFade("StandingIdle");
				m_PatrolState = PatrolState.StandingIdle;
			}
			m_EventNode = null;
			m_PatrolState = PatrolState.TurningTowardPatrol;
		}
	}

	private void UpdateNPCTurningTowardPatrol()
	{
		if (m_TransitionalAnimation != null)
		{
			float t = 1f;
			if (m_BodyAnimator.IsPlaying(m_TransitionalAnimation))
			{
				t = Mathf.Min(m_BodyAnimator[m_TransitionalAnimation].time / m_BodyAnimator[m_TransitionalAnimation].clip.length, 1f);
			}
			base.transform.rotation = Quaternion.Lerp(m_OriginalTurnDirection, m_TargetTurnDirection, t);
		}
		if (m_TransitionalAnimation == null || !m_BodyAnimator.IsPlaying(m_TransitionalAnimation))
		{
			if (m_PosedAnimation != null)
			{
				m_BodyAnimator.Play(m_PosedAnimation);
				m_HeadAnimator.Play(m_PosedAnimation);
			}
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
			if (m_TargetNode != null)
			{
				m_BodyAnimator.CrossFade("vanillaWalk");
				m_HeadAnimator.CrossFade("vanillaWalk");
				m_PatrolState = PatrolState.WalkingPatrol;
			}
			else
			{
				m_BodyAnimator.CrossFade("StandingIdle");
				m_HeadAnimator.CrossFade("StandingIdle");
				m_PatrolState = PatrolState.StandingIdle;
			}
		}
	}

	private void UpdateNPCTalkingToPlayer()
	{
		if (m_TransitionalAnimation != null)
		{
			float t = 1f;
			if (m_BodyAnimator.IsPlaying(m_TransitionalAnimation))
			{
				t = Mathf.Min(m_BodyAnimator[m_TransitionalAnimation].time / m_BodyAnimator[m_TransitionalAnimation].clip.length, 1f);
			}
			base.transform.rotation = Quaternion.Lerp(m_OriginalTurnDirection, m_TargetTurnDirection, t);
		}
		if (m_TransitionalAnimation == null || !m_BodyAnimator.IsPlaying(m_TransitionalAnimation))
		{
			if (m_PosedAnimation != null)
			{
				m_BodyAnimator.Play(m_PosedAnimation);
				m_HeadAnimator.Play(m_PosedAnimation);
				m_PosedAnimation = null;
			}
			if (!CurrentlySpeakingDialog())
			{
				m_TransitionalAnimation = null;
				m_StateTimer = Random.Range(2f, 5f);
				m_BodyAnimator.CrossFade("StandingIdle");
				m_HeadAnimator.CrossFade("StandingIdle");
				m_PatrolState = PatrolState.StandingIdle;
			}
		}
	}

	private void UpdateNPCCowering()
	{
		m_StateTimer -= Time.deltaTime;
		if (m_StateTimer <= 0f)
		{
			m_TransitionalAnimation = null;
			m_NPCState = NPCState.Patrolling;
			m_StateTimer = Random.Range(1f, 4f);
			m_BodyAnimator.CrossFade("StandingIdle");
			m_HeadAnimator.CrossFade("StandingIdle");
			m_PatrolState = PatrolState.StandingIdle;
		}
	}

	private void UpdateNPCDying()
	{
		if (!m_BodyAnimator.IsPlaying(m_TransitionalAnimation))
		{
			m_StateTimer -= Time.deltaTime;
			if (m_StateTimer <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
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

	public void Cower()
	{
		if (m_NPCState == NPCState.Dying)
		{
			return;
		}
		if (m_NPCState == NPCState.Cowering)
		{
			if (m_StateTimer <= 8f)
			{
				m_StateTimer += Random.Range(3f, 8f);
			}
		}
		else
		{
			m_NPCState = NPCState.Cowering;
			m_FaceFX.StopAnim();
			m_StateTimer = Random.Range(10f, 20f);
			m_BodyAnimator.CrossFade("cowerIdle");
			m_HeadAnimator.CrossFade("cowerIdle");
		}
	}

	public bool Interact()
	{
		if (m_NPCState == NPCState.Dying || m_NPCState == NPCState.Cowering)
		{
			return false;
		}
		if (m_NPCState == NPCState.Patrolling && (m_PatrolState == PatrolState.AtWaypoint || m_PatrolState == PatrolState.TurningToWaypoint))
		{
			return false;
		}
		if ((Globals.m_PlayerController.transform.position - base.transform.position).sqrMagnitude >= 15f)
		{
			return false;
		}
		if (!CurrentlySpeakingDialog())
		{
			if (!Globals.m_PlayerController.WeaponHolstered())
			{
				SpeakDialog(m_ExposedGunAnim, m_ExposedGunClip);
			}
			else if (m_HasSpokenDialog)
			{
				SpeakDialog(m_SecondaryDialogAnim, m_SecondaryDialogClip);
			}
			else
			{
				SpeakDialog(m_InitialDialogAnim, m_InitialDialogClip);
				m_HasSpokenDialog = true;
			}
			Vector3 forward = Globals.m_PlayerController.transform.position - base.transform.position;
			Vector3 lhs = Vector3.Normalize(new Vector3(forward.x, 0f, forward.z));
			float num = Vector3.Dot(lhs, base.transform.forward);
			float num2 = Vector3.Dot(lhs, base.transform.right);
			m_TransitionalAnimation = null;
			if (Mathf.Abs(num) >= Mathf.Abs(num2))
			{
				if (num < 0f)
				{
					if (num2 >= 0f)
					{
						m_TransitionalAnimation = "TurnRight";
					}
					else
					{
						m_TransitionalAnimation = "TurnLeft";
					}
				}
				else
				{
					m_BodyAnimator.CrossFade("StandingIdle");
					m_HeadAnimator.CrossFade("StandingIdle");
				}
			}
			else if (num2 >= 0f)
			{
				m_TransitionalAnimation = "TurnRight";
			}
			else
			{
				m_TransitionalAnimation = "TurnLeft";
			}
			if (m_TransitionalAnimation != null)
			{
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_BodyAnimator.CrossFade(m_TransitionalAnimation);
				m_HeadAnimator.CrossFade(m_TransitionalAnimation);
			}
			m_TargetTurnDirection = Quaternion.LookRotation(forward);
			m_OriginalTurnDirection = base.transform.rotation;
			m_NPCState = NPCState.Patrolling;
			m_PatrolState = PatrolState.TalkingToPlayer;
			m_EventNode = null;
			return true;
		}
		return false;
	}

	private bool NPCPatrolEventTriggered()
	{
		if (m_TargetNode.m_PatrolEvent == NPC_PatrolNode.PatrolEvent.None || m_TargetNode.m_EventChance == NPC_PatrolNode.EventChance.Never)
		{
			return false;
		}
		float num = -1f;
		switch (m_TargetNode.m_EventChance)
		{
		case NPC_PatrolNode.EventChance.Always:
			num = 1f;
			break;
		case NPC_PatrolNode.EventChance.Often:
			num = 0.75f;
			break;
		case NPC_PatrolNode.EventChance.Sometimes:
			num = 0.5f;
			break;
		case NPC_PatrolNode.EventChance.Rarely:
			num = 0.25f;
			break;
		case NPC_PatrolNode.EventChance.Once:
			num = 1f;
			m_TargetNode.m_EventChance = NPC_PatrolNode.EventChance.Never;
			break;
		}
		if (Random.value <= num)
		{
			if (m_TargetNode.m_PatrolEvent == NPC_PatrolNode.PatrolEvent.Dialog)
			{
				if (!CurrentlySpeakingDialog() && m_TargetNode.m_DialogClip != null && m_TargetNode.m_DialogAnimation != null && m_TargetNode.m_DialogAnimation.Length > 0)
				{
					SpeakDialog(m_TargetNode.m_DialogAnimation, m_TargetNode.m_DialogClip);
				}
				return false;
			}
			m_EventNode = m_TargetNode;
			return true;
		}
		return false;
	}

	private void SpeakDialog(string anim, AudioClip clip)
	{
		m_HeadAnimator[anim].layer = 1;
		m_HeadAnimator[anim].wrapMode = WrapMode.ClampForever;
		m_HeadAnimator[anim].blendMode = AnimationBlendMode.Blend;
		m_FaceFX.StopAnim();
		m_FaceFX.PlayAnim(anim, clip);
	}

	private bool CurrentlySpeakingDialog()
	{
		return m_FaceFX.GetPlayState() != 0;
	}

	public override bool TakeDamage(int Damage, GameObject Damager, DamageType Type = DamageType.Normal)
	{
		if (m_Invulnerable)
		{
			return false;
		}
		bool flag = base.TakeDamage(Damage, Damager, Type);
		if (!flag)
		{
			Cower();
		}
		return flag;
	}

	public override void Die(GameObject Damager, DamageType Type = DamageType.Normal)
	{
		base.Die(Damager, Type);
	}

	public override Ray WeaponRequestForBulletRay(out bool PlayTracer)
	{
		PlayTracer = true;
		return default(Ray);
	}

	public override void WeaponWantsReload()
	{
	}

	public override void WeaponDoneReloading()
	{
	}

	protected override void AttachShadowObject()
	{
		m_ShadowObject.transform.parent = null;
		m_ShadowObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	protected override void UpdateShadowObjectPosition()
	{
		if (m_BodyRenderer != null)
		{
			Vector3 center = m_BodyRenderer.bounds.center;
			center.y = base.transform.position.y;
			m_ShadowObject.transform.position = center;
		}
	}
}
