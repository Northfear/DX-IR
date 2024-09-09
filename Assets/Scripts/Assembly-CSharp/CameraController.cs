using UnityEngine;

public class CameraController : TouchDragBase
{
	private bool m_DragPressed;

	[HideInInspector]
	public int m_DragTouchID = -1;

	private Vector2 m_DragHoldPosition;

	private Vector2 m_DragHoldPositionLast;

	public float m_DragSensitivityYaw = 0.35f;

	public float m_DragSensitivityPitch = 0.2f;

	private float m_DragSpeedYaw;

	private float m_DragSpeedPitch;

	public float m_DragMomentumDecayYaw = 0.35f;

	public float m_DragMomentumDecayPitch = 0.1f;

	[HideInInspector]
	public float m_DragSensitivityAdjustment = 1f;

	public float m_CameraDistThirdPerson = 0.65f;

	public float m_CameraHeightThirdPerson = 0.87f;

	public float m_CameraHeightThirdPersonStanding = 1.2f;

	[HideInInspector]
	public Vector3 m_ThirdPersonAttachOriginalPos;

	private float m_CurrentYaw;

	private float m_CurrentPitch;

	public float m_PitchMin = -70f;

	public float m_PitchMax = 70f;

	private float m_LookAtOverTimeSpeed;

	[HideInInspector]
	public float m_LookAtT;

	private CharacterBase m_LookAtCharacter;

	private Quaternion m_LookAtYaw;

	private Quaternion m_LookAtPitch;

	private Quaternion m_LookAtYawStart;

	private Quaternion m_LookAtPitchStart;

	private Quaternion m_MagnetizeYawLastFrame;

	protected Transform m_TransformYaw;

	protected Transform m_TransformPitch;

	public GameObject m_FirstPersonAttach;

	public GameObject m_ThirdPersonAttach;

	[HideInInspector]
	public float m_ZRotation;

	protected virtual void Awake()
	{
		Globals.m_CameraController = this;
		m_CurrentYaw = base.transform.parent.transform.eulerAngles.y;
		m_CurrentPitch = base.transform.eulerAngles.x;
		SetFirstPerson();
	}

	protected virtual void Start()
	{
		m_ThirdPersonAttachOriginalPos = Globals.m_CameraController.m_ThirdPersonAttach.transform.localPosition;
	}

	private void Update()
	{
		if (m_DragPressed)
		{
			RotateYaw((m_DragHoldPosition.x - m_DragHoldPositionLast.x) * m_DragSensitivityYaw * m_DragSensitivityAdjustment);
			RotatePitch((m_DragHoldPositionLast.y - m_DragHoldPosition.y) * m_DragSensitivityPitch * m_DragSensitivityAdjustment);
			m_DragSpeedYaw = m_DragHoldPosition.x - m_DragHoldPositionLast.x;
			m_DragSpeedPitch = m_DragHoldPositionLast.y - m_DragHoldPosition.y;
			m_DragHoldPositionLast = m_DragHoldPosition;
		}
		else if (m_DragSpeedYaw != 0f || m_DragSpeedPitch != 0f)
		{
			m_DragSpeedYaw *= m_DragMomentumDecayYaw;
			m_DragSpeedPitch *= m_DragMomentumDecayPitch;
			if (Mathf.Abs(m_DragSpeedYaw) < 0.01f)
			{
				m_DragSpeedYaw = 0f;
			}
			if (Mathf.Abs(m_DragSpeedPitch) < 0.01f)
			{
				m_DragSpeedPitch = 0f;
			}
			if (m_DragSpeedYaw != 0f)
			{
				RotateYaw(m_DragSpeedYaw);
			}
			if (m_DragSpeedPitch != 0f)
			{
				RotatePitch(m_DragSpeedPitch);
			}
		}
		if (m_LookAtT > 0f)
		{
			m_LookAtT -= m_LookAtOverTimeSpeed * Time.deltaTime;
			if (m_LookAtT < 0f)
			{
				m_LookAtT = 0f;
			}
			UpdateLookAtOverTime();
			float num = 0f;
			num = Quaternion.Lerp(m_LookAtYawStart, m_LookAtYaw, 1f - m_LookAtT).eulerAngles.y;
			float num2 = 0f;
			num2 = Quaternion.Lerp(m_LookAtPitchStart, m_LookAtPitch, 1f - m_LookAtT).eulerAngles.x;
			if (num2 > m_PitchMax)
			{
				num2 -= 360f;
			}
			if (m_LookAtT >= 0f)
			{
				SetRotation(num, num2);
				PlayerMovement.CameraDragged();
			}
		}
		SetRotation(m_CurrentYaw, m_CurrentPitch);
	}

	public override void ButtonPress(Vector2 position, int id)
	{
		if (m_DragTouchID == -1 && (position.x > (float)(Screen.width / 2) || position.y > (float)(Screen.height / 2)))
		{
			m_DragPressed = true;
			m_DragHoldPosition = position;
			m_DragHoldPositionLast = m_DragHoldPosition;
			m_DragTouchID = id;
			base.ButtonPress(position, id);
		}
	}

	public override void ButtonHeld(Vector2 position, int id)
	{
		if (m_DragTouchID == id && m_DragPressed)
		{
			m_DragHoldPosition = position;
			PlayerMovement.CameraDragged();
			base.ButtonHeld(position, id);
		}
	}

	public override void ButtonRelease(Vector2 position, int id)
	{
		if (m_DragTouchID == id)
		{
			m_DragPressed = false;
			m_DragTouchID = -1;
			base.ButtonRelease(position, id);
		}
	}

	public virtual void LookAt(Vector3 point, bool yawOnly = true)
	{
		Vector3 vector = point;
		vector.y = m_TransformYaw.position.y;
		Quaternion quaternion = Quaternion.LookRotation(vector - m_TransformYaw.position, new Vector3(0f, 1f, 0f));
		if (base.transform.localPosition.x != 0f)
		{
			vector -= quaternion * new Vector3(1f, 0f, 0f) * base.transform.localPosition.x;
			quaternion = Quaternion.LookRotation(vector - m_TransformYaw.position, new Vector3(0f, 1f, 0f));
		}
		float y = quaternion.eulerAngles.y;
		float num = m_CurrentPitch;
		if (!yawOnly)
		{
			vector = point;
			Vector3 vector2 = vector - m_TransformPitch.position;
			vector2.y = 0f;
			float magnitude = vector2.magnitude;
			float y2 = vector.y - m_TransformPitch.position.y;
			vector2 = new Vector3(0f, y2, magnitude);
			quaternion = Quaternion.LookRotation(vector2, new Vector3(0f, 1f, 0f));
			float num2 = base.transform.position.y - m_TransformPitch.position.y;
			if (num2 != 0f)
			{
				vector -= quaternion * new Vector3(0f, 1f, 0f) * base.transform.localPosition.y;
				vector2 = vector - m_TransformPitch.position;
				vector2.y = 0f;
				magnitude = vector2.magnitude;
				y2 = vector.y - m_TransformPitch.position.y;
				vector2 = new Vector3(0f, y2, magnitude);
				quaternion = Quaternion.LookRotation(vector2, new Vector3(0f, 1f, 0f));
			}
			num = quaternion.eulerAngles.x;
			if (num > m_PitchMax)
			{
				num -= 360f;
			}
		}
		SetRotation(y, num);
	}

	public virtual void LookAtOverTime(GameObject targetobj, float speed)
	{
		m_LookAtT = 1f;
		m_LookAtOverTimeSpeed = speed;
		m_LookAtCharacter = targetobj.GetComponent<CharacterBase>();
		m_LookAtYawStart = m_TransformYaw.rotation;
		m_LookAtPitchStart = m_TransformPitch.rotation;
		UpdateLookAtOverTime();
	}

	private void UpdateLookAtOverTime()
	{
		Vector3 chestLocation = m_LookAtCharacter.GetChestLocation();
		Vector3 vector = chestLocation;
		vector.y = m_TransformYaw.position.y;
		m_LookAtYaw = Quaternion.LookRotation(vector - m_TransformYaw.position, new Vector3(0f, 1f, 0f));
		if (base.transform.localPosition.x != 0f)
		{
			vector -= m_LookAtYaw * new Vector3(1f, 0f, 0f) * base.transform.localPosition.x;
			m_LookAtYaw = Quaternion.LookRotation(vector - m_TransformYaw.position, new Vector3(0f, 1f, 0f));
		}
		vector = chestLocation;
		Vector3 vector2 = vector - m_TransformPitch.position;
		vector2.y = 0f;
		float magnitude = vector2.magnitude;
		float y = vector.y - m_TransformPitch.position.y;
		vector2 = new Vector3(0f, y, magnitude);
		m_LookAtPitch = Quaternion.LookRotation(vector2, new Vector3(0f, 1f, 0f));
		float num = base.transform.position.y - m_TransformPitch.position.y;
		if (num != 0f)
		{
			vector -= m_LookAtPitch * new Vector3(0f, 1f, 0f) * base.transform.localPosition.y;
			vector2 = vector - m_TransformPitch.position;
			vector2.y = 0f;
			magnitude = vector2.magnitude;
			y = vector.y - m_TransformPitch.position.y;
			vector2 = new Vector3(0f, y, magnitude);
			m_LookAtPitch = Quaternion.LookRotation(vector2, new Vector3(0f, 1f, 0f));
		}
	}

	public virtual void MagnetizeTowardsTarget(GameObject targetobj, float speed, CharacterBase charscript, bool force = false)
	{
		Vector3 chestLocation = charscript.GetChestLocation();
		Vector3 vector = chestLocation;
		vector.y = m_TransformYaw.position.y;
		Quaternion quaternion = Quaternion.LookRotation(vector - m_TransformYaw.position, new Vector3(0f, 1f, 0f));
		if (base.transform.localPosition.x != 0f)
		{
			vector -= quaternion * new Vector3(1f, 0f, 0f) * base.transform.localPosition.x;
			quaternion = Quaternion.LookRotation(vector - m_TransformYaw.position, new Vector3(0f, 1f, 0f));
		}
		if (!object.Equals(quaternion, m_MagnetizeYawLastFrame) || force)
		{
			Quaternion quaternion2 = Quaternion.RotateTowards(m_TransformYaw.rotation, quaternion, Time.deltaTime * speed);
			PlayerMovement.CameraDragged();
			SetRotation(quaternion2.eulerAngles.y, m_CurrentPitch);
		}
		m_MagnetizeYawLastFrame = quaternion;
	}

	public virtual void SetYaw(float yaw)
	{
		m_CurrentYaw = yaw;
		m_TransformYaw.rotation = Quaternion.Euler(0f, m_CurrentYaw, 0f);
	}

	public virtual float GetYaw()
	{
		return m_CurrentYaw;
	}

	public virtual float GetPitch()
	{
		return m_CurrentPitch;
	}

	public virtual void SetRotation(float yaw, float pitch)
	{
		m_CurrentYaw = yaw;
		m_CurrentPitch = pitch;
		m_TransformYaw.rotation = Quaternion.identity;
		m_TransformPitch.rotation = Quaternion.identity;
		m_TransformYaw.rotation = Quaternion.Euler(0f, m_CurrentYaw, 0f);
		m_TransformPitch.Rotate(m_CurrentPitch, 0f, m_ZRotation, Space.Self);
	}

	public virtual void RotateYaw(float amount)
	{
		m_CurrentYaw += amount;
		while (m_CurrentYaw < 0f)
		{
			m_CurrentYaw += 360f;
		}
		while (m_CurrentYaw >= 360f)
		{
			m_CurrentYaw -= 360f;
		}
		m_TransformYaw.Rotate(0f, amount, 0f, Space.World);
	}

	public virtual void RotatePitch(float amount)
	{
		float num = amount;
		if (num + m_CurrentPitch < m_PitchMin)
		{
			num = m_PitchMin - m_CurrentPitch;
		}
		if (num + m_CurrentPitch > m_PitchMax)
		{
			num = m_PitchMax - m_CurrentPitch;
		}
		m_CurrentPitch += num;
		m_TransformPitch.Rotate(num, 0f, 0f, Space.Self);
	}

	public void SetFirstPerson()
	{
		if (base.transform.parent != m_FirstPersonAttach.transform)
		{
			base.transform.parent = m_FirstPersonAttach.transform;
			base.transform.localPosition = new Vector3(0f, Globals.m_PlayerController.GetCameraHeight(), 0f);
		}
		m_TransformYaw = base.transform.parent.transform;
		m_TransformPitch = base.transform;
		SetRotation(m_CurrentYaw, m_CurrentPitch);
	}

	public void SetThirdPerson()
	{
		base.transform.parent = m_ThirdPersonAttach.transform;
		float y = m_CameraHeightThirdPerson;
		if (Globals.m_PlayerController.m_Stance == PlayerController.Stance.Stand)
		{
			y = m_CameraHeightThirdPersonStanding;
		}
		base.transform.localPosition = new Vector3(0f, y, 0f - m_CameraDistThirdPerson);
		base.transform.localRotation = Quaternion.identity;
		m_TransformYaw = base.transform.parent.transform;
		m_TransformPitch = base.transform.parent.transform;
		SetRotation(m_CurrentYaw, m_CurrentPitch);
	}
}
