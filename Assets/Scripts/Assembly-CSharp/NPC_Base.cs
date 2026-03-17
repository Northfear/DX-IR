using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class NPC_Base : CharacterBase
{
	public enum NPCState
	{
		None = -1,
		Patrolling = 0,
		Cowering = 1,
		Stunned = 2,
		Dying = 3,
		Total = 4
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

	private const float m_DeathFadeTime = 5f;

	public Animation m_NPCAnimator;

	public FaceFXControllerScript m_FaceFX;

	public Transform m_StunGunEffectAttachment;

	public Renderer m_BodyRenderer;

	public Renderer m_HeadRenderer;

	public Material m_BodyDeathMaterial;

	public Material m_HeadDeathMaterial;

	public NavMeshAgent m_NavAgent;

	public Transform m_Neck;

	public CapsuleCollider m_Collider;

	[HideInInspector]
	public Vector3 m_SpawnPosition;

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

	public string m_ExposedGunEvent = string.Empty;

	public string m_InitialDialogAnim;

	public string m_InitialDialogEvent = string.Empty;

	public string m_SecondaryDialogAnim;

	public string m_SecondaryDialogEvent = string.Empty;

	public static LinkedList<Vector3> m_DestroyedNPCs = new LinkedList<Vector3>();

	public override int GetGlobalLayer()
	{
		return 21;
	}

	public virtual void Awake()
	{
		m_SpawnPosition = base.transform.position;
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
			m_NPCAnimator.CrossFade("Walk_Forward");
		}
		else
		{
			m_NPCAnimator.CrossFade("Idle_Standing");
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
			case NPCState.Stunned:
				UpdateNPCStunned();
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
					m_TransitionalAnimation = "Turn_Left";
					m_PosedAnimation = "Turn_Left_Pose";
					m_OriginalTurnDirection = base.transform.rotation;
					m_TargetTurnDirection = Quaternion.LookRotation(new Vector3(0f - base.transform.right.x, 0f, 0f - base.transform.right.z));
					m_NPCAnimator.CrossFade(m_TransitionalAnimation);
				}
				else if (m_WPDir == NPC_PatrolNode.WaypointDirection.Right)
				{
					m_TransitionalAnimation = "Turn_Right";
					m_PosedAnimation = "Turn_Right_Pose";
					m_OriginalTurnDirection = base.transform.rotation;
					m_TargetTurnDirection = Quaternion.LookRotation(new Vector3(base.transform.right.x, 0f, base.transform.right.z));
					m_NPCAnimator.CrossFade(m_TransitionalAnimation);
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
				m_NPCAnimator.CrossFade("Walk_Forward");
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
			m_NPCAnimator.CrossFade("Walk_Forward");
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
				m_StateTimer = UnityEngine.Random.Range(m_EventNode.m_MinIdle, m_EventNode.m_MaxIdle);
				if (m_EventNode.m_PatrolEvent == NPC_PatrolNode.PatrolEvent.TurnBack)
				{
					m_PatrolForward = !m_PatrolForward;
				}
				m_TargetNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
				m_PatrolState = PatrolState.StandingIdle;
				m_NPCAnimator.CrossFade("Idle_Standing");
				return;
			}
			m_TargetNode = ((!m_PatrolForward) ? m_TargetNode.m_ReverseConnection : m_TargetNode.m_ForwardConnection);
			if (!(m_TargetNode != null))
			{
				m_PatrolState = PatrolState.StandingIdle;
				m_NPCAnimator.CrossFade("Idle_Standing");
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
			if (m_NPCAnimator.IsPlaying(m_TransitionalAnimation))
			{
				t = Mathf.Min(m_NPCAnimator[m_TransitionalAnimation].time / m_NPCAnimator[m_TransitionalAnimation].clip.length, 1f);
			}
			base.transform.rotation = Quaternion.Lerp(m_OriginalTurnDirection, m_TargetTurnDirection, t);
		}
		if (m_TransitionalAnimation == null || !m_NPCAnimator.IsPlaying(m_TransitionalAnimation))
		{
			if (m_PosedAnimation != null)
			{
				m_NPCAnimator.Play(m_PosedAnimation);
			}
			m_TransitionalAnimation = m_EventNode.m_WaypointAnimation;
			m_PosedAnimation = null;
			m_NPCAnimator.CrossFade(m_TransitionalAnimation);
			if (m_EventNode.m_DialogEvent != string.Empty && m_EventNode.m_DialogAnimation != null && m_EventNode.m_DialogAnimation.Length > 0)
			{
				SpeakDialog(m_EventNode.m_DialogAnimation, m_EventNode.m_DialogEvent);
			}
			m_PatrolState = PatrolState.AtWaypoint;
		}
	}

	private void UpdateNPCAtWaypoint()
	{
		if (m_TransitionalAnimation == null || !m_NPCAnimator.IsPlaying(m_TransitionalAnimation))
		{
			m_TransitionalAnimation = null;
			if (m_WPDir == NPC_PatrolNode.WaypointDirection.Right)
			{
				m_TransitionalAnimation = "Turn_Left";
				m_PosedAnimation = "Turn_Left_Pose";
				m_OriginalTurnDirection = base.transform.rotation;
				m_TargetTurnDirection = Quaternion.LookRotation(new Vector3(0f - base.transform.right.x, 0f, 0f - base.transform.right.z));
				m_NPCAnimator.CrossFade(m_TransitionalAnimation);
			}
			else if (m_WPDir == NPC_PatrolNode.WaypointDirection.Left)
			{
				m_TransitionalAnimation = "Turn_Right";
				m_PosedAnimation = "Turn_Right_Pose";
				m_OriginalTurnDirection = base.transform.rotation;
				m_TargetTurnDirection = Quaternion.LookRotation(new Vector3(base.transform.right.x, 0f, base.transform.right.z));
				m_NPCAnimator.CrossFade(m_TransitionalAnimation);
			}
			else if (m_TargetNode != null)
			{
				m_NPCAnimator.CrossFade("Walk_Forward");
				m_PatrolState = PatrolState.WalkingPatrol;
			}
			else
			{
				m_NPCAnimator.CrossFade("Idle_Standing");
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
			if (m_NPCAnimator.IsPlaying(m_TransitionalAnimation))
			{
				t = Mathf.Min(m_NPCAnimator[m_TransitionalAnimation].time / m_NPCAnimator[m_TransitionalAnimation].clip.length, 1f);
			}
			base.transform.rotation = Quaternion.Lerp(m_OriginalTurnDirection, m_TargetTurnDirection, t);
		}
		if (m_TransitionalAnimation == null || !m_NPCAnimator.IsPlaying(m_TransitionalAnimation))
		{
			if (m_PosedAnimation != null)
			{
				m_NPCAnimator.Play(m_PosedAnimation);
			}
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
			if (m_TargetNode != null)
			{
				m_NPCAnimator.CrossFade("Walk_Forward");
				m_PatrolState = PatrolState.WalkingPatrol;
			}
			else
			{
				m_NPCAnimator.CrossFade("Idle_Standing");
				m_PatrolState = PatrolState.StandingIdle;
			}
		}
	}

	private void UpdateNPCTalkingToPlayer()
	{
		if (m_TransitionalAnimation != null)
		{
			float t = 1f;
			if (m_NPCAnimator.IsPlaying(m_TransitionalAnimation))
			{
				t = Mathf.Min(m_NPCAnimator[m_TransitionalAnimation].time / m_NPCAnimator[m_TransitionalAnimation].clip.length, 1f);
			}
			base.transform.rotation = Quaternion.Lerp(m_OriginalTurnDirection, m_TargetTurnDirection, t);
		}
		if (m_TransitionalAnimation == null || !m_NPCAnimator.IsPlaying(m_TransitionalAnimation))
		{
			if (m_PosedAnimation != null)
			{
				m_NPCAnimator.Play(m_PosedAnimation);
				m_PosedAnimation = null;
			}
			if (!CurrentlySpeakingDialog())
			{
				m_TransitionalAnimation = null;
				m_StateTimer = UnityEngine.Random.Range(2f, 5f);
				m_NPCAnimator.CrossFade("Idle_Standing");
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
			m_StateTimer = UnityEngine.Random.Range(1f, 4f);
			m_NPCAnimator.CrossFade("Idle_Standing");
			m_PatrolState = PatrolState.StandingIdle;
		}
	}

	private void UpdateNPCStunned()
	{
		if (m_TransitionalAnimation == null || !m_NPCAnimator.IsPlaying(m_TransitionalAnimation))
		{
			if (m_PosedAnimation != null)
			{
				m_NPCAnimator.Play(m_PosedAnimation);
			}
			m_TransitionalAnimation = null;
			m_PosedAnimation = null;
			Cower(true);
		}
	}

	private void UpdateNPCDying()
	{
		if (!m_NPCAnimator.IsPlaying(m_TransitionalAnimation))
		{
			m_StateTimer -= Time.deltaTime;
			m_BodyRenderer.material.SetFloat("_DissolvePower", m_StateTimer / 5f);
			m_HeadRenderer.material.SetFloat("_DissolvePower", m_StateTimer / 5f);
			if (m_StateTimer <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
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

	public void Stun(Vector3 sourceLocation, bool rotate = true)
	{
		if (m_NPCState != NPCState.Dying && m_NPCState != NPCState.Stunned)
		{
			m_NPCState = NPCState.Stunned;
			if (rotate)
			{
				Vector3 vector = sourceLocation - base.transform.position;
				vector.y = 0f;
				base.transform.rotation = Quaternion.LookRotation(vector.normalized);
			}
			m_NPCAnimator.Stop();
			m_TransitionalAnimation = "Stunned_ConcussionGrenade";
			m_PosedAnimation = "Stunned_ConcussionGrenade_Pose";
			m_NPCAnimator.Play(m_TransitionalAnimation);
		}
	}

	public void Cower(bool FromStun = false)
	{
		if (m_NPCState == NPCState.Dying || (m_NPCState == NPCState.Stunned && !FromStun))
		{
			return;
		}
		if (m_NPCState == NPCState.Cowering)
		{
			if (m_StateTimer <= 8f)
			{
				m_StateTimer += UnityEngine.Random.Range(3f, 8f);
			}
		}
		else
		{
			m_NPCState = NPCState.Cowering;
			m_FaceFX.StopAnim();
			m_StateTimer = UnityEngine.Random.Range(10f, 20f);
			m_NPCAnimator.CrossFade("Idle_Cower");
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
				SpeakDialog(m_ExposedGunAnim, m_ExposedGunEvent);
			}
			else if (m_HasSpokenDialog)
			{
				SpeakDialog(m_SecondaryDialogAnim, m_SecondaryDialogEvent);
			}
			else
			{
				SpeakDialog(m_InitialDialogAnim, m_InitialDialogEvent);
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
						m_TransitionalAnimation = "Turn_Right";
					}
					else
					{
						m_TransitionalAnimation = "Turn_Left";
					}
				}
				else
				{
					m_NPCAnimator.CrossFade("Idle_Standing");
				}
			}
			else if (num2 >= 0f)
			{
				m_TransitionalAnimation = "Turn_Right";
			}
			else
			{
				m_TransitionalAnimation = "Turn_Left";
			}
			if (m_TransitionalAnimation != null)
			{
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_NPCAnimator.CrossFade(m_TransitionalAnimation);
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
		if (UnityEngine.Random.value <= num)
		{
			if (m_TargetNode.m_PatrolEvent == NPC_PatrolNode.PatrolEvent.Dialog)
			{
				if (!CurrentlySpeakingDialog() && m_TargetNode.m_DialogEvent != string.Empty && m_TargetNode.m_DialogAnimation != null && m_TargetNode.m_DialogAnimation.Length > 0)
				{
					SpeakDialog(m_TargetNode.m_DialogAnimation, m_TargetNode.m_DialogEvent);
				}
				return false;
			}
			m_EventNode = m_TargetNode;
			return true;
		}
		return false;
	}

	private void SpeakDialog(string anim, string clip)
	{
		m_NPCAnimator[anim].layer = 1;
		m_NPCAnimator[anim].wrapMode = WrapMode.ClampForever;
		m_NPCAnimator[anim].blendMode = AnimationBlendMode.Blend;
		m_FaceFX.StopAnim();
		m_FaceFX.PlayAnim(anim, clip);
	}

	private bool CurrentlySpeakingDialog()
	{
		return m_FaceFX.GetPlayState() != 0;
	}

	public override bool TakeDamage(DamageData data)
	{
		if (m_Invulnerable)
		{
			return false;
		}
		if (data.m_Headshot)
		{
			data.m_Damage *= 2;
		}
		bool flag = base.TakeDamage(data);
		if (!flag)
		{
			if (data.m_DamageType == DamageType.Concussion)
			{
				Stun(data.m_SourceLocation, true);
			}
			else
			{
				Cower(false);
			}
		}
		return flag;
	}

	public override void Die(DamageData data)
	{
		if (m_NPCState != NPCState.Dying)
		{
			base.Die(data);
			Globals.m_AIDirector.NPCKilled(this);
			m_NPCState = NPCState.Dying;
			if ((bool)m_Collider)
			{
				m_Collider.enabled = false;
			}
			m_NavAgent.Stop(true);
			m_NavAgent.updateRotation = false;
			PlayDeathAnimation(data);
			m_StateTimer = 5f;
			m_BodyRenderer.material = m_BodyDeathMaterial;
			m_HeadRenderer.material = m_HeadDeathMaterial;
			m_DestroyedNPCs.AddLast(m_SpawnPosition);
		}
	}

	private void PlayDeathAnimation(DamageData data)
	{
		if (data.m_DamageType == DamageType.NonLethal)
		{
			bool flag = UnityEngine.Random.value <= 0.5f;
			Ray ray = new Ray(base.transform.position + Vector3.up * 0.25f, (!flag) ? (-base.transform.forward) : base.transform.forward);
			if ((!Physics.Raycast(ray, 2f, 6374145)) ? flag : (!flag))
			{
				m_TransitionalAnimation = "Death_DartFallForward";
			}
			else
			{
				m_TransitionalAnimation = "Death_DartFallBack";
			}
			m_NPCAnimator.CrossFade(m_TransitionalAnimation);
			return;
		}
		if (data.m_DamageType == DamageType.StunGun)
		{
			bool flag2 = UnityEngine.Random.value <= 0.5f;
			Ray ray2 = new Ray(base.transform.position + Vector3.up * 0.25f, (!flag2) ? (-base.transform.forward) : base.transform.forward);
			if ((!Physics.Raycast(ray2, 2f, 6374145)) ? flag2 : (!flag2))
			{
				m_TransitionalAnimation = "Death_StunGun_Forward";
			}
			else
			{
				m_TransitionalAnimation = "Death_StunGun_Back";
			}
			m_NPCAnimator.CrossFade(m_TransitionalAnimation);
			GameObject gameObject = UnityEngine.Object.Instantiate(Globals.m_This.m_StunGunEffect) as GameObject;
			gameObject.transform.parent = m_StunGunEffectAttachment;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			return;
		}
		if (data.m_DamageType == DamageType.Explosive)
		{
			Vector3 lhs = data.m_SourceLocation - base.transform.position;
			lhs.y = 0f;
			if (Vector3.Dot(lhs, base.transform.forward) > 0f)
			{
				base.transform.position = base.transform.position;
				base.transform.rotation = Quaternion.LookRotation(lhs.normalized);
				m_TransitionalAnimation = "Death_Explosion";
			}
			else
			{
				base.transform.position = base.transform.position;
				base.transform.rotation = Quaternion.LookRotation(-lhs.normalized);
				m_TransitionalAnimation = "Death_Back_Heavy";
			}
			m_NPCAnimator.Stop();
			m_NPCAnimator.Play(m_TransitionalAnimation);
			return;
		}
		Vector3 lhs2 = data.m_SourceLocation - base.transform.position;
		float sqrMagnitude = lhs2.sqrMagnitude;
		lhs2.y = 0f;
		bool flag3 = Vector3.Dot(lhs2, base.transform.forward) > 0f;
		if (data.m_Headshot)
		{
			if (UnityEngine.Random.value <= 0.5f)
			{
				m_TransitionalAnimation = "Death_Head_Back_Heavy";
			}
			else
			{
				m_TransitionalAnimation = "Death_Head_Back_Light";
			}
			m_NPCAnimator.CrossFade(m_TransitionalAnimation);
			return;
		}
		if (data.m_SourceEnemy != null && data.m_SourceEnemy.GetEquippedWeaponType() == WeaponType.Shotgun && sqrMagnitude <= 9f)
		{
			if (flag3)
			{
				base.transform.position = base.transform.position;
				base.transform.rotation = Quaternion.LookRotation(lhs2.normalized);
				m_TransitionalAnimation = "Death_Front_Chest_Heavy";
			}
			else
			{
				base.transform.position = base.transform.position;
				base.transform.rotation = Quaternion.LookRotation(-lhs2.normalized);
				m_TransitionalAnimation = "Death_Back_Heavy";
			}
			m_NPCAnimator.Stop();
			m_NPCAnimator.Play(m_TransitionalAnimation);
			return;
		}
		int num = ((!(UnityEngine.Random.value <= 0.5f)) ? 1 : 0);
		Ray ray3 = new Ray(base.transform.position + Vector3.up * 0.25f, (num != 0) ? (-base.transform.forward) : base.transform.forward);
		if (Physics.Raycast(ray3, 2f, 6374145))
		{
			num = (num + 1) % 2;
			ray3.direction = ((num != 0) ? (-base.transform.forward) : base.transform.forward);
			if (Physics.Raycast(ray3, 2f, 6374145))
			{
				num = -1;
				m_TransitionalAnimation = "Death_FallLeft";
			}
		}
		switch (num)
		{
		case 0:
			m_TransitionalAnimation = ((!(UnityEngine.Random.value <= 0.35f)) ? "Death_Back_Light" : "Death_Front_Head");
			break;
		case 1:
			m_TransitionalAnimation = ((!(UnityEngine.Random.value <= 0.35f)) ? "Death_Front_Stomach_Light" : "Death_Front_Chest");
			break;
		}
		m_NPCAnimator.CrossFade(m_TransitionalAnimation);
	}

	public virtual void Removed()
	{
		m_DestroyedNPCs.AddLast(m_SpawnPosition);
		UnityEngine.Object.Destroy(base.gameObject);
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

	public XmlElement SaveGame(XmlElement root, XmlDocument doc)
	{
		XmlElement xmlElement = null;
		XmlNodeList elementsByTagName = root.GetElementsByTagName("NPCs");
		foreach (XmlNode item in elementsByTagName)
		{
			if (item.Name == "NPCs")
			{
				xmlElement = (XmlElement)item;
				break;
			}
		}
		if (xmlElement == null)
		{
			return null;
		}
		XmlElement xmlElement2 = (XmlElement)xmlElement.AppendChild(doc.CreateElement("NPC"));
		xmlElement2.SetAttribute("Spawn_Position", m_SpawnPosition.x + "," + m_SpawnPosition.y + "," + m_SpawnPosition.z);
		xmlElement2.SetAttribute("Position", base.transform.position.x + "," + base.transform.position.y + "," + base.transform.position.z);
		xmlElement2.SetAttribute("Rotation", base.transform.rotation.x + "," + base.transform.rotation.y + "," + base.transform.rotation.z + "," + base.transform.rotation.w);
		xmlElement2.SetAttribute("NPCState", m_NPCState.ToString());
		xmlElement2.SetAttribute("PatrolState", m_PatrolState.ToString());
		xmlElement2.SetAttribute("StateTimer", m_StateTimer.ToString());
		switch (m_NPCState)
		{
		case NPCState.Patrolling:
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
			if (m_NPCState == NPCState.Patrolling)
			{
				switch (m_PatrolState)
				{
				case PatrolState.TurningToWaypoint:
					xmlElement2.SetAttribute("WaypointAnimTime", m_NPCAnimator[m_TransitionalAnimation].time.ToString());
					xmlElement2.SetAttribute("TurningWaypointAnim", m_TransitionalAnimation);
					xmlElement2.SetAttribute("TurningWaypointPosedAnim", m_PosedAnimation);
					break;
				case PatrolState.AtWaypoint:
					xmlElement2.SetAttribute("WaypointAnimTime", m_NPCAnimator[m_TransitionalAnimation].time.ToString());
					break;
				}
			}
			break;
		case NPCState.Stunned:
			xmlElement2.SetAttribute("StunnedAnimTime", m_NPCAnimator[m_TransitionalAnimation].time.ToString());
			break;
		}
		return xmlElement2;
	}

	public void LoadGame(XmlElement el)
	{
		NPCState nPCState = NPCState.None;
		float time = 0f;
		float stateTimer = 0f;
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
			case "NPCState":
				nPCState = (NPCState)(int)Enum.Parse(typeof(NPCState), attribute.InnerText);
				break;
			case "PatrolState":
				m_PatrolState = (PatrolState)(int)Enum.Parse(typeof(PatrolState), attribute.InnerText);
				break;
			case "StateTimer":
				m_StateTimer = float.Parse(attribute.InnerText);
				stateTimer = m_StateTimer;
				break;
			case "TargetPatrolNode":
			{
				string[] array = attribute.InnerText.Split(',');
				Vector3 vector = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				NPC_PatrolNode[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(NPC_PatrolNode)) as NPC_PatrolNode[];
				NPC_PatrolNode[] array4 = array2;
				foreach (NPC_PatrolNode nPC_PatrolNode2 in array4)
				{
					if (nPC_PatrolNode2.transform.position == vector)
					{
						m_TargetNode = nPC_PatrolNode2;
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
				NPC_PatrolNode[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(NPC_PatrolNode)) as NPC_PatrolNode[];
				NPC_PatrolNode[] array3 = array2;
				foreach (NPC_PatrolNode nPC_PatrolNode in array3)
				{
					if (nPC_PatrolNode.transform.position == vector)
					{
						m_EventNode = nPC_PatrolNode;
						break;
					}
				}
				break;
			}
			case "WPDir":
				m_WPDir = (NPC_PatrolNode.WaypointDirection)(int)Enum.Parse(typeof(NPC_PatrolNode.WaypointDirection), attribute.InnerText);
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
			case "StunnedAnimTime":
				time = float.Parse(attribute.InnerText);
				break;
			}
		}
		switch (nPCState)
		{
		case NPCState.Patrolling:
			m_NPCState = NPCState.Patrolling;
			switch (m_PatrolState)
			{
			case PatrolState.StandingIdle:
				m_NPCAnimator.CrossFade("Idle_Standing");
				break;
			case PatrolState.AtWaypoint:
				m_TransitionalAnimation = m_EventNode.m_WaypointAnimation;
				m_PosedAnimation = m_TransitionalAnimation + "_Pose";
				m_NPCAnimator.CrossFade(m_EventNode.m_WaypointAnimation);
				m_NPCAnimator[m_EventNode.m_WaypointAnimation].time = time;
				break;
			case PatrolState.TurningToWaypoint:
				m_TransitionalAnimation = transitionalAnimation;
				m_PosedAnimation = posedAnimation;
				m_NPCAnimator.CrossFade(transitionalAnimation);
				m_NPCAnimator[transitionalAnimation].time = time;
				break;
			case PatrolState.WalkingPatrol:
				break;
			}
			break;
		case NPCState.Cowering:
			Cower(false);
			m_StateTimer = stateTimer;
			break;
		case NPCState.Stunned:
			Stun(Vector3.zero, false);
			m_NPCAnimator[m_TransitionalAnimation].time = time;
			break;
		}
	}
}
