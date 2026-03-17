using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
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
		Move = 13,
		MoveBackwards = 14,
		Holster = 15,
		Draw = 16,
		CoverFlip = 17,
		CoverDive = 18,
		CoverSlideInner = 19,
		CoverSlideOuter = 20,
		ThrowGrenade = 21,
		ThrowGrenadeUpOver = 22,
		Vault = 23,
		Ladder_EnterBottom = 24,
		Ladder_EnterTop = 25,
		Ladder_ExitBottom = 26,
		Ladder_ExitTop = 27,
		Ladder_ClimbUp = 28,
		Ladder_ClimbDown = 29,
		Ladder_Idle = 30,
		Total = 31
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
		Vaulting = 18,
		Ladder_EnterBottom = 19,
		Ladder_EnterTop = 20,
		Ladder_ExitBottom = 21,
		Ladder_ExitTop = 22,
		Ladder_ClimbUp = 23,
		Ladder_ClimbDown = 24,
		Ladder_Idle = 25,
		Total = 26
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
		Wall = 2,
		Total = 3
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
			List<TakedownAnimationData> list = ((count != TakedownEnemyCount.Single) ? m_TakedownDataDouble : m_TakedownDataSingle);
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

	private CoverAnimationData[,,] m_CoverAnimationsLeft = new CoverAnimationData[2, 7, 31];

	private CoverAnimationData[,,] m_CoverAnimationsRight = new CoverAnimationData[2, 7, 31];

	[HideInInspector]
	public static Vector3 m_DefaultCrouchModelLeftPosition = new Vector3(0.1201786f, -0.3416501f, 0.1390992f);

	[HideInInspector]
	public static Vector3 m_DefaultCrouchModelLeftRotation = new Vector3(354.7026f, 350.5434f, 2.738506f);

	[HideInInspector]
	public static Vector3 m_DefaultCrouchModelRightPosition = new Vector3(-0.1018031f, -0.3580197f, 0.06062237f);

	[HideInInspector]
	public static Vector3 m_DefaultCrouchModelRightRotation = new Vector3(356.7647f, 7.310671f, 357.0506f);

	[HideInInspector]
	public static Vector3 m_DefaultStandModelLeftPosition = new Vector3(0.2486276f, -0.3915758f, 0.2556792f);

	[HideInInspector]
	public static Vector3 m_DefaultStandModelLeftRotation = new Vector3(352.7686f, 349.9556f, 2f);

	[HideInInspector]
	public static Vector3 m_DefaultStandModelRightPosition = new Vector3(-0.4501003f, -0.3020859f, 0.2339182f);

	[HideInInspector]
	public static Vector3 m_DefaultStandModelRightRotation = new Vector3(358.9457f, 5.942993f, 0.2234338f);

	[HideInInspector]
	public static Vector3 m_DefaultUpOverModelLeftPosition = new Vector3(-0.1866937f, -0.5279203f, 0.1564844f);

	[HideInInspector]
	public static Vector3 m_DefaultUpOverModelLeftRotation = new Vector3(356.7148f, 348.948f, 358.0896f);

	[HideInInspector]
	public static Vector3 m_DefaultUpOverModelRightPosition = new Vector3(0.112804f, -0.5175542f, 0.06964448f);

	[HideInInspector]
	public static Vector3 m_DefaultUpOverModelRightRotation = new Vector3(358.2076f, 0.7141621f, 357.6329f);

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

	[HideInInspector]
	public float m_CameraCoverDot;

	public float m_CameraCoverAimLimit;

	public float m_CameraCoverAimLimitStanding;

	[HideInInspector]
	public Collider m_CoverCollider;

	[HideInInspector]
	public bool m_CoverAllowsCornering;

	[HideInInspector]
	public bool m_CoverAllowsVaulting;

	[HideInInspector]
	public Vector3 m_CoverNormal;

	[HideInInspector]
	public Vector3 m_CoverPoint;

	[HideInInspector]
	public Vector3 m_VaultDestination;

	private float m_VaultTimer;

	private float m_VaultTime;

	private GameObject m_LadderObject;

	private Collider m_LadderCollider;

	[HideInInspector]
	public Vector3 m_LadderTapWorldHit;

	[HideInInspector]
	public Vector3 m_LadderTapWorldNormal;

	private float m_LadderCheckDistAbove = 2.8f;

	private float m_LadderCheckDistBelow = 0.1f;

	public GameObject m_LadderAttachPoint;

	private float m_LadderTimer;

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

	private float m_ConcussionGrenadeTime = 5f;

	private float m_ConcussionQuadFadeTime = 3f;

	private float m_ConcussionGrenadeTimer;

	private PackedSprite m_ConcussionQuad;

	private bool m_EMPed;

	public float m_FootstepMaxTime = 0.78f;

	public float m_FootstepMinTime = 0.35f;

	public float m_FootstepTimeCoverStand = 0.73f;

	public float m_FootstepTimeCoverCrouch = 1.13f;

	public float m_FootstepTimeCoverStandBack = 0.73f;

	public float m_FootstepTimeCoverCrouchBack = 0.9f;

	private float m_FootstepTimer;

	private int m_FireTapID = -1;

	private bool m_WantFire;

	public float m_WeaponMoveAnimSpeedMax = 2f;

	public float m_WeaponMoveAnimSpeedMin = 1f;

	[HideInInspector]
	public int m_AfterHolsterWeapon = -1;

	private GrenadeState m_GrenadeState;

	private float m_GrenadeThrowTime;

	private float m_GrenadeThrowTimer;

	private float m_GrenadeReleaseTime;

	public GameObject m_GrenadeAttachPointFirstPerson;

	public GameObject m_GrenadeAttachPointThirdPersonLeft;

	public GameObject m_GrenadeAttachPointThirdPersonRight;

	private GameObject m_LastGrenadeObject;

	[HideInInspector]
	public GrenadeBase m_LastGrenadeScript;

	private Color m_ReticleBaseColor = new Color(0.83f, 0.68f, 0.21f);

	private Color m_ReticleHostileColor = Color.red;

	private Color m_ReticleFriendlyColor = Color.green;

	public float m_TimeBeforeHealthRegen = 5f;

	private float m_HealthRegenTimer;

	public float m_HealthRegenPerSecond = 2f;

	private float m_HealthAccum;

	private float m_DeathTime = 5f;

	private float m_DeathTimer;

	private float m_CurrentEnergy;

	public float m_TimeBeforeEnergyRegen = 5f;

	private float m_EnergyRegenTimer;

	private float m_EnergyRegenTarget;

	private EnergyRegenMode m_EnergyRegenMode = EnergyRegenMode.NotRegenerating;

	private bool m_EnergyUsedThisFrame;

	private int m_EnergyRechargeCapacity;

	private float m_DamageVODelay;

	private TakedownAnimation[,] m_TakedownAnimation = new TakedownAnimation[2, 3];

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

	private Enemy_Base m_TakedownEnemy2;

	private InteractiveObject_BreakableWall m_BreakableWall;

	private TakedownAnimationData m_CurrentAnimationData;

	private bool m_LethalTakedown;

	private bool m_PressedForTakedown;

	private float m_TakedownTimer;

	private WeaponBase.WeaponState m_PreWeaponState;

	private Renderer m_WeaponRenderer;

	private TakedownPositioning m_TakedownPositioning;

	private GameObject m_TakedownModel;

	private bool m_CloakingWasActive;

	private bool m_SeeThroughWallsWasActive;

	private static float m_BreakableWallOffset = -0.9649176f;

	public float GetCurrentEnergy()
	{
		return m_CurrentEnergy;
	}

	public bool AtMaxEnergy()
	{
		return m_CurrentEnergy >= GetMaxEnergy();
	}

	public float GetMaxEnergy()
	{
		Augmentation_Energy augmentation_Energy = (Augmentation_Energy)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Energy);
		return augmentation_Energy.GetEnergyContainerAmount();
	}

	public int GetEnergyRechargeCapacity()
	{
		Augmentation_Energy augmentation_Energy = (Augmentation_Energy)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Energy);
		return augmentation_Energy.GetRechargeCapacity();
	}

	public float GetEnergyRechargeRate()
	{
		Augmentation_Energy augmentation_Energy = (Augmentation_Energy)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Energy);
		return augmentation_Energy.GetEnergyRechargeRate();
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
			SoundManager.TriggerEvent("Play_HUD_Energy_Empty", base.gameObject);
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
					SoundManager.TriggerEvent("Play_HUD_Energy_Empty");
				}
				m_EnergyRegenTimer = m_TimeBeforeEnergyRegen;
				m_EnergyRegenMode = EnergyRegenMode.BeforeRegen;
				m_EnergyRegenTarget = Mathf.Min((int)m_CurrentEnergy + GetEnergyRechargeCapacity(), (int)GetMaxEnergy());
			}
		}
		else
		{
			Globals.m_HUD.DisplayEnergyWarning();
		}
	}

	public void CheckForRechargeCapacityIncrease()
	{
		int energyRechargeCapacity = GetEnergyRechargeCapacity();
		if (m_EnergyRechargeCapacity < energyRechargeCapacity)
		{
			m_EnergyRegenTarget = Mathf.Min((int)m_CurrentEnergy + (energyRechargeCapacity - m_EnergyRechargeCapacity), (int)GetMaxEnergy());
			m_EnergyRechargeCapacity = energyRechargeCapacity;
			if (m_EnergyRegenMode == EnergyRegenMode.NotRegenerating)
			{
				m_EnergyRegenMode = EnergyRegenMode.Regernating;
			}
		}
	}

	public override Vector3 GetChestLocation()
	{
		return base.transform.position + Vector3.up * ((m_Stance != Stance.Stand) ? 0.5f : 1.5f);
	}

	public void GiveAmmo(WeaponType type, int amount)
	{
		int weaponAmmo = Globals.m_Inventory.GetWeaponAmmo((WeaponItemID)Inventory.WeaponTypeToItemID(type));
		Globals.m_Inventory.SetWeaponAmmo((WeaponItemID)Inventory.WeaponTypeToItemID(type), weaponAmmo + amount);
		Globals.m_HUD.SetCurrentAmmo(m_WeaponScript.m_CurrentAmmo, GetAmmoForCurrentWeapon());
	}

	public int GetAmmoForCurrentWeapon()
	{
		return Globals.m_Inventory.GetWeaponAmmo(m_WeaponScript.m_WeaponItemID);
	}

	public bool NoAmmoForCurrentWeapon()
	{
		return Globals.m_Inventory.GetWeaponAmmo(m_WeaponScript.m_WeaponItemID) <= 0 && m_WeaponScript.m_CurrentAmmo <= 0;
	}

	public bool LowAmmoForCurrentWeaponClip()
	{
		return m_WeaponScript.m_LowAmmoThreshold > 0 && m_WeaponScript.m_CurrentAmmo <= m_WeaponScript.m_LowAmmoThreshold;
	}

	public override WeaponType GetEquippedWeaponType()
	{
		if (m_WeaponScript != null)
		{
			return m_WeaponScript.m_WeaponType;
		}
		return WeaponType.None;
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

	public override bool IsInCover()
	{
		return m_CoverState != CoverState.Firing && m_CoverState != CoverState.Outside && m_CoverState != CoverState.CoverDive;
	}

	public override bool ExposedInCover()
	{
		return m_CoverState == CoverState.Firing;
	}

	public override Vector3 GetCoverNormal()
	{
		return m_CoverNormal;
	}

	public override Collider GetCoverCollider()
	{
		return m_CoverCollider;
	}

	public override int GetGlobalLayer()
	{
		return 14;
	}

	public bool IsMoving()
	{
		return m_MovementScript.m_IsMoving;
	}

	public float GetNormalizedSpeed()
	{
		return m_MovementScript.m_NormalizedSpeed;
	}

	private void Awake()
	{
		Globals.m_PlayerController = this;
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			m_InputDevice = InputDevice.Touch;
		}
		GameObject gameObject = GameObject.Find("dir_sunlight");
		if (gameObject != null)
		{
			gameObject.transform.parent = base.transform;
		}
		m_MovementScript = GetComponent<PlayerMovement>();
		m_CurrentCamera = m_Camera.GetComponent<Camera>();
		m_PlayerDamage = GetComponent<PlayerDamage>();
		m_FirstPersonWeaponCameraComponent = m_FirstPersonWeaponCamera.GetComponent<Camera>();
		Augmentation_Energy augmentation_Energy = (Augmentation_Energy)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Energy);
		m_CurrentEnergy = augmentation_Energy.GetEnergyContainerAmount();
		m_EnergyRechargeCapacity = augmentation_Energy.GetRechargeCapacity();
		SetupAnimList();
		SetupTakedowns();
		GameManager.LoadHUD();
	}

	protected override void Start()
	{
		base.Start();
		m_DamageModifiers[0] = 1f;
		m_DamageModifiers[1] = 1f;
		m_DamageModifiers[2] = 1f;
		m_DamageModifiers[3] = 0f;
		m_DamageModifiers[4] = 0f;
		m_DamageModifiers[6] = 1f;
		m_DamageModifiers[5] = 1f;
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
		SetWeapon(Globals.m_Inventory.m_ActiveWeaponQuickslot, false);
		Globals.m_HUD.SetWeaponIcon();
		MobileBloom component = m_CurrentCamera.GetComponent<MobileBloom>();
		if ((bool)component)
		{
			component.enabled = Globals.m_Bloom;
		}
		m_ConcussionQuad = GameManager.CreateFullscreenQuad(GameManager.FullscreenQuadType.Add, 0.4f);
		m_ConcussionQuad.gameObject.active = false;
		SoundManager.SetSoundSwitch("MATERIAL", "CONCRETE", base.gameObject);
		HostilityZone.ClearHostilityLevels();
		GameManager.OnPause += OnPause;
		GameManager.OnSceneLoad += OnSceneLoad;
	}

	private void OnDestroy()
	{
		GameManager.OnPause -= OnPause;
		GameManager.OnSceneLoad -= OnSceneLoad;
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
				Globals.m_HUD.Display(true, true, false);
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
			HostilityZone.ClearHostilityLevels();
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
			Globals.m_HUD.Display(false, false, false);
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
		HostilityZone.ClearHostilityLevels();
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
			m_DeathTimer -= Time.deltaTime;
			if (m_DeathTimer <= 0f)
			{
				GameManager.GameLoaded();
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
					BeginTakedown(true, m_PossibleEnemyTarget, null);
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
				if (!m_LastGrenadeScript.m_Mine)
				{
					m_LastGrenadeObject.layer = 18;
				}
				else
				{
					m_LastGrenadeObject.layer = 16;
				}
				m_LastGrenadeObject.transform.parent = null;
				if (m_TargetedEnemy != null && !m_LastGrenadeScript.m_Mine)
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
					SoundManager.TriggerEvent("Play_Footsteps_1st", base.gameObject);
					if (m_Stance == Stance.Stand && m_CoverState == CoverState.Outside)
					{
						Augmentation_Movement augmentation_Movement = (Augmentation_Movement)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Movement);
						float num3 = augmentation_Movement.GetMoveSilentlyNoiseRadius();
						Globals.m_AIDirector.CheckAudioSenses(base.transform.position, num3 * m_MovementScript.m_NormalizedSpeed, DisturbanceEvent.MinorAudio, false);
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
				Globals.m_CameraController.MagnetizeTowardsTarget(m_TargetedEnemy, 70f, m_TargetedEnemyScript, false);
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
						SoundManager.TriggerEvent("Play_HUD_Health_Full", base.gameObject);
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
					m_CurrentEnergy += Time.deltaTime * GetEnergyRechargeRate();
					if (m_CurrentEnergy >= m_EnergyRegenTarget)
					{
						EnergyRegenComplete();
					}
					break;
				}
			}
			Globals.m_HUD.SetCurrentEnergy(m_CurrentEnergy, m_EnergyUsedThisFrame);
			m_EnergyUsedThisFrame = false;
		}
		if (m_ConcussionQuad.gameObject.active)
		{
			m_ConcussionGrenadeTimer -= Time.deltaTime;
			if (m_ConcussionGrenadeTimer < m_ConcussionQuadFadeTime)
			{
				SoundManager.TriggerEvent("Stop_Concussion", base.gameObject);
				Color color = m_ConcussionQuad.Color;
				color.a = 1f - (m_ConcussionQuadFadeTime - m_ConcussionGrenadeTimer) / m_ConcussionQuadFadeTime;
				m_ConcussionQuad.Color = color;
				if (m_EMPed && !Globals.m_HUD.m_Showing)
				{
					Globals.m_HUD.Display(true, true, false);
				}
			}
			if (m_ConcussionGrenadeTimer <= 0f)
			{
				m_ConcussionQuad.gameObject.active = false;
			}
		}
		m_DamageVODelay -= Time.deltaTime;
	}

	private void LateUpdate()
	{
		switch (m_CoverState)
		{
		case CoverState.Ladder_EnterBottom:
		case CoverState.Ladder_EnterTop:
		case CoverState.Ladder_ExitBottom:
		case CoverState.Ladder_ExitTop:
		case CoverState.Ladder_ClimbUp:
		case CoverState.Ladder_ClimbDown:
		case CoverState.Ladder_Idle:
			m_Camera.transform.position = new Vector3(m_Camera.transform.position.x, m_LadderAttachPoint.transform.position.y, m_Camera.transform.position.z);
			break;
		}
	}

	private void EnergyRegenComplete()
	{
		m_EnergyRegenMode = EnergyRegenMode.NotRegenerating;
		m_CurrentEnergy = m_EnergyRegenTarget;
		SoundManager.TriggerEvent("Play_HUD_Energy_Full", base.gameObject);
	}

	private void SetGrenadeState(GrenadeState newstate)
	{
		m_GrenadeState = newstate;
		switch (newstate)
		{
		case GrenadeState.Holstering:
			m_LastGrenadeObject.SetActiveRecursively(false);
			break;
		case GrenadeState.Throwing:
			m_LastGrenadeObject.SetActiveRecursively(true);
			SoundManager.TriggerEvent("Play_VO_PC_Throw", base.gameObject);
			break;
		case GrenadeState.Drawing:
			break;
		case GrenadeState.None:
			m_WeaponScript.m_ModelFirstPerson.SetActiveRecursively(true);
			break;
		}
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
		bool armsonly = false;
		bool force = false;
		if (m_GrenadeState != GrenadeState.None)
		{
			switch (m_GrenadeState)
			{
			case GrenadeState.Holstering:
				anim = WeaponBase.FirstPersonAnimation.Holster;
				m_WeaponScript.m_FirstPersonAnimationSpeed = 2f;
				if (!m_ModelFirstPerson.animation.isPlaying || m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Holstered)
				{
					SetGrenadeState(GrenadeState.Throwing);
					anim = WeaponBase.FirstPersonAnimation.ThrowGrenade;
					force = true;
				}
				break;
			case GrenadeState.Throwing:
				armsonly = true;
				m_WeaponScript.m_ModelFirstPerson.SetActiveRecursively(false);
				anim = WeaponBase.FirstPersonAnimation.ThrowGrenade;
				if (!m_ModelFirstPerson.animation.isPlaying)
				{
					if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Holstered)
					{
						SetGrenadeState(GrenadeState.None);
						break;
					}
					SetGrenadeState(GrenadeState.Drawing);
					anim = WeaponBase.FirstPersonAnimation.Draw;
				}
				break;
			case GrenadeState.Drawing:
				m_WeaponScript.m_ModelFirstPerson.SetActiveRecursively(true);
				anim = WeaponBase.FirstPersonAnimation.Draw;
				m_WeaponScript.m_FirstPersonAnimationSpeed = 2f;
				if (!m_ModelFirstPerson.animation.isPlaying)
				{
					SetGrenadeState(GrenadeState.None);
				}
				break;
			}
		}
		if (m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Holstered || m_GrenadeState != GrenadeState.None)
		{
			m_WeaponScript.SetFirstPersonAnim(anim, force, armsonly);
		}
		else if (m_AfterHolsterWeapon != -1)
		{
			DestroyCurrentWeapon();
			SetWeapon(m_AfterHolsterWeapon, false);
			m_AfterHolsterWeapon = -1;
			Globals.m_HUD.SetWeaponIcon();
		}
	}

	private void GetDesiredCameraPosition(out float desiredcamshift, out float desiredcamzoom, out float shiftspeed)
	{
		desiredcamshift = 0f;
		desiredcamzoom = Globals.m_CameraController.m_CameraDistThirdPerson;
		shiftspeed = m_CameraShiftSpeed;
		switch (m_CoverEdge)
		{
		case CoverEdge.Left:
			desiredcamshift = 0f - m_CoverEdgeCameraShiftAmount;
			break;
		case CoverEdge.Right:
			desiredcamshift = m_CoverEdgeCameraShiftAmount;
			break;
		case CoverEdge.Both:
			switch (m_CoverSide)
			{
			case CoverSide.Left:
				desiredcamshift = 0f - m_CoverEdgeCameraShiftAmount;
				break;
			case CoverSide.Right:
				desiredcamshift = m_CoverEdgeCameraShiftAmount;
				break;
			}
			break;
		case CoverEdge.None:
			switch (m_CoverState)
			{
			case CoverState.CoverFlip:
				desiredcamshift = 0f - m_CoverFlipPlane.GetDistanceToPoint(base.transform.position);
				if (m_CoverSide == CoverSide.Right)
				{
					desiredcamshift = 0f - desiredcamshift;
				}
				shiftspeed = 10f;
				break;
			case CoverState.CoverDive:
			{
				switch (m_CoverSide)
				{
				case CoverSide.Left:
					desiredcamshift = m_CoverEdgeCameraShiftAmount;
					break;
				case CoverSide.Right:
					desiredcamshift = 0f - m_CoverEdgeCameraShiftAmount;
					break;
				}
				float num = m_CoverFlipTimer / (m_CoverFlipTime * m_CoverDiveMoveStop) * 2f;
				if (num > 2f)
				{
					num = 2f;
				}
				if (num > 1f)
				{
					num = 1f - (num - 1f);
				}
				desiredcamzoom += m_CoverDiveCameraZoom * num;
				break;
			}
			case CoverState.Ladder_ExitTop:
				desiredcamzoom = Globals.m_CameraController.m_CameraDistThirdPerson - 0.3f - 1f * m_LadderTimer;
				break;
			case CoverState.Ladder_ExitBottom:
				desiredcamzoom = Globals.m_CameraController.m_CameraDistThirdPerson - 0.3f - 0.2f * m_LadderTimer;
				break;
			case CoverState.Ladder_EnterTop:
			case CoverState.Ladder_ClimbUp:
			case CoverState.Ladder_ClimbDown:
			case CoverState.Ladder_Idle:
				desiredcamzoom = Globals.m_CameraController.m_CameraDistThirdPerson - 0.3f;
				break;
			case CoverState.CoverSlideInner:
			case CoverState.CoverSlideOuter:
			case CoverState.ThrowGrenade:
			case CoverState.Vaulting:
			case CoverState.Ladder_EnterBottom:
				break;
			}
			break;
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
				SoundManager.TriggerEvent(coverAnimationSoundEvent.name, base.gameObject);
				m_PendingSoundEvents.Remove(coverAnimationSoundEvent);
			}
		}
		m_WeaponScript.SetFirstPersonAnim(WeaponBase.FirstPersonAnimation.None, false, false);
		m_LadderTimer += Time.deltaTime;
		float desiredcamshift;
		float desiredcamzoom;
		float shiftspeed;
		GetDesiredCameraPosition(out desiredcamshift, out desiredcamzoom, out shiftspeed);
		m_CurrentCameraShift += (desiredcamshift - m_CurrentCameraShift) * shiftspeed * Time.deltaTime;
		m_CurrentCamera.transform.localPosition = new Vector3(m_CurrentCameraShift, m_CurrentCamera.transform.localPosition.y, 0f - desiredcamzoom);
		if (m_CoverState == CoverState.Vaulting)
		{
			float num2 = 1f - m_VaultTimer / m_VaultTime;
			float num3 = 1.4f;
			m_CurrentCamera.transform.Translate(base.transform.forward * num2 * num3, Space.World);
		}
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
			float num4 = Globals.m_CameraController.GetPitch() / 90f;
			Vector3 thirdPersonAttachOriginalPos = Globals.m_CameraController.m_ThirdPersonAttachOriginalPos;
			if (num4 > 0f)
			{
				thirdPersonAttachOriginalPos.y += num4 * m_UpAndOverCameraHeight;
			}
			Globals.m_CameraController.m_ThirdPersonAttach.transform.localPosition = thirdPersonAttachOriginalPos;
		}
		else
		{
			Globals.m_CameraController.m_ThirdPersonAttach.transform.localPosition = Globals.m_CameraController.m_ThirdPersonAttachOriginalPos;
		}
		float num5 = m_CameraCoverAimLimit;
		if (m_WantFire && m_CoverEdge == CoverEdge.None && !m_AllowUpAndOverCoverFire)
		{
			num5 = m_CameraCoverAimLimitStanding;
		}
		switch (m_CoverState)
		{
		case CoverState.TransitioningIn:
		case CoverState.SwitchingSides:
		case CoverState.Drawing:
			if (m_CurrentCoverAnimationState.time >= m_CurrentCoverAnimationState.length - 0.2f || !m_CurrentCoverAnimationState.enabled)
			{
				SetCoverState(CoverState.Inside);
				UpdateCoverEdge(false);
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
			float num6 = 1f;
			if (m_CoverSide == CoverSide.Left)
			{
				num6 = -1f;
			}
			base.transform.Rotate(0f, 90f / m_CoverSlideTime * Time.deltaTime * num6, 0f);
			Globals.m_CameraController.RotateYaw(90f / m_CoverSlideTime * Time.deltaTime * num6);
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
			float num7 = 1f;
			if (m_CoverSide == CoverSide.Left)
			{
				num7 = -1f;
			}
			base.transform.Rotate(0f, -45f / m_CoverSlideTime * Time.deltaTime * num7, 0f);
			Globals.m_CameraController.RotateYaw(m_CoverSlideRotationAmount * 0.5f / m_CoverSlideTime * Time.deltaTime * num7);
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
			if (!m_ModelThirdPerson.animation.isPlaying)
			{
				SetCoverState(CoverState.Inside);
			}
			break;
		case CoverState.Reloading:
			if (m_CameraCoverDot < num5)
			{
				ExitCover(false);
			}
			else if (m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Reloading)
			{
				SetCoverState(CoverState.Inside);
			}
			break;
		case CoverState.Inside:
		{
			if (m_CameraCoverDot < num5)
			{
				ExitCover(false);
				if (m_WantFire)
				{
					StartFiring();
				}
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
			if (m_CoverEdge != CoverEdge.None)
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
				m_WeaponScript.StartFire();
				if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Firing)
				{
					SetCoverState(CoverState.Firing);
				}
				else
				{
					SetCoverState(CoverState.CoverAiming);
				}
				if (m_OneCoverShot)
				{
					StopFiringAfterOneShot();
				}
			}
			break;
		case CoverState.Firing:
			if (m_CameraCoverDot < num5)
			{
				if (m_WeaponScript.m_FireType == WeaponBase.FireType.RapidFire)
				{
					StopFiring();
				}
				ExitCover(true);
				if (m_WeaponScript.m_FireType == WeaponBase.FireType.RapidFire)
				{
					StartFiring();
				}
				break;
			}
			if (IsAimingAtWall())
			{
				StopFiringAfterOneShot();
			}
			if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Firing)
			{
				break;
			}
			if (!m_WantFire)
			{
				m_WeaponScript.ResetFOV(false);
				SetCoverState(CoverState.TransitioningFromFire);
				break;
			}
			m_ModelThirdPerson.animation.Stop();
			if (m_CoverEdge != CoverEdge.None)
			{
				PlayCoverAnimation(CoverAnimation.LeanFiring, 0f);
			}
			else
			{
				PlayCoverAnimation(CoverAnimation.UpOverFiring, 0f);
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
			if (m_CameraCoverDot >= num5)
			{
				if (IsAimingAtWall() && m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Firing)
				{
					SetCoverState(CoverState.TransitioningFromFire);
					m_WeaponScript.CancelFire();
					m_WeaponScript.ResetFOV(false);
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
				m_WeaponScript.ResetFOV(false);
			}
			else
			{
				ExitCover(false);
				m_WeaponScript.ResetFOV(false);
			}
			break;
		case CoverState.Holstering:
			if (!m_ModelThirdPerson.animation.isPlaying)
			{
				if (m_AfterHolsterWeapon != -1)
				{
					DestroyCurrentWeapon();
					SetWeapon(m_AfterHolsterWeapon, false);
					m_AfterHolsterWeapon = -1;
					Globals.m_HUD.SetWeaponIcon();
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
			if (m_CameraCoverDot < num5)
			{
				ExitCover(true);
			}
			else if (!m_ModelThirdPerson.animation.isPlaying)
			{
				SetCoverState(CoverState.Inside);
				m_GrenadeState = GrenadeState.None;
			}
			break;
		case CoverState.Vaulting:
			m_VaultTimer -= Time.deltaTime;
			if (m_VaultTimer <= 0f)
			{
				ExitCover(true);
				base.transform.position = m_VaultDestination;
			}
			break;
		case CoverState.Ladder_EnterBottom:
			if (!m_ModelThirdPerson.animation.isPlaying)
			{
				SetCoverState(CoverState.Ladder_ClimbUp);
			}
			break;
		case CoverState.Ladder_EnterTop:
			if (!m_ModelThirdPerson.animation.isPlaying)
			{
				SetCoverState(CoverState.Ladder_ClimbDown);
			}
			break;
		case CoverState.Ladder_ClimbUp:
		{
			Ray ray2 = new Ray(base.transform.position + new Vector3(0f, m_LadderCheckDistAbove, 0f), base.transform.forward);
			RaycastHit hitInfo2;
			if (Physics.Raycast(ray2, out hitInfo2, 1.5f, 65536))
			{
				float y2 = base.transform.position.y;
				y2 += Time.deltaTime * 1.5f;
				base.transform.position = new Vector3(base.transform.position.x, y2, base.transform.position.z);
			}
			else
			{
				SetCoverState(CoverState.Ladder_ExitTop);
			}
			break;
		}
		case CoverState.Ladder_ClimbDown:
		{
			Ray ray = new Ray(base.transform.position, Vector3.down);
			RaycastHit hitInfo;
			if (!Physics.Raycast(ray, out hitInfo, m_LadderCheckDistBelow, 257))
			{
				float y = base.transform.position.y;
				y -= Time.deltaTime * 1.5f;
				base.transform.position = new Vector3(base.transform.position.x, y, base.transform.position.z);
			}
			else
			{
				SetCoverState(CoverState.Ladder_ExitBottom);
			}
			break;
		}
		case CoverState.Ladder_ExitBottom:
		case CoverState.Ladder_ExitTop:
			if (!m_ModelThirdPerson.animation.isPlaying)
			{
				ExitLadder();
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

	public bool SetWeapon(int index, bool instant = false)
	{
		if (m_CurrentHealth <= 0)
		{
			return false;
		}
		if (m_CurrentWeapon != null && index == Globals.m_Inventory.m_ActiveWeaponQuickslot)
		{
			ToggleWeaponHolstered();
			return true;
		}
		if (m_CurrentWeapon != null)
		{
			if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Holstered)
			{
				DestroyCurrentWeapon();
			}
			else
			{
				m_AfterHolsterWeapon = index;
				m_WeaponScript.ToggleHostered();
				if (m_CameraMode == CameraMode.Third)
				{
					SetCoverState(CoverState.Holstering);
				}
			}
		}
		if (m_CurrentWeapon == null)
		{
			int itemID = Globals.m_Inventory.m_WeaponQuickslots[index].m_ItemID;
			int num = Inventory.ItemIDToWeaponType((WeaponItemID)itemID);
			m_CurrentWeapon = (GameObject)UnityEngine.Object.Instantiate(m_WeaponList[num], base.transform.position, base.transform.rotation);
			m_CurrentWeapon.transform.parent = base.transform;
			m_WeaponScript = m_CurrentWeapon.GetComponent<WeaponBase>();
			m_WeaponScript.m_CurrentAmmo = (Globals.m_Inventory.m_Items[1][(int)m_WeaponScript.m_WeaponItemID] as Item_Weapon).m_CurrentAmmoInClip;
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
				if (!instant)
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
				if (!instant)
				{
					SetCoverState(CoverState.Drawing);
				}
			}
			if (Globals.m_AugmentCloaking.enabled)
			{
				Globals.m_AugmentCloaking.PlayerWeaponSwitched();
			}
		}
		return true;
	}

	public void DestroyCurrentWeapon()
	{
		(Globals.m_Inventory.m_Items[1][(int)m_WeaponScript.m_WeaponItemID] as Item_Weapon).m_CurrentAmmoInClip = m_WeaponScript.m_CurrentAmmo;
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
			SoundManager.TriggerEvent("Play_Weapon_Handle", base.gameObject);
			m_ModelThirdPerson.transform.parent = Globals.m_CameraController.m_ThirdPersonAttach.transform;
			m_ModelRotateCoverTimer = m_ModelRotateCoverTime;
			m_OriginalCoverRotation = m_ModelThirdPerson.transform.localRotation;
			m_OriginalCoverPosition = m_ModelThirdPerson.transform.localPosition;
			switch (m_CoverFireSide)
			{
			case CoverFireSide.Left:
				if (m_Stance == Stance.Crouch)
				{
					m_TargetCoverRotation = Quaternion.Euler(m_WeaponScript.m_CrouchModelLeftRotation);
					m_TargetCoverPosition = m_WeaponScript.m_CrouchModelLeftPosition;
				}
				else
				{
					m_TargetCoverRotation = Quaternion.Euler(m_WeaponScript.m_StandModelLeftRotation);
					m_TargetCoverPosition = m_WeaponScript.m_StandModelLeftPosition;
				}
				break;
			case CoverFireSide.Right:
				if (m_Stance == Stance.Crouch)
				{
					m_TargetCoverRotation = Quaternion.Euler(m_WeaponScript.m_CrouchModelRightRotation);
					m_TargetCoverPosition = m_WeaponScript.m_CrouchModelRightPosition;
				}
				else
				{
					m_TargetCoverRotation = Quaternion.Euler(m_WeaponScript.m_StandModelRightRotation);
					m_TargetCoverPosition = m_WeaponScript.m_StandModelRightPosition;
				}
				break;
			case CoverFireSide.UpOver:
				switch (m_CoverSide)
				{
				case CoverSide.Left:
					m_TargetCoverRotation = Quaternion.Euler(m_WeaponScript.m_UpOverModelLeftRotation);
					m_TargetCoverPosition = m_WeaponScript.m_UpOverModelLeftPosition;
					break;
				case CoverSide.Right:
					m_TargetCoverRotation = Quaternion.Euler(m_WeaponScript.m_UpOverModelRightRotation);
					m_TargetCoverPosition = m_WeaponScript.m_UpOverModelRightPosition;
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
			m_MovementScript.CancelMovement();
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
			if (m_CoverEdge != CoverEdge.None)
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
		case CoverState.Vaulting:
			m_VaultTime = PlayCoverAnimation(CoverAnimation.Vault, 0f);
			m_VaultTimer = m_VaultTime;
			m_CoverEdge = CoverEdge.None;
			break;
		case CoverState.Ladder_EnterBottom:
			m_LadderTimer = 0f;
			m_MovementScript.enabled = false;
			PlayCoverAnimation(CoverAnimation.Ladder_EnterBottom, 0f);
			break;
		case CoverState.Ladder_EnterTop:
			m_LadderTimer = 0f;
			m_MovementScript.enabled = false;
			PlayCoverAnimation(CoverAnimation.Ladder_EnterTop, 0f);
			break;
		case CoverState.Ladder_ClimbUp:
			m_LadderTimer = 0f;
			PlayCoverAnimation(CoverAnimation.Ladder_ClimbUp, 0f);
			break;
		case CoverState.Ladder_ClimbDown:
			m_LadderTimer = 0f;
			PlayCoverAnimation(CoverAnimation.Ladder_ClimbDown, 0f);
			break;
		case CoverState.Ladder_ExitTop:
			m_LadderTimer = 0f;
			PlayCoverAnimation(CoverAnimation.Ladder_ExitTop, 0f);
			break;
		case CoverState.Ladder_ExitBottom:
			m_LadderTimer = 0f;
			PlayCoverAnimation(CoverAnimation.Ladder_ExitBottom, 0f);
			break;
		}
	}

	private float PlayCoverAnimation(CoverAnimation anim, float starttime = 0f)
	{
		CoverAnimationData coverAnimationData = ((m_CoverSide != CoverSide.Left) ? m_CoverAnimationsRight[(int)m_Stance, (int)m_WeaponScript.m_WeaponAnimSet, (int)anim] : m_CoverAnimationsLeft[(int)m_Stance, (int)m_WeaponScript.m_WeaponAnimSet, (int)anim]);
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
					SoundManager.TriggerEvent(coverAnimationSoundEvent.name, base.gameObject);
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
		CoverAnimationData coverAnimationData = ((m_CoverSide != CoverSide.Left) ? m_CoverAnimationsRight[(int)m_Stance, (int)m_WeaponScript.m_WeaponAnimSet, (int)anim] : m_CoverAnimationsLeft[(int)m_Stance, (int)m_WeaponScript.m_WeaponAnimSet, (int)anim]);
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
						CharacterBase characterBase = Globals.FindCharacterBase(hitInfo2.collider.transform);
						if (characterBase != null)
						{
							m_PossibleEnemyTarget = characterBase.gameObject;
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
		if (flag && m_TapTime[ptr.id] < Globals.m_TapTimeLimit)
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
				m_LadderTapWorldHit = hitInfo.point;
				m_LadderTapWorldNormal = hitInfo.normal;
				if ((bool)component)
				{
					m_TappedInteractiveObject = component.InteractWithObject(false);
				}
				else
				{
					Debug.Log(hitInfo.collider.gameObject.name + " has a layer of Interactive Object but doesn't have an InteractiveObject component.");
				}
				if (m_TappedInteractiveObject && component is InteractiveObject_Ladder)
				{
					m_LadderCollider = hitInfo.collider;
					m_LadderObject = hitInfo.collider.gameObject;
					EnterLadder();
				}
			}
		}
		m_MovementScript.ButtonRelease(ptr.devicePos, ptr.id);
		m_TappedInteractiveObject = false;
		Globals.m_CameraController.ButtonRelease(ptr.devicePos, ptr.id);
		if (!(m_PossibleEnemyTarget != null) || ptr.id != m_EnemyTapID)
		{
			return;
		}
		if (m_TapTime[m_EnemyTapID] <= Globals.m_TapTimeLimit)
		{
			if (m_TargetedEnemy != m_PossibleEnemyTarget)
			{
				m_TargetedEnemy = m_PossibleEnemyTarget;
				m_TargetedEnemyScript = m_TargetedEnemy.GetComponent<CharacterBase>();
				SoundManager.TriggerEvent("Play_HUD_Target_Lock", base.gameObject);
			}
			else
			{
				m_TargetedEnemy = null;
				SoundManager.TriggerEvent("Play_HUD_Target_Unlock", base.gameObject);
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

	public void ToggleSeeThroughWalls()
	{
		if (!Globals.m_AugmentSeeThroughWalls.enabled)
		{
			Globals.m_AugmentSeeThroughWalls.Enable();
		}
		else
		{
			Globals.m_AugmentSeeThroughWalls.Disable();
		}
	}

	public void StanceButtonTapped()
	{
		if (m_CurrentHealth <= 0 || (m_CoverState != CoverState.Outside && m_CoverState != CoverState.Inside && m_CoverState != CoverState.CoverAiming) || (m_CameraMode == CameraMode.Third && m_Stance == Stance.Crouch && m_AllowUpAndOverCoverFire))
		{
			return;
		}
		if (m_Stance == Stance.Stand)
		{
			if (m_CameraMode == CameraMode.Third && m_CoverState != CoverState.CoverAiming)
			{
				SetCoverState(CoverState.Crouch);
			}
			SetStance(Stance.Crouch, false);
		}
		else
		{
			if (m_CameraMode == CameraMode.Third && m_CoverState != CoverState.CoverAiming)
			{
				SetCoverState(CoverState.Stand);
			}
			SetStance(Stance.Stand, false);
		}
	}

	public void VaultTapped()
	{
		SetCoverState(CoverState.Vaulting);
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

	private void SetStance(Stance stance, bool instant = false)
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
		if (!instant)
		{
			if (m_Stance == Stance.Crouch)
			{
				SoundManager.TriggerEvent("Set_Volume_Footsteps_Stealth", base.gameObject);
			}
			else
			{
				SoundManager.TriggerEvent("Reset_Volume_Footsteps_Stealth", base.gameObject);
			}
		}
		else
		{
			m_Camera.transform.localPosition = new Vector3(m_Camera.transform.localPosition.x, GetCameraHeight(), m_Camera.transform.localPosition.z);
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
			if (m_WeaponScript.m_CurrentAmmo < (Globals.m_Inventory.m_Items[1][(int)m_WeaponScript.m_WeaponItemID] as Item_Weapon).m_AmmoPerClip && GetAmmoForCurrentWeapon() > 0)
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
				EnterCover(m_AutoCoverPosition, m_AutoCoverNormal, m_AutoCoverCollider, false, false);
			}
			else if (CanExitCover())
			{
				ExitCover(false);
			}
		}
	}

	public void ArmorButtonTapped()
	{
		if (m_CurrentHealth > 0)
		{
			Globals.m_AugmentArmor.Activate();
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

	private void ListLoadedAnimations()
	{
		Resources.UnloadUnusedAssets();
		AnimationClip[] array = Resources.FindObjectsOfTypeAll(typeof(AnimationClip)) as AnimationClip[];
		string empty = string.Empty;
		empty += "Dumping loaded animations:\n";
		int num = 0;
		int num2 = 1;
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i].name == string.Empty))
			{
				string text = empty;
				empty = text + num2 + " [" + Profiler.GetRuntimeMemorySize(array[i]) / 1024 + "k]" + array[i].name + " (" + array[i].length + ")\n";
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

	private void ListLoadedTextures()
	{
		Resources.UnloadUnusedAssets();
		Texture[] array = Resources.FindObjectsOfTypeAll(typeof(Texture)) as Texture[];
		for (int i = 1; i < array.Length; i++)
		{
			for (int j = 0; j < array.Length - 1; j++)
			{
				if (Profiler.GetRuntimeMemorySize(array[j]) < Profiler.GetRuntimeMemorySize(array[j + 1]))
				{
					Texture texture = array[j];
					array[j] = array[j + 1];
					array[j + 1] = texture;
				}
			}
		}
		string empty = string.Empty;
		empty += "\nDumping loaded textures:\n\n";
		int num = 0;
		int num2 = 1;
		int num3 = 0;
		for (int k = 0; k < array.Length; k++)
		{
			if (!(array[k].name == string.Empty))
			{
				string text = empty;
				empty = text + num2 + " [" + Profiler.GetRuntimeMemorySize(array[k]) + "]" + array[k].name + " (" + array[k].width + ", " + array[k].height + ")\n";
				num++;
				num2++;
				num3 += Profiler.GetRuntimeMemorySize(array[k]);
				if (num >= 500)
				{
					Debug.Log(empty);
					empty = string.Empty;
					num = 0;
				}
			}
		}
		empty = empty + "\nTOTAL SIZE IN RAM: " + num3 + "\n\n";
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
		Globals.m_HUD.UserReleased(position, id);
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
		if (m_CoverState != CoverState.Outside)
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
			Debug.LogError("WHOA HOW DID THIS HAPPEN ALERT JASON AND TELL HIM WHAT YOU DID");
			break;
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

	public void StopFiringAfterOneShot()
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

	private void UpdateCoverEdge(bool instant = false)
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
		if (instant)
		{
			float desiredcamshift;
			float desiredcamzoom;
			float shiftspeed;
			GetDesiredCameraPosition(out desiredcamshift, out desiredcamzoom, out shiftspeed);
			m_CurrentCamera.transform.localPosition = new Vector3(desiredcamshift, m_CurrentCamera.transform.localPosition.y, 0f - desiredcamzoom);
			m_CurrentCameraShift = desiredcamshift;
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
		UpdateCoverEdge(false);
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
			SetStance(Stance.Crouch, false);
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

	public void EnterCover(Vector3 position, Vector3 normal, Collider collider, bool tap = false, bool instant = false)
	{
		if (!CanEnterCover())
		{
			return;
		}
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
		Globals.m_CameraController.LookAt(vector, true);
		SetThirdPerson();
		Globals.m_CameraController.SetRotation(yaw, pitch);
		Vector3 forward2 = m_Camera.transform.forward;
		forward2.y = 0f;
		forward2.Normalize();
		m_CameraCoverDot = Vector3.Dot(base.transform.forward, forward2);
		UpdateCoverEdge(false);
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
			SetStance(Stance.Crouch, false);
		}
		if (m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Reloading)
		{
			if (!instant)
			{
				SetCoverState(CoverState.TransitioningIn);
			}
			else
			{
				SetCoverState(CoverState.Inside);
			}
		}
		else
		{
			SetCoverState(CoverState.Reloading);
		}
		Globals.m_HUD.TurnOnCoverButton(false);
		UpdateCoverInternalInfo(vector, normal, collider);
		m_ForceCoverButtonInactive = false;
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
			m_CoverAllowsVaulting = true;
			DisableVaulting component2 = collider.gameObject.GetComponent<DisableVaulting>();
			if (component2 != null)
			{
				m_CoverAllowsVaulting = false;
			}
		}
		m_CoverPoint = wallPos;
		m_CoverNormal = normal;
		m_CoverCollider = collider;
	}

	public void ExitCover(bool force = false)
	{
		if (!CanExitCover() && !force)
		{
			return;
		}
		m_ForceCoverButtonInactive = false;
		m_MovementScript.CancelTapToMove();
		SetCoverState(CoverState.Outside);
		m_CurrentCameraShift = 0f;
		m_CurrentCamera.transform.parent.transform.localPosition = new Vector3(m_CurrentCameraShift, m_CurrentCamera.transform.parent.transform.localPosition.y, m_CurrentCamera.transform.parent.transform.localPosition.z);
		m_ModelThirdPerson.transform.parent = base.transform;
		m_ModelThirdPerson.transform.localRotation = Quaternion.identity;
		m_ModelThirdPerson.transform.localPosition = Vector3.zero;
		m_WeaponScript.ResetFOV(false);
		SetFirstPerson();
		if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Reloading)
		{
			m_WeaponScript.m_ReloadTime = m_WeaponScript.GetFirstPersonAnimLength(WeaponBase.FirstPersonAnimation.Reload);
			if (m_WeaponScript.m_ReloadTimer > m_WeaponScript.m_ReloadTime)
			{
				m_WeaponScript.m_ReloadTimer = m_WeaponScript.m_ReloadTime;
			}
		}
		if (m_GrenadeState != GrenadeState.None)
		{
			m_LastGrenadeObject.SetActiveRecursively(true);
		}
		Globals.m_HUD.TurnOnCoverButton(false);
	}

	public void EnterLadder()
	{
		StopFiring();
		Globals.m_HUD.Display(false, true, false);
		float y = base.transform.position.y;
		Vector3 position = m_LadderObject.transform.position;
		position += m_LadderObject.transform.forward * 0.5f;
		if (y < m_LadderCollider.bounds.center.y)
		{
			position.y = GetGroundHeight(position);
		}
		else
		{
			position.y = base.transform.position.y - 2f;
		}
		base.transform.position = position;
		Vector3 point = position - m_LadderObject.transform.forward;
		point.y = m_CurrentCamera.gameObject.transform.position.y;
		Globals.m_CameraController.LookAt(point, false);
		SetThirdPerson();
		m_WeaponScript.m_ModelThirdPersonPlayer.SetActiveRecursively(false);
		if (y < m_LadderCollider.bounds.center.y)
		{
			SetCoverState(CoverState.Ladder_EnterBottom);
		}
		else
		{
			SetCoverState(CoverState.Ladder_EnterTop);
		}
		m_CoverEdge = CoverEdge.None;
		Globals.m_HUD.TurnOnCoverButton(false);
	}

	public void ExitLadder()
	{
		Vector3 position = base.transform.position;
		if (m_CoverState == CoverState.Ladder_ExitTop)
		{
			position.y += m_LadderCheckDistAbove;
			position += base.transform.forward * 1.5f;
		}
		else
		{
			position.y = GetGroundHeight(position) + 0.1f;
		}
		base.transform.position = position;
		Globals.m_HUD.Display(true, true, false);
		m_MovementScript.enabled = true;
		SetCoverState(CoverState.Outside);
		m_WeaponScript.m_ModelThirdPersonPlayer.SetActiveRecursively(true);
		SetFirstPerson();
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

	public void WeaponHolster(bool holster)
	{
		if (holster)
		{
			if (m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Holstered && m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Holstering)
			{
				ToggleWeaponHolstered();
			}
		}
		else if (m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Holstered)
		{
			ToggleWeaponHolstered();
		}
	}

	public bool IsWeaponHolstered()
	{
		if (m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Holstered && m_WeaponScript.m_WeaponState != WeaponBase.WeaponState.Holstering)
		{
			return false;
		}
		return true;
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
		if (GetAmmoForCurrentWeapon() > 0)
		{
			ReloadWeapon();
		}
	}

	public override void WeaponDoneReloading()
	{
		Globals.m_HUD.SetCurrentAmmo(m_WeaponScript.m_CurrentAmmo, GetAmmoForCurrentWeapon());
		if (m_WantFire)
		{
			StartFiring();
		}
	}

	public override void HitByTranquilizer()
	{
		base.HitByTranquilizer();
	}

	public override bool TakeDamage(DamageData data)
	{
		bool result = false;
		switch (data.m_DamageType)
		{
		case DamageType.Concussion:
		{
			SoundManager.TriggerEvent("Start_Concussion", base.gameObject);
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
			SoundManager.TriggerEvent("Start_Concussion", base.gameObject);
			m_ConcussionQuad.gameObject.active = true;
			Color color = m_ConcussionQuad.Color;
			color.a = 1f;
			m_ConcussionQuad.Color = color;
			m_ConcussionGrenadeTimer = m_ConcussionGrenadeTime;
			Globals.m_HUD.Display(false, true, false);
			m_EMPed = true;
			break;
		}
		default:
			Globals.m_AugmentArmor.ApplyArmorDampening(ref data.m_Damage);
			if (Globals.m_GodMode && m_CurrentHealth - data.m_Damage <= 0)
			{
				data.m_Damage = 0;
			}
			result = base.TakeDamage(data);
			m_PlayerDamage.TakeDamage(data.m_SourceLocation);
			if (m_DamageVODelay <= 0f && (double)UnityEngine.Random.value <= 0.7)
			{
				SoundManager.TriggerEvent("Play_VO_PC_Damage", base.gameObject);
				m_DamageVODelay = 3f;
			}
			m_HealthRegenTimer = m_TimeBeforeHealthRegen;
			break;
		}
		return result;
	}

	public override void Die(DamageData data)
	{
		if (!Globals.m_GodMode)
		{
			CancelMovement();
			StopFiring();
			Globals.m_AugmentCloaking.Disable();
			if (m_CameraMode == CameraMode.First)
			{
				m_ModelFirstPerson.SetActiveRecursively(false);
			}
			SoundManager.TriggerEvent("Play_VO_PC_Death", base.gameObject);
			m_DeathTimer = m_DeathTime;
			HostilityZone.ClearHostilityLevels();
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
		if (Event.current.type != EventType.Repaint || GameManager.IsGamePaused() || !Globals.m_HUD.m_Showing || m_WeaponScript.m_WeaponState == WeaponBase.WeaponState.Reloading)
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
		if (Physics.Raycast(ray, out hitInfo, m_WeaponScript.m_MaxRange, layerMask))
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

	private void OnPause(bool Paused)
	{
		if (Paused && m_WeaponScript != null)
		{
			(Globals.m_Inventory.m_Items[1][(int)m_WeaponScript.m_WeaponItemID] as Item_Weapon).m_CurrentAmmoInClip = m_WeaponScript.m_CurrentAmmo;
		}
	}

	private void OnSceneLoad()
	{
		if (m_WeaponScript != null)
		{
			(Globals.m_Inventory.m_Items[1][(int)m_WeaponScript.m_WeaponItemID] as Item_Weapon).m_CurrentAmmoInClip = m_WeaponScript.m_CurrentAmmo;
		}
		GameManager.OnSceneLoad -= OnSceneLoad;
	}

	public bool UseEnergyBar()
	{
		if (!AtMaxEnergy())
		{
			m_CurrentEnergy = Mathf.Min(m_CurrentEnergy + 1f, GetMaxEnergy());
			if (m_EnergyRegenMode != EnergyRegenMode.NotRegenerating && m_CurrentEnergy > m_EnergyRegenTarget)
			{
				m_EnergyRegenTarget += 1f;
				if (m_EnergyRegenTarget > GetMaxEnergy())
				{
					EnergyRegenComplete();
				}
			}
			SoundManager.TriggerEvent("Play_PU_EnergyBar", base.gameObject);
			return true;
		}
		SoundManager.TriggerEvent("Play_UI_Error", base.gameObject);
		return false;
	}

	public bool UseBooze()
	{
		if (!AtMaxHealth())
		{
			m_CurrentHealth = m_MaxHealth;
			SoundManager.TriggerEvent("Play_PU_Alcohol", base.gameObject);
			return true;
		}
		SoundManager.TriggerEvent("Play_UI_Error", base.gameObject);
		return false;
	}

	public bool UseGrenade(int type)
	{
		switch ((GrenadeType)type)
		{
		case GrenadeType.Frag:
			m_LastGrenadeObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_FragGrenadePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			break;
		case GrenadeType.EMP:
			m_LastGrenadeObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_EMPGrenadePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			break;
		case GrenadeType.Concussion:
			m_LastGrenadeObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_ConcussionGrenadePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			break;
		case GrenadeType.FragMine:
			m_LastGrenadeObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_FragMinePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			break;
		case GrenadeType.EMPMine:
			m_LastGrenadeObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_EMPMinePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			break;
		case GrenadeType.ConcussionMine:
			m_LastGrenadeObject = UnityEngine.Object.Instantiate(Globals.m_AIDirector.m_ConcussionMinePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			break;
		}
		m_LastGrenadeScript = m_LastGrenadeObject.GetComponent<GrenadeBase>();
		m_LastGrenadeScript.m_Owner = this;
		if (m_CameraMode == CameraMode.First)
		{
			SetGrenadeState(GrenadeState.Holstering);
			m_GrenadeThrowTime = m_WeaponScript.GetFirstPersonAnimLength(WeaponBase.FirstPersonAnimation.ThrowGrenade);
			m_GrenadeThrowTimer = m_GrenadeThrowTime;
			m_GrenadeReleaseTime = m_GrenadeThrowTime - 0.2f;
			m_LastGrenadeObject.transform.parent = m_GrenadeAttachPointFirstPerson.transform;
			if (!m_LastGrenadeScript.m_Mine)
			{
				m_LastGrenadeObject.transform.localPosition = Vector3.zero;
				m_LastGrenadeObject.transform.localRotation = Quaternion.identity;
			}
			else
			{
				m_LastGrenadeObject.transform.localPosition = new Vector3(-20.49834f, -4.313606f, -3.664007f);
				m_LastGrenadeObject.transform.localRotation = Quaternion.Euler(0f, 270f, 270f);
			}
			m_LastGrenadeObject.SetActiveRecursively(false);
			m_LastGrenadeObject.layer = 15;
		}
		else
		{
			SetCoverState(CoverState.ThrowGrenade);
			m_GrenadeThrowTime = m_ModelThirdPerson.animation[m_CoverAnimationsLeft[1, (int)m_WeaponScript.m_WeaponAnimSet, 21].name].length;
			m_GrenadeThrowTimer = m_GrenadeThrowTime;
			m_GrenadeReleaseTime = m_GrenadeThrowTime - 0.82f;
			if (m_CoverSide == CoverSide.Left)
			{
				m_LastGrenadeObject.transform.parent = m_GrenadeAttachPointThirdPersonLeft.transform;
			}
			else
			{
				m_LastGrenadeObject.transform.parent = m_GrenadeAttachPointThirdPersonRight.transform;
			}
			if (!m_LastGrenadeScript.m_Mine)
			{
				m_LastGrenadeObject.transform.localPosition = Vector3.zero;
				m_LastGrenadeObject.transform.localRotation = Quaternion.identity;
			}
			else
			{
				m_LastGrenadeObject.transform.localPosition = new Vector3(-0.1866146f, -0.05913883f, 0.0912298f);
				m_LastGrenadeObject.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
			}
			m_GrenadeState = GrenadeState.Throwing;
			CancelMovement();
			SoundManager.TriggerEvent("Play_VO_PC_Throw", base.gameObject);
		}
		return true;
	}

	public XmlElement SaveGame(XmlElement root, XmlDocument doc)
	{
		XmlElement xmlElement = (XmlElement)root.AppendChild(doc.CreateElement("PlayerController"));
		xmlElement.SetAttribute("Position", base.transform.position.x + "," + base.transform.position.y + "," + base.transform.position.z);
		xmlElement.SetAttribute("Rotation", Globals.m_CameraController.GetYaw() + "," + Globals.m_CameraController.GetPitch());
		xmlElement.SetAttribute("CoverState", m_CoverState.ToString() + "," + m_CoverSide);
		xmlElement.SetAttribute("Stance", m_Stance.ToString());
		int weaponItemID = (int)m_WeaponScript.m_WeaponItemID;
		xmlElement.SetAttribute("Current_Weapon", weaponItemID.ToString());
		xmlElement.SetAttribute("WeaponState", m_WeaponScript.m_WeaponState.ToString());
		xmlElement.SetAttribute("CurrentHealth", m_CurrentHealth.ToString());
		return xmlElement;
	}

	public XmlElement LoadGame(XmlElement root)
	{
		XmlNodeList elementsByTagName = root.GetElementsByTagName("PlayerController");
		foreach (XmlNode item in elementsByTagName)
		{
			if (!(item.Name == "PlayerController"))
			{
				continue;
			}
			foreach (XmlAttribute attribute in item.Attributes)
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
					Globals.m_CameraController.SetRotation(float.Parse(array[0]), float.Parse(array[1]));
					break;
				}
				case "CoverState":
				{
					string[] array = attribute.InnerText.Split(',');
					switch (array[0])
					{
					case "Outside":
						if (m_CoverState != CoverState.Outside)
						{
							ExitCover(true);
							m_WeaponScript.ResetFOV(true);
						}
						break;
					default:
						if (m_CoverState == CoverState.Outside)
						{
							Ray ray = new Ray(base.transform.position + new Vector3(0f, 0.1f, 0f), base.transform.forward);
							RaycastHit hitInfo;
							if (Physics.Raycast(ray, out hitInfo, 256f))
							{
								CoverSide coverSide = CoverSide.Left;
								if (array[1] == "Right")
								{
									coverSide = CoverSide.Right;
								}
								EnterCover(base.transform.position, hitInfo.normal, hitInfo.collider, false, true);
								SetCoverSide(coverSide);
								m_WeaponScript.ResetFOV(true);
							}
						}
						UpdateCoverEdge(true);
						break;
					}
					break;
				}
				case "Stance":
					switch (attribute.InnerText)
					{
					case "Stand":
						SetStance(Stance.Stand, true);
						break;
					case "Crouch":
						SetStance(Stance.Crouch, true);
						break;
					}
					break;
				case "Current_Weapon":
				{
					WeaponItemID weaponItemID = (WeaponItemID)int.Parse(attribute.InnerText);
					int currentAmmoInClip = (Globals.m_Inventory.m_Items[1][(int)weaponItemID] as Item_Weapon).m_CurrentAmmoInClip;
					DestroyCurrentWeapon();
					(Globals.m_Inventory.m_Items[1][(int)weaponItemID] as Item_Weapon).m_CurrentAmmoInClip = currentAmmoInClip;
					SetWeapon(Globals.m_Inventory.m_ActiveWeaponQuickslot, true);
					break;
				}
				case "WeaponState":
					if (attribute.InnerText == "Holstering" || attribute.InnerText == "Holstered")
					{
						m_WeaponScript.Holster();
					}
					break;
				case "CurrentHealth":
					m_CurrentHealth = int.Parse(attribute.InnerText);
					break;
				}
			}
			return (XmlElement)item;
		}
		return null;
	}

	private void SetupAnimList()
	{
		LoadAnim("StandingIdle");
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionLeftToRight, "COV_Crouch_LefttoRight_WPN_CombatRifle", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionRightToLeft, "COV_Crouch_RighttoLeft_WPN_CombatRifle", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionLeftToRight, "COV_Stand_LefttoRight_WPN_CombatRifle", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionRightToLeft, "COV_Stand_RighttoLeft_WPN_CombatRifle", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.TransitionLeftToRight, WeaponAnimSet.CombatRifle, "Play_Weapon_Handle", 0f);
		SetAnimSoundEventName(CoverAnimation.TransitionRightToLeft, WeaponAnimSet.CombatRifle, "Play_Weapon_Handle", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Enter, "COV_CrouchEnterLeft_WPN_CombatRifle", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Enter, "COV_CrouchEnterRight_WPN_CombatRifle", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Enter, "COV_StandEnterLeft_WPN_CombatRifle", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Enter, "COV_StandEnterRight_WPN_CombatRifle", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.Enter, WeaponAnimSet.CombatRifle, "Play_Cover_Enter", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Idle, "COV_CrouchIdleLeft_WPN_CombatRifle", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Idle, "COV_CrouchIdleRight_WPN_CombatRifle", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Idle, "COV_StandLeftIdle_WPN_CombatRifle", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Idle, "COV_StandRightIdle_WPN_CombatRifle", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Reload, "COV_CrouchLeft_WPN_CombatRifle_Reload", false, "CBR_TP_CrouchLeft_Reload", 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Reload, "COV_CrouchRight_WPN_CombatRifle_Reload", false, "CBR_TP_CrouchRight_Reload", 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Reload, "COV_StandLeft_WPN_CombatRifle_Reload", false, "CBR_TP_StandLeft_Reload", 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Reload, "COV_StandRight_WPN_CombatRifle_Reload", false, "CBR_TP_StandRight_Reload", 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Stand, "COV_CrouchLefttoStandLeft_WPN_CombatRifle", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Stand, "COV_CrouchRighttoStandRight_WPN_CombatRifle", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Crouch, "COV_StandLefttoCrouchLeft_WPN_CombatRifle", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Crouch, "COV_StandRighttoCrouchRight_WPN_CombatRifle", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionToLeanFire, "COV_CrouchLeft_WPN_CombatRifle_Fire_IN", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionToLeanFire, "COV_CrouchRight_WPN_CombatRifle_Fire_IN", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionToLeanFire, "COV_StandLeft_WPN_CombatRifle_Fire_IN", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionToLeanFire, "COV_StandRight_WPN_CombatRifle_Fire_IN", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.LeanFiring, "COV_CrouchLeft_WPN_CombatRifle_Fire_Loop", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.LeanFiring, "COV_CrouchRight_WPN_CombatRifle_Fire_Loop", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.LeanFiring, "COV_StandLeft_WPN_CombatRifle_Fire_Loop", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.LeanFiring, "COV_StandRight_WPN_CombatRifle_Fire_Loop", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionFromLeanFire, "COV_CrouchLeft_WPN_CombatRifle_Fire_OUT", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionFromLeanFire, "COV_CrouchRight_WPN_CombatRifle_Fire_OUT", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionFromLeanFire, "COV_StandLeft_WPN_CombatRifle_Fire_OUT", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionFromLeanFire, "COV_StandRight_WPN_CombatRifle_Fire_OUT", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.TransitionFromLeanFire, WeaponAnimSet.CombatRifle, "Play_Weapon_Handle", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionToUpOverFire, "COV_CrouchLeft_UpOver_WPN_CombatRifle_Fire_IN", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionToUpOverFire, "COV_CrouchRight_UpOver_WPN_CombatRifle_Fire_IN", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.UpOverFiring, "COV_CrouchLeft_UpOver_WPN_CombatRifle_Fire_Loop", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.UpOverFiring, "COV_CrouchRight_UpOver_WPN_CombatRifle_Fire_Loop", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionFromUpOverFire, "COV_CrouchLeft_UpOver_WPN_CombatRifle_Fire_OUT", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.TransitionFromUpOverFire, "COV_CrouchRight_UpOver_WPN_CombatRifle_Fire_OUT", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Move, "COV_CrouchLeft_WPN_CombatRifle_walkFWD", true, string.Empty, 0.3f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Move, "COV_CrouchRight_WPN_CombatRifle_walkFWD", true, string.Empty, 0.3f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Move, "COV_StandLeft_WPN_CombatRifle_walkFWD", true, string.Empty, 0.3f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Move, "COV_StandRight_WPN_CombatRifle_walkFWD", true, string.Empty, 0.3f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.MoveBackwards, "COV_CrouchLeft_WPN_CombatRifle_walkBACK", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.MoveBackwards, "COV_CrouchRight_WPN_CombatRifle_walkBACK", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.MoveBackwards, "COV_StandLeft_WPN_CombatRifle_walkBACK", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.MoveBackwards, "COV_StandRight_WPN_CombatRifle_walkBACK", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Holster, "COV_CrouchLeft_WPN_CombatRifle_Holster", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Holster, "COV_CrouchRight_WPN_CombatRifle_Holster", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Holster, "COV_StandLeft_WPN_CombatRifle_Holster", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Holster, "COV_StandRight_WPN_CombatRifle_Holster", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.Holster, WeaponAnimSet.CombatRifle, "Play_Weapon_Holster", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Draw, "COV_CrouchLeft_WPN_CombatRifle_Draw", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Draw, "COV_CrouchRight_WPN_CombatRifle_Draw", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Draw, "COV_StandLeft_WPN_CombatRifle_Draw", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Draw, "COV_StandRight_WPN_CombatRifle_Draw", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.Holster, WeaponAnimSet.CombatRifle, "Play_Weapon_Draw", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverFlip, "COV_CrouchLeft_WPN_CombatRifle_Flip", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverFlip, "COV_CrouchRight_WPN_CombatRifle_Flip", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverFlip, "COV_StandLeft_WPN_CombatRifle_Flip", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverFlip, "COV_StandRight_WPN_CombatRifle_Flip", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.CoverFlip, WeaponAnimSet.CombatRifle, "Play_Cover_Flip_Short", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverDive, "COV_CrouchLeft_WPN_CombatRifle_Dive", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverDive, "COV_CrouchRight_WPN_CombatRifle_Dive", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverDive, "COV_StandLeft_WPN_CombatRifle_Dive", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverDive, "COV_StandRight_WPN_CombatRifle_Dive", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.CoverDive, WeaponAnimSet.CombatRifle, "Play_Cover_Dive_Long", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideInner, "COV_CrouchLeft_WPN_CombatRifle_Inside90Edge", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideInner, "COV_CrouchRight_WPN_CombatRifle_Inside90Edge", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideInner, "COV_StandLeft_WPN_CombatRifle_Inside90Edge", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideInner, "COV_StandRight_WPN_CombatRifle_Inside90Edge", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.CoverSlideInner, WeaponAnimSet.CombatRifle, "Play_Cover_Slide", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideOuter, "COV_CrouchLeft_WPN_CombatRifle_CornerOut90Edge", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideOuter, "COV_CrouchRight_WPN_CombatRifle_CornerOut90Edge", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideOuter, "COV_StandLeft_WPN_CombatRifle_CornerOut90Edge", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.CoverSlideOuter, "COV_StandRight_WPN_CombatRifle_CornerOut90Edge", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.CoverSlideOuter, WeaponAnimSet.CombatRifle, "Play_Cover_Slide", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.ThrowGrenade, "COV_CrouchLeft_WPN_CombatRifle_throwGrenadeCorner", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.ThrowGrenade, "COV_CrouchRight_WPN_CombatRifle_throwGrenadeCorner", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.ThrowGrenade, "COV_StandLeft_WPN_CombatRifle_throwGrenadeCorner", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.ThrowGrenade, "COV_StandRight_WPN_CombatRifle_throwGrenadeCorner", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.ThrowGrenadeUpOver, "COV_CrouchLeft_WPN_CombatRifle_throwGrenadeUpOver", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.ThrowGrenadeUpOver, "COV_CrouchRight_WPN_CombatRifle_throwGrenadeUpOver", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Vault, "COV_CrouchLeft_WPN_CombatRifle_Vault", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Vault, "COV_CrouchRight_WPN_CombatRifle_Vault", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_EnterBottom, "NCB_Ladder_EnterBottom", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_EnterBottom, "NCB_Ladder_EnterBottom", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_EnterBottom, "NCB_Ladder_EnterBottom", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_EnterBottom, "NCB_Ladder_EnterBottom", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.Ladder_EnterBottom, WeaponAnimSet.CombatRifle, "Play_Ladder_Feet", 0.333f);
		SetAnimSoundEventName(CoverAnimation.Ladder_EnterBottom, WeaponAnimSet.CombatRifle, "Play_Ladder_Hands", 0.433f);
		SetAnimSoundEventName(CoverAnimation.Ladder_EnterBottom, WeaponAnimSet.CombatRifle, "Play_Ladder_Climb", 0.666f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_EnterTop, "NCB_Ladder_EnterTop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_EnterTop, "NCB_Ladder_EnterTop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_EnterTop, "NCB_Ladder_EnterTop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_EnterTop, "NCB_Ladder_EnterTop", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.Ladder_EnterTop, WeaponAnimSet.CombatRifle, "Play_Footsteps_Scuff", 0.1f);
		SetAnimSoundEventName(CoverAnimation.Ladder_EnterTop, WeaponAnimSet.CombatRifle, "Play_Ladder_Feet", 0.466f);
		SetAnimSoundEventName(CoverAnimation.Ladder_EnterTop, WeaponAnimSet.CombatRifle, "Play_Ladder_Feet", 0.9f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ClimbUp, "NCB_Ladder_ClimbUp", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ClimbUp, "NCB_Ladder_ClimbUp", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ClimbUp, "NCB_Ladder_ClimbUp", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ClimbUp, "NCB_Ladder_ClimbUp", true, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ClimbUp, WeaponAnimSet.CombatRifle, "Play_Ladder_Loop", 0.333f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ClimbDown, "NCB_Ladder_ClimbDown", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ClimbDown, "NCB_Ladder_ClimbDown", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ClimbDown, "NCB_Ladder_ClimbDown", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ClimbDown, "NCB_Ladder_ClimbDown", true, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ClimbDown, WeaponAnimSet.CombatRifle, "Play_Ladder_Loop", 0.333f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ExitTop, "NCB_Ladder_ExitTop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ExitTop, "NCB_Ladder_ExitTop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ExitTop, "NCB_Ladder_ExitTop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ExitTop, "NCB_Ladder_ExitTop", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitTop, WeaponAnimSet.CombatRifle, "Stop_Ladder_Loop", 0f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitTop, WeaponAnimSet.CombatRifle, "Play_Ladder_Feet", 0.166f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitTop, WeaponAnimSet.CombatRifle, "Play_Ladder_Hands", 0.333f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitTop, WeaponAnimSet.CombatRifle, "Stop_Ladder_Loop", 0.333f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitTop, WeaponAnimSet.CombatRifle, "Play_Movement_01", 0.5f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitTop, WeaponAnimSet.CombatRifle, "Play_Footsteps_Scuff", 0.633f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitTop, WeaponAnimSet.CombatRifle, "Play_Footsteps_3D", 1f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitTop, WeaponAnimSet.CombatRifle, "Play_Movement_03", 1.1f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitTop, WeaponAnimSet.CombatRifle, "Play_Footsteps_1st", 1.5f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ExitBottom, "NCB_Ladder_ExitBottom", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ExitBottom, "NCB_Ladder_ExitBottom", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ExitBottom, "NCB_Ladder_ExitBottom", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.CombatRifle, CoverAnimation.Ladder_ExitBottom, "NCB_Ladder_ExitBottom", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitBottom, WeaponAnimSet.CombatRifle, "Stop_Ladder_Loop", 0f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitBottom, WeaponAnimSet.CombatRifle, "Play_Movement_01", 0.066f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitBottom, WeaponAnimSet.CombatRifle, "Play_Footsteps_1st", 0.266f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitBottom, WeaponAnimSet.CombatRifle, "Play_Movement_03", 0.333f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitBottom, WeaponAnimSet.CombatRifle, "Stop_Ladder_Loop", 0.333f);
		SetAnimSoundEventName(CoverAnimation.Ladder_ExitBottom, WeaponAnimSet.CombatRifle, "Play_Footsteps_1st", 0.366f);
		CopyAnimSet(WeaponAnimSet.CombatRifle, WeaponAnimSet.Crossbow);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.LeanFiring, "COV_CrouchLeft_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire", 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.LeanFiring, "COV_CrouchRight_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire", 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Crossbow, CoverAnimation.LeanFiring, "COV_StandLeft_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire", 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Crossbow, CoverAnimation.LeanFiring, "COV_StandRight_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire", 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.UpOverFiring, "COV_CrouchLeft_UpOver_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire", 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.UpOverFiring, "COV_CrouchRight_UpOver_WPN_Crossbow_Fire_Loop", false, "CRB_TP_Fire", 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.Reload, "COV_CrouchLeft_WPN_Crossbow_Reload", false, "CRB_TP_CrouchLeft_Reload", 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Crossbow, CoverAnimation.Reload, "COV_CrouchRight_WPN_Crossbow_Reload", false, "CRB_TP_CrouchRight_Reload", 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Crossbow, CoverAnimation.Reload, "COV_StandLeft_WPN_Crossbow_Reload", false, "CRB_TP_StandLeft_Reload", 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Crossbow, CoverAnimation.Reload, "COV_StandRight_WPN_Crossbow_Reload", false, "CRB_TP_StandRight_Reload", 0.1f);
		CopyAnimSet(WeaponAnimSet.CombatRifle, WeaponAnimSet.Shotgun);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.LeanFiring, "COV_CrouchLeft_WPN_Shotgun_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.LeanFiring, "COV_CrouchRight_WPN_Shotgun_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Shotgun, CoverAnimation.LeanFiring, "COV_StandLeft_WPN_Shotgun_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Shotgun, CoverAnimation.LeanFiring, "COV_StandRight_WPN_Shotgun_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.UpOverFiring, "COV_CrouchLeft_UpOver_WPN_Shotgun_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.UpOverFiring, "COV_CrouchRight_UpOver_WPN_Shotgun_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.Reload, "COV_CrouchLeft_WPN_Shotgun_Reload", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Shotgun, CoverAnimation.Reload, "COV_CrouchRight_WPN_Shotgun_Reload", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Shotgun, CoverAnimation.Reload, "COV_StandLeft_WPN_Shotgun_Reload", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Shotgun, CoverAnimation.Reload, "COV_StandRight_WPN_Shotgun_Reload", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.TransitionLeftToRight, "COV_Crouch_LefttoRight_WPN_Pistol", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.TransitionRightToLeft, "COV_Crouch_RighttoLeft_WPN_Pistol", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.TransitionLeftToRight, "COV_Stand_LefttoRight_WPN_Pistol", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.TransitionRightToLeft, "COV_Stand_RighttoLeft_WPN_Pistol", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.TransitionLeftToRight, WeaponAnimSet.Pistol, "Play_Weapon_Handle", 0f);
		SetAnimSoundEventName(CoverAnimation.TransitionRightToLeft, WeaponAnimSet.Pistol, "Play_Weapon_Handle", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Enter, "COV_CrouchEnterLeft_WPN_Pistol", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Enter, "COV_CrouchEnterRight_WPN_Pistol", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Enter, "COV_StandEnterLeft_WPN_Pistol", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Enter, "COV_StandEnterRight_WPN_Pistol", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.Enter, WeaponAnimSet.Pistol, "Play_Cover_Enter", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Idle, "COV_CrouchIdleLeft_WPN_Pistol", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Idle, "COV_CrouchIdleRight_WPN_Pistol", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Idle, "COV_StandLeftIdle_WPN_Pistol", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Idle, "COV_StandRightIdle_WPN_Pistol", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Reload, "COV_CrouchLeft_WPN_Pistol_Reload", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Reload, "COV_CrouchRight_WPN_Pistol_Reload", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Reload, "COV_StandLeft_WPN_Pistol_Reload", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Reload, "COV_StandRight_WPN_Pistol_Reload", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Stand, "COV_CrouchLefttoStandLeft_WPN_Pistol", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Stand, "COV_CrouchRighttoStandRight_WPN_Pistol", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Crouch, "COV_StandLefttoCrouchLeft_WPN_Pistol", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Crouch, "COV_StandRighttoCrouchRight_WPN_Pistol", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.TransitionToLeanFire, "COV_CrouchLeft_WPN_Pistol_Fire_IN", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.TransitionToLeanFire, "COV_CrouchRight_WPN_Pistol_Fire_IN", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.TransitionToLeanFire, "COV_StandLeft_WPN_Pistol_Fire_IN", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.TransitionToLeanFire, "COV_StandRight_WPN_Pistol_Fire_IN", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.LeanFiring, "COV_CrouchLeft_WPN_Pistol_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.LeanFiring, "COV_CrouchRight_WPN_Pistol_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.LeanFiring, "COV_StandLeft_WPN_Pistol_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.LeanFiring, "COV_StandRight_WPN_Pistol_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.TransitionFromLeanFire, "COV_CrouchLeft_WPN_Pistol_Fire_OUT", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.TransitionFromLeanFire, "COV_CrouchRight_WPN_Pistol_Fire_OUT", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.TransitionFromLeanFire, "COV_StandLeft_WPN_Pistol_Fire_OUT", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.TransitionFromLeanFire, "COV_StandRight_WPN_Pistol_Fire_OUT", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.TransitionFromLeanFire, WeaponAnimSet.Pistol, "Play_Weapon_Handle", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.TransitionToUpOverFire, "COV_CrouchLeft_UpOver_WPN_Pistol_Fire_IN", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.TransitionToUpOverFire, "COV_CrouchRight_UpOver_WPN_Pistol_Fire_IN", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.UpOverFiring, "COV_CrouchLeft_UpOver_WPN_Pistol_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.UpOverFiring, "COV_CrouchRight_UpOver_WPN_Pistol_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.TransitionFromUpOverFire, "COV_CrouchLeft_UpOver_WPN_Pistol_Fire_OUT", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.TransitionFromUpOverFire, "COV_CrouchRight_UpOver_WPN_Pistol_Fire_OUT", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Move, "COV_CrouchLeft_WPN_Pistol_walkFWD", true, string.Empty, 0.3f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Move, "COV_CrouchRight_WPN_Pistol_walkFWD", true, string.Empty, 0.3f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Move, "COV_StandLeft_WPN_Pistol_walkFWD", true, string.Empty, 0.3f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Move, "COV_StandRight_WPN_Pistol_walkFWD", true, string.Empty, 0.3f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.MoveBackwards, "COV_CrouchLeft_WPN_Pistol_walkBACK", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.MoveBackwards, "COV_CrouchRight_WPN_Pistol_walkBACK", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.MoveBackwards, "COV_StandLeft_WPN_Pistol_walkBACK", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.MoveBackwards, "COV_StandRight_WPN_Pistol_walkBACK", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Holster, "COV_CrouchLeft_WPN_Pistol_Holster", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Holster, "COV_CrouchRight_WPN_Pistol_Holster", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Holster, "COV_StandLeft_WPN_Pistol_Holster", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Holster, "COV_StandRight_WPN_Pistol_Holster", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.Holster, WeaponAnimSet.Pistol, "Play_Weapon_Holster", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Draw, "COV_CrouchLeft_WPN_Pistol_Draw", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Draw, "COV_CrouchRight_WPN_Pistol_Draw", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Draw, "COV_StandLeft_WPN_Pistol_Draw", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Draw, "COV_StandRight_WPN_Pistol_Draw", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.Holster, WeaponAnimSet.Pistol, "Play_Weapon_Draw", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.CoverFlip, "COV_CrouchLeft_WPN_Pistol_Flip", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.CoverFlip, "COV_CrouchRight_WPN_Pistol_Flip", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.CoverFlip, "COV_StandLeft_WPN_Pistol_Flip", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.CoverFlip, "COV_StandRight_WPN_Pistol_Flip", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.CoverFlip, WeaponAnimSet.Pistol, "Play_Cover_Flip_Short", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.CoverDive, "COV_CrouchLeft_WPN_Pistol_Dive", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.CoverDive, "COV_CrouchRight_WPN_Pistol_Dive", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.CoverDive, "COV_StandLeft_WPN_Pistol_Dive", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.CoverDive, "COV_StandRight_WPN_Pistol_Dive", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.CoverDive, WeaponAnimSet.Pistol, "Play_Cover_Dive_Long", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.CoverSlideInner, "COV_CrouchLeft_WPN_Pistol_Inside90Edge", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.CoverSlideInner, "COV_CrouchRight_WPN_Pistol_Inside90Edge", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.CoverSlideInner, "COV_StandLeft_WPN_Pistol_Inside90Edge", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.CoverSlideInner, "COV_StandRight_WPN_Pistol_Inside90Edge", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.CoverSlideInner, WeaponAnimSet.Pistol, "Play_Cover_Slide", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.CoverSlideOuter, "COV_CrouchLeft_WPN_Pistol_CornerOut90Edge", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.CoverSlideOuter, "COV_CrouchRight_WPN_Pistol_CornerOut90Edge", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.CoverSlideOuter, "COV_StandLeft_WPN_Pistol_CornerOut90Edge", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.CoverSlideOuter, "COV_StandRight_WPN_Pistol_CornerOut90Edge", false, string.Empty, 0.1f);
		SetAnimSoundEventName(CoverAnimation.CoverSlideOuter, WeaponAnimSet.Pistol, "Play_Cover_Slide", 0f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.ThrowGrenade, "COV_CrouchLeft_WPN_Pistol_throwGrenadeCorner", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.ThrowGrenade, "COV_CrouchRight_WPN_Pistol_throwGrenadeCorner", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.ThrowGrenade, "COV_StandLeft_WPN_Pistol_throwGrenadeCorner", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.ThrowGrenade, "COV_StandRight_WPN_Pistol_throwGrenadeCorner", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.ThrowGrenadeUpOver, "COV_CrouchLeft_WPN_Pistol_throwGrenadeUpOver", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.ThrowGrenadeUpOver, "COV_CrouchRight_WPN_Pistol_throwGrenadeUpOver", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Vault, "COV_CrouchLeft_WPN_CombatRifle_Vault", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Vault, "COV_CrouchRight_WPN_CombatRifle_Vault", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Ladder_EnterBottom, "NCB_Ladder_EnterBottom", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Ladder_EnterBottom, "NCB_Ladder_EnterBottom", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Ladder_EnterBottom, "NCB_Ladder_EnterBottom", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Ladder_EnterBottom, "NCB_Ladder_EnterBottom", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Ladder_EnterTop, "NCB_Ladder_EnterTop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Ladder_EnterTop, "NCB_Ladder_EnterTop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Ladder_EnterTop, "NCB_Ladder_EnterTop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Ladder_EnterTop, "NCB_Ladder_EnterTop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ClimbUp, "NCB_Ladder_ClimbUp", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ClimbUp, "NCB_Ladder_ClimbUp", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ClimbUp, "NCB_Ladder_ClimbUp", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ClimbUp, "NCB_Ladder_ClimbUp", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ClimbDown, "NCB_Ladder_ClimbDown", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ClimbDown, "NCB_Ladder_ClimbDown", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ClimbDown, "NCB_Ladder_ClimbDown", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ClimbDown, "NCB_Ladder_ClimbDown", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ExitTop, "NCB_Ladder_ExitTop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ExitTop, "NCB_Ladder_ExitTop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ExitTop, "NCB_Ladder_ExitTop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ExitTop, "NCB_Ladder_ExitTop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ExitBottom, "NCB_Ladder_ExitBottom", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ExitBottom, "NCB_Ladder_ExitBottom", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ExitBottom, "NCB_Ladder_ExitBottom", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.Pistol, CoverAnimation.Ladder_ExitBottom, "NCB_Ladder_ExitBottom", false, string.Empty, 0.1f);
		CopyAnimSet(WeaponAnimSet.CombatRifle, WeaponAnimSet.PlasmaRifle);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.PlasmaRifle, CoverAnimation.LeanFiring, "COV_CrouchLeft_WPN_PlasmaRifle_Fire_Loop", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.PlasmaRifle, CoverAnimation.LeanFiring, "COV_CrouchRight_WPN_PlasmaRifle_Fire_Loop", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.PlasmaRifle, CoverAnimation.LeanFiring, "COV_StandLeft_WPN_PlasmaRifle_Fire_Loop", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.PlasmaRifle, CoverAnimation.LeanFiring, "COV_StandRight_WPN_PlasmaRifle_Fire_Loop", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.PlasmaRifle, CoverAnimation.UpOverFiring, "COV_CrouchLeft_UpOver_WPN_PlasmaRifle_Fire_Loop", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.PlasmaRifle, CoverAnimation.UpOverFiring, "COV_CrouchRight_UpOver_WPN_PlasmaRifle_Fire_Loop", true, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.PlasmaRifle, CoverAnimation.Reload, "COV_CrouchLeft_WPN_PlasmaRifle_Reload", false, "plasmaRifle_TP_CrouchLeft_Reload", 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.PlasmaRifle, CoverAnimation.Reload, "COV_CrouchRight_WPN_PlasmaRifle_Reload", false, "plasmaRifle_TP_CrouchRight_Reload", 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.PlasmaRifle, CoverAnimation.Reload, "COV_StandLeft_WPN_PlasmaRifle_Reload", false, "plasmaRifle_TP_StandLeft_Reload", 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.PlasmaRifle, CoverAnimation.Reload, "COV_StandRight_WPN_PlasmaRifle_Reload", false, "plasmaRifle_TP_StandRight_Reload", 0.1f);
		CopyAnimSet(WeaponAnimSet.Pistol, WeaponAnimSet.StunGun);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.StunGun, CoverAnimation.LeanFiring, "COV_CrouchLeft_WPN_Stungun_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.StunGun, CoverAnimation.LeanFiring, "COV_CrouchRight_WPN_Stungun_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.StunGun, CoverAnimation.LeanFiring, "COV_StandLeft_WPN_Stungun_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.StunGun, CoverAnimation.LeanFiring, "COV_StandRight_WPN_Stungun_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.StunGun, CoverAnimation.UpOverFiring, "COV_CrouchLeft_UpOver_WPN_Stungun_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.StunGun, CoverAnimation.UpOverFiring, "COV_CrouchRight_UpOver_WPN_Stungun_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.StunGun, CoverAnimation.Reload, "COV_CrouchLeft_WPN_Stungun_Reload", false, "stungun_TP_CrouchLeft_Reload", 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.StunGun, CoverAnimation.Reload, "COV_CrouchRight_WPN_Stungun_Reload", false, "stungun_TP_CrouchRight_Reload", 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.StunGun, CoverAnimation.Reload, "COV_StandLeft_WPN_Stungun_Reload", false, "stungun_TP_StandLeft_Reload", 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.StunGun, CoverAnimation.Reload, "COV_StandRight_WPN_Stungun_Reload", false, "stungun_TP_StandRight_Reload", 0.1f);
		CopyAnimSet(WeaponAnimSet.CombatRifle, WeaponAnimSet.MiniRPG);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.MiniRPG, CoverAnimation.LeanFiring, "COV_CrouchLeft_WPN_RPG_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.MiniRPG, CoverAnimation.LeanFiring, "COV_CrouchRight_WPN_RPG_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.MiniRPG, CoverAnimation.LeanFiring, "COV_StandLeft_WPN_RPG_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.MiniRPG, CoverAnimation.LeanFiring, "COV_StandRight_WPN_RPG_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.MiniRPG, CoverAnimation.UpOverFiring, "COV_CrouchLeft_UpOver_WPN_RPG_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.MiniRPG, CoverAnimation.UpOverFiring, "COV_CrouchRight_UpOver_WPN_RPG_Fire_Loop", false, string.Empty, 0.1f);
		SetupAnim(CoverSide.Left, Stance.Crouch, WeaponAnimSet.MiniRPG, CoverAnimation.Reload, "COV_CrouchLeft_WPN_RPG_Reload", false, "RPG_TP_CrouchLeft_Reload", 0.1f);
		SetupAnim(CoverSide.Right, Stance.Crouch, WeaponAnimSet.MiniRPG, CoverAnimation.Reload, "COV_CrouchRight_WPN_RPG_Reload", false, "RPG_TP_CrouchRight_Reload", 0.1f);
		SetupAnim(CoverSide.Left, Stance.Stand, WeaponAnimSet.MiniRPG, CoverAnimation.Reload, "COV_StandLeft_WPN_RPG_Reload", false, "RPG_TP_StandLeft_Reload", 0.1f);
		SetupAnim(CoverSide.Right, Stance.Stand, WeaponAnimSet.MiniRPG, CoverAnimation.Reload, "COV_StandRight_WPN_RPG_Reload", false, "RPG_TP_StandRight_Reload", 0.1f);
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
			for (int j = 0; j < 31; j++)
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
			for (int j = 0; j < 3; j++)
			{
				m_TakedownAnimation[i, j] = new TakedownAnimation();
			}
		}
		TakedownAnimationData takedownAnimationData = m_TakedownAnimation[0, 1].AddAnimation("TD_LT_DoubleBlade_Back_C_3", TakedownEnemyCount.Single, "Play_TD_LT_DoubleBlade_Back_C_3");
		takedownAnimationData.AddCameraData(TakedownCamera.Camera1, 5.33f);
		takedownAnimationData.AddVFXData(m_DoubleBladeBackVFXPrefab);
		takedownAnimationData = m_TakedownAnimation[0, 0].AddAnimation("TD_LT_ArmLockBlade_Front_3", TakedownEnemyCount.Single, "Play_TD_LT_ArmLockBlade_Front_3");
		takedownAnimationData.AddCameraData(TakedownCamera.Camera1, 0.833f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera2, 2.033f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera3, 5.8f);
		takedownAnimationData.AddVFXData(m_ArmlockBladeFrontVFXPrefab);
		takedownAnimationData = m_TakedownAnimation[0, 1].AddAnimation("TD_LT_Double_Throw_Back", TakedownEnemyCount.Double, "Play_TD_LT_Double_Throw_Back");
		takedownAnimationData.AddCameraData(TakedownCamera.Camera1, 4.0667f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera2, 8.8667f);
		takedownAnimationData = m_TakedownAnimation[0, 0].AddAnimation("TD_LT_Shield_Front", TakedownEnemyCount.Double, "Play_TD_LT_Shield_Front");
		takedownAnimationData.AddCameraData(TakedownCamera.Camera1, 4.3667f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera2, 8.33f);
		takedownAnimationData = m_TakedownAnimation[0, 2].AddAnimation("TD_LT_PunchThroughWall_Kill", TakedownEnemyCount.Single, "Play_TD_LT_PunchThroughWall_Kill");
		takedownAnimationData.AddCameraData(TakedownCamera.Camera1, 1.8f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera2, 3.833f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera3, 6f);
		takedownAnimationData = m_TakedownAnimation[1, 1].AddAnimation("TD_NT_Knee_BK_4", TakedownEnemyCount.Single, "Play_TD_NT_Knee_BK_4");
		takedownAnimationData.AddCameraData(TakedownCamera.Camera1, 3.67f);
		takedownAnimationData.AddVFXData(m_KneeBackVFXPrefab);
		takedownAnimationData = m_TakedownAnimation[1, 0].AddAnimation("TD_NT_SinglePunch_Front_2", TakedownEnemyCount.Single, "Play_TD_NT_SinglePunch_Front_2");
		takedownAnimationData.AddCameraData(TakedownCamera.Camera1, 2.367f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera2, 4.33f);
		takedownAnimationData.AddVFXData(m_SinglePunchFrontVFXPrefab);
		takedownAnimationData = m_TakedownAnimation[1, 1].AddAnimation("TD_NT_KickLegs_5", TakedownEnemyCount.Double, "Play_TD_NT_KickLegs_5");
		takedownAnimationData.AddCameraData(TakedownCamera.Camera1, 1.667f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera2, 4.7667f);
		takedownAnimationData = m_TakedownAnimation[1, 0].AddAnimation("TD_NT_Fk_D_4", TakedownEnemyCount.Double, "Play_TD_NT_Fk_D_4");
		takedownAnimationData.AddCameraData(TakedownCamera.Camera1, 2.3f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera2, 3.5f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera3, 6.033f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera4, 10f);
		takedownAnimationData = m_TakedownAnimation[1, 2].AddAnimation("TD_LT_PunchThroughWall_Empty", TakedownEnemyCount.Single, "Play_TD_LT_PunchThroughWall_Empty");
		takedownAnimationData.AddCameraData(TakedownCamera.Camera1, 1.8f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera2, 3.833f);
		takedownAnimationData.AddCameraData(TakedownCamera.Camera3, 6f);
		DisableTakedownCameras();
	}

	public void BeginTakedown(bool lethal, GameObject enemy, GameObject enemy2)
	{
		m_PressedForTakedown = false;
		if (!IsEnergyAvailable(1f, true))
		{
			return;
		}
		m_TakedownEnemy = enemy.GetComponent<Enemy_Base>();
		if (enemy2 != null)
		{
			m_TakedownEnemy2 = enemy2.GetComponent<Enemy_Base>();
		}
		if (m_TakedownEnemy.IsDead())
		{
			m_TakedownEnemy = null;
			return;
		}
		if ((bool)m_TakedownEnemy2 && m_TakedownEnemy2.IsDead())
		{
			m_TakedownEnemy2 = null;
		}
		InteractiveObject_Takedown componentInChildren = enemy.GetComponentInChildren<InteractiveObject_Takedown>();
		if (componentInChildren != null)
		{
			componentInChildren.InteractWithObject(false);
		}
		else
		{
			Debug.Log(enemy.name + " is missing an InteractiveObject_Takedown script.");
		}
		if ((bool)enemy2)
		{
			componentInChildren = enemy2.GetComponentInChildren<InteractiveObject_Takedown>();
			if (componentInChildren != null)
			{
				componentInChildren.InteractWithObject(false);
			}
			else
			{
				Debug.Log(enemy2.name + " is missing an InteractiveObject_Takedown script.");
			}
		}
		UseEnergy(1f);
		Globals.m_HUD.Display(false, true, false);
		Globals.m_HUD.EnablePassThruInput(false);
		if (m_CoverState != CoverState.Outside)
		{
			ExitCover(false);
		}
		CancelMovement();
		m_PreWeaponState = m_WeaponScript.m_WeaponState;
		SetTakedown();
		Globals.m_AIDirector.SetEnemiesPaused(true);
		m_TakedownEnemy.BeginTakedownOnEnemy();
		if ((bool)m_TakedownEnemy2)
		{
			m_TakedownEnemy2.BeginTakedownOnEnemy();
		}
		m_TakedownEnemy.StopBurstFire();
		if ((bool)m_TakedownEnemy2)
		{
			m_TakedownEnemy2.StopBurstFire();
		}
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
		TakedownEnemyCount takedownEnemyCount = ((!(m_TakedownEnemy2 == null)) ? TakedownEnemyCount.Double : TakedownEnemyCount.Single);
		enemy.transform.rotation = Quaternion.Euler(0f, enemy.transform.rotation.eulerAngles.y + ((m_TakedownPositioning != TakedownPositioning.Front) ? 0f : 180f), 0f);
		Globals.m_CameraController.SetYaw(enemy.transform.rotation.eulerAngles.y);
		m_CurrentAnimationData = m_TakedownAnimation[(int)takedownAttackType, (int)m_TakedownPositioning].GetAnimationData(takedownEnemyCount);
		m_CurrentAnimationData.Reset();
		m_CurrentAnimationData.PlayVFX(componentInChildren.m_TakedownVFXJoint);
		EnableTakedownCamera(m_CurrentAnimationData.GetCurrentTakedownCamera());
		m_CurrentCamera.enabled = false;
		if ((bool)m_TakedownEnemy)
		{
			foreach (AnimationState item in m_TakedownEnemy.m_Animator)
			{
				item.speed = m_TakedownEnemy.m_CurrentAnimationSpeed;
			}
		}
		if ((bool)m_TakedownEnemy2)
		{
			foreach (AnimationState item2 in m_TakedownEnemy2.m_Animator)
			{
				item2.speed = m_TakedownEnemy2.m_CurrentAnimationSpeed;
			}
		}
		if (takedownEnemyCount == TakedownEnemyCount.Single)
		{
			m_TakedownModel.animation.Play(m_CurrentAnimationData.name);
			m_TakedownAnimator.animation.Play("CameraJnt_" + m_CurrentAnimationData.name);
			m_TakedownEnemy.m_Animator.Play(m_CurrentAnimationData.name);
		}
		else
		{
			enemy2.transform.position = enemy.transform.position;
			enemy2.transform.rotation = Quaternion.Euler(0f, enemy2.transform.rotation.eulerAngles.y + ((m_TakedownPositioning != TakedownPositioning.Front) ? 0f : 180f), 0f);
			m_TakedownModel.animation.Play(m_CurrentAnimationData.name);
			m_TakedownAnimator.animation.Play("CameraJnt_" + m_CurrentAnimationData.name);
			m_TakedownEnemy.m_Animator.Play(m_CurrentAnimationData.name + "_Enemy1");
			m_TakedownEnemy2.m_Animator.Play(m_CurrentAnimationData.name + "_Enemy2");
		}
		if (m_CurrentAnimationData.soundEventName != string.Empty)
		{
			SoundManager.TriggerEvent(m_CurrentAnimationData.soundEventName, base.gameObject);
		}
		m_CloakingWasActive = Globals.m_AugmentCloaking.enabled;
		if (m_CloakingWasActive)
		{
			Globals.m_AugmentCloaking.Disable();
		}
		m_SeeThroughWallsWasActive = Globals.m_AugmentSeeThroughWalls.enabled;
		if (m_SeeThroughWallsWasActive)
		{
			Globals.m_AugmentSeeThroughWalls.InstantDisable();
		}
	}

	public void BeginWallTakedown(GameObject wall, GameObject enemy)
	{
		if (!IsEnergyAvailable(1f, true))
		{
			return;
		}
		Globals.m_AIDirector.SetEnemiesPaused(true);
		m_BreakableWall = wall.GetComponent<InteractiveObject_BreakableWall>();
		if (wall == null)
		{
			Debug.Log(wall.name + " is missing an InteractiveObject_BreakableWall script.");
		}
		else
		{
			if (m_BreakableWall.IsWallBroken())
			{
				return;
			}
			m_BreakableWall.BeginBreakingWall(false);
			UseEnergy(1f);
			Globals.m_HUD.Display(false, true, false);
			Globals.m_HUD.EnablePassThruInput(false);
			if (m_CoverState != CoverState.Outside)
			{
				ExitCover(false);
			}
			CancelMovement();
			m_PreWeaponState = m_WeaponScript.m_WeaponState;
			SetTakedown();
			m_CloakingWasActive = Globals.m_AugmentCloaking.enabled;
			if (m_CloakingWasActive)
			{
				Globals.m_AugmentCloaking.Disable();
			}
			m_SeeThroughWallsWasActive = Globals.m_AugmentSeeThroughWalls.enabled;
			if (m_SeeThroughWallsWasActive)
			{
				Globals.m_AugmentSeeThroughWalls.InstantDisable();
			}
			m_TakedownEnemy = null;
			if (enemy == null || ((bool)enemy && Vector3.Distance(m_BreakableWall.gameObject.transform.position, enemy.transform.position) > m_BreakableWall.m_DistanceToCheckForEnemy))
			{
				m_TakedownModel = UnityEngine.Object.Instantiate(m_TakedownModelPrefab) as GameObject;
				m_TakedownModel.transform.parent = base.gameObject.transform;
				m_TakedownModel.transform.localPosition = Vector3.zero;
				m_TakedownModel.transform.localRotation = Quaternion.identity;
				m_TakedownModel.transform.localScale = Vector3.one;
				float num = Vector3.Dot(m_TakedownModel.transform.forward, m_BreakableWall.gameObject.transform.forward);
				TakedownPositioning takedownPositioning = ((!(num < 0f)) ? TakedownPositioning.Back : TakedownPositioning.Front);
				Vector3 position = m_BreakableWall.gameObject.transform.position;
				position.y = GetGroundHeight(position) + 0.01f;
				Vector3 position2 = m_BreakableWall.gameObject.transform.position;
				position2 += ((takedownPositioning != TakedownPositioning.Front) ? 0f : m_BreakableWallOffset) * m_BreakableWall.gameObject.transform.forward;
				m_BreakableWall.m_WallCubes.transform.position = position2;
				base.gameObject.transform.position = position;
				m_LethalTakedown = true;
				TakedownAttackType takedownAttackType = TakedownAttackType.NonLethal;
				m_TakedownPositioning = TakedownPositioning.Wall;
				TakedownEnemyCount count = TakedownEnemyCount.Single;
				m_BreakableWall.m_WallCubes.transform.rotation = Quaternion.Euler(0f, m_BreakableWall.gameObject.transform.rotation.eulerAngles.y + ((takedownPositioning != TakedownPositioning.Front) ? 0f : 180f), 0f);
				Globals.m_CameraController.SetYaw(Quaternion.Euler(0f, m_BreakableWall.gameObject.transform.rotation.eulerAngles.y + ((takedownPositioning != TakedownPositioning.Front) ? 0f : 180f), 0f).eulerAngles.y);
				m_CurrentAnimationData = m_TakedownAnimation[(int)takedownAttackType, (int)m_TakedownPositioning].GetAnimationData(count);
				m_CurrentAnimationData.Reset();
				m_CurrentAnimationData.PlayVFX(m_BreakableWall.m_VFXPrefab);
				EnableTakedownCamera(m_CurrentAnimationData.GetCurrentTakedownCamera());
				m_CurrentCamera.enabled = false;
				if ((bool)m_TakedownEnemy)
				{
					foreach (AnimationState item in m_TakedownEnemy.m_Animator)
					{
						item.speed = m_TakedownEnemy.m_CurrentAnimationSpeed;
					}
				}
				m_TakedownModel.animation.Play(m_CurrentAnimationData.name);
				m_TakedownAnimator.animation.Play("CameraJnt_" + m_CurrentAnimationData.name);
				if (m_CurrentAnimationData.soundEventName != string.Empty)
				{
					SoundManager.TriggerEvent(m_CurrentAnimationData.soundEventName, base.gameObject);
				}
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
				componentInChildren.InteractWithObject(false);
			}
			else
			{
				Debug.Log(enemy.name + " is missing an InteractiveObject_Takedown script.");
			}
			m_TakedownEnemy.BeginTakedownOnEnemy();
			m_TakedownEnemy.StopBurstFire();
			m_TakedownModel = UnityEngine.Object.Instantiate(m_TakedownModelPrefab) as GameObject;
			m_TakedownModel.transform.parent = base.gameObject.transform;
			m_TakedownModel.transform.localPosition = Vector3.zero;
			m_TakedownModel.transform.localRotation = Quaternion.identity;
			m_TakedownModel.transform.localScale = Vector3.one;
			float num2 = Vector3.Dot(m_TakedownModel.transform.forward, m_BreakableWall.gameObject.transform.forward);
			TakedownPositioning takedownPositioning2 = ((!(num2 < 0f)) ? TakedownPositioning.Back : TakedownPositioning.Front);
			Vector3 position3 = m_BreakableWall.gameObject.transform.position;
			position3 += ((takedownPositioning2 != TakedownPositioning.Front) ? 0f : m_BreakableWallOffset) * m_BreakableWall.gameObject.transform.forward;
			enemy.transform.position = position3;
			Vector3 position4 = enemy.transform.position;
			position4.y = GetGroundHeight(position4) + 0.01f;
			position3 -= ((takedownPositioning2 != TakedownPositioning.Front) ? 0f : (m_BreakableWallOffset * 0.5f)) * m_BreakableWall.gameObject.transform.forward;
			m_BreakableWall.m_WallCubes.transform.position = position3;
			base.gameObject.transform.position = position4;
			m_LethalTakedown = true;
			TakedownAttackType takedownAttackType2 = TakedownAttackType.Lethal;
			m_TakedownPositioning = TakedownPositioning.Wall;
			TakedownEnemyCount count2 = TakedownEnemyCount.Single;
			enemy.transform.rotation = Quaternion.Euler(0f, m_BreakableWall.gameObject.transform.rotation.eulerAngles.y + ((takedownPositioning2 != TakedownPositioning.Front) ? 0f : 180f), 0f);
			m_BreakableWall.m_WallCubes.transform.rotation = Quaternion.Euler(0f, m_BreakableWall.gameObject.transform.rotation.eulerAngles.y + ((takedownPositioning2 != TakedownPositioning.Front) ? 0f : 180f), 0f);
			Globals.m_CameraController.SetYaw(enemy.transform.rotation.eulerAngles.y);
			m_CurrentAnimationData = m_TakedownAnimation[(int)takedownAttackType2, (int)m_TakedownPositioning].GetAnimationData(count2);
			m_CurrentAnimationData.Reset();
			m_CurrentAnimationData.PlayVFX(componentInChildren.m_TakedownVFXJoint);
			EnableTakedownCamera(m_CurrentAnimationData.GetCurrentTakedownCamera());
			m_CurrentCamera.enabled = false;
			if ((bool)m_TakedownEnemy)
			{
				foreach (AnimationState item2 in m_TakedownEnemy.m_Animator)
				{
					item2.speed = m_TakedownEnemy.m_CurrentAnimationSpeed;
				}
			}
			m_TakedownModel.animation.Play(m_CurrentAnimationData.name);
			m_TakedownAnimator.animation.Play("CameraJnt_" + m_CurrentAnimationData.name);
			m_TakedownEnemy.m_Animator.Play(m_CurrentAnimationData.name);
			if (m_CurrentAnimationData.soundEventName != string.Empty)
			{
				SoundManager.TriggerEvent(m_CurrentAnimationData.soundEventName, base.gameObject);
			}
		}
	}

	private void EndTakedown()
	{
		m_CurrentAnimationData.CleanUpVFX();
		if (m_BreakableWall != null)
		{
			m_BreakableWall.CleanUpVFX();
		}
		DisableTakedownCameras();
		m_CurrentCamera.enabled = true;
		m_RendererThirdPerson.enabled = true;
		SetFirstPerson();
		if (m_PreWeaponState != WeaponBase.WeaponState.Holstering && m_PreWeaponState != WeaponBase.WeaponState.Holstered)
		{
			m_WeaponScript.Unholster();
		}
		Globals.m_AIDirector.SetEnemiesPaused(false);
		if ((bool)m_TakedownEnemy)
		{
			if (m_LethalTakedown)
			{
				m_TakedownEnemy.TakedownLethal();
			}
			else
			{
				m_TakedownEnemy.TakedownNonLethal();
			}
			m_TakedownEnemy = null;
		}
		if ((bool)m_TakedownEnemy2)
		{
			if (m_LethalTakedown)
			{
				m_TakedownEnemy2.TakedownLethal();
			}
			else
			{
				m_TakedownEnemy2.TakedownNonLethal();
			}
			m_TakedownEnemy2 = null;
		}
		if (m_LethalTakedown)
		{
			Globals.m_AIDirector.CheckAudioSenses(base.transform.position, 20f, DisturbanceEvent.MajorAudio, false);
		}
		Globals.m_HUD.Display(true, true, false);
		Globals.m_HUD.EnablePassThruInput(true);
		UnityEngine.Object.Destroy(m_TakedownModel);
		m_BreakableWall = null;
		if (m_CloakingWasActive)
		{
			Globals.m_AugmentCloaking.Enable();
		}
		if (m_SeeThroughWallsWasActive)
		{
			Globals.m_AugmentSeeThroughWalls.Enable();
		}
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
			position += enemy.transform.right * num2;
		}
		else if (!flag4 && flag3)
		{
			position += enemy.transform.right * num2;
		}
		else if (!flag3 && !flag4)
		{
			float num3 = 0f - (num2 - Vector3.Distance(position, hitInfo3.point));
			float num4 = num2 - Vector3.Distance(position, hitInfo4.point);
			position += enemy.transform.right * ((num3 + num4) * 0.5f);
		}
		if (!flag && flag2)
		{
			float num5 = 0f - (num - Vector3.Distance(position, hitInfo.point));
			position += enemy.transform.forward * num5;
		}
		else if (!flag2 && flag)
		{
			float num6 = num - Vector3.Distance(position, hitInfo2.point);
			position += enemy.transform.forward * num6;
		}
		else if (!flag && !flag2)
		{
			float num7 = 0f - (num - Vector3.Distance(position, hitInfo.point));
			float num8 = num - Vector3.Distance(position, hitInfo2.point);
			position += enemy.transform.forward * ((num7 + num8) * 0.5f);
		}
		enemy.transform.position = position;
	}
}
