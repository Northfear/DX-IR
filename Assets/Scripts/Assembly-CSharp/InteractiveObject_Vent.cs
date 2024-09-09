using Fabric;
using UnityEngine;

public class InteractiveObject_Vent : InteractiveObject_Base
{
	public Animation m_VentAnimator;

	public string m_VentOpenSoundEffect = "Vent_Open";

	private void Update()
	{
	}

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
		if (!base.InteractWithObject())
		{
			return false;
		}
		m_VentAnimator.Play();
		EventManager.Instance.PostEvent(m_VentOpenSoundEffect, EventAction.PlaySound, null, base.gameObject);
		DisableInteractiveObject();
		m_InteractiveCollider.enabled = false;
		m_Active = false;
		return true;
	}
}
