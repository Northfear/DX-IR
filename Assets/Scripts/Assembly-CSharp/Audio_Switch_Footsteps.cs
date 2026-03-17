using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Audio_Switch_Footsteps : MonoBehaviour
{
	public string switchGroup;

	public string switchValue;

	private void OnTriggerEnter(Collider other)
	{
		AkSoundEngine.SetSwitch(switchGroup, switchValue, Globals.m_PlayerController.gameObject);
	}
}
