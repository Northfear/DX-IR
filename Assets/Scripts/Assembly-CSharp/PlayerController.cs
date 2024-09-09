using System;
using System.Collections;
using System.Collections.Generic;
using Fabric;
using UnityEngine;

public class PlayerController : CharacterBase
{
	public enum InputDevice
	{
		Mouse = 0,
		Touch = 1
	}

	public enum Stance
	{
		Stand = 0,
		Crouch = 1,
		Total = 2
	}

	private enum CoverAnimation
	{
		TransitionLeftToRight = 0,
		TransitionRightToLeft = 1,
		Enter = 2,
		Idle = 3,
		Reload = 4,
		Stand = 5,
		Crouch = 6,
		TransitionToLeanFire = 7,
		LeanFiring = 8,
		TransitionFromLeanFire = 9,
		TransitionToUpOverFire = 10,
		UpOverFiring = 11,
		TransitionFromUpOverFire = 12,
		CoverAiming = 13,
		CoverAimFiring = 14,
		CoverAimReloading = 15,
		CoverAimThrowingGrenade = 16,
		CoverAimingUpOver = 17,
		CoverAimFiringUpOver = 18,
		CoverAimReloadingUpOver = 19,
		Move = 20,
		MoveBackwards = 21,
		Holster = 22,
		Draw = 23,
		CoverFlip = 24,
		CoverDive = 25,
		CoverSlideInner = 26,
		CoverSlideOuter = 27,
		ThrowGrenade = 28,
		ThrowGrenadeUpOver = 29,
		Total = 30
	}

	private class CoverAnimationSoundEvent
	{
		public string name;

		public float delay;

		public float delayTimer;
	}

	private struct CoverAnimationData
	{
		public string name;

		public string weaponAnimName;

		public bool loop;

		public float blendTime;

		public List<CoverAnimationSoundEvent> soundEvents;
	}

	public enum CoverState
	{
		Outside = 0,
		TransitioningIn = 1,
		Inside = 2,
		SwitchingSides = 3,
		TransitioningToFire = 4,
		TransitioningFromFire = 5,
		Firing = 6,
		CoverAiming = 7,
		Reloading = 8,
		Stand = 9,
		Holstering = 10,
		Drawing = 11,
		Crouch = 12,
		CoverFlip = 13,
		CoverDive = 14,
		CoverSlideInner = 15,
		CoverSlideOuter = 16,
		ThrowGrenade = 17,
		Total = 18
	}

	public enum CoverSide
	{
		Left = 0,
		Right = 1
	}

	public enum CoverFireSide
	{
		None = 0,
		UpOver = 1,
		Left = 2,
		Right = 3
	}

	public enum CoverEdge
	{
		None = 0,
		Left = 1,
		Right = 2,
		Both = 3
	}

	public enum CameraMode
	{
		First = 0,
		Third = 1,
		Takedown = 2
	}

	private enum GrenadeState
	{
		None = 0,
		Holstering = 1,
		Throwing = 2,
		Drawing = 3
	}

	private enum EnergyRegenMode
	{
		BeforeRegen = 0,
		Regernating = 1,
		NotRegenerating = 2
	}

	public enum TakedownPositioning
	{
		Front = 0,
		Back = 1,
		Total = 2
	}

	public enum TakedownAttackType
	{
		Lethal = 0,
		NonLethal = 1,
		Total = 2
	}

	public enum TakedownEnemyCount
	{
		Single = 0,
		Double = 1,
		Total = 2
	}

	public enum TakedownCamera
	{
		NoCamera = 0,
		Camera1 = 1,
		Camera2 = 2,
		Camera3 = 3,
		Camera4 = 4,
		Total = 5
	}

	private class TakedownCameraData
	{
		public TakedownCamera Cam;

		public float TransitionTime;
	}

	private class TakedownAnimationData
	{
		public string name = string.Empty;

		public string soundEventName = string.Empty;

		public float weight = 1f;

		private List<TakedownCameraData> cameraData = new List<TakedownCameraData>();

		private int currentIndex;

		private float transitionTime;

		private GameObject m_VFXPrefab;

		private GameObject m_ParticleEffect;

		public void Reset()
		{
			currentIndex = 0;
			transitionTime = 0f;
		}

		public bool Update(float updateTime)
		{
			if (currentIndex == cameraData.Count)
			{
				return false;
			}
			transitionTime += updateTime;
			if (transitionTime >= cameraData[currentIndex].TransitionTime)
			{
				currentIndex++;
				return true;
			}
			return false;
		}

		public TakedownCamera GetCurrentTakedownCamera()
		{
			return (currentIndex < cameraData.Count) ? cameraData[currentIndex].Cam : TakedownCamera.NoCamera;
		}

		public void AddCameraData(TakedownCamera cam, float transTime)
		{
			TakedownCameraData takedownCameraData = new TakedownCameraData();
			takedownCameraData.Cam = cam;
			takedownCameraData.TransitionTime = transTime;
			cameraData.Add(takedownCameraData);
		}

		public void AddVFXData(GameObject vfxPrefab)
		{
			m_VFXPrefab = vfxPrefab;
		}

		public void PlayVFX(GameObject parent)
		{
			if ((bool)m_VFXPrefab)
			{
				m_ParticleEffect = UnityEngine.Object.Instantiate(m_VFXPrefab) as GameObject;
				m_ParticleEffect.transform.parent = parent.transform;
				m_ParticleEffect.transform.localPosition = Vector3.zero;
				m_ParticleEffect.transform.localRotation = Quaternion.identity;
				m_ParticleEffect.transform.localScale = Vector3.one;
			}
		}

		public void CleanUpVFX()
		{
			if ((bool)m_ParticleEffect)
			{
				UnityEngine.Object.Destroy(m_ParticleEffect);
			}
		}
	}

	private class TakedownAnimation
	{
		private List<TakedownAnimationData> m_TakedownDataSingle = new List<TakedownAnimationData>();

		private List<TakedownAnimationData> m_TakedownDataDouble = new List<TakedownAnimationData>();

		public TakedownAnimationData AddAnimation(string name, TakedownEnemyCount count, string soundEventName)
		{
			TakedownAnimationData takedownAnimationData = new TakedownAnimationData();
			takedownAnimationData.name = name;
			takedownAnimationData.soundEventName = soundEventName;
			if (count == TakedownEnemyCount.Single)
			{
				m_TakedownDataSingle.Add(takedownAnimationData);
			}
			else
			{
				m_TakedownDataDouble.Add(takedownAnimationData);
			}
			return takedownAnimationData;
		}

		public TakedownAnimationData GetAnimationData(TakedownEnemyCount count)
		{
			List<TakedownAnimationData> list = ((count != 0) ? m_TakedownDataDouble : m_TakedownDataSingle);
			TakedownAnimationData takedownAnimationData = null;
			float num = 0f;
			for (int i = 0; i < list.Count; i++)
			{
				num += list[i].weight;
			}
			float num2 = UnityEngine.Random.Range(0f, num);
			bool flag = false;
			float num3 = 0f;
			for (int j = 0; j < list.Count; j++)
			{
				num3 += list[j].weight;
				if (num2 <= num3 && !flag)
				{
					flag = true;
					list[j].weight = 1f / (float)(list.Count * 2);
					takedownAnimationData = list[j];
				}
				else
				{
					list[j].weight = Mathf.Min(list[j].weight + 1f / (float)list.Count, 1f);
				}
			}
			if (takedownAnimationData == null)
			{
				Debug.Log("Oh sweet jeezus, something has gone terribly wrong. Tell Josh that Takedown randomness is broken.");
			}
			return takedownAnimationData;
		}
	}

	[Serializable]
	public class TakedownCameras
	{
		public Camera Camera1;

		public Camera Camera2;

		public Camera Camera3;

		public Camera Camera4;
	}

	[HideInInspector]
	public bool m_TappedInteractiveObject;

	[HideInInspector]
	public bool m_ForceCoverEdgeFacing;

	private bool m_ManualDeactivation;

	[HideInInspector]
	public InputDevice m_InputDevice;

	[HideInInspector]
	public Stance m_Stance;

	public float m_CameraHeightStanding = 1.7f;

	public float m_CameraHeightCrouching = 0.7f;

	public float m_ColliderHeightStanding = 1.8f;

	public float m_ColliderHeightCrouching = 0.8f;

	private CoverAnimationData[,,] m_CoverAnimationsLeft = new CoverAnimationData[2, 3, 30];

	private CoverAnimationData[,,] m_CoverAnimationsRight = new CoverAnimationData[2, 3, 30];

	private Vector3 m_CrouchModelLeftPosition = new Vector3(0.1201786f, -0.3416501f, 0.1390992f);

	private Vector3 m_CrouchModelLeftRotation = new Vector3(354.7026f, 350.5434f, 2.738506f);

	private Vector3 m_CrouchModelRightPosition = new Vector3(-0.1018031f, -0.3580197f, 0.06062237f);

	private Vector3 m_CrouchModelRightRotation = new Vector3(356.7647f, 7.310671f, 357.0506f);

	private Vector3 m_StandModelLeftPosition = new Vector3(0.2486276f, -0.3915758f, 0.2556792f);

	private Vector3 m_StandModelLeftRotation = new Vector3(352.7686f, 349.9556f, 2f);

	private Vector3 m_StandModelRightPosition = new Vector3(-0.4501003f, -0.3020859f, 0.2339182f);

	private Vector3 m_StandModelRightRotation = new Vector3(358.9457f, 5.942993f, 0.2234338f);

	private Vector3 m_UpOverModelLeftPosition = new Vector3(-0.1866937f, -0.5279203f, 0.1564844f);

	private Vector3 m_UpOverModelLeftRotation = new Vector3(356.7148f, 348.948f, 358.0896f);

	private Vector3 m_UpOverModelRightPosition = new Vector3(0.112804f, -0.5175542f, 0.06964448f);

	private Vector3 m_UpOverModelRightRotation = new Vector3(358.2076f, 0.7141621f, 357.6329f);

	private List<CoverAnimationSoundEvent> m_PendingSoundEvents = new List<CoverAnimationSoundEvent>();

	private AnimationState m_CurrentCoverAnimationState;

	[HideInInspector]
	public CoverState m_CoverState;

	[HideInInspector]
	public CoverSide m_CoverSide;

	[HideInInspector]
	public CoverFireSide m_CoverFireSide;

	[HideInInspector]
	public CoverEdge m_CoverEdge;

	[HideInInspector]
	public bool m_AllowUpAndOverCoverFire;

	private bool m_OneCoverShot;

	private bool m_ReloadNextCoverIdle;

	public float m_CoverDistFromWall = 0.5f;

	public float m_CoverCheckTime = 0.33f;

	private Vector3 m_AutoCoverPosition;

	private Vector3 m_AutoCoverNormal;

	private Collider m_AutoCoverCollider;

	public float m_CoverTestDistance = 1.5f;

	[HideInInspector]
	public bool m_ForceCoverButtonInactive;

	public float m_CoverEdgeCheckDistance = 0.3f;

	public float m_CoverEdgeCameraShiftAmount = 0.8f;

	private Quaternion m_OriginalCoverRotation;

	private Quaternion m_TargetCoverRotation;

	private Vector3 m_OriginalCoverPosition;

	private Vector3 m_TargetCoverPosition;

	private float m_ModelRotateCoverTimer;

	private float m_ModelRotateCoverTime = 0.3f;

	[HideInInspector]
	public float m_CoverSwitchSidesDot = 0.6f;

	private float m_CoverAimWeight;

	[HideInInspector]
	public float m_CameraCoverDot = 1f;

	[HideInInspector]
	public float m_CameraCoverAimLimit = 0.3f;

	[HideInInspector]
	public float m_CameraCoverAimLimitStanding = 0.8f;

	[HideInInspector]
	public Collider m_CoverCollider;

	[HideInInspector]
	public bool m_CoverAllowsCornering;

	[HideInInspector]
	public Vector3 m_CoverNormal;

	[HideInInspector]
	public Vector3 m_CoverPoint;

	[HideInInspector]
	public CoverFlipVolume m_CoverFlipVolume;

	private Vector3 m_CoverFlipVector;

	private Plane m_CoverFlipPlane;

	private float m_CoverFlipTimer;

	private float m_CoverFlipTime;

	private float m_CoverFlipDistance;

	public float m_CoverFlipDiveRange = 3f;

	private float m_CoverDiveMoveStop = 0.7f;

	private float m_CoverDiveCameraZoom = 1.1f;

	private Vector3 m_CoverFlipDestination;

	private bool m_CoverSlideFirstStage;

	private float m_CoverSlideTime = 0.5f;

	private float m_CoverSlideTimer;

	private Vector3 m_CoverSlideStartpoint;

	private Vector3 m_CoverSlideMidpoint;

	private Vector3 m_CoverSlideEndpoint;

	private float m_CoverSlideRotationAmount;

	public float m_StrafeSpeed = 4.25f;

	public float m_RunSpeed = 5f;

	public float m_CoverMoveSpeed = 2f;

	public GameObject m_Camera;

	public GameObject m_FirstPersonWeaponCamera;

	[HideInInspector]
	public Camera m_CurrentCamera;

	[HideInInspector]
	public Camera m_FirstPersonWeaponCameraComponent;

	[HideInInspector]
	public CameraMode m_CameraMode;

	public float m_CameraShiftSpeed = 5.5f;

	private float m_CurrentCameraShift;

	public float m_CameraBobAmount = 0.25f;

	private float m_CurrentCameraBobRotation;

	private float m_CameraBobTimer;

	public float m_UpAndOverCameraHeight = 1.5f;

	[HideInInspector]
	public int m_FrameNum;

	private PlayerMovement m_MovementScript;

	[HideInInspector]
	public WeaponBase m_WeaponScript;

	private PlayerDamage m_PlayerDamage;

	[HideInInspector]
	public GameObject m_PossibleEnemyTarget;

	[HideInInspector]
	public GameObject m_TargetedEnemy;

	[HideInInspector]
	public CharacterBase m_TargetedEnemyScript;

	private int m_EnemyTapID = -1;

	private float[] m_TapTime = new float[12];

	public GameObject m_ModelThirdPerson;

	public GameObject m_ModelFirstPerson;

	public Renderer m_RendererThirdPerson;

	public Renderer m_RendererFirstPerson;

	public Transform m_ThirdPersonHead;

	public GameObject[] m_WeaponList;

	public GameObject m_CurrentWeapon;

	public GameObject m_WeaponAttachPointFirstPerson;

	public GameObject m_WeaponAttachPointThirdPerson;

	[HideInInspector]
	public int m_NearInteractiveObject;

	public Collider m_BulletWhizCollider;

	private float m_ConcussionGrenadeTime = 4f;

	private float m_ConcussionQuadFadeTime = 2f;

	private float m_ConcussionGrenadeTimer;

	private PackedSprite m_ConcussionQuad;

	private bool m_EMPed;

	public string m_FootstepSoundEvent;

	public float m_FootstepMaxTime = 0.78f;

	public float m_FootstepMinTime = 0.35f;

	public float m_FootstepTimeCoverStand = 0.73f;

	public float m_FootstepTimeCoverCrouch = 1.13f;

	public float m_FootstepTimeCoverStandBack = 0.73f;

	public float m_FootstepTimeCoverCrouchBack = 0.9f;

	private float m_FootstepTimer;

	[HideInInspector]
	public int m_CurrentWeaponIndex = -1;

	private int m_FireTapID = -1;

	private bool m_WantFire;

	public float m_WeaponMoveAnimSpeedMax = 2f;

	public float m_WeaponMoveAnimSpeedMin = 1f;

	[HideInInspector]
	public int m_AfterHolsterWeapon;

	private int[] m_Ammo = new int[3];

	private int[] m_CurrentAmmo = new int[3];

	public int m_StartingCombatRifleAmmo = 200;

	public int m_StartingCrossbowAmmo = 20;

	public int m_StartingShotgunAmmo = 35;

	public int m_CurrentGrenadeType;

	private GrenadeState m_GrenadeState;

	private float m_GrenadeThrowTime;

	private float m_GrenadeThrowTimer;

	private float m_GrenadeReleaseTime;

	public GameObject m_GrenadeAttachPointFirstPerson;

	public GameObject m_GrenadeAttachPointThirdPersonLeft;

	public GameObject m_GrenadeAttachPointThirdPersonRight;

	private GameObject m_LastGrenadeObject;

	private GrenadeFrag m_LastGrenadeScript;

	private Color m_ReticleBaseColor = new Color(0.83f, 0.68f, 0.21f);

	private Color m_ReticleHostileColor = Color.red;

	private Color m_ReticleFriendlyColor = Color.green;

	[HideInInspector]
	public bool m_GodMode;

	public float m_TimeBeforeHealthRegen = 5f;

	private float m_HealthRegenTimer;

	public float m_HealthRegenPerSecond = 2f;

	private float m_HealthAccum;

	private float m_DeathTime = 5f;

	private float m_DeathTimer;

	private float m_MaxEnergy = 1f;

	private float m_CurrentEnergy;

	public float m_TimeBeforeEnergyRegen = 5f;

	private float m_EnergyRegenTimer;

	private float m_EnergyRegenTarget;

	public float m_EnergyRegenTime = 20f;

	private EnergyRegenMode m_EnergyRegenMode = EnergyRegenMode.NotRegenerating;

	private bool m_EnergyUsedThisFrame;

	private float m_DamageVODelay;

	private int m_CurrentArmor = 100;

	private TakedownAnimation[,] m_TakedownAnimation = new TakedownAnimation[2, 2];

	public Animation m_TakedownAnimator;

	public TakedownCameras m_TakedownCameras;

	public float m_TakedownDistance = 3f;

	public float m_TimeToHoldForLethal = 0.5f;

	public Collider m_PlayerInteractiveCollider;

	public GameObject m_TakedownModelPrefab;

	public GameObject m_ArmlockBladeFrontVFXPrefab;

	public GameObject m_DoubleBladeBackVFXPrefab;

	public GameObject m_KneeBackVFXPrefab;

	public GameObject m_SinglePunchFrontVFXPrefab;

	private Enemy_Base m_TakedownEnemy;

	private TakedownAnimationData m_CurrentAnimationData;

	private bool m_LethalTakedown;

	private bool m_PressedForTakedown;

	private float m_TakedownTimer;

	private WeaponBase.WeaponState m_PreWeaponState;

	private Renderer m_WeaponRenderer;

	private TakedownPositioning m_TakedownPositioning;

	private GameObject m_TakedownModel;

	public float GetCurrentEnergy()
	{
		return m_CurrentEnergy;
	}

	public void SetMaxEnergy(float maxEnergy)
	{
		m_MaxEnergy = maxEnergy;
	}

	public float GetMaxEnergy()
	{
		return m_MaxEnergy;
	}

	public bool AtMaxEnergy()
	{
		return m_CurrentEnergy >= m_MaxEnergy;
	}

	public bool IsEnergyAvailable(float amount, bool showWarning = true)
	{
		if (m_CurrentEnergy >= amount)
		{
			return true;
		}
		if (showWarning)
		{
			Globals.m_HUD.DisplayEnergyWarning();
			EventManager.Instance.PostEvent("HUD_Energy_Empty", EventAction.PlaySound, null, base.gameObject);
		}
		return false;
	}

	public void UseEnergy(float energy)
	{
		if (m_CurrentEnergy > 0f)
		{
			m_CurrentEnergy -= energy;
			m_EnergyUsedThisFrame = true;
			if ((float)(int)m_CurrentEnergy != m_CurrentEnergy || m_CurrentEnergy <= 0f)
			{
				if (m_CurrentEnergy < 0f)
				{
					m_CurrentEnergy = 0f;
					Globals.m_HUD.DisplayEnergyWarning();
				}
				m_EnergyRegenTimer = m_TimeBeforeEnergyRegen;
				m_EnergyRegenMode = EnergyRegenMode.BeforeRegen;
				m_EnergyRegenTarget = Mathf.Min((int)m_CurrentEnergy + 1, (int)m_MaxEnergy);
			}
		}
		else
		{
			Globals.m_HUD.DisplayEnergyWarning();
		}
	}

	public override Vector3 GetChestLocation()
	{
		return base.transform.position + Vector3.up * ((m_Stance != 0) ? 0.5f : 1.5f);
	}

	public int GetWeaponCurrentClip(int WeaponID)
	{
		if (WeaponID == m_CurrentWeaponIndex)
		{
			return m_WeaponScript.m_CurrentAmmo;
		}
		return m_CurrentAmmo[WeaponID];
	}

	public int GetWeaponTotalAmmo(int WeaponID)
	{
		if (WeaponID == m_CurrentWeaponIndex)
		{
			return m_WeaponScript.m_Ammo;
		}
		return m_Ammo[WeaponID];
	}

	public void AddAmmo(int WeaponID, int Ammo)
	{
		if (WeaponID == m_CurrentWeaponIndex)
		{
			m_WeaponScript.m_Ammo += Ammo;
			Globals.m_HUD.SetTotalAmmo(m_WeaponScript.m_Ammo);
		}
		else
		{
			m_Ammo[WeaponID] += Ammo;
		}
	}

	public bool NoAmmoForCurrentWeapon()
	{
		return m_WeaponScript.m_Ammo <= 0 && m_WeaponScript.m_CurrentAmmo <= 0;
	}

	public bool LowAmmoForCurrentWeaponClip()
	{
		return m_WeaponScript.m_LowAmmoThreshold > 0 && m_WeaponScript.m_CurrentAmmo <= m_WeaponScript.m_LowAmmoThreshold;
	}

	public WeaponType GetEquippedWeaponType()
	{
		return m_WeaponScript.m_WeaponType;
	}

	public bool WeaponHolstered()
	{
		return m_WeaponScript == null || m_WeaponScript.m_WeaponType == WeaponType.None || m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Holstering || m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Holstered;
	}

	public Vector3 GetHeadPosition()
	{
		if (m_CameraMode == CameraMode.First)
		{
			return m_CurrentCamera.transform.position;
		}
		if (m_ThirdPersonHead != null)
		{
			return m_ThirdPersonHead.position;
		}
		return m_CurrentCamera.transform.position;
	}

	public bool IsMoving()
	{
		return Globals.m_PlayerController.m_MovementScript.m_IsMoving;
	}

	private void Awake()
	{
		Globals.m_PlayerController = this;
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			m_InputDevice = InputDevice.Touch;
		}
		m_MovementScript = GetComponent<PlayerMovement>();
		m_CurrentCamera = m_Camera.GetComponent<Camera>();
		m_PlayerDamage = GetComponent<PlayerDamage>();
		m_FirstPersonWeaponCameraComponent = m_FirstPersonWeaponCamera.GetComponent<Camera>();
		Augmentation_Energy augmentation_Energy = (Augmentation_Energy)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Energy);
		m_CurrentEnergy = (m_MaxEnergy = augmentation_Energy.GetEnergyContainerAmount());
		SetupAnimList();
		SetupTakedowns();
		GameManager.LoadHUD();
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(CheckForNearbyCover());
		if (Globals.m_ShowFPS)
		{
			Globals.m_PlayerController.m_Camera.GetComponent<HUDFPS>().enabled = true;
		}
		else
		{
			Globals.m_PlayerController.m_Camera.GetComponent<HUDFPS>().enabled = false;
		}
		SetFirstPerson();
		if (GameManager.m_LastLevelCombatRifleAmmo == -1)
		{
			m_Ammo[0] = m_StartingCombatRifleAmmo;
		}
		else
		{
			m_Ammo[0] = GameManager.m_LastLevelCombatRifleAmmo;
		}
		if (GameManager.m_LastLevelCombatRifleCurrentAmmo == -1)
		{
			m_CurrentAmmo[0] = 20;
		}
		else
		{
			m_CurrentAmmo[0] = GameManager.m_LastLevelCombatRifleCurrentAmmo;
		}
		if (GameManager.m_LastLevelCrossbowAmmo == -1)
		{
			m_Ammo[1] = m_StartingCrossbowAmmo;
		}
		else
		{
			m_Ammo[1] = GameManager.m_LastLevelCrossbowAmmo;
		}
		if (GameManager.m_LastLevelCrossbowCurrentAmmo == -1)
		{
			m_CurrentAmmo[1] = 1;
		}
		else
		{
			m_CurrentAmmo[1] = GameManager.m_LastLevelCrossbowCurrentAmmo;
		}
		if (GameManager.m_LastLevelShotgunAmmo == -1)
		{
			m_Ammo[2] = m_StartingShotgunAmmo;
		}
		else
		{
			m_Ammo[2] = GameManager.m_LastLevelShotgunAmmo;
		}
		if (GameManager.m_LastLevelShotgunCurrentAmmo == -1)
		{
			m_CurrentAmmo[2] = 6;
		}
		else
		{
			m_CurrentAmmo[2] = GameManager.m_LastLevelShotgunCurrentAmmo;
		}
		SetWeapon(0);
		MobileBloom component = m_CurrentCamera.GetComponent<MobileBloom>();
		if ((bool)component)
		{
			component.enabled = Globals.m_Bloom;
		}
		m_ConcussionQuad = GameManager.CreateFullscreenQuad(GameManager.FullscreenQuadType.Add, 0.4f);
		m_ConcussionQuad.gameObject.active = false;
	}

	private void OnDestroy()
	{
		GameManager.m_LastLevelCombatRifleCurrentAmmo = GetWeaponCurrentClip(0);
		GameManager.m_LastLevelCombatRifleAmmo = GetWeaponTotalAmmo(0);
		GameManager.m_LastLevelCrossbowCurrentAmmo = GetWeaponCurrentClip(1);
		GameManager.m_LastLevelCrossbowAmmo = GetWeaponTotalAmmo(1);
		GameManager.m_LastLevelShotgunCurrentAmmo = GetWeaponCurrentClip(2);
		GameManager.m_LastLevelShotgunAmmo = GetWeaponTotalAmmo(2);
	}

	private void OnEnable()
	{
		if (m_ManualDeactivation)
		{
			if (m_Camera != null)
			{
				m_Camera.active = true;
			}
			if (m_FirstPersonWeaponCamera != null)
			{
				m_FirstPersonWeaponCamera.active = true;
			}
			if (Globals.m_HUD != null)
			{
				Globals.m_HUD.Display(true, true);
			}
			switch (m_CameraMode)
			{
			case CameraMode.First:
				m_ModelFirstPerson.SetActiveRecursively(true);
				break;
			case CameraMode.Third:
				m_ModelThirdPerson.SetActiveRecursively(true);
				break;
			}
			m_WeaponScript.ToggleHostered();
			StartCoroutine(CheckForNearbyCover());
			m_ManualDeactivation = false;
		}
	}

	private void OnDisable()
	{
		if (m_Camera != null)
		{
			m_Camera.active = false;
		}
		if (m_FirstPersonWeaponCamera != null)
		{
			m_FirstPersonWeaponCamera.active = false;
		}
		if (Globals.m_HUD != null && Globals.m_HUD.m_HUDPanel != null && UIManager.instance != null)
		{
			Globals.m_HUD.Display(false, false);
		}
		switch (m_CameraMode)
		{
		case CameraMode.First:
			if (m_ModelFirstPerson != null)
			{
				m_ModelFirstPerson.SetActiveRecursively(false);
			}
			break;
		case CameraMode.Third:
			if (m_ModelThirdPerson != null)
			{
				m_ModelThirdPerson.SetActiveRecursively(false);
			}
			break;
		}
		if (m_WeaponScript != null)
		{
			m_WeaponScript.Holster();
		}
		m_ManualDeactivation = true;
	}

	protected override void Update()
	{
		if (m_FrameNum == 0)
		{
			m_ModelThirdPerson.SetActiveRecursively(true);
			m_ModelThirdPerson.animation.Play("StandingIdle");
		}
		if (m_FrameNum == 1)
		{
			m_ModelThirdPerson.SetActiveRecursively(false);
		}
		if (m_FrameNum == 2)
		{
			Light[] array = UnityEngine.Object.FindObjectsOfType(typeof(Light)) as Light[];
			Debug.Log("Number of lights in scene: " + array.Length);
			Camera[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(Camera)) as Camera[];
			int num = 0;
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i].enabled && (array2[i].cullingMask & 1) == 1)
				{
					num++;
				}
			}
			Debug.Log("Number of cameras rendering default: " + num);
		}
		m_FrameNum++;
		base.Update();
		if (m_DeathTimer > 0f)
		{
			Globals.m_HUD.SetCurrentHealth(m_CurrentHealth);
			Globals.m_HUD.SetCurrentArmor(m_CurrentArmor);
			m_DeathTimer -= Time.deltaTime;
			if (m_DeathTimer <= 0f)
			{
				GameManager.ReloadCurrentLevel();
			}
			return;
		}
		if (m_PressedForTakedown)
		{
			m_TakedownTimer += Time.deltaTime;
			if (m_TakedownTimer >= m_TimeToHoldForLethal)
			{
				if (Vector3.Distance(m_ModelThirdPerson.transform.position, m_PossibleEnemyTarget.transform.position) <= m_TakedownDistance)
				{
					BeginTakedown(m_PossibleEnemyTarget, true);
					if (m_TargetedEnemy == m_PossibleEnemyTarget)
					{
						m_TargetedEnemy = null;
					}
				}
				else
				{
					m_PressedForTakedown = false;
				}
			}
		}
		switch (m_InputDevice)
		{
		case InputDevice.Mouse:
			MouseInputUpdate();
			break;
		case InputDevice.Touch:
			TouchInputUpdate();
			break;
		}
		if (m_GrenadeState == GrenadeState.Throwing)
		{
			m_GrenadeThrowTimer -= Time.deltaTime;
			if (m_GrenadeThrowTimer <= m_GrenadeReleaseTime)
			{
				m_GrenadeReleaseTime = -1000f;
				m_LastGrenadeObject.layer = 18;
				m_LastGrenadeObject.transform.parent = null;
				if (m_TargetedEnemy != null)
				{
					m_LastGrenadeScript.Throw(m_TargetedEnemyScript.GetChestLocation());
				}
				else
				{
					Vector3 forward = Vector3.forward;
					Quaternion quaternion = Quaternion.Euler(-20f, 0f, 0f);
					forward = quaternion * forward;
					forward = Globals.m_CameraController.transform.rotation * forward;
					forward *= 10f;
					m_LastGrenadeScript.ThrowVelocity(forward);
				}
			}
		}
		switch (m_CameraMode)
		{
		case CameraMode.First:
			UpdateFirstPerson();
			break;
		case CameraMode.Third:
			UpdateThirdPerson();
			break;
		case CameraMode.Takedown:
			UpdateTakedown();
			break;
		}
		if (m_CameraMode != CameraMode.Takedown)
		{
			float cameraHeight = GetCameraHeight();
			if (m_Camera.transform.localPosition.y != cameraHeight)
			{
				float y = m_Camera.transform.localPosition.y;
				y += (cameraHeight - y) * 4f * Time.deltaTime;
				if (m_Stance == Stance.Stand && y > cameraHeight - 0.001f)
				{
					y = cameraHeight;
				}
				if (m_Stance == Stance.Crouch && y < cameraHeight + 0.001f)
				{
					y = cameraHeight;
				}
				m_Camera.transform.localPosition = new Vector3(m_Camera.transform.localPosition.x, y, m_Camera.transform.localPosition.z);
			}
			float num2 = 0f;
			if (m_CoverState == CoverState.Outside)
			{
				num2 = Mathf.Lerp(m_FootstepMaxTime, m_FootstepMinTime, m_MovementScript.m_NormalizedSpeed);
			}
			else
			{
				switch (m_Stance)
				{
				case Stance.Stand:
					num2 = (m_MovementScript.m_CoverMovingBackward ? m_FootstepTimeCoverStandBack : m_FootstepTimeCoverStand);
					break;
				case Stance.Crouch:
					num2 = (m_MovementScript.m_CoverMovingBackward ? m_FootstepTimeCoverCrouchBack : m_FootstepTimeCoverCrouch);
					break;
				}
			}
			if (m_MovementScript.m_IsMoving)
			{
				m_FootstepTimer += Time.deltaTime;
				if (m_FootstepTimer > num2)
				{
					EventManager.Instance.PostEvent(m_FootstepSoundEvent, EventAction.PlaySound, base.gameObject);
					if (m_Stance == Stance.Stand && m_CoverState == CoverState.Outside)
					{
						Augmentation_Movement augmentation_Movement = (Augmentation_Movement)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Movement);
						float num3 = augmentation_Movement.GetMoveSilentlyNoiseRadius();
						Globals.m_AIDirector.CheckAudioSenses(base.transform.position, num3 * m_MovementScript.m_NormalizedSpeed, DisturbanceEvent.MinorAudio);
					}
					m_FootstepTimer -= num2;
				}
				if (m_CoverState == CoverState.Outside)
				{
					m_CameraBobTimer += Time.deltaTime * 0.5f;
					if (m_CameraBobTimer > num2)
					{
						m_CameraBobTimer -= num2;
					}
					float num4 = m_CameraBobTimer / num2;
					float num5 = num4 * 2f;
					if (num5 > 1f)
					{
						num5 = 1f - (num5 - 1f);
					}
					m_CurrentCameraBobRotation = Mathf.Lerp(0f - m_CameraBobAmount, m_CameraBobAmount, num5);
				}
				else
				{
					m_CurrentCameraBobRotation = 0f;
				}
			}
			else
			{
				m_FootstepTimer = num2 * 0.5f;
				m_CurrentCameraBobRotation = 0f;
				m_CameraBobTimer = num2 * 0.75f;
			}
			Globals.m_CameraController.m_ZRotation = m_CurrentCameraBobRotation;
			if (Globals.m_AutoRotate && m_TargetedEnemy != null)
			{
				Globals.m_CameraController.LookAtOverTime(m_TargetedEnemy, 32f);
			}
			else if (m_TargetedEnemy != null && Globals.m_CameraController.m_LookAtT == 0f && m_WantFire)
			{
				Globals.m_CameraController.MagnetizeTowardsTarget(m_TargetedEnemy, 70f, m_TargetedEnemyScript);
			}
			m_HealthRegenTimer -= Time.deltaTime;
			if (m_HealthRegenTimer <= 0f && m_CurrentHealth < m_MaxHealth)
			{
				m_HealthAccum += Time.deltaTime * m_HealthRegenPerSecond;
				if (m_HealthAccum >= 1f)
				{
					m_CurrentHealth++;
					m_HealthAccum -= 1f;
					if (m_CurrentHealth >= m_MaxHealth)
					{
						EventManager.Instance.PostEvent("HUD_Health_Full", EventAction.PlaySound, null, base.gameObject);
					}
				}
			}
			Globals.m_HUD.SetCurrentHealth(m_CurrentHealth);
			m_EnergyRegenTimer -= Time.deltaTime;
			if (m_EnergyRegenTimer <= 0f && m_EnergyRegenMode != EnergyRegenMode.NotRegenerating)
			{
				switch (m_EnergyRegenMode)
				{
				case EnergyRegenMode.BeforeRegen:
					m_EnergyRegenMode = EnergyRegenMode.Regernating;
					break;
				case EnergyRegenMode.Regernating:
					m_CurrentEnergy += Time.deltaTime * (1f / m_EnergyRegenTime);
					if (m_CurrentEnergy >= m_EnergyRegenTarget)
					{
						EnergyRegenComplete();
					}
					break;
				}
			}
			Globals.m_HUD.SetCurrentEnergy(m_CurrentEnergy, m_EnergyUsedThisFrame);
			m_EnergyUsedThisFrame = false;
			Globals.m_HUD.SetCurrentArmor(m_CurrentArmor);
		}
		if (m_ConcussionQuad.gameObject.active)
		{
			m_ConcussionGrenadeTimer -= Time.deltaTime;
			if (m_ConcussionGrenadeTimer < m_ConcussionQuadFadeTime)
			{
				Color color = m_ConcussionQuad.Color;
				color.a = 1f - (m_ConcussionQuadFadeTime - m_ConcussionGrenadeTimer) / m_ConcussionQuadFadeTime;
				m_ConcussionQuad.Color = color;
				if (m_EMPed && !Globals.m_HUD.m_Showing)
				{
					Globals.m_HUD.Display(true, true);
				}
			}
			if (m_ConcussionGrenadeTimer <= 0f)
			{
				m_ConcussionQuad.gameObject.active = false;
			}
		}
		m_DamageVODelay -= Time.deltaTime;
	}

	private void EnergyRegenComplete()
	{
		m_EnergyRegenMode = EnergyRegenMode.NotRegenerating;
		m_CurrentEnergy = m_EnergyRegenTarget;
		EventManager.Instance.PostEvent("HUD_Energy_Full", EventAction.PlaySound, null, base.gameObject);
	}

	private void UpdateFirstPerson()
	{
		WeaponBase.FirstPersonAnimation anim = WeaponBase.FirstPersonAnimation.Idle;
		m_WeaponScript.m_FirstPersonAnimationSpeed = 1f;
		if (m_MovementScript.m_IsMoving)
		{
			m_WeaponScript.m_FirstPersonAnimationSpeed = Mathf.Lerp(m_WeaponMoveAnimSpeedMin, m_WeaponMoveAnimSpeedMax, m_MovementScript.m_NormalizedSpeed);
			anim = WeaponBase.FirstPersonAnimation.Walk;
			Globals.m_HUD.TurnOnStanceButton(true);
			if (m_Stance == Stance.Crouch)
			{
				Vector3 origin = base.transform.position + Vector3.up * (m_MovementScript.m_PlayerCharacterController.radius + 0.1f);
				RaycastHit hitInfo;
				if (Physics.SphereCast(origin, m_MovementScript.m_PlayerCharacterController.radius + 0.2f, Vector3.up, out hitInfo, m_MovementScript.m_PlayerCharacterController.height + 0.4f, 257))
				{
					Globals.m_HUD.TurnOnStanceButton(false);
				}
			}
		}
		if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Firing)
		{
			anim = WeaponBase.FirstPersonAnimation.Fire;
		}
		if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Reloading)
		{
			anim = WeaponBase.FirstPersonAnimation.Reload;
		}
		if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Holstering)
		{
			anim = WeaponBase.FirstPersonAnimation.Holster;
		}
		if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Drawing)
		{
			anim = WeaponBase.FirstPersonAnimation.Draw;
		}
		if (m_GrenadeState != 0)
		{
			switch (m_GrenadeState)
			{
			case GrenadeState.Holstering:
				anim = WeaponBase.FirstPersonAnimation.Holster;
				m_WeaponScript.m_FirstPersonAnimationSpeed = 2f;
				if (!m_ModelFirstPerson.animation.isPlaying)
				{
					m_LastGrenadeObject.SetActiveRecursively(true);
					m_GrenadeState = GrenadeState.Throwing;
					anim = WeaponBase.FirstPersonAnimation.ThrowGrenade;
				}
				break;
			case GrenadeState.Throwing:
				anim = WeaponBase.FirstPersonAnimation.ThrowGrenade;
				if (!m_ModelFirstPerson.animation.isPlaying)
				{
					m_GrenadeState = GrenadeState.Drawing;
					anim = WeaponBase.FirstPersonAnimation.Draw;
				}
				break;
			case GrenadeState.Drawing:
				anim = WeaponBase.FirstPersonAnimation.Draw;
				m_WeaponScript.m_FirstPersonAnimationSpeed = 2f;
				if (!m_ModelFirstPerson.animation.isPlaying)
				{
					m_GrenadeState = GrenadeState.None;
				}
				break;
			}
		}
		if (m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Holstered)
		{
			m_WeaponScript.SetFirstPersonAnim(anim);
		}
		else if (m_AfterHolsterWeapon != m_CurrentWeaponIndex)
		{
			DestroyCurrentWeapon();
			SetWeapon(m_AfterHolsterWeapon);
		}
	}

	private void UpdateThirdPerson()
	{
		for (int num = m_PendingSoundEvents.Count - 1; num >= 0; num--)
		{
			CoverAnimationSoundEvent coverAnimationSoundEvent = m_PendingSoundEvents[num];
			coverAnimationSoundEvent.delayTimer -= Time.deltaTime;
			if (coverAnimationSoundEvent.delayTimer <= 0f)
			{
				EventManager.Instance.PostEvent(coverAnimationSoundEvent.name, EventAction.PlaySound, null, base.gameObject);
				m_PendingSoundEvents.Remove(coverAnimationSoundEvent);
			}
		}
		m_WeaponScript.SetFirstPersonAnim(WeaponBase.FirstPersonAnimation.None);
		float num2 = 0f;
		float num3 = Globals.m_CameraController.m_CameraDistThirdPerson;
		float num4 = m_CameraShiftSpeed;
		switch (m_CoverEdge)
		{
		case CoverEdge.Left:
			num2 = 0f - m_CoverEdgeCameraShiftAmount;
			break;
		case CoverEdge.Right:
			num2 = m_CoverEdgeCameraShiftAmount;
			break;
		case CoverEdge.Both:
			switch (m_CoverSide)
			{
			case CoverSide.Left:
				num2 = 0f - m_CoverEdgeCameraShiftAmount;
				break;
			case CoverSide.Right:
				num2 = m_CoverEdgeCameraShiftAmount;
				break;
			}
			break;
		case CoverEdge.None:
			if (m_CoverState == CoverState.CoverFlip)
			{
				num2 = 0f - m_CoverFlipPlane.GetDistanceToPoint(base.transform.position);
				if (m_CoverSide == CoverSide.Right)
				{
					num2 = 0f - num2;
				}
				num4 = 10f;
			}
			else if (m_CoverState == CoverState.CoverDive)
			{
				switch (m_CoverSide)
				{
				case CoverSide.Left:
					num2 = m_CoverEdgeCameraShiftAmount;
					break;
				case CoverSide.Right:
					num2 = 0f - m_CoverEdgeCameraShiftAmount;
					break;
				}
				float num5 = m_CoverFlipTimer / (m_CoverFlipTime * m_CoverDiveMoveStop) * 2f;
				if (num5 > 2f)
				{
					num5 = 2f;
				}
				if (num5 > 1f)
				{
					num5 = 1f - (num5 - 1f);
				}
				num3 += m_CoverDiveCameraZoom * num5;
			}
			break;
		}
		m_CurrentCameraShift += (num2 - m_CurrentCameraShift) * num4 * Time.deltaTime;
		m_CurrentCamera.transform.localPosition = new Vector3(m_CurrentCameraShift, m_CurrentCamera.transform.localPosition.y, 0f - num3);
		if (m_ModelRotateCoverTimer > 0f)
		{
			m_ModelRotateCoverTimer -= Time.deltaTime;
			if (m_ModelRotateCoverTimer < 0f)
			{
				m_ModelRotateCoverTimer = 0f;
			}
			m_ModelThirdPerson.transform.localRotation = Quaternion.Slerp(m_OriginalCoverRotation, m_TargetCoverRotation, 1f - m_ModelRotateCoverTimer / m_ModelRotateCoverTime);
			m_ModelThirdPerson.transform.localPosition = Vector3.Lerp(m_OriginalCoverPosition, m_TargetCoverPosition, 1f - m_ModelRotateCoverTimer / m_ModelRotateCoverTime);
		}
		Vector3 forward = m_Camera.transform.forward;
		forward.y = 0f;
		forward.Normalize();
		m_CameraCoverDot = Vector3.Dot(base.transform.forward, forward);
		if (m_AllowUpAndOverCoverFire && m_CoverEdge == CoverEdge.None)
		{
			float num6 = Globals.m_CameraController.GetPitch() / 90f;
			Vector3 thirdPersonAttachOriginalPos = Globals.m_CameraController.m_ThirdPersonAttachOriginalPos;
			if (num6 > 0f)
			{
				thirdPersonAttachOriginalPos.y += num6 * m_UpAndOverCameraHeight;
			}
			Globals.m_CameraController.m_ThirdPersonAttach.transform.localPosition = thirdPersonAttachOriginalPos;
		}
		else
		{
			Globals.m_CameraController.m_ThirdPersonAttach.transform.localPosition = Globals.m_CameraController.m_ThirdPersonAttachOriginalPos;
		}
		float num7 = m_CameraCoverAimLimit;
		if (m_WantFire && m_CoverEdge == CoverEdge.None && !m_AllowUpAndOverCoverFire)
		{
			num7 = m_CameraCoverAimLimitStanding;
		}
		switch (m_CoverState)
		{
		case CoverState.TransitioningIn:
		case CoverState.SwitchingSides:
		case CoverState.Drawing:
			if (m_CurrentCoverAnimationState.time >= m_CurrentCoverAnimationState.length - 0.2f || !m_CurrentCoverAnimationState.enabled)
			{
				SetCoverState(CoverState.Inside);
				UpdateCoverEdge();
			}
			break;
		case CoverState.CoverFlip:
		case CoverState.CoverDive:
			m_CoverEdge = CoverEdge.None;
			m_CoverFlipTimer += Time.deltaTime;
			if (m_CoverState == CoverState.CoverFlip || m_CoverFlipTimer < m_CoverFlipTime * m_CoverDiveMoveStop)
			{
				m_MovementScript.m_PlayerCharacterController.SimpleMove(m_CoverFlipVector);
			}
			else
			{
				base.transform.position = m_CoverFlipDestination;
			}
			if (m_CoverFlipTimer >= m_CoverFlipTime)
			{
				base.transform.position = m_CoverFlipDestination;
				if (m_CoverSide == CoverSide.Left)
				{
					SetCoverSide(CoverSide.Right);
				}
				else
				{
					SetCoverSide(CoverSide.Left);
				}
				SetCoverState(CoverState.Inside);
				UpdateCover();
			}
			break;
		case CoverState.CoverSlideInner:
		{
			m_CoverEdge = CoverEdge.None;
			m_CoverSlideTimer -= Time.deltaTime;
			float t = 1f - m_CoverSlideTimer / m_CoverSlideTime;
			base.transform.position = Vector3.Lerp(m_CoverSlideStartpoint, m_CoverSlideEndpoint, t);
			float num9 = 1f;
			if (m_CoverSide == CoverSide.Left)
			{
				num9 = -1f;
			}
			base.transform.Rotate(0f, 90f / m_CoverSlideTime * Time.deltaTime * num9, 0f);
			Globals.m_CameraController.RotateYaw(90f / m_CoverSlideTime * Time.deltaTime * num9);
			if (m_CoverSlideTimer <= 0f)
			{
				base.transform.position = m_CoverSlideEndpoint;
				Vector3 coverPoint = m_CoverPoint;
				coverPoint.y = base.transform.position.y;
				base.transform.LookAt(coverPoint);
				SetCoverState(CoverState.Inside);
				UpdateCover();
			}
			break;
		}
		case CoverState.CoverSlideOuter:
		{
			m_CoverEdge = CoverEdge.None;
			m_CoverSlideTimer -= Time.deltaTime;
			float t2 = 1f - m_CoverSlideTimer / m_CoverSlideTime;
			if (m_CoverSlideFirstStage)
			{
				base.transform.position = Vector3.Lerp(m_CoverSlideStartpoint, m_CoverSlideMidpoint, t2);
			}
			else
			{
				base.transform.position = Vector3.Lerp(m_CoverSlideMidpoint, m_CoverSlideEndpoint, t2);
			}
			float num10 = 1f;
			if (m_CoverSide == CoverSide.Left)
			{
				num10 = -1f;
			}
			base.transform.Rotate(0f, -45f / m_CoverSlideTime * Time.deltaTime * num10, 0f);
			Globals.m_CameraController.RotateYaw(m_CoverSlideRotationAmount * 0.5f / m_CoverSlideTime * Time.deltaTime * num10);
			if (m_CoverSlideTimer <= 0f)
			{
				if (m_CoverSlideFirstStage)
				{
					m_CoverSlideTimer = m_CoverSlideTime;
					m_CoverSlideFirstStage = false;
					break;
				}
				base.transform.position = m_CoverSlideEndpoint;
				Vector3 coverPoint2 = m_CoverPoint;
				coverPoint2.y = base.transform.position.y;
				base.transform.LookAt(coverPoint2);
				SetCoverState(CoverState.Inside);
				UpdateCover();
			}
			break;
		}
		case CoverState.Stand:
		case CoverState.Crouch:
			if (m_CameraCoverDot < num7)
			{
				SetCoverState(CoverState.CoverAiming);
			}
			else if (!m_ModelThirdPerson.animation.isPlaying)
			{
				SetCoverState(CoverState.Inside);
			}
			break;
		case CoverState.Reloading:
			if (m_CameraCoverDot < num7)
			{
				SetCoverState(CoverState.CoverAiming);
			}
			else if (m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Reloading)
			{
				SetCoverState(CoverState.Inside);
			}
			break;
		case CoverState.Inside:
		{
			if (m_CameraCoverDot < num7)
			{
				SetCoverState(CoverState.CoverAiming);
				break;
			}
			if (m_MovementScript.m_IsMoving)
			{
				if (!m_MovementScript.m_CoverMovingBackward)
				{
					PlayCoverAnimation(CoverAnimation.Move, 0f);
				}
				else
				{
					PlayCoverAnimation(CoverAnimation.MoveBackwards, 0f);
				}
			}
			else
			{
				PlayCoverAnimation(CoverAnimation.Idle, 0f);
			}
			if (m_CoverEdge != 0)
			{
				break;
			}
			float num8 = Vector3.Dot(base.transform.right, forward);
			switch (m_CoverSide)
			{
			case CoverSide.Left:
				if (num8 > m_CoverSwitchSidesDot)
				{
					SetCoverState(CoverState.SwitchingSides);
				}
				break;
			case CoverSide.Right:
				if (num8 < 0f - m_CoverSwitchSidesDot)
				{
					SetCoverState(CoverState.SwitchingSides);
				}
				break;
			}
			break;
		}
		case CoverState.TransitioningToFire:
			if (!m_ModelThirdPerson.animation.isPlaying)
			{
				if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Firing)
				{
					SetCoverState(CoverState.Firing);
				}
				else
				{
					SetCoverState(CoverState.CoverAiming);
				}
				m_WeaponScript.StartFire();
				if (m_OneCoverShot)
				{
					StopFiringAfterOneShot();
				}
			}
			break;
		case CoverState.Firing:
			if (m_CameraCoverDot < num7)
			{
				SetCoverState(CoverState.CoverAiming);
				break;
			}
			if (IsAimingAtWall())
			{
				StopFiringAfterOneShot();
			}
			if (m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Firing)
			{
				if (!m_WantFire)
				{
					m_WeaponScript.ResetFOV();
					SetCoverState(CoverState.TransitioningFromFire);
				}
				else
				{
					m_ModelThirdPerson.animation.Stop();
					PlayCoverAnimation(CoverAnimation.LeanFiring, 0f);
				}
			}
			break;
		case CoverState.TransitioningFromFire:
			if (!m_ModelThirdPerson.animation.isPlaying)
			{
				if (m_ReloadNextCoverIdle)
				{
					SetCoverState(CoverState.Reloading);
				}
				else
				{
					SetCoverState(CoverState.Inside);
				}
			}
			break;
		case CoverState.CoverAiming:
		{
			if (m_CameraCoverDot >= num7)
			{
				if (IsAimingAtWall() && m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Firing)
				{
					SetCoverState(CoverState.TransitioningFromFire);
					m_WeaponScript.CancelFire();
					m_WeaponScript.ResetFOV();
					break;
				}
				if (m_GrenadeState != GrenadeState.Throwing)
				{
					switch (m_WeaponScript.m_WeaponState)
					{
					case WeaponBase.WeaponState.Idle:
						SetCoverState(CoverState.TransitioningFromFire);
						break;
					case WeaponBase.WeaponState.Firing:
						if (m_CoverEdge == CoverEdge.None && m_CoverFireSide == CoverFireSide.None)
						{
							StopFiringAfterOneShot();
						}
						else
						{
							SetCoverState(CoverState.Firing);
						}
						break;
					case WeaponBase.WeaponState.Reloading:
						SetCoverState(CoverState.Reloading);
						break;
					}
				}
				else
				{
					SetCoverState(CoverState.ThrowGrenade);
				}
				m_WeaponScript.ResetFOV();
				break;
			}
			float num11 = Vector3.Dot(base.transform.right, m_Camera.transform.forward);
			float num12 = num11 * 0.5f + 0.5f;
			if (m_CoverAimWeight < num12)
			{
				m_CoverAimWeight += Time.deltaTime * 6f;
				if (m_CoverAimWeight > num12)
				{
					m_CoverAimWeight = num12;
				}
			}
			if (m_CoverAimWeight > num12)
			{
				m_CoverAimWeight -= Time.deltaTime * 6f;
				if (m_CoverAimWeight < num12)
				{
					m_CoverAimWeight = num12;
				}
			}
			if (m_CoverEdge != 0)
			{
				switch (m_CoverSide)
				{
				case CoverSide.Left:
					m_CoverAimWeight = 0f;
					break;
				case CoverSide.Right:
					m_CoverAimWeight = 1f;
					break;
				}
			}
			AnimationState animationState = m_ModelThirdPerson.animation[m_CoverAnimationsLeft[1, (int)m_WeaponScript.m_WeaponAnimSet, 13].name];
			AnimationState animationState2 = m_ModelThirdPerson.animation[m_CoverAnimationsRight[1, (int)m_WeaponScript.m_WeaponAnimSet, 13].name];
			AnimationState animationState3 = m_ModelThirdPerson.animation[m_CoverAnimationsLeft[1, (int)m_WeaponScript.m_WeaponAnimSet, 14].name];
			AnimationState animationState4 = m_ModelThirdPerson.animation[m_CoverAnimationsRight[1, (int)m_WeaponScript.m_WeaponAnimSet, 14].name];
			AnimationState animationState5 = m_ModelThirdPerson.animation[m_CoverAnimationsLeft[1, (int)m_WeaponScript.m_WeaponAnimSet, 15].name];
			AnimationState animationState6 = m_ModelThirdPerson.animation[m_CoverAnimationsRight[1, (int)m_WeaponScript.m_WeaponAnimSet, 15].name];
			AnimationState animationState7 = m_ModelThirdPerson.animation[m_CoverAnimationsLeft[1, (int)m_WeaponScript.m_WeaponAnimSet, 16].name];
			AnimationState animationState8 = m_ModelThirdPerson.animation[m_CoverAnimationsRight[1, (int)m_WeaponScript.m_WeaponAnimSet, 16].name];
			AnimationState animationState9 = m_ModelThirdPerson.animation[m_CoverAnimationsLeft[0, (int)m_WeaponScript.m_WeaponAnimSet, 13].name];
			AnimationState animationState10 = m_ModelThirdPerson.animation[m_CoverAnimationsRight[0, (int)m_WeaponScript.m_WeaponAnimSet, 13].name];
			AnimationState animationState11 = m_ModelThirdPerson.animation[m_CoverAnimationsLeft[0, (int)m_WeaponScript.m_WeaponAnimSet, 14].name];
			AnimationState animationState12 = m_ModelThirdPerson.animation[m_CoverAnimationsRight[0, (int)m_WeaponScript.m_WeaponAnimSet, 14].name];
			AnimationState animationState13 = m_ModelThirdPerson.animation[m_CoverAnimationsLeft[0, (int)m_WeaponScript.m_WeaponAnimSet, 15].name];
			AnimationState animationState14 = m_ModelThirdPerson.animation[m_CoverAnimationsRight[0, (int)m_WeaponScript.m_WeaponAnimSet, 15].name];
			AnimationState animationState15 = m_ModelThirdPerson.animation[m_CoverAnimationsLeft[0, (int)m_WeaponScript.m_WeaponAnimSet, 16].name];
			AnimationState animationState16 = m_ModelThirdPerson.animation[m_CoverAnimationsRight[0, (int)m_WeaponScript.m_WeaponAnimSet, 16].name];
			AnimationState animationState17 = m_ModelThirdPerson.animation[m_CoverAnimationsLeft[1, (int)m_WeaponScript.m_WeaponAnimSet, 17].name];
			AnimationState animationState18 = m_ModelThirdPerson.animation[m_CoverAnimationsRight[1, (int)m_WeaponScript.m_WeaponAnimSet, 17].name];
			AnimationState animationState19 = m_ModelThirdPerson.animation[m_CoverAnimationsLeft[1, (int)m_WeaponScript.m_WeaponAnimSet, 18].name];
			AnimationState animationState20 = m_ModelThirdPerson.animation[m_CoverAnimationsRight[1, (int)m_WeaponScript.m_WeaponAnimSet, 18].name];
			AnimationState animationState21 = m_ModelThirdPerson.animation[m_CoverAnimationsLeft[1, (int)m_WeaponScript.m_WeaponAnimSet, 19].name];
			AnimationState animationState22 = m_ModelThirdPerson.animation[m_CoverAnimationsRight[1, (int)m_WeaponScript.m_WeaponAnimSet, 19].name];
			animationState.weight = 0f;
			animationState2.weight = 0f;
			animationState3.weight = 0f;
			animationState4.weight = 0f;
			animationState5.weight = 0f;
			animationState6.weight = 0f;
			animationState7.weight = 0f;
			animationState8.weight = 0f;
			animationState.enabled = true;
			animationState2.enabled = true;
			animationState3.enabled = true;
			animationState4.enabled = true;
			animationState5.enabled = true;
			animationState6.enabled = true;
			animationState7.enabled = true;
			animationState8.enabled = true;
			animationState9.weight = 0f;
			animationState10.weight = 0f;
			animationState11.weight = 0f;
			animationState12.weight = 0f;
			animationState13.weight = 0f;
			animationState14.weight = 0f;
			animationState15.weight = 0f;
			animationState16.weight = 0f;
			animationState9.enabled = true;
			animationState10.enabled = true;
			animationState11.enabled = true;
			animationState12.enabled = true;
			animationState13.enabled = true;
			animationState14.enabled = true;
			animationState15.enabled = true;
			animationState16.enabled = true;
			animationState17.weight = 0f;
			animationState18.weight = 0f;
			animationState19.weight = 0f;
			animationState20.weight = 0f;
			animationState21.weight = 0f;
			animationState22.weight = 0f;
			animationState17.enabled = true;
			animationState18.enabled = true;
			animationState19.enabled = true;
			animationState20.enabled = true;
			animationState21.enabled = true;
			animationState22.enabled = true;
			float num13 = (m_Camera.transform.localPosition.y - Globals.m_CameraController.m_CameraHeightThirdPerson) / (Globals.m_CameraController.m_CameraHeightThirdPersonStanding - Globals.m_CameraController.m_CameraHeightThirdPerson);
			switch (m_WeaponScript.m_WeaponState)
			{
			case WeaponBase.WeaponState.Idle:
			case WeaponBase.WeaponState.Aiming:
				if (m_GrenadeState == GrenadeState.Throwing)
				{
					animationState7.wrapMode = WrapMode.Loop;
					animationState7.weight = (1f - m_CoverAimWeight) * (1f - num13);
					animationState8.wrapMode = WrapMode.Loop;
					animationState8.weight = m_CoverAimWeight * (1f - num13);
					animationState15.wrapMode = WrapMode.Loop;
					animationState15.weight = (1f - m_CoverAimWeight) * num13;
					animationState16.wrapMode = WrapMode.Loop;
					animationState16.weight = m_CoverAimWeight * num13;
					animationState7.time = m_GrenadeThrowTime - m_GrenadeThrowTimer;
					animationState8.time = m_GrenadeThrowTime - m_GrenadeThrowTimer;
					animationState15.time = m_GrenadeThrowTime - m_GrenadeThrowTimer;
					animationState16.time = m_GrenadeThrowTime - m_GrenadeThrowTimer;
					if (m_GrenadeThrowTimer < 0f)
					{
						m_GrenadeState = GrenadeState.None;
					}
				}
				else if (m_CoverFireSide != CoverFireSide.UpOver)
				{
					animationState.wrapMode = WrapMode.Loop;
					animationState.weight = (1f - m_CoverAimWeight) * (1f - num13);
					animationState2.wrapMode = WrapMode.Loop;
					animationState2.weight = m_CoverAimWeight * (1f - num13);
					animationState9.wrapMode = WrapMode.Loop;
					animationState9.weight = (1f - m_CoverAimWeight) * num13;
					animationState10.wrapMode = WrapMode.Loop;
					animationState10.weight = m_CoverAimWeight * num13;
				}
				else
				{
					animationState17.wrapMode = WrapMode.Loop;
					animationState17.weight = 1f - m_CoverAimWeight;
					animationState18.wrapMode = WrapMode.Loop;
					animationState18.weight = m_CoverAimWeight;
				}
				break;
			case WeaponBase.WeaponState.Firing:
				if (m_CoverFireSide != CoverFireSide.UpOver)
				{
					animationState3.wrapMode = WrapMode.Loop;
					animationState3.weight = (1f - m_CoverAimWeight) * (1f - num13);
					animationState4.wrapMode = WrapMode.Loop;
					animationState4.weight = m_CoverAimWeight * (1f - num13);
					animationState11.wrapMode = WrapMode.Loop;
					animationState11.weight = (1f - m_CoverAimWeight) * num13;
					animationState12.wrapMode = WrapMode.Loop;
					animationState12.weight = m_CoverAimWeight * num13;
				}
				else
				{
					animationState19.wrapMode = WrapMode.Loop;
					animationState19.weight = 1f - m_CoverAimWeight;
					animationState20.wrapMode = WrapMode.Loop;
					animationState20.weight = m_CoverAimWeight;
				}
				break;
			case WeaponBase.WeaponState.Reloading:
				if (m_CoverFireSide != CoverFireSide.UpOver)
				{
					animationState5.wrapMode = WrapMode.Once;
					animationState5.weight = (1f - m_CoverAimWeight) * (1f - num13);
					animationState6.wrapMode = WrapMode.Once;
					animationState6.weight = m_CoverAimWeight * (1f - num13);
					animationState13.wrapMode = WrapMode.Once;
					animationState13.weight = (1f - m_CoverAimWeight) * num13;
					animationState14.wrapMode = WrapMode.Once;
					animationState14.weight = m_CoverAimWeight * num13;
					animationState5.time = m_WeaponScript.m_ReloadTime - m_WeaponScript.m_ReloadTimer;
					animationState6.time = m_WeaponScript.m_ReloadTime - m_WeaponScript.m_ReloadTimer;
					animationState13.time = m_WeaponScript.m_ReloadTime - m_WeaponScript.m_ReloadTimer;
					animationState14.time = m_WeaponScript.m_ReloadTime - m_WeaponScript.m_ReloadTimer;
				}
				else
				{
					animationState21.wrapMode = WrapMode.Once;
					animationState21.weight = 1f - m_CoverAimWeight;
					animationState22.wrapMode = WrapMode.Once;
					animationState22.weight = m_CoverAimWeight;
					animationState21.time = m_WeaponScript.m_ReloadTime - m_WeaponScript.m_ReloadTimer;
					animationState22.time = m_WeaponScript.m_ReloadTime - m_WeaponScript.m_ReloadTimer;
				}
				break;
			}
			if (m_CoverFireSide != CoverFireSide.UpOver)
			{
				float num14 = 0f;
				if (m_CoverEdge == CoverEdge.None)
				{
					num14 = m_CoverEdgeCameraShiftAmount;
				}
				Quaternion from = Quaternion.Slerp(Quaternion.Euler(m_CrouchModelLeftRotation), Quaternion.Euler(m_CrouchModelRightRotation), m_CoverAimWeight);
				Vector3 from2 = new Vector3(Mathf.Lerp(m_CrouchModelLeftPosition.x + num14, m_CrouchModelRightPosition.x - num14, m_CoverAimWeight), Mathf.Lerp(m_CrouchModelLeftPosition.y, m_CrouchModelRightPosition.y, m_CoverAimWeight), Mathf.Lerp(m_CrouchModelLeftPosition.z, m_CrouchModelRightPosition.z, m_CoverAimWeight));
				Quaternion to = Quaternion.Slerp(Quaternion.Euler(m_StandModelLeftRotation), Quaternion.Euler(m_StandModelRightRotation), m_CoverAimWeight);
				Vector3 to2 = new Vector3(Mathf.Lerp(m_StandModelLeftPosition.x + num14, m_StandModelRightPosition.x - num14, m_CoverAimWeight), Mathf.Lerp(m_StandModelLeftPosition.y, m_StandModelRightPosition.y, m_CoverAimWeight), Mathf.Lerp(m_StandModelLeftPosition.z, m_StandModelRightPosition.z, m_CoverAimWeight));
				m_ModelThirdPerson.transform.localRotation = Quaternion.Slerp(from, to, num13);
				m_ModelThirdPerson.transform.localPosition = Vector3.Slerp(from2, to2, num13);
			}
			else
			{
				m_ModelThirdPerson.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(m_UpOverModelLeftRotation), Quaternion.Euler(m_UpOverModelRightRotation), m_CoverAimWeight);
				m_ModelThirdPerson.transform.localPosition = new Vector3(Mathf.Lerp(m_UpOverModelLeftPosition.x, m_UpOverModelRightPosition.x, m_CoverAimWeight), Mathf.Lerp(m_UpOverModelLeftPosition.y, m_UpOverModelRightPosition.y, m_CoverAimWeight), Mathf.Lerp(m_UpOverModelLeftPosition.z, m_UpOverModelRightPosition.z, m_CoverAimWeight));
			}
			if (m_CoverEdge != 0)
			{
				break;
			}
			float num15 = Vector3.Dot(base.transform.right, forward);
			switch (m_CoverSide)
			{
			case CoverSide.Left:
				if (num15 > 0f)
				{
					SetCoverSide(CoverSide.Right);
				}
				break;
			case CoverSide.Right:
				if (num15 <= 0f)
				{
					SetCoverSide(CoverSide.Left);
				}
				break;
			}
			break;
		}
		case CoverState.Holstering:
			if (!m_ModelThirdPerson.animation.isPlaying)
			{
				if (m_AfterHolsterWeapon != m_CurrentWeaponIndex)
				{
					DestroyCurrentWeapon();
					SetWeapon(m_AfterHolsterWeapon);
					SetCoverState(CoverState.Drawing);
				}
				else
				{
					m_WeaponScript.ToggleHostered();
					SetCoverState(CoverState.Inside);
				}
			}
			break;
		case CoverState.ThrowGrenade:
			if (m_CameraCoverDot < num7)
			{
				SetCoverState(CoverState.CoverAiming);
			}
			else if (!m_ModelThirdPerson.animation.isPlaying)
			{
				SetCoverState(CoverState.Inside);
				m_GrenadeState = GrenadeState.None;
			}
			break;
		}
	}

	private bool IsAimingAtWall()
	{
		Vector2 vector = new Vector2(Screen.width / 2, Screen.height / 2);
		Ray ray = m_CurrentCamera.ScreenPointToRay(vector);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 3f, 257) && hitInfo.distance < 2f)
		{
			return true;
		}
		return false;
	}

	public void SetWeapon(int index)
	{
		if (m_CurrentHealth <= 0)
		{
			return;
		}
		m_AfterHolsterWeapon = index;
		if (index == m_CurrentWeaponIndex)
		{
			ToggleWeaponHolstered();
			return;
		}
		if (m_CurrentWeapon != null)
		{
			if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Holstered)
			{
				DestroyCurrentWeapon();
			}
			else if (m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Holstered)
			{
				m_WeaponScript.ToggleHostered();
				if (m_CameraMode == CameraMode.Third)
				{
					SetCoverState(CoverState.Holstering);
				}
			}
		}
		if (!(m_CurrentWeapon == null))
		{
			return;
		}
		m_CurrentWeapon = (GameObject)UnityEngine.Object.Instantiate(m_WeaponList[index], base.transform.position, base.transform.rotation);
		m_CurrentWeapon.transform.parent = base.transform;
		m_WeaponScript = m_CurrentWeapon.GetComponent<WeaponBase>();
		m_WeaponScript.m_Ammo = m_Ammo[index];
		m_WeaponScript.m_CurrentAmmo = m_CurrentAmmo[index];
		Globals.m_HUD.SetTotalAmmo(m_WeaponScript.m_Ammo);
		Globals.m_HUD.SetCurrentAmmo(m_WeaponScript.m_CurrentAmmo);
		Globals.m_HUD.SetWeaponIcon((WeaponType)index);
		m_WeaponScript.SetUser(this);
		AttachThirdPersonWeapon();
		m_WeaponScript.m_ModelFirstPerson.transform.parent = m_WeaponAttachPointFirstPerson.transform;
		m_WeaponScript.m_ModelFirstPerson.transform.localPosition = Vector3.zero;
		m_WeaponScript.m_ModelFirstPerson.transform.localRotation = Quaternion.identity;
		m_WeaponScript.m_ModelThirdPersonEnemy.SetActiveRecursively(false);
		if (m_CameraMode == CameraMode.First)
		{
			m_WeaponScript.SetFirstPerson();
			m_WeaponScript.m_ModelFirstPerson.SetActiveRecursively(true);
			m_WeaponScript.m_ModelThirdPersonPlayer.SetActiveRecursively(false);
			if (m_CurrentWeaponIndex != -1)
			{
				m_WeaponScript.Holster();
				m_WeaponScript.ToggleHostered();
			}
		}
		else
		{
			m_WeaponScript.SetThirdPerson();
			m_WeaponScript.PlayFirstPersonIdle(true);
			m_WeaponScript.m_ModelFirstPerson.SetActiveRecursively(false);
			m_WeaponScript.m_ModelThirdPersonPlayer.SetActiveRecursively(true);
			SetCoverState(CoverState.Drawing);
		}
		if (Globals.m_AugmentCloaking.enabled)
		{
			Globals.m_AugmentCloaking.PlayerWeaponSwitched();
		}
		m_CurrentWeaponIndex = index;
	}

	private void DestroyCurrentWeapon()
	{
		m_Ammo[m_CurrentWeaponIndex] = m_WeaponScript.m_Ammo;
		m_CurrentAmmo[m_CurrentWeaponIndex] = m_WeaponScript.m_CurrentAmmo;
		m_WeaponScript.m_ModelFirstPerson.transform.parent = m_CurrentWeapon.transform;
		m_WeaponScript.m_ModelThirdPersonPlayer.transform.parent = m_CurrentWeapon.transform;
		m_WeaponScript.m_ModelThirdPersonEnemy.transform.parent = m_CurrentWeapon.transform;
		m_WeaponScript = null;
		UnityEngine.Object.Destroy(m_CurrentWeapon);
		m_CurrentWeapon = null;
	}

	private void AttachThirdPersonWeapon()
	{
		m_WeaponScript.m_ModelThirdPersonPlayer.transform.parent = m_WeaponAttachPointThirdPerson.transform;
		m_WeaponScript.m_ModelThirdPersonPlayer.transform.localPosition = Vector3.zero;
		m_WeaponScript.m_ModelThirdPersonPlayer.transform.localRotation = Quaternion.identity;
	}

	public void SetCoverState(CoverState newstate)
	{
		if (m_CoverState == newstate)
		{
			return;
		}
		m_CoverState = newstate;
		switch (m_CoverState)
		{
		case CoverState.Outside:
			break;
		case CoverState.TransitioningIn:
			PlayCoverAnimation(CoverAnimation.Enter, 0f);
			break;
		case CoverState.Inside:
			m_CoverFireSide = CoverFireSide.None;
			PlayCoverAnimation(CoverAnimation.Idle, 0f);
			break;
		case CoverState.SwitchingSides:
			switch (m_CoverSide)
			{
			case CoverSide.Left:
				PlayCoverAnimation(CoverAnimation.TransitionLeftToRight, 0f);
				SetCoverSide(CoverSide.Right);
				break;
			case CoverSide.Right:
				PlayCoverAnimation(CoverAnimation.TransitionRightToLeft, 0f);
				SetCoverSide(CoverSide.Left);
				break;
			}
			break;
		case CoverState.TransitioningToFire:
			switch (m_CoverEdge)
			{
			case CoverEdge.Left:
				m_CoverFireSide = CoverFireSide.Left;
				break;
			case CoverEdge.Right:
				m_CoverFireSide = CoverFireSide.Right;
				break;
			case CoverEdge.Both:
				switch (m_CoverSide)
				{
				case CoverSide.Left:
					m_CoverFireSide = CoverFireSide.Left;
					break;
				case CoverSide.Right:
					m_CoverFireSide = CoverFireSide.Right;
					break;
				}
				break;
			case CoverEdge.None:
				if (m_AllowUpAndOverCoverFire)
				{
					m_CoverFireSide = CoverFireSide.UpOver;
				}
				else
				{
					SetCoverState(CoverState.Inside);
				}
				break;
			}
			if (m_CoverFireSide == CoverFireSide.None)
			{
				break;
			}
			switch (m_CoverFireSide)
			{
			case CoverFireSide.Left:
			case CoverFireSide.Right:
				PlayCoverAnimation(CoverAnimation.TransitionToLeanFire, 0f);
				break;
			case CoverFireSide.UpOver:
				PlayCoverAnimation(CoverAnimation.TransitionToUpOverFire, 0f);
				break;
			}
			EventManager.Instance.PostEvent("Weapon_Handle", EventAction.PlaySound, null, base.gameObject);
			m_ModelThirdPerson.transform.parent = Globals.m_CameraController.m_ThirdPersonAttach.transform;
			m_ModelRotateCoverTimer = m_ModelRotateCoverTime;
			m_OriginalCoverRotation = m_ModelThirdPerson.transform.localRotation;
			m_OriginalCoverPosition = m_ModelThirdPerson.transform.localPosition;
			switch (m_CoverFireSide)
			{
			case CoverFireSide.Left:
				if (m_Stance == Stance.Crouch)
				{
					m_TargetCoverRotation = Quaternion.Euler(m_CrouchModelLeftRotation);
					m_TargetCoverPosition = m_CrouchModelLeftPosition;
				}
				else
				{
					m_TargetCoverRotation = Quaternion.Euler(m_StandModelLeftRotation);
					m_TargetCoverPosition = m_StandModelLeftPosition;
				}
				break;
			case CoverFireSide.Right:
				if (m_Stance == Stance.Crouch)
				{
					m_TargetCoverRotation = Quaternion.Euler(m_CrouchModelRightRotation);
					m_TargetCoverPosition = m_CrouchModelRightPosition;
				}
				else
				{
					m_TargetCoverRotation = Quaternion.Euler(m_StandModelRightRotation);
					m_TargetCoverPosition = m_StandModelRightPosition;
				}
				break;
			case CoverFireSide.UpOver:
				switch (m_CoverSide)
				{
				case CoverSide.Left:
					m_TargetCoverRotation = Quaternion.Euler(m_UpOverModelLeftRotation);
					m_TargetCoverPosition = m_UpOverModelLeftPosition;
					break;
				case CoverSide.Right:
					m_TargetCoverRotation = Quaternion.Euler(m_UpOverModelRightRotation);
					m_TargetCoverPosition = m_UpOverModelRightPosition;
					break;
				}
				break;
			}
			break;
		case CoverState.Firing:
			switch (m_CoverFireSide)
			{
			case CoverFireSide.Left:
			case CoverFireSide.Right:
				PlayCoverAnimation(CoverAnimation.LeanFiring, 0f);
				break;
			case CoverFireSide.UpOver:
				PlayCoverAnimation(CoverAnimation.UpOverFiring, 0f);
				break;
			}
			break;
		case CoverState.TransitioningFromFire:
			switch (m_CoverFireSide)
			{
			case CoverFireSide.Left:
			case CoverFireSide.Right:
				PlayCoverAnimation(CoverAnimation.TransitionFromLeanFire, 0f);
				break;
			case CoverFireSide.UpOver:
				PlayCoverAnimation(CoverAnimation.TransitionFromUpOverFire, 0f);
				Globals.m_CameraController.m_ThirdPersonAttach.transform.localPosition = Globals.m_CameraController.m_ThirdPersonAttachOriginalPos;
				break;
			default:
				PlayCoverAnimation(CoverAnimation.TransitionFromLeanFire, 0f);
				break;
			}
			m_ModelThirdPerson.transform.parent = base.transform;
			m_ModelRotateCoverTimer = m_ModelRotateCoverTime;
			m_OriginalCoverRotation = m_ModelThirdPerson.transform.localRotation;
			m_OriginalCoverPosition = m_ModelThirdPerson.transform.localPosition;
			m_TargetCoverRotation = Quaternion.identity;
			m_TargetCoverPosition = Vector3.zero;
			break;
		case CoverState.Reloading:
		{
			float coverAnimationLength = GetCoverAnimationLength(CoverAnimation.Reload);
			float num3 = 0f;
			if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Reloading)
			{
				m_WeaponScript.m_ReloadTime = coverAnimationLength;
				num3 = m_WeaponScript.m_ReloadTime - m_WeaponScript.m_ReloadTimer;
				if (num3 < 0f)
				{
					num3 = 0f;
				}
				m_ModelThirdPerson.transform.parent = base.transform;
				m_ModelRotateCoverTimer = m_ModelRotateCoverTime;
				m_OriginalCoverRotation = m_ModelThirdPerson.transform.localRotation;
				m_OriginalCoverPosition = m_ModelThirdPerson.transform.localPosition;
				m_TargetCoverRotation = Quaternion.identity;
				m_TargetCoverPosition = Vector3.zero;
			}
			PlayCoverAnimation(CoverAnimation.Reload, num3);
			if (m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Reloading)
			{
				m_WeaponScript.Reload(coverAnimationLength);
			}
			m_ReloadNextCoverIdle = false;
			break;
		}
		case CoverState.CoverAiming:
			m_ModelThirdPerson.animation.Stop();
			m_MovementScript.CancelMovement();
			m_ModelThirdPerson.transform.parent = Globals.m_CameraController.m_ThirdPersonAttach.transform;
			switch (m_CoverSide)
			{
			case CoverSide.Left:
				m_CoverAimWeight = 0f;
				break;
			case CoverSide.Right:
				m_CoverAimWeight = 1f;
				break;
			}
			if (m_WantFire && m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Firing)
			{
				StartFiring();
			}
			break;
		case CoverState.Stand:
			PlayCoverAnimation(CoverAnimation.Stand, 0f);
			break;
		case CoverState.Crouch:
			PlayCoverAnimation(CoverAnimation.Crouch, 0f);
			break;
		case CoverState.Holstering:
			PlayCoverAnimation(CoverAnimation.Holster, 0f);
			break;
		case CoverState.Drawing:
			PlayCoverAnimation(CoverAnimation.Draw, 0f);
			break;
		case CoverState.CoverFlip:
			PlayCoverAnimation(CoverAnimation.CoverFlip, 0f);
			break;
		case CoverState.CoverDive:
			PlayCoverAnimation(CoverAnimation.CoverDive, 0f);
			break;
		case CoverState.CoverSlideInner:
		{
			m_CoverSlideEndpoint = base.transform.position;
			Vector3 direction = base.transform.right;
			if (m_CoverSide == CoverSide.Left)
			{
				direction = -base.transform.right;
			}
			Ray ray = new Ray(m_CoverSlideEndpoint, direction);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 2f, 256))
			{
				m_CoverSlideEndpoint = hitInfo.point + hitInfo.normal * m_CoverDistFromWall;
				m_CoverSlideEndpoint.y = base.transform.position.y;
				UpdateCoverInternalInfo(hitInfo.point, hitInfo.normal, hitInfo.collider);
			}
			else
			{
				SetCoverState(CoverState.Inside);
				Debug.LogError("Can't find the cover on the wall next to you, this should never happen!");
			}
			m_CoverSlideStartpoint = base.transform.position;
			m_CoverSlideTime = PlayCoverAnimation(CoverAnimation.CoverSlideInner, 0f);
			m_CoverSlideTimer = m_CoverSlideTime;
			break;
		}
		case CoverState.CoverSlideOuter:
		{
			float num = 1f;
			if (m_CoverSide == CoverSide.Left)
			{
				num = -1f;
			}
			m_CoverSlideMidpoint = base.transform.position + base.transform.right * num * (m_MovementScript.m_CoverEdgeCollisionDist + m_CoverDistFromWall);
			m_CoverSlideMidpoint.y = base.transform.position.y;
			m_CoverSlideEndpoint = m_CoverSlideMidpoint + base.transform.forward * (m_MovementScript.m_CoverEdgeCollisionDist + m_CoverDistFromWall);
			m_CoverNormal = base.transform.right;
			if (m_CoverSide == CoverSide.Left)
			{
				m_CoverNormal = -base.transform.right;
			}
			Ray ray2 = new Ray(m_CoverSlideEndpoint + new Vector3(0f, 0.1f, 0f), -m_CoverNormal);
			RaycastHit hitInfo2;
			if (Physics.Raycast(ray2, out hitInfo2, 2f, 256))
			{
				m_CoverSlideEndpoint = hitInfo2.point + hitInfo2.normal * m_CoverDistFromWall;
				m_CoverSlideEndpoint.y = GetGroundHeight(m_CoverSlideEndpoint) + 0.1f;
				UpdateCoverInternalInfo(hitInfo2.point, hitInfo2.normal, hitInfo2.collider);
			}
			else
			{
				SetCoverState(CoverState.Inside);
				Debug.LogError("Can't find the cover on the other corner, this should never happen!");
			}
			m_CoverSlideStartpoint = base.transform.position;
			m_CoverSlideTime = PlayCoverAnimation(CoverAnimation.CoverSlideOuter, 0f) / 2f;
			m_CoverSlideFirstStage = true;
			m_CoverSlideTimer = m_CoverSlideTime;
			Quaternion rotation = base.transform.rotation;
			float y = -30f;
			if (m_CoverSide == CoverSide.Left)
			{
				y = 30f;
			}
			rotation *= Quaternion.Euler(0f, y, 0f);
			Quaternion quaternion = Quaternion.Euler(0f, Globals.m_CameraController.GetYaw(), 0f);
			m_CoverSlideRotationAmount = Quaternion.Angle(rotation, quaternion);
			float num2 = ((!(Vector3.Cross(rotation * Vector3.forward, quaternion * Vector3.forward).y < 0f)) ? 1 : (-1));
			if (m_CoverSide == CoverSide.Left)
			{
				num2 = 0f - num2;
			}
			m_CoverSlideRotationAmount = (0f - m_CoverSlideRotationAmount) * num2;
			break;
		}
		case CoverState.ThrowGrenade:
		{
			float starttime = 0f;
			if (m_GrenadeState == GrenadeState.Throwing)
			{
				starttime = m_GrenadeThrowTime - m_GrenadeThrowTimer;
				m_ModelThirdPerson.transform.parent = base.transform;
				m_ModelRotateCoverTimer = m_ModelRotateCoverTime;
				m_OriginalCoverRotation = m_ModelThirdPerson.transform.localRotation;
				m_OriginalCoverPosition = m_ModelThirdPerson.transform.localPosition;
				m_TargetCoverRotation = Quaternion.identity;
				m_TargetCoverPosition = Vector3.zero;
			}
			if (m_CoverEdge != 0)
			{
				PlayCoverAnimation(CoverAnimation.ThrowGrenade, starttime);
			}
			else if (m_AllowUpAndOverCoverFire)
			{
				PlayCoverAnimation(CoverAnimation.ThrowGrenadeUpOver, starttime);
			}
			else
			{
				SetCoverState(CoverState.Inside);
			}
			break;
		}
		}
	}

	private float PlayCoverAnimation(CoverAnimation anim, float starttime = 0f)
	{
		CoverAnimationData coverAnimationData = ((m_CoverSide != 0) ? m_CoverAnimationsRight[(int)m_Stance, (int)m_WeaponScript.m_WeaponAnimSet, (int)anim] : m_CoverAnimationsLeft[(int)m_Stance, (int)m_WeaponScript.m_WeaponAnimSet, (int)anim]);
		if (coverAnimationData.loop)
		{
			m_ModelThirdPerson.animation[coverAnimationData.name].wrapMode = WrapMode.Loop;
		}
		else
		{
			m_ModelThirdPerson.animation[coverAnimationData.name].wrapMode = WrapMode.Once;
		}
		if (anim != CoverAnimation.Reload)
		{
			m_ModelThirdPerson.animation.CrossFade(coverAnimationData.name, coverAnimationData.blendTime);
		}
		else
		{
			m_ModelThirdPerson.animation.Play(coverAnimationData.name);
		}
		m_CurrentCoverAnimationState = m_ModelThirdPerson.animation[coverAnimationData.name];
		if (starttime != 0f)
		{
			m_ModelThirdPerson.animation[coverAnimationData.name].time = starttime;
		}
		if (coverAnimationData.weaponAnimName != string.Empty)
		{
			if (coverAnimationData.loop)
			{
				m_WeaponScript.m_ModelThirdPersonPlayer.animation[coverAnimationData.weaponAnimName].wrapMode = WrapMode.Loop;
			}
			else
			{
				m_WeaponScript.m_ModelThirdPersonPlayer.animation[coverAnimationData.weaponAnimName].wrapMode = WrapMode.Once;
			}
			m_WeaponScript.m_ModelThirdPersonPlayer.animation.Play(coverAnimationData.weaponAnimName);
			m_WeaponScript.m_ModelThirdPersonPlayer.animation[coverAnimationData.weaponAnimName].time = starttime;
		}
		if (coverAnimationData.soundEvents != null)
		{
			for (int num = coverAnimationData.soundEvents.Count - 1; num >= 0; num--)
			{
				CoverAnimationSoundEvent coverAnimationSoundEvent = coverAnimationData.soundEvents[num];
				if (coverAnimationSoundEvent.delay == 0f)
				{
					EventManager.Instance.PostEvent(coverAnimationSoundEvent.name, EventAction.PlaySound, null, base.gameObject);
				}
				else
				{
					coverAnimationSoundEvent.delayTimer = coverAnimationSoundEvent.delay;
					m_PendingSoundEvents.Add(coverAnimationSoundEvent);
				}
			}
		}
		return m_ModelThirdPerson.animation[coverAnimationData.name].length;
	}

	private float GetCoverAnimationLength(CoverAnimation anim)
	{
		CoverAnimationData coverAnimationData = ((m_CoverSide != 0) ? m_CoverAnimationsRight[(int)m_Stance, (int)m_WeaponScript.m_WeaponAnimSet, (int)anim] : m_CoverAnimationsLeft[(int)m_Stance, (int)m_WeaponScript.m_WeaponAnimSet, (int)anim]);
		return m_ModelThirdPerson.animation[coverAnimationData.name].length;
	}

	private void SetCoverSide(CoverSide newside)
	{
		if (newside != m_CoverSide)
		{
			m_CoverSide = newside;
		}
	}

	public void ScreenPress(POINTER_INFO ptr)
	{
		if (ptr.devicePos.x > 492f && ptr.devicePos.x < 532f && ptr.devicePos.y > Globals.m_FullScreenRect.height - 40f)
		{
			if (QualitySettings.antiAliasing == 0)
			{
				QualitySettings.antiAliasing = 2;
			}
			else
			{
				QualitySettings.antiAliasing = 0;
			}
			Debug.Log("Anti Aliasing: " + QualitySettings.antiAliasing);
		}
		else
		{
			if (m_CurrentHealth <= 0)
			{
				return;
			}
			if (CharacterBase.m_OffscreenTargetX != -1 && ptr.devicePos.x >= (float)CharacterBase.m_OffscreenTargetX && ptr.devicePos.x < (float)(CharacterBase.m_OffscreenTargetX + CharacterBase.m_OffscreenTargetWidth) && ptr.devicePos.y <= (float)CharacterBase.m_OffscreenTargetY && ptr.devicePos.y > (float)(CharacterBase.m_OffscreenTargetY - CharacterBase.m_OffscreenTargetHeight))
			{
				Globals.m_CameraController.LookAtOverTime(m_TargetedEnemy, 8f);
				return;
			}
			m_PossibleEnemyTarget = null;
			Ray ray = m_CurrentCamera.ScreenPointToRay(ptr.devicePos);
			RaycastHit hitInfo;
			bool flag = Physics.Raycast(ray, out hitInfo, 50f, 66305);
			bool flag2 = false;
			if (!flag || hitInfo.collider.gameObject.layer != 16)
			{
				RaycastHit hitInfo2;
				flag2 = Physics.Raycast(ray, out hitInfo2, 50f, 131072);
				if (!flag2 && flag && hitInfo.collider.gameObject.layer == 9)
				{
					flag2 = true;
					hitInfo2 = hitInfo;
				}
				if (flag2)
				{
					if (hitInfo2.distance - hitInfo.distance > 2f)
					{
						flag2 = false;
					}
					if (flag2)
					{
						m_PossibleEnemyTarget = hitInfo2.collider.transform.parent.gameObject;
						if (m_PossibleEnemyTarget.GetComponent<CharacterBase>() == null)
						{
							m_PossibleEnemyTarget = hitInfo2.collider.transform.parent.parent.gameObject;
						}
						if (m_PossibleEnemyTarget.GetComponent<CharacterBase>() == null)
						{
							m_PossibleEnemyTarget = hitInfo2.collider.gameObject;
						}
						m_EnemyTapID = ptr.id;
					}
				}
			}
			m_MovementScript.ButtonPress(ptr.devicePos, ptr.id);
			Globals.m_CameraController.ButtonPress(ptr.devicePos, ptr.id);
		}
	}

	public void ScreenRelease(POINTER_INFO ptr)
	{
		InteractiveObjectManager.m_PopupPressed = null;
		if (m_CurrentHealth <= 0)
		{
			return;
		}
		Ray ray = m_CurrentCamera.ScreenPointToRay(ptr.devicePos);
		RaycastHit hitInfo;
		bool flag = Physics.Raycast(ray, out hitInfo, 50f, 2163457);
		m_TappedInteractiveObject = false;
		if (flag)
		{
			if (hitInfo.collider.gameObject.layer == 21)
			{
				NPC_Base nPC_Base = Globals.FindCharacterBase(hitInfo.collider.transform) as NPC_Base;
				if ((bool)nPC_Base)
				{
					nPC_Base.Interact();
				}
			}
			else if (hitInfo.collider.gameObject.layer == 16)
			{
				InteractiveObject_Base component = hitInfo.collider.gameObject.GetComponent<InteractiveObject_Base>();
				if ((bool)component)
				{
					m_TappedInteractiveObject = component.InteractWithObject();
				}
				else
				{
					Debug.Log(hitInfo.collider.gameObject.name + " has a layer of Interactive Object but doesn't have an InteractiveObject component.");
				}
			}
		}
		m_MovementScript.ButtonRelease(ptr.devicePos, ptr.id);
		m_TappedInteractiveObject = false;
		Globals.m_CameraController.ButtonRelease(ptr.devicePos, ptr.id);
		if (!(m_PossibleEnemyTarget != null) || !(m_PossibleEnemyTarget != null) || ptr.id != m_EnemyTapID)
		{
			return;
		}
		if (m_TapTime[m_EnemyTapID] <= Globals.m_TapTimeLimit)
		{
			if (m_TargetedEnemy != m_PossibleEnemyTarget)
			{
				m_TargetedEnemy = m_PossibleEnemyTarget;
				m_TargetedEnemyScript = m_TargetedEnemy.GetComponent<CharacterBase>();
				EventManager.Instance.PostEvent("HUD_Target_Lock", EventAction.PlaySound, null, base.gameObject);
			}
			else
			{
				m_TargetedEnemy = null;
				EventManager.Instance.PostEvent("HUD_Target_Unlock", EventAction.PlaySound, null, base.gameObject);
			}
		}
		m_EnemyTapID = -1;
	}

	public void ToggleCloaking()
	{
		if (!Globals.m_AugmentCloaking.enabled)
		{
			Globals.m_AugmentCloaking.Enable();
		}
		else
		{
			Globals.m_AugmentCloaking.Disable();
		}
		if (Debug.isDebugBuild)
		{
			ListLoadedTextures();
		}
	}

	public void UseItem(ItemType item)
	{
		switch (item)
		{
		case ItemType.EnergyBar:
			if (m_CurrentEnergy < m_MaxEnergy)
			{
				EventManager.Instance.PostEvent("PU_EnergyBar", EventAction.PlaySound, null, Globals.m_PlayerController.gameObject);
				m_CurrentEnergy += 1f;
				if (m_CurrentEnergy > m_MaxEnergy)
				{
					m_CurrentEnergy = m_MaxEnergy;
				}
				if (m_EnergyRegenMode != EnergyRegenMode.NotRegenerating && m_CurrentEnergy > m_EnergyRegenTarget)
				{
					m_EnergyRegenTarget += 1f;
					if (m_EnergyRegenTarget > m_MaxEnergy)
					{
						EnergyRegenComplete();
					}
				}
			}
			else
			{
				EventManager.Instance.PostEvent("UI_Error", EventAction.PlaySound, null, base.gameObject);
			}
			break;
		case ItemType.Booze:
			m_CurrentHealth = m_MaxHealth;
			EventManager.Instance.PostEvent("PU_Alcohol", EventAction.PlaySound, null, Globals.m_PlayerController.gameObject);
			break;
		}
	}

	public void GrenadeButtonTapped()
	{
		switch (m_CurrentGrenadeType)
		{
		case 0:
			m_LastGrenadeObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_FragGrenadePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			break;
		case 1:
			m_LastGrenadeObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_EMPGrenadePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			break;
		case 2:
			m_LastGrenadeObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_ConcussionGrenadePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			break;
		}
		m_LastGrenadeScript = m_LastGrenadeObject.GetComponent<GrenadeFrag>();
		if (m_CameraMode == CameraMode.First)
		{
			m_GrenadeState = GrenadeState.Holstering;
			m_GrenadeThrowTime = m_WeaponScript.GetFirstPersonAnimLength(WeaponBase.FirstPersonAnimation.ThrowGrenade);
			m_GrenadeThrowTimer = m_GrenadeThrowTime;
			m_GrenadeReleaseTime = m_GrenadeThrowTime - 0.2f;
			m_LastGrenadeObject.transform.parent = m_GrenadeAttachPointFirstPerson.transform;
			m_LastGrenadeObject.transform.localPosition = Vector3.zero;
			m_LastGrenadeObject.transform.localRotation = Quaternion.identity;
			m_LastGrenadeObject.SetActiveRecursively(false);
			m_LastGrenadeObject.layer = 15;
			EventManager.Instance.PostEvent("VO_PC_Throw", EventAction.PlaySound, null, base.gameObject);
			return;
		}
		if (m_CoverState != CoverState.CoverAiming)
		{
			SetCoverState(CoverState.ThrowGrenade);
		}
		m_GrenadeThrowTime = m_ModelThirdPerson.animation[m_CoverAnimationsLeft[1, (int)m_WeaponScript.m_WeaponAnimSet, 16].name].length;
		m_GrenadeThrowTimer = m_GrenadeThrowTime;
		m_GrenadeReleaseTime = m_GrenadeThrowTime - 0.82f;
		if (m_CoverState == CoverState.CoverAiming)
		{
			m_GrenadeReleaseTime = m_GrenadeThrowTime - 0.7f;
		}
		if (m_CoverSide == CoverSide.Left)
		{
			m_LastGrenadeObject.transform.parent = m_GrenadeAttachPointThirdPersonLeft.transform;
		}
		else
		{
			m_LastGrenadeObject.transform.parent = m_GrenadeAttachPointThirdPersonRight.transform;
		}
		m_LastGrenadeObject.transform.localPosition = Vector3.zero;
		m_LastGrenadeObject.transform.localRotation = Quaternion.identity;
		m_GrenadeState = GrenadeState.Throwing;
		CancelMovement();
		EventManager.Instance.PostEvent("VO_PC_Throw", EventAction.PlaySound, null, base.gameObject);
	}

	public void StanceButtonTapped()
	{
		if (m_CurrentHealth <= 0 || (m_CoverState != 0 && m_CoverState != CoverState.Inside && m_CoverState != CoverState.CoverAiming) || (m_CameraMode == CameraMode.Third && m_Stance == Stance.Crouch && m_AllowUpAndOverCoverFire))
		{
			return;
		}
		if (m_Stance == Stance.Stand)
		{
			if (m_CameraMode == CameraMode.Third && m_CoverState != CoverState.CoverAiming)
			{
				SetCoverState(CoverState.Crouch);
			}
			SetStance(Stance.Crouch);
		}
		else
		{
			if (m_CameraMode == CameraMode.Third && m_CoverState != CoverState.CoverAiming)
			{
				SetCoverState(CoverState.Stand);
			}
			SetStance(Stance.Stand);
		}
	}

	public void CoverFlipButtonTapped()
	{
		if (m_CurrentHealth > 0)
		{
			Vector3 vector = base.transform.right;
			if (m_CoverSide == CoverSide.Left)
			{
				vector = -vector;
			}
			m_CoverFlipPlane = new Plane(-vector, m_CoverFlipVolume.transform.position);
			m_CoverFlipDistance = m_CoverFlipPlane.GetDistanceToPoint(base.transform.position) * 2f;
			float num = 0f;
			if (m_CoverFlipDistance >= m_CoverFlipDiveRange)
			{
				num = GetCoverAnimationLength(CoverAnimation.CoverDive);
				float num2 = m_CoverFlipDistance / (num * m_CoverDiveMoveStop);
				m_CoverFlipVector = m_CoverFlipVolume.m_Direction * num2;
				SetCoverState(CoverState.CoverDive);
			}
			else
			{
				num = GetCoverAnimationLength(CoverAnimation.CoverFlip);
				float num3 = m_CoverFlipDistance / num;
				m_CoverFlipVector = m_CoverFlipVolume.m_Direction * num3;
				SetCoverState(CoverState.CoverFlip);
			}
			float num4 = Vector3.Dot(m_CoverFlipVolume.m_Direction, vector);
			if (num4 < 0f)
			{
				m_CoverFlipVector *= -1f;
			}
			m_CoverFlipDestination = base.transform.position + m_CoverFlipVolume.m_Direction * m_CoverFlipDistance;
			if (num4 < 0f)
			{
				m_CoverFlipDestination = base.transform.position - m_CoverFlipVolume.m_Direction * m_CoverFlipDistance;
			}
			m_CoverFlipTimer = 0f;
			m_CoverFlipTime = num;
		}
	}

	public void CoverInnerFlipButtonTapped()
	{
		SetCoverState(CoverState.CoverSlideInner);
	}

	public void CoverOuterFlipButtonTapped()
	{
		SetCoverState(CoverState.CoverSlideOuter);
	}

	private void SetStance(Stance stance)
	{
		m_Stance = stance;
		switch (m_Stance)
		{
		case Stance.Stand:
			m_MovementScript.m_PlayerCharacterController.height = m_ColliderHeightStanding;
			break;
		case Stance.Crouch:
			m_MovementScript.m_PlayerCharacterController.height = m_ColliderHeightCrouching;
			break;
		}
		m_MovementScript.m_PlayerCharacterController.center = new Vector3(0f, m_MovementScript.m_PlayerCharacterController.height * 0.5f, 0f);
		if (m_Stance == Stance.Crouch)
		{
			EventManager.Instance.PostEvent("Footsteps_Stealth", EventAction.SetVolume, 0.3f, base.gameObject);
		}
		else
		{
			EventManager.Instance.PostEvent("Footsteps_Stealth", EventAction.SetVolume, 0.5f, base.gameObject);
		}
	}

	public void ReloadButtonTapped()
	{
		if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Holstered)
		{
			ToggleWeaponHolstered();
		}
		else if (m_CurrentHealth > 0)
		{
			if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Firing)
			{
				m_WeaponScript.EndFire();
			}
			else
			{
				m_WeaponScript.CancelFire();
			}
			if (m_WeaponScript.m_CurrentAmmo < m_WeaponScript.m_AmmoPerClip && m_WeaponScript.m_Ammo > 0)
			{
				ReloadWeapon();
			}
		}
	}

	public void CoverButtonTapped()
	{
		if (m_CurrentHealth > 0)
		{
			if (m_CoverState == CoverState.Outside)
			{
				EnterCover(m_AutoCoverPosition, m_AutoCoverNormal, m_AutoCoverCollider);
			}
			else if (CanExitCover())
			{
				ExitCover();
			}
		}
	}

	public void ArmorButtonTapped()
	{
		if (m_CurrentHealth > 0 && m_CurrentEnergy >= 2f && m_CurrentArmor < 100)
		{
			UseEnergy(2f);
			m_CurrentArmor = 100;
		}
	}

	private void MouseInputUpdate()
	{
		if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
		{
			GeneralInputPress(Input.mousePosition, 11);
		}
		if (Input.GetButton("Fire1") || Input.GetButton("Fire2"))
		{
			GeneralInputHeld(Input.mousePosition, 11);
		}
		if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2"))
		{
			GeneralInputRelease(Input.mousePosition, 11);
		}
	}

	private void ListLoadedTextures()
	{
		Resources.UnloadUnusedAssets();
		Texture[] array = Resources.FindObjectsOfTypeAll(typeof(Texture)) as Texture[];
		string empty = string.Empty;
		empty += "Dumping loaded textures:\n";
		int num = 0;
		int num2 = 1;
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i].name == string.Empty))
			{
				string text = empty;
				empty = text + num2 + " " + array[i].name + " (" + array[i].width + ", " + array[i].height + ")\n";
				num++;
				num2++;
				if (num >= 500)
				{
					Debug.Log(empty);
					empty = string.Empty;
					num = 0;
				}
			}
		}
		if (empty.Length > 0)
		{
			Debug.Log(empty);
		}
	}

	private void ListLoadedSounds()
	{
		Resources.UnloadUnusedAssets();
		AudioClip[] array = Resources.FindObjectsOfTypeAll(typeof(AudioClip)) as AudioClip[];
		string empty = string.Empty;
		empty += "Dumping loaded sounds:\n";
		int num = 0;
		int num2 = 1;
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i].name == string.Empty))
			{
				string text = empty;
				empty = text + num2 + " " + array[i].name + "\n";
				num++;
				num2++;
				if (num >= 500)
				{
					Debug.Log(empty);
					empty = string.Empty;
					num = 0;
				}
			}
		}
		if (empty.Length > 0)
		{
			Debug.Log(empty);
		}
	}

	private void TouchInputUpdate()
	{
		for (int i = 0; Input.touchCount > i; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began)
			{
				GeneralInputPress(touch.position, touch.fingerId);
			}
			if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
			{
				GeneralInputHeld(touch.position, touch.fingerId);
			}
			if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				GeneralInputRelease(touch.position, touch.fingerId);
			}
		}
	}

	private void GeneralInputPress(Vector2 position, int id)
	{
		m_TapTime[id] = 0f;
	}

	private void GeneralInputHeld(Vector2 position, int id)
	{
		m_MovementScript.ButtonHeld(position, id);
		Globals.m_CameraController.ButtonHeld(position, id);
		m_TapTime[id] += Time.deltaTime;
	}

	private void GeneralInputRelease(Vector2 position, int id)
	{
		UserStopFiring(position, id);
		Globals.m_HUD.UserReleased();
		Globals.m_CameraController.ButtonRelease(position, id);
		m_MovementScript.ButtonRelease(position, id);
	}

	public void FireButtonPress(POINTER_INFO ptr)
	{
		if (m_CurrentHealth > 0)
		{
			UserStartFiring(ptr.devicePos, ptr.id);
		}
	}

	public void FireButtonRelease(POINTER_INFO ptr)
	{
		if (m_CurrentHealth > 0)
		{
		}
	}

	private void UserStartFiring(Vector2 devicePos, int id)
	{
		if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Holstered)
		{
			ToggleWeaponHolstered();
			return;
		}
		m_WantFire = true;
		m_FireTapID = id;
		StartFiring();
		Globals.m_CameraController.ButtonPress(devicePos, id);
		if (m_TargetedEnemy != null)
		{
			Globals.m_CameraController.LookAtOverTime(m_TargetedEnemy, 8f);
		}
	}

	private void UserStopFiring(Vector2 devicePos, int id)
	{
		if (id == m_FireTapID)
		{
			m_WantFire = false;
			if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Firing || m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Aiming)
			{
				StopFiringAfterOneShot();
			}
			else if (m_CoverState == CoverState.TransitioningToFire)
			{
				m_OneCoverShot = true;
			}
			m_FireTapID = -1;
		}
	}

	public float GetCameraHeight()
	{
		switch (m_Stance)
		{
		case Stance.Stand:
			if (m_CameraMode == CameraMode.First)
			{
				return m_CameraHeightStanding;
			}
			return Globals.m_CameraController.m_CameraHeightThirdPersonStanding;
		case Stance.Crouch:
			if (m_CameraMode == CameraMode.First)
			{
				return m_CameraHeightCrouching;
			}
			return Globals.m_CameraController.m_CameraHeightThirdPerson;
		default:
			return 0f;
		}
	}

	public float GetRunSpeed()
	{
		if (m_CoverState != 0)
		{
			switch (m_Stance)
			{
			case Stance.Stand:
				return m_CoverMoveSpeed;
			case Stance.Crouch:
				return m_CoverMoveSpeed;
			}
		}
		else
		{
			switch (m_Stance)
			{
			case Stance.Stand:
				return m_RunSpeed;
			case Stance.Crouch:
				return m_RunSpeed * 0.5f;
			}
		}
		return 0f;
	}

	public float GetStrafeSpeed()
	{
		switch (m_Stance)
		{
		case Stance.Stand:
			return m_StrafeSpeed;
		case Stance.Crouch:
			return m_StrafeSpeed * 0.5f;
		default:
			return 0f;
		}
	}

	public float GetMinSpeed()
	{
		return m_RunSpeed * 0.2f;
	}

	private void ReloadWeapon()
	{
		if (m_CameraMode == CameraMode.First)
		{
			float firstPersonAnimLength = m_WeaponScript.GetFirstPersonAnimLength(WeaponBase.FirstPersonAnimation.Reload);
			m_WeaponScript.Reload(firstPersonAnimLength);
			return;
		}
		switch (m_CoverState)
		{
		case CoverState.Inside:
			SetCoverState(CoverState.Reloading);
			break;
		case CoverState.TransitioningFromFire:
		case CoverState.Firing:
			if (m_CoverState == CoverState.Firing)
			{
				StopFiring();
			}
			m_ReloadNextCoverIdle = true;
			break;
		case CoverState.CoverAiming:
		{
			float num = 0f;
			num = ((m_CoverFireSide != CoverFireSide.UpOver) ? ((m_CoverSide != 0) ? m_ModelThirdPerson.animation[m_CoverAnimationsRight[(int)m_Stance, (int)m_WeaponScript.m_WeaponAnimSet, 15].name].length : m_ModelThirdPerson.animation[m_CoverAnimationsLeft[(int)m_Stance, (int)m_WeaponScript.m_WeaponAnimSet, 15].name].length) : ((m_CoverSide != 0) ? m_ModelThirdPerson.animation[m_CoverAnimationsRight[(int)m_Stance, (int)m_WeaponScript.m_WeaponAnimSet, 19].name].length : m_ModelThirdPerson.animation[m_CoverAnimationsLeft[(int)m_Stance, (int)m_WeaponScript.m_WeaponAnimSet, 19].name].length));
			m_WeaponScript.Reload(num);
			break;
		}
		case CoverState.SwitchingSides:
		case CoverState.TransitioningToFire:
			break;
		}
	}

	private void StartFiring()
	{
		if (m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Reloading)
		{
			bool flag = false;
			if (m_CameraMode == CameraMode.First || m_CoverState == CoverState.CoverAiming)
			{
				flag = true;
			}
			if (m_CameraMode == CameraMode.Third && m_CameraCoverDot < m_CameraCoverAimLimitStanding && m_CoverEdge == CoverEdge.None && !m_AllowUpAndOverCoverFire)
			{
				flag = true;
			}
			if (flag)
			{
				m_WeaponScript.StartFire();
			}
			else if (m_CoverState == CoverState.Inside)
			{
				SetCoverState(CoverState.TransitioningToFire);
			}
		}
	}

	private void StopFiring()
	{
		m_WeaponScript.EndFire();
	}

	private void StopFiringAfterOneShot()
	{
		StartCoroutine(EndFireOneShot());
	}

	private IEnumerator EndFireOneShot()
	{
		float waittime = m_WeaponScript.m_OneBulletFireTime;
		if (m_CameraMode == CameraMode.First)
		{
			waittime -= m_TapTime[m_FireTapID];
		}
		yield return new WaitForSeconds(waittime);
		StopFiring();
		m_OneCoverShot = false;
	}

	private IEnumerator CheckForNearbyCover()
	{
		while (true)
		{
			bool m_NearbyCover = false;
			bool entericon = true;
			if (CanEnterCover() || CanExitCover())
			{
				if (m_CoverState == CoverState.Outside && !m_ForceCoverButtonInactive)
				{
					Ray coverRay = new Ray(base.transform.position + new Vector3(0f, 0.1f, 0f), base.transform.forward);
					RaycastHit rayHit;
					if (Physics.Raycast(coverRay, out rayHit, m_CoverTestDistance, 256) && rayHit.normal.y == 0f)
					{
						m_NearbyCover = true;
						m_AutoCoverPosition = rayHit.point;
						m_AutoCoverNormal = rayHit.normal;
						m_AutoCoverCollider = rayHit.collider;
					}
					coverRay.direction = base.transform.right + base.transform.forward;
					if (!m_NearbyCover && Physics.Raycast(coverRay, out rayHit, m_CoverTestDistance, 256) && rayHit.normal.y == 0f)
					{
						m_NearbyCover = true;
						m_AutoCoverPosition = rayHit.point;
						m_AutoCoverNormal = rayHit.normal;
						m_AutoCoverCollider = rayHit.collider;
					}
					coverRay.direction = -base.transform.right + base.transform.forward;
					if (!m_NearbyCover && Physics.Raycast(coverRay, out rayHit, m_CoverTestDistance, 256) && rayHit.normal.y == 0f)
					{
						m_NearbyCover = true;
						m_AutoCoverPosition = rayHit.point;
						m_AutoCoverNormal = rayHit.normal;
						m_AutoCoverCollider = rayHit.collider;
					}
					coverRay.direction = base.transform.right;
					if (!m_NearbyCover && Physics.Raycast(coverRay, out rayHit, m_CoverTestDistance, 256) && rayHit.normal.y == 0f)
					{
						m_NearbyCover = true;
						m_AutoCoverPosition = rayHit.point;
						m_AutoCoverNormal = rayHit.normal;
						m_AutoCoverCollider = rayHit.collider;
					}
					coverRay.direction = -base.transform.right;
					if (!m_NearbyCover && Physics.Raycast(coverRay, out rayHit, m_CoverTestDistance, 256) && rayHit.normal.y == 0f)
					{
						m_NearbyCover = true;
						m_AutoCoverPosition = rayHit.point;
						m_AutoCoverNormal = rayHit.normal;
						m_AutoCoverCollider = rayHit.collider;
					}
				}
				else if (CanExitCover() && m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Firing)
				{
					m_NearbyCover = true;
					entericon = false;
				}
			}
			if (Globals.m_HUD != null)
			{
				Globals.m_HUD.TurnOnCoverButton(m_NearbyCover, entericon);
			}
			yield return new WaitForSeconds(m_CoverCheckTime);
		}
	}

	private void UpdateCoverEdge()
	{
		m_CoverEdge = CoverEdge.None;
		bool flag = false;
		RaycastHit hitInfo;
		Ray ray;
		if (m_ForceCoverEdgeFacing || m_CoverSide == CoverSide.Left)
		{
			ray = new Ray(base.transform.position - base.transform.right * m_CoverEdgeCheckDistance + new Vector3(0f, 0.1f, 0f), base.transform.forward);
			if (!Physics.Raycast(ray, out hitInfo, m_CoverDistFromWall + 0.1f, 256))
			{
				ray = new Ray(base.transform.position + new Vector3(0f, 0.1f, 0f), -base.transform.right);
				if (!Physics.Raycast(ray, out hitInfo, m_MovementScript.m_CoverWallCollisionDist + m_CoverEdgeCheckDistance, 256))
				{
					flag = true;
				}
			}
		}
		bool flag2 = false;
		if (m_ForceCoverEdgeFacing || m_CoverSide == CoverSide.Right)
		{
			ray = new Ray(base.transform.position + base.transform.right * m_CoverEdgeCheckDistance + new Vector3(0f, 0.1f, 0f), base.transform.forward);
			if (!Physics.Raycast(ray, out hitInfo, m_CoverDistFromWall + 0.1f, 256))
			{
				ray = new Ray(base.transform.position + new Vector3(0f, 0.1f, 0f), base.transform.right);
				if (!Physics.Raycast(ray, out hitInfo, m_MovementScript.m_CoverWallCollisionDist + m_CoverEdgeCheckDistance, 256))
				{
					flag2 = true;
				}
			}
		}
		if (flag && !flag2)
		{
			m_CoverEdge = CoverEdge.Left;
		}
		if (!flag && flag2)
		{
			m_CoverEdge = CoverEdge.Right;
		}
		if (flag && flag2)
		{
			m_CoverEdge = CoverEdge.Both;
		}
		m_AllowUpAndOverCoverFire = true;
		ray = new Ray(base.transform.position + base.transform.up * 1.7f, base.transform.forward);
		if (Physics.Raycast(ray, out hitInfo, m_CoverDistFromWall + 0.1f, 256))
		{
			m_AllowUpAndOverCoverFire = false;
		}
	}

	public void UpdateCover()
	{
		Ray ray = new Ray(base.transform.position + new Vector3(0f, 0.1f, 0f), base.transform.forward);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, m_CoverTestDistance, 256))
		{
			UpdateCoverInternalInfo(hitInfo.point, hitInfo.normal, hitInfo.collider);
		}
		UpdateCoverEdge();
		CoverSide coverSide = m_CoverSide;
		if (m_CoverEdge == CoverEdge.Left)
		{
			coverSide = CoverSide.Left;
		}
		if (m_CoverEdge == CoverEdge.Right)
		{
			coverSide = CoverSide.Right;
		}
		Stance stance = m_Stance;
		if (m_Stance == Stance.Stand && m_AllowUpAndOverCoverFire)
		{
			stance = Stance.Crouch;
		}
		if (coverSide != m_CoverSide && stance == m_Stance)
		{
			SetCoverState(CoverState.SwitchingSides);
		}
		if (stance != m_Stance)
		{
			SetCoverSide(coverSide);
			SetCoverState(CoverState.Crouch);
			SetStance(Stance.Crouch);
		}
	}

	private bool CanEnterCover()
	{
		if (m_CoverState == CoverState.Outside && m_GrenadeState == GrenadeState.None)
		{
			return true;
		}
		return false;
	}

	private bool CanExitCover()
	{
		if (m_CoverState == CoverState.Inside || m_CoverState == CoverState.CoverAiming || m_CoverState == CoverState.Reloading)
		{
			return true;
		}
		return false;
	}

	public void EnterCover(Vector3 position, Vector3 normal, Collider collider, bool tap = false)
	{
		if (CanEnterCover())
		{
			StopFiring();
			Vector3 vector = position;
			Ray ray = new Ray(base.transform.position + new Vector3(0f, 0.1f, 0f), -normal);
			RaycastHit hitInfo;
			vector = ((tap || !Physics.Raycast(ray, out hitInfo, 2f, 256)) ? position : hitInfo.point);
			Quaternion quaternion = Quaternion.Euler(0f, 90f, 0f);
			Vector3 vector2 = quaternion * normal;
			Ray ray2 = new Ray(vector + normal * 0.1f, vector2);
			RaycastHit hitInfo2;
			if (Physics.Raycast(ray2, out hitInfo2, m_MovementScript.m_CoverWallCollisionDist, 256))
			{
				float num = m_MovementScript.m_CoverWallCollisionDist - hitInfo2.distance;
				vector -= vector2 * num;
			}
			quaternion = Quaternion.Euler(0f, -90f, 0f);
			vector2 = quaternion * normal;
			ray2 = new Ray(vector + normal * 0.1f, vector2);
			if (Physics.Raycast(ray2, out hitInfo2, m_MovementScript.m_CoverWallCollisionDist, 256))
			{
				float num2 = m_MovementScript.m_CoverWallCollisionDist - hitInfo2.distance;
				vector -= vector2 * num2;
			}
			Vector3 position2 = vector + normal * m_CoverDistFromWall;
			position2.y = GetGroundHeight(position2) + 0.1f;
			base.transform.position = position2;
			Vector3 forward = base.transform.forward;
			float num3 = normal.x * forward.z - normal.z * forward.x;
			m_CoverSide = CoverSide.Left;
			if (num3 > 0f)
			{
				m_CoverSide = CoverSide.Right;
			}
			float yaw = Globals.m_CameraController.GetYaw();
			float pitch = Globals.m_CameraController.GetPitch();
			Globals.m_CameraController.LookAt(vector);
			SetThirdPerson();
			Globals.m_CameraController.SetRotation(yaw, pitch);
			Vector3 forward2 = m_Camera.transform.forward;
			forward2.y = 0f;
			forward2.Normalize();
			m_CameraCoverDot = Vector3.Dot(base.transform.forward, forward2);
			UpdateCoverEdge();
			if (m_CoverEdge == CoverEdge.Left)
			{
				m_CoverSide = CoverSide.Left;
			}
			if (m_CoverEdge == CoverEdge.Right)
			{
				m_CoverSide = CoverSide.Right;
			}
			if (m_Stance == Stance.Stand && m_AllowUpAndOverCoverFire)
			{
				SetStance(Stance.Crouch);
			}
			if (m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Reloading)
			{
				SetCoverState(CoverState.TransitioningIn);
			}
			else
			{
				SetCoverState(CoverState.Reloading);
			}
			Globals.m_HUD.TurnOnCoverButton(false);
			UpdateCoverInternalInfo(vector, normal, collider);
			m_ForceCoverButtonInactive = false;
		}
	}

	private void UpdateCoverInternalInfo(Vector3 wallPos, Vector3 normal, Collider collider)
	{
		if (m_CoverCollider != collider)
		{
			m_CoverAllowsCornering = true;
			DisableCornering component = collider.gameObject.GetComponent<DisableCornering>();
			if (component != null)
			{
				m_CoverAllowsCornering = false;
			}
		}
		m_CoverPoint = wallPos;
		m_CoverNormal = normal;
		m_CoverCollider = collider;
	}

	public void ExitCover()
	{
		if (!CanExitCover())
		{
			return;
		}
		m_MovementScript.CancelTapToMove();
		SetCoverState(CoverState.Outside);
		m_CurrentCameraShift = 0f;
		m_CurrentCamera.transform.parent.transform.localPosition = new Vector3(m_CurrentCameraShift, m_CurrentCamera.transform.parent.transform.localPosition.y, m_CurrentCamera.transform.parent.transform.localPosition.z);
		m_ModelThirdPerson.transform.parent = base.transform;
		m_ModelThirdPerson.transform.localRotation = Quaternion.identity;
		m_ModelThirdPerson.transform.localPosition = Vector3.zero;
		m_WeaponScript.ResetFOV();
		SetFirstPerson();
		if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Reloading)
		{
			m_WeaponScript.m_ReloadTime = m_WeaponScript.GetFirstPersonAnimLength(WeaponBase.FirstPersonAnimation.Reload);
			if (m_WeaponScript.m_ReloadTimer > m_WeaponScript.m_ReloadTime)
			{
				m_WeaponScript.m_ReloadTimer = m_WeaponScript.m_ReloadTime;
			}
		}
		Globals.m_HUD.TurnOnCoverButton(false);
	}

	public void SetFirstPerson()
	{
		m_CameraMode = CameraMode.First;
		Globals.m_CameraController.SetFirstPerson();
		m_ModelFirstPerson.SetActiveRecursively(true);
		if (m_CurrentWeapon != null)
		{
			m_WeaponScript.SetFirstPerson();
		}
		if (m_FrameNum != 0)
		{
			m_ModelThirdPerson.SetActiveRecursively(false);
		}
		Globals.m_HUDRoot.SetParentCamera();
	}

	public void SetThirdPerson()
	{
		m_CameraMode = CameraMode.Third;
		Globals.m_CameraController.SetThirdPerson();
		m_ModelThirdPerson.SetActiveRecursively(true);
		m_ModelThirdPerson.animation.Play("StandingIdle");
		m_WeaponScript.SetThirdPerson();
		m_ModelFirstPerson.SetActiveRecursively(false);
		Globals.m_HUDRoot.SetParentCamera();
	}

	public void SetTakedown()
	{
		m_CameraMode = CameraMode.Takedown;
		m_ModelThirdPerson.SetActiveRecursively(false);
		m_ModelFirstPerson.SetActiveRecursively(false);
	}

	public void ToggleWeaponHolstered()
	{
		if (m_CameraMode == CameraMode.First || m_CameraMode == CameraMode.Takedown)
		{
			m_WeaponScript.ToggleHostered();
			return;
		}
		if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Idle)
		{
			SetCoverState(CoverState.Holstering);
			return;
		}
		m_WeaponScript.ToggleHostered();
		SetCoverState(CoverState.Drawing);
	}

	public void CancelMovement()
	{
		m_MovementScript.CancelMovement();
	}

	public float GetTapTime(int tapID)
	{
		return m_TapTime[tapID];
	}

	public override Ray WeaponRequestForBulletRay(out bool PlayTracer)
	{
		PlayTracer = true;
		return Globals.m_PlayerController.m_CurrentCamera.ScreenPointToRay(Globals.m_ScreenCenter);
	}

	public override void WeaponWantsReload()
	{
		StopFiring();
		if (m_CameraMode == CameraMode.Third && m_CoverState == CoverState.Firing)
		{
			SetCoverState(CoverState.TransitioningFromFire);
		}
		if (m_WeaponScript.GetAmmo() > 0)
		{
			ReloadWeapon();
		}
	}

	public override void WeaponDoneReloading()
	{
		if (m_WantFire)
		{
			StartFiring();
		}
	}

	public void GiveAmmo(WeaponType type, int amount)
	{
		if (type == m_WeaponScript.m_WeaponType)
		{
			m_WeaponScript.GiveAmmo(amount);
		}
		else
		{
			m_Ammo[(int)type] += amount;
		}
	}

	public override void HitByTranquilizer()
	{
		base.HitByTranquilizer();
	}

	public override bool TakeDamage(int Damage, GameObject Damager, DamageType Type = DamageType.Normal)
	{
		bool result = false;
		switch (Type)
		{
		case DamageType.Concussion:
		{
			m_ConcussionQuad.gameObject.active = true;
			Color color = m_ConcussionQuad.Color;
			color.a = 1f;
			m_ConcussionQuad.Color = color;
			m_ConcussionGrenadeTimer = m_ConcussionGrenadeTime;
			m_EMPed = false;
			break;
		}
		case DamageType.EMP:
		{
			m_ConcussionQuad.gameObject.active = true;
			Color color = m_ConcussionQuad.Color;
			color.a = 1f;
			m_ConcussionQuad.Color = color;
			m_ConcussionGrenadeTimer = m_ConcussionGrenadeTime;
			Globals.m_HUD.Display(false, true);
			m_EMPed = true;
			break;
		}
		default:
			if (m_CurrentArmor > 0)
			{
				Damage /= 2;
			}
			m_CurrentArmor -= Damage;
			if (m_CurrentArmor < 0)
			{
				m_CurrentArmor = 0;
			}
			if (m_GodMode && m_CurrentHealth - Damage <= 0)
			{
				Damage = 0;
			}
			result = base.TakeDamage(Damage, Damager, Type);
			m_PlayerDamage.TakeDamage(Damager.transform.position);
			if (m_DamageVODelay <= 0f && (double)UnityEngine.Random.value <= 0.7)
			{
				EventManager.Instance.PostEvent("VO_PC_Damage", EventAction.PlaySound, null, base.gameObject);
				m_DamageVODelay = 3f;
			}
			m_HealthRegenTimer = m_TimeBeforeHealthRegen;
			break;
		}
		return result;
	}

	public override void Die(GameObject Damager, DamageType Type = DamageType.Normal)
	{
		if (!m_GodMode)
		{
			CancelMovement();
			StopFiring();
			Globals.m_AugmentCloaking.Disable();
			if (m_CameraMode == CameraMode.First)
			{
				m_ModelFirstPerson.SetActiveRecursively(false);
			}
			EventManager.Instance.PostEvent("VO_PC_Death", EventAction.PlaySound, null, base.gameObject);
			m_DeathTimer = m_DeathTime;
		}
	}

	public float GetGroundHeight(Vector3 position)
	{
		Ray ray = new Ray(position + new Vector3(0f, 0.5f, 0f), new Vector3(0f, -1f, 0f));
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 30f, 257))
		{
			return hitInfo.point.y;
		}
		return 0f;
	}

	protected override void AttachShadowObject()
	{
		m_ShadowObject.transform.parent = base.transform;
		m_ShadowObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		m_ShadowObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	private void OnGUI()
	{
		if (UnityEngine.Event.current.type != EventType.Repaint || GameManager.IsGamePaused() || !Globals.m_HUD.m_Showing || m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Reloading)
		{
			return;
		}
		int layerMask = 2163457;
		Ray ray = default(Ray);
		Vector2 vector = new Vector2(Screen.width / 2, Screen.height / 2);
		ray = Globals.m_PlayerController.m_CurrentCamera.ScreenPointToRay(vector);
		bool flag = false;
		bool flag2 = false;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, layerMask))
		{
			if (hitInfo.collider.gameObject.layer == 9 || hitInfo.collider.gameObject.layer == 22)
			{
				flag = true;
			}
			if (hitInfo.collider.gameObject.layer == 21 || hitInfo.collider.gameObject.tag == "FriendlyNPC")
			{
				flag2 = true;
			}
		}
		float num = (float)Screen.width / 1024f;
		if (num < 1f)
		{
			num = 1f;
		}
		GUI.color = m_ReticleBaseColor;
		if (flag)
		{
			GUI.color = m_ReticleHostileColor;
		}
		if (flag2)
		{
			GUI.color = m_ReticleFriendlyColor;
		}
		if (m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Holstered && m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Holstering && m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Drawing)
		{
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - (float)m_WeaponScript.m_ReticleImage.width * num / 2f, (float)(Screen.height / 2) - (float)m_WeaponScript.m_ReticleImage.height * num / 2f, (float)m_WeaponScript.m_ReticleImage.width * num, (float)m_WeaponScript.m_ReticleImage.height * num), m_WeaponScript.m_ReticleImage);
		}
		else
		{
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - (float)m_WeaponScript.m_ReticleImageDot.width * num / 2f, (float)(Screen.height / 2) - (float)m_WeaponScript.m_ReticleImageDot.height * num / 2f, (float)m_WeaponScript.m_ReticleImageDot.width * num, (float)m_WeaponScript.m_ReticleImageDot.height * num), m_WeaponScript.m_ReticleImageDot);
		}
		GUI.color = Color.white;
	}

	private void SetupAnimList()
	{
		LoadAnim("StandingIdle");
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionLeftToRight, "COV_Crouch_LefttoRight_WPN_CombatRifle", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionRightToLeft, "COV_Crouch_RighttoLeft_WPN_CombatRifle", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionLeftToRight, "COV_Stand_LefttoRight_WPN_CombatRifle", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionRightToLeft, "COV_Stand_RighttoLeft_WPN_CombatRifle", false, string.Empty);
		SetAnimSoundEventName(CoverAnimation.TransitionLeftToRight, WeaponAnimSet.CombatRifle, "Weapon_Handle");
		SetAnimSoundEventName(CoverAnimation.TransitionRightToLeft, WeaponAnimSet.CombatRifle, "Weapon_Handle");
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Enter, "COV_CrouchEnterLeft_WPN_CombatRifle", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Enter, "COV_CrouchEnterRight_WPN_CombatRifle", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Enter, "COV_StandEnterLeft_WPN_CombatRifle", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Enter, "COV_StandEnterRight_WPN_CombatRifle", false, string.Empty);
		SetAnimSoundEventName(CoverAnimation.Enter, WeaponAnimSet.CombatRifle, "Weapon_Handle");
		SetAnimSoundEventName(CoverAnimation.Enter, WeaponAnimSet.CombatRifle, "Footsteps_3rd", 0.233f);
		SetAnimSoundEventName(CoverAnimation.Enter, WeaponAnimSet.CombatRifle, "Footsteps_3rd", 0.533f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Idle, "COV_CrouchIdleLeft_WPN_CombatRifle", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Idle, "COV_CrouchIdleRight_WPN_CombatRifle", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Idle, "COV_StandLeftIdle_WPN_CombatRifle", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Idle, "COV_StandRightIdle_WPN_CombatRifle", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Reload, "COV_CrouchLeft_WPN_CombatRifle_Reload", false, "CBR_TP_CrouchLeft_Reload");
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Reload, "COV_CrouchRight_WPN_CombatRifle_Reload", false, "CBR_TP_CrouchRight_Reload");
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Reload, "COV_StandLeft_WPN_CombatRifle_Reload", false, "CBR_TP_StandLeft_Reload");
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Reload, "COV_StandRight_WPN_CombatRifle_Reload", false, "CBR_TP_StandRight_Reload");
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Stand, "COV_CrouchLefttoStandLeft_WPN_CombatRifle", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Stand, "COV_CrouchRighttoStandRight_WPN_CombatRifle", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Crouch, "COV_StandLefttoCrouchLeft_WPN_CombatRifle", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Crouch, "COV_StandRighttoCrouchRight_WPN_CombatRifle", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionToLeanFire, "COV_CrouchLeft_WPN_CombatRifle_Fire_IN", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionToLeanFire, "COV_CrouchRight_WPN_CombatRifle_Fire_IN", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionToLeanFire, "COV_StandLeft_WPN_CombatRifle_Fire_IN", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionToLeanFire, "COV_StandRight_WPN_CombatRifle_Fire_IN", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.LeanFiring, "COV_CrouchLeft_WPN_CombatRifle_Fire_Loop", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.LeanFiring, "COV_CrouchRight_WPN_CombatRifle_Fire_Loop", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.LeanFiring, "COV_StandLeft_WPN_CombatRifle_Fire_Loop", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.LeanFiring, "COV_StandRight_WPN_CombatRifle_Fire_Loop", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionFromLeanFire, "COV_CrouchLeft_WPN_CombatRifle_Fire_OUT", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionFromLeanFire, "COV_CrouchRight_WPN_CombatRifle_Fire_OUT", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionFromLeanFire, "COV_StandLeft_WPN_CombatRifle_Fire_OUT", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionFromLeanFire, "COV_StandRight_WPN_CombatRifle_Fire_OUT", false, string.Empty);
		SetAnimSoundEventName(CoverAnimation.TransitionFromLeanFire, WeaponAnimSet.CombatRifle, "Weapon_Handle");
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionToUpOverFire, "COV_CrouchLeft_UpOver_WPN_CombatRifle_Fire_IN", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionToUpOverFire, "COV_CrouchRight_UpOver_WPN_CombatRifle_Fire_IN", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.UpOverFiring, "COV_CrouchLeft_UpOver_WPN_CombatRifle_Fire_Loop", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.UpOverFiring, "COV_CrouchRight_UpOver_WPN_CombatRifle_Fire_Loop", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionFromUpOverFire, "COV_CrouchLeft_UpOver_WPN_CombatRifle_Fire_OUT", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionFromUpOverFire, "COV_CrouchRight_UpOver_WPN_CombatRifle_Fire_OUT", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAiming, "COV_CrouchLeft_WPN_CombatRifle_Aim_Idle", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAiming, "COV_CrouchRight_WPN_CombatRifle_Aim_Idle", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAiming, "COV_StandLeft_WPN_CombatRifle_Aim_Idle", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAiming, "COV_StandRight_WPN_CombatRifle_Aim_Idle", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimFiring, "COV_CrouchLeft_WPN_CombatRifle_Fire_Loop", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimFiring, "COV_CrouchRight_WPN_CombatRifle_Fire_Loop", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimFiring, "COV_StandLeft_WPN_CombatRifle_Fire_Loop", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimFiring, "COV_StandRight_WPN_CombatRifle_Fire_Loop", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimReloading, "COV_CrouchLeft_AimPose_WPN_CombatRifle_Reload", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimReloading, "COV_CrouchRight_AimPose_WPN_CombatRifle_Reload", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimReloading, "COV_StandLeft_AimPose_WPN_CombatRifle_Reload", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimReloading, "COV_StandRight_AimPose_WPN_CombatRifle_Reload", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimThrowingGrenade, "COV_CrouchLeft_WPN_CombatRifle_throwGrenadeUpAim", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimThrowingGrenade, "COV_CrouchRight_WPN_CombatRifle_throwGrenadeUpAim", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimThrowingGrenade, "COV_StandLeft_WPN_CombatRifle_throwGrenadeAim", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimThrowingGrenade, "COV_StandRight_WPN_CombatRifle_throwGrenadeAim", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimingUpOver, "COV_CrouchLeft_UpOver_WPN_CombatRifle_Aim_Idle", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimingUpOver, "COV_CrouchRight_UpOver_WPN_CombatRifle_Aim_Idle", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimFiringUpOver, "COV_CrouchLeft_UpOver_WPN_CombatRifle_Fire_Loop", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimFiringUpOver, "COV_CrouchRight_UpOver_WPN_CombatRifle_Fire_Loop", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimReloadingUpOver, "COV_UpOverLeft_AimPose_WPN_CombatRifle_Reload", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverAimReloadingUpOver, "COV_UpOverRight_AimPose_WPN_CombatRifle_Reload", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Move, "COV_CrouchLeft_WPN_CombatRifle_walkFWD", true, string.Empty, 0.3f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Move, "COV_CrouchRight_WPN_CombatRifle_walkFWD", true, string.Empty, 0.3f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Move, "COV_StandLeft_WPN_CombatRifle_walkFWD", true, string.Empty, 0.3f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Move, "COV_StandRight_WPN_CombatRifle_walkFWD", true, string.Empty, 0.3f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.MoveBackwards, "COV_CrouchLeft_WPN_CombatRifle_walkBACK", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.MoveBackwards, "COV_CrouchRight_WPN_CombatRifle_walkBACK", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.MoveBackwards, "COV_StandLeft_WPN_CombatRifle_walkBACK", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.MoveBackwards, "COV_StandRight_WPN_CombatRifle_walkBACK", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Holster, "COV_CrouchLeft_WPN_CombatRifle_Holster", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Holster, "COV_CrouchRight_WPN_CombatRifle_Holster", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Holster, "COV_StandLeft_WPN_CombatRifle_Holster", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Holster, "COV_StandRight_WPN_CombatRifle_Holster", false, string.Empty);
		SetAnimSoundEventName(CoverAnimation.Holster, WeaponAnimSet.CombatRifle, "Weapon_Holster");
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Draw, "COV_CrouchLeft_WPN_CombatRifle_Draw", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Draw, "COV_CrouchRight_WPN_CombatRifle_Draw", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Draw, "COV_StandLeft_WPN_CombatRifle_Draw", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Draw, "COV_StandRight_WPN_CombatRifle_Draw", false, string.Empty);
		SetAnimSoundEventName(CoverAnimation.Holster, WeaponAnimSet.CombatRifle, "Weapon_Draw");
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverFlip, "COV_CrouchLeft_WPN_CombatRifle_Flip", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverFlip, "COV_CrouchRight_WPN_CombatRifle_Flip", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverFlip, "COV_StandLeft_WPN_CombatRifle_Flip", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverFlip, "COV_StandRight_WPN_CombatRifle_Flip", false, string.Empty);
		SetAnimSoundEventName(CoverAnimation.CoverFlip, WeaponAnimSet.CombatRifle, "Cover_Flip_Short");
		SetAnimSoundEventName(CoverAnimation.CoverFlip, WeaponAnimSet.CombatRifle, "Footsteps_Land", 0.38f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverDive, "COV_CrouchLeft_WPN_CombatRifle_Dive", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverDive, "COV_CrouchRight_WPN_CombatRifle_Dive", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverDive, "COV_StandLeft_WPN_CombatRifle_Dive", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverDive, "COV_StandRight_WPN_CombatRifle_Dive", false, string.Empty);
		SetAnimSoundEventName(CoverAnimation.CoverDive, WeaponAnimSet.CombatRifle, "Cover_Dive_Long");
		SetAnimSoundEventName(CoverAnimation.CoverDive, WeaponAnimSet.CombatRifle, "Footsteps_Land", 0.4f);
		SetAnimSoundEventName(CoverAnimation.CoverDive, WeaponAnimSet.CombatRifle, "Footsteps_Scuff", 0.7f);
		SetAnimSoundEventName(CoverAnimation.CoverDive, WeaponAnimSet.CombatRifle, "Footsteps_Scuff", 0.8f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideInner, "COV_CrouchLeft_WPN_CombatRifle_Inside90Edge", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideInner, "COV_CrouchRight_WPN_CombatRifle_Inside90Edge", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideInner, "COV_StandLeft_WPN_CombatRifle_Inside90Edge", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideInner, "COV_StandRight_WPN_CombatRifle_Inside90Edge", false, string.Empty);
		SetAnimSoundEventName(CoverAnimation.CoverSlideInner, WeaponAnimSet.CombatRifle, "Footsteps_Scuff");
		SetAnimSoundEventName(CoverAnimation.CoverSlideInner, WeaponAnimSet.CombatRifle, "Footsteps_3rd");
		SetAnimSoundEventName(CoverAnimation.CoverSlideInner, WeaponAnimSet.CombatRifle, "Weapon_Handle");
		SetAnimSoundEventName(CoverAnimation.CoverSlideInner, WeaponAnimSet.CombatRifle, "Footsteps_3rd", 0.36f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideOuter, "COV_CrouchLeft_WPN_CombatRifle_CornerOut90Edge", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideOuter, "COV_CrouchRight_WPN_CombatRifle_CornerOut90Edge", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideOuter, "COV_StandLeft_WPN_CombatRifle_CornerOut90Edge", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideOuter, "COV_StandRight_WPN_CombatRifle_CornerOut90Edge", false, string.Empty);
		SetAnimSoundEventName(CoverAnimation.CoverSlideOuter, WeaponAnimSet.CombatRifle, "Footsteps_Scuff");
		SetAnimSoundEventName(CoverAnimation.CoverSlideOuter, WeaponAnimSet.CombatRifle, "Footsteps_3rd");
		SetAnimSoundEventName(CoverAnimation.CoverSlideOuter, WeaponAnimSet.CombatRifle, "Weapon_Handle");
		SetAnimSoundEventName(CoverAnimation.CoverSlideOuter, WeaponAnimSet.CombatRifle, "Footsteps_3rd", 0.36f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.ThrowGrenade, "COV_CrouchLeft_WPN_CombatRifle_throwGrenadeCorner", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.ThrowGrenade, "COV_CrouchRight_WPN_CombatRifle_throwGrenadeCorner", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.ThrowGrenade, "COV_StandLeft_WPN_CombatRifle_throwGrenadeCorner", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.ThrowGrenade, "COV_StandRight_WPN_CombatRifle_throwGrenadeCorner", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.ThrowGrenadeUpOver, "COV_CrouchLeft_WPN_CombatRifle_throwGrenadeUpOver", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.ThrowGrenadeUpOver, "COV_CrouchRight_WPN_CombatRifle_throwGrenadeUpOver", false, string.Empty);
		CopyAnimSet(WeaponAnimSet.CombatRifle, WeaponAnimSet.Crossbow);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.LeanFiring, "COV_CrouchLeft_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire");
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.LeanFiring, "COV_CrouchRight_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire");
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Crossbow, CoverAnimation.LeanFiring, "COV_StandLeft_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire");
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Crossbow, CoverAnimation.LeanFiring, "COV_StandRight_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire");
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.UpOverFiring, "COV_CrouchLeft_UpOver_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire");
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.UpOverFiring, "COV_CrouchRight_UpOver_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire");
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.CoverAimFiring, "COV_CrouchLeft_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire");
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.CoverAimFiring, "COV_CrouchRight_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire");
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Crossbow, CoverAnimation.CoverAimFiring, "COV_StandLeft_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire");
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Crossbow, CoverAnimation.CoverAimFiring, "COV_StandRight_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire");
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.CoverAimFiringUpOver, "COV_CrouchLeft_UpOver_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire");
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.CoverAimFiringUpOver, "COV_CrouchRight_UpOver_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire");
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.Reload, "COV_CrouchLeft_WPN_Crossbow_Reload", false, "CRB_TP_CrouchLeft_Reload");
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.Reload, "COV_CrouchRight_WPN_Crossbow_Reload", false, "CRB_TP_CrouchRight_Reload");
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Crossbow, CoverAnimation.Reload, "COV_StandLeft_WPN_Crossbow_Reload", false, "CRB_TP_StandLeft_Reload");
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Crossbow, CoverAnimation.Reload, "COV_StandRight_WPN_Crossbow_Reload", false, "CRB_TP_StandRight_Reload");
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.CoverAimReloading, "COV_CrouchLeft_AimPose_WPN_Crossbow_Reload", true, "CRB_TP_CrouchLeft_AimPose_Reload");
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.CoverAimReloading, "COV_CrouchRight_AimPose_WPN_Crossbow_Reload", true, "CRB_TP_CrouchRight_AimPose_Reload");
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Crossbow, CoverAnimation.CoverAimReloading, "COV_StandLeft_AimPose_WPN_Crossbow_Reload", true, "CRB_TP_StandLeft_AimPose_Reload");
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Crossbow, CoverAnimation.CoverAimReloading, "COV_StandRight_AimPose_WPN_Crossbow_Reload", true, "CRB_TP_StandRight_AimPose_Reload");
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.CoverAimReloadingUpOver, "COV_UpOverLeft_AimPose_WPN_Crossbow_Reload", true, "CRB_TP_UpOverLeft_AimPose_WPN_Crossbow_Reload");
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.CoverAimReloadingUpOver, "COV_UpOverRight_AimPose_WPN_Crossbow_Reload", true, "CRB_TP_UpOverRight_AimPose_WPN_Crossbow_Reload");
		CopyAnimSet(WeaponAnimSet.CombatRifle, WeaponAnimSet.Shotgun);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.LeanFiring, "COV_CrouchLeft_WPN_Shotgun_Fire_Loop", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.LeanFiring, "COV_CrouchRight_WPN_Shotgun_Fire_Loop", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Shotgun, CoverAnimation.LeanFiring, "COV_StandLeft_WPN_Shotgun_Fire_Loop", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Shotgun, CoverAnimation.LeanFiring, "COV_StandRight_WPN_Shotgun_Fire_Loop", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.UpOverFiring, "COV_CrouchLeft_UpOver_WPN_Shotgun_Fire_Loop", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.UpOverFiring, "COV_CrouchRight_UpOver_WPN_Shotgun_Fire_Loop", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.CoverAimFiring, "COV_CrouchLeft_WPN_Shotgun_Fire_Loop", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.CoverAimFiring, "COV_CrouchRight_WPN_Shotgun_Fire_Loop", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Shotgun, CoverAnimation.CoverAimFiring, "COV_StandLeft_WPN_Shotgun_Fire_Loop", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Shotgun, CoverAnimation.CoverAimFiring, "COV_StandRight_WPN_Shotgun_Fire_Loop", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.CoverAimFiringUpOver, "COV_CrouchLeft_UpOver_WPN_Shotgun_Fire_Loop", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.CoverAimFiringUpOver, "COV_CrouchRight_UpOver_WPN_Shotgun_Fire_Loop", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.Reload, "COV_CrouchLeft_WPN_Shotgun_Reload", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.Reload, "COV_CrouchRight_WPN_Shotgun_Reload", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Shotgun, CoverAnimation.Reload, "COV_StandLeft_WPN_Shotgun_Reload", false, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Shotgun, CoverAnimation.Reload, "COV_StandRight_WPN_Shotgun_Reload", false, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.CoverAimReloading, "COV_CrouchLeft_AimPose_WPN_Shotgun_Reload", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.CoverAimReloading, "COV_CrouchRight_AimPose_WPN_Shotgun_Reload", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Shotgun, CoverAnimation.CoverAimReloading, "COV_StandLeft_AimPose_WPN_Shotgun_Reload", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Shotgun, CoverAnimation.CoverAimReloading, "COV_StandRight_AimPose_WPN_Shotgun_Reload", true, string.Empty);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.CoverAimReloadingUpOver, "COV_UpOverLeft_AimPose_WPN_Shotgun_Reload", true, string.Empty);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.CoverAimReloadingUpOver, "COV_UpOverRight_AimPose_WPN_Shotgun_Reload", true, string.Empty);
	}

	private void SetupAnim(CoverSide side, Stance stance, WeaponAnimSet animset, CoverAnimation anim, string name, bool loop, string weaponanimname = "", float blendtime = 0.1f)
	{
		if (side == CoverSide.Left)
		{
			m_CoverAnimationsLeft[(int)stance, (int)animset, (int)anim].name = name;
			m_CoverAnimationsLeft[(int)stance, (int)animset, (int)anim].weaponAnimName = weaponanimname;
			m_CoverAnimationsLeft[(int)stance, (int)animset, (int)anim].loop = loop;
			m_CoverAnimationsLeft[(int)stance, (int)animset, (int)anim].blendTime = blendtime;
		}
		else
		{
			m_CoverAnimationsRight[(int)stance, (int)animset, (int)anim].name = name;
			m_CoverAnimationsRight[(int)stance, (int)animset, (int)anim].weaponAnimName = weaponanimname;
			m_CoverAnimationsRight[(int)stance, (int)animset, (int)anim].loop = loop;
			m_CoverAnimationsRight[(int)stance, (int)animset, (int)anim].blendTime = blendtime;
		}
		LoadAnim(name);
	}

	private void LoadAnim(string name)
	{
		GameObject gameObject = Resources.Load("Animation/saxon_game/saxon@" + name) as GameObject;
		if (gameObject != null)
		{
			m_ModelThirdPerson.animation.AddClip(gameObject.animation.clip, name);
		}
		else
		{
			Debug.LogError("Missing Player Anim: " + name);
		}
	}

	private void SetAnimSoundEventName(CoverAnimation anim, WeaponAnimSet weapon, string soundeventname, float delay = 0f)
	{
		CoverAnimationSoundEvent coverAnimationSoundEvent = new CoverAnimationSoundEvent();
		coverAnimationSoundEvent.name = soundeventname;
		coverAnimationSoundEvent.delay = delay;
		coverAnimationSoundEvent.delayTimer = delay;
		if (m_CoverAnimationsLeft[1, (int)weapon, (int)anim].soundEvents == null)
		{
			m_CoverAnimationsLeft[1, (int)weapon, (int)anim].soundEvents = new List<CoverAnimationSoundEvent>();
			m_CoverAnimationsRight[1, (int)weapon, (int)anim].soundEvents = new List<CoverAnimationSoundEvent>();
			m_CoverAnimationsLeft[0, (int)weapon, (int)anim].soundEvents = new List<CoverAnimationSoundEvent>();
			m_CoverAnimationsRight[0, (int)weapon, (int)anim].soundEvents = new List<CoverAnimationSoundEvent>();
		}
		m_CoverAnimationsLeft[1, (int)weapon, (int)anim].soundEvents.Add(coverAnimationSoundEvent);
		m_CoverAnimationsRight[1, (int)weapon, (int)anim].soundEvents.Add(coverAnimationSoundEvent);
		m_CoverAnimationsLeft[0, (int)weapon, (int)anim].soundEvents.Add(coverAnimationSoundEvent);
		m_CoverAnimationsRight[0, (int)weapon, (int)anim].soundEvents.Add(coverAnimationSoundEvent);
	}

	private void CopyAnimSet(WeaponAnimSet source, WeaponAnimSet dest)
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 30; j++)
			{
				m_CoverAnimationsLeft[i, (int)dest, j] = m_CoverAnimationsLeft[i, (int)source, j];
				m_CoverAnimationsRight[i, (int)dest, j] = m_CoverAnimationsRight[i, (int)source, j];
			}
		}
	}

	private void SetupTakedowns()
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				m_TakedownAnimation[i, j] = new TakedownAnimation();
			}
		}
		TakedownAnimationData takedownAnimationData = m_TakedownAnimation[0, 1].AddAnimation("TD_LT_DoubleBlade_Back_C_3", TakedownEnemyCount.Single, "TD_LT_DoubleBlade_Back_C_3");
		takedownAnimationData.AddCameraData(TakedownCamera.Camera1, 5.33f);
		takedownAnimationData.AddVFXData(m_DoubleBladeBackVFXPrefab);
		takedownAnimationData = m_TakedownAnimation[0, 0].AddAnimation("TD_LT_ArmLockBlade_Front_3", TakedownEnemyCount.Single, "TD_LT_ArmLockBlade_Front_3");
		takedownAnimationData.AddCameraData(TakedownCamera.Camera1, 0.833f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera2, 2.033f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera3, 5.8f);
		takedownAnimationData.AddVFXData(m_ArmlockBladeFrontVFXPrefab);
		takedownAnimationData = m_TakedownAnimation[1, 1].AddAnimation("TD_NT_Knee_BK_4", TakedownEnemyCount.Single, "TD_NT_Knee_BK_4");
		takedownAnimationData.AddCameraData(TakedownCamera.Camera1, 3.67f);
		takedownAnimationData.AddVFXData(m_KneeBackVFXPrefab);
		takedownAnimationData = m_TakedownAnimation[1, 0].AddAnimation("TD_NT_SinglePunch_Front_2", TakedownEnemyCount.Single, "TD_NT_SinglePunch_Front_2");
		takedownAnimationData.AddCameraData(TakedownCamera.Camera1, 2.367f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera2, 4.33f);
		takedownAnimationData.AddVFXData(m_SinglePunchFrontVFXPrefab);
		DisableTakedownCameras();
	}

	public void BeginTakedown(GameObject enemy, bool lethal)
	{
		m_PressedForTakedown = false;
		if (!IsEnergyAvailable(1f))
		{
			return;
		}
		m_TakedownEnemy = enemy.GetComponent<Enemy_Base>();
		if (m_TakedownEnemy.IsDead())
		{
			m_TakedownEnemy = null;
			return;
		}
		InteractiveObject_Takedown componentInChildren = enemy.GetComponentInChildren<InteractiveObject_Takedown>();
		if (componentInChildren != null)
		{
			componentInChildren.InteractWithObject();
		}
		else
		{
			Debug.Log(enemy.name + " is missing an InteractiveObject_Takedown script.");
		}
		m_TakedownEnemy.BeginTakedownOnEnemy();
		UseEnergy(1f);
		Globals.m_HUD.Display(false, true);
		Globals.m_HUD.EnablePassThruInput(false);
		if (m_CoverState != 0)
		{
			ExitCover();
		}
		CancelMovement();
		m_PreWeaponState = m_WeaponScript.m_WeaponState;
		SetTakedown();
		Globals.m_AIDirector.SetEnemiesPaused(true);
		m_TakedownEnemy.StopBurstFire();
		m_TakedownModel = UnityEngine.Object.Instantiate(m_TakedownModelPrefab) as GameObject;
		m_TakedownModel.transform.parent = base.gameObject.transform;
		m_TakedownModel.transform.localPosition = Vector3.zero;
		m_TakedownModel.transform.localRotation = Quaternion.identity;
		m_TakedownModel.transform.localScale = Vector3.one;
		float num = Vector3.Dot(m_TakedownModel.transform.forward, enemy.transform.forward);
		PositionTakedown(enemy);
		Vector3 position = enemy.transform.position;
		position.y = GetGroundHeight(position) + 0.01f;
		base.gameObject.transform.position = position;
		m_LethalTakedown = lethal;
		TakedownAttackType takedownAttackType = ((!m_LethalTakedown) ? TakedownAttackType.NonLethal : TakedownAttackType.Lethal);
		m_TakedownPositioning = ((!(num < 0f)) ? TakedownPositioning.Back : TakedownPositioning.Front);
		TakedownEnemyCount count = TakedownEnemyCount.Single;
		enemy.transform.rotation = Quaternion.Euler(0f, enemy.transform.rotation.eulerAngles.y + ((m_TakedownPositioning != 0) ? 0f : 180f), 0f);
		Globals.m_CameraController.SetYaw(enemy.transform.rotation.eulerAngles.y);
		m_CurrentAnimationData = m_TakedownAnimation[(int)takedownAttackType, (int)m_TakedownPositioning].GetAnimationData(count);
		m_CurrentAnimationData.Reset();
		m_CurrentAnimationData.PlayVFX(componentInChildren.m_TakedownVFXJoint);
		EnableTakedownCamera(m_CurrentAnimationData.GetCurrentTakedownCamera());
		m_CurrentCamera.enabled = false;
		m_TakedownModel.animation.Play(m_CurrentAnimationData.name);
		m_TakedownAnimator.animation.Play("CameraJnt_" + m_CurrentAnimationData.name);
		m_TakedownEnemy.m_Animator.Play(m_CurrentAnimationData.name);
		if (m_CurrentAnimationData.soundEventName != string.Empty)
		{
			EventManager.Instance.PostEvent(m_CurrentAnimationData.soundEventName, EventAction.PlaySound, null, base.gameObject);
		}
	}

	private void EndTakedown()
	{
		m_CurrentAnimationData.CleanUpVFX();
		Globals.m_AIDirector.SetEnemiesPaused(false);
		DisableTakedownCameras();
		m_CurrentCamera.enabled = true;
		m_RendererThirdPerson.enabled = true;
		SetFirstPerson();
		if (m_PreWeaponState != WeaponBase.WeaponState.Holstering && m_PreWeaponState != WeaponBase.WeaponState.Holstered)
		{
			m_WeaponScript.Unholster();
		}
		if (m_LethalTakedown)
		{
			m_TakedownEnemy.TakedownLethal();
			Globals.m_AIDirector.CheckAudioSenses(base.transform.position, 20f, DisturbanceEvent.MajorAudio);
		}
		else
		{
			m_TakedownEnemy.TakedownNonLethal();
		}
		Globals.m_HUD.Display(true, true);
		Globals.m_HUD.EnablePassThruInput(true);
		m_TakedownEnemy = null;
		UnityEngine.Object.Destroy(m_TakedownModel);
	}

	private void UpdateTakedown()
	{
		if (m_CurrentAnimationData.Update(Time.deltaTime))
		{
			if (m_CurrentAnimationData.GetCurrentTakedownCamera() == TakedownCamera.NoCamera)
			{
				EndTakedown();
			}
			else
			{
				EnableTakedownCamera(m_CurrentAnimationData.GetCurrentTakedownCamera());
			}
		}
	}

	private void DisableTakedownCameras()
	{
		if ((bool)m_TakedownCameras.Camera1)
		{
			m_TakedownCameras.Camera1.enabled = false;
		}
		if ((bool)m_TakedownCameras.Camera2)
		{
			m_TakedownCameras.Camera2.enabled = false;
		}
		if ((bool)m_TakedownCameras.Camera3)
		{
			m_TakedownCameras.Camera3.enabled = false;
		}
		if ((bool)m_TakedownCameras.Camera4)
		{
			m_TakedownCameras.Camera4.enabled = false;
		}
	}

	private void EnableTakedownCamera(TakedownCamera camera)
	{
		if ((bool)m_TakedownCameras.Camera1)
		{
			m_TakedownCameras.Camera1.enabled = camera == TakedownCamera.Camera1;
		}
		if ((bool)m_TakedownCameras.Camera2)
		{
			m_TakedownCameras.Camera2.enabled = camera == TakedownCamera.Camera2;
		}
		if ((bool)m_TakedownCameras.Camera3)
		{
			m_TakedownCameras.Camera3.enabled = camera == TakedownCamera.Camera3;
		}
		if ((bool)m_TakedownCameras.Camera4)
		{
			m_TakedownCameras.Camera4.enabled = camera == TakedownCamera.Camera4;
		}
	}

	private void PositionTakedown(GameObject enemy)
	{
		float num = 2f;
		float num2 = 1f;
		Vector3 position = enemy.transform.position;
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		bool flag4 = true;
		RaycastHit hitInfo;
		if (Physics.Raycast(position, enemy.transform.forward, out hitInfo, num, 257))
		{
			flag = false;
		}
		RaycastHit hitInfo2;
		if (Physics.Raycast(position, -enemy.transform.forward, out hitInfo2, num, 257))
		{
			flag2 = false;
		}
		RaycastHit hitInfo3;
		if (Physics.Raycast(position, enemy.transform.right, out hitInfo3, num2, 257))
		{
			flag3 = false;
		}
		RaycastHit hitInfo4;
		if (Physics.Raycast(position, -enemy.transform.right, out hitInfo4, num2, 257))
		{
			flag4 = false;
		}
		if (!flag3 && flag4)
		{
			float num3 = 0f - (num2 - Vector3.Distance(position, hitInfo3.point));
			position += enemy.transform.right * num2;
		}
		else if (!flag4 && flag3)
		{
			float num4 = num2 - Vector3.Distance(position, hitInfo4.point);
			position += enemy.transform.right * num2;
		}
		else if (!flag3 && !flag4)
		{
			float num5 = 0f - (num2 - Vector3.Distance(position, hitInfo3.point));
			float num6 = num2 - Vector3.Distance(position, hitInfo4.point);
			position += enemy.transform.right * ((num5 + num6) * 0.5f);
		}
		if (!flag && flag2)
		{
			float num7 = 0f - (num - Vector3.Distance(position, hitInfo.point));
			position += enemy.transform.forward * num7;
		}
		else if (!flag2 && flag)
		{
			float num8 = num - Vector3.Distance(position, hitInfo2.point);
			position += enemy.transform.forward * num8;
		}
		else if (!flag && !flag2)
		{
			float num9 = 0f - (num - Vector3.Distance(position, hitInfo.point));
			float num10 = num - Vector3.Distance(position, hitInfo2.point);
			position += enemy.transform.forward * ((num9 + num10) * 0.5f);
		}
		enemy.transform.position = position;
	}
}
