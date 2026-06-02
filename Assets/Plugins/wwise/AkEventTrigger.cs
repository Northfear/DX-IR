using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AkEventTrigger : MonoBehaviour
{
	public string enterEventName = string.Empty;

	public string exitEventName = string.Empty;

	public bool playOnOtherObject = true;

	public GameObject soundEmitterObject;

	private void OnTriggerEnter(Collider other)
	{
		if (!(enterEventName == string.Empty))
		{
			GameObject gameObject = soundEmitterObject;
			if (gameObject == null)
			{
				gameObject = ((!playOnOtherObject) ? base.gameObject : other.gameObject);
			}
			AkSoundEngine.PostEvent(enterEventName, gameObject);
		}
	}

	private void EventFinished(object in_cookie, AkCallbackType out_type, object out_info)
	{
		MonoBehaviour.print("Finished");
	}

	private void OnTriggerExit(Collider other)
	{
		if (!(exitEventName == string.Empty))
		{
			GameObject gameObject = soundEmitterObject;
			if (gameObject == null)
			{
				gameObject = ((!playOnOtherObject) ? base.gameObject : other.gameObject);
			}
			AkSoundEngine.PostEvent(exitEventName, gameObject);
		}
	}
}
