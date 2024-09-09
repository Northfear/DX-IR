using UnityEngine;

public class MaterialTintAnim : MonoBehaviour
{
	public Color colorStart = Color.white;

	public Color colorEnd = Color.black;

	public float duration = 1f;

	private Renderer m_renderer;

	public void Awake()
	{
		m_renderer = base.renderer;
	}

	private void Start()
	{
		m_renderer.material.shader = Shader.Find("Particles/Additive");
		m_renderer.material.SetColor("_TintColor", Color.white);
	}

	private void Update()
	{
		float t = Mathf.PingPong(Time.time, duration) / duration;
		m_renderer.material.SetColor("_TintColor", Color.Lerp(colorStart, colorEnd, t));
	}
}
