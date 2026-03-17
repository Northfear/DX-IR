using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Mine : GrenadeBase
{
	public DamageType m_DamageType = DamageType.Explosive;

	private float m_Speed;

	private float m_Gravity = 5f;

	private float m_DetectionRadius = 3f;

	private bool m_Blown;

	private float m_ArmTimer = 1f;

	private float m_FastBlinkTime = 0.1f;

	private float m_SlowBlinkTime = 0.8f;

	private float m_BlinkTimer = 0.8f;

	private bool m_BlinkState = true;

	private float m_GraceTime = 0.5f;

	private float m_GraceTimer;

	public GameObject m_GroundEffects;

	public GameObject m_ExplosionFX;

	private bool m_Triggered;

	public static LinkedList<Mine> m_Mines = new LinkedList<Mine>();

	public override void Awake()
	{
		m_Mine = true;
		m_Mines.AddLast(this);
	}

	private void OnDestroy()
	{
		m_Mines.Remove(this);
	}

	private void Update()
	{
		m_GroundEffects.SetActiveRecursively(false);
		if (m_GraceTimer > 0f)
		{
			m_BlinkTimer -= Time.deltaTime;
			if (m_BlinkTimer <= 0f)
			{
				m_BlinkState = !m_BlinkState;
				m_BlinkTimer = m_FastBlinkTime;
			}
			m_GroundEffects.SetActiveRecursively(m_BlinkState);
			switch (m_DamageType)
			{
			case DamageType.Explosive:
				m_GroundEffects.renderer.material.SetColor("_TintColor", new Color(1f, 0f, 0f, 0.2f));
				break;
			case DamageType.EMP:
				m_GroundEffects.renderer.material.SetColor("_TintColor", new Color(0f, 0.4f, 1f, 0.2f));
				break;
			case DamageType.Concussion:
				m_GroundEffects.renderer.material.SetColor("_TintColor", new Color(1f, 1f, 0f, 0.2f));
				break;
			}
			m_GraceTimer -= Time.deltaTime;
			if (m_GraceTimer <= 0f)
			{
				Explode();
			}
			return;
		}
		if (m_Speed != 0f)
		{
			Vector3 vector = base.transform.forward * m_Speed;
			vector.y -= m_Gravity;
			vector *= Time.deltaTime;
			m_Gravity += 5f * Time.deltaTime;
			Ray ray = new Ray(base.transform.position, vector);
			int layerMask = 82689;
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, vector.magnitude, layerMask))
			{
				base.transform.position = hitInfo.point;
				base.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
				base.transform.rotation = base.transform.rotation * Quaternion.Euler(90f, 0f, 0f);
				m_Speed = 0f;
				SoundManager.TriggerEvent("Play_Mine_Land", base.gameObject);
				if (hitInfo.collider.gameObject.layer == 14 || hitInfo.collider.gameObject.layer == 9)
				{
					BlowUp();
				}
			}
			else
			{
				base.transform.position += vector;
			}
			return;
		}
		if (m_ArmTimer > 0f)
		{
			m_ArmTimer -= Time.deltaTime;
			return;
		}
		m_BlinkTimer -= Time.deltaTime;
		if (m_BlinkTimer <= 0f)
		{
			m_BlinkState = !m_BlinkState;
			m_BlinkTimer = m_SlowBlinkTime;
			if (m_BlinkState)
			{
				SoundManager.TriggerEvent("Play_Mine_Beep", base.gameObject);
			}
		}
		m_GroundEffects.SetActiveRecursively(m_BlinkState);
		switch (m_DamageType)
		{
		case DamageType.Explosive:
			m_GroundEffects.renderer.material.SetColor("_TintColor", new Color(1f, 0f, 0f, 0.2f));
			break;
		case DamageType.EMP:
			m_GroundEffects.renderer.material.SetColor("_TintColor", new Color(0f, 0.4f, 1f, 0.2f));
			break;
		case DamageType.Concussion:
			m_GroundEffects.renderer.material.SetColor("_TintColor", new Color(1f, 1f, 0f, 0.2f));
			break;
		}
		float num = m_DetectionRadius * m_DetectionRadius;
		float num2 = Vector3.SqrMagnitude(Globals.m_PlayerController.transform.position - base.transform.position);
		if (Globals.m_PlayerController.GetNormalizedSpeed() > 0f && (Globals.m_PlayerController.m_Stance == PlayerController.Stance.Stand || Globals.m_PlayerController.GetNormalizedSpeed() > 0.5f) && num2 <= num)
		{
			BlowUp();
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = Globals.m_AIDirector.m_Squads[i].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if ((linkedListNode.Value.transform.position - base.transform.position).sqrMagnitude <= num)
				{
					BlowUp();
					return;
				}
			}
		}
		for (LinkedListNode<Sentry> linkedListNode2 = Globals.m_AIDirector.m_Sentries.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
		{
			if ((linkedListNode2.Value.transform.position - base.transform.position).sqrMagnitude <= num)
			{
				BlowUp();
				return;
			}
		}
		for (LinkedListNode<BoxRobot> linkedListNode3 = Globals.m_AIDirector.m_BoxRobots.First; linkedListNode3 != null; linkedListNode3 = linkedListNode3.Next)
		{
			if ((linkedListNode3.Value.transform.position - base.transform.position).sqrMagnitude <= num)
			{
				BlowUp();
				break;
			}
		}
	}

	public override void ThrowVelocity(Vector3 Velocity)
	{
		base.transform.rotation = Quaternion.LookRotation(Velocity, new Vector3(0f, 1f, 0f));
		m_Speed = 12f;
	}

	public void BlowUp()
	{
		m_GraceTimer = m_GraceTime;
		m_Triggered = true;
		m_BlinkTimer = 0f;
		m_BlinkState = false;
		switch (m_DamageType)
		{
		case DamageType.Explosive:
			SoundManager.TriggerEvent("Play_Mine_Tripped_Frag", base.gameObject);
			break;
		case DamageType.EMP:
			SoundManager.TriggerEvent("Play_Mine_Tripped_EMP", base.gameObject);
			break;
		case DamageType.Concussion:
			SoundManager.TriggerEvent("Play_Mine_Tripped_Concussion", base.gameObject);
			break;
		}
	}

	public void Explode()
	{
		if (m_Blown)
		{
			return;
		}
		m_Blown = true;
		if (!m_Triggered)
		{
			switch (m_DamageType)
			{
			case DamageType.Explosive:
				SoundManager.TriggerEvent("Play_Grenade_Frag", base.gameObject);
				break;
			case DamageType.EMP:
				SoundManager.TriggerEvent("Play_Grenade_EMP", base.gameObject);
				break;
			case DamageType.Concussion:
				SoundManager.TriggerEvent("Play_Grenade_Concussion", base.gameObject);
				break;
			}
		}
		GameObject obj = (GameObject)Object.Instantiate(m_ExplosionFX, base.transform.position, Quaternion.identity);
		Object.Destroy(obj, 3f);
		DamageData data = new DamageData(m_Owner, base.transform.position + Vector3.up * 0.1f, (int)m_Damage, m_DamageType, false);
		Globals.DealSplashDamage(data, m_MinRadius, m_MaxRadius);
		Globals.m_AIDirector.CheckAudioSenses(base.transform.position, 20f, DisturbanceEvent.MajorAudio, false);
		Globals.m_AIDirector.ScareNearbyNPCs(base.transform.position, 20f);
		Object.Destroy(base.gameObject);
	}

	public override XmlElement SaveGame(XmlElement root, XmlDocument doc)
	{
		XmlElement xmlElement = null;
		XmlNodeList elementsByTagName = root.GetElementsByTagName("Mines");
		foreach (XmlNode item in elementsByTagName)
		{
			if (item.Name == "Mines")
			{
				xmlElement = (XmlElement)item;
				break;
			}
		}
		if (xmlElement == null)
		{
			return null;
		}
		XmlElement xmlElement2 = (XmlElement)xmlElement.AppendChild(doc.CreateElement("Mine"));
		xmlElement2.SetAttribute("DamageType", m_DamageType.ToString());
		xmlElement2.SetAttribute("Position", base.transform.position.x + "," + base.transform.position.y + "," + base.transform.position.z);
		xmlElement2.SetAttribute("Rotation", base.transform.rotation.x + "," + base.transform.rotation.y + "," + base.transform.rotation.z + "," + base.transform.rotation.w);
		if (m_Owner != null)
		{
			xmlElement2.SetAttribute("Owner_Position", m_Owner.transform.position.x + "," + m_Owner.transform.position.y + "," + m_Owner.transform.position.z);
		}
		xmlElement2.SetAttribute("Speed", m_Speed.ToString());
		xmlElement2.SetAttribute("Gravity", m_Gravity.ToString());
		xmlElement2.SetAttribute("GraceTimer", m_GraceTimer.ToString());
		xmlElement2.SetAttribute("ArmTimer", m_ArmTimer.ToString());
		xmlElement2.SetAttribute("Triggered", m_Triggered.ToString());
		xmlElement2.SetAttribute("Blown", m_Blown.ToString());
		xmlElement2.SetAttribute("BlinkTimer", m_BlinkTimer.ToString());
		xmlElement2.SetAttribute("BlinkState", m_BlinkState.ToString());
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
			case "Owner_Position":
			{
				string[] array = attribute.InnerText.Split(',');
				Vector3 ownerFromPosition = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				SetOwnerFromPosition(ownerFromPosition);
				break;
			}
			case "Speed":
				m_Speed = float.Parse(attribute.InnerText);
				break;
			case "Gravity":
				m_Gravity = float.Parse(attribute.InnerText);
				break;
			case "GraceTimer":
				m_GraceTimer = float.Parse(attribute.InnerText);
				break;
			case "ArmTimer":
				m_ArmTimer = float.Parse(attribute.InnerText);
				break;
			case "Triggered":
				m_Triggered = bool.Parse(attribute.InnerText);
				break;
			case "Blown":
				m_Blown = bool.Parse(attribute.InnerText);
				break;
			case "BlinkTimer":
				m_BlinkTimer = float.Parse(attribute.InnerText);
				break;
			case "BlinkState":
				m_BlinkState = bool.Parse(attribute.InnerText);
				break;
			}
		}
	}
}
