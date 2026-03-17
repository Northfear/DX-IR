using UnityEngine;

public class InteractiveObject_ElectricSwitch : InteractiveObject_Base
{
	public ElectricTrap m_ElectricTrapScript;

	public Material m_InactiveMaterial;

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
		if (!instant)
		{
			if (m_ElectricTrapScript.m_TrapActive)
			{
				SoundManager.TriggerEvent("Play_Switch_Electric_Off", base.gameObject);
			}
			else
			{
				SoundManager.TriggerEvent("Play_Switch_Electric_On", base.gameObject);
			}
		}
		m_ElectricTrapScript.ToggleActivation();
		return true;
	}

	private void Start()
	{
		m_Save = true;
		m_InteractOnLoad = true;
	}
}
