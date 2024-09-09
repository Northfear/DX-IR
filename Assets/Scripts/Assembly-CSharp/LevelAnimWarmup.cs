using UnityEngine;

public class LevelAnimWarmup : MonoBehaviour
{
	private void Start()
	{
		base.animation.Play();
		foreach (AnimationState item in base.animation)
		{
			item.speed = 0f;
		}
	}
}
