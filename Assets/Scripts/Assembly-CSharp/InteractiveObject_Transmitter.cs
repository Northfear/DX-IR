using Fabric;
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
		m_ActivationTarget.SetActiveRecursively(true);
		EventManager.Instance.PostEvent(m_ActivationSoundEffect, EventAction.PlaySound, null, base.gameObject);
		if (m_CommLinkIndex != -1)
		{
			CommLinkDialog.PlayDialog(m_CommLinkIndex);
		}
		DisableInteractiveObject();
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
