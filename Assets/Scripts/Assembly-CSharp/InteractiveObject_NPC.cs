using UnityEngine;

public class InteractiveObject_NPC : InteractiveObject_Base
{
	public NPC_Base m_NPCBase;

	public override bool EnableInteractiveObject()
	{
		if (!base.EnableInteractiveObject())
		{
			return false;
		}
		return true;
	}

	public override bool DisableInteractiveObject()
	{
		if (!base.DisableInteractiveObject())
		{
			return false;
		}
		return true;
	}

	public override bool InteractWithObject()
	{
		if (base.InteractWithObject() && m_NPCBase != null)
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
				DisableInteractiveObject();
			}
		}
		else if (num <= sphereCollider.radius)
		{
			EnableInteractiveObject();
		}
	}
}
