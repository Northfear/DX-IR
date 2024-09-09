using Fabric;
using UnityEngine;

public class LevelChangeTrigger : MonoBehaviour
{
	public string m_NextLevelName;

	public string m_NameOfGameObjectToSpawnAt;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			GameManager.SetGameObjectToSpawnAt(m_NameOfGameObjectToSpawnAt);
			GameManager.LoadLevel(m_NextLevelName);
			FabricManager.Instance.Stop(0f);
		}
	}
}
