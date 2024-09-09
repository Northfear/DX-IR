using UnityEngine;

public class ScreenFade : MonoBehaviour
{
	public enum ScreenFadeState
	{
		None = -1,
		FadeIn = 0,
		Idle = 1,
		FadeOut = 2,
		Total = 3
	}

	private ScreenFadeState m_State = ScreenFadeState.None;

	private float m_Timer = -1f;

	public Texture2D m_Texture;

	public Color m_Color = Color.white;

	public int m_Depth = 1;

	public bool m_Fullscreen = true;

	private Rect m_Rect = default(Rect);

	private float m_InLength;

	private float m_IdleLength;

	private float m_OutLength;

	private void Start()
	{
		if (m_Fullscreen)
		{
			m_Rect = Globals.m_FullScreenRect;
		}
		else if (m_Texture != null)
		{
			m_Rect.width = m_Texture.width;
			m_Rect.height = m_Texture.height;
			m_Rect.x = (float)Screen.width * 0.5f - m_Rect.width * 0.5f;
			m_Rect.y = (float)Screen.height * 0.5f - m_Rect.height * 0.5f;
		}
	}

	public void StartFade(float In, float Idle = -1f, float Out = -1f)
	{
		if (!(m_Texture == null))
		{
			m_State = ScreenFadeState.FadeIn;
			m_InLength = In;
			m_IdleLength = Idle;
			m_OutLength = Out;
			m_Timer = 0f;
			m_Color.a = 0f;
		}
	}

	private void Update()
	{
		if (m_State == ScreenFadeState.FadeIn)
		{
			m_Timer += Time.deltaTime;
			m_Color.a = EZAnimation.sinusOut(m_Timer, 0f, 1f, m_InLength);
			if (m_Timer >= m_InLength)
			{
				m_Color.a = 1f;
				m_State = ScreenFadeState.Idle;
				m_Timer = 0f;
			}
		}
		else if (m_State == ScreenFadeState.Idle)
		{
			if (m_IdleLength >= 0f)
			{
				m_Timer += Time.deltaTime;
				if (m_Timer >= m_IdleLength)
				{
					m_State = ScreenFadeState.FadeOut;
					m_Timer = 0f;
				}
			}
		}
		else if (m_State == ScreenFadeState.FadeOut)
		{
			m_Timer += Time.deltaTime;
			m_Color.a = EZAnimation.sinusOut(m_Timer, 1f, -1f, m_OutLength);
			if (m_Timer >= m_OutLength)
			{
				m_State = ScreenFadeState.None;
			}
		}
	}

	private void OnGUI()
	{
		if (Event.current.type == EventType.Repaint && m_State != ScreenFadeState.None)
		{
			int depth = GUI.depth;
			Color color = GUI.color;
			GUI.depth = m_Depth;
			GUI.color = m_Color;
			GUI.DrawTexture(m_Rect, m_Texture);
			GUI.color = color;
			GUI.depth = depth;
		}
	}
}
