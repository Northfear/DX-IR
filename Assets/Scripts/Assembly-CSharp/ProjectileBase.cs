using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
	[HideInInspector]
	public WeaponBase m_Firer;

	[HideInInspector]
	public CharacterBase m_Owner;

	public static LinkedList<ProjectileBase> m_Projectiles = new LinkedList<ProjectileBase>();

	private void Awake()
	{
		m_Projectiles.AddLast(this);
	}

	private void OnDestroy()
	{
		m_Projectiles.Remove(this);
	}

	protected void SetFirerFromPosition(Vector3 fpos)
	{
		if (fpos == Globals.m_PlayerController.transform.position)
		{
			m_Firer = Globals.m_PlayerController.m_WeaponScript;
			m_Owner = Globals.m_PlayerController;
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = Globals.m_AIDirector.m_Squads[i].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value.transform.position == fpos)
				{
					m_Firer = linkedListNode.Value.m_Weapon;
					m_Owner = linkedListNode.Value;
					return;
				}
			}
		}
	}

	public virtual XmlElement SaveGame(XmlElement root, XmlDocument doc)
	{
		XmlElement xmlElement = null;
		XmlNodeList elementsByTagName = root.GetElementsByTagName("Projectiles");
		foreach (XmlNode item in elementsByTagName)
		{
			if (item.Name == "Projectiles")
			{
				xmlElement = (XmlElement)item;
				break;
			}
		}
		if (xmlElement == null)
		{
			return null;
		}
		XmlElement xmlElement2 = (XmlElement)xmlElement.AppendChild(doc.CreateElement("Projectile"));
		xmlElement2.SetAttribute("Class", GetType().ToString());
		xmlElement2.SetAttribute("Position", base.transform.position.x + "," + base.transform.position.y + "," + base.transform.position.z);
		xmlElement2.SetAttribute("Rotation", base.transform.rotation.x + "," + base.transform.rotation.y + "," + base.transform.rotation.z + "," + base.transform.rotation.w);
		xmlElement2.SetAttribute("Firer_Position", m_Firer.m_User.transform.position.x + "," + m_Firer.m_User.transform.position.y + "," + m_Firer.m_User.transform.position.z);
		return xmlElement2;
	}

	public virtual void LoadGame(XmlElement el)
	{
		foreach (XmlAttribute attribute in el.Attributes)
		{
			switch (attribute.Name)
			{
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
			case "Firer_Position":
			{
				string[] array = attribute.InnerText.Split(',');
				Vector3 firerFromPosition = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				SetFirerFromPosition(firerFromPosition);
				break;
			}
			}
		}
	}
}
