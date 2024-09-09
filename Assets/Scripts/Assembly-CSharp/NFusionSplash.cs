using UnityEngine;

public class NFusionSplash : MonoBehaviour
{
	public enum SplashState
	{
		None = -1,
		FadeInLogo = 0,
		Idle = 1,
		FadeOut = 2,
		Total = 3
	}

	public static NFusionSplash m_This;

	private SplashState m_State = SplashState.None;

	private float m_IdleTimer = 3f;

	public Texture2D m_WhiteTexture;

	public Texture2D m_NFusionLogo;

	private Rect m_LogoRect = default(Rect);

	private Color m_BackgroundColor = Color.black;

	private Color m_LogoColor = Globals.m_ClearWhite;

	private void Awake()
	{
		m_This = this;
	}

	private void Start()
	{
		if (m_NFusionLogo != null)
		{
			m_LogoRect.width = m_NFusionLogo.width;
			m_LogoRect.height = m_NFusionLogo.height;
			m_LogoRect.x = (float)Screen.width * 0.5f - m_LogoRect.width * 0.5f;
			m_LogoRect.y = (float)Screen.height * 0.5f - m_LogoRect.height * 0.5f;
		}
	}

	public static void BeginSplash()
	{
		if (!(m_This == null))
		{
			m_This.m_State = SplashState.FadeInLogo;
		}
	}

	private void Update()
	{
		if (m_State == SplashState.FadeInLogo)
		{
			m_LogoColor.a += 0.5f * Time.deltaTime;
			if (m_LogoColor.a >= 1f)
			{
				m_LogoColor.a = 1f;
				m_State = SplashState.Idle;
			}
		}
		else if (m_State == SplashState.Idle)
		{
			m_IdleTimer -= Time.deltaTime;
			if (m_IdleTimer <= 0f)
			{
				m_State = SplashState.FadeOut;
				UIManager.instance.blockInput = false;
			}
		}
		else if (m_State == SplashState.FadeOut)
		{
			m_LogoColor.a = Mathf.Max(m_LogoColor.a - 2f * Time.deltaTime, 0f);
			m_BackgroundColor.a = Mathf.Max(m_BackgroundColor.a - 1f * Time.deltaTime, 0f);
			if (m_LogoColor.a <= 0f && m_BackgroundColor.a <= 0f)
			{
				Object.Destroy(base.gameObject);
				Resources.UnloadUnusedAssets();
			}
		}
	}

	private void OnGUI()
	{
		GUI.color = m_BackgroundColor;
		GUI.DrawTexture(Globals.m_FullScreenRect, m_WhiteTexture);
		GUI.color = m_LogoColor;
		GUI.DrawTexture(m_LogoRect, m_NFusionLogo);
	}
}
