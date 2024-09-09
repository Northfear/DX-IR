using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
	private class DamagerInfo
	{
		public bool active;

		public float rotation;

		public float timer;

		public PackedSprite damageDirectionIndicator;

		public PackedSprite damageDirectionArrow;
	}

	private const int m_MaxDamagers = 2;

	private float m_DamageTimer;

	private float m_DamageTime = 0.5f;

	public float m_WhiteBounceStart;

	public float m_WhiteBounceEnd;

	private PackedSprite m_FullscreenQuad;

	private List<DamagerInfo> m_Damagers = new List<DamagerInfo>();

	private int m_CurDamager;

	private void Start()
	{
		m_FullscreenQuad = GameManager.CreateFullscreenQuad(GameManager.FullscreenQuadType.Multiply, 0.5f);
		m_FullscreenQuad.gameObject.active = false;
		m_WhiteBounceStart = Globals.m_DamageDirectionArrow.offset.y;
		m_WhiteBounceEnd = m_WhiteBounceStart - 0.2f;
		for (int i = 0; i < 2; i++)
		{
			DamagerInfo damagerInfo = new DamagerInfo();
			damagerInfo.active = false;
			damagerInfo.damageDirectionIndicator = Object.Instantiate(Globals.m_DamageDirectionIndicator) as PackedSprite;
			damagerInfo.damageDirectionIndicator.transform.parent = Globals.m_HUD.transform.parent.transform;
			damagerInfo.damageDirectionIndicator.transform.localPosition = new Vector3(0f, 0f, 6.8f);
			damagerInfo.damageDirectionIndicator.transform.localRotation = Quaternion.Euler(0f, 0f, 0f - damagerInfo.rotation);
			damagerInfo.damageDirectionIndicator.gameObject.active = false;
			damagerInfo.damageDirectionArrow = Object.Instantiate(Globals.m_DamageDirectionArrow) as PackedSprite;
			damagerInfo.damageDirectionArrow.transform.parent = Globals.m_HUD.transform.parent.transform;
			damagerInfo.damageDirectionArrow.transform.localPosition = new Vector3(0f, 0f, 6.7f);
			damagerInfo.damageDirectionArrow.transform.localRotation = Quaternion.Euler(0f, 0f, 0f - damagerInfo.rotation);
			damagerInfo.damageDirectionArrow.SetOffset(new Vector2(0f, m_WhiteBounceEnd));
			damagerInfo.damageDirectionArrow.gameObject.active = false;
			m_Damagers.Add(damagerInfo);
		}
	}

	private void Update()
	{
		if (m_DamageTimer > 0f)
		{
			m_DamageTimer -= Time.deltaTime;
			if (m_DamageTimer <= 0f)
			{
				m_DamageTimer = 0f;
			}
		}
		Color red = Color.red;
		red.a = m_DamageTimer / m_DamageTime;
		float num = (float)Globals.m_PlayerController.GetCurrentHealth() / 40f;
		if (num > 1f)
		{
			num = 1f;
		}
		num = 1f - num;
		if (red.a < num)
		{
			red.a = num;
		}
		if (red.a > 0f)
		{
			m_FullscreenQuad.gameObject.active = true;
			m_FullscreenQuad.Color = red;
		}
		else
		{
			m_FullscreenQuad.gameObject.active = false;
		}
		for (int num2 = m_Damagers.Count - 1; num2 >= 0; num2--)
		{
			DamagerInfo damagerInfo = m_Damagers[num2];
			if (damagerInfo.active)
			{
				red = Color.white;
				red.a = damagerInfo.timer / m_DamageTime;
				damagerInfo.damageDirectionIndicator.Color = red;
				damagerInfo.damageDirectionArrow.Color = red;
				float num3 = 1f - damagerInfo.timer / m_DamageTime;
				num3 *= 4f;
				if (num3 <= 2f)
				{
					float num4 = 0f;
					if (num3 < 0f)
					{
						num3 = 0f;
					}
					num4 = ((!(num3 <= 1f)) ? Mathf.Lerp(m_WhiteBounceEnd, m_WhiteBounceStart, num3 - 1f) : Mathf.Lerp(m_WhiteBounceStart, m_WhiteBounceEnd, num3));
					damagerInfo.damageDirectionArrow.SetOffset(new Vector2(0f, num4));
				}
				damagerInfo.timer -= Time.deltaTime;
				if (damagerInfo.timer <= 0f)
				{
					damagerInfo.damageDirectionIndicator.gameObject.active = false;
					damagerInfo.damageDirectionArrow.gameObject.active = false;
					damagerInfo.active = false;
				}
			}
		}
	}

	public void TakeDamage(Vector3 damagerpos)
	{
		m_DamageTimer = m_DamageTime;
		DamagerInfo damagerInfo = m_Damagers[m_CurDamager];
		damagerInfo.active = true;
		Vector3 to = damagerpos - base.transform.position;
		to.Normalize();
		damagerInfo.rotation = Vector3.Angle(base.transform.forward, to);
		float num = to.x * base.transform.forward.z - to.z * base.transform.forward.x;
		if (num < 0f)
		{
			damagerInfo.rotation = 0f - damagerInfo.rotation;
		}
		damagerInfo.timer = m_DamageTime;
		damagerInfo.damageDirectionIndicator.transform.localPosition = new Vector3(0f, 0f, Globals.m_HUD.transform.localPosition.z + 0.1f);
		damagerInfo.damageDirectionIndicator.transform.localRotation = Quaternion.Euler(0f, 0f, 0f - damagerInfo.rotation);
		damagerInfo.damageDirectionIndicator.gameObject.active = true;
		damagerInfo.damageDirectionArrow.transform.localPosition = new Vector3(0f, 0f, Globals.m_HUD.transform.localPosition.z + 0.1f);
		damagerInfo.damageDirectionArrow.transform.localRotation = Quaternion.Euler(0f, 0f, 0f - damagerInfo.rotation);
		damagerInfo.damageDirectionArrow.SetOffset(new Vector2(0f, m_WhiteBounceEnd));
		damagerInfo.damageDirectionArrow.gameObject.active = true;
		m_CurDamager++;
		if (m_CurDamager >= 2)
		{
			m_CurDamager = 0;
		}
	}
}
