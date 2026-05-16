using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Audio_Switch_Footsteps : MonoBehaviour
{
	public string switchGroup;

	public string switchValue;

	private void OnTriggerEnter(Collider other)
	{
#if SOUND_WWISE
		AkSoundEngine.SetSwitch(switchGroup, switchValue, Globals.m_PlayerController.gameObject);
#endif
	}
}
