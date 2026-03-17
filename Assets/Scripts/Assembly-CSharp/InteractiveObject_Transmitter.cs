using UnityEngine;

public class InteractiveObject_Transmitter : InteractiveObject_Base
{
	public GameObject m_ActivationTarget;

	public string m_ActivationSoundEffect;

	public GameObject m_DestroyTarget;

	public Material m_InactiveMaterial;

	public GameObject m_EmailPrefab;

	public InteractiveObject_HackingTerminal m_LinkedHackingTerminalForEmail;

	public int m_CommLinkIndex = -1;

	public MonoBehaviour m_ScriptingTriggerClass;

	public string m_ScriptingTriggerName;

	private void Start()
	{
		m_Save = true;
		m_InteractOnLoad = true;
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
		m_ActivationTarget.SetActiveRecursively(true);
		if (!instant)
		{
			SoundManager.TriggerEvent(m_ActivationSoundEffect, base.gameObject);
		}
		if (m_CommLinkIndex != -1 && !instant)
		{
			CommLinkDialog.PlayDialog(m_CommLinkIndex);
		}
		DisableInteractiveObject(null, true);
		base.renderer.material = m_InactiveMaterial;
		Globals.m_SecondaryObjective = null;
		if (m_ScriptingTriggerClass != null && m_ScriptingTriggerName != string.Empty)
		{
			m_ScriptingTriggerClass.Invoke(m_ScriptingTriggerName, 0f);
		}
		if ((bool)m_LinkedHackingTerminalForEmail)
		{
			m_LinkedHackingTerminalForEmail.LearnedPassword();
		}
		if (m_DestroyTarget != null)
		{
			Object.Destroy(m_DestroyTarget);
		}
		m_Active = false;
		return true;
	}
}
