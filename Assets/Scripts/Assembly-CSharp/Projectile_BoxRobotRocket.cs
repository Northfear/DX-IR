using System.Xml;
using UnityEngine;

public class Projectile_BoxRobotRocket : ProjectileBase
{
	[HideInInspector]
	public Vector3 m_TargetLocation = Vector3.zero;

	private float m_Lifetime;

	public float m_MaxLifetime = 7f;

	private bool m_Detonated;

	public float m_TurningSpeed = 2.5f;

	public float m_MovementSpeed = 5f;

	public float m_Acceleration = 5f;

	public float m_Damage = 200f;

	public float m_MinExplosionRadius = 0.45f;

	public float m_MaxExplosionRadius = 12f;

	private void Start()
	{
		SoundManager.TriggerEvent("Play_Rocket_Loop", base.gameObject);
	}

	private void Update()
	{
		if (Time.timeScale != 0f && !(Time.deltaTime <= 0f))
		{
			m_Lifetime += Time.deltaTime;
			m_MovementSpeed += m_Acceleration * Time.deltaTime;
			float num = m_MovementSpeed * Time.deltaTime;
			if (m_Lifetime >= m_MaxLifetime)
			{
				Explode();
			}
			if (m_Lifetime >= 0.25f && Physics.Raycast(new Ray(base.transform.position, base.transform.forward), num, 6374145))
			{
				Explode();
				return;
			}
			Quaternion to = Quaternion.LookRotation(m_TargetLocation - base.transform.position, base.transform.up);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, m_TurningSpeed);
			base.transform.position += base.transform.forward * num;
		}
	}

	private void Explode()
	{
		if (!m_Detonated)
		{
			DamageData data = new DamageData(m_Owner, base.transform.position, (int)m_Damage, DamageType.Explosive, false);
			Globals.DealSplashDamage(data, m_MinExplosionRadius, m_MaxExplosionRadius);
			Object.Instantiate(Globals.m_This.m_FragExplosion, base.transform.position, Quaternion.identity);
			SoundManager.TriggerEvent("Play_Rocket_Explosion", base.gameObject);
			m_Detonated = true;
			Object.Destroy(base.gameObject);
		}
	}

	public override XmlElement SaveGame(XmlElement root, XmlDocument doc)
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
		xmlElement2.SetAttribute("Target_Location", m_TargetLocation.x + "," + m_TargetLocation.y + "," + m_TargetLocation.z);
		xmlElement2.SetAttribute("Lifetime", m_Lifetime.ToString());
		xmlElement2.SetAttribute("Movement_Speed", m_MovementSpeed.ToString());
		return xmlElement2;
	}

	public override void LoadGame(XmlElement el)
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
			case "Target_Location":
			{
				string[] array = attribute.InnerText.Split(',');
				m_TargetLocation = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				break;
			}
			case "Lifetime":
				m_Lifetime = float.Parse(attribute.InnerText);
				break;
			case "Movement_Speed":
				m_MovementSpeed = float.Parse(attribute.InnerText);
				break;
			}
		}
	}
}
