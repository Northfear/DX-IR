using System.Collections.Generic;
using UnityEngine;

public class WeaponStunGun : WeaponBase
{
	private float m_Cone = 0.6f;

	protected override void Awake()
	{
		m_WeaponType = WeaponType.StunGun;
		m_WeaponItemID = WeaponItemID.StunGun;
		m_FireSound3rdEventName = "Play_StunGun_3rd_Fire";
		m_FireSound3rdEventNameStop = "Stop_StunGun_3rd_Fire";
		m_FireSound1stEventName = "Play_StunGun_1st_Fire";
		m_FireSound1stEventNameStop = "Stop_StunGun_1st_Fire";
		m_ReloadSound3rdEventName = "Play_StunGun_3rd_Reload";
		m_ReloadSound1stEventName = "Play_StunGun_1st_Reload";
		m_HolsterSoundEventName = "Play_StunGun_1st_Holster";
		m_DrawSoundEventName = "Play_StunGun_1st_Draw";
		base.Awake();
	}

	protected override void FireBullet()
	{
		m_CurrentAmmo--;
		bool flag = false;
		for (int i = 0; i < 8; i++)
		{
			for (LinkedListNode<Enemy_Base> linkedListNode = Globals.m_AIDirector.m_Squads[i].m_EnemyUnits.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				float sqrMagnitude = (linkedListNode.Value.transform.position - m_User.transform.position).sqrMagnitude;
				if (sqrMagnitude < m_MaxRange * m_MaxRange)
				{
					Vector3 lhs = linkedListNode.Value.transform.position - m_User.transform.position;
					lhs.Normalize();
					float num = Vector3.Dot(lhs, m_User.transform.forward);
					if (num > m_Cone)
					{
						flag = true;
						linkedListNode.Value.TakeDamage(new DamageData(m_User, base.transform.position, 999, DamageType.StunGun, false));
						break;
					}
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (!flag)
		{
			bool PlayTracer = true;
			Ray ray = m_User.WeaponRequestForBulletRay(out PlayTracer);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, m_MaxRange, 65793))
			{
				PlayHitFX(hitInfo.point, hitInfo.normal, hitInfo.collider.gameObject.layer, false);
			}
		}
		PlayerFiredBullet();
	}

	public override string GetFirstPersonModelAnimName(FirstPersonAnimation anim)
	{
		switch (anim)
		{
		case FirstPersonAnimation.Idle:
			return "StungunIdle";
		case FirstPersonAnimation.Fire:
			return "StungunFire";
		case FirstPersonAnimation.Reload:
			return "StungunReload";
		case FirstPersonAnimation.Walk:
			return "StungunWalk";
		case FirstPersonAnimation.Draw:
			return "StungunDraw";
		case FirstPersonAnimation.Holster:
			return "StungunHolster";
		default:
			return base.GetFirstPersonModelAnimName(anim);
		}
	}

	public override float GetFirstPersonAnimSpeedMod(FirstPersonAnimation anim)
	{
		switch (anim)
		{
		case FirstPersonAnimation.Idle:
			return 1f;
		case FirstPersonAnimation.Fire:
			return 1f;
		case FirstPersonAnimation.Reload:
			return 1f;
		case FirstPersonAnimation.Walk:
			return 1f;
		case FirstPersonAnimation.Draw:
			return 1f;
		case FirstPersonAnimation.Holster:
			return 1f;
		case FirstPersonAnimation.None:
			return 1f;
		default:
			return 1f;
		}
	}

	public override string GetFirstPersonWeaponAnimName(FirstPersonAnimation anim)
	{
		switch (anim)
		{
		case FirstPersonAnimation.Idle:
			return "idle";
		case FirstPersonAnimation.Fire:
			return "fire";
		case FirstPersonAnimation.Reload:
			return "reload";
		case FirstPersonAnimation.Walk:
			return "walk";
		case FirstPersonAnimation.Draw:
			return "draw";
		case FirstPersonAnimation.Holster:
			return "holster";
		case FirstPersonAnimation.None:
			return "none";
		default:
			return null;
		}
	}
}
