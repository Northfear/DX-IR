using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
	public enum CharacterType
	{
		Flesh = 0,
		Machine = 1
	}

	[HideInInspector]
	public CharacterType m_CharacterType;

	[HideInInspector]
	public int m_MaxHealth = 100;

	protected int m_CurrentHealth;

	private bool m_Dead;

	public Renderer m_MeshRenderer;

	private GameObject m_SelectIndicator;

	private PackedSprite m_SelectCorner_TopLeft;

	private PackedSprite m_SelectCorner_TopRight;

	private PackedSprite m_SelectCorner_BottomLeft;

	private PackedSprite m_SelectCorner_BottomRight;

	private PackedSprite m_OffscreenArrowLeft;

	private PackedSprite m_OffscreenArrowRight;

	public GameObject m_InteractiveTakedownObject;

	private InteractiveObject_Takedown m_TakedownScript;

	public GameObject m_TapCollisionObject;

	protected GameObject m_ShadowObject;

	public static int m_OffscreenTargetX;

	public static int m_OffscreenTargetY;

	public static int m_OffscreenTargetWidth;

	public static int m_OffscreenTargetHeight;

	public int GetCurrentHealth()
	{
		return m_CurrentHealth;
	}

	public bool AtMaxHealth()
	{
		return m_CurrentHealth >= m_MaxHealth;
	}

	protected virtual void Start()
	{
		m_CurrentHealth = m_MaxHealth;
		if (m_InteractiveTakedownObject != null)
		{
			m_TakedownScript = m_InteractiveTakedownObject.GetComponent<InteractiveObject_Takedown>();
		}
		m_ShadowObject = Object.Instantiate(Globals.m_This.m_ShadowProjectorObject) as GameObject;
		AttachShadowObject();
	}

	public void SetupHUD()
	{
		if (Globals.m_PlayerController != this)
		{
			m_SelectIndicator = Object.Instantiate(Globals.m_This.m_EnemyTargetIndicator) as GameObject;
			m_SelectIndicator.transform.parent = Globals.m_HUD.transform.parent.transform;
			m_SelectIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
			m_SelectIndicator.transform.localRotation = Quaternion.identity;
			EnemyTargetIndicator component = m_SelectIndicator.GetComponent<EnemyTargetIndicator>();
			m_SelectCorner_TopLeft = component.m_SelectCorner_TopLeft;
			m_SelectCorner_TopRight = component.m_SelectCorner_TopRight;
			m_SelectCorner_BottomLeft = component.m_SelectCorner_BottomLeft;
			m_SelectCorner_BottomRight = component.m_SelectCorner_BottomRight;
			m_OffscreenArrowLeft = component.m_OffscreenArrowLeft;
			m_OffscreenArrowRight = component.m_OffscreenArrowRight;
			m_OffscreenTargetWidth = (int)(m_OffscreenArrowRight.width * (1f / Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel));
			m_OffscreenTargetHeight = (int)(m_OffscreenArrowRight.height * (1f / Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel));
			m_SelectIndicator.SetActiveRecursively(false);
		}
	}

	private void OnDestroy()
	{
		Object.Destroy(m_SelectIndicator);
	}

	protected abstract void AttachShadowObject();

	protected virtual void UpdateShadowObjectPosition()
	{
	}

	protected virtual void UpdateShadowObjectRotation()
	{
		Ray ray = new Ray(m_ShadowObject.transform.position + new Vector3(0f, 0.5f, 0f), new Vector3(0f, -1f, 0f));
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 10f, 257))
		{
			m_ShadowObject.transform.rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.forward);
			m_ShadowObject.transform.Rotate(90f, 0f, 0f);
			Vector3 position = m_ShadowObject.transform.position;
			position.y = hitInfo.point.y + 0.08f;
			m_ShadowObject.transform.position = position;
		}
	}

	protected virtual void Update()
	{
		if (Globals.m_HUD != null && m_SelectIndicator == null)
		{
			SetupHUD();
		}
		if (m_ShadowObject != null)
		{
			UpdateShadowObjectPosition();
			UpdateShadowObjectRotation();
		}
		if (!(Globals.m_PlayerController != null) || !(Globals.m_PlayerController != this))
		{
			return;
		}
		if (!Globals.m_HUD.m_Showing || m_CurrentHealth <= 0)
		{
			m_SelectCorner_TopLeft.gameObject.active = false;
			m_SelectCorner_TopRight.gameObject.active = false;
			m_SelectCorner_BottomLeft.gameObject.active = false;
			m_SelectCorner_BottomRight.gameObject.active = false;
			m_OffscreenArrowLeft.gameObject.active = false;
			m_OffscreenArrowRight.gameObject.active = false;
			return;
		}
		bool flag = false;
		if ((Globals.m_PlayerController.m_TargetedEnemy == base.gameObject || flag) && m_MeshRenderer != null && m_MeshRenderer.isVisible)
		{
			m_SelectCorner_TopLeft.gameObject.active = true;
			m_SelectCorner_TopRight.gameObject.active = true;
			m_SelectCorner_BottomLeft.gameObject.active = true;
			m_SelectCorner_BottomRight.gameObject.active = true;
			Vector3 vector = base.transform.InverseTransformPoint(m_MeshRenderer.bounds.center);
			Vector3 vector2 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector + new Vector3(0f, m_MeshRenderer.bounds.extents.y, 0f)));
			Vector3 vector3 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector + new Vector3(0f, 0f - m_MeshRenderer.bounds.extents.y, 0f)));
			Vector3 vector4 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector + new Vector3(0f - m_MeshRenderer.bounds.extents.x, 0f, 0f)));
			Vector3 vector5 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector + new Vector3(m_MeshRenderer.bounds.extents.x, 0f, 0f)));
			Vector3 vector6 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector + new Vector3(0f, 0f, 0f - m_MeshRenderer.bounds.extents.z)));
			Vector3 vector7 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector + new Vector3(0f, 0f, m_MeshRenderer.bounds.extents.z)));
			Vector3 vector8 = vector4;
			Vector3 vector9 = vector5;
			if (vector6.x < vector8.x)
			{
				vector8.x = vector6.x;
			}
			if (vector5.x < vector8.x)
			{
				vector8.x = vector5.x;
			}
			if (vector7.x < vector8.x)
			{
				vector8.x = vector7.x;
			}
			if (vector6.x > vector9.x)
			{
				vector9.x = vector6.x;
			}
			if (vector4.x > vector9.x)
			{
				vector9.x = vector4.x;
			}
			if (vector7.x > vector9.x)
			{
				vector9.x = vector7.x;
			}
			vector8.x -= (float)Screen.width * 0.015f;
			vector9.x += (float)Screen.width * 0.015f;
			vector2.y += (float)Screen.height * 0.02f;
			vector3.y -= (float)Screen.height * 0.02f;
			float num = vector9.x - vector8.x;
			float num2 = (float)Screen.width * 0.072f;
			if (num < num2)
			{
				float num3 = num2 - num;
				vector8.x -= num3 / 2f;
				vector9.x += num3 / 2f;
			}
			float num4 = vector2.y - vector3.y;
			float num5 = (float)Screen.height * 0.099f;
			if (num4 < num5)
			{
				float num6 = num5 - num4;
				vector2.y += num6 / 2f;
				vector3.y -= num6 / 2f;
			}
			m_SelectCorner_TopLeft.transform.localPosition = new Vector3((vector8.x - (float)(Screen.width / 2)) * Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel, (vector2.y - (float)(Screen.height / 2)) * Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel, Globals.m_HUD.transform.localPosition.z + 0.1f);
			m_SelectCorner_TopRight.transform.localPosition = new Vector3((vector9.x - (float)(Screen.width / 2)) * Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel, (vector2.y - (float)(Screen.height / 2)) * Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel, Globals.m_HUD.transform.localPosition.z + 0.1f);
			m_SelectCorner_BottomLeft.transform.localPosition = new Vector3((vector8.x - (float)(Screen.width / 2)) * Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel, (vector3.y - (float)(Screen.height / 2)) * Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel, Globals.m_HUD.transform.localPosition.z + 0.1f);
			m_SelectCorner_BottomRight.transform.localPosition = new Vector3((vector9.x - (float)(Screen.width / 2)) * Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel, (vector3.y - (float)(Screen.height / 2)) * Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel, Globals.m_HUD.transform.localPosition.z + 0.1f);
		}
		else
		{
			m_SelectCorner_TopLeft.gameObject.active = false;
			m_SelectCorner_TopRight.gameObject.active = false;
			m_SelectCorner_BottomLeft.gameObject.active = false;
			m_SelectCorner_BottomRight.gameObject.active = false;
		}
		m_OffscreenArrowLeft.gameObject.active = false;
		m_OffscreenArrowRight.gameObject.active = false;
		if (!(Globals.m_PlayerController.m_TargetedEnemy == base.gameObject))
		{
			return;
		}
		Vector3 rhs = base.transform.position - Globals.m_PlayerController.m_CurrentCamera.transform.position;
		rhs.y = 0f;
		rhs.Normalize();
		Vector3 forward = Globals.m_PlayerController.m_CurrentCamera.transform.forward;
		forward.y = 0f;
		forward.Normalize();
		float num7 = Vector3.Dot(forward, rhs);
		if (num7 < 0.7f)
		{
			rhs = base.transform.position - Globals.m_PlayerController.m_CurrentCamera.transform.position;
			float magnitude = rhs.magnitude;
			rhs /= magnitude;
			Vector3 position = Globals.m_PlayerController.m_CurrentCamera.transform.position + Globals.m_PlayerController.m_CurrentCamera.transform.forward * magnitude;
			position.y = base.transform.position.y + 1.75f;
			Vector3 vector10 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(position);
			float num8 = Vector3.Dot(Globals.m_PlayerController.m_CurrentCamera.transform.right, rhs);
			m_OffscreenTargetY = (int)vector10.y;
			if (num8 > 0f)
			{
				m_OffscreenTargetX = Screen.width - 5 - (int)(m_OffscreenArrowRight.width * (1f / Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel));
				m_OffscreenArrowRight.transform.localPosition = new Vector3((float)(m_OffscreenTargetX - Screen.width / 2) * Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel, (float)(m_OffscreenTargetY - Screen.height / 2) * Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel, Globals.m_HUD.transform.localPosition.z + 0.1f);
				m_OffscreenArrowRight.gameObject.active = true;
			}
			else
			{
				m_OffscreenTargetX = 5;
				m_OffscreenArrowLeft.transform.localPosition = new Vector3((float)(m_OffscreenTargetX - Screen.width / 2) * Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel, (float)(m_OffscreenTargetY - Screen.height / 2) * Globals.m_HUD.m_RadarRoot.worldUnitsPerScreenPixel, Globals.m_HUD.transform.localPosition.z + 0.1f);
				m_OffscreenArrowLeft.gameObject.active = true;
			}
		}
	}

	public abstract Ray WeaponRequestForBulletRay(out bool PlayTracer);

	public abstract void WeaponWantsReload();

	public abstract void WeaponDoneReloading();

	public virtual void HitByTranquilizer()
	{
	}

	public virtual bool TakeDamage(int Damage, GameObject Damager, DamageType Type = DamageType.Normal)
	{
		m_CurrentHealth -= Damage;
		if (m_CurrentHealth < 0)
		{
			m_CurrentHealth = 0;
		}
		if (m_CurrentHealth <= 0 && !m_Dead)
		{
			m_Dead = true;
			Die(Damager, Type);
			return true;
		}
		return false;
	}

	public virtual Vector3 GetChestLocation()
	{
		return base.transform.position + Vector3.up * 1.4f;
	}

	public virtual void Die(GameObject Damager, DamageType Type = DamageType.Normal)
	{
		if (m_TapCollisionObject != null)
		{
			m_TapCollisionObject.SetActiveRecursively(false);
		}
		Object.Destroy(m_ShadowObject);
	}
}
