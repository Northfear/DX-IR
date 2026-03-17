using UnityEngine;

public class InteractiveObject_NPC : InteractiveObject_Base
{
	public NPC_Base m_NPCBase;

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
		if (base.InteractWithObject(false) && m_NPCBase != null)
		{
			return m_NPCBase.Interact();
		}
		return false;
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
