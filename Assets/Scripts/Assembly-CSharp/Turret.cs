using System.Collections.Generic;
using UnityEngine;

public class Turret : CharacterBase
{
	public enum TurretState
	{
		None = -1,
		Panning = 0,
		LockingOn = 1,
		LockedOn = 2,
		Destroyed = 3,
		Deactivated = 4,
		Total = 5
	}

	public enum PanningState
	{
		PanningRight = 0,
		PanningRightIdle = 1,
		PanningLeft = 2,
		PanningLeftIdle = 3
	}

	private TurretState m_State = TurretState.None;

	private MachineAwareness m_Awareness;

	public bool m_Neutral;

	public GameObject m_Explosion;

	public GameObject m_FunctioningYaw;

	public GameObject m_FunctioningPitch;

	public Renderer m_GlowRenderer;

	public Renderer m_BaseRenderer;

	public Renderer m_ArmsRenderer;

	public Renderer m_AmmoRenderer;

	public Renderer m_LeftCannonRenderer;

	public Renderer m_RightCannonRenderer;

	public GameObject m_RaycastObject;

	public ParticleSystem m_LeftMuzzleFlash;

	public ParticleSystem m_RightMuzzleFlash;

	private bool m_Firing;

	public float m_FireRate = 0.2f;

	private float m_FiringTimer;

	public int m_BulletDamage = 10;

	public float m_PanRangeInDegrees = 90f;

	private float m_PanRangeExtent;

	private PanningState m_PanningState;

	private float m_CurrentYaw;

	private float m_ViewingAngleDot;

	private float m_DistanceThresholdSqr;

	private float m_TurningSpeedInDegrees = 12.5f;

	private float m_HostileTurnSpeedInDegrees = 55f;

	private float m_PlayerLostResetDelay = 5f;

	private float m_PlayerRecognizedDelay = 2f;

	private float m_PanningIdleDelay = 3f;

	private float m_ResetTimer;

	private float m_HostileTimer;

	private CharacterBase m_Target;

	private Vector3 m_TargetLocation = Vector3.zero;

	public InteractiveObject_Domination m_ConnectedDominationPanel;

	public bool IsAttacking()
	{
		return m_State == TurretState.LockedOn;
	}

	public bool IsAlarmed()
	{
		return m_State == TurretState.LockingOn;
	}

	public bool IsPassive()
	{
		return m_State == TurretState.Panning;
	}

	public bool IsDeactivated()
	{
		return m_State == TurretState.Deactivated;
	}

	public MachineAwareness GetMachineAwareness()
	{
		return m_Awareness;
	}

	private void Awake()
	{
		m_DistanceThresholdSqr = 625f;
		m_ViewingAngleDot = Mathf.Cos(0.65449846f);
		m_PanRangeExtent = Mathf.Abs(m_PanRangeInDegrees * 0.5f);
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
		m_State = TurretState.Panning;
		m_PanningState = PanningState.PanningRight;
		m_GlowRenderer.material.SetColor("_TintColor", Globals.m_This.m_PassiveGlow);
		Globals.m_AIDirector.TurretSpawned(this);
		if ((bool)m_ConnectedDominationPanel)
		{
			m_ConnectedDominationPanel.RegisterTurret(this);
		}
		SoundManager.TriggerEvent("Play_Turret_Movement", base.gameObject);
		m_CharacterType = CharacterType.Machine;
	}

	public void Dominate(MachineAwareness awareness)
	{
		if (m_State == TurretState.Destroyed)
		{
			return;
		}
		m_Awareness = awareness;
		if (awareness == MachineAwareness.Deactivated && m_State != TurretState.Deactivated)
		{
			m_State = TurretState.Deactivated;
			EndFiring();
			m_Target = null;
			m_GlowRenderer.material.SetColor("_TintColor", Color.gray);
			SoundManager.TriggerEvent("Stop_Turret_Movement", base.gameObject);
			SoundManager.TriggerEvent("Stop_Turret_Warning", base.gameObject);
		}
		else if (awareness != MachineAwareness.Deactivated && m_State == TurretState.Deactivated)
		{
			m_State = TurretState.Panning;
			EndFiring();
			m_Target = null;
			m_GlowRenderer.material.SetColor("_TintColor", Globals.m_This.m_PassiveGlow);
			m_CurrentYaw = m_FunctioningYaw.transform.localRotation.eulerAngles.y;
			if (m_CurrentYaw >= 180f)
			{
				m_CurrentYaw -= 360f;
			}
			SoundManager.TriggerEvent("Play_Turret_Movement", base.gameObject);
			SoundManager.TriggerEvent("Stop_Turret_Warning", base.gameObject);
		}
	}

	protected override void Update()
	{
		if (Time.timeScale != 0f && !(Time.deltaTime <= 0f))
		{
			base.Update();
			switch (m_State)
			{
			case TurretState.Panning:
				UpdateTurretPanning();
				break;
			case TurretState.LockingOn:
				UpdateTurretLockingOn();
				break;
			case TurretState.LockedOn:
				UpdateTurretLockedOn();
				break;
			case TurretState.Destroyed:
				UpdateTurretDestroyed();
				break;
			case TurretState.Deactivated:
				UpdateTurretDeactivated();
				break;
			}
		}
	}

	private void UpdateTurretPanning()
	{
		if (m_PanningState == PanningState.PanningRight)
		{
			m_CurrentYaw += m_TurningSpeedInDegrees * Time.deltaTime;
			if (m_CurrentYaw >= m_PanRangeExtent)
			{
				m_PanningState = PanningState.PanningRightIdle;
				m_ResetTimer = 0f;
				SoundManager.TriggerEvent("Stop_Turret_Movement", base.gameObject);
			}
		}
		else if (m_PanningState == PanningState.PanningRightIdle)
		{
			m_ResetTimer += Time.deltaTime;
			if (m_ResetTimer >= m_PanningIdleDelay)
			{
				m_PanningState = PanningState.PanningLeft;
				SoundManager.TriggerEvent("Play_Turret_Movement", base.gameObject);
			}
		}
		else if (m_PanningState == PanningState.PanningLeft)
		{
			m_CurrentYaw -= m_TurningSpeedInDegrees * Time.deltaTime;
			if (m_CurrentYaw <= 0f - m_PanRangeExtent)
			{
				m_PanningState = PanningState.PanningLeftIdle;
				m_ResetTimer = 0f;
				SoundManager.TriggerEvent("Stop_Turret_Movement", base.gameObject);
			}
		}
		else
		{
			m_ResetTimer += Time.deltaTime;
			if (m_ResetTimer >= m_PanningIdleDelay)
			{
				m_PanningState = PanningState.PanningRight;
				SoundManager.TriggerEvent("Play_Turret_Movement", base.gameObject);
			}
		}
		m_FunctioningYaw.transform.localRotation = Quaternion.Euler(0f, m_CurrentYaw, 0f);
		m_FunctioningPitch.transform.localRotation = Quaternion.RotateTowards(m_FunctioningPitch.transform.localRotation, Quaternion.identity, 50f * Time.deltaTime);
		if (CheckVisualSenses())
		{
			m_ResetTimer = 0f;
			m_HostileTimer = 0f;
			m_State = TurretState.LockingOn;
			m_GlowRenderer.material.SetColor("_TintColor", Globals.m_This.m_AlarmedGlow);
			SoundManager.TriggerEvent("Stop_Turret_Movement", base.gameObject);
			SoundManager.TriggerEvent("Play_Turret_Warning", base.gameObject);
			SoundManager.TriggerEvent("Play_EnemyState_Suspicious", base.gameObject);
		}
	}

	private void UpdateTurretLockingOn()
	{
		if (CheckVisualSenses())
		{
			m_ResetTimer = 0f;
			m_HostileTimer += Time.deltaTime;
		}
		else
		{
			m_ResetTimer += Time.deltaTime;
			m_HostileTimer = 0f;
		}
		Vector3 forward = m_TargetLocation - m_RaycastObject.transform.position;
		forward.y = 0f;
		Quaternion to = Quaternion.LookRotation(forward);
		m_FunctioningYaw.transform.rotation = Quaternion.RotateTowards(m_FunctioningYaw.transform.rotation, to, m_HostileTurnSpeedInDegrees * Time.deltaTime);
		forward = m_TargetLocation - m_RaycastObject.transform.position;
		forward.y = 0f;
		forward.z = forward.magnitude;
		forward.x = 0f;
		forward.y = m_TargetLocation.y - m_RaycastObject.transform.position.y;
		to = Quaternion.LookRotation(forward);
		m_FunctioningPitch.transform.localRotation = Quaternion.RotateTowards(m_FunctioningPitch.transform.localRotation, to, m_HostileTurnSpeedInDegrees * Time.deltaTime);
		if (m_ResetTimer >= m_PlayerLostResetDelay)
		{
			m_State = TurretState.Panning;
			m_PanningState = PanningState.PanningRight;
			m_GlowRenderer.material.SetColor("_TintColor", Globals.m_This.m_PassiveGlow);
			m_CurrentYaw = m_FunctioningYaw.transform.localRotation.eulerAngles.y;
			if (m_CurrentYaw >= 180f)
			{
				m_CurrentYaw -= 360f;
			}
			m_Target = null;
			SoundManager.TriggerEvent("Play_Turret_Movement", base.gameObject);
			SoundManager.TriggerEvent("Stop_Turret_Warning", base.gameObject);
		}
		else if (m_HostileTimer >= m_PlayerRecognizedDelay)
		{
			m_State = TurretState.LockedOn;
			m_GlowRenderer.material.SetColor("_TintColor", Globals.m_This.m_HostileGlow);
			SoundManager.TriggerEvent("Stop_Turret_Warning", base.gameObject);
		}
	}

	private void UpdateTurretLockedOn()
	{
		if (CheckVisualSenses())
		{
			m_ResetTimer = 0f;
		}
		else
		{
			m_ResetTimer += Time.deltaTime;
		}
		Vector3 forward = m_TargetLocation - m_RaycastObject.transform.position;
		forward.y = 0f;
		Quaternion to = Quaternion.LookRotation(forward);
		m_FunctioningYaw.transform.rotation = Quaternion.RotateTowards(m_FunctioningYaw.transform.rotation, to, m_HostileTurnSpeedInDegrees * Time.deltaTime);
		forward = m_TargetLocation - m_RaycastObject.transform.position;
		forward.y = 0f;
		forward.z = forward.magnitude;
		forward.x = 0f;
		forward.y = m_TargetLocation.y - m_RaycastObject.transform.position.y;
		to = Quaternion.LookRotation(forward);
		m_FunctioningPitch.transform.localRotation = Quaternion.RotateTowards(m_FunctioningPitch.transform.localRotation, to, m_HostileTurnSpeedInDegrees * Time.deltaTime);
		if (m_ResetTimer >= m_PlayerLostResetDelay)
		{
			m_State = TurretState.Panning;
			m_PanningState = PanningState.PanningRight;
			m_GlowRenderer.sharedMaterial.SetColor("_TintColor", Globals.m_This.m_PassiveGlow);
			EndFiring();
			m_Target = null;
			m_CurrentYaw = m_FunctioningYaw.transform.localRotation.eulerAngles.y;
			if (m_CurrentYaw >= 180f)
			{
				m_CurrentYaw -= 360f;
			}
			SoundManager.TriggerEvent("Play_Turret_Movement", base.gameObject);
		}
		else
		{
			UpdateFiring();
		}
	}

	private void UpdateTurretDestroyed()
	{
		Quaternion to = Quaternion.Euler(60f, 0f, 0f);
		m_FunctioningPitch.transform.localRotation = Quaternion.RotateTowards(m_FunctioningPitch.transform.localRotation, to, 30f * Time.deltaTime);
	}

	private void UpdateTurretDeactivated()
	{
		Quaternion to = Quaternion.Euler(60f, 0f, 0f);
		m_FunctioningPitch.transform.localRotation = Quaternion.RotateTowards(m_FunctioningPitch.transform.localRotation, to, 30f * Time.deltaTime);
	}

	private void StartFiring()
	{
		if (!m_Firing)
		{
			m_Firing = true;
			SoundManager.TriggerEvent("Play_Turret_Fire", base.gameObject);
			m_LeftMuzzleFlash.Play();
			m_RightMuzzleFlash.Play();
		}
	}

	private void EndFiring()
	{
		if (m_Firing)
		{
			m_Firing = false;
			SoundManager.TriggerEvent("Stop_Turret_Fire", base.gameObject);
			m_LeftMuzzleFlash.Stop();
			m_RightMuzzleFlash.Stop();
			m_LeftMuzzleFlash.Clear();
			m_RightMuzzleFlash.Clear();
		}
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
			if (num < 0.96f || m_ResetTimer >= 1.5f)
			{
				EndFiring();
			}
		}
		else if (num >= 0.96f && m_ResetTimer <= 0.5f)
		{
			StartFiring();
		}
	}

	private void FireBullet()
	{
		Ray ray = new Ray(m_RaycastObject.transform.position, m_TargetLocation - m_RaycastObject.transform.position);
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
		if (m_State == TurretState.Destroyed || m_State == TurretState.Deactivated)
		{
			return false;
		}
		if (m_Awareness == MachineAwareness.Hostile)
		{
			if (Globals.m_PlayerController == null || !Globals.m_PlayerController.gameObject.active)
			{
				return false;
			}
			if (Globals.m_AugmentCloaking.enabled)
			{
				return false;
			}
			if ((m_State == TurretState.LockedOn || m_State == TurretState.LockingOn || !m_Neutral || HostilityZone.IsPlayerInHostileTerritory()) && CanSeeEnemy(Globals.m_PlayerController) > 0f)
			{
				m_Target = Globals.m_PlayerController;
				m_TargetLocation = m_Target.GetChestLocation();
				return true;
			}
			return false;
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
				return true;
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
			if (m_Target == null && flag)
			{
				m_ResetTimer = 10000f;
			}
			return m_Target != null;
		}
		return false;
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

	public void Destroy()
	{
		Globals.m_AIDirector.TurretDestroyed(this);
		Object.Destroy(base.gameObject);
	}

	public override bool TakeDamage(DamageData data)
	{
		bool flag = base.TakeDamage(data);
		if (!flag && m_State != TurretState.Deactivated)
		{
			m_State = TurretState.LockedOn;
			m_ResetTimer = 0f;
			m_HostileTimer = 0f;
			m_Target = data.m_SourceEnemy;
			m_TargetLocation = m_Target.GetChestLocation();
			m_GlowRenderer.material.SetColor("_TintColor", Globals.m_This.m_HostileGlow);
		}
		return flag;
	}

	public override void Die(DamageData data)
	{
		base.Die(data);
		Globals.m_AIDirector.TurretDestroyed(this);
		m_State = TurretState.Destroyed;
		EndFiring();
		m_Target = null;
		if (Globals.m_PlayerController.m_TargetedEnemy == base.gameObject)
		{
			Globals.m_PlayerController.m_TargetedEnemy = null;
		}
		SoundManager.TriggerEvent("Play_Security_Robot_Death", base.gameObject);
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

	protected override void AttachShadowObject()
	{
		Object.Destroy(m_ShadowObject);
	}
}
