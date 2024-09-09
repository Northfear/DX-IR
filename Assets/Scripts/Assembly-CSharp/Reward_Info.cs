using System;
using UnityEngine;

[Serializable]
public class Reward_Info
{
	public PackedSprite m_background;

	public SpriteText m_Text;

	private GameObject m_parentObject;

	private int m_RewardAmount;

	private bool m_active;

	private float m_alpha = 1f;

	private float m_fadeInRate = 4f;

	public int GetRewardAmount()
	{
		return m_RewardAmount;
	}

	public void ResetRewardAmount()
	{
		m_RewardAmount = 0;
	}

	public void SetFadeInRate(int fadeInRate)
	{
		m_fadeInRate = fadeInRate;
	}

	public void AddToRewardAmount(int amount, string format)
	{
		m_RewardAmount += amount;
		m_Text.Text = string.Format(format, m_RewardAmount);
	}

	public void Deactivate(float yHeight)
	{
		m_alpha = 0f;
		SetAlpha(m_alpha);
		m_parentObject = m_background.gameObject.transform.parent.gameObject;
		Vector3 localPosition = m_parentObject.transform.localPosition;
		localPosition.y = yHeight;
		m_parentObject.transform.localPosition = localPosition;
	}

	public void SetAlpha(float alpha)
	{
		Color color = new Color(1f, 1f, 1f, m_alpha);
		m_background.Color = color;
		m_Text.Color = color;
	}

	public void Activate()
	{
		m_active = true;
	}

	public void UnHide()
	{
		m_background.Hide(false);
		m_Text.Hide(false);
	}

	public void Shift(float offset)
	{
		if (m_active)
		{
			Vector3 localPosition = m_parentObject.transform.localPosition;
			localPosition.y += offset;
			AnimatePosition.Do(m_parentObject, EZAnimation.ANIM_MODE.To, localPosition, EZAnimation.GetInterpolator(EZAnimation.EASING_TYPE.Linear), 1f / m_fadeInRate, 0f, null, null);
		}
	}

	public void Update()
	{
		if (m_active && m_alpha < 1f)
		{
			m_alpha = Mathf.Min(m_alpha + Time.deltaTime * m_fadeInRate, 1f);
			SetAlpha(m_alpha);
		}
	}
}
