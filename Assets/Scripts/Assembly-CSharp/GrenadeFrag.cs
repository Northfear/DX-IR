using Fabric;
using UnityEngine;

public class GrenadeFrag : MonoBehaviour
{
	public enum GrenadeState
	{
		Idle = 0,
		Thrown = 1,
		WaitingToDespawn = 2
	}

	public GrenadeType m_GrenadeType;

	public GrenadeState m_State;

	public float m_DetonationDelay = 4f;

	private float m_DetonationTimer;

	public float m_Damage = 200f;

	public float m_MinRadius = 0.4f;

	public float m_MaxRadius = 2f;

	[HideInInspector]
	public float m_MinRadiusSqr;

	[HideInInspector]
	public float m_MaxRadiusSqr;

	public float m_MinAirTime = 0.4f;

	public float m_MaxAirTime = 1.7f;

	public float m_DistanceForMaxAirTime = 20f;

	[HideInInspector]
	public bool m_HUDTrackable;

	public int CalculateDamage(float DistanceFromGrenadeSqr)
	{
		switch (m_GrenadeType)
		{
		case GrenadeType.Frag:
			return (int)(m_Damage * Mathf.Clamp(1f - DistanceFromGrenadeSqr / m_MaxRadiusSqr, 0f, 1f));
		case GrenadeType.EMP:
			return 1000;
		case GrenadeType.Concussion:
			return 0;
		default:
			return 0;
		}
	}

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

	private void Awake()
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

	public void Throw(Vector3 TargetLocation)
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

	public void ThrowVelocity(Vector3 Velocity)
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
					EventManager.Instance.PostEvent("Grenade_Frag", EventAction.PlaySound, null, base.gameObject);
					Object.Instantiate(Globals.m_This.m_FragExplosion, base.transform.position, Quaternion.identity);
					break;
				case GrenadeType.EMP:
					EventManager.Instance.PostEvent("Grenade_EMP", EventAction.PlaySound, null, base.gameObject);
					Object.Instantiate(Globals.m_This.m_EMPExplosion, base.transform.position, Quaternion.identity);
					break;
				case GrenadeType.Concussion:
					EventManager.Instance.PostEvent("Grenade_Concussion", EventAction.PlaySound, null, base.gameObject);
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
			EventManager.Instance.PostEvent("Grenade_Bounce", EventAction.PlaySound, null, base.gameObject);
			Globals.m_AIDirector.CheckAudioSenses(base.transform.position, 10f, DisturbanceEvent.MinorAudio);
		}
	}

	private void OnCollisionStay(Collision other)
	{
		if (m_State == GrenadeState.Thrown && m_DetonationTimer < m_DetonationDelay - 0.2f)
		{
			m_HUDTrackable = true;
		}
	}
}
