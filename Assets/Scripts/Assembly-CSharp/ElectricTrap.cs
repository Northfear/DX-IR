using UnityEngine;

public class ElectricTrap : MonoBehaviour
{
	public bool m_TrapActive = true;

	public ParticleSystem[] m_ElectricFX;

	private int m_EmitterCount = 1;

	private float m_DamageTimerRate = 0.25f;

	private float m_DamageTimer;

	public int m_DamageAmount = 10;

	private void Start()
	{
		m_EmitterCount = m_ElectricFX.Length - 1;
		if (m_TrapActive)
		{
			ActivateTrap();
			return;
		}
		DeactivateTrap();
		SoundManager.TriggerEvent("Stop_Electricity", base.gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (m_TrapActive && other.gameObject.tag == "Player")
		{
			Globals.m_PlayerController.TakeDamage(new DamageData(null, base.transform.position, m_DamageAmount, DamageType.EMP, false));
			m_DamageTimer = m_DamageTimerRate;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (m_TrapActive && other.gameObject.tag == "Player")
		{
			m_DamageTimer -= Time.deltaTime;
			if (m_DamageTimer <= 0f)
			{
				Globals.m_PlayerController.TakeDamage(new DamageData(null, base.transform.position, m_DamageAmount, DamageType.EMP, false));
				Globals.m_PlayerController.TakeDamage(new DamageData(null, base.transform.position, m_DamageAmount, DamageType.Normal, false));
				m_DamageTimer = m_DamageTimerRate;
			}
		}
	}

	public void ActivateTrap()
	{
		m_TrapActive = true;
		for (int num = m_EmitterCount; num >= 0; num--)
		{
			SoundManager.TriggerEvent("Play_Electricity", base.gameObject);
			m_ElectricFX[num].enableEmission = true;
		}
	}

	public void DeactivateTrap()
	{
		m_TrapActive = false;
		for (int num = m_EmitterCount; num >= 0; num--)
		{
			SoundManager.TriggerEvent("Stop_Electricity", base.gameObject);
			m_ElectricFX[num].enableEmission = false;
		}
	}

	public void ToggleActivation()
	{
		if (m_TrapActive)
		{
			DeactivateTrap();
		}
		else
		{
			ActivateTrap();
		}
	}
}
