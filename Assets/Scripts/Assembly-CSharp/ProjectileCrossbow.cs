using System.Xml;
using UnityEngine;

public class ProjectileCrossbow : ProjectileBase
{
	public float m_Speed;

	public int m_Damage;

	public float m_DamageTimer;

	public float m_DeathTimer;

	public float m_Range;

	private float m_DeathCountdown;

	private CharacterBase m_NPCHit;

	private float m_TravelDist;

	[HideInInspector]
	public bool m_Lethal = true;

	private void Update()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}
		m_TravelDist += Time.deltaTime * m_Speed;
		if (m_TravelDist >= m_Range)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (m_DeathCountdown <= 0f)
		{
			Ray ray = new Ray(base.transform.position, base.transform.forward);
			int layerMask = 2179329;
			if (m_Firer.m_User == Globals.m_PlayerController)
			{
				layerMask = 2163457;
			}
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, Time.deltaTime * m_Speed, layerMask))
			{
				if (hitInfo.collider.gameObject.layer == 9 || hitInfo.collider.gameObject.layer == 14 || hitInfo.collider.gameObject.layer == 21)
				{
					m_NPCHit = Globals.FindCharacterBase(hitInfo.transform);
					if (m_NPCHit != null)
					{
						m_NPCHit.HitByTranquilizer();
					}
					m_DeathCountdown = m_DamageTimer;
					if (m_Lethal)
					{
						m_DeathCountdown = 0.1f;
					}
				}
				else
				{
					m_DeathCountdown = m_DeathTimer;
				}
				base.transform.position = hitInfo.point;
			}
			else
			{
				base.transform.Translate(base.transform.forward * Time.deltaTime * m_Speed, Space.World);
			}
			return;
		}
		m_DeathCountdown -= Time.deltaTime;
		if (!(m_DeathCountdown <= 0f))
		{
			return;
		}
		if (m_NPCHit != null)
		{
			DamageType type = DamageType.NonLethal;
			if (m_Lethal)
			{
				type = DamageType.Normal;
			}
			m_NPCHit.TakeDamage(new DamageData(m_Owner, base.transform.position, m_Damage, type, false));
		}
		Object.Destroy(base.gameObject);
	}

	public override XmlElement SaveGame(XmlElement root, XmlDocument doc)
	{
		if (m_DeathCountdown > 0f)
		{
			return null;
		}
		return base.SaveGame(root, doc);
	}
}
