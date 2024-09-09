using UnityEngine;

public class BlinkShaderSettings : MonoBehaviour
{
	public float m_OnDuration = 1f;

	public float m_OffDuration = 1f;

	public float m_ColorMultiplier = 0.5f;

	public float m_Bias = 0.7f;

	public Color m_Color = Color.white;

	public float m_BlinkOffsetSeconds = 1f;

	private void Awake()
	{
		Renderer renderer = base.renderer;
		renderer.material.SetFloat("_TimeOnDuration", m_OnDuration);
		renderer.material.SetFloat("_TimeOffDuration", m_OffDuration);
		renderer.material.SetFloat("_Multiplier", m_ColorMultiplier);
		renderer.material.SetFloat("_Bias", m_Bias);
		renderer.material.SetColor("_Color", m_Color);
		renderer.material.SetFloat("_BlinkingTimeOffsScale", m_BlinkOffsetSeconds);
		Object.Destroy(this);
	}
}
