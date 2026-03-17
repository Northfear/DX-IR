using System.Xml;
using UnityEngine;

public class GrenadeFrag : GrenadeBase
{
	public enum GrenadeState
	{
		Idle = 0,
		Thrown = 1,
		WaitingToDespawn = 2
	}

	public GrenadeState m_State;

	public float m_DetonationDelay = 4f;

	private float m_DetonationTimer;

	[HideInInspector]
	public float m_MinRadiusSqr;

	[HideInInspector]
	public float m_MaxRadiusSqr;

	public float m_MinAirTime = 0.4f;

	public float m_MaxAirTime = 1.7f;

	public float m_DistanceForMaxAirTime = 20f;

	[HideInInspector]
	public bool m_HUDTrackable;

	public DamageType GetDamageType()
	{
		switch (m_GrenadeType)
		{
		case GrenadeType.Frag:
			return DamageType.Explosive;
		case GrenadeType.EMP:
			return DamageType.EMP;
		case GrenadeType.Concussion:
			return DamageType.Concussion;
		default:
			return DamageType.Normal;
		}
	}

	public override void Awake()
	{
		m_MinRadiusSqr = m_MinRadius * m_MinRadius;
		m_MaxRadiusSqr = m_MaxRadius * m_MaxRadius;
		base.rigidbody.isKinematic = true;
		base.collider.enabled = false;
	}

	private void Start()
	{
		m_DetonationTimer = m_DetonationDelay;
		Globals.m_AIDirector.AddGrenade(this);
	}

	public override void Throw(Vector3 TargetLocation)
	{
		Vector3 vector = TargetLocation - base.transform.position;
		float y = vector.y;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		float num = Mathf.Lerp(m_MinAirTime, m_MaxAirTime, Mathf.Clamp(magnitude / m_DistanceForMaxAirTime, 0f, 1f));
		float num2 = (y + (0f - Physics.gravity.y) * (num * num) / 2f) / num;
		float num3 = magnitude / num;
		ThrowVelocity(vector.normalized * num3 + Vector3.up * num2);
	}

	public override void ThrowVelocity(Vector3 Velocity)
	{
		m_State = GrenadeState.Thrown;
		base.transform.parent = null;
		base.rigidbody.isKinematic = false;
		base.collider.enabled = true;
		base.rigidbody.velocity = Velocity;
	}

	private void Update()
	{
		if (m_State == GrenadeState.Thrown)
		{
			m_DetonationTimer -= Time.deltaTime;
			if (m_DetonationTimer <= 0f)
			{
				Globals.m_AIDirector.GrenadeDetonated(this);
				switch (m_GrenadeType)
				{
				case GrenadeType.Frag:
					SoundManager.TriggerEvent("Play_Grenade_Frag", base.gameObject);
					Object.Instantiate(Globals.m_This.m_FragExplosion, base.transform.position, Quaternion.identity);
					break;
				case GrenadeType.EMP:
					SoundManager.TriggerEvent("Play_Grenade_EMP", base.gameObject);
					Object.Instantiate(Globals.m_This.m_EMPExplosion, base.transform.position, Quaternion.identity);
					break;
				case GrenadeType.Concussion:
					SoundManager.TriggerEvent("Play_Grenade_Concussion", base.gameObject);
					Object.Instantiate(Globals.m_This.m_ConcussionExplosion, base.transform.position, Quaternion.identity);
					break;
				}
				base.renderer.enabled = false;
				base.rigidbody.isKinematic = true;
				base.collider.enabled = false;
				m_State = GrenadeState.WaitingToDespawn;
				m_DetonationTimer = 7f;
			}
		}
		else if (m_State == GrenadeState.WaitingToDespawn)
		{
			m_DetonationTimer -= Time.deltaTime;
			if (m_DetonationTimer <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (m_State == GrenadeState.Thrown && m_DetonationTimer < m_DetonationDelay - 0.2f)
		{
			m_HUDTrackable = true;
		}
		if (other.relativeVelocity.magnitude >= 2.5f)
		{
			SoundManager.TriggerEvent("Play_Grenade_Bounce", base.gameObject);
			Globals.m_AIDirector.CheckAudioSenses(base.transform.position, 10f, DisturbanceEvent.MinorAudio, false);
		}
	}

	private void OnCollisionStay(Collision other)
	{
		if (m_State == GrenadeState.Thrown && m_DetonationTimer < m_DetonationDelay - 0.2f)
		{
			m_HUDTrackable = true;
		}
	}

	public override XmlElement SaveGame(XmlElement root, XmlDocument doc)
	{
		if (m_State == GrenadeState.WaitingToDespawn)
		{
			return null;
		}
		XmlElement xmlElement = base.SaveGame(root, doc);
		xmlElement.SetAttribute("DetonationTimer", m_DetonationTimer.ToString());
		return xmlElement;
	}

	public override void LoadGame(XmlElement el)
	{
		base.LoadGame(el);
		foreach (XmlAttribute attribute in el.Attributes)
		{
			if (attribute.Name == "DetonationTimer")
			{
				m_DetonationTimer = float.Parse(attribute.InnerText);
			}
		}
	}
}
