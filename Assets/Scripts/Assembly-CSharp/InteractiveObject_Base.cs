using System.Xml;
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

	protected bool m_Save;

	protected bool m_InteractOnLoad;

	[HideInInspector]
	public Vector3 m_SpawnPosition;

	protected bool m_Activated;

	public Vector3 m_OffScreen = new Vector3(0f, 0f, -20f);

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

	public virtual void MarkForDelete()
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

	public virtual string GetPopupString()
	{
		return m_PopupString;
	}

	protected virtual void Awake()
	{
		m_SpawnPosition = base.transform.position;
		GameManager.OnSaveGame += SaveGame;
		GameManager.OnLoadGame += LoadGame;
	}

	private void OnDestroy()
	{
		GameManager.OnSaveGame -= SaveGame;
		GameManager.OnLoadGame -= LoadGame;
		if (m_BlockCover)
		{
			Globals.m_PlayerController.m_NearInteractiveObject--;
			if (Globals.m_PlayerController.m_NearInteractiveObject < 0)
			{
				Globals.m_PlayerController.m_NearInteractiveObject = 0;
			}
		}
	}

	public virtual bool EnableInteractiveObject(GameObject livingEntity, bool calledByPlayer = true)
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

	public virtual bool DisableInteractiveObject(GameObject livingEntity, bool calledByPlayer = true)
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

	public virtual bool InteractWithObject(bool instant = false)
	{
		if ((!m_Active || !m_Enabled) && !instant)
		{
			return false;
		}
		m_Activated = !m_Activated;
		return true;
	}

	public virtual Vector3 GetPopupLocation()
	{
		if (m_InteractiveCollider == null || !m_Active || m_PopupString == string.Empty)
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

	public virtual XmlElement SaveGame(XmlElement root, XmlDocument doc)
	{
		if (!m_Save)
		{
			return null;
		}
		XmlElement xmlElement = null;
		XmlNodeList elementsByTagName = root.GetElementsByTagName("InteractiveObjects");
		foreach (XmlNode item in elementsByTagName)
		{
			if (item.Name == "InteractiveObjects")
			{
				xmlElement = (XmlElement)item;
				break;
			}
		}
		if (xmlElement == null)
		{
			return null;
		}
		XmlElement xmlElement2 = (XmlElement)xmlElement.AppendChild(doc.CreateElement("InteractiveObject"));
		xmlElement2.SetAttribute("Class", GetType().ToString());
		xmlElement2.SetAttribute("Spawn_Position", m_SpawnPosition.x + "," + m_SpawnPosition.y + "," + m_SpawnPosition.z);
		xmlElement2.SetAttribute("Activated", m_Activated.ToString());
		xmlElement2.SetAttribute("MarkedForDelete", m_MarkedForDelete.ToString());
		return xmlElement2;
	}

	public virtual XmlElement LoadGame(XmlElement root)
	{
		if (!m_Save)
		{
			return null;
		}
		XmlElement xmlElement = null;
		XmlNodeList elementsByTagName = root.GetElementsByTagName("InteractiveObjects");
		foreach (XmlNode item in elementsByTagName)
		{
			if (item.Name == "InteractiveObjects")
			{
				xmlElement = (XmlElement)item;
				break;
			}
		}
		if (xmlElement == null)
		{
			return null;
		}
		foreach (XmlNode childNode in xmlElement.ChildNodes)
		{
			if (!(childNode.Name == "InteractiveObject"))
			{
				continue;
			}
			string attribute = ((XmlElement)childNode).GetAttribute("Spawn_Position");
			if (attribute != null)
			{
				string[] array = attribute.Split(',');
				Vector3 vector = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				if (vector != m_SpawnPosition)
				{
					continue;
				}
			}
			foreach (XmlAttribute attribute2 in childNode.Attributes)
			{
				switch (attribute2.Name)
				{
				case "Activated":
					m_Activated = bool.Parse(attribute2.InnerText);
					if (m_InteractOnLoad && m_Activated)
					{
						InteractWithObject(true);
					}
					break;
				case "MarkedForDelete":
					m_MarkedForDelete = bool.Parse(attribute2.InnerText);
					break;
				}
			}
			return (XmlElement)childNode;
		}
		return null;
	}
}
