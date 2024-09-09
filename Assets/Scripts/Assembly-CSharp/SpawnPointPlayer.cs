using UnityEngine;

public class SpawnPointPlayer : MonoBehaviour
{
	public GameObject m_PlayerPrefab;

	public bool m_DisablePlayerOnSpawn;

	[HideInInspector]
	public GameObject m_SpawnedPlayer;

	private void Awake()
	{
		string gameObjectToSpawnAt = GameManager.GetGameObjectToSpawnAt();
		if (gameObjectToSpawnAt != string.Empty)
		{
			GameObject gameObject = GameObject.Find(gameObjectToSpawnAt);
			if ((bool)gameObject)
			{
				m_SpawnedPlayer = Object.Instantiate(m_PlayerPrefab, gameObject.transform.position + new Vector3(0f, 0.1f, 0f), gameObject.transform.rotation) as GameObject;
			}
		}
		if (m_SpawnedPlayer == null)
		{
			m_SpawnedPlayer = Object.Instantiate(m_PlayerPrefab, base.transform.position + new Vector3(0f, 0.1f, 0f), base.transform.rotation) as GameObject;
		}
		GameManager.SetGameObjectToSpawnAt(string.Empty);
		if (!m_DisablePlayerOnSpawn)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		if (Globals.m_PlayerController.m_FrameNum == 2)
		{
			m_SpawnedPlayer.active = false;
			Object.Destroy(base.gameObject);
		}
	}
}
