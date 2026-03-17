using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AkSwitchTrigger : MonoBehaviour
{
	public string switchGroup;

	public string switchValue;

	public bool setOnOtherObject = true;

	public GameObject switchTargetObject;

	private void OnTriggerEnter(Collider other)
	{
		GameObject gameObject = switchTargetObject;
		if (gameObject == null)
		{
			gameObject = ((!setOnOtherObject) ? base.gameObject : other.gameObject);
		}
		AkSoundEngine.SetSwitch(switchGroup, switchValue, gameObject);
	}
}
