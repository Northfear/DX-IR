using System.Collections.Generic;
using UnityEngine;

public class TouchDragBase : MonoBehaviour
{
	private class ThumbMark
	{
		public Vector2 position;

		public float timer;
	}

	private const float m_ThumbMarkTime = 0.8f;

	private const float m_NewThumbMarkDist = 23f;

	public Texture2D m_TouchTrailImage;

	private List<ThumbMark> m_ThumbMarks = new List<ThumbMark>();

	protected bool m_TouchPressed;

	protected Vector2 m_TouchPosition;

	protected Vector2 m_LastThumbMarkPos;

	private Vector3 m_OriginalTouchPosition;

	private float m_TouchPressTimer;

	public virtual void ButtonPress(Vector2 position, int id)
	{
		m_TouchPosition = position;
		m_LastThumbMarkPos = position;
		m_TouchPressTimer = Globals.m_TapTimeLimit;
		m_OriginalTouchPosition = position;
	}

	public virtual void ButtonHeld(Vector2 position, int id)
	{
		m_TouchPosition = position;
		m_TouchPressTimer -= Time.deltaTime;
		if (m_TouchPressTimer <= 0f || Vector2.Distance(m_OriginalTouchPosition, position) > 10f)
		{
			m_TouchPressed = true;
		}
		if (Vector2.Distance(position, m_LastThumbMarkPos) > 23f)
		{
			m_LastThumbMarkPos = position;
			ThumbMark thumbMark = new ThumbMark();
			thumbMark.position = position;
			thumbMark.timer = 0.8f;
			m_ThumbMarks.Add(thumbMark);
		}
	}

	public virtual void ButtonRelease(Vector2 position, int id)
	{
		m_TouchPressed = false;
	}

	protected virtual void OnGUI()
	{
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		float num = (float)Screen.width / 1024f;
		num = ((!(num <= 1f)) ? 2f : 1f);
		float num2 = 23f * num;
		float num3 = 11f * num;
		if (m_TouchPressed)
		{
			Vector2 touchPosition = m_TouchPosition;
			touchPosition.x = (float)(int)(touchPosition.x / num2) * num2;
			touchPosition.y = (float)(int)(touchPosition.y / num3) * num3;
			GUI.DrawTexture(new Rect(touchPosition.x - (float)m_TouchTrailImage.width * num / 2f, (float)Screen.height - touchPosition.y - (float)m_TouchTrailImage.height * num / 2f, (float)m_TouchTrailImage.width * num, (float)m_TouchTrailImage.height * num), m_TouchTrailImage);
		}
		for (int num4 = m_ThumbMarks.Count - 1; num4 >= 0; num4--)
		{
			ThumbMark thumbMark = m_ThumbMarks[num4];
			Color white = Color.white;
			white.a = thumbMark.timer / 0.8f;
			GUI.color = white;
			Vector2 position = thumbMark.position;
			position.x = (float)(int)(position.x / num2) * num2;
			position.y = (float)(int)(position.y / num3) * num3;
			GUI.DrawTexture(new Rect(position.x - (float)m_TouchTrailImage.width * num / 2f, (float)Screen.height - position.y - (float)m_TouchTrailImage.height * num / 2f, (float)m_TouchTrailImage.width * num, (float)m_TouchTrailImage.height * num), m_TouchTrailImage);
			GUI.color = Color.white;
			float num5 = 1f;
			if (!m_TouchPressed)
			{
				num5 = 4f;
			}
			thumbMark.timer -= Time.deltaTime * num5;
			if (thumbMark.timer <= 0f)
			{
				m_ThumbMarks.Remove(thumbMark);
			}
		}
	}

	public virtual void CancelMovement()
	{
		m_TouchPressed = false;
	}
}
