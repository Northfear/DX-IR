using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class InteractiveObject_Doors : InteractiveObject_Base
{
	public enum DoorState
	{
		Closed = 0,
		Open = 1
	}

	public InteractiveObject_Doors m_PartnerDoor;

	public GameObject m_DoorGameObject;

	public string m_PopupStringLocked;

	public Animation m_Animator;

	public string m_DoorOpenAnimationName;

	public string m_DoorCloseAnimationName;

	public string m_SuccessSoundEffect;

	public string m_FailSoundEffect;

	public bool m_LockedFromFront;

	public bool m_LockedFromBack;

	public bool m_ShowPopup = true;

	private List<GameObject> m_NearbyEnemies = new List<GameObject>();

	private DoorState m_DoorState;

	private bool m_animating;

	public void SetDoorState(DoorState state)
	{
		m_DoorState = state;
	}

	public bool IsDoorOpen()
	{
		return m_DoorState == DoorState.Open;
	}

	public bool FacingFrontOfDoor()
	{
		Vector3 lhs = m_DoorGameObject.transform.position - Globals.m_PlayerController.m_CurrentCamera.transform.position;
		lhs.Normalize();
		float num = Vector3.Dot(lhs, m_DoorGameObject.transform.forward);
		return num > 0f;
	}

	public override string GetPopupString()
	{
		if (!m_ShowPopup)
		{
			return string.Empty;
		}
		bool flag = FacingFrontOfDoor();
		if ((flag && !m_LockedFromFront) || (!flag && !m_LockedFromBack))
		{
			return m_PopupString;
		}
		return m_PopupStringLocked;
	}

	public override bool EnableInteractiveObject(GameObject livingEntity, bool calledByPlayer = true)
	{
		if (!base.EnableInteractiveObject(livingEntity, calledByPlayer))
		{
			return false;
		}
		if (!calledByPlayer)
		{
			RegisterLivingEntity(livingEntity);
		}
		if (!calledByPlayer)
		{
			InteractWithDoor(true, false);
		}
		return true;
	}

	public override bool DisableInteractiveObject(GameObject livingEntity, bool calledByPlayer = true)
	{
		if (!base.DisableInteractiveObject(livingEntity, calledByPlayer))
		{
			return false;
		}
		int count = m_NearbyEnemies.Count;
		if (!calledByPlayer)
		{
			UnRegisterLivingEntity(livingEntity);
		}
		if (count > 0 && m_NearbyEnemies.Count == 0)
		{
			InteractWithDoor(false, false);
		}
		return true;
	}

	public override bool InteractWithObject(bool instant = false)
	{
		if (!base.InteractWithObject(instant))
		{
			return false;
		}
		bool flag = FacingFrontOfDoor();
		if ((flag && m_LockedFromFront) || (!flag && m_LockedFromBack))
		{
			SoundManager.TriggerEvent(m_FailSoundEffect, base.gameObject);
			return true;
		}
		if (IsDoorOpen())
		{
			if (m_NearbyEnemies.Count == 0)
			{
				InteractWithDoor(false, false);
			}
		}
		else
		{
			InteractWithDoor(true, false);
			UnlockDoor();
		}
		SoundManager.TriggerEvent(m_SuccessSoundEffect, base.gameObject);
		return true;
	}

	public override Vector3 GetPopupLocation()
	{
		if (!m_ShowPopup)
		{
			return InteractiveObject_Base.m_OffScreen;
		}
		if (m_PartnerDoor == null)
		{
			return GetActualPopupLocation();
		}
		Vector3 actualPopupLocation = GetActualPopupLocation();
		Vector3 actualPopupLocation2 = m_PartnerDoor.GetActualPopupLocation();
		actualPopupLocation.x = (actualPopupLocation.x + actualPopupLocation2.x) * 0.5f;
		return actualPopupLocation;
	}

	public Vector3 GetActualPopupLocation()
	{
		if (m_InteractiveCollider == null || !m_Active)
		{
			return InteractiveObject_Base.m_OffScreen;
		}
		Renderer renderer = ((!(m_ObjectRenderer == null)) ? m_ObjectRenderer : base.gameObject.GetComponentInChildren<Renderer>());
		if (renderer == null || !renderer.isVisible)
		{
			return InteractiveObject_Base.m_OffScreen;
		}
		Vector3 forward = Globals.m_PlayerController.m_Camera.transform.forward;
		forward.y = 0f;
		forward.Normalize();
		Vector3 rhs = Globals.m_PlayerController.transform.position - base.transform.position;
		rhs.y = 0f;
		rhs.Normalize();
		float num = Vector3.Dot(forward, rhs);
		if (num > 0f)
		{
			return InteractiveObject_Base.m_OffScreen;
		}
		Bounds bounds = m_InteractiveCollider.bounds;
		Vector3 vector = base.transform.InverseTransformPoint(bounds.center);
		Vector3 vector2 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector + new Vector3(0f, bounds.extents.y, 0f)));
		Vector3 vector3 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector + new Vector3(0f - bounds.extents.x, 0f, 0f)));
		Vector3 vector4 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector + new Vector3(bounds.extents.x, 0f, 0f)));
		Vector3 vector5 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector + new Vector3(0f, 0f, 0f - bounds.extents.z)));
		Vector3 vector6 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector + new Vector3(0f, 0f, bounds.extents.z)));
		Vector3 vector7 = vector3;
		Vector3 vector8 = vector4;
		if (vector5.x < vector7.x)
		{
			vector7.x = vector5.x;
		}
		if (vector4.x < vector7.x)
		{
			vector7.x = vector4.x;
		}
		if (vector6.x < vector7.x)
		{
			vector7.x = vector6.x;
		}
		if (vector5.x > vector8.x)
		{
			vector8.x = vector5.x;
		}
		if (vector3.x > vector8.x)
		{
			vector8.x = vector3.x;
		}
		if (vector6.x > vector8.x)
		{
			vector8.x = vector6.x;
		}
		Vector3 zero = Vector3.zero;
		zero.y = (float)Screen.height - vector2.y;
		zero.x = vector7.x + (vector8.x - vector7.x) * 0.5f;
		return zero;
	}

	public void InteractWithDoor(bool openDoor, bool instant = false)
	{
		if (openDoor)
		{
			m_Animator[m_DoorOpenAnimationName].speed = 1f;
			m_Animator[m_DoorOpenAnimationName].time = 0f;
			m_Animator[m_DoorOpenAnimationName].wrapMode = WrapMode.Once;
			m_Animator.Play(m_DoorOpenAnimationName);
			if (instant)
			{
				m_Animator[m_DoorOpenAnimationName].time = m_Animator[m_DoorOpenAnimationName].length;
			}
		}
		else if (m_DoorCloseAnimationName != string.Empty)
		{
			m_Animator.Play(m_DoorCloseAnimationName);
			if (instant)
			{
				m_Animator[m_DoorOpenAnimationName].time = m_Animator[m_DoorOpenAnimationName].length;
			}
		}
		else
		{
			m_Animator[m_DoorOpenAnimationName].speed = -1f;
			m_Animator[m_DoorOpenAnimationName].time = m_Animator[m_DoorOpenAnimationName].length;
			m_Animator[m_DoorOpenAnimationName].wrapMode = WrapMode.Once;
			m_Animator.Play(m_DoorOpenAnimationName);
			if (instant)
			{
				m_Animator[m_DoorOpenAnimationName].time = 0f;
			}
		}
		m_animating = true;
		m_DoorState = (openDoor ? DoorState.Open : DoorState.Closed);
		if ((bool)m_PartnerDoor)
		{
			m_PartnerDoor.SetDoorState(openDoor ? DoorState.Open : DoorState.Closed);
		}
	}

	public void RegisterLivingEntity(GameObject livingEntity)
	{
		if (!m_NearbyEnemies.Contains(livingEntity))
		{
			m_NearbyEnemies.Add(livingEntity);
		}
	}

	public void UnRegisterLivingEntity(GameObject livingEntity)
	{
		m_NearbyEnemies.Remove(livingEntity);
	}

	public void UnlockDoor()
	{
		m_LockedFromFront = false;
		m_LockedFromBack = false;
		if ((bool)m_PartnerDoor)
		{
			m_PartnerDoor.m_LockedFromFront = false;
			m_PartnerDoor.m_LockedFromBack = false;
		}
	}

	private void Update()
	{
		if (m_animating && !m_Animator.isPlaying)
		{
			if (m_DoorState == DoorState.Closed && m_DoorCloseAnimationName == null)
			{
				m_Animator[m_DoorOpenAnimationName].time = 0f;
			}
			m_animating = false;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		m_Save = true;
		if (m_LockedFromFront || ((bool)m_PartnerDoor && m_PartnerDoor.m_LockedFromFront))
		{
			m_LockedFromFront = true;
			if ((bool)m_PartnerDoor)
			{
				m_PartnerDoor.m_LockedFromFront = true;
			}
		}
		if (m_LockedFromBack || ((bool)m_PartnerDoor && m_PartnerDoor.m_LockedFromBack))
		{
			m_LockedFromBack = true;
			if ((bool)m_PartnerDoor)
			{
				m_PartnerDoor.m_LockedFromBack = true;
			}
		}
	}

	public override XmlElement SaveGame(XmlElement root, XmlDocument doc)
	{
		if (m_DoorState == DoorState.Open)
		{
			m_Activated = true;
		}
		else
		{
			m_Activated = false;
		}
		XmlElement xmlElement = base.SaveGame(root, doc);
		xmlElement.SetAttribute("LockedFromFront", m_LockedFromFront.ToString());
		xmlElement.SetAttribute("LockedFromBack", m_LockedFromBack.ToString());
		return xmlElement;
	}

	public override XmlElement LoadGame(XmlElement root)
	{
		XmlElement xmlElement = base.LoadGame(root);
		if (m_Activated)
		{
			InteractWithDoor(true, true);
		}
		m_LockedFromFront = bool.Parse(xmlElement.GetAttribute("LockedFromFront"));
		m_LockedFromBack = bool.Parse(xmlElement.GetAttribute("LockedFromBack"));
		return xmlElement;
	}
}
