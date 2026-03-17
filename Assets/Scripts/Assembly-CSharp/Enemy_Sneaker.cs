using UnityEngine;

public class Enemy_Sneaker : Enemy_Base
{
	public enum MaterialState
	{
		Normal = 0,
		Cloaked = 1
	}

	private MaterialState m_TargetMaterialState;

	private MaterialState m_CurrentMaterialState;

	public Material m_NormalMaterial;

	public Material m_NormalWeaponMaterial;

	public Material m_CloakedMaterial;

	public Material m_CloakedWeaponMaterial;

	private Texture m_WeaponDiffuse;

	private Texture m_WeaponNormal;

	public Color m_StartNormalRimColor = Color.white;

	public Color m_EndNormalRimColor = Color.black;

	public Color m_StartCloakedRimColor = Color.white;

	public Color m_EndCloakedRimColor = new Color(0.2f, 0.2f, 0.2f, 1f);

	public float m_StartNormalRimPower = 0.1f;

	public float m_EndNormalRimPower = 0.5f;

	public float m_StartCloakedRimPower = 0.5f;

	public float m_EndCloakedRimPower = 8f;

	public float m_CloakSpeed = 3f;

	public float m_NormalSpeed = 1.5f;

	private float m_CloakingCooldown = -1f;

	private float m_CurrentPercentage = 1f;

	public override void Awake()
	{
		m_EnemyType = EnemyType.Sneaker;
		GameManager.OnSmartVisionToggled += SmartVisionToggled;
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		m_DamageModifiers[0] = 1f;
		m_DamageModifiers[1] = 1f;
		m_DamageModifiers[2] = 1f;
		m_DamageModifiers[3] = 1f;
		m_DamageModifiers[4] = 0f;
		m_DamageModifiers[6] = 1f;
		m_DamageModifiers[5] = 1f;
		m_TargetMaterialState = MaterialState.Normal;
		m_CurrentMaterialState = m_TargetMaterialState;
		m_Renderer.material = m_NormalMaterial;
		m_Renderer.material.SetColor("_RimColor", m_EndNormalRimColor);
		m_Renderer.material.SetFloat("_RimPower", m_EndNormalRimPower);
	}

	private void OnDestroy()
	{
		GameManager.OnSmartVisionToggled -= SmartVisionToggled;
	}

	protected override void Update()
	{
		base.Update();
		if (m_EnemyState == EnemyState.Death)
		{
			return;
		}
		float cloakingCooldown = m_CloakingCooldown;
		m_CloakingCooldown -= Time.deltaTime;
		if (InCombat())
		{
			if (m_CloakingCooldown <= 0f)
			{
				if (cloakingCooldown > 0f && Globals.m_PlayerController.m_TargetedEnemy == base.gameObject)
				{
					Globals.m_PlayerController.m_TargetedEnemy = null;
				}
				m_TargetMaterialState = MaterialState.Cloaked;
			}
		}
		else
		{
			m_TargetMaterialState = MaterialState.Normal;
		}
		if (m_TargetMaterialState == MaterialState.Normal)
		{
			if (m_CurrentMaterialState == MaterialState.Cloaked)
			{
				m_CurrentPercentage -= m_CloakSpeed * Time.deltaTime;
				if (m_CurrentPercentage <= 0f)
				{
					m_CurrentMaterialState = MaterialState.Normal;
					m_CurrentPercentage = 0f;
					if (!Globals.m_AugmentSeeThroughWalls.enabled)
					{
						m_Renderer.material = m_NormalMaterial;
						if (m_Weapon != null)
						{
							m_Weapon.m_RendererThirdPersonEnemy.material = m_NormalWeaponMaterial;
							m_Weapon.m_RendererThirdPersonEnemy.material.SetTexture("_MainTex", m_WeaponDiffuse);
							m_Weapon.m_RendererThirdPersonEnemy.material.SetTexture("_BumpMap", m_WeaponNormal);
						}
					}
				}
			}
			else if (m_CurrentMaterialState == MaterialState.Normal)
			{
				m_CurrentPercentage = Mathf.Clamp01(m_CurrentPercentage + m_NormalSpeed * Time.deltaTime);
			}
		}
		else if (m_TargetMaterialState == MaterialState.Cloaked)
		{
			if (m_CurrentMaterialState == MaterialState.Normal)
			{
				m_CurrentPercentage -= m_NormalSpeed * Time.deltaTime;
				if (m_CurrentPercentage <= 0f)
				{
					m_CurrentMaterialState = MaterialState.Cloaked;
					m_CurrentPercentage = 0f;
					if (!Globals.m_AugmentSeeThroughWalls.enabled)
					{
						m_Renderer.material = m_CloakedMaterial;
						if (m_Weapon != null)
						{
							m_Weapon.m_RendererThirdPersonEnemy.material = m_CloakedWeaponMaterial;
							m_Weapon.m_RendererThirdPersonEnemy.material.SetTexture("_MainTex", m_WeaponDiffuse);
							m_Weapon.m_RendererThirdPersonEnemy.material.SetTexture("_BumpMap", m_WeaponNormal);
						}
					}
				}
			}
			else if (m_CurrentMaterialState == MaterialState.Cloaked)
			{
				m_CurrentPercentage = Mathf.Clamp01(m_CurrentPercentage + m_CloakSpeed * Time.deltaTime);
			}
		}
		if (!Globals.m_AugmentSeeThroughWalls.enabled)
		{
			if (m_CurrentMaterialState == MaterialState.Normal)
			{
				m_Renderer.material.SetColor("_RimColor", Color.Lerp(m_StartNormalRimColor, m_EndNormalRimColor, m_CurrentPercentage));
				m_Renderer.material.SetFloat("_RimPower", Mathf.Lerp(m_StartNormalRimPower, m_EndNormalRimPower, m_CurrentPercentage));
				m_Weapon.m_RendererThirdPersonEnemy.material.SetColor("_RimColor", Color.Lerp(m_StartNormalRimColor, m_EndNormalRimColor, m_CurrentPercentage));
				m_Weapon.m_RendererThirdPersonEnemy.material.SetFloat("_RimPower", Mathf.Lerp(m_StartNormalRimPower, m_EndNormalRimPower, m_CurrentPercentage));
			}
			else if (m_CurrentMaterialState == MaterialState.Cloaked)
			{
				m_Renderer.material.SetColor("_RimColor", Color.Lerp(m_StartCloakedRimColor, m_EndCloakedRimColor, m_CurrentPercentage));
				m_Renderer.material.SetFloat("_RimPower", Mathf.Lerp(m_StartCloakedRimPower, m_EndCloakedRimPower, m_CurrentPercentage));
				m_Weapon.m_RendererThirdPersonEnemy.material.SetColor("_RimColor", Color.Lerp(m_StartCloakedRimColor, m_EndCloakedRimColor, m_CurrentPercentage));
				m_Weapon.m_RendererThirdPersonEnemy.material.SetFloat("_RimPower", Mathf.Lerp(m_StartCloakedRimPower, m_EndCloakedRimPower, m_CurrentPercentage));
			}
		}
	}

	private void SmartVisionToggled(bool Enabled)
	{
		if (m_EnemyState == EnemyState.Death || Enabled)
		{
			return;
		}
		if (m_CurrentMaterialState == MaterialState.Normal)
		{
			m_Renderer.material = m_NormalMaterial;
			m_Renderer.material.SetColor("_RimColor", Color.Lerp(m_StartNormalRimColor, m_EndNormalRimColor, m_CurrentPercentage));
			m_Renderer.material.SetFloat("_RimPower", Mathf.Lerp(m_StartNormalRimPower, m_EndNormalRimPower, m_CurrentPercentage));
			if (m_Weapon != null)
			{
				m_Weapon.m_RendererThirdPersonEnemy.material = m_NormalWeaponMaterial;
				m_Weapon.m_RendererThirdPersonEnemy.material.SetTexture("_MainTex", m_WeaponDiffuse);
				m_Weapon.m_RendererThirdPersonEnemy.material.SetTexture("_BumpMap", m_WeaponNormal);
				m_Weapon.m_RendererThirdPersonEnemy.material.SetColor("_RimColor", Color.Lerp(m_StartNormalRimColor, m_EndNormalRimColor, m_CurrentPercentage));
				m_Weapon.m_RendererThirdPersonEnemy.material.SetFloat("_RimPower", Mathf.Lerp(m_StartNormalRimPower, m_EndNormalRimPower, m_CurrentPercentage));
			}
		}
		else if (m_CurrentMaterialState == MaterialState.Cloaked)
		{
			m_Renderer.material = m_CloakedMaterial;
			m_Renderer.material.SetColor("_RimColor", Color.Lerp(m_StartCloakedRimColor, m_EndCloakedRimColor, m_CurrentPercentage));
			m_Renderer.material.SetFloat("_RimPower", Mathf.Lerp(m_StartCloakedRimPower, m_EndCloakedRimPower, m_CurrentPercentage));
			if (m_Weapon != null)
			{
				m_Weapon.m_RendererThirdPersonEnemy.material = m_CloakedWeaponMaterial;
				m_Weapon.m_RendererThirdPersonEnemy.material.SetTexture("_MainTex", m_WeaponDiffuse);
				m_Weapon.m_RendererThirdPersonEnemy.material.SetTexture("_BumpMap", m_WeaponNormal);
				m_Weapon.m_RendererThirdPersonEnemy.material.SetColor("_RimColor", Color.Lerp(m_StartCloakedRimColor, m_EndCloakedRimColor, m_CurrentPercentage));
				m_Weapon.m_RendererThirdPersonEnemy.material.SetFloat("_RimPower", Mathf.Lerp(m_StartCloakedRimPower, m_EndCloakedRimPower, m_CurrentPercentage));
			}
		}
	}

	public override void AssignWeapon(WeaponBase weapon)
	{
		base.AssignWeapon(weapon);
		if (m_Weapon != null)
		{
			m_WeaponDiffuse = m_Weapon.m_RendererThirdPersonEnemy.material.GetTexture("_MainTex");
			m_WeaponNormal = m_Weapon.m_RendererThirdPersonEnemy.material.GetTexture("_BumpMap");
			if (m_CurrentMaterialState == MaterialState.Normal)
			{
				m_Weapon.m_RendererThirdPersonEnemy.material = m_NormalWeaponMaterial;
				m_Weapon.m_RendererThirdPersonEnemy.material.SetTexture("_MainTex", m_WeaponDiffuse);
				m_Weapon.m_RendererThirdPersonEnemy.material.SetTexture("_BumpMap", m_WeaponNormal);
				m_Weapon.m_RendererThirdPersonEnemy.material.SetColor("_RimColor", Color.Lerp(m_StartNormalRimColor, m_EndNormalRimColor, m_CurrentPercentage));
				m_Weapon.m_RendererThirdPersonEnemy.material.SetFloat("_RimPower", Mathf.Lerp(m_StartNormalRimPower, m_EndNormalRimPower, m_CurrentPercentage));
			}
			else if (m_CurrentMaterialState == MaterialState.Cloaked)
			{
				m_Weapon.m_RendererThirdPersonEnemy.material = m_CloakedWeaponMaterial;
				m_Weapon.m_RendererThirdPersonEnemy.material.SetTexture("_MainTex", m_WeaponDiffuse);
				m_Weapon.m_RendererThirdPersonEnemy.material.SetTexture("_BumpMap", m_WeaponNormal);
				m_Weapon.m_RendererThirdPersonEnemy.material.SetColor("_RimColor", Color.Lerp(m_StartCloakedRimColor, m_EndCloakedRimColor, m_CurrentPercentage));
				m_Weapon.m_RendererThirdPersonEnemy.material.SetFloat("_RimPower", Mathf.Lerp(m_StartCloakedRimPower, m_EndCloakedRimPower, m_CurrentPercentage));
			}
		}
	}

	public override bool TakeDamage(DamageData data)
	{
		bool flag = base.TakeDamage(data);
		if (!flag)
		{
			m_CloakingCooldown = 4f;
			m_TargetMaterialState = MaterialState.Normal;
		}
		return flag;
	}

	public override void Die(DamageData data)
	{
		base.Die(data);
		GameManager.OnSmartVisionToggled -= SmartVisionToggled;
	}
}
