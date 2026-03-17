using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class GrenadeBase : MonoBehaviour
{
	[HideInInspector]
	public bool m_Mine;

	public GrenadeType m_GrenadeType;

	[HideInInspector]
	public CharacterBase m_Owner;

	public float m_Damage = 200f;

	public float m_MinRadius = 0.4f;

	public float m_MaxRadius = 2f;

	public virtual void Awake()
	{
		base.rigidbody.isKinematic = true;
		base.collider.enabled = false;
	}

	public virtual void Throw(Vector3 TargetLocation)
	{
	}

	public virtual void ThrowVelocity(Vector3 Velocity)
	{
		base.transform.parent = null;
		base.rigidbody.isKinematic = false;
		base.collider.enabled = true;
		base.rigidbody.velocity = Velocity;
	}

	protected void SetOwnerFromPosition(Vector3 fpos)
	{
		if (fpos == Globals.m_PlayerController.transform.position)
		{
			m_Owner = Globals.m_PlayerController;
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = Globals.m_AIDirector.m_Squads[i].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value.transform.position == fpos)
				{
					m_Owner = linkedListNode.Value;
					return;
				}
			}
		}
	}

	public virtual XmlElement SaveGame(XmlElement root, XmlDocument doc)
	{
		XmlElement xmlElement = null;
		XmlNodeList elementsByTagName = root.GetElementsByTagName("Grenades");
		foreach (XmlNode item in elementsByTagName)
		{
			if (item.Name == "Grenades")
			{
				xmlElement = (XmlElement)item;
				break;
			}
		}
		if (xmlElement == null)
		{
			return null;
		}
		XmlElement xmlElement2 = (XmlElement)xmlElement.AppendChild(doc.CreateElement("Grenade"));
		xmlElement2.SetAttribute("GrenadeType", m_GrenadeType.ToString());
		xmlElement2.SetAttribute("Position", base.transform.position.x + "," + base.transform.position.y + "," + base.transform.position.z);
		xmlElement2.SetAttribute("Rotation", base.transform.rotation.x + "," + base.transform.rotation.y + "," + base.transform.rotation.z + "," + base.transform.rotation.w);
		xmlElement2.SetAttribute("Owner_Position", m_Owner.transform.position.x + "," + m_Owner.transform.position.y + "," + m_Owner.transform.position.z);
		xmlElement2.SetAttribute("Velocity", base.rigidbody.velocity.x + "," + base.rigidbody.velocity.y + "," + base.rigidbody.velocity.z);
		return xmlElement2;
	}

	public virtual void LoadGame(XmlElement el)
	{
		foreach (XmlAttribute attribute in el.Attributes)
		{
			switch (attribute.Name)
			{
			case "GrenadeType":
				m_GrenadeType = (GrenadeType)(int)Enum.Parse(typeof(GrenadeType), attribute.InnerText);
				break;
			case "Position":
			{
				string[] array = attribute.InnerText.Split(',');
				base.transform.position = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				break;
			}
			case "Rotation":
			{
				string[] array = attribute.InnerText.Split(',');
				base.transform.rotation = new Quaternion(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
				break;
			}
			case "Owner_Position":
			{
				string[] array = attribute.InnerText.Split(',');
				Vector3 ownerFromPosition = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				SetOwnerFromPosition(ownerFromPosition);
				break;
			}
			case "Velocity":
			{
				string[] array = attribute.InnerText.Split(',');
				Vector3 velocity = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				ThrowVelocity(velocity);
				break;
			}
			}
		}
	}
}
