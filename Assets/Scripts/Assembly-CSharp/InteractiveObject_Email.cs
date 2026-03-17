using UnityEngine;

public class InteractiveObject_Email : InteractiveObject_Base
{
	public GameObject m_EmailPrefab;

	public InteractiveObject_HackingTerminal m_LinkedHackingTerminalForEmail;

	protected override void Awake()
	{
		m_BlockCover = true;
		base.Awake();
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
		if (m_EmailPrefab == null)
		{
			return false;
		}
		DisableUI();
		Object.Instantiate(m_EmailPrefab);
		if ((bool)m_LinkedHackingTerminalForEmail)
		{
			m_LinkedHackingTerminalForEmail.LearnedPassword();
		}
		return true;
	}

	private void DisableUI()
	{
		Globals.m_HUD.Display(false, true, false);
		Globals.m_HUD.EnablePassThruInput(false);
		Globals.m_PlayerController.ToggleWeaponHolstered();
		Globals.m_PlayerController.CancelMovement();
	}
}
