using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject_Takedown : InteractiveObject_Base
{
	public Texture m_PopupTexture2;

	public string m_PopupString2;

	public GameObject m_TakedownVFXJoint;

	public float m_DoubleTakedownRange = 3f;

	public override bool UseInteractiveCollider()
	{
		return false;
	}

	public override InteractivePopup.PopupType GetPopupType()
	{
		return InteractivePopup.PopupType.Takedown;
	}

	public override bool EnableInteractiveObject(GameObject livingEntity, bool calledByPlayer = true)
	{
		if (!base.EnableInteractiveObject(livingEntity, calledByPlayer))
		{
			return false;
		}
		return true;
	}

	public override bool DisableInteractiveObject(GameObject livingEntity, bool calledByPlayer = true)
	{
		if (!base.DisableInteractiveObject(livingEntity, calledByPlayer))
		{
			return false;
		}
		return true;
	}

	public override bool InteractWithObject(bool instant = false)
	{
		if (!base.InteractWithObject(instant))
		{
			return false;
		}
		m_Active = false;
		Globals.m_InteractiveObjectManager.DisableInteractivePopup(this);
		return true;
	}

	public void TakeDown(bool lethal)
	{
		GameObject enemy = null;
		Augmentation_ReflexBooster augmentation_ReflexBooster = (Augmentation_ReflexBooster)Globals.m_AugmentationData.GetAugmentationContainer(AugmentationData.Augmentations.ReflexBooster);
		if (augmentation_ReflexBooster.IsMultipleTakedownActive())
		{
			Enemy_Base enemy_Base = null;
			Vector2 vector = default(Vector2);
			for (int i = 0; i < 8; i++)
			{
				for (LinkedListNode<Enemy_Base> linkedListNode = Globals.m_AIDirector.GetFirstEnemy(i); linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					Enemy_Base component = base.gameObject.transform.parent.gameObject.GetComponent<Enemy_Base>();
					if (linkedListNode.Value != component)
					{
						vector.x = linkedListNode.Value.m_RigMotion.transform.position.x - component.m_RigMotion.transform.position.x;
						vector.y = linkedListNode.Value.m_RigMotion.transform.position.z - component.m_RigMotion.transform.position.z;
						if (vector.magnitude < m_DoubleTakedownRange)
						{
							enemy_Base = linkedListNode.Value;
							break;
						}
					}
				}
				if (enemy_Base != null)
				{
					enemy = enemy_Base.gameObject;
					break;
				}
			}
		}
		Globals.m_PlayerController.BeginTakedown(lethal, base.gameObject.transform.parent.gameObject, enemy);
	}

	public override Vector3 GetPopupLocation()
	{
		if (m_InteractiveCollider == null || !m_Active)
		{
			return m_OffScreen;
		}
		Enemy_Base component = base.gameObject.transform.parent.GetComponent<Enemy_Base>();
		if (component == null || !component.m_MeshRenderer.isVisible)
		{
			return m_OffScreen;
		}
		if (component.IsDead())
		{
			Globals.m_InteractiveObjectManager.DisableInteractivePopup(this);
			return m_OffScreen;
		}
		SphereCollider sphereCollider = base.gameObject.collider as SphereCollider;
		SphereCollider sphereCollider2 = Globals.m_PlayerController.m_PlayerInteractiveCollider as SphereCollider;
		float distance = Vector3.Distance(sphereCollider.transform.position, sphereCollider2.transform.position);
		bool flag = false;
		float num = 0.5f;
		Vector3 vector = sphereCollider.transform.position + -sphereCollider.transform.right * num;
		Vector3 direction = sphereCollider2.transform.position - vector;
		if (!Physics.Raycast(vector, direction, distance, 257))
		{
			flag = true;
		}
		vector = sphereCollider.transform.position + sphereCollider.transform.right * num;
		direction = sphereCollider2.transform.position - vector;
		if (!Physics.Raycast(vector, direction, distance, 257))
		{
			flag = true;
		}
		if (!flag)
		{
			return m_OffScreen;
		}
		Bounds bounds = m_InteractiveCollider.bounds;
		Vector3 vector2 = base.transform.InverseTransformPoint(bounds.center);
		Vector3 vector3 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector2 + new Vector3(0f, bounds.extents.y, 0f)));
		Vector3 vector4 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector2 + new Vector3(0f, 0f - bounds.extents.y, 0f)));
		Vector3 vector5 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector2 + new Vector3(0f - bounds.extents.x, 0f, 0f)));
		Vector3 vector6 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector2 + new Vector3(bounds.extents.x, 0f, 0f)));
		Vector3 vector7 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector2 + new Vector3(0f, 0f, 0f - bounds.extents.z)));
		Vector3 vector8 = Globals.m_PlayerController.m_CurrentCamera.WorldToScreenPoint(base.transform.TransformPoint(vector2 + new Vector3(0f, 0f, bounds.extents.z)));
		Vector3 vector9 = vector5;
		Vector3 vector10 = vector6;
		if (vector7.x < vector9.x)
		{
			vector9.x = vector7.x;
		}
		if (vector6.x < vector9.x)
		{
			vector9.x = vector6.x;
		}
		if (vector8.x < vector9.x)
		{
			vector9.x = vector8.x;
		}
		if (vector7.x > vector10.x)
		{
			vector10.x = vector7.x;
		}
		if (vector5.x > vector10.x)
		{
			vector10.x = vector5.x;
		}
		if (vector8.x > vector10.x)
		{
			vector10.x = vector8.x;
		}
		Vector3 zero = Vector3.zero;
		zero.y = (float)Screen.height - (vector4.y + (vector3.y - vector4.y) * 0.5f);
		zero.x = vector9.x + (vector10.x - vector9.x) * 0.5f;
		return zero;
	}

	private void Update()
	{
		if (!m_Active || !(Globals.m_PlayerController != null) || Globals.m_PlayerController.IsMoving())
		{
			return;
		}
		SphereCollider sphereCollider = base.gameObject.collider as SphereCollider;
		SphereCollider sphereCollider2 = Globals.m_PlayerController.m_PlayerInteractiveCollider as SphereCollider;
		float num = Vector3.Distance(sphereCollider.transform.position, sphereCollider2.transform.position);
		if (m_Enabled)
		{
			if (num > sphereCollider.radius)
			{
				DisableInteractiveObject(null, true);
			}
		}
		else if (num <= sphereCollider.radius)
		{
			EnableInteractiveObject(null, true);
		}
	}
}
