using UnityEngine;

public class PlacedMineTrap : MonoBehaviour
{
	public bool m_MineArmed = true;

	public GameObject m_ExplosionPrefab;

	public int m_ExplosionDamage = 70;

	private Vector3 m_ExplosionPositionOffset = new Vector3(0f, 0.5f, 0f);

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" && m_MineArmed)
		{
			Object.Instantiate(m_ExplosionPrefab, base.transform.localPosition + m_ExplosionPositionOffset, Quaternion.identity);
			Globals.m_PlayerController.TakeDamage(new DamageData(null, base.transform.position, m_ExplosionDamage, DamageType.Explosive, false));
			SoundManager.TriggerEvent("Play_Grenade_Frag", base.gameObject);
			m_MineArmed = false;
			Object.Destroy(base.gameObject);
		}
	}
}
