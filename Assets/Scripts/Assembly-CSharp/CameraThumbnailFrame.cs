using System;
using UnityEngine;

[Serializable]
public class CameraThumbnailFrame
{
	public PackedSprite m_LostConnection;

	public PackedSprite m_NoConnection;

	public SimpleSprite m_Display;

	[HideInInspector]
	public RenderTexture m_RenderTexture;

	[HideInInspector]
	public Texture m_Texture;

	public void NoConnection()
	{
		m_LostConnection.Hide(true);
		m_NoConnection.Hide(false);
		m_Display.Hide(true);
	}

	public void LostConnection()
	{
		m_LostConnection.Hide(false);
		m_NoConnection.Hide(true);
		m_Display.Hide(true);
	}

	public void HasConnection()
	{
		m_LostConnection.Hide(true);
		m_NoConnection.Hide(true);
		m_Display.Hide(false);
	}
}
