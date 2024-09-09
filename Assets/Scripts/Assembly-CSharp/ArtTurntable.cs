using Fabric;
using UnityEngine;

public class ArtTurntable : MonoBehaviour
{
	public Texture2D m_DXLogoTexture;

	public Camera m_TurnTableUICamera;

	public float m_TurnResponsiveness = 500f;

	public float m_RotationSpeed = 1f;

	public float m_RotationSpeedMax = 1f;

	private float m_RotationFriction = 10f;

	private bool m_PressingToTurn;

	private Vector2 m_PressPosition = Vector2.zero;

	private Vector2 m_HoldPosition = Vector2.zero;

	private float m_TargetRotation;

	private float m_RotationOffset;

	private float m_SetRotation;

	public float m_TurnRange = 360f;

	public GameObject m_TurntableCamera;

	public Vector2 m_CameraRangeZ = new Vector2(-0.5f, -5f);

	public float m_CameraTargetZ = -3.333f;

	private int m_TurnTableModelIndex;

	private int m_TurnTableModelIndexMax;

	public GameObject[] m_TurnTableModel = new GameObject[2];

	private bool m_TyrantMusicOn;

	private bool m_StripperMusicOn;

	private void Start()
	{
		UIManager.instance.ResetCameras(m_TurnTableUICamera, 1024, 100f);
		m_TurnTableModelIndexMax = m_TurnTableModel.Length - 1;
		m_TurnTableModel[0].SetActiveRecursively(true);
	}

	private void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			if (Input.mousePosition.y > (float)Screen.height * 0.2f)
			{
				m_PressingToTurn = true;
				m_PressPosition = Input.mousePosition;
				m_PressPosition.x /= Screen.width;
				m_PressPosition.y /= Screen.height;
				m_TargetRotation = base.transform.localEulerAngles.y;
				m_RotationOffset = base.transform.localEulerAngles.y;
				m_SetRotation = base.transform.localEulerAngles.y;
				if (m_RotationOffset > 360f)
				{
					m_RotationOffset -= 360f;
				}
				if (m_RotationOffset < 0f)
				{
					m_RotationOffset += 360f;
				}
			}
			else
			{
				m_PressingToTurn = false;
			}
		}
		if (Input.GetButton("Fire1") && m_PressingToTurn)
		{
			m_HoldPosition = Input.mousePosition;
			m_HoldPosition.x /= Screen.width;
			m_HoldPosition.y /= Screen.height;
			m_TargetRotation = (m_PressPosition.x - m_HoldPosition.x) * m_TurnRange;
			m_RotationSpeed = m_TargetRotation + m_RotationOffset - m_SetRotation;
			m_CameraTargetZ = Input.mousePosition.y / (float)Screen.height;
			m_CameraTargetZ = Mathf.Lerp(m_CameraRangeZ.x, m_CameraRangeZ.y, m_CameraTargetZ);
		}
		if (Input.GetButtonUp("Fire1"))
		{
			m_PressingToTurn = false;
		}
		m_RotationSpeed = Mathf.Clamp(m_RotationSpeed, 0f - m_RotationSpeedMax, m_RotationSpeedMax);
		if (Mathf.Abs(m_RotationSpeed) > 3f)
		{
			if (m_RotationSpeed > 0f)
			{
				m_RotationSpeed -= m_RotationFriction * Time.deltaTime;
			}
			else
			{
				m_RotationSpeed += m_RotationFriction * Time.deltaTime;
			}
		}
		base.transform.Rotate(Vector3.up * m_RotationSpeed * Time.deltaTime, Space.World);
		float num = (m_CameraTargetZ - m_TurntableCamera.transform.position.z) * 0.333f;
		m_TurntableCamera.transform.Translate(Vector3.forward * num * Time.deltaTime, Space.World);
	}

	private void OnGUI()
	{
		Vector2 vector = new Vector2(512f, 116f);
		GUI.DrawTexture(new Rect(0f, 0f, vector.x, vector.y), m_DXLogoTexture, ScaleMode.StretchToFill, true, 10f);
	}

	private void NextModel()
	{
		m_TurnTableModel[m_TurnTableModelIndex].SetActiveRecursively(false);
		m_TurnTableModelIndex++;
		if (m_TurnTableModelIndex > m_TurnTableModelIndexMax)
		{
			m_TurnTableModelIndex = 0;
		}
		m_TurnTableModel[m_TurnTableModelIndex].SetActiveRecursively(true);
		CheckAudio();
	}

	private void PreviousModel()
	{
		m_TurnTableModel[m_TurnTableModelIndex].SetActiveRecursively(false);
		m_TurnTableModelIndex--;
		if (m_TurnTableModelIndex < 0)
		{
			m_TurnTableModelIndex = m_TurnTableModelIndexMax;
		}
		m_TurnTableModel[m_TurnTableModelIndex].SetActiveRecursively(true);
		CheckAudio();
	}

	private void CheckAudio()
	{
		if (m_TurnTableModelIndex >= 9 && m_TurnTableModelIndex <= 11)
		{
			if (!m_TyrantMusicOn)
			{
				EventManager.Instance.PostEvent("Music_Turntable", EventAction.SetSwitch, "Tyrant");
				m_TyrantMusicOn = true;
			}
		}
		else if (m_TurnTableModelIndex == 8)
		{
			EventManager.Instance.PostEvent("Music_Turntable", EventAction.SetSwitch, "Brothel");
			m_TyrantMusicOn = false;
			m_StripperMusicOn = true;
		}
		else if (m_TyrantMusicOn || m_StripperMusicOn)
		{
			EventManager.Instance.PostEvent("Music_Turntable", EventAction.SetSwitch, "Turntable");
			m_TyrantMusicOn = false;
			m_StripperMusicOn = false;
		}
		EventManager.Instance.PostEvent("TD_NT_Knee_BK_4", EventAction.StopSound, null, base.gameObject);
		EventManager.Instance.PostEvent("TD_LT_ArmLockBlade_Front_3", EventAction.StopSound, null, base.gameObject);
		if (m_TurnTableModelIndex == 4)
		{
			EventManager.Instance.PostEvent("TD_NT_Knee_BK_4", EventAction.PlaySound, null, base.gameObject);
		}
		if (m_TurnTableModelIndex == 5)
		{
			EventManager.Instance.PostEvent("TD_LT_ArmLockBlade_Front_3", EventAction.PlaySound, null, base.gameObject);
		}
	}

	private void QuitTapped()
	{
		GameManager.LoadMenu();
		FabricManager.Instance.Stop(0f);
	}
}
