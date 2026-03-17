using System;

[Serializable]
public class ObjectControl_NoConnections
{
	public PackedSprite m_PackedSprite;

	public SpriteText m_SpriteText;

	public void Hide(bool tf)
	{
		m_PackedSprite.Hide(tf);
		m_SpriteText.Hide(tf);
	}
}
