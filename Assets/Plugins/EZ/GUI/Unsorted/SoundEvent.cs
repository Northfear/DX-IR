using System;
using Fabric;

[Serializable]
public class SoundEvent
{
	public string SoundEventName = string.Empty;

	public EventAction SoundEventAction;

	public string SoundEventParameter;
}
