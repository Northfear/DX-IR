using UnityEngine;

public class AutoDestruct : MonoBehaviour
{
	private ParticleSystem m_ParticleSystem;

	private void Awake()
	{
		m_ParticleSystem = base.particleSystem;
		if (m_ParticleSystem == null)
		{
			Object.Destroy(this);
		}
	}

	private void LateUpdate()
	{
		if (!m_ParticleSystem.IsAlive())
		{
			Object.Destroy(base.gameObject);
		}
	}
}
