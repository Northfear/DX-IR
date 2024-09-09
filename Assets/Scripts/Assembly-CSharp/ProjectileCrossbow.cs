using UnityEngine;

public class ProjectileCrossbow : MonoBehaviour
{
	public float m_Speed = 1f;

	public int m_Damage = 200;

	public float m_DamageTimer = 4f;

	public float m_DeathTimer = 10f;

	public float m_Range = 200f;

	[HideInInspector]
	public WeaponBase m_Firer;

	private float m_DeathCountdown;

	private CharacterBase m_NPCHit;

	private float m_TravelDist;

	private void Start()
	{
	}

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
			int layerMask = 16641;
			if (m_Firer.m_User == Globals.m_PlayerController)
			{
				layerMask = 769;
			}
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, Time.deltaTime * m_Speed, layerMask))
			{
				if (hitInfo.collider.gameObject.layer == 9 || hitInfo.collider.gameObject.layer == 14)
				{
					m_NPCHit = hitInfo.collider.gameObject.GetComponent<CharacterBase>();
					if (m_NPCHit == null)
					{
						m_NPCHit = hitInfo.collider.transform.parent.parent.gameObject.GetComponent<CharacterBase>();
					}
					if (m_NPCHit != null)
					{
						m_NPCHit.HitByTranquilizer();
					}
					m_DeathCountdown = m_DamageTimer;
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
		if (m_DeathCountdown <= 0f)
		{
			if (m_NPCHit != null)
			{
				m_NPCHit.TakeDamage(m_Damage, m_Firer.gameObject, DamageType.NonLethal);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
