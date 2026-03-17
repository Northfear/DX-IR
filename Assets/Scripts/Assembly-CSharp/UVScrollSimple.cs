using UnityEngine;

public class UVScrollSimple : MonoBehaviour
{
	public int materialIndex;

	public Vector2 uvAnimationRate = new Vector2(0.01f, 0.02f);

	public string textureName = "_MainTex";

	public Vector2 uvAnimationRateTwo = new Vector2(-0.03f, -0.027f);

	public string textureNameTwo = "_Texture2";

	private Renderer m_renderer;

	private Vector2 uvOffset = Vector2.zero;

	private Vector2 uvOffsetTwo = Vector2.zero;

	private bool HasSecondTexture;

	public void Awake()
	{
		m_renderer = base.renderer;
	}

	private void Start()
	{
		uvOffset = m_renderer.materials[materialIndex].GetTextureOffset(textureName);
		if (m_renderer.materials[materialIndex].HasProperty(textureNameTwo))
		{
			uvOffsetTwo = m_renderer.materials[materialIndex].GetTextureOffset(textureNameTwo);
			HasSecondTexture = true;
		}
	}

	private void LateUpdate()
	{
		uvOffset += uvAnimationRate * Time.deltaTime;
		while (uvOffset.x >= 1f)
		{
			uvOffset.x -= 1f;
		}
		while (uvOffset.y >= 1f)
		{
			uvOffset.y -= 1f;
		}
		if (m_renderer.enabled)
		{
			m_renderer.materials[materialIndex].SetTextureOffset(textureName, uvOffset);
		}
		if (HasSecondTexture)
		{
			uvOffsetTwo += uvAnimationRateTwo * Time.deltaTime;
			while (uvOffsetTwo.x >= 1f)
			{
				uvOffsetTwo.x -= 1f;
			}
			while (uvOffsetTwo.y >= 1f)
			{
				uvOffsetTwo.y -= 1f;
			}
			if (m_renderer.enabled)
			{
				m_renderer.materials[materialIndex].SetTextureOffset(textureNameTwo, uvOffsetTwo);
			}
		}
	}
}
