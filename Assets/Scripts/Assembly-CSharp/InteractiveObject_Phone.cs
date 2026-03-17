using UnityEngine;

public class InteractiveObject_Phone : InteractiveObject_Base
{
	public string m_PhoneInteractSoundEffect = "UI_Select";

	private void Update()
	{
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
		if (base.audio.isPlaying)
		{
			base.audio.Stop();
		}
		else
		{
			base.audio.Play();
		}
		if (m_PhoneInteractSoundEffect != null && m_PhoneInteractSoundEffect.Length > 0)
		{
			SoundManager.TriggerEvent(m_PhoneInteractSoundEffect, base.gameObject);
		}
		return true;
	}
}
