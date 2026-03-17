using UnityEngine;

public class ProjectileMiniRPGRocket : ProjectileBase
{
	public float m_Speed;

	public int m_Damage;

	public float m_DamageRadius;

	public float m_Range;

	private float m_TravelDist;

	private void Start()
	{
		SoundManager.TriggerEvent("Play_Rocket_Loop", base.gameObject);
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
			SoundManager.TriggerEvent("Stop_Rocket_Loop", base.gameObject);
			Object.Destroy(base.gameObject);
			return;
		}
		Ray ray = new Ray(base.transform.position, base.transform.forward);
		int layerMask = 2179329;
		if (m_Firer.m_User == Globals.m_PlayerController)
		{
			layerMask = 2163457;
		}
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, Time.deltaTime * m_Speed, layerMask))
		{
			SoundManager.TriggerEvent("Play_Rocket_Explosion", base.gameObject);
			SoundManager.TriggerEvent("Stop_MiniRPG_Fire", m_Firer.gameObject);
			base.transform.position = hitInfo.point;
			DamageData data = new DamageData(m_Owner, hitInfo.point + hitInfo.normal * 0.1f, m_Damage, DamageType.Explosive, false);
			Globals.DealSplashDamage(data, 0.4f, m_DamageRadius);
			Quaternion rotation = Quaternion.LookRotation(Globals.m_PlayerController.m_Camera.transform.forward);
			GameObject obj = (GameObject)Object.Instantiate(m_Firer.m_HitFX, hitInfo.point, rotation);
			Object.Destroy(obj, 3f);
			Object.Destroy(base.gameObject);
		}
		else
		{
			base.transform.Translate(base.transform.forward * Time.deltaTime * m_Speed, Space.World);
		}
	}
}
