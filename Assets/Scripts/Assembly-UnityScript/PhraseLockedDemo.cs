using System;
using UnityEngine;

[Serializable]
public class PhraseLockedDemo : MonoBehaviour
{
	private int numUpdates;

	public AudioClip audioClip;

	public string animName;

	public AnimationState animState;

	public FaceFXControllerScript ffxController;

	public GameObject PlayerObject;

	public virtual void Start()
	{
		numUpdates = 0;
		Debug.Log("Starting PhraseLockedDemo");
	}

	public virtual void Update()
	{
		if (numUpdates == 0)
		{
			if (PlayerObject != null)
			{
				if (PlayerObject.animation != null)
				{
					animState = PlayerObject.animation[animName];
					if (animState != null)
					{
						animState.layer = 1;
						animState.wrapMode = WrapMode.ClampForever;
						animState.blendMode = AnimationBlendMode.Blend;
					}
					else
					{
						Debug.Log("animState is NULL!");
					}
				}
				else
				{
					Debug.Log("PlayerObject.animation is NULL!");
				}
			}
			else
			{
				Debug.Log("PlayerObject is NULL!");
			}
		}
		if (numUpdates == 10 && PlayerObject != null && PlayerObject.animation != null)
		{
			ffxController = (FaceFXControllerScript)PlayerObject.GetComponent(typeof(FaceFXControllerScript));
			if (ffxController != null)
			{
				ffxController.PlayAnim(animName, audioClip);
			}
			else
			{
				Debug.Log("ffxController is NULL!");
			}
		}
		numUpdates++;
	}

	public virtual void Main()
	{
	}
}
