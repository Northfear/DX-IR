using System;

[Serializable]
public class ObjectControl_Button
{
	public UIButton m_UIButton;

	public PackedSprite m_Background;

	public SpriteText m_SpriteText;

	public void Hide(bool tf)
	{
		if (m_UIButton != null)
		{
			m_UIButton.Hide(tf);
		}
		if (m_Background != null)
		{
			m_Background.Hide(tf);
		}
		if (m_SpriteText != null)
		{
			m_SpriteText.Hide(tf);
		}
	}

	public void SetActive(bool active)
	{
		if (!(m_Background == null))
		{
			m_Background.PlayAnim((!active) ? "Inactive" : "Active");
		}
	}
}
