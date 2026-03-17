using UnityEngine;

public class Enemy_Ogre : Enemy_Base
{
	public override void Awake()
	{
		m_EnemyType = EnemyType.Ogre;
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		m_CurrentHealth = (m_MaxHealth = 800);
		m_DamageModifiers[0] = 0.8f;
		m_DamageModifiers[1] = 1f;
		m_DamageModifiers[2] = 0.8f;
		m_DamageModifiers[3] = 0.5f;
		m_DamageModifiers[4] = 0f;
		m_DamageModifiers[6] = 1f;
		m_DamageModifiers[5] = 1f;
	}

	protected override void LoadAnimations()
	{
		m_CBRAimDirections[0] = m_Animator["Aim_Stand_Center"];
		m_CBRAimDirections[3] = m_Animator["Aim_Stand_Left"];
		m_CBRAimDirections[4] = m_Animator["Aim_Stand_Right"];
		m_CBRAimDirections[1] = m_Animator["Aim_Stand_Up"];
		m_CBRAimDirections[2] = m_Animator["Aim_Stand_Down"];
		m_CBRAimDirections[5] = m_Animator["Aim_Stand_UpLeft"];
		m_CBRAimDirections[6] = m_Animator["Aim_Stand_UpRight"];
		m_CBRAimDirections[7] = m_Animator["Aim_Stand_DownLeft"];
		m_CBRAimDirections[8] = m_Animator["Aim_Stand_DownRight"];
		for (int i = 0; i < 9; i++)
		{
			m_CBRAimDirections[i].AddMixingTransform(m_Waist, true);
			m_CBRAimDirections[i].AddMixingTransform(m_WeaponAttachRight.transform, true);
			m_CBRAimDirections[i].blendMode = AnimationBlendMode.Blend;
			m_CBRAimDirections[i].weight = 1f;
			m_CBRAimDirections[i].layer = 10;
		}
		m_CBRFiring = m_Animator["Weapon_Fire"];
		m_CBRFiring.AddMixingTransform(m_Waist, true);
		m_CBRFiring.blendMode = AnimationBlendMode.Additive;
		m_CBRFiring.layer = 20;
		m_Animator["Damaged_DartIdle"].AddMixingTransform(m_Waist, true);
		m_Animator["Damaged_DartIdle"].layer = 40;
	}

	protected override void PlayDisturbance(bool Major, bool Visual, TurnDirection Direction)
	{
		string transitionalAnimation = m_TransitionalAnimation;
		switch (Direction)
		{
		case TurnDirection.BackLeft:
			m_TransitionalAnimation = "Disturbance_SouthWest";
			break;
		case TurnDirection.BackRight:
			m_TransitionalAnimation = "Disturbance_SouthEast";
			break;
		case TurnDirection.Left:
			m_TransitionalAnimation = "Disturbance_West";
			break;
		case TurnDirection.Right:
			m_TransitionalAnimation = "Disturbance_East";
			break;
		default:
			m_TransitionalAnimation = "Disturbance_North";
			break;
		}
		if (transitionalAnimation != null)
		{
			BeginCompoundTransition(transitionalAnimation, m_TransitionalAnimation);
		}
		m_PosedAnimation = m_TransitionalAnimation + "_Pose";
		m_Animator.CrossFade(m_TransitionalAnimation);
	}

	protected override void PlayDeathAnimation(DamageData data)
	{
		string transitionalAnimation = m_TransitionalAnimation;
		if (data.m_DamageType == DamageType.NonLethal)
		{
			bool flag = Random.value <= 0.5f;
			Ray ray = new Ray(m_RigMotion.transform.position + Vector3.up * 0.25f, (!flag) ? (-m_RigMotion.transform.forward) : m_RigMotion.transform.forward);
			if ((!Physics.Raycast(ray, 2f, 6374145)) ? flag : (!flag))
			{
				m_TransitionalAnimation = "Death_DartFallForward";
				PlayVO("Play_Enemy_Death_Tranq_FWD", 100);
			}
			else
			{
				m_TransitionalAnimation = "Death_DartFallBack";
				PlayVO("Play_Enemy_Death_Tranq_Back", 100);
			}
			if (transitionalAnimation != null)
			{
				BeginCompoundTransition(transitionalAnimation, m_TransitionalAnimation);
			}
			m_Animator.CrossFade(m_TransitionalAnimation);
		}
		else if (data.m_DamageType == DamageType.StunGun)
		{
			bool flag2 = Random.value <= 0.5f;
			Ray ray2 = new Ray(m_RigMotion.transform.position + Vector3.up * 0.25f, (!flag2) ? (-m_RigMotion.transform.forward) : m_RigMotion.transform.forward);
			if ((!Physics.Raycast(ray2, 2f, 6374145)) ? flag2 : (!flag2))
			{
				m_TransitionalAnimation = "Death_StunGun_FallForward";
				PlayVO("Play_Ogre_Death_StunGun_FallForward", 100);
			}
			else
			{
				m_TransitionalAnimation = "Death_StunGun_FallBack";
				PlayVO("Play_Ogre_Death_StunGun_FallBack", 100);
			}
			if (transitionalAnimation != null)
			{
				BeginCompoundTransition(transitionalAnimation, m_TransitionalAnimation);
			}
			m_Animator.CrossFade(m_TransitionalAnimation);
			GameObject gameObject = Object.Instantiate(Globals.m_This.m_StunGunEffect) as GameObject;
			gameObject.transform.parent = m_StunGunEffectAttachment;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
		}
		else if (data.m_DamageType == DamageType.Explosive)
		{
			Vector3 vector = data.m_SourceLocation - m_RigMotion.transform.position;
			vector.y = 0f;
			base.transform.position = m_RigMotion.transform.position;
			base.transform.rotation = Quaternion.LookRotation(vector.normalized);
			m_Animator.Stop();
			m_TransitionalAnimation = "Death_Explosion";
			m_Animator.Play(m_TransitionalAnimation);
			PlayVO("Play_Enemy_Death_Explosion", 100);
		}
		else
		{
			bool flag3 = Random.value <= 0.5f;
			Ray ray3 = new Ray(m_RigMotion.transform.position + Vector3.up * 0.25f, (!flag3) ? (-m_RigMotion.transform.forward) : m_RigMotion.transform.forward);
			if ((!Physics.Raycast(ray3, 2f, 6374145)) ? flag3 : (!flag3))
			{
				m_TransitionalAnimation = "Death_FrontHead";
				PlayVO("Play_Ogre_Death_FrontHead", 100);
			}
			else
			{
				m_TransitionalAnimation = "Death_FrontStomach";
				PlayVO("Play_Ogre_Death_FrontStomach", 100);
			}
			if (transitionalAnimation != null)
			{
				BeginCompoundTransition(transitionalAnimation, m_TransitionalAnimation);
			}
			m_Animator.CrossFade(m_TransitionalAnimation);
		}
	}

	public override void AssignWeapon(WeaponBase weapon)
	{
		base.AssignWeapon(weapon);
		if (m_Weapon != null)
		{
			m_Weapon.m_UsesAmmo = false;
		}
	}
}
