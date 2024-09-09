using System;
using System.Collections;
using Fabric;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
	public enum FireType
	{
		SingleShot = 0,
		RapidFire = 1,
		SemiAuto = 2
	}

	public enum WeaponState
	{
		Idle = 0,
		Firing = 1,
		Reloading = 2,
		Holstering = 3,
		Holstered = 4,
		Drawing = 5,
		Aiming = 6
	}

	public enum FirstPersonAnimation
	{
		None = 0,
		Idle = 1,
		Fire = 2,
		Reload = 3,
		Walk = 4,
		Draw = 5,
		Holster = 6,
		ThrowGrenade = 7
	}

	[HideInInspector]
	public WeaponType m_WeaponType = WeaponType.None;

	public FireType m_FireType = FireType.RapidFire;

	public float m_FireRate = 0.12f;

	public float m_BulletSpread = 0.1f;

	[HideInInspector]
	public float m_BulletSpreadAIModifier;

	public float m_Kick = 0.5f;

	public float m_HUDKick = 0.015f;

	public int m_AmmoPerClip = 20;

	public int m_Ammo = 200;

	public int m_Damage = 30;

	public int m_LowAmmoThreshold = 5;

	public WeaponAnimSet m_WeaponAnimSet;

	public int m_TracerBullets = 3;

	public int m_TracerCounter;

	public float m_OneBulletFireTime = 0.08f;

	public bool m_Silenced;

	public GameObject m_ProjectilePrefab;

	[HideInInspector]
	public CharacterBase m_User;

	[HideInInspector]
	public WeaponState m_WeaponState;

	private float m_FireTimer;

	[HideInInspector]
	public float m_ReloadTimer;

	[HideInInspector]
	public float m_ReloadTime;

	public int m_CurrentAmmo;

	private int m_StartFireCount;

	public float m_PlayerCoverZoomScale = 3f;

	public float m_ZoomScale = 1.3f;

	public float m_ZoomSpeed = 8f;

	public float m_ZoomTimer;

	public float m_ZoomTime = 0.2f;

	private float m_DefaultFOV;

	private float m_DesiredFOV;

	private float m_DefaultWeaponFOV;

	private float m_DesiredWeaponFOV;

	public float m_CameraJiggleAmount = 0.6f;

	private float m_JiggleFOV;

	protected FirstPersonAnimation m_CurrentFirstPersonAnimation;

	[HideInInspector]
	public float m_FirstPersonAnimationSpeed = 1f;

	public GameObject m_ModelFirstPerson;

	public Renderer m_RendererFirstPerson;

	public GameObject m_ModelThirdPersonPlayer;

	public Renderer m_RendererThirdPersonPlayer;

	public GameObject m_ModelThirdPersonEnemy;

	public string m_FireSoundEventName;

	public string m_FireTailSoundEventName;

	public string m_FireSoundFirstPersonEventName;

	public string m_FireTailSoundFirstPersonEventName;

	public string m_ReloadSoundEventName;

	public string m_ReloadSoundFirstPersonEventName;

	public string m_HolsterSoundEventName;

	public string m_DrawSoundEventName;

	public string m_BulletHitSoundEventName;

	public string m_BulletWhizSoundEventName;

	public GameObject m_HitFX;

	public GameObject m_BloodFX;

	public GameObject m_TracerFX;

	public GameObject m_MuzzleFlashFX;

	public GameObject m_MuzzleFlashAttachFirstPerson;

	public GameObject m_MuzzleFlashAttachThirdPerson;

	public GameObject m_MuzzleFlashAttachEnemy;

	private GameObject m_MuzzleFlashAttachObject;

	private ParticleSystem m_MuzzleFlashParticle;

	public float m_MuzzleFlashTime = 0.1f;

	public Texture m_ReticleImage;

	public Texture m_ReticleImageDot;

	public InteractiveVolume m_InteractiveVolume;

	public Material m_DroppedMaterial;

	public WeaponType GetWeaponType()
	{
		return m_WeaponType;
	}

	public float GetKick()
	{
		if (m_User == Globals.m_PlayerController)
		{
			Augmentation_AimStabilization augmentation_AimStabilization = (Augmentation_AimStabilization)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.AimStabilization);
			return m_Kick * augmentation_AimStabilization.GetKickScaler();
		}
		return m_Kick;
	}

	protected virtual void Awake()
	{
		if (m_MuzzleFlashFX != null)
		{
			m_MuzzleFlashFX = (GameObject)UnityEngine.Object.Instantiate(m_MuzzleFlashFX, new Vector3(0f, 0f, 0f), Quaternion.identity);
			m_MuzzleFlashParticle = m_MuzzleFlashFX.GetComponent<ParticleSystem>();
			m_MuzzleFlashParticle.Stop();
			m_MuzzleFlashParticle.Clear();
		}
		m_TracerCounter = m_TracerBullets;
		if ((bool)m_InteractiveVolume)
		{
			m_InteractiveVolume.DisableCollider();
		}
		m_CurrentAmmo = m_AmmoPerClip;
	}

	protected virtual void Start()
	{
		if (m_User == Globals.m_PlayerController)
		{
			m_DefaultFOV = Globals.m_PlayerController.m_CurrentCamera.fieldOfView;
			m_DesiredFOV = m_DefaultFOV;
			m_DefaultWeaponFOV = Globals.m_PlayerController.m_FirstPersonWeaponCameraComponent.fieldOfView;
			m_DesiredWeaponFOV = m_DefaultWeaponFOV;
			Globals.m_HUD.SetCurrentAmmo(m_CurrentAmmo);
			Globals.m_HUD.SetTotalAmmo(m_Ammo);
		}
	}

	protected virtual void Update()
	{
		switch (m_WeaponState)
		{
		case WeaponState.Firing:
		case WeaponState.Aiming:
			m_FireTimer += Time.deltaTime;
			if (m_WeaponState == WeaponState.Firing && m_FireType != FireType.RapidFire && m_FireTimer > m_MuzzleFlashTime)
			{
				StopFireFX();
			}
			if (m_WeaponState == WeaponState.Firing && m_FireType != FireType.RapidFire && m_FireTimer > m_FireRate)
			{
				m_WeaponState = WeaponState.Idle;
				m_FireTimer = 0f;
				ResetFOV();
				if (m_CurrentAmmo <= 0)
				{
					m_User.WeaponWantsReload();
				}
			}
			break;
		case WeaponState.Reloading:
			m_ReloadTimer -= Time.deltaTime;
			if (m_ReloadTimer <= 0f)
			{
				m_WeaponState = WeaponState.Idle;
				m_User.WeaponDoneReloading();
			}
			break;
		case WeaponState.Idle:
			if (m_FireType == FireType.SemiAuto && m_StartFireCount > 0)
			{
				StartFire();
				EndFire();
			}
			break;
		}
		if (m_User == Globals.m_PlayerController)
		{
			PlayerUpdate();
		}
	}

	protected virtual void PlayerUpdate()
	{
		switch (m_WeaponState)
		{
		case WeaponState.Holstering:
			if (!m_ModelFirstPerson.animation.isPlaying)
			{
				m_WeaponState = WeaponState.Holstered;
			}
			break;
		case WeaponState.Drawing:
			if (!m_ModelFirstPerson.animation.isPlaying)
			{
				m_WeaponState = WeaponState.Idle;
			}
			break;
		case WeaponState.Reloading:
		{
			float num = m_ReloadTime - m_ReloadTimer;
			if (num < 0f)
			{
				num = 0f;
			}
			Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(FirstPersonAnimation.Reload)].time = num;
			m_ModelFirstPerson.animation[GetFirstPersonWeaponAnimName(FirstPersonAnimation.Reload)].time = num;
			break;
		}
		}
		if (m_StartFireCount > 0 && m_WeaponState != WeaponState.Reloading)
		{
			m_ZoomTimer += Time.deltaTime;
		}
		else
		{
			m_ZoomTimer = 0f;
		}
		if (m_ZoomTimer >= m_ZoomTime)
		{
			m_DesiredFOV = m_DefaultFOV / m_ZoomScale;
			m_DesiredWeaponFOV = m_DefaultWeaponFOV / m_ZoomScale;
		}
		if (Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.TransitioningToFire || Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.CoverAiming || Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Firing || (Globals.m_PlayerController.m_CoverState == PlayerController.CoverState.Reloading && Globals.m_PlayerController.m_CameraCoverDot < Globals.m_PlayerController.m_CameraCoverAimLimit))
		{
			m_DesiredFOV = m_DefaultFOV / m_PlayerCoverZoomScale;
			m_DesiredWeaponFOV = m_DefaultWeaponFOV / m_PlayerCoverZoomScale;
		}
		Globals.m_CameraController.m_DragSensitivityAdjustment = m_DesiredFOV / m_DefaultFOV;
		if (m_WeaponState == WeaponState.Firing)
		{
			m_JiggleFOV += Time.deltaTime / m_FireRate * (float)Math.PI * 2f;
			float num2 = Mathf.Sin(m_JiggleFOV) * m_CameraJiggleAmount;
			m_DesiredFOV += num2;
			m_DesiredWeaponFOV += num2;
		}
		float fieldOfView = Globals.m_PlayerController.m_CurrentCamera.fieldOfView;
		if (fieldOfView != m_DesiredFOV)
		{
			float fieldOfView2 = Globals.m_PlayerController.m_FirstPersonWeaponCameraComponent.fieldOfView;
			fieldOfView += (m_DesiredFOV - fieldOfView) * m_ZoomSpeed * Time.deltaTime;
			fieldOfView2 += (m_DesiredWeaponFOV - fieldOfView2) * m_ZoomSpeed * Time.deltaTime;
			if (Mathf.Abs(fieldOfView - m_DesiredFOV) < 0.01f)
			{
				fieldOfView = m_DesiredFOV;
				fieldOfView2 = m_DesiredWeaponFOV;
			}
			Globals.m_PlayerController.m_CurrentCamera.fieldOfView = fieldOfView;
			Globals.m_PlayerController.m_FirstPersonWeaponCameraComponent.fieldOfView = fieldOfView2;
		}
		if (Globals.m_PlayerController.m_CameraMode == PlayerController.CameraMode.First)
		{
			UpdateFirstPersonAnimSpeed();
		}
	}

	public virtual void StartFire()
	{
		if (m_CurrentAmmo <= 0)
		{
			EventManager.Instance.PostEvent("Weapon_DryFire", EventAction.PlaySound, null, base.gameObject);
		}
		else if (m_WeaponState == WeaponState.Idle)
		{
			m_StartFireCount++;
			m_FireTimer = 0f;
			m_JiggleFOV = 0f;
			switch (m_FireType)
			{
			case FireType.RapidFire:
				PlayFireFX();
				m_WeaponState = WeaponState.Firing;
				StartCoroutine("FireBulletCoroutine");
				break;
			case FireType.SingleShot:
				m_WeaponState = WeaponState.Aiming;
				break;
			case FireType.SemiAuto:
				PlayFireFX();
				FireBullet();
				m_WeaponState = WeaponState.Firing;
				break;
			}
		}
	}

	public virtual void EndFire()
	{
		if (m_StartFireCount <= 0)
		{
			return;
		}
		m_StartFireCount--;
		switch (m_FireType)
		{
		case FireType.RapidFire:
			StopCoroutine("FireBulletCoroutine");
			if (m_User != Globals.m_PlayerController || Globals.m_PlayerController.m_CameraMode == PlayerController.CameraMode.Third)
			{
				EventManager.Instance.PostEvent(m_FireSoundEventName, EventAction.StopSound, null, base.gameObject);
				EventManager.Instance.PostEvent(m_FireTailSoundEventName, base.gameObject);
			}
			else
			{
				EventManager.Instance.PostEvent(m_FireSoundFirstPersonEventName, EventAction.StopSound, null, base.gameObject);
				EventManager.Instance.PostEvent(m_FireTailSoundFirstPersonEventName, base.gameObject);
			}
			StopFireFX();
			m_FireTimer = 0f;
			m_WeaponState = WeaponState.Idle;
			break;
		case FireType.SingleShot:
			PlayFireFX();
			FireBullet();
			m_WeaponState = WeaponState.Firing;
			if (m_FireTimer > m_ZoomTime)
			{
				m_FireTimer = m_ZoomTime;
			}
			else
			{
				m_FireTimer = 0f;
			}
			break;
		}
		ResetFOV();
	}

	public virtual void CancelFire()
	{
		if (m_StartFireCount > 0)
		{
			m_StartFireCount--;
			m_FireTimer = 0f;
			m_WeaponState = WeaponState.Idle;
			ResetFOV();
		}
	}

	protected virtual void SetupFireFX()
	{
		if (m_MuzzleFlashFX != null)
		{
			m_MuzzleFlashFX.transform.parent = m_MuzzleFlashAttachObject.transform;
			m_MuzzleFlashFX.transform.localPosition = Vector3.zero;
			m_MuzzleFlashFX.transform.localRotation = Quaternion.identity;
		}
	}

	public virtual void PlayFireFX()
	{
		if (m_User != Globals.m_PlayerController || Globals.m_PlayerController.m_CameraMode == PlayerController.CameraMode.Third)
		{
			EventManager.Instance.PostEvent(m_FireSoundEventName, base.gameObject);
		}
		else
		{
			EventManager.Instance.PostEvent(m_FireSoundFirstPersonEventName, base.gameObject);
		}
		if (m_MuzzleFlashFX != null)
		{
			m_MuzzleFlashParticle.Play();
		}
	}

	public virtual void StopFireFX()
	{
		if (m_MuzzleFlashFX != null)
		{
			m_MuzzleFlashParticle.Stop();
			m_MuzzleFlashParticle.Clear();
		}
	}

	public virtual void PlayHitFX(Vector3 hitloc, Vector3 hitnorm, int hitlayer, bool isturret)
	{
		if ((hitlayer != 9 && hitlayer != 14) || isturret)
		{
			Quaternion rotation = Quaternion.LookRotation(hitnorm);
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(m_HitFX, hitloc, rotation);
			float num = gameObject.GetComponent<ParticleSystem>().duration;
			if (num < 1f)
			{
				num = 1f;
			}
			UnityEngine.Object.Destroy(gameObject, num);
			EventManager.Instance.PostEvent(m_BulletHitSoundEventName, gameObject);
		}
		else if (hitlayer != 14)
		{
			Quaternion rotation2 = Quaternion.LookRotation(hitnorm);
			GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(m_BloodFX, hitloc, rotation2);
			UnityEngine.Object.Destroy(gameObject2, gameObject2.GetComponent<ParticleSystem>().duration);
		}
	}

	public virtual void ResetFOV()
	{
		m_DesiredFOV = m_DefaultFOV;
		m_DesiredWeaponFOV = m_DefaultWeaponFOV;
	}

	public virtual int GetAmmo()
	{
		return m_Ammo;
	}

	public virtual void GiveAmmo(int amount)
	{
		m_Ammo += amount;
		if (m_User == Globals.m_PlayerController)
		{
			Globals.m_HUD.SetTotalAmmo(m_Ammo);
		}
	}

	public virtual int GetNumBulletsInClip()
	{
		return m_CurrentAmmo;
	}

	public virtual float GetPercentageBulletsInClip()
	{
		return Mathf.Clamp((float)m_CurrentAmmo / (float)m_AmmoPerClip, 0f, 1f);
	}

	public virtual void Reload(float reloadTime)
	{
		if (m_Ammo > 0)
		{
			int num = m_AmmoPerClip - m_CurrentAmmo;
			if (num > m_Ammo)
			{
				num = m_Ammo;
			}
			m_CurrentAmmo += num;
			if (m_User == Globals.m_PlayerController)
			{
				m_Ammo -= num;
			}
			m_WeaponState = WeaponState.Reloading;
			m_ReloadTimer = reloadTime;
			m_ReloadTime = reloadTime;
			if (m_User != Globals.m_PlayerController || Globals.m_PlayerController.m_CameraMode == PlayerController.CameraMode.Third)
			{
				EventManager.Instance.PostEvent(m_ReloadSoundEventName, EventAction.PlaySound, null, base.gameObject);
			}
			else
			{
				EventManager.Instance.PostEvent(m_ReloadSoundFirstPersonEventName, base.gameObject);
			}
			if (m_User == Globals.m_PlayerController)
			{
				Globals.m_HUD.SetTotalAmmo(m_Ammo);
				Globals.m_HUD.SetCurrentAmmo(m_CurrentAmmo);
			}
		}
	}

	protected virtual IEnumerator FireBulletCoroutine()
	{
		while (true)
		{
			FireBullet();
			if (m_CurrentAmmo <= 0)
			{
				break;
			}
			yield return new WaitForSeconds(m_FireRate);
		}
		EndFire();
		m_User.WeaponWantsReload();
	}

	protected virtual void FireBullet()
	{
		m_CurrentAmmo--;
		FireHitScanBullet();
		PlayerFiredBullet();
	}

	protected virtual void FireHitScanBullet()
	{
		bool PlayTracer = true;
		Ray ray = m_User.WeaponRequestForBulletRay(out PlayTracer);
		Quaternion quaternion = Quaternion.Euler(UnityEngine.Random.Range(0f - m_BulletSpread, m_BulletSpread), UnityEngine.Random.Range(0f - m_BulletSpread, m_BulletSpread), 0f);
		Quaternion quaternion2 = Quaternion.LookRotation(ray.direction);
		ray.direction = quaternion2 * quaternion * Vector3.forward;
		int layerMask = 16641;
		if (m_User == Globals.m_PlayerController)
		{
			layerMask = 4195073;
		}
		bool flag = false;
		RaycastHit hitInfo;
		Vector3 vector;
		if (Physics.Raycast(ray, out hitInfo, 100f, layerMask))
		{
			vector = hitInfo.point;
			flag = true;
		}
		else
		{
			vector = ray.origin + ray.direction * 100f;
		}
		if (m_ProjectilePrefab == null)
		{
			if (flag)
			{
				bool isturret = false;
				CharacterBase characterBase = Globals.FindCharacterBase(hitInfo.transform);
				if (characterBase != null && characterBase.m_CharacterType == CharacterBase.CharacterType.Machine)
				{
					isturret = true;
				}
				PlayHitFX(hitInfo.point, hitInfo.normal, hitInfo.collider.gameObject.layer, isturret);
				switch (hitInfo.collider.gameObject.layer)
				{
				case 22:
				{
					SecurityCamera securityCamera = Globals.FindSecurityCameraComponent(hitInfo.collider.transform);
					if (securityCamera != null)
					{
						securityCamera.Destroy();
					}
					break;
				}
				case 9:
					characterBase = hitInfo.collider.gameObject.GetComponent<CharacterBase>();
					if (characterBase == null)
					{
						characterBase = hitInfo.collider.transform.parent.parent.gameObject.GetComponent<CharacterBase>();
					}
					if (characterBase != null)
					{
						characterBase.TakeDamage(m_Damage, m_User.gameObject);
					}
					break;
				case 14:
					Globals.m_PlayerController.TakeDamage(m_Damage, m_User.gameObject);
					break;
				}
			}
			m_TracerCounter++;
			if (PlayTracer && m_TracerFX != null && m_TracerCounter >= m_TracerBullets && m_TracerBullets != 0)
			{
				GameObject obj = (GameObject)UnityEngine.Object.Instantiate(m_TracerFX, m_MuzzleFlashAttachObject.transform.position, Quaternion.LookRotation(ray.direction));
				UnityEngine.Object.Destroy(obj, 0.5f);
				m_TracerCounter = 0;
			}
			RaycastHit hitInfo2;
			if ((!flag || hitInfo.collider.gameObject.layer != 14) && m_User != Globals.m_PlayerController && Globals.m_PlayerController.m_BulletWhizCollider.Raycast(ray, out hitInfo2, 100f))
			{
				GameObject gameObject = new GameObject("temp bullet whiz GO");
				gameObject.transform.position = hitInfo2.point;
				EventManager.Instance.PostEvent(m_BulletWhizSoundEventName, EventAction.PlaySound, null, gameObject);
				UnityEngine.Object.Destroy(gameObject, 1f);
			}
		}
		else
		{
			Vector3 vector2 = vector - m_MuzzleFlashAttachObject.transform.position;
			vector2.Normalize();
			GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(m_ProjectilePrefab, m_MuzzleFlashAttachObject.transform.position + vector2 * 0.1f, Quaternion.LookRotation(vector2));
			gameObject2.GetComponent<ProjectileCrossbow>().m_Firer = this;
		}
	}

	protected virtual void PlayerFiredBullet()
	{
		if (!(m_User != Globals.m_PlayerController))
		{
			Globals.m_HUD.SetCurrentAmmo(m_CurrentAmmo);
			Globals.m_HUD.Kickback(m_HUDKick);
			float kick = GetKick();
			float num = kick - Mathf.Pow(m_FireTimer * 0.1f, 0.5f);
			if (num < 0.05f)
			{
				num = 0.05f;
			}
			Globals.m_CameraController.RotatePitch(0f - num);
			Globals.m_CameraController.RotateYaw(0f - kick + UnityEngine.Random.Range(0f, 1f) * kick * 2f);
			if (!m_Silenced)
			{
				Globals.m_AIDirector.CheckAudioSenses(Globals.m_PlayerController.transform.position, 25f, DisturbanceEvent.MajorAudio, true);
				Globals.m_AIDirector.ScareNearbyNPCs(Globals.m_PlayerController.transform.position, 25f);
			}
		}
	}

	public void SetUser(CharacterBase user)
	{
		m_User = user;
	}

	public virtual void ToggleHostered()
	{
		if (m_WeaponState != WeaponState.Holstered)
		{
			m_WeaponState = WeaponState.Holstering;
			if (Globals.m_PlayerController.m_CameraMode == PlayerController.CameraMode.First)
			{
				SetFirstPersonAnim(FirstPersonAnimation.Holster);
				EventManager.Instance.PostEvent(m_HolsterSoundEventName, EventAction.PlaySound, null, base.gameObject);
			}
			else
			{
				m_ModelThirdPersonPlayer.SetActiveRecursively(false);
			}
		}
		else if (m_WeaponState == WeaponState.Holstering || m_WeaponState == WeaponState.Holstered)
		{
			m_WeaponState = WeaponState.Drawing;
			if (Globals.m_PlayerController.m_CameraMode == PlayerController.CameraMode.First)
			{
				SetFirstPersonAnim(FirstPersonAnimation.Draw);
				EventManager.Instance.PostEvent(m_DrawSoundEventName, EventAction.PlaySound, null, base.gameObject);
			}
			else
			{
				m_ModelThirdPersonPlayer.SetActiveRecursively(true);
			}
		}
	}

	public void Holster()
	{
		m_WeaponState = WeaponState.Holstered;
		PlayFirstPersonHolster(true);
	}

	public void Unholster()
	{
		m_WeaponState = WeaponState.Drawing;
		PlayFirstPersonDraw(true);
	}

	public virtual void SetFirstPerson()
	{
		m_MuzzleFlashAttachObject = m_MuzzleFlashAttachFirstPerson;
		SetupFireFX();
		if (m_WeaponState == WeaponState.Holstered)
		{
			PlayFirstPersonHolster(true);
			Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(FirstPersonAnimation.Holster)].time = Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(FirstPersonAnimation.Holster)].length;
			m_ModelFirstPerson.animation[GetFirstPersonWeaponAnimName(FirstPersonAnimation.Holster)].time = m_ModelFirstPerson.animation[GetFirstPersonWeaponAnimName(FirstPersonAnimation.Holster)].length;
		}
		else
		{
			SetFirstPersonAnim(FirstPersonAnimation.Idle, true);
		}
	}

	public virtual void SetThirdPerson()
	{
		m_MuzzleFlashAttachObject = m_MuzzleFlashAttachThirdPerson;
		SetupFireFX();
		if (m_WeaponState == WeaponState.Holstered || m_WeaponState == WeaponState.Holstering)
		{
			m_ModelThirdPersonPlayer.SetActiveRecursively(false);
		}
	}

	public virtual void SetEnemy()
	{
		m_MuzzleFlashAttachObject = m_MuzzleFlashAttachEnemy;
		SetupFireFX();
	}

	public virtual void SetFirstPersonAnim(FirstPersonAnimation anim, bool force = false)
	{
		if (m_ModelFirstPerson.active && (m_CurrentFirstPersonAnimation != anim || force))
		{
			m_CurrentFirstPersonAnimation = anim;
			switch (anim)
			{
			case FirstPersonAnimation.None:
				StopFirstPersonAnims();
				break;
			case FirstPersonAnimation.Idle:
				PlayFirstPersonIdle(force);
				break;
			case FirstPersonAnimation.Fire:
				PlayFirstPersonFire(force);
				break;
			case FirstPersonAnimation.Reload:
				PlayFirstPersonReload(force);
				break;
			case FirstPersonAnimation.Walk:
				PlayFirstPersonWalk(force);
				break;
			case FirstPersonAnimation.Draw:
				PlayFirstPersonDraw(force);
				break;
			case FirstPersonAnimation.Holster:
				PlayFirstPersonHolster(force);
				break;
			case FirstPersonAnimation.ThrowGrenade:
				PlayFirstPersonThrowGrenade(force);
				break;
			}
		}
	}

	public virtual string GetFirstPersonWeaponAnimName(FirstPersonAnimation anim)
	{
		switch (anim)
		{
		case FirstPersonAnimation.None:
			return "none";
		case FirstPersonAnimation.Idle:
			return "Idle";
		case FirstPersonAnimation.Fire:
			return "Fire";
		case FirstPersonAnimation.Reload:
			return "Reload";
		case FirstPersonAnimation.Walk:
			return "Walk";
		case FirstPersonAnimation.Draw:
			return "Holster";
		case FirstPersonAnimation.Holster:
			return "Holster";
		case FirstPersonAnimation.ThrowGrenade:
			return "throwGrenade";
		default:
			return null;
		}
	}

	public virtual float GetFirstPersonAnimSpeedMod(FirstPersonAnimation anim)
	{
		switch (anim)
		{
		case FirstPersonAnimation.None:
			return 1f;
		case FirstPersonAnimation.Idle:
			return 1f;
		case FirstPersonAnimation.Fire:
			return 1f;
		case FirstPersonAnimation.Reload:
			return 1f;
		case FirstPersonAnimation.Walk:
			return 1f;
		case FirstPersonAnimation.Draw:
			return -1f;
		case FirstPersonAnimation.Holster:
			return 1f;
		case FirstPersonAnimation.ThrowGrenade:
			return 1f;
		default:
			return 1f;
		}
	}

	public virtual string GetFirstPersonModelAnimName(FirstPersonAnimation anim)
	{
		switch (anim)
		{
		case FirstPersonAnimation.ThrowGrenade:
			return "throwGrenade";
		case FirstPersonAnimation.None:
			return "none";
		default:
			return null;
		}
	}

	public float GetFirstPersonAnimLength(FirstPersonAnimation anim)
	{
		return Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(anim)].length;
	}

	public void StopFirstPersonAnims()
	{
		Globals.m_PlayerController.m_ModelFirstPerson.animation.Stop();
		m_ModelFirstPerson.animation.Stop();
	}

	public void PlayFirstPersonIdle(bool restart)
	{
		float fadeLength = 0.1f;
		if (restart)
		{
			fadeLength = 0f;
		}
		Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(FirstPersonAnimation.Idle)].wrapMode = WrapMode.Loop;
		Globals.m_PlayerController.m_ModelFirstPerson.animation.CrossFade(GetFirstPersonModelAnimName(FirstPersonAnimation.Idle), fadeLength);
		m_ModelFirstPerson.animation[GetFirstPersonWeaponAnimName(FirstPersonAnimation.Idle)].wrapMode = WrapMode.Loop;
		m_ModelFirstPerson.animation.CrossFade(GetFirstPersonWeaponAnimName(FirstPersonAnimation.Idle), fadeLength);
	}

	private void PlayFirstPersonFire(bool restart)
	{
		float speed = Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(FirstPersonAnimation.Fire)].length / m_FireRate;
		switch (m_FireType)
		{
		case FireType.RapidFire:
			Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(FirstPersonAnimation.Fire)].wrapMode = WrapMode.Loop;
			m_ModelFirstPerson.animation[GetFirstPersonWeaponAnimName(FirstPersonAnimation.Fire)].wrapMode = WrapMode.Loop;
			break;
		case FireType.SingleShot:
		case FireType.SemiAuto:
			Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(FirstPersonAnimation.Fire)].wrapMode = WrapMode.Once;
			m_ModelFirstPerson.animation[GetFirstPersonWeaponAnimName(FirstPersonAnimation.Fire)].wrapMode = WrapMode.Once;
			break;
		}
		float fadeLength = 0.1f;
		if (restart)
		{
			fadeLength = 0f;
		}
		Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(FirstPersonAnimation.Fire)].speed = speed;
		Globals.m_PlayerController.m_ModelFirstPerson.animation.CrossFade(GetFirstPersonModelAnimName(FirstPersonAnimation.Fire), fadeLength);
		m_ModelFirstPerson.animation[GetFirstPersonWeaponAnimName(FirstPersonAnimation.Fire)].speed = speed;
		m_ModelFirstPerson.animation.CrossFade(GetFirstPersonWeaponAnimName(FirstPersonAnimation.Fire), fadeLength);
	}

	private void PlayFirstPersonReload(bool restart)
	{
		Globals.m_PlayerController.m_ModelFirstPerson.animation.CrossFade(GetFirstPersonModelAnimName(FirstPersonAnimation.Reload));
		m_ModelFirstPerson.animation.CrossFade(GetFirstPersonWeaponAnimName(FirstPersonAnimation.Reload));
	}

	private void PlayFirstPersonWalk(bool restart)
	{
		Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(FirstPersonAnimation.Walk)].wrapMode = WrapMode.Loop;
		Globals.m_PlayerController.m_ModelFirstPerson.animation.CrossFade(GetFirstPersonModelAnimName(FirstPersonAnimation.Walk));
		m_ModelFirstPerson.animation[GetFirstPersonWeaponAnimName(FirstPersonAnimation.Walk)].wrapMode = WrapMode.Loop;
		m_ModelFirstPerson.animation.CrossFade(GetFirstPersonWeaponAnimName(FirstPersonAnimation.Walk));
	}

	private void PlayFirstPersonDraw(bool restart)
	{
		Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(FirstPersonAnimation.Draw)].speed = GetFirstPersonAnimSpeedMod(m_CurrentFirstPersonAnimation);
		if (GetFirstPersonAnimSpeedMod(m_CurrentFirstPersonAnimation) < 0f)
		{
			Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(FirstPersonAnimation.Draw)].time = Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(FirstPersonAnimation.Draw)].length;
		}
		Globals.m_PlayerController.m_ModelFirstPerson.animation.Play(GetFirstPersonModelAnimName(FirstPersonAnimation.Draw));
		m_ModelFirstPerson.animation[GetFirstPersonWeaponAnimName(FirstPersonAnimation.Draw)].speed *= GetFirstPersonAnimSpeedMod(m_CurrentFirstPersonAnimation);
		if (GetFirstPersonAnimSpeedMod(m_CurrentFirstPersonAnimation) < 0f)
		{
			m_ModelFirstPerson.animation[GetFirstPersonWeaponAnimName(FirstPersonAnimation.Draw)].time = m_ModelFirstPerson.animation[GetFirstPersonWeaponAnimName(FirstPersonAnimation.Holster)].length;
		}
		m_ModelFirstPerson.animation.CrossFade(GetFirstPersonWeaponAnimName(FirstPersonAnimation.Draw));
	}

	private void PlayFirstPersonHolster(bool restart)
	{
		if (!(Globals.m_PlayerController == null) && !(Globals.m_PlayerController.m_ModelFirstPerson == null) && !(m_ModelFirstPerson == null))
		{
			Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(FirstPersonAnimation.Holster)].speed = 1f;
			Globals.m_PlayerController.m_ModelFirstPerson.animation.CrossFade(GetFirstPersonModelAnimName(FirstPersonAnimation.Holster));
			m_ModelFirstPerson.animation[GetFirstPersonWeaponAnimName(FirstPersonAnimation.Holster)].speed = 1f;
			m_ModelFirstPerson.animation.CrossFade(GetFirstPersonWeaponAnimName(FirstPersonAnimation.Holster));
		}
	}

	private void PlayFirstPersonThrowGrenade(bool restart)
	{
		Globals.m_PlayerController.m_ModelFirstPerson.animation.CrossFade(GetFirstPersonModelAnimName(FirstPersonAnimation.ThrowGrenade));
	}

	private void UpdateFirstPersonAnimSpeed()
	{
		if (m_CurrentFirstPersonAnimation != FirstPersonAnimation.Fire && m_CurrentFirstPersonAnimation != FirstPersonAnimation.ThrowGrenade)
		{
			float speed = m_FirstPersonAnimationSpeed * GetFirstPersonAnimSpeedMod(m_CurrentFirstPersonAnimation);
			Globals.m_PlayerController.m_ModelFirstPerson.animation[GetFirstPersonModelAnimName(m_CurrentFirstPersonAnimation)].speed = speed;
			m_ModelFirstPerson.animation[GetFirstPersonWeaponAnimName(m_CurrentFirstPersonAnimation)].speed = speed;
		}
	}
}
