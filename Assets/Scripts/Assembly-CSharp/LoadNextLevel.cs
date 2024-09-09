using UnityEngine;

public class LoadNextLevel : MonoBehaviour
{
	private void Start()
	{
		Resources.UnloadUnusedAssets();
		Application.LoadLevel(GameManager.m_LevelToLoad);
	}

	private void Update()
	{
	}
}
