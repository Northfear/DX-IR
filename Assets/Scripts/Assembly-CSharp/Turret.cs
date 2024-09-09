using System;
using Fabric;
using UnityEngine;

public class Turret : CharacterBase
{
	public enum TurretState
	{
		None = -1,
		Idle = 0,
		Alarmed = 1,
		Attacking = 2,
		Dying = 3,
		Total = 4
	}

	private TurretState m_State;

	public GameObject m_HorizontalRotator;

	public GameObject m_VerticalRotator;

	public float m_ViewingAngleInDegrees = 90f;

	private float m_ViewingAngleDot;

	public float m_DistanceThreshold = 15f;

	private float m_DistanceThresholdSqr;

	private Vector3 m_TargetLocation = Vector3.zero;

	public float m_DegreesPerSecond = 90f;

	private bool m_Firing;

	public float m_FireRate = 0.2f;

	private float m_FiringTimer;

	public int m_BulletDamage = 10;

	public float m_PlayerLostResetDelay = 5f;

	public float m_AlarmedResetDelay = 5f;

	private float m_ResetTimer = -1f;

	public GameObject m_RaycastObject;

	public ParticleSystem m_LeftMuzzleFlash;

	public ParticleSystem m_RightMuzzleFlash;

	public Renderer m_GlowRenderer;

	public Color m_IdleColor = Color.green;

	public Color m_AlarmedColor = Color.yellow;

	public Color m_AttackingColor = Color.red;

	public bool IsAttacking()
	{
		return m_State == TurretState.Attacking;
	}

	public bool IsAlarmed()
	{
		return m_State == TurretState.Alarmed;
	}

	public bool IsPassive()
	{
		return m_State == TurretState.Idle;
	}

	private void Awake()
	{
		m_DistanceThresholdSqr = m_DistanceThreshold * m_DistanceThreshold;
		m_ViewingAngleDot = Mathf.Cos((float)Math.PI / 180f * (m_ViewingAngleInDegrees * 0.5f));
		m_LeftMuzzleFlash.Stop();
		m_RightMuzzleFlash.Stop();
		if ((bool)m_GlowRenderer)
		{
			m_GlowRenderer.material.SetColor("_TintColor", m_IdleColor);
		}
	}

	protected override void Start()
	{
		Globals.m_AIDirector.TurretSpawned(this);
		base.Start();
		m_CharacterType = CharacterType.Machine;
		m_MaxHealth = 10000;
		m_CurrentHealth = 10000;
	}

	protected override void Update()
	{
		if (Time.timeScale != 0f)
		{
			base.Update();
			switch (m_State)
			{
			case TurretState.Idle:
				UpdateIdleState();
				break;
			case TurretState.Alarmed:
				UpdateAlarmedState();
				break;
			case TurretState.Attacking:
				UpdateAttackingState();
				break;
			case TurretState.Dying:
				UpdateDyingState();
				break;
			}
		}
	}

	private void UpdateIdleState()
	{
		if (Globals.m_PlayerController == null)
		{
			return;
		}
		m_HorizontalRotator.transform.localRotation = Quaternion.RotateTowards(m_HorizontalRotator.transform.localRotation, Quaternion.identity, m_DegreesPerSecond * Time.deltaTime);
		m_VerticalRotator.transform.localRotation = Quaternion.RotateTowards(m_VerticalRotator.transform.localRotation, Quaternion.identity, m_DegreesPerSecond * Time.deltaTime);
		Vector3 vector = Globals.m_PlayerController.transform.position - m_RaycastObject.transform.position;
		vector.y = 0f;
		float num = Vector3.Dot(vector.normalized, m_HorizontalRotator.transform.forward);
		if (vector.sqrMagnitude <= m_DistanceThresholdSqr && num >= m_ViewingAngleDot && CanSeePlayer())
		{
			m_State = TurretState.Alarmed;
			m_ResetTimer = 0f;
			m_TargetLocation = Globals.m_PlayerController.GetChestLocation();
			EventManager.Instance.PostEvent("Turret_Warning", EventAction.PlaySound, null, base.gameObject);
			if ((bool)m_GlowRenderer)
			{
				m_GlowRenderer.material.SetColor("_TintColor", m_AlarmedColor);
			}
		}
	}

	private void UpdateAlarmedState()
	{
		bool flag = false;
		Vector3 vector = Globals.m_PlayerController.transform.position - m_RaycastObject.transform.position;
		vector.y = 0f;
		float num = Vector3.Dot(vector.normalized, m_HorizontalRotator.transform.forward);
		if (vector.sqrMagnitude <= m_DistanceThresholdSqr && num >= m_ViewingAngleDot)
		{
			m_TargetLocation = Globals.m_PlayerController.GetChestLocation();
			flag = true;
		}
		Vector3 forward = m_TargetLocation - m_RaycastObject.transform.position;
		forward.y = 0f;
		Quaternion to = Quaternion.LookRotation(forward);
		m_HorizontalRotator.transform.rotation = Quaternion.RotateTowards(m_HorizontalRotator.transform.rotation, to, m_DegreesPerSecond * Time.deltaTime);
		forward = m_TargetLocation - m_RaycastObject.transform.position;
		forward.y = 0f;
		forward.z = forward.magnitude;
		forward.x = 0f;
		forward.y = m_TargetLocation.y - m_RaycastObject.transform.position.y;
		to = Quaternion.LookRotation(forward);
		m_VerticalRotator.transform.localRotation = Quaternion.RotateTowards(m_VerticalRotator.transform.localRotation, to, m_DegreesPerSecond * Time.deltaTime);
		m_ResetTimer += Time.deltaTime;
		if (!(m_ResetTimer >= m_AlarmedResetDelay))
		{
			return;
		}
		if (flag && CanSeePlayer())
		{
			m_State = TurretState.Attacking;
			if ((bool)m_GlowRenderer)
			{
				m_GlowRenderer.material.SetColor("_TintColor", m_AttackingColor);
			}
			return;
		}
		m_State = TurretState.Idle;
		m_Firing = false;
		m_FiringTimer = 0f;
		m_ResetTimer = 0f;
		EventManager.Instance.PostEvent("Turret_Warning", EventAction.StopSound, null, base.gameObject);
		if ((bool)m_GlowRenderer)
		{
			m_GlowRenderer.material.SetColor("_TintColor", m_IdleColor);
		}
	}

	private void UpdateAttackingState()
	{
		bool flag = false;
		Vector3 vector = Globals.m_PlayerController.transform.position - m_RaycastObject.transform.position;
		vector.y = 0f;
		float num = Vector3.Dot(vector.normalized, m_HorizontalRotator.transform.forward);
		if (vector.sqrMagnitude <= m_DistanceThresholdSqr && num >= m_ViewingAngleDot && CanSeePlayer())
		{
			m_TargetLocation = Globals.m_PlayerController.GetChestLocation();
			flag = true;
		}
		Vector3 forward = m_TargetLocation - m_RaycastObject.transform.position;
		forward.y = 0f;
		Quaternion to = Quaternion.LookRotation(forward);
		m_HorizontalRotator.transform.rotation = Quaternion.RotateTowards(m_HorizontalRotator.transform.rotation, to, m_DegreesPerSecond * Time.deltaTime);
		forward = m_TargetLocation - m_RaycastObject.transform.position;
		forward.y = 0f;
		forward.z = forward.magnitude;
		forward.x = 0f;
		forward.y = m_TargetLocation.y - m_RaycastObject.transform.position.y;
		to = Quaternion.LookRotation(forward);
		m_VerticalRotator.transform.localRotation = Quaternion.RotateTowards(m_VerticalRotator.transform.localRotation, to, m_DegreesPerSecond * Time.deltaTime);
		m_ResetTimer += Time.deltaTime;
		if (flag)
		{
			m_ResetTimer = 0f;
		}
		if (m_ResetTimer >= m_PlayerLostResetDelay)
		{
			m_State = TurretState.Idle;
			EventManager.Instance.PostEvent("Turret_Warning", EventAction.StopSound, null, base.gameObject);
			if (m_Firing)
			{
				EndFiring();
			}
			if ((bool)m_GlowRenderer)
			{
				m_GlowRenderer.material.SetColor("_TintColor", m_IdleColor);
			}
			return;
		}
		float num2 = Vector3.Dot(m_RaycastObject.transform.forward, (m_TargetLocation - m_RaycastObject.transform.position).normalized);
		m_FiringTimer -= Time.deltaTime;
		if (m_Firing)
		{
			if (m_FiringTimer <= 0f)
			{
				FireBullet();
				m_FiringTimer = m_FireRate;
			}
			if (num2 < 0.96f)
			{
				EndFiring();
			}
		}
		else if (num2 >= 0.96f)
		{
			StartFiring();
		}
	}

	private void UpdateDyingState()
	{
	}

	private void LateUpdate()
	{
		if (Time.timeScale != 0f && m_State == TurretState.Dying)
		{
			Vector3 forward = m_VerticalRotator.transform.localRotation * Vector3.forward;
			forward.y = 0f;
			forward.Normalize();
			forward.y = -1f;
			Quaternion to = Quaternion.LookRotation(forward);
			m_VerticalRotator.transform.localRotation = Quaternion.RotateTowards(m_VerticalRotator.transform.localRotation, to, 40f * Time.deltaTime);
		}
	}

	private void StartFiring()
	{
		m_Firing = true;
		EventManager.Instance.PostEvent("Turret_Fire_Loop", EventAction.PlaySound, null, base.gameObject);
		m_LeftMuzzleFlash.Play();
		m_RightMuzzleFlash.Play();
	}

	private void EndFiring()
	{
		m_Firing = false;
		EventManager.Instance.PostEvent("Turret_Fire_Loop", EventAction.StopSound, null, base.gameObject);
		EventManager.Instance.PostEvent("Turret_Fire_Tail", EventAction.PlaySound, null, base.gameObject);
		m_LeftMuzzleFlash.Stop();
		m_RightMuzzleFlash.Stop();
		m_LeftMuzzleFlash.Clear();
		m_RightMuzzleFlash.Clear();
	}

	private void FireBullet()
	{
		Ray ray = new Ray(m_RaycastObject.transform.position, m_RaycastObject.transform.forward);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, 17153) && (hitInfo.collider.gameObject.layer == 14 || hitInfo.collider.gameObject.layer == 9))
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
		Globals.m_AIDirector.TurretDestroyed(this);
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
		if (Type == DamageType.EMP)
		{
			m_State = TurretState.Dying;
			EndFiring();
			EventManager.Instance.PostEvent("Turret_Warning", EventAction.StopSound, null, base.gameObject);
		}
		return false;
	}

	protected override void AttachShadowObject()
	{
		UnityEngine.Object.Destroy(m_ShadowObject);
	}

	private bool CanSeePlayer()
	{
		if (!Globals.m_AugmentCloaking.enabled)
		{
			Ray ray = new Ray(m_RaycastObject.transform.position, Globals.m_PlayerController.GetChestLocation() - m_RaycastObject.transform.position);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 100f, 16641) && hitInfo.collider.gameObject.layer == 14)
			{
				float num = -1f;
				Vector3 vector = Globals.m_PlayerController.transform.position - m_RaycastObject.transform.position;
				Vector3 lhs = Vector3.Normalize(new Vector3(vector.x, 0f, vector.z));
				if (Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.Firing && Globals.m_PlayerController.m_CoverState != 0 && Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.CoverDive)
				{
					num = Vector3.Dot(lhs, Globals.m_PlayerController.m_CoverNormal);
				}
				if (num < 0.3f)
				{
					return true;
				}
			}
		}
		return false;
	}
}
