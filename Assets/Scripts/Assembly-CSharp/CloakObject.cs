using UnityEngine;

public class CloakObject : MonoBehaviour
{
	public float m_StartDelay;

	public float m_StayCloakedTime;

	public Renderer m_RendererToCloak;

	public Color m_Color = new Color(0f, 1f, 0f);

	public Material m_CloakMaterial;

	private float m_Timer;

	private bool m_Activated;

	private Material m_MaterialBefore;

	private int m_MaterialStage;

	private float m_MaterialStage0Time = 0.5f;

	private float m_MaterialStage1Time = 0.6f;

	private float m_MaterialTimer;

	private bool m_WantDisable;

	private void Start()
	{
		m_Timer = 0f;
	}

	private void Update()
	{
		m_Timer += Time.deltaTime;
		if (m_Timer > m_StartDelay && !m_Activated && !m_WantDisable)
		{
			ActivateCloak();
		}
		if (m_StayCloakedTime > 0f && m_Timer > m_StartDelay + m_StayCloakedTime && m_Activated)
		{
			DeactivateCloak();
		}
		if (!m_Activated && !m_WantDisable)
		{
			return;
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
			m_RendererToCloak.material.SetFloat("_Tile", value4);
			m_RendererToCloak.material.SetFloat("_Speed", 0.5f);
			m_RendererToCloak.material.SetFloat("_Inside", 1f);
			m_RendererToCloak.material.SetFloat("_Rim", 1f);
			m_RendererToCloak.material.SetFloat("_Strength", 10f);
			break;
		}
		case 1:
		{
			float value = Mathf.Lerp(1f, 0f, num);
			m_RendererToCloak.material.SetFloat("_Inside", value);
			float value2 = Mathf.Lerp(10f, 0.5f, num);
			m_RendererToCloak.material.SetFloat("_Strength", value2);
			float value3 = Mathf.Lerp(1f, 2f, num);
			m_RendererToCloak.material.SetFloat("_Rim", value3);
			break;
		}
		}
		if (!(m_MaterialTimer <= 0f) || m_MaterialStage > 1)
		{
			return;
		}
		if (!m_WantDisable)
		{
			m_MaterialStage++;
			m_MaterialTimer = m_MaterialStage1Time;
			return;
		}
		m_MaterialStage--;
		m_MaterialTimer = m_MaterialStage0Time;
		if (m_MaterialStage < 0)
		{
			m_Timer = 0f;
			m_WantDisable = false;
			m_RendererToCloak.material = m_MaterialBefore;
			base.enabled = false;
		}
	}

	private void ActivateCloak()
	{
		m_Activated = true;
		m_MaterialBefore = m_RendererToCloak.material;
		m_RendererToCloak.material = m_CloakMaterial;
		m_RendererToCloak.material.SetColor("_Color", m_Color);
		m_MaterialTimer = m_MaterialStage0Time;
		m_MaterialStage = 0;
	}

	private void DeactivateCloak()
	{
		m_Activated = false;
		m_WantDisable = true;
		m_MaterialTimer = m_MaterialStage1Time;
		m_MaterialStage = 1;
	}
}
