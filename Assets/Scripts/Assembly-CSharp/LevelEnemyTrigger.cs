using UnityEngine;

public class LevelEnemyTrigger : MonoBehaviour
{
	public GameObject m_TargetDoor;

	public GameObject m_LockedDoor;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != 9)
		{
			return;
		}
		Debug.Log("Enemy is walking thru a doorway");
		if (m_TargetDoor != null)
		{
			InteractiveObject_Doors component = m_TargetDoor.GetComponent<InteractiveObject_Doors>();
			if (component != null)
			{
				component.InteractWithDoor(true, false);
				Debug.Log("Enemy is interacting with door.");
			}
			else
			{
				Debug.Log("Door class not found.");
			}
		}
		if (m_LockedDoor != null)
		{
			m_LockedDoor.animation.Play();
		}
		Object.Destroy(base.gameObject);
	}
}
