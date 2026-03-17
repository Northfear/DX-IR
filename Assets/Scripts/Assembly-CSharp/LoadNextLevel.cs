using UnityEngine;

public class LoadNextLevel : MonoBehaviour
{
	private void Start()
	{
		Resources.UnloadUnusedAssets();
		GameManager.m_FrameNum = 0;
		Application.LoadLevel(GameManager.m_LevelToLoad);
	}

	private void OnDestroy()
	{
		SoundManager.SetSoundSwitch("Music_Gameplay", "Ambient", Globals.m_This.gameObject);
		SoundManager.PlayLevelMusic();
	}

	private void Update()
	{
	}
}
