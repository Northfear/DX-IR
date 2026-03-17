using UnityEngine;

public class LevelCameraAnimation : MonoBehaviour
{
	public float m_AnimationRate;

	private void Start()
	{
		foreach (AnimationState item in base.animation)
		{
			item.speed = m_AnimationRate;
		}
		base.animation.Play();
	}
}
