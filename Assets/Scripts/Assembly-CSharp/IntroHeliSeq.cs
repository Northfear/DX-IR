using UnityEngine;

public class IntroHeliSeq : MonoBehaviour
{
	private bool m_StartGameplay;

	public float m_SequenceTimer;

	public float m_SequenceTimerMax = 9f;

	public float m_SequenceDelay = 4f;

	private bool m_SequenceStarted;

	public float[] m_CameraTimer = new float[4];

	public GameObject m_AnimationSequence;

	public GameObject m_BensonObject;

	public GameObject m_IntroCamera;

	private int m_CurrentCameraDummy;

	public GameObject[] m_CameraDummy = new GameObject[4];

	public float[] m_CameraFOV = new float[4];

	public float[] m_CameraFOVSpeed = new float[4];

	public float[] m_cameraFOVAccel = new float[4];

	private float m_CurrentCameraFOVSpeed;

	public float m_ShowBensonTimer = 9f;

	private bool m_BensonActive;

	public float m_HideHeliTimer = 9f;

	private bool m_HeliHidden;

	public GameObject m_HeliObject;

	public Texture2D m_DXLogoTexture;

	public Texture2D m_FadeTexture;

	private float m_LogoFadeInTime = 1f;

	private float m_LogoHoldTime = 3f;

	private float m_LogoFadeOutTime = 1f;

	private float m_ScreenFadeInTime = 1f;

	private bool m_LogoFadingIn = true;

	private bool m_LogoHolding;

	private bool m_LogoFadingOut;

	private bool m_ScreenFadingIn;

	private Color m_FadeColor = Color.black;

	private Color m_LogoColor = Color.white;

	private float m_LogoAlpha;

	private float m_ScreenAlpha = 1f;

	public GameObject m_Skybox;

	public float m_SkyboxHeightOffset = -110f;

	private bool m_IcarusSparksPlaying;

	public float m_IcarusSparkFXStartTime = 3f;

	public float m_IcarusSparkFXEndTime = 6f;

	public GameObject[] m_IcarusSparkFXObject = new GameObject[2];

	private ParticleSystem[] m_IcarusSparkFXSystem = new ParticleSystem[2];

	private void Start()
	{
		m_IcarusSparkFXSystem[0] = m_IcarusSparkFXObject[0].GetComponent<ParticleSystem>();
		m_IcarusSparkFXSystem[1] = m_IcarusSparkFXObject[1].GetComponent<ParticleSystem>();
		m_IntroCamera.transform.parent = m_CameraDummy[m_CurrentCameraDummy].transform;
		m_IntroCamera.transform.localPosition = Vector3.zero;
		m_IntroCamera.transform.localEulerAngles = new Vector3(0f, 270f, 0f);
		m_IntroCamera.camera.fieldOfView = m_CameraFOV[0];
		m_CurrentCameraFOVSpeed = m_CameraFOVSpeed[0];
		m_HeliObject.SetActiveRecursively(true);
	}

	private void Update()
	{
		if (m_StartGameplay)
		{
		}
		if (m_LogoFadingIn)
		{
			m_LogoAlpha += Time.deltaTime;
			m_LogoFadeInTime -= Time.deltaTime;
			if (m_LogoFadeInTime <= 0f)
			{
				m_LogoAlpha = 1f;
				m_LogoFadingIn = false;
				m_LogoHolding = true;
			}
		}
		if (m_LogoHolding)
		{
			m_LogoHoldTime -= Time.deltaTime;
			if (m_LogoHoldTime <= 0f)
			{
				m_LogoHolding = false;
				m_LogoFadingOut = true;
			}
		}
		if (m_LogoFadingOut)
		{
			m_LogoAlpha -= Time.deltaTime;
			m_LogoFadeOutTime -= Time.deltaTime;
			if (m_LogoFadeOutTime <= 0f)
			{
				m_LogoFadingOut = false;
				m_ScreenFadingIn = true;
				m_LogoAlpha = 0f;
			}
		}
		if (m_ScreenFadingIn)
		{
			m_ScreenAlpha -= Time.deltaTime;
			m_ScreenFadeInTime -= Time.deltaTime;
			if (m_ScreenFadeInTime <= 0f)
			{
				m_ScreenFadingIn = false;
				m_ScreenAlpha = 0f;
			}
		}
		if (!m_SequenceStarted)
		{
			m_SequenceDelay -= Time.deltaTime;
			if (m_SequenceDelay <= 0f)
			{
				m_SequenceStarted = true;
				m_AnimationSequence.animation.Play();
				m_BensonObject.animation.Play();
				base.audio.Play();
			}
			return;
		}
		m_SequenceTimer += Time.deltaTime;
		m_ShowBensonTimer -= Time.deltaTime;
		if (m_ShowBensonTimer <= 0f && !m_BensonActive)
		{
			m_BensonActive = true;
			m_BensonObject.SetActiveRecursively(true);
		}
		m_HideHeliTimer -= Time.deltaTime;
		if (m_HideHeliTimer <= 0f && !m_HeliHidden)
		{
			m_HeliHidden = true;
			m_HeliObject.SetActiveRecursively(false);
		}
		if (m_SequenceTimer >= m_IcarusSparkFXStartTime && m_SequenceTimer <= m_IcarusSparkFXEndTime && !m_IcarusSparksPlaying)
		{
			m_IcarusSparksPlaying = true;
			m_IcarusSparkFXSystem[0].Play();
			m_IcarusSparkFXSystem[1].Play();
		}
		if (m_SequenceTimer >= m_IcarusSparkFXEndTime && m_IcarusSparksPlaying)
		{
			m_IcarusSparksPlaying = false;
			m_IcarusSparkFXSystem[0].Stop();
			m_IcarusSparkFXSystem[1].Stop();
		}
		if (m_SequenceTimer >= m_SequenceTimerMax)
		{
			m_IntroCamera.active = false;
			m_StartGameplay = true;
			Globals.m_PlayerController.gameObject.active = true;
			Object.Destroy(base.gameObject);
		}
		m_CurrentCameraFOVSpeed += m_cameraFOVAccel[m_CurrentCameraDummy] * Time.deltaTime;
		m_IntroCamera.camera.fieldOfView += m_CurrentCameraFOVSpeed * Time.deltaTime;
		m_CameraTimer[m_CurrentCameraDummy] -= Time.deltaTime;
		if (m_CameraTimer[m_CurrentCameraDummy] <= 0f)
		{
			m_CurrentCameraDummy++;
			m_IntroCamera.transform.parent = m_CameraDummy[m_CurrentCameraDummy].transform;
			m_IntroCamera.transform.localPosition = Vector3.zero;
			m_IntroCamera.transform.localEulerAngles = new Vector3(0f, 270f, 0f);
			m_IntroCamera.camera.fieldOfView = m_CameraFOV[m_CurrentCameraDummy];
		}
	}

	private void OnGUI()
	{
		Color fadeColor = m_FadeColor;
		fadeColor.a = m_ScreenAlpha;
		GUI.color = fadeColor;
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), m_FadeTexture, ScaleMode.StretchToFill, true, 10f);
		Color logoColor = m_LogoColor;
		logoColor.a = m_LogoAlpha;
		GUI.color = logoColor;
		Vector2 vector = new Vector2(512f, 116f);
		GUI.DrawTexture(new Rect((float)Screen.width * 0.5f - vector.x * 0.5f, (float)Screen.height * 0.5f - vector.y * 0.5f, vector.x, vector.y), m_DXLogoTexture, ScaleMode.StretchToFill, true, 10f);
		GUI.color = Color.white;
	}

	public void TriggerSoundEvent()
	{
	}
}
