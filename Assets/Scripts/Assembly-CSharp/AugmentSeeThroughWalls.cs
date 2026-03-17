using System.Collections.Generic;
using UnityEngine;

public class AugmentSeeThroughWalls : AugmentBase
{
	public Camera m_SeeThroughWallCamera;

	public Material m_SeeThroughWallMaterial;

	public Color m_OverlayColor = new Color(0.83f, 0.68f, 0.21f);

	private bool m_WantDisable;

	private float m_GlowTime = 0.55f;

	private float m_GlowTimer;

	private bool m_IconStrobeIn;

	private float m_IconStrobeTime = 0.5f;

	private float m_IconStrobeTimer;

	private int m_MaterialStage;

	private float m_MaterialStage0Time = 0.25f;

	private float m_MaterialStage1Time = 0.3f;

	private float m_MaterialTimer;

	private PackedSprite m_FullscreenQuad;

	private List<EnemyMaterialInfo> m_EnemyMaterialInfo = new List<EnemyMaterialInfo>();

	private List<TurretMaterialInfo> m_TurretMaterialInfo = new List<TurretMaterialInfo>();

	private List<SentryMaterialInfo> m_SentryMaterialInfo = new List<SentryMaterialInfo>();

	private List<SecrityCameraMaterialInfo> m_SecurityCameraMaterialInfo = new List<SecrityCameraMaterialInfo>();

	[HideInInspector]
	public Color m_DesiredHUDColor = Color.white;

	protected override void Awake()
	{
		base.Awake();
		Globals.m_AugmentSeeThroughWalls = this;
		m_SeeThroughWallCamera.enabled = false;
	}

	protected override void Start()
	{
		m_FullscreenQuad = GameManager.CreateFullscreenQuad(GameManager.FullscreenQuadType.Add, 0.5f);
		m_FullscreenQuad.gameObject.active = false;
	}

	protected override void Update()
	{
		if (!base.enabled)
		{
			return;
		}
		base.Update();
		m_GlowTimer -= Time.deltaTime;
		if (m_GlowTimer <= 0f && m_WantDisable)
		{
			DisableComplete();
		}
		m_MaterialTimer -= Time.deltaTime;
		float num = 1f - m_MaterialTimer / m_MaterialStage0Time;
		if (m_WantDisable)
		{
			num = 1f - num;
		}
		switch (m_MaterialStage)
		{
		case 0:
		{
			float tile = Mathf.Lerp(5f, 15f, num);
			SetMaterialData(tile, 0.5f, 1f, 1f, 10f);
			break;
		}
		case 1:
		{
			float inside = Mathf.Lerp(1f, 0f, num);
			float rim = Mathf.Lerp(1f, 2f, num);
			float strength = Mathf.Lerp(10f, 0.5f, num);
			SetMaterialData(float.MaxValue, float.MaxValue, inside, rim, strength);
			break;
		}
		}
		if (m_MaterialTimer <= 0f && m_MaterialStage <= 1)
		{
			if (!m_WantDisable)
			{
				m_MaterialStage++;
				m_MaterialTimer = m_MaterialStage1Time;
			}
			else
			{
				m_MaterialStage--;
				m_MaterialTimer = m_MaterialStage0Time;
			}
		}
		if (!m_WantDisable)
		{
			Augmentation_SmartVision augmentation_SmartVision = (Augmentation_SmartVision)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.SmartVision);
			float durationPerEnergyCell = augmentation_SmartVision.GetDurationPerEnergyCell();
			float energy = Time.deltaTime * (1f / durationPerEnergyCell);
			Globals.m_PlayerController.UseEnergy(energy);
			if (Globals.m_PlayerController.GetCurrentEnergy() <= 0f)
			{
				Disable();
			}
			m_IconStrobeTimer -= Time.deltaTime;
			if (m_IconStrobeTimer <= 0f)
			{
				m_IconStrobeTimer += m_IconStrobeTime;
				m_IconStrobeIn = !m_IconStrobeIn;
			}
			float t = m_IconStrobeTimer / m_IconStrobeTime;
			if (!m_IconStrobeIn)
			{
				t = (m_IconStrobeTime - m_IconStrobeTimer) / m_IconStrobeTime;
			}
			m_DesiredHUDColor = Color.Lerp(Color.green, Color.white, t);
		}
		Color overlayColor = m_OverlayColor;
		if (m_GlowTimer < m_GlowTime * 0.5f)
		{
			overlayColor.a = m_GlowTimer / (m_GlowTime * 0.5f);
		}
		else
		{
			overlayColor.a = (m_GlowTime - m_GlowTimer) / (m_GlowTime * 0.5f);
		}
		if (overlayColor.a > 0f)
		{
			m_FullscreenQuad.Color = overlayColor;
			m_FullscreenQuad.gameObject.active = true;
		}
		else
		{
			m_FullscreenQuad.gameObject.active = false;
		}
	}

	public override void Enable()
	{
		if (Globals.m_PlayerController.IsEnergyAvailable(0.01f, true))
		{
			Augmentation_SmartVision augmentation_SmartVision = (Augmentation_SmartVision)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.SmartVision);
			if (augmentation_SmartVision.IsSeeThroughWallsActive())
			{
				base.enabled = true;
				m_IconStrobeTimer = m_IconStrobeTime;
				m_IconStrobeIn = false;
				m_GlowTimer = m_GlowTime;
				m_WantDisable = false;
				m_MaterialStage = 0;
				m_MaterialTimer = m_MaterialStage0Time;
				SetupEnemyMaterials();
				SetupTurretMaterials();
				SetupSentryMaterials();
				Globals.m_PlayerController.m_CurrentCamera.cullingMask &= -513;
				m_SeeThroughWallCamera.enabled = true;
				SoundManager.TriggerEvent("Play_See_Through_Walls", Globals.m_PlayerController.gameObject);
				GameManager.SmartVisionToggled(true);
			}
		}
	}

	public override void Disable()
	{
		if (base.enabled)
		{
			m_GlowTimer = m_GlowTime;
			m_WantDisable = true;
			m_MaterialStage = 1;
			m_MaterialTimer = m_MaterialStage1Time;
			SoundManager.TriggerEvent("Stop_See_Through_Walls", Globals.m_PlayerController.gameObject);
		}
	}

	public void InstantDisable()
	{
		Disable();
		DisableComplete();
	}

	protected override void DisableComplete()
	{
		base.enabled = false;
		for (int i = 0; i < m_EnemyMaterialInfo.Count; i++)
		{
			if (!(m_EnemyMaterialInfo[i].m_Enemy == null))
			{
				m_EnemyMaterialInfo[i].m_Enemy.m_MeshRenderer.material = m_EnemyMaterialInfo[i].m_Material;
				m_EnemyMaterialInfo[i].m_Enemy.GetWeapon().m_ModelThirdPersonEnemy.renderer.material = m_EnemyMaterialInfo[i].m_WeaponMaterial;
			}
		}
		for (int j = 0; j < m_TurretMaterialInfo.Count; j++)
		{
			if (!(m_TurretMaterialInfo[j].m_Enemy == null))
			{
				m_TurretMaterialInfo[j].m_Enemy.m_MeshRenderer.material = m_TurretMaterialInfo[j].m_Material;
				m_TurretMaterialInfo[j].m_Enemy.m_BaseRenderer.material = m_TurretMaterialInfo[j].m_Material2;
				m_TurretMaterialInfo[j].m_Enemy.m_ArmsRenderer.material = m_TurretMaterialInfo[j].m_Material3;
				m_TurretMaterialInfo[j].m_Enemy.m_AmmoRenderer.material = m_TurretMaterialInfo[j].m_Material4;
				m_TurretMaterialInfo[j].m_Enemy.m_LeftCannonRenderer.material = m_TurretMaterialInfo[j].m_Material5;
				m_TurretMaterialInfo[j].m_Enemy.m_RightCannonRenderer.material = m_TurretMaterialInfo[j].m_Material6;
			}
		}
		for (int k = 0; k < m_SentryMaterialInfo.Count; k++)
		{
			if (!(m_SentryMaterialInfo[k].m_Enemy == null))
			{
				m_SentryMaterialInfo[k].m_Enemy.m_MeshRenderer.material = m_SentryMaterialInfo[k].m_Material;
			}
		}
		for (int l = 0; l < m_SecurityCameraMaterialInfo.Count; l++)
		{
			if (!(m_SecurityCameraMaterialInfo[l].m_Enemy == null))
			{
				m_SecurityCameraMaterialInfo[l].m_Enemy.m_NormalBaseMeshRenderer.material = m_SecurityCameraMaterialInfo[l].m_Material;
				m_SecurityCameraMaterialInfo[l].m_Enemy.m_NormalYawMeshRenderer.material = m_SecurityCameraMaterialInfo[l].m_Material2;
				m_SecurityCameraMaterialInfo[l].m_Enemy.m_NormalPitchMeshRenderer.material = m_SecurityCameraMaterialInfo[l].m_Material3;
				m_SecurityCameraMaterialInfo[l].m_Enemy.m_BrokenBaseMeshRenderer.material = m_SecurityCameraMaterialInfo[l].m_Material4;
				m_SecurityCameraMaterialInfo[l].m_Enemy.m_BrokenYawMeshRenderer.material = m_SecurityCameraMaterialInfo[l].m_Material5;
				m_SecurityCameraMaterialInfo[l].m_Enemy.m_BrokenPitchMeshRenderer.material = m_SecurityCameraMaterialInfo[l].m_Material6;
			}
		}
		m_EnemyMaterialInfo.Clear();
		m_TurretMaterialInfo.Clear();
		m_SentryMaterialInfo.Clear();
		m_SecurityCameraMaterialInfo.Clear();
		Globals.m_PlayerController.m_CurrentCamera.cullingMask |= 512;
		m_SeeThroughWallCamera.enabled = false;
		GameManager.SmartVisionToggled(false);
	}

	private void SetMaterialData(float tile, float speed, float inside, float rim, float strength)
	{
		for (int i = 0; i < m_EnemyMaterialInfo.Count; i++)
		{
			if (!(m_EnemyMaterialInfo[i].m_Enemy == null))
			{
				Material material = m_EnemyMaterialInfo[i].m_Enemy.m_Renderer.material;
				Material material2 = m_EnemyMaterialInfo[i].m_Enemy.GetWeapon().m_ModelThirdPersonEnemy.renderer.material;
				if (tile != float.MaxValue)
				{
					material.SetFloat("_Tile", tile);
					material2.SetFloat("_Tile", tile);
				}
				if (speed != float.MaxValue)
				{
					material.SetFloat("_Speed", speed);
					material2.SetFloat("_Speed", speed);
				}
				if (inside != float.MaxValue)
				{
					material.SetFloat("_Inside", inside);
					material2.SetFloat("_Inside", inside);
				}
				if (rim != float.MaxValue)
				{
					material.SetFloat("_Rim", rim);
					material2.SetFloat("_Rim", rim);
				}
				if (strength != float.MaxValue)
				{
					material.SetFloat("_Strength", strength);
					material2.SetFloat("_Strength", strength);
				}
			}
		}
	}

	private void SetupEnemyMaterials()
	{
		for (int i = 0; i < 8; i++)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = Globals.m_AIDirector.GetFirstEnemy(i); linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				EnemyMaterialInfo enemyMaterialInfo = new EnemyMaterialInfo();
				enemyMaterialInfo.m_Enemy = linkedListNode.Value;
				enemyMaterialInfo.m_Material = linkedListNode.Value.m_MeshRenderer.material;
				WeaponBase weapon = linkedListNode.Value.GetWeapon();
				enemyMaterialInfo.m_WeaponMaterial = weapon.m_ModelThirdPersonEnemy.renderer.material;
				m_EnemyMaterialInfo.Add(enemyMaterialInfo);
				linkedListNode.Value.m_MeshRenderer.material = m_SeeThroughWallMaterial;
				weapon.m_ModelThirdPersonEnemy.renderer.material = m_SeeThroughWallMaterial;
			}
		}
	}

	private void SetupTurretMaterials()
	{
		for (LinkedListNode<Turret> linkedListNode = Globals.m_AIDirector.GetFirstTurret(); linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			TurretMaterialInfo turretMaterialInfo = new TurretMaterialInfo();
			turretMaterialInfo.m_Enemy = linkedListNode.Value;
			turretMaterialInfo.m_Material = linkedListNode.Value.m_MeshRenderer.material;
			turretMaterialInfo.m_Material2 = linkedListNode.Value.m_BaseRenderer.material;
			turretMaterialInfo.m_Material3 = linkedListNode.Value.m_ArmsRenderer.material;
			turretMaterialInfo.m_Material4 = linkedListNode.Value.m_AmmoRenderer.material;
			turretMaterialInfo.m_Material5 = linkedListNode.Value.m_LeftCannonRenderer.material;
			turretMaterialInfo.m_Material6 = linkedListNode.Value.m_RightCannonRenderer.material;
			m_TurretMaterialInfo.Add(turretMaterialInfo);
			linkedListNode.Value.m_MeshRenderer.material = m_SeeThroughWallMaterial;
			linkedListNode.Value.m_BaseRenderer.material = m_SeeThroughWallMaterial;
			linkedListNode.Value.m_ArmsRenderer.material = m_SeeThroughWallMaterial;
			linkedListNode.Value.m_AmmoRenderer.material = m_SeeThroughWallMaterial;
			linkedListNode.Value.m_LeftCannonRenderer.material = m_SeeThroughWallMaterial;
			linkedListNode.Value.m_RightCannonRenderer.material = m_SeeThroughWallMaterial;
		}
	}

	private void SetupSentryMaterials()
	{
		for (LinkedListNode<Sentry> linkedListNode = Globals.m_AIDirector.GetFirstSentry(); linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			SentryMaterialInfo sentryMaterialInfo = new SentryMaterialInfo();
			sentryMaterialInfo.m_Enemy = linkedListNode.Value;
			sentryMaterialInfo.m_Material = linkedListNode.Value.m_MeshRenderer.material;
			m_SentryMaterialInfo.Add(sentryMaterialInfo);
			linkedListNode.Value.m_MeshRenderer.material = m_SeeThroughWallMaterial;
		}
	}

	private void SetupSecurityCameraMaterials()
	{
		for (LinkedListNode<SecurityCamera> linkedListNode = Globals.m_AIDirector.GetFirstSecurityCamera(); linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			SecrityCameraMaterialInfo secrityCameraMaterialInfo = new SecrityCameraMaterialInfo();
			secrityCameraMaterialInfo.m_Enemy = linkedListNode.Value;
			secrityCameraMaterialInfo.m_Material = linkedListNode.Value.m_NormalBaseMeshRenderer.material;
			secrityCameraMaterialInfo.m_Material2 = linkedListNode.Value.m_NormalYawMeshRenderer.material;
			secrityCameraMaterialInfo.m_Material3 = linkedListNode.Value.m_NormalPitchMeshRenderer.material;
			secrityCameraMaterialInfo.m_Material4 = linkedListNode.Value.m_BrokenBaseMeshRenderer.material;
			secrityCameraMaterialInfo.m_Material5 = linkedListNode.Value.m_BrokenYawMeshRenderer.material;
			secrityCameraMaterialInfo.m_Material6 = linkedListNode.Value.m_BrokenPitchMeshRenderer.material;
			m_SecurityCameraMaterialInfo.Add(secrityCameraMaterialInfo);
			linkedListNode.Value.m_NormalBaseMeshRenderer.material = m_SeeThroughWallMaterial;
			linkedListNode.Value.m_NormalYawMeshRenderer.material = m_SeeThroughWallMaterial;
			linkedListNode.Value.m_NormalPitchMeshRenderer.material = m_SeeThroughWallMaterial;
			linkedListNode.Value.m_BrokenBaseMeshRenderer.material = m_SeeThroughWallMaterial;
			linkedListNode.Value.m_BrokenYawMeshRenderer.material = m_SeeThroughWallMaterial;
			linkedListNode.Value.m_BrokenPitchMeshRenderer.material = m_SeeThroughWallMaterial;
		}
	}
}
