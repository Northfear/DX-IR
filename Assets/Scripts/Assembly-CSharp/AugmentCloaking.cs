using Fabric;
using UnityEngine;

public class AugmentCloaking : AugmentBase
{
	public Texture m_WhiteTexture;

	public Material m_OverlayMaterial;

	public Material m_CloakMaterial;

	private Shader m_CloakShader;

	private Material m_PlayerFirstPersonMaterial;

	private Material m_PlayerThirdPersonMaterial;

	private Material m_PlayerFirstPersonWeaponMaterial;

	private Material m_PlayerThirdPersonWeaponMaterial;

	private Color m_OverlayColor = new Color(0.83f, 0.68f, 0.21f);

	private bool m_WantDisable;

	private float m_GlowTime = 1.1f;

	private float m_GlowTimer;

	private bool m_IconStrobeIn;

	private float m_IconStrobeTime = 0.5f;

	private float m_IconStrobeTimer;

	private int m_MaterialStage;

	private float m_MaterialStage0Time = 0.5f;

	private float m_MaterialStage1Time = 0.6f;

	private float m_MaterialTimer;

	public float m_SecondsPerEnergy = 5f;

	[HideInInspector]
	public Color m_DesiredHUDColor = Color.white;

	private PackedSprite m_FullscreenQuad;

	protected override void Awake()
	{
		base.Awake();
		Globals.m_AugmentCloaking = this;
	}

	protected override void Start()
	{
		m_FullscreenQuad = GameManager.CreateFullscreenQuad(GameManager.FullscreenQuadType.Add, 0.5f);
		m_FullscreenQuad.gameObject.active = false;
	}

	protected override void Update()
	{
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
			float value4 = Mathf.Lerp(5f, 15f, num);
			Globals.m_PlayerController.m_RendererFirstPerson.material.SetFloat("_Tile", value4);
			Globals.m_PlayerController.m_RendererThirdPerson.material.SetFloat("_Tile", value4);
			Globals.m_PlayerController.m_WeaponScript.m_RendererFirstPerson.material.SetFloat("_Tile", value4);
			Globals.m_PlayerController.m_WeaponScript.m_RendererThirdPersonPlayer.material.SetFloat("_Tile", value4);
			Globals.m_PlayerController.m_RendererFirstPerson.material.SetFloat("_Speed", 0.5f);
			Globals.m_PlayerController.m_RendererThirdPerson.material.SetFloat("_Speed", 0.5f);
			Globals.m_PlayerController.m_WeaponScript.m_RendererFirstPerson.material.SetFloat("_Speed", 0.5f);
			Globals.m_PlayerController.m_WeaponScript.m_RendererThirdPersonPlayer.material.SetFloat("_Speed", 0.5f);
			Globals.m_PlayerController.m_RendererFirstPerson.material.SetFloat("_Inside", 1f);
			Globals.m_PlayerController.m_RendererThirdPerson.material.SetFloat("_Inside", 1f);
			Globals.m_PlayerController.m_WeaponScript.m_RendererFirstPerson.material.SetFloat("_Inside", 1f);
			Globals.m_PlayerController.m_WeaponScript.m_RendererThirdPersonPlayer.material.SetFloat("_Inside", 1f);
			Globals.m_PlayerController.m_RendererFirstPerson.material.SetFloat("_Rim", 1f);
			Globals.m_PlayerController.m_RendererThirdPerson.material.SetFloat("_Rim", 1f);
			Globals.m_PlayerController.m_WeaponScript.m_RendererFirstPerson.material.SetFloat("_Rim", 1f);
			Globals.m_PlayerController.m_WeaponScript.m_RendererThirdPersonPlayer.material.SetFloat("_Rim", 1f);
			Globals.m_PlayerController.m_RendererFirstPerson.material.SetFloat("_Strength", 10f);
			Globals.m_PlayerController.m_RendererThirdPerson.material.SetFloat("_Strength", 10f);
			Globals.m_PlayerController.m_WeaponScript.m_RendererFirstPerson.material.SetFloat("_Strength", 10f);
			Globals.m_PlayerController.m_WeaponScript.m_RendererThirdPersonPlayer.material.SetFloat("_Strength", 10f);
			break;
		}
		case 1:
		{
			float value = Mathf.Lerp(1f, 0f, num);
			Globals.m_PlayerController.m_RendererFirstPerson.material.SetFloat("_Inside", value);
			Globals.m_PlayerController.m_RendererThirdPerson.material.SetFloat("_Inside", value);
			Globals.m_PlayerController.m_WeaponScript.m_RendererFirstPerson.material.SetFloat("_Inside", value);
			Globals.m_PlayerController.m_WeaponScript.m_RendererThirdPersonPlayer.material.SetFloat("_Inside", value);
			float value2 = Mathf.Lerp(10f, 0.5f, num);
			Globals.m_PlayerController.m_RendererFirstPerson.material.SetFloat("_Strength", value2);
			Globals.m_PlayerController.m_RendererThirdPerson.material.SetFloat("_Strength", value2);
			Globals.m_PlayerController.m_WeaponScript.m_RendererFirstPerson.material.SetFloat("_Strength", value2);
			Globals.m_PlayerController.m_WeaponScript.m_RendererThirdPersonPlayer.material.SetFloat("_Strength", value2);
			float value3 = Mathf.Lerp(1f, 2f, num);
			Globals.m_PlayerController.m_RendererFirstPerson.material.SetFloat("_Rim", value3);
			Globals.m_PlayerController.m_RendererThirdPerson.material.SetFloat("_Rim", value3);
			Globals.m_PlayerController.m_WeaponScript.m_RendererFirstPerson.material.SetFloat("_Rim", value3);
			Globals.m_PlayerController.m_WeaponScript.m_RendererThirdPersonPlayer.material.SetFloat("_Rim", value3);
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
			float energy = Time.deltaTime * (1f / m_SecondsPerEnergy);
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
		if (Globals.m_PlayerController.IsEnergyAvailable(0.01f))
		{
			base.enabled = true;
			m_IconStrobeTimer = m_IconStrobeTime;
			m_IconStrobeIn = false;
			m_GlowTimer = m_GlowTime;
			m_WantDisable = false;
			m_MaterialStage = 0;
			m_MaterialTimer = m_MaterialStage0Time;
			m_DesiredHUDColor = Color.green;
			m_PlayerFirstPersonMaterial = Globals.m_PlayerController.m_RendererFirstPerson.material;
			Globals.m_PlayerController.m_RendererFirstPerson.material = m_CloakMaterial;
			m_PlayerThirdPersonMaterial = Globals.m_PlayerController.m_RendererThirdPerson.material;
			Globals.m_PlayerController.m_RendererThirdPerson.material = m_CloakMaterial;
			m_PlayerFirstPersonWeaponMaterial = Globals.m_PlayerController.m_WeaponScript.m_RendererFirstPerson.material;
			Globals.m_PlayerController.m_WeaponScript.m_RendererFirstPerson.material = m_CloakMaterial;
			m_PlayerThirdPersonWeaponMaterial = Globals.m_PlayerController.m_WeaponScript.m_RendererThirdPersonPlayer.material;
			Globals.m_PlayerController.m_WeaponScript.m_RendererThirdPersonPlayer.material = m_CloakMaterial;
			EventManager.Instance.PostEvent("Cloak_Activate", EventAction.PlaySound, null, base.gameObject);
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
			EventManager.Instance.PostEvent("Cloak_Deactivate", EventAction.PlaySound, null, base.gameObject);
			EventManager.Instance.PostEvent("Cloak_Activate", EventAction.StopSound, null, base.gameObject);
		}
	}

	protected override void DisableComplete()
	{
		Globals.m_PlayerController.m_RendererFirstPerson.material = m_PlayerFirstPersonMaterial;
		Globals.m_PlayerController.m_RendererThirdPerson.material = m_PlayerThirdPersonMaterial;
		Globals.m_PlayerController.m_WeaponScript.m_RendererFirstPerson.material = m_PlayerFirstPersonWeaponMaterial;
		Globals.m_PlayerController.m_WeaponScript.m_RendererThirdPersonPlayer.material = m_PlayerThirdPersonWeaponMaterial;
		m_DesiredHUDColor = Color.white;
		base.enabled = false;
	}

	public override void PlayerWeaponSwitched()
	{
		base.PlayerWeaponSwitched();
		m_PlayerFirstPersonWeaponMaterial = Globals.m_PlayerController.m_WeaponScript.m_RendererFirstPerson.material;
		Globals.m_PlayerController.m_WeaponScript.m_RendererFirstPerson.material = m_CloakMaterial;
		m_PlayerThirdPersonWeaponMaterial = Globals.m_PlayerController.m_WeaponScript.m_RendererThirdPersonPlayer.material;
		Globals.m_PlayerController.m_WeaponScript.m_RendererThirdPersonPlayer.material = m_CloakMaterial;
	}
}
