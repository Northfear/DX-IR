using System;

[Serializable]
public class ConversationText
{
	public UIButton m_Button;

	public SimpleSprite m_LeftBackground;

	public SimpleSprite m_MiddleBackground;

	public SimpleSprite m_RightBackground;

	public SpriteText m_Text;

	public void Hide(bool tf)
	{
		m_LeftBackground.Hide(tf);
		m_MiddleBackground.Hide(tf);
		m_RightBackground.Hide(tf);
	}
}
