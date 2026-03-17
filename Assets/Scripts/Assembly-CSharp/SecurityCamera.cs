using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
	public enum CameraState
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

	public enum TargetType
	{
		None = -1,
		Player = 0,
		DeadBody = 1
	}

	private CameraState m_State = CameraState.None;

	public bool m_Neutral;

	public EnemySquad m_SquadToAlert = EnemySquad.None;

	public GameObject m_Explosion;

	public GameObject m_FunctioningRoot;

	public GameObject m_DestroyedRoot;

	public GameObject m_FunctioningYaw;

	public GameObject m_FunctioningPitch;

	public GameObject m_DestroyedYaw;

	public GameObject m_DestroyedPitch;

	public Renderer m_GlowRenderer1;

	public Renderer m_GlowRenderer2;

	public LineRenderer m_TopLine;

	public LineRenderer m_BottomLine;

	public GameObject m_CameraCone;

	public float m_PanRangeInDegrees = 90f;

	private float m_PanRangeExtent;

	private PanningState m_PanningState;

	private float m_CurrentYaw;

	public float m_DefaultPitch = 30f;

	public Camera m_Camera;

	public Renderer m_NormalBaseMeshRenderer;

	public Renderer m_NormalYawMeshRenderer;

	public Renderer m_NormalPitchMeshRenderer;

	public Renderer m_BrokenBaseMeshRenderer;

	public Renderer m_BrokenYawMeshRenderer;

	public Renderer m_BrokenPitchMeshRenderer;

	private float m_ViewingAngleDot;

	private float m_DistanceThresholdSqr;

	private float m_TurningSpeedInDegrees = 12.5f;

	private float m_HostileTurnSpeedInDegrees = 55f;

	private float m_PlayerLostResetDelay = 5f;

	private float m_PlayerRecognizedDelay = 2f;

	private float m_PanningIdleDelay = 3f;

	private float m_ResetTimer;

	private float m_HostileTimer;

	private TargetType m_TargetType = TargetType.None;

	private Vector3 m_TargetLocation = Vector3.zero;

	public CameraState GetCameraState()
	{
		return m_State;
	}

	public void SetCameraState(CameraState state)
	{
		m_State = state;
	}

	public bool IsAttacking()
	{
		return m_State == CameraState.LockedOn;
	}

	public bool IsAlarmed()
	{
		return m_State == CameraState.LockingOn;
	}

	public bool IsPassive()
	{
		return m_State == CameraState.Panning;
	}

	public bool IsDeactivated()
	{
		return m_State == CameraState.Deactivated;
	}

	private void Awake()
	{
		m_DistanceThresholdSqr = 225f;
		m_ViewingAngleDot = Mathf.Cos(0.65449846f);
		m_PanRangeExtent = Mathf.Abs(m_PanRangeInDegrees * 0.5f);
	}

	private void Start()
	{
		m_DestroyedRoot.SetActiveRecursively(false);
		m_FunctioningRoot.SetActiveRecursively(true);
		m_State = CameraState.Panning;
		m_PanningState = PanningState.PanningRight;
		m_CameraCone.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		m_GlowRenderer1.material.SetColor("_TintColor", Globals.m_This.m_PassiveGlow);
		m_GlowRenderer2.material.SetColor("_TintColor", Globals.m_This.m_PassiveGlow);
		m_TopLine.SetColors(Globals.m_This.m_PassiveGlow, Color.clear);
		m_BottomLine.SetColors(Globals.m_This.m_PassiveGlow, Color.clear);
		Globals.m_AIDirector.SecurityCameraSpawned(this);
		SoundManager.TriggerEvent("Play_Security_Camera_Movement", base.gameObject);
	}

	public void Dominate(MachineAwareness awareness)
	{
		if (m_State == CameraState.Destroyed)
		{
			return;
		}
		if (awareness != MachineAwareness.Hostile && m_State != CameraState.Deactivated)
		{
			m_State = CameraState.Deactivated;
			m_GlowRenderer1.material.SetColor("_TintColor", Color.gray);
			m_GlowRenderer2.material.SetColor("_TintColor", Color.gray);
			m_TopLine.gameObject.active = false;
			m_BottomLine.gameObject.active = false;
			SoundManager.TriggerEvent("Stop_Security_Camera_Movement", base.gameObject);
			SoundManager.TriggerEvent("Stop_Security_Camera_Alarmed", base.gameObject);
		}
		else if (awareness == MachineAwareness.Hostile && m_State == CameraState.Deactivated)
		{
			m_State = CameraState.Panning;
			m_GlowRenderer1.material.SetColor("_TintColor", Globals.m_This.m_PassiveGlow);
			m_GlowRenderer2.material.SetColor("_TintColor", Globals.m_This.m_PassiveGlow);
			m_TopLine.gameObject.active = true;
			m_BottomLine.gameObject.active = true;
			m_TopLine.SetColors(Globals.m_This.m_PassiveGlow, Color.clear);
			m_BottomLine.SetColors(Globals.m_This.m_PassiveGlow, Color.clear);
			m_CurrentYaw = m_FunctioningYaw.transform.localRotation.eulerAngles.y;
			if (m_CurrentYaw >= 180f)
			{
				m_CurrentYaw -= 360f;
			}
			SoundManager.TriggerEvent("Play_Security_Camera_Movement", base.gameObject);
			SoundManager.TriggerEvent("Stop_Security_Camera_Alarmed", base.gameObject);
		}
	}

	private void Update()
	{
		if (Time.timeScale != 0f && !(Time.deltaTime <= 0f))
		{
			switch (m_State)
			{
			case CameraState.Panning:
				UpdateCameraPanning();
				break;
			case CameraState.LockingOn:
				UpdateCameraLockingOn();
				break;
			case CameraState.LockedOn:
				UpdateCameraLockedOn();
				break;
			case CameraState.Destroyed:
				UpdateCameraDestroyed();
				break;
			case CameraState.Deactivated:
				UpdateCameraDeactivated();
				break;
			}
			if (m_TargetType == TargetType.DeadBody)
			{
				m_TargetType = TargetType.None;
			}
		}
	}

	private void UpdateCameraPanning()
	{
		if (m_PanningState == PanningState.PanningRight)
		{
			m_CurrentYaw += m_TurningSpeedInDegrees * Time.deltaTime;
			if (m_CurrentYaw >= m_PanRangeExtent)
			{
				m_CurrentYaw = m_PanRangeExtent;
				m_PanningState = PanningState.PanningRightIdle;
				m_ResetTimer = 0f;
				SoundManager.TriggerEvent("Stop_Security_Camera_Movement", base.gameObject);
			}
		}
		else if (m_PanningState == PanningState.PanningRightIdle)
		{
			m_ResetTimer += Time.deltaTime;
			if (m_ResetTimer >= m_PanningIdleDelay)
			{
				m_PanningState = PanningState.PanningLeft;
				SoundManager.TriggerEvent("Play_Security_Camera_Movement", base.gameObject);
			}
		}
		else if (m_PanningState == PanningState.PanningLeft)
		{
			m_CurrentYaw -= m_TurningSpeedInDegrees * Time.deltaTime;
			if (m_CurrentYaw <= 0f - m_PanRangeExtent)
			{
				m_CurrentYaw = 0f - m_PanRangeExtent;
				m_PanningState = PanningState.PanningLeftIdle;
				m_ResetTimer = 0f;
				SoundManager.TriggerEvent("Stop_Security_Camera_Movement", base.gameObject);
			}
		}
		else
		{
			m_ResetTimer += Time.deltaTime;
			if (m_ResetTimer >= m_PanningIdleDelay)
			{
				m_PanningState = PanningState.PanningRight;
				SoundManager.TriggerEvent("Play_Security_Camera_Movement", base.gameObject);
			}
		}
		m_FunctioningYaw.transform.localRotation = Quaternion.Euler(0f, m_CurrentYaw, 0f);
		Quaternion to = Quaternion.Euler(m_DefaultPitch, 0f, 0f);
		m_FunctioningPitch.transform.localRotation = Quaternion.RotateTowards(m_FunctioningPitch.transform.localRotation, to, 30f * Time.deltaTime);
		float num = m_CurrentYaw / m_PanRangeExtent;
		m_CameraCone.transform.localRotation = Quaternion.Euler(0f, 0f, num * 90f);
		if (CheckVisualSenses())
		{
			m_ResetTimer = 0f;
			m_HostileTimer = 0f;
			m_State = CameraState.LockingOn;
			m_GlowRenderer1.material.SetColor("_TintColor", Globals.m_This.m_AlarmedGlow);
			m_GlowRenderer2.material.SetColor("_TintColor", Globals.m_This.m_AlarmedGlow);
			m_TopLine.SetColors(Globals.m_This.m_AlarmedGlow, Color.clear);
			m_BottomLine.SetColors(Globals.m_This.m_AlarmedGlow, Color.clear);
			SoundManager.TriggerEvent("Stop_Security_Camera_Movement", base.gameObject);
			SoundManager.TriggerEvent("Play_Security_Camera_Alarmed", base.gameObject);
			SoundManager.TriggerEvent("Play_EnemyState_Suspicious", base.gameObject);
		}
	}

	private void UpdateCameraLockingOn()
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
		Vector3 forward = m_TargetLocation - m_CameraCone.transform.position;
		forward.y = 0f;
		Quaternion to = Quaternion.LookRotation(forward);
		m_FunctioningYaw.transform.rotation = Quaternion.RotateTowards(m_FunctioningYaw.transform.rotation, to, m_HostileTurnSpeedInDegrees * Time.deltaTime);
		Vector3 eulerAngles = m_FunctioningYaw.transform.localRotation.eulerAngles;
		if (eulerAngles.y >= 180f)
		{
			eulerAngles.y -= 360f;
		}
		eulerAngles.y = Mathf.Clamp(eulerAngles.y, 0f - m_PanRangeExtent, m_PanRangeExtent);
		m_FunctioningYaw.transform.localRotation = Quaternion.Euler(eulerAngles);
		forward = m_TargetLocation - m_CameraCone.transform.position;
		forward.y = 0f;
		forward.z = forward.magnitude;
		forward.x = 0f;
		forward.y = m_TargetLocation.y - m_CameraCone.transform.position.y;
		to = Quaternion.LookRotation(forward);
		m_FunctioningPitch.transform.localRotation = Quaternion.RotateTowards(m_FunctioningPitch.transform.localRotation, to, m_HostileTurnSpeedInDegrees * Time.deltaTime);
		float num = Mathf.Clamp(m_CurrentYaw / m_PanRangeExtent, -1f, 1f);
		m_CameraCone.transform.localRotation = Quaternion.Euler(0f, 0f, num * 90f);
		if (m_ResetTimer >= m_PlayerLostResetDelay)
		{
			m_State = CameraState.Panning;
			m_PanningState = PanningState.PanningRight;
			m_GlowRenderer1.material.SetColor("_TintColor", Globals.m_This.m_PassiveGlow);
			m_GlowRenderer2.material.SetColor("_TintColor", Globals.m_This.m_PassiveGlow);
			m_TopLine.SetColors(Globals.m_This.m_PassiveGlow, Color.clear);
			m_BottomLine.SetColors(Globals.m_This.m_PassiveGlow, Color.clear);
			m_CurrentYaw = m_FunctioningYaw.transform.localRotation.eulerAngles.y;
			if (m_CurrentYaw >= 180f)
			{
				m_CurrentYaw -= 360f;
			}
			SoundManager.TriggerEvent("Play_Security_Camera_Movement", base.gameObject);
			SoundManager.TriggerEvent("Stop_Security_Camera_Alarmed", base.gameObject);
		}
		else if (m_HostileTimer >= m_PlayerRecognizedDelay)
		{
			m_State = CameraState.LockedOn;
			m_GlowRenderer1.material.SetColor("_TintColor", Globals.m_This.m_HostileGlow);
			m_GlowRenderer2.material.SetColor("_TintColor", Globals.m_This.m_HostileGlow);
			m_TopLine.SetColors(Globals.m_This.m_HostileGlow, Color.clear);
			m_BottomLine.SetColors(Globals.m_This.m_HostileGlow, Color.clear);
			if (m_SquadToAlert != EnemySquad.None)
			{
				Globals.m_AIDirector.TriggerAlarm(m_SquadToAlert);
			}
			SoundManager.TriggerEvent("Stop_Security_Camera_Alarmed", base.gameObject);
		}
	}

	private void UpdateCameraLockedOn()
	{
		if (CheckVisualSenses())
		{
			m_ResetTimer = 0f;
		}
		else
		{
			m_ResetTimer += Time.deltaTime;
		}
		Vector3 forward = m_TargetLocation - m_CameraCone.transform.position;
		forward.y = 0f;
		Quaternion to = Quaternion.LookRotation(forward);
		m_FunctioningYaw.transform.rotation = Quaternion.RotateTowards(m_FunctioningYaw.transform.rotation, to, m_HostileTurnSpeedInDegrees * Time.deltaTime);
		Vector3 eulerAngles = m_FunctioningYaw.transform.localRotation.eulerAngles;
		if (eulerAngles.y >= 180f)
		{
			eulerAngles.y -= 360f;
		}
		eulerAngles.y = Mathf.Clamp(eulerAngles.y, 0f - m_PanRangeExtent, m_PanRangeExtent);
		m_FunctioningYaw.transform.localRotation = Quaternion.Euler(eulerAngles);
		forward = m_TargetLocation - m_CameraCone.transform.position;
		forward.y = 0f;
		forward.z = forward.magnitude;
		forward.x = 0f;
		forward.y = m_TargetLocation.y - m_CameraCone.transform.position.y;
		to = Quaternion.LookRotation(forward);
		m_FunctioningPitch.transform.localRotation = Quaternion.RotateTowards(m_FunctioningPitch.transform.localRotation, to, m_HostileTurnSpeedInDegrees * Time.deltaTime);
		float num = Mathf.Clamp(m_CurrentYaw / m_PanRangeExtent, -1f, 1f);
		m_CameraCone.transform.localRotation = Quaternion.Euler(0f, 0f, num * 90f);
		if (m_ResetTimer >= m_PlayerLostResetDelay)
		{
			m_State = CameraState.Panning;
			m_PanningState = PanningState.PanningRight;
			m_GlowRenderer1.sharedMaterial.SetColor("_TintColor", Globals.m_This.m_PassiveGlow);
			m_GlowRenderer2.sharedMaterial.SetColor("_TintColor", Globals.m_This.m_PassiveGlow);
			m_TopLine.SetColors(Globals.m_This.m_PassiveGlow, Color.clear);
			m_BottomLine.SetColors(Globals.m_This.m_PassiveGlow, Color.clear);
			m_CurrentYaw = m_FunctioningYaw.transform.localRotation.eulerAngles.y;
			if (m_CurrentYaw >= 180f)
			{
				m_CurrentYaw -= 360f;
			}
			SoundManager.TriggerEvent("Play_Security_Camera_Movement", base.gameObject);
		}
	}

	private void UpdateCameraDestroyed()
	{
		Quaternion to = Quaternion.Euler(60f, 0f, 0f);
		m_DestroyedPitch.transform.localRotation = Quaternion.RotateTowards(m_DestroyedPitch.transform.localRotation, to, 30f * Time.deltaTime);
	}

	private void UpdateCameraDeactivated()
	{
		Quaternion to = Quaternion.Euler(60f, 0f, 0f);
		m_FunctioningPitch.transform.localRotation = Quaternion.RotateTowards(m_FunctioningPitch.transform.localRotation, to, 30f * Time.deltaTime);
	}

	public void Destroy()
	{
		m_DestroyedRoot.SetActiveRecursively(true);
		m_FunctioningRoot.SetActiveRecursively(false);
		m_DestroyedYaw.transform.localRotation = m_FunctioningYaw.transform.localRotation;
		m_DestroyedPitch.transform.localRotation = m_FunctioningPitch.transform.localRotation;
		Globals.m_AIDirector.SecurityCameratDestroyed(this);
		if (m_SquadToAlert != EnemySquad.None && m_State != CameraState.Deactivated)
		{
			Globals.m_AIDirector.TriggerAlarm(m_SquadToAlert);
		}
		if ((bool)m_Explosion)
		{
			Object.Instantiate(m_Explosion, base.transform.position, Quaternion.identity);
		}
		m_State = CameraState.Destroyed;
		SoundManager.TriggerEvent("Play_Security_Camera_Death", base.gameObject);
		SoundManager.TriggerEvent("Stop_Security_Camera_Movement", base.gameObject);
		SoundManager.TriggerEvent("Stop_Security_Camera_Alarmed", base.gameObject);
	}

	private bool CheckVisualSenses()
	{
		if (m_TargetType == TargetType.Player)
		{
			m_TargetType = TargetType.None;
		}
		if (m_State == CameraState.Destroyed || m_State == CameraState.Deactivated)
		{
			return false;
		}
		if (Globals.m_PlayerController == null || !Globals.m_PlayerController.gameObject.active)
		{
			return m_TargetType != TargetType.None;
		}
		if (Globals.m_AugmentCloaking.enabled)
		{
			return m_TargetType != TargetType.None;
		}
		Vector3 vector = Globals.m_PlayerController.GetChestLocation() - m_CameraCone.transform.position;
		Vector3 normalized = vector.normalized;
		float num = Vector3.Dot(normalized, m_CameraCone.transform.forward);
		if (Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.Firing && Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.Outside && Globals.m_PlayerController.m_CoverState != PlayerController.CoverState.CoverDive && Vector3.Dot(normalized, Globals.m_PlayerController.m_CoverNormal) >= 0.3f)
		{
			return m_TargetType != TargetType.None;
		}
		if ((!m_Neutral || !Globals.m_PlayerController.IsWeaponHolstered() || HackingSystem.m_this != null || Globals.m_PlayerController.m_CameraMode == PlayerController.CameraMode.Takedown) && num >= m_ViewingAngleDot && vector.sqrMagnitude <= m_DistanceThresholdSqr)
		{
			Ray ray = new Ray(m_CameraCone.transform.position, Globals.m_PlayerController.GetChestLocation() - m_CameraCone.transform.position);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 100f, 82177) && (hitInfo.collider.gameObject.layer == 14 || (Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Firing && hitInfo.collider == Globals.m_PlayerController.m_CoverCollider)))
			{
				if (m_TargetType != TargetType.None && m_TargetType != TargetType.Player)
				{
					m_HostileTimer = 0f;
				}
				m_TargetType = TargetType.Player;
				m_TargetLocation = Globals.m_PlayerController.GetChestLocation();
				return true;
			}
		}
		return m_TargetType != TargetType.None;
	}

	public void CheckBody(Collider collider)
	{
		if (m_TargetType == TargetType.Player || m_State == CameraState.Destroyed || m_State == CameraState.Deactivated)
		{
			return;
		}
		Vector3 vector = collider.transform.position - m_CameraCone.transform.position;
		Vector3 normalized = vector.normalized;
		float num = Vector3.Dot(normalized, m_CameraCone.transform.forward);
		if (num >= m_ViewingAngleDot && vector.sqrMagnitude <= m_DistanceThresholdSqr)
		{
			Ray ray = new Ray(m_CameraCone.transform.position, collider.transform.position - m_CameraCone.transform.position);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 20f, 66305) && hitInfo.collider == collider)
			{
				m_TargetType = TargetType.DeadBody;
				m_TargetLocation = collider.transform.position;
			}
		}
	}
}
