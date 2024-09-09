using UnityEngine;

public class InteractiveObject_Base : MonoBehaviour
{
	public Collider m_InteractiveCollider;

	public string m_PopupString;

	public Renderer m_ObjectRenderer;

	public Vector3 m_PositionOffset;

	protected bool m_Active = true;

	protected bool m_Enabled;

	protected bool m_MarkedForDelete;

	protected bool m_BlockCover;

	protected static Vector3 m_OffScreen = new Vector3(0f, 0f, -20f);

	public bool IsMarkedForDelete()
	{
		return m_MarkedForDelete;
	}

	public bool IsEnabled()
	{
		return m_Enabled;
	}

	public bool IsActive()
	{
		return m_Active;
	}

	public void MarkForDelete()
	{
		m_MarkedForDelete = true;
	}

	public void MarkInactive()
	{
		m_Active = false;
	}

	public virtual bool UseInteractiveCollider()
	{
		return true;
	}

	public virtual InteractivePopup.PopupType GetPopupType()
	{
		return InteractivePopup.PopupType.Normal;
	}

	private void OnDestroy()
	{
		if (m_BlockCover)
		{
			Globals.m_PlayerController.m_NearInteractiveObject--;
			if (Globals.m_PlayerController.m_NearInteractiveObject < 0)
			{
				Globals.m_PlayerController.m_NearInteractiveObject = 0;
			}
		}
	}

	public virtual bool EnableInteractiveObject()
	{
		if (m_BlockCover)
		{
			Globals.m_PlayerController.m_NearInteractiveObject++;
		}
		if (!m_Active || m_Enabled)
		{
			return false;
		}
		m_Enabled = true;
		Globals.m_InteractiveObjectManager.EnableInteractivePopup(this, GetPopupType());
		if ((bool)m_InteractiveCollider && UseInteractiveCollider())
		{
			m_InteractiveCollider.enabled = true;
		}
		return true;
	}

	public virtual bool DisableInteractiveObject()
	{
		if (m_BlockCover)
		{
			Globals.m_PlayerController.m_NearInteractiveObject--;
			if (Globals.m_PlayerController.m_NearInteractiveObject < 0)
			{
				Globals.m_PlayerController.m_NearInteractiveObject = 0;
			}
		}
		if (!m_Active || !m_Enabled)
		{
			return false;
		}
		m_Enabled = false;
		Globals.m_InteractiveObjectManager.DisableInteractivePopup(this);
		return true;
	}

	public virtual bool InteractWithObject()
	{
		if (!m_Active || !m_Enabled)
		{
			return false;
		}
		return true;
	}

	public virtual Vector3 GetPopupLocation()
	{
		if (m_InteractiveCollider == null || !m_Active)
		{
			return m_OffScreen;
		}
		Renderer renderer = ((!(m_ObjectRenderer == null)) ? m_ObjectRenderer : base.gameObject.GetComponentInChildren<Renderer>());
		if (renderer == null || !renderer.isVisible)
		{
			return m_OffScreen;
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
			return m_OffScreen;
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

	protected virtual void Awake()
	{
	}
}
