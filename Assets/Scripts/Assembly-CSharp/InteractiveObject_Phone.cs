using Fabric;

public class InteractiveObject_Phone : InteractiveObject_Base
{
	public string m_PhoneInteractSoundEffect = "UI_Select";

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
			EventManager.Instance.PostEvent(m_PhoneInteractSoundEffect, EventAction.PlaySound, null, base.gameObject);
		}
		return true;
	}
}
