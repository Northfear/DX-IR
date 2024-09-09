using System;

[Serializable]
public class PersuasionProfile
{
	public BTButton m_ProfileFrame;

	public SpriteText m_ProfileTitleText;

	public SpriteText m_ProfileText;

	public void Hide(bool hide)
	{
		m_ProfileFrame.Hide(hide);
		m_ProfileTitleText.Hide(hide);
		m_ProfileText.Hide(hide);
	}
}
