using System;
using UnityEngine;

[Serializable]
public class BridgeSpriteInfo
{
	public Material m_material;

	public Vector2 m_spriteDimensions = new Vector2(6f, 2f);

	public Vector2 m_pixelDimensions = new Vector2(64f, 32f);

	public float m_spacing = 0.25f;
}
