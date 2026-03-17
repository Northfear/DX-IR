using UnityEngine;

public class LevelAmbSound : MonoBehaviour
{
	public float m_MinDelay;

	public float m_MaxDelay;

	private float m_DelayTimer;

	public string m_SoundEvent;

	private void Start()
	{
		m_DelayTimer = Random.Range(m_MinDelay, m_MaxDelay);
	}

	private void Update()
	{
		if (m_DelayTimer >= 0f)
		{
			m_DelayTimer -= Time.deltaTime;
			return;
		}
		SoundManager.TriggerEvent(m_SoundEvent, base.gameObject);
		m_DelayTimer = Random.Range(m_MinDelay, m_MaxDelay);
	}
}
