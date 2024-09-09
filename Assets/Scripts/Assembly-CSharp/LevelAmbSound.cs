using UnityEngine;

public class LevelAmbSound : MonoBehaviour
{
	public float m_MinDelay;

	public float m_MaxDelay;

	private float m_DelayTimer;

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
		base.audio.Play();
		m_DelayTimer = Random.Range(m_MinDelay, m_MaxDelay) + base.audio.clip.length;
	}
}
