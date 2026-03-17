using UnityEngine;

public class AugmentArmor : AugmentBase
{
	public Material m_ArmorMaterial;

	public Color m_OverlayColor = new Color(0.83f, 0.68f, 0.21f);

	public float m_ArmorDamageScaler = 2f;

	private PackedSprite m_FullscreenQuad;

	private int m_CurrentArmor;

	private Material m_PlayerFirstPersonMaterial;

	private Material m_PlayerThirdPersonMaterial;

	private bool m_EffectActive;

	private float m_GlowTime = 1.1f;

	private float m_GlowTimer;

	private float m_IconStrobeTimer;

	private int m_MaterialStage;

	private float m_CurrentTimer;

	private float m_NextTimer;

	private float m_PauseTime;

	private float m_Stage0Time;

	private float m_Stage2Time;

	private float m_StartingRimPower = 0.15f;

	private float m_MaxRimPower = 3f;

	private float m_TOA_PauseTime = 1f;

	private float m_TOA_Stage0Time = 1f;

	private float m_TOA_Stage2Time = 1f;

	private float m_TD_PauseTime = 1f;

	private float m_TD_Stage0Time = 1f;

	private float m_TD_Stage2Time = 1f;

	protected override void Awake()
	{
		base.Awake();
		Globals.m_AugmentArmor = this;
	}

	protected override void Start()
	{
		Globals.m_HUD.SetCurrentArmor(m_CurrentArmor, true);
		m_FullscreenQuad = GameManager.CreateFullscreenQuad(GameManager.FullscreenQuadType.Add, 0.5f);
		m_FullscreenQuad.gameObject.active = false;
	}

	protected override void Update()
	{
		if (m_EffectActive)
		{
			m_GlowTimer -= Time.deltaTime;
			m_CurrentTimer -= Time.deltaTime;
			m_CurrentTimer = Mathf.Max(m_CurrentTimer, 0f);
			int materialStage = m_MaterialStage;
			switch (m_MaterialStage)
			{
			case 0:
			{
				float materialData2 = m_StartingRimPower + Mathf.Lerp(0f, m_MaxRimPower - m_StartingRimPower, 1f - m_CurrentTimer / m_Stage0Time);
				SetMaterialData(materialData2);
				m_NextTimer = m_PauseTime;
				break;
			}
			case 1:
				m_NextTimer = m_Stage2Time;
				break;
			case 2:
			{
				float materialData = Mathf.Lerp(m_MaxRimPower, 8f, 1f - m_CurrentTimer / m_Stage0Time);
				SetMaterialData(materialData);
				break;
			}
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
			if (m_CurrentTimer <= 0f)
			{
				m_MaterialStage++;
			}
			if (materialStage != m_MaterialStage)
			{
				m_CurrentTimer = m_NextTimer;
			}
			if (m_MaterialStage > 2)
			{
				Disable();
			}
		}
	}

	public void Activate()
	{
		Augmentation_Armor augmentation_Armor = (Augmentation_Armor)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Armor);
		if (augmentation_Armor.IsDamageReductionPurchased() && Globals.m_PlayerController.IsEnergyAvailable(2f, true) && m_CurrentArmor < 100)
		{
			Globals.m_PlayerController.UseEnergy(2f);
			m_CurrentArmor = 100;
			Globals.m_HUD.SetCurrentArmor(m_CurrentArmor, false);
			m_GlowTimer = m_GlowTime;
			m_PauseTime = m_TOA_PauseTime;
			m_Stage0Time = m_TOA_Stage0Time;
			m_Stage2Time = m_TOA_Stage2Time;
			Enable();
			SoundManager.TriggerEvent("Play_Armor", Globals.m_PlayerController.gameObject);
		}
	}

	public override void Enable()
	{
		if (!m_EffectActive)
		{
			m_PlayerFirstPersonMaterial = Globals.m_PlayerController.m_RendererFirstPerson.material;
			m_PlayerThirdPersonMaterial = Globals.m_PlayerController.m_RendererThirdPerson.material;
			Globals.m_PlayerController.m_RendererFirstPerson.material = m_ArmorMaterial;
			Globals.m_PlayerController.m_RendererThirdPerson.material = m_ArmorMaterial;
		}
		SetMaterialData(m_StartingRimPower);
		m_EffectActive = true;
		m_MaterialStage = 0;
		m_CurrentTimer = m_Stage0Time;
	}

	public override void Disable()
	{
		if (m_EffectActive)
		{
			Globals.m_PlayerController.m_RendererFirstPerson.material = m_PlayerFirstPersonMaterial;
			Globals.m_PlayerController.m_RendererThirdPerson.material = m_PlayerThirdPersonMaterial;
		}
		m_EffectActive = false;
	}

	public void ApplyArmorDampening(ref int Damage)
	{
		if (m_CurrentArmor > 0)
		{
			Augmentation_Armor augmentation_Armor = (Augmentation_Armor)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.Armor);
			Damage -= (int)((float)Damage * (augmentation_Armor.DamageReductionAmount() / 100f));
			m_CurrentArmor = Mathf.Max(m_CurrentArmor - (int)((float)Damage * m_ArmorDamageScaler), 0);
			Globals.m_HUD.SetCurrentArmor(m_CurrentArmor, true);
			m_PauseTime = m_TD_PauseTime;
			m_Stage0Time = m_TD_Stage0Time;
			m_Stage2Time = m_TD_Stage2Time;
			Enable();
			SoundManager.TriggerEvent("Play_Armor_Hit", Globals.m_PlayerController.gameObject);
			if (m_CurrentArmor <= 0)
			{
				SoundManager.TriggerEvent("Stop_Armor", Globals.m_PlayerController.gameObject);
			}
		}
	}

	private void SetMaterialData(float rimpower)
	{
		Globals.m_PlayerController.m_RendererFirstPerson.material.SetFloat("_RimPower", rimpower);
		Globals.m_PlayerController.m_RendererThirdPerson.material.SetFloat("_RimPower", rimpower);
	}
}
