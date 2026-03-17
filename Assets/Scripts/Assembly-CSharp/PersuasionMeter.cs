using System;
using UnityEngine;

[Serializable]
public class PersuasionMeter
{
	public BTButton m_MeterFrame;

	public SpriteText m_MeterText;

	public PackedSprite[] m_MeterTicks;

	public PackedSprite m_MeterTickBackground;

	public float m_MovementRate = 5f;

	public float m_PersuasionEasing = 5f;

	private bool m_hidden;

	private float m_PersuasionAmount;

	private static float Top = 3.4f;

	private static float Bottom = -2.6f;

	private static float Left = -4f;

	private static float Right = 62f;

	public void SetPersuasionAmount(float amount)
	{
		m_PersuasionAmount = amount;
	}

	public void Hide(bool hide)
	{
		m_hidden = hide;
		m_MeterFrame.Hide(hide);
		m_MeterText.Hide(hide);
		m_MeterTickBackground.Hide(hide);
		float num = m_PersuasionAmount / 100f;
		for (int i = 0; i < m_MeterTicks.Length; i++)
		{
			m_MeterTicks[i].Hide(hide);
			if (!hide)
			{
				Vector3 localPosition = m_MeterTicks[i].transform.localPosition;
				localPosition.y = Top - (Top - Bottom) * (1f - num);
				m_MeterTicks[i].transform.localPosition = localPosition;
			}
		}
	}

	public void UpdateTicks(float persuasionAmount)
	{
		if (m_hidden)
		{
			return;
		}
		if (m_PersuasionAmount > persuasionAmount)
		{
			m_PersuasionAmount = Mathf.Max(m_PersuasionAmount - m_PersuasionEasing * Time.deltaTime, persuasionAmount);
		}
		else if (m_PersuasionAmount < persuasionAmount)
		{
			m_PersuasionAmount = Mathf.Min(m_PersuasionAmount + m_PersuasionEasing * Time.deltaTime, persuasionAmount);
		}
		float num = m_PersuasionAmount / 100f;
		for (int i = 0; i < m_MeterTicks.Length; i++)
		{
			Vector3 localPosition = m_MeterTicks[i].transform.localPosition;
			localPosition.x -= m_MovementRate * Time.deltaTime;
			if (localPosition.x <= Left)
			{
				localPosition.x += Right - Left;
				localPosition.y = Top - (Top - Bottom) * (1f - num);
			}
			m_MeterTicks[i].transform.localPosition = localPosition;
		}
	}
}
